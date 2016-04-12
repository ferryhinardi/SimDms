using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using EP = SimDms.Common.EPPlusHelper;
using SimDms.Common;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class ExcelController : BaseController
    {
        public JsonResult Customers()
        {
            var list = from p in ctx.Customers
                       select new
                       {
                           p.CompanyCode,
                           p.CustomerCode,
                           p.CustomerName,
                           p.HPNo,
                       };
            return Json(list.OrderBy(p => p.CompanyCode).ThenBy(p => p.CustomerCode).Take(15));
        }

        public ActionResult List()
        {
            var list = ctx.Customers.Take(100).ToList();
            var grid = new GridView();
            grid.DataSource = list;
            grid.DataBind();

            var name = Request["name"];
            if (string.IsNullOrWhiteSpace(name)) name = "XlsData";

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return null;
        }


        public string Export()
        {
            var name = Request["name"];
            var html = Request["html"];

            if (string.IsNullOrWhiteSpace(name)) name = "XlsData";

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";

            Response.Output.Write(HttpUtility.HtmlDecode(html));
            Response.Flush();
            Response.End();

            return name;
        }

        public JsonResult Printxls(string BranchCodeParam, string DateFromParam, string DateToParam, string StatusParam)
        {
            var message = "";
            var BranchCode = (BranchCodeParam == null) ? Request.Params["BranchCode"] : BranchCodeParam;
            var DateFrom = (DateFromParam == null) ? Request.Params["DateFrom"] : DateFromParam;
            var DateTo = (DateToParam == null) ? Request.Params["DateTo"] : DateToParam;
            var Status = (StatusParam == null) ? Request.Params["Status"] : StatusParam;
            var model = new
            {
                BranchCode = BranchCode,
                DateFrom = DateFrom,
                DateTo = DateTo,
                Status = Status
            };

            try
            {
                var data = (dynamic)null;
                if (Status == "")
                    data = ctx.Database.SqlQuery<OutstandingDeliveryModel>(string.Format("exec uspfn_CsOutstandingDlvryReport '{0}', '{1}', '{2}', NULL", BranchCode, DateFrom, DateTo)).ToList();
                else
                    data = ctx.Database.SqlQuery<OutstandingDeliveryModel>(string.Format("exec uspfn_CsOutstandingDlvryReport '{0}', '{1}', '{2}', {3}", BranchCode, DateFrom, DateTo, Status)).ToList();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                var package = new ExcelPackage();
                package = GenerateExcel(package, model, data);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);
                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        private static ExcelPackage GenerateExcel(ExcelPackage package, dynamic model, List<OutstandingDeliveryModel> data)
        {
            string z = "", x = "";

            #region -- Constants --
            const int
                rTitle = 1,
                rSubtitle = 4,
                rHdrFirst = 5,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 14;
            double
                w586 = EP.GetTrueColWidth(5.86),
                w786 = EP.GetTrueColWidth(7.86),
                w1086 = EP.GetTrueColWidth(10.86),
                w1143 = EP.GetTrueColWidth(11.43),
                w1243 = EP.GetTrueColWidth(12.43),
                w1600 = EP.GetTrueColWidth(16.00),
                w1700 = EP.GetTrueColWidth(17.00),
                w1800 = EP.GetTrueColWidth(18.00),
                w1900 = EP.GetTrueColWidth(19.00),
                w3000 = EP.GetTrueColWidth(30.00),
                w4214 = EP.GetTrueColWidth(42.14);
            #endregion
            var sheet = package.Workbook.Worksheets.Add((model.BranchCode == "") ? "ALL" : model.BranchCode);

            //TITLE
            #region -- Title & Header --
            z = EP.GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;
            x = EP.GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "OUTSTANDING DELIVERY";
            sheet.Cells[x].Style.Font.Bold = true;
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[x].Style.Font.Size = 11;

            string from = model.DateFrom;
            string to = model.DateTo;
            if (from != "")
                from = from.Substring(6, 2) + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Int32.Parse(from.Substring(4, 2))) + "-" + from.Substring(0, 4);
            if (to != "")
                to = to.Substring(6, 2) + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Int32.Parse(to.Substring(4, 2))) + "-" + to.Substring(0, 4);
            z = EP.GetRange((rTitle+1), cStart, (rTitle+1), cTitleVal);
            sheet.Cells[z].Merge = true;
            x = EP.GetCell((rTitle + 1), cStart);
            sheet.Cells[x].Value = "PERIODE TGL BPK : " + from + " - " + to;
            sheet.Cells[x].Style.Font.Bold = true;
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[x].Style.Font.Size = 11;
            
            x = EP.GetCell(rSubtitle, cStart);
            sheet.Cells[x].Value = "STATUS DELIVERY : " + ((model.Status == "1") ? "INPUTTED" : (model.Status == "0") ? "NOT INPUTTED" : "ALL");
            sheet.Cells[x].Style.Font.Bold = true;
            sheet.Cells[x].Style.Font.Size = 11;
            #endregion

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w586, Value = "NO" },
                new ExcelCellItem { Column = 2, Width = w1243, Value = "Branch" },
                new ExcelCellItem { Column = 3, Width = w3000, Value = "" },
                new ExcelCellItem { Column = 4, Width = w1600, Value = "BPK" },
                new ExcelCellItem { Column = 5, Width = w1600, Value = "" },
                new ExcelCellItem { Column = 6, Width = w1243, Value = "Delivery Date" },
                new ExcelCellItem { Column = 7, Width = w1243, Value = "Customer" },
                new ExcelCellItem { Column = 8, Width = w1900, Value = "" },
                new ExcelCellItem { Column = 9, Width = w1800, Value = "Sales Model" },
                new ExcelCellItem { Column = 10, Width = w786, Value = "" },
                new ExcelCellItem { Column = 11, Width = w1600, Value = "Chassis" },
                new ExcelCellItem { Column = 12, Width = w1143, Value = "" },
                new ExcelCellItem { Column = 13, Width = w1243, Value = "Engine" },
                new ExcelCellItem { Column = 14, Width = w1243, Value = "" },
            };

            sheet.Cells[EP.GetRange(rHdrFirst, 1, (rHdrFirst + 1), 1)].Merge = true;
            sheet.Cells[EP.GetRange(rHdrFirst, 2, rHdrFirst, 3)].Merge = true;
            sheet.Cells[EP.GetRange(rHdrFirst, 4, rHdrFirst, 5)].Merge = true;
            sheet.Cells[EP.GetRange(rHdrFirst, 6, (rHdrFirst + 1), 6)].Merge = true;
            sheet.Cells[EP.GetRange(rHdrFirst, 7, rHdrFirst, 8)].Merge = true;
            sheet.Cells[EP.GetRange(rHdrFirst, 9, rHdrFirst, 10)].Merge = true;
            sheet.Cells[EP.GetRange(rHdrFirst, 11, rHdrFirst, 12)].Merge = true;
            sheet.Cells[EP.GetRange(rHdrFirst, 13, rHdrFirst, 14)].Merge = true;

            var headers2 = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w586, Value = "" },
                new ExcelCellItem { Column = 2, Width = w1243, Value = "Code" },
                new ExcelCellItem { Column = 3, Width = w3000, Value = "Name" },
                new ExcelCellItem { Column = 4, Width = w1600, Value = "No" },
                new ExcelCellItem { Column = 5, Width = w1600, Value = "Date" },
                new ExcelCellItem { Column = 6, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 7, Width = w1243, Value = "Code" },
                new ExcelCellItem { Column = 8, Width = w1900, Value = "Name" },
                new ExcelCellItem { Column = 9, Width = w1800, Value = "Code" },
                new ExcelCellItem { Column = 10, Width = w786, Value = "Year" },
                new ExcelCellItem { Column = 11, Width = w1600, Value = "Code" },
                new ExcelCellItem { Column = 12, Width = w1143, Value = "No" },
                new ExcelCellItem { Column = 13, Width = w1243, Value = "Code" },
                new ExcelCellItem { Column = 14, Width = w1243, Value = "No" },
            };

            foreach (var header in headers)
            {
                x = EP.GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            foreach (var header in headers2)
            {
                x = EP.GetCell((rHdrFirst + 1), header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            
            #region -- Data --
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].No },
                    new ExcelCellItem { Column = 2, Value = data[i].BranchCode },
                    new ExcelCellItem { Column = 3, Value = data[i].OutletAbbreviation },
                    new ExcelCellItem { Column = 4, Value = data[i].BPKNo },
                    new ExcelCellItem { Column = 5, Value = data[i].BPKDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 6, Value = ((data[i].DeliveryDate != "") ? data[i].LockingDate : null), Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 7, Value = data[i].CustomerCode },
                    new ExcelCellItem { Column = 8, Value = data[i].CustomerName },
                    new ExcelCellItem { Column = 9, Value = data[i].SalesModelCode },
                    new ExcelCellItem { Column = 10, Value = data[i].SalesModelYear },
                    new ExcelCellItem { Column = 11, Value = data[i].ChassisCode },
                    new ExcelCellItem { Column = 12, Value = data[i].ChassisNo },
                    new ExcelCellItem { Column = 13, Value = data[i].EngineCode },
                    new ExcelCellItem { Column = 14, Value = data[i].EngineNo },
                };

                foreach (var item in items)
                {
                    x = EP.GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }
            #endregion
            
            return package;
        }

        public FileContentResult DownloadExcelFile(string key, string filename)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + "_" + DateTime.Now.ToString("dd-M-yyyy hh:mm:ss") + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }


        #region -- Model Data --

        private class OutstandingDeliveryModel
        {
            public Int64 No { get; set; }
            public string BranchCode { get; set; }
            public string OutletAbbreviation { get; set; }
            public string BPKNo { get; set; }
            public DateTime? BPKDate { get; set; }
            public string DeliveryDate { get; set; }
            public DateTime? LockingDate { get; set; }            
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string SalesModelCode { get; set; }
            public decimal SalesModelYear { get; set; }
            public string ChassisCode { get; set; }
            public decimal ChassisNo { get; set; }
            public string EngineCode { get; set; }
            public decimal EngineNo { get; set; }
        }
        #endregion
    }
}
