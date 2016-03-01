using CodeReaction.Domain;
using CodeReaction.Domain.Commits;
using CodeReaction.Domain.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeReaction.Web.Models;

namespace CodeReaction.Tests.WebApp.RevisionDetail
{
    [TestFixture]
    public class RevisionDetailViewModelFixture
    {
        [Test]
        public void Create_Empty()
        {
            var result = RevisionDetailViewModel.Create(new Commit(), new CommitDiff(0), new Like[]{}, new Comment[]{});

            Assert.IsNotNull(result);
            Assert.IsEmpty(result.RevisedFileDetails);
        }

        [Test]
        public void Create_One_FileDiff_No_Likes()
        {
            CommitDiff cd = CreateCommitDiff(0);
            FileDiff fd = AddFileDiff(cd, "file1");
            AddLineDiff(fd,ChangeState.Added, "line0" );
            AddLineDiff(fd, ChangeState.Removed, "line1");

            var result = RevisionDetailViewModel.Create(new Commit(), cd, new Like[] { }, new Comment[] { });

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RevisedFileDetails.Count());
            Assert.AreEqual(2, result.RevisedFileDetails.First().LineDetails.Count());
            Assert.AreEqual(0, result.RevisedFileDetails.Sum(i => i.LikedBy.Count()));
        }
        //[Test]
        //public void Create_With_Liked_File()
        //{
        //    CommitDiff cd = CreateCommitDiff(0);
        //    FileDiff fd1 = AddFileDiff(cd, "file1");
        //    AddLineDiff(fd1, ChangeState.Added, "line0");
        //    AddLineDiff(fd1, ChangeState.Removed, "line1");

        //    FileDiff fd2 = AddFileDiff(cd, "file2");
        //    AddLineDiff(fd2, ChangeState.Added, "line0");
        //    AddLineDiff(fd2, ChangeState.Removed, "line1");

        //    Like like = LikeFile(fd2, "mickey");

        //    var result = RevisionDetailViewModel.Create(new Commit(), cd, new Like[] { like }, new Comment[] { });

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(2, result.RevisedFileDetails.Count());
        //    Assert.AreEqual(0, result.RevisedFileDetails.Where(i => i.Filename == fd1.Name).First().LikedBy.Count());
        //    Assert.AreEqual(1, result.RevisedFileDetails.Where(i => i.Filename == fd2.Name).First().LikedBy.Count());

        //    Like like2 = LikeFile(fd2, "daffy");
        //    result = RevisionDetailViewModel.Create(new Commit(), cd, new Like[] { like, like2 }, new Comment[] { });

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(2, result.RevisedFileDetails.Count());
        //    Assert.AreEqual(0, result.RevisedFileDetails.Where(i => i.Filename == fd1.Name).First().LikedBy.Count());
        //    Assert.AreEqual(2, result.RevisedFileDetails.Where(i => i.Filename == fd2.Name).First().LikedBy.Count());

        //    Like like3 = LikeFile(fd1, "donald");
        //    result = RevisionDetailViewModel.Create(new Commit(), cd, new Like[] { like, like2, like3 }, new Comment[] { });

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(2, result.RevisedFileDetails.Count());
        //    Assert.AreEqual(1, result.RevisedFileDetails.Where(i => i.Filename == fd1.Name).First().LikedBy.Count());
        //    Assert.AreEqual(2, result.RevisedFileDetails.Where(i => i.Filename == fd2.Name).First().LikedBy.Count());
        //}

        [Test]
        public void Create_With_Liked_Line()
        {
            var cd = CreateCommitDiff( 123 );
            var f1 = AddFileDiff( cd, "file1");
            var l1f1 = AddLineDiff( f1, ChangeState.Added, "text" );
          
            var f2 = AddFileDiff( cd, "file2");
            var l1f2 = AddLineDiff(f2, ChangeState.Removed, "text");
            var l2f2 = AddLineDiff(f2, ChangeState.Removed, "text");

            var likel1f2 = LikeLine(f2, l1f2, "bob");

            var result = RevisionDetailViewModel.Create(new Commit(), cd, new Like[] { likel1f2 }, new Comment[] { });

            Assert.AreEqual(2, result.RevisedFileDetails.Count());

            var itemToCheck = result.RevisedFileDetails
                                    .First( i => i.Filename == "file2")
                                        .LineDetails.First( i => i.LineId == l1f2.Id );

            Assert.AreEqual(1, itemToCheck.LikedBy.Count());
            Assert.AreEqual("bob", itemToCheck.LikedBy.First() );
        }

        [Test]
        public void Create_With_Commented_Line()
        {
            var cd1 = CreateCommitDiff(1);
            var f1 = AddFileDiff(cd1, "file1");
            var l1f1 = AddLineDiff(f1, ChangeState.Added, "text");
            var l2f1 = AddLineDiff(f1, ChangeState.Added, "text");

            var cd2 = CreateCommitDiff(2);
            var f2 = AddFileDiff(cd2, "file1");
            var l1f2 = AddLineDiff(f2, ChangeState.Added, "text");
            var l2f2 = AddLineDiff(f2, ChangeState.Added, "text");

            var result = RevisionDetailViewModel.Create(new Commit(), cd1, new Like[] { }, new Comment[] { });
            Assert.AreEqual( 0, result.RevisedFileDetails.SelectMany ( file => file.LineDetails ).Count( line => line.Comments.Count() > 0 ) );

            var comment = CommentLine(f2, l1f2, "bob", "comment");
            result = RevisionDetailViewModel.Create(new Commit(), cd2, new Like[] { }, new Comment[] { comment });
            Assert.AreEqual(1, result.RevisedFileDetails.SelectMany(file => file.LineDetails).Count(line => line.Comments.Count() > 0));

        }

        CommitDiff CreateCommitDiff(int revision)
        {
            return new CommitDiff(revision);
        }

        FileDiff AddFileDiff(CommitDiff commitDiff, string name)
        {
            var result = new FileDiff( FileState.Added, FileType.None, name, null);
            commitDiff.AddFileDiff(result);
            return result;
        }

        static int lineNumber = 0;
        LineDiff AddLineDiff(FileDiff fileDiff, ChangeState state, string text)
        {
            return fileDiff.AddLine(state, text, lineNumber++, lineNumber++);
        }

        Like LikeFile(FileDiff fileDiff, string user)
        {
            return new Like() { File = fileDiff.Name, User = user };
        }

        Like LikeLine(FileDiff fileDiff, LineDiff lineDiff, string user)
        {
            return new Like() { File = fileDiff.Name, LineId = lineDiff.Id, User = user };
        }

        Comment CommentLine( FileDiff fileDiff, LineDiff lineDiff, string user, string comment )
        {
            return new Comment() { File = fileDiff.Name, LineId = lineDiff.Id, User = user, Text = comment };
        }
    }
}
