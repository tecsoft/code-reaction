using CodeReaction.Domain;
using CodeReaction.Domain.Entities;
using CodeReaction.Domain.Repositories;
using CodeReaction.Domain.Services;
using CodeReaction.Tests.Services.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void GetComments_Given_Revision_Get_All_Comments()
        {
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork() )
                {
                    unitOfWork.Context.Comments.Add(new Comment() { Revision = 1234, User = "tcarter", FileId = 0, LineId = "1:1", Text = "comment" });
                    unitOfWork.Context.Comments.Add(new Comment() { Revision = 9122, User = "cdog", FileId = 1, LineId = "2:2", Text = "comment" });
                    unitOfWork.Context.Comments.Add(new Comment() { Revision = 9122, User = "frefr", FileId = 2, LineId = "-1:-1", Text = "comment" });

                    unitOfWork.Context.SaveChanges();
                }

                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    var commentsRevision1234 = new CommentService(unitOfWork).GetComments(1234);
                    Assert.AreEqual(1, commentsRevision1234.Count());
                    Assert.AreEqual(1, commentsRevision1234.Count(l => l.User == "tcarter" && l.FileId == 0 && l.LineId == "1:1"));

                    var commentsRevision9122 = new CommentService(unitOfWork).GetComments(9122);
                    Assert.AreEqual(2, commentsRevision9122.Count());
                    Assert.AreEqual(1, commentsRevision9122.Count(l => l.User == "cdog" && l.FileId == 1 && l.LineId == "2:2"));
                    Assert.AreEqual(1, commentsRevision9122.Count(l => l.User == "frefr" && l.FileId == 2));

                    var commentsRevision444 = new CommentService(unitOfWork).GetComments(444);
                    Assert.AreEqual(0, commentsRevision444.Count());
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                DbTestHelper.DebugValidationErrors(ex);
                throw;
            }
        }
    }
}
