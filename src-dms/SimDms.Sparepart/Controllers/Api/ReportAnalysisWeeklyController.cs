using ClosedXML.Excel;
using SimDms.Common;
using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers.Api
{
    public class ReportAnalysisWeeklyController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                IsBranch = IsBranch
            });
        }

        public JsonResult Area()
        {
            var area = ctx.DealerMapping.Select(a => new { value = a.Area, text = a.Area }).ToList();
            return Json(area);
        }

        public JsonResult Company()
        {
            List<object> listOfCompany = new List<object>();

            listOfCompany.Add(new { value = CompanyCode, text = CompanyName });

            return Json(listOfCompany);
        }

        public JsonResult loadData(SparepartWeekly model)
        {
            try
            {
                var Year = (int)DateTime.Now.DayOfYear;
                var Week = (int)DateTime.Now.DayOfWeek;
                var _week = model.PeriodYear == Year ? Week : 0;

                var query = "exec sp_GetSparepartAnalysisWeekly @p0,@p1,@p2,@p3,@p4";
                object[] parameters = { model.CompanyCode, model.BranchCode, model.PeriodYear, model.PeriodMonth, model.TypeOfGoods };
                var data = ctx.Database.SqlQuery<SparepartWeeklyGrid>(query, parameters);

                if (data.Sum(a=>a.Netto_WS) == 0)
                {
                    var queryInsert = "exec sp_InsertSparepartAnalysisWeekly @p0,@p1,@p2";
                    object[] paramInsert = { model.PeriodYear, model.PeriodMonth, _week };
                    ctx.Database.ExecuteSqlCommand(queryInsert, paramInsert);

                    data = ctx.Database.SqlQuery<SparepartWeeklyGrid>(query, parameters);
                }

                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        public ActionResult GenerateExcel(string CompanyCode, string BranchCode,  decimal PeriodYear, decimal PeriodMonth, string TypeOfGoods)
        {
            DateTime now = DateTime.Now;
            string fileName = "Sparepart_Analysis_Weekly_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "sp_GetSparepartAnalysisWeekly";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@PeriodYear", PeriodYear);
            cmd.Parameters.AddWithValue("@PeriodMonth", PeriodMonth);
            cmd.Parameters.AddWithValue("@TypeOfGoods", TypeOfGoods);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];

            int recNo = 5;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("AnalysisWeekly");

            ws.Column("B").Width = 14;
            ws.Column("C").Width = 14;
            ws.Column("D").Width = 14;
            ws.Column("E").Width = 14;
            ws.Column("F").Width = 14;
            ws.Column("G").Width = 14;

            var _dealer = "Dealer : ";
                _dealer += BranchCode == "" ? "ALL" : ctx.CoProfiles.Find(CompanyCode, BranchCode).CompanyName;
            var _type = "Type Of Goods : ";
            _type += TypeOfGoods == "" ? "ALL" : ctx.LookUpDtls.Find(CompanyCode, GnMstLookUpHdr.TypeOfGoods, TypeOfGoods).LookUpValueName;
             
            ws.Cell("A1").Value = _dealer;
            ws.Cell("A2").Value = _type;

            ws.Cell("A3").Value = "Minggu";
            ws.Range("A3:A4").Merge();
            ws.Range("A3:A4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A3:A4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A3:A4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A3:A4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Range("B3:C3").Merge();
            ws.Range("B3:C3").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("B3:C3").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("B3:C3").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("B3:C3").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("B3").Value = "Sales Out Bengkel ";

            ws.Range("D3:E3").Merge();
            ws.Range("D3:E3").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("D3:E3").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("D3:E3").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("D3:E3").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("D3").Value = "Sales Out Counter ";

            ws.Range("F3:G3").Merge();
            ws.Range("F3:G3").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("F3:G3").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("F3:G3").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("F3:G3").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("F3").Value = "Sales Out PartShop ";

            ws.Cell("H3").Value = "Stock";
            ws.Range("H3:H4").Merge();
            ws.Range("H3:H4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("H3:H4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("H3:H4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("H3:H4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("B4").Value = "Netto";
            ws.Cell("B4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("B4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("B4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("B4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("C4").Value = "HPP";
            ws.Cell("C4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("C4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("C4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("C4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("D4").Value = "Netto";
            ws.Cell("D4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("D4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("D4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("D4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("E4").Value = "HPP";
            ws.Cell("E4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("E4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("E4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("E4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("F4").Value = "Netto";
            ws.Cell("F4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("F4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("F4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("F4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("G4").Value = "HPP";
            ws.Cell("G4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("G4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("G4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("G4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            foreach (DataRow row in dt.Rows)
            {
                ws.Cell("A" + recNo).Value = "Ke - " + row["PeriodWeek"].ToString();
                ws.Cell("A" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("B" + recNo).Value = row["Netto_WS"].ToString();
                ws.Cell("B" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("C" + recNo).Value = row["HPP_WS"].ToString();
                ws.Cell("C" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("D" + recNo).Value = row["Netto_C"].ToString();
                ws.Cell("D" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("E" + recNo).Value = row["HPP_C"].ToString();
                ws.Cell("E" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("F" + recNo).Value = row["Netto_PS"].ToString();
                ws.Cell("F" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("G" + recNo).Value = row["HPP_PS"].ToString();
                ws.Cell("G" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("H" + recNo).Value = row["NilaiStock"].ToString();
                ws.Cell("H" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + recNo).Style.NumberFormat.Format = "#,##0.00";

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }
    }
}
