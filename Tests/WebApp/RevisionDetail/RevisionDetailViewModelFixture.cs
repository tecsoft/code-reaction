using CodeReaction.Domain.Commits;
using CodeReaction.Domain.Entities;
using CodeReaction.Web.Models;
using NUnit.Framework;
using System.Linq;

namespace CodeReaction.Tests.WebApp.RevisionDetail
{
    [TestFixture]
    public class RevisionDetailViewModelFixture
    {
        [Test]
        public void Create_Empty()
        {
            var result = RevisionDetailViewModel.Create(new Commit(), new CommitDiff(0), new Comment[]{});

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

            var result = RevisionDetailViewModel.Create(new Commit(), cd, new Comment[] { });

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RevisedFileDetails.Count());
            Assert.AreEqual(2, result.RevisedFileDetails.First().LineDetails.Count());
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

            var result = RevisionDetailViewModel.Create(new Commit(), cd1, new Comment[] { });
            Assert.AreEqual( 0, result.RevisedFileDetails.SelectMany ( file => file.LineDetails ).Count( line => line.Comments.Count() > 0 ) );

            var comment = CommentLine(f2, l1f2, "bob", "comment");
            result = RevisionDetailViewModel.Create(new Commit(), cd2, new Comment[] { comment });
            Assert.AreEqual(1, result.RevisedFileDetails.SelectMany(file => file.LineDetails).Count(line => line.Comments.Count() > 0));

        }

        [Test]
        public void Create_Order_By_File_Name()
        {
            var cd1 = CreateCommitDiff(1);
            var f1 = AddFileDiff(cd1, "Test/Show/By/Filename/CTheSecondone");
            var l1f1 = AddLineDiff(f1, ChangeState.Added, "text");
            var l2f1 = AddLineDiff(f1, ChangeState.Added, "text");

            var f2 = AddFileDiff(cd1, "Test/Show/By/Filename/ZTheLastone");
            var l1f2 = AddLineDiff(f2, ChangeState.Added, "text");
            var l2f2 = AddLineDiff(f2, ChangeState.Added, "text");

            var f3 = AddFileDiff(cd1, "Test/Show/By/Filename/ATheFirstone");
            var l1f3 = AddLineDiff(f2, ChangeState.Added, "text");
            var l2f3 = AddLineDiff(f2, ChangeState.Added, "text");

            var result = RevisionDetailViewModel.Create(new Commit(), cd1, new Comment[] { });

            var fileList = result.RevisedFileDetails.ToList();

            Assert.AreEqual(3, fileList.Count );
            Assert.IsTrue(fileList[0].Filename.CompareTo(fileList[1].Filename) < 0);
            Assert.IsTrue(fileList[1].Filename.CompareTo(fileList[2].Filename) < 0);
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

        Comment CommentLine( FileDiff fileDiff, LineDiff lineDiff, string user, string comment )
        {
            return new Comment() { File = fileDiff.Name, LineId = lineDiff.Id, User = user, Text = comment };
        }
    }
}
