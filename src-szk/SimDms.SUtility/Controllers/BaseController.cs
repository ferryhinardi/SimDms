using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Configuration;

using SimDms.SUtility.Models;
using System.Web.Script.Serialization;
using System.Data;
using SimDms.SUtility.Models.Others;
using System.Data.SqlClient;

using ClosedXML.Excel;

namespace SimDms.SUtility.Controllers
{
    public class BaseController : Controller
    {
        protected DataContext ctx = new DataContext();
        protected DataDealerContext ctxDealer = new DataDealerContext();
        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/util/"), jsname);
        }
        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/util/"), jsname);
            }
            return string.Format(@"<div id=""{0}"" ></div>", id) + jshtml;
        }
        protected bool CheckTokenAccess(string CompanyName, string ComputerName, string TokenID)
        {
            bool isValid = false;
            var entityToken = ctxDealer.TokenAccesses.Find(CompanyName);
            if (entityToken != null)
            {
                if (entityToken.ExpiredDate > DateTime.Now)
                {
                    if (entityToken.TokenID == TokenID)
                    {
                        //return entityToken;
                        isValid = true;
                    }
                    else
                    {
                        throw new Exception("Token yang anda gunakan tidak valid");
                    }
                }
                else
                {
                    //generate new token
                    int expiredTime = int.Parse(ConfigurationManager.AppSettings["ExpiredTimeToken"].ToString());
                    entityToken.TokenID = Guid.NewGuid().ToString();
                    entityToken.ComputerName = ComputerName;
                    entityToken.ExpiredDate = DateTime.Now.AddMinutes(expiredTime);
                    try
                    {
                        ctx.SaveChanges();
                        isValid = true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error di function CheckTokenAccess, Message=" + ex.ToString());
                    }
                }
            }
            else
            {
                throw new Exception("Maaf komputer anda tidak memiliki akses ke server");
            }
            return isValid;
        }
        protected internal JsonpResult Jsonp(object data, string callback, JsonRequestBehavior jsonRequestBehavior)
        {
            return this.Jsonp(data, callback, null, jsonRequestBehavior);
        }
        protected internal JsonpResult Jsonp(object data, string callback, string contentType, JsonRequestBehavior jsonRequestBehavior)
        {
            return this.Jsonp(data, callback, contentType, null, jsonRequestBehavior);
        }
        protected internal virtual JsonpResult Jsonp(
            object data,
            string callback,
            string contentType,
            Encoding contentEncoding,
            JsonRequestBehavior jsonRequestBehavior)
        {
            return new JsonpResult
            {
                Callback = callback,
                ContentEncoding = contentEncoding,
                ContentType = contentType,
                Data = data,
                JsonRequestBehavior = jsonRequestBehavior
            };
        }
        protected void extend(object object1, object object2)
        {
            var props = object2.GetType().GetProperties();
            foreach (var prop2 in props)
            {
                var prop1 = object1.GetType().GetProperties().Where(p => p.Name == prop2.Name).FirstOrDefault();
                if (prop1 != null)
                {
                    prop1.SetValue(object1, prop2.GetValue(object2, null), null);
                }
            }
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
        protected JsonResult Exec(dynamic options)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = options.query;
            cmd.CommandTimeout = 3600;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

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
                    default:
                        return Json(GetJson(ds.Tables[0]));
                }
            }
        }

        #region Excel
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
        #endregion
    }
}
