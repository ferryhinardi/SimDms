using ClosedXML.Excel;
using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers.Api
{
    public class AOSListController : BaseController
    {
        DateTimeFormatInfo mfi = new DateTimeFormatInfo();

        public JsonResult Default()
        {
            return Json(new
            {
                IsBranch = IsBranch,
                BranchCode = BranchCode,
            });
        }

        public JsonResult Validate(string BranchCode, DateTime StartDate, DateTime EndDate)
        {
            //var data = ctx.SpTrnPSuggorAOSs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
            //  && (a.ProcessDate.Date >= StartDate && a.ProcessDate <= EndDate));

            var query = "SELECT * FROM spTrnPSuggorAOS where CompanyCode = @p0 AND BranchCode = @p1 AND CONVERT(date,ProcessDate) BETWEEN @p2 AND @p3";
            object[] parameters = { CompanyCode, BranchCode, StartDate, EndDate };
            var data = ctx.Database.SqlQuery<SpTrnPSuggorAOS>(query, parameters);

            return Json(new { success = data.Count() > 0, message = "Tidak ada data yang ditampilkan" });
        }

        /// <summary>
        /// Generate Excel
        /// </summary>
        /// <param name="BranchCode"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public ActionResult GenerateExcel(string BranchCode, DateTime StartDate, DateTime EndDate)
        {
            var SuggorAOS = new SpTrnPSuggorAOS();

            string fileName = "AOS List";

            //var data = ctx.SpTrnPSuggorAOSs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
            //&& a.ProcessDate >= StartDate && a.ProcessDate <= EndDate);

            var query = "SELECT * FROM spTrnPSuggorAOS where CompanyCode = @p0 AND BranchCode = @p1 AND CONVERT(date,ProcessDate) BETWEEN @p2 AND @p3";
            object[] parameters = { CompanyCode, BranchCode, StartDate, EndDate };
            var data = ctx.Database.SqlQuery<SpTrnPSuggorAOS>(query, parameters).AsQueryable();

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("AOS List");

            // Setting Title
            ws.Cell("A1").Value = "REPORT AOS LIST";
            var Title = ws.Range("A1:C1");
            Title.Merge();
            Title.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            Title.Style.Font.FontSize = 16;
            Title.Style.Font.SetBold(true);

            ws.Cell("A2").Value = "Company : " + CompanyCode + " - " + ctx.DealerMapping.FirstOrDefault(a => a.DealerCode == CompanyCode).DealerAbbreviation;
            var Company = ws.Range("A2:C2");
            Company.Merge();
            Company.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

            ws.Cell("A3").Value = "Branch : " + BranchCode + " - " + ctx.GnMstDealerOutletMappings.FirstOrDefault(a => a.OutletCode == BranchCode).OutletAbbreviation;
            var Branch = ws.Range("A3:C3");
            Branch.Merge();
            Branch.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

            ws.Cell("A4").Value = "Periode : " + StartDate.ToString("dd-MMM-yyyy") + " s/d " + EndDate.ToString("dd-MMM-yyy");
            var Periode = ws.Range("A4:C4");
            Periode.Merge();
            Periode.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

            //// Setting Header
            var preHeader = SuggorAOS.GetType().GetProperties();

            StyleHeader(ws, 1, 1, 13.30);
            StyleHeader(ws, 2, 2, 11);
            StyleHeader(ws, 3, 3, 11.15);
            StyleHeader(ws, 4, 4, 16.75);
            StyleHeader(ws, 5, 12, 13);
            StyleHeader(ws, 13, 24, 10);
            StyleHeader(ws, 25, 27, 8);
            StyleHeader(ws, 28, 37, 11.45);
            StyleHeader(ws, 38, 45, 9.15);
            StyleHeader(ws, 46, 48, 10.15);
            StyleHeader(ws, 49, 49, 15);
            StyleHeader(ws, 50, 50, 9.5);
            StyleHeader(ws, 51, 51, 5.71);

            foreach (var h in preHeader.Select((item, index) => new { Index = index +1, Value = item }))
            {
                ws.Cell(6, h.Index).Value = h.Value.Name;
                ws.Cell(6, h.Index).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                ws.Cell(6, h.Index).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(6, h.Index).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Cell(6, h.Index).Style.Fill.SetBackgroundColor(XLColor.FromArgb(75, 172, 198));
            }

            // data
            int startIndex = 7;
            var iItem = 1;


            foreach (var item in data)
            {
                iItem = 1;
                foreach (var d in data.ElementType.GetProperties())
                {
                    var propertyName = d.Name;
                    var propertyValue = item.GetType().GetProperty(propertyName).GetValue(item, null);
                    ws.Cell(startIndex, iItem).Value = propertyValue;
                    ws.Cell(startIndex, iItem).Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
                    ws.Cell(startIndex, iItem).Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);

                    iItem++;
                }

                // format column
                // 3 
                ws.Cell(startIndex, 3).Style.DateFormat.SetFormat("dd-MMM-yyyy");
                //10
                ws.Cell(startIndex, 10).SetDataType(XLCellValues.Text);
                ws.Cell(startIndex, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //11-12
                ws.Cell(startIndex, 11).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* - ??_);_(@_) ";
                ws.Cell(startIndex, 12).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* - ??_);_(@_) ";
                //13-37
                for (int i = 13; i <= 37; i++)
                {
                    ws.Cell(startIndex, i).Style.NumberFormat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* - ??_);_(@_) ";
                }
                //39,40,41,42
                ws.Cell(startIndex, 39).Style.NumberFormat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* - ??_);_(@_) ";
                ws.Cell(startIndex, 40).Style.NumberFormat.Format = "_(* #,##0.000000_);_(* (#,##0.000000);_(* - ??_);_(@_) ";
                ws.Cell(startIndex, 41).Style.NumberFormat.Format = "_(* #,##0.000000_);_(* (#,##0.000000);_(* - ??_);_(@_) ";
                ws.Cell(startIndex, 42).Style.NumberFormat.SetFormat("_(* #,##0.00_);_(* (#,##0.00);_(* - ??_);_(@_) ");

                for (int i = 43; i <= 50; i++)
                {
                    ws.Cell(startIndex, i).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* - ??_);_(@_) ";
                }

                startIndex++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        private static void StyleHeader(IXLWorksheet ws, int start, int end, double width)
        {
            for(int i = start; i <= end; i++)
            {
                ws.Column(i).Width = width;
            }
        }
    }
}
