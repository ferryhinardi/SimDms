using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.DataWarehouse.Models;
using System.Data.SqlClient;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SimDms.DataWarehouse.Helpers;
using EP = SimDms.DataWarehouse.Helpers.EPPlusHelper;
using System.Drawing;
using System.Globalization;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class ChartController : BaseController
    {
        public JsonResult CsMonitoring(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_CsChartMonitoring",
                param = new List<dynamic>
                {
                    new { key = "Inquiry", value = Request.Params["Inquiry"] },
                    new { key = "DateFrom", value = Request.Params["DateFrom"] },
                    new { key = "DateTo", value = Request.Params["DateTo"] },
                },
                result = "dataset"
            });
            return Json(json.Data);
        }

        public JsonResult CsDataMonitoring(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_CsDataMonitoring",
                param = new List<dynamic>
                {
                    new { key = "DateInit", value = Request.Params["DateInit"] },
                    new { key = "DateReff", value = Request.Params["DateReff"] },
                    new { key = "Interval", value = Request.Params["Interval"] },
                },
                result = "dataset"
            });
            return Json(json.Data);
        }

        public JsonResult CsDataTDaysCallDO(string callback)
        {
            var json = Exec(new
            {
                //query = "uspfn_CsDataTDaysCallDO",
                //param = new List<dynamic>
                //{
                //    new { key = "CompanyCode", value = Request.Params["CompanyCode"] },
                //    new { key = "Year", value = Request.Params["Year"] },
                //    new { key = "Month", value = Request.Params["Month"] },
                //},
                query = "uspfn_CsDataTDaysCallDO_New",
                param = new List<dynamic>
                {
                    new { key = "Area", value = Request.Params["Area"] },
                    new { key = "Company", value = Request.Params["CompanyCode"] },
                    new { key = "Year", value = Request.Params["Year"] },
                    new { key = "Month", value = Request.Params["Month"] },
                },
                result = "dataset"
            });
            return Json(json.Data);
        }

        public JsonResult SvMonitoringByPeriode(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_SvChartMonitoringByPeriode",
                param = new List<dynamic>
                {
                    new { key = "Area", value = Request.Params["Area"] },
                    new { key = "Dealer", value = Request.Params["Dealer"] },
                    new { key = "Outlet", value = Request.Params["Outlet"] },
                    new { key = "Year", value = Request.Params["Year"] },
                    new { key = "Month", value = Request.Params["Month"] },
                },
                result = "dataset"
            });
            return Json(json.Data);
        }

        public JsonResult SvUnitIntakeSummary(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_SvChartUnitIntakeSummary",
                param = new List<dynamic>
                {
                    new { key = "Year", value = Request.Params["Year"] },
                    new { key = "Month", value = Request.Params["Month"] },
                    new { key = "Area", value = Request.Params["Area"] },
                    new { key = "Dealer", value = Request.Params["Dealer"] },
                },
                result = "dataset"
            });
            return Json(json.Data);
        }

        public JsonResult SvRegisterSpk1(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_SvChartRegisterSpk1",
                param = new List<dynamic>
                {
                    new { key = "DateFrom", value = Request.Params["DateFrom"] },
                    new { key = "DateTo", value = Request.Params["DateTo"] },
                    new { key = "Area", value = Request.Params["Area"] },
                    new { key = "Dealer", value = Request.Params["Dealer"] },
                },
                //result = "dataset"
            });
            return Json(json.Data);
        }

        public JsonResult CsReportTDayCall()
        {
            var json = Exec(new
            {
                //query = "uspfn_CsChartTDayCall",
                query = "uspfn_CsChartTDayCall_new",
                param = new List<dynamic>
                {
                    new { key = "GroupArea", value = Request.Params["GroupArea"] },
                    new { key = "Company", value = Request.Params["CompanyCode"] },
                    new { key = "BranchCode", value = Request.Params["BranchCode"] },
                    new { key = "DateFrom", value = Request.Params["DateFrom"] },
                    new { key = "DateTo", value = Request.Params["DateTo"] },
                    new { key = "SelectAll", value = (Request["SelectAll"] == "on" ? 1 : 0) },
                }
            });
            return Json(json.Data);
        }

        public JsonResult CsReportBPKBReminder()
        {
            var json = Exec(new
            {
                query = "uspfn_CsRptBPKBReminder",
                param = new List<dynamic>
                {
                    new { key = "GroupArea", value = Request.Params["GroupArea"] },
                    new { key = "CompanyCode", value = Request.Params["CompanyCode"] },
                    new { key = "BranchCode", value = Request.Params["BranchCode"] },
                    new { key = "DateFrom", value = Request.Params["DateFrom"] },
                    new { key = "DateTo", value = Request.Params["DateTo"] },
                    new { key = "SelectAll", value = (Request["SelectAll"] == "on" ? 1 : 0) },
                }
            });
            return Json(json.Data);
        }

        public JsonResult CustBirthdayReport(string callback)
        {
            var GroupArea = Request["GroupArea"];
            var CompanyCode = Request["CompanyCode"];
            var BranchCode = Request["BranchCode"];
            var PeriodYear = Request["PeriodYear"];
            var ParMonth1 = Request["ParMonth1"];
            var ParMonth2 = Request["ParMonth2"];
            var ParStatus = Request["ParStatus"];
            bool ShowAll = Boolean.Parse(Request["ShowAll"]);
            var dataMonthly = (dynamic)null;
            ctx.Database.CommandTimeout = 3600;
            if (!ShowAll)
                //dataMonthly = ctx.Database.SqlQuery<CustomerBirthdayMonitoring>(string.Format("exec uspfn_GetMonitoringCustBirthday '{0}', '{1}', '{2}', {3}, {4}, {5}, {6}", GroupArea, CompanyCode, BranchCode, PeriodYear, ParMonth1, ParMonth2, ParStatus)).ToList();
                dataMonthly = ctx.Database.SqlQuery<CustomerBirthdayMonitoring>(string.Format("exec uspfn_GetMonitoringCustBirthday_New '{0}', '{1}', '{2}', {3}, {4}, {5}, {6}", GroupArea, CompanyCode, BranchCode, PeriodYear, ParMonth1, ParMonth2, ParStatus)).ToList();
            else
                //dataMonthly = ctx.Database.SqlQuery<CustomerBirthdayMonitoring>(string.Format("exec uspfn_GetMonitoringCustBirthdayShowAll '{0}', '{1}', '{2}', {3}, {4}, {5}, {6}", GroupArea, CompanyCode, BranchCode, PeriodYear, ParMonth1, ParMonth2, ParStatus)).ToList();
                dataMonthly = ctx.Database.SqlQuery<CustomerBirthdayMonitoring>(string.Format("exec uspfn_GetMonitoringCustBirthdayShowAll_New '{0}', '{1}', '{2}', {3}, {4}, {5}, {6}", GroupArea, CompanyCode, BranchCode, PeriodYear, ParMonth1, ParMonth2, ParStatus)).ToList();
            return Json(dataMonthly);
        }

        public JsonResult StnkExtension(string callback)
        {
            var GroupArea = Request["GroupArea"];
            var CompanyCode = Request["CompanyCode"];
            var BranchCode = Request["BranchCode"];
            var DateFrom = Request["DateFrom"];
            var DateTo = Request["DateTo"];
            var SelectAll = Request["SelectAll"] == "on" ? 1 : 0;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsChartStnkExt";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@GroupNo", GroupArea);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("@DateTo", DateTo);
            cmd.Parameters.AddWithValue("@SelectAll", SelectAll);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult doExport()
        {
            var areacode = Request.Params["Area"];
            var cmpCode = Request.Params["CompanyCode"];
            var year = Request.Params["Year"];
            var month = Request.Params["Month"];
            var carea = Request.Params["Area"];
            var company = Request.Params["CompanyText"];
            //var query = @"exec uspfn_CsDataTDaysCallDOExport @CompanyCode, @Year, @Month";
            var query = @"exec uspfn_CsDataTDaysCallDOExport_New @Area,@Company, @Year, @Month";

            //var parameters = new[]
            //    {
            //        new SqlParameter("@CompanyCode", cmpCode),
            //        new SqlParameter("@Year", year),
            //        new SqlParameter("@Month", month)
            //    };

            var parameters = new[]
                {
                    new SqlParameter("@Area", areacode),
                    new SqlParameter("@Company", cmpCode),
                    new SqlParameter("@Year", year),
                    new SqlParameter("@Month", month)
                };

            var result = ctx.MultiResultSetSqlQuery(query, parameters);

            var periode = DateTimeFormatInfo.CurrentInfo.GetMonthName(Convert.ToInt16(month)) + " " + year;
            //var dealer = ctx.Organizations.Where(x => x.CompanyCode == cmpCode).FirstOrDefault().CompanyName;
            var dealer = company;
            var area = ctx.GroupAreas.Where(x => x.GroupNo == carea).FirstOrDefault().AreaDealer;

            var message = "";
            try
            {
                var package = new ExcelPackage();
                package = GenerateExcel(package, periode, dealer, area, result);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception e)
            {
                message = e.Message;
                return Json(new { message = message });
            }
        }

        private static ExcelPackage GenerateExcel(ExcelPackage package, string period, string dealer, string area, MultiResultSetReader result)
        {
            var sheet = package.Workbook.Worksheets.Add("3DaysCall");
            var z = sheet.Cells[1, 1];
            var data = result.ResultSetFor<TdCallModel>().ToList();
            var total = result.ResultSetFor<TdCallModelTotal>().ToList();

            #region -- Constants --
            const int
                rTitle = 1,
                rArea = 2,
                rDealer = 3,
                rHeader1 = 4,
                rData = 5,

                cStart = 1,
                cPeriode = 1,
                cOutlet = 2,
                cDoData = 3,
                cDelivery = 4,
                cTdCallByDO = 5,
                cTdCallByInput = 6,
                cEnd = 6;

            double
                wOutlet = EP.GetTrueColWidth(65),
                wPeriode = EP.GetTrueColWidth(18),
                wCol1 = EP.GetTrueColWidth(10),
                wCol2 = EP.GetTrueColWidth(20);
            //hTotal = EP.GetTrueColWidth();

            const string
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";
            #endregion

            sheet.Column(cOutlet).Width = wOutlet;
            sheet.Column(cPeriode).Width = wPeriode;
            sheet.Column(cDoData).Width = wCol1;
            sheet.Column(cDelivery).Width = wCol1;
            sheet.Column(cTdCallByDO).Width = wCol2;
            sheet.Column(cTdCallByInput).Width = wCol2;

            #region -- Title --
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cEnd);
            z.Value = "Data DO VS 3 Days Call";
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetCell(rArea, cPeriode);
            z.Value = "AREA";
            //z.Merge = true;
            //z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rArea, cOutlet);
            z.Value = ": " + area;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rDealer, cPeriode);
            z.Value = "DEALER";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rDealer, cOutlet);
            z.Value = ": " + dealer;
            z.Style.Font.Bold = true;
            #endregion

            #region -- Header --
            z.Address = EP.GetRange(rHeader1, cStart, rHeader1, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
            z.Style.Font.Bold = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z.Address = EP.GetCell(rHeader1, cPeriode);
            z.Value = "PERIODE";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cOutlet);
            z.Value = "OUTLET";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cDoData);
            z.Value = "DO DATA";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cDelivery);
            z.Value = "DELIVERY";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cTdCallByDO);
            z.Value = "3 DAYS CALL BY DO";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cTdCallByInput);
            z.Value = "3 DAYS CALL BY INPUT";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            #endregion

            #region -- Data --
            if (data.Count == 0) return package;
            for (int i = 0; i < data.Count; i++)
            {
                var row = rData + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cPeriode, Value = period },
                    new ExcelCellItem { Column = cOutlet, Value = data[i].BranchCode + " - " + data[i].BranchName },
                    new ExcelCellItem { Column = cDoData, Value = data[i].DOData },
                    new ExcelCellItem { Column = cDelivery, Value = data[i].DeliveryDate },
                    new ExcelCellItem { Column = cTdCallByDO, Value = data[i].TDaysCallData },
                    new ExcelCellItem { Column = cTdCallByInput, Value = data[i].TDaysCallByInput },
                };

                foreach (var item in items)
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
            //z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z.Address = EP.GetRange(rTotal, cPeriode, rTotal, cOutlet);
            z.Value = "TOTAL";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            var sums = new List<ExcelCellItem>
            {
                //new ExcelCellItem { Column = cPeriode, Value = "TOTAL" },
                //new ExcelCellItem { Column = cOutlet, Value = "" },
                new ExcelCellItem { Column = cDoData, Value = total[0].TotDoData },
                new ExcelCellItem { Column = cDelivery, Value = total[0].TotDeliveryDate },
                new ExcelCellItem { Column = cTdCallByDO, Value = total[0].TotTDaysCallData },
                new ExcelCellItem { Column = cTdCallByInput, Value = total[0].TotTDaysCallByInput },
            };

            foreach (var sum in sums)
            {
                z.Address = EP.GetCell(rTotal, sum.Column);
                z.Value = sum.Value;
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                z.Style.Numberformat.Format = sum.Format != null ? sum.Format : fCustom;
            }

            //sheet.Row(rTotal).Height = hTotal;

            #endregion

            return package;
        }


        public JsonResult doExport2()
        {
            var garea = Request.Params["GroupArea"];
            var cmpCode = Request.Params["CompanyCode"];
            var branchCode = Request.Params["BranchCode"];
            var dateFrom = Request.Params["DateFrom"];
            var dateTo = Request.Params["DateTo"];
            var SelectAll = Request["SelectAll"] == "on" ? 1 : 0;

            //var query = @"exec uspfn_CsChartTDayCallExport @GroupArea, @CompanyCode, @BranchCode, @DateFrom, @DateTo, @SelectAll";
            var query = @"exec uspfn_CsChartTDayCallExport_new @GroupArea, @CompanyCode, @BranchCode, @DateFrom, @DateTo, @SelectAll";

            var parameters = new[]
                {
                    new SqlParameter("@GroupArea", garea),
                    new SqlParameter("@CompanyCode", cmpCode),
                    new SqlParameter("@BranchCode", branchCode),
                    new SqlParameter("@DateFrom", dateFrom),
                    new SqlParameter("@DateTo", dateTo),
                    new SqlParameter("@SelectAll", SelectAll)
                };

            var result = ctx.MultiResultSetSqlQuery(query, parameters);

            //var periode = DateTimeFormatInfo.CurrentInfo.GetMonthName(Convert.ToInt16(month)) + " " + year;
            //var dealer = cmpCode != "" ? ctx.Organizations.Where(x => x.CompanyCode == cmpCode).FirstOrDefault().CompanyName : "All Dealer";
            var dealer = cmpCode;
            var area = garea != "" ? ctx.GroupAreas.Where(x => x.GroupNo == garea).FirstOrDefault().AreaDealer : "All Area";
            var outlet = branchCode != "" ? ctx.OutletInfos.Where(x => x.BranchCode == branchCode).FirstOrDefault().BranchName : "All Outlet";
            var dateFromTo = dateFrom + " s/d " + dateTo;

            var message = "";
            try
            {
                var package = new ExcelPackage();
                package = GenerateExcel2(package, area, dealer, outlet, dateFromTo, result);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception e)
            {
                message = e.Message;
                return Json(new { message = message });
            }
        }

        private static ExcelPackage GenerateExcel2(ExcelPackage package, string area, string dealer, string outlet, string dateFromTo, MultiResultSetReader result)
        {
            var sheet = package.Workbook.Worksheets.Add("Report3DaysCall");
            var z = sheet.Cells[1, 1];
            var data = result.ResultSetFor<ReportTdCallModel>().ToList();
            var total = result.ResultSetFor<ReportTdCallModelTotal>().ToList();

            #region -- Constants --
            const int
                rTitle = 1,
                rArea = 2,
                rDealer = 3,
                rOutlet = 4,
                rDateFrom = 5,
                rHeader1 = 6,
                rData = 7,

                cStart = 1,
                cDealer = 1,
                cOutlet = 2,
                cJumBpk = 3,
                cTdCallByInputCRO = 4,
                cPersentase = 5,
                cEnd = 5;

            double
                wOutlet = EP.GetTrueColWidth(65),
                wDealer = EP.GetTrueColWidth(18),
                wCol1 = EP.GetTrueColWidth(25);

            const string
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";
            #endregion

            sheet.Column(cOutlet).Width = wOutlet;
            sheet.Column(cDealer).Width = wDealer;
            sheet.Column(cJumBpk).Width = wCol1;
            sheet.Column(cTdCallByInputCRO).Width = wCol1;
            sheet.Column(cPersentase).Width = wCol1;

            #region -- Title --
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cEnd);
            z.Value = "Report 3 Days Call";
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetCell(rArea, cDealer);
            z.Value = "AREA";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rArea, cOutlet);
            z.Value = ": " + area;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rDealer, cDealer);
            z.Value = "DEALER";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rDealer, cOutlet);
            z.Value = ": " + dealer;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rOutlet, cDealer);
            z.Value = "OUTLET";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rOutlet, cOutlet);
            z.Value = ": " + outlet;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rDateFrom, cDealer);
            z.Value = "DATE FROM - TO";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rDateFrom, cOutlet);
            z.Value = ": " + dateFromTo;
            z.Style.Font.Bold = true;

            #endregion

            #region -- Header --
            z.Address = EP.GetRange(rHeader1, cStart, rHeader1, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
            z.Style.Font.Bold = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z.Address = EP.GetCell(rHeader1, cDealer);
            z.Value = "DEALER";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cOutlet);
            z.Value = "OUTLET";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cJumBpk);
            z.Value = "JUMLAH BPK";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cTdCallByInputCRO);
            z.Value = "INPUT 3 DAYS BY CRO";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cPersentase);
            z.Value = "PERSENTASE (%)";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            #endregion

            #region -- Data --
            if (data.Count == 0) return package;
            for (int i = 0; i < data.Count; i++)
            {
                var row = rData + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cDealer, Value = data[i].Dealer },
                    new ExcelCellItem { Column = cOutlet, Value = data[i].Outlet },
                    new ExcelCellItem { Column = cJumBpk, Value = data[i].JumBPK },
                    new ExcelCellItem { Column = cTdCallByInputCRO, Value = data[i].InputByCRO },
                    new ExcelCellItem { Column = cPersentase, Value = data[i].Percentation },
                };

                foreach (var item in items)
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
            //z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z.Address = EP.GetRange(rTotal, cDealer, rTotal, cOutlet);
            z.Value = "TOTAL";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            var sums = new List<ExcelCellItem>
            {
                //new ExcelCellItem { Column = cDealer, Value = "TOTAL" },
                //new ExcelCellItem { Column = cOutlet, Value = "" },
                new ExcelCellItem { Column = cJumBpk, Value = total[0].TotJumBPK },
                new ExcelCellItem { Column = cTdCallByInputCRO, Value = total[0].TotInputByCRO },
                new ExcelCellItem { Column = cPersentase, Value = total[0].TotPercentation },
            };

            foreach (var sum in sums)
            {
                z.Address = EP.GetCell(rTotal, sum.Column);
                z.Value = sum.Value;
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                z.Style.Numberformat.Format = sum.Format != null ? sum.Format : fCustom;
            }
            #endregion

            return package;
        }

        public JsonResult doExport3()
        {
            var garea = Request.Params["GroupArea"];
            var cmpCode = Request.Params["CompanyCode"];
            var branchCode = Request.Params["BranchCode"];
            var month1 = Request.Params["ParMonth1"];
            var month2 = Request.Params["ParMonth2"];
            var year = Request.Params["PeriodYear"];
            var status = Request.Params["ParStatus"];
            var companyText = Request.Params["CompanyText"];
            var month1Text = Request.Params["ParMonth1"];
            var month2Text = Request.Params["ParMonth2"];
            bool ShowAll = Boolean.Parse(Request["ShowAll"]);
            var query = "";
            if (!ShowAll)
                //query = @"exec uspfn_GetMonitoringCustBirthdayExport @GroupArea, @CompanyCode, @BranchCode, @ParYear, @ParMonth1, @ParMonth2, @ParStatus";
                query = @"exec uspfn_GetMonitoringCustBirthdayExport_New @GroupArea, @Company, @BranchCode, @ParYear, @ParMonth1, @ParMonth2, @ParStatus";
            else
                //query = @"exec uspfn_GetMonitoringCustBirthdayShowAllExport @GroupArea, @CompanyCode, @BranchCode, @ParYear, @ParMonth1, @ParMonth2, @ParStatus";
                query = @"exec uspfn_GetMonitoringCustBirthdayShowAllexport_New @GroupArea, @Company, @BranchCode, @ParYear, @ParMonth1, @ParMonth2, @ParStatus";

            var parameters = new[]
                {
                    new SqlParameter("@GroupArea", garea),
                    //new SqlParameter("@CompanyCode", cmpCode),
                    new SqlParameter("@Company", cmpCode),
                    new SqlParameter("@BranchCode", branchCode),
                    new SqlParameter("@ParYear", year),
                    new SqlParameter("@ParMonth1", month1),
                    new SqlParameter("@ParMonth2", month2),
                    new SqlParameter("@ParStatus", status)
                };
            ctx.Database.CommandTimeout = 3600;
            var result = ctx.MultiResultSetSqlQuery(query, parameters);

            //var periode = DateTimeFormatInfo.CurrentInfo.GetMonthName(Convert.ToInt16(month)) + " " + year;
            //var dealer = cmpCode != "" ? ctx.Organizations.Where(x => x.CompanyCode == cmpCode).FirstOrDefault().CompanyName : "All Dealer";
            var dealer = companyText;
            var area = garea != "" ? ctx.GroupAreas.Where(x => x.GroupNo == garea).FirstOrDefault().AreaDealer : "All Area";
            var outlet = branchCode != "" ? ctx.OutletInfos.Where(x => x.BranchCode == branchCode).FirstOrDefault().BranchName : "All Outlet";
            //var pmonth1 = DateTimeFormatInfo.CurrentInfo.GetMonthName(Convert.ToInt16(month1));
            //var pmonth2 = DateTimeFormatInfo.CurrentInfo.GetMonthName(Convert.ToInt16(month2));
            var pstatus = status == "0" ? "All Status" : status == "1" ? "Not Inputted" : "Inputted";
            var pperiode = month1Text + " s/d " + month2Text + " " + year;

            var message = "";
            try
            {
                var package = new ExcelPackage();
                package = GenerateExcel3(package, area, dealer, outlet, pperiode, pstatus, result);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception e)
            {
                message = e.Message;
                return Json(new { message = message });
            }
        }

        private static ExcelPackage GenerateExcel3(ExcelPackage package, string area, string dealer, string outlet, string periode, string status, MultiResultSetReader result)
        {
            var sheet = package.Workbook.Worksheets.Add("CustBirthday");
            var z = sheet.Cells[1, 1];
            var data = result.ResultSetFor<ReportCustBirthdayModel>().ToList();
            var total = result.ResultSetFor<ReportCustBirthdayModelTotal>().ToList();

            #region -- Constants --
            const int
                rTitle = 1,
                rArea = 2,
                rDealer = 3,
                rOutlet = 4,
                rStatus = 5,
                rPeriod = 6,
                rHeader1 = 7,
                rData = 8,

                cStart = 1,
                cNo = 1,
                cDealer = 2,
                cBulan = 3,
                cJumCust = 4,
                cInputByCRO = 5,
                cGift = 6,
                cSMS = 7,
                cTelephone = 8,
                cEnd = 8;

            double
                wNo = EP.GetTrueColWidth(10),
                wDealer = EP.GetTrueColWidth(25),
                wBulan = EP.GetTrueColWidth(20),
                wCol1 = EP.GetTrueColWidth(18);

            const string
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";
            #endregion

            sheet.Column(cNo).Width = wNo;
            sheet.Column(cDealer).Width = wDealer;
            sheet.Column(cBulan).Width = wBulan;
            sheet.Column(cJumCust).Width = wCol1;
            sheet.Column(cInputByCRO).Width = wCol1;
            sheet.Column(cGift).Width = wCol1;
            sheet.Column(cSMS).Width = wCol1;
            sheet.Column(cTelephone).Width = wCol1;

            #region -- Title --
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cEnd);
            z.Value = "Report Customer Birthday";
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetCell(rArea, cNo);
            z.Value = "AREA";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rArea, cDealer);
            z.Value = ": " + area;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rDealer, cNo);
            z.Value = "DEALER";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rDealer, cDealer);
            z.Value = ": " + dealer;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rOutlet, cNo);
            z.Value = "OUTLET";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rOutlet, cDealer);
            z.Value = ": " + outlet;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rStatus, cNo);
            z.Value = "STATUS";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rStatus, cDealer);
            z.Value = ": " + status;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rPeriod, cNo);
            z.Value = "PERIODE";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rPeriod, cDealer);
            z.Value = ": " + periode;
            z.Style.Font.Bold = true;

            #endregion

            #region -- Header --
            z.Address = EP.GetRange(rHeader1, cStart, rHeader1, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
            z.Style.Font.Bold = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z.Address = EP.GetCell(rHeader1, cNo);
            z.Value = "NO";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cDealer);
            z.Value = "NAMA DEALER";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cBulan);
            z.Value = "BULAN";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cJumCust);
            z.Value = "JUMLAH CUSTOMER";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cInputByCRO);
            z.Value = "INPUT BY CRO";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cGift);
            z.Value = "GIFT";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cSMS);
            z.Value = "SMS";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cTelephone);
            z.Value = "TELEPHONE";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            #endregion

            #region -- Data --
            if (data.Count == 0) return package;
            for (int i = 0; i < data.Count; i++)
            {
                var row = rData + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cNo, Value = i + 1 },
                    new ExcelCellItem { Column = cDealer, Value = data[i].CompanyName },
                    new ExcelCellItem { Column = cBulan, Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(data[i].Month) },
                    new ExcelCellItem { Column = cJumCust, Value = data[i].TotalCustomer, Format = "number" },
                    new ExcelCellItem { Column = cInputByCRO, Value = data[i].Reminder, Format = "number" },
                    new ExcelCellItem { Column = cGift, Value = data[i].Gift, Format = "number" },
                    new ExcelCellItem { Column = cSMS, Value = data[i].SMS, Format = "number" },
                    new ExcelCellItem { Column = cTelephone, Value = data[i].Telephone, Format = "number" },
                };

                foreach (var item in items)
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

            z.Address = EP.GetRange(rTotal, cNo, rTotal, cBulan);
            z.Value = "TOTAL";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            var sums = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = cJumCust, Value = total[0].TotCustomer, Format = "number" },
                new ExcelCellItem { Column = cInputByCRO, Value = total[0].TotReminder, Format = "number" },
                new ExcelCellItem { Column = cGift, Value = total[0].TotGift, Format = "number" },
                new ExcelCellItem { Column = cSMS, Value = total[0].TotSMS, Format = "number" },
                new ExcelCellItem { Column = cTelephone, Value = total[0].TotTelephone, Format = "number" },
            };

            foreach (var sum in sums)
            {
                z.Address = EP.GetCell(rTotal, sum.Column);
                z.Value = sum.Value;
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                z.Style.Numberformat.Format = sum.Format != null ? sum.Format : fCustom;
            }


            z.Address = EP.GetRange(rTotal + 1, cStart, rTotal + 1, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 241, 221));
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rTotal + 1, cNo, rTotal + 1, cBulan);
            z.Value = "PERSENTASE (%)";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            decimal tcus = 0;
            decimal trmd = 0;
            decimal tgif = 0;
            decimal tsms = 0;
            decimal ttlp = 0;

            if (total[0].TotCustomer != 0)
            {
                tcus = (total[0].TotCustomer) / (total[0].TotCustomer) ;
                trmd = (total[0].TotReminder) / (total[0].TotCustomer);
                tgif = (total[0].TotGift) / (total[0].TotCustomer);
                tsms = (total[0].TotSMS) / (total[0].TotCustomer);
                ttlp = (total[0].TotTelephone) / (total[0].TotCustomer);
            }

            var perc = new List<ExcelCellItem>
            {
                //new ExcelCellItem { Column = cJumCust, Value =total[0].TotCustomer, Format = "number" },
                //new ExcelCellItem { Column = cInputByCRO, Value = total[0].TotReminder, Format = "number" },
                //new ExcelCellItem { Column = cGift, Value = total[0].TotGift, Format = "number" },
                //new ExcelCellItem { Column = cSMS, Value = total[0].TotSMS, Format = "number" },
                //new ExcelCellItem { Column = cTelephone, Value = total[0].TotTelephone, Format = "number" },

                new ExcelCellItem { Column = cJumCust, Value = tcus, Format = "number" },
                new ExcelCellItem { Column = cInputByCRO, Value = trmd, Format = "number" },
                new ExcelCellItem { Column = cGift, Value = tgif, Format = "number" },
                new ExcelCellItem { Column = cSMS, Value = tsms, Format = "number" },
                new ExcelCellItem { Column = cTelephone, Value = ttlp, Format = "number" },

            };



            foreach (var p in perc)
            {
                z.Address = EP.GetCell(rTotal + 1, p.Column);
                z.Value = p.Value;
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //z.Style.Numberformat.Format = p.Format != null ? p.Format : fCustom;
                z.Style.Numberformat.Format = "#,##0.00" + " %";
            }

            #endregion

            return package;
        }

        public JsonResult doExport4()
        {
            var garea = Request.Params["GroupArea"];
            var cmpCode = Request.Params["CompanyCode"];
            var branchCode = Request.Params["BranchCode"];
            var dateFrom = Request.Params["DateFrom"];
            var dateTo = Request.Params["DateTo"];
            var SelectAll = Request["SelectAll"] == "on" ? 1 : 0;

            var query = @"exec uspfn_CsRptBPKBReminderExport @GroupArea, @CompanyCode, @BranchCode, @DateFrom, @DateTo, @SelectAll";

            var parameters = new[]
                {
                    new SqlParameter("@GroupArea", garea),
                    new SqlParameter("@CompanyCode", cmpCode),
                    new SqlParameter("@BranchCode", branchCode),
                    new SqlParameter("@DateFrom", dateFrom),
                    new SqlParameter("@DateTo", dateTo),
                    new SqlParameter("@SelectAll", SelectAll)
                };

            var result = ctx.MultiResultSetSqlQuery(query, parameters);

            var dealer = cmpCode != "" ? ctx.Organizations.Where(x => x.CompanyCode == cmpCode).FirstOrDefault().CompanyName : "All Dealer";
            var area = garea != "" ? ctx.GroupAreas.Where(x => x.GroupNo == garea).FirstOrDefault().AreaDealer : "All Area";
            var outlet = branchCode != "" ? ctx.OutletInfos.Where(x => x.BranchCode == branchCode).FirstOrDefault().BranchName : "All Outlet";
            var periode = dateFrom + " s/d " + dateTo;

            var message = "";
            try
            {
                var package = new ExcelPackage();
                package = GenerateExcel4(package, area, dealer, outlet, periode, result);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception e)
            {
                message = e.Message;
                return Json(new { message = message });
            }
        }

        private static ExcelPackage GenerateExcel4(ExcelPackage package, string area, string dealer, string outlet, string periode, MultiResultSetReader result)
        {
            var sheet = package.Workbook.Worksheets.Add("BpkbReminder");
            var z = sheet.Cells[1, 1];
            var data = result.ResultSetFor<ReportBpkbReminderModel>().ToList();
            var total = result.ResultSetFor<ReportBpkbReminderModelTotal>().ToList();

            #region -- Constants --
            const int
                rTitle = 1,
                rArea = 2,
                rDealer = 3,
                rOutlet = 4,
                rPeriod = 5,
                rHeader1 = 6,
                rData = 7,

                cStart = 1,
                cDealer = 1,
                cOutlet = 2,
                cReadyDate = 3,
                cJumCust = 4,
                cInputByCRO = 5,
                cNoCall = 6,
                cPeresentase = 7,
                cEnd = 7;

            double
                wDealer = EP.GetTrueColWidth(25),
                wReadyDate = EP.GetTrueColWidth(20),
                wNoCall = EP.GetTrueColWidth(25),
                wCol1 = EP.GetTrueColWidth(18);

            const string
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";
            #endregion

            sheet.Column(cDealer).Width = wDealer;
            sheet.Column(cOutlet).Width = wDealer;
            sheet.Column(cReadyDate).Width = wReadyDate;
            sheet.Column(cJumCust).Width = wCol1;
            sheet.Column(cInputByCRO).Width = wCol1;
            sheet.Column(cNoCall).Width = wNoCall;
            sheet.Column(cPeresentase).Width = wCol1;

            #region -- Title --
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cEnd);
            z.Value = "Report BPKB Reminder";
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetCell(rArea, cDealer);
            z.Value = "AREA";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rArea, cReadyDate);
            z.Value = ": " + area;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rDealer, cDealer);
            z.Value = "DEALER";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rDealer, cReadyDate);
            z.Value = ": " + dealer;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rOutlet, cDealer);
            z.Value = "OUTLET";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rOutlet, cReadyDate);
            z.Value = ": " + outlet;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rPeriod, cDealer);
            z.Value = "READY DATE";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rPeriod, cReadyDate);
            z.Value = ": " + periode;
            z.Style.Font.Bold = true;

            #endregion

            #region -- Header --
            z.Address = EP.GetRange(rHeader1, cStart, rHeader1, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
            z.Style.Font.Bold = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z.Address = EP.GetCell(rHeader1, cDealer);
            z.Value = "DEALER";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cOutlet);
            z.Value = "OUTLET";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cReadyDate);
            z.Value = "READY DATE";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cJumCust);
            z.Value = "JUMLAH CUSTOMER";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cInputByCRO);
            z.Value = "INPUT BY CRO";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cNoCall);
            z.Value = "TIDAK DAPAT DIHUBUNGI";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cPeresentase);
            z.Value = "PERSENTASE (%)";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            #endregion

            #region -- Data --

            var iMerger = 0;
            var iMerger2 = 0;
            var oldVal = "";
            var oldVal2 = "";
         
            if (data.Count == 0) return package;
            for (int i = 0; i < data.Count; i++)
            {
                var row = rData + i;

                if (i == 0)
                {
                    oldVal = data[i].Dealer;
                    oldVal2 = data[i].Outlet;
                }

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cDealer, Value = data[i].Dealer },
                    new ExcelCellItem { Column = cOutlet, Value = data[i].Outlet },
                    new ExcelCellItem { Column = cReadyDate, Value = (data[i].BpkbReadyDate.HasValue ? data[i].BpkbReadyDate.Value.ToString("MMMM yyyy") : "Invalid Date") },
                    new ExcelCellItem { Column = cJumCust, Value = data[i].CustomerCount },
                    new ExcelCellItem { Column = cInputByCRO, Value = data[i].InputByCRO },
                    new ExcelCellItem { Column = cNoCall, Value = data[i].Unreachable },
                    new ExcelCellItem { Column = cPeresentase, Value = data[i].Percentation },
                };


                var j = 1;
                foreach (var item in items)
                {
                    //if (j == 1)
                    //{
                    //    //oldVal = item.Value.ToString();
                    //    if (oldVal == item.Value.ToString())
                    //    {
                    //        if (i != 0)
                    //        {
                    //            iMerger += 1;
                    //            //doMerger = true;
                    //        }
                    //        //else
                    //        //{
                    //        //    doMerger = false;
                    //        //}
                    //        rows = row;
                    //    }
                    //    else
                    //    {
                    //        frow = (rows - iMerger) + 1;
                    //        if (iMerger == 0)
                    //        {
                    //            z.Address = EP.GetCell(row, item.Column);
                    //        }
                    //        else
                    //        {
                    //            z.Address = EP.GetRange(frow, item.Column, rows, item.Column);
                    //            z.Merge = true;
                    //        }
                    //        z.Value = item.Value;
                    //        z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //        z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                    //        //z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //        z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //        iMerger = 0;
                    //        //doMerger = false;
                    //    }

                    //    if (iMerger == 0 && i == data.Count - 1)
                    //    {
                    //        frow = (rows - iMerger) + 1;
                    //        if (iMerger == 0)
                    //        {
                    //            z.Address = EP.GetCell(row, item.Column);
                    //        }
                    //        else
                    //        {
                    //            z.Address = EP.GetRange(frow, item.Column, rows, item.Column);
                    //            z.Merge = true;
                    //        }
                    //        z.Value = item.Value;
                    //        z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //        z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                    //        //z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //        z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //        iMerger = 0;
                    //        //doMerger = true;
                    //    }
                    //    else if (i == 0)
                    //    {
                    //        z.Address = EP.GetCell(row, item.Column);
                    //        z.Value = item.Value;
                    //        z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //        z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                    //        z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //    }

                    //}
                    //else
                    //{
                    //    z.Address = EP.GetCell(row, item.Column);
                    //    z.Value = item.Value;
                    //    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                    //}

                    //if (i == 0)
                    //{
                    //    z.Address = EP.GetCell(row, item.Column);
                    //    z.Value = item.Value;
                    //    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                    //}else
                    //{
                        if (j == 1)
                        {          
                            if (oldVal == item.Value.ToString() && i != 0)
                            {
                                //Merger
                                iMerger += 1;

                                if (iMerger > 0 && i == data.Count - 1)
                                {
                                    z.Address = EP.GetRange(row - iMerger, item.Column, row, item.Column);
                                    z.Merge = true;
                                    z.Value = oldVal;
                                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                                    z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                }

                            }
                            else
                            {
                                if (iMerger > 0)
                                {
                                    z.Address = EP.GetRange(row - (iMerger + 1), item.Column, row-1, item.Column);
                                    z.Merge = true;
                                    z.Value = oldVal;
                                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                                    z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                    z.Address = EP.GetCell(row, item.Column);
                                    z.Value = item.Value;
                                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;     
                                    
                                    iMerger = 0;
                                }else
                                {
                                    z.Address = EP.GetCell(row, item.Column);
                                    z.Value = item.Value;
                                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                                }
                            }
                        }
                        else if (j == 2)
                        {
                            if (oldVal2 == item.Value.ToString() && i != 0)
                            {
                                //Merger
                                iMerger2 += 1;

                                if (iMerger2 > 0 && i == data.Count - 1)
                                {
                                    z.Address = EP.GetRange(row - iMerger2, item.Column, row, item.Column);
                                    z.Merge = true;
                                    z.Value = oldVal2;
                                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                                    z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                }

                            }
                            else
                            {
                                if (iMerger2 > 0)
                                {
                                    z.Address = EP.GetRange(row - (iMerger2 + 1), item.Column, row - 1, item.Column);
                                    z.Merge = true;
                                    z.Value = oldVal2;
                                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                                    z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                    z.Address = EP.GetCell(row, item.Column);
                                    z.Value = item.Value;
                                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;

                                    iMerger2 = 0;
                                }
                                else
                                {
                                    z.Address = EP.GetCell(row, item.Column);
                                    z.Value = item.Value;
                                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                                }
                            }
                        }else
                        {
                            z.Address = EP.GetCell(row, item.Column);
                            z.Value = item.Value;
                            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                        }
                        j += 1;
                }
                oldVal = data[i].Dealer;
                oldVal2 = data[i].Outlet;
            }
            #endregion

            #region -- Total --
            var rTotal = data.Count + rData;
            z.Address = EP.GetRange(rTotal, cStart, rTotal, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 241, 221));
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rTotal, cDealer, rTotal, cReadyDate);
            z.Value = "TOTAL";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            var sums = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = cJumCust, Value = total[0].TotCustomerCount },
                new ExcelCellItem { Column = cInputByCRO, Value = total[0].TotInputByCRO },
                new ExcelCellItem { Column = cNoCall, Value = total[0].TotUnreachable },
                new ExcelCellItem { Column = cPeresentase, Value = total[0].TotPercentation },
            };

            foreach (var sum in sums)
            {
                z.Address = EP.GetCell(rTotal, sum.Column);
                z.Value = sum.Value;
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                z.Style.Numberformat.Format = sum.Format != null ? sum.Format : fCustom;
            }

            #endregion

            return package;
        }

        public JsonResult doExport5()
        {
            var garea = Request.Params["GroupArea"];
            var cmpCode = Request.Params["CompanyCode"];
            var branchCode = Request.Params["BranchCode"];
            var dateFrom = Request.Params["DateFrom"];
            var dateTo = Request.Params["DateTo"];
            var SelectAll = Request["SelectAll"] == "on" ? 1 : 0;

            var query = @"exec uspfn_CsChartStnkExtExport @GroupArea, @CompanyCode, @BranchCode, @DateFrom, @DateTo, @SelectAll";

            var parameters = new[]
                {
                    new SqlParameter("@GroupArea", garea),
                    new SqlParameter("@CompanyCode", cmpCode),
                    new SqlParameter("@BranchCode", branchCode),
                    new SqlParameter("@DateFrom", dateFrom),
                    new SqlParameter("@DateTo", dateTo),
                    new SqlParameter("@SelectAll", SelectAll),
                };

            var result = ctx.MultiResultSetSqlQuery(query, parameters);

            var dealer = cmpCode != "" ? ctx.Organizations.Where(x => x.CompanyCode == cmpCode).FirstOrDefault().CompanyName : "All Dealer";
            var area = garea != "" ? ctx.GroupAreas.Where(x => x.GroupNo == garea).FirstOrDefault().AreaDealer : "All Area";
            var outlet = branchCode != "" ? ctx.OutletInfos.Where(x => x.BranchCode == branchCode).FirstOrDefault().BranchName : "All Outlet";
            var periode = dateFrom + " s/d " + dateTo;

            var message = "";
            try
            {
                var package = new ExcelPackage();
                package = GenerateExcel5(package, area, dealer, outlet, periode, result);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception e)
            {
                message = e.Message;
                return Json(new { message = message });
            }
        }

        private static ExcelPackage GenerateExcel5(ExcelPackage package, string area, string dealer, string outlet, string periode, MultiResultSetReader result)
        {
            var sheet = package.Workbook.Worksheets.Add("STNKExtension");
            var z = sheet.Cells[1, 1];
            var data = result.ResultSetFor<ReportSTNKExtensionModel>().ToList();
            var total = result.ResultSetFor<ReportSTNKExtensionModelTotal>().ToList();

            #region -- Constants --
            const int
                rTitle = 1,
                rArea = 2,
                rDealer = 3,
                rOutlet = 4,
                rPeriod = 5,
                rHeader1 = 6,
                rData = 7,

                cStart = 1,
                cNo = 1,
                cDealer = 2,
                cCabang = 3,
                cJumCust = 4,
                cInputByCRO = 5,
                cPeresentase = 6,
                cEnd = 6;

            double
                wNo = EP.GetTrueColWidth(10),
                wDealer = EP.GetTrueColWidth(20),
                wCabang = EP.GetTrueColWidth(45),
                wCol1 = EP.GetTrueColWidth(18);

            const string
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";
            #endregion

            sheet.Column(cNo).Width = wNo;
            sheet.Column(cDealer).Width = wDealer;
            sheet.Column(cCabang).Width = wCabang;
            sheet.Column(cJumCust).Width = wCol1;
            sheet.Column(cInputByCRO).Width = wCol1;
            sheet.Column(cPeresentase).Width = wCol1;

            #region -- Title --
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cEnd);
            z.Value = "Report STNK Extension";
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetCell(rArea, cNo);
            z.Value = "AREA";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rArea, cDealer);
            z.Value = ": " + area;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rDealer, cNo);
            z.Value = "DEALER";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rDealer, cDealer);
            z.Value = ": " + dealer;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rOutlet, cNo);
            z.Value = "OUTLET";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rOutlet, cDealer);
            z.Value = ": " + outlet;
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rPeriod, cNo);
            z.Value = "INPUT DATE";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rPeriod, cDealer);
            z.Value = ": " + periode;
            z.Style.Font.Bold = true;

            #endregion

            #region -- Header --
            z.Address = EP.GetRange(rHeader1, cStart, rHeader1, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
            z.Style.Font.Bold = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z.Address = EP.GetCell(rHeader1, cNo);
            z.Value = "NO";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cDealer);
            z.Value = "DEALER";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cCabang);
            z.Value = "CABANG";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cJumCust);
            z.Value = "JUMLAH CUSTOMER";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cInputByCRO);
            z.Value = "INPUT BY CRO";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cPeresentase);
            z.Value = "PERSENTASE (%)";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            #endregion

            #region -- Data --
            if (data.Count == 0) return package;
            for (int i = 0; i < data.Count; i++)
            {
                var row = rData + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cNo, Value = i+1 },
                    new ExcelCellItem { Column = cDealer, Value = data[i].Dealer },
                    new ExcelCellItem { Column = cCabang, Value = data[i].Outlet },
                    new ExcelCellItem { Column = cJumCust, Value = data[i].CustomerCount },
                    new ExcelCellItem { Column = cInputByCRO, Value = data[i].InputByCRO },
                    new ExcelCellItem { Column = cPeresentase, Value = data[i].Percentage },
                };

                foreach (var item in items)
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

            z.Address = EP.GetRange(rTotal, cNo, rTotal, cCabang);
            z.Value = "TOTAL";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            var sums = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = cJumCust, Value = total[0].TotCustomerCount },
                new ExcelCellItem { Column = cInputByCRO, Value = total[0].TotInputByCRO },
                new ExcelCellItem { Column = cPeresentase, Value = total[0].TotPercentage },
            };

            foreach (var sum in sums)
            {
                z.Address = EP.GetCell(rTotal, sum.Column);
                z.Value = sum.Value;
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                z.Style.Numberformat.Format = sum.Format != null ? sum.Format : fCustom;
            }

            #endregion

            return package;
        }

        public JsonResult doExport6()
        {
            var dateInit = Request.Params["DateInit"];
            var dateReff = Request.Params["DateReff"];
            var interval = Request.Params["Interval"];

            var query = @"exec uspfn_CsDataMonitoringExport @DateInit, @DateReff, @Interval";

            var parameters = new[]
                {
                    new SqlParameter("@DateInit", dateInit),
                    new SqlParameter("@DateReff", dateReff),
                    new SqlParameter("@Interval", interval),
                };

            var result = ctx.MultiResultSetSqlQuery(query, parameters);

            var message = "";
            try
            {
                var package = new ExcelPackage();
                package = GenerateExcel6(package, dateInit, dateReff, interval, result);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception e)
            {
                message = e.Message;
                return Json(new { message = message });
            }
        }

        private static ExcelPackage GenerateExcel6(ExcelPackage package, string dateInit, string dateReff, string interval, MultiResultSetReader result)
        {
            var sheet = package.Workbook.Worksheets.Add("Data Summery");
            var z = sheet.Cells[1, 1];
            var data = result.ResultSetFor<DataSummeryModel>().ToList();
            //var total = result.ResultSetFor<ReportBpkbReminderModelTotal>().ToList();

            DateTime dInit = Convert.ToDateTime(dateInit);
            DateTime dReff = Convert.ToDateTime(dateReff);

            #region -- Constants --
            const int
                rTitle = 1,
                rDateInit = 2,
                rDateReff = 3,
                rInterval = 4,
                rHeader1 = 5,
                rData = 6,

                cStart = 1,
                cDate = 1,
                cDealer = 2,
                cTdsCall = 3,
                cCustBd = 4,
                cCustBPKB = 5,
                cSTNKExt = 6,
                cEnd = 6;

            double
                wDate = EP.GetTrueColWidth(20),
                wDealer = EP.GetTrueColWidth(45),
                wCol1 = EP.GetTrueColWidth(18);

            const string
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";
            #endregion

            sheet.Column(cDate).Width = wDate;
            sheet.Column(cDealer).Width = wDealer;
            sheet.Column(cTdsCall).Width = wCol1;
            sheet.Column(cCustBd).Width = wCol1;
            sheet.Column(cCustBPKB).Width = wCol1;
            sheet.Column(cSTNKExt).Width = wCol1;

            #region -- Title --
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cEnd);
            z.Value = "Data Summery";
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetCell(rDateInit, cDate);
            z.Value = "DATE INIT";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rDateInit, cDealer);
            z.Value = ": " + dInit.ToString("dd MMM yyyy");
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rDateReff, cDate);
            z.Value = "DATE REFF";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rDateReff, cDealer);
            z.Value = ": " + dReff.ToString("dd MMM yyyy");
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rInterval, cDate);
            z.Value = "INTERVAL";
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rInterval, cDealer);
            z.Value = ": " + interval + " Days";
            z.Style.Font.Bold = true;

            #endregion

            #region -- Header --
            z.Address = EP.GetRange(rHeader1, cStart, rHeader1, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
            z.Style.Font.Bold = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z.Address = EP.GetCell(rHeader1, cDate);
            z.Value = "DATE";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cDealer);
            z.Value = "DEALER";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cTdsCall);
            z.Value = "3 DAYS CALL";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cCustBd);
            z.Value = "CUST. BIRTHDAY";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cCustBPKB);
            z.Value = "CUST. BPKB";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cSTNKExt);
            z.Value = "STNK EXT.";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            #endregion

            #region -- Data --

            var rows = 0;
            var frow = 0;
            var iMerger = 0;
            var oldVal = "";
            bool doMerger = false;
            var rdate = "";

            if (data.Count == 0) return package;
            for (int i = 0; i < data.Count; i++)
            {
                var row = rData + i;

                if (i == 0)
                {
                    oldVal = data[i].DateInput.ToString("dd MMM yyyy");
                }

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cDate, Value = data[i].DateInput.ToString("dd MMM yyyy") },
                    new ExcelCellItem { Column = cDealer, Value = data[i].DealerName },
                    new ExcelCellItem { Column = cTdsCall, Value = data[i].C3DaysCall },
                    new ExcelCellItem { Column = cCustBd, Value = data[i].CsBirthday },
                    new ExcelCellItem { Column = cCustBPKB, Value = data[i].CsCustBpkb },
                    new ExcelCellItem { Column = cSTNKExt, Value = data[i].CsStnkExt },
                };


                var j = 1;
                foreach (var item in items)
                {
                    if (j == 1)
                    {
                        //oldVal = item.Value.ToString();
                        if (oldVal == item.Value.ToString())
                        {
                            iMerger += 1;
                            rows = row;
                            doMerger = false;
                        }
                        else
                        {
                            frow = (rows - iMerger) + 1;
                            if (iMerger == 0)
                            {
                                z.Address = EP.GetCell(row, item.Column);
                            }
                            else
                            {
                                z.Address = EP.GetRange(frow, item.Column, rows, item.Column);
                                z.Merge = true;
                            }
                            z.Value = item.Value;
                            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                            //z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            iMerger = 0;
                            doMerger = true;
                        }
                        if (!doMerger && i == data.Count - 1)
                        {
                            frow = (rows - iMerger) + 1;
                            if (iMerger == 0)
                            {
                                z.Address = EP.GetCell(row, item.Column);
                            }
                            else
                            {
                                z.Address = EP.GetRange(frow, item.Column, rows, item.Column);
                                z.Merge = true;
                            }
                            z.Value = item.Value;
                            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                            //z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            iMerger = 0;
                            doMerger = true;
                        }
                    }
                    else
                    {
                        z.Address = EP.GetCell(row, item.Column);
                        z.Value = item.Value;
                        z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                    }
                    //z.Address = EP.GetCell(row, item.Column);
                    //z.Value = item.Value;
                    //z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                    j += 1;
                }
                oldVal = data[i].DateInput.ToString("dd MMM yyyy");
            }
            #endregion

            //#region -- Total --
            //var rTotal = data.Count + rData;
            //z.Address = EP.GetRange(rTotal, cStart, rTotal, cEnd);
            //z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 241, 221));
            //z.Style.Font.Bold = true;

            //z.Address = EP.GetRange(rTotal, cDealer, rTotal, cReadyDate);
            //z.Value = "TOTAL";
            //z.Merge = true;
            //z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            //var sums = new List<ExcelCellItem>
            //{
            //    new ExcelCellItem { Column = cJumCust, Value = total[0].TotCustomerCount },
            //    new ExcelCellItem { Column = cInputByCRO, Value = total[0].TotInputByCRO },
            //    new ExcelCellItem { Column = cNoCall, Value = total[0].TotUnreachable },
            //    new ExcelCellItem { Column = cPeresentase, Value = total[0].TotPercentation },
            //};

            //foreach (var sum in sums)
            //{
            //    z.Address = EP.GetCell(rTotal, sum.Column);
            //    z.Value = sum.Value;
            //    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            //    z.Style.Numberformat.Format = sum.Format != null ? sum.Format : fCustom;
            //}

            return package;
        }

        public FileContentResult DownloadExcelFile(string key, string fileName)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=" + fileName + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }


        private class TdCallModel
        {
            public string CompanyCode { get; set; }
            public string BranchCode { get; set; }
            public string BranchName { get; set; }
            public Int32 DOData { get; set; }
            public Int32 DeliveryDate { get; set; }
            public Int32 TDaysCallData { get; set; }
            public Int32 TDaysCallByInput { get; set; }
        }

        private class TdCallModelTotal
        {
            public Int32 TotDoData { get; set; }
            public Int32 TotDeliveryDate { get; set; }
            public Int32 TotTDaysCallData { get; set; }
            public Int32 TotTDaysCallByInput { get; set; }
        }

        private class ReportTdCallModel
        {
            public string Dealer { get; set; }
            public string Outlet { get; set; }
            public Int32 JumBPK { get; set; }
            public Int32 InputByCRO { get; set; }
            public Decimal Percentation { get; set; }
        }

        private class ReportTdCallModelTotal
        {
            public Int32 TotJumBPK { get; set; }
            public Int32 TotInputByCRO { get; set; }
            public Decimal TotPercentation { get; set; }
        }

        private class ReportCustBirthdayModel
        {
            public Int32 Month { get; set; }
            public string CompanyName { get; set; }
            public Int32 TotalCustomer { get; set; }
            public Int32 Reminder { get; set; }
            public Int32 Gift { get; set; }
            public Int32 SMS { get; set; }
            public Int32 Telephone { get; set; }
        }

        private class ReportCustBirthdayModelTotal
        {
            public decimal TotCustomer { get; set; }
            public decimal TotReminder { get; set; }
            public decimal TotGift { get; set; }
            public decimal TotSMS { get; set; }
            public decimal TotTelephone { get; set; }
        }

        private class ReportBpkbReminderModel
        {
            public string Dealer { get; set; }
            public string Outlet { get; set; }
            public DateTime? BpkbReadyDate { get; set; }
            public Int32 CustomerCount { get; set; }
            public Int32 InputByCRO { get; set; }
            public Int32 Unreachable { get; set; }
            public Decimal Percentation { get; set; }
        }

        private class ReportBpkbReminderModelTotal
        {
            public Int32 TotCustomerCount { get; set; }
            public Int32 TotInputByCRO { get; set; }
            public Int32 TotUnreachable { get; set; }
            public Decimal TotPercentation { get; set; }
        }

        private class ReportSTNKExtensionModel
        {
            public string Dealer { get; set; }
            public string Outlet { get; set; }
            public Int32 CustomerCount { get; set; }
            public Int32 InputByCRO { get; set; }
            public Decimal Percentage { get; set; }
        }

        private class ReportSTNKExtensionModelTotal
        {
            public Int32 TotCustomerCount { get; set; }
            public Int32 TotInputByCRO { get; set; }
            public Decimal TotPercentage { get; set; }
        }

        private class DataSummeryModel
        {
            public DateTime DateInput { get; set; }
            public string DealerName { get; set; }
            public Int32? SeqNo { get; set; }
            public Int32 C3DaysCall { get; set; }
            public Int32 CsBirthday { get; set; }
            public Int32 CsCustBpkb { get; set; }
            public Int32 CsStnkExt { get; set; }
        }


    }
}
