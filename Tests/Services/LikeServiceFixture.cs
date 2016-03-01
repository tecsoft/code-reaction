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
    public class LikeServiceFixture
    {
        [SetUp]
        public void TestSetup()
        {
            DbTestHelper.ResetDatabase();
        }

        [Test]
        public void GetLikes_Given_Revision_Get_All_Likes()
        {
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork() )
                {
                    unitOfWork.Context.Likes.Add(new Like() { Revision = 1234, User = "tcarter", File = "0", LineId = "1:1" });
                    unitOfWork.Context.Likes.Add(new Like() { Revision = 9122, User = "cdog", File = "1", LineId = "2:2" });
                    unitOfWork.Context.Likes.Add(new Like() { Revision = 9122, User = "frefr", File = "2", LineId = "-1:-1" });

                    unitOfWork.Context.SaveChanges();
                }

                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    var likesRevision1234 = new LikeService(unitOfWork).GetLikes(1234);
                    Assert.AreEqual(1, likesRevision1234.Count());
                    Assert.AreEqual(1, likesRevision1234.Count(l => l.User == "tcarter" && l.File == "0" && l.LineId == "1:1"));

                    var likesRevision9122 = new LikeService(unitOfWork).GetLikes(9122);
                    Assert.AreEqual(2, likesRevision9122.Count());
                    Assert.AreEqual(1, likesRevision9122.Count(l => l.User == "cdog" && l.File == "1" && l.LineId == "2:2"));
                    Assert.AreEqual(1, likesRevision9122.Count(l => l.User == "frefr" && l.File == "2"));

                    var likesRevision444 = new LikeService(unitOfWork).GetLikes(444);
                    Assert.AreEqual(0, likesRevision444.Count());
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
