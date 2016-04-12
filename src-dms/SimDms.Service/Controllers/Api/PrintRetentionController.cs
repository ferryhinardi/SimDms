using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Diagnostics;using ClosedXML.Excel;
using System.Data;
using System.Data.SqlClient;

namespace SimDms.Service.Controllers.Api
{
    public class PrintRetentionController : BaseController
    {
        public JsonResult Default()
        {
            var date = DateTime.Now.Date;
            return Json(new
            {
                ServiceDateFrom = date,
                ServiceDateTo = date,
                ReminderDateFrom = date,
                ReminderDateTo = date,
                FollowUpDateFrom = date,
                FollowUpDateTo = date,
                Priode = date
            });
        }

        public ActionResult SvRpCrm002V2(string DateParam, string OptionType, string Range, string Interval, string Priode)
        {

            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "SvRpCrm002V2";
            //fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_SvRpCrm002V2"; // "usprpt_SvRpCrm002V3";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateParam", DateParam);
            cmd.Parameters.AddWithValue("@OptionType", OptionType);
            cmd.Parameters.AddWithValue("@Range", Convert.ToInt32(Range));
            cmd.Parameters.AddWithValue("@Interval", Convert.ToInt32(Interval));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 5;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Report Retention");

                //First Names  
                ws.Range("A2", "B2").Merge();
                ws.Range("A3", "B3").Merge();
                ws.Range("A4", "B4").Merge();
                ws.Range("G4", "H4").Merge();
                ws.Cell("A2").Value = "Update";
                ws.Cell("C2").Value = ": " + DateTime.Now;
                ws.Cell("A3").Value = "Priode";
                ws.Cell("C3").Value = ": " + Priode;
                ws.Cell("A4").Value = "Beres S";
                ws.Cell("C4").Value = ": " + CompanyName;
                ws.Cell("G4").Value = "Nama SMR";
                ws.Cell("I4").Value = ": " + CurrentUser.UserId;
                //ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(12);

                ws.Range("A5:A6").Merge();
                ws.Range("A5:A6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("A5:A6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("A5:A6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A5:A6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("B5:D6").Merge();
                ws.Range("B5:D6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("B5:D6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("B5:D6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("B5:D6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("E5:E6").Merge();
                ws.Range("E5:E6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("E5:E6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("E5:E6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("E5:E6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("F5:F6").Merge();
                ws.Range("F5:F6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("F5:F6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("F5:F6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("F5:F6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("G5:O5").Merge();
                ws.Range("G5:O5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("G5:O5").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("G5:O5").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("G5:O5").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("H6:J6").Merge();
                ws.Range("H6:J6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("H6:J6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("H6:J6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("H6:J6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("Q5:Q6").Merge();
                ws.Range("Q5:Q6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("Q5:Q6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("Q5:Q6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("Q5:Q6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("R5:R6").Merge();
                ws.Range("R5:R6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("R5:R6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("R5:R6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("R5:R6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("S5:S6").Merge();
                ws.Range("S5:S6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("S5:S6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("S5:S6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("S5:S6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("T5:T6").Merge();
                ws.Range("T5:T6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("T5:T6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("T5:T6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("T5:T6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("U5:U6").Merge();
                ws.Range("U5:U6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("U5:U6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("U5:U6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("U5:U6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("V5:V6").Merge();
                ws.Range("V5:V6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("V5:V6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("V5:V6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("V5:V6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("W5:W6").Merge();
                ws.Range("W5:W6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("W5:W6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("W5:W6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("W5:W6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("X5:X6").Merge();
                ws.Range("X5:X6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("X5:X6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("X5:X6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("X5:X6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("Y5:Y6").Merge();
                ws.Range("Y5:Y6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("Y5:Y6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("Y5:Y6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("Y5:Y6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("Z5:Z6").Merge();
                ws.Range("Z5:Z6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("Z5:Z6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("Z5:Z6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("Z5:Z6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("AA5:AA6").Merge();
                ws.Range("AA5:AA6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("AA5:AA6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("AA5:AA6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("AA5:AA6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("AB5:AB6").Merge();
                ws.Range("AB5:AB6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("AB5:AB6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("AB5:AB6").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("AB5:AB6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Columns("1").Width = 5;	
                ws.Columns("2").Width = 5;	
                ws.Columns("3").Width = 5;	
                ws.Columns("4").Width = 5;	
                ws.Columns("5").Width = 15;	
                ws.Columns("6").Width = 25;
                ws.Columns("7").Width = 10;	
                ws.Columns("8").Width = 5;	
                ws.Columns("9").Width = 5;	
                ws.Columns("10").Width = 5;	
                ws.Columns("11").Width = -1;	
                ws.Columns("12").Width = 15;	
                ws.Columns("13").Width = 15;	
                ws.Columns("14").Width = 15;	
                ws.Columns("15").Width = 15;	
                ws.Columns("16").Width = -1;	
                ws.Columns("17").Width = 60;	
                ws.Columns("18").Width = 30;	
                ws.Columns("19").Width = 30;	
                ws.Columns("20").Width = 30;	
                ws.Columns("21").Width = 30;	
                ws.Columns("22").Width = 30;	
                ws.Columns("23").Width = 30;	
                ws.Columns("24").Width = 80;	
                ws.Columns("25").Width = 50;	
                ws.Columns("26").Width = 30;	
                ws.Columns("27").Width = 30;
                ws.Columns("28").Width = 60;

                ws.Cell("A5").Value = "No";
                ws.Cell("B5").Value = "Kategori Pelanggan";
                ws.Cell("E5").Value = "Tipe";
                ws.Cell("F5").Value = "No Polisi";
                ws.Cell("G5").Value = "SPesifikasi Unit";
                ws.Cell("G6").Value = "TM";
                ws.Cell("G6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("G6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("H6").Value = "Tahun Pembuatan";
                ws.Cell("L6").Value = "Kode Mesin";
                ws.Cell("L6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("L6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("M6").Value = "Nomor Mesin";
                ws.Cell("M6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("M6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("N6").Value = "Kode Rangka";
                ws.Cell("N6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("N6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("O6").Value = "Nomor Rangka";
                ws.Cell("O6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("O6").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q5").Value = "Nama Pelanggan";
                ws.Cell("R5").Value = "No. Telp Rumah";
                ws.Cell("S5").Value = "No. Telp kantor";
                ws.Cell("T5").Value = "No. HP";
                ws.Cell("U5").Value = "Tanggal Unit";
                ws.Cell("V5").Value = "Tanggal Lahir";
                ws.Cell("W5").Value = "Jenis Kelamin";
                ws.Cell("X5").Value = "Alamat";
                ws.Cell("Y5").Value = "Nama Kontak";
                ws.Cell("Z5").Value = "Alamat Kontak";
                ws.Cell("AA5").Value = "Telepon Kontak";
                ws.Cell("AB5").Value = "Keterangan";

                lastRow = lastRow + 1;
                lastRow++;

                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    ws.Cell("A" + lastRow).Value = dt.Rows[i].ItemArray[0];
                    ws.Cell("A" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("A" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Range("B" + lastRow + ":D" + lastRow).Merge();
                    ws.Cell("B" + lastRow).Value = dt.Rows[i].ItemArray[1];
                    ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("B" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("E" + lastRow).Value = dt.Rows[i].ItemArray[1];
                    ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("E" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("F" + lastRow).Value = dt.Rows[i].ItemArray[3];
                    ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("F" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("G" + lastRow).Value = dt.Rows[i].ItemArray[4];
                    ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("G" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Range("H" + lastRow + ":J" + lastRow).Merge();
                    ws.Cell("H" + lastRow).Value = dt.Rows[i].ItemArray[5];
                    ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("H" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("L" + lastRow).Value = dt.Rows[i].ItemArray[6];
                    ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("L" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("M" + lastRow).Value = dt.Rows[i].ItemArray[7];
                    ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("M" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("N" + lastRow).Value = dt.Rows[i].ItemArray[8];
                    ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("N" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("O" + lastRow).Value = dt.Rows[i].ItemArray[9];
                    ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("O" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("Q" + lastRow).Style.Alignment.SetWrapText();
                    ws.Cell("Q" + lastRow).Value = dt.Rows[i].ItemArray[10];
                    ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("Q" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("R" + lastRow).Value = dt.Rows[i].ItemArray[11];
                    ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("R" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("S" + lastRow).Value = dt.Rows[i].ItemArray[12];
                    ws.Cell("S" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("S" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("T" + lastRow).Value = dt.Rows[i].ItemArray[13];
                    ws.Cell("T" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("T" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("U" + lastRow).Value = dt.Rows[i].ItemArray[14];
                    ws.Cell("U" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("U" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("V" + lastRow).Value = dt.Rows[i].ItemArray[15];
                    ws.Cell("V" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("V" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("W" + lastRow).Value = dt.Rows[i].ItemArray[16];
                    ws.Cell("W" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("W" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("X" + lastRow).Style.Alignment.SetWrapText();
                    ws.Cell("X" + lastRow).Value = dt.Rows[i].ItemArray[17];
                    ws.Cell("X" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("X" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("Y" + lastRow).Value = dt.Rows[i].ItemArray[18];
                    ws.Cell("Y" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("Y" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("Z" + lastRow).Value = dt.Rows[i].ItemArray[19];
                    ws.Cell("Z" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("Z" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("AA" + lastRow).Value = dt.Rows[i].ItemArray[20];
                    ws.Cell("AA" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("AA" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("AB" + lastRow).Value = dt.Rows[i].ItemArray[21];
                    ws.Cell("AB" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("AB" + lastRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


                    lastRow++;
                }
                //return GenerateExcel(wb, dt, lastRow, fileName, true);
                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
        }

        public ActionResult SvRpCrm003V2(DateTime DateParam, string OptionType, string Range, string Interval, string IncPDI, string ServiceDateFrom, string ServiceDateTo, string ReminderDateFrom, string ReminderDateTo, string FollowUpDateFrom, string FollowUpDateTo, string PrintOption)
        {
            if (ServiceDateFrom == null) { ServiceDateFrom = ""; } if (ServiceDateTo == null) { ServiceDateTo = ""; }
            if (ReminderDateFrom == null) { ReminderDateFrom = ""; } if (ReminderDateTo == null) { ReminderDateTo = ""; }
            if (FollowUpDateFrom == null) { FollowUpDateFrom = ""; } if (FollowUpDateTo == null) { FollowUpDateTo = ""; }
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "SvRpCrm003V2";
            //fileName = fileName + sFromDate + '_' + sEndDate;
            if (PrintOption == "1")
            {
                ServiceDateFrom = "";
                ServiceDateTo = "";
                ReminderDateFrom = "";
                ReminderDateTo = "";
                FollowUpDateFrom = "";
                FollowUpDateTo = "";
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_SvRpCrm003V2";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateParam", DateParam.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@OptionType", OptionType);
            cmd.Parameters.AddWithValue("@Range", Convert.ToInt32(Range));
            cmd.Parameters.AddWithValue("@Interval", Convert.ToInt32(Interval));
            cmd.Parameters.AddWithValue("@IncPDI", Convert.ToBoolean(IncPDI) );
            cmd.Parameters.AddWithValue("@ServiceDateFrom", ServiceDateFrom);
            cmd.Parameters.AddWithValue("@ServiceDateTo", ServiceDateTo);
            cmd.Parameters.AddWithValue("@ReminderDateFrom", ReminderDateFrom);
            cmd.Parameters.AddWithValue("@ReminderDateTo", ReminderDateTo);
            cmd.Parameters.AddWithValue("@FollowUpDateFrom", FollowUpDateFrom);
            cmd.Parameters.AddWithValue("@FollowUpDateTo", FollowUpDateTo);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 6;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Data Retensi Harian");
                //First Names  
                ws.Cell("A2").Value = "Update";
                ws.Cell("B2").Value = ": " + DateTime.Now.Date;
                ws.Cell("A3").Value = "Priode";
                ws.Cell("B3").Value = ": " + DateTime.Now.Month + DateTime.Now.Year;
                ws.Cell("A4").Value = "Beres S";
                ws.Cell("B4").Value = ": " + CompanyName;
                ws.Cell("G4").Value = "Nama SMR";
                ws.Cell("H4").Value = ": " + CurrentUser.UserId;


                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public JsonResult SvRpCrm004(string Year)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "SvRpCrm004";
            //fileName = fileName + sFromDate + '_' + sEndDate;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_SvRpCrm004New";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Year", Year);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            
            List<string> sheets = new List<string>();
            foreach (DataTable dt in ds.Tables)
            {
                sheets.Add(dt.TableName);
            }

            return GenerateReportXls(ds, sheets, fileName);
            
            //DataTable dt = new DataTable("datTable1");
            //da.Fill(dt);
            //if (dt.Rows.Count == 0)
            //{
            //    return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    int lastRow = 1;

            //    var wb = new XLWorkbook();
            //    var ws = wb.Worksheets.Add("Data Retensi Harian");

            //    return GenerateExcel(wb, dt, lastRow, fileName);
            //}
        }


        private ActionResult GenerateExcel(XLWorkbook wb, DataTable dt, int lastRow, string fileName, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var ws = wb.Worksheet(1);
            var tmpLastRow = lastRow;

            int iCol = 1;
            char iChar = 'A';
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 1;
                iChar = 'A';
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,##0";
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRow == lastRow)
                    {
                        ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                        ws.Cell(lastRow, iCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(12);
                        ws.Cell(lastRow, iCol).Style.Border.TopBorder = XLBorderStyleValues.Double;
                        ws.Cell(lastRow, iCol).Style.Border.BottomBorder = XLBorderStyleValues.Double;

                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }
                    else
                    {
                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }

                    iCol++;
                    iChar++;
                }

                lastRow++;
            }

            if (isShowSummary)
            {
                ws.Cell(lastRow + 1, 1).Value = "TOTAL";
                int j = 2;
                for (char i = 'B'; i < iChar; i++)
                {
                    ws.Cell(lastRow + 1, j).FormulaA1 = "=SUM(" + i + (tmpLastRow + 1) + ":" + i + lastRow + ")";
                    j++;
                }
            }

            var rngTable = ws.Range(tmpLastRow + 1, 1, lastRow + 1, iCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);


            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

    }
}
