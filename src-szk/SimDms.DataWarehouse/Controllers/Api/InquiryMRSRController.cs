using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Mvc;
using SimDms.DataWarehouse.Controllers;
using System.Data.SqlClient;
using ClosedXML.Excel;
using Spire.Xls;
using Spire.Xls.Charts;
using SimDms.DataWarehouse.Models;
using System.Linq;

namespace SimDms.PreSales.Controllers.Api 
{
    public class InquiryMRSRController : BaseController
    {
        private class prmProc
        {
            public string Dlr { get; set; }
            public string Otlt { get; set; }
            public int Mnth { get; set; }
            public int Yr { get; set; }
        }

        private class prmDSR
        {
            public string date { get; set; }
            public string param1 { get; set; }
            public string param2 { get; set; }
        }
         
        private class Rowcol
        {
            public int Row { get; set; }
            public int col { get; set; } 
        }

        private class Alignment 
        {
            public bool isHorizontal { get; set; }
            public bool isVertical { get; set; }
            public bool isLeft { get; set; }
            public bool isRight { get; set; }
            public bool isWraptext { get; set; }
        }

        private class month
        {
            public string date { get; set; }
        }

        private DataTable CreateTable(prmProc param, string SPName)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = SPName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", param.Dlr);
            cmd.Parameters.AddWithValue("@BranchCode", param.Otlt);
            cmd.Parameters.AddWithValue("@Month2", param.Mnth);
            cmd.Parameters.AddWithValue("@PeriodYear", param.Yr);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.Username);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        private DataTable CreateTable1(prmDSR param, string SPName)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 72000; cmd.CommandText = SPName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Date", param.date);
            cmd.Parameters.AddWithValue("@param1", param.param1);
            cmd.Parameters.AddWithValue("@param2", param.param2);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        } 

        private Rowcol CreateChart(DataTable dt, int lastRow, Spire.Xls.Workbook wb, string fileName) 
        {
            var ws = wb.Worksheets["MRSR DATA"]; 
            var iCol = 2; 
            var tmpLastRow = lastRow;
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 2;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    
                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws.Range[lastRow + 1, iCol].Style.NumberFormat = "dd-MMM-yyyy";
                    }

                    if (tmpLastRow == lastRow)
                    {
                        ws.Range[lastRow, iCol].Value = ((dc.ColumnName).Substring(0, 2)) != "ks" ? dc.ColumnName : "";
                        ws.Range[lastRow, iCol].Style.Font.Size = 10;
                        ws.Range[lastRow + 1, iCol].Style.HorizontalAlignment = HorizontalAlignType.Left;

                        ws.Range[lastRow + 1, iCol].Style.Font.Size =10;
                        ws.Range[lastRow + 1, iCol].Value = val.ToString();
                        if (iCol != 2)
                        {
                            ws.Range[lastRow + 1, iCol].Style.HorizontalAlignment = HorizontalAlignType.Right;
                        }
                    }
                    else
                    {
                        ws.Range[lastRow + 1, iCol].Style.Font.Size = 10;
                        ws.Range[lastRow + 1, iCol].Value = val.ToString();
                    }

                    iCol++;
                }

                lastRow++;
            }
            ws.Range[tmpLastRow, 2, lastRow, iCol - 1].Borders.LineStyle = LineStyleType.Thin;
            ws.Range[tmpLastRow, 2, lastRow, iCol - 1].Borders[BordersLineType.DiagonalDown].LineStyle = LineStyleType.None;
            ws.Range[tmpLastRow, 2, lastRow, iCol - 1].Borders[BordersLineType.DiagonalUp].LineStyle = LineStyleType.None;
            Rowcol test = (new Rowcol { Row = lastRow, col = iCol });
            return test;
        }
 
        #region ActionResult MRSR

        public ActionResult SvMRSR(string Dealer, string Outlet, int Month, int Year, int Penjualan)
        {
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("MRSR DATA");
            DateTime now = DateTime.Now;
            string fileName = "MRSR DATA_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            ws.Columns().Style.Font.FontName = "Arial";
            ws.Columns().Style.Font.FontSize = 10;
            ws.Row(lastRow).Height = 27;
            ws.Range("A1", "F1").Merge();
            ws.Cell("A" + lastRow).Value = "<Monthly retail service report>";
            ws.Cell("A" + lastRow).Style.Font.FontSize = 20;
            ws.Cell("A" + lastRow).Style.Font.Bold = true;

            ws.Range("A2", "B2").Merge();
            lastRow = 2;
            ws.Cell("A" + lastRow).Value = "Distributor ";
            ws.Cell("C" + lastRow).Value = ": " + "xxxxxxxxxxxxx";
            ws.Range("C" + lastRow, "F" + lastRow).Style.Fill.SetBackgroundColor(XLColor.FromArgb(204, 255, 255));
            ws.Range("C" + lastRow, "F" + lastRow).Merge();

            ws.Cell("I" + lastRow).Value = "Year :";
            ws.Cell("J" + lastRow).Value = Year;
            ws.Cell("J" + lastRow).Style.Fill.SetBackgroundColor(XLColor.FromArgb(204, 255, 255));
            ws.Cell("A" + lastRow).Style.Font.Bold = true;
            ws.Cell("C" + lastRow).Style.Font.Bold = true;
            ws.Cell("I" + lastRow).Style.Font.Bold = true;
            ws.Cell("J" + lastRow).Style.Font.Bold = true;

            lastRow = 3;
            ws.Cell("A" + lastRow).Value = "Currency";
            ws.Cell("B" + lastRow).Value = "IDR ";
            ws.Cell("B" + lastRow).Style.Fill.SetBackgroundColor(XLColor.FromArgb(204, 255, 255));
            ws.Cell("A" + lastRow).Style.Font.Bold = true;
            ws.Cell("B" + lastRow).Style.Font.Bold = true;
            lastRow = 4;
            ws.Column(1).Width = 10;
            ws.Column(2).Width = 8.88;
            ws.Column(3).Width = 13.5;
            ws.Column(4).Width = 10;
            ws.Column(5).Width = 13;
            ws.Column(6).Width = 13.5;
            ws.Columns(7,21).Width = 13.5;
            ws.Columns(7, 40).Width = 13.5;
            prmProc param = new prmProc
            {
                Dlr = Dealer,
                Otlt = Outlet,
                Mnth = Month,
                Yr = Year
            };
            DataTable dt = CreateTable(param, "usprpt_WhgnDatamrsr");
            return GenerateExcel(param, wb, dt, lastRow, fileName, true);
        }
        
        #endregion

        #region Private Method

        private ActionResult GenerateExcel(prmProc param, XLWorkbook wb, DataTable dt, int lastRow, string fileName, bool isCustomize = false, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var ws = wb.Worksheet(1);
            var tmpLastRow = lastRow;

            int iCol = 1;
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRow == lastRow)
                    {
                        if (iCol == 1)
                        {
                            ws.Range(lastRow, iCol, lastRow, 6).Merge();
                            ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                            ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.White).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(11);
                            //ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                            ws.Cell(lastRow + 1, iCol).Value = val;
                        }
                        else
                        {
                            ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                            ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.White).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            //ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(10);
                            if (iCol == 16 || iCol == 17 || iCol == 18 || iCol == 19)
                            {
                                ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(250,192,144)).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }
                            //ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                            ws.Cell(lastRow + 1, iCol).Value = val;
                        }
                        
                    }
                    else
                    {
                        if (iCol == 1)
                        {
                            ws.Range(lastRow, iCol, lastRow, 6).Merge();
                            //ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                            if ((lastRow + 1) > 15 && (lastRow + 1) < 26)
                            {
                                //ws.Range(16, 1, 25, 3).Merge();
                                ws.Cell(16, 1).Value = "Service breakdown";
                               // ws.Range(16, 4, 20, 4).Merge();
                                ws.Cell(16, 4).Value = "Customer paid";
                                //ws.Range(21, 4, 24, 4).Merge();
                                ws.Cell(21, 4).Value = "Free for customer";
                                ws.Range(lastRow + 1, 5, lastRow + 1, 6).Merge();
                                iCol = 5;
                            }

                            if ((lastRow + 1) == 25)
                            {
                                iCol = 4;
                                ws.Range(25, iCol, 25, 6).Merge();
                            }
                            

                            if ((lastRow + 1) > 25 && (lastRow + 1) < 32)
                            {
                                ws.Cell(26, 1).Value = "Profitability (Customer paid)";
                                ws.Cell(26, 2).Value = "Sales amount";
                                ws.Cell(26, 4).Value = "*Labor";
                                ws.Cell(27, 4).Value = "*Parts";
                                ws.Cell(28, 4).Value = "Sublet";
                                ws.Cell(29, 4).Value = "Total";
                                ws.Cell(30, 2).Value = "Cost(Parts & Sublet)";
                                ws.Cell(31, 2).Value = "Gross Profit";
                                iCol = 6;
                            }

                            if ((lastRow + 1) > 31 && (lastRow + 1) < 39)
                            {
                                ws.Cell(32, 1).Value = "Profitability (Free for customer)";
                                ws.Cell(32, 2).Value = "Income amount";
                                ws.Cell(32, 4).Value = "Labor (Warranty/Recall work) ";
                                ws.Cell(33, 4).Value = "Labor (Others) ";
                                ws.Cell(34, 4).Value = "Parts";
                                ws.Cell(35, 4).Value = "Sublet";
                                ws.Cell(36, 4).Value = "Total";
                                ws.Cell(37, 2).Value = "Cost(Parts & Sublet)";
                                ws.Cell(38, 2).Value = "Gross Profit";
                                iCol = 6;
                            }

                            ws.Cell(lastRow + 1, iCol).Value = val;
                            iCol = 1;
                        }
                        else
                        {
                            //ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                            ws.Cell(lastRow + 1, iCol).Value = val;
                        }
                    }

                    if (iCol == 1) { iCol = iCol + 6; }
                    else {iCol++;}
                }

                lastRow++;
            }

            //if (isShowSummary)
            //{
            //    ws.Cell(lastRow + 1, 1).Value = "TOTAL";
            //    for (char i = 'A'; i <= iCol; i++)
            //    {
            //        ws.Cell(lastRow + 1, 1).FormulaA1 = "=SUM(" + i + (tmpLastRow + 1) + ":" + i + lastRow + ")";
            //    }
            //}

            var rngTable = ws.Range(tmpLastRow, 1, lastRow, iCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);


            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            ws.Columns().Style.Alignment.SetWrapText();
            //ws.Columns().AdjustToContents();
            ws.Range(5, 7, lastRow, iCol - 3).Style.Fill.SetBackgroundColor(XLColor.FromArgb(204, 255, 255));
            ws.Range(4, 1, 15, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Range(37, 7, 37, iCol - 3).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));
            ws.Cell("G5").Value = "";
            ws.Cell("G5").Style.Fill.SetBackgroundColor(XLColor.White);
            ws.Cell("G5").Style.Border.DiagonalBorder = XLBorderStyleValues.Thin;
            ws.Cell("G5").Style.Border.DiagonalBorderColor = XLColor.Black;
            ws.Cell("G5").Style.Border.DiagonalUp = true;

            ws.Cell("T5").Value = "";
            ws.Cell("T5").Style.Fill.SetBackgroundColor(XLColor.White);
            ws.Cell("T5").Style.Border.DiagonalBorder = XLBorderStyleValues.Thin;
            ws.Cell("T5").Style.Border.DiagonalBorderColor = XLColor.Black;
            ws.Cell("T5").Style.Border.DiagonalUp = true;

            ws.Range(5, 21, 15, 21).Value = "";
            ws.Range(5, 21, 15, 21).Style.Fill.SetBackgroundColor(XLColor.White);
            ws.Range(5, 21, 15, 21).Style.Border.DiagonalBorder = XLBorderStyleValues.Thin;
            ws.Range(5, 21, 15, 21).Style.Border.DiagonalBorderColor = XLColor.Black;
            ws.Range(5, 21, 15, 21).Style.Border.DiagonalUp = true;

            ws.Range(16, 1, 25, 3).Merge();
            ws.Range(16, 4, 20, 4).Merge();
            ws.Range(21, 4, 24, 4).Merge();
            ws.Range(25, 4, 25, 6).Merge();
            ws.Range(16, 5, 16, 6).Merge();
            ws.Range(17, 5, 17, 6).Merge();
            ws.Range(18, 5, 18, 6).Merge();
            ws.Range(19, 5, 19, 6).Merge();
            ws.Range(20, 5, 20, 6).Merge();
            ws.Range(21, 5, 21, 6).Merge();
            ws.Range(22, 5, 22, 6).Merge();
            ws.Range(23, 5, 23, 6).Merge();
            ws.Range(24, 5, 24, 6).Merge();
            //ws.Cell(16, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            //                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            //ws.Cell(16, 1).Style.Font.Bold = true;
            //ws.Cell(16, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            //                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            //ws.Cell(21, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            //                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Range(26, 1, 31, 1).Merge();
            ws.Range(26, 2, 29, 3).Merge();
            ws.Range(26, 2, 29, 3).Merge();
            ws.Range(30, 2, 30, 4).Merge();
            ws.Range(31, 2, 31, 4).Merge();
            //ws.Cell(26, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            //                       .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            //ws.Cell(26, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            //                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Cell(26, 1).Style.Alignment.SetTextRotation(90);
            ws.Cell(26, 1).Style.Font.Bold = true;
            //ws.Cell(25, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Range(32, 1, 38, 1).Merge();
            ws.Range(32, 2, 36, 3).Merge();
            ws.Range(32, 2, 36, 3).Merge();
            ws.Range(37, 2, 37, 4).Merge();
            ws.Range(38, 2, 38, 4).Merge();
            //ws.Cell(32, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            //                       .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            //ws.Cell(32, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            //                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Cell(32, 1).Style.Alignment.SetTextRotation(90);
            ws.Cell(32, 1).Style.Font.Bold = true;
            //string[] month = from deploy in ctx.OmMstModels
            //                 select new month { date = deploy.CreatedDate.ToString() })
            //             ;//ctx.Database.SqlQuery("select top 12 replace(left(convert(varchar,getdate(),106),6),' ','-') from SvHstSzkmrsrT1");
            //string[] month = { "20-Feb", "20-Mar", "11-Apr", "20-May", "20-Jun", "20-Jul", "1-Aug", "1-Sep", "1-Nov", "25-Nov", "25-Des", "25-Jan" };
            //var J = 0;
            //for (var i = 8; i < 20; i++)
            //{
            //    ws.Cell(5, i).Style.NumberFormat.Format = "@";
            //    ws.Cell(5, i).Value = (month[J]).ToString();
            //    J++;
            //}
          
            lastRow = lastRow + 2;
            //wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            //return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            return GenerateExcelT2(param, wb, lastRow, fileName, true);
        }

        private ActionResult GenerateExcelT2(prmProc param, XLWorkbook wb, int lastRow, string fileName, bool isCustomize = false, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var ws = wb.Worksheet(1);
            var tmpLastRow = lastRow;
            ws.Cell(lastRow, 1).Style.Font.Bold = true;
            ws.Cell(lastRow, 1).Value = "Performance index (*: Item shown in the chart.)";
            DataTable dt = CreateTable(param, "usprpt_WhgnDatamrsrT2");
            
            int iCol = 1;
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRow == lastRow)
                    {
                        ws.Cell(lastRow + 1, iCol).Value = val;
                        ws.Range(lastRow, 1, lastRow, 6).Merge();
                    }
                    else
                    {
                        if (iCol == 1)
                        {
                            ws.Range(lastRow, 1, lastRow, 6).Merge();
                            ws.Range(lastRow + 1, iCol, lastRow + 1, 6).Merge();
                            ws.Cell(lastRow + 1, iCol).Value = val;
                            iCol = 1;
                        }
                        else
                        {
                            ws.Cell(lastRow + 1, iCol).Value = val;
                        }
                        
                    }

                    if (iCol == 1) { iCol = iCol + 6; }
                    else { iCol++; }
                }

                lastRow++;
            }
            var rngTable = ws.Range(tmpLastRow + 1, 1, lastRow, iCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Range(tmpLastRow + 1, 1, lastRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Range(tmpLastRow + 1, 1, lastRow, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            for (var i = tmpLastRow; i <= lastRow; i++)
            {
                ws.Row(i).Height = 36.75;
            }
            lastRow = lastRow + 2;
            return GenerateExcelT3(param, wb, lastRow, fileName, true);
        }

        private ActionResult GenerateExcelT3(prmProc param, XLWorkbook wb, int lastRow, string fileName, bool isCustomize = false, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var ws = wb.Worksheet(1);
            ws.Range(lastRow, 1, lastRow, 6).Merge();
            ws.Cell(lastRow, 1).Value = "Customer retention rate for SUZUKI vehicle only";
            ws.Cell(lastRow, 1).Style.Font.Bold = true;
            ws.Cell(lastRow + 1, 1).Style.Font.Bold = true;
            lastRow = lastRow + 1;
            var tmpLastRow = lastRow;
            DataTable dt = CreateTable(param, "usprpt_WhgnDatamrsrT3");
            
            int iCol = 1;
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRow == lastRow)
                    {
                        ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                        //ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(11);
                        if (iCol == 1)
                        {
                            ws.Cell(lastRow + 1, iCol + 5).Value = val;
                        }
                        else
                        {
                            ws.Cell(lastRow + 1, iCol).Value = val;
                        }
                        ws.Row(lastRow+1).Height = 27;
                        ws.Range(lastRow, 1, lastRow, 6).Merge();
                    }
                    else
                    {
                        if (iCol == 1)
                        {
                            ws.Cell(lastRow + 1, iCol + 5).Value = val;
                        }
                        else
                        {
                            ws.Cell(lastRow + 1, iCol).Value = val;
                        }
                        ws.Row(lastRow + 1).Height = 27;
                    }

                    if (iCol == 1) { iCol = iCol + 6; }
                    else { iCol++; }
                }
                
                lastRow++;
            }
            var rngTable = ws.Range(tmpLastRow, 1, lastRow, iCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);
            ws.Columns().Style.Alignment.SetWrapText();
            
            var fsrow = 56;
            for (var w = 1; w < 17; w++ )
            {
                if (w < 14 && w != 8 ){
                    ws.Cell(fsrow + 1, 3).Value = "month";
                    ws.Cell(fsrow + 2, 3).Value = "Km";
                    ws.Range(fsrow, 2, fsrow, 3).Merge();
                }
                else
                {
                    ws.Range(fsrow, 2, fsrow + 2 , 3).Merge();
                }
                ws.Range(fsrow, 4, fsrow, 5).Merge();
                ws.Range(fsrow + 1, 4, fsrow + 1, 5).Merge();
                ws.Range(fsrow + 2, 4, fsrow + 2, 5).Merge();
                ws.Cell(fsrow, 4).Value = "Number of vehicles due in this month";
                ws.Cell(fsrow + 1, 4).Value = "Number of vehicles actually done";
                ws.Cell(fsrow + 2, 4).Value = "Retention rate(%)";
                
                fsrow = fsrow + 3;
            }
            ws.Cell(58, 1).Value = "Free 1";
            ws.Cell(61, 1).Value = "Free 2";
            ws.Cell(64, 1).Value = "Free 3";
            ws.Cell(67, 1).Value = "Free 4";
            ws.Cell(70, 1).Value = "Free 5";
            ws.Cell(73, 1).Value = "Free";
            ws.Cell(76, 1).Value = "Free";
            ws.Cell(82, 1).Value = "Paid 1";
            ws.Cell(85, 1).Value = "Paid 2";
            ws.Cell(88, 1).Value = "Paid 3";
            ws.Cell(91, 1).Value = "Paid 4";
            ws.Cell(94, 1).Value = "Paid 5";

            ws.Cell(56, 2).Value = "First time";
            ws.Cell(59, 2).Value = "2nd";
            ws.Cell(62, 2).Value = "3rd";
            ws.Cell(65, 2).Value = "4th";
            ws.Cell(68, 2).Value = "5th";
            ws.Cell(71, 2).Value = "6th";
            ws.Cell(74, 2).Value = "7th";
            ws.Cell(80, 2).Value = "8th";
            ws.Cell(83, 2).Value = "9th";
            ws.Cell(86, 2).Value = "10th";
            ws.Cell(89, 2).Value = "11th";
            ws.Cell(92, 2).Value = "12th";

            ws.Cell(57, 2).Value = "2";
            ws.Cell(60, 2).Value = "3";
            ws.Cell(63, 2).Value = "6";
            ws.Cell(66, 2).Value = "12";
            ws.Cell(69, 2).Value = "18";
            ws.Cell(72, 2).Value = "24";
            ws.Cell(75, 2).Value = "30";
            ws.Cell(81, 2).Value = "36";
            ws.Cell(84, 2).Value = "42";
            ws.Cell(87, 2).Value = "48";
            ws.Cell(90, 2).Value = "54";
            ws.Cell(93, 2).Value = "60";

            ws.Cell(58, 2).Value = "1000";
            ws.Cell(61, 2).Value = "5000";
            ws.Cell(64, 2).Value = "10000";
            ws.Cell(67, 2).Value = "20000";
            ws.Cell(70, 2).Value = "30000";
            ws.Cell(73, 2).Value = "40000";
            ws.Cell(76, 2).Value = "50000";
            ws.Cell(82, 2).Value = "60000";
            ws.Cell(85, 2).Value = "70000";
            ws.Cell(88, 2).Value = "80000";
            ws.Cell(91, 2).Value = "90000";
            ws.Cell(94, 2).Value = "100000";

            ws.Cell(77, 2).Value = "Total Free Service";
            ws.Cell(95, 2).Value = "multiple 5000";
            ws.Cell(98, 2).Value = "Total Under Warranty";
            ws.Cell(101, 2).Value = "Total Pasca Warranty";
            ws.Range(77, 2, 79, 21).Style.Font.Bold = true;
            ws.Range(95, 2, 103, 21).Style.Font.Bold = true;

            ws.Range(tmpLastRow + 1, 7, lastRow, 19).Style
                .Fill.SetBackgroundColor(XLColor.FromArgb(204, 255, 255));

            for (int w = 56; w < 103; w += 3)
            {
                ws.Cell(w + 2, 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(204, 255, 255));
                ws.Range(w + 2, 7, w + 2, 19).Style
                .Fill.SetBackgroundColor(XLColor.FromArgb(255, 255, 255));
                ws.Cell(w, 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 255, 255));
                ws.Cell(w + 1, 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 255, 255));
                ws.Cell(w, 1).Style
               .Border.SetBottomBorder(XLBorderStyleValues.None);
                ws.Cell(w + 1, 1).Style
                .Border.SetTopBorder(XLBorderStyleValues.None)
                .Border.SetBottomBorder(XLBorderStyleValues.None);
                ws.Cell(w + 2, 1).Style
                .Border.SetTopBorder(XLBorderStyleValues.None);
            }
            ws.Range(101, 7, 103, 19).Style
               .Fill.SetBackgroundColor(XLColor.FromArgb(255, 255, 255));
            
            //int[] prcn = { 41, 42, 46, 47 };
            //for (var p = 0; p < prcn.Length; p++)
            //{
            //    ws.Range(prcn[p], 7, prcn[p], 20).Style.NumberFormat.Format = "0%";
            //}

            int[] BdrBold = { 4, 5, 16, 25, 26, 32, 39, 55, 56, 77, 80, 95, 98, 101, 104 };
            for (var p = 0; p < BdrBold.Length; p++)
            {
                ws.Range(BdrBold[p], 1, BdrBold[p], 21).Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thick);
            }

            int[] BdrBld = { 41, 53 };
            for (var p = 0; p < BdrBld.Length; p++)
            {
                ws.Range(BdrBld[p], 1, BdrBld[p], 19).Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thick);
            }

            lastRow = lastRow + 2;
            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return GenerateExcel3(param, wb, dt, lastRow, fileName, true);
            //return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        private ActionResult GenerateExcel3(prmProc param, XLWorkbook wb, DataTable dt, int lastRow, string fileName, bool isCustomize = false, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var Rowcol = new Rowcol();
            int FirstRow = 273;
            var tmpLastRow = lastRow;

            //variable x n y chart
            var leftside = 15;
            var topside = 3300;

            //variable size of charh
            var height = 500;
            var width = 900; 

            Workbook workbook = new Workbook();
            workbook.LoadFromFile(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            var sheet = workbook.Worksheets[0];
            sheet.Columns[0].Style.WrapText = true;
            
            sheet.Range[32, 1].Style.Rotation = 90;
            sheet.Range[26, 1].Style.Rotation = 90;

            var currentformula = "=\"Currency: \"&B3";
            sheet.Range[26, 5].Formula = currentformula;
            sheet.Range[27, 5].Formula = currentformula;
            sheet.Range[28, 5].Formula = currentformula;
            sheet.Range[29, 5].Formula = currentformula;
            sheet.Range[30, 5].Formula = currentformula;
            sheet.Range[31, 5].Formula = currentformula;

            sheet.Range[32, 5].Formula = currentformula;
            sheet.Range[33, 5].Formula = currentformula;
            sheet.Range[34, 5].Formula = currentformula;
            sheet.Range[35, 5].Formula = currentformula;
            sheet.Range[36, 5].Formula = currentformula;
            sheet.Range[37, 5].Formula = currentformula;
            sheet.Range[38, 5].Formula = currentformula;

            sheet.Range[12, 1].Formula = "=\"Dealer retail labor rate/h  (Average of reported dealers) Currency: \"&B3";
            sheet.Range[13, 1].Formula = "=\"Warranty labor rate/h  (Average of reported dealers) Currency: \"&B3";

            #region chart1
            Chart chart1 = sheet.Charts.Add();
            chart1.ChartTitle = "Labor amount and parts amount for 'Customer paid'";
            chart1.ChartType = ExcelChartType.ColumnStacked;
            chart1.Height = height;
            chart1.Width = width;
            chart1.Left = leftside;
            chart1.Top = topside;
            chart1.Legend.Width = 100;
            //chart1.PrimaryValueAxis.HasMajorGridLines = false;
            DataTable dat = CreateTable(param, "usprpt_WhgnDatamrsrCH1");
            Rowcol = CreateChart(dat, FirstRow, workbook, fileName);
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(252, 213, 180);
            chart1.DataRange = sheet.Range[FirstRow + 2, 2, Rowcol.Row, Rowcol.col-2];
            ChartSerie cs1 = chart1.Series[0];
            cs1.CategoryLabels = sheet.Range[FirstRow, 3, FirstRow + 1, Rowcol.col - 2];
            //chart1.ChartArea.ForeGroundColor = Color.Bisque;
            //chart1.PlotArea.ForeGroundColor = System.Drawing.Color.Aqua;
            chart1.Legend.Position = LegendPositionType.Bottom;
            #endregion

            #region chart2
            FirstRow = Rowcol.Row + 2;
            Chart chart2 = sheet.Charts.Add();
            chart2.ChartTitle = "Service breakdown";
            chart2.ChartType = ExcelChartType.ColumnStacked;
            chart2.Height = height;
            chart2.Width = width;
            chart2.Left = leftside + 935;
            chart2.Top = topside;
            DataTable dt2 = CreateTable(param, "usprpt_WhgnDatamrsrCH2");
            Rowcol = CreateChart(dt2, FirstRow, workbook, fileName);
            chart2.DataRange = sheet.Range[FirstRow + 2, 2, Rowcol.Row, Rowcol.col - 2];
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(194, 214, 154);
            ChartSerie cs2 = chart2.Series[0];
            cs2.CategoryLabels = sheet.Range[FirstRow, 3, FirstRow + 1, Rowcol.col - 2];
            chart2.Legend.Position = LegendPositionType.Bottom;
            #endregion 

            #region chart3
            FirstRow = Rowcol.Row + 2;
            Chart chart3 = sheet.Charts.Add();
            chart3.ChartTitle = "Productive labor rate (Hours sold/ Hours attended in service)";
            chart3.ChartType = ExcelChartType.LineMarkers;
            chart3.Height = height;
            chart3.Width = width;
            chart3.Left = leftside;
            chart3.Top = topside + 550;
            DataTable dt3 = CreateTable(param, "usprpt_WhgnDatamrsrCH3");
            Rowcol = CreateChart(dt3, FirstRow, workbook, fileName);
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(184, 204, 228);
            chart3.DataRange = sheet.Range[FirstRow + 1, 2, Rowcol.Row, Rowcol.col - 1];
            ChartSerie cs3 = chart3.Series[0];
            cs3.CategoryLabels = sheet.Range[FirstRow, 3, Rowcol.Row - 4, Rowcol.col - 1];
            chart3.Legend.Position = LegendPositionType.Bottom;
            #endregion 
            
            #region chart4
            FirstRow = Rowcol.Row + 2;
            Chart chart4 = sheet.Charts.Add();
            chart4.ChartTitle = "Sales amount per vehicle entry for 'Customer paid'(Sales amount /Total units by paid service)";
            chart4.ChartType = ExcelChartType.LineMarkers;
            chart4.Height = height;
            chart4.Width = width;
            chart4.Left = leftside + 935;
            chart4.Top = topside  + 550;
            DataTable dt4 = CreateTable(param, "usprpt_WhgnDatamrsrCH4");
            Rowcol = CreateChart(dt4, FirstRow, workbook, fileName);
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(141, 180, 227);
            chart4.DataRange = sheet.Range[FirstRow + 1, 2, Rowcol.Row, Rowcol.col - 1];
            ChartSerie cs4 = chart4.Series[0];
            cs4.CategoryLabels = sheet.Range[FirstRow, 3, Rowcol.Row - 2, Rowcol.col - 1];
            chart4.Legend.Position = LegendPositionType.Bottom;
            #endregion

            #region chart5
            FirstRow = Rowcol.Row + 2;
            Chart chart5 = sheet.Charts.Add();
            chart5.ChartTitle = "Customer retention rate (Average)(Vehicles done/ Vehicles due)";
            chart5.ChartType = ExcelChartType.LineMarkers;
            chart5.Height = height;
            chart5.Width = width;
            chart5.Left = leftside;
            chart5.Top = topside + 1100;
            DataTable dt5 = CreateTable(param, "usprpt_WhgnDatamrsrCH5");
            Rowcol = CreateChart(dt5, FirstRow, workbook, fileName);
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(204, 192, 218);
            chart5.DataRange = sheet.Range[FirstRow + 1, 2, Rowcol.Row, Rowcol.col-1];
            ChartSerie cs5 = chart5.Series[0];
            cs5.CategoryLabels = sheet.Range[FirstRow, 3, Rowcol.Row - 2, Rowcol.col - 1];
            chart5.Legend.Position = LegendPositionType.Bottom;
            #endregion

            #region chart6
            FirstRow = Rowcol.Row + 2;
            Chart chart6 = sheet.Charts.Add();
            chart6.ChartTitle = "Customer retention rate (Free1 and Paid1)(Vehicles done/ Vehicles due)";
            chart6.ChartType = ExcelChartType.LineMarkers;
            chart6.Height = height;
            chart6.Width = width;
            chart6.Left = leftside + 935;
            chart6.Top = topside + 1100;
            DataTable dt6 = CreateTable(param, "usprpt_WhgnDatamrsrCH6");
            Rowcol = CreateChart(dt6, FirstRow, workbook, fileName);
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(204, 192, 218);
            chart6.DataRange = sheet.Range[FirstRow + 1 , 2, Rowcol.Row, Rowcol.col-1];
            ChartSerie cs6 = chart6.Series[0];
            cs6.CategoryLabels = sheet.Range[FirstRow, 3, Rowcol.Row - 4, Rowcol.col - 1];
            chart6.Legend.Position = LegendPositionType.Bottom;
            #endregion

            #region chart7
            FirstRow = Rowcol.Row + 2;
            Chart chart7 = sheet.Charts.Add();
            chart7.ChartTitle = "Customer retention rate (Month by month)(Vehicles done/ Vehicles due)";
            chart7.ChartType = ExcelChartType.LineMarkers;
            chart7.Height = height;
            chart7.Width = width;
            chart7.Left = leftside;
            chart7.Top = topside + 1650;
            DataTable dt7 = CreateTable(param, "usprpt_WhgnDatamrsrCH7");
            Rowcol = CreateChart(dt7, FirstRow, workbook, fileName);
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(252, 213, 180);
            chart7.DataRange = sheet.Range[FirstRow + 1, 2, Rowcol.Row, Rowcol.col - 1];
            ChartSerie cs7 = chart7.Series[0];
            cs7.CategoryLabels = sheet.Range[FirstRow, 3, Rowcol.Row - 10, Rowcol.col - 1];
            chart7.Legend.Position = LegendPositionType.Bottom;
            //chart7.SecondaryCategoryAxis.IsMaxCross = true;
            #endregion

            #region chart8
            Chart chart8 = sheet.Charts.Add();
            chart8.ChartTitle = "Customer retention rate (Time by time)(Vehicles done/ Vehicled due)";
            chart8.ChartType = ExcelChartType.LineMarkers;
            chart8.SeriesDataFromRange = false;
            chart8.Height = height;
            chart8.Width = width;
            chart8.Left = leftside + 935;
            chart8.Top = topside + 1650;
            chart8.DataRange = sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1];
            ChartSerie cs8 = chart8.Series[0];
            cs8.CategoryLabels = sheet.Range["B" + FirstRow + ":B" + Rowcol.Row ];
            chart8.Legend.Position = LegendPositionType.Bottom;
            FirstRow = Rowcol.Row + 2;
            DataTable dt8 = CreateTable(param, "usprpt_WhgnDatamrsrCH8");
            Rowcol = CreateChart(dt8, FirstRow, workbook, fileName);
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(252, 213, 180);
            #endregion

            #region chart9
            FirstRow = Rowcol.Row + 2;
            Chart chart9 = sheet.Charts.Add();
            chart9.ChartTitle = "Number of vehicle/ Day/ Service staff";
            chart9.ChartType = ExcelChartType.LineMarkers;
            chart9.Height = height;
            chart9.Width = width;
            chart9.Left = leftside;
            chart9.Top = topside  + 2200;
            DataTable dt9 = CreateTable(param, "usprpt_WhgnDatamrsrCH9");
            Rowcol = CreateChart(dt9, FirstRow, workbook, fileName);
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(83, 142, 213);
            chart9.DataRange = sheet.Range[FirstRow + 1, 2, Rowcol.Row, Rowcol.col - 1];
            ChartSerie cs9 = chart9.Series[0];
            cs9.CategoryLabels = sheet.Range[FirstRow, 3, Rowcol.Row - 4, Rowcol.col - 1];
            chart9.Legend.Position = LegendPositionType.Bottom;
            #endregion

            #region chart10
            FirstRow = Rowcol.Row + 2;
            Chart chart10 = sheet.Charts.Add();
            chart10.ChartTitle = "Sales amount for 'Customer paid'/ Month/ Service staff";
            chart10.ChartType = ExcelChartType.LineMarkers;
            chart10.Height = height;
            chart10.Width = width;
            chart10.Left = leftside + 935;
            chart10.Top = topside + 2200;
            DataTable dt10 = CreateTable(param, "usprpt_WhgnDatamrsrCH10");
            Rowcol = CreateChart(dt10, FirstRow, workbook, fileName);
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(197, 192, 151);
            chart10.DataRange = sheet.Range[FirstRow + 1, 2, Rowcol.Row, Rowcol.col-1];
            ChartSerie cs10 = chart10.Series[0];
            cs10.CategoryLabels = sheet.Range[FirstRow, 3, Rowcol.Row - 4, Rowcol.col - 1];
            chart10.Legend.Position = LegendPositionType.Bottom;
            #endregion

            #region chart11
            FirstRow = Rowcol.Row + 2;
            Chart chart11 = sheet.Charts.Add();
            chart11.ChartTitle = "Number of dealers";
            chart11.ChartType = ExcelChartType.ColumnClustered;
            chart11.Height = height;
            chart11.Width = width;
            chart11.Left = leftside;
            chart11.Top = topside + 2750;
            DataTable dt11 = CreateTable(param, "usprpt_WhgnDatamrsrCH11");
            Rowcol = CreateChart(dt11, FirstRow, workbook, fileName);
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(253, 213, 180);
            chart11.DataRange = sheet.Range[FirstRow + 1, 2, Rowcol.Row, Rowcol.col - 1];
            ChartSerie cs11 = chart11.Series[0];
            cs11.CategoryLabels = sheet.Range[FirstRow, 3, Rowcol.Row - 6, Rowcol.col - 1];
            chart11.Legend.Position = LegendPositionType.Bottom;
            ChartSerie cs112 = chart11.Series[1];
            ChartSerie cs113 = chart11.Series[2];
            ChartSerie cs114 = chart11.Series[3];
            ChartSerie cs115 = chart11.Series[4];
            ChartSerie cs116 = chart11.Series[5];
            cs113.SerieType = ExcelChartType.LineMarkers;
            cs116.SerieType = ExcelChartType.LineMarkers;
            chart11.SecondaryCategoryAxis.IsMaxCross = true;
            cs113.UsePrimaryAxis = false;
            cs116.UsePrimaryAxis = false;
            cs11.Values = sheet.Range[FirstRow, 1];
            cs112.Values = sheet.Range[FirstRow + 2, 3, Rowcol.Row - 4, Rowcol.col - 1];
            cs113.Values = sheet.Range[FirstRow + 3, 3, Rowcol.Row - 3, Rowcol.col - 1];
            cs114.Values = sheet.Range[FirstRow, 1];
            cs115.Values = sheet.Range[FirstRow + 5, 3, Rowcol.Row - 1, Rowcol.col - 1];
            cs116.Values = sheet.Range[FirstRow + 6, 3, Rowcol.Row, Rowcol.col - 1];
            #endregion

            #region chart12
            FirstRow = Rowcol.Row + 2;
            Chart chart12 = sheet.Charts.Add();
            chart12.ChartTitle = "UIO and retained customers";
            chart12.ChartType = ExcelChartType.ColumnClustered;
            chart12.Height = height;
            chart12.Width = width;
            chart12.Left = leftside + 935;
            chart12.Top = topside + 2750;
            DataTable dt12 = CreateTable(param, "usprpt_WhgnDatamrsrCH12");
            Rowcol = CreateChart(dt12, FirstRow, workbook, fileName);
            sheet.Range[FirstRow, 2, Rowcol.Row, Rowcol.col - 1].Style.Color = Color.FromArgb(182, 221, 232);
            chart12.DataRange = sheet.Range[FirstRow + 1, 2, Rowcol.Row, Rowcol.col - 1];
            ChartSerie cs12 = chart12.Series[0];
            cs12.CategoryLabels = sheet.Range[FirstRow, 3, Rowcol.Row - 4, Rowcol.col - 1];
            chart12.Legend.Position = LegendPositionType.Bottom;
            ChartSerie cs123 = chart12.Series[1];
            ChartSerie cs124 = chart12.Series[3];
            cs123.SerieType = ExcelChartType.LineMarkers;
            cs124.SerieType = ExcelChartType.LineMarkers;
            chart12.SecondaryCategoryAxis.IsMaxCross = true;
            cs123.UsePrimaryAxis = false;
            cs124.UsePrimaryAxis = false;
            chart11.SecondaryCategoryAxis.IsMaxCross = true;
            #endregion

            //int[] row1 = { 4, 5, 55, 56, 26, 56, 56 };
            //int[] col1 = { 1, 1, 1, 1, 6, 6, 4 };
            //int[] row2 = { 4, 38, 55, 88, 38, 88, 88 };
            //int[] col2 = { 21, 4, 21, 3, 6, 6, 4 };
            //for (int i = 0; i < row1.Length; i++)
            //{
            //    // setAlign(workbook, row1[i], col1[i], row2[i], col2[i], new Alignment { isHorizontal = true, isVertical = true, isWraptext = true, isLeft = false, isRight = false });
            //    sheet.Range[row1[i], col1[i], row2[i], col2[i]].Style.HorizontalAlignment = HorizontalAlignType.Center;
            //    sheet.Range[row1[i], col1[i], row2[i], col2[i]].Style.VerticalAlignment = VerticalAlignType.Center;
            //}

            sheet.Range[4, 7, tmpLastRow, 21].Style.HorizontalAlignment = HorizontalAlignType.Center;
            sheet.Range[4, 7, tmpLastRow, 21].Style.VerticalAlignment = VerticalAlignType.Center;
            sheet.Range[4, 1, tmpLastRow, 3].Style.HorizontalAlignment = HorizontalAlignType.Center;
            sheet.Range[4, 1, tmpLastRow, 3].Style.VerticalAlignment = VerticalAlignType.Center;
            sheet.Range[16, 4, 24, 4].Style.HorizontalAlignment = HorizontalAlignType.Center;
            sheet.Range[16, 4, 24, 4].Style.VerticalAlignment = VerticalAlignType.Center;
            sheet.Range[26, 6, 103, 6].Style.HorizontalAlignment = HorizontalAlignType.Center;
            sheet.Range[26, 6, 103, 6].Style.VerticalAlignment = VerticalAlignType.Center;

            sheet.Range[23, 7, 23, 19].Value = "No Data";
            sheet.Range[35, 7, 35, 19].Value = "No Data";

            sheet.Range[5, 7, 5, 21].Style.NumberFormat = "dd-MMM";
            sheet.Range[259, 3, 260, 40].Style.NumberFormat = "#,##0";
            sheet.Range[6, 7, 38, 21].Style.NumberFormat = "#,##0";
            sheet.Range[43, 7, 52, 21].Style.NumberFormat = "#,##0";
            sheet.Range[56, 7, 103, 21].Style.NumberFormat = "#,##0";

            for (var w = 58; w < 104; w += 3)
            {
                sheet.Range[w, 7, w, 21].Style.NumberFormat = "0%";
            }

            //format number table 2
            int[] prcn = { 41,42,46,47 };
            for (var p = 0; p < prcn.Length; p++)
            {
                sheet.Range[prcn[p], 7, prcn[p], 20].Style.NumberFormat = "0%";
            }

            int[] coma1 = { 49, 50 };
            for (var p = 0; p < coma1.Length; p++) 
            {
                sheet.Range[coma1[p], 7, coma1[p], 20].Style.NumberFormat = "0.0";
            }

            //chart format number percent
            sheet.Range[284, 3, 287, 14].Style.NumberFormat = "0%";
            sheet.Range[298, 3, 301, 14].Style.NumberFormat = "0%";
            sheet.Range[304, 3, 313, 14].Style.NumberFormat = "0%";
            sheet.Range[316, 3, 325, 14].Style.NumberFormat = "0%";

            int[] qprcn = {294, 295, 342, 345, 349, 351 };
            for (var q = 0; q < qprcn.Length; q++)
            {
                sheet.Range[qprcn[q], 3, qprcn[q], 14].Style.NumberFormat = "0%";
            }
            //chart format number percent

            //chart format number integer
            sheet.Range[334, 3, 337, 14].Style.NumberFormat = "#,##0";
            int[] qint = { 275, 276, 280, 281, 290, 291, 348, 350 };
            for (var q = 0; q < qint.Length; q++)
            {
                sheet.Range[qint[q], 3, qint[q], 40].Style.NumberFormat = "#,##0";
            }
            //chart format number integer

            workbook.SaveToFile(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"), ExcelVersion.Version2007);  
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        #endregion

        #region ActionResult DSR

        public string DSR() 
        {
            int lastRo = 1;
            int lastRow = 1;
            int lastRows = 1;
            int lastRowss = 1;

            string strdate = DateTime.Now.ToString("dd-MMM-yyyy");

            var wb = new XLWorkbook();
            var ws3 = wb.Worksheets.Add("Model Code"); 
            var ws = wb.Worksheets.Add("Group Model");
            var ws1 = wb.Worksheets.Add("Variant");
            var ws2 = wb.Worksheets.Add("Colour");
            DateTime now = DateTime.Now;
            string fileName = "DSR_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            string username = "System";

            #region Model Code
            ws3.Columns().Style.Font.FontName = "Arial";
            ws3.Columns().Style.Font.FontSize = 10;
            //ws3.Range("A1", "F1").Merge();
            ws3.Cell("A" + lastRo).Value = "Printed by";
            ws3.Cell("B" + lastRo).Value = ": " + username + "/ " + strdate;
            ws3.Cell("A" + lastRo).Style.Font.FontSize = 10;
            ws3.Cell("A" + lastRo).Style.Font.Bold = true;
            ws3.Cell("B" + lastRo).Style.Font.FontSize = 10;
            ws3.Cell("B" + lastRo).Style.Font.Bold = true;

            //ws3.Range("A2", "B2").Merge();
            lastRo = 2;
            ws3.Cell("A" + lastRo).Value = "Daily Sales Report - by Model Code";
            ws3.Cell("A" + lastRo).Style.Font.FontSize = 14;
            ws3.Cell("A" + lastRo).Style.Font.Bold = true;

            lastRo = 4;
            ws3.Range("A4", "A6").Merge();
            ws3.Cell("A4").Value = "Model Code";
            ws3.Range("B4", "B6").Merge();
            ws3.Cell("B4").Value = "Colour Code";

            ws3.Range("C4", "E5").Merge();
            ws3.Cell("C4").Value = "INQUIRY";
            ws3.Range("F4", "H5").Merge();
            ws3.Cell("F4").Value = "SPK";
            ws3.Range("I4", "K5").Merge();
            ws3.Cell("I4").Value = "RKA";
            ws3.Range("L4", "N5").Merge();
            ws3.Cell("L4").Value = "SIS SALES";
            ws3.Range("O4", "O6").Merge();
            ws3.Cell("O4").Value = "SIS STOCK";
            ws3.Range("P4", "R5").Merge();
            ws3.Cell("P4").Value = "Registration this year";
            ws3.Range("S4", "S6").Merge();
            ws3.Cell("S4").Value = "Dealer STOCK";
            ws3.Range("T4", "V5").Merge();
            ws3.Cell("T4").Value = "Registration last year";
            ws3.Range("W4", "Z4").Merge();
            ws3.Cell("W4").Value = "Registration previous month";
            ws3.Range("W5", "X5").Merge();
            ws3.Cell("W5").Value = "Total Days";
            ws3.Range("Y5", "Z5").Merge();
            ws3.Cell("Y5").Value = "Working Days";

            lastRo = 6;
            char[] lop = { 'C', 'F', 'I', 'L', 'P', 'T', 'W', 'Y' };
            for (var p = 0; p < lop.Length; p++)
            {
                if (lop[p] == 'W' || lop[p] == 'Y')
                {
                    ws3.Cell((lop[p]).ToString() + lastRo).Value = "DD";
                    ws3.Cell(((char)(lop[p] + 1)).ToString() + lastRo).Value = "MM";
                    //.Cell(((char)loop[p] + 2).ToString() + lastRo).Value = "YY";
                }
                else
                {
                    ws3.Cell((lop[p]).ToString() + lastRo).Value = "DD";
                    ws3.Cell(((char)(lop[p] + 1)).ToString() + lastRo).Value = "MM";
                    ws3.Cell(((char)(lop[p] + 2)).ToString() + lastRo).Value = "YY";
                }
            }
            ws3.Range(4, 1, 6, 26).Style.Font.Bold = true;

            var rangTabl = ws3.Range(4, 1, 6, 26);
            rangTabl.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thick)
                .Border.SetBottomBorder(XLBorderStyleValues.Thick)
                .Border.SetLeftBorder(XLBorderStyleValues.Thick)
                .Border.SetRightBorder(XLBorderStyleValues.Thick);

            rangTabl.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws3.Cell("O4").Style.Alignment.SetWrapText();
            ws3.Cell("S4").Style.Alignment.SetWrapText();

            lastRo = 7;
            ws3.Column(1).Width = 24;
            ws3.Column(2).Width = 22;
            ws3.Columns(3, 25).Width = 11;
            #endregion

            #region group Model
            ws.Columns().Style.Font.FontName = "Arial";
            ws.Columns().Style.Font.FontSize = 10;
            //ws.Range("A1", "F1").Merge();
            ws.Cell("A" + lastRow).Value = "Printed by";
            ws.Cell("B" + lastRow).Value = ": " + username + "/ " + strdate;
            ws.Cell("A" + lastRow).Style.Font.FontSize = 10;
            ws.Cell("A" + lastRow).Style.Font.Bold = true;
            ws.Cell("B" + lastRow).Style.Font.FontSize = 10;
            ws.Cell("B" + lastRow).Style.Font.Bold = true;

            //ws.Range("A2", "B2").Merge();
            lastRow = 2;
            ws.Range("A" + lastRow, "C" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Daily Sales Report - by Group Model";
            ws.Cell("A" + lastRow).Style.Font.FontSize = 14;
            ws.Cell("A" + lastRow).Style.Font.Bold = true;

            lastRow = 4;
            ws.Range("A4", "A6").Merge();
            ws.Cell("A4").Value = "GROUP MODEL";

            ws.Range("B4", "B6").Merge();
            ws.Cell("B4").Value = "YEARS";

            ws.Range("c4", "E5").Merge();
            ws.Cell("c4").Value = "INQUIRY";
            ws.Range("F4", "H5").Merge();
            ws.Cell("F4").Value = "SPK";
            ws.Range("I4", "K5").Merge();
            ws.Cell("I4").Value = "RKA";
            ws.Range("L4", "N5").Merge();
            ws.Cell("L4").Value = "SIS SALES";
            ws.Range("O4", "O6").Merge();
            ws.Cell("O4").Value = "SIS STOCK";
            ws.Range("P4", "R5").Merge();
            ws.Cell("P4").Value = "Registration this year";
            ws.Range("S4", "S6").Merge();
            ws.Cell("S4").Value = "Dealer STOCK";
            ws.Range("T4", "V5").Merge();
            ws.Cell("T4").Value = "Registration last year";
            ws.Range("W4", "Z4").Merge();
            ws.Cell("W4").Value = "Registration previous month";
            ws.Range("W5", "X5").Merge();
            ws.Cell("W5").Value = "Total Days";
            ws.Range("Y5", "Z5").Merge();
            ws.Cell("Y5").Value = "Working Days";

            lastRow = 6;
            char[] loop = { 'C', 'F', 'I', 'L', 'P', 'T', 'W', 'Y' };
            for (var p = 0; p < loop.Length; p++)
            {
                if (loop[p] == 'W' || loop[p] == 'Y')
                {
                    ws.Cell((loop[p]).ToString() + lastRow).Value = "DD";
                    ws.Cell(((char)(loop[p] + 1)).ToString() + lastRow).Value = "MM";
                    //.Cell(((char)loop[p] + 2).ToString() + lastRow).Value = "YY";
                }
                else
                {
                    ws.Cell((loop[p]).ToString() + lastRow).Value = "DD";
                    ws.Cell(((char)(loop[p] + 1)).ToString() + lastRow).Value = "MM";
                    ws.Cell(((char)(loop[p] + 2)).ToString() + lastRow).Value = "YY";
                }
            }
            ws.Range(4, 1, 6, 25).Style.Font.Bold = true;

            var rngTable = ws.Range(4, 1, 6, 26);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thick)
                .Border.SetBottomBorder(XLBorderStyleValues.Thick)
                .Border.SetLeftBorder(XLBorderStyleValues.Thick)
                .Border.SetRightBorder(XLBorderStyleValues.Thick);

            rngTable.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Cell("O4").Style.Alignment.SetWrapText();
            ws.Cell("S4").Style.Alignment.SetWrapText();

            lastRow = 7;
            ws.Column(1).Width = 24;
            ws.Column(1).Width = 20;
            ws.Columns(3, 25).Width = 11;
            #endregion

            #region variant
            ws1.Columns().Style.Font.FontName = "Arial";
            ws1.Columns().Style.Font.FontSize = 10;
            //ws1.Range("A1", "F1").Merge();
            ws1.Cell("A" + lastRows).Value = "Printed by";
            ws1.Cell("B" + lastRows).Value = ": " + username + "/ " + strdate;
            ws1.Cell("A" + lastRows).Style.Font.FontSize = 10;
            ws1.Cell("A" + lastRows).Style.Font.Bold = true;
            ws1.Cell("B" + lastRows).Style.Font.FontSize = 10;
            ws1.Cell("B" + lastRows).Style.Font.Bold = true;

            //ws1.Range("A2", "B2").Merge();
            lastRows = 2;
            ws1.Range("A" + lastRows, "C" + lastRows).Merge();
            ws1.Cell("A" + lastRows).Value = "Daily Sales Report - by Variant";
            ws1.Cell("A" + lastRows).Style.Font.FontSize = 14;
            ws1.Cell("A" + lastRows).Style.Font.Bold = true;

            lastRows = 4;
            ws1.Range("A4", "A6").Merge();
            ws1.Cell("A4").Value = "GROUP MODEL";
            ws1.Range("B4", "B6").Merge();
            ws1.Cell("B4").Value = "YEARS";
            ws1.Range("C4", "C6").Merge();
            ws1.Cell("C4").Value = "VARIANT";

            ws1.Range("D4", "F5").Merge();
            ws1.Cell("D4").Value = "INQUIRY";
            ws1.Range("G4", "I5").Merge();
            ws1.Cell("G4").Value = "SPK";
            ws1.Range("J4", "L5").Merge();
            ws1.Cell("J4").Value = "RKA";
            ws1.Range("M4", "M5").Merge();
            ws1.Cell("M4").Value = "SIS SALES";
            ws1.Range("P4", "P6").Merge();
            ws1.Cell("P4").Value = "SIS STOCK";
            ws1.Range("Q4", "S5").Merge();
            ws1.Cell("Q4").Value = "Registration this year";
            ws1.Range("T4", "T6").Merge();
            ws1.Cell("T4").Value = "Dealer STOCK";
            ws1.Range("U4", "W5").Merge();
            ws1.Cell("U4").Value = "Registration last year";
            ws1.Range("X4", "Z4").Merge();
            ws1.Cell("X4").Value = "Registration previous month";
            ws1.Range("X5", "Y5").Merge();
            ws1.Cell("X5").Value = "Total Days";
            ws1.Range("Z5", "AA5").Merge();
            ws1.Cell("Z5").Value = "Working Days";

            lastRows = 6;
            char[] looop = { 'D', 'G', 'J', 'M', 'Q', 'U', 'X', 'Z' };
            for (var p = 0; p < looop.Length; p++)
            {
                if (looop[p] == 'X' || looop[p] == 'Z')
                {
                    ws1.Cell((looop[p]).ToString() + lastRows).Value = "DD";
                    if (looop[p] == 'Z')
                    {
                        ws1.Cell("AA" + lastRows).Value = "MM";
                    }
                    else
                    {
                        ws1.Cell(((char)(looop[p] + 1)).ToString() + lastRows).Value = "MM";
                    }

                    //.Cell(((char)loop[p] + 2).ToString() + lastRows).Value = "YY";
                }
                else
                {
                    ws1.Cell((looop[p]).ToString() + lastRows).Value = "DD";
                    ws1.Cell(((char)(looop[p] + 1)).ToString() + lastRows).Value = "MM";
                    ws1.Cell(((char)(looop[p] + 2)).ToString() + lastRows).Value = "YY";
                }
            }
            ws1.Range(4, 1, 6, 26).Style.Font.Bold = true;

            var rangTable = ws1.Range(4, 1, 6, 27);
            rangTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thick)
                .Border.SetBottomBorder(XLBorderStyleValues.Thick)
                .Border.SetLeftBorder(XLBorderStyleValues.Thick)
                .Border.SetRightBorder(XLBorderStyleValues.Thick);

            rangTable.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws1.Cell("P4").Style.Alignment.SetWrapText();
            ws1.Cell("T4").Style.Alignment.SetWrapText();

            lastRows = 7;
            ws1.Column(1).Width = 23;
            ws1.Column(2).Width = 20;
            ws1.Column(3).Width = 30;
            ws1.Columns(4, 27).Width = 11;
            #endregion

            #region Colour
            ws2.Columns().Style.Font.FontName = "Arial";
            ws2.Columns().Style.Font.FontSize = 10;
            //ws2.Range("A1", "F1").Merge();
            ws2.Cell("A" + lastRowss).Value = "Printed by";
            ws2.Cell("B" + lastRowss).Value = ": " + username + "/ " + strdate;
            ws2.Cell("A" + lastRowss).Style.Font.FontSize = 10;
            ws2.Cell("A" + lastRowss).Style.Font.Bold = true;
            ws2.Cell("B" + lastRowss).Style.Font.FontSize = 10;
            ws2.Cell("B" + lastRowss).Style.Font.Bold = true;

            //ws2.Range("A2", "B2").Merge();
            lastRowss = 2;
            ws2.Range("A" + lastRowss, "C" + lastRowss).Merge();
            ws2.Cell("A" + lastRowss).Value = "Daily Sales Report - by Colour";
            ws2.Cell("A" + lastRowss).Style.Font.FontSize = 14;
            ws2.Cell("A" + lastRowss).Style.Font.Bold = true;

            lastRowss = 4;
            ws2.Range("A4", "A6").Merge();
            ws2.Cell("A4").Value = "GROUP MODEL";
            ws2.Range("B4", "B6").Merge();
            ws2.Cell("B4").Value = "YEARS";
            ws2.Range("C4", "C6").Merge();
            ws2.Cell("C4").Value = "VARIANT";
            ws2.Range("D4", "D6").Merge();
            ws2.Cell("D4").Value = "COLOUR NAME";

            ws2.Range("E4", "G5").Merge();
            ws2.Cell("E4").Value = "INQUIRY";
            ws2.Range("H4", "J5").Merge();
            ws2.Cell("H4").Value = "SPK";
            ws2.Range("K4", "M5").Merge();
            ws2.Cell("K4").Value = "RKA";
            ws2.Range("N4", "P5").Merge();
            ws2.Cell("N4").Value = "SIS SALES";
            ws2.Range("Q4", "Q6").Merge();
            ws2.Cell("Q4").Value = "SIS STOCK";
            ws2.Range("R4", "T5").Merge();
            ws2.Cell("R4").Value = "Registration this year";
            ws2.Range("U4", "U6").Merge();
            ws2.Cell("U4").Value = "Dealer STOCK";
            ws2.Range("V4", "X5").Merge();
            ws2.Cell("V4").Value = "Registration last year";
            ws2.Range("Y4", "AB4").Merge();
            ws2.Cell("Y4").Value = "Registration previous month";
            ws2.Range("Y5", "Z5").Merge();
            ws2.Cell("Y5").Value = "Total Days";
            ws2.Range("AA5", "AB5").Merge();
            ws2.Cell("AA5").Value = "Working Days";
            lastRowss = 6;
            char[] loooop = { 'E', 'H', 'K', 'N', 'R', 'V', 'Y' };
            for (var p = 0; p < loooop.Length; p++)
            {
                if (loooop[p] == 'Y')
                {
                    ws2.Cell((loooop[p]).ToString() + lastRowss).Value = "DD";
                    ws2.Cell(((char)(loooop[p] + 1)).ToString() + lastRowss).Value = "MM";
                    //.Cell(((char)loop[p] + 2).ToString() + lastRowss).Value = "YY";
                }
                else
                {
                    ws2.Cell((loooop[p]).ToString() + lastRowss).Value = "DD";
                    ws2.Cell(((char)(loooop[p] + 1)).ToString() + lastRowss).Value = "MM";
                    ws2.Cell(((char)(loooop[p] + 2)).ToString() + lastRowss).Value = "YY";
                }
            }
            ws2.Range(4, 1, 6, 27).Style.Font.Bold = true;
            ws2.Cell("AA" + lastRowss).Value = "DD";
            ws2.Cell("AB" + lastRowss).Value = "MM";
            var rangTab = ws2.Range(4, 1, 6, 28);
            rangTab.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thick)
                .Border.SetBottomBorder(XLBorderStyleValues.Thick)
                .Border.SetLeftBorder(XLBorderStyleValues.Thick)
                .Border.SetRightBorder(XLBorderStyleValues.Thick);

            rangTab.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws2.Cell("Q4").Style.Alignment.SetWrapText();
            ws2.Cell("U4").Style.Alignment.SetWrapText();

            lastRowss = 7;
            ws2.Column(1).Width = 24;
            ws2.Column(2).Width = 20;
            ws2.Column(3).Width = 30;
            ws2.Column(4).Width = 35;
            ws2.Columns(5, 28).Width = 11;
            #endregion

            prmDSR param = new prmDSR
            {
                date = "",
                param1 = "1",
                param2 = "1"
            };
            DataTable dt4 = CreateTable1(param, "uspfn_gendsrMaster");
            DataTable dt = CreateTable1(param, "uspfn_gendsrgroupmodel");
            DataTable dt2 = CreateTable1(param, "uspfn_gendsrvariant");
            DataTable dt3 = CreateTable1(param, "uspfn_gendsrcolour");
            GenerateExcelDSR(param, wb, dt, dt2, dt3, dt4, lastRow, fileName, true);

            return "OK";
        }

        private void GenerateExcelDSR(prmDSR param, XLWorkbook wb, DataTable dt, DataTable dt2, DataTable dt3, DataTable dt4, int lastRow, string fileName, bool isCustomize = false, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var MRow = lastRow;


            #region model code
            var lastRo = MRow;
            var ws4 = wb.Worksheet(1);
            var tmplastRo = MRow;

            int iCl = 1;
            foreach (DataRow dr in dt4.Rows)
            {
                iCl = 1;
                foreach (DataColumn dc in dt4.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws4.Cell(lastRo, iCl).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws4.Cell(lastRo, iCl).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws4.Cell(lastRo, iCl).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws4.Cell(lastRo, iCl).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws4.Cell(lastRo, iCl).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws4.Cell(lastRo, iCl).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws4.Cell(lastRo, iCl).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws4.Cell(lastRo, iCl).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws4.Cell(lastRo, iCl).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmplastRo == lastRo && isCustomize == false)
                    {
                        ws4.Cell(lastRo, iCl).Value = dc.ColumnName;
                        ws4.Cell(lastRo, iCl).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws4.Cell(lastRo, iCl).Style.Font.SetBold().Font.SetFontSize(10);

                        ws4.Cell(lastRo + 1, iCl).Style.Font.SetFontSize(10);
                        ws4.Cell(lastRo + 1, iCl).Value = val;
                    }
                    else
                    {
                        ws4.Cell(lastRo, iCl).Style.Font.SetFontSize(10);
                        ws4.Cell(lastRo, iCl).Value = val;
                    }

                    iCl++;
                }

                lastRo++;
            }
            ws4.Cell(lastRo, 1).Value = "TOTAL";
            var column = 3;
            for (char i = 'C'; i <= 'Z'; i++)
            {
                ws4.Cell(lastRo, column).FormulaA1 = "=SUM(" + i + (tmplastRo) + ":" + i + (lastRo - 1) + ")";
                column++;
            }
            ws4.Range(lastRo, 1, lastRo, iCl - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws4.Range(lastRo, 1, lastRo, iCl - 1).Style.Font.Bold = true;
            ws4.Range(lastRo, 1, lastRo, iCl - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));
            var rngTabl = ws4.Range(tmplastRo, 1, lastRo, iCl - 1);
            rngTabl.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);

            ws4.Range(tmplastRo, 2, lastRo, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws4.Range(tmplastRo, 2, lastRo, 2).Style.NumberFormat.Format = "###0";
            #endregion

            #region group model
            var ws = wb.Worksheet(2);
            var tmpLastRow = MRow;

            int iCol = 1;
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(lastRow, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow, iCol).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow, iCol).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws.Cell(lastRow, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRow == lastRow && isCustomize == false)
                    {
                        ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                        ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(10);

                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }
                    else
                    {
                        ws.Cell(lastRow, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow, iCol).Value = val;
                    }

                    iCol++;
                }

                lastRow++;
            }
            ws.Cell(lastRow, 1).Value = "TOTAL";
            column = 3;
            for (char i = 'C'; i <= 'Z'; i++)
            {
                ws.Cell(lastRow, column).FormulaA1 = "=SUM(" + i + (tmpLastRow) + ":" + i + (lastRow - 1) + ")";
                column++;
            }
            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.Font.Bold = true;
            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));
            var rngTable = ws.Range(tmpLastRow, 1, lastRow, iCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);

            ws.Range(tmpLastRow, 2, lastRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws.Range(tmpLastRow, 2, lastRow, 2).Style.NumberFormat.Format = "###0";
            //ws.Columns(1, 2).AdjustToContents();
            #endregion

            #region Variant
            var lastRows = MRow;
            var ws1 = wb.Worksheet(3);
            var tmplastRows = lastRows;

            int iCols = 1;
            foreach (DataRow dr in dt2.Rows)
            {
                iCols = 1;
                foreach (DataColumn dc in dt2.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws1.Cell(lastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws1.Cell(lastRows, iCols).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws1.Cell(lastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws1.Cell(lastRows, iCols).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws1.Cell(lastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws1.Cell(lastRows, iCols).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws1.Cell(lastRow, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws1.Cell(lastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws1.Cell(lastRows + 1, iCols).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmplastRows == lastRows && isCustomize == false)
                    {
                        ws1.Cell(lastRows, iCols).Value = dc.ColumnName;
                        ws1.Cell(lastRows, iCols).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws1.Cell(lastRows, iCols).Style.Font.SetBold().Font.SetFontSize(10);

                        ws1.Cell(lastRows + 1, iCols).Style.Font.SetFontSize(10);
                        ws1.Cell(lastRows + 1, iCols).Value = val;
                    }
                    else
                    {
                        ws1.Cell(lastRows, iCols).Style.Font.SetFontSize(10);
                        ws1.Cell(lastRows, iCols).Value = val;
                    }

                    iCols++;
                }

                lastRows++;
            }
            ws1.Cell(lastRows, 1).Value = "TOTAL";
            column = 4;
            for (char i = 'D'; i <= 'Z'; i++)
            {
                ws1.Cell(lastRows, column).FormulaA1 = "=SUM(" + i + (tmplastRows) + ":" + i + (lastRows - 1) + ")";
                column++;
            }
            ws1.Cell(lastRows, 27).FormulaA1 = "=SUM(AA" + (tmplastRows) + ": AA" + (lastRows - 1) + ")";
            ws1.Range(lastRows, 1, lastRows, iCols - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws1.Range(lastRows, 1, lastRows, iCols - 1).Style.Font.Bold = true;
            ws1.Range(lastRows, 1, lastRows, iCols - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));

            var rangTable = ws1.Range(tmplastRows, 1, lastRows, iCols - 1);
            rangTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);

            ws1.Range(tmpLastRow, 2, lastRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws1.Range(tmpLastRow, 2, lastRow, 2).Style.NumberFormat.Format = "###0";
            //ws1.Columns(1, 3).AdjustToContents();
            #endregion

            #region Colour Name

            var lastRowss = MRow;
            var ws2 = wb.Worksheet(4);
            var tmplastRowss = lastRowss;

            int iCools = 1;
            foreach (DataRow dr in dt3.Rows)
            {
                iCools = 1;
                foreach (DataColumn dc in dt3.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws2.Cell(lastRowss, iCools).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws2.Cell(lastRowss, iCools).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws2.Cell(lastRowss, iCools).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws2.Cell(lastRowss, iCools).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws2.Cell(lastRowss, iCools).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws2.Cell(lastRowss, iCools).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws2.Cell(lastRowss, iCools).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws2.Cell(lastRowss, iCools).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws2.Cell(lastRowss + 1, iCools).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmplastRowss == lastRowss && isCustomize == false)
                    {
                        ws2.Cell(lastRowss, iCools).Value = dc.ColumnName;
                        ws2.Cell(lastRowss, iCools).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws2.Cell(lastRowss, iCools).Style.Font.SetBold().Font.SetFontSize(10);

                        ws2.Cell(lastRowss + 1, iCools).Style.Font.SetFontSize(10);
                        ws2.Cell(lastRowss + 1, iCools).Value = val;
                    }
                    else
                    {
                        ws2.Cell(lastRowss, iCools).Style.Font.SetFontSize(10);
                        ws2.Cell(lastRowss, iCools).Value = val;
                    }

                    iCools++;
                }

                lastRowss++;
            }
            ws2.Cell(lastRowss, 1).Value = "TOTAL";
            column = 5;
            for (char i = 'E'; i <= 'Z'; i++)
            {
                ws2.Cell(lastRowss, column).FormulaA1 = "=SUM(" + i + (tmplastRowss) + ":" + i + (lastRowss - 1) + ")";
                column++;
            }
            ws2.Cell(lastRowss, 27).FormulaA1 = "=SUM(AA" + (tmplastRowss) + ": AA" + (lastRowss - 1) + ")";
            ws2.Cell(lastRowss, 28).FormulaA1 = "=SUM(AB" + (tmplastRowss) + ": AB" + (lastRowss - 1) + ")";
            ws2.Range(lastRowss, 1, lastRowss, iCools - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws2.Range(lastRowss, 1, lastRowss, iCools - 1).Style.Font.Bold = true;
            ws2.Range(lastRowss, 1, lastRowss, iCools - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));

            var rangTables = ws2.Range(tmplastRowss, 1, lastRowss, iCools - 1);
            rangTables.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);
            ws2.Range(tmplastRowss, 2, lastRowss, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws2.Range(tmplastRowss, 2, lastRowss, 2).Style.NumberFormat.Format = "###0";
            //ws2.Columns(1, 4).AdjustToContents();
            #endregion

            string fullFileName = Server.MapPath("~/ReportTemp/" + fileName + ".xlsx");
            wb.SaveAs(fullFileName);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_sysdealergetmaillist 'DSR'";            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dtX = new DataTable();
            da.Fill(dtX);

            var Recepient = "sdms.osen.kusnadi@suzuki.co.id";
            if (dtX.Rows.Count > 0)
            {
                Recepient = dtX.Rows[0][0].ToString();
            }
            var getdate = DateTime.Now;
            var CopyRecepient = "";
            var profilename = "MailServer";
            var subject = "Daily Sales Report";

            if (fullFileName.Substring(0,11) == "D:\\Sdms.Web"){
                fullFileName = "";
            }

            SqlCommand cmds = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmds.CommandTimeout = 3600;
            cmds.CommandText = "uspfn_insertMailProfile '" + profilename + "','" + fullFileName + "','" + Recepient + "','" + CopyRecepient + "','" + subject + "','Developer'";
            SqlDataAdapter das = new SqlDataAdapter(cmds);
            DataTable dtXs = new DataTable();
            das.Fill(dtXs);
        }

        #endregion

        #region ActionResult DSR WEB

        public ActionResult DSR_WEB(string strdate, string fltrDate, string upslsmdlcd, string uptblrgstr) 
        {
            int lastRo = 1;
            int lastRow = 1;
            int lastRows = 1;
            int lastRowss = 1;
            var wb = new XLWorkbook();
            var ws3 = wb.Worksheets.Add("Model Code"); 
            var ws = wb.Worksheets.Add("Group Model");
            var ws1 = wb.Worksheets.Add("Variant");
            var ws2 = wb.Worksheets.Add("Colour");
            DateTime now = DateTime.Now;
            string fileName = "DSR_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            string username = CurrentUser.Username;

            #region Model Code
            ws3.Columns().Style.Font.FontName = "Arial";
            ws3.Columns().Style.Font.FontSize = 10;
            //ws3.Range("A1", "F1").Merge();
            ws3.Cell("A" + lastRo).Value = "Printed by";
            ws3.Cell("B" + lastRo).Value = ": " + username + "/ " + strdate;
            ws3.Cell("A" + lastRo).Style.Font.FontSize = 10;
            ws3.Cell("A" + lastRo).Style.Font.Bold = true;
            ws3.Cell("B" + lastRo).Style.Font.FontSize = 10;
            ws3.Cell("B" + lastRo).Style.Font.Bold = true;

            //ws3.Range("A2", "B2").Merge();
            lastRo = 2;
            ws3.Cell("A" + lastRo).Value = "Daily Sales Report - by Model Code";
            ws3.Cell("A" + lastRo).Style.Font.FontSize = 14;
            ws3.Cell("A" + lastRo).Style.Font.Bold = true;

            lastRo = 4;
            ws3.Range("A4", "A6").Merge();
            ws3.Cell("A4").Value = "Model Code";
            ws3.Range("B4", "B6").Merge();
            ws3.Cell("B4").Value = "Colour Code";

            ws3.Range("C4", "E5").Merge();
            ws3.Cell("C4").Value = "INQUIRY";
            ws3.Range("F4", "H5").Merge();
            ws3.Cell("F4").Value = "SPK";
            ws3.Range("I4", "K5").Merge();
            ws3.Cell("I4").Value = "RKA";
            ws3.Range("L4", "N5").Merge();
            ws3.Cell("L4").Value = "SIS SALES";
            ws3.Range("O4", "O6").Merge();
            ws3.Cell("O4").Value = "SIS STOCK";
            ws3.Range("P4", "R5").Merge();
            ws3.Cell("P4").Value = "Registration this year";
            ws3.Range("S4", "S6").Merge();
            ws3.Cell("S4").Value = "Dealer STOCK";
            ws3.Range("T4", "V5").Merge();
            ws3.Cell("T4").Value = "Registration last year";
            ws3.Range("W4", "Z4").Merge();
            ws3.Cell("W4").Value = "Registration previous month";
            ws3.Range("W5", "X5").Merge();
            ws3.Cell("W5").Value = "Total Days";
            ws3.Range("Y5", "Z5").Merge();
            ws3.Cell("Y5").Value = "Working Days";

            lastRo = 6;
            char[] lop = { 'C', 'F', 'I', 'L', 'P', 'T', 'W', 'Y' };
            for (var p = 0; p < lop.Length; p++)
            {
                if (lop[p] == 'W' || lop[p] == 'Y')
                {
                    ws3.Cell((lop[p]).ToString() + lastRo).Value = "DD";
                    ws3.Cell(((char)(lop[p] + 1)).ToString() + lastRo).Value = "MM";
                    //.Cell(((char)loop[p] + 2).ToString() + lastRo).Value = "YY";
                }
                else
                {
                    ws3.Cell((lop[p]).ToString() + lastRo).Value = "DD";
                    ws3.Cell(((char)(lop[p] + 1)).ToString() + lastRo).Value = "MM";
                    ws3.Cell(((char)(lop[p] + 2)).ToString() + lastRo).Value = "YY";
                }
            }
            ws3.Range(4, 1, 6, 26).Style.Font.Bold = true;

            var rangTabl = ws3.Range(4, 1, 6, 26);
            rangTabl.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thick)
                .Border.SetBottomBorder(XLBorderStyleValues.Thick)
                .Border.SetLeftBorder(XLBorderStyleValues.Thick)
                .Border.SetRightBorder(XLBorderStyleValues.Thick);

            rangTabl.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws3.Cell("O4").Style.Alignment.SetWrapText();
            ws3.Cell("S4").Style.Alignment.SetWrapText();

            lastRo = 7;
            ws3.Column(1).Width = 25;
            ws3.Column(2).Width = 22;
            ws3.Columns(3, 26).Width = 12;
            #endregion

            #region group Model
            ws.Columns().Style.Font.FontName = "Arial";
            ws.Columns().Style.Font.FontSize = 10;
            //ws.Range("A1", "F1").Merge();
            ws.Cell("A" + lastRow).Value = "Printed by";
            ws.Cell("B" + lastRow).Value = ": " + username + "/ " + strdate;
            ws.Cell("A" + lastRow).Style.Font.FontSize = 10;
            ws.Cell("A" + lastRow).Style.Font.Bold = true;
            ws.Cell("B" + lastRow).Style.Font.FontSize = 10;
            ws.Cell("B" + lastRow).Style.Font.Bold = true;

            //ws.Range("A2", "B2").Merge();
            lastRow = 2;
            ws.Range("A" + lastRow, "C" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Daily Sales Report - by Group Model";
            ws.Cell("A" + lastRow).Style.Font.FontSize = 14;
            ws.Cell("A" + lastRow).Style.Font.Bold = true;

            lastRow = 4;
            ws.Range("A4", "A6").Merge();
            ws.Cell("A4").Value = "GROUP MODEL";

            ws.Range("B4", "B6").Merge();
            ws.Cell("B4").Value = "YEARS";

            ws.Range("c4", "E5").Merge();
            ws.Cell("c4").Value = "INQUIRY";
            ws.Range("F4", "H5").Merge();
            ws.Cell("F4").Value = "SPK";
            ws.Range("I4", "K5").Merge();
            ws.Cell("I4").Value = "RKA";
            ws.Range("L4", "N5").Merge();
            ws.Cell("L4").Value = "SIS SALES";
            ws.Range("O4", "O6").Merge();
            ws.Cell("O4").Value = "SIS STOCK";
            ws.Range("P4", "R5").Merge();
            ws.Cell("P4").Value = "Registration this year";
            ws.Range("S4", "S6").Merge();
            ws.Cell("S4").Value = "Dealer STOCK";
            ws.Range("T4", "V5").Merge();
            ws.Cell("T4").Value = "Registration last year";
            ws.Range("W4", "Z4").Merge();
            ws.Cell("W4").Value = "Registration previous month";
            ws.Range("W5", "X5").Merge();
            ws.Cell("W5").Value = "Total Days";
            ws.Range("Y5", "Z5").Merge();
            ws.Cell("Y5").Value = "Working Days";

            lastRow = 6;
            char[] loop = { 'C', 'F', 'I', 'L', 'P', 'T', 'W', 'Y' };
            for (var p = 0; p < loop.Length; p++)
            {
                if (loop[p] == 'W' || loop[p] == 'Y')
                {
                    ws.Cell((loop[p]).ToString() + lastRow).Value = "DD";
                    ws.Cell(((char)(loop[p] + 1)).ToString() + lastRow).Value = "MM";
                    //.Cell(((char)loop[p] + 2).ToString() + lastRow).Value = "YY";
                }
                else
                {
                    ws.Cell((loop[p]).ToString() + lastRow).Value = "DD";
                    ws.Cell(((char)(loop[p] + 1)).ToString() + lastRow).Value = "MM";
                    ws.Cell(((char)(loop[p] + 2)).ToString() + lastRow).Value = "YY";
                }
            }
            ws.Range(4, 1, 6, 25).Style.Font.Bold = true;

            var rngTable = ws.Range(4, 1, 6, 26);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thick)
                .Border.SetBottomBorder(XLBorderStyleValues.Thick)
                .Border.SetLeftBorder(XLBorderStyleValues.Thick)
                .Border.SetRightBorder(XLBorderStyleValues.Thick);

            rngTable.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Cell("O4").Style.Alignment.SetWrapText();
            ws.Cell("S4").Style.Alignment.SetWrapText();

            lastRow = 7;
            ws.Column(1).Width = 23;
            ws.Column(2).Width = 20;
            ws.Columns(3, 26).Width = 11;
            #endregion

            #region variant
            ws1.Columns().Style.Font.FontName = "Arial";
            ws1.Columns().Style.Font.FontSize = 10;
            //ws1.Range("A1", "F1").Merge();
            ws1.Cell("A" + lastRows).Value = "Printed by";
            ws1.Cell("B" + lastRows).Value = ": " + username + "/ " + strdate;
            ws1.Cell("A" + lastRows).Style.Font.FontSize = 10;
            ws1.Cell("A" + lastRows).Style.Font.Bold = true;
            ws1.Cell("B" + lastRows).Style.Font.FontSize = 10;
            ws1.Cell("B" + lastRows).Style.Font.Bold = true;

            //ws1.Range("A2", "B2").Merge();
            lastRows = 2;
            ws1.Range("A" + lastRows, "C" + lastRows).Merge();
            ws1.Cell("A" + lastRows).Value = "Daily Sales Report - by Variant";
            ws1.Cell("A" + lastRows).Style.Font.FontSize = 14;
            ws1.Cell("A" + lastRows).Style.Font.Bold = true;

            lastRows = 4;
            ws1.Range("A4", "A6").Merge();
            ws1.Cell("A4").Value = "GROUP MODEL";
            ws1.Range("B4", "B6").Merge();
            ws1.Cell("B4").Value = "YEARS";
            ws1.Range("C4", "C6").Merge();
            ws1.Cell("C4").Value = "VARIANT";

            ws1.Range("D4", "F5").Merge();
            ws1.Cell("D4").Value = "INQUIRY";
            ws1.Range("G4", "I5").Merge();
            ws1.Cell("G4").Value = "SPK";
            ws1.Range("J4", "L5").Merge();
            ws1.Cell("J4").Value = "RKA";
            ws1.Range("M4", "M5").Merge();
            ws1.Cell("M4").Value = "SIS SALES";
            ws1.Range("P4", "P6").Merge();
            ws1.Cell("P4").Value = "SIS STOCK";
            ws1.Range("Q4", "S5").Merge();
            ws1.Cell("Q4").Value = "Registration this year";
            ws1.Range("T4", "T6").Merge();
            ws1.Cell("T4").Value = "Dealer STOCK";
            ws1.Range("U4", "W5").Merge();
            ws1.Cell("U4").Value = "Registration last year";
            ws1.Range("X4", "Z4").Merge();
            ws1.Cell("X4").Value = "Registration previous month";
            ws1.Range("X5", "Y5").Merge();
            ws1.Cell("X5").Value = "Total Days";
            ws1.Range("Z5", "AA5").Merge();
            ws1.Cell("Z5").Value = "Working Days";

            lastRows = 6;
            char[] looop = { 'D', 'G', 'J', 'M', 'Q', 'U', 'X', 'Z' };
            for (var p = 0; p < looop.Length; p++)
            {
                if (lop[p] == 'X' || looop[p] == 'Z')
                {
                    ws1.Cell((looop[p]).ToString() + lastRows).Value = "DD";
                    if (lop[p] == 'Z')
                    {
                        ws1.Cell("AA" + lastRows).Value = "MM";
                    }
                    else
                    {
                        ws1.Cell(((char)(lop[p] + 1)).ToString() + lastRows).Value = "MM";
                    }
                    
                    //.Cell(((char)loop[p] + 2).ToString() + lastRows).Value = "YY";
                }
                else
                {
                    ws1.Cell((looop[p]).ToString() + lastRows).Value = "DD";
                    ws1.Cell(((char)(looop[p] + 1)).ToString() + lastRows).Value = "MM";
                    ws1.Cell(((char)(looop[p] + 2)).ToString() + lastRows).Value = "YY";
                }
            }
            ws1.Range(4, 1, 6, 26).Style.Font.Bold = true;

            var rangTables = ws1.Range(4, 1, 6, 27);
            rangTables.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thick)
                .Border.SetBottomBorder(XLBorderStyleValues.Thick)
                .Border.SetLeftBorder(XLBorderStyleValues.Thick)
                .Border.SetRightBorder(XLBorderStyleValues.Thick);

            rangTables.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws1.Cell("P4").Style.Alignment.SetWrapText();
            ws1.Cell("T4").Style.Alignment.SetWrapText();

            lastRows = 7;
            ws1.Column(1).Width = 23;
            ws1.Column(2).Width = 20;
            ws1.Column(3).Width = 30;
            ws1.Columns(4, 27).Width = 11;
            #endregion
          
            #region Colour
            ws2.Columns().Style.Font.FontName = "Arial";
            ws2.Columns().Style.Font.FontSize = 10;
            //ws2.Range("A1", "F1").Merge();
            ws2.Cell("A" + lastRowss).Value = "Printed by";
            ws2.Cell("B" + lastRowss).Value = ": " + username + "/ " + strdate;
            ws2.Cell("A" + lastRowss).Style.Font.FontSize = 10;
            ws2.Cell("A" + lastRowss).Style.Font.Bold = true;
            ws2.Cell("B" + lastRowss).Style.Font.FontSize = 10;
            ws2.Cell("B" + lastRowss).Style.Font.Bold = true;

            //ws2.Range("A2", "B2").Merge();
            lastRowss = 2;
            ws2.Range("A" + lastRowss, "C" + lastRowss).Merge();
            ws2.Cell("A" + lastRowss).Value = "Daily Sales Report - by Colour";
            ws2.Cell("A" + lastRowss).Style.Font.FontSize = 14;
            ws2.Cell("A" + lastRowss).Style.Font.Bold = true;

            lastRowss = 4;
            ws2.Range("A4", "A6").Merge();
            ws2.Cell("A4").Value = "GROUP MODEL";
            ws2.Range("B4", "B6").Merge();
            ws2.Cell("B4").Value = "YEARS";
            ws2.Range("C4", "C6").Merge();
            ws2.Cell("C4").Value = "VARIANT";
            ws2.Range("D4", "D6").Merge();
            ws2.Cell("D4").Value = "COLOUR NAME";
            
            ws2.Range("E4", "G5").Merge();
            ws2.Cell("E4").Value = "INQUIRY";
            ws2.Range("H4", "J5").Merge();
            ws2.Cell("H4").Value = "SPK";
            ws2.Range("K4", "M5").Merge();
            ws2.Cell("K4").Value = "RKA";
            ws2.Range("N4", "P5").Merge();
            ws2.Cell("N4").Value = "SIS SALES";
            ws2.Range("Q4", "Q6").Merge();
            ws2.Cell("Q4").Value = "SIS STOCK";
            ws2.Range("R4", "T5").Merge();
            ws2.Cell("R4").Value = "Registration this year";
            ws2.Range("U4", "U6").Merge();
            ws2.Cell("U4").Value = "Dealer STOCK";
            ws2.Range("V4", "X5").Merge();
            ws2.Cell("V4").Value = "Registration last year";
            ws2.Range("Y4", "AB4").Merge();
            ws2.Cell("Y4").Value = "Registration previous month";
            ws2.Range("Y5", "Z5").Merge();
            ws2.Cell("Y5").Value = "Total Days";
            ws2.Range("AA5", "AB5").Merge();
            ws2.Cell("AA5").Value = "Working Days";
            lastRowss = 6;
            char[] loooop = { 'E', 'H', 'K', 'N', 'R', 'V', 'Y' };
            for (var p = 0; p < loooop.Length; p++)
            {
                if (loooop[p] == 'Y')
                {
                    ws2.Cell((loooop[p]).ToString() + lastRowss).Value = "DD";
                    ws2.Cell(((char)(loooop[p] + 1)).ToString() + lastRowss).Value = "MM";
                    //.Cell(((char)loop[p] + 2).ToString() + lastRowss).Value = "YY";
                }
                else
                {
                    ws2.Cell((loooop[p]).ToString() + lastRowss).Value = "DD";
                    ws2.Cell(((char)(loooop[p] + 1)).ToString() + lastRowss).Value = "MM";
                    ws2.Cell(((char)(loooop[p] + 2)).ToString() + lastRowss).Value = "YY";
                }
            }
            ws2.Range(4, 1, 6, 27).Style.Font.Bold = true;
            ws2.Cell("AA" + lastRowss).Value = "DD";
            ws2.Cell("AB" + lastRowss).Value = "MM";
            var rangTab = ws2.Range(4, 1, 6, 28);
            rangTab.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thick)
                .Border.SetBottomBorder(XLBorderStyleValues.Thick)
                .Border.SetLeftBorder(XLBorderStyleValues.Thick)
                .Border.SetRightBorder(XLBorderStyleValues.Thick);

            rangTab.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws2.Cell("Q4").Style.Alignment.SetWrapText();
            ws2.Cell("U4").Style.Alignment.SetWrapText();

            lastRowss = 7;
            ws2.Column(1).Width = 24;
            ws2.Column(2).Width = 20;
            ws2.Column(3).Width = 30;
            ws2.Column(4).Width = 35;
            ws2.Columns(5, 28).Width = 11;
            #endregion

            prmDSR param = new prmDSR
            {
                date = fltrDate,
                param1 = upslsmdlcd,
                param2 = uptblrgstr
            };
            DataTable dt4 = CreateTable1(param, "uspfn_gendsrMaster");
            DataTable dt = CreateTable1(param, "uspfn_gendsrgroupmodel");
            DataTable dt2 = CreateTable1(param, "uspfn_gendsrvariant");
            DataTable dt3 = CreateTable1(param, "uspfn_gendsrcolour"); 
            //DataTable dt = CreateTable1(param, "uspfn_testdsr");
            //DataTable dt2 = CreateTable1(param, "uspfn_testdsr2");
            //DataTable dt3 = CreateTable1(param, "uspfn_testdsr3");
            //DataTable dt4 = CreateTable1(param, "uspfn_testdsr");
            return GenerateExcel_DSR(param, wb, dt, dt2, dt3, dt4, lastRow, fileName, true);
        }

        private ActionResult GenerateExcel_DSR(prmDSR param, XLWorkbook wb, DataTable dt, DataTable dt2, DataTable dt3, DataTable dt4, int lastRow, string fileName, bool isCustomize = false, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var MRow = lastRow;

            #region model code
            var lastRo = MRow;
            var ws4 = wb.Worksheet(1);
            var tmplastRo = MRow; 

            int iCl = 1;
            foreach (DataRow dr in dt4.Rows)
            {
                iCl = 1;
                foreach (DataColumn dc in dt4.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws4.Cell(lastRo, iCl).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws4.Cell(lastRo, iCl).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws4.Cell(lastRo, iCl).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws4.Cell(lastRo, iCl).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws4.Cell(lastRo, iCl).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws4.Cell(lastRo, iCl).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws4.Cell(lastRo, iCl).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws4.Cell(lastRo, iCl).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws4.Cell(lastRo, iCl).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmplastRo == lastRo && isCustomize == false)
                    {
                        ws4.Cell(lastRo, iCl).Value = dc.ColumnName;
                        ws4.Cell(lastRo, iCl).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws4.Cell(lastRo, iCl).Style.Font.SetBold().Font.SetFontSize(10);

                        ws4.Cell(lastRo + 1, iCl).Style.Font.SetFontSize(10);
                        ws4.Cell(lastRo + 1, iCl).Value = val;
                    }
                    else
                    {
                        ws4.Cell(lastRo, iCl).Style.Font.SetFontSize(10);
                        ws4.Cell(lastRo, iCl).Value = val;
                    }

                    iCl++;
                }

                lastRo++;
            }
            ws4.Cell(lastRo, 1).Value = "TOTAL";
            var column = 3;
            for (char i = 'C'; i <= 'Z'; i++)
            {
                ws4.Cell(lastRo, column).FormulaA1 = "=SUM(" + i + (tmplastRo) + ":" + i + (lastRo - 1) + ")";
                column++;
            }
            ws4.Range(lastRo,1,lastRo, iCl - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws4.Range(lastRo, 1, lastRo, iCl - 1).Style.Font.Bold = true;
            ws4.Range(lastRo, 1, lastRo, iCl - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));
            var rngTabl = ws4.Range(tmplastRo, 1, lastRo, iCl - 1);
            rngTabl.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);

            ws4.Range(tmplastRo, 2, lastRo, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws4.Range(tmplastRo, 2, lastRo, 2).Style.NumberFormat.Format = "###0";
            #endregion

            #region group model
            var ws = wb.Worksheet(2);
            var tmpLastRow = MRow;

            int iCol = 1;
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(lastRow, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow, iCol).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow, iCol).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws.Cell(lastRow, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRow == lastRow && isCustomize == false)
                    {
                        ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                        ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(10);

                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }
                    else
                    {
                        ws.Cell(lastRow, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow, iCol).Value = val;
                    }

                    iCol++;
                }

                lastRow++;
            }
            ws.Cell(lastRow, 1).Value = "TOTAL";
            column = 3;
            for (char i = 'C'; i <= 'Z'; i++)
            {
                ws.Cell(lastRow, column).FormulaA1 = "=SUM(" + i + (tmpLastRow) + ":" + i + (lastRow - 1) + ")";
                column++;
            }
            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.Font.Bold = true;
            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));
            var rngTable = ws.Range(tmpLastRow, 1, lastRow, iCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);

            ws.Range(tmpLastRow, 2, lastRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws.Range(tmpLastRow, 2, lastRow, 2).Style.NumberFormat.Format = "###0";
            //ws.Columns(1, 2).AdjustToContents();
            #endregion

            #region Variant
            var lastRows = MRow;
            var ws1 = wb.Worksheet(3);
            var tmplastRows = lastRows;

            int iCols = 1;
            foreach (DataRow dr in dt2.Rows)
            {
                iCols = 1;
                foreach (DataColumn dc in dt2.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws1.Cell(lastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws1.Cell(lastRows, iCols).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws1.Cell(lastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws1.Cell(lastRows, iCols).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws1.Cell(lastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws1.Cell(lastRows, iCols).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws1.Cell(lastRow, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws1.Cell(lastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws1.Cell(lastRows + 1, iCols).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmplastRows == lastRows && isCustomize == false)
                    {
                        ws1.Cell(lastRows, iCols).Value = dc.ColumnName;
                        ws1.Cell(lastRows, iCols).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws1.Cell(lastRows, iCols).Style.Font.SetBold().Font.SetFontSize(10);

                        ws1.Cell(lastRows + 1, iCols).Style.Font.SetFontSize(10);
                        ws1.Cell(lastRows + 1, iCols).Value = val;
                    }
                    else
                    {
                        ws1.Cell(lastRows, iCols).Style.Font.SetFontSize(10);
                        ws1.Cell(lastRows, iCols).Value = val;
                    }

                    iCols++;
                }

                lastRows++;
            }
            ws1.Cell(lastRows, 1).Value = "TOTAL";
            column = 4;
            for (char i = 'D'; i <= 'Z'; i++)
            {
                ws1.Cell(lastRows, column).FormulaA1 = "=SUM(" + i + (tmplastRows) + ":" + i + (lastRows - 1) + ")";
                column++;
            }
            ws1.Cell(lastRows, 27).FormulaA1 = "=SUM(AA" + (tmplastRows) + ": AA" + (lastRows - 1) + ")";
            ws1.Range(lastRows, 1, lastRows, iCols - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws1.Range(lastRows, 1, lastRows, iCols - 1).Style.Font.Bold = true;
            ws1.Range(lastRows, 1, lastRows, iCols - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));
            
            var rangTable = ws1.Range(tmplastRows, 1, lastRows, iCols - 1);
            rangTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);

            ws1.Range(tmpLastRow, 2, lastRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws1.Range(tmpLastRow, 2, lastRow, 2).Style.NumberFormat.Format = "###0";
            //ws1.Columns(1, 3).AdjustToContents();
            #endregion

            #region Colour Name
            
            var lastRowss = MRow;
            var ws2 = wb.Worksheet(4);
            var tmplastRowss = lastRowss;

            int iCools = 1;
            foreach (DataRow dr in dt3.Rows)
            {
                iCools = 1;
                foreach (DataColumn dc in dt3.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws2.Cell(lastRowss, iCools).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws2.Cell(lastRowss, iCools).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws2.Cell(lastRowss, iCools).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws2.Cell(lastRowss, iCools).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws2.Cell(lastRowss, iCools).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws2.Cell(lastRowss, iCools).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws2.Cell(lastRowss, iCools).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws2.Cell(lastRowss, iCools).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws2.Cell(lastRowss + 1, iCools).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmplastRowss == lastRowss && isCustomize == false)
                    {
                        ws2.Cell(lastRowss, iCools).Value = dc.ColumnName;
                        ws2.Cell(lastRowss, iCools).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws2.Cell(lastRowss, iCools).Style.Font.SetBold().Font.SetFontSize(10);

                        ws2.Cell(lastRowss + 1, iCools).Style.Font.SetFontSize(10);
                        ws2.Cell(lastRowss + 1, iCools).Value = val;
                    }
                    else
                    {
                        ws2.Cell(lastRowss, iCools).Style.Font.SetFontSize(10);
                        ws2.Cell(lastRowss, iCools).Value = val;
                    }

                    iCools++;
                }

                lastRowss++;
            }
            ws2.Cell(lastRowss, 1).Value = "TOTAL";
            column = 5;
            for (char i = 'E'; i <= 'Z'; i++)
            {
                ws2.Cell(lastRowss, column).FormulaA1 = "=SUM(" + i + (tmplastRowss) + ":" + i + (lastRowss - 1) + ")";
                column++;
            }
            ws2.Cell(lastRowss, 27).FormulaA1 = "=SUM(AA" + (tmplastRowss) + ": AA" + (lastRowss - 1) + ")";
            ws2.Cell(lastRowss, 28).FormulaA1 = "=SUM(AB" + (tmplastRowss) + ": AB" + (lastRowss - 1) + ")";
            ws2.Range(lastRowss, 1, lastRowss, iCools - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws2.Range(lastRowss, 1, lastRowss, iCools - 1).Style.Font.Bold = true;
            ws2.Range(lastRowss, 1, lastRowss, iCools - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));
            
            var rangTables = ws2.Range(tmplastRowss, 1, lastRowss, iCools - 1);
            rangTables.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);
            ws2.Range(tmplastRowss, 2, lastRowss, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws2.Range(tmplastRowss, 2, lastRowss, 2).Style.NumberFormat.Format = "###0";
            //ws2.Columns(1, 4).AdjustToContents();
            #endregion

            var CopyRecepient = "";
            var profilename = "MailServer";
            var subject = "Daily Sales Report";
            var fullFileName = "";
            var Recepient = "sdms.abdul.syawal@suzuki.co.id";

            SqlCommand cmds = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmds.CommandTimeout = 3600;
            cmds.CommandText = "uspfn_insertMailProfile '" + profilename + "','" + fullFileName + "','" + Recepient + "','" + CopyRecepient + "','" + subject + "','Developer'";
            SqlDataAdapter das = new SqlDataAdapter(cmds);
            DataTable dtXs = new DataTable();
            das.Fill(dtXs);

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }
            
        #endregion

        private void setAlign (Workbook workbook, int row1, int col1, int row2, int col2, Alignment align  ){
            var sheet = workbook.Worksheets[0];
            if (align.isHorizontal){
                sheet.Range[row1, col1, row2, col2].Style.HorizontalAlignment = HorizontalAlignType.Center;
            }
            if (align.isVertical)
            {
                sheet.Range[row1, col1, row2, col2].Style.VerticalAlignment = VerticalAlignType.Center;
            }
            if (align.isRight)
            {
                sheet.Range[row1, col1, row2, col2].Style.HorizontalAlignment = HorizontalAlignType.Right;
            }
            if (align.isLeft)
            {
                sheet.Range[row1, col1, row2, col2].Style.HorizontalAlignment = HorizontalAlignType.Left;
            }
        }


    }
}
