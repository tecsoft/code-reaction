using CodeReaction.Domain;
using CodeReaction.Domain.Commits;
using CodeReaction.Domain.Entities;
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
    public class CommitServiceFixture
    {
        [SetUp]
        public void TestSetup()
        {
            DbTestHelper.ResetDatabase();
        }


        //[Test]
        //public void GetCommitStats()
        //{
        //    using (UnitOfWork unitOfWork = new UnitOfWork())
        //    {
        //        unitOfWork.Context.Commits.Add(
        //            new CodeReaction.Domain.Commits.Commit()
        //            {
        //                Revision = 1,
        //                Author = "tc",
        //                Message = "Message",
        //                Timestamp = DateTime.Now
        //            });

        //        unitOfWork.Context.Commits.Add(
        //            new CodeReaction.Domain.Commits.Commit()
        //            {
        //                Revision = 2,
        //                Author = "tc",
        //                Message = "Message",
        //                Timestamp = DateTime.Now
        //            });

        //        unitOfWork.Context.Comments.Add(new Comment() { Revision = 2, User = "ab" });
        //        unitOfWork.Context.Comments.Add(new Comment() { Revision = 2, User = "ab" });

        //        unitOfWork.Context.Commits.Add(
        //           new CodeReaction.Domain.Commits.Commit()
        //           {
        //               Revision = 3,
        //               Author = "tc",
        //               Message = "Message",
        //               Timestamp = DateTime.Now
        //           });

        //        unitOfWork.Context.Comments.Add(new Comment() { Revision = 3, User = "ab" });
        //        unitOfWork.Context.Comments.Add(new Comment() { Revision = 3, User = "xy" });

        //        unitOfWork.Context.SaveChanges();

        //        var data = new CommitService(unitOfWork).GetCommitStats("tcarter", 20);

        //        Assert.AreEqual(3, data.Count());

        //        Assert.AreEqual(0, data.First(d => d.Item1.Revision == 1).Item2.NumberReviewers);
        //        Assert.AreEqual(0, data.First(d => d.Item1.Revision == 1).Item2.NumberComments);

        //        Assert.AreEqual(1, data.First(d => d.Item1.Revision == 2).Item2.NumberReviewers);
        //        Assert.AreEqual(2, data.First(d => d.Item1.Revision == 2).Item2.NumberComments);

        //        Assert.AreEqual(2, data.First(d => d.Item1.Revision == 3).Item2.NumberReviewers);
        //        Assert.AreEqual(2, data.First(d => d.Item1.Revision == 3).Item2.NumberComments);



        //    }
    }
}
