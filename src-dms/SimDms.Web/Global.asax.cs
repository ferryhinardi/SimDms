using SimDms.Common;
using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using TracerX;
using CSScriptLibrary;
using DoddleReport;
using DoddleReport.Web;
using System.Web.Http;
using System.Web.Optimization;
using System.Data.Entity.Infrastructure.Interception;

namespace SimDms.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapReportingRoute();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("sp-app", "sp/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Sparepart.Controllers" });
            routes.MapRoute("sp-api", "sp.api/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Sparepart.Controllers.Api" });

            routes.MapRoute("gn-app", "gn/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.General.Controllers" });
            routes.MapRoute("gn-api", "gn.api/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.General.Controllers.Api" });

            routes.MapRoute("sv-api", "sv.api/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Service.Controllers.Api" });
            routes.MapRoute("sv-app", "sv/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Service.Controllers" });

            routes.MapRoute("cs-api", "cs.api/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.CStatisfication.Controllers.Api" });
            routes.MapRoute("cs-app", "cs/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.CStatisfication.Controllers" });

            routes.MapRoute("ab-api", "ab.api/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Absence.Controllers.Api" });
            routes.MapRoute("ab-lookup", "ab.lookup/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Absence.Controllers.Lookup" });
            routes.MapRoute("ab-app", "ab/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Absence.Controllers" });

            routes.MapRoute("its-api", "its.api/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.PreSales.Controllers.Api" });
            routes.MapRoute("its-app", "its/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.PreSales.Controllers" });

            routes.MapRoute("om-api", "om.api/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Sales.Controllers.Api" });
            routes.MapRoute("om-app", "om/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Sales.Controllers" });
            
            //routes.MapRoute("wh-app", "wh/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Sales.Controllers" });
            routes.MapRoute("wh-api", "wh.api/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Sales.Controllers.Api" });
            routes.MapRoute("wh-app", "wh/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Sales.Controllers" });

            routes.MapRoute("tax-api", "tax.api/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Tax.Controllers.Api" });
            routes.MapRoute("tax-app", "tax/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Tax.Controllers" });

            routes.MapRoute("secure", "secure/{id}", new { controller = "Home", action = "Secure", id = UrlParameter.Optional }, new[] { "SimDms.Web.Controllers" });
            routes.MapRoute("default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Web.Controllers" });
            
            

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            Database.SetInitializer<SimDms.Web.Models.LayoutContext>(null);
            Database.SetInitializer<SimDms.General.Models.DataContext>(null);
            Database.SetInitializer<SimDms.CStatisfication.Models.DataContext>(null);
            Database.SetInitializer<SimDms.Absence.Models.DataContext>(null);
            Database.SetInitializer<SimDms.Service.Models.DataContext>(null);
            Database.SetInitializer<SimDms.PreSales.Models.DataContext>(null);
            Database.SetInitializer<SimDms.Sparepart.Models.DataContext>(null);
            Database.SetInitializer<SimDms.Sales.Models.MDContext>(null);
            //Database.SetInitializer<SimDms.Tax.Models.DataContext>(null);

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            if (string.IsNullOrEmpty(Thread.CurrentThread.Name) || Thread.CurrentThread.Name != "SimDMS.Web.Global")
            { Thread.CurrentThread.Name = "SimDMS.Web.Global"; };

            DbInterception.Add(new ElmahCommandInterceptor());

            MyLogger.Log.Info("Application start");
            DbInterception.Add(new CustomEFInterceptor());

        }

        protected void Session_Start()
        {
            MyLogger.SetDataString(0, HttpContext.Current.Request.ApplicationPath.ToString().Replace(@"/",""));
            MyLogger.Log.Info("Session started ...");
            MyLogger.Log.Info("App Path: " + HttpContext.Current.Request.ApplicationPath);

            var allowDebug = System.Configuration.ConfigurationManager.AppSettings["AllowDebug"];
            var UserIdforDebug = System.Configuration.ConfigurationManager.AppSettings["UserIdforDebug"];

            if (allowDebug != null)
            {
                MyLogger.SetDataString(1, allowDebug.ToString());
            }
            else
            {
                MyLogger.SetDataString(1, "false");
            }

            if (UserIdforDebug != null)
            {
                MyLogger.SetDataString(2, UserIdforDebug.ToString());
            }
            else
            {
                MyLogger.SetDataString(2, "");
            }

            //var ctx = new LayoutContext();

            
            // checking for expired password
            //int nChanges = ctx.Database.ExecuteSqlCommand("EXEC uspfn_InitialScript");
            // create log
            //MyLogger.Log.Info("Checking expired password done!");
            //MyLogger.Log.Info(nChanges.ToString() + " row(s) affected");
        }

   }


}