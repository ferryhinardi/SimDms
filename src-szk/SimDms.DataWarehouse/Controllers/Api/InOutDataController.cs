using OfficeOpenXml;
using OfficeOpenXml.Style;
using SimDms.DataWarehouse.Helpers;
using EP = SimDms.DataWarehouse.Helpers.EPPlusHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Drawing;
using System.Dynamic;


namespace SimDms.DataWarehouse.Controllers.Api
{
    public class InOutDataController : BaseController
    {
        #region -- Combo Boxes --

        //public JsonResult Dealers(string area)
        //{
        //    if (area != "")
        //    {
        //        var groupCode = int.Parse(area);
        //        var dealers = ctx.GnMstDealerMappings.Where(x => x.GroupNo == groupCode).Select(x => new
        //        {
        //            value = x.DealerCode,
        //            text = "(" + x.DealerAbbreviation + ") - " + x.DealerName
        //        });
        //        return Json(dealers);
        //    }
        //    return Json(new { });
        //}
        public JsonResult Dealers()
        {
           var dealers = ctx.GnMstDealerMappings.Select(x => new
           {
                value = x.DealerCode,
                text = "(" + x.DealerAbbreviation + ") - " + x.DealerName
           });
           return Json(dealers);
        }

        public JsonResult Positions()
        {
            var positions = ctx.GnMstPositions.Where(x => x.DeptCode == "SALES").Select(x => new 
            {
                value = x.PosCode,
                text = x.PosName
            });
            return Json(positions.Distinct());
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
            Response.AppendHeader("content-disposition", "attachment; filename=InOutData.xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        public JsonResult QryToGrid(string dealer, string position, DateTime fromdate, DateTime todate, string outlet, string filter)
        {
            var message = "";
            var calendar = new System.Globalization.GregorianCalendar();

            try
            {
                var query = @"exec usprpt_mpInOutData @DealerCode, @Position, @FromDate, @ToDate, @Outlet, @Filter";
                var parameters = new[]
                {
                    new SqlParameter("@DealerCode", dealer),
                    new SqlParameter("@Position", position),
                    new SqlParameter("@FromDate", fromdate),
                    new SqlParameter("@ToDate", todate),
                    new SqlParameter("@Outlet", outlet),
                    new SqlParameter("@Filter", filter)
                };

                ctx.Database.CommandTimeout = 60;
                var result = ctx.MultiResultSetSqlQuery(query, parameters);
                var data = new List<InOutData>();
                var total = new List<InOutData>();
                var detail = new List<InOutDataDetail>();

                if (outlet == "" && filter == "" )
                {
                    data = result.ResultSetFor<InOutData>().ToList();
                    total = result.ResultSetFor<InOutData>().ToList();
                }
                else
                {
                    detail = result.ResultSetFor<InOutDataDetail>().ToList();
                }
               
                return Json(new { message = message, data = data, total = total, detail = detail });
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message });
            }
        }


