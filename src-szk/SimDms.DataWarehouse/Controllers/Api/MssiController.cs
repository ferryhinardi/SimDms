using OfficeOpenXml;
using OfficeOpenXml.Style;
using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ep = SimDms.DataWarehouse.Helpers.EPPlusHelper;
using SimDms.DataWarehouse.Helpers;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class MssiController : BaseController
    {
        public JsonResult Years(string rptType)
        {
            if (rptType == "F")
            {
                var init = DateTime.Now.Year - 10;
                var years = new List<object>();
                for (int i = 0; i < 11; i++)
                {
                    if (DateTime.Now.Month < 4)
                    {
                        years.Add(new { value = init + i, text = (init + i - 1).ToString() + " - " + (init + i).ToString() });
                    }
                    else if (DateTime.Now.Month >= 4)
                    {
                        years.Add(new { value = init + i, text = (init + i).ToString() + " - " + (init + i + 1).ToString() });
                    }                    
                }
                return Json(years);
            }
            else
            {
                var init = DateTime.Now.Year - 10;
                var years = new List<object>();
                for (int i = 0; i < 11; i++) years.Add(new { value = init + i, text = (init + i).ToString() });
                return Json(years);
            }
        }

        public JsonResult Months(string rptType)
        {
            if (rptType == "F")
            {
                var months = new List<object>();
                for (int i = 4; i < 13; i++)
                {
                    var date = new DateTime(1900, i, 1);
                    var monthString = date.ToString("MMMM");
                    months.Add(new { value = i, text = monthString });
                }
                for (int i = 1; i < 4; i++)
                {
                    var date = new DateTime(1900, i, 1);
                    var monthString = date.ToString("MMMM");
                    months.Add(new { value = i, text = monthString });
                }
                return Json(months);
            }
            else
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
                var dealers = ctx.GnMstDealerMappings.Where(x => x.GroupNo == groupCode && x.isActive).Select(x => new
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
                              && a.isActive && b.isActive.Value
                              select new
                              {
                                  value = b.OutletCode,
                                  text = b.OutletAbbreviation
                              };
                return Json(outlets);
            }
            return Json(new { });
        }
        public JsonResult YearMaster()
        {
            var init = DateTime.Now.Year - 5;
            var years = new List<object>();
            for (int i = 0; i < 6; i++) years.Add(new { value = init + i, text = (init + i).ToString() });
            for (int i = 1; i < 6; i++) years.Add(new { value = DateTime.Now.Year + i, text = (DateTime.Now.Year + i).ToString() });
            return Json(years);
        }

        public JsonResult LoadTableMaster(string PeriodYear)
        {
            decimal PeriodYearParam;
            if (!Decimal.TryParse(PeriodYear, out PeriodYearParam))
                PeriodYearParam = 0;

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<MssiMonthlyMaster>(string.Format("EXEC uspfn_slGetMstMSSI {0}", PeriodYearParam)).ToList();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveMSSI(string Data)
        {
            var items = Data;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<MssiMonthlyMaster> listTarget = ser.Deserialize<List<MssiMonthlyMaster>>(Data);
            foreach (var item in listTarget)
            {
                ctx.Database.ExecuteSqlCommand(string.Format("EXEC uspfn_slSaveMstMSSI '{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}", item.MSSICode, item.CompanyCode, item.Year, item.Jan, item.Feb, item.Mar, item.Apr, item.May, item.Jun, item.Jul, item.Aug, item.Sep, item.Oct, item.Nov, item.Dec));
            }
            return Json(new { success = true });
        }

        public FileContentResult DownloadExcelFile(string key, string r)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=MSsI Nasional" + r.ToUpper() + DateTime.Now.ToString("yyyyMMddhhmm") + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        public JsonResult Query(MssiModel model)
        {
            var message = "";
            var calendar = new System.Globalization.GregorianCalendar();
            model.AreaCode = model.AreaCode ?? "";
            model.DealerCode = model.DealerCode ?? "";
            model.OutletCode = model.OutletCode ?? "";

            try
            {
                ctx.Database.CommandTimeout = 3600;
                var query = string.Empty;
                var suffix = (model.RptType == "F" ? "_fiscal" : "") + " @AreaCode, @DealerCode, @OutletCode, @Year, @YTDMonth, @ReportPart";

                query = "exec usprpt_mssi_salesunit" + suffix;
                var result_su = ctx.Database.SqlQuery<MssiResultModel>(query, SqlParams(model)).ToList();
                
                query = "exec usprpt_mssi_its" + suffix;
                var result_its = ctx.Database.SqlQuery<MssiResultModel>(query, SqlParams(model)).ToList();

                query = "exec usprpt_mssi_unitstock" + suffix;
                var result_stock = ctx.Database.SqlQuery<MssiResultModel>(query, SqlParams(model)).ToList();

                query = "exec usprpt_mssi_manpower" + suffix;
                var result_mp = ctx.Database.SqlQuery<MssiResultModel>(query, SqlParams(model)).ToList(); 
                
                query = "exec usprpt_mssi_mp_development" + suffix;
                var result_dev = ctx.Database.SqlQuery<MssiResultModel>(query, SqlParams(model)).ToList();

                var package = new ExcelPackage();
                package = GenerateExcel(package, model, result_su, result_its, result_stock, result_mp, result_dev);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch(Exception ex)
            {
                message = ex.Message;
                var inner = ex.InnerException != null ? ex.InnerException.Message : "";
                return Json(new { message = message, inner = inner });
            }
        }

        private SqlParameter[] SqlParams(MssiModel model)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@AreaCode", model.AreaCode),
                new SqlParameter("@DealerCode", model.DealerCode),
                new SqlParameter("@OutletCode", model.OutletCode),
                new SqlParameter("@Year", model.Year),
                new SqlParameter("@YTDMonth", model.YTDMonth),
                new SqlParameter("@ReportPart", "all")
            };
        }

        private static ExcelPackage GenerateExcel(ExcelPackage package, MssiModel model
            , List<MssiResultModel> data_su
            , List<MssiResultModel> data_its
            , List<MssiResultModel> data_stock
            , List<MssiResultModel> data_mp
            , List<MssiResultModel> data_dev)
        {
            var ctx = new DataContext();
            var sheet = package.Workbook.Worksheets.Add("MSsI Nasional");
            var z = sheet.Cells[1, 1];
            var mtd = model.RptType == "F" ? (model.YTDMonth < 4 ? 9 + model.YTDMonth : model.YTDMonth - 3) : model.YTDMonth;

            var scope = string.Empty;
            var areaCode = model.AreaCode == "" ? 0 : int.Parse(model.AreaCode);
            if (areaCode == 0 || (model.DealerCode == "" && model.OutletCode == "")) scope = "Nasional";
            else if (model.DealerCode != "" && model.OutletCode == "")
            {
                var dealer = ctx.GnMstDealerMappings.FirstOrDefault(x => 
                    x.DealerCode == model.DealerCode && x.GroupNo == areaCode && x.isActive);
                if (dealer != null) scope = dealer.DealerAbbreviation;
            }
            else if (model.DealerCode != "" && model.OutletCode != "")
            {
                var outlet = ctx.GnMstDealerOutletMappings.FirstOrDefault(x =>
                    x.OutletCode == model.OutletCode && x.GroupNo == areaCode &&
                    x.DealerCode == model.DealerCode && x.isActive.Value);
                if (outlet != null) scope = outlet.OutletAbbreviation;
            }

            #region --Constants--
            int
                rTitle = 1,
                rScope = 2,
                rHeader1 = 3,
                rHeader2 = 4,
                rData = 5,

                cPadLeft = 1,
                cLine = 2,
                cNumber = 3,
                cIndent1 = 4,
                cIndent2 = 5,
                cIndent3 = 6,
                cItem = 7,
                cTgtLastYear = 8,
                cTgtThisYear = 9,
                cAvgThisYear = 10,
                cLastYear = 11,
                cYTDLastYear = 12,
                cYTD = 13,
                cAvgActual = 14,
                cJan = model.RptType == "C" ? 15 : 24,
                cFeb = model.RptType == "C" ? 16 : 25,
                cMar = model.RptType == "C" ? 17 : 26,
                cApr = model.RptType == "C" ? 18 : 15,
                cMay = model.RptType == "C" ? 19 : 16,
                cJun = model.RptType == "C" ? 20 : 17,
                cJul = model.RptType == "C" ? 21 : 18,
                cAug = model.RptType == "C" ? 22 : 19,
                cSep = model.RptType == "C" ? 23 : 20,
                cOct = model.RptType == "C" ? 24 : 21,
                cNov = model.RptType == "C" ? 25 : 22,
                cDec = model.RptType == "C" ? 26 : 23;

            double
                wMonth = ep.GetTrueColWidth(12.00),
                hHeader2 = ep.GetTrueColWidth(23.25),
                wLine = ep.GetTrueColWidth(3.00),
                wItem = ep.GetTrueColWidth(19.50);

            const string
                fBlank = "0.00;0.00;",
                fPct = "#0\\%",
                fDecimal = "#0.00",
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";
            #endregion

            #region --Global--
            sheet.Cells.Style.Font.Name = "Candara";
            sheet.View.FreezePanes(5, 8);
            sheet.Cells.Style.Font.Size = 8f;
            #endregion

            #region --Width & Height--
            sheet.Row(rHeader2).Height = hHeader2;
            for (int i = cPadLeft; i <= cIndent3; i++) sheet.Column(i).Width = wLine;
            sheet.Column(cItem).Width = wItem;
            for (int i = cTgtLastYear; i <= cDec; i++) sheet.Column(i).Width = wMonth;
            #endregion

            #region --Title--
            z.Address = ep.GetRange(rTitle, cLine, rTitle, cDec);
            z.Value = "MONTHLY SALES INDICATOR";
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.Font.Size = 16f;
            z.Style.Font.Bold = true;
            z.Merge = true;

            z.Address = ep.GetRange(rScope, cLine, rScope, cIndent3);
            z.Value = scope.ToUpper();
            z.Style.Font.Size = 10f;
            z.Style.Font.Bold = true;
            z.Merge = true;

            z.Address = ep.GetCell(rScope, cItem);
            z.Value = model.Year;
            z.Style.Font.Size = 10f;
            z.Style.Font.Bold = true;
            #endregion

            #region --Header--
            z.Address = ep.GetRange(rHeader1, cLine, rHeader2, cNumber);
            z.Value = "Line";
            z.Merge = true;

            z.Address = ep.GetRange(rHeader1, cIndent1, rHeader2, cItem);
            z.Value = "ITEM";
            z.Merge = true;

            z.Address = ep.GetRange(rHeader1, cTgtLastYear, rHeader1, cAvgThisYear);
            z.Value = "TARGET";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cTgtLastYear);
            z.Value = "LAST YEAR";
            z.Style.WrapText = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cTgtThisYear);
            z.Value = "THIS YEAR";
            z.Style.WrapText = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cAvgThisYear);
            z.Value = "AVERAGE THIS YEAR";
            z.Style.WrapText = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetRange(rHeader1, cLastYear, rHeader1, model.RptType == "C" ? cDec : cMar);
            z.Value = "ACTUAL";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cLastYear);
            z.Value = "LAST YEAR";
            z.Style.WrapText = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cYTDLastYear);
            z.Value = "YTD LAST YEAR";
            z.Style.WrapText = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cYTD);
            z.Value = "YTD";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cAvgActual);
            z.Value = "AVERAGE";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            if (model.RptType == "C")
            {
                z.Address = ep.GetCell(rHeader2, cJan);
                z.Value = "JAN";
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                z.Address = ep.GetCell(rHeader2, cFeb);
                z.Value = "FEB";
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                z.Address = ep.GetCell(rHeader2, cMar);
                z.Value = "MAR";
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            z.Address = ep.GetCell(rHeader2, cApr);
            z.Value = "APR";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cMay);
            z.Value = "MAY";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cJun);
            z.Value = "JUN";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cJul);
            z.Value = "JUL";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cAug);
            z.Value = "AUG";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cSep);
            z.Value = "SEP";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cOct);
            z.Value = "OCT";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cNov);
            z.Value = "NOV";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = ep.GetCell(rHeader2, cDec);
            z.Value = "DEC";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            if (model.RptType == "F")
            {
                z.Address = ep.GetCell(rHeader2, cJan);
                z.Value = "JAN";
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                z.Address = ep.GetCell(rHeader2, cFeb);
                z.Value = "FEB";
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                z.Address = ep.GetCell(rHeader2, cMar);
                z.Value = "MAR";
                z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            z.Address = ep.GetRange(rHeader1, cLine, rHeader2, model.RptType == "C" ? cDec : cMar);
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            z.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            #endregion

            #region --Data SalesUnit--
            var su_start = rData;
            var su_lastrow = su_start + 0;
            if (data_su.Count == 0) goto _its;
            for (int i = 0; i < data_su.Count; i++)
            {
                su_lastrow = su_start + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cLine,         Value = data_su[i].Line                                       , Styles = new object[] { epStyles.NoBorder, epStyles.FillPurple } },
                    new ExcelCellItem { Column = cNumber,       Value = i + 1                                                 , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cIndent1,      Value = data_su[i].Indent1 == "" ? null : data_su[i].Indent1  , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cIndent2,      Value = data_su[i].Indent2 == "" ? null : data_su[i].Indent2  , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cIndent3,      Value = data_su[i].Indent3 == "" ? null : data_su[i].Indent3  , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cItem,         Value = data_su[i].Item == "" ? null : data_su[i].Item        , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cTgtLastYear,  Value = 0                     , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cTgtThisYear,  Value = 0                     , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAvgThisYear,  Value = 0                     , Format = fPct                 , Styles = new object[] { epStyles.Border, epStyles.FillBlack } },
                    new ExcelCellItem { Column = cLastYear,     Value = data_su[i].LastYear   , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cYTDLastYear,  Value = data_su[i].YTDLastYear, Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cYTD,          Value = data_su[i].YTD        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAvgActual,    Value = data_su[i].Average    , Format = fPct                 , Styles = new object[] { epStyles.Border, epStyles.FillBlack } },
                    new ExcelCellItem { Column = cJan,          Value = data_su[i].Jan        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cFeb,          Value = data_su[i].Feb        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cMar,          Value = data_su[i].Mar        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cApr,          Value = data_su[i].Apr        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cMay,          Value = data_su[i].May        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cJun,          Value = data_su[i].Jun        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cJul,          Value = data_su[i].Jul        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAug,          Value = data_su[i].Aug        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cSep,          Value = data_su[i].Sep        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cOct,          Value = data_su[i].Oct        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cNov,          Value = data_su[i].Nov        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cDec,          Value = data_su[i].Dec        , Format = fPct                 , Styles = new object[] { epStyles.Border, } }
                };

                foreach (var item in items)
                {
                    z.Address = ep.GetCell(su_lastrow, item.Column);
                    z.Value = item.Value ?? null;

                    z.Style.Numberformat.Format = fCustom;
                    if (item.Format == fPct)
                    {
                        if (i == 0) z.Style.Numberformat.Format = fBlank;
                        if (data_su[i].Indent2 == "Ratio Suzuki to Others Brand") z.Style.Numberformat.Format = fBlank;
                    }
                    
                    if (item.Styles.Contains(epStyles.Border)) z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Styles.Contains(epStyles.NoBorder))
                    {
                        z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        z.Style.Fill.BackgroundColor.SetColor(Color.White);
                    }

                    if (item.Styles.Contains(epStyles.FillPurple))
                    {
                        if (item.Value.ToString() != "")
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(96, 73, 123));
                        }
                    }
                    if (item.Styles.Contains(epStyles.FillBlack))
                    {
                        var filter1 = new string[]
                        {
                            "Polreg Suzuki",
                            "By Class",
                            "Suzuki Class", 
                            "Non Suzuki Class",
                        };
                        var filter2 = new string[]
                        {
                            "Market Share Suzuki (WS)",
                            "Market Share Suzuki (RS)",
                            "Σ Total Market (u/ Non Jabodetabek)",
                            "Market Share (u/ Non Jabodetabek)",
                            "Σ Faktur (u/ Jabodetabek)",
                            "% Kontribusi Faktur (u/ Jabodetabek)"
                        };

                        if (item.Column == cAvgThisYear)
                        {
                            if (filter2.Contains(data_su[i].Indent2) || filter2.Contains(data_su[i].Item))
                            {
                                z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                z.Style.Fill.BackgroundColor.SetColor(Color.Black);
                            }
                        }
                        else if (item.Column == cAvgActual)
                        {
                            if (filter1.Contains(data_su[i].Indent2) || filter1.Contains(data_su[i].Item) ||
                                filter2.Contains(data_su[i].Indent2) || filter2.Contains(data_su[i].Item))
                            {
                                z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                z.Style.Fill.BackgroundColor.SetColor(Color.Black);
                            }
                        }
                    }

                    if (item.Column == cLine) z.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    else if (item.Column == (model.RptType == "C" ? cDec : cMar)) z.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    if (i == 0) z.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    else if (i == data_su.Count - 1) z.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                }
            }

            #endregion

            #region --Data ITS--
        _its:
            var its_start = su_lastrow + 2;
            var its_lastrow = 0;
            if (data_its.Count == 0) goto _stock;
            
            for (int i = 0; i < data_its.Count; i++)
            {
                its_lastrow = its_start + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cLine,         Value = data_its[i].Line                                       , Styles = new object[] { epStyles.NoBorder, epStyles.FillPurple } },
                    new ExcelCellItem { Column = cNumber,       Value = i + 1                                                  , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cIndent1,      Value = data_its[i].Indent1 == "" ? null : data_its[i].Indent1 , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cIndent2,      Value = data_its[i].Indent2 == "" ? null : data_its[i].Indent2 , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cIndent3,      Value = data_its[i].Indent3 == "" ? null : data_its[i].Indent3 , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cItem,         Value = data_its[i].Item == "" ? null : data_its[i].Item       , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cTgtLastYear,  Value = 0                      , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cTgtThisYear,  Value = 0                      , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAvgThisYear,  Value = 0                      , Format = fPct                 , Styles = new object[] { epStyles.Border, epStyles.FillBlack } },
                    new ExcelCellItem { Column = cLastYear,     Value = data_its[i].LastYear   , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cYTDLastYear,  Value = data_its[i].YTDLastYear, Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cYTD,          Value = data_its[i].YTD        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAvgActual,    Value = data_its[i].Average    , Format = fPct                 , Styles = new object[] { epStyles.Border, epStyles.FillBlack } },
                    new ExcelCellItem { Column = cJan,          Value = data_its[i].Jan        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cFeb,          Value = data_its[i].Feb        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cMar,          Value = data_its[i].Mar        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cApr,          Value = data_its[i].Apr        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cMay,          Value = data_its[i].May        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cJun,          Value = data_its[i].Jun        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cJul,          Value = data_its[i].Jul        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAug,          Value = data_its[i].Aug        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cSep,          Value = data_its[i].Sep        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cOct,          Value = data_its[i].Oct        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cNov,          Value = data_its[i].Nov        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cDec,          Value = data_its[i].Dec        , Format = fPct                 , Styles = new object[] { epStyles.Border, } }
                };

                foreach (var item in items)
                {
                    z.Address = ep.GetCell(its_lastrow, item.Column);
                    z.Value = item.Value ?? null;

                    z.Style.Numberformat.Format = fCustom;
                    if (item.Format == fPct)
                    {
                        if (new int[] { 21 }.Contains(i + 1))
                        {
                            if (item.Column <= cAvgActual + mtd) z.Style.Numberformat.Format = fPct;
                        }
                        else if (new int[] { 14, 15, 16, 17, 18, 19 }.Contains(i + 1))
                        {
                            if (item.Column <= cAvgActual + mtd) z.Style.Numberformat.Format = fDecimal;
                        }
                        
                        if (i == 0) z.Style.Numberformat.Format = fBlank;
                        if (new int[] { 13, 20 }.Contains(i + 1)) z.Style.Numberformat.Format = fBlank;
                    }

                    if (item.Format == fBlank)
                    {
                        z.Style.Numberformat.Format = fBlank;
                    }
                     
                    if (item.Styles.Contains(epStyles.Border)) z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Styles.Contains(epStyles.NoBorder))
                    {
                        z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        z.Style.Fill.BackgroundColor.SetColor(Color.White);
                    }

                    if (new int[] { 2, 6, 14, 15, 16, 17, 18, 19, 21 }.Contains(i + 1))
                    {
                        if (item.Column > cItem)
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(182, 221, 232));
                        }
                    }

                    if (item.Styles.Contains(epStyles.FillPurple))
                    {
                        if (item.Value.ToString() != "")
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(96, 73, 123));
                        }
                    }

                    if (item.Styles.Contains(epStyles.FillBlack))
                    {
                        if (item.Column == cAvgActual)
                        {
                            if ((i + 1) >= 13 && (i + 1) <= 21)
                            {
                                z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                z.Style.Fill.BackgroundColor.SetColor(Color.Black);
                            }
                        }
                        else
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.Black);
                        }
                    }

                    if (item.Column == cLine) z.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    else if (item.Column == (model.RptType == "C" ? cDec : cMar)) z.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    if (i == 0) z.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    else if (i == data_its.Count - 1) z.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                }
            }
            #endregion

            #region --Data Unit Stock--
        _stock:
            var stock_start = its_lastrow + 2;
            var stock_lastrow = 0;
            if (data_stock.Count == 0) goto _mp;
            for (int i = 0; i < data_stock.Count; i++)
            {
                stock_lastrow = stock_start + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cLine,         Value = data_stock[i].Line                                          , Styles = new object[] { epStyles.NoBorder, epStyles.FillPurple } },
                    new ExcelCellItem { Column = cNumber,       Value = i + 1                                                       , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cIndent1,      Value = data_stock[i].Indent1 == "" ? null : data_stock[i].Indent1  , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cIndent2,      Value = data_stock[i].Indent2 == "" ? null : data_stock[i].Indent2  , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cIndent3,      Value = data_stock[i].Indent3 == "" ? null : data_stock[i].Indent3  , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cItem,         Value = data_stock[i].Item == "" ? null : data_stock[i].Item        , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cTgtLastYear,  Value = 0                                                           , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cTgtThisYear,  Value = 0                                                           , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAvgThisYear,  Value = 0                                                           , Styles = new object[] { epStyles.Border, epStyles.FillBlack } },
                    new ExcelCellItem { Column = cLastYear,     Value = data_stock[i].LastYear   , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cYTDLastYear,  Value = data_stock[i].YTDLastYear, Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cYTD,          Value = data_stock[i].YTD        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAvgActual,    Value = data_stock[i].Average    , Format = fDecimal                , Styles = new object[] { epStyles.Border, epStyles.FillBlack } },
                    new ExcelCellItem { Column = cJan,          Value = data_stock[i].Jan        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cFeb,          Value = data_stock[i].Feb        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cMar,          Value = data_stock[i].Mar        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cApr,          Value = data_stock[i].Apr        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cMay,          Value = data_stock[i].May        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cJun,          Value = data_stock[i].Jun        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cJul,          Value = data_stock[i].Jul        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAug,          Value = data_stock[i].Aug        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cSep,          Value = data_stock[i].Sep        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cOct,          Value = data_stock[i].Oct        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cNov,          Value = data_stock[i].Nov        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cDec,          Value = data_stock[i].Dec        , Format = fDecimal                , Styles = new object[] { epStyles.Border, } }
                };

                foreach (var item in items)
                {
                    z.Address = ep.GetCell(stock_lastrow, item.Column);
                    z.Value = item.Value ?? null;
                    
                    z.Style.Numberformat.Format = fCustom;
                    
                    if (item.Format == fBlank)
                    {
                        z.Style.Numberformat.Format = fBlank;
                    }

                    if (item.Format == fDecimal)
                    {
                        if (new int[] { 6 }.Contains(i + 1))
                        {
                            if (item.Column <= cAvgActual + mtd) z.Style.Numberformat.Format = fDecimal;
                        }
                    }

                    if (item.Styles.Contains(epStyles.Border)) z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Styles.Contains(epStyles.NoBorder))
                    {
                        z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        z.Style.Fill.BackgroundColor.SetColor(Color.White);
                    }
                    if (item.Styles.Contains(epStyles.FillGreen))
                    {
                        if (new int[] { 39, 40, 42, 43, 45, 46, 48, 49 }.Contains(i + 1))
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(146, 208, 80));
                        }
                    }

                    if (new int[] { 5, 19, 22, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 }.Contains(i + 1))
                    {
                        if (item.Column > cItem)
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(182, 221, 232));
                        }
                    }

                    if (item.Styles.Contains(epStyles.FillPurple))
                    {
                        if (item.Value.ToString() != "")
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(96, 73, 123));
                        }
                    }

                    if (item.Styles.Contains(epStyles.FillBlack))
                    {
                        if (item.Column == cAvgActual)
                        {
                            if ((i + 1) >= 6 && (i + 1) <= 13)
                            {
                                z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                z.Style.Fill.BackgroundColor.SetColor(Color.Black);
                            }
                        }
                        else
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.Black);
                        }
                    }

                    if (item.Column == cLine) z.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    else if (item.Column == (model.RptType == "C" ? cDec : cMar)) z.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    if (i == 0) z.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    else if (i == data_stock.Count - 1) z.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                }
            }

            #endregion

            #region --Data ManPower--
        _mp:

            // CHANGE THIS
            var mp_start = stock_lastrow + 2;
            var mp_lastrow = 0;
            if (data_mp.Count == 0) goto _dev;
            for (int i = 0; i < data_mp.Count; i++)
            {
                mp_lastrow = mp_start + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cLine,         Value = data_mp[i].Line                                       , Styles = new object[] { epStyles.NoBorder, epStyles.FillPurple } },
                    new ExcelCellItem { Column = cNumber,       Value = i + 1                                                 , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cIndent1,      Value = data_mp[i].Indent1 == "" ? null : data_mp[i].Indent1  , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cIndent2,      Value = data_mp[i].Indent2 == "" ? null : data_mp[i].Indent2  , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cIndent3,      Value = data_mp[i].Indent3 == "" ? null : data_mp[i].Indent3  , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cItem,         Value = data_mp[i].Item == "" ? null : data_mp[i].Item        , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cTgtLastYear,  Value = 0                     , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cTgtThisYear,  Value = 0                     , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAvgThisYear,  Value = 0                     , Format = fPct                 , Styles = new object[] { epStyles.Border, epStyles.FillBlack } },
                    new ExcelCellItem { Column = cLastYear,     Value = data_mp[i].LastYear   , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cYTDLastYear,  Value = data_mp[i].YTDLastYear, Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cYTD,          Value = data_mp[i].YTD        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAvgActual,    Value = 0                                                     , Styles = new object[] { epStyles.Border, epStyles.FillBlack } },
                    new ExcelCellItem { Column = cJan,          Value = data_mp[i].Jan        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cFeb,          Value = data_mp[i].Feb        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cMar,          Value = data_mp[i].Mar        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cApr,          Value = data_mp[i].Apr        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cMay,          Value = data_mp[i].May        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cJun,          Value = data_mp[i].Jun        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cJul,          Value = data_mp[i].Jul        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAug,          Value = data_mp[i].Aug        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cSep,          Value = data_mp[i].Sep        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cOct,          Value = data_mp[i].Oct        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cNov,          Value = data_mp[i].Nov        , Format = fPct                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cDec,          Value = data_mp[i].Dec        , Format = fPct                 , Styles = new object[] { epStyles.Border, } }
                };

                foreach (var item in items)
                {
                    z.Address = ep.GetCell(mp_lastrow, item.Column);
                    z.Value = item.Value ?? null;
                    
                    z.Style.Numberformat.Format = fCustom;
                    if (item.Format == fPct)
                    {
                        if (new int[] { 10, 12, 13, 14, 15 }.Contains(i + 1))
                        {
                            if (item.Column <= cAvgActual + mtd) z.Style.Numberformat.Format = fPct;
                        }
                        else if (i + 1 == 50)
                        {
                            if (item.Column <= cAvgActual + mtd) z.Style.Numberformat.Format = fDecimal;
                        }

                        if (i == 0) z.Style.Numberformat.Format = fBlank;
                        if (new int[] { 11, 18, 37 }.Contains(i + 1))
                            z.Style.Numberformat.Format = fBlank;
                    }

                    if (item.Format == fBlank)
                    {
                        z.Style.Numberformat.Format = fBlank;
                    }

                    if (item.Styles.Contains(epStyles.Border)) z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Styles.Contains(epStyles.NoBorder))
                    {
                        z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        z.Style.Fill.BackgroundColor.SetColor(Color.White);
                    }
                    //if (item.Styles.Contains(epStyles.FillGreen))
                    //{
                    //    if (new int[] { 39, 40, 42, 43, 45, 46, 48, 49 }.Contains(i + 1))
                    //    {
                    //        z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //        z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(146, 208, 80));
                    //    }
                    //}

                    if (new int[] { 5, 19, 22, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 }.Contains(i + 1))
                    {
                        if (item.Column > cItem)
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(182, 221, 232));
                        }
                    }

                    if (item.Styles.Contains(epStyles.FillPurple))
                    {
                        if (item.Value.ToString() != "")
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(96, 73, 123));
                        }
                    }

                    if (item.Styles.Contains(epStyles.FillBlack))
                    {
                        z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        z.Style.Fill.BackgroundColor.SetColor(Color.Black);
                    }

                    if (item.Column == cLine) z.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    else if (item.Column == (model.RptType == "C" ? cDec : cMar)) z.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    if (i == 0) z.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    else if (i == data_mp.Count - 1) z.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                }
            }
            #endregion

            #region --Data ManPower Development--
        _dev:
            var dev_start = mp_lastrow + 2;
            var dev_lastrow = 0;
            if (data_dev.Count == 0) return package;
            for (int i = 0; i < data_dev.Count; i++)
            {
                dev_lastrow = dev_start + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cLine,         Value = data_dev[i].Line                                        , Styles = new object[] { epStyles.NoBorder, epStyles.FillPurple } },
                    new ExcelCellItem { Column = cNumber,       Value = i + 1                                                   , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cIndent1,      Value = data_dev[i].Indent1 == "" ? null : data_dev[i].Indent1  , Styles = new object[] { epStyles.NoBorder } },
                    new ExcelCellItem { Column = cIndent2,      Value = data_dev[i].Indent2 == "" ? null : data_dev[i].Indent2  , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cIndent3,      Value = data_dev[i].Indent3 == "" ? null : data_dev[i].Indent3  , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cItem,         Value = data_dev[i].Item == "" ? null : data_dev[i].Item        , Styles = new object[] { epStyles.NoBorder, epStyles.FillGreen } },
                    new ExcelCellItem { Column = cTgtLastYear,  Value = 0                                                       , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cTgtThisYear,  Value = 0                                                       , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAvgThisYear,  Value = 0                                                       , Styles = new object[] { epStyles.Border, epStyles.FillBlack } },
                    new ExcelCellItem { Column = cLastYear,     Value = data_dev[i].LastYear                                    , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cYTDLastYear,  Value = data_dev[i].YTDLastYear                                 , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cYTD,          Value = data_dev[i].YTD                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAvgActual,    Value = 0                                                       , Styles = new object[] { epStyles.Border, epStyles.FillBlack } },
                    new ExcelCellItem { Column = cJan,          Value = data_dev[i].Jan                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cFeb,          Value = data_dev[i].Feb                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cMar,          Value = data_dev[i].Mar                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cApr,          Value = data_dev[i].Apr                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cMay,          Value = data_dev[i].May                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cJun,          Value = data_dev[i].Jun                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cJul,          Value = data_dev[i].Jul                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cAug,          Value = data_dev[i].Aug                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cSep,          Value = data_dev[i].Sep                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cOct,          Value = data_dev[i].Oct                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cNov,          Value = data_dev[i].Nov                                         , Styles = new object[] { epStyles.Border, } },
                    new ExcelCellItem { Column = cDec,          Value = data_dev[i].Dec                                         , Styles = new object[] { epStyles.Border, } }
                };

                foreach (var item in items)
                {
                    z.Address = ep.GetCell(dev_lastrow, item.Column);
                    z.Value = item.Value ?? null;

                    z.Style.Numberformat.Format = fCustom;
                    if (i + 1 == 0 && item.Column != cLine) z.Style.Numberformat.Format = fBlank;

                    if (item.Styles.Contains(epStyles.Border)) z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Styles.Contains(epStyles.NoBorder))
                    {
                        z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        z.Style.Fill.BackgroundColor.SetColor(Color.White);
                    }
                    if (new int[] { 2, 5, 8, 19, 22 }.Contains(i + 1))
                    {
                        if (item.Column > cItem)
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(182, 221, 232));
                        }
                    }

                    if (item.Styles.Contains(epStyles.FillPurple))
                    {
                        if (item.Value.ToString() != "")
                        {
                            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(96, 73, 123));
                        }
                    }

                    if (item.Styles.Contains(epStyles.FillBlack))
                    {
                        z.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        z.Style.Fill.BackgroundColor.SetColor(Color.Black);
                    }

                    if (item.Column == cLine) z.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    else if (item.Column == (model.RptType == "C" ? cDec : cMar)) z.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    if (i == 0) z.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    else if (i == data_dev.Count - 1) z.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                }
            }

            #endregion
            return package;
        }
    }

    public class MssiModel
    {
        public string RptType { get; set; }
        public string AreaCode { get; set; }
        public string DealerCode { get; set; }
        public string OutletCode { get; set; }
        public int Year { get; set; }
        public int YTDMonth { get; set; }
    }

    public class MssiResultModel
    {
        public string Line { get; set; }
        public int Seq { get; set; }
        public string Indent1 { get; set; }
        public string Indent2 { get; set; }
        public string Indent3 { get; set; }
        public string Item { get; set; }
        public decimal LastYear { get; set; }
        public decimal YTDLastYear { get; set; }
        public decimal YTD { get; set; }
        public decimal Average { get; set; }
        public decimal Jan { get; set; }
        public decimal Feb { get; set; }
        public decimal Mar { get; set; }
        public decimal Apr { get; set; }
        public decimal May { get; set; }
        public decimal Jun { get; set; }
        public decimal Jul { get; set; }
        public decimal Aug { get; set; }
        public decimal Sep { get; set; }
        public decimal Oct { get; set; }
        public decimal Nov { get; set; }
        public decimal Dec { get; set; }
    }

    public enum epStyles
    {
        Border, NoBorder, FillBlack, FillGreen, FillPurple
    }

    public class MssiMonthlyMaster
    {
        public string MSSICode { get; set; }
        public string CompanyCode { get; set; }
        public int Year { get; set; }
        public decimal Jan { get; set; }
        public decimal Feb { get; set; }
        public decimal Mar { get; set; }
        public decimal Apr { get; set; }
        public decimal May { get; set; }
        public decimal Jun { get; set; }
        public decimal Jul { get; set; }
        public decimal Aug { get; set; }
        public decimal Sep { get; set; }
        public decimal Oct { get; set; }
        public decimal Nov { get; set; }
        public decimal Dec { get; set; }
    }
}