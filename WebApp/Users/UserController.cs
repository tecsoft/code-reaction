using CodeReaction.Domain;
using CodeReaction.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CodeReaction.Web.Auth;

namespace CodeReaction.Web.Users
{
    [IdentityBasicAuthentication]
    public class UserController : ApiController
    {
        [Route("api/users/create/{user}")]
        public IHttpActionResult CreateUser(string user )
        {
            UnitOfWork unitOfWork = new UnitOfWork();

            try
            {
                var password = this.Request.GetQueryNameValuePairs().FirstOrDefault(i => i.Key == "password");
                new UserService(unitOfWork).CreateUser(user, password.Value);
                unitOfWork.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("CreateUser: " + ex);
                return InternalServerError(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return Ok();
        }

        [Route("api/users/login")]
        [Authorize]
        public IHttpActionResult Login()
        {
            return Ok();
        }
    }

}
