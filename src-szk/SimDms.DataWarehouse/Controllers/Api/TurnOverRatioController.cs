using OfficeOpenXml;
using OfficeOpenXml.Style;
using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EP = SimDms.DataWarehouse.Helpers.EPPlusHelper;
using SimDms.DataWarehouse.Helpers;

namespace SimDms.DataWarehouse.Controllers.Api
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

        public JsonResult Dealers(string area)
        {
            if (area != "")
            {
                var groupCode = int.Parse(area);
                var dealers = ctx.GnMstDealerMappings.Where(x => x.GroupNo == groupCode).Select(x => new
                {
                    value = x.DealerCode,
                    text = "(" + x.DealerAbbreviation + ") - " + x.DealerName
                });
                return Json(dealers);
            }
            return Json(new { });
        }

        public JsonResult Outlets(string area, string dealer)
        {
            if (area != "" && area != null && dealer != "" && dealer != null)
            {
                var groupCode = int.Parse(area);
                var outlets = from a in ctx.GnMstDealerMappings
                              join b in ctx.GnMstDealerOutletMappings
                              on new { a.DealerCode } equals new { b.DealerCode }
                              where a.GroupNo == groupCode
                              && a.DealerCode == dealer
                              select new
                              {
                                  value = b.OutletCode,
                                  text = b.OutletAbbreviation
                              };
                return Json(outlets);
            }
            return Json(new { });
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

        public JsonResult Query(string area, string dealer, string outlet, int startYear, int startMonth, int endYear, int endMonth, string position)
        {
            var message = "";
            var calendar = new System.Globalization.GregorianCalendar();

            try
            {
                var startDate = new DateTime(startYear, startMonth, calendar.GetDaysInMonth(startYear, startMonth));
                var endDate = new DateTime(endYear, endMonth, calendar.GetDaysInMonth(endYear, endMonth));

                //var query = @"exec usprpt_abInqTurnOverRatio @p0, @p1, @p2, @p3, @p4";
                //var result = ctx.Database.SqlQuery<TurnOverRatio>(query, dealer, outlet, startDate, endDate, position)
                //    .OrderBy(x => x.DealerAbbreviation).ToList();
                var query = @"exec usprpt_abInqTurnOverRatio_New @p0, @p1, @p2, @p3, @p4, @p5";
                var result = ctx.Database.SqlQuery<TurnOverRatio>(query, area, dealer, outlet, startDate, endDate, position)
                    .OrderBy(x => x.DealerAbbreviation).ToList();
                
                var model = new RatioModel
                {
                    Dealer = dealer,
                    Outlet = outlet,
                    Position = position,
                    Start = startDate,
                    End = endDate
                };

                var book = GenerateExcel(model, result);
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

        private static ExcelPackage GenerateExcel(RatioModel model, List<TurnOverRatio> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
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

            var package = new ExcelPackage();

            var sheet = package.Workbook.Worksheets.Add("book1");

            #region -- Title & Header --
            //TITLE
            z = EP.GetRange(rTitle, cStart, rTitle, cTitleEnd);
            sheet.Cells[z].Merge = true;

            z = EP.GetRange(rTitle, cStart, rHdrSecond, cTrainee2);

            x = EP.GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Turn Over Ratio Dealer";
            sheet.Cells[x].Style.Font.Size = 20;

            x = EP.GetCell(rPeriod, cStart);
            sheet.Cells[x].Value = "Periode :";

            z = EP.GetRange(rPeriod, cTitleVal, rPeriod, cTitleEnd);
            sheet.Cells[z].Merge = true;
            x = EP.GetCell(rPeriod, cTitleVal);
            sheet.Cells[x].Value = model.Start.ToString("MMM yyyy") + " s/d " + model.End.ToString("MMM yyyy");

            x = EP.GetCell(rSalesForce, cStart);
            sheet.Cells[x].Value = "Sales Force :";

            x = EP.GetCell(rSalesForce, cTitleVal);
            var position = ctx.GnMstPositions.FirstOrDefault(a => a.CompanyCode == model.Dealer
                && a.DeptCode == "SALES" && a.PosCode == model.Position);
            var posName = position != null ? position.PosName : "ALL";
            sheet.Cells[x].Value = posName;

            //TABLE HEADER
            z = EP.GetRange(rHdrFirst, cStart, rHdrSecond, cTrainee2);
            sheet.Cells[z].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[z].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
            sheet.Cells[z].Style.WrapText = true;
            sheet.Cells[z].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[z].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Row(rHdrSecond).CustomHeight = true;
            sheet.Row(rHdrSecond).Height = 60.00;

            z = EP.GetRange(rHdrFirst, cStart, rHdrSecond, cStart);
            sheet.Cells[z].Merge = true;
            x = EP.GetCell(rHdrFirst, cStart);
            sheet.Cells[x].Value = "Dealer";
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cStart).Width = EP.GetTrueColWidth(13.57);

            z = EP.GetRange(rHdrFirst, cOutletName, rHdrSecond, cOutletName);
            sheet.Cells[z].Merge = true;
            x = EP.GetCell(rHdrFirst, cOutletName);
            sheet.Cells[x].Value = "Outlet";
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[z].AutoFitColumns();
            sheet.Column(cOutletName).Width = EP.GetTrueColWidth(33.57);

            z = EP.GetRange(rHdrFirst, cRatio, rHdrSecond, cRatio);
            sheet.Cells[z].Merge = true;
            x = EP.GetCell(rHdrFirst, cRatio);
            sheet.Cells[z].Value = "Turn Over Ratio";
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cRatio).Width = EP.GetTrueColWidth(10.14);

            // START PERIOD
            z = EP.GetRange(rHdrFirst, cEmpCount1, rHdrFirst, cTrainee1);
            sheet.Cells[z].Merge = true;
            x = EP.GetCell(rHdrFirst, cEmpCount1);
            sheet.Cells[x].Value = "Periode Awal";
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Medium);

            x = EP.GetCell(rHdrSecond, cEmpCount1);
            sheet.Cells[x].Value = "Jumlah Wiraniaga";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cEmpCount1).Width = EP.GetTrueColWidth(cWidth);

            x = EP.GetCell(rHdrSecond, cEmpIn);
            sheet.Cells[x].Value = "Wiraniaga In";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cEmpIn).Width = EP.GetTrueColWidth(cWidth);

            x = EP.GetCell(rHdrSecond, cPlatinum1);
            sheet.Cells[x].Value = "Platinum";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cPlatinum1).Width = EP.GetTrueColWidth(cWidth);

            x = EP.GetCell(rHdrSecond, cGold1);
            sheet.Cells[x].Value = "Gold";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cGold1).Width = EP.GetTrueColWidth(cWidth);

            x = EP.GetCell(rHdrSecond, cSilver1);
            sheet.Cells[x].Value = "Silver";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cSilver1).Width = EP.GetTrueColWidth(cWidth);

            x = EP.GetCell(rHdrSecond, cTrainee1);
            sheet.Cells[x].Value = "Trainee";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cTrainee1).Width = EP.GetTrueColWidth(cWidth);

            //END PERIOD
            z = EP.GetRange(rHdrFirst, cLoyal, rHdrFirst, cTrainee2);
            sheet.Cells[z].Merge = true;
            x = EP.GetCell(rHdrFirst, cLoyal);
            sheet.Cells[x].Value = "Periode Akhir";
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Medium);

            x = EP.GetCell(rHdrSecond, cLoyal);
            sheet.Cells[x].Value = "Wiraniaga Loyal dari Periode Awal";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cLoyal).Width = EP.GetTrueColWidth(cWidth);

            x = EP.GetCell(rHdrSecond, cEmpCount2);
            sheet.Cells[x].Value = "Jumlah Wiraniaga";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cEmpCount2).Width = EP.GetTrueColWidth(cWidth);

            x = EP.GetCell(rHdrSecond, cEmpOut);
            sheet.Cells[x].Value = "Wiraniaga Out";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cEmpOut).Width = EP.GetTrueColWidth(cWidth);

            x = EP.GetCell(rHdrSecond, cPlatinum2);
            sheet.Cells[x].Value = "Platinum";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cPlatinum2).Width = EP.GetTrueColWidth(cWidth);

            x = EP.GetCell(rHdrSecond, cGold2);
            sheet.Cells[x].Value = "Gold";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cGold2).Width = EP.GetTrueColWidth(cWidth);

            x = EP.GetCell(rHdrSecond, cSilver2);
            sheet.Cells[x].Value = "Silver";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cSilver2).Width = EP.GetTrueColWidth(cWidth);

            x = EP.GetCell(rHdrSecond, cTrainee2);
            sheet.Cells[x].Value = "Trainee";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Column(cSilver2).Width = EP.GetTrueColWidth(cWidth);

            #endregion

            if (data.Count() == 0) return package;

            #region -- Data --
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;

                x = EP.GetCell(row, cStart);
                sheet.Cells[x].Value = data[i].DealerAbbreviation;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cOutletName);
                sheet.Cells[x].Value = data[i].OutletAbbreviation;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cRatio);
                sheet.Cells[x].Value = data[i].Ratio;
                sheet.Cells[x].Style.Numberformat.Format = "0.00 %";
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cEmpCount1);
                sheet.Cells[x].Value = data[i].StartEmployeeCount;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cEmpIn);
                sheet.Cells[x].Value = data[i].EmployeeIn;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cPlatinum1);
                sheet.Cells[x].Value = data[i].StartPlatinum;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cGold1);
                sheet.Cells[x].Value = data[i].StartGold;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cSilver1);
                sheet.Cells[x].Value = data[i].StartSilver;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cTrainee1);
                sheet.Cells[x].Value = data[i].StartTrainee;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cLoyal);
                sheet.Cells[x].Value = data[i].LoyalCount;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cEmpCount2);
                sheet.Cells[x].Value = data[i].EndEmployeeCount;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cEmpOut);
                sheet.Cells[x].Value = data[i].EmployeeOut;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cPlatinum2);
                sheet.Cells[x].Value = data[i].EndPlatinum;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cGold2);
                sheet.Cells[x].Value = data[i].EndGold;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cSilver2);
                sheet.Cells[x].Value = data[i].EndSilver;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                x = EP.GetCell(row, cTrainee2);
                sheet.Cells[x].Value = data[i].EndTrainee;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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