        public JsonResult Query(string dealer, string position, DateTime fromdate, DateTime todate, string outlet, string filter)
        {
            var message = "";
            var calendar = new System.Globalization.GregorianCalendar();

            try
            {
                var query = @"exec usprpt_mpInOutData @DealerCode, @Position, @FromDate, @ToDate, @Outlet, @Filter";
                var parameters = new[]
                {
                    new SqlParameter("@DealerCode", dealer),
                    new SqlParameter("@Position", position),
                    new SqlParameter("@FromDate", fromdate),
                    new SqlParameter("@ToDate", todate),
                    new SqlParameter("@Outlet", outlet),
                    new SqlParameter("@Filter", filter)
                };

                var result = ctx.MultiResultSetSqlQuery(query, parameters);


                //var result = ctx.Database.SqlQuery<TrainingDashboardFinalResult>(query, area, dealer, startDate, endDate).FirstOrDefault();

                var model = new InOutDataModel
                {
                    DealerCode = dealer,
                    Position = position,
                    FromDate = fromdate,
                    ToDate = todate
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

        private static ExcelPackage GenerateExcel(ExcelPackage package, InOutDataModel model, MultiResultSetReader result)
        {
            var ctx = new DataContext();
            var sheet = package.Workbook.Worksheets.Add("InOutData");
            var z = sheet.Cells[1, 1];

            var dealerName = "ALL";
            var positionName = "ALL";

            if (model.DealerCode != "" && model.DealerCode != null)
            {
                dealerName = (from a in ctx.GnMstDealerMappings
                              where a.DealerCode == model.DealerCode
                              select a.DealerName).FirstOrDefault();

                if (dealerName == null) throw new Exception("Kode dealer salah / tidak ditemukan");
            }

            if (model.Position != "" && model.Position != null)
            {
                positionName = (from a in ctx.GnMstPositions
                              where a.DeptCode == "SALES" && a.PosCode == model.Position
                              select a.PosName).Distinct().FirstOrDefault();

                if (positionName == null) throw new Exception("Kode position salah / tidak ditemukan");
            }



            var data = result.ResultSetFor<InOutData>().ToList();
            var total = result.ResultSetFor<InOutData>().ToList();

            #region -- Constants --
            const int
                rTitle = 1,
                rDealer = 2,
                rPosition = 3,
                rPeriod = 4,
                rHeader = 6,
                rData = 7,

                cStart = 1,
                cOutlet = 1,
                cJmlAwal = 2,
                cJoin = 3,
                cResign = 4,
                cMutationIn = 5,
                cMutationOut = 6,
                cJmlAkhir = 7,
                cEnd = 7;
                
            double
                wOutlet = EP.GetTrueColWidth(22.43),
                w1 = EP.GetTrueColWidth(19),
                w2 = EP.GetTrueColWidth(10),
                hTotal = EP.GetTrueColWidth(27.75);

            const string
                fDate = "dd/MMM/yyyy",
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";
            #endregion

            sheet.Column(cOutlet).Width = wOutlet;
            sheet.Column(cJmlAwal).Width = w1;
            sheet.Column(cJoin).Width = w2;
            sheet.Column(cResign).Width = w2;
            sheet.Column(cMutationIn).Width = w1;
            sheet.Column(cMutationOut).Width = w1;
            sheet.Column(cJmlAkhir).Width = w1;

            #region -- Title --
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cEnd);
            z.Value = "IN OUT DATA";
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetCell(rDealer, cStart);
            z.Value = "DEALER NAME:";
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rDealer, cJmlAwal, rDealer, cJoin);
            z.Value = dealerName;
            z.Merge = true;
            //z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rPosition, cStart);
            z.Value = "POSITION:";
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rPosition, cJmlAwal, rPosition, cJoin);
            z.Value = positionName;
            z.Merge = true;

            z.Address = EP.GetCell(rPeriod, cStart);
            z.Value = "PERIODE:";
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rPeriod, cJmlAwal, rPeriod, cJoin);
            z.Value = model.FromDate.ToShortDateString()  + " - " + model.ToDate.ToShortDateString();
            z.Merge = true;
            //z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Style.Numberformat.Format = fDate;

            //z.Address = EP.GetCell(rPeriod, cBMNT);
            //z.Value = "to";
            //z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //z.Address = EP.GetRange(rPeriod, cSHJml, rPeriod, cSHT);
            //z.Value = model.End;
            //z.Merge = true;
            //z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Style.Numberformat.Format = fDate;
            #endregion

            #region -- Header --
            z.Address = EP.GetRange(rHeader, cStart, rHeader, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
            z.Style.Font.Bold = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z.Address = EP.GetCell(rHeader, cOutlet);
            z.Value = "OUTLET";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader, cJmlAwal);
            z.Value = "JUMLAH AWAL";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader, cJoin);
            z.Value = "JOIN";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader, cResign);
            z.Value = "RESIGN";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader, cMutationIn);
            z.Value = "MUTATION IN";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader, cMutationOut);
            z.Value = "MUTATION OUT";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader, cJmlAkhir);
            z.Value = "JUMLAH AKHIR";
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
                    new ExcelCellItem { Column = cJmlAwal, Value = data[i].Start },
                    new ExcelCellItem { Column = cJoin  , Value = data[i].Join },
                    new ExcelCellItem { Column = cResign , Value = data[i].Resign },
                    new ExcelCellItem { Column = cMutationIn , Value = data[i].MutationIn },
                    new ExcelCellItem { Column = cMutationOut  , Value = data[i].MutationOut },
                    new ExcelCellItem { Column = cJmlAkhir , Value = data[i].End },
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
                new ExcelCellItem { Column = cOutlet, Value = "TOTAL" },
                new ExcelCellItem { Column = cJmlAwal, Value = total[0].Start },
                new ExcelCellItem { Column = cJoin  , Value = total[0].Join },
                new ExcelCellItem { Column = cResign , Value = total[0].Resign },
                new ExcelCellItem { Column = cMutationIn, Value = total[0].MutationIn },
                new ExcelCellItem { Column = cMutationOut  , Value = total[0].MutationOut },
                new ExcelCellItem { Column = cEnd , Value = total[0].End },
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
        private class InOutDataModel
        {
            public string DealerCode { get; set; }
            public string Position { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }

        private class InOutData
        {
            public String OutletCode { get; set; }
            public String OutletAbbr { get; set; }
            public Int32? Start { get; set; }
            public Int32? Join { get; set; }
            public Int32? Resign { get; set; }
            public Int32? MutationIn { get; set; }
            public Int32? MutationOut { get; set; }
            public Int32? End { get; set; }
        }

        private class InOutDataDetail
        {
            public string DealerCode { get; set; }
            public string OutletCode { get; set; }
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