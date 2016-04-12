using OfficeOpenXml;
using OfficeOpenXml.Style;
using SimDms.Common;
using EP = SimDms.Common.EPPlusHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Drawing;
using System.Dynamic;
using SimDms.Absence.Models;


namespace SimDms.Absence.Controllers.Api
{
    public class TrainingDashboardController : BaseController
    {
        #region -- Combo Boxes --
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

        public JsonResult Outlets()
        {
            var outlets = from a in ctx.GnMstDealerMappings
                          join b in ctx.GnMstDealerOutletMappings
                          on new { a.DealerCode } equals new { b.DealerCode }
                          where a.DealerCode == CompanyCode
                          && a.isActive.Value
                          select new
                          {
                              value = b.OutletCode,
                              text = b.OutletAbbreviation
                          };
            return Json(outlets);
        }
        #endregion

        #region -- Excel Functions --
        public FileContentResult DownloadExcelFile(string key)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=TrainingDashboard.xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        public JsonResult QryToGrid(int startYear, int startMonth, int endYear, int endMonth, string filter)
        {
            var message = "";
            var calendar = new System.Globalization.GregorianCalendar();

            try
            {
                var startDate = new DateTime(startYear, startMonth, 1);
                var endDate = new DateTime(endYear, endMonth, calendar.GetDaysInMonth(endYear, endMonth));

                var query = @"exec usprpt_mpTrainingDashboardDealer @DealerCode, @Start, @End, @Outlet, @Filter";
                var parameters = new[]
                {
                    new SqlParameter("@DealerCode", CompanyCode),
                    new SqlParameter("@Start", startDate),
                    new SqlParameter("@End", endDate),
                    new SqlParameter("@Outlet", BranchCode),
                    new SqlParameter("@Filter", filter)
                };

                var result = ctx.MultiResultSetSqlQuery(query, parameters);
                var data = new List<TrainingDashboardCount>();
                var total = new List<TrainingDashboardTotal>();
                var detail = new List<TrainingDashboardDetail>();

                if (filter == "" )
                {
                    data = result.ResultSetFor<TrainingDashboardCount>().ToList();
                    total = result.ResultSetFor<TrainingDashboardTotal>().ToList();
                }
                else
                {
                    detail = result.ResultSetFor<TrainingDashboardDetail>().ToList();
                }

                //var all = Combine(data, total);
               
                return Json(new { message = message, data = data, total = total, detail = detail });
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message });
            }
        }

        
        public JsonResult Query(int startYear, int startMonth, int endYear, int endMonth)
        {
            var message = "";
            var calendar = new System.Globalization.GregorianCalendar();

            try
            {
                var startDate = new DateTime(startYear, startMonth, 1);
                var endDate = new DateTime(endYear, endMonth, calendar.GetDaysInMonth(endYear, endMonth));

                var query = @"exec usprpt_mpTrainingDashboardDealer @DealerCode, @Start, @End, @OutletCode, @FilterColumn";

                var parameters = new[]
                {
                    new SqlParameter("@DealerCode", CompanyCode),
                    new SqlParameter("@Start", startDate),
                    new SqlParameter("@End", endDate),
                    new SqlParameter("@OutletCode", BranchCode),
                    new SqlParameter("@FilterColumn", "")
                };

                var result = ctx.MultiResultSetSqlQuery(query, parameters);

                var model = new TrainingDashboardModel
                {
                    DealerCode = CompanyCode,
                    OutletCode = BranchCode,
                    Start = startDate,
                    End = endDate
                };

                var package = new ExcelPackage();
                package = GenerateExcel(package, model, result);
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

        private static ExcelPackage GenerateExcel(ExcelPackage package, TrainingDashboardModel model, MultiResultSetReader result)
        {
            var ctx = new DataContext();
            var sheet = package.Workbook.Worksheets.Add("TrainingDashboard");
            var z = sheet.Cells[1, 1];
            
            var dealerName = "ALL";

            if (model.OutletCode != "" && model.OutletCode != null)
            {
                dealerName = (from a in ctx.GnMstDealerMappings
                              where a.DealerCode == model.DealerCode
                              select a.DealerName).FirstOrDefault();

                if (dealerName == null) throw new Exception("Kode dealer salah / tidak ditemukan");
            }

            var data = result.ResultSetFor<TrainingDashboardCount>().ToList();
            var total = result.ResultSetFor<TrainingDashboardTotal>().ToList();

            #region -- Constants --
            const int
                rTitle   = 1,
                rDealer  = 2,
                rPeriod  = 3,
                rHeader1 = 5,
                rHeader2 = 6,
                rData    = 7,
                
                cStart   = 1,
                cOutlet  = 1,
                cBMJml   = 2,
                cBMT     = 3,
                cBMNT    = 4,
                cSHJml   = 5,
                cSHT     = 6,
                cSHNT    = 7,
                cS4Jml   = 8,
                cS4T     = 9,
                cS4NT    = 10,
                cS3Jml   = 11,
                cS3T     = 12,
                cS3NT    = 13,
                cS2Jml   = 14,
                cS2T     = 15,
                cS2NT    = 16,
                cS1Jml   = 17,
                cS1T     = 18,
                cS1NT    = 19,
                cEnd     = 19;

            double
                wOutlet  = EP.GetTrueColWidth(22.43),
                wCount   = EP.GetTrueColWidth(6.86),
                hTotal   = EP.GetTrueColWidth(27.75);

            const string 
                fDate    = "dd/MMM/yyyy",
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";
            #endregion

            sheet.Column(cOutlet).Width = wOutlet;

            #region -- Title --
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cEnd);
            z.Value = "TRAINING DASHBOARD";
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetCell(rDealer, cStart);
            z.Value = "DEALER NAME";
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rDealer, cBMJml, rDealer, cBMNT);
            z.Value = dealerName;
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rPeriod, cStart);
            z.Value = "CUT OFF PERIODE";
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rPeriod, cBMJml, rPeriod, cBMT);
            z.Value = model.Start;
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Style.Numberformat.Format = fDate;

            z.Address = EP.GetCell(rPeriod, cBMNT);
            z.Value = "to";
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetRange(rPeriod, cSHJml, rPeriod, cSHT);
            z.Value = model.End;
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Style.Numberformat.Format = fDate;
            #endregion

            #region -- Header --
            z.Address = EP.GetRange(rHeader1, cStart, rHeader2, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
            z.Style.Font.Bold = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            
            z.Address = EP.GetRange(rHeader1, cOutlet, rHeader2, cOutlet);
            z.Value = "OUTLET";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cBMJml, rHeader1, cBMNT);
            z.Value = "BRANCH MANAGER";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cBMJml);
            z.Value = "JML";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cBMT);
            z.Value = "T";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cBMNT);
            z.Value = "NT";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cSHJml, rHeader1, cSHNT);
            z.Value = "SALES HEAD";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cSHJml);
            z.Value = "JML";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cSHT);
            z.Value = "T";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cSHNT);
            z.Value = "NT";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cS4Jml, rHeader1, cS4NT);
            z.Value = "SALES PLATINUM";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS4Jml);
            z.Value = "JML";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS4T);
            z.Value = "T";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS4NT);
            z.Value = "NT";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cS3Jml, rHeader1, cS3NT);
            z.Value = "SALES GOLD";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS3Jml);
            z.Value = "JML";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS3T);
            z.Value = "T";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS3NT);
            z.Value = "NT";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cS2Jml, rHeader1, cS2NT);
            z.Value = "SALES SILVER";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS2Jml);
            z.Value = "JML";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS2T);
            z.Value = "T";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS2NT);
            z.Value = "NT";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cS1Jml, rHeader1, cS1NT);
            z.Value = "SALES TRAINEE";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS1Jml);
            z.Value = "JML";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS1T);
            z.Value = "T";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cS1NT);
            z.Value = "NT";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            #endregion

            #region -- Data --
            if (data.Count == 0) return package;
            for (int i = 0; i < data.Count; i++)
            {
                var row = rData + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cOutlet, Value = data[i].OutletAbbr, Format = fDate },
                    new ExcelCellItem { Column = cBMJml, Value = data[i].BM_jml },
                    new ExcelCellItem { Column = cBMT  , Value = data[i].BM_t },
                    new ExcelCellItem { Column = cBMNT , Value = data[i].BM_nt },
                    new ExcelCellItem { Column = cSHJml, Value = data[i].SH_jml },
                    new ExcelCellItem { Column = cSHT  , Value = data[i].SH_t },
                    new ExcelCellItem { Column = cSHNT , Value = data[i].SH_nt },
                    new ExcelCellItem { Column = cS4Jml, Value = data[i].S4_jml },
                    new ExcelCellItem { Column = cS4T  , Value = data[i].S4_t },
                    new ExcelCellItem { Column = cS4NT , Value = data[i].S4_nt },
                    new ExcelCellItem { Column = cS3Jml, Value = data[i].S3_jml },
                    new ExcelCellItem { Column = cS3T  , Value = data[i].S3_t },
                    new ExcelCellItem { Column = cS3NT , Value = data[i].S3_nt },
                    new ExcelCellItem { Column = cS2Jml, Value = data[i].S2_jml },
                    new ExcelCellItem { Column = cS2T  , Value = data[i].S2_t },
                    new ExcelCellItem { Column = cS2NT , Value = data[i].S2_nt },
                    new ExcelCellItem { Column = cS1Jml, Value = data[i].S1_jml },
                    new ExcelCellItem { Column = cS1T  , Value = data[i].S1_t },
                    new ExcelCellItem { Column = cS1NT , Value = data[i].S1_nt }
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

            #region -- Total --
            var rTotal = data.Count + rData;
            z.Address = EP.GetRange(rTotal, cStart, rTotal, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 241, 221));
            z.Style.Font.Bold = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var sums = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = cOutlet, Value = "TOTAL" },
                new ExcelCellItem { Column = cBMJml, Value = total[0].ΣBM_jml },
                new ExcelCellItem { Column = cBMT  , Value = total[0].ΣBM_t },
                new ExcelCellItem { Column = cBMNT , Value = total[0].ΣBM_nt },
                new ExcelCellItem { Column = cSHJml, Value = total[0].ΣSH_jml },
                new ExcelCellItem { Column = cSHT  , Value = total[0].ΣSH_t },
                new ExcelCellItem { Column = cSHNT , Value = total[0].ΣSH_nt },
                new ExcelCellItem { Column = cS4Jml, Value = total[0].ΣS4_jml },
                new ExcelCellItem { Column = cS4T  , Value = total[0].ΣS4_t },
                new ExcelCellItem { Column = cS4NT , Value = total[0].ΣS4_nt },
                new ExcelCellItem { Column = cS3Jml, Value = total[0].ΣS3_jml },
                new ExcelCellItem { Column = cS3T  , Value = total[0].ΣS3_t },
                new ExcelCellItem { Column = cS3NT , Value = total[0].ΣS3_nt },
                new ExcelCellItem { Column = cS2Jml, Value = total[0].ΣS2_jml },
                new ExcelCellItem { Column = cS2T  , Value = total[0].ΣS2_t },
                new ExcelCellItem { Column = cS2NT , Value = total[0].ΣS2_nt },
                new ExcelCellItem { Column = cS1Jml, Value = total[0].ΣS1_jml },
                new ExcelCellItem { Column = cS1T  , Value = total[0].ΣS1_t },
                new ExcelCellItem { Column = cS1NT , Value = total[0].ΣS1_nt }
            };

            foreach (var sum in sums)
            {
                z.Address = EP.GetCell(rTotal, sum.Column);
                z.Value = sum.Value;
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                z.Style.Numberformat.Format = sum.Format != null ? sum.Format : fCustom;
            }

            sheet.Row(rTotal).Height = hTotal;

            #endregion

            return package;
        }

        #endregion
        
        #region -- Models --
        private class TrainingDashboardModel
        {
            public string DealerCode { get; set; }
            public string OutletCode { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        private class TrainingDashboardCount
        {
            public String OutletCode { get; set; }
            public String OutletAbbr { get; set; }
            public Int32? BM_jml { get; set; }
            public Int32? BM_t { get; set; }
            public Int32? BM_nt { get; set; }
            public Int32? SH_jml { get; set; }
            public Int32? SH_t { get; set; }
            public Int32? SH_nt { get; set; }
            public Int32? S1_jml { get; set; }
            public Int32? S1_t { get; set; }
            public Int32? S1_nt { get; set; }
            public Int32? S2_jml { get; set; }
            public Int32? S2_t { get; set; }
            public Int32? S2_nt { get; set; }
            public Int32? S3_jml { get; set; }
            public Int32? S3_t { get; set; }
            public Int32? S3_nt { get; set; }
            public Int32? S4_jml { get; set; }
            public Int32? S4_t { get; set; }
            public Int32? S4_nt { get; set; }
        }

        private class TrainingDashboardTotal
        {
            public Int32? ΣBM_jml { get; set; }
            public Int32? ΣBM_t { get; set; }
            public Int32? ΣBM_nt { get; set; }
            public Int32? ΣSH_jml { get; set; }
            public Int32? ΣSH_t { get; set; }
            public Int32? ΣSH_nt { get; set; }
            public Int32? ΣS1_jml { get; set; }
            public Int32? ΣS1_t { get; set; }
            public Int32? ΣS1_nt { get; set; }
            public Int32? ΣS2_jml { get; set; }
            public Int32? ΣS2_t { get; set; }
            public Int32? ΣS2_nt { get; set; }
            public Int32? ΣS3_jml { get; set; }
            public Int32? ΣS3_t { get; set; }
            public Int32? ΣS3_nt { get; set; }
            public Int32? ΣS4_jml { get; set; }
            public Int32? ΣS4_t { get; set; }
            public Int32? ΣS4_nt { get; set; }
        }

        private class TrainingDashboardDetail
        {
            public string BranchCode { get; set; }
            public string OutletAbbr { get; set; }
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string Position { get; set; }
            public string PosName { get; set; }
            public string Grade { get; set; }
            public string GradeName { get; set; }
            public DateTime JoinDate { get; set; }
        }
        #endregion

        static dynamic Combine(dynamic item1, dynamic item2)
        {
            var dictionary1 = (IDictionary<string, object>)item1;
            var dictionary2 = (IDictionary<string, object>)item2;
            var result = new ExpandoObject();
            var d = result as IDictionary<string, object>; //work with the Expando as a Dictionary

            foreach (var pair in dictionary1.Concat(dictionary2))
            {
                d[pair.Key] = pair.Value;
            }

            return result;
        }
        
    }
}