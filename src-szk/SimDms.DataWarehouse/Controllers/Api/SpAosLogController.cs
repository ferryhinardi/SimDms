using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml.Style;
using EP = SimDms.DataWarehouse.Helpers.EPPlusHelper;
using System.Drawing;
using SimDms.DataWarehouse.Helpers;
using ClosedXML.Excel;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class SpAosLogController : BaseController
    {
        public JsonResult Years()
        {
            var init = DateTime.Now.Year - 10;
            var years = new List<object>();
            for (int i = 0; i < 11; i++) years.Add(new { value = init + i, text = (init + i).ToString() });
            return Json(years);
        }

        public JsonResult Months()
        {
            var months = new List<object>();
            for (int i = 1; i < 13; i++)
            {
                var date = new DateTime(1900, i, 1);
                var monthString = date.ToString("MMMM");
                months.Add(new { value = i, text = monthString });
            }
            return Json(months);
        }

        public JsonResult Areas(string prodType)
        {
            var db = prodType == "2W" ? "SimDmsR2" : "SimDms";
            var query = String.Format(
                "select distinct cast(GroupNo as varchar) value, Area text from {0}..gnMstDealerMapping where isActive = 1"
                , db);
            var areas = ctx.Database.SqlQuery<ComboResult>(query).ToList();
            
            return Json(areas);
        }

        public JsonResult Dealers(string prodType, string area)
        {
            var db = prodType == "2W" ? "SimDmsR2" : "SimDms";
            if (area != "")
            {
                var groupCode = int.Parse(area);
                var query = String.Format(
                    "select DealerCode value, '(' + DealerAbbreviation + ') - ' + DealerName text from {0}..gnMstDealerMapping where GroupNo = {1} and isActive = 1"
                    , db, groupCode);
                var dealers = ctx.Database.SqlQuery<ComboResult>(query).ToList();
                
                return Json(dealers);
            }
            return Json(new { });
        }

        public JsonResult Outlets(string prodType, string area, string dealer)
        {
            var db = prodType == "2W" ? "SimDmsR2" : "SimDms";
            if (area != "" && area != null && dealer != "" && dealer != null)
            {
                var groupCode = int.Parse(area);
                var query = String.Format(@"
select OutletCode value, OutletAbbreviation text 
from {0}..gnMstDealerOutletMapping
where GroupNo = {1}
and DealerCode = '{2}'
and IsActive = 1
", db, groupCode, dealer);

                var outlets = ctx.Database.SqlQuery<ComboResult>(query).ToList();
                return Json(outlets);
            }
            return Json(new { });
        }

        public JsonResult Query(SpAosLogModel model)
        {
            try
            {
                var areaCode = model.AreaCode ?? "";
                var dealerCode = model.DealerCode ?? "";
                var outletCode = model.OutletCode ?? "";
                model.AreaCode = model.AreaCode ?? "";
                model.DealerCode = model.DealerCode ?? "";
                model.OutletCode = model.OutletCode ?? "";

                string groupNo = "";

                if (dealerCode != "" && dealerCode.IndexOf('|') > 0)
                {
                    string[] param = dealerCode.Split('|');
                    groupNo = param[0];
                    dealerCode = param[1];
                }

                //if (outletCode != "" && outletCode.IndexOf('|') > 0 )
                //{
                //    string[] param2 = outletCode.Split('|');
                //    outletCode = param2[0];
                //}

                var query = "exec usprpt_spAosLogNew @GroupNo, @CompanyCode, @BranchCode, @ProdType, @Year, @Month, @NewArea";
                var parameters = new SqlParameter[]
                {                    
                    new SqlParameter("@GroupNo", areaCode),
                    new SqlParameter("@CompanyCode", dealerCode),
                    new SqlParameter("@BranchCode", outletCode ),
                    new SqlParameter("@ProdType", model.ProdType),
                    new SqlParameter("@Year", model.Year),
                    new SqlParameter("@Month", model.Month),
                    new SqlParameter("@NewArea", groupNo)
                };

                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<SpAosLogResult>(query, parameters).ToList();
                if (data.Count() == 0) throw new Exception("There is no data that matches your query");

                var package = new ExcelPackage();
                var book = GenerateExcel(package, model, data);
                var content = book.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = "", value = guid });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    message = ex.Message,
                    inner = ex.InnerException != null ? ex.InnerException.Message : ""
                });
            }
        }

        public FileContentResult DownloadExcelFile(string key)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=AosLog_" + DateTime.Now.ToString("yyyyMMdd-hhmm") + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        public ExcelPackage GenerateExcel(ExcelPackage package, SpAosLogModel model, List<SpAosLogResult> data)
        {
            var ctx = new DataContext();
            //var areaCode = model.AreaCode == "" ? 0 : int.Parse(model.AreaCode);
            var area = model.AreaCode == "" ? "ALL" : 
                  ctx.GroupAreas.Where(x => x.GroupNo == model.AreaCode).FirstOrDefault().AreaDealer;

            string groupNo = "";
            string cdealer = "";

            if (model.DealerCode.IndexOf('|') > 0)
            {
                string[] param = model.DealerCode.Split('|');
                groupNo = param[0];
                cdealer = param[1];
            }
            
            var dealer = cdealer == "" ? "ALL" :
                ctx.GnMstDealerMappings.FirstOrDefault(x => x.DealerCode == cdealer).DealerName;
            var outlet = model.OutletCode == "" ? "ALL" :
                ctx.GnMstDealerOutletMappingNews.FirstOrDefault(x => x.DealerCode == cdealer &&
                    x.OutletCode == model.OutletCode && x.GroupNo.ToString() == groupNo).OutletArea;
            
            var sheet = package.Workbook.Worksheets.Add("Sheet1");
            var z = sheet.Cells[1, 1];

            #region -- Constants --
            const int
                rProdType = 1,
                rArea = 2,
                rDealer = 3,
                rOutlet = 4,
                rMonth = 5,
                rYear = 6,
                rHeader = 8,
                rData = 9,

                cNo = 1,
                cDealer = 2,
                cOutlet = 3,
                cPOSNo = 4,
                cDate = 5,
                cPartNo = 6,
                cPartName = 7,
                cQty = 8,
                cStatus = 9;

            const string 
                fNumber = "#",
                fDate = "m/d/yyyy",
                fGeneral = "";
            #endregion

            #region -- Title --
            z.Address = EP.GetRange(rProdType, cNo, rYear, cDate);
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rProdType, cNo, rProdType, cDealer);
            z.Merge = true;
            z.Value = "Prod. Type";

            z.Address = EP.GetCell(rProdType, cOutlet);
            z.Value = ": " + model.ProdType;

            z.Address = EP.GetRange(rArea, cNo, rArea, cDealer);
            z.Merge = true;
            z.Value = "Area";

            z.Address = EP.GetCell(rArea, cOutlet);
            z.Value = ": " + area;

            z.Address = EP.GetRange(rDealer, cNo, rDealer, cDealer);
            z.Merge = true;
            z.Value = "Dealer";

            z.Address = EP.GetCell(rDealer, cOutlet);
            z.Value = ": " + dealer;

            z.Address = EP.GetRange(rOutlet, cNo, rOutlet, cDealer);
            z.Merge = true;
            z.Value = "Outlet";

            z.Address = EP.GetCell(rOutlet, cOutlet);
            z.Value = ": " + outlet;

            z.Address = EP.GetRange(rMonth, cNo, rMonth, cDealer);
            z.Merge = true;
            z.Value = "Month";

            z.Address = EP.GetCell(rMonth, cOutlet);
            z.Value = ": " + new DateTime(2000, model.Month, 1).ToString("MMMM");

            z.Address = EP.GetRange(rYear, cNo, rYear, cDealer);
            z.Merge = true;
            z.Value = "Year";

            z.Address = EP.GetCell(rYear, cOutlet);
            z.Value = ": " + model.Year;
            #endregion

            #region -- Headers --
            z.Address = EP.GetCell(rHeader, cNo);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "No.";

            z.Address = EP.GetCell(rHeader, cDealer);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Dealer";

            z.Address = EP.GetCell(rHeader, cOutlet);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Outlet";

            z.Address = EP.GetCell(rHeader, cPOSNo);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "PO No";

            z.Address = EP.GetCell(rHeader, cDate);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "PO Date";

            z.Address = EP.GetCell(rHeader, cPartNo);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Part No";

            z.Address = EP.GetCell(rHeader, cPartName);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Part Name";

            z.Address = EP.GetCell(rHeader, cQty);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Qty Order";

            z.Address = EP.GetCell(rHeader, cStatus);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Status";
            #endregion

            if (data.Count == 0) return package;

            #region -- Data --
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rData + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cNo, Format = fNumber, Value = data[i].No, Width = 6.57 },
                    new ExcelCellItem { Column = cDate, Format = fDate, Value = data[i].POSDate, Width = 15.00 },
                    new ExcelCellItem { Column = cDealer, Format = fDate, Value = data[i].Dealer, Width = 10.43 },
                    new ExcelCellItem { Column = cOutlet, Format = fDate, Value = data[i].Outlet, Width = 24.00 },
                    new ExcelCellItem { Column = cPOSNo, Format = fGeneral, Value = data[i].POSNo, Width = 17.57 },
                    new ExcelCellItem { Column = cPartNo, Format = fGeneral, Value = data[i].PartNo, Width = 20.43 },
                    new ExcelCellItem { Column = cPartName, Format = fGeneral, Value = data[i].PartName, Width = 37.57 },
                    new ExcelCellItem { Column = cQty, Format = fNumber, Value = data[i].OrderQty, Width = 11.14 },
                    new ExcelCellItem { Column = cStatus, Format = fGeneral, Value = data[i].Status, Width = 10.71 },
                };

                foreach (var item in items)
                {
                    z.Address = EP.GetCell(row, item.Column);
                    z.Value = item.Value;
                    z.Style.Numberformat.Format = item.Format;
                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    sheet.Column(item.Column).Width = EP.GetTrueColWidth(item.Width);
                }
            }

            #endregion

            return package;
        }

        public class SpAosLogModel
        {
            public string ProdType { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public string AreaCode { get; set; }
            public string DealerCode { get; set; }
            public string OutletCode { get; set; }
        }

        public class SpAosLogResult
        {
            public Int64? No { get; set; }
            public DateTime? POSDate { get; set; }
            public String Dealer { get; set; }
            public String Outlet { get; set; }
            public String POSNo { get; set; }
            public String PartNo { get; set; }
            public String PartName { get; set; }
            public Decimal? OrderQty { get; set; }
            public String Status { get; set; }
        }

        public class ComboResult
        {
            public string value { get; set; }
            public string text { get; set; }
        }

//        public ActionResult AOSLIstGenerateExcel()
//        {
//            var SuggorAOS = new SpTrnPSuggorAOS();

//            string fileName = "AOS List";

//            string areaRaw = Request.Params["Area"] ?? "";
//            string dealer = Request.Params["Dealer"] ?? "";
//            string outlet = Request.Params["Outlet"] ?? "";
//            string dateFrom = Convert.ToDateTime(Request.Params["DateFrom"]).ToString("yyyyMMdd");
//            string dateTo = Convert.ToDateTime(Request.Params["DateTo"]).ToString("yyyyMMdd");

//            string groupNo = "";
//            if (dealer.IndexOf("|") > 0)
//            {
//                string[] dd = dealer.Split('|');
//                groupNo = dd[0];
//                dealer = dd[1];
//            }

//            string query = "";
//            if (!string.IsNullOrWhiteSpace(outlet))
//            {
//                query = String.Format(@"select a.*, o.DealerAbbreviation, o.OutletAbbreviation from spTrnPSuggorAOS a inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o
//	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode and o.OutletCode ='{3}'
//                                    where convert(varchar, a.ProcessDate, 112) between '{4}' and '{5}'", areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);
//            }
//            else
//            {
//                query = String.Format(@"select a.*, o.DealerAbbreviation, o.OutletAbbreviation from spTrnPSuggorAOS a inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o  
//	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode
//                                    where convert(varchar, a.ProcessDate, 112) between '{3}' and '{4}'", areaRaw, dealer, groupNo, dateFrom, dateTo);
//            }

//            ctx.Database.CommandTimeout = 3600;
//            var data = ctx.Database.SqlQuery<spAOSListExportModel>(query).AsQueryable();




//            //var data = ctx.SpTrnPSuggorAOSs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
//            //&& a.ProcessDate >= StartDate && a.ProcessDate <= EndDate);
//            if (data.Count() == 0) throw new Exception("tidak ada data");

//            var wb = new XLWorkbook();
//            var ws = wb.Worksheets.Add("AOS List");

//            // Setting Title
//            ws.Cell("A1").Value = "REPORT AOS LIST";
//            var Title = ws.Range("A1:C1");
//            Title.Merge();
//            Title.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
//            Title.Style.Font.FontSize = 16;
//            Title.Style.Font.SetBold(true);

//            ws.Cell("A2").Value = "Company : " + CompanyCode + " - "; //+ ctx.DealerMapping.FirstOrDefault(a => a.DealerCode == CompanyCode).DealerAbbreviation;
//            var Company = ws.Range("A2:C2");
//            Company.Merge();
//            Company.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

//            ws.Cell("A3").Value = "Branch : " + BranchCode + " - "; //+ctx.GnMstDealerOutletMappings.FirstOrDefault(a => a.OutletCode == BranchCode).OutletAbbreviation;
//            var Branch = ws.Range("A3:C3");
//            Branch.Merge();
//            Branch.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

//            ws.Cell("A4").Value = "Periode : "; //+ StartDate.ToString("dd-MMM-yyyy") + " s/d " + EndDate.ToString("dd-MMM-yyy");
//            var Periode = ws.Range("A4:C4");
//            Periode.Merge();
//            Periode.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

//            //// Setting Header
//            var preHeader = SuggorAOS.GetType().GetProperties();

//            StyleHeader(ws, 1, 1, 13.30);
//            StyleHeader(ws, 2, 2, 11);
//            StyleHeader(ws, 3, 3, 11.15);
//            StyleHeader(ws, 4, 4, 16.75);
//            StyleHeader(ws, 5, 12, 13);
//            StyleHeader(ws, 13, 24, 10);
//            StyleHeader(ws, 25, 27, 8);
//            StyleHeader(ws, 28, 37, 11.45);
//            StyleHeader(ws, 38, 45, 9.15);
//            StyleHeader(ws, 46, 48, 10.15);
//            StyleHeader(ws, 49, 49, 15);
//            StyleHeader(ws, 50, 50, 9.5);
//            StyleHeader(ws, 51, 51, 5.71);

//            foreach (var h in preHeader.Select((item, index) => new { Index = index + 1, Value = item }))
//            {
//                ws.Cell(6, h.Index).Value = h.Value.Name;
//                ws.Cell(6, h.Index).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
//                //ws.Cell(6, h.Index).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
//                //ws.Cell(6, h.Index).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
//                ws.Cell(6, h.Index).Style.Fill.SetBackgroundColor(XLColor.FromArgb(75, 172, 198));
//            }

//            // data
//            int startIndex = 7;
//            var iItem = 1;


//            foreach (var item in data)
//            {
//                iItem = 1;
//                foreach (var d in data.ElementType.GetProperties())
//                {
//                    var propertyName = d.Name;
//                    var propertyValue = item.GetType().GetProperty(propertyName).GetValue(item, null);
//                    ws.Cell(startIndex, iItem).Value = propertyValue;
//                    //ws.Cell(startIndex, iItem).Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
//                    //ws.Cell(startIndex, iItem).Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);

//                    iItem++;
//                }

//                // format column
//                // 3 
//                ws.Cell(startIndex, 3).Style.DateFormat.SetFormat("dd-MMM-yyyy");
//                //10
//                //ws.Cell(startIndex, 10).SetDataType(XLCellValues.Text);
//                //ws.Cell(startIndex, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
//                ////11-12
//                //ws.Cell(startIndex, 11).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* - ??_);_(@_) ";
//                //ws.Cell(startIndex, 12).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* - ??_);_(@_) ";
//                ////13-37
//                //for (int i = 13; i <= 37; i++)
//                //{
//                //    ws.Cell(startIndex, i).Style.NumberFormat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* - ??_);_(@_) ";
//                //}
//                ////39,40,41,42
//                //ws.Cell(startIndex, 39).Style.NumberFormat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* - ??_);_(@_) ";
//                //ws.Cell(startIndex, 40).Style.NumberFormat.Format = "_(* #,##0.000000_);_(* (#,##0.000000);_(* - ??_);_(@_) ";
//                //ws.Cell(startIndex, 41).Style.NumberFormat.Format = "_(* #,##0.000000_);_(* (#,##0.000000);_(* - ??_);_(@_) ";
//                //ws.Cell(startIndex, 42).Style.NumberFormat.SetFormat("_(* #,##0.00_);_(* (#,##0.00);_(* - ??_);_(@_) ");

//                //for (int i = 43; i <= 50; i++)
//                //{
//                //    ws.Cell(startIndex, i).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* - ??_);_(@_) ";
//                //}

//                startIndex++;
//            }

//            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
//            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
//        }

//        private static void StyleHeader(IXLWorksheet ws, int start, int end, double width)
//        {
//            for (int i = start; i <= end; i++)
//            {
//                ws.Column(i).Width = width;
//            }
//        }

        public ActionResult AOSListExport()
        {
            string areaRaw = Request.Params["Area"] ?? "";
            string dealer = Request.Params["Dealer"] ?? "";
            string outlet = Request.Params["Outlet"] ?? "";
            string dateFrom = Convert.ToDateTime(Request.Params["DateFrom"]).ToString("yyyyMMdd");
            string dateTo = Convert.ToDateTime(Request.Params["DateTo"]).ToString("yyyyMMdd");

            string groupNo = "";
            if (dealer.IndexOf("|") > 0)
            {
                string[] dd = dealer.Split('|');
                groupNo = dd[0];
                dealer = dd[1];
            }

            string query = "";
            if (!string.IsNullOrWhiteSpace(outlet))
            {
                query = String.Format(@"select a.*, o.DealerAbbreviation, o.OutletAbbreviation from spTrnPSuggorAOS a inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o
	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode and o.OutletCode ='{3}'
                                    where convert(varchar, a.ProcessDate, 112) between '{4}' and '{5}'", areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);
            }
            else
            {
                query = String.Format(@"select a.*, o.DealerAbbreviation, o.OutletAbbreviation from spTrnPSuggorAOS a inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o  
	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode
                                    where convert(varchar, a.ProcessDate, 112) between '{3}' and '{4}'", areaRaw, dealer, groupNo, dateFrom, dateTo);
            }

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<spAOSListExportModel>(query).AsQueryable();


            DateTime now = DateTime.Now;
            string fileName = "AOSListReport" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            int recNo = 5;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("AOSList");

            var rngTable = ws.Range("A5:BA5");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.PastelBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.Black;
            //rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;
            rngTable.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
            rngTable.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
            rngTable.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
            rngTable.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
            rngTable.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            ws.Columns("1").Width = 15;
            ws.Columns("2").Width = 30;
            ws.Columns("3").Width = 12;
            ws.Columns("4").Width = 30;
            ws.Columns("5").Width = 15;
            ws.Columns("6").Width = 20;
            ws.Columns("7").Width = 12;
            ws.Columns("8").Width = 12;
            ws.Columns("9").Width = 12;
            ws.Columns("10").Width = 15;
            ws.Columns("11").Width = 15;
            ws.Columns("12").Width = 15;
            ws.Columns("13").Width = 15;
            ws.Columns("14").Width = 15;
            ws.Columns("15").Width = 12;
            ws.Columns("16").Width = 15;
            ws.Columns("17").Width = 15;
            ws.Columns("18").Width = 15;
            ws.Columns("19").Width = 15;
            ws.Columns("20").Width = 15;
            ws.Columns("21").Width = 15;
            ws.Columns("22").Width = 15;
            ws.Columns("23").Width = 15;
            ws.Columns("24").Width = 15;
            ws.Columns("25").Width = 15;
            ws.Columns("26").Width = 15;
            ws.Columns("27").Width = 15;
            ws.Columns("28").Width = 15;
            ws.Columns("29").Width = 15;
            ws.Columns("30").Width = 15;
            ws.Columns("31").Width = 15;
            ws.Columns("32").Width = 15;
            ws.Columns("33").Width = 15;
            ws.Columns("34").Width = 15;
            ws.Columns("35").Width = 15;
            ws.Columns("36").Width = 15;
            ws.Columns("37").Width = 15;
            ws.Columns("38").Width = 15;
            ws.Columns("39").Width = 15;
            ws.Columns("40").Width = 15;
            ws.Columns("41").Width = 15;
            ws.Columns("42").Width = 15;
            ws.Columns("43").Width = 15;
            ws.Columns("44").Width = 15;
            ws.Columns("45").Width = 15;
            ws.Columns("46").Width = 15;
            ws.Columns("47").Width = 15;
            ws.Columns("48").Width = 15;
            ws.Columns("49").Width = 15;
            ws.Columns("50").Width = 15;
            ws.Columns("51").Width = 18;
            ws.Columns("52").Width = 15;
            ws.Columns("53").Width = 15;

            //Header
            ws.Cell("A1").Value = "Area";
            ws.Cell("A2").Value = "Dealer";
            ws.Cell("A3").Value = "Outlet";
            ws.Cell("A4").Value = "Periode";
            ws.Cell("B1").Value = ": " + ctx.GroupAreas.Where(x => x.GroupNo == areaRaw).FirstOrDefault().AreaDealer;
            ws.Cell("B2").Value = ": " + ctx.GnMstDealerMappingNews.Where(x => x.GroupNo.ToString() == groupNo && x.DealerCode == dealer).FirstOrDefault().DealerName;
            ws.Cell("B3").Value = ": " + ctx.GnMstDealerOutletMappingNews.Where(x => x.GroupNo.ToString() == groupNo && x.OutletCode == outlet).FirstOrDefault().OutletName;
            ws.Cell("B4").Value = ": " + dateFrom + " s/d " + dateTo;
            //End Header

            ws.Cell("A" + recNo).Value = "Company Code";
            ws.Cell("B" + recNo).Value = "Dealer Abbr";
            ws.Cell("C" + recNo).Value = "Branch Code";
            ws.Cell("D" + recNo).Value = "Outlet Abbr";
            ws.Cell("E" + recNo).Value = "Process Date";
            ws.Cell("F" + recNo).Value = "New PartNo";
            ws.Cell("G" + recNo).Value = "Moving Code";
            ws.Cell("H" + recNo).Value = "ABC Class";
            ws.Cell("I" + recNo).Value = "Product Type";
            ws.Cell("J" + recNo).Value = "Part Category";
            ws.Cell("K" + recNo).Value = "Type of Goods";
            ws.Cell("L" + recNo).Value = "Supplier Code";
            ws.Cell("M" + recNo).Value = "Cost Price";
            ws.Cell("N" + recNo).Value = "Purchase Price";
            ws.Cell("O" + recNo).Value = "Dmn Qty12";
            ws.Cell("P" + recNo).Value = "Dmn Qty11";
            ws.Cell("Q" + recNo).Value = "Dmn Qty10";
            ws.Cell("R" + recNo).Value = "Dmn Qty09";
            ws.Cell("S" + recNo).Value = "Dmn Qty08";
            ws.Cell("T" + recNo).Value = "Dmn Qty07";
            ws.Cell("U" + recNo).Value = "Dmn Qty06";
            ws.Cell("V" + recNo).Value = "Dmn Qty05";
            ws.Cell("W" + recNo).Value = "Dmn Qty04";
            ws.Cell("X" + recNo).Value = "Dmn Qty03";
            ws.Cell("Y" + recNo).Value = "Dmn Qty02";
            ws.Cell("Z" + recNo).Value = "Dmn Qty01";
            ws.Cell("AA" + recNo).Value = "On Hand";
            ws.Cell("AB" + recNo).Value = "On Order";
            ws.Cell("AC" + recNo).Value = "In Transit";
            ws.Cell("AD" + recNo).Value = "Allocation SP";
            ws.Cell("AE" + recNo).Value = "Allocation SR";
            ws.Cell("AF" + recNo).Value = "Allocation SL";
            ws.Cell("AG" + recNo).Value = "BackOrder SP";
            ws.Cell("AH" + recNo).Value = "BackOrder SR";
            ws.Cell("AI" + recNo).Value = "BackOrder SL";
            ws.Cell("AJ" + recNo).Value = "Reserved SP";
            ws.Cell("AK" + recNo).Value = "Reserved SR";
            ws.Cell("AL" + recNo).Value = "Reserved SL";
            ws.Cell("AM" + recNo).Value = "Available Qty";
            ws.Cell("AN" + recNo).Value = "Order Unit";
            ws.Cell("AO" + recNo).Value = "Dmn Qty";
            ws.Cell("AP" + recNo).Value = "Dmn Avg";
            ws.Cell("AQ" + recNo).Value = "Std Dev";
            ws.Cell("AR" + recNo).Value = "Dev Fac";
            ws.Cell("AS" + recNo).Value = "Max Qty";
            ws.Cell("AT" + recNo).Value = "Min Qty";
            ws.Cell("AU" + recNo).Value = "Lead Time";
            ws.Cell("AV" + recNo).Value = "Order Cycle";
            ws.Cell("AW" + recNo).Value = "Safety Stock";
            ws.Cell("AX" + recNo).Value = "Order Point";
            ws.Cell("AY" + recNo).Value = "Safety Stock Point";
            ws.Cell("AZ" + recNo).Value = "Suggor Qty";
            ws.Cell("BA" + recNo).Value = "Status";

            recNo++;

            foreach (var row in data)
            {
                ws.Cell("A" + recNo).Value = row.CompanyCode;
                //ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("B" + recNo).Value = row.DealerAbbreviation;
                ws.Cell("C" + recNo).Value = row.BranchCode;
                ws.Cell("D" + recNo).Value = row.OutletAbbreviation;
                ws.Cell("E" + recNo).Value = row.ProcessDate;
                ws.Cell("E" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("F" + recNo).Value = row.NewPartNo;
                ws.Cell("G" + recNo).Value = row.MovingCode;
                ws.Cell("H" + recNo).Value = row.ABCClass;
                ws.Cell("I" + recNo).Value = row.ProductType;
                ws.Cell("J" + recNo).Value = row.PartCategory;
                ws.Cell("K" + recNo).Value = row.TypeOfGoods;
                ws.Cell("L" + recNo).Value = row.SupplierCode;
                ws.Cell("M" + recNo).Value = row.CostPrice;
                ws.Cell("N" + recNo).Value = row.PurchasePrice;
                ws.Cell("O" + recNo).Value = row.DmnQty12;
                ws.Cell("P" + recNo).Value = row.DmnQty11;
                ws.Cell("Q" + recNo).Value = row.DmnQty10;
                ws.Cell("R" + recNo).Value = row.DmnQty09;
                ws.Cell("S" + recNo).Value = row.DmnQty08;
                ws.Cell("T" + recNo).Value = row.DmnQty07;
                ws.Cell("U" + recNo).Value = row.DmnQty06;
                ws.Cell("V" + recNo).Value = row.DmnQty05;
                ws.Cell("W" + recNo).Value = row.DmnQty04;
                ws.Cell("X" + recNo).Value = row.DmnQty03;
                ws.Cell("Y" + recNo).Value = row.DmnQty02;
                ws.Cell("Z" + recNo).Value = row.DmnQty01;
                ws.Cell("AA" + recNo).Value = row.OnHand;
                ws.Cell("AB" + recNo).Value = row.OnOrder;
                ws.Cell("AC" + recNo).Value = row.InTransit;
                ws.Cell("AD" + recNo).Value = row.AllocationSP;
                ws.Cell("AE" + recNo).Value = row.AllocationSR;
                ws.Cell("AF" + recNo).Value = row.AllocationSL;
                ws.Cell("AG" + recNo).Value = row.BackOrderSP;
                ws.Cell("AH" + recNo).Value = row.BackOrderSR;
                ws.Cell("AI" + recNo).Value = row.BackOrderSL;
                ws.Cell("AJ" + recNo).Value = row.ReservedSP;
                ws.Cell("AK" + recNo).Value = row.ReservedSR;
                ws.Cell("AL" + recNo).Value = row.ReservedSL;
                ws.Cell("AM" + recNo).Value = row.AvailableQty;
                ws.Cell("AN" + recNo).Value = row.OrderUnit;
                ws.Cell("AO" + recNo).Value = row.DmnQty;
                ws.Cell("AP" + recNo).Value = row.DmnAvg;
                ws.Cell("AQ" + recNo).Value = row.StdDev;
                ws.Cell("AR" + recNo).Value = row.DevFac;
                ws.Cell("AS" + recNo).Value = row.MaxQty;
                ws.Cell("AT" + recNo).Value = row.MinQty;
                ws.Cell("AU" + recNo).Value = row.LeadTime;
                ws.Cell("AV" + recNo).Value = row.OrderCycle;
                ws.Cell("AW" + recNo).Value = row.SafetyStock;
                ws.Cell("AX" + recNo).Value = row.OrderPoint;
                ws.Cell("AY" + recNo).Value = row.SafetyStokPoin;
                ws.Cell("AZ" + recNo).Value = row.SuggorQty;
                ws.Cell("AB" + recNo).Value = row.Status;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public class spAOSListExportModel
        {
            public string CompanyCode { get; set; }
            public string DealerAbbreviation { get; set; }
            public string BranchCode { get; set; }
            public string OutletAbbreviation { get; set; }
            public string NewPartNo { get; set; }
            public string MovingCode { get; set; }
            public string ABCClass { get; set; }
            public string ProductType { get; set; }
            public string PartCategory { get; set; }
            public string TypeOfGoods { get; set; }
            public string SupplierCode { get; set; }
            public decimal CostPrice { get; set; }
            public decimal PurchasePrice { get; set; }
            public decimal DmnQty12 { get; set; }
            public decimal DmnQty11 { get; set; }
            public decimal DmnQty10 { get; set; }
            public decimal DmnQty09 { get; set; }
            public decimal DmnQty08 { get; set; }
            public decimal DmnQty07 { get; set; }
            public decimal DmnQty06 { get; set; }
            public decimal DmnQty05 { get; set; }
            public decimal DmnQty04 { get; set; }
            public decimal DmnQty03 { get; set; }
            public decimal DmnQty02 { get; set; }
            public decimal DmnQty01 { get; set; }
            public decimal OnHand { get; set; }
            public decimal OnOrder { get; set; }
            public decimal InTransit { get; set; }
            public decimal AllocationSP { get; set; }
            public decimal AllocationSR { get; set; }
            public decimal AllocationSL { get; set; }
            public decimal BackOrderSP { get; set; }
            public decimal BackOrderSR { get; set; }
            public decimal BackOrderSL { get; set; }
            public decimal ReservedSP { get; set; }
            public decimal ReservedSR { get; set; }
            public decimal ReservedSL { get; set; }
            public decimal AvailableQty { get; set; }
            public decimal OrderUnit { get; set; }
            public decimal DmnQty { get; set; }
            public decimal DmnAvg { get; set; }
            public decimal StdDev { get; set; }
            public decimal DevFac { get; set; }
            public decimal MaxQty { get; set; }
            public decimal MinQty { get; set; }
            public decimal LeadTime { get; set; }
            public decimal OrderCycle { get; set; }
            public decimal SafetyStock { get; set; }
            public decimal OrderPoint { get; set; }
            public decimal SafetyStokPoin { get; set; }
            public decimal SuggorQty { get; set; }
            public string Status { get; set; }
            public DateTime ProcessDate { get; set; }
        }

        public class SpTrnPSuggorAOS
        {
            public string CompanyCode { get; set; }
            public string BranchCode { get; set; }
            public DateTime ProcessDate { get; set; }
            public string NewPartNo { get; set; }
            public string MovingCode { get; set; }
            public string ABCClass { get; set; }
            public string ProductType { get; set; }
            public string PartCategory { get; set; }
            public string TypeOfGoods { get; set; }
            public string SupplierCode { get; set; }
            public decimal? CostPrice { get; set; }
            public decimal? PurchasePrice { get; set; }
            public decimal? DmnQty12 { get; set; }
            public decimal? DmnQty11 { get; set; }
            public decimal? DmnQty10 { get; set; }
            public decimal? DmnQty09 { get; set; }
            public decimal? DmnQty08 { get; set; }
            public decimal? DmnQty07 { get; set; }
            public decimal? DmnQty06 { get; set; }
            public decimal? DmnQty05 { get; set; }
            public decimal? DmnQty04 { get; set; }
            public decimal? DmnQty03 { get; set; }
            public decimal? DmnQty02 { get; set; }
            public decimal? DmnQty01 { get; set; }
            public decimal? OnHand { get; set; }
            public decimal? OnOrder { get; set; }
            public decimal? InTransit { get; set; }
            public decimal? AllocationSP { get; set; }
            public decimal? AllocationSR { get; set; }
            public decimal? AllocationSL { get; set; }
            public decimal? BackOrderSP { get; set; }
            public decimal? BackOrderSR { get; set; }
            public decimal? BackOrderSL { get; set; }
            public decimal? ReservedSP { get; set; }
            public decimal? ReservedSR { get; set; }
            public decimal? ReservedSL { get; set; }
            public decimal? AvailableQty { get; set; }
            public decimal? OrderUnit { get; set; }
            public decimal? DmnQty { get; set; }
            public decimal? DmnAvg { get; set; }
            public decimal? StdDev { get; set; }
            public decimal? DevFac { get; set; }
            public decimal? MaxQty { get; set; }
            public decimal? MinQty { get; set; }
            public decimal? LeadTime { get; set; }
            public decimal? OrderCycle { get; set; }
            public decimal? SafetyStock { get; set; }
            public decimal? OrderPoint { get; set; }
            public decimal? SafetyStokPoint { get; set; }
            public decimal? SuggorQty { get; set; }
            public string Status { get; set; }
        }
    }
}