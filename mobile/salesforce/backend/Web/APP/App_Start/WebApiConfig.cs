using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web;

namespace eXpressAPP
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}/{ext}",
                defaults: new { action = RouteParameter.Optional, id = RouteParameter.Optional, ext = RouteParameter.Optional }
            );

            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
        }
    }

    public class ElmahExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(context.Exception));
        }
    }
}
