using CodeReaction.Domain;
using CodeReaction.Domain.Entities;
using CodeReaction.Domain.Services;
using CodeReaction.Tests.Services.Helpers;
using NUnit.Framework;
using System.Linq;

namespace CodeReaction.Tests.Likes
{
    [TestFixture]
    public class CommentServiceFixture
    {
        [SetUp]
        public void TestSetup()
        {
            DbTestHelper.ResetDatabase();
        }

        [Test]
        public void CommentLine_Valid_Comment_Saves_And_Assigns_Id()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var sut = new CommentService(uow);
                sut.CommentLine("daffy", 1212, 1, "line_id", "a nice comment");
                uow.Save();
            }

            using (UnitOfWork uow = new UnitOfWork())
            {
                var results = uow.Context.Comments.ToList();

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual("daffy", results[0].User);
                Assert.AreEqual(1212, results[0].Revision);
                Assert.AreEqual(1, results[0].FileId);
                Assert.AreEqual("line_id", results[0].LineId);
                Assert.AreEqual("a nice comment", results[0].Text);
                Assert.AreEqual(null, results[0].ReplyToId);
                Assert.AreNotEqual(0, results[0].Id);
            }
        }

        [Test]
        [ExpectedException( ExpectedException = typeof(System.Data.Entity.Validation.DbEntityValidationException))]
        public void CommentLine_Throws_When_Missing_Mandatory_User()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var sut = new CommentService(uow);
                sut.CommentLine(null, 1212, 1, "line_id", "a nice comment");
                uow.Save();
            }
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(System.Data.Entity.Validation.DbEntityValidationException))]
        public void CommentLine_Throws_When_Missing_Mandatory_Comment()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var sut = new CommentService(uow);
                sut.CommentLine("fred", 1212, 1, "line_id", null);
                uow.Save();
            }
        }

        [Test]
        public void Reply_Valid_Reply_Saves_And_Assigns_Id()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var sut = new CommentService(uow);
                sut.CommentLine("daffy", 1212, 1, "line_id", "a nice comment1");
                sut.CommentLine("daffy", 1212, 1, "line_id1", "a nice comment2");
                sut.CommentLine("daffy", 1212, 1, "line_id2", "a nice comment3");
                uow.Save();
            }

            Comment originalComment;

            using (UnitOfWork uow = new UnitOfWork())
            {
                var sut = new CommentService(uow);
                originalComment = uow.Context.Comments.ToList()[1];

                sut.Reply(originalComment.Id, "pluto", "thank you");
                uow.Save();
            }

            using (UnitOfWork uow = new UnitOfWork())
            {
                var results = uow.Context.Comments.ToList();

                var replies = results.Where(r => r.ReplyToId != null).ToList();
                Assert.AreEqual(1, replies.Count);

                Assert.AreEqual(originalComment.Id, replies[0].ReplyToId);
                Assert.AreEqual("pluto", replies[0].User);
                Assert.AreEqual("thank you", replies[0].Text);
                Assert.AreEqual(originalComment.FileId, replies[0].FileId);
                Assert.AreEqual(originalComment.LineId, replies[0].LineId);
                Assert.AreEqual(originalComment.Revision, replies[0].Revision);
                Assert.AreNotEqual(originalComment.Id, replies[0].Id);
            }
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(System.Data.Entity.Validation.DbEntityValidationException))]
        public void Reply_Throws_When_Missing_Mandatory_User()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var sut = new CommentService(uow);
                sut.CommentLine("daffy", 1212, 1, "line_id", "a nice comment1");
                uow.Save();
            }

            using (UnitOfWork uow = new UnitOfWork())
            {
                var originalComment = uow.Context.Comments.First();
                var sut = new CommentService(uow);
                sut.Reply(originalComment.Id, null, "thanks");
                uow.Save();
            }
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(System.Data.Entity.Validation.DbEntityValidationException))]
        public void Reply_Throws_When_Missing_Mandatory_Comment()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var sut = new CommentService(uow);
                sut.CommentLine("daffy", 1212, 1, "line_id", "a nice comment1");
                uow.Save();
            }

            using (UnitOfWork uow = new UnitOfWork())
            {
                var originalComment = uow.Context.Comments.First();
                var sut = new CommentService(uow);
                sut.Reply(originalComment.Id, "minnie", null);
                uow.Save();
            }
        }
    }
}
