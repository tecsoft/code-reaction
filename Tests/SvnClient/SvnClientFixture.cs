using NUnit.Framework;
using SharpSvn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Tests.SvnClient2
{
    [TestFixture]
    public class SvnClientFixture
    {
        Uri rep = new Uri(@"svn://technix01/directory/trunk/Dev");

        [Test]
        public void GetLog()
        {
            using (SvnClient svnClient = new SvnClient())
            {
                try
                {
                    svnClient.Log(rep, new SvnLogArgs()
                        {
                            Range = new SvnRevisionRange(30018, 30019)
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
        public void Diff()
        {
            using (SvnClient svnClient = new SvnClient())
            {
                try
                {
                    SvnTarget target = SvnTarget.FromUri(rep);

                    MemoryStream ms = new MemoryStream();
                    
                    svnClient.Diff(target, new SvnRevisionRange(30018, 30019), ms);

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
            Console.WriteLine(args.Revision + " -> " + args.LogMessage);
        }
    }
}
