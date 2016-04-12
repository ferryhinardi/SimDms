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
    public class InquiryITSController : BaseController
    {
        public ActionResult GenerateITSReportSPKWorkDay()
        {
            string StartDate = Request["StartDate"];
            string EndDate = Request["EndDate"];
            string TipeKendaraan = Request["TipeKendaraan"] ?? "";
            string StartDateName = Request["StartDateName"];
            string EndDateName = Request["EndDateName"];
            string fileName = "SPK Work Day";
            string Tipe = "";
            var KendaraanArray = new string[] { "one", "two", "three" };
            var exceptionList = new List<string> { "A-STAR", "BALENO", "ESTILO", "ST100", "ST100-PU", "SX4", "OTHERS" };
            if (TipeKendaraan != "")
            {
                KendaraanArray = TipeKendaraan.Split(',');
            }
            else
            {
                KendaraanArray = ctx.MsMstGroupModels.Where(e => !exceptionList.Contains(e.GroupModel))
                    .Select(c => c.GroupModel.ToString()).Distinct().ToArray();
            }

            if (TipeKendaraan != "") { Tipe = TipeKendaraan; } else { Tipe = "ALL TYPE"; }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GenerateITSSPKWorkDay";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@TipeKendaraan", "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            DataTable dt4 = new DataTable();
            DataTable dt11 = new DataTable();

            da.Fill(ds);
            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];
            dt3 = ds.Tables[2];
            dt4 = ds.Tables[3];
            dt11 = ds.Tables[4];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;
                int lastColHeader = 0;
                var accum = 0;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Nasional Area");
                var ws2 = wb.Worksheets.Add("Nasional Type");
                var ws3 = wb.Worksheets.Add("Jabodetabek Type");
                var ws4 = wb.Worksheets.Add("Non Jabodetabek Type");

                #region NASIONAL

                ws.Columns("1").Width = 10;
                ws.Range("A" + lastRow + ":AA" + lastRow).Merge();
                var RowTitelA1 = lastRow;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A" + lastRow).Value = "WORK DAY SPK NASIONAL";
                lastRow = 3;
                ws.Columns("1").Width = 20;
                ws.Columns(2, 26).Width = 5;
                ws.Columns("27").Width = 7;
                ws.Range("A" + lastRow + ":B" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = "ALL TYPE";
                ws.Range("T" + lastRow + ":AA" + lastRow).Merge();
                ws.Cell("T" + lastRow).Value = "Periode " + StartDateName + " S/D " + EndDateName;
                lastRow++;

                #region Work Day (Acumm)
                var tgl = 0;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Work Day (Accum)";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                for (int i = 2; i <= 26; i++)
                {
                    tgl++;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = tgl;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastColHeader = i;
                }
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = "Total";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Value = "Grwth";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                lastRow++;
                var LastAccumbefore = 0;
                var Grwth = 0;
                var x = 0;
                var LastAccum = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = row[dt1.Columns[1].Caption];
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i < dt1.Columns.Count; i++)
                    {
                        var a = i;
                        var inq = Convert.ToInt32(row[dt1.Columns[a].Caption]);
                        accum = accum + inq;
                        LastAccum = LastAccum + inq;
                        if (x != accum || x == 0) { accum = accum; } else { accum = 0; }
                        var NilaiAcum = "";
                        if (accum != 0) { NilaiAcum = Convert.ToString(accum); }
                        if (LastAccum != LastAccumbefore) { Grwth = i; }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = NilaiAcum;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        x = accum;
                        LastAccumbefore = LastAccum;
                        lastColHeader = i;
                    }
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = LastAccum;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    accum = 0;
                    LastAccum = 0;
                    lastRow++;
                }
                var row1 = lastRow - 1;
                var row2 = lastRow - 2;
                var GrwthA1 = lastRow - 2;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow - 2)).SetFormulaA1("=" + GetExcelColumnName(Grwth) + row1 + "/" + GetExcelColumnName(Grwth) + row2 + "-1");
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow - 2)).Style.NumberFormat.Format = "0.0%";
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 1, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                LastAccumbefore = 0;
                Grwth = 0;
                #endregion

                #region Work Day
                lastRow++;
                tgl = 0;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Work Day";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                for (int i = 2; i <= 26; i++)
                {
                    tgl++;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = tgl;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastColHeader = i;
                }
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = "Total";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Value = "Grwth";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                lastRow++;

                x = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = row[dt1.Columns[1].Caption];
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i < dt1.Columns.Count; i++)
                    {
                        var a = i;
                        var inq = Convert.ToInt32(row[dt1.Columns[a].Caption]);
                        accum = accum + inq;
                        if (x != accum || x == 0 || inq == 0) { accum = accum; } else { accum = 0; }
                        var b = row[dt1.Columns[a].Caption];
                        var nilai = "";
                        if (Convert.ToString(b) != "0") { nilai = Convert.ToString(b); }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = nilai;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        x = accum;
                        lastColHeader = i;
                    }
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = accum;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    accum = 0;
                    lastRow++;
                }
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow - 2)).SetFormulaA1("=AB" + (lastRow - 7));
                ws.Range(lastRow - 2, lastColHeader + 2, lastRow - 2, lastColHeader + 2).AddConditionalFormat()
                         .IconSet(XLIconSetStyle.ThreeArrows, false, true)
                         .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, -1, XLCFContentType.Number)
                         .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number)
                         .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number);
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 1, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                var BackRow = lastRow - 3;
                foreach (DataRow row in dt11.Rows)
                {
                    for (int i = 2; i < dt11.Columns.Count; i++)
                    {
                        var b = row[dt1.Columns[i].Caption];
                        if (Convert.ToString(b) != "0") {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), BackRow))
                                .Style.Fill.SetBackgroundColor(XLColor.Gray);
                        }
                        lastColHeader = i;
                    }
                    lastColHeader = 1;
                    BackRow++;
                }

                #endregion

                lastRow = lastRow + 18;

                #endregion

                #region JABODETABEK

                ws.Range("A" + lastRow + ":AA" + lastRow).Merge();
                var RowTitelA2 = lastRow;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A" + lastRow).Value = "WORK DAY SPK JABODETABEK";
                lastRow = lastRow + 2;
                ws.Columns("1").Width = 20;
                ws.Columns(2, 26).Width = 5;
                ws.Columns("27").Width = 7;
                ws.Range("A" + lastRow + ":B" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = "ALL TYPE";
                ws.Range("T" + lastRow + ":AA" + lastRow).Merge();
                ws.Cell("T" + lastRow).Value = "Periode " + StartDateName + " S/D " + EndDateName;
                lastRow++;

                #region Work Day (Acumm)
                tgl = 0;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Work Day (Accum)";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                for (int i = 2; i <= 26; i++)
                {
                    tgl++;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = tgl;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastColHeader = i;
                }
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = "Total";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Value = "Grwth";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                lastRow++;

                x = 0;
                LastAccum = 0;
                foreach (DataRow row in dt2.Rows)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = row[dt2.Columns[1].Caption];
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i < dt2.Columns.Count; i++)
                    {
                        var a = i;
                        var inq = Convert.ToInt32(row[dt2.Columns[a].Caption]);
                        accum = accum + inq;
                        LastAccum = LastAccum + inq;
                        if (x != accum || x == 0) { accum = accum; } else { accum = 0; }
                        var NilaiAcum = "";
                        if (accum != 0) { NilaiAcum = Convert.ToString(accum); }
                        if (LastAccum != LastAccumbefore) { Grwth = i; }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = NilaiAcum;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        x = accum;
                        LastAccumbefore = LastAccum;
                        lastColHeader = i;
                    }
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = LastAccum;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    accum = 0;
                    LastAccum = 0;
                    lastRow++;
                }
                row1 = lastRow - 1;
                row2 = lastRow - 2;
                var GrwthA2 = lastRow - 2;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow - 2)).SetFormulaA1("=" + GetExcelColumnName(Grwth) + row1 + "/" + GetExcelColumnName(Grwth) + row2 + "-1");
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow - 2)).Style.NumberFormat.Format = "0.0%";
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 1, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                LastAccumbefore = 0;
                Grwth = 0;
                #endregion

                #region Work Day
                lastRow++;
                tgl = 0;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Work Day";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                for (int i = 2; i <= 26; i++)
                {
                    tgl++;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = tgl;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastColHeader = i;
                }
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = "Total";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Value = "Grwth";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                lastRow++;

                x = 0;
                foreach (DataRow row in dt2.Rows)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = row[dt2.Columns[1].Caption];
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i < dt2.Columns.Count; i++)
                    {
                        var a = i;
                        var inq = Convert.ToInt32(row[dt2.Columns[a].Caption]);
                        accum = accum + inq;
                        if (x != accum || x == 0 || inq == 0) { accum = accum; } else { accum = 0; }
                        var b = row[dt2.Columns[a].Caption];
                        var nilai = "";
                        if (Convert.ToString(b) != "0") { nilai = Convert.ToString(b); }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = nilai;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        x = accum;
                        lastColHeader = i;
                    }
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = accum;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    accum = 0;
                    lastRow++;
                }
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow - 2)).SetFormulaA1("=AB" + (lastRow - 7));
                ws.Range(lastRow - 2, lastColHeader + 2, lastRow - 2, lastColHeader + 2).AddConditionalFormat()
                         .IconSet(XLIconSetStyle.ThreeArrows, false, true)
                         .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, -1, XLCFContentType.Number)
                         .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number)
                         .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number);
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 1, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                BackRow = lastRow - 3;
                foreach (DataRow row in dt11.Rows)
                {
                    for (int i = 2; i < dt11.Columns.Count; i++)
                    {
                        var b = row[dt1.Columns[i].Caption];
                        if (Convert.ToString(b) != "0")
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), BackRow))
                                .Style.Fill.SetBackgroundColor(XLColor.Gray);
                        }
                        lastColHeader = i;
                    }
                    lastColHeader = 1;
                    BackRow++;
                }
                #endregion

                lastRow = lastRow + 18;

                #endregion

 #region NON JABODETABEK

                ws.Range("A" + lastRow + ":AA" + lastRow).Merge();
                var RowTitelA3 = lastRow;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A" + lastRow).Value = "WORK DAY SPK NON JABODETABEK";
                lastRow = lastRow + 2;
                ws.Columns("1").Width = 20;
                ws.Columns(2, 26).Width = 5;
                ws.Columns("27").Width = 7;
                ws.Range("A" + lastRow + ":B" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = "ALL TYPE";
                ws.Range("T" + lastRow + ":AA" + lastRow).Merge();
                ws.Cell("T" + lastRow).Value = "Periode " + StartDateName + " S/D " + EndDateName;
                lastRow++;

                #region Work Day (Acumm)
                tgl = 0;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Work Day (Accum)";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                for (int i = 2; i <= 26; i++)
                {
                    tgl++;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = tgl;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastColHeader = i;
                }
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = "Total";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Value = "Grwth";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                lastRow++;

                x = 0;
                LastAccum = 0;
                foreach (DataRow row in dt3.Rows)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = row[dt3.Columns[1].Caption];
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i < dt3.Columns.Count; i++)
                    {
                        var a = i;
                        var inq = Convert.ToInt32(row[dt3.Columns[a].Caption]);
                        accum = accum + inq;
                        LastAccum = LastAccum + inq;
                        if (x != accum || x == 0) { accum = accum; } else { accum = 0; }
                        var NilaiAcum = "";
                        if (accum != 0) { NilaiAcum = Convert.ToString(accum); }
                        if (LastAccum != LastAccumbefore) { Grwth = i; }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = NilaiAcum;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        x = accum;
                        LastAccumbefore = LastAccum;
                        lastColHeader = i;
                    }
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = LastAccum;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    accum = 0;
                    LastAccum = 0;
                    lastRow++;
                }
                row1 = lastRow - 1;
                row2 = lastRow - 2;
                var GrwthA3 = lastRow - 2;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow - 2)).SetFormulaA1("=" + GetExcelColumnName(Grwth) + row1 + "/" + GetExcelColumnName(Grwth) + row2 + "-1");
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow - 2)).Style.NumberFormat.Format = "0.0%";
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 1, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                LastAccumbefore = 0;
                Grwth = 0;
                #endregion

                #region Work Day
                lastRow++;
                tgl = 0;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Work Day";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                for (int i = 2; i <= 26; i++)
                {
                    tgl++;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = tgl;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastColHeader = i;
                }
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = "Total";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Value = "Grwth";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                lastRow++;

                x = 0;
                foreach (DataRow row in dt3.Rows)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = row[dt3.Columns[1].Caption];
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i < dt3.Columns.Count; i++)
                    {
                        var a = i;
                        var inq = Convert.ToInt32(row[dt3.Columns[a].Caption]);
                        accum = accum + inq;
                        if (x != accum || x == 0 || inq == 0) { accum = accum; } else { accum = 0; }
                        var b = row[dt3.Columns[a].Caption];
                        var nilai = "";
                        if (Convert.ToString(b) != "0") { nilai = Convert.ToString(b); }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = nilai;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        x = accum;
                        lastColHeader = i;
                    }
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Value = accum;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    accum = 0;
                    lastRow++;
                }
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow - 2)).SetFormulaA1("=AB" + (lastRow - 7));
                ws.Range(lastRow - 2, lastColHeader + 2, lastRow - 2, lastColHeader + 2).AddConditionalFormat()
                         .IconSet(XLIconSetStyle.ThreeArrows, false, true)
                         .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, -1, XLCFContentType.Number)
                         .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number)
                         .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number);
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 3, lastColHeader + 2, lastRow - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(lastRow - 1, lastColHeader + 2, lastRow - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                BackRow = lastRow - 3;
                foreach (DataRow row in dt11.Rows)
                {
                    for (int i = 2; i < dt11.Columns.Count; i++)
                    {
                        var b = row[dt1.Columns[i].Caption];
                        if (Convert.ToString(b) != "0")
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), BackRow))
                                .Style.Fill.SetBackgroundColor(XLColor.Gray);
                        }
                        lastColHeader = i;
                    }
                    lastColHeader = 1;
                    BackRow++;
                }
                #endregion

                lastRow = lastRow + 18;

                #endregion

                var lastRow1 = 1;
                var lastRow2 = 1;
                var lastRow3 = 1;
                lastColHeader = 0;
                accum = 0;

                ws2.Columns("1").Width = 10;
                ws2.Range("A" + lastRow1 + ":AA" + lastRow1).Merge();
                //ws2.Range("A" + lastRow1 + ":AA" + lastRow1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                var RowTitelB1 = lastRow1;
                ws2.Cell("A" + lastRow1).Style.Font.SetBold().Font.SetFontSize(14);
                ws2.Cell("A" + lastRow1).Value = "WORK DAY SPK NASIONAL BY TYPE";
                lastRow1 = 3;

                ws3.Columns("1").Width = 10;
                ws3.Range("A" + lastRow2 + ":AA" + lastRow2).Merge();
                //ws3.Range("A" + lastRow2 + ":AA" + lastRow2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                var RowTitelC1 = lastRow2;
                ws3.Cell("A" + lastRow2).Style.Font.SetBold().Font.SetFontSize(14);
                ws3.Cell("A" + lastRow2).Value = "WORK DAY SPK JABODETABEK BY TYPE";
                lastRow2 = 3;

                ws4.Columns("1").Width = 10;
                ws4.Range("A" + lastRow3 + ":AA" + lastRow3).Merge();
                //ws4.Range("A" + lastRow3 + ":AA" + lastRow3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                var RowTitelD1 = lastRow3;
                ws4.Cell("A" + lastRow3).Style.Font.SetBold().Font.SetFontSize(14);
                ws4.Cell("A" + lastRow3).Value = "WORK DAY SPK NON JABODETABEK BY TYPE";
                lastRow3 = 3;

                for (int z = 0; z < KendaraanArray.Length; z++)
                {
                    string ok = KendaraanArray[z].ToString();
                    cmd.CommandTimeout = 3600;
                    cmd.CommandText = "uspfn_GenerateITSSPKWorkDay";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@StartDate", StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", EndDate);
                    cmd.Parameters.AddWithValue("@TipeKendaraan", KendaraanArray[z].ToString());
                    SqlDataAdapter da2 = new SqlDataAdapter(cmd);
                    DataSet ds2 = new DataSet();
                    da2.Fill(ds2);
                    dt1 = ds2.Tables[0];
                    dt2 = ds2.Tables[1];
                    dt3 = ds2.Tables[2];

                    #region NASIONAL

                    ws2.Columns("1").Width = 20;
                    ws2.Columns(2, 26).Width = 5;
                    ws2.Columns("27").Width = 7;
                    ws2.Range("A" + lastRow1 + ":B" + lastRow1).Merge();
                    ws2.Cell("A" + lastRow1).Value = KendaraanArray[z].ToString();
                    ws2.Range("T" + lastRow1 + ":AA" + lastRow1).Merge();
                    ws2.Cell("T" + lastRow1).Value = "Periode " + StartDateName + " S/D " + EndDateName;
                    lastRow1++;

                    #region Work Day (Acumm)
                    tgl = 0;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Value = "Work Day (Accum)";
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i <= 26; i++)
                    {
                        tgl++;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Value = tgl;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        lastColHeader = i;
                    }
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Value = "Total";
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Value = "Grwth";
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastRow1++;

                    x = 0;
                    LastAccum = 0;
                    foreach (DataRow row in dt1.Rows)
                    {
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Value = row[dt1.Columns[1].Caption];
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        for (int i = 2; i < dt1.Columns.Count; i++)
                        {
                            var a = i;
                            var inq = Convert.ToInt32(row[dt1.Columns[a].Caption]);
                            accum = accum + inq;
                            LastAccum = LastAccum + inq;
                            if (x != accum || x == 0) { accum = accum; } else { accum = 0; }
                            var NilaiAcum = "";
                            if (accum != 0) { NilaiAcum = Convert.ToString(accum); }
                            if (LastAccum != LastAccumbefore) { Grwth = i; }
                            ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Value = NilaiAcum;
                            ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            x = accum;
                            LastAccumbefore = LastAccum;
                            lastColHeader = i;
                        }
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Value = LastAccum;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        accum = 0;
                        LastAccum = 0;
                        lastRow1++;
                    }
                    row1 = lastRow1 - 1;
                    row2 = lastRow1 - 2;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1 - 2)).SetFormulaA1("=" + GetExcelColumnName(Grwth) + row1 + "/" + GetExcelColumnName(Grwth) + row2 + "-1");
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1 - 2)).Style.NumberFormat.Format = "0.0%";
                    ws2.Range(lastRow1 - 3, lastColHeader + 2, lastRow1 - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws2.Range(lastRow1 - 3, lastColHeader + 2, lastRow1 - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws2.Range(lastRow1 - 3, lastColHeader + 2, lastRow1 - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws2.Range(lastRow1 - 1, lastColHeader + 2, lastRow1 - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    LastAccumbefore = 0;
                    Grwth = 0;
                    #endregion

                    #region Work Day
                    lastRow1++;
                    tgl = 0;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Value = "Work Day";
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i <= 26; i++)
                    {
                        tgl++;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Value = tgl;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        lastColHeader = i;
                    }
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Value = "Total";
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Value = "Grwth";
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastRow1++;

                    x = 0;
                    foreach (DataRow row in dt1.Rows)
                    {
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Value = row[dt1.Columns[1].Caption];
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        for (int i = 2; i < dt1.Columns.Count; i++)
                        {
                            var a = i;
                            var inq = Convert.ToInt32(row[dt1.Columns[a].Caption]);
                            accum = accum + inq;
                            if (x != accum || x == 0 || inq == 0) { accum = accum; } else { accum = 0; }
                            var b = row[dt1.Columns[a].Caption];
                            var nilai = "";
                            if (Convert.ToString(b) != "0") { nilai = Convert.ToString(b); }
                            ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Value = nilai;
                            ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            x = accum;
                            lastColHeader = i;
                        }
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Value = accum;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        accum = 0;
                        lastRow1++;
                    }
                    ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow1 - 2)).SetFormulaA1("=AB" + (lastRow1 - 7));
                    ws2.Range(lastRow1 - 2, lastColHeader + 2, lastRow1 - 2, lastColHeader + 2).AddConditionalFormat()
                             .IconSet(XLIconSetStyle.ThreeArrows, false, true)
                             .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, -1, XLCFContentType.Number)
                             .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number)
                             .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number);
                    ws2.Range(lastRow1 - 3, lastColHeader + 2, lastRow1 - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws2.Range(lastRow1 - 3, lastColHeader + 2, lastRow1 - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws2.Range(lastRow1 - 3, lastColHeader + 2, lastRow1 - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws2.Range(lastRow1 - 1, lastColHeader + 2, lastRow1 - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    BackRow = lastRow1 - 3;
                    foreach (DataRow row in dt11.Rows)
                    {
                        for (int i = 2; i < dt11.Columns.Count; i++)
                        {
                            var b = row[dt1.Columns[i].Caption];
                            if (Convert.ToString(b) != "0")
                            {
                                ws2.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), BackRow))
                                    .Style.Fill.SetBackgroundColor(XLColor.Gray);
                            }
                            lastColHeader = i;
                        }
                        lastColHeader = 1;
                        BackRow++;
                    }
                    #endregion

                    lastRow1 = lastRow1 + 18;

                    #endregion

                    #region JABODETABEK

                    ws3.Columns("1").Width = 20;
                    ws3.Columns(2, 26).Width = 5;
                    ws3.Columns("27").Width = 7;
                    ws3.Range("A" + lastRow2 + ":B" + lastRow2).Merge();
                    ws3.Cell("A" + lastRow2).Value = KendaraanArray[z].ToString();
                    ws3.Range("T" + lastRow2 + ":AA" + lastRow2).Merge();
                    ws3.Cell("T" + lastRow2).Value = "Periode " + StartDateName + " S/D " + EndDateName;
                    lastRow2++;

                    #region Work Day (Acumm)
                    tgl = 0;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Value = "Work Day (Accum)";
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i <= 26; i++)
                    {
                        tgl++;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Value = tgl;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        lastColHeader = i;
                    }
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Value = "Total";
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Value = "Grwth";
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastRow2++;

                    x = 0;
                    LastAccum = 0;
                    foreach (DataRow row in dt2.Rows)
                    {
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Value = row[dt2.Columns[1].Caption];
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        for (int i = 2; i < dt2.Columns.Count; i++)
                        {
                            var a = i;
                            var inq = Convert.ToInt32(row[dt2.Columns[a].Caption]);
                            accum = accum + inq;
                            LastAccum = LastAccum + inq;
                            if (x != accum || x == 0) { accum = accum; } else { accum = 0; }
                            var NilaiAcum = "";
                            if (accum != 0) { NilaiAcum = Convert.ToString(accum); }
                            if (LastAccum != LastAccumbefore) { Grwth = i; }
                            ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Value = NilaiAcum;
                            ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            x = accum;
                            LastAccumbefore = LastAccum;
                            lastColHeader = i;
                        }
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Value = LastAccum;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        accum = 0;
                        LastAccum = 0;
                        lastRow2++;
                    }
                    row1 = lastRow2 - 1;
                    row2 = lastRow2 - 2;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2 - 2)).SetFormulaA1("=" + GetExcelColumnName(Grwth) + row1 + "/" + GetExcelColumnName(Grwth) + row2 + "-1");
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2 - 2)).Style.NumberFormat.Format = "0.0%";
                    ws3.Range(lastRow2 - 3, lastColHeader + 2, lastRow2 - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws3.Range(lastRow2 - 3, lastColHeader + 2, lastRow2 - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws3.Range(lastRow2 - 3, lastColHeader + 2, lastRow2 - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws3.Range(lastRow2 - 1, lastColHeader + 2, lastRow2 - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    LastAccumbefore = 0;
                    Grwth = 0;
                    #endregion

                    #region Work Day
                    lastRow2++;
                    tgl = 0;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Value = "Work Day";
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i <= 26; i++)
                    {
                        tgl++;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Value = tgl;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        lastColHeader = i;
                    }
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Value = "Total";
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Value = "Grwth";
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastRow2++;

                    x = 0;
                    foreach (DataRow row in dt2.Rows)
                    {
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Value = row[dt2.Columns[1].Caption];
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        for (int i = 2; i < dt2.Columns.Count; i++)
                        {
                            var a = i;
                            var inq = Convert.ToInt32(row[dt2.Columns[a].Caption]);
                            accum = accum + inq;
                            if (x != accum || x == 0 || inq == 0) { accum = accum; } else { accum = 0; }
                            var b = row[dt2.Columns[a].Caption];
                            var nilai = "";
                            if (Convert.ToString(b) != "0") { nilai = Convert.ToString(b); }
                            ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Value = nilai;
                            ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            x = accum;
                            lastColHeader = i;
                        }
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Value = accum;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow2)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        accum = 0;
                        lastRow2++;
                    }
                    ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow2 - 2)).SetFormulaA1("=AB" + (lastRow2 - 7));
                    ws3.Range(lastRow2 - 2, lastColHeader + 2, lastRow2 - 2, lastColHeader + 2).AddConditionalFormat()
                             .IconSet(XLIconSetStyle.ThreeArrows, false, true)
                             .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, -1, XLCFContentType.Number)
                             .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number)
                             .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number);
                    ws3.Range(lastRow2 - 3, lastColHeader + 2, lastRow2 - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws3.Range(lastRow2 - 3, lastColHeader + 2, lastRow2 - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws3.Range(lastRow2 - 3, lastColHeader + 2, lastRow2 - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws3.Range(lastRow2 - 1, lastColHeader + 2, lastRow2 - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    BackRow = lastRow2 - 3;
                    foreach (DataRow row in dt11.Rows)
                    {
                        for (int i = 2; i < dt11.Columns.Count; i++)
                        {
                            var b = row[dt1.Columns[i].Caption];
                            if (Convert.ToString(b) != "0")
                            {
                                ws3.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), BackRow))
                                    .Style.Fill.SetBackgroundColor(XLColor.Gray);
                            }
                            lastColHeader = i;
                        }
                        lastColHeader = 1;
                        BackRow++;
                    }
                    #endregion

                    lastRow2 = lastRow2 + 18;

                    #endregion

                    #region NON JABODETABEK

                    ws4.Columns("1").Width = 20;
                    ws4.Columns(2, 26).Width = 5;
                    ws4.Columns("27").Width = 7;
                    ws4.Range("A" + lastRow3 + ":B" + lastRow3).Merge();
                    ws4.Cell("A" + lastRow3).Value = KendaraanArray[z].ToString();
                    ws4.Range("T" + lastRow3 + ":AA" + lastRow3).Merge();
                    ws4.Cell("T" + lastRow3).Value = "Periode " + StartDateName + " S/D " + EndDateName;
                    lastRow3++;

                    #region Work Day (Acumm)
                    tgl = 0;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Value = "Work Day (Accum)";
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i <= 26; i++)
                    {
                        tgl++;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Value = tgl;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        lastColHeader = i;
                    }
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Value = "Total";
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Value = "Grwth";
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastRow3++;

                    x = 0;
                    LastAccum = 0;
                    foreach (DataRow row in dt3.Rows)
                    {
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Value = row[dt3.Columns[1].Caption];
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        for (int i = 2; i < dt3.Columns.Count; i++)
                        {
                            var a = i;
                            var inq = Convert.ToInt32(row[dt3.Columns[a].Caption]);
                            accum = accum + inq;
                            LastAccum = LastAccum + inq;
                            if (x != accum || x == 0) { accum = accum; } else { accum = 0; }
                            var NilaiAcum = "";
                            if (accum != 0) { NilaiAcum = Convert.ToString(accum); }
                            if (LastAccum != LastAccumbefore) { Grwth = i; }
                            ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Value = NilaiAcum;
                            ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            x = accum;
                            LastAccumbefore = LastAccum;
                            lastColHeader = i;
                        }
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Value = LastAccum;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        accum = 0;
                        LastAccum = 0;
                        lastRow3++;
                    }
                    row1 = lastRow3 - 1;
                    row2 = lastRow3 - 2;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3 - 2)).SetFormulaA1("=" + GetExcelColumnName(Grwth) + row1 + "/" + GetExcelColumnName(Grwth) + row2 + "-1");
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3 - 2)).Style.NumberFormat.Format = "0.0%";
                    ws4.Range(lastRow3 - 3, lastColHeader + 2, lastRow3 - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws4.Range(lastRow3 - 3, lastColHeader + 2, lastRow3 - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws4.Range(lastRow3 - 3, lastColHeader + 2, lastRow3 - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws4.Range(lastRow3 - 1, lastColHeader + 2, lastRow3 - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    LastAccumbefore = 0;
                    Grwth = 0;
                    #endregion

                    #region Work Day
                    lastRow3++;
                    tgl = 0;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Value = "Work Day";
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i <= 26; i++)
                    {
                        tgl++;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Value = tgl;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        lastColHeader = i;
                    }
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Value = "Total";
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Value = "Grwth";
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    lastRow3++;

                    x = 0;
                    foreach (DataRow row in dt3.Rows)
                    {
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Value = row[dt3.Columns[1].Caption];
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        for (int i = 2; i < dt3.Columns.Count; i++)
                        {
                            var a = i;
                            var inq = Convert.ToInt32(row[dt3.Columns[a].Caption]);
                            accum = accum + inq;
                            if (x != accum || x == 0 || inq == 0) { accum = accum; } else { accum = 0; }
                            var b = row[dt3.Columns[a].Caption];
                            var nilai = "";
                            if (Convert.ToString(b) != "0") { nilai = Convert.ToString(b); }
                            ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Value = nilai;
                            ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            x = accum;
                            lastColHeader = i;
                        }
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Value = accum;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), lastRow3)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        accum = 0;
                        lastRow3++;
                    }
                    ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 2), lastRow3 - 2)).SetFormulaA1("=AB" + (lastRow3 - 7));
                    ws4.Range(lastRow3 - 2, lastColHeader + 2, lastRow3 - 2, lastColHeader + 2).AddConditionalFormat()
                             .IconSet(XLIconSetStyle.ThreeArrows, false, true)
                             .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, -1, XLCFContentType.Number)
                             .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number)
                             .AddValue(XLCFIconSetOperator.EqualOrGreaterThan, 0, XLCFContentType.Number);
                    ws4.Range(lastRow3 - 3, lastColHeader + 2, lastRow3 - 1, lastColHeader + 2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws4.Range(lastRow3 - 3, lastColHeader + 2, lastRow3 - 1, lastColHeader + 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws4.Range(lastRow3 - 3, lastColHeader + 2, lastRow3 - 3, lastColHeader + 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws4.Range(lastRow3 - 1, lastColHeader + 2, lastRow3 - 1, lastColHeader + 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    BackRow = lastRow3 - 3;
                    foreach (DataRow row in dt11.Rows)
                    {
                        for (int i = 2; i < dt11.Columns.Count; i++)
                        {
                            var b = row[dt1.Columns[i].Caption];
                            if (Convert.ToString(b) != "0")
                            {
                                ws4.Cell(string.Format("{0}{1}", GetExcelColumnName(lastColHeader + 1), BackRow))
                                    .Style.Fill.SetBackgroundColor(XLColor.Gray);
                            }
                            lastColHeader = i;
                        }
                        lastColHeader = 1;
                        BackRow++;
                    }

                    #endregion

                    lastRow3 = lastRow3 + 18;

                    #endregion

                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

                Workbook book = new Workbook();
                book.LoadFromFile(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                Worksheet sheet = book.Worksheets["Nasional Area"];
                Worksheet sheet2 = book.Worksheets["Nasional Type"];
                Worksheet sheet3 = book.Worksheets["Jabodetabek Type"];
                Worksheet sheet4 = book.Worksheets["Non Jabodetabek Type"];

                #region Char NASIONAL
                //Add chart and set chart data range
                Chart chart = sheet.Charts.Add(ExcelChartType.LineMarkers);
                chart.DataRange = sheet.Range["A5:Z7"];
                chart.SeriesDataFromRange = true;

                //Chart border  
                chart.ChartArea.Border.Weight = ChartLineWeightType.Medium;

                //Chart position  
                chart.LeftColumn = 2;
                chart.TopRow = 14;
                chart.RightColumn = 27;
                chart.BottomRow = 30;

                //Chart title  
                chart.ChartTitle = "WORK DAY SPK COMPARED TO THREE MONTHS " + "ALL TYPE";
                chart.ChartTitleArea.Font.FontName = "Calibri";
                chart.ChartTitleArea.Font.Size = 13;
                chart.ChartTitleArea.Font.IsBold = true;

                //Data Label

                //foreach (Spire.Xls.Charts.ChartSerie cs in chart.Series)
                //{
                //    cs.DataPoints[20].DataLabels.HasValue = true;
                //    //cs.DataPoints.DefaultDataPoint.DataLabels.HasValue = true;
                //    cs.DataPoints.DefaultDataPoint.DataLabels.Position = DataLabelPositionType.Right;
                //}
                var index = 0;
                foreach (DataRow row in dt4.Rows)
                {
                    int poin = Convert.ToInt32(row[dt4.Columns[0].Caption]);
                    Spire.Xls.Charts.ChartSerie cs = chart.Series[index];
                    cs.DataPoints[poin].DataLabels.HasValue = true;
                    cs.DataPoints.DefaultDataPoint.DataLabels.Position = DataLabelPositionType.Right;
                    index++;
                }

                //Chart axis  
                chart.PrimaryCategoryAxis.Title = " ";
                chart.PrimaryValueAxis.Title = " ";
                //chart.PrimaryValueAxis.MaxValue = 3000;
                //chart.PrimaryValueAxis.MinValue = 0;
                chart.PrimaryValueAxis.TitleArea.TextRotationAngle = 90;
                chart.PrimaryValueAxis.HasMajorGridLines = false;

                //Chart legend  
                chart.Legend.Position = LegendPositionType.Top;
                //book.SaveToFile("Result.xlsx", ExcelVersion.Version2010);

                #endregion

                #region Chart JABODETABEK
                chart = sheet.Charts.Add(ExcelChartType.LineMarkers);
                chart.DataRange = sheet.Range["A35:Z37"];
                chart.SeriesDataFromRange = true;

                chart.ChartArea.Border.Weight = ChartLineWeightType.Medium;

                chart.LeftColumn = 2;
                chart.TopRow = 44;
                chart.RightColumn = 27;
                chart.BottomRow = 60;

                chart.ChartTitle = "WORK DAY SPK COMPARED TO THREE MONTHS " + "ALL TYPE";
                chart.ChartTitleArea.Font.FontName = "Calibri";
                chart.ChartTitleArea.Font.Size = 13;
                chart.ChartTitleArea.Font.IsBold = true;

                //foreach (Spire.Xls.Charts.ChartSerie cs in chart.Series)
                //{
                //    cs.Format.Options.IsVaryColor = true;
                //    cs.DataPoints.DefaultDataPoint.DataLabels.HasValue = true;
                //}
                index = 0;
                foreach (DataRow row in dt4.Rows)
                {
                    int poin = Convert.ToInt32(row[dt4.Columns[0].Caption]);
                    Spire.Xls.Charts.ChartSerie cs = chart.Series[index];
                    cs.DataPoints[poin].DataLabels.HasValue = true;
                    cs.DataPoints.DefaultDataPoint.DataLabels.Position = DataLabelPositionType.Right;
                    index++;
                }
                chart.PrimaryCategoryAxis.Title = " ";
                chart.PrimaryValueAxis.Title = " ";
                chart.PrimaryValueAxis.TitleArea.TextRotationAngle = 90;
                chart.PrimaryValueAxis.HasMajorGridLines = false;

                chart.Legend.Position = LegendPositionType.Top;
                #endregion

                #region Chart NON JABODETABEK
                chart = sheet.Charts.Add(ExcelChartType.LineMarkers);
                chart.DataRange = sheet.Range["A65:Z67"];
                chart.SeriesDataFromRange = true;

                chart.ChartArea.Border.Weight = ChartLineWeightType.Medium;

                chart.LeftColumn = 2;
                chart.TopRow = 74;
                chart.RightColumn = 27;
                chart.BottomRow = 90;

                chart.ChartTitle = "WORK DAY SPK COMPARED TO THREE MONTHS " + "ALL TYPE";
                chart.ChartTitleArea.Font.FontName = "Calibri";
                chart.ChartTitleArea.Font.Size = 13;
                chart.ChartTitleArea.Font.IsBold = true;

                //foreach (Spire.Xls.Charts.ChartSerie cs in chart.Series)
                //{
                //    cs.Format.Options.IsVaryColor = true;
                //    cs.DataPoints.DefaultDataPoint.DataLabels.HasValue = true;
                //}
                index = 0;
                foreach (DataRow row in dt4.Rows)
                {
                    int poin = Convert.ToInt32(row[dt4.Columns[0].Caption]);
                    Spire.Xls.Charts.ChartSerie cs = chart.Series[index];
                    cs.DataPoints[poin].DataLabels.HasValue = true;
                    cs.DataPoints.DefaultDataPoint.DataLabels.Position = DataLabelPositionType.Right;
                    index++;
                }

                chart.PrimaryCategoryAxis.Title = " ";
                chart.PrimaryValueAxis.Title = " ";
                chart.PrimaryValueAxis.TitleArea.TextRotationAngle = 90;
                chart.PrimaryValueAxis.HasMajorGridLines = false;

                chart.Legend.Position = LegendPositionType.Top;
                #endregion

                book.SaveToFile(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

                var rangeA1 = 5; var rangeZ1 = 7;
                var TopRow1 = 14; var BottomRow1 = 30;

                var rangeA2 = 5; var rangeZ2 = 7;
                var TopRow2 = 14; var BottomRow2 = 30;

                var rangeA3 = 5; var rangeZ3 = 7;
                var TopRow3 = 14; var BottomRow3 = 30;

                for (int z = 0; z < KendaraanArray.Length; z++)
                {
                    #region Char NASIONAL
                    //Add chart and set chart data range
                    chart = sheet2.Charts.Add(ExcelChartType.LineMarkers);

                    chart.DataRange = sheet2.Range["A" + rangeA1 + ":Z" + rangeZ1];

                    chart.SeriesDataFromRange = true;
                    chart.ChartArea.Border.Weight = ChartLineWeightType.Medium;

                    chart.LeftColumn = 2;
                    chart.TopRow = TopRow1;
                    chart.RightColumn = 27;
                    chart.BottomRow = BottomRow1;

                    chart.ChartTitle = "WORK DAY SPK COMPARED TO THREE MONTHS " + KendaraanArray[z].ToString();
                    chart.ChartTitleArea.Font.FontName = "Calibri";
                    chart.ChartTitleArea.Font.Size = 13;
                    chart.ChartTitleArea.Font.IsBold = true;
                    chart.PrimaryValueAxis.HasMajorGridLines = false;
                    chart.PrimaryCategoryAxis.Title = " ";
                    chart.PrimaryValueAxis.Title = " ";
                    chart.PrimaryValueAxis.TitleArea.TextRotationAngle = 90;
                    chart.Legend.Position = LegendPositionType.Top;

                    //foreach (Spire.Xls.Charts.ChartSerie cs in chart.Series)
                    //{
                    //    cs.Format.Options.IsVaryColor = true;
                    //    cs.DataPoints.DefaultDataPoint.DataLabels.HasValue = true;
                    //}
                    index = 0;
                    foreach (DataRow row in dt4.Rows)
                    {
                        int poin = Convert.ToInt32(row[dt4.Columns[0].Caption]);
                        Spire.Xls.Charts.ChartSerie cs = chart.Series[index];
                        cs.DataPoints[poin].DataLabels.HasValue = true;
                        cs.DataPoints.DefaultDataPoint.DataLabels.Position = DataLabelPositionType.Right;
                        index++;
                    }

                    rangeA1 = rangeA1 + 28; rangeZ1 = rangeZ1 + 28;
                    TopRow1 = TopRow1 + 28; BottomRow1 = BottomRow1 + 28;
                    #endregion

                    #region Char JABODETABEK
                    //Add chart and set chart data range
                    chart = sheet3.Charts.Add(ExcelChartType.LineMarkers);

                    chart.DataRange = sheet3.Range["A" + rangeA2 + ":Z" + rangeZ2];

                    chart.SeriesDataFromRange = true;
                    chart.ChartArea.Border.Weight = ChartLineWeightType.Medium;

                    chart.LeftColumn = 2;
                    chart.TopRow = TopRow2;
                    chart.RightColumn = 27;
                    chart.BottomRow = BottomRow2;

                    chart.ChartTitle = "WORK DAY SPK COMPARED TO THREE MONTHS " + KendaraanArray[z].ToString();
                    chart.ChartTitleArea.Font.FontName = "Calibri";
                    chart.ChartTitleArea.Font.Size = 13;
                    chart.ChartTitleArea.Font.IsBold = true;
                    chart.PrimaryValueAxis.HasMajorGridLines = false;
                    chart.PrimaryCategoryAxis.Title = " ";
                    chart.PrimaryValueAxis.Title = " ";
                    chart.PrimaryValueAxis.TitleArea.TextRotationAngle = 90;
                    chart.Legend.Position = LegendPositionType.Top;

                    //foreach (Spire.Xls.Charts.ChartSerie cs in chart.Series)
                    //{
                    //    cs.Format.Options.IsVaryColor = true;
                    //    cs.DataPoints.DefaultDataPoint.DataLabels.HasValue = true;
                    //}
                    index = 0;
                    foreach (DataRow row in dt4.Rows)
                    {
                        int poin = Convert.ToInt32(row[dt4.Columns[0].Caption]);
                        Spire.Xls.Charts.ChartSerie cs = chart.Series[index];
                        cs.DataPoints[poin].DataLabels.HasValue = true;
                        cs.DataPoints.DefaultDataPoint.DataLabels.Position = DataLabelPositionType.Right;
                        index++;
                    }

                    rangeA2 = rangeA2 + 28; rangeZ2 = rangeZ2 + 28;
                    TopRow2 = TopRow2 + 28; BottomRow2 = BottomRow2 + 28;
                    #endregion

                    #region Char NON JABODETABEK
                    //Add chart and set chart data range
                    chart = sheet4.Charts.Add(ExcelChartType.LineMarkers);

                    chart.DataRange = sheet4.Range["A" + rangeA3 + ":Z" + rangeZ3];

                    chart.SeriesDataFromRange = true;
                    chart.ChartArea.Border.Weight = ChartLineWeightType.Medium;

                    chart.LeftColumn = 2;
                    chart.TopRow = TopRow3;
                    chart.RightColumn = 27;
                    chart.BottomRow = BottomRow3;

                    chart.ChartTitle = "WORK DAY SPK COMPARED TO THREE MONTHS " + KendaraanArray[z].ToString();
                    chart.ChartTitleArea.Font.FontName = "Calibri";
                    chart.ChartTitleArea.Font.Size = 13;
                    chart.ChartTitleArea.Font.IsBold = true;
                    chart.PrimaryValueAxis.HasMajorGridLines = false;
                    chart.PrimaryCategoryAxis.Title = " ";
                    chart.PrimaryValueAxis.Title = " ";
                    chart.PrimaryValueAxis.TitleArea.TextRotationAngle = 90;
                    chart.Legend.Position = LegendPositionType.Top;

                    //foreach (Spire.Xls.Charts.ChartSerie cs in chart.Series)
                    //{
                    //    cs.Format.Options.IsVaryColor = true;
                    //    cs.DataPoints.DefaultDataPoint.DataLabels.HasValue = true;
                    //}
                    index = 0;
                    foreach (DataRow row in dt4.Rows)
                    {
                        int poin = Convert.ToInt32(row[dt4.Columns[0].Caption]);
                        Spire.Xls.Charts.ChartSerie cs = chart.Series[index];
                        cs.DataPoints[poin].DataLabels.HasValue = true;
                        cs.DataPoints.DefaultDataPoint.DataLabels.Position = DataLabelPositionType.Right;
                        index++;
                    }

                    rangeA3 = rangeA3 + 28; rangeZ3 = rangeZ3 + 28;
                    TopRow3 = TopRow3 + 28; BottomRow3 = BottomRow3 + 28;
                    #endregion

                }

                sheet.Range["A" + RowTitelA1].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["A" + RowTitelA2].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["A" + RowTitelA3].Style.HorizontalAlignment = HorizontalAlignType.Center;

                sheet.Range["AB" + GrwthA1].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["AB" + GrwthA1].Style.VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["AB" + GrwthA1].NumberFormat = "0.0%";
                sheet.Range["AB" + GrwthA2].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["AB" + GrwthA2].Style.VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["AB" + GrwthA2].NumberFormat = "0.0%";
                sheet.Range["AB" + GrwthA3].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["AB" + GrwthA3].Style.VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["AB" + GrwthA3].NumberFormat = "0.0%";

                //ConditionalFormatWrapper format1 = sheet.Range["AB" + (GrwthA1 + 5)].ConditionalFormats.AddCondition();
                //format1.FormatType = ConditionalFormatType.IconSet;
                //format1.IconSet.ShowIconOnly = true;
                //format1.IconSet.IsReverseOrder = false;

                //ConditionalFormatWrapper format2 = sheet.Range["AB" + (GrwthA2 + 5)].ConditionalFormats.AddCondition();
                //format2.FormatType = ConditionalFormatType.IconSet;
                //format2.IconSet.ShowIconOnly = true;
                //format2.IconSet.IsReverseOrder = false;

                //ConditionalFormatWrapper format3 = sheet.Range["AB" + (GrwthA3 + 5)].ConditionalFormats.AddCondition();
                //format3.FormatType = ConditionalFormatType.IconSet;
                //format3.IconSet.ShowIconOnly = true;
                //format3.IconSet.IsReverseOrder = false;


                sheet.Range["AB" + (GrwthA1 + 5)].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["AB" + (GrwthA1 + 5)].Style.VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["AB" + (GrwthA1 + 5)].NumberFormat = "0.0%";
                sheet.Range["AB" + (GrwthA2 + 5)].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["AB" + (GrwthA2 + 5)].Style.VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["AB" + (GrwthA2 + 5)].NumberFormat = "0.0%";
                sheet.Range["AB" + (GrwthA3 + 5)].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["AB" + (GrwthA3 + 5)].Style.VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["AB" + (GrwthA3 + 5)].NumberFormat = "0.0%";


                sheet2.Range["A" + RowTitelB1].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet3.Range["A" + RowTitelC1].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet4.Range["A" + RowTitelD1].Style.HorizontalAlignment = HorizontalAlignType.Center;

                var GrwthType = 6;
                for (int z = 0; z < KendaraanArray.Length; z++)
                {
                    sheet2.Range["AB" + GrwthType].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet2.Range["AB" + GrwthType].Style.VerticalAlignment = VerticalAlignType.Center;
                    sheet2.Range["AB" + GrwthType].NumberFormat = "0.0%";
                    sheet3.Range["AB" + GrwthType].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet3.Range["AB" + GrwthType].Style.VerticalAlignment = VerticalAlignType.Center;
                    sheet3.Range["AB" + GrwthType].NumberFormat = "0.0%";
                    sheet4.Range["AB" + GrwthType].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet4.Range["AB" + GrwthType].Style.VerticalAlignment = VerticalAlignType.Center;
                    sheet4.Range["AB" + GrwthType].NumberFormat = "0.0%";

                    //ConditionalFormatWrapper formatB1 = sheet2.Range["AB" + (GrwthType + 5)].ConditionalFormats.AddCondition();
                    //formatB1.FormatType = ConditionalFormatType.IconSet;
                    //formatB1.IconSet.ShowIconOnly = true;
                    //formatB1.IconSet.IsReverseOrder = false;

                    //ConditionalFormatWrapper formatC1 = sheet3.Range["AB" + (GrwthType + 5)].ConditionalFormats.AddCondition();
                    //formatC1.FormatType = ConditionalFormatType.IconSet;
                    //formatC1.IconSet.ShowIconOnly = true;
                    //formatC1.IconSet.IsReverseOrder = false;

                    //ConditionalFormatWrapper formatD1 = sheet4.Range["AB" + (GrwthType + 5)].ConditionalFormats.AddCondition();
                    //formatD1.FormatType = ConditionalFormatType.IconSet;
                    //formatD1.IconSet.ShowIconOnly = true;
                    //formatD1.IconSet.IsReverseOrder = false;

                    sheet2.Range["AB" + (GrwthType + 5)].Style.IncludeAlignment = true;
                    sheet2.Range["AB" + (GrwthType + 5)].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet2.Range["AB" + (GrwthType + 5)].Style.VerticalAlignment = VerticalAlignType.Center;
                    sheet3.Range["AB" + (GrwthType + 5)].Style.IncludeAlignment = true;
                    sheet3.Range["AB" + (GrwthType + 5)].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet3.Range["AB" + (GrwthType + 5)].Style.VerticalAlignment = VerticalAlignType.Center;
                    sheet4.Range["AB" + (GrwthType + 5)].Style.IncludeAlignment = true;
                    sheet4.Range["AB" + (GrwthType + 5)].Style.HorizontalAlignment = HorizontalAlignType.Center;
                    sheet4.Range["AB" + (GrwthType + 5)].Style.VerticalAlignment = VerticalAlignType.Center;

                    GrwthType = GrwthType + 28;
                }

                book.SaveToFile(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
        }
    }
}