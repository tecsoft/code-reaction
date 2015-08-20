using CodeReaction.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Users
{
    public class UserService
    {
        UnitOfWork unitOfWork;
        public UserService(UnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public void CreateUser(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new DomainException("Username_Mandatory");
            }

            User user = unitOfWork.Context.Users.Where(u => u.Name == userName).FirstOrDefault();

            if (user != null)
            {
                throw new DomainException("Duplicate_User");
            }

            user = new User() { Name = userName, Password = password };

            unitOfWork.Context.Users.Add(user);
        }

        public bool ValidateUserAndPassword(string userName, string password)
        {
            return unitOfWork.Context.Users.FirstOrDefault(u => u.Name == userName && u.Password == password) != null;
        }

    }
}
