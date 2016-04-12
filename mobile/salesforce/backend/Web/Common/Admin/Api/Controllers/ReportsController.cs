using System.Web.Http;
using IdentityAdmin.Api.Filters;
using Telerik.Reporting.Services.WebApi;
using eXpressAPI.Models;

namespace IdentityAdmin.Api.Controllers
{
    [NoCache]
    [AllowAnonymous]
    public class ReportsController : ReportsControllerBase
    {
        static Telerik.Reporting.Services.ReportServiceConfiguration configurationInstance =

        new Telerik.Reporting.Services.ReportServiceConfiguration
        {
            HostAppId = "CommonReport",
            ReportResolver = new CustomReportResolver(),
            Storage = new Telerik.Reporting.Cache.File.FileStorage(),
        };

        public ReportsController()
        {
            this.ReportServiceConfiguration = configurationInstance;
        }
    }


    class CustomReportResolver : Telerik.Reporting.Services.Engine.IReportResolver
    {
        CodeEditorRepository db = new CodeEditorRepository();
 
        public Telerik.Reporting.ReportSource Resolve(string reportId)
        {
            var key = db.GetSetting("ReportUrl") + reportId.ToLower();

            var reportXml = "";

            var fi = db.GetFileInfo3(key);

            if (fi != null)
            {
                byte[] buffer;

                using (var reader = db.GetFile3(fi.Fileid))
                {
                    int length = (int)reader.Length;  // get file length
                    buffer = new byte[length];            // create buffer
                    int count;                            // actual number of bytes read
                    int sum = 0;                          // total number of bytes read

                    // read until Read method returns 0 (end of the stream has been reached)
                    while ((count = reader.Read(buffer, sum, length - sum)) > 0)
                        sum += count;

                    reportXml = System.Text.Encoding.UTF8.GetString(buffer);
                }
            } 

            if (string.IsNullOrEmpty(reportXml))
            {
                throw new System.Exception("Unable to load a report with the specified ID: " + reportId);
            }

            return new Telerik.Reporting.XmlReportSource { Xml = reportXml };
        }
    }

}
