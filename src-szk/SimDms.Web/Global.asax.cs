using DoddleReport.Web;
using SimDms.DataWarehouse.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TracerX;

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
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("LegacyUrl",
                     "{controller}/{action}.{extension}",
                     new { controller = "Report" },
                     new { extension = new ReportRouteConstraint() }
                 );

            routes.MapReportingRoute();


            routes.MapRoute("dwh-api", "wh.api/{controller}/{action}/{id}", new { id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers.Api" });
            routes.MapRoute("utl-api", "util.api/{controller}/{action}/{id}", new { controller = "Layout", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.SUtility.Controllers.Api" });

            routes.MapRoute("utl-app", "util/{controller}/{action}/{id}", new { controller = "Layout", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.SUtility.Controllers" });
            routes.MapRoute("gn-app", "gn/{controller}/{action}/{id}", new { controller = "Layout", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.SUtility.Controllers" });
            routes.MapRoute("man-gen", "mp/pi/{action}/{id}", new { controller = "ManPower", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("man-sfm", "mp/sf/{action}/{id}", new { controller = "SalesForce", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("man-srv", "mp/sv/{action}/{id}", new { controller = "ServicePerson", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });

            routes.MapRoute("its-inq", "its/inq/{action}/{id}", new { controller = "Its", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("its-trn", "its/trans/{action}/{id}", new { controller = "Its", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            
            routes.MapRoute("ins-trn", "ins/trans/{action}/{id}", new { controller = "Ins", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("ins-mst", "ins/mst/{action}/{id}", new { controller = "Ins", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });

            routes.MapRoute("cschart", "cs/chart/{action}/{id}", new { controller = "CsChart", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("csreview", "cs/review/{action}/{id}", new { controller = "CsReview", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("csinqry", "cs/inq/{action}/{id}", new { controller = "CustomerSatisfactionInq", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });

            routes.MapRoute("svinqry", "sv/inq/{action}/{id}", new { controller = "ServiceInq", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("svchart", "sv/chr/{action}/{id}", new { controller = "SvChart", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("svtrans", "sv/trn/{action}/{id}", new { controller = "SvTrans", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("svmaster", "sv/master/{action}/{id}", new { controller = "SvMaster", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("svdashboard", "sv/dashboard/{action}/{id}", new { controller = "SvDashboard", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });

            routes.MapRoute("slinqry", "sl/inq/{action}/{id}", new { controller = "Sl", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("spinqry", "sp/inq/{action}/{id}", new { controller = "SparePart", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });

            routes.MapRoute("spkeinq", "spke/inq/{action}/{id}", new { controller = "SPKExhibition", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });
            routes.MapRoute("spketrn", "spke/trans/{action}/{id}", new { controller = "SPKExhibition", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });

            routes.MapRoute("qainqry", "qa/inq/{action}/{id}", new { controller = "Questionnaire", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.DataWarehouse.Controllers" });

            routes.MapRoute("secure", "secure/{id}", new { controller = "Home", action = "Secure", id = UrlParameter.Optional }, new[] { "SimDms.Web.Controllers" });

            routes.MapRoute("default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "SimDms.Web.Controllers" });
           
            //routes.MapRoute(
            //    "Default", // Route name
            //    "{controller}/{action}/{id}", // URL with parameters
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional, area = "" } // Parameter defaults
            //);

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            Database.SetInitializer<SimDms.Web.Models.DataContext>(null);
            Database.SetInitializer<SimDms.DataWarehouse.Controllers.DataContext>(null);
            Database.SetInitializer<SimDms.SUtility.Controllers.DataContext>(null);
            Database.SetInitializer<SimDms.SUtility.Controllers.DataDealerContext>(null);

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            if (string.IsNullOrEmpty(Thread.CurrentThread.Name) || Thread.CurrentThread.Name != "SimDMS.Web.Global")
            { Thread.CurrentThread.Name = "SimDMS.Web.Global"; };
            DbInterception.Add(new ElmahCommandInterceptor());
            MyLogger.Log.Info("Application start");
        }

        protected void Session_Start()
        {
            MyLogger.SetDataString(0, HttpContext.Current.Request.ApplicationPath.ToString().Replace(@"/", ""));
            MyLogger.Log.Info("Session started ...");
            MyLogger.Log.Info("App Path: " + HttpContext.Current.Request.ApplicationPath);
        }
    }
}