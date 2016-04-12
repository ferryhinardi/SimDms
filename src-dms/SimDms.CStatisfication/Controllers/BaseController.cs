using ClosedXML.Excel;
using SimDms.Common;
using SimDms.CStatisfication.Models;
using SimDms.CStatisfication.Models.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SimDms.CStatisfication.Controllers
{
    public class BaseController : Controller
    {
        protected DataContext ctx;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ctx = new DataContext(MyHelpers.GetConnString("DataContext"));
        }

        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/cs/"), jsname);
        }

        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/cs/"), jsname);
            }
            return string.Format(@"<div id=""{0}"" ></div>", id) + jshtml;
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
                return ctx.GnMstOrganizationHdrs.Find(CurrentUser.CompanyCode).CompanyName;
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

        protected ResultModel InitializeResultModel()
        {
            return new ResultModel()
            {
                success = false,
                message = "",
                details = "",
                data = null
            };
        }

        protected DealerInfo DealerInfo()
        {
            DealerInfo result = (from x in ctx.GnMstOrganizationHdrs
                                 select new DealerInfo()
                                 {
                                     CompanyCode = x.CompanyCode,
                                     CompanyName = x.CompanyName
                                 }).FirstOrDefault();

            return result;
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
            JavaScriptSerializer serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
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

        protected JsonResult Exec(dynamic options)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = options.query;
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
                        return Json(GetJsonRow(ds.Tables[0]));
                    case "dataset":
                        return Json(GetJson(ds));
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

        protected JsonResult GenerateReportXlsx(DataTable dt, string sheetName, string fileName, List<List<dynamic>> header = null)
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
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Font.SetBold();
                    }
                    seqno++;
                }
                seqno++;
            }
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
                {//ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    var caption = dt.Columns[i].Caption;
                    if (row[caption].GetType().Name == "String")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                    else if (row[caption].GetType().Name == "DateTime")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.DateFormat.Format = "dd-MMM-yyyy";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                    else if (row[caption].GetType().Name == "Decimal")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = (Convert.ToDecimal(row[caption])) / 100;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.NumberFormat.Format = "0%";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                    else
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                }

                seqno++;
            }

            ws.Range(string.Format("A{0}:{1}{0}", seqno-1, GetExcelColumnName(dt.Columns.Count)))
                       .Style.Font.SetBold()
                       .Fill.SetBackgroundColor(XLColor.CornflowerBlue);

            // set width columns
            int lengths = 0;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].ToString().Length >= dt.Columns[i].Caption.Length)
                {
                    lengths = dt.Rows[0][i].ToString().Length;
                }
                else
                {
                    lengths = dt.Columns[i].Caption.Length;
                }
                ws.Columns((i + 1).ToString()).Width = lengths + 5;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName));

            return Json(new
            {
                rows = dt.Rows.Count,
                cols = dt.Columns.Count,
                range = string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(dt.Columns.Count)),
                fileUrl = Url.Content("~/ReportTemp/" + fileName),
                header = header
            });
        }

        protected JsonResult GenerateReportXlsxwithpercent(DataTable dt, string sheetName, string fileName, List<List<dynamic>> header = null)
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
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Font.SetBold();
                    }
                    seqno++;
                }
                seqno++;
            }
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
                {//ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    var caption = dt.Columns[i].Caption;
                    if (row[caption].GetType().Name == "String")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                    else if (row[caption].GetType().Name == "DateTime")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.DateFormat.Format = "dd-MMM-yyyy";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                    else
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                }

                seqno++;
            }

            ws.Range(string.Format("A{0}:{1}{0}", seqno - 2, GetExcelColumnName(dt.Columns.Count)))
                       .Style.Font.SetBold()
                       .Fill.SetBackgroundColor(XLColor.CornflowerBlue);
            ws.Range(string.Format("A{0}:{1}{0}", seqno - 1, GetExcelColumnName(dt.Columns.Count)))
                       .Style.Font.SetBold()
                       .Fill.SetBackgroundColor(XLColor.CornflowerBlue);

            // set width columns
            int lengths = 0;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].ToString().Length >= dt.Columns[i].Caption.Length)
                {
                    lengths = dt.Rows[0][i].ToString().Length;
                }
                else
                {
                    lengths = dt.Columns[i].Caption.Length;
                }
                ws.Columns((i + 1).ToString()).Width = lengths + 5;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName));

            return Json(new
            {
                rows = dt.Rows.Count,
                cols = dt.Columns.Count,
                range = string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(dt.Columns.Count)),
                fileUrl = Url.Content("~/ReportTemp/" + fileName),
                header = header
            });
        }        

        protected JsonResult GenerateReportXlsxNonTotal(DataTable dt, string sheetName, string fileName, List<List<dynamic>> header = null)
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
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Font.SetBold();
                    }
                    seqno++;
                }
                seqno++;
            }
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
                {//ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    var caption = dt.Columns[i].Caption;
                    if (row[caption].GetType().Name == "String")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                    else if (row[caption].GetType().Name == "DateTime")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.DateFormat.Format = "dd-MMM-yyyy";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                    else if (row[caption].GetType().Name == "Decimal")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = (Convert.ToDecimal(row[caption])) / 100;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.NumberFormat.Format = "0%";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                    else
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                }

                seqno++;
            }

            // set width columns
            int lengths = 0;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].ToString().Length >= dt.Columns[i].Caption.Length)
                {
                    lengths = dt.Rows[0][i].ToString().Length;
                }
                else
                {
                    lengths = dt.Columns[i].Caption.Length;
                }
                ws.Columns((i + 1).ToString()).Width = lengths + 5;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName));

            return Json(new
            {
                rows = dt.Rows.Count,
                cols = dt.Columns.Count,
                range = string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(dt.Columns.Count)),
                fileUrl = Url.Content("~/ReportTemp/" + fileName),
                header = header
            });
        }
        #endregion

    }
}
