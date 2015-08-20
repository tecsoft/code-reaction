using CodeReaction.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodeReaction.Domain.Commits
{
    public class CommitReader
    {
        public string LocalRepro { get; protected set; }

        public string RemoteRepro { get; protected set; }

        public CommitReader(string localRepro)
        {
            LocalRepro = localRepro;
            RemoteRepro = "svn://technix01/directory/trunk/Dev";
        }

        public IEnumerable<Commit> GetLatestLogs(long lastRevision, int limit)
        {
            var reg = RunSvnCommand( string.Format( "log -r {0}:HEAD --limit {1} --xml {2}", lastRevision + 1, limit, RemoteRepro) );

            XmlDocument doc = new XmlDocument();
            doc.Load(reg.StandardOutput);

            return ParseXmlLogs(doc);
        }

        public IEnumerable<Commit> ParseLogs(StreamReader reader)
        {
            IList<Commit> commits = new List<Commit>();

            string line = reader.ReadLine();

            Commit currentCommit = null;
            while (line != null)
            {
                currentCommit = new Commit();

                line = reader.ReadLine(); // to read info bar

                if (line == null) break;


                string[] data = line.Split('|');
                currentCommit.Revision = int.Parse(data[0].Substring(1).Trim());
                currentCommit.Author = data[1].Trim();
                currentCommit.Timestamp = DateTime.Parse(data[2].Trim().Substring(0, 25));
                currentCommit.Message = String.Empty;

                line = reader.ReadLine(); // read past the following blank line
                line = reader.ReadLine();
                while (line != null && line.StartsWith("----------------------") == false)
                {
                    currentCommit.Message += (line + System.Environment.NewLine);
                    line = reader.ReadLine();
                }

                commits.Add(currentCommit);                   
            }

            return commits;
        }

        public IEnumerable<Commit> ParseXmlLogs(XmlDocument doc)
        {
            var nodes = doc.SelectNodes("//log//logentry");

            IList<Commit> logs = new List<Commit>();
            foreach (XmlNode node in nodes)
            {
                Commit log = new Commit();
                log.Revision = int.Parse(node.Attributes.GetNamedItem("revision").InnerText );

                log.Author = node.SelectSingleNode("author").InnerText;
                log.Message = node.SelectSingleNode("msg").InnerText;
                log.Timestamp = DateTime.Parse(node.SelectSingleNode("date").InnerText); // UTC or local time ?

                logs.Add(log);
            }

            return logs;
        }

        //public IList<LineDiff> GetUnifiedDiff(FileDiff diff)
        //{
        //    IList<LineDiff> lines = new List<LineDiff>();

        //    var reg = RunSvnCommand(string.Format("cat -r {0} {1}", diff.Revision, diff.Index ));

        //    int fileIndex = 0;
        //    int diffStartIndex = int.Parse( diff.Current.Split(',')[0] );
        //    int diffLineIndex = 0;

        //    using (var reader = reg.StandardOutput)
        //    {
        //        string line = reader.ReadLine();
        //        while (line != null)
        //        {
        //            if (fileIndex < diffStartIndex)
        //            {
        //                diff.AddLine(ChangeState.None, line, 0, 0);

        //                line = reader.ReadLine();
        //                fileIndex++;
        //            }
        //            else if (diffLineIndex < diff.Lines.Count)
        //            {
        //                // possible change
        //                var diffLine = diff.Lines[diffLineIndex];

        //                lines.Add(diffLine);

        //                diffLineIndex++;
        //                fileIndex++;
        //                line = reader.ReadLine();
       
        //            }
        //            else
        //            {
        //                diff.AddLine( ChangeState.None, line, 0, 0);
        //                line = reader.ReadLine();
        //            }
        //        }
        //    }

        //    return lines;
        //}

        public CommitDiff GetRevisionDiffs( long revision )
        {
            try
            {
                var reg = RunSvnCommand(string.Format("log -r {0} --verbose --xml {1}", revision, RemoteRepro));

                var fileInfo = GetCommitedFileInfo(revision, reg.StandardOutput);

                // use this information to avoid diffing or handling files not needing a diff

                reg = RunSvnCommand(string.Format(" diff -r {0}:{1} {2}", revision - 1, revision, RemoteRepro));

                return GetRevisionDiffs(revision, fileInfo, reg.StandardOutput);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /* RENAMED FILE - could be in a differnet order
         *<path
             kind=""
               copyfrom-path="/directory/trunk/Dev/WebViews/Player/Quizzes/ProposalFeedbackdTOServiceImpl.cs"
               copyfrom-rev="35581"
               action="A">/directory/trunk/Dev/WebViews/Player/Quizzes/ProposalFeedbackDtoServiceImpl.cs</path>
            <path
               kind=""
               action="D">/directory/trunk/Dev/WebViews/Player/Quizzes/ProposalFeedbackdTOServiceImpl.cs</path>
            </paths> */

        /*  Kind does not seem to be used - cannot tel if file is a directory or not */

        /// <summary>
        // Action D: deleted file M: modified file, A : Added
        /// </summary>
        /// <param name="revision"></param>
        /// <param name="reader"></param>
        public IDictionary<string, FileDiff> GetCommitedFileInfo(long revision, StreamReader reader)
        {
            //IList<CommitedFileInfo> infos = new List<CommitedFileInfo>();
            IDictionary<string, FileDiff> fileInfo = new Dictionary<string, FileDiff>();

            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            //var directoryName = Path.GetDirectoryName(LocalRepro);

            // filename starts with /directory
            // need repos url ex svn://technixO1
            // path: ex /directory/trunk/Dev

            


            var nodes = doc.SelectNodes("//paths//path");
            foreach ( XmlNode node in nodes)
            {
                FileState fileState = ParseAction( node.Attributes.GetNamedItem("action").InnerText );
                string fileName = node.FirstChild.InnerText;


                XmlNode copyFromAttr = node.Attributes.GetNamedItem("copyfrom-path");

                // dont bother check if its a directory

                if ( copyFromAttr == null )
                {
                    fileInfo[fileName] = new FileDiff( fileState, FileType.None, fileName, null  );
                }
                else
                {
                    string copyFrom = copyFromAttr.InnerText;
                    // action added or modified possible ?
                    // create a moved item but first check so see of the old file has already been logged
                    // if that's the case remove it first
                    if ( fileInfo.ContainsKey( copyFrom ) )
                    {
                        fileInfo.Remove(copyFrom);
                    }
                    fileInfo[fileName] = new FileDiff(FileState.Moved, FileType.None, fileName, copyFrom );
                } 
            }

            return fileInfo;
        }

        //FileType GetFileType (string fileName, string kind, FileState fileState )
        //{
        //    // can we get it from kind ? 
        //}

        FileState ParseAction(string action )
        {
            switch (action)
            {
                case "A":
                    return FileState.Added;
                case "D":
                    return FileState.Deleted;
                case "M":
                    return FileState.Modified;
                default:
                    return FileState.None;
            }
        }
        
        public CommitDiff GetRevisionDiffs(long revision, IDictionary<string, FileDiff> fileInfo, StreamReader reader)
        {

            Uri repro = new Uri(RemoteRepro);
            string path = repro.AbsolutePath + '/';

            var results = new CommitDiff(revision);
            FileDiff diff = null;

                bool ignoreFileContent = false;

                using (reader)
                {
                    string line = reader.ReadLine();

                    int startRemovedLineNumber = 1;
                    int startAddedLineNumber = 1;

                    while ( line != null )
                    {
                        if (line.StartsWith("Index: "))
                        {
                            if (diff != null)
                            {
                                // save current file
                                results.AddFileDiff(diff);
                            }

                            string fileName = path + line.Substring("Index: ".Length);

                            // ex: revision 33513 (ref14240)
                            // deleted directory : file name not found in log but found in diff (can ignore)

                            if (fileInfo.ContainsKey(fileName) == false)
                            {
                                ignoreFileContent = true;
                                // Add an entry thought as deleted or moved out of trunk
                                // TODO maybe see if file is realy deelted or just ignore with filestate unknown
                                // assume deleted for the moment
                                results.AddFileDiff(new FileDiff(FileState.Deleted, FileType.None, fileName, null));
                                diff = null;

                            }
                            else
                            {
                                diff = fileInfo[fileName];
                                ignoreFileContent = diff.FileState == FileState.Deleted;
                            }

                            line = reader.ReadLine();
                        }
                        else if (line.StartsWith("+++ ") || line.StartsWith("--- ") || line.StartsWith("=============="))
                        {
                            line = reader.ReadLine();
                        }
                        else
                        {
                            if (ignoreFileContent == false)
                            {
                                if (line.StartsWith("@@"))
                                {
                                    string[] filePos = line.Split(new char[] { '@', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                    startRemovedLineNumber = int.Parse(filePos[0].Split(',')[0].Substring(1));
                                    startAddedLineNumber = int.Parse(filePos[1].Split(',')[0].Substring(1));

                                    diff.AddLine(ChangeState.BreakPoint, line, startRemovedLineNumber, startAddedLineNumber);

                                }
                                else
                                {
                                    // we could have a binary file and wish to ignore the content of such things
                                    // so precheck the line and ignore content if that is the conclusion

                                    if (IsBinaryFile(line))
                                    {
                                        ignoreFileContent = true;
                                        diff.Lines.Clear();
                                        diff.FileType = FileType.Binary;
                                    }
                                    else
                                    {
                                        if (line.StartsWith("+"))
                                        {
                                            diff.AddLine(ChangeState.Added, line.Substring(1), startRemovedLineNumber, startAddedLineNumber++);
                                        }
                                        else if (line.StartsWith("-"))
                                        {
                                            diff.AddLine(ChangeState.Removed, line.Substring(1), startRemovedLineNumber++, startAddedLineNumber);
                                        }
                                        else
                                        {
                                            diff.AddLine(ChangeState.None, line.Substring(Math.Min(1, line.Length)), startRemovedLineNumber++, startAddedLineNumber++);
                                        }
                                    }
                                }
                            }

                        line = reader.ReadLine();
                    }
                }

                if (diff != null)
                {
                    // save current file
                    results.AddFileDiff(diff);
                }
            }

            return results;
        }

        private bool IsBinaryFile(string line)
        {
            return line.Contains('\0');
        }

        //FileDiff ParseFile(string file, StreamReader reader)
        //{
        //    FileDiff diff = new FileDiff();
        //    diff.Name = file;

        //    string line = reader.ReadLine();
        //    while (line != null)
        //    {
        //        if (line.StartsWith("Index: "))
        //        {
        //            // next file
        //            line = null;
        //        }
        //        else if ( line.StartsWith( "+++ " ) || line.StartsWith( "--- " ) || line.StartsWith( "==============" ) )
        //        {
        //            line = reader.ReadLine();
        //        }
        //        else
        //        {
        //            if (line.StartsWith("@@"))
        //            {
        //                string[] filePos = line.Split(new char[] { '@', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        //                diff.Previous = filePos[0];
        //                diff.Current = filePos[1];
        //            } 
        //            else if (line.StartsWith("+"))
        //            {
        //                diff.AddLine(ChangeState.Added, line.Substring(1));
        //            }
        //            else if ( line.StartsWith("-") )
        //            {
        //                diff.AddLine(ChangeState.Removed, line.Substring(1) );
        //            }
        //            else
        //            {
        //                diff.AddLine(ChangeState.None, line );
        //            }

        //            line = reader.ReadLine();
        //        }
        //    }

        //    return diff;
        //}

        private Process RunSvnCommand(string command)
        {
            var proc = new ProcessStartInfo(@"c:\Program Files\CollabNet\Subversion Client\svn.exe", command );
            proc.StandardOutputEncoding = Encoding.UTF8;
            proc.StandardErrorEncoding = Encoding.UTF8;
            proc.RedirectStandardError = true;
            proc.RedirectStandardOutput = true;
            proc.CreateNoWindow = true;
            proc.UseShellExecute = false;

            proc.WorkingDirectory = LocalRepro;

            return System.Diagnostics.Process.Start(proc);
        }
    }
}
