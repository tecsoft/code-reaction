using CodeReaction.Domain.Commits;
using CodeReaction.Domain.Repositories;
using CodeReaction.Tests.Services.Helpers;
using NUnit.Framework;
using System;
using System.Linq;

namespace CodeReaction.Tests.Services
{
    [TestFixture]
    public class CommitQueryFixture
    {
        [SetUp]
        public void TestSetup()
        {
            DbTestHelper.ResetDatabase();
        }
        [Test]
        public void By_ExcludeAuthor()
        {
            using (var database = new DbCodeReview())
            {
                database.Commits.AddRange(new Commit[]
                {
                new Commit() { Author = "daffy", Message = "A message", Timestamp = new DateTime( 2000,1,1 ) },
                new Commit() { Author = "bugs", Message = "Another message", Timestamp = new DateTime(2002,1,2) },
                new Commit() { Author = "mickey", Message = "Yet another", Timestamp = new DateTime( 2000,2,3) },
                });
                database.SaveChanges();
            }

            using (var database = new DbCodeReview())
            {
                var sut = new CommitQuery(database.Commits)
                {
                    ExcludeAuthor = "daffy"
                };

                var result = sut.Execute();

                Assert.AreEqual(2, result.Count<Commit>());
                Assert.AreEqual(0, result.Where(c => c.Author == "daffy").Count<Commit>());
            }
        }

        [Test]
        public void By_IncludeAuthor()
        {
            using (var database = new DbCodeReview())
            {
                database.Commits.AddRange(new Commit[]
                {
                new Commit() { Author = "daffy", Message = "A message", Timestamp = new DateTime( 2000,1,1 ) },
                new Commit() { Author = "bugs", Message = "Another message", Timestamp = new DateTime(2002,1,2) },
                new Commit() { Author = "mickey", Message = "Yet another", Timestamp = new DateTime( 2000,2,3) },
                });
                database.SaveChanges();
            }

            using (var database = new DbCodeReview())
            {
                var sut = new CommitQuery(database.Commits)
                {
                    IncludeAuthor = "daffy"
                };

                var result = sut.Execute();

                Assert.AreEqual(1, result.Count<Commit>());
                Assert.AreEqual(1, result.Where(c => c.Author == "daffy").Count<Commit>());
            }
        }

        [Test]
        public void By_KeyWord_Includes_Log_And_Author()
        {
            using (var database = new DbCodeReview())
            {
                database.Commits.AddRange(new Commit[]
                {
                new Commit() { Author = "daffy", Message = "A message", Timestamp = new DateTime( 2000,1,1 ) },
                new Commit() { Author = "bugs", Message = "Another message", Timestamp = new DateTime(2002,1,2) },
                new Commit() { Author = "mickey", Message = "Yet another", Timestamp = new DateTime( 2000,2,3) },
                });
                database.SaveChanges();
            }

            using (var database = new DbCodeReview())
            {
                var sut = new CommitQuery(database.Commits)
                {
                    Keyword = "m"
                };

                var result = sut.Execute();

                Assert.AreEqual(3, result.Count<Commit>());
            }

            using (var database = new DbCodeReview())
            {
                var sut = new CommitQuery(database.Commits)
                {
                    Keyword = "mess"
                };

                var result = sut.Execute();

                Assert.AreEqual(2, result.Count<Commit>());
                Assert.AreEqual(2, result.Where(c => c.Author == "daffy" || c.Author == "bugs").Count<Commit>());
            }
        }
        
        [Test]
        public void By_ExcludeApproved()
        {
            using (var database = new DbCodeReview())
            {
                database.Commits.AddRange(new Commit[]
                {
                new Commit() { Author = "daffy", Message = "A message", Timestamp = new DateTime( 2000,1,1 ), ApprovedBy="bugs" },
                new Commit() { Author = "bugs", Message = "Another message", Timestamp = new DateTime(2002,1,2) },
                new Commit() { Author = "mickey", Message = "Yet another", Timestamp = new DateTime( 2000,2,3) },
                });
                database.SaveChanges();
            }

            using (var database = new DbCodeReview())
            {
                var sut = new CommitQuery(database.Commits)
                {
                    ExcludeApproved = true
                };

                var result = sut.Execute();

                Assert.AreEqual(2, result.Count<Commit>());
                Assert.AreEqual(0, result.Where(c => c.Author == "daffy").Count<Commit>());

                sut = new CommitQuery(database.Commits)
                {
                    ExcludeApproved = false
                };

                result = sut.Execute();

                Assert.AreEqual(3, result.Count<Commit>());
            }
        }
        
