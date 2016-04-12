using SimDms.PreSales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Script.Serialization;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using SimDms.PreSales.Models.Result;
using SimDms.Common;
using SimDms.Common.Models;
using System.Text;
using OfficeOpenXml;

namespace SimDms.PreSales.Controllers
{
    public class BaseController : Controller
    {
        protected DataContext ctx;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ctx = new DataContext(MyHelpers.GetConnString("DataContext"));
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/its/"), jsname);
        }

        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/its/"), jsname);
            }
            return string.Format(@"<div id=""{0}"" ></div>", id) + jshtml;
        }

        protected List<Dictionary<string, object>> GetJson(DataTable dt)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName.Trim(), dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }

        protected SysUser CurrentUser
        {
            get
            {
                var user = ctx.SysUsers.Find(User.Identity.Name);
                user.CoProfile = ctx.CoProfiles.Find(user.CompanyCode, user.BranchCode);
                return user;
            }
            set { }
        }

        protected string CompanyCode
        {
            get
            {
                return CurrentUser.CompanyCode;
            }
        }

        protected string CompanyName
        {
            get
            {
                return ctx.OrganizationHdrs.Find(CurrentUser.CompanyCode).CompanyName;
            }
        }

        protected string BranchCode
        {
            get
            {
                return CurrentUser.BranchCode;
            }
        }

        protected string NationalSLS
        {
            get
            {
                return ctx.LookUpDtls.Find(CurrentUser.CompanyCode, "QSLS", "NATIONAL").ParaValue;
            }
        }

        protected string BranchName
        {
            get
            {
                return ctx.CoProfiles.Find(CompanyCode, BranchCode).CompanyName;
            }
        }

        protected bool IsBranch
        {
            get
            {
                return ctx.OrganizationDtls.Find(CompanyCode, BranchCode).IsBranch;
            }
        }      

        protected string SALESMAN 
        {
            get
            {
                return "10";
            }
        }

        protected string SALES_COORDINATOR
        {
            get
            {
                return "20";
            }
        }

        protected string SALES_HEAD
        {
            get
            {
                return "30";
            }
        }

        protected string BRANCH_MANAGER
        {
            get 
            {
                return "40";
            }
        }

        protected string COO
        {
            get
            {
                return "50";
            }
        }

        protected string SALES_ADMIN 
        {
            get 
            {
                return "60";
            }
        }

        protected void RenderReport(string reportPath, string fileName, double reportWidth, double reportHeight, string reportType, params IEnumerable<object>[] dataSources)
        {
            LocalReport report = new LocalReport()
            {
                ReportPath = reportPath
            };
            report.DataSources.Clear();
            //report.DataSources.Add(new ReportDataSource("MonthlyActivities", dataSource.ToArray()));

            string dataSourcePrefix = "ds";
            int iterator = 0;
            foreach (var item in dataSources)
            {
                report.DataSources.Add(new ReportDataSource((dataSourcePrefix + "_" + iterator), item.ToArray()));
                iterator++;
            }

            string mimeType;
            string encoding;
            string fileNameExtension;

            if (reportHeight == 0)
            {
                string strReportHeight = ConfigurationManager.AppSettings["defaultReportHeight"] ?? "8.3";
                reportHeight = Convert.ToInt32(strReportHeight);
            }

            if (reportWidth == 0)
            {
                string strReportWidth = ConfigurationManager.AppSettings["defaultReportWidth"] ?? "11.7";
                reportWidth = Convert.ToInt32(strReportWidth);
            }

            var deviceInfo =
            string.Format(@""
            + "<DeviceInfo>"
            + "    <OutputFormat>{0}</OutputFormat>"
                + "    <PageWidth>" + reportWidth + "in</PageWidth>"
                + "    <PageHeight>" + reportHeight + "in</PageHeight>"
            + "    <MarginTop>0.5in</MarginTop>"
            + "    <MarginLeft>0.5in</MarginLeft>"
            + "    <MarginRight>0.5in</MarginRight>"
            + "    <MarginBottom>0.5in</MarginBottom>"
            + "</DeviceInfo>", reportType);

            Warning[] warnings;
            string[] streams;

            //Render the report
            var renderedBytes = report.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName + "." + fileNameExtension);
            Response.BinaryWrite(renderedBytes);
            Response.End();

            //RenderReport(Server.MapPath("~/Reports/rdlc/its/followup.rdlc"), "MonthlyActivities", 10, 11.7, "pdf", ds_0, ds_1, ds_2);
            //return null;
        }

        protected ResultModel InitializeResult()
        {
            return new ResultModel()
            {
                status = false,
                message = "",
                details = "",
                data = null
            };
        }

        #region CreatingExcelWithEPPlus

        public FileContentResult DownloadExcelFile(string key, string filename)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + "_" + string.Format("{0}", DateTime.Now.ToString("dd-MMM-yyy HH.mm")) + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        protected static string GetColName(int number)
        {
            return EPPlusHelper.ExcelColumnNameFromNumber(number);
        }

        protected static int GetColNumber(string name)
        {
            return EPPlusHelper.ExcelColumnNameToNumber(name);
        }

        protected static string GetCell(int row, int col)
        {
            return GetColName(col) + row.ToString();
        }

        protected static string GetRange(int row1, int col1, int row2, int col2)
        {
            return GetCell(row1, col1) + ":" + GetCell(row2, col2);
        }

        protected class ExcelCellItem
        {
            public int Column { get; set; }
            public double Width { get; set; }
            public object Value { get; set; }
            public string Format { get; set; }
        }

        protected static double GetTrueColWidth(double width)
        {
            //DEDUCE WHAT THE COLUMN WIDTH WOULD REALLY GET SET TO
            double z = 1d;
            if (width >= (1 + 2 / 3))
            {
                z = Math.Round((Math.Round(7 * (width - 1 / 256), 0) - 5) / 7, 2);
            }
            else
            {
                z = Math.Round((Math.Round(12 * (width - 1 / 256), 0) - Math.Round(5 * width, 0)) / 12, 2);
            }

            //HOW FAR OFF? (WILL BE LESS THAN 1)
            double errorAmt = width - z;

            //CALCULATE WHAT AMOUNT TO TACK ONTO THE ORIGINAL AMOUNT TO RESULT IN THE CLOSEST POSSIBLE SETTING 
            double adj = 0d;
            if (width >= (1 + 2 / 3))
            {
                adj = (Math.Round(7 * errorAmt - 7 / 256, 0)) / 7;
            }
            else
            {
                adj = ((Math.Round(12 * errorAmt - 12 / 256, 0)) / 12) + (2 / 12);
            }

            //RETURN A SCALED-VALUE THAT SHOULD RESULT IN THE NEAREST POSSIBLE VALUE TO THE TRUE DESIRED SETTING
            if (z > 0)
            {
                return width + adj;
            }

            return 0d;
        }

        #endregion
    }
}
