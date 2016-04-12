using System.Web.Optimization;

namespace eXpressAPP
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            if (Settings.DevelopmentMode == "true")
            {

                //bundles.Add(new StyleBundle("~/Content/bootstrap-release").Include(
                //           "~/Content/less/smartadmin-production.min.css",
                //           "~/Content/less/smartadmin-production-plugins.min.css"
                //      ));

                // Custom Css path ... base on SmartAdmin
                bundles.Add(new StyleBundle("~/content/codemirror").IncludeDirectory("~/content/css/codemirror", "*.min.css"));

                bundles.Add(new StyleBundle("~/content/release").IncludeDirectory("~/content/css/release", "*.min.css"));
                //bundles.Add(new StyleBundle("~/content/css/datatables-bundle").IncludeDirectory("~/content/css/datatables", "*.min.css"));

                // Js
                bundles.Add(new ScriptBundle("~/content/scripts/jquery-bundle.js").Include(
                        "~/Content/scripts/libs/jquery-2.2.0.min.js",
                        "~/Content/scripts/libs/jquery/jquery-ui.min.js",
                        "~/Content/scripts/libs/modernizr-2.8.3.min.js",
                        "~/Content/scripts/libs/bootstrap/bootstrap.min.js",
                        "~/Content/scripts/libs/angular-ui/bootbox.min.js",
                        "~/Content/scripts/libs/angular-ui/q.min.js",
                        "~/Content/scripts/libs/jquery/jquery.mousewheel.min.js",
                        "~/Content/scripts/libs/jquery/jquery.mCustomScrollbar.min.js",
                        "~/Content/scripts/libs/underscore.min.js",
                        "~/Content/scripts/libs/moment/moment.min.js",
                        "~/Content/scripts/app.config.min.js",
                        "~/Content/scripts/libs/jquery/jquery.validate.min.js",
                        "~/Content/scripts/libs/jquery/jquery.mb.browser.min.js",
                        "~/Content/scripts/libs/jquery/jquery.ui.touch-punch.min.js",
                        "~/Content/scripts/libs/jquery/fastclick.min.js",
                        //"~/Content/scripts/libs/jquery/superbox.min.js",
                        "~/Content/scripts/libs/smartadmin/notification/SmartNotification.min.js",
                        "~/Content/scripts/libs/smartadmin/smartwidgets/jarvis.widget.min.js",
                        "~/Content/scripts/app.min.js"
                    ));

                bundles.Add(new ScriptBundle("~/content/Scripts/Codemirror-bundle.js").IncludeDirectory("~/content/scripts/libs/codemirror", "*.min.js"));

                bundles.Add(new ScriptBundle("~/content/scripts/fileupload.js").Include(
                    "~/content/scripts/libs/upload/jquery.ui.widget.js",
                    "~/content/scripts/libs/upload/load-image.js",
                    "~/content/scripts/libs/upload/jquery.fileupload.js",
                    "~/content/scripts/libs/upload/jquery.iframe-transport.js",
                    "~/content/scripts/libs/upload/jquery.fileupload.js",
                    "~/content/scripts/libs/upload/jquery.fileupload-process.js",
                    "~/content/scripts/libs/upload/jquery.fileupload-image.js",
                    "~/content/scripts/libs/upload/jquery.fileupload-audio.js",
                    "~/content/scripts/libs/upload/jquery.fileupload-validate.js",
                    "~/content/scripts/libs/upload/jquery.fileupload-angular.js"
                ));

                //bundles.Add(new ScriptBundle("~/content/scripts/datatables-bundle.js").IncludeDirectory("~/content/scripts/libs/datatables", "*.min.js"));

                //bundles.Add(new ScriptBundle("~/content/scripts/datatables-plugins.js").IncludeDirectory("~/content/scripts/libs/grid/datatables/plugins", "*.min.js"));

                bundles.Add(new ScriptBundle("~/content/scripts/core-bundle.js").Include(
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
}
