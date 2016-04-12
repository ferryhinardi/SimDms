using System.Web;
using System.Web.Optimization;

namespace eXpressAPP
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new StyleBundle("~/Content/bootstrap-release").Include(
                       "~/Content/less/font-awesome.min.css",
                       "~/Content/css/google-fonts.min.css",
                       "~/Content/less/bootstrap.min.css",
                       "~/Content/less/smartadmin-production.min.css",
                       "~/Content/less/smartadmin-production-plugins.min.css"
                  ));
            
            // Custom Css path ... base on SmartAdmin
            bundles.Add(new StyleBundle("~/content/codemirror").IncludeDirectory("~/content/css/codemirror", "*.min.css"));
            bundles.Add(new StyleBundle("~/content/release").IncludeDirectory("~/content/css/release", "*.min.css"));
            bundles.Add(new StyleBundle("~/content/css/datatables-bundle").IncludeDirectory("~/content/css/datatables", "*.min.css"));
            
            // Js
            bundles.Add(new ScriptBundle("~/content/scripts/jquery").Include(
                    "~/Content/scripts/libs/jquery/jquery-2.1.4.min.js",
                    "~/Content/scripts/libs/jquery/jquery-ui.min.js",
                    "~/Content/scripts/libs/modernizr-2.8.3.min.js",
                    "~/Content/scripts/libs/bootstrap/bootstrap.min.js",
                    "~/Content/scripts/libs/angular-ui/bootbox.min.js",
                    "~/Content/scripts/libs/angular-ui/q.min.js",
                    "~/Content/scripts/libs/jquery/jquery.mousewheel.min.js",
                    "~/Content/scripts/libs/jquery/jquery.mCustomScrollbar.min.js",
                    "~/Content/scripts/libs/underscore.min.js",
                    "~/Content/scripts/libs/moment/moment.min.js",
                    "~/Content/scripts/libs/fullcalendar/dist/fullcalendar.min.js"                
                ));

            bundles.Add(new ScriptBundle("~/content/Scripts/Codemirror").IncludeDirectory("~/content/scripts/libs/codemirror", "*.min.js"));

            bundles.Add(new ScriptBundle("~/content/Scripts/kendo").IncludeDirectory("~/content/scripts/libs/kendo", "*.min.js"));

            bundles.Add(new ScriptBundle("~/scripts/datatables").IncludeDirectory("~/content/scripts/libs/grid/datatables", "*.min.js")); 
            
            bundles.Add(new ScriptBundle("~/scripts/datatables-plugins.js").IncludeDirectory("~/content/scripts/libs/grid/datatables/plugins", "*.min.js"));

            bundles.Add(new ScriptBundle("~/content/scripts/core").Include(
                "~/Content/scripts/app.config.min.js",
                "~/Content/scripts/libs/jquery/jquery.validate.min.js",
                "~/Content/scripts/libs/jquery/jquery.mb.browser.min.js",
                "~/Content/scripts/libs/jquery/jquery.ui.touch-punch.min.js",                
                "~/Content/scripts/libs/jquery/fastclick.min.js",
                "~/Content/scripts/libs/smartadmin/smartwidgets/jarvis.widget.min.js",
                "~/Content/scripts/app.min.js"));
           
            BundleTable.EnableOptimizations = true;

        }
    }
}
