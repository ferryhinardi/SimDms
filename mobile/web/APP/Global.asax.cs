using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MvcCodeRouting;
using eXpressAPI;

namespace eXpressAPP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //RegisterViewEngines(ViewEngines.Engines);

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
