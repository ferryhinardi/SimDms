using ClosedXML.Excel;
using SimDms.Absence.Controllers.Utilities;
using SimDms.Absence.Models;
using SimDms.Absence.Models.Results;
using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SimDms.Absence.Controllers
{
    public class BaseController : Controller
    {

        protected DocumentContext ctxDoc = new DocumentContext();
        protected Settings settings = new Settings();
        protected Utility utility = new Utility();
        protected Upload upload = new Upload();

        protected DataContext ctx;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ctx = new DataContext(MyHelpers.GetConnString("DataContext"));
        }

        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/ab/"), jsname);
        }

        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/ab/"), jsname);
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
                return ctx.SysUsers.Find(User.Identity.Name);
            }
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

        protected string BranchName
        {
            get
            {
                return ctx.CoProfiles.Find(CompanyCode, BranchCode).CompanyName;
            }
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

        protected object ObjectMapper(object ObjectFrom, object ObjectTo)
        {
            var listProperty = ObjectFrom.GetType().GetProperties();
            foreach (var prop in listProperty)
            {
                var PropObjB = (from obj in ObjectTo.GetType().GetProperties().ToList()
                                where obj.Name == prop.Name
                                select obj).FirstOrDefault();
                if (PropObjB != null)
                {
                    ObjectTo.GetType().GetProperty(prop.Name).SetValue(ObjectTo, prop.GetValue(ObjectFrom, null), null);
                }
            }
            return ObjectTo;
        }

        protected JsonResult GenerateChart(DataSet ds)
        {
            if (ds.Tables.Count > 1)
            {
                var table1 = ds.Tables[0];
                var table2 = ds.Tables[1];
                var title = "default";
                var type = "pie";
                var template = "#= category #: #= value#";

                foreach (DataRow row in table2.Rows)
                {
                    switch (row[0].ToString())
                    {
                        case "title":
                            title = row[1].ToString();
                            break;
                        case "type":
                            type = row[1].ToString();
                            break;
                        case "template":
                            template = row[1].ToString();
                            break;
                        default:
                            break;
                    }
                }

                switch (type)
                {
                    case "pie":
                        return Json(new
                        {
                            success = true,
                            title = title,
                            type = type,
                            template = template,
                            series = new[] { new { type = type, data = GetJson(table1) } }
                        });
                    default:
                        var series = new List<dynamic>();
                        foreach (DataRow row in table1.Rows)
                        {
                            series.Add(new { name = row[0], data = new object[1] { row[1] } });
                        }

                        return Json(new
                        {
                            success = true,
                            title = title,
                            type = type,
                            template = template,
                            series = series
                        });
                }
            }

            return Json(new { success = false });
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
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
   
    }
}
