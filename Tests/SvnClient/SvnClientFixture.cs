using NUnit.Framework;
using SharpSvn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Tests.Svn
{
    [TestFixture]
    public class SvnClientFixture
    {
        Uri rep = new Uri(@"svn://technix01/directory/trunk/Dev");

        [Test]
        public void  GetFile()
        {
            using (SvnClient client = new SvnClient())
            {
                Uri fileUri = new Uri(@"svn://technix01/directory/trunk/Dev/WebSite/Studio/Courses/Items/Modules/Edit.ascx.cs");
                SvnTarget target = SvnTarget.FromUri(fileUri);

                client.FileVersions(target,
                    new SvnFileVersionsArgs()
                    {
                        Start = new SvnRevision(30078),
                        //Start = new SvnRevision(30078),
                        //End = new SvnRevision(30078),
                        RetrieveContents = true,
                         
                    },  
                    versionHandler );
            }
        }

        [Test]
        public void Write()
        {
            using (SvnClient client = new SvnClient())
            {
                Uri fileUri = new Uri(@"svn://technix01/directory/trunk/Dev/WebSite/Studio/Courses/Items/Modules/Edit.ascx.cs");
                SvnTarget target = new SvnUriTarget(fileUri, 30078);

                using (MemoryStream ms = new MemoryStream())
                {
                    client.Write(target,
                        ms, new SvnWriteArgs() { Revision = new SvnRevision(30078) });

                    ms.Position = 0;
                    using (StreamReader reader = new StreamReader(ms))
                    {
                        Console.WriteLine(reader.ReadToEnd());
                    }
                }
            }
        }

        private void versionHandler( object sender, SvnFileVersionEventArgs args )
        {
            Console.WriteLine(args.VersionFile + " " + args.Revision);

            using (var reader = new StreamReader(args.GetContentStream()))
            {
                Console.WriteLine(reader.ReadToEnd());
            }
        }

        [Test]
        public void Fusion()
        {
            Uri fileUri = new Uri(@"svn://technix01/directory/trunk/Dev");
            string rest = @"/directory/trunk/Dev/Catalog.Impl/TrainingSessions/Impl/TrainingSessionValidator.cs";

            Console.WriteLine(fileUri.GetLeftPart(UriPartial.Authority) + rest);
        }

        [Test]
        public void GetLog()
        {
            using (SvnClient svnClient = new SvnClient())
            {
                try
                {
                    svnClient.Log(rep, new SvnLogArgs()
                        {
                            Range = new SvnRevisionRange(30078, 30078)
                        },
                        OnLogResult

                        );

                }
                catch(Exception ex )
                {
                    Console.WriteLine(ex);
                }
            }

        }

        [Test]
        public void FildDiffs()
        {

            using (SvnClient svnClient = new SvnClient())
            {
                try
                {
                    svnClient.Log(rep, new SvnLogArgs()
                    {
                        Range = new SvnRevisionRange(30078, 30078)
                    },
                        OnLogResult2

                        );

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void OnLogResult2( object sender, SvnLogEventArgs args )
        {
            foreach( var item in args.ChangedPaths )
            {
                Console.WriteLine(item.Action + " " + item.NodeKind + " " + item.RepositoryPath + " " + item.CopyFromPath);
            }
        }

        [Test]
        public void GetRevisionDiffs()
        {
            using (SvnClient svnClient = new SvnClient())
            {
                try
                {
                    SvnTarget target = SvnTarget.FromUri(rep);

                    MemoryStream ms = new MemoryStream();
                    
                    svnClient.Diff(target, new SvnRevisionRange(30077, 30078), ms);

                    ms.Position = 0;
                    StreamReader sr= new StreamReader(ms);

                    Console.WriteLine(sr.ReadToEnd());

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void OnLogResult( object sender, SvnLogEventArgs args )
        {
            Console.WriteLine(args.Revision + " -> " + args.LogMessage + " by " + args.Author);

            foreach (var item in args.ChangedPaths)
                Console.WriteLine(item.Path);

        }
    }
}
