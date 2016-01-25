using CodeReaction.Domain;
using SharpSvn;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodeReaction.Domain.Commits
{
    public class SvnLogReader
    {
        Uri RemoteRepro { get; set; }
        string Username { get; set; }
        string Password { get; set; }

        public SvnLogReader()
        {
            var svnConfig = (ConfigurationManager.GetSection("codeReaction") as CodeReactionConfigurationSection).Svn;

            RemoteRepro = new Uri(svnConfig.Server);
            Username = svnConfig.Username;
            Password = svnConfig.Password;
        }

        public IEnumerable<Commit> GetLatestLogs(long lastRevision, int limit)
        {
            SvnLogHandler handler = new SvnLogHandler();

            using (SvnClient svnClient = new SvnClient())
            {
                svnClient.Authentication.ForceCredentials(Username, Password);

                svnClient.Log( 
                    RemoteRepro, 
                    new SvnLogArgs()
                    {
                        Range = new SvnRevisionRange(lastRevision, SvnRevision.Head),
                        Limit = limit
                    },
                    handler.Handler);
            }

            return handler.Logs;
        }

        class SvnLogHandler
        {
            IList<Commit> _commits;
            public SvnLogHandler()
            {
                _commits = new List<Commit>();
            }

            public IList<Commit> Logs { get { return _commits; } }

            public void Handler(object sender, SvnLogEventArgs args)
            {
                Commit commit = new Commit()
                {
                    Author = args.Author,
                    Message = args.LogMessage,
                    Timestamp = args.Time,
                    Revision = args.Revision,
                };

                Logs.Add(commit);
            
            }
        }

        class SvnChangeHandler
        {
            IDictionary<string, FileDiff> _fileInfo;
            public SvnChangeHandler()
            {
                _fileInfo = new Dictionary<string, FileDiff>();
            }

            public IDictionary<string, FileDiff> FileChanges { get { return _fileInfo; } }

            public void Handler(object sender, SvnLogEventArgs args)
            {
                foreach( var changedPath in args.ChangedPaths )
                {
                    FileDiff fileInfo = new FileDiff(
                        ConvertFrom(changedPath.Action),
                        ConvertFrom(changedPath.NodeKind),
                        changedPath.Path,
                        changedPath.CopyFromPath);

                    FileChanges.Add(fileInfo.Name, fileInfo);
                }
            }

            private FileState ConvertFrom( SvnChangeAction action )
            {
                switch(action)
                {
                    case SvnChangeAction.Add:
                        return FileState.Added;
                    case SvnChangeAction.Delete:
                        return FileState.Deleted;
                    case SvnChangeAction.Modify:
                        return FileState.Modified;
                    case SvnChangeAction.Replace:
                        return FileState.Moved;
                    default:
                        return FileState.None;
                }
            }

            private FileType ConvertFrom( SvnNodeKind nodeKind )
            {
                switch(nodeKind)
                {
                    case SvnNodeKind.File:
                        return FileType.Text;
                    case SvnNodeKind.Directory:
                        return FileType.Directory;
                    default:
                        return FileType.None;
                }
            }
        }

        public CommitDiff GetRevisionDiffs( long revision )
        {
            CommitDiff result = null;

            SvnChangeHandler handler = new SvnChangeHandler();

            using (SvnClient svnClient = new SvnClient())
            {
                svnClient.Authentication.ForceCredentials(Username, Password);

                svnClient.Log(
                    RemoteRepro,
                    new SvnLogArgs()
                    {
                        Range = new SvnRevisionRange(revision-1, revision)
                    },
                    handler.Handler);


                SvnTarget target = SvnTarget.FromUri(RemoteRepro);

                using (MemoryStream ms = new MemoryStream())
                {
                    svnClient.Diff(target, new SvnRevisionRange(revision - 1, revision), ms);
                    ms.Position = 0;
                    StreamReader reader = new StreamReader(ms);
                    result = GetRevisionDiffs(revision, handler.FileChanges, reader);
                }
            }

            return result;
        }

        public CommitDiff GetRevisionDiffs(long revision, IDictionary<string, FileDiff> fileInfo, StreamReader reader)
        {

            Uri repro = RemoteRepro;
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
    }
}
