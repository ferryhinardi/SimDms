using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Absence.Models;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using SimDms.Common;
using System.Drawing;
using EP = SimDms.Common.EPPlusHelper;

namespace SimDms.Absence.Controllers.Api
{
    public class TurnOverRatioController : BaseController
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

        public JsonResult Areas()
        {
            var areas = ctx.GnMstDealerMappings.Select(x => new
            {
                value = x.GroupNo,
                text = x.Area
            }).Distinct();
            return Json(areas);
        }

        public JsonResult Dealers(int area)
        {
            var dealers = ctx.GnMstDealerMappings.Where(x => x.GroupNo == area).Select(x => new 
            { 
                value = x.DealerCode,
                text = "(" + x.DealerAbbreviation + ") - " + x.DealerName
            });
            return Json(dealers);
        }

        public JsonResult Outlets(int area, string dealer)
        {
            var outlets = from a in ctx.GnMstDealerMappings
                          join b in ctx.GnMstDealerOutletMappings
                          on new { a.DealerCode } equals new { b.DealerCode }
                          where a.GroupNo == area
                          && a.DealerCode == dealer
                          select new
                          {
                              value = b.OutletCode,
                              text = b.OutletAbbreviation
                          };
            return Json(outlets);
        }

        public JsonResult SalesForces()
        {
            var forces = new List<object>
            {
                new { value = "BM", text = "Branch Manager" },
                new { value = "SH", text = "Sales Head" },
                new { value = "S", text = "Salesman" }
            };
            return Json(forces);
        }

        public JsonResult Query(string dealer, string outlet, int startYear, int startMonth, int endYear, int endMonth, string position)
        {
            var message = "";
            var calendar = new System.Globalization.GregorianCalendar();

            try
            {
                var startDate = new DateTime(startYear, startMonth, calendar.GetDaysInMonth(startYear, startMonth));
                var endDate = new DateTime(endYear, endMonth, calendar.GetDaysInMonth(endYear, endMonth));

                var query = @"exec usprpt_abInqTurnOverRatio @p0, @p1, @p2, @p3, @p4";
                var result = ctx.Database.SqlQuery<TurnOverRatio>(query, dealer, outlet, startDate, endDate, position).ToList();
                var model = new RatioModel
                {
                    Dealer = dealer,
                    Outlet = outlet,
                    Position = position,
                    Start = startDate,
                    End = endDate
                };

                var package = new ExcelPackage();
                var book = GenerateExcel(package, model, result);
                var content = book.GetAsByteArray();
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

        public FileContentResult DownloadExcelFile(string key)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=TurnOverRatio.xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        private static ExcelPackage GenerateExcel(ExcelPackage package, RatioModel model, List<TurnOverRatio> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            var sheet = package.Workbook.Worksheets.Add("book1");
            var z = sheet.Cells[1, 1];

            const int 
                rTitle = 1,
                rPeriod = 2,
                rSalesForce = 3,
                rHdrFirst = 6,
                rHdrSecond = 7,
                rDataStart = 8,

                cStart = 1,
                cTitleEnd = 3,
                cTitleVal = 2,
                cOutletName = 2,
                cRatio = 3,
                cEmpCount1 = 4,
                cEmpIn = 5,
                cPlatinum1 = 6,
                cGold1 = 7,
                cSilver1 = 8,
                cTrainee1 = 9,
                cLoyal = 10,
                cEmpCount2 = 11,
                cEmpOut = 12,
                cPlatinum2 = 13,
                cGold2 = 14,
                cSilver2 = 15,
                cTrainee2 = 16;

            const double cWidth = 10.43;
            #endregion

            #region -- Title --
            //TITLE
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cTitleEnd);
            z.Merge = true;

            z.Address = EP.GetCell(rTitle, cStart);
            z.Value = "Report Turn Over Ratio All Dealer";
            z.Style.Font.Size = 20;

            z.Address = EP.GetCell(rPeriod, cStart);
            z.Value = "Periode :";

            z.Address = EP.GetRange(rPeriod, cTitleVal, rPeriod, cTitleEnd);
            z.Merge = true;
            z.Address = EP.GetCell(rPeriod, cTitleVal);
            z.Value = model.Start.ToString("MMM yyyy") + " s/d " + model.End.ToString("MMM yyyy");

            z.Address = EP.GetCell(rSalesForce, cStart);
            z.Value = "Sales Force :";

            z.Address = EP.GetCell(rSalesForce, cTitleVal);
            var position = ctx.GnMstPositions.FirstOrDefault(a => a.CompanyCode == model.Dealer
                && a.DeptCode == "SALES" && a.PosCode == model.Position);
            var posName = position != null ? position.PosName : "ALL";
            z.Value = posName;
            #endregion

            #region -- Header --
            z.Address = EP.GetRange(rHdrFirst, cStart, rHdrSecond, cTrainee2);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
            z.Style.WrapText = true;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Row(rHdrSecond).CustomHeight = true;
            sheet.Row(rHdrSecond).Height = 60.00;

            z.Address = EP.GetRange(rHdrFirst, cStart, rHdrSecond, cStart);
            z.Merge = true;
            
            z.Address = EP.GetCell(rHdrFirst, cStart);
            z.Value = "Dealer";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cStart).Width = EP.GetTrueColWidth(13.57);

            z.Address = EP.GetRange(rHdrFirst, cOutletName, rHdrSecond, cOutletName);
            z.Merge = true;

            z.Address = EP.GetCell(rHdrFirst, cOutletName);
            z.Value = "Outlet";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            z.AutoFitColumns();
            sheet.Column(cOutletName).Width = EP.GetTrueColWidth(33.57);

            z.Address = EP.GetRange(rHdrFirst, cRatio, rHdrSecond, cRatio);
            z.Merge = true;
            
            z.Address = EP.GetCell(rHdrFirst, cRatio);
            z.Value = "Turn Over Ratio";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cRatio).Width = EP.GetTrueColWidth(10.14);