        [Test]
        public void Take_Max()
        {
            using (var database = new DbCodeReview())
            {
                database.Commits.AddRange(new Commit[]
                {
                new Commit() { Author = "daffy", Message = "A message", Timestamp = new DateTime( 2000,1,1 ) },
                new Commit() { Author = "bugs", Message = "Another message", Timestamp = new DateTime(2002,1,2) },
                new Commit() { Author = "mickey", Message = "Yet another", Timestamp = new DateTime( 2000,2,3) },
                new Commit() { Author = "mickey", Message = "Yet another mickey message", Timestamp = new DateTime( 2000,2,1) },
                new Commit() { Author = "mickey", Message = "Yet another mickey mouse message", Timestamp = new DateTime( 2000,2,4) },
                });
                database.SaveChanges();
            }

            using (var database = new DbCodeReview())
            {
                var sut = new CommitQuery(database.Commits)
                {
                   Max = 2
                };

                var result = sut.Execute();

                Assert.AreEqual(2, result.Count<Commit>());

                var resultList = result.ToList();
                Assert.IsTrue(resultList[0].Timestamp <= resultList[1].Timestamp);   
            
            }
        }

        [Test]
        public void By_All_Criteria()
        {
            using (var database = new DbCodeReview())
            {
                database.Commits.AddRange(new Commit[]
                {
                new Commit() { Author = "daffy", Message = "A message", Timestamp = new DateTime( 2000,1,1 ), ApprovedBy="bugs" },
                new Commit() { Author = "bugs", Message = "Another message", Timestamp = new DateTime(2002,1,2) },
                new Commit() { Author = "mickey", Message = "Yet another", Timestamp = new DateTime( 2000,2,3) },
                new Commit() { Author = "mickey", Message = "Yet another message", Timestamp = new DateTime( 2000,2,3) },
                new Commit() { Author = "mickey", Message = "Mickey mouse message", Timestamp = new DateTime(2003,2,3) }
                });
                database.SaveChanges();
            }

            using (var database = new DbCodeReview())
            {
                var sut = new CommitQuery(database.Commits)
                {
                    ExcludeApproved = true,
                    ExcludeAuthor = "daffy",
                    IncludeAuthor = "mickey",
                    Keyword = "message",
                    Max = 2
                };

                var result = sut.Execute();

                Assert.AreEqual(2, result.Count<Commit>());
                Assert.AreEqual(2, result.Where(c => c.Author == "mickey" && c.Message.Contains("message") && c.ApprovedBy == null ).Count<Commit>());
            }
        }

        [Test]
        public void Ordered_By_Timestamp_Ascending()
        {
            using (var database = new DbCodeReview())
            {
                database.Commits.AddRange(new Commit[]
                {
                new Commit() { Author = "daffy", Message = "A message", Timestamp = new DateTime( 2000,1,1 ), ApprovedBy="bugs" },
                new Commit() { Author = "bugs", Message = "Another message", Timestamp = new DateTime(2002,1,2) },
                new Commit() { Author = "mickey", Message = "Yet another", Timestamp = new DateTime( 2000,2,3) },
                new Commit() { Author = "mickey", Message = "Yet another message", Timestamp = new DateTime( 2000,2,3) },
                new Commit() { Author = "mickey", Message = "Mickey mouse message", Timestamp = new DateTime(2003,2,3) }
                });
                database.SaveChanges();
            }

            using (var database = new DbCodeReview())
            {
                var sut = new CommitQuery(database.Commits);

                var resultList = sut.Execute().ToList();

                for ( int i = 1; i < resultList.Count; i++)
                {
                    Assert.IsTrue(resultList[i - 1].Timestamp <= resultList[i].Timestamp);
                }
            }
        }

        [Test]
        public void No_Match_Returns_Empty_Collection()
        {
            using (var database = new DbCodeReview())
            {
                database.Commits.AddRange(new Commit[]
                {
                new Commit() { Author = "daffy", Message = "A message", Timestamp = new DateTime( 2000,1,1 ), },
                new Commit() { Author = "bugs", Message = "Another message", Timestamp = new DateTime(2002,1,2) },
                });
                database.SaveChanges();
            }

            using (var database = new DbCodeReview())
            {
                var sut = new CommitQuery(database.Commits)
                {
                    IncludeAuthor = "donald"
                };

                Assert.AreEqual(0, sut.Execute().Count<Commit>());

                var resultList = sut.Execute().ToList();

                for (int i = 1; i < resultList.Count; i++)
                {
                    Assert.IsTrue(resultList[i - 1].Timestamp < resultList[i].Timestamp);
                }
            }
        }
    }
}
