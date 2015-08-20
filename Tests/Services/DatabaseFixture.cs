using CodeReaction.Domain.Entities;
using CodeReaction.Domain.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Tests
{
    [TestFixture]
    public class DatabaseFixture
    {
        [Test]
        public void EF()
        {
            using (var db = new DbCodeReview()  )
            {
                try
                {
                    User user = new User();
                    //user.Id = 1;
                    user.Name = "tcarter";
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var error in ex.EntityValidationErrors)
                    {
                        foreach (var message in error.ValidationErrors)
                        {
                            Console.WriteLine(message.PropertyName + ": " + message.ErrorMessage);
                        }
                    }

                }
            }

            using (var db = new DbCodeReview())
            {
                Assert.AreEqual( 1, db.Users.Count() );
            }
        }
    }
}
