using System.Web.Mvc;

namespace eXpressAPP
{
    public class AuthAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = filterContext.HttpContext.User.Identity.IsAuthenticated
                ? new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden)
                : new HttpUnauthorizedResult();
        }
    }
}