using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.DataWarehouse.Models;
using ClosedXML.Excel;
using SimDms.DataWarehouse.Helpers;
using System.Drawing;
using System.Data;// .Objects;
using System.Data.Entity.Core.Objects;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class MpReportController : ReportController
    {
        #region Promotion Region

        public JsonResult Promotion()
        {
            string comp = Request["CompanyCode"];// ?? "--";
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<PromotionViewModel>("exec uspfn_MpViewPromotion @CompanyCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PromotionTotal()
        {
            string comp = Request["CompanyCode"];// ?? "--";
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<PromotionViewModel>("exec uspfn_MpViewPromotion @CompanyCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            var total = new PromotionViewModel();
            total.BM = data.Sum(x => x.BM);
            total.SH = data.Sum(x => x.SH);
            total.Platinum = data.Sum(x => x.Platinum);
            total.Gold = data.Sum(x => x.Gold);
            total.Silver = data.Sum(x => x.Silver);

            return Json(total, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PromotionDetail()
        {
            string comp = Request["CompanyCode"];// ?? "--";
            string bran = Request["BranchCode"];// ?? "--";
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];
            string Promosi = Request["PromosiType"];

            int to = 0; bool fa = true;
            switch (Promosi)
            {
                case "3": Promosi = "2"; break;
                case "4": Promosi = "3"; break;
                case "5": Promosi = "4"; break;
                case "6": Promosi = "SH"; break;
                case "7": Promosi = "BM"; break;
                default: break;
            }

            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<PromotionDetailViewModel>("exec uspfn_MpViewPromotionDetail @CompanyCode=@p0, @BranchCode=@p1, @PeriodFrom=@p2, @PeriodTo=@p3, @PromosiType=@p4", comp, bran, DateFrom, DateTo, Promosi).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PromotionExcel()
        {
            string comp = Request["CompanyCode"];// ?? "--";
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            var message = "";
            try
            {
                var data = ctx.Database.SqlQuery<PromotionViewModel>("exec uspfn_MpViewPromotion @CompanyCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).ToList();
                //return Json(data);
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                comp = comp == "" ? comp = "All Dealer" : ctx.GnMstDealerMappings.FirstOrDefault(x => x.DealerCode == comp).DealerName;
                package = GEPromotionExcel(package, data, comp, DateFrom, DateTo);

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

        private static ExcelPackage GEPromotionExcel(ExcelPackage package, List<PromotionViewModel> data, string comp, string DateFrom, string DateTo)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,
                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1500 = GetTrueColWidth(15.00),
                w1800 = GetTrueColWidth(18.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Promotion Data";
            sheet.Cells[x].Style.Font.Size = 20;

            x = GetCell(rTitle + 2, cStart);
            sheet.Cells[x].Value = "Dealer";
            sheet.Cells[x].Style.Font.Bold = true;
            x = GetCell(rTitle + 2, cStart + 1);
            sheet.Cells[x].Value = comp;

            x = GetCell(rTitle + 3, cStart);
            sheet.Cells[x].Value = "Period";
            sheet.Cells[x].Style.Font.Bold = true;
            x = GetCell(rTitle + 3, cStart + 1);
            sheet.Cells[x].Value = DateTime.Parse(DateFrom).ToString("dd MMM yyyy");
            x = GetCell(rTitle + 3, cStart + 2);
            sheet.Cells[x].Value = "to";
            sheet.Cells[x].Style.Font.Bold = true;
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(rTitle + 3, cStart + 3);
            sheet.Cells[x].Value = DateTime.Parse(DateTo).ToString("dd MMM yyyy");

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w3000, Value = "Outlet" },
                new ExcelCellItem { Column = 2, Width = w1500, Value = "Trainee/to/Silver".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 3, Width = w1500, Value = "Silver/to/Gold".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 4, Width = w1500, Value = "Gold/to/Platinum".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 5, Width = w1500, Value = "Sales Person/to/Sales Head".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 6, Width = w1500, Value = "Sales Head/to/Branch Manager".Replace("/", System.Environment.NewLine) },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Value = header.Value;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.Font.Bold = true;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].Outlet },
                    new ExcelCellItem { Column = 2, Value = data[i].Silver },
                    new ExcelCellItem { Column = 3, Value = data[i].Gold },
                    new ExcelCellItem { Column = 4, Value = data[i].Platinum },
                    new ExcelCellItem { Column = 5, Value = data[i].SH },
                    new ExcelCellItem { Column = 6, Value = data[i].BM },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            //TABLE FOOTER
            var rFooter = rDataStart + data.Count();

            var footers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Value = "TOTAL" },
                new ExcelCellItem { Column = 2, Value = data.Sum(o => o.Silver) },
                new ExcelCellItem { Column = 3, Value = data.Sum(o => o.Gold) },
                new ExcelCellItem { Column = 4, Value = data.Sum(o => o.Platinum) },
                new ExcelCellItem { Column = 5, Value = data.Sum(o => o.SH) },
                new ExcelCellItem { Column = 6, Value = data.Sum(o => o.BM) },
            };

            foreach (var footer in footers)
            {
                x = GetCell(rFooter, footer.Column);
                sheet.Cells[x].Value = footer.Value;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            x = GetCell(rFooter, 1);
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            return package;
        }

        public class PromotionViewModel
        {
            public string OutletCode { get; set; }
            public string Outlet { get; set; }
            public int? BM { get; set; }
            public int? SH { get; set; }
            public int? Platinum { get; set; }
            public int? Gold { get; set; }
            public int? Silver { get; set; }
        }

        public class PromotionDetailViewModel
        {
            public string OutletName { get; set; }
            public string Name { get; set; }
            public string Position { get; set; }
            public string Grade { get; set; }
            public DateTime? JoinDate { get; set; }
        }
        #endregion

        #region Demotion Region

        public JsonResult Demotion()
        {
            string comp = Request["CompanyCode"];// ?? "--";
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<DemotionViewModel>("exec uspfn_MpViewDemotion @CompanyCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DemotionTotal()
        {
            string comp = Request["CompanyCode"];// ?? "--";
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<DemotionViewModel>("exec uspfn_MpViewDemotion @CompanyCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            var total = new DemotionViewModel();
            total.SH = data.Sum(x => x.SH);
            total.SC = data.Sum(x => x.SC);
            total.Gold = data.Sum(x => x.Gold);
            total.Silver = data.Sum(x => x.Silver);

            return Json(total, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DemotionDetail()
        {
            string comp = Request["CompanyCode"];// ?? "--";
            string bran = Request["BranchCode"];// ?? "--";
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];
            string Demosi = Request["DemosiType"];

            int to = 0; bool fa = true;
            switch (Demosi)
            {
                case "3": Demosi = "SH"; break;
                case "4": Demosi = "SC"; break;
                case "5": Demosi = "3"; break;
                case "6": Demosi = "2"; break;
                default: break;
            }

            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<DemotionDetailViewModel>("exec uspfn_MpViewDemotionDetail @CompanyCode=@p0, @BranchCode=@p1, @PeriodFrom=@p2, @PeriodTo=@p3, @DemosiType=@p4", comp, bran, DateFrom, DateTo, Demosi).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DemotionExcel()
        {
            string comp = Request["CompanyCode"];// ?? "--";
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            var message = "";
            try
            {
                var data = ctx.Database.SqlQuery<DemotionViewModel>("exec uspfn_MpViewDemotion @CompanyCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).ToList();
                //return Json(data);
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                comp = comp == "" ? comp = "All Dealer" : ctx.GnMstDealerMappings.FirstOrDefault(x => x.DealerCode == comp).DealerName;
                package = GEDemotionExcel(package, data, comp, DateFrom, DateTo);

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

        private static ExcelPackage GEDemotionExcel(ExcelPackage package, List<DemotionViewModel> data, string comp, string DateFrom, string DateTo)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,
                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1500 = GetTrueColWidth(15.00),
                w1800 = GetTrueColWidth(18.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Demotion Data";
            sheet.Cells[x].Style.Font.Size = 20;

            x = GetCell(rTitle + 2, cStart);
            sheet.Cells[x].Value = "Dealer";
            sheet.Cells[x].Style.Font.Bold = true;
            x = GetCell(rTitle + 2, cStart + 1);
            sheet.Cells[x].Value = comp;

            x = GetCell(rTitle + 3, cStart);
            sheet.Cells[x].Value = "Period";
            sheet.Cells[x].Style.Font.Bold = true;
            x = GetCell(rTitle + 3, cStart + 1);
            sheet.Cells[x].Value = DateTime.Parse(DateFrom).ToString("dd MMM yyyy");
            x = GetCell(rTitle + 3, cStart + 2);
            sheet.Cells[x].Value = "to";
            sheet.Cells[x].Style.Font.Bold = true;
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(rTitle + 3, cStart + 3);
            sheet.Cells[x].Value = DateTime.Parse(DateTo).ToString("dd MMM yyyy");

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w3000, Value = "Outlet" },
                new ExcelCellItem { Column = 2, Width = w1500, Value = "Branch Manager/to/Sales Head".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 3, Width = w1500, Value = "Sales Head/to/Sales Person".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 4, Width = w1500, Value = "Platinum/to/Gold".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 5, Width = w1500, Value = "Gold/to/Silver".Replace("/", System.Environment.NewLine) },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Value = header.Value;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.Font.Bold = true;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].Outlet },
                    new ExcelCellItem { Column = 2, Value = data[i].SH },
                    new ExcelCellItem { Column = 3, Value = data[i].SC },
                    new ExcelCellItem { Column = 4, Value = data[i].Gold },
                    new ExcelCellItem { Column = 5, Value = data[i].Silver },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            //TABLE FOOTER
            var rFooter = rDataStart + data.Count();

            var footers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Value = "TOTAL" },
                new ExcelCellItem { Column = 2, Value = data.Sum(o => o.SH) },
                new ExcelCellItem { Column = 3, Value = data.Sum(o => o.SC) },
                new ExcelCellItem { Column = 4, Value = data.Sum(o => o.Gold) },
                new ExcelCellItem { Column = 5, Value = data.Sum(o => o.Silver) },
            };

            foreach (var footer in footers)
            {
                x = GetCell(rFooter, footer.Column);
                sheet.Cells[x].Value = footer.Value;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            x = GetCell(rFooter, 1);
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            return package;
        }

        public class DemotionViewModel
        {
            public string OutletCode { get; set; }
            public string Outlet { get; set; }
            public int? SH { get; set; }
            public int? SC { get; set; }
            public int? Gold { get; set; }
            public int? Silver { get; set; }
        }

        public class DemotionDetailViewModel : PromotionDetailViewModel { }

        #endregion
    }
}