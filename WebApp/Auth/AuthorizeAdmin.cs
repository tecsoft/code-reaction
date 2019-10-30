using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace CodeReaction.Web.Auth
{
    public class AuthorizeAdminAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            bool standardAuthorisation = base.IsAuthorized(actionContext);
            if (standardAuthorisation == false)
                return false;

            string userId = actionContext.ControllerContext.RequestContext.Principal.Identity.GetUserId();

            var applicationUserManager = actionContext.ControllerContext.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var user = applicationUserManager.FindById(userId);

            return user != null && user.IsAdmin;

        }
    }
}