using CodeReaction.Domain;
using CodeReaction.Web.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace CodeReaction.Web.Users
{
    public class UserController : ApiController
    {
        [Route("api/users/register")]
        public IHttpActionResult CreateUser()
        {
            try
            {
                // do an async version
                var data = Request.Content.ReadAsFormDataAsync();
                data.Wait();
                var result = data.Result;

                string username = result["newuser-username"];
                string password = result["newuser-password"];
                string email = result["newuser-email"];

                ApplicationUser newUser = new ApplicationUser()
                {
                    UserName = username,
                    Email = email
                };

                IdentityResult identity = ApplicationUserManager.Create(newUser, password);

                if ( identity.Succeeded == false )
                {
                    return new AuthenticationFailureResult(identity.Errors.First(), Request);
                }

                ApplicationSignInManager.SignIn(newUser, isPersistent: false, rememberBrowser: false);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("CreateUser: " + ex);
                return InternalServerError(ex);
            }
            return Ok();
        }

        [Route("api/users/reset/{username}")]
        [AuthorizeAdmin]
        [HttpPost]
        public IHttpActionResult ResetUser(string username)
        {
            try
            {
                var appUser = ApplicationUserManager.FindByName(username);
                if (appUser == null)
                    throw new ArgumentException("Username not known");

                ApplicationUserManager.Delete(appUser);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Tried to reset user: {username}: {ex}");
                return InternalServerError(ex);
            }
            return Ok("User reset");
        }

        [Route("api/users")]
        [Authorize]
        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            try
            {
                var users = ApplicationUserManager.Users.Select(u => new { u.UserName, u.IsAdmin }).ToList();

                return Ok(users);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Tried get user list: {ex}");
                return InternalServerError(ex);
            }
        }

        [Route("api/users/admin/{username}/{isAdmin}")]
        [AuthorizeAdmin]
        [HttpPost]
        public IHttpActionResult SetAdmin(string username, bool isAdmin)
        {
            UnitOfWork unitOfWork = null;
            try
            {
                var appUser = ApplicationUserManager.FindByName(username);
           
                if (appUser == null)
                    throw new ArgumentException("Username not known");

                unitOfWork = new UnitOfWork();

                appUser.IsAdmin = isAdmin;
                ApplicationUserManager.Update(appUser);

                unitOfWork.Save();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Tried to set admin for user: {username}: {ex}");
                return InternalServerError(ex);
            }
            finally
            {
                if (unitOfWork != null )
                {
                    unitOfWork.Dispose();
                }
            }
            return Ok();
        }

        [Route("api/users/admin/{username}")]
        [Authorize]
        [HttpGet]
        public IHttpActionResult GetAdmin(string username)
        {
            
            try
            {
                var appUser = ApplicationUserManager.FindByName(username);

                if (appUser == null)
                    throw new ArgumentException("Username not known");

                return Ok(appUser.IsAdmin);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Tried to set admin for user: {username}: {ex}");
                return InternalServerError(ex);
            }
        }

        [Route("api/users/login")]
        [Authorize]
        public IHttpActionResult Login()
        {
            return Ok();
        }

        [Route("api/users/authorize")]
        [AllowAnonymous]
        public IHttpActionResult Authorize()
        {
            var claims = new ClaimsPrincipal(User).Claims.ToArray();
            var identity = new ClaimsIdentity(claims, "Bearer");
            AuthenticationManager.SignIn(identity);
            return Ok();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return Request.GetOwinContext().Authentication;
            }
        }

        private ApplicationUserManager ApplicationUserManager
        {
            get {
                return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private ApplicationSignInManager ApplicationSignInManager
        {
            get
            {
                return Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }
    }

}
