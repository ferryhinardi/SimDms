using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClosedXML.Excel;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using SimDms.DataWarehouse.Controllers;
using SimDms.DataWarehouse.Models;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Excel = Microsoft.Office.Interop.Excel;
using Spire.Xls;
using Spire.Xls.Charts;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class DataFakturPolisiController : BaseController
    {
        public ActionResult GenerateDataFakturPolisi()
        {
            string EndDate = Request["EndDate"];
            string Area = Request["Area"];
            string Dealer = Request["Dealer"];
            string Periode = Request["Periode"];
            string fileName = "ReportFPolModel4w_"+EndDate;

            string DealerCode = Dealer.Substring(4,(Dealer.Length - 4));
            string DealerName = ctx.GnMstDealerMappingNews.Where(o => o.DealerCode == DealerCode).FirstOrDefault().DealerName;
            //string StartDate = "20151101";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GenerateDataFakturPolisi_AllType";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@StartDate", EndDate);
            cmd.Parameters.AddWithValue("@Date", EndDate);
            cmd.Parameters.AddWithValue("@Dealer", Dealer);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dtx = new DataTable();
            da.Fill(ds);
            dt1 = ds.Tables[0];
            dtx = ds.Tables[1];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("FP Polisi");
                var ws2 = wb.Worksheets.Add("Char FP Polisi");

                ws.Columns("1").Width = 10;
                ws.Columns("2").Width = 15;
                ws.Columns("3").Width = 20;
                ws.Columns(4,35).Width = 5;

                ws.Range("A" + lastRow + ":C" + lastRow).Merge();
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A" + lastRow).Value = "DATA FAKTUR POLISI";

                ws2.Range("A" + lastRow + ":C" + lastRow).Merge();
                ws2.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                ws2.Cell("A" + lastRow).Value = "DATA FAKTUR POLISI";

                lastRow = 2;
                ws.Range("A" + lastRow + ":J" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = "Date For " + Periode;

                ws2.Range("A" + lastRow + ":J" + lastRow).Merge();
                ws2.Cell("A" + lastRow).Value = "Date For " + Periode;

                lastRow = 3;
                ws.Range("A" + lastRow + ":J" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = "Dealer : " + DealerName;

                ws2.Range("A" + lastRow + ":J" + lastRow).Merge();
                ws2.Cell("A" + lastRow).Value = "Dealer : " + DealerName;
                lastRow = 4;
                lastRow++;

                #region All Type
                var tgl = 0;
                ws.Range("A" + lastRow + ":C" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = "ALL TYPE";
                ws.Range("A" + lastRow + ":C" + lastRow).Style.Font.SetBold();
                ws.Range("A" + lastRow + ":C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + lastRow + ":C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + lastRow + ":C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + lastRow + ":C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                for (int i = 1 + 3; i <= 31 + 3; i++)
                {
                    tgl++;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = tgl;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Font.SetBold();
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    if ((i - 3) == Convert.ToInt32(Periode.Substring(0, 2)))
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                    }
                }
                lastRow++;

                foreach (DataRow row in dt1.Rows)
                {
                    var NO = row[dt1.Columns[2].Caption];
                    var Year = Convert.ToInt32((row[dt1.Columns[3].Caption]));
                    var Month = Convert.ToInt32((row[dt1.Columns[4].Caption]));
                    var Bulan = "";

                    if (NO.ToString() == "1000")
                    {
                        var D = Year + "-" + Month + "-01";
                        string sql = string.Format(@"SELECT CONVERT(varchar(10),DATEADD(dd,-DAY(DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))), DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))),120) as Tanggal");
                        var data = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
                        var sql2 = string.Format(@"SELECT COUNT(*) FROM dbo.GetWorkingDays_V2('" + data  + "')");
                        var data2 = ctx.Database.SqlQuery<int>(sql2).FirstOrDefault();
                        var sql3 = string.Format(@"SELECT COUNT(*) FROM dbo.GetWorkingDays_V2('" + EndDate + "')");
                        var data3 = ctx.Database.SqlQuery<int>(sql3).FirstOrDefault();

                        var p = 0;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "SPK";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Value = row[dt1.Columns[0].Caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        for (int i = 4; i < dt1.Columns.Count - 1; i++)
                        {
                            var a = i + 1;
                            p = p + Convert.ToInt32(row[dt1.Columns[a].Caption]);
                            if ((i - 3) > Convert.ToInt32(Periode.Substring(0, 2))) { p = 0; }

                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = p.ToString() == "0" ? (i - 3) > Convert.ToInt32(Periode.Substring(0, 2)) ? "" : p.ToString() : p.ToString();
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            if ((i - 3) == Convert.ToInt32(Periode.Substring(0, 2)))
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Font.FontColor = XLColor.Red;
                            }
                            var Holiday = ctx.gnMstCalendar.Where(z=> z.CalendarDate.Year == Year 
                                                        && z.CalendarDate.Month == Month
                                                        && z.CalendarDate.Day == (i-3)).FirstOrDefault();
                            if (Holiday != null)
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Orange;
                            }
                        }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Value = "Day " + data3 + " from " + data2 + " days";
                        lastRow++;
                    }
                    else if (NO.ToString() == "2000")
                    {
                        if (Month.ToString().Length == 1) { Bulan = "0" + Month.ToString(); }
                        var D = Year + "-" + Bulan + "-01";
                        string sql = string.Format(@"SELECT CONVERT(varchar(10),DATEADD(dd,-DAY(DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))), DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))),120) as Tanggal");
                        var data = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
                        var sql2 = string.Format(@"SELECT COUNT(*) FROM dbo.GetWorkingDays_V2('" + data + "')");
                        var data2 = ctx.Database.SqlQuery<int>(sql2).FirstOrDefault();

                        var p = 0;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Stock";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Value = row[dt1.Columns[0].Caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        for (int i = 4; i < dt1.Columns.Count - 1; i++)
                        {
                            var a = i + 1;
                            p = p + Convert.ToInt32(row[dt1.Columns[a].Caption]);
                            if ((i - 3) > Convert.ToInt32(Periode.Substring(0, 2))) { p = 0; }

                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = p.ToString() == "0" ? (i - 3) > Convert.ToInt32(Periode.Substring(0, 2)) ? "" : p.ToString() : p.ToString(); //row[dt1.Columns[a].Caption];
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            if ((i - 3) == Convert.ToInt32(Periode.Substring(0, 2)))
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Font.FontColor = XLColor.Red;
                            }
                            var Holiday = ctx.gnMstCalendar.Where(z => z.CalendarDate.Year == Year
                                                        && z.CalendarDate.Month == Month
                                                        && z.CalendarDate.Day == (i - 3)).FirstOrDefault();
                            if (Holiday != null)
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Orange;
                            }
                        }
                        //ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Value = p.ToString() + "/" + data2.ToString() + "days=" + (p / data2 + "/day");
                        lastRow++;
                    }
                    else
                    {
                        if (Month.ToString().Length == 1) { Bulan = "0" + Month.ToString(); } else { Bulan = Month.ToString(); }
                        var D = Year + "-" + Bulan + "-01";
                        string sql = string.Format(@"SELECT CONVERT(varchar(10),DATEADD(dd,-DAY(DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))), DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))),120) as Tanggal");
                        var data = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
                        var sql2 = string.Format(@"SELECT COUNT(*) FROM dbo.GetWorkingDays_V2('" + data + "')");
                        var data2 = ctx.Database.SqlQuery<int>(sql2).FirstOrDefault();
                        var sql3 = string.Format(@"SELECT COUNT(*) FROM dbo.GetWorkingDays_V2('" + EndDate + "')");
                        var data3 = ctx.Database.SqlQuery<int>(sql3).FirstOrDefault();
                        var sql4 = string.Format(@"SELECT CONVERT(varchar(10),DATEADD(month, ((YEAR('" + D + "') - 1900) * 12) + MONTH('" + D + "'), -1),120) as Tanggal");
                        var data4 = ctx.Database.SqlQuery<string>(sql4).FirstOrDefault();

                        var p = 0;
                        var q = 0;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Faktur";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Value = row[dt1.Columns[0].Caption]; 
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Range("A" + lastRow + ":A" + (lastRow - 3)).Merge();
                        for (int i = 4; i < dt1.Columns.Count - 1; i++)
                        {
                            var a = i + 1;
                            p = p + Convert.ToInt32(row[dt1.Columns[a].Caption]);
                            q = p == 0 ? q : p;
                            if ((i - 3) > Convert.ToInt32(data4.Substring(8, 2)) || ((i - 3) > (Convert.ToInt32(Periode.Substring(0, 2))) && Convert.ToInt32(EndDate.Substring(5, 2)) == Month)) { p = 0; }

                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = p.ToString() == "0" ? (i - 3) > Convert.ToInt32(data4.Substring(8, 2)) || ((i - 3) > (Convert.ToInt32(Periode.Substring(0, 2))) && Convert.ToInt32(EndDate.Substring(5, 2)) == Month) ? "" : p.ToString() : p.ToString(); //row[dt1.Columns[a].Caption];
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            if ((i - 3) == Convert.ToInt32(Periode.Substring(0, 2)))
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                            }
                            var Holiday = ctx.gnMstCalendar.Where(z => z.CalendarDate.Year == Year
                                                        && z.CalendarDate.Month == Month
                                                        && z.CalendarDate.Day == (i - 3)).FirstOrDefault();
                            if (Holiday != null)
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Orange;
                            }

                            //Mark WorkDay Compare
                            foreach (DataRow row2 in dtx.Rows)
                            {
                                if (Month == Convert.ToInt32((row2[dtx.Columns[0].Caption])) && (i - 3) == Convert.ToInt32(row2[dtx.Columns[2].Caption]))
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Font.FontColor = XLColor.Red;
                                }
                                if (Month == Convert.ToInt32((row2[dtx.Columns[0].Caption])) && (i - 3) == Convert.ToInt32(row2[dtx.Columns[3].Caption]))
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Font.FontColor = XLColor.Red;
                                }
                            }


                        }
                        if (Convert.ToInt32(D.Substring(5,2)) == Convert.ToInt32(EndDate.Substring(5,2)))
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Value = "Day " + data3 + " from " + data2 + " days";
                        }
                        else
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Value = q.ToString() + "/" + data2.ToString() + "days=" + (p / data2 + "/day");
                        
                        }
                        lastRow++;
                    }
                }
                #endregion

                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GenerateDataFakturPolisi_ByType";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Date", EndDate);
                cmd.Parameters.AddWithValue("@Dealer", Dealer);
                SqlDataAdapter da2 = new SqlDataAdapter(cmd);
                DataSet ds2 = new DataSet();
                DataTable dt2 = new DataTable();
                da2.Fill(ds2);
                dt2 = ds2.Tables[0];

                #region ByType
                var Type = "";
                var TypeLast = "";
                foreach (DataRow row2 in dt2.Rows)
                {
                    Type = (row2[dt2.Columns[1].Caption]).ToString();

                    if (TypeLast != Type)
                    {
                        lastRow++;
                        tgl = 0;
                        ws.Range("A" + lastRow + ":C" + lastRow).Merge();
                        ws.Cell("A" + lastRow).Value = Type;
                        ws.Cell("A" + lastRow).Style.Font.SetBold();
                        ws.Range("A" + lastRow + ":C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + lastRow + ":C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + lastRow + ":C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + lastRow + ":C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        for (int i = 1 + 3; i <= 31 + 3; i++)
                        {
                            tgl++;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = tgl;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Font.SetBold();
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            if ((i - 3) == Convert.ToInt32(Periode.Substring(0, 2)))
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                            }
                        }
                        lastRow++;
                    }

                    var NO = row2[dt2.Columns[2].Caption];
                    var Year = Convert.ToInt32((row2[dt2.Columns[3].Caption]));
                    var Month = Convert.ToInt32((row2[dt2.Columns[4].Caption]));
                    var Bulan = "";

                    if (NO.ToString() == "1000")
                    {
                        if (Month.ToString().Length == 1) { Bulan = "0" + Month.ToString(); } else { Bulan = Month.ToString(); }
                        var D = Year + "-" + Bulan + "-01";
                        string sql = string.Format(@"SELECT CONVERT(varchar(10),DATEADD(dd,-DAY(DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))), DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))),120) as Tanggal");
                        var data = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
                        var sql2 = string.Format(@"SELECT COUNT(*) FROM dbo.GetWorkingDays_V2('" + data + "')");
                        var data2 = ctx.Database.SqlQuery<int>(sql2).FirstOrDefault();
                        var sql3 = string.Format(@"SELECT COUNT(*) FROM dbo.GetWorkingDays_V2('" + EndDate + "')");
                        var data3 = ctx.Database.SqlQuery<int>(sql3).FirstOrDefault();

                        var p = 0;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "SPK";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Value = row2[dt2.Columns[0].Caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        for (int i = 4; i < dt1.Columns.Count - 1; i++)
                        {
                            var a = i + 1;
                            p = p + Convert.ToInt32(row2[dt2.Columns[a].Caption]);
                            if ((i - 3) > Convert.ToInt32(Periode.Substring(0, 2))) { p = 0; }

                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = p.ToString() == "0" ? (i - 3) > Convert.ToInt32(Periode.Substring(0, 2)) ? "" : p.ToString() : p.ToString();
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            if ((i - 3) == Convert.ToInt32(Periode.Substring(0, 2)))
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Font.FontColor = XLColor.Red;
                            }
                            var Holiday = ctx.gnMstCalendar.Where(z => z.CalendarDate.Year == Year
                                                        && z.CalendarDate.Month == Month
                                                        && z.CalendarDate.Day == (i - 3)).FirstOrDefault();
                            if (Holiday != null)
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Orange;
                            }
                        }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Value = "Day " + data3 + " from " + data2 + " days";
                        lastRow++;
                    }
                    else if (NO.ToString() == "2000")
                    {
                        var D = Year + "-" + Month + "-01";
                        string sql = string.Format(@"SELECT CONVERT(varchar(10),DATEADD(dd,-DAY(DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))), DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))),120) as Tanggal");
                        var data = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
                        var sql2 = string.Format(@"SELECT COUNT(*) FROM dbo.GetWorkingDays_V2('" + data + "')");
                        var data2 = ctx.Database.SqlQuery<int>(sql2).FirstOrDefault(); 

                        var p = 0;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Stock";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Value = row2[dt2.Columns[0].Caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        for (int i = 4; i < dt1.Columns.Count - 1; i++)
                        {
                            var a = i + 1;
                            p = p + Convert.ToInt32(row2[dt2.Columns[a].Caption]);
                            if ((i - 3) > Convert.ToInt32(Periode.Substring(0, 2))) { p = 0; }

                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = p.ToString() == "0" ? (i - 3) > Convert.ToInt32(Periode.Substring(0, 2)) ? "" : p.ToString() : p.ToString();
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            if ((i - 3) == Convert.ToInt32(Periode.Substring(0, 2)))
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Font.FontColor = XLColor.Red;
                            }
                            var Holiday = ctx.gnMstCalendar.Where(z => z.CalendarDate.Year == Year
                                                        && z.CalendarDate.Month == Month
                                                        && z.CalendarDate.Day == (i - 3)).FirstOrDefault();
                            if (Holiday != null)
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Orange;
                            }
                        }
                        //ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Value = p.ToString() + "/" + data2.ToString() + "days=" + (p / data2 + "/day");
                        lastRow++;
                    }
                    else
                    {
                        if (Month.ToString().Length == 1) { Bulan = "0" + Month.ToString(); } else { Bulan = Month.ToString(); }
                        var D = Year + "-" + Bulan + "-01";
                        string sql = string.Format(@"SELECT CONVERT(varchar(10),DATEADD(dd,-DAY(DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))), DATEADD(mm,1,(DATEADD(mm,-1,'" + D + "')))),120) as Tanggal");
                        var data = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
                        var sql2 = string.Format(@"SELECT COUNT(*) FROM dbo.GetWorkingDays_V2('" + data + "')");
                        var data2 = ctx.Database.SqlQuery<int>(sql2).FirstOrDefault();
                        var sql3 = string.Format(@"SELECT COUNT(*) FROM dbo.GetWorkingDays_V2('" + EndDate + "')");
                        var data3 = ctx.Database.SqlQuery<int>(sql3).FirstOrDefault();
                        var sql4 = string.Format(@"SELECT CONVERT(varchar(10),DATEADD(month, ((YEAR('" + D + "') - 1900) * 12) + MONTH('" + D + "'), -1),120) as Tanggal");
                        var data4 = ctx.Database.SqlQuery<string>(sql4).FirstOrDefault();

                        var p = 0;
                        var q = 0;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Faktur";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Value = row2[dt1.Columns[0].Caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        for (int i = 4; i < dt1.Columns.Count - 1; i++)
                        {
                            var a = i + 1;
                            p = p + Convert.ToInt32(row2[dt2.Columns[a].Caption]);
                            q = p == 0 ? q : p;
                            if ((i - 3) > Convert.ToInt32(data4.Substring(8, 2)) || ((i - 3) > (Convert.ToInt32(Periode.Substring(0, 2))) && Convert.ToInt32(EndDate.Substring(5, 2)) == Month)) { p = 0; }

                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = p.ToString() == "0" ? (i - 3) > Convert.ToInt32(data4.Substring(8, 2)) || ((i - 3) > (Convert.ToInt32(Periode.Substring(0, 2))) && Convert.ToInt32(EndDate.Substring(5, 2)) == Month) ? "" : p.ToString() : p.ToString(); 
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            if ((i - 3) == Convert.ToInt32(Periode.Substring(0, 2)))
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Yellow;
                            }
                            var Holiday = ctx.gnMstCalendar.Where(z => z.CalendarDate.Year == Year
                                                        && z.CalendarDate.Month == Month
                                                        && z.CalendarDate.Day == (i - 3)).FirstOrDefault();
                            if (Holiday != null)
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.Orange;
                            }

                            //Mark WorkDay Compare
                            foreach (DataRow row3 in dtx.Rows)
                            {
                                if (Month == Convert.ToInt32((row3[dtx.Columns[0].Caption])) && (i - 3) == Convert.ToInt32(row3[dtx.Columns[2].Caption]))
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Font.FontColor = XLColor.Red;
                                }
                                if (Month == Convert.ToInt32((row3[dtx.Columns[0].Caption])) && (i - 3) == Convert.ToInt32(row3[dtx.Columns[3].Caption]))
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Font.FontColor = XLColor.Red;
                                }
                            }
                        }
                        if (Convert.ToInt32(D.Substring(5, 2)) == Convert.ToInt32(EndDate.Substring(5, 2)))
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Value = "Day " + data3 + " from " + data2 + " days";
                        }
                        else
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(3), lastRow)).Value = q.ToString() + "/" + data2.ToString() + "days=" + (p / data2 + "/day");

                        }
                        lastRow++;
                    }

                    TypeLast = (row2[dt2.Columns[1].Caption]).ToString();
                   
                }
                #endregion

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

                Workbook book = new Workbook();
                book.LoadFromFile(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                Worksheet sheet = book.Worksheets["FP Polisi"];
                Worksheet sheet2 = book.Worksheets["Char FP Polisi"];

                #region Char ALL Type
                sheet.Range["A6:AH11"].NumberFormat = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";

                //Add chart and set chart data range
                Chart chart = sheet2.Charts.Add(ExcelChartType.Line);
                chart.DataRange = sheet.Range["A6:AH11"];
                chart.SeriesDataFromRange = true;

                //Chart border  
                chart.ChartArea.Border.Weight = ChartLineWeightType.Medium;

                //Chart position  
                chart.LeftColumn = 1;
                chart.TopRow = 5;
                chart.RightColumn = 20;
                chart.BottomRow = 21;

                //Chart title  
                chart.ChartTitle = "CHART FAKTUR POLISI " + "ALL TYPE";
                chart.ChartTitleArea.Font.FontName = "Calibri";
                chart.ChartTitleArea.Font.Size = 13;
                chart.ChartTitleArea.Font.IsBold = true;


                //Chart axis  
                chart.PrimaryCategoryAxis.Title = " ";
                chart.PrimaryValueAxis.Title = " ";
                chart.PrimaryValueAxis.TitleArea.TextRotationAngle = 90;
                chart.PrimaryValueAxis.HasMajorGridLines = false;

                //Chart legend  
                chart.Legend.Position = LegendPositionType.Top;
                //book.SaveToFile("Result.xlsx", ExcelVersion.Version2010);
                #endregion

                book.SaveToFile(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

                Type = "";
                TypeLast = "";
                var rangeA1 = 14; var rangeZ1 = 19;
                var TopRow1 = 22; var BottomRow1 = 37;

                foreach (DataRow row2 in dt2.Rows)
                {
                    Type = (row2[dt2.Columns[1].Caption]).ToString();

                    if (TypeLast != Type)
                    {
                        #region Char By Type
                        sheet.Range["A" + rangeA1 + ":AH" + rangeZ1].NumberFormat = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";

                        //Add chart and set chart data range
                        Chart chart1 = sheet2.Charts.Add(ExcelChartType.Line);
                        chart1.DataRange = sheet.Range["A" + rangeA1 + ":AH" + rangeZ1];
                        chart1.SeriesDataFromRange = true;

                        chart1.ChartArea.Border.Weight = ChartLineWeightType.Medium;

                        chart1.LeftColumn = 1;
                        chart1.TopRow = TopRow1;
                        chart1.RightColumn = 20;
                        chart1.BottomRow = BottomRow1;

                        chart1.ChartTitle = "CHART FAKTUR POLISI " + Type;
                        chart1.ChartTitleArea.Font.FontName = "Calibri";
                        chart1.ChartTitleArea.Font.Size = 13;
                        chart1.ChartTitleArea.Font.IsBold = true;

                        chart1.PrimaryCategoryAxis.Title = " ";
                        chart1.PrimaryValueAxis.Title = " ";
                        chart1.PrimaryValueAxis.TitleArea.TextRotationAngle = 90;
                        chart1.PrimaryValueAxis.HasMajorGridLines = false;

                        chart1.Legend.Position = LegendPositionType.Top;

                        rangeA1 = rangeA1 + 8; rangeZ1 = rangeZ1 + 8;
                        TopRow1 = TopRow1 + 17; BottomRow1 = BottomRow1 + 17;

                        #endregion
                    }

                    TypeLast = (row2[dt2.Columns[1].Caption]).ToString();
                }

                book.SaveToFile(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
        }
    }
}