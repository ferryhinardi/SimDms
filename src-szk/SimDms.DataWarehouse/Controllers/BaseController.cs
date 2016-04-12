using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;
using SimDms.DataWarehouse.Models;
using ClosedXML.Excel;
using System.Text.RegularExpressions;

namespace SimDms.DataWarehouse.Controllers
{
    public class BaseController : Controller
    {
        protected DataContext ctx = new DataContext();

        protected SuzukiR4Context ctxR4 = new SuzukiR4Context();

        private string[] p_GroupNoNewWithCompany
        {
            get
            {
                return !string.IsNullOrEmpty(Request["CompanyCode"]) ?
                    (Request["CompanyCode"].Split('|').Count() > 1 ? Request["CompanyCode"].Split('|') : ("" + "|" + Request["CompanyCode"]).Split('|'))
                    : ("" + "|" + "").Split('|');
            }
        }

        protected string ParamGroupNoNew
        {
            get
            {
                return p_GroupNoNewWithCompany[0];
            }
        }

        protected string ParamDealerCode
        {
            get
            {
                return p_GroupNoNewWithCompany[1];
            }
        }

        protected string[] GroupNoNewWithCompany(string companyCode)
        {
                return !string.IsNullOrEmpty(companyCode) ?
                    (companyCode.Split('|').Count() > 1 ? companyCode.Split('|') : ("" + "|" + companyCode).Split('|'))
                    : ("" + "|" + "").Split('|');
        }

        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/wh/"), jsname);
        }

        protected dynamic GetJson(DataSet ds)
        {
            var result = new List<dynamic>();
            foreach (DataTable dt in ds.Tables)
            {
                var rows = GetJson(dt);
                result.Add(rows);
            }
            return result;
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

        protected Dictionary<string, object> GetJson(DataRow dr)
        {
            Dictionary<string, object> row = new Dictionary<string, object>();
            foreach (DataColumn col in dr.Table.Columns)
            {
                row.Add(col.ColumnName.Trim(), dr[col]);
            }

            return row;
        }

        public bool updateLastTrnInfo(string moduleName, string tableName)
        {
            string user = CurrentUser.Username;
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter("@ModuleName", moduleName));
            param.Add(new SqlParameter("@TableName", tableName));
            param.Add(new SqlParameter("@UserId", user));
            param.Add(new SqlParameter("@TrnDate", DateTime.UtcNow));

            //var result = octx.ObjectContext.ExecuteStoreCommand("exec uspfn_GetLastUserTrnInfo @ModuleName, @TableName, @UserId, @TrnDate", param.ToArray());

            var result = ctx.Database.ExecuteSqlCommand("exec uspfn_GetLastUserTrnInfo @ModuleName, @TableName, @UserId, @TrnDate", param.ToArray());

            ctx.SaveChanges();

            return true;
        }

