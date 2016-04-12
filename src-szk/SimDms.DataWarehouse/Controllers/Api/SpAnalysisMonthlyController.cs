using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class SpAnalysisMonthlyController : BaseController
    {
        public JsonResult SpAnalisisBulananGrid()
        {
            var area = Request["Area"];
            var companyName = Request["CompanyCode"];
            var branchCode = Request["BranchCode"];
            var year = Request["Year"];
            var month = Request["Month"];
            var typeOfGoods = Request["TypeOfGoods"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spAnalisisBulananViewgrid";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@areas", area);
            cmd.Parameters.AddWithValue("@Company", companyName);
            cmd.Parameters.AddWithValue("@BranchCodes", branchCode);
            cmd.Parameters.AddWithValue("@TypeOfGoods", typeOfGoods);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            cmd.Parameters.AddWithValue("@Month", month);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.Username);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public ActionResult exportExcel(string Area, string Dealer, string Outlet, string SpID, string Year, string AreaText, string DealerText, string OutletText, string Month, string MonthText, string TypeOfGoods, string TypeOfGoodsText)
        {
            string type = "";
            string branchId = "";
            string branchText = "";
            string fileName = "";
            int flag = 0;
            fileName = "Lap_SpAnalisisBulanan" + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "EXEC " + SpID + "'" + Area + "','" + Dealer + "','" + Outlet + "','" + TypeOfGoods + "','" + Year + "','" + Month + "','" + CurrentUser.Username + "'";
            cmd.CommandTimeout = 360;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt1 = new DataTable();

            if (TypeOfGoods == null || TypeOfGoods == string.Empty || TypeOfGoods == "")
            {
                type = "SEMUA TYPE OF GOODS";
            }
            else
            {
                type = TypeOfGoodsText;
            }

            if (Area == "")
            {
                branchId = "SEMUA AREA";
                branchText = "SEMUA AREA";
            }
            else if (Area != "" && Dealer == "" && Outlet == "")
            {
                branchId = "SEMUA DEALER AREA " + AreaText;
                branchText = "SEMUA DEALER AREA " + AreaText;
            }
            else if (Area != "" && Dealer != "" && Outlet == "")
            {
                branchId = "SEMUA BRANCH DEALER-" + DealerText;
                branchText = "SEMUA BRANCH DEALER-" + DealerText;
            }
            else
            {
                branchId = Outlet;
                branchText = OutletText;
            }

            if (Area != "" && Dealer != "" && Outlet != "")
            {
                flag = 0;
            }
            else if (Area != "" && Dealer != "" && Outlet == "")
            {
                flag = 1;
            }
            else if (Area != "" && Dealer == "" && Outlet == "")
            {
                flag = 2;
            }
            else
            {
                flag = 2;
            }

            dt1 = ds.Tables[0];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }

            int lastRow = 9;

            var wb = new XLWorkbook();

            var sheetName = "";
            if (Outlet == null || Outlet == string.Empty || Outlet == "")
            {
                sheetName = "SpAnalisisBulananAll";
            }
            else
            {
                sheetName = "SpAnalisisBulanan_" + Outlet;
            }

            var ws = wb.Worksheets.Add(sheetName);

            var hdrTable = ws.Range("A1:U7");
            hdrTable.Style
                .Font.SetBold()
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            var rngTable = ws.Range("A7:AQ7");
            rngTable.Style
                .Font.SetBold()
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Alignment.SetWrapText();

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;

            ws.Columns("1").Width = 20;
            ws.Columns("2").Width = 20;
            ws.Columns("3").Width = 15;
            ws.Columns("4").Width = 15;
            ws.Columns("5").Width = 15;
            ws.Columns("6").Width = 15;
            ws.Columns("7").Width = 15;
            ws.Columns("8").Width = 15;
            ws.Columns("9").Width = 15;
            ws.Columns("10").Width = 15;
            ws.Columns("11").Width = 15;
            ws.Columns("12").Width = 15;
            ws.Columns("13").Width = 15;
            ws.Columns("14").Width = 15;
            ws.Columns("15").Width = 15;
            ws.Columns("16").Width = 20;
            ws.Columns("17").Width = 15;
            ws.Columns("18").Width = 15;
            ws.Columns("19").Width = 15;
            ws.Columns("20").Width = 15;
            ws.Columns("21").Width = 15;
            ws.Columns("22").Width = 15;
            ws.Columns("23").Width = 15;
            ws.Columns("24").Width = 15;
            ws.Columns("25").Width = 15;
            ws.Columns("26").Width = 15;
            ws.Columns("27").Width = 15;
            ws.Columns("28").Width = 15;
            ws.Columns("29").Width = 15;
            ws.Columns("30").Width = 15;
            ws.Columns("31").Width = 15;
            ws.Columns("32").Width = 15;
            ws.Columns("33").Width = 15;
            ws.Columns("34").Width = 15;
            ws.Columns("35").Width = 15;
            ws.Columns("36").Width = 15;
            ws.Columns("37").Width = 15;
            ws.Columns("38").Width = 15;
            ws.Columns("39").Width = 15;
            ws.Columns("40").Width = 15;
            ws.Columns("41").Width = 15;
            ws.Columns("42").Width = 15;
            ws.Columns("43").Width = 15;
            ws.Columns("44").Width = 15;
            ws.Columns("45").Width = 15;
            ws.Columns("46").Width = 15;
            ws.Columns("47").Width = 15;

            //First Names   
            ws.Cell("A1").Value = "Kode Branch ";
            ws.Cell("A2").Value = "Nama Branch";
            ws.Cell("A3").Value = "System";
            ws.Cell("A4").Value = "Tahun";

            ws.Cell("C1").Value = "SUZUKI SPAREPART ANALISIS BULANAN " + type;
            ws.Cell("C1").Style.Font.SetBold().Font.SetFontSize(14);

            ws.Cell("B1").Value = branchId;
            ws.Cell("B2").Value = branchText;
            ws.Cell("B3").Value = "SDMS";
            ws.Cell("B4").Value = Year;

            ws.Range("A" + 6 + ":" + "A" + 8).Merge();
            ws.Range("A" + 6 + ":" + "A" + 8).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + 6 + ":" + "A" + 8).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + 6 + ":" + "A" + 8).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + 6 + ":" + "A" + 8).Value = "Bulan";
            ws.Range("A" + 6 + ":" + "A" + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("A" + 6 + ":" + "A" + 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("A" + 6 + ":" + "A" + 8).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("B" + 6 + ":" + "B" + 8).Merge();
            ws.Range("B" + 6 + ":" + "B" + 8).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("B" + 6 + ":" + "B" + 8).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("B" + 6 + ":" + "B" + 8).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("B" + 6 + ":" + "B" + 8).Value = "Jml Jaringan";
            ws.Range("B" + 6 + ":" + "B" + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("B" + 6 + ":" + "B" + 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("B" + 6 + ":" + "B" + 8).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("C" + 6 + ":" + "F" + 7).Merge();
            ws.Range("C" + 6 + ":" + "F" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("C" + 6 + ":" + "F" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("C" + 6 + ":" + "F" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("C" + 6 + ":" + "F" + 7).Value = "Penjualan Kotor";
            ws.Range("C" + 6 + ":" + "F" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("C" + 6 + ":" + "F" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("C" + 6 + ":" + "F" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("G" + 6 + ":" + "J" + 7).Merge();
            ws.Range("G" + 6 + ":" + "J" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("G" + 6 + ":" + "J" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("G" + 6 + ":" + "J" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("G" + 6 + ":" + "J" + 7).Value = "Penjualan Bersih";
            ws.Range("G" + 6 + ":" + "J" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("G" + 6 + ":" + "J" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("G" + 6 + ":" + "J" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("K" + 6 + ":" + "O" + 7).Merge();
            ws.Range("K" + 6 + ":" + "O" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("K" + 6 + ":" + "O" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("K" + 6 + ":" + "O" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("K" + 6 + ":" + "O" + 7).Value = "Harga Pokok (HPP)";
            ws.Range("K" + 6 + ":" + "O" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("K" + 6 + ":" + "O" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("K" + 6 + ":" + "O" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("P" + 6 + ":" + "S" + 7).Merge();
            ws.Range("P" + 6 + ":" + "S" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("P" + 6 + ":" + "S" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("P" + 6 + ":" + "S" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("P" + 6 + ":" + "S" + 7).Value = "Margin";
            ws.Range("P" + 6 + ":" + "S" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("P" + 6 + ":" + "S" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("P" + 6 + ":" + "S" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("T" + 6 + ":" + "T" + 8).Merge();
            ws.Range("T" + 6 + ":" + "T" + 8).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("T" + 6 + ":" + "T" + 8).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("T" + 6 + ":" + "T" + 8).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("T" + 6 + ":" + "T" + 8).Value = "Penerimaan Pembelian";
            ws.Range("T" + 6 + ":" + "T" + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("T" + 6 + ":" + "T" + 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("T" + 6 + ":" + "T" + 8).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("U" + 6 + ":" + "U" + 8).Merge();
            ws.Range("U" + 6 + ":" + "U" + 8).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("U" + 6 + ":" + "U" + 8).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("U" + 6 + ":" + "U" + 8).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("U" + 6 + ":" + "U" + 8).Value = "Nilai Stock";
            ws.Range("U" + 6 + ":" + "U" + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("U" + 6 + ":" + "U" + 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("U" + 6 + ":" + "U" + 8).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("V" + 6 + ":" + "V" + 8).Merge();
            ws.Range("V" + 6 + ":" + "V" + 8).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("V" + 6 + ":" + "V" + 8).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("V" + 6 + ":" + "V" + 8).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("V" + 6 + ":" + "V" + 8).Value = "ITO";
            ws.Range("V" + 6 + ":" + "V" + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("V" + 6 + ":" + "V" + 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("V" + 6 + ":" + "V" + 8).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("W" + 6 + ":" + "Y" + 7).Merge();
            ws.Range("W" + 6 + ":" + "Y" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("W" + 6 + ":" + "Y" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("W" + 6 + ":" + "Y" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("W" + 6 + ":" + "Y" + 7).Value = "Demand";
            ws.Range("W" + 6 + ":" + "Y" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("W" + 6 + ":" + "Y" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("W" + 6 + ":" + "Y" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("Z" + 6 + ":" + "AB" + 7).Merge();
            ws.Range("Z" + 6 + ":" + "AB" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("Z" + 6 + ":" + "AB" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("Z" + 6 + ":" + "AB" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("Z" + 6 + ":" + "AB" + 7).Value = "Supply";
            ws.Range("Z" + 6 + ":" + "AB" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("Z" + 6 + ":" + "AB" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("Z" + 6 + ":" + "AB" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("AC" + 6 + ":" + "AE" + 7).Merge();
            ws.Range("AC" + 6 + ":" + "AE" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("AC" + 6 + ":" + "AE" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("AC" + 6 + ":" + "AE" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("AC" + 6 + ":" + "AE" + 7).Value = "Service Ratio";
            ws.Range("AC" + 6 + ":" + "AE" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("AC" + 6 + ":" + "AE" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("AC" + 6 + ":" + "AE" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("AF" + 6 + ":" + "AQ" + 6).Merge();
            ws.Range("AF" + 6 + ":" + "AQ" + 6).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("AF" + 6 + ":" + "AQ" + 6).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("AF" + 6 + ":" + "AQ" + 6).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("AF" + 6 + ":" + "AQ" + 6).Value = "Data Stock ";
            ws.Range("AF" + 6 + ":" + "AQ" + 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("AF" + 6 + ":" + "AQ" + 6).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("AF" + 6 + ":" + "AQ" + 8).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("AR" + 6 + ":" + "AR" + 8).Merge();
            ws.Range("AR" + 6 + ":" + "AR" + 8).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("AR" + 6 + ":" + "AR" + 8).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("AR" + 6 + ":" + "AR" + 8).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("AR" + 6 + ":" + "AR" + 8).Value = "% Dead Moving";
            ws.Range("AR" + 6 + ":" + "AR" + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("AR" + 6 + ":" + "AR" + 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("AR" + 6 + ":" + "AR" + 8).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("AS" + 6 + ":" + "AT" + 7).Merge();
            ws.Range("AS" + 6 + ":" + "AT" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("AS" + 6 + ":" + "AT" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("AS" + 6 + ":" + "AT" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("AS" + 6 + ":" + "AT" + 7).Value = "Lead Time Order (hari)";
            ws.Range("AS" + 6 + ":" + "AT" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("AS" + 6 + ":" + "AT" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("AS" + 6 + ":" + "AT" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("AF" + 7 + ":" + "AG" + 7).Merge();
            ws.Range("AF" + 7 + ":" + "AG" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("AF" + 7 + ":" + "AG" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("AF" + 7 + ":" + "AG" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("AF" + 7 + ":" + "AG" + 7).Value = "Moving Code 0 ";
            ws.Range("AF" + 7 + ":" + "AG" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("AF" + 7 + ":" + "AG" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("AF" + 7 + ":" + "AG" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("AH" + 7 + ":" + "AI" + 7).Merge();
            ws.Range("AH" + 7 + ":" + "AI" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("AH" + 7 + ":" + "AI" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("AH" + 7 + ":" + "AI" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("AH" + 7 + ":" + "AI" + 7).Value = "Moving Code 1 ";
            ws.Range("AH" + 7 + ":" + "AI" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("AH" + 7 + ":" + "AI" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("AH" + 7 + ":" + "AI" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("AJ" + 7 + ":" + "AK" + 7).Merge();
            ws.Range("AJ" + 7 + ":" + "AK" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("AJ" + 7 + ":" + "AK" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("AJ" + 7 + ":" + "AK" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("AJ" + 7 + ":" + "AK" + 7).Value = "Moving Code 2 ";
            ws.Range("AJ" + 7 + ":" + "AK" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("AJ" + 7 + ":" + "AK" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("AJ" + 7 + ":" + "AK" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("AL" + 7 + ":" + "AM" + 7).Merge();
            ws.Range("AL" + 7 + ":" + "AM" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("AL" + 7 + ":" + "AM" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("AL" + 7 + ":" + "AM" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("AL" + 7 + ":" + "AM" + 7).Value = "Moving Code 3 ";
            ws.Range("AL" + 7 + ":" + "AM" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("AL" + 7 + ":" + "AM" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("AL" + 7 + ":" + "AM" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("AN" + 7 + ":" + "AO" + 7).Merge();
            ws.Range("AN" + 7 + ":" + "AO" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("AN" + 7 + ":" + "AO" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("AN" + 7 + ":" + "AO" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("AN" + 7 + ":" + "AO" + 7).Value = "Moving Code 4 ";
            ws.Range("AN" + 7 + ":" + "AO" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("AN" + 7 + ":" + "AO" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("AN" + 7 + ":" + "AO" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Range("AP" + 7 + ":" + "AQ" + 7).Merge();
            ws.Range("AP" + 7 + ":" + "AQ" + 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("AP" + 7 + ":" + "AQ" + 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("AP" + 7 + ":" + "AQ" + 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("AP" + 7 + ":" + "AQ" + 7).Value = "Moving Code 5 ";
            ws.Range("AP" + 7 + ":" + "AQ" + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("AP" + 7 + ":" + "AQ" + 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range("AP" + 7 + ":" + "AQ" + 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            ws.Cell("C8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("C8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("C8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("C8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("C8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("C8").Value = "Workshop ";

            ws.Cell("D8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("D8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("D8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("D8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("D8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("D8").Value = "Counter ";

            ws.Cell("E8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("E8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("E8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("E8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("E8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("E8").Value = "Partshop ";

            ws.Cell("F8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("F8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("F8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("F8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("F8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("F8").Value = "Sub Dealer ";

            ws.Cell("G8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("G8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("G8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("G8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("G8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("G8").Value = "Workshop ";

            ws.Cell("H8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("H8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("H8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("H8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("H8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("H8").Value = "Counter ";

            ws.Cell("I8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("I8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("I8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("I8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("I8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("I8").Value = "Partshop ";

            ws.Cell("J8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("J8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("J8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("J8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("J8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("J8").Value = "Sub Dealer ";

            ws.Cell("K8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("K8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("K8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("K8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("K8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("K8").Value = "Workshop ";

            ws.Cell("L8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("L8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("L8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("L8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("L8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("L8").Value = "Counter ";

            ws.Cell("M8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("M8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("M8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("M8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("M8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("M8").Value = "Partshop ";

            ws.Cell("N8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("N8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("N8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("N8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("N8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("N8").Value = "Sub Dealer ";

            ws.Cell("O8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("O8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("O8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("O8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("O8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("O8").Value = "Total HPP ";

            ws.Cell("P8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("P8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("P8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("P8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("P8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("P8").Value = "Workshop ";

            ws.Cell("Q8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("Q8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("Q8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("Q8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("Q8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("Q8").Value = "Counter";

            ws.Cell("R8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("R8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("R8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("R8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("R8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("R8").Value = "Partshop";

            ws.Cell("S8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("S8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("S8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("S8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("S8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("S8").Value = "Sub Dealer";

            ws.Cell("W8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("W8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("W8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("W8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("W8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("W8").Value = "Line ";

            ws.Cell("X8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("X8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("X8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("X8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("X8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("X8").Value = "Quantity ";

            ws.Cell("Y8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("Y8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("Y8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("Y8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("Y8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("Y8").Value = "Nilai";

            ws.Cell("Z8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("Z8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("Z8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("Z8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("Z8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("Z8").Value = "Line ";

            ws.Cell("AA8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AA8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AA8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AA8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AA8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AA8").Value = "Quantity ";

            ws.Cell("AB8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AB8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AB8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AB8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AB8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AB8").Value = "Nilai";

            ws.Cell("AC8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AC8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AC8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AC8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AC8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AC8").Value = "Line ";

            ws.Cell("AD8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AD8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AD8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AD8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AD8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AD8").Value = "Quantity ";

            ws.Cell("AE8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AE8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AE8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AE8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AE8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AE8").Value = "Nilai";

            ws.Cell("AF8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AF8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AF8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AF8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AF8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AF8").Value = "Amount ";

            ws.Cell("AG8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AG8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AG8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AG8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AG8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AG8").Value = "Qty";

            ws.Cell("AH8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AH8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AH8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AH8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AH8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AH8").Value = "Amount ";

            ws.Cell("AI8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AI8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AI8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AI8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AI8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AI8").Value = "Qty";

            ws.Cell("AJ8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AJ8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AJ8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AJ8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AJ8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AJ8").Value = "Amount ";

            ws.Cell("AK8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AK8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AK8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AK8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AK8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AK8").Value = "Qty";

            ws.Cell("AL8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AL8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AL8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AL8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AL8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AL8").Value = "Amount ";

            ws.Cell("AM8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AM8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AM8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AM8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AM8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AM8").Value = "Qty";

            ws.Cell("AN8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AN8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AN8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AN8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AN8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AN8").Value = "Amount ";

            ws.Cell("AO8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AO8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AO8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AO8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AO8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AO8").Value = "Qty";

            ws.Cell("AP8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AP8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AP8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AP8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AP8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AP8").Value = "Amount ";

            ws.Cell("AQ8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AQ8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AQ8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AQ8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AQ8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AQ8").Value = "Qty";

            ws.Cell("AS8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AS8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AS8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AS8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AS8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AS8").Value = "Regular ";

            ws.Cell("AT8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("AT8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("AT8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("AT8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("AT8").Style.Fill.BackgroundColor = XLColor.LightBlue;
            ws.Cell("AT8").Value = "Emergency";

            foreach (var row in dt1.Rows)
            {
                //Bulan
                ws.Cell("A" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[0];
                ws.Cell("A" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                //Jumlah jaringan
                ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[5 - flag];
                ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("B" + lastRow).Style.NumberFormat.Format = "#,##0";

                //WORKSHOP PK
                ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6 - flag];
                ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("C" + lastRow).Style.NumberFormat.Format = "#,##0";

                //COUNTER PK
                ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7 - flag];
                ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("D" + lastRow).Style.NumberFormat.Format = "#,##0";

                //PARTSHOP PK
                ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8 - flag];
                ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("E" + lastRow).Style.NumberFormat.Format = "#,##0";

                //SUBDEALER PK
                ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9 - flag];
                ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("F" + lastRow).Style.NumberFormat.Format = "#,##0";

                //WORKSHOP PB
                ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[10 - flag];
                ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("G" + lastRow).Style.NumberFormat.Format = "#,##0";

                //COUNTER PB
                ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[11 - flag];
                ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("H" + lastRow).Style.NumberFormat.Format = "#,##0";

                //PARTSHOP PB
                ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("I" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[12 - flag];
                ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("I" + lastRow).Style.NumberFormat.Format = "#,##0";

                //SUBDEALER PB
                ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("J" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[13 - flag];
                ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("J" + lastRow).Style.NumberFormat.Format = "#,##0";

                //WORKSHOP HPP
                ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("K" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[14 - flag];
                ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("K" + lastRow).Style.NumberFormat.Format = "#,##0";

                //COUNTER HPP
                ws.Cell("L" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("L" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("L" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("L" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("L" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[15 - flag];
                ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("L" + lastRow).Style.NumberFormat.Format = "#,##0";

                //PARTSHOP HPP
                ws.Cell("M" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("M" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("M" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("M" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("M" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[16 - flag];
                ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("M" + lastRow).Style.NumberFormat.Format = "#,##0";

                //SUBDEALER HPP
                ws.Cell("N" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("N" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("N" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("N" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("N" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[17 - flag];
                ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("N" + lastRow).Style.NumberFormat.Format = "#,##0";

                //TOTAL HPP
                ws.Cell("O" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("O" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("O" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("O" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("O" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[18 - flag];
                ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("O" + lastRow).Style.NumberFormat.Format = "#,##0";

                //WORKSHOP MARGIN
                ws.Cell("P" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("P" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("P" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("P" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("P" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[19 - flag];
                ws.Cell("P" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("P" + lastRow).Style.NumberFormat.Format = "#,##0";

                //COUNTER MARGIN
                ws.Cell("Q" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[20 - flag];
                ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("Q" + lastRow).Style.NumberFormat.Format = "#,##0";

                //PARTSHOP MARGIN
                ws.Cell("R" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("R" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("R" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("R" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("R" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[21 - flag];
                ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("R" + lastRow).Style.NumberFormat.Format = "#,##0";

                //SUB DEALER MARGIN
                ws.Cell("S" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("S" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("S" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("S" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("S" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[22 - flag];
                ws.Cell("S" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("S" + lastRow).Style.NumberFormat.Format = "#,##0";

                //PENERIMAAN PEMBELIAN
                ws.Cell("T" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("T" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("T" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("T" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("T" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[23 - flag];
                ws.Cell("T" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("T" + lastRow).Style.NumberFormat.Format = "#,##0";

                //NILAI STOCK
                ws.Cell("U" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("U" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("U" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("U" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("U" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[24 - flag];
                ws.Cell("U" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("U" + lastRow).Style.NumberFormat.Format = "#,##0";

                //ITO
                ws.Cell("V" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("V" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("V" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("V" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("V" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[25 - flag];
                ws.Cell("V" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("V" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                //LINE DEMAND
                ws.Cell("W" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("W" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("W" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("W" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("W" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[26 - flag];
                ws.Cell("W" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("W" + lastRow).Style.NumberFormat.Format = "#,##0";

                //QTY DEMAND
                ws.Cell("X" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("X" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("X" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("X" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("X" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[27 - flag];
                ws.Cell("X" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("X" + lastRow).Style.NumberFormat.Format = "#,##0.0";

                //NILAI DEMAND
                ws.Cell("Y" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("Y" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("Y" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("Y" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("Y" + lastRow).Value = (Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[28 - flag]));
                ws.Cell("Y" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("Y" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                //LINE SUPPLY
                ws.Cell("Z" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("Z" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("Z" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("Z" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("Z" + lastRow).Value = (Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[29 - flag]));
                ws.Cell("Z" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("Z" + lastRow).Style.NumberFormat.Format = "#,##0";

                //QTY SUPPLY
                ws.Cell("AA" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AA" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AA" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AA" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AA" + lastRow).Value = (Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[30 - flag]));
                ws.Cell("AA" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AA" + lastRow).Style.NumberFormat.Format = "#,##0.0";

                //NILAI SUPPLY
                ws.Cell("AB" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AB" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AB" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AB" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AB" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[31 - flag];
                ws.Cell("AB" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AB" + lastRow).Style.NumberFormat.Format = "#,##0";

                //LINE SERVICE RATIO
                ws.Cell("AC" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AC" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AC" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AC" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AC" + lastRow).Value = (Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[32 - flag])) / 100;
                ws.Cell("AC" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AC" + lastRow).Style.NumberFormat.Format = "#,##0.00" + " %";

                //QTY SERVICE RATIO
                ws.Cell("AD" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AD" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AD" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AD" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AD" + lastRow).Value = (Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[33 - flag])) / 100;
                ws.Cell("AD" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AD" + lastRow).Style.NumberFormat.Format = "#,##0.00" + " %";

                //NILAI SERVICE RATIO
                ws.Cell("AE" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AE" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AE" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AE" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AE" + lastRow).Value = (Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[34 - flag])) / 100;
                ws.Cell("AE" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AE" + lastRow).Style.NumberFormat.Format = "#,##0.00" + " %";

                //AMOUNT MC0
                ws.Cell("AF" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AF" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AF" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AF" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AF" + lastRow).Value = (Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[35 - flag]));
                ws.Cell("AF" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AF" + lastRow).Style.NumberFormat.Format = "#,##0";

                //QTY MC0
                ws.Cell("AG" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AG" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AG" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AG" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AG" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[36 - flag];
                ws.Cell("AG" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AG" + lastRow).Style.NumberFormat.Format = "#,##0.0";

                //AMOUNT MC1
                ws.Cell("AH" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AH" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AH" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AH" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AH" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[37 - flag];
                ws.Cell("AH" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AH" + lastRow).Style.NumberFormat.Format = "#,##0";

                //QTY MC1
                ws.Cell("AI" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AI" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AI" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AI" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AI" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[38 - flag];
                ws.Cell("AI" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AI" + lastRow).Style.NumberFormat.Format = "#,##0.0";

                //AMOUNT MC2
                ws.Cell("AJ" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AJ" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AJ" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AJ" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AJ" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[39 - flag];
                ws.Cell("AJ" + lastRow).Style.NumberFormat.Format = "#,##0";

                //QTY MC2
                ws.Cell("AK" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AK" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AK" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AK" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AK" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[40 - flag];
                ws.Cell("AK" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AK" + lastRow).Style.NumberFormat.Format = "#,##0.0";

                //AMOUNT MC3
                ws.Cell("AL" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AL" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AL" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AL" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AL" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[41 - flag];
                ws.Cell("AL" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AL" + lastRow).Style.NumberFormat.Format = "#,##0";

                //QTY MC3
                ws.Cell("AM" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AM" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AM" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AM" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AM" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[42 - flag];
                ws.Cell("AM" + lastRow).Style.NumberFormat.Format = "#,##0.0";

                //AMOUNT MC4
                ws.Cell("AN" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AN" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AN" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AN" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AN" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[43 - flag];
                ws.Cell("AN" + lastRow).Style.NumberFormat.Format = "#,##0";

                //QTY MC4
                ws.Cell("AO" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AO" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AO" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AO" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AO" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[44 - flag];
                ws.Cell("AO" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AO" + lastRow).Style.NumberFormat.Format = "#,##0.0";

                //AMOUNT MC5
                ws.Cell("AP" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AP" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AP" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AP" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AP" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[45 - flag];
                ws.Cell("AP" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AP" + lastRow).Style.NumberFormat.Format = "#,##0";

                //QTY MC5
                ws.Cell("AQ" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AQ" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AQ" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AQ" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AQ" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[46 - flag];
                ws.Cell("AQ" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AQ" + lastRow).Style.NumberFormat.Format = "#,##0.0";

                // % DEAD MOVING
                ws.Cell("AR" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AR" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AR" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AR" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AR" + lastRow).Value = (Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[47 - flag]))/100;
                ws.Cell("AR" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AR" + lastRow).Style.NumberFormat.Format = "#,##0.00" + " %";

                //REGULER LT
                ws.Cell("AS" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AS" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AS" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AS" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AS" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[48 - flag];
                ws.Cell("AS" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AS" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                //EMERGENCY LT
                ws.Cell("AT" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("AT" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("AT" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("AT" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("AT" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[49 - flag];
                ws.Cell("AT" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("AT" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                lastRow++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }
    }
}