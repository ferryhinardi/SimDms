using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MvcCodeRouting;

namespace eXpressAPP
{
    public class MvcApplication : System.Web.HttpApplication
    {

        //public static IEmbeddedReportingServer EmbeddedReportingServer { get; set; }

        protected void Application_Start()
        {
 
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void RegisterViewEngines(ViewEngineCollection viewEngines)
        {
            // Call AFTER you are done making changes to viewEngines
            viewEngines.EnableCodeRouting();
        }

        protected void Session_Start()
        {
             
        }

    }
}