        protected string DealerType
        {
            get
            {
                var group = "";
                var userName = User.Identity.Name;
                var oUser = ctx.SysUserViews.Where(p => p.Username == userName).FirstOrDefault();
                if (oUser != null)
                {
                    if (oUser.RoleName.ToLower() == "sfm") { group = "4W"; }
                    if (oUser.RoleName.ToLower() == "sfm2w") { group = "2W"; }
                }

                return group;
            }
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

        protected Dictionary<string, object> GetJsonRow(DataTable dt)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            var row = new Dictionary<string, object>();

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName.Trim(), dr[col]);
                }
            }

            return row;
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
            string fileNameExtension = "xlsx";

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

        protected JsonResult Exec(dynamic options)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;  cmd.CommandText = options.query;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandTimeout = 3600;

            var vars = new Dictionary<string, object>();

            if (options.GetType().GetProperty("param") != null)
            {
                foreach (var par in options.param)
                {
                    cmd.Parameters.Add(new SqlParameter(par.key, par.value));
                    vars.Add(par.key, par.value);
                }
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (options.GetType().GetProperty("result") == null)
            {
                return Json(GetJson(ds.Tables[0]));
            }
            else
            {
                switch ((string)options.result)
                {
                    case "row":
                        return Json(GetJson(ds.Tables[0].Rows[0]));
                    case "dataset":
                        return Json(GetJson(ds));
                    default:
                        return Json(GetJson(ds.Tables[0]));
                }
            }
        }

        protected string ConvertTo112DateFormat(string dateRaw)
        {
            string formattedDate = "";
            string year = "";
            string month = "";
            DateTime date;

            if (!string.IsNullOrWhiteSpace(dateRaw))
            {
                date = DateTime.ParseExact(dateRaw, "yyyy-MM-dd", null);
            }
            else
            {
                date = DateTime.Now;
            }

            if (date != null)
            {
                year = date.Year.ToString();
                month = "0" + date.Month.ToString();
                month = month.Substring(month.Length - 2, 2);
            }
            else
            {
                DateTime now = DateTime.Now;
                year = now.Year.ToString();
                month = "0" + now.Month.ToString();
                month = month.Substring(month.Length - 2, 2);
            }

            formattedDate = year + month;
            return formattedDate;
        }

        protected SysUserView CurrentUser
        {
            get
            {
                return ctx.SysUserViews.Where(p => p.Username == User.Identity.Name).FirstOrDefault();
            }
        }

        protected string CompanyCode
        {
            get
            {
                return ctx.CoProfiles.Where(p => p.BranchCode == CurrentUser.OutletCode).FirstOrDefault() == null ? CurrentUser.DealerCode : ctx.CoProfiles.Where(p => p.BranchCode == CurrentUser.OutletCode).FirstOrDefault().CompanyCode;
            }
        }

        protected string CompanyName
        {
            get
            {
                var companyCode = ctx.CoProfiles.Where(p => p.BranchCode == CurrentUser.OutletCode).FirstOrDefault() == null ? CurrentUser.DealerCode : ctx.CoProfiles.Where(p => p.BranchCode == CurrentUser.OutletCode).FirstOrDefault().CompanyCode;

                return ctx.Organizations.Find(companyCode).CompanyName;
            }
        }

        protected string BranchCode
        {
            get
            {
                return CurrentUser.OutletCode;
            }
        }

        protected string BranchName
        {
            get
            {
                if (CompanyCode == null || BranchCode == null) return "";

                return ctx.CoProfiles.Find(CompanyCode, BranchCode).CompanyName;
            }
        }

        protected string NationalSLS
        {
            get
            {
                return ctx.GnMstLookUpDtls.Find(CurrentUser.DealerCode, "QSLS", "NATIONAL").ParaValue;
            }
        }

        protected string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
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

        string ReplaceHexadecimalSymbols(object txt)
        {
            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(txt.ToString(), r, "", RegexOptions.Compiled);
        }

        protected JsonResult GenerateReportXls(DataTable dt, string sheetName, string fileName, List<List<dynamic>> header = null, List<dynamic> format = null)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);
            var seqno = 1;
            fileName = fileName + "_" + DateTime.Now.ToString("yyyy_MMdd_HHmm") + ".xlsx";

            // add header information
            if (header != null)
            {
                seqno++;
                foreach (List<dynamic> row in header)
                {
                    //foreach (var col in row)
                    for (int i = 0; i < row.Count; i++)
                    {
                        var caption = row[i];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = caption;
                    }
                    seqno++;
                }
                seqno++;
            }

            if (format != null)
            {
                // set caption
                for (int i = 0; i < format.Count; i++)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = format[i].text;
                }
                ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(format.Count)))
                    .Style.Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                seqno++;

                // set cell value
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < format.Count; i++)
                    {
                        var caption = format[i].text;
                        if (row[format[i].name].GetType().Name == "String")
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + ReplaceHexadecimalSymbols(row[format[i].name]);
                        }
                        else
                        {
                            
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = ReplaceHexadecimalSymbols(row[format[i].name]);
                        }
                    }
                    seqno++;
                }

                // set width columns
                for (int i = 0; i < format.Count; i++)
                {
                    ws.Columns((i + 1).ToString()).Width = format[i].width;
                }
            }
            else
            {
                // set caption
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = dt.Columns[i].Caption;
                }
                ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(dt.Columns.Count)))
                    .Style.Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                seqno++;

                // set cell value
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var caption = dt.Columns[i].Caption;
                        if (row[caption].GetType().Name == "String")
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + row[caption];
                        }
                        else
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                        }
                    }
                    seqno++;
                }

                // set width columns
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ws.Columns((i + 1).ToString()).Width = dt.Columns[i].Caption.Length + 5;
                }
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName));

            return Json(new
            {
                rows = dt.Rows.Count,
                cols = dt.Columns.Count,
                range = string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(dt.Columns.Count)),
                fileUrl = Url.Content("~/ReportTemp/" + fileName),
                header = header,
                format = format
            });
        }

        protected JsonResult GenerateReportXls(DataSet ds, List<string> sheets, string fileName, List<List<dynamic>> header = null, List<List<dynamic>> formats = null)
        {
            var wb = new XLWorkbook();
            var count = 0;
            fileName = fileName + "_" + DateTime.Now.ToString("yyyy_MMdd_HHmm") + ".xlsx";
            for (int idx = 0; idx < sheets.Count; idx++)
            {
                var sheetName = sheets[idx];
                var ws = wb.Worksheets.Add(sheetName);
                var dt = ds.Tables[idx];
                var seqno = 1;
                List<dynamic> format = (formats != null && formats.Count > idx) ? formats[idx] : null;
                count += dt.Rows.Count;

                // add header information
                if (header != null)
                {
                    seqno++;
                    foreach (List<dynamic> row in header)
                    {
                        //foreach (var col in row)
                        for (int i = 0; i < row.Count; i++)
                        {
                            var caption = row[i];
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = caption;
                        }
                        seqno++;
                    }
                    seqno++;
                }

                if (format != null)
                {
                    // set caption
                    for (int i = 0; i < format.Count; i++)
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = format[i].text;
                    }
                    ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(format.Count)))
                        .Style.Font.SetBold()
                        .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    seqno++;

                    // set cell value
                    foreach (DataRow row in dt.Rows)
                    {
                        for (int i = 0; i < format.Count; i++)
                        {
                            var caption = format[i].text;
                            if (row[format[i].name].GetType().Name == "String")
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + row[format[i].name];
                            }
                            else
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[format[i].name];
                            }
                        }
                        seqno++;
                    }

                    // set width columns
                    for (int i = 0; i < format.Count; i++)
                    {
                        ws.Columns((i + 1).ToString()).Width = format[i].width;
                    }
                }
                else
                {
                    // set caption
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = dt.Columns[i].Caption;
                    }
                    ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(dt.Columns.Count)))
                        .Style.Font.SetBold()
                        .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    seqno++;

                    // set cell value
                    foreach (DataRow row in dt.Rows)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            var caption = dt.Columns[i].Caption;
                            if (row[caption].GetType().Name == "String")
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + row[caption];
                            }
                            else
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                            }
                        }
                        seqno++;
                    }

                    // set width columns
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ws.Columns((i + 1).ToString()).Width = dt.Columns[i].Caption.Length + 5;
                    }
                }
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName));

            return Json(new
            {
                fileUrl = Url.Content("~/ReportTemp/" + fileName),
                header = header,
                rows = count
            });
        }

        public bool GenerateSQL(SysSQLGateway sql)
        {
            bool ret = false;

            if (sql == null) return ret;

            var findFirst = ctx.SQLGateway.Find(sql.TaskNo);

            if (findFirst == null)
            {
                try
                {
                    sql.UserId = CurrentUser.Username;
                    var filename = DateTime.Now.ToString("yyyyMMdd") + "_" + sql.TaskNo + ".js";
                    sql.FileName = filename;
                    ctx.SQLGateway.Add(sql);
                    ctx.SaveChanges();

                    ret = true;

                    var fileContents = System.IO.File.ReadAllText(Server.MapPath(@"~/assets/js/app/util/tpl/publish_sql.js"));
                    var fileDir = System.Configuration.ConfigurationManager.AppSettings["ScriptFolder"].ToString();
                    
                    fileContents = fileContents.Replace("{{$TASKNO$}}", sql.TaskNo).Replace("{{$TASKNAME$}}", sql.TaskName).Replace("{{$SQL$}}", sql.SQL);

                    System.IO.File.WriteAllText(fileDir +  filename, fileContents);

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            return ret;
        }

    }

    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "POST, GET, OPTIONS");
            base.OnActionExecuting(filterContext);
        }
    }
}
