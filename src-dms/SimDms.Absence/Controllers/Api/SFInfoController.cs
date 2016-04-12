using SimDms.Absence.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SimDms.Common;
using EP = SimDms.Common.EPPlusHelper;

namespace SimDms.Absence.Controllers.Api
{
    public class SFInfoController : BaseController
    {
        #region MpDashboard

        public JsonResult Query(DateTime start, DateTime end)
        {
            //select @GroupNo = '500', @DealerCode = '6093401', @Start = '7/22/2004', @End = '12/22/2014'

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<SfMpDashboard>("exec uspfn_MpDashboard @BranchCode=@p0, @Start=@p1, @End=@p2, @Total=@p3", BranchCode, start, end, 0).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Total(DateTime start, DateTime end)
        {
            //select @GroupNo = '500', @DealerCode = '6093401', @Start = '7/22/2004', @End = '12/22/2014'

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<SfMpDashboard>("exec uspfn_MpDashboard @BranchCode=@p0, @Start=@p1, @End=@p2, @Total=@p3", BranchCode, start, end, 1).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

        #region -- Excel Functions --
        public FileContentResult DownloadExcelFile(string key, string filename)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + "_" + string.Format("{0}", DateTime.Now.ToString("dd-MMM-yyy_HH.mm")) + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        #endregion

        public JsonResult Excel(DateTime start, DateTime end)
        {
            var message = "";

            try
            {
                var query = @"exec uspfn_MpDashboard @BranchCode, @Start, @End, @Total";

                var parameters = new[]
                {
                    new SqlParameter("@BranchCode", BranchCode),
                    new SqlParameter("@Start", start),
                    new SqlParameter("@End", end),
                    new SqlParameter("@Total", 2)
                };

                var model = new ManPowerDashboardModel
                {
                    BranchCode = BranchCode,
                    BranchName = BranchName,
                    Start = start,
                    End = end
                };

                var result = ctx.MultiResultSetSqlQuery(query, parameters);

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

        private static ExcelPackage GenerateExcel(ExcelPackage package, ManPowerDashboardModel model, MultiResultSetReader result)
        {
            var ctx = new DataContext();
            var sheet = package.Workbook.Worksheets.Add("ManpowerDashboard");
            var z = sheet.Cells[1, 1];

            var data = result.ResultSetFor<SfMpDashboard>().ToList();
            var total = result.ResultSetFor<SfMpDashboard>().ToList();

            #region -- Constants --
            const int
                rTitle = 1,
                rDealer = 2,
                rPeriod = 3,
                rHeader1 = 5,
                rHeader2 = 6,
                rData = 7,

                cStart = 1,
                cOutlet = 1,
                cBMJml = 2,
                cSHJml = 3,
                cPlt = 4,
                cPltPct = 5,
                cGold = 6,
                cGoldPct = 7,
                cSlv = 8,
                cSlvPct = 9,
                cTrn = 10,
                cTrnPct = 11,
                cSPJml = 12,
                cSFJml = 13,
                cEnd = 13;

            double
                wOutlet = EP.GetTrueColWidth(35.71),
                wBM = EP.GetTrueColWidth(22.57),
                wSH = EP.GetTrueColWidth(10.71),
                wTotal = EP.GetTrueColWidth(19.30),
                hTotal = EP.GetTrueColWidth(27.75);

            const string
                fDate = "dd/MMM/yyyy",
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)",
                fPct = "#0\\.00%";

            #endregion

            sheet.Column(cOutlet).Width = wOutlet;
            sheet.Column(cBMJml).Width = wBM;
            sheet.Column(cSHJml).Width = wSH;
            sheet.Column(cSPJml).Width = wTotal;
            sheet.Column(cSFJml).Width = wTotal;

            #region -- Title --
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cEnd);
            z.Value = "MANPOWER DASHBOARD";
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetCell(rDealer, cStart);
            z.Value = "DEALER NAMA";
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rDealer, cBMJml, rDealer, cBMJml);
            z.Value = model.BranchName;
            z.Merge = true;
            //z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rPeriod, cStart);
            z.Value = "CUT OFF PERIODE";
            z.Style.Font.Bold = true;

            z.Address = EP.GetCell(rPeriod, cBMJml);
            z.Value = model.Start;
            z.Merge = true;
            //z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Style.Numberformat.Format = fDate;

            z.Address = EP.GetCell(rPeriod, cSHJml);
            z.Value = "to";
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetRange(rPeriod, cPlt, rPeriod, cPltPct);
            z.Value = model.End;
            z.Merge = true;
            //z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
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

            z.Address = EP.GetRange(rHeader1, cBMJml, rHeader2, cBMJml);
            z.Value = "BRANCH MANAGER";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cSHJml, rHeader2, cSHJml);
            z.Value = "SALES HEAD";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cPlt, rHeader1, cTrnPct);
            z.Value = "SALES";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader2, cPlt, rHeader2, cPltPct);
            z.Value = "PLATINUM";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader2, cGold, rHeader2, cGoldPct);
            z.Value = "GOLD";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader2, cSlv, rHeader2, cSlvPct);
            z.Value = "SILVER";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader2, cTrn, rHeader2, cTrnPct);
            z.Value = "TRAINEE";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cSPJml, rHeader2, cSPJml);
            z.Value = "TOTAL SALES PERSON";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cSFJml, rHeader2, cSFJml);
            z.Value = "TOTAL SALES FORCE";
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
                    new ExcelCellItem { Column = cOutlet, Value = data[i].Outlet },
                    new ExcelCellItem { Column = cBMJml, Value = data[i].BranchManager },
                    new ExcelCellItem { Column = cSHJml  , Value = data[i].SalesHead },
                    new ExcelCellItem { Column = cPlt , Value = data[i].Platinum },
                    new ExcelCellItem { Column = cPltPct, Value = data[i].PlatinumPct, Format = fPct },
                    new ExcelCellItem { Column = cGold  , Value = data[i].Gold },
                    new ExcelCellItem { Column = cGoldPct , Value = data[i].GoldPct, Format = fPct},
                    new ExcelCellItem { Column = cSlv, Value = data[i].Silver },
                    new ExcelCellItem { Column = cSlvPct  , Value = data[i].SilverPct , Format = fPct},
                    new ExcelCellItem { Column = cTrn , Value = data[i].Trainee },
                    new ExcelCellItem { Column = cTrnPct, Value = data[i].TraineePct , Format = fPct},
                    new ExcelCellItem { Column = cSPJml  , Value = data[i].TotalSalesPerson },
                    new ExcelCellItem { Column = cSFJml , Value = data[i].TotalSalesForce },
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
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var sums = new List<ExcelCellItem>
            {
                    new ExcelCellItem { Column = cOutlet, Value = total[0].Outlet },
                    new ExcelCellItem { Column = cBMJml, Value = total[0].BranchManager },
                    new ExcelCellItem { Column = cSHJml  , Value = total[0].SalesHead },
                    new ExcelCellItem { Column = cPlt , Value = total[0].Platinum },
                    new ExcelCellItem { Column = cPltPct, Value = total[0].PlatinumPct , Format = fPct},
                    new ExcelCellItem { Column = cGold  , Value = total[0].Gold },
                    new ExcelCellItem { Column = cGoldPct , Value = total[0].GoldPct, Format = fPct},
                    new ExcelCellItem { Column = cSlv, Value = total[0].Silver },
                    new ExcelCellItem { Column = cSlvPct  , Value = total[0].SilverPct , Format = fPct},
                    new ExcelCellItem { Column = cTrn , Value = total[0].Trainee },
                    new ExcelCellItem { Column = cTrnPct, Value = total[0].TraineePct , Format = fPct},
                    new ExcelCellItem { Column = cSPJml  , Value = total[0].TotalSalesPerson },
                    new ExcelCellItem { Column = cSFJml , Value = total[0].TotalSalesForce },
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
    }
}