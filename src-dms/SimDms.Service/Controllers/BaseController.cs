using SimDms.Common;
using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ClosedXML.Excel;
using SimDms.Common.Models;
using System.Text;

namespace SimDms.Service.Controllers
{
    public class BaseController : Controller
    {

        protected DataContext ctx;

        protected MDContext ctxMD;


        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ctx = new DataContext(MyHelpers.GetConnString("DataContext"));
            ctxMD = new MDContext(MyHelpers.GetConnString("MDContext"));
        }


        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/sv/"), jsname);
        }

        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/sv/"), jsname);
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

        protected SysUser CurrentUserByUname(string username)
        {
            return ctx.SysUsers.Find(User.Identity.Name);
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

        protected string ProductType
        {
            get
            {
                return ctx.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
            }
        }

        protected string ProfitCenter
        {
            get
            {
                var itm = ctx.SysUserViews.Where(x => x.UserId == CurrentUser.UserId)
                                .FirstOrDefault();

                bool IsAdmin = false;
                if (itm != null)
                {
                    IsAdmin = itm.RoleId == "ADMIN";
                }

                string profitCenter = "200";
                if (!IsAdmin)
                {
                    string s = "000";
                    var x = ctx.SysUserProfitCenters.Find(CurrentUser.UserId);
                    if (x != null) s = x.ProfitCenter;
                    return s;
                }
                else
                {
                    return profitCenter;
                }
            }
        }

        protected bool IsBranch
        {
            get
            {
                return ctx.OrganizationDtls.Find(CompanyCode, BranchCode).IsBranch;
            }
        }

        protected string DateTransValidation(DateTime date)
        {
            var md = DealerCode() == "MD";
            var user = CurrentUser.UserId;
            var currDate = DateTime.Now.Date;
            var errMsg1 = string.Format("{0} tidak sesuai dengan {1}", "Tanggal transaksi", "periode transaksi");
            var errMsg2 = string.Format("{0} tidak sesuai dengan {1}", "Tanggal Transaksi", "Tanggal Server");
            var errMsg3 = string.Format("Periode sedang di locked");
            var errMsg4 = string.Format("Tanggal Transaksi lebih kecil dari tanggal [TransDate]");

            try
            {
                var oService = md ? ctx.CoProfileServices.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode) : ctxMD.CoProfileServices.FirstOrDefault(x => x.CompanyCode == CompanyMD
                    && x.BranchCode == BranchMD);
                if (oService == null) return "Data Service tidak ditemukan";
                if (oService.TransDate == null || oService.TransDate < new DateTime(1900, 1, 2)) oService.TransDate = currDate;
                if (date.Date >= oService.PeriodBeg.Date && date.Date <= oService.PeriodEnd.Date)
                {
                    if (date.Date <= currDate)
                    {
                        if (date.Date >= oService.TransDate.Value.Date)
                        {
                            if (oService.isLocked.Value) throw new Exception(errMsg3);
                        }
                        else
                        {
                            errMsg4 = errMsg4.Replace("[TransDate]", oService.TransDate.Value.Date.ToString("dd-MMM-yyyy"));
                            throw new Exception(errMsg4);
                        }
                        return string.Empty;
                    }
                    else throw new Exception(errMsg2);
                }
                else throw new Exception(errMsg1);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        protected string GetNewDocumentNo(string doctype, DateTime transdate)
        {
            var sql = "uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            var result = ctx.Database.SqlQuery<string>(sql, CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate);
            return result.First();
        }

        protected string DealerCode()
        {
            var result = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
            if (result != null)
            {
                if (result.CompanyMD == CompanyCode && result.BranchMD == BranchCode) { return "MD"; }
                else { return "SD"; }
            }
            else return "MD";
        }

        protected string CompanyMD
        {
            get
            {
                return ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).CompanyMD;
            }
        }

        protected string BranchMD
        {
            get
            {
                return ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).BranchMD;
            }
        }

        protected string WarehouseMD
        {
            get
            {
                return ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).WarehouseMD;
            }
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

        protected string Message(string MessageCode)
        {
            return ctx.Messages.Find(MessageCode).MessageCaption;
        }

        protected bool IsMD
        {
            get
            {
                return DealerCode() == "MD" ? true : false;
            }
        }

        protected bool IsIndependentDealer
        {
            get
            {
                return CompanyCode == CompanyMD && BranchCode == BranchMD ? true : false;
            }
        }

        /// <summary>
        /// Insert Into Table spMstItems, spMstItemLoc, spMstItemPrice
        /// </summary>
        /// <param name="PartNo"></param>
        /// <returns>Int</returns>
        protected int InsertItemsLocPriceFromMD(string PartNo)
        {
            string sql = string.Format(@"EXEC uspfn_spMstItemsInsertFromMD '{0}', '{1}', '{2}','{3}'",
                CompanyCode, BranchCode, PartNo, CurrentUser.UserId);

            return ctx.Database.ExecuteSqlCommand(sql);
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
                    }
                    else if (row[caption].GetType().Name == "DateTime")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.DateFormat.Format = "dd-MMM-yyyy";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }
                    else
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                    }
                }
                seqno++;
            }

            // set width columns
            int lengths = 0;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].ToString().Length >=dt.Columns[i].Caption.Length)
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
