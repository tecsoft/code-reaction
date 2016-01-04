using CodeReaction.Domain;
using CodeReaction.Domain.Commits;
using CodeReaction.Domain.HouseKeeping;
using CodeReaction.Tests.Services.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Tests.Services
{
    [TestFixture]
    public class HouseKeepingServiceFixture
    {
        [SetUp]
        public void TestSetup()
        {
            DbTestHelper.ResetDatabase();
        }
        [Test]
        public void ImportLatestLogs_No_New_Logs_Leaves_Database_Unchanged()
        {
            SourceControlDummy sourceControl = new SourceControlDummy(
                new List<Commit>()
                {
                    new Commit() { Revision = 0, Author = "mickey", Message = "Commit0", Timestamp = DateTime.Now  },
                    new Commit() { Revision = 1, Author = "daffy", Message = "Commit1", Timestamp = DateTime.Now }
                });

            using (var uow = new UnitOfWork())
            {
                HouseKeepingService sut = new HouseKeepingService(uow, sourceControl);

                sut.ImportLatestLogs();

                uow.Save();
            }

            using (var uow = new UnitOfWork())
            {
                Assert.AreEqual(2, uow.Context.Commits.Count());
            }

            using (var uow = new UnitOfWork())
            {
                HouseKeepingService sut = new HouseKeepingService(uow, sourceControl);

                sut.ImportLatestLogs();

                uow.Save();
            }

            using (var uow = new UnitOfWork())
            {
                Assert.AreEqual(2, uow.Context.Commits.Count());
            }
        }

        [Test]
        public void ImportLatestLogs_Inserts_Commits_Since_Last_Imported()
        {
            SourceControlDummy sourceControl = new SourceControlDummy(
                new List<Commit>()
                {
                    new Commit() { Revision = 0, Author = "mickey", Message = "Commit0", Timestamp = DateTime.Now  },
                    new Commit() { Revision = 1, Author = "daffy", Message = "Commit1", Timestamp = DateTime.Now }
                });

            using (var uow = new UnitOfWork())
            {
                HouseKeepingService sut = new HouseKeepingService(uow, sourceControl);

                sut.ImportLatestLogs();

                uow.Save();
            }

            
            using (var uow = new UnitOfWork())
            {
                Assert.AreEqual(2, uow.Context.Commits.Count() );
            }

            sourceControl.Commits.Add(new Commit() { Revision = 2, Author = "donald", Message = "Commit2", Timestamp = DateTime.Now });
            sourceControl.Commits.Add(new Commit() { Revision = 3, Author = "minnie", Message = "Commit3", Timestamp = DateTime.Now });
            sourceControl.Commits.Add(new Commit() { Revision = 4, Author = "pluto", Message = "Commit4", Timestamp = DateTime.Now });

            using (var uow = new UnitOfWork())
            {
                HouseKeepingService sut = new HouseKeepingService(uow, sourceControl);

                sut.ImportLatestLogs();

                uow.Save();
            }

            using (var uow = new UnitOfWork())
            {
                Assert.AreEqual(5, uow.Context.Commits.Count());
            }
        }
    }
}
