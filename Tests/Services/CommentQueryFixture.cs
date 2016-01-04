using CodeReaction.Domain.Entities;
using CodeReaction.Domain.Feedback;
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
    public class CommentQueryFixture
    {
        [SetUp]
        public void TestSetup()
        {
            DbTestHelper.ResetDatabase();
        }

        [Test]
        public void By_Revision()
        {
            using (var database = new DbCodeReview())
            {
                database.Comments.AddRange(new Comment[]
                {
                new Comment() { Revision = 1111, LineId = "1212", User = "mickey", Text = "1111Comment1"  },
                new Comment() { Revision = 1111, LineId = "1212", User = "mickey", Text = "1111Comment2"  },
                new Comment() { Revision = 3333, LineId = "1212", User = "mickey", Text = "333Comment1"  },
                new Comment() { Revision = 11111, LineId = "1212", User = "mickey", Text = "1111Comment1"  },

                });
                database.SaveChanges();
            }

            using (var database = new DbCodeReview())
            {
                var sut = new CommentQuery(database.Comments)
                {
                    Revision = 1111
                };

                var result = sut.Execute();

                Assert.AreEqual(2, result.Count<Comment>());
                Assert.AreEqual(2, result.Where(c => c.Revision == 1111).Count<Comment>());
            }
        }
    }
}