            // START PERIOD
            z.Address = EP.GetRange(rHdrFirst, cEmpCount1, rHdrFirst, cTrainee1);
            z.Merge = true;
            z.Address = EP.GetCell(rHdrFirst, cEmpCount1);
            z.Value = "Periode Awal";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);

            z.Address = EP.GetCell(rHdrSecond, cEmpCount1);
            z.Value = "Jumlah Wiraniaga";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cEmpCount1).Width = EP.GetTrueColWidth(cWidth);

            z.Address = EP.GetCell(rHdrSecond, cEmpIn);
            z.Value = "Wiraniaga In";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cEmpIn).Width = EP.GetTrueColWidth(cWidth);

            z.Address = EP.GetCell(rHdrSecond, cPlatinum1);
            z.Value = "Platinum";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cPlatinum1).Width = EP.GetTrueColWidth(cWidth);

            z.Address = EP.GetCell(rHdrSecond, cGold1);
            z.Value = "Gold";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cGold1).Width = EP.GetTrueColWidth(cWidth);

            z.Address = EP.GetCell(rHdrSecond, cSilver1);
            z.Value = "Silver";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cSilver1).Width = EP.GetTrueColWidth(cWidth);

            z.Address = EP.GetCell(rHdrSecond, cTrainee1);
            z.Value = "Trainee";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cTrainee1).Width = EP.GetTrueColWidth(cWidth);

            //END PERIOD
            z.Address = EP.GetRange(rHdrFirst, cLoyal, rHdrFirst, cTrainee2);
            z.Merge = true;
            z.Address = EP.GetCell(rHdrFirst, cLoyal);
            z.Value = "Periode Akhir";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);

            z.Address = EP.GetCell(rHdrSecond, cLoyal);
            z.Value = "Wiraniaga Loyal dari Periode Awal";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cLoyal).Width = EP.GetTrueColWidth(cWidth);

            z.Address = EP.GetCell(rHdrSecond, cEmpCount2);
            z.Value = "Jumlah Wiraniaga";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cEmpCount2).Width = EP.GetTrueColWidth(cWidth);

            z.Address = EP.GetCell(rHdrSecond, cEmpOut);
            z.Value = "Wiraniaga Out";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cEmpOut).Width = EP.GetTrueColWidth(cWidth);

            z.Address = EP.GetCell(rHdrSecond, cPlatinum2);
            z.Value = "Platinum";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cPlatinum2).Width = EP.GetTrueColWidth(cWidth);

            z.Address = EP.GetCell(rHdrSecond, cGold2);
            z.Value = "Gold";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cGold2).Width = EP.GetTrueColWidth(cWidth);

            z.Address = EP.GetCell(rHdrSecond, cSilver2);
            z.Value = "Silver";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cSilver2).Width = EP.GetTrueColWidth(cWidth);

            z.Address = EP.GetCell(rHdrSecond, cTrainee2);
            z.Value = "Trainee";
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cSilver2).Width = EP.GetTrueColWidth(cWidth);
            
            #endregion

            if (data.Count() == 0) return package;

            #region -- Data --
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var fPercent = "0.00 %";
                var fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cStart, Value = data[i].DealerAbbreviation },
                    new ExcelCellItem { Column = cOutletName, Value = data[i].OutletAbbreviation },
                    new ExcelCellItem { Column = cRatio, Value = data[i].Ratio, Format = fPercent },
                    new ExcelCellItem { Column = cEmpCount1, Value = data[i].StartEmployeeCount },
                    new ExcelCellItem { Column = cEmpIn, Value = data[i].EmployeeIn },
                    new ExcelCellItem { Column = cPlatinum1, Value = data[i].StartPlatinum },
                    new ExcelCellItem { Column = cGold1, Value = data[i].StartGold },
                    new ExcelCellItem { Column = cSilver1, Value = data[i].StartSilver },
                    new ExcelCellItem { Column = cTrainee1, Value = data[i].StartTrainee },
                    new ExcelCellItem { Column = cLoyal, Value = data[i].LoyalCount },
                    new ExcelCellItem { Column = cEmpCount2, Value = data[i].EndEmployeeCount },
                    new ExcelCellItem { Column = cEmpOut, Value = data[i].EmployeeOut },
                    new ExcelCellItem { Column = cPlatinum2, Value = data[i].EndPlatinum },
                    new ExcelCellItem { Column = cGold2, Value = data[i].EndGold },
                    new ExcelCellItem { Column = cSilver2, Value = data[i].EndSilver },
                    new ExcelCellItem { Column = cTrainee2, Value = data[i].EndTrainee }
                };

                foreach(var item in items)
                {
                    z.Address = EP.GetCell(row, item.Column);
                    z.Value = item.Value;
                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                }
            } 
            #endregion

            return package;
        }

        private class RatioModel
        {
            public string Dealer { get; set; }
            public string Outlet { get; set; }
            public string Position { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
    }   
}