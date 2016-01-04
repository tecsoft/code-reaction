using CodeReaction.Domain;
using CodeReaction.Domain.Entities;
using CodeReaction.Domain.Users;
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
    public class UserServiceFixture
    {
        [SetUp]
        public void TestSetup()
        {
            DbTestHelper.ResetDatabase();
        }

        [Test]
        public void CreateNewUser()
        {
            User user = new User() { Name = "bob" };

            using (UnitOfWork uow = new UnitOfWork())
            {
                uow.Context.Users.Add(user);
                uow.Save();
            }

            using (UnitOfWork uow = new UnitOfWork())
            {
                Assert.AreEqual(1, uow.Context.Users.Count());
                Assert.AreEqual("bob", uow.Context.Users.First().Name);
                Assert.AreEqual(1, uow.Context.Users.First().Id);
            }
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(DomainException), ExpectedMessage = "Username_Mandatory")]
        public void CreateNewUser_Name_Is_Mandatory()
        {
            User user = new User() { };

            using (UnitOfWork uow = new UnitOfWork())
            {
                var service = new UserService(uow);
                service.CreateUser("", "");
                uow.Save();
            }
        }

        [Test]
        public void CreateUser_Ok()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                Assert.AreEqual(0, uow.Context.Users.Count());
                var service = new UserService(uow);

                service.CreateUser("bob", "bob");
                uow.Save();

                Assert.AreEqual(1, uow.Context.Users.Count(u => u.Name == "bob"));

                service.CreateUser("jim", "jim");
                uow.Save();

                Assert.AreEqual(1, uow.Context.Users.Count(u => u.Name == "jim"));
                Assert.AreEqual(1, uow.Context.Users.Count(u => u.Name == "bob"));
            }
        }

        [Test]
        [ExpectedException(ExpectedException=typeof(DomainException), ExpectedMessage="Duplicate_User")]
        public void CreateUser_Duplicate_UserNames_Not_Allowed()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                Assert.AreEqual(0, uow.Context.Users.Count());
                var service = new UserService(uow);

                service.CreateUser("bob", "bob");
                uow.Save();

                Assert.AreEqual(1, uow.Context.Users.Count(u => u.Name == "bob"));
            }

            using (UnitOfWork uow = new UnitOfWork())
            {
                var service = new UserService(uow);
                service.CreateUser("bob", "jim");

            }
        }

        [Test]
        public void ValidateUserAndPassword_True_If_Password_Matches_User_Else_False()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var service = new UserService(uow);
                service.CreateUser("mickey", "somepassword");
                service.CreateUser("minnie", "somepassword");
                service.CreateUser("daffy", "anotherpassword");
                uow.Save();
            }

            using (UnitOfWork uow = new UnitOfWork())
            {
                var service = new UserService(uow);

                Assert.IsTrue(service.ValidateUserAndPassword("mickey", "somepassword"));
                Assert.IsTrue(service.ValidateUserAndPassword("minnie", "somepassword"));
                Assert.IsFalse(service.ValidateUserAndPassword("daffy", "somepassword"));

                Assert.IsFalse(service.ValidateUserAndPassword("mickey", "anotherpassword"));
                Assert.IsFalse(service.ValidateUserAndPassword("minnie", "anotherpassword"));
                Assert.IsTrue(service.ValidateUserAndPassword("daffy", "anotherpassword"));

                Assert.IsFalse(service.ValidateUserAndPassword("unknownuser", "somepassword"));
                Assert.IsFalse(service.ValidateUserAndPassword("unknownuser", "anotherpassword"));
            }
        }
    }
}
