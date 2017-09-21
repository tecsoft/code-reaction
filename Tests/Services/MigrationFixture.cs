using CodeReaction.Domain.Database;
using CodeReaction.Domain.Database.Migrations;
using CodeReaction.Domain.Repositories;
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
    public class MigrationFixture
    {
        [SetUp]
        public void TestSetup()
        {
            DbTestHelper.ResetDatabase();
        }

        [Test]
        public void MigrateAll()
        {
            using (var db = new DbCodeReview())
            {
                try
                {
                    db.SchemaVersion.Any();

                    Assert.Fail("Schema vesions exist !");
                }
                catch { }
            }

            SchemaMigration.Execute();

            using (var db = new DbCodeReview())
            {
                Assert.AreEqual(Index.Max(), db.SchemaVersion.First().Number);
            }
        }
    }
}
