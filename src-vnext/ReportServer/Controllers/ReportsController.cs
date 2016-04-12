using System;
using Telerik.Reporting.Cache.Interfaces;

namespace eXpressReport.Controllers
{
    using eXpressReport.Models;
    using System.IO;
    using System.Web;
    using Telerik.Reporting.Cache.File;
    using Telerik.Reporting.Services;
    using Telerik.Reporting.Services.WebApi;

    public class ReportsController : ReportsControllerBase
    {

        static Telerik.Reporting.Services.ReportServiceConfiguration configurationInstance =

        new Telerik.Reporting.Services.ReportServiceConfiguration
        {
            HostAppId = "eXpressReport",
            ReportResolver = new ReportFileResolver(HttpContext.Current.Server.MapPath("~/Reports"))
                .AddFallbackResolver(new ReportTypeResolver()),
            Storage = new Telerik.Reporting.Cache.File.FileStorage(),
        };

        public ReportsController()
        {
            this.ReportServiceConfiguration = configurationInstance;
        }

    }
}
