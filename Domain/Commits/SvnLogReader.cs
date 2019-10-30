using CodeReaction.Domain;
using CodeReaction.Domain.Projects;
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
        SvnElement _svnConfig;

        string RemoteReproAuthority { get; set; }

        string Username { get; set; }
        string Password { get; set; }

        public SvnLogReader()
        {
            _svnConfig = (ConfigurationManager.GetSection("codeReaction") as CodeReactionConfigurationSection).Svn;
            RemoteReproAuthority = new Uri(_svnConfig.Server).GetLeftPart(UriPartial.Authority);

            Username = _svnConfig.Username;
            Password = _svnConfig.Password;
        }

        Uri RemoteRepro(Project project)
        {
            var svnConfig = (ConfigurationManager.GetSection("codeReaction") as CodeReactionConfigurationSection).Svn;
            string reproName = svnConfig.Server + project.Path;
            return new Uri(reproName);
        }

        public IEnumerable<Commit> GetLatestLogs(long lastRevision, int limit)
        {
            IList<Commit> commits = new List<Commit>();

            var projects = new ProjectService(new UnitOfWork()).GetAll();

            using (SvnClient svnClient = new SvnClient())
            {
                svnClient.LoadConfiguration(Path.Combine(Path.GetTempPath(), "Svn"), true);
                svnClient.Authentication.ForceCredentials(Username, Password);

                foreach(var project in projects)
                {
                    var repository = RemoteRepro(project);
                    SvnLogHandler handler = new SvnLogHandler(project, commits);

                    svnClient.Log( 
                        repository, 
                        new SvnLogArgs()
                        {
                            Range = new SvnRevisionRange(lastRevision + 1, SvnRevision.Head),
                            Limit = limit
                        },
                        handler.Handler);
                }
            }

            return commits;
        }

        class SvnLogHandler
        {
            IList<Commit> _commits;
            Project _project;
            public SvnLogHandler(Project project, IList<Commit> commits)
            {
                _commits = commits;
                _project = project;
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
                    Project = _project.Id
                };
                _commits.Add(commit);

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
                foreach (var changedPath in args.ChangedPaths)
                {
                    FileDiff fileInfo = new FileDiff(
                        ConvertFrom(changedPath.Action),
                        ConvertFrom(changedPath.NodeKind),
                        changedPath.Path,
                        changedPath.CopyFromPath);

                    FileChanges.Add(fileInfo.Name, fileInfo);
                }
            }

            private FileState ConvertFrom(SvnChangeAction action)
            {
                switch (action)
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
                svnClient.LoadConfiguration(Path.Combine(Path.GetTempPath(), "Svn"), true);
                svnClient.Authentication.ForceCredentials(Username, Password);

                Uri repository = new Uri(_svnConfig.Server);
                svnClient.Log(
                    repository,
                    new SvnLogArgs()
                    {
                        Range = new SvnRevisionRange(revision, revision)
                    },
                    handler.Handler);


                SvnTarget target = SvnTarget.FromUri(repository);

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
            Uri repository = new Uri(_svnConfig.Server);
            string path = repository.AbsolutePath; // + '/';

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

        public IList<string> GetCurrentVersionOfFile(long revision, string filename)
        {
            IList<string> lines = new List<string>();
            using (SvnClient client = new SvnClient())
            {
                client.LoadConfiguration(Path.Combine(Path.GetTempPath(), "Svn"), true);
                client.Authentication.ForceCredentials(Username, Password);
                SvnTarget target = new SvnUriTarget(new Uri(_svnConfig.Server + filename), revision );

                using (MemoryStream ms = new MemoryStream())
                {
                    client.Write(target,
                        ms,
                        new SvnWriteArgs() { Revision = new SvnRevision(revision) });

                    ms.Position = 0;
                    using (StreamReader reader = new StreamReader(ms))
                    {
                        string line = reader.ReadLine();
                        while (line != null)
                        {
                            lines.Add(line);
                            line = reader.ReadLine();
                        }
                    }
                }
            }

            return lines;
        }

        private bool IsBinaryFile(string line)
        {
            return line.Contains('\0');
        }
       
    }
}
