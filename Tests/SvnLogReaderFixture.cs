using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CodeReview.Tests;
using CodeReviewer.Domain;
using CodeReview.Domain;
using System.IO;

namespace CodeReview.Tests
{
    [TestFixture]
    public class CommitReaderFixture
    {
        [Test]
        public void GetLogs()
        {
            var sut = new CommitReader(@"D:\travail\trunk");

            var doc = sut.GetLatestLogs(1);

            Console.WriteLine( doc.OuterXml );

            var logs = sut.ParseLogs(doc);

            var logList = logs.ToList();

            Assert.AreEqual(1, logList.Count);

            Console.WriteLine(logList[0].DateTime);
        }

        [Test]
        public void GetRevisionDiff()
        {
            var sut = new CommitReader(@"D:\travail\trunk");

            FileDiff diff = sut.GetRevisionDiffs(34699)[0];

            Assert.AreEqual("Dev/MSBuild.proj", diff.Index);
            Assert.AreEqual("-155,11", diff.Previous);
            Assert.AreEqual("+155,14", diff.Current);

            var unifiedDiff = sut.GetUnifiedDiff(diff);


            foreach (LineDiff line in unifiedDiff)
            {
                Console.WriteLine(string.Format("[{0}]: {1}", line.Changed, line.Text));
            }
        }

        [Test]
        public void Read_Of_MultiFile_Revision_Diff_Creates_One_FileDiff_Per_File()
        {
            var sut = new CommitReader(@"d:\travail\trunk\dev");

            IList<FileDiff> results;
            using (StreamReader reader = new StreamReader("DiffExample.txt"))
            {
                results = sut.GetRevisionDiffs( 12345, reader );
            }

            Assert.AreEqual(5, results.Count );
        }

        [Test]
        public void Read_Of_Single_File_Revision_Diff_Creates_One_FileDiff()
        {
            var sut = new CommitReader(@"d:\travail\trunk\dev");

            IList<FileDiff> results;
            using (StreamReader reader = new StreamReader("OneDiff.txt"))
            {
                results = sut.GetRevisionDiffs(12345, reader);
            }

            Assert.AreEqual(1, results.Count);
        }
    }
}
