using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using SimDms.DataWarehouse.Controllers;
using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.Objects;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Excel = Microsoft.Office.Interop.Excel;
using GeLang;

namespace SimDms.PreSales.Controllers.Api
{
    public class InquiryProdController : BaseController
    {
        static string ReplaceHexadecimalSymbols(string txt)
        {
            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26\x19]";
            return Regex.Replace(txt, r, "", RegexOptions.Compiled);
        }

        #region Private Method

        public JsonResult Years()
        {
            List<object> listOfYears = new List<object>();
            int after = DateTime.Now.Year + 5;
            int before = DateTime.Now.Year - 10;
            for (int i = before; i <= after; i++)
            {
                listOfYears.Add(new { value = i, text = i });
            }

            return Json(listOfYears);
        }

        public JsonResult Months()
        {
            List<object> listOfMonths = new List<object>();
            int Now = DateTime.Now.Year;
            int Past = DateTime.Now.Year - 10;

            listOfMonths.Add(new { value = 1, text = "January" });
            listOfMonths.Add(new { value = 2, text = "February" });
            listOfMonths.Add(new { value = 3, text = "March" });
            listOfMonths.Add(new { value = 4, text = "April" });
            listOfMonths.Add(new { value = 5, text = "May" });
            listOfMonths.Add(new { value = 6, text = "June" });
            listOfMonths.Add(new { value = 7, text = "July" });
            listOfMonths.Add(new { value = 8, text = "August" });
            listOfMonths.Add(new { value = 9, text = "September" });
            listOfMonths.Add(new { value = 10, text = "October" });
            listOfMonths.Add(new { value = 11, text = "November" });
            listOfMonths.Add(new { value = 12, text = "December" });

            return Json(listOfMonths);
        }

        private class InqEmployee
        {
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string UserID { get; set; }
            public string OutletID { get; set; }
            public string OutletName { get; set; }
        }

        private class InquiryBtn
        {
            public string GroupArea { get; set; }
            public string Area { get; set; }
            public string DealerCode { get; set; }
            public string DealerName { get; set; }
            public string OutletCode { get; set; }
            public string OutletName { get; set; }
        }

        private class Combo
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }

        private ActionResult GenerateExcel(XLWorkbook wb, DataTable dt, int lastRow, string fileName, bool isCustomHeader = false, bool isShowSummary = false)
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
                        ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(10);

                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }
                    else
                    {
                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }

                    iCol++;
                }

                lastRow++;
            }

            if (isShowSummary)
            {
                ws.Cell(lastRow + 1, 1).Value = "TOTAL";
                for (char i = 'A'; i <= iCol; i++)
                {
                    ws.Cell(lastRow + 1, 1).FormulaA1 = "=SUM(" + i + (tmpLastRow + 1) + ":" + i + lastRow + ")";
                }
            }

            var rngTable = ws.Range(tmpLastRow, 1, lastRow, iCol - 1);
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

        private ActionResult GenerateExcelR2(XLWorkbook wb, DataTable dt, int lastRow, string fileName, bool isCustomHeader = false, bool isShowSummary = false)
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
                    if (lastRow + 1 == 5 || lastRow + 1 == 70 || lastRow + 1 == 87 || lastRow + 1 == 122 || lastRow + 1 == 139 || lastRow + 1 == 164 || lastRow + 1 == 183 || lastRow + 1 == 188)
                    {
                        switch (Type.GetTypeCode(typ))
                        {
                            case TypeCode.DateTime:
                                ws.Cell(lastRow + 2, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Cell(lastRow + 2, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                                break;
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.Double:
                            case TypeCode.Single:
                                ws.Cell(lastRow + 2, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(lastRow + 2, iCol).Style.NumberFormat.Format = "#,##0";
                                break;
                            case TypeCode.Decimal:
                                ws.Cell(lastRow + 2, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(lastRow + 2, iCol).Style.NumberFormat.Format = "#,##0.0";
                                break;
                            case TypeCode.Boolean:
                                ws.Cell(lastRow + 2, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                break;
                            default:
                                if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                                {
                                    val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                                }
                                ws.Cell(lastRow + 2, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                break;
                        };

                    }
                    else
                    {
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

                    }


                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }


                    if (tmpLastRow == lastRow)
                    {
                        ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                        ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.White).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(14);

                        switch (lastRow + 1)
                        {
                            case 5:
                                if (iCol == 2)
                                {
                                    ws.Cell(lastRow + 1, 2).Value = "A. SALES REVENUE";
                                    ws.Cell(lastRow + 1, 2).Style.Font.Bold = true;
                                    ws.Cell(lastRow + 1, 2).Style.Font.SetFontSize(12);
                                }
                                if (iCol == 3)
                                {
                                    ws.Cell(lastRow + 3, iCol - 1).Style.Font.SetFontSize(10);
                                    ws.Cell(lastRow + 3, iCol - 1).Value = val;
                                }
                                else
                                {
                                    ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10);
                                    ws.Cell(lastRow + 2, iCol).Value = val;
                                }


                                break;
                            default:
                                ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10);
                                ws.Cell(lastRow + 2, iCol).Value = val;
                                break;
                        };
                        //ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        //ws.Cell(lastRow + 1, iCol).Value = val;
                    }
                    else
                    {
                        if (tmpLastRow != lastRow)
                        {
                            switch (lastRow + 1)
                            {
                                case 70:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow + 1, 2).Value = "B. NO OF UNIT INTAKE";
                                        ws.Cell(lastRow + 1, 2).Style.Font.Bold = true;
                                        ws.Cell(lastRow + 1, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 3, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 3, iCol - 1).Value = val;
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 2, iCol).Value = val;
                                    }
                                    break;
                                case 87:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow + 1, 2).Value = "C. NO OF JOB TYPE";
                                        ws.Cell(lastRow + 1, 2).Style.Font.Bold = true;
                                        ws.Cell(lastRow + 1, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 3, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 3, iCol - 1).Value = val;
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 2, iCol).Value = val;
                                    }
                                    break;
                                case 122:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow + 1, 2).Value = "D. WORKSHOP SERVICE STRENGTH";
                                        ws.Cell(lastRow + 1, 2).Style.Font.Bold = true;
                                        ws.Cell(lastRow + 1, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 3, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 3, iCol - 1).Value = val;
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 2, iCol).Value = val;
                                    }
                                    break;
                                case 139:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow + 1, 2).Value = "E. PRODUCTIVITY INDICATORS";
                                        ws.Cell(lastRow + 1, 2).Style.Font.Bold = true;
                                        ws.Cell(lastRow + 1, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 3, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 3, iCol - 1).Value = val;
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 2, iCol).Value = val;
                                    }
                                    break;
                                case 164:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow + 1, 2).Value = "F. SERVICE RETENTION & MARKETING ACTIVITY";
                                        ws.Cell(lastRow + 1, 2).Style.Font.Bold = true;
                                        ws.Cell(lastRow + 1, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 3, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 3, iCol - 1).Value = val;
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 2, iCol).Value = val;
                                    }
                                    break;
                                case 183:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow + 1, 2).Value = "G. UNIT SALES MOTOR CYCLE";
                                        ws.Cell(lastRow + 1, 2).Style.Font.Bold = true;
                                        ws.Cell(lastRow + 1, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 3, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 3, iCol - 1).Value = val;
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 2, iCol).Value = val;
                                    }
                                    break;
                                case 188:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow + 1, 2).Value = "H. CSI PERFORMANCE";
                                        ws.Cell(lastRow + 1, 2).Style.Font.Bold = true;
                                        ws.Cell(lastRow + 1, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 3, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 3, iCol - 1).Value = val;
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 2, iCol).Value = val;
                                    }
                                    break;
                                default:
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 2, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 2, iCol - 1).Value = val;
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                    }

                                    break;
                            };

                        }

                    }
                    iCol++;
                }
                switch (lastRow)
                {
                    case 4:
                        lastRow++; lastRow++; lastRow++;
                        break;
                    case 65:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 69:
                        lastRow++; lastRow++; lastRow++;
                        break;
                    case 82:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 86:
                        lastRow++; lastRow++; lastRow++;
                        break;
                    case 117:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 121:
                        lastRow++; lastRow++; lastRow++;
                        break;
                    case 134:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 138:
                        lastRow++; lastRow++; lastRow++;
                        break;
                    case 159:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 163:
                        lastRow++; lastRow++; lastRow++;
                        break;
                    case 178:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 182:
                        lastRow++; lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 187:
                        lastRow++; lastRow++; lastRow++;
                        break;
                    default:
                        lastRow++; lastRow++;
                        break;
                };
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
                .Border.SetTopBorder(XLBorderStyleValues.Hair)
                .Border.SetBottomBorder(XLBorderStyleValues.Hair)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);

            ws.Range(4, 1, 4, 15).Style
                .Border.SetTopBorder(XLBorderStyleValues.Thick)
                .Border.SetBottomBorder(XLBorderStyleValues.Thick)
                .Border.SetLeftBorder(XLBorderStyleValues.Thick)
                .Border.SetRightBorder(XLBorderStyleValues.Thick);

            ws.Range(6, 2, 212, 2).Style.Border.SetTopBorder(XLBorderStyleValues.None);
            ws.Range(6, 2, 212, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.None);
            // merge cell i as Row1
            int row2 = 7;
            for (int i = 6; i <= 66; i++)
            {
                for (var col = 1; col < 16; col++)
                {
                    if (col != 2)
                    {
                        ws.Range(i, col, row2, col).Merge();
                        ws.Cell(row2, 2).Style.Font.Italic = true;
                        ws.Cell(row2, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.Hair);
                    }
                }
                row2++;
                row2++;
                i++;
            }

            row2 = 72;
            for (int i = 71; i <= 83; i++)
            {
                for (var col = 1; col < 16; col++)
                {
                    if (col != 2)
                    {
                        ws.Range(i, col, row2, col).Merge();
                        ws.Cell(row2, 2).Style.Font.Italic = true;
                        ws.Cell(row2, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.Hair);
                    }
                }
                row2++;
                row2++;
                i++;
            }

            row2 = 89;
            for (int i = 88; i <= 118; i++)
            {
                for (var col = 1; col < 16; col++)
                {
                    if (col != 2)
                    {
                        ws.Range(i, col, row2, col).Merge();
                        ws.Cell(row2, 2).Style.Font.Italic = true;
                        ws.Cell(row2, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.Hair);
                    }
                }
                row2++;
                row2++;
                i++;
            }

            row2 = 124;
            for (int i = 123; i <= 135; i++)
            {
                for (var col = 1; col < 16; col++)
                {
                    if (col != 2)
                    {
                        ws.Range(i, col, row2, col).Merge();
                        ws.Cell(row2, 2).Style.Font.Italic = true;
                        ws.Cell(row2, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.Hair);
                    }
                }
                row2++;
                row2++;
                i++;
            }

            row2 = 141;
            for (int i = 140; i <= 160; i++)
            {
                for (var col = 1; col < 16; col++)
                {
                    if (col != 2)
                    {
                        ws.Range(i, col, row2, col).Merge();
                        ws.Cell(row2, 2).Style.Font.Italic = true;
                        ws.Cell(row2, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.Hair);
                    }
                }
                row2++;
                row2++;
                i++;
            }

            row2 = 166;
            for (int i = 165; i <= 179; i++)
            {
                for (var col = 1; col < 16; col++)
                {
                    if (col != 2)
                    {
                        ws.Range(i, col, row2, col).Merge();
                        ws.Cell(row2, 2).Style.Font.Italic = true;
                        ws.Cell(row2, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.Hair);
                    }
                }
                row2++;
                row2++;
                i++;
            }

            for (var col = 1; col < 16; col++)
            {
                if (col != 2)
                {
                    ws.Range(184, col, 185, col).Merge();
                    ws.Cell(185, 2).Style.Font.Italic = true;
                    ws.Cell(185, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.Hair);
                }
            }


            row2 = 190;
            for (int i = 189; i <= 211; i++)
            {
                for (var col = 1; col < 16; col++)
                {
                    if (col != 2)
                    {
                        ws.Range(i, col, row2, col).Merge();
                        ws.Cell(row2, 2).Style.Font.Italic = true;
                        ws.Cell(row2, 2).Style.Border.SetBottomBorder(XLBorderStyleValues.Hair);
                    }
                }
                row2++;
                row2++;
                i++;
            }

            row2 = 103;
            for (var row = 102; row <= 110; row++)
            {
                ws.Range(row, 2, row2, 2).Merge();
                row++;
                row2++;
                row2++;
            }

            //Border none and thick

            int[] Brdr = new int[] { 5, 68, 69, 70, 85, 86, 87, 120, 121, 122, 137, 138, 139, 162, 163, 164, 181, 182, 183, 186, 187, 188 };
            foreach (var c in Brdr)
            {
                ws.Range(c, 1, c, 15).Style.Border.SetLeftBorder(XLBorderStyleValues.None);
                ws.Range(c, 1, c, 15).Style.Border.SetRightBorder(XLBorderStyleValues.None);
                ws.Range(c, 1, c, 15).Style.Border.SetTopBorder(XLBorderStyleValues.Thick);
                ws.Range(c, 1, c, 15).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick);
            }

            ws.Range(212, 1, 212, 15).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick);

            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Range(6, 1, 120, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Columns(3, 15).Width = 14;
            //color
            ws.Columns().Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 255, 255));
            ws.Rows().Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 255, 255));

            int[] cll = new int[] { 6, 14, 24, 34, 42, 66, 73, 90, 100, 112, 118, 125, 131, 140, 152, 195 };
            foreach (var c in cll)
            {
                ws.Range(c, 2, c, 15).Style.Fill.SetBackgroundColor(XLColor.FromArgb(153, 204, 255));
                ws.Range(c, 2, c, 15).Style.Font.Bold = true;
                ws.Range(c + 1, 2, c + 1, 15).Style.Fill.SetBackgroundColor(XLColor.FromArgb(153, 204, 255));
                ws.Range(c + 1, 2, c + 1, 15).Style.Font.Bold = true;
            }

            //ws.Columns().Style.Alignment.SetWrapText();
            //ws.Columns().AdjustToContents();
            //ws.Rows().AdjustToContents();
            ws.Columns(2, 2).AdjustToContents();
            ws.Column(3).Delete();
            ws.Column(2).Style.Font.Bold = true;

            ws.Range(8, 2, 13, 2).Style.Alignment.Indent = 1;
            ws.Range(16, 2, 21, 2).Style.Alignment.Indent = 1;
            ws.Range(26, 2, 31, 2).Style.Alignment.Indent = 1;
            ws.Range(36, 2, 41, 2).Style.Alignment.Indent = 1;
            ws.Range(44, 2, 49, 2).Style.Alignment.Indent = 1;
            ws.Range(75, 2, 82, 2).Style.Alignment.Indent = 1;
            ws.Range(92, 2, 93, 2).Style.Alignment.Indent = 1;
            ws.Range(98, 2, 99, 2).Style.Alignment.Indent = 1;
            ws.Range(100, 2, 111, 2).Style.Alignment.Indent = 1;
            ws.Range(114, 2, 119, 2).Style.Alignment.Indent = 1;
            ws.Range(127, 2, 132, 2).Style.Alignment.Indent = 1;
            ws.Range(142, 2, 143, 2).Style.Alignment.Indent = 1;
            ws.Range(154, 2, 155, 2).Style.Alignment.Indent = 1;
            ws.Range(197, 2, 198, 2).Style.Alignment.Indent = 1;
            ws.Range(199, 2, 200, 2).Style.Alignment.Indent = 1;

            ws.Range(94, 2, 97, 2).Style.Alignment.Indent = 2;
            ws.Range(133, 2, 136, 2).Style.Alignment.Indent = 2;

            ws.Range(8, 2, 13, 2).Style.Font.Bold = false;
            ws.Range(16, 2, 21, 2).Style.Font.Bold = false;
            ws.Range(26, 2, 31, 2).Style.Font.Bold = false;
            ws.Range(36, 2, 41, 2).Style.Font.Bold = false;
            ws.Range(44, 2, 49, 2).Style.Font.Bold = false;
            ws.Range(75, 2, 82, 2).Style.Font.Bold = false;
            ws.Range(102, 2, 111, 2).Style.Font.Bold = false;

            ws.Cell("A2").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
            ws.Cell("A3").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        private ActionResult GenerateExcelR2V2(XLWorkbook wb, DataTable dt, int lastRow, string fileName, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var ws = wb.Worksheet(1);
            var tmpLastRow = lastRow;
            var rngTable = ws.Range(8, 1, 170, 19);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin)
                .Border.SetTopBorderColor(XLColor.FromArgb(55, 96, 145))
                .Border.SetBottomBorderColor(XLColor.FromArgb(55, 96, 145))
                .Border.SetLeftBorderColor(XLColor.FromArgb(55, 96, 145))
                .Border.SetRightBorderColor(XLColor.FromArgb(55, 96, 145));
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
                        ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.White).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        //ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(14);

                        switch (lastRow)
                        {
                            case 8:
                                if (iCol == 2)
                                {
                                    ws.Cell(lastRow + 1, 2).Value = "A. PENJUALAN";
                                    ws.Cell(lastRow + 1, 2).Style.Font.Bold = true;
                                    //ws.Cell(lastRow + 1, 2).Style.Font.SetFontSize(12);
                                    ws.Range('B' + (lastRow).ToString(), 'C' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                    ws.Range('B' + (lastRow + 1).ToString(), 'R' + (lastRow + 1).ToString()).Merge();
                                }
                                if (iCol == 3)
                                {
                                    ws.Cell(lastRow + 2, iCol - 1).Style.Font.SetFontSize(10);
                                    ws.Cell(lastRow + 2, iCol - 1).Value = val;
                                    ws.Range('B' + (lastRow + 2).ToString(), 'C' + (lastRow + 2).ToString()).Merge();
                                }
                                else
                                {
                                    ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10); // no and count of month
                                    ws.Cell(lastRow + 2, iCol).Value = val;
                                    if (iCol == 6 || iCol == 7)
                                    {
                                        ws.Cell(lastRow + 2, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                        ws.Cell(lastRow + 2, iCol).Style.Font.SetFontColor(XLColor.White)
                                            .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                    }
                                }


                                break;
                            default:
                                ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10);
                                ws.Cell(lastRow + 2, iCol).Value = val;
                                if (iCol == 6 || iCol == 7)
                                {
                                    ws.Cell(lastRow + 2, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                    ws.Cell(lastRow + 2, iCol).Style.Font.SetFontColor(XLColor.White)
                                        .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                }
                                break;
                        };
                    }
                    else
                    {
                        if (tmpLastRow != lastRow)
                        {
                            switch (lastRow)
                            {
                                case 47:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "B. UNIT SERVIS";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                        if (iCol == 6 || iCol == 7)
                                        {
                                            ws.Cell(lastRow + 1, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontColor(XLColor.White)
                                                .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                        }
                                    }
                                    break;
                                case 59:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "C. JENIS PEKERJAAN";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                    }
                                    break;
                                case 111:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "D. KEKUATAN BENGKEL SERVIS";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                    }
                                    break;
                                case 126:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "E. INDIKATOR-INDIKATOR PRODUKTIVITAS";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                    }
                                    break;
                                case 142:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "F. RETENSI SERVIS & AKTIVITAS MARKETING";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                        if (iCol == 6 || iCol == 7)
                                        {
                                            ws.Cell(lastRow + 1, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontColor(XLColor.White)
                                                .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                        }
                                    }
                                    break;
                                case 153:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "G. PENJUALAN UNIT SEPEDA MOTOR";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                        if (iCol == 6 || iCol == 7)
                                        {
                                            ws.Cell(lastRow + 1, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontColor(XLColor.White)
                                                .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                        }
                                    }
                                    break;
                                case 157:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "H. PERFORMA CS INDEKS";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                        if (iCol == 6 || iCol == 7)
                                        {
                                            ws.Cell(lastRow + 1, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontColor(XLColor.White)
                                                .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                        }
                                    }
                                    break;
                                default:
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge();
                                    }
                                    else if (iCol != 2)
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                        if (iCol == 6 || iCol == 7)
                                        {
                                            ws.Cell(lastRow + 1, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontColor(XLColor.White)
                                                .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                        }
                                    }
                                    break;
                            };

                        }

                    }
                    iCol++;
                }
                switch (lastRow)
                {
                    case 8:
                        lastRow++; lastRow++;
                        break;
                    case 43:
                        lastRow++; lastRow++; lastRow++;lastRow++;
                        break;
                    case 55:
                        lastRow++; lastRow++; lastRow++;lastRow++;
                        break;
                    case 107:
                        lastRow++; lastRow++; lastRow++;lastRow++;
                        break;
                    case 122:
                        lastRow++; lastRow++; lastRow++;lastRow++;
                        break;
                    case 138:
                        lastRow++; lastRow++; lastRow++;lastRow++;
                        break;
                    case 149:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 153:
                        lastRow++; lastRow++; lastRow++;lastRow++;
                        break;
                    case 168:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    default:
                        lastRow++;
                        break;
                };
            }

            ws.Range(11, 2, 13, 2).Style.Alignment.Indent = 1;
            ws.Range(15, 2, 17, 2).Style.Alignment.Indent = 1;
            ws.Range(19, 2, 21, 2).Style.Alignment.Indent = 1;
            ws.Range(23, 2, 24, 2).Style.Alignment.Indent = 1;
            ws.Range(26, 2, 27, 2).Style.Alignment.Indent = 1;
            ws.Range(29, 2, 31, 2).Style.Alignment.Indent = 1;
            ws.Range(50, 2, 52, 2).Style.Alignment.Indent = 1;
            ws.Range(62, 2, 64, 2).Style.Alignment.Indent = 1;
            ws.Range(65, 2, 70, 2).Style.Alignment.Indent = 1;
            ws.Range(72, 2, 78, 2).Style.Alignment.Indent = 1;
            ws.Range(80, 2, 89, 2).Style.Alignment.Indent = 1;
            ws.Range(92, 2, 98, 2).Style.Alignment.Indent = 1;
            ws.Range(100, 2, 104, 2).Style.Alignment.Indent = 1;
            ws.Range(106, 2, 108, 2).Style.Alignment.Indent = 1;
            ws.Range(113, 2, 114, 2).Style.Alignment.Indent = 1;
            ws.Range(116, 2, 119, 2).Style.Alignment.Indent = 1;
            ws.Range(121, 2, 123, 2).Style.Alignment.Indent = 1;
            ws.Range(128, 2, 128, 2).Style.Alignment.Indent = 1;
            ws.Range(135, 2, 135, 2).Style.Alignment.Indent = 1;
            ws.Range(162, 2, 163, 2).Style.Alignment.Indent = 1;
            ws.Range(167, 2, 169, 2).Style.Alignment.Indent = 1;

           

            //Border none and thick

            int[] Brdr = new int[] { 9,47,59,111,126,142,153,157 };
            foreach (var c in Brdr)
            {
                ws.Cell(c,1).Style.Border.SetRightBorder(XLBorderStyleValues.None);
            }

            //ws.Range(170, 1, 170, 17).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick);

            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Columns(4, 19).Width = 15;
            //color
            //ws.Columns().Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 255, 255));
            //ws.Rows().Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 255, 255));
            ws.Range(8, 1, 8, 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
            ws.Range(8, 1, 8, 1).Style.Font.SetFontColor(XLColor.White)
                .Border.SetTopBorderColor(XLColor.White)
                .Border.SetBottomBorderColor(XLColor.White)
                .Border.SetLeftBorderColor(XLColor.White)
                .Border.SetRightBorderColor(XLColor.White);
            ws.Range(8, 1, 8, 1).Style.Font.Bold = true;

            int[] clls = new int[] { 45,57,109,124,140,151,155,170 };
            foreach (var d in clls)
            { 
                ws.Range('A' + (d).ToString(), 'R' + (d).ToString()).Merge();

                ws.Range(d+1, 1, d+1, 19).Style.Border.SetLeftBorder(XLBorderStyleValues.None);
                ws.Range(d+1, 1, d+1, 19).Style.Border.SetRightBorder(XLBorderStyleValues.None);
                ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetTopBorder(XLBorderStyleValues.Thin).Border.SetTopBorderColor(XLColor.FromArgb(55, 96, 145));
                ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetBottomBorder(XLBorderStyleValues.Thin).Border.SetTopBorderColor(XLColor.FromArgb(55, 96, 145));

                if (d == 170)
                {
                    ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetLeftBorder(XLBorderStyleValues.None);
                    ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetRightBorder(XLBorderStyleValues.None);
                    ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetTopBorder(XLBorderStyleValues.Thin).Border.SetTopBorderColor(XLColor.Blue);
                    ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetBottomBorder(XLBorderStyleValues.None).Border.SetTopBorderColor(XLColor.Blue);
                }
            }
            int[] cll = new int[] { 8,10,14,18,22,25,28,32,33,34,35,38,40,41,42,43,44,49,60,61,62,63,64,71,79,90,91,99,105,112,115,120,127,128,129,130,131,132,134,135,136,137,138,139,148,149,150,160,161,167,168,169 };
            foreach (var c in cll)
            {
                ws.Range(c, 2, c, 19).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                ws.Range(c, 2, c, 19).Style.Font.Bold = true;
                ws.Range(c, 2, c, 19).Style.Font.SetFontColor(XLColor.White)
                .Border.SetTopBorderColor(XLColor.White)
                .Border.SetBottomBorderColor(XLColor.White)
                .Border.SetLeftBorderColor(XLColor.White)
                .Border.SetRightBorderColor(XLColor.White);
            }

            int[] cell = new int[] { 143, 144, 145, 146, 147 };
            foreach (var c in cell)
            {
                ws.Range(c, 5, c, 5).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                ws.Range(c, 5, c, 5).Style.Font.Bold = true;
                ws.Range(c, 5, c, 5).Style.Font.SetFontColor(XLColor.White)
                .Border.SetTopBorderColor(XLColor.White)
                .Border.SetBottomBorderColor(XLColor.White)
                .Border.SetLeftBorderColor(XLColor.White)
                .Border.SetRightBorderColor(XLColor.White);
            }

            int[] call = new int[] { 11, 49, 112, 133 };
            foreach (var c in call)
            {
                ws.Range(c, 8, c, 19).Style.Fill.SetBackgroundColor(XLColor.Red);
                ws.Range(c, 8, c, 19).Style.Font.Bold = true;
                ws.Range(c, 8, c, 19).Style.Font.SetFontColor(XLColor.White)
                .Border.SetTopBorderColor(XLColor.White)
                .Border.SetBottomBorderColor(XLColor.White)
                .Border.SetLeftBorderColor(XLColor.White)
                .Border.SetRightBorderColor(XLColor.White);
            }

            int[] coll = new int[] { 127, 128, 129, 130, 131, 132, 134, 135, 136, 137, 138, 139 };
            foreach (var c in coll)
            {
                ws.Cell(c, 7).Style.Fill.PatternType = XLFillPatternValues.DarkGray;
                ws.Cell(c, 7).Style.Fill.PatternColor = XLColor.Black;
                ws.Cell(c, 7).Style.Fill.PatternBackgroundColor = XLColor.FromArgb(55, 96, 145);
                ws.Cell(c, 7).Style.Font.Bold = true;
                ws.Cell(c, 7).Style.Font.SetFontColor(XLColor.White);
            }

            int[] xxx = new int[] { 158, 148, 149, 150, 160, 167, 168, 169 };
            foreach (var v in xxx)
            {
                ws.Cell(v, 7).Style.Fill.SetPatternType(XLFillPatternValues.DarkGray);
                ws.Cell(v, 7).Style.Fill.SetPatternColor(XLColor.Black);
                ws.Cell(v, 7).Style.Fill.SetPatternBackgroundColor(XLColor.FromArgb(219, 229, 241));
                ws.Cell(v, 7).Style.Font.Bold = true;
                ws.Cell(v, 7).Style.Font.SetFontColor(XLColor.White);
            }

            ws.Columns(1, 19).AdjustToContents();
            ws.Column(4).Delete();
            ws.Columns().Style.Font.SetFontName("Arial");
            ws.Columns().Style.Font.SetFontSize(10);
            ws.Cell("A1").Style.Font.SetFontSize(36);
            ws.Column(2).Width = 2.5;
            ws.Column(2).Width = 25;
            ws.Column(3).Width = 60;
            ws.Cell(109, 1).Value = "             *) Tanpa Kupon Servis gratis KSG";

            ws.Range(10, 5, 10, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            ws.Range(10, 5, 10, 19).Style.NumberFormat.Format = "#,##0.0";

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }
        
        private ActionResult GenerateExcelnoborder(XLWorkbook wb, DataTable dt, int lastRow, string fileName, bool isCustomHeader = false, bool isShowSummary = false)
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
                            ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,###";
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,###.0";
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
                        //ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(10);

                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }
                    else
                    {
                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }

                    iCol++;
                }

                lastRow++;
            }

            if (isShowSummary)
            {
                ws.Cell(lastRow + 1, 1).Value = "TOTAL";
                for (char i = 'A'; i <= iCol; i++)
                {
                    ws.Cell(lastRow + 1, 1).FormulaA1 = "=SUM(" + i + (tmpLastRow + 1) + ":" + i + lastRow + ")";
                }
            }

            var rngTable = ws.Range(tmpLastRow, 1, lastRow, iCol - 1);
            //ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            //ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        private void CreateHeaderSummaryByDealer(ref ExcelFileWriter excelReport, ref int rowIndex, string pPeriode, string pArea, string pDealer, string pOutlet)
        {
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);

            excelReport.SetCellValue("ITS Status Reports (Summary)", 0, 0, 1, 13, ExcelCellStyle.Header, false, "20");
            excelReport.FreezeCols(0, 6);

            excelReport.SetCellValue("Periode ", 2, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pPeriode, 2, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Area", 3, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pArea, 3, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 4, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pDealer, 4, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Outlet", 5, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pOutlet, 5, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Last Month Result", 6, 6, 1, 7, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("This Month Result", 6, 13, 1, 7, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 6, 20, 1, 1, ExcelCellStyle.LeftBorderedBold);

            excelReport.SetCellValue("Area", 7, 0, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Dealer", 7, 1, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outlet", 7, 2, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Tipe", 7, 3, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Variant", 7, 4, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Status", 7, 5, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7, 6, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding + New", 7, 7, 1, 5, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7, 12, 2, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7, 13, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding + New", 7, 14, 1, 5, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7, 19, 2, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("VS Last Month", 7, 20, 2, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("1st-7th", 8, 7, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 8, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 9, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 10, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 11, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("1st-7th", 8, 14, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 15, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 16, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 17, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 18, 1, 1, ExcelCellStyle.CenterBorderedBold);

            rowIndex = 0;
        }

        private void CreateHeaderSummary(ref ExcelFileWriter excelReport, ref int rowIndex, string pPeriode, string pArea, string pDealer, string pOutlet)
        {
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);

            excelReport.SetCellValue("ITS Status Reports (Summary)", 0, 0, 1, 13, ExcelCellStyle.Header, false, "20");
            excelReport.FreezeCols(0, 4);

            excelReport.SetCellValue("Periode ", 2, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pPeriode, 2, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Area", 3, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pArea, 3, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 4, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pDealer, 4, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Outlet", 5, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pOutlet, 5, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Last Month Result", 6, 4, 1, 7, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("This Month Result", 6, 11, 1, 7, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 6, 18, 1, 1, ExcelCellStyle.LeftBorderedBold);

            excelReport.SetCellValue("Area", 7, 0, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Dealer", 7, 1, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outlet", 7, 2, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Status", 7, 3, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7, 4, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding + New", 7, 5, 1, 5, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7, 10, 2, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7, 11, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding + New", 7, 12, 1, 5, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7, 17, 2, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("VS Last Month", 7, 18, 2, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("1st-7th", 8, 5, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 6, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 7, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 8, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 9, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("1st-7th", 8, 12, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 13, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 14, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 15, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 16, 1, 1, ExcelCellStyle.CenterBorderedBold);

            rowIndex = 0;
        }

        private void CreateHeaderSummaryDetail(ref ExcelFileWriter excelReport, ref int rowIndex, string pPeriode, string pArea, string pDealer, string pOutlet)
        {
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);

            excelReport.SetCellValue("ITS Status Reports (Summary)", 0, 0, 1, 13, ExcelCellStyle.Header, false, "20");
            excelReport.FreezeCols(0, 4);

            excelReport.SetCellValue("Periode ", 2, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pPeriode, 2, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Area", 3, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pArea, 3, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 4, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pDealer, 4, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Outlet", 5, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pOutlet, 5, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Last Month Result", 6, 4, 1, 14, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("This Month Result", 6, 18, 1, 14, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 6, 32, 1, 1, ExcelCellStyle.LeftBorderedBold);

            excelReport.SetCellValue("Area", 7, 0, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Dealer", 7, 1, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outlet", 7, 2, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Status", 7, 3, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7, 4, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding", 7, 5, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("New", 7, 11, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7, 17, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7, 18, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding", 7, 19, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("New", 7, 25, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7, 31, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("VS Last Month", 7, 32, 2, 1, ExcelCellStyle.LeftBorderedBoldWrap);

            excelReport.SetCellValue("1st-7th", 8, 5, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 6, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 7, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 8, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 9, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 10, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("1st-7th", 8, 11, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 12, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 13, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 14, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 15, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 16, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("1st-7th", 8, 19, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 20, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 21, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 22, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 23, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 24, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("1st-7th", 8, 25, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 26, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 27, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 28, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 29, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 30, 1, 1, ExcelCellStyle.CenterBorderedBold);

            rowIndex = 0;
        }

        private void CreateHeaderDetail(ref ExcelFileWriter excelReport, ref int rowIndex, string pPeriode, string pArea, string pDealer, string pOutlet)
        {
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);

            excelReport.SetCellValue("ITS Status Reports (Detail)", 0, 0, 1, 13, ExcelCellStyle.Header, false, "20");
            excelReport.FreezeCols(0, 6);

            excelReport.SetCellValue("Periode ", 2, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pPeriode, 2, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Area", 3, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pArea, 3, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 4, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pDealer, 4, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Outlet", 5, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pOutlet, 5, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Last Month Result", 6, 6, 1, 14, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("This Month Result", 6, 20, 1, 14, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 6, 34, 1, 1, ExcelCellStyle.LeftBorderedBold);

            excelReport.SetCellValue("Area", 7, 0, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Dealer", 7, 1, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outlet", 7, 2, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Tipe", 7, 3, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Varian", 7, 4, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Status", 7, 5, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7, 6, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding", 7, 7, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("New", 7, 13, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7, 19, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7, 20, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding", 7, 21, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("New", 7, 27, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7, 33, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("VS Last Month", 7, 34, 2, 1, ExcelCellStyle.LeftBorderedBoldWrap);

            excelReport.SetCellValue("1st-7th", 8, 7, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 8, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 9, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 10, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 11, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 12, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("1st-7th", 8, 13, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 14, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 15, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 16, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 17, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 18, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("1st-7th", 8, 21, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 22, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 23, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 24, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 25, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 26, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("1st-7th", 8, 27, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 28, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 29, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 30, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 31, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 32, 1, 1, ExcelCellStyle.CenterBorderedBold);

            rowIndex = 0;
        }

        private void CreateHeaderbyType(ExcelFileWriter excelReport, ref int rowIndex, string pPeriode, string pArea, string pDealer, string pOutlet, string option)
        {
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);

            excelReport.SetCellValue("ITS Status Reports " + option, 0, 0, 1, 13, ExcelCellStyle.Header, false, "20");
            excelReport.FreezeCols(0, 4);

            excelReport.SetCellValue("Periode ", 2, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pPeriode, 2, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Area", 3, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pArea, 3, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 4, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pDealer, 4, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Outlet", 5, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pOutlet, 5, 1, 1, 12, ExcelCellStyle.LeftBold);

            rowIndex = 0;
        }

        #endregion

        #region ExcelFileWriter

        public ExcelFileWriter CreateReportExcelInqWithSts(string fileName, DataSet dt0, string pPeriode, string pArea, string pDealer, string pOutlet, string report, string reportType)
        {
            string companyName = "", modelType = "";
            int rowIndex = 0;
            ExcelFileWriter excelReport = null;

            if (report == "0")
            {
                #region By Dealer
                excelReport = new ExcelFileWriter(fileName, "Summary", "Inquiry ITS With Status");

                #region Summary
                if (reportType == "0")
                {
                    CreateHeaderSummaryByDealer(ref excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet);

                    int count = 0; int countRow = 0;

                    foreach (DataRow row in dt0.Tables[0].Rows)
                    {
                        if (countRow == 12) break;
                        countRow += 1;

                        if (count == 0)
                        {
                            excelReport.SetCellValue(row["Area"].ToString(), 9 + rowIndex, 0, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["CompanyName"].ToString(), 9 + rowIndex, 1, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["BranchName"].ToString(), 9 + rowIndex, 2, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["TipeKendaraan"].ToString(), 9 + rowIndex, 3, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["Variant"].ToString(), 9 + rowIndex, 4, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            count = 6;
                        }

                        int Week1Last = Convert.ToInt32(row["WeekOuts1Last"].ToString()) + Convert.ToInt32(row["Week1Last"].ToString());
                        int Week2Last = Convert.ToInt32(row["WeekOuts2Last"].ToString()) + Convert.ToInt32(row["Week2Last"].ToString());
                        int Week3Last = Convert.ToInt32(row["WeekOuts3Last"].ToString()) + Convert.ToInt32(row["Week3Last"].ToString());
                        int Week4Last = Convert.ToInt32(row["WeekOuts4Last"].ToString()) + Convert.ToInt32(row["Week4Last"].ToString());
                        int Week5Last = Convert.ToInt32(row["WeekOuts5Last"].ToString()) + Convert.ToInt32(row["Week5Last"].ToString());

                        int Week1 = Convert.ToInt32(row["WeekOuts1"].ToString()) + Convert.ToInt32(row["Week1"].ToString());
                        int Week2 = Convert.ToInt32(row["WeekOuts2"].ToString()) + Convert.ToInt32(row["Week2"].ToString());
                        int Week3 = Convert.ToInt32(row["WeekOuts3"].ToString()) + Convert.ToInt32(row["Week3"].ToString());
                        int Week4 = Convert.ToInt32(row["WeekOuts4"].ToString()) + Convert.ToInt32(row["Week4"].ToString());
                        int Week5 = Convert.ToInt32(row["WeekOuts5"].ToString()) + Convert.ToInt32(row["Week5"].ToString());

                        excelReport.SetCellValue(row["LastProgress"].ToString(), 9 + rowIndex, 5, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.SetCellValue(row["SaldoAwalLast"].ToString(), 9 + rowIndex, 6, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week1Last.ToString(), 9 + rowIndex, 7, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week2Last.ToString(), 9 + rowIndex, 8, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week3Last.ToString(), 9 + rowIndex, 9, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week4Last.ToString(), 9 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week5Last.ToString(), 9 + rowIndex, 11, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalLast"].ToString(), 9 + rowIndex, 12, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);

                        excelReport.SetCellValue(row["SaldoAwal"].ToString(), 9 + rowIndex, 13, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week1.ToString(), 9 + rowIndex, 14, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week2.ToString(), 9 + rowIndex, 15, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week3.ToString(), 9 + rowIndex, 16, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week4.ToString(), 9 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week5.ToString(), 9 + rowIndex, 18, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 19, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotPercent"].ToString(), 9 + rowIndex, 20, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalDecimal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalDecimal : ExcelCellStyle.RightBorderedStandardDecimal, true);

                        count--;
                        rowIndex++;
                    }
                }
                #endregion

                #region Detail
                if (reportType == "1")
                {
                    CreateHeaderSummaryDetail(ref excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet);

                    foreach (DataRow row in dt0.Tables[0].Rows)
                    {
                        if (rowIndex % 6 == 0)
                        {
                            excelReport.SetCellValue(row["Area"].ToString(), 9 + rowIndex, 0, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["CompanyName"].ToString(), 9 + rowIndex, 1, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["BranchName"].ToString(), 9 + rowIndex, 2, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotal : ExcelCellStyle.LeftBorderedStandardWrap);

                        }
                        excelReport.SetCellValue(row["LastProgress"].ToString(), 9 + rowIndex, 3, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.SetCellValue(row["SaldoAwalLast"].ToString(), 9 + rowIndex, 4, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts1Last"].ToString(), 9 + rowIndex, 5, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts2Last"].ToString(), 9 + rowIndex, 6, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts3Last"].ToString(), 9 + rowIndex, 7, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts4Last"].ToString(), 9 + rowIndex, 8, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts5Last"].ToString(), 9 + rowIndex, 9, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekOutsLast"].ToString(), 9 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week1Last"].ToString(), 9 + rowIndex, 11, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week2Last"].ToString(), 9 + rowIndex, 12, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week3Last"].ToString(), 9 + rowIndex, 13, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week4Last"].ToString(), 9 + rowIndex, 14, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week5Last"].ToString(), 9 + rowIndex, 15, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekLast"].ToString(), 9 + rowIndex, 16, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalLast"].ToString(), 9 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);

                        excelReport.SetCellValue(row["SaldoAwal"].ToString(), 9 + rowIndex, 18, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts1"].ToString(), 9 + rowIndex, 19, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts2"].ToString(), 9 + rowIndex, 20, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts3"].ToString(), 9 + rowIndex, 21, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts4"].ToString(), 9 + rowIndex, 22, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts5"].ToString(), 9 + rowIndex, 23, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekOuts"].ToString(), 9 + rowIndex, 24, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week1"].ToString(), 9 + rowIndex, 25, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week2"].ToString(), 9 + rowIndex, 26, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week3"].ToString(), 9 + rowIndex, 27, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week4"].ToString(), 9 + rowIndex, 28, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week5"].ToString(), 9 + rowIndex, 29, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeek"].ToString(), 9 + rowIndex, 30, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 31, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotPercent"].ToString(), 9 + rowIndex, 32, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalDecimal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalDecimal : ExcelCellStyle.RightBorderedStandardDecimal, true);

                        rowIndex++;
                    }

                    companyName = dt0.Tables[1].Rows[0]["CompanyName"].ToString();
                    excelReport.ChangeSheet(companyName);
                    CreateHeaderDetail(ref excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet);

                    foreach (DataRow row in dt0.Tables[1].Rows)
                    {
                        if (companyName != row["CompanyName"].ToString())
                        {
                            companyName = row["CompanyName"].ToString();

                            excelReport.ChangeSheet(companyName);
                            CreateHeaderDetail(ref excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet);
                        }

                        if (rowIndex % 6 == 0)
                        {
                            excelReport.SetCellValue(row["Area"].ToString(), 9 + rowIndex, 0, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["CompanyName"].ToString(), 9 + rowIndex, 1, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["BranchName"].ToString(), 9 + rowIndex, 2, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["TipeKendaraan"].ToString(), 9 + rowIndex, 3, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["Variant"].ToString(), 9 + rowIndex, 4, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                        }

                        excelReport.SetCellValue(row["LastProgress"].ToString(), 9 + rowIndex, 5, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.SetCellValue(row["SaldoAwalLast"].ToString(), 9 + rowIndex, 6, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts1Last"].ToString(), 9 + rowIndex, 7, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts2Last"].ToString(), 9 + rowIndex, 8, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts3Last"].ToString(), 9 + rowIndex, 9, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts4Last"].ToString(), 9 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts5Last"].ToString(), 9 + rowIndex, 11, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekOutsLast"].ToString(), 9 + rowIndex, 12, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week1Last"].ToString(), 9 + rowIndex, 13, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week2Last"].ToString(), 9 + rowIndex, 14, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week3Last"].ToString(), 9 + rowIndex, 15, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week4Last"].ToString(), 9 + rowIndex, 16, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week5Last"].ToString(), 9 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekLast"].ToString(), 9 + rowIndex, 18, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalLast"].ToString(), 9 + rowIndex, 19, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["SaldoAwal"].ToString(), 9 + rowIndex, 20, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);

                        excelReport.SetCellValue(row["WeekOuts1"].ToString(), 9 + rowIndex, 21, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts2"].ToString(), 9 + rowIndex, 22, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts3"].ToString(), 9 + rowIndex, 23, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts4"].ToString(), 9 + rowIndex, 24, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts5"].ToString(), 9 + rowIndex, 25, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekOuts"].ToString(), 9 + rowIndex, 26, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week1"].ToString(), 9 + rowIndex, 27, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week2"].ToString(), 9 + rowIndex, 28, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week3"].ToString(), 9 + rowIndex, 29, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week4"].ToString(), 9 + rowIndex, 30, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week5"].ToString(), 9 + rowIndex, 31, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeek"].ToString(), 9 + rowIndex, 32, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 33, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotPercent"].ToString(), 9 + rowIndex, 34, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalDecimal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalDecimal : ExcelCellStyle.RightBorderedStandardDecimal, true);

                        rowIndex++;
                    }
                }
                #endregion

                #endregion
            }
            else
            {
                #region By Type
                int count = 0;

                if (reportType == "0")
                    excelReport = new ExcelFileWriter(fileName, dt0.Tables[0].Rows[0]["TipeKendaraan"].ToString(), "Inquiry ITS With Status");
                else
                    excelReport = new ExcelFileWriter(fileName, "Summary", "Inquiry ITS With Status");

                #region Summary

                if (reportType == "0")
                {
                    CreateHeaderSummary(ref excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet);
                    bool notFirst = false;
                    modelType = "";

                    foreach (DataRow row in dt0.Tables[0].Rows)
                    {
                        if (modelType != (row["TipeKendaraan"].ToString() == "" ? "Unknown" : row["TipeKendaraan"].ToString()))
                        {
                            if (notFirst)
                            {
                                excelReport.ChangeSheet(row["TipeKendaraan"].ToString());
                                CreateHeaderSummary(ref excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet);
                            }

                            modelType = (row["TipeKendaraan"].ToString() == "" ? "Unknown" : row["TipeKendaraan"].ToString());
                        }

                        notFirst = true;
                        if (count == 0)
                        {
                            excelReport.SetCellValue(row["Area"].ToString(), 9 + rowIndex, 0, 6, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["CompanyName"].ToString() != "ZTOTAL" ? row["CompanyName"].ToString() : "", 9 + rowIndex, 1, 6, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["BranchName"].ToString(), 9 + rowIndex, 2, 6, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            count = 6;
                        }

                        int Week1Last = Convert.ToInt32(row["WeekOuts1Last"].ToString()) + Convert.ToInt32(row["Week1Last"].ToString());
                        int Week2Last = Convert.ToInt32(row["WeekOuts2Last"].ToString()) + Convert.ToInt32(row["Week2Last"].ToString());
                        int Week3Last = Convert.ToInt32(row["WeekOuts3Last"].ToString()) + Convert.ToInt32(row["Week3Last"].ToString());
                        int Week4Last = Convert.ToInt32(row["WeekOuts4Last"].ToString()) + Convert.ToInt32(row["Week4Last"].ToString());
                        int Week5Last = Convert.ToInt32(row["WeekOuts5Last"].ToString()) + Convert.ToInt32(row["Week5Last"].ToString());

                        int Week1 = Convert.ToInt32(row["WeekOuts1"].ToString()) + Convert.ToInt32(row["Week1"].ToString());
                        int Week2 = Convert.ToInt32(row["WeekOuts2"].ToString()) + Convert.ToInt32(row["Week2"].ToString());
                        int Week3 = Convert.ToInt32(row["WeekOuts3"].ToString()) + Convert.ToInt32(row["Week3"].ToString());
                        int Week4 = Convert.ToInt32(row["WeekOuts4"].ToString()) + Convert.ToInt32(row["Week4"].ToString());
                        int Week5 = Convert.ToInt32(row["WeekOuts5"].ToString()) + Convert.ToInt32(row["Week5"].ToString());

                        excelReport.SetCellValue(row["LastProgress"].ToString(), 9 + rowIndex, 3, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.SetCellValue(row["SaldoAwalLast"].ToString(), 9 + rowIndex, 4, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week1Last.ToString(), 9 + rowIndex, 5, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week2Last.ToString(), 9 + rowIndex, 6, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week3Last.ToString(), 9 + rowIndex, 7, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week4Last.ToString(), 9 + rowIndex, 8, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week5Last.ToString(), 9 + rowIndex, 9, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalLast"].ToString(), 9 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);

                        excelReport.SetCellValue(row["SaldoAwal"].ToString(), 9 + rowIndex, 11, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week1.ToString(), 9 + rowIndex, 12, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week2.ToString(), 9 + rowIndex, 13, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week3.ToString(), 9 + rowIndex, 14, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week4.ToString(), 9 + rowIndex, 15, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Week5.ToString(), 9 + rowIndex, 16, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotPercent"].ToString(), 9 + rowIndex, 18, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalDecimal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalDecimal : ExcelCellStyle.RightBorderedStandardDecimal, true);

                        count--;
                        rowIndex++;
                    }
                }
                #endregion

                #region Detail
                else
                {
                    CreateHeaderSummaryDetail(ref excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet);

                    foreach (DataRow row in dt0.Tables[0].Rows)
                    {
                        if (rowIndex % 6 == 0)
                        {
                            excelReport.SetCellValue(row["Area"].ToString(), 9 + rowIndex, 0, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["CompanyName"].ToString() != "ZTOTAL" ? row["CompanyName"].ToString() : "", 9 + rowIndex, 1, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["BranchName"].ToString(), 9 + rowIndex, 2, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                        }
                        excelReport.SetCellValue(row["LastProgress"].ToString(), 9 + rowIndex, 3, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.SetCellValue(row["SaldoAwalLast"].ToString(), 9 + rowIndex, 4, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts1Last"].ToString(), 9 + rowIndex, 5, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts2Last"].ToString(), 9 + rowIndex, 6, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts3Last"].ToString(), 9 + rowIndex, 7, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts4Last"].ToString(), 9 + rowIndex, 8, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts5Last"].ToString(), 9 + rowIndex, 9, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekOutsLast"].ToString(), 9 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week1Last"].ToString(), 9 + rowIndex, 11, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week2Last"].ToString(), 9 + rowIndex, 12, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week3Last"].ToString(), 9 + rowIndex, 13, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week4Last"].ToString(), 9 + rowIndex, 14, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week5Last"].ToString(), 9 + rowIndex, 15, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekLast"].ToString(), 9 + rowIndex, 16, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalLast"].ToString(), 9 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);

                        excelReport.SetCellValue(row["SaldoAwal"].ToString(), 9 + rowIndex, 18, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts1"].ToString(), 9 + rowIndex, 19, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts2"].ToString(), 9 + rowIndex, 20, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts3"].ToString(), 9 + rowIndex, 21, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts4"].ToString(), 9 + rowIndex, 22, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts5"].ToString(), 9 + rowIndex, 23, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekOuts"].ToString(), 9 + rowIndex, 24, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week1"].ToString(), 9 + rowIndex, 25, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week2"].ToString(), 9 + rowIndex, 26, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week3"].ToString(), 9 + rowIndex, 27, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week4"].ToString(), 9 + rowIndex, 28, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week5"].ToString(), 9 + rowIndex, 29, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeek"].ToString(), 9 + rowIndex, 30, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 31, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotPercent"].ToString(), 9 + rowIndex, 32, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalDecimal : ExcelCellStyle.RightBorderedStandardDecimal, true);

                        rowIndex++;
                    }

                    modelType = (dt0.Tables[1].Rows[0]["TipeKendaraan"].ToString() == "" ? "Unknown" : dt0.Tables[1].Rows[0]["TipeKendaraan"].ToString());
                    excelReport.ChangeSheet(modelType);
                    CreateHeaderbyType(excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet, "(Detail)");
                    excelReport.SetCellValue("Tipe", 6, 0, 1, 1, ExcelCellStyle.LeftBold);
                    excelReport.SetCellValue(": " + modelType, 6, 1, 1, 12, ExcelCellStyle.LeftBold);
                    excelReport.SetCellValue("Last Month Result", 7, 4, 1, 14, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("This Month Result", 7, 18, 1, 14, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("%", 7, 32, 1, 1, ExcelCellStyle.LeftBorderedBold);
                    rowIndex++;

                    excelReport.SetCellValue("Area", 7 + rowIndex, 0, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Dealer", 7 + rowIndex, 1, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Outlet", 7 + rowIndex, 2, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Status", 7 + rowIndex, 3, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Saldo Awal", 7 + rowIndex, 4, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Outstanding", 7 + rowIndex, 5, 1, 6, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("New", 7 + rowIndex, 11, 1, 6, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Total", 7 + rowIndex, 17, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Saldo Awal", 7 + rowIndex, 18, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Outstanding", 7 + rowIndex, 19, 1, 6, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("New", 7 + rowIndex, 25, 1, 6, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Total", 7 + rowIndex, 31, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("VS Last Month", 7 + rowIndex, 32, 2, 1, ExcelCellStyle.LeftBorderedBoldWrap);

                    excelReport.SetCellValue("1st-7th", 8 + rowIndex, 5, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("8th-14th", 8 + rowIndex, 6, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("15th-21st", 8 + rowIndex, 7, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 8, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("29th-31st", 8 + rowIndex, 9, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Sub Total", 8 + rowIndex, 10, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("1st-7th", 8 + rowIndex, 11, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("8th-14th", 8 + rowIndex, 12, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("15th-21st", 8 + rowIndex, 13, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 14, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("29th-31st", 8 + rowIndex, 15, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Sub Total", 8 + rowIndex, 16, 1, 1, ExcelCellStyle.CenterBorderedBold);

                    excelReport.SetCellValue("1st-7th", 8 + rowIndex, 19, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("8th-14th", 8 + rowIndex, 20, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("15th-21st", 8 + rowIndex, 21, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 22, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("29th-31st", 8 + rowIndex, 23, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Sub Total", 8 + rowIndex, 24, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("1st-7th", 8 + rowIndex, 25, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("8th-14th", 8 + rowIndex, 26, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("15th-21st", 8 + rowIndex, 27, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 28, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("29th-31st", 8 + rowIndex, 29, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Sub Total", 8 + rowIndex, 30, 1, 1, ExcelCellStyle.CenterBorderedBold);

                    //int rows = 0;
                    foreach (DataRow row in dt0.Tables[1].Rows)
                    {
                        //rows += 1;
                        //if (rows > 4878) break;

                        if (modelType != (row["TipeKendaraan"].ToString() == "" ? "Unknown" : row["TipeKendaraan"].ToString()))
                        {
                            modelType = (row["TipeKendaraan"].ToString() == "" ? "Unknown" : row["TipeKendaraan"].ToString());

                            excelReport.ChangeSheet((modelType.Contains('/') ? modelType.Replace('/', '-') : modelType));
                            CreateHeaderbyType(excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet, "(Detail)");

                            excelReport.SetCellValue("Tipe", 6, 0, 1, 1, ExcelCellStyle.LeftBold);
                            excelReport.SetCellValue(": " + modelType, 6, 1, 1, 12, ExcelCellStyle.LeftBold);
                            excelReport.SetCellValue("Last Month Result", 7, 4, 1, 14, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("This Month Result", 7, 18, 1, 14, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("%", 7, 32, 1, 1, ExcelCellStyle.LeftBorderedBold);
                            rowIndex++;

                            excelReport.SetCellValue("Area", 7 + rowIndex, 0, 2, 1, ExcelCellStyle.LeftBorderedBold);
                            excelReport.SetCellValue("Dealer", 7 + rowIndex, 1, 2, 1, ExcelCellStyle.LeftBorderedBold);
                            excelReport.SetCellValue("Outlet", 7 + rowIndex, 2, 2, 1, ExcelCellStyle.LeftBorderedBold);
                            excelReport.SetCellValue("Status", 7 + rowIndex, 3, 2, 1, ExcelCellStyle.LeftBorderedBold);
                            excelReport.SetCellValue("Saldo Awal", 7 + rowIndex, 4, 2, 1, ExcelCellStyle.LeftBorderedBold);
                            excelReport.SetCellValue("Outstanding", 7 + rowIndex, 5, 1, 6, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("New", 7 + rowIndex, 11, 1, 6, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("Total", 7 + rowIndex, 17, 2, 1, ExcelCellStyle.LeftBorderedBold);
                            excelReport.SetCellValue("Saldo Awal", 7 + rowIndex, 18, 2, 1, ExcelCellStyle.LeftBorderedBold);
                            excelReport.SetCellValue("Outstanding", 7 + rowIndex, 19, 1, 6, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("New", 7 + rowIndex, 25, 1, 6, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("Total", 7 + rowIndex, 31, 2, 1, ExcelCellStyle.LeftBorderedBold);
                            excelReport.SetCellValue("VS Last Month", 7 + rowIndex, 32, 2, 1, ExcelCellStyle.LeftBorderedBoldWrap);

                            excelReport.SetCellValue("1st-7th", 8 + rowIndex, 5, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("8th-14th", 8 + rowIndex, 6, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("15th-21st", 8 + rowIndex, 7, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 8, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("29th-31st", 8 + rowIndex, 9, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("Sub Total", 8 + rowIndex, 10, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("1st-7th", 8 + rowIndex, 11, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("8th-14th", 8 + rowIndex, 12, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("15th-21st", 8 + rowIndex, 13, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 14, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("29th-31st", 8 + rowIndex, 15, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("Sub Total", 8 + rowIndex, 16, 1, 1, ExcelCellStyle.CenterBorderedBold);

                            excelReport.SetCellValue("1st-7th", 8 + rowIndex, 19, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("8th-14th", 8 + rowIndex, 20, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("15th-21st", 8 + rowIndex, 21, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 22, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("29th-31st", 8 + rowIndex, 23, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("Sub Total", 8 + rowIndex, 24, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("1st-7th", 8 + rowIndex, 25, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("8th-14th", 8 + rowIndex, 26, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("15th-21st", 8 + rowIndex, 27, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 28, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("29th-31st", 8 + rowIndex, 29, 1, 1, ExcelCellStyle.CenterBorderedBold);
                            excelReport.SetCellValue("Sub Total", 8 + rowIndex, 30, 1, 1, ExcelCellStyle.CenterBorderedBold);
                        }

                        if (count == 0)
                        {
                            excelReport.SetCellValue(row["Area"].ToString(), 9 + rowIndex, 0, 6, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["CompanyName"].ToString() != "ZTOTAL" ? row["CompanyName"].ToString() : "", 9 + rowIndex, 1, 6, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue(row["BranchName"].ToString(), 9 + rowIndex, 2, 6, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                            count = 6;
                        }

                        excelReport.SetCellValue(row["LastProgress"].ToString(), 9 + rowIndex, 3, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.SetCellValue(row["SaldoAwalLast"].ToString(), 9 + rowIndex, 4, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts1Last"].ToString(), 9 + rowIndex, 5, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts2Last"].ToString(), 9 + rowIndex, 6, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts3Last"].ToString(), 9 + rowIndex, 7, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts4Last"].ToString(), 9 + rowIndex, 8, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts5Last"].ToString(), 9 + rowIndex, 9, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekOutsLast"].ToString(), 9 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week1Last"].ToString(), 9 + rowIndex, 11, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week2Last"].ToString(), 9 + rowIndex, 12, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week3Last"].ToString(), 9 + rowIndex, 13, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week4Last"].ToString(), 9 + rowIndex, 14, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week5Last"].ToString(), 9 + rowIndex, 15, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekLast"].ToString(), 9 + rowIndex, 16, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalLast"].ToString(), 9 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);

                        excelReport.SetCellValue(row["SaldoAwal"].ToString(), 9 + rowIndex, 18, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts1"].ToString(), 9 + rowIndex, 19, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts2"].ToString(), 9 + rowIndex, 20, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts3"].ToString(), 9 + rowIndex, 21, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts4"].ToString(), 9 + rowIndex, 22, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["WeekOuts5"].ToString(), 9 + rowIndex, 23, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeekOuts"].ToString(), 9 + rowIndex, 24, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week1"].ToString(), 9 + rowIndex, 25, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week2"].ToString(), 9 + rowIndex, 26, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week3"].ToString(), 9 + rowIndex, 27, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week4"].ToString(), 9 + rowIndex, 28, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Week5"].ToString(), 9 + rowIndex, 29, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotalWeek"].ToString(), 9 + rowIndex, 30, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 31, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(row["TotPercent"].ToString(), 9 + rowIndex, 32, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalDecimal : ExcelCellStyle.RightBorderedStandardDecimal, true);

                        count--;
                        rowIndex++;
                    }
                }
                #endregion

                //excelReport.CloseExcelFileWriter();
                #endregion
            }

            return excelReport;
        }

        public ExcelFileWriter CreateReportExportGenerateITS(string fileName, DataSet dt0)
        {
            ExcelFileWriter excelReport = new ExcelFileWriter(fileName, "Generate ITS", "Generate ITS");

            for (int i = 0; i < 35; i++)
            {
                excelReport.SettingColumnWidth(1);
            }

            int rowIndex = 0;
            excelReport.SetCellValue("ITS DATA", rowIndex, 0, 1, 3, ExcelCellStyle.Header2, false, "14");
            rowIndex += 2;
            excelReport.SetCellValue("Inquiry Number", rowIndex, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Inquiry Date", rowIndex, 1, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Area", rowIndex, 2, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Company Code", rowIndex, 3, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Dealer Name", rowIndex, 4, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Outlet Abbreviation", rowIndex, 5, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("CityId", rowIndex, 6, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("City Name", rowIndex, 7, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Tipe Kendaraan", rowIndex, 8, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Variant", rowIndex, 9, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Transmisi", rowIndex, 10, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Colour Code", rowIndex, 11, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Wiraniaga", rowIndex, 12, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Sales Coordinator", rowIndex, 13, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Sales Head", rowIndex, 14, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Last Progress", rowIndex, 15, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Lost Case Category", rowIndex, 16, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Lost Case ReasonID", rowIndex, 17, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("SPK Date", rowIndex, 18, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Quantity Inquiry", rowIndex, 19, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Wiraniaga Flag", rowIndex, 20, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Grading", rowIndex, 21, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Branch Head", rowIndex, 22, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Perolehan Data", rowIndex, 23, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Test Drive", rowIndex, 24, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Cara Pembayaran", rowIndex, 25, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Leasing", rowIndex, 26, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Down Payment", rowIndex, 27, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Tenor", rowIndex, 28, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Nama Prospek", rowIndex, 29, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Alamat Prospek", rowIndex, 30, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Telp Rumah", rowIndex, 31, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Next Follow Up Date", rowIndex, 32, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Nama Perusahaan", rowIndex, 33, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Alamat Perusahaan", rowIndex, 34, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Handphone", rowIndex, 35, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Merk Lain", rowIndex, 36, 1, 1, ExcelCellStyle.LeftBold);
            rowIndex += 1;

            foreach (DataRow item in dt0.Tables[0].Rows)
            {
                //var item = dt0.Tables[0].Rows[i];
                //foreach (DataRow item in dt0.Tables[0].Rows)
                //{
                excelReport.SetCellValue(item["InquiryNumber"].ToString(), rowIndex, 0, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["InquiryDate"].ToString(), rowIndex, 1, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["Area"].ToString(), rowIndex, 2, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["CompanyCode"].ToString(), rowIndex, 3, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["DealerName"].ToString(), rowIndex, 4, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["OutletAbbreviation"].ToString(), rowIndex, 5, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["CityId"].ToString(), rowIndex, 6, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["CityName"].ToString(), rowIndex, 7, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["TipeKendaraan"].ToString(), rowIndex, 8, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["Variant"].ToString(), rowIndex, 9, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["Transmisi"].ToString(), rowIndex, 10, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["ColourCode"].ToString(), rowIndex, 11, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["Wiraniaga"].ToString(), rowIndex, 12, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["SalesCoordinator"].ToString(), rowIndex, 13, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["SalesHead"].ToString(), rowIndex, 14, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["LastProgress"].ToString(), rowIndex, 15, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["LostCaseCategory"].ToString(), rowIndex, 16, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["LostCaseReasonID"].ToString(), rowIndex, 17, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["SPKDate"].ToString(), rowIndex, 18, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["QuantityInquiry"].ToString(), rowIndex, 19, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["WiraniagaFlag"].ToString(), rowIndex, 20, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["Grading"].ToString(), rowIndex, 21, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["BranchHead"].ToString(), rowIndex, 22, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["PerolehanData"].ToString(), rowIndex, 23, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["TestDrive"].ToString(), rowIndex, 24, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["CaraPembayaran"].ToString(), rowIndex, 25, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["Leasing"].ToString(), rowIndex, 26, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["DownPayment"].ToString(), rowIndex, 27, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["Tenor"].ToString(), rowIndex, 28, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["NamaProspek"].ToString(), rowIndex, 29, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["AlamatProspek"].ToString(), rowIndex, 30, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["TelpRumah"].ToString(), rowIndex, 31, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["NextFollowUpDate"].ToString(), rowIndex, 32, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["NamaPerusahaan"].ToString(), rowIndex, 33, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["AlamatPerusahaan"].ToString(), rowIndex, 34, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["Handphone"].ToString(), rowIndex, 35, 1, 1, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(item["MerkLain"].ToString(), rowIndex, 36, 1, 1, ExcelCellStyle.LeftBold);
                rowIndex += 1;
            }

            return excelReport;
        }

        #endregion

        #region JsonResult

        public JsonResult Default()
        {
            string comp = Request["CompanyCode"] ?? "--";

            return Json(new
            {
                success = true,
                data = new
                {
                    DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    DateTo = DateTime.Now,
                    DateToDSR = DateTime.Now.AddDays(-1)
                }
            });

        }

        public JsonResult ReportInqWihtSts()
        {
            List<Combo> listCombo = new List<Combo>();

            Combo combo1 = new Combo();
            combo1.Value = "0";
            combo1.Text = "By Dealer";

            listCombo.Add(combo1);

            Combo combo2 = new Combo();
            combo2.Value = "1";
            combo2.Text = "By Type";

            listCombo.Add(combo2);

            return Json(listCombo.Select(p => new { value = p.Value, text = p.Text }).ToList());
        }

        public JsonResult ReportTypeInqWihtSts()
        {
            List<Combo> listCombo = new List<Combo>();

            Combo combo1 = new Combo();
            combo1.Value = "0";
            combo1.Text = "Summary";

            listCombo.Add(combo1);

            Combo combo2 = new Combo();
            combo2.Value = "1";
            combo2.Text = "Detail";

            listCombo.Add(combo2);

            return Json(listCombo.Select(p => new { value = p.Value, text = p.Text }).ToList());
        }

        public JsonResult ReportTypes()
        {
            List<Combo> listCombo = new List<Combo>();

            Combo combo1 = new Combo();
            combo1.Value = "0";
            combo1.Text = "Summary Inquiry";

            listCombo.Add(combo1);

            Combo combo2 = new Combo();
            combo2.Value = "1";
            combo2.Text = "Saldo Inquiry";

            listCombo.Add(combo2);

            return Json(listCombo.Select(p => new { value = p.Value, text = p.Text }).ToList());
        }

        public JsonResult ProductivityBy()
        {
            List<Combo> listCombo = new List<Combo>();

            Combo combo1 = new Combo();
            combo1.Value = "0";
            combo1.Text = "Salesman";

            listCombo.Add(combo1);

            Combo combo2 = new Combo();
            combo2.Value = "1";
            combo2.Text = "Vehicle Type";

            listCombo.Add(combo2);

            Combo combo3 = new Combo();
            combo3.Value = "2";
            combo3.Text = "Source Data";

            listCombo.Add(combo3);

            return Json(listCombo.Select(p => new { value = p.Value, text = p.Text }).ToList());
        }

        public JsonResult FilterGenerateITS()
        {
            List<Combo> listCombo = new List<Combo>();

            Combo combo1 = new Combo();
            combo1.Value = "0";
            combo1.Text = "Inquiry Date";

            listCombo.Add(combo1);

            Combo combo2 = new Combo();
            combo2.Value = "1";
            combo2.Text = "Next Follow up Date";

            listCombo.Add(combo2);

            return Json(listCombo.Select(p => new { value = p.Value, text = p.Text }).ToList());
        }

        public JsonResult ComboSalesman(string employeeID, string lookup, string dealer, string outlet)
        {
            string sql = string.Empty;
            if (lookup == "40")
            {
                sql = string.Format(@"
	                select distinct BranchHead EmployeeID, BranchHead EmployeeName 
                    from SuzukiR4..pmHstITS
                    where (case when '{0}' = '' then '' else CompanyCode end) = '{0}' and (case when '{1}' = '' then '' else BranchCode end) = '{1}' and BranchHead != ''
                    ", dealer == null ? string.Empty : dealer, outlet == null ? string.Empty : outlet);
            }
            else if (lookup == "30")
            {
                sql = string.Format(@"
	                select distinct SalesHead EmployeeID, SalesHead EmployeeName 
                    from SuzukiR4..pmHstITS
                    where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                        and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                        and (case when '{2}' = '' then '' else BranchHead end) = '{2}'
                        and SalesHead != ''"
                        , dealer, outlet == null ? string.Empty : outlet, employeeID);
            }
            else if (lookup == "20")
            {
                sql = string.Format(@"
	                select distinct SalesCoordinator EmployeeID, SalesCoordinator EmployeeName 
                    from SuzukiR4..pmHstITS
                    where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                        and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                        and (case when '{2}' = '' then '' else SalesHead end) = '{2}'
                        and SalesCoordinator != ''"
                        , dealer, outlet == null ? string.Empty : outlet, employeeID);
            }
            else if (lookup == "10")
            {
                sql = string.Format(@"
	                select distinct Wiraniaga EmployeeID, Wiraniaga EmployeeName 
                    from SuzukiR4..pmHstITS
                    where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                        and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                        and (case when '{2}' = '' then '' else SalesCoordinator end) = '{2}'
                        and Wiraniaga != ''"
                        , dealer, outlet == null ? string.Empty : outlet, employeeID);
            }

            var list = (from p in ctx.Database.SqlQuery<InqEmployee>(sql)
                        select new { value = p.EmployeeID, text = p.EmployeeName }).ToList();

            return Json(list);
        }

        public JsonResult ComboSalesmanHrEmp(string employeeID, string lookup, string dealer, string outlet)
        {
            string sql = string.Empty;
            var list = (from p in ctx.Database.SqlQuery<InqEmployee>("select '' EmployeeID, '' EmployeeName")
                        select new { value = p.EmployeeID, text = p.EmployeeName }).ToList();
 
            if (lookup == "40")
            {
                if (string.IsNullOrEmpty(dealer) || string.IsNullOrEmpty(outlet)) return Json(list); 
 
                sql = string.Format(@"
                    select distinct a.EmployeeID, a.EmployeeName 
                    from HrEmployee a with(nolock,nowait)
                    inner join HrEmployeeMutation b with(nolock,nowait) on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
                    where (case when '{0}' = '' then '' else a.CompanyCode end) = '{0}' 
                    and (case when '{1}' = '' then '' else b.BranchCode end) = '{1}' 
                    and a.Department = 'SALES' and a.Position = 'BM'
                    and b.IsJoinDate = 1", dealer == null ? string.Empty : dealer, outlet == null ? string.Empty : outlet);
            }
            else if (lookup == "30")
            {
                if (string.IsNullOrEmpty(dealer) || string.IsNullOrEmpty(outlet)) return Json(list); 
                sql = string.Format(@"
	               select distinct a.EmployeeID, a.EmployeeName 
                    from HrEmployee a with(nolock,nowait)
                    inner join HrEmployeeMutation b with(nolock,nowait) on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
                    where (case when '{0}' = '' then '' else a.CompanyCode end) = '{0}' 
                    and (case when '{1}' = '' then '' else b.BranchCode end) = '{1}' 
                    and (case when '{2}' = '' then '' else a.TeamLeader end) = '{2}'
                    and a.Department = 'SALES' and a.Position = 'SH'
                    and b.IsJoinDate = 1", dealer, outlet == null ? string.Empty : outlet, employeeID);
            }
            else if (lookup == "10")
            {
                if (string.IsNullOrEmpty(dealer) || string.IsNullOrEmpty(outlet)) return Json(list); 

                sql = string.Format(@"
	                select distinct a.EmployeeID, a.EmployeeName 
                    from HrEmployee a with(nolock,nowait)
                    inner join HrEmployeeMutation b with(nolock,nowait) on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
                    where (case when '{0}' = '' then '' else a.CompanyCode end) = '{0}' 
                    and (case when '{1}' = '' then '' else b.BranchCode end) = '{1}' 
                    and (case when '{2}' = '' then '' else a.TeamLeader end) = '{2}'
                    and a.Department = 'SALES' and a.Position = 'S'
                    and b.IsJoinDate = 1", dealer, outlet == null ? string.Empty : outlet, employeeID);
            }

            list = (from p in ctx.Database.SqlQuery<InqEmployee>(sql)
                        select new { value = p.EmployeeID, text = p.EmployeeName }).ToList();

            return Json(list);
        }

        public JsonResult CheckComboSalesmanHrEmp(string employeeID, string lookup, string dealer, string outlet)
        {
            string sql = string.Empty;
            if (lookup == "40")
            {
                sql = string.Format(@"
                    select distinct a.EmployeeID, a.EmployeeName 
                    from HrEmployee a
                    inner join HrEmployeeMutation b on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
                    where (case when '{0}' = '' then '' else a.CompanyCode end) = '{0}' 
                    and (case when '{1}' = '' then '' else b.BranchCode end) = '{1}' 
                    and a.Department = 'SALES' and a.Position = 'BM'
                    and b.IsJoinDate = 1", dealer == null ? string.Empty : dealer, outlet == null ? string.Empty : outlet);
            }
            else if (lookup == "30")
            {
                sql = string.Format(@"
	               select distinct a.EmployeeID, a.EmployeeName 
                    from HrEmployee a
                    inner join HrEmployeeMutation b on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
                    where (case when '{0}' = '' then '' else a.CompanyCode end) = '{0}' 
                    and (case when '{1}' = '' then '' else b.BranchCode end) = '{1}' 
                    and (case when '{2}' = '' then '' else a.TeamLeader end) = '{2}'
                    and a.Department = 'SALES' and a.Position = 'SH'
                    and b.IsJoinDate = 1", dealer, outlet == null ? string.Empty : outlet, employeeID);
            }
            else if (lookup == "10")
            {
                sql = string.Format(@"
	                select distinct a.EmployeeID, a.EmployeeName 
                    from HrEmployee a
                    inner join HrEmployeeMutation b on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
                    where (case when '{0}' = '' then '' else a.CompanyCode end) = '{0}' 
                    and (case when '{1}' = '' then '' else b.BranchCode end) = '{1}' 
                    and (case when '{2}' = '' then '' else a.TeamLeader end) = '{2}'
                    and a.Department = 'SALES' and a.Position = 'S'
                    and b.IsJoinDate = 1", dealer, outlet == null ? string.Empty : outlet, employeeID);
            }

            var list = (from p in ctx.Database.SqlQuery<InqEmployee>(sql)
                        select new { value = p.EmployeeID, text = p.EmployeeName }).ToList();

            return Json(new { success = true, count = list.Count, data = list.FirstOrDefault() });
        }

        public JsonResult DealerMappingAreas()
        {
            string sql = string.Format("exec uspfn_gnInquiryBtn '', '{0}', '', '1'", "");

            var list = (from p in ctx.Database.SqlQuery<InquiryBtn>(sql)
                        select new InquiryBtn()
                        {
                            GroupArea = p.Area,
                            Area = p.Area
                        }).Select(p => new { value = p.GroupArea, text = p.Area }).ToList();
            return Json(list);
        }

        public JsonResult DealerMappingDealers()
        {
            string area = Request["id"] ?? "";
            string dealer = Request["dealer"] ?? "";

            string sql = string.Format("exec uspfn_gnInquiryBtn '{0}', '{1}', '', '2'", area, dealer);

            var list = (from p in ctx.Database.SqlQuery<InquiryBtn>(sql)
                        select new InquiryBtn()
                        {
                            DealerCode = p.DealerCode,
                            DealerName = p.DealerName
                        }).Select(p => new { value = p.DealerCode, text = p.DealerCode + " - " + p.DealerName }).ToList();
            return Json(list);
        }

        public JsonResult Outlets()
        {
            string area = Request["area"] ?? "";
            string dealer = Request["dealer"] ?? "";

            string sql = string.Format("exec uspfn_gnInquiryBtn '{0}', '{1}', '', '3'", area, dealer);

            var list = (from p in ctx.Database.SqlQuery<InquiryBtn>(sql)
                        select new InquiryBtn()
                        {
                            OutletCode = p.OutletCode,
                            OutletName = p.OutletName
                        }).Select(p => new { value = p.OutletCode, text = p.OutletCode + " - " + p.OutletName }).ToList();
            return Json(list);
        }

        public JsonResult GroupModels()
        {
            string sql = "select distinct GroupModel as value, GroupModel as text from SuzukiR4..msMstGroupModel";
            //string sql = "select distinct GroupCode as value, GroupCode as text from omMstModel";
            var list = (from p in ctx.Database.SqlQuery<Combo>(sql)
                        select new Combo()
                        {
                            Value = p.Value,
                            Text = p.Text
                        }).Select(p => new { value = p.Value, text = p.Text }).ToList();

            return Json(list);
        }

        public JsonResult ModelTypes(string id = "")
        {
            string sql = string.Format("select distinct ModelType as value, ModelType as text from SuzukiR4..msMstGroupModel where GroupModel = '{0}'", id);
            //string sql = string.Format("select distinct SalesModelcode as value, SalesModelcode as text from omMstModel where GroupCode = '{0}'", id);
            var list = (from p in ctx.Database.SqlQuery<Combo>(sql)
                        select new Combo()
                        {
                            Value = p.Value,
                            Text = p.Text
                        }).Select(p => new { value = p.Value, text = p.Text }).ToList();

            return Json(list);
        }

        public JsonResult GroupModelsR2()
        {
            string sql = "select distinct substring(TipeKendaraan,1,2) as value, substring(TipeKendaraan,1,2) as text  from pmHstIts where isnull(TipeKendaraan,'')<>'' and Substring(CompanyCode,5,1)=2 ORDER BY substring(TipeKendaraan,1,2) ";
            var list = (from p in ctx.Database.SqlQuery<Combo>(sql)
                        select new Combo()
                        {
                            Value = p.Value,
                            Text = p.Text
                        }).Select(p => new { value = p.Value, text = p.Text }).ToList();

            return Json(list);
        }

        public JsonResult ModelTypesR2(string id = "")
        {
            string sql = string.Format("select distinct replace((TipeKendaraan+Variant),' ','') as value, (TipeKendaraan + Variant) as text from pmHstIts where isnull(variant,'')<>'' and substring(tipekendaraan,1,2) = '{0}' ORDER BY VALUE", id);
            var list = (from p in ctx.Database.SqlQuery<Combo>(sql)
                        select new Combo()
                        {
                            Value = p.Value,
                            Text = p.Text
                        }).Select(p => new { value = p.Value, text = p.Text }).ToList();

            return Json(list);
        }

        public JsonResult GroupModelsR4()
        {
            string sql = "select distinct substring(TipeKendaraan,1,2) as value, substring(TipeKendaraan,1,2) as text  from pmHstIts where isnull(TipeKendaraan,'')<>'' and Substring(CompanyCode,5,1)=4";
            var list = (from p in ctx.Database.SqlQuery<Combo>(sql)
                        select new Combo()
        {
            Value = p.Value,
            Text = p.Text
        }).Select(p => new { value = p.Value, text = p.Text }).ToList();

            return Json(list);
        }

        public JsonResult ModelTypesR4(string id = "")
        {
            string sql = string.Format("select distinct (TipeKendaraan + Variant) as value, (TipeKendaraan + Variant) as text from pmHstIts where isnull(variant,'')<>'' and substring(tipekendaraan,1,2) = '{0}'", id);
            var list = (from p in ctx.Database.SqlQuery<Combo>(sql)
                        select new Combo()
                        {
                            Value = p.Value,
                            Text = p.Text
                        }).Select(p => new { value = p.Value, text = p.Text }).ToList();

            return Json(list);
        }

        public JsonResult CarVariants()
        {
            string dealer = Request["dealer"] ?? "";
            string groupmodel = Request["groupmodel"] ?? "";
            string modeltype = Request["modeltype"] ?? "";

            string sql = string.Format("select distinct TypeCode as value, TypeCode as text from SuzukiR4..pmGroupTypeSeq where case when '{0}' = '' then '' else CompanyCode end = '{0}' and (GroupCode = '{1}' or GroupCode = '{2}')", dealer, groupmodel, modeltype);

            var list = (from p in ctx.Database.SqlQuery<Combo>(sql)
                        select new Combo()
                        {
                            Value = p.Value,
                            Text = p.Text
                        }).Select(p => new { value = p.Value, text = p.Text }).ToList();

            return Json(list);
        }

        public JsonResult ExportGenerateITS(string area, string dealerCode, string outletCode, DateTime startDate, DateTime endDate, string reportType)
        {
            var qry = ctx.GenerateITSViews.AsQueryable();
            if (!string.IsNullOrWhiteSpace(area)) { qry = qry.Where(p => p.Area.Contains(area)); };
            if (!string.IsNullOrWhiteSpace(dealerCode)) { qry = qry.Where(p => p.CompanyCode.Contains(dealerCode)); };
            if (!string.IsNullOrWhiteSpace(outletCode)) { qry = qry.Where(p => p.BranchCode.Contains(outletCode)); };

            string fileName = "GenerateITS_";
            if (reportType == "0")
            {
                qry = qry.Where(p => p.InquiryDate >= startDate && p.InquiryDate <= endDate);
                fileName = fileName + "InquiryDt";
            }
            else if (reportType == "1")
            {
                qry = qry.Where(p => p.NextFollowUpdate >= startDate && p.NextFollowUpdate <= endDate);
                fileName = fileName + "NextFollowUpDt";
            }

            RenderReport(Server.MapPath("~/Reports/rdlc/its/itsgenerate.rdlc"), fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmss"), 10, 11.7, "excel", qry.ToList());
            return null;
        }

        public JsonResult DefaultRawDatSPK()
        {
            return Json(new
            {
                success = true,
                data = new
                {
                    cCode = CompanyCode,
                    bCode = BranchCode,
                    DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    DateTo = DateTime.Now,
                }
            });
        }

        #endregion

        #region ActionResult

        public ActionResult CreateExcelInqWithSts(DateTime DateFrom, DateTime DateTo, string Area, string Dealer, string Outlet, string GroupModel, string ModelType, string Variant, string report, string reportType)
        {
            //ExcelFileWriter excelReport;

            string groupNo = "";
            if (Dealer.IndexOf("|") > 0)
            {
                string[] xCode = Dealer.Split('|');
                groupNo = xCode[0];
                Dealer = xCode[1];
            }
            
            string fileName = "";
            if (report == "0")
            {
                if (reportType == "0")
                    fileName = "InqItsWithStatusByDealerSum_";
                else
                    fileName = "InqItsWithStatusByDealerDtl_";

                fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");
            }
            else
            {
                if (reportType == "0")
                    fileName = "InqItsWithStatusByTypeSum_";
                else
                    fileName = "InqItsWithStatusByTypeDtl_";

                fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");
            }

            DateTime dtTempLastStart = DateFrom.AddMonths(-1);
            DateTime dtLastStart = new DateTime(dtTempLastStart.Year, dtTempLastStart.Month, 1);
            string pLastDateFrom = dtLastStart.ToString("yyyyMMdd");

            //DateTime dtEnd = Convert.ToDateTime(Request["DateTo"]);
            DateTime dtTempLastEnd = DateTo.AddMonths(-1);
            int daysInMonth = DateTime.DaysInMonth(dtTempLastEnd.Year, dtTempLastEnd.Month);
            DateTime dtLastEnd = new DateTime(dtTempLastEnd.Year, dtTempLastEnd.Month, daysInMonth);

            string pLastDateTo = dtLastEnd.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            if (report == "0")
                cmd.CommandText = "uspfn_InquiryITSWithStatusByDealer_RevNew";
            else
                cmd.CommandText = "uspfn_InquiryITSWithStatusByTypeNew";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", Dealer);
            cmd.Parameters.AddWithValue("@BranchCode", Outlet);
            cmd.Parameters.AddWithValue("@StartDate", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@EndDate", DateTo.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@LastStartDate", pLastDateFrom);
            cmd.Parameters.AddWithValue("@LastEndDate", pLastDateTo);
            cmd.Parameters.AddWithValue("@Area", Area);
            cmd.Parameters.AddWithValue("@GroupModel", GroupModel);
            cmd.Parameters.AddWithValue("@TipeKendaraan", ModelType);
            cmd.Parameters.AddWithValue("@Variant", Variant);
            cmd.Parameters.AddWithValue("@Summary", (reportType == "0") ? 1 : 0);
            cmd.Parameters.AddWithValue("@NewArea", groupNo);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            da.Fill(dt);

            string pPeriode = DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");
            string sql = string.Empty;
            string AreaName = "";
            if (Area == string.Empty)
            {
                AreaName = "ALL";
            }else{
                AreaName = ctx.GroupAreas.Where(x => x.GroupNo == Area).FirstOrDefault().AreaDealer;
            }
               
            if (Dealer != string.Empty)
            {
                if (Area == "100")
                {
                    sql = string.Format("select DealerName from gnMstDealerMappingNew where DealerCode = '{0}' and groupno = '{1}'", Dealer, groupNo);
                    Dealer = ctx.Database.SqlQuery<string>(sql).FirstOrDefault().ToString();
                } else {
                    sql = string.Format("select DealerName from gnMstDealerMappingNew where DealerCode = '{0}' and groupno = '{1}'", Dealer, Area);
                    Dealer = ctx.Database.SqlQuery<string>(sql).FirstOrDefault().ToString();
                }
            }
            else
                Dealer = "ALL";

            if (Outlet != string.Empty)
            {
                sql = string.Format("select OutletName from gnMstDealerOutletMappingNew where OutletCode = '{0}' and groupno = '{1}'", Outlet, groupNo);
                Outlet = ctx.Database.SqlQuery<string>(sql).FirstOrDefault().ToString();
            }
            else
                Outlet = "ALL";

            var files = Directory.GetFiles(HttpContext.Server.MapPath(@"~/ReportTemp/"));
            foreach (var file in files)
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch
                {
                    // Do Nothing.
                }
            }

            if (dt.Tables[0].Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                ExcelFileWriter excelReport = CreateReportExcelInqWithSts(fileName, dt, pPeriode, AreaName, Dealer, Outlet, report, reportType);
                string result = excelReport.CloseExcelFileWriter();

                HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + fileName + ".xls");
                HttpContext.Response.AddHeader("Content-Length", result.Length.ToString(CultureInfo.InvariantCulture));
                this.Response.ContentType = "application/vnd.ms-excel";
                System.IO.File.WriteAllText(HttpContext.Server.MapPath(@"~/ReportTemp/") + fileName + ".xls", result);

                return Redirect(("~/ReportTemp/") + fileName + ".xls");
            }
        }

        public ActionResult CreateExcelExportGenerateITS(string area, string dealerCode, string outletCode, DateTime startDate, DateTime endDate, string reportType)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            if (reportType == "0")
                fileName = "InquiryDt_" + DateTime.Now.ToString("yyyyMMdd-hhmmss");
            else
                fileName = "NextFollowUpDt" + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            //cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GenerateITS";
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GenerateITS_New";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@CompanyName", dealerCode);
            cmd.Parameters.AddWithValue("@OutletCode", outletCode);
            cmd.Parameters.AddWithValue("@FilterBy", reportType);

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Generate ITS");

            return GenerateExcel(wb, dt, lastRow, fileName);

            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataSet dt = new DataSet();
            //da.Fill(dt);

            //var files = Directory.GetFiles(HttpContext.Server.MapPath(@"~/ReportTemp/"));
            //foreach (var file in files)
            //{
            //    try
            //    {
            //        System.IO.File.Delete(file);
            //    }
            //    catch
            //    {
            //        // Do Nothing.
            //    }
            //}

            //ExcelFileWriter excelReport = CreateReportExportGenerateITS(fileName, dt);
            //string result = excelReport.CloseExcelFileWriter();

            //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + fileName + ".xls");
            //HttpContext.Response.AddHeader("Content-Length", result.Length.ToString(CultureInfo.InvariantCulture));
            //this.Response.ContentType = "application/vnd.ms-excel";
            //System.IO.File.WriteAllText(HttpContext.Server.MapPath(@"~/ReportTemp/") + fileName + ".xls", result);

            //return Redirect(("~/ReportTemp/") + fileName + ".xls");
        }

        public ActionResult GenerateITSWithStatusAndTestDrive(DateTime StartDate, DateTime EndDate)
        {
            HttpContext.Server.ScriptTimeout = 13600;
            DateTime now = DateTime.Now;
            string fileName = "ITS_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var data = ctx.Database.SqlQuery<GenerateITSWithStatusAndTestDriveModel>("exec uspfn_GenerateITSWithStatusAndTestDrive_Rev1 @StartDate=@p0, @EndDate=@p1", StartDate.ToString("yyyyMMdd"), EndDate.ToString("yyyyMMdd"));

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 360000;
            cmd.CommandText = "uspfn_GenerateITSWithStatusAndTestDrive_Rev1";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@StartDate", StartDate.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@EndDate", EndDate.ToString("yyyyMMdd"));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("ITSReport");

            //First Names
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer";
            ws.Cell("C" + recNo).Value = "Abbr";
            ws.Cell("D" + recNo).Value = "Outlet";
            ws.Cell("E" + recNo).Value = "OutletAbbr";
            ws.Cell("F" + recNo).Value = "CityId";
            ws.Cell("G" + recNo).Value = "CityName";
            ws.Cell("H" + recNo).Value = "Date";
            ws.Cell("I" + recNo).Value = "WorkDay";
            ws.Cell("J" + recNo).Value = "Model";
            ws.Cell("K" + recNo).Value = "Var";
            ws.Cell("L" + recNo).Value = "ColourName";
            ws.Cell("M" + recNo).Value = "Inq";
            ws.Cell("N" + recNo).Value = "InqTestDrive";
            ws.Cell("O" + recNo).Value = "HP";
            ws.Cell("P" + recNo).Value = "HPTestDrive";
            ws.Cell("Q" + recNo).Value = "SPK";
            ws.Cell("R" + recNo).Value = "SPKTesDrive";
            ws.Cell("S" + recNo).Value = "DO";
            ws.Cell("T" + recNo).Value = "DOTesDrive";
            ws.Cell("U" + recNo).Value = "DELIVERY";
            ws.Cell("V" + recNo).Value = "DELIVERYTesDrive";
            ws.Cell("W" + recNo).Value = "INVOICE";
            ws.Cell("X" + recNo).Value = "INVOICETesDrive";
            ws.Cell("Y" + recNo).Value = "LOST";

            recNo++;

            foreach (DataRow row in dt.Rows)
            {
                ws.Cell("A" + recNo).Value = row["Area"].ToString();
                ws.Cell("B" + recNo).Value = row["Dealer"].ToString();
                ws.Cell("C" + recNo).Value = row["Abbr"].ToString();
                ws.Cell("D" + recNo).Value = row["OutletCode"].ToString();
                ws.Cell("E" + recNo).Value = row["OutletAbbreviation"].ToString();
                ws.Cell("F" + recNo).Value = row["CityId"].ToString();
                ws.Cell("G" + recNo).Value = row["CityName"].ToString();
                ws.Cell("H" + recNo).Value = row["Date"].ToString();
                ws.Cell("I" + recNo).Value = row["WorkDay"].ToString();
                ws.Cell("J" + recNo).Value = row["Model"].ToString();
                ws.Cell("K" + recNo).Value = row["Var"].ToString();
                ws.Cell("L" + recNo).Value = row["ColourName"].ToString();
                ws.Cell("M" + recNo).Value = row["INQ"].ToString();
                ws.Cell("N" + recNo).Value = row["InqTestDrive"].ToString();
                ws.Cell("O" + recNo).Value = row["HP"].ToString();
                ws.Cell("P" + recNo).Value = row["HPTestDrive"].ToString();
                ws.Cell("Q" + recNo).Value = row["SPK"].ToString();
                ws.Cell("R" + recNo).Value = row["SPKTestDrive"].ToString();
                ws.Cell("S" + recNo).Value = row["DO"].ToString();
                ws.Cell("T" + recNo).Value = row["DOTestDrive"].ToString();
                ws.Cell("U" + recNo).Value = row["DELIVERY"].ToString();
                ws.Cell("V" + recNo).Value = row["DELIVERYTestDrive"].ToString();
                ws.Cell("W" + recNo).Value = row["INVOICE"].ToString();
                ws.Cell("X" + recNo).Value = row["INVOICETestDrive"].ToString();
                ws.Cell("Y" + recNo).Value = row["LOST"].ToString();
                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult GenerateInquiryProd(DateTime startDate, DateTime endDate, string area, string dealerCode, string outletCode, string branchHead, string salesHead,
            string salesman, string typeReport, string productivityBy, string idReport)
        {
            //ExcelFileWriter excelReport;
            string fileName = idReport + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmss");
            
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "usprpt_InquiryITSProd";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@DealerCode", dealerCode);
            cmd.Parameters.AddWithValue("@OutletCode", outletCode);
            cmd.Parameters.AddWithValue("@BranchHead", branchHead);
            cmd.Parameters.AddWithValue("@SalesHead", salesHead);
            cmd.Parameters.AddWithValue("@Salesman", salesman);
            cmd.Parameters.AddWithValue("@TypeReport", typeReport);
            cmd.Parameters.AddWithValue("@ProductivityBy", productivityBy);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                return Json(new { success = true });
            }
            else return Json(new{success = false });
        }

        public ActionResult GenerateInquiryProductivity(DateTime startDate, DateTime endDate, string area, string dealerCode, string outletCode, string branchHead, string salesHead,
           string salesman, string typeReport, string productivityBy, string idReport)
        {
            //ExcelFileWriter excelReport;
            string fileName = idReport + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmss");
            string title = "";
            if (typeReport == "0")
            {
                if (productivityBy == "0")
                    title = "Summary Inquiry Productivity By Salesman";
                else if (productivityBy == "1")
                    title = "Summary Inquiry Productivity By Vehicle";
                else if (productivityBy == "2")
                    title = "Summary Inquiry Productivity By Source";
            }
            else if (typeReport == "1")
            {
                if (productivityBy == "0")
                    title = "Saldo Inquiry Productivity By Salesman";
                else if (productivityBy == "1")
                    title = "Saldo Inquiry Productivity By Vehicle";
                else if (productivityBy == "2")
                    title = "Saldo Inquiry Productivity By Source";
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "usprpt_InquiryITSProd";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@DealerCode", dealerCode);
            cmd.Parameters.AddWithValue("@OutletCode", outletCode);
            cmd.Parameters.AddWithValue("@BranchHead", branchHead);
            cmd.Parameters.AddWithValue("@SalesHead", salesHead);
            cmd.Parameters.AddWithValue("@Salesman", salesman);
            cmd.Parameters.AddWithValue("@TypeReport", typeReport);
            cmd.Parameters.AddWithValue("@ProductivityBy", productivityBy);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("sum");
            if (dealerCode != "" && outletCode == "")
            {
                var ws1 = wb.Worksheets.Add("perOutlet");
            }

            return GenerateExcelInqProd(wb, ds, lastRow, title, fileName, productivityBy, typeReport);
        }

        private ActionResult GenerateExcelInqProd(XLWorkbook wb, DataSet ds, int lastRow, string title, string fileName, string productivityBy, string reportType)
        {
            //var ws = wb.Worksheet(1);
            var tmpLastRow = lastRow;
            DataTable dt = ds.Tables[0];
            DataTable dt1 = new DataTable();
            if (ds.Tables.Count > 1)
                dt1 = ds.Tables[1];

            foreach (var ws in wb.Worksheets)
            {
                ws.Row(1).Height = 22.25;
                ws.Column("A").Width = 2.29;
                ws.Column("B").Width = 17.43;
                ws.Column("C").Width = 33;
                ws.Column("D").Width = ws.Column("E").Width = ws.Column("F").Width = ws.Column("G").Width = ws.Column("H").Width = ws.Column("I").Width = ws.Column("J").Width = 11.43;
                ws.Row(3).Height = 28.5;

                //Title
                ws.Cell("B2").Value = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                ws.Cell("B3").Value = title;
                ws.Range("B3", "J3").Merge();
                ws.Cell("B3").Style.Font.SetFontSize(16);
                ws.Cell("B3").Style.Font.SetBold(true);
                ws.Cell("B3").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                ws.Range("B4", "J4").Merge();

                // Sub Title
                ws.Cell("B5").Value = "Outstanding Per";
                ws.Cell("C5").Value = dt.Rows[0]["PerDate"];
                ws.Cell("B6").Value = "Area";
                ws.Cell("C6").Value = dt.Rows[0]["Area"].ToString();
                ws.Cell("B7").Value = "Dealer";
                ws.Cell("C7").Value = dt.Rows[0]["Dealer"].ToString();
                ws.Cell("B8").Value = "Outlet";
                ws.Cell("C8").Value = dt.Rows[0]["Outlet"].ToString();

                ws.Cell("F5").Value = "Periode DO";
                ws.Cell("H5").Value = dt.Rows[0]["PeriodeDO"];
                ws.Cell("F6").Value = "Sales Head";
                ws.Cell("H6").Value = dt.Rows[0]["SalesHead"];
                ws.Cell("F7").Value = "Salesman";
                ws.Cell("H7").Value = dt.Rows[0]["Salesman"];

                ws.Cell("B10").Value = "Berdasarkan " + dt.Rows[0]["ProductivityBy"];
                if (productivityBy == "0")
                {
                    if (ws == wb.Worksheet(1))
                        ws.Cell("B11").Value = "Posisi";
                    else
                        ws.Cell("B11").Value = "Kode Cabang";
                    ws.Cell("C11").Value = "Nama";

                    ws.Range("B11", "B12").Merge();
                    ws.Range("C11", "C12").Merge();
                }
                else 
                {
                    if (ws == wb.Worksheet(1))
                    {
                        if (productivityBy == "1") ws.Cell("B11").Value = "Tipe Kendaraan";
                        else ws.Cell("B11").Value = "Perolehan Data";
                        ws.Range("B11", "C12").Merge();
                        ws.Cell("B11").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    }
                    else
                    {
                        ws.Cell("B11").Value = "Kode Cabang";
                        if (productivityBy == "1") ws.Cell("C11").Value = "Tipe";
                        else ws.Cell("C11").Value = "Perolehan Data";
                        ws.Range("B11", "B12").Merge();
                        ws.Range("C11", "C12").Merge();
                    }
                }

                if (reportType == "0")
                {
                    ws.Cell("D11").Value = "Periode " + dt.Rows[0]["PeriodeDO"];
                    ws.Cell("D11").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Range("D11", "J11").Merge();
                }
                else
                {
                    ws.Cell("D11").Value = "Outstanding Per " + dt.Rows[0]["PerDate"];
                    ws.Cell("D11").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Range("D11", "G11").Merge();
                    ws.Cell("H11").Value = "Periode " + dt.Rows[0]["PeriodeDO"];
                    ws.Cell("H11").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Range("H11", "J11").Merge();
                }

                if (reportType == "0")
                {
                    ws.Cell("D12").Value = "NEW";
                    ws.Cell("D12").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell("E12").Value = "P";
                    ws.Cell("E12").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell("F12").Value = "HP";
                    ws.Cell("F12").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell("G12").Value = "SPK";
                    ws.Cell("G12").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                }
                else
                {
                    ws.Cell("D12").Value = "P";
                    ws.Cell("D12").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell("E12").Value = "HP";
                    ws.Cell("E12").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell("F12").Value = "SPK";
                    ws.Cell("F12").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell("G12").Value = "Sum Outs";
                    ws.Cell("G12").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                }

                ws.Cell("H12").Value = "DO";
                ws.Cell("H12").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell("I12").Value = "DELIVERY";
                ws.Cell("I12").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell("J12").Value = "LOST";
                ws.Cell("J12").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                for (int i = 5; i <= 10; i++)
                {
                    ws.Range("C" + i, "E" + i).Merge();
                    ws.Range("F" + i, "G" + i).Merge();
                    ws.Range("H" + i, "J" + i).Merge();
                    if (i == 9 || i == 10)
                        ws.Range("B" + i, "J" + i).Merge();
                }

                for (int i = 5; i <= 12; i++)
                {
                    ws.Cells("B" + i).Style.Font.SetBold(true);
                    ws.Cells("C" + i).Style.Font.SetBold(true);
                    ws.Cells("D" + i).Style.Font.SetBold(true);
                    ws.Cells("E" + i).Style.Font.SetBold(true);
                    ws.Cells("F" + i).Style.Font.SetBold(true);
                    ws.Cells("G" + i).Style.Font.SetBold(true);
                    ws.Cells("H" + i).Style.Font.SetBold(true);
                    ws.Cells("I" + i).Style.Font.SetBold(true);
                    ws.Cells("J" + i).Style.Font.SetBold(true);
                }

                int iFirstRow = 13;
                int iRow = 13;
                DataTable dtLoop = new DataTable();
                if (ws == wb.Worksheet(1))
                    dtLoop = dt;
                else if (ws == wb.Worksheet(2))
                    dtLoop = dt1;

                List<string> listD = new List<string>();
                List<string> listE = new List<string>();
                List<string> listF = new List<string>();
                List<string> listG = new List<string>();
                List<string> listH = new List<string>();
                List<string> listI = new List<string>();
                List<string> listJ = new List<string>();

                foreach (DataRow dr in dtLoop.Rows)
                {
                    if (ws == wb.Worksheet(1))
                    {
                        if (productivityBy == "0")
                        {
                            ws.Cells("B" + iRow).Value = dr["Position"];
                            ws.Cells("C" + iRow).Value = dr["EmployeeName"];
                        }
                        else 
                        {
                            if (productivityBy == "1") ws.Cells("B" + iRow).Value = dr["TipeKendaraan"];
                            else ws.Cells("B" + iRow).Value = dr["Source"];
                            ws.Range("B" + iRow, "C" + iRow).Merge();
                        }
                    }
                    else
                    {
                        if (productivityBy == "0")
                        {
                            if (dr["EmployeeName"].ToString() == "")
                            {
                                ws.Cells("B" + iRow).Value = "Sub Total " + dr["BranchCode"];
                                ws.Cells("B" + iRow).Style.Font.SetBold(true);
                                ws.Cell("B" + iRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Range("B" + iRow, "C" + iRow).Merge();
                                ws.Cells("D" + iRow).Style.Font.SetBold(true);
                                listD.Add("D" + iRow);
                                ws.Cells("E" + iRow).Style.Font.SetBold(true);
                                listE.Add("E" + iRow);
                                ws.Cells("F" + iRow).Style.Font.SetBold(true);
                                listF.Add("F" + iRow);
                                ws.Cells("G" + iRow).Style.Font.SetBold(true);
                                listG.Add("G" + iRow);
                                ws.Cells("H" + iRow).Style.Font.SetBold(true);
                                listH.Add("H" + iRow);
                                ws.Cells("I" + iRow).Style.Font.SetBold(true);
                                listI.Add("I" + iRow);
                                ws.Cells("J" + iRow).Style.Font.SetBold(true);
                                listJ.Add("J" + iRow);
                            }
                            else
                            {
                                ws.Cells("B" + iRow).Value = dr["BranchCode"];
                                ws.Cell("B" + iRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Cells("C" + iRow).Value = dr["EmployeeName"];
                            }   
                        }
                        else if (productivityBy == "1")
                        {
                            if (dr["TipeKendaraan"].ToString() == "")
                            {
                                ws.Cells("B" + iRow).Value = "Sub Total " + dr["BranchCode"];
                                ws.Cells("B" + iRow).Style.Font.SetBold(true);
                                ws.Cell("B" + iRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Range("B" + iRow, "C" + iRow).Merge();
                                ws.Cells("D" + iRow).Style.Font.SetBold(true);
                                listD.Add("D" + iRow);
                                ws.Cells("E" + iRow).Style.Font.SetBold(true);
                                listE.Add("E" + iRow);
                                ws.Cells("F" + iRow).Style.Font.SetBold(true);
                                listF.Add("F" + iRow);
                                ws.Cells("G" + iRow).Style.Font.SetBold(true);
                                listG.Add("G" + iRow);
                                ws.Cells("H" + iRow).Style.Font.SetBold(true);
                                listH.Add("H" + iRow);
                                ws.Cells("I" + iRow).Style.Font.SetBold(true);
                                listI.Add("I" + iRow);
                                ws.Cells("J" + iRow).Style.Font.SetBold(true);
                                listJ.Add("J" + iRow);
                            }
                            else
                            {
                                ws.Cells("B" + iRow).Value = dr["BranchCode"];
                                ws.Cell("B" + iRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Cells("C" + iRow).Value = dr["TipeKendaraan"];
                            }
                        }
                        else if (productivityBy == "2")
                        {
                            if (dr["Source"].ToString() == "")
                            {
                                ws.Cells("B" + iRow).Value = "Sub Total " + dr["BranchCode"];
                                ws.Cells("B" + iRow).Style.Font.SetBold(true);
                                ws.Cell("B" + iRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Range("B" + iRow, "C" + iRow).Merge();
                                ws.Cells("D" + iRow).Style.Font.SetBold(true);
                                listD.Add("D" + iRow);
                                ws.Cells("E" + iRow).Style.Font.SetBold(true);
                                listE.Add("E" + iRow);
                                ws.Cells("F" + iRow).Style.Font.SetBold(true);
                                listF.Add("F" + iRow);
                                ws.Cells("G" + iRow).Style.Font.SetBold(true);
                                listG.Add("G" + iRow);
                                ws.Cells("H" + iRow).Style.Font.SetBold(true);
                                listH.Add("H" + iRow);
                                ws.Cells("I" + iRow).Style.Font.SetBold(true);
                                listI.Add("I" + iRow);
                                ws.Cells("J" + iRow).Style.Font.SetBold(true);
                                listJ.Add("J" + iRow);
                            }
                            else
                            {
                                ws.Cells("B" + iRow).Value = dr["BranchCode"];
                                ws.Cell("B" + iRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Cells("C" + iRow).Value = dr["Source"];
                            }
                        }
                    }
                    
                    if (reportType == "0")
                    {
                        ws.Cells("D" + iRow).Value = dr["NEW"];
                        ws.Cells("E" + iRow).Value = dr["P"];
                        ws.Cells("F" + iRow).Value = dr["HP"];
                        ws.Cells("G" + iRow).Value = dr["SPK"];
                    }
                    else
                    {
                        ws.Cells("D" + iRow).Value = dr["P"];
                        ws.Cells("E" + iRow).Value = dr["HP"];
                        ws.Cells("F" + iRow).Value = dr["SPK"];
                        ws.Cells("G" + iRow).Value = dr["SumOuts"];
                    }

                    ws.Cells("H" + iRow).Value = dr["DO"];
                    ws.Cells("I" + iRow).Value = dr["DELIVERY"];
                    ws.Cells("J" + iRow).Value = dr["LOST"];
                    iRow += 1;

                    ws.Cells("D" + iRow).Style.NumberFormat.Format = "#,##0";
                    ws.Cells("E" + iRow).Style.NumberFormat.Format = "#,##0";
                    ws.Cells("F" + iRow).Style.NumberFormat.Format = "#,##0";
                    ws.Cells("G" + iRow).Style.NumberFormat.Format = "#,##0";
                    ws.Cells("H" + iRow).Style.NumberFormat.Format = "#,##0";
                    ws.Cells("I" + iRow).Style.NumberFormat.Format = "#,##0";
                    ws.Cells("J" + iRow).Style.NumberFormat.Format = "#,##0";
                }

                ws.Cells("B" + iRow).Value = "Total";
                ws.Cells("B" + iRow).Style.Font.SetBold(true);
                ws.Cell("B" + iRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Range("B" + iRow, "C" + iRow).Merge();

                if (ws == wb.Worksheet(1))
                {
                    ws.Cells("D" + iRow).FormulaA1 = "=SUM(D" + iFirstRow + ":D" + (iRow - 1) + ")";
                    ws.Cells("E" + iRow).FormulaA1 = "=SUM(E" + iFirstRow + ":E" + (iRow - 1) + ")";
                    ws.Cells("F" + iRow).FormulaA1 = "=SUM(F" + iFirstRow + ":F" + (iRow - 1) + ")";
                    ws.Cells("G" + iRow).FormulaA1 = "=SUM(G" + iFirstRow + ":G" + (iRow - 1) + ")";
                    ws.Cells("H" + iRow).FormulaA1 = "=SUM(H" + iFirstRow + ":H" + (iRow - 1) + ")";
                    ws.Cells("I" + iRow).FormulaA1 = "=SUM(I" + iFirstRow + ":I" + (iRow - 1) + ")";
                    ws.Cells("J" + iRow).FormulaA1 = "=SUM(J" + iFirstRow + ":J" + (iRow - 1) + ")";
                }
                else
                {
                    string sumD = "", sumE = "", sumF = "", sumG = "", sumH = "", sumI = "", sumJ = "";
                    int i = 0;
                    foreach (string list in listD)
                    {
                        i += 1;
                        if (i == listD.Count)
                            sumD = sumD + list;
                        else
                            sumD = sumD + list + "+";
                    }
                    i = 0;
                    foreach (string list in listE)
                    {
                        i += 1;
                        if (i == listE.Count)
                            sumE = sumE + list;
                        else
                            sumE = sumE + list + "+";
                    }
                    i = 0;
                    foreach (string list in listF)
                    {
                        i += 1;
                        if (i == listF.Count)
                            sumF = sumF + list;
                        else
                            sumF = sumF + list + "+";
                    }
                    i = 0;
                    foreach (string list in listG)
                    {
                        i += 1;
                        if (i == listG.Count)
                            sumG = sumG + list;
                        else
                            sumG = sumG + list + "+";
                    }
                    i = 0;
                    foreach (string list in listH)
                    {
                        i += 1;
                        if (i == listH.Count)
                            sumH = sumH + list;
                        else
                            sumH = sumH + list + "+";
                    } 
                    i = 0;
                    foreach (string list in listI)
                    {
                        i += 1;
                        if (i == listI.Count)
                            sumI = sumI + list;
                        else
                            sumI = sumI + list + "+";
                    }
                    i = 0;
                    foreach (string list in listJ)
                    {
                        i += 1;
                        if (i == listJ.Count)
                            sumJ = sumJ + list;
                        else
                            sumJ = sumJ + list + "+";
                    }
                    ws.Cells("D" + iRow).FormulaA1 = "=" + sumD;
                    ws.Cells("E" + iRow).FormulaA1 = "=" + sumE;
                    ws.Cells("F" + iRow).FormulaA1 = "=" + sumF;
                    ws.Cells("G" + iRow).FormulaA1 = "=" + sumG;
                    ws.Cells("H" + iRow).FormulaA1 = "=" + sumH;
                    ws.Cells("I" + iRow).FormulaA1 = "=" + sumI;
                    ws.Cells("J" + iRow).FormulaA1 = "=" + sumJ;
                }
                ws.Cells("D" + iRow).Style.Font.SetBold(true);
                ws.Cells("E" + iRow).Style.Font.SetBold(true);
                ws.Cells("F" + iRow).Style.Font.SetBold(true);
                ws.Cells("G" + iRow).Style.Font.SetBold(true);
                ws.Cells("H" + iRow).Style.Font.SetBold(true);
                ws.Cells("I" + iRow).Style.Font.SetBold(true);
                ws.Cells("J" + iRow).Style.Font.SetBold(true);

                ws.Cells("D" + iRow).Style.NumberFormat.Format = "#,##0";
                ws.Cells("E" + iRow).Style.NumberFormat.Format = "#,##0";
                ws.Cells("F" + iRow).Style.NumberFormat.Format = "#,##0";
                ws.Cells("G" + iRow).Style.NumberFormat.Format = "#,##0";
                ws.Cells("H" + iRow).Style.NumberFormat.Format = "#,##0";
                ws.Cells("I" + iRow).Style.NumberFormat.Format = "#,##0";
                ws.Cells("J" + iRow).Style.NumberFormat.Format = "#,##0";

                ws.Range("B5", "J8").Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                ws.Range("B5", "J8").Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                ws.Range("B10", "J" + iRow).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                ws.Range("B10", "J" + iRow).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                ws.Cell("C5").Value = dt.Rows[0]["PerDate"];
                ws.Cell("C5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult GenerateITSReportFreshInqHarianDealer(DateTime StartDate,string model,string groupmodel,string variant)
        {

            DateTime now = DateTime.Now;
            DateTime EndDate;

            if(StartDate.Month==now.Month && StartDate.Year==now.Year)
            {
                EndDate = now;
            }
            else
            {
                EndDate = new DateTime(StartDate.Year, StartDate.Month, DateTime.DaysInMonth(StartDate.Year, StartDate.Month));
            }



            int tdays = (EndDate - StartDate).Days + 1;

            if (tdays > 31)
                return Content("<h3>Invalid Date Range</h3>");

            var prevdate=StartDate.AddMonths(-2);
            var prevstart1 = new DateTime(prevdate.Year, prevdate.Month, 1);
            var prevend1 = new DateTime(prevstart1.Year, prevstart1.Month, DateTime.DaysInMonth(prevstart1.Year, prevstart1.Month));
            

            var prevstart2 = prevstart1.AddMonths(1);
            var prevend2 = new DateTime(prevstart2.Year, prevstart2.Month, DateTime.DaysInMonth(prevstart2.Year, prevstart2.Month));
            var tmpdate = new DateTime(StartDate.Year, StartDate.Month, 1);
            var tmpenddate = new DateTime(StartDate.Year, StartDate.Month, DateTime.DaysInMonth( tmpdate.Year,tmpdate.Month));            
            string sdlrtemp = "";
            string sarea = "";
            string ltrInq = "";
            string ltrSPK = "";
            int cdlr = 1;
            int tdlr = -1;
            int lno = 0;
            int itoparea = 0;

            List<int> indxArea=new List<int>();
            List<int> lstSunday=new List<int>();

            while (tmpdate <= tmpenddate)
            {
                if(tmpdate.DayOfWeek.ToString().ToLower()=="sunday")
                {
                   lstSunday.Add(cdlr);
                }
               tmpdate= tmpdate.AddDays(1);
                cdlr++;
            }

            cdlr = 1;

            string fileName = "ITS_Report_Fresh_Inq_Harian_Dealer" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");


            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_GenerateITSReportFreshInqHarianDealer";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@StartDate", StartDate.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@EndDate", EndDate.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@StartDatePrevMonth1", prevstart1.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@EndDatePrevMonth1", prevend1.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@StartDatePrevMonth2", prevstart2.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@EndDatePrevMonth2", prevend2.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@TipeKendaraan", model ?? "");
            cmd.Parameters.AddWithValue("@Variant", variant ?? "");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            DataTable dtprev1 = ds.Tables[1];
            DataTable dtprev2 = ds.Tables[2];
            
            
            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("ITSReport");
            
            //DataView view = new DataView(dt);
            //int tline = view.ToTable(true, "area").Rows.Count; 
            //tline+= view.ToTable(true, "dealercode").Rows.Count;
            //tline += 2;
            //ws.Range("C5", "AM" + tline).AddConditionalFormat().WhenEquals(0).Font.SetFontColor(XLColor.Red);
            //ws.Range("AQ5", "BV" + tline).AddConditionalFormat().WhenEquals(0).Font.SetFontColor(XLColor.Red);
            //ws.Range("BZ5", "DE" + tline).AddConditionalFormat().WhenEquals(0).Font.SetFontColor(XLColor.Red);



            ws.Column("A").Width = 3.29;
            ws.Column("B").Width = 35;
            ws.Column("AO").Width = 3.29;
            ws.Column("AP").Width = 35;
            ws.Column("BX").Width = 3.29;
            ws.Column("BY").Width = 35;
            ws.Columns(7, 37).Width = 5;
            ws.Columns(43, 73).Width = 5;
            ws.Columns(78, 108).Width = 5;



            //Title
            var smodel = string.IsNullOrEmpty(groupmodel) ? "All Model" : groupmodel;
            ws.Cell("A" + recNo).Value = "REPORT FRESH INQUIRY HARIAN DEALER: " + smodel;
            ws.Range("A1", "AM1").Merge().Style.Fill.SetBackgroundColor(XLColor.FromArgb(242, 221, 220));
            ws.Cell("A1").Style.Font.SetFontSize(16);
            ws.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


            ws.Cell("AO" + recNo).Value = "REPORT FRESH SPK HARIAN DEALER: " + smodel; ;
            ws.Range("AO1", "BV1").Merge().Style.Fill.SetBackgroundColor(XLColor.FromArgb(242, 221, 220));
            ws.Cell("AO1").Style.Font.SetFontSize(16);
            ws.Cell("AO1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("BX" + recNo).Value = "SUCCESS RATIO INQ TO SPK: " + smodel; ;
            ws.Range("BX1", "DE1").Merge().Style.Fill.SetBackgroundColor(XLColor.FromArgb(242, 221, 220));
            ws.Cell("BX1").Style.Font.SetFontSize(16);
            ws.Cell("BX1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("A2").Value = "Tanggal: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" ) + " " + CurrentUser.Username;

            recNo += 2;

            //Table Header
            ws.Cell("A" + recNo).Value = "No";
            ws.Range("A"+recNo+":A"+(recNo+1)).Merge();

            ws.Cell("B" + recNo).Value = "DEALER";
            ws.Range("B" + recNo + ":B" + (recNo + 1)).Merge();

            ws.Cell("C" + recNo).Value = "QTY MF";
            ws.Range("C" + recNo + ":C" + (recNo + 1)).Merge();

            ws.Cell("D" + recNo).Value = "WORK DAYS";            
            ws.Range("D" + recNo + ":D" + (recNo + 1)).Merge();
            ws.Cell("D" + recNo).Style.Alignment.WrapText = true;

            ws.Cell("E" + recNo).Value = "TARGET INQ";            
            ws.Range("E" + recNo + ":E" + (recNo + 1)).Merge();
            ws.Cell("E" + recNo).Style.Alignment.WrapText = true;

            ws.Cell("F" + recNo).Value = "Ach Days INQ";
            ws.Range("F" + recNo + ":F" + (recNo + 1)).Merge();
            ws.Cell("F" + recNo).Style.Alignment.WrapText = true;
            
            


            ws.Cell("AL" + recNo).Value = "TOTAL INQ";
            ws.Range("AL" + recNo + ":AL" + (recNo + 1)).Merge();
            ws.Cell("AL" + recNo).Style.Alignment.WrapText = true;

            ws.Cell("AM" + recNo).Value = "ACH Act INQ";
            ws.Range("AM" + recNo + ":AM" + (recNo + 1)).Merge();
            ws.Cell("AM" + recNo).Style.Alignment.WrapText = true;


            ws.Cell("AO" + recNo).Value = "No";
            ws.Range("AO" + recNo + ":AO" + (recNo + 1)).Merge();            
            
            ws.Cell("AP" + recNo).Value = "DEALER";
            ws.Range("AP" + recNo + ":AP" + (recNo + 1)).Merge();
            

            ws.Cell("BV" + recNo).Value = "TOTAL SPK";
            ws.Range("BV" + recNo + ":BV" + (recNo + 1)).Merge();
            ws.Cell("BV" + recNo).Style.Alignment.WrapText = true;

            ws.Cell("BX" + recNo).Value = "No";
            ws.Range("BX" + recNo + ":BX" + (recNo + 1)).Merge();

            ws.Cell("BY" + recNo).Value = "DEALER";
            ws.Range("BY" + recNo + ":BY" + (recNo + 1)).Merge();

            ws.Cell("DE" + recNo).Value = "RASIO";
            ws.Range("DE" + recNo + ":DE" + (recNo + 1)).Merge();

            ws.Range("G" + recNo + ":AK" + recNo).Merge();
            ws.Range("AQ" + recNo + ":BU" + recNo).Merge();
            ws.Range("BZ" + recNo + ":DD" + recNo).Merge();

            ws.Range("A"+recNo+":AM"+(recNo+1)).Style.Fill.SetBackgroundColor(XLColor.FromArgb(184,204,228));
            ws.Range("A" + recNo + ":AM" + (recNo + 1)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);            
            ws.Range("A" + recNo + ":AM" + (recNo + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Range("AO" + recNo + ":BV" + (recNo + 1)).Style.Fill.SetBackgroundColor(XLColor.FromArgb(184, 204, 228));
            ws.Range("AO" + recNo + ":BV" + (recNo + 1)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Range("AO" + recNo + ":BV" + (recNo + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Range("BX" + recNo + ":DE" + (recNo + 1)).Style.Fill.SetBackgroundColor(XLColor.FromArgb(184, 204, 228));
            ws.Range("BX" + recNo + ":DE" + (recNo + 1)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Range("BX" + recNo + ":DE" + (recNo + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);           
            
            recNo++;
            for (int i = 0; i < 31; i++)
            {
                ws.Cell(recNo, i + 7).Value = i + 1;
                ws.Cell(recNo, i + 43).Value = i + 1;
                ws.Cell(recNo, i + 78).Value = i + 1;
            }

          


            Action<string, int, int, int> summary = (area, tdl, itop, rno) =>
            {
                ws.Range("A" + rno, "AM" + rno).Style.Fill.SetBackgroundColor(XLColor.FromArgb(140,242,126));
                ws.Range("AO" + rno, "BV" + rno).Style.Fill.SetBackgroundColor(XLColor.FromArgb(140, 242, 126));
                ws.Range("BX" + rno, "DE" + rno).Style.Fill.SetBackgroundColor(XLColor.FromArgb(140, 242, 126));

                ws.Cell("B" + rno).Value = area + " (" + tdl + ")";
                ws.Cell("AP" + rno).Value = area + " (" + tdl + ")";
                ws.Cell("BY" + rno).Value = area + " (" + tdl + ")";
                indxArea.Add(rno);


                ws.Cell("C" + rno).SetFormulaA1("=sum(C" + itop + ":C" + (rno - 1) + ")");
                ws.Cell("E" + rno).SetFormulaA1( "=sum(E" + itop + ":E" + (rno - 1) + ")");

                ltrInq = ws.Cell(rno, 6 + tdays).WorksheetColumn().ColumnLetter();                
                ws.Cell("F" + rno).SetFormulaA1("=IFERROR("+ltrInq + (rno) + "/C" + (rno) + ",\"0%\")");
                ws.Cell("F" + rno).Style.NumberFormat.Format = "0%";

                ws.Cell("G" + rno).SetFormulaA1("=sum(G" + itop + ":G" + (rno - 1) + ")");
                ws.Cell("H" + rno).SetFormulaA1("=sum(H" + itop + ":H" + (rno - 1) + ")");
                ws.Cell("I" + rno).SetFormulaA1("=sum(I" + itop + ":I" + (rno - 1) + ")");
                ws.Cell("J" + rno).SetFormulaA1("=sum(J" + itop + ":J" + (rno - 1) + ")");
                ws.Cell("K" + rno).SetFormulaA1("=sum(K" + itop + ":K" + (rno - 1) + ")");
                ws.Cell("L" + rno).SetFormulaA1("=sum(L" + itop + ":L" + (rno - 1) + ")");
                ws.Cell("M" + rno).SetFormulaA1("=sum(M" + itop + ":M" + (rno - 1) + ")");
                ws.Cell("N" + rno).SetFormulaA1("=sum(N" + itop + ":N" + (rno - 1) + ")");
                ws.Cell("O" + rno).SetFormulaA1("=sum(O" + itop + ":O" + (rno - 1) + ")");
                ws.Cell("P" + rno).SetFormulaA1("=sum(P" + itop + ":P" + (rno - 1) + ")");
                ws.Cell("Q" + rno).SetFormulaA1("=sum(Q" + itop + ":Q" + (rno - 1) + ")");
                ws.Cell("R" + rno).SetFormulaA1("=sum(R" + itop + ":R" + (rno - 1) + ")");
                ws.Cell("S" + rno).SetFormulaA1("=sum(S" + itop + ":S" + (rno - 1) + ")");
                ws.Cell("T" + rno).SetFormulaA1("=sum(T" + itop + ":T" + (rno - 1) + ")");
                ws.Cell("U" + rno).SetFormulaA1("=sum(U" + itop + ":U" + (rno - 1) + ")");
                ws.Cell("V" + rno).SetFormulaA1("=sum(V" + itop + ":V" + (rno - 1) + ")");
                ws.Cell("W" + rno).SetFormulaA1("=sum(W" + itop + ":W" + (rno - 1) + ")");
                ws.Cell("X" + rno).SetFormulaA1("=sum(X" + itop + ":X" + (rno - 1) + ")");
                ws.Cell("Y" + rno).SetFormulaA1("=sum(Y" + itop + ":Y" + (rno - 1) + ")");
                ws.Cell("Z" + rno).SetFormulaA1("=sum(Z" + itop + ":Z" + (rno - 1) + ")");
                ws.Cell("AA" + rno).SetFormulaA1( "=sum(AA" + itop + ":AA" + (rno - 1) + ")");
                ws.Cell("AB" + rno).SetFormulaA1( "=sum(AB" + itop + ":AB" + (rno - 1) + ")");
                ws.Cell("AC" + rno).SetFormulaA1( "=sum(AC" + itop + ":AC" + (rno - 1) + ")");
                ws.Cell("AD" + rno).SetFormulaA1( "=sum(AD" + itop + ":AD" + (rno - 1) + ")");
                ws.Cell("AE" + rno).SetFormulaA1( "=sum(AE" + itop + ":AE" + (rno - 1) + ")");
                ws.Cell("AF" + rno).SetFormulaA1( "=sum(AF" + itop + ":AF" + (rno - 1) + ")");
                ws.Cell("AG" + rno).SetFormulaA1( "=sum(AG" + itop + ":AG" + (rno - 1) + ")");
                ws.Cell("AH" + rno).SetFormulaA1( "=sum(AH" + itop + ":AH" + (rno - 1) + ")");
                ws.Cell("AI" + rno).SetFormulaA1( "=sum(AI" + itop + ":AI" + (rno - 1) + ")");
                ws.Cell("AJ" + rno).SetFormulaA1( "=sum(AJ" + itop + ":AJ" + (rno - 1) + ")");
                ws.Cell("AK" + rno).SetFormulaA1( "=sum(AK" + itop + ":AK" + (rno - 1) + ")");
                ws.Cell("AL" + rno).SetFormulaA1( "=sum(AL" + itop + ":AL" + (rno - 1) + ")");
                ws.Cell("AM" + rno).SetFormulaA1( "=IFERROR(AL"+(rno)+"/E"+(rno)+",\"0%\")");
                ws.Cell("AM" + rno).Style.NumberFormat.Format = "0%";


                ws.Cell("AQ" + rno).SetFormulaA1(  "=sum(AQ" + itop + ":AQ" + (rno - 1) + ")");
                ws.Cell("AR" + rno).SetFormulaA1(  "=sum(AR" + itop + ":AR" + (rno - 1) + ")");
                ws.Cell("AS" + rno).SetFormulaA1(  "=sum(AS" + itop + ":AS" + (rno - 1) + ")");
                ws.Cell("AT" + rno).SetFormulaA1(  "=sum(AT" + itop + ":AT" + (rno - 1) + ")");
                ws.Cell("AU" + rno).SetFormulaA1(  "=sum(AU" + itop + ":AU" + (rno - 1) + ")");
                ws.Cell("AV" + rno).SetFormulaA1(  "=sum(AV" + itop + ":AV" + (rno - 1) + ")");
                ws.Cell("AW" + rno).SetFormulaA1(  "=sum(AW" + itop + ":AW" + (rno - 1) + ")");
                ws.Cell("AX" + rno).SetFormulaA1(  "=sum(AX" + itop + ":AX" + (rno - 1) + ")");
                ws.Cell("AY" + rno).SetFormulaA1(  "=sum(AY" + itop + ":AY" + (rno - 1) + ")");
                ws.Cell("AZ" + rno).SetFormulaA1(  "=sum(AZ" + itop + ":AZ" + (rno - 1) + ")");
                ws.Cell("BA" + rno).SetFormulaA1(  "=sum(BA" + itop + ":BA" + (rno - 1) + ")");
                ws.Cell("BB" + rno).SetFormulaA1(  "=sum(BB" + itop + ":BB" + (rno - 1) + ")");
                ws.Cell("BC" + rno).SetFormulaA1(  "=sum(BC" + itop + ":BC" + (rno - 1) + ")");
                ws.Cell("BD" + rno).SetFormulaA1(  "=sum(BD" + itop + ":BD" + (rno - 1) + ")");
                ws.Cell("BE" + rno).SetFormulaA1(  "=sum(BE" + itop + ":BE" + (rno - 1) + ")");
                ws.Cell("BF" + rno).SetFormulaA1(  "=sum(BF" + itop + ":BF" + (rno - 1) + ")");
                ws.Cell("BG" + rno).SetFormulaA1(  "=sum(BG" + itop + ":BG" + (rno - 1) + ")");
                ws.Cell("BH" + rno).SetFormulaA1(  "=sum(BH" + itop + ":BH" + (rno - 1) + ")");
                ws.Cell("BI" + rno).SetFormulaA1(  "=sum(BI" + itop + ":BI" + (rno - 1) + ")");
                ws.Cell("BJ" + rno).SetFormulaA1(  "=sum(BJ" + itop + ":BJ" + (rno - 1) + ")");
                ws.Cell("BK" + rno).SetFormulaA1(  "=sum(BK" + itop + ":BK" + (rno - 1) + ")");
                ws.Cell("BL" + rno).SetFormulaA1(  "=sum(BL" + itop + ":BL" + (rno - 1) + ")");
                ws.Cell("BM" + rno).SetFormulaA1(  "=sum(BM" + itop + ":BM" + (rno - 1) + ")");
                ws.Cell("BN" + rno).SetFormulaA1(  "=sum(BN" + itop + ":BN" + (rno - 1) + ")");
                ws.Cell("BO" + rno).SetFormulaA1(  "=sum(BO" + itop + ":BO" + (rno - 1) + ")");
                ws.Cell("BP" + rno).SetFormulaA1(  "=sum(BP" + itop + ":BP" + (rno - 1) + ")");
                ws.Cell("BQ" + rno).SetFormulaA1(  "=sum(BQ" + itop + ":BQ" + (rno - 1) + ")");
                ws.Cell("BR" + rno).SetFormulaA1(  "=sum(BR" + itop + ":BR" + (rno - 1) + ")");
                ws.Cell("BS" + rno).SetFormulaA1(  "=sum(BS" + itop + ":BS" + (rno - 1) + ")");
                ws.Cell("BT" + rno).SetFormulaA1(  "=sum(BT" + itop + ":BT" + (rno - 1) + ")");
                ws.Cell("BU" + rno).SetFormulaA1(  "=sum(BU" + itop + ":BU" + (rno - 1) + ")");
                ws.Cell("BV" + rno).SetFormulaA1(  "=sum(BV" + itop + ":BV" + (rno - 1) + ")");

                

                ws.Cell("BZ" + rno).SetFormulaA1("=IFERROR(AQ" + rno + "/G" + rno + ",\"0%\")");
                ws.Cell("CA" + rno).SetFormulaA1("=IFERROR(AR" + rno + "/H" + rno + ",\"0%\")");
                ws.Cell("CB" + rno).SetFormulaA1("=IFERROR(AS" + rno + "/I" + rno + ",\"0%\")");
                ws.Cell("CC" + rno).SetFormulaA1("=IFERROR(AT" + rno + "/J" + rno + ",\"0%\")");
                ws.Cell("CD" + rno).SetFormulaA1("=IFERROR(AU" + rno + "/K" + rno + ",\"0%\")");
                ws.Cell("CE" + rno).SetFormulaA1("=IFERROR(AV" + rno + "/L" + rno + ",\"0%\")");
                ws.Cell("CF" + rno).SetFormulaA1("=IFERROR(AW" + rno + "/M" + rno + ",\"0%\")");
                ws.Cell("CG" + rno).SetFormulaA1("=IFERROR(AX" + rno + "/N" + rno + ",\"0%\")");
                ws.Cell("CH" + rno).SetFormulaA1("=IFERROR(AY" + rno + "/O" + rno + ",\"0%\")");
                ws.Cell("CI" + rno).SetFormulaA1("=IFERROR(AZ" + rno + "/P" + rno + ",\"0%\")");
                ws.Cell("CJ" + rno).SetFormulaA1("=IFERROR(BA" + rno + "/Q" + rno + ",\"0%\")");
                ws.Cell("CK" + rno).SetFormulaA1("=IFERROR(BB" + rno + "/R" + rno + ",\"0%\")");
                ws.Cell("CL" + rno).SetFormulaA1("=IFERROR(BC" + rno + "/S" + rno + ",\"0%\")");
                ws.Cell("CM" + rno).SetFormulaA1("=IFERROR(BD" + rno + "/T" + rno + ",\"0%\")");
                ws.Cell("CN" + rno).SetFormulaA1("=IFERROR(BE" + rno + "/U" + rno + ",\"0%\")");
                ws.Cell("CO" + rno).SetFormulaA1("=IFERROR(BF" + rno + "/V" + rno + ",\"0%\")");
                ws.Cell("CP" + rno).SetFormulaA1("=IFERROR(BG" + rno + "/W" + rno + ",\"0%\")");
                ws.Cell("CQ" + rno).SetFormulaA1("=IFERROR(BH" + rno + "/X" + rno + ",\"0%\")");
                ws.Cell("CR" + rno).SetFormulaA1("=IFERROR(BI" + rno + "/Y" + rno + ",\"0%\")");
                ws.Cell("CS" + rno).SetFormulaA1("=IFERROR(BJ" + rno + "/Z" + rno + ",\"0%\")");
                ws.Cell("CT" + rno).SetFormulaA1("=IFERROR(BK" + rno + "/AA" + rno + ",\"0%\")");
                ws.Cell("CU" + rno).SetFormulaA1("=IFERROR(BL" + rno + "/AB" + rno + ",\"0%\")");
                ws.Cell("CV" + rno).SetFormulaA1("=IFERROR(BM" + rno + "/AC" + rno + ",\"0%\")");
                ws.Cell("CW" + rno).SetFormulaA1("=IFERROR(BN" + rno + "/AD" + rno + ",\"0%\")");
                ws.Cell("CX" + rno).SetFormulaA1("=IFERROR(BO" + rno + "/AE" + rno + ",\"0%\")");
                ws.Cell("CY" + rno).SetFormulaA1("=IFERROR(BP" + rno + "/AF" + rno + ",\"0%\")");
                ws.Cell("CZ" + rno).SetFormulaA1("=IFERROR(BQ" + rno + "/AG" + rno + ",\"0%\")");
                ws.Cell("DA" + rno).SetFormulaA1("=IFERROR(BR" + rno + "/AH" + rno + ",\"0%\")");
                ws.Cell("DB" + rno).SetFormulaA1("=IFERROR(BS" + rno + "/AI" + rno + ",\"0%\")");
                ws.Cell("DC" + rno).SetFormulaA1("=IFERROR(BT" + rno + "/AJ" + rno + ",\"0%\")");
                ws.Cell("DD" + rno).SetFormulaA1("=IFERROR(BU" + rno + "/AK" + rno + ",\"0%\")");
                ws.Cell("DE" + rno).SetFormulaA1("=IFERROR(BV" + rno + "/AL" + rno + ",\"0%\")");
                
                ws.Range("BZ"+rno,"DE"+rno).Style.NumberFormat.Format = "0%";
                                                                         
            };
                                                       
            foreach (DataRow row in dt.Rows)                             
            {
                var sdrdt = row["InqDate"].ToString();
                var cdate = DateTime.Parse( sdrdt.Substring(0, 4)+ "-"+sdrdt.Substring(4,2)+"-"+sdrdt.Substring(6,2));
                //if(cdate.DayOfWeek)
          
                if (sdlrtemp != row["dealercode"].ToString())
                {
                    recNo++;
                    lno++;
                    tdlr++;
                    if (lno > 1 && sarea != row["area"].ToString())
                    {
                        summary(sarea, tdlr, itoparea, recNo);
                        sarea = row["area"].ToString();
                        tdlr = 0;
                        recNo++;
                        itoparea = recNo;
                    }
                    else
                    {
                        if (sarea == "")
                            itoparea = recNo;
                    }

                    cdlr = 1;

                    ws.Cell("A"+recNo).Value = lno;
                    ws.Cell("B"+recNo).Value = row["dealername"].ToString();
                    ws.Cell("C" + recNo).Value = row["QtySF"].ToString();
                    ws.Cell("D" + recNo).Value = tdays;
                    ws.Cell("E" + recNo).Value = (Int32.Parse(row["QtySF"].ToString()) * tdays).ToString();
                    
                    string cl= ws.Cell(recNo, 6 + tdays).WorksheetColumn().ColumnLetter();
                    ws.Cell("F" + recNo).SetFormulaA1("=IFERROR(" + cl + recNo + "/C" + recNo + ",\"0%\")");
                    ws.Cell("F" + recNo).Style.NumberFormat.Format = "0%"; 

                    ws.Cell("AL" + recNo).SetFormulaA1("=sum(G" + recNo + ":AK" + recNo + ")");
                    ws.Cell("AM" + recNo).SetFormulaA1("=IFERROR(AL"+recNo+"/E"+recNo+",\"0%\")");
                    ws.Cell("AM" + recNo).Style.NumberFormat.Format = "0%"; 
                    
                    ws.Cell("AO"+recNo).Value = lno;
                    ws.Cell("AP"+recNo).Value = row["dealername"].ToString();
                    ws.Cell("BV" + recNo).SetFormulaA1("=sum(AQ" + recNo + ":BU" + recNo + ")");

                    ws.Cell("BX"+recNo).Value = lno;
                    ws.Cell("BY"+recNo).Value = row["dealername"].ToString();
                    ws.Cell("DE" + recNo).SetFormulaA1("IFERROR(BV" + recNo + "/AL" + recNo + ",\"0%\")");
                    ws.Cell("DE" + recNo).Style.NumberFormat.Format = "0%"; 

                    ws.Cell(recNo, cdlr + 6).Value = row["INQ"];
                    ws.Cell(recNo, cdlr + 42).Value = row["SPK"];

                    
                    ltrInq = ws.Cell(recNo, 6 + cdlr).WorksheetColumn().ColumnLetter();
                    ltrSPK = ws.Cell(recNo, 42 + cdlr).WorksheetColumn().ColumnLetter();
                    ws.Cell(recNo, cdlr + 77).SetFormulaA1("=IFERROR(" + ltrSPK + recNo + "/" + ltrInq + recNo + ",\"0%\")");
                    ws.Cell(recNo, cdlr + 77).Style.NumberFormat.Format = "0%";                    

                    sdlrtemp = row["dealercode"].ToString();
                    sarea = row["area"].ToString();

                }
                else
                {
                   
                    ws.Cell(recNo, cdlr + 7).Value = row["INQ"];
                    ws.Cell(recNo, cdlr + 43).Value = row["SPK"];                    
                    ltrInq = ws.Cell(recNo, 7 + cdlr).WorksheetColumn().ColumnLetter();
                    ltrSPK = ws.Cell(recNo, 43 + cdlr).WorksheetColumn().ColumnLetter();
                    ws.Cell(recNo, cdlr + 78).SetFormulaA1("=IFERROR("+ltrSPK+recNo+"/"+ltrInq+recNo+",\"0%\")");
                    ws.Cell(recNo, cdlr + 78).Style.NumberFormat.Format = "0%";                  

                    cdlr++;
                }

            }
            summary(sarea, tdlr + 1, itoparea, recNo + 1);


            recNo += 2;            

            ws.Cell("B" + recNo).Value = "Total "+ StartDate.ToString("MMMM yyyy");
            ws.Cell("AP" + recNo).Value = "Total " + StartDate.ToString("MMMM yyyy");
            ws.Cell("BY" + recNo).Value = "Total " + StartDate.ToString("MMMM yyyy");

            ws.Cell("C" + recNo).SetFormulaA1("=C" + string.Join("+C", indxArea.ToArray()));
            ws.Cell("E" + recNo).SetFormulaA1("=E" + string.Join("+E", indxArea.ToArray()));
            ltrInq = ws.Cell(recNo, 6 + tdays).WorksheetColumn().ColumnLetter();
            ws.Cell("F" + recNo).SetFormulaA1("=IFERROR(" + ltrInq + (recNo) + "/C" + (recNo) + ",\"0%\")");
            ws.Cell("F" + recNo).Style.NumberFormat.Format = "0%";  


            ws.Cell("G" + recNo).SetFormulaA1("=G" + string.Join("+G", indxArea.ToArray()));
            ws.Cell("H" + recNo).SetFormulaA1("=H" + string.Join("+H", indxArea.ToArray()));
            ws.Cell("I" + recNo).SetFormulaA1("=I" + string.Join("+I", indxArea.ToArray()));
            ws.Cell("J" + recNo).SetFormulaA1("=J" + string.Join("+J", indxArea.ToArray()));
            ws.Cell("K" + recNo).SetFormulaA1("=K" + string.Join("+K", indxArea.ToArray()));
            ws.Cell("L" + recNo).SetFormulaA1("=L" + string.Join("+L", indxArea.ToArray()));
            ws.Cell("M" + recNo).SetFormulaA1("=M" + string.Join("+M", indxArea.ToArray()));
            ws.Cell("N" + recNo).SetFormulaA1("=N" + string.Join("+N", indxArea.ToArray()));
            ws.Cell("O" + recNo).SetFormulaA1("=O" + string.Join("+O", indxArea.ToArray()));
            ws.Cell("P" + recNo).SetFormulaA1("=P" + string.Join("+P", indxArea.ToArray()));
            ws.Cell("Q" + recNo).SetFormulaA1("=Q" + string.Join("+Q", indxArea.ToArray()));
            ws.Cell("R" + recNo).SetFormulaA1("=R" + string.Join("+R", indxArea.ToArray()));
            ws.Cell("S" + recNo).SetFormulaA1("=S" + string.Join("+S", indxArea.ToArray()));
            ws.Cell("T" + recNo).SetFormulaA1("=T" + string.Join("+T", indxArea.ToArray()));
            ws.Cell("U" + recNo).SetFormulaA1("=U" + string.Join("+U", indxArea.ToArray()));
            ws.Cell("V" + recNo).SetFormulaA1("=V" + string.Join("+V", indxArea.ToArray()));
            ws.Cell("W" + recNo).SetFormulaA1("=W" + string.Join("+W", indxArea.ToArray()));
            ws.Cell("X" + recNo).SetFormulaA1("=X" + string.Join("+X", indxArea.ToArray()));
            ws.Cell("Y" + recNo).SetFormulaA1("=Y" + string.Join("+Y", indxArea.ToArray()));
            ws.Cell("Z" + recNo).SetFormulaA1("=Z" + string.Join("+Z", indxArea.ToArray()));
            ws.Cell("AA" + recNo).SetFormulaA1("=AA" + string.Join("+AA", indxArea.ToArray()));
            ws.Cell("AB" + recNo).SetFormulaA1("=AB" + string.Join("+AB", indxArea.ToArray()));
            ws.Cell("AC" + recNo).SetFormulaA1("=AC" + string.Join("+AC", indxArea.ToArray()));
            ws.Cell("AD" + recNo).SetFormulaA1("=AD" + string.Join("+AD", indxArea.ToArray()));
            ws.Cell("AE" + recNo).SetFormulaA1("=AE" + string.Join("+AE", indxArea.ToArray()));
            ws.Cell("AF" + recNo).SetFormulaA1("=AF" + string.Join("+AF", indxArea.ToArray()));
            ws.Cell("AG" + recNo).SetFormulaA1("=AG" + string.Join("+AG", indxArea.ToArray()));
            ws.Cell("AH" + recNo).SetFormulaA1("=AH" + string.Join("+AH", indxArea.ToArray()));
            ws.Cell("AI" + recNo).SetFormulaA1("=AI" + string.Join("+AI", indxArea.ToArray()));
            ws.Cell("AJ" + recNo).SetFormulaA1("=AJ" + string.Join("+AJ", indxArea.ToArray()));
            ws.Cell("AK" + recNo).SetFormulaA1("=AK" + string.Join("+AK", indxArea.ToArray()));
            ws.Cell("AL" + recNo).SetFormulaA1("=AL" + string.Join("+AL", indxArea.ToArray()));
            ws.Cell("AM" + recNo).SetFormulaA1("=AM" + string.Join("+AM", indxArea.ToArray()));
            ws.Cell("AM" + recNo).SetFormulaA1("=AM" + string.Join("+AM", indxArea.ToArray()));


            ws.Cell("AQ" + recNo).SetFormulaA1("=AQ" + string.Join("+AQ", indxArea.ToArray()));
            ws.Cell("AR" + recNo).SetFormulaA1("=AR" + string.Join("+AR", indxArea.ToArray()));
            ws.Cell("AS" + recNo).SetFormulaA1("=AS" + string.Join("+AS", indxArea.ToArray()));
            ws.Cell("AT" + recNo).SetFormulaA1("=AT" + string.Join("+AT", indxArea.ToArray()));
            ws.Cell("AU" + recNo).SetFormulaA1("=AU" + string.Join("+AU", indxArea.ToArray()));
            ws.Cell("AV" + recNo).SetFormulaA1("=AV" + string.Join("+AV", indxArea.ToArray()));
            ws.Cell("AW" + recNo).SetFormulaA1("=AW" + string.Join("+AW", indxArea.ToArray()));
            ws.Cell("AX" + recNo).SetFormulaA1("=AX" + string.Join("+AX", indxArea.ToArray()));
            ws.Cell("AY" + recNo).SetFormulaA1("=AY" + string.Join("+AY", indxArea.ToArray()));
            ws.Cell("AZ" + recNo).SetFormulaA1("=AZ" + string.Join("+AZ", indxArea.ToArray()));
            ws.Cell("BA" + recNo).SetFormulaA1("=BA" + string.Join("+BA", indxArea.ToArray()));
            ws.Cell("BB" + recNo).SetFormulaA1("=BB" + string.Join("+BB", indxArea.ToArray()));
            ws.Cell("BC" + recNo).SetFormulaA1("=BC" + string.Join("+BC", indxArea.ToArray()));
            ws.Cell("BD" + recNo).SetFormulaA1("=BD" + string.Join("+BD", indxArea.ToArray()));
            ws.Cell("BE" + recNo).SetFormulaA1("=BE" + string.Join("+BE", indxArea.ToArray()));
            ws.Cell("BF" + recNo).SetFormulaA1("=BF" + string.Join("+BF", indxArea.ToArray()));
            ws.Cell("BG" + recNo).SetFormulaA1("=BG" + string.Join("+BG", indxArea.ToArray()));
            ws.Cell("BH" + recNo).SetFormulaA1("=BH" + string.Join("+BH", indxArea.ToArray()));
            ws.Cell("BI" + recNo).SetFormulaA1("=BI" + string.Join("+BI", indxArea.ToArray()));
            ws.Cell("BJ" + recNo).SetFormulaA1("=BJ" + string.Join("+BJ", indxArea.ToArray()));
            ws.Cell("BK" + recNo).SetFormulaA1("=BK" + string.Join("+BK", indxArea.ToArray()));
            ws.Cell("BL" + recNo).SetFormulaA1("=BL" + string.Join("+BL", indxArea.ToArray()));
            ws.Cell("BM" + recNo).SetFormulaA1("=BM" + string.Join("+BM", indxArea.ToArray()));
            ws.Cell("BN" + recNo).SetFormulaA1("=BN" + string.Join("+BN", indxArea.ToArray()));
            ws.Cell("BO" + recNo).SetFormulaA1("=BO" + string.Join("+BO", indxArea.ToArray()));
            ws.Cell("BP" + recNo).SetFormulaA1("=BP" + string.Join("+BP", indxArea.ToArray()));
            ws.Cell("BQ" + recNo).SetFormulaA1("=BQ" + string.Join("+BQ", indxArea.ToArray()));
            ws.Cell("BR" + recNo).SetFormulaA1("=BR" + string.Join("+BR", indxArea.ToArray()));
            ws.Cell("BS" + recNo).SetFormulaA1("=BS" + string.Join("+BS", indxArea.ToArray()));
            ws.Cell("BT" + recNo).SetFormulaA1("=BT" + string.Join("+BT", indxArea.ToArray()));
            ws.Cell("BU" + recNo).SetFormulaA1("=BU" + string.Join("+BU", indxArea.ToArray()));
            ws.Cell("BV" + recNo).SetFormulaA1("=BV" + string.Join("+BV", indxArea.ToArray()));

            for (int u = 0;u<=2; u++ )
            {
                ws.Cell("BZ" + (recNo + u)).SetFormulaA1("=IFERROR(AQ" + (recNo + u) + "/G" + (recNo + u) + ",\"0%\")");
                ws.Cell("CA" + (recNo + u)).SetFormulaA1("=IFERROR(AR" + (recNo + u) + "/H" + (recNo + u) + ",\"0%\")");
                ws.Cell("CB" + (recNo + u)).SetFormulaA1("=IFERROR(AS" + (recNo + u) + "/I" + (recNo + u) + ",\"0%\")");
                ws.Cell("CC" + (recNo + u)).SetFormulaA1("=IFERROR(AT" + (recNo + u) + "/J" + (recNo + u) + ",\"0%\")");
                ws.Cell("CD" + (recNo + u)).SetFormulaA1("=IFERROR(AU" + (recNo + u) + "/K" + (recNo + u) + ",\"0%\")");
                ws.Cell("CE" + (recNo + u)).SetFormulaA1("=IFERROR(AV" + (recNo + u) + "/L" + (recNo + u) + ",\"0%\")");
                ws.Cell("CF" + (recNo + u)).SetFormulaA1("=IFERROR(AW" + (recNo + u) + "/M" + (recNo + u) + ",\"0%\")");
                ws.Cell("CG" + (recNo + u)).SetFormulaA1("=IFERROR(AX" + (recNo + u) + "/N" + (recNo + u) + ",\"0%\")");
                ws.Cell("CH" + (recNo + u)).SetFormulaA1("=IFERROR(AY" + (recNo + u) + "/O" + (recNo + u) + ",\"0%\")");
                ws.Cell("CI" + (recNo + u)).SetFormulaA1("=IFERROR(AZ" + (recNo + u) + "/P" + (recNo + u) + ",\"0%\")");
                ws.Cell("CJ" + (recNo + u)).SetFormulaA1("=IFERROR(BA" + (recNo + u) + "/Q" + (recNo + u) + ",\"0%\")");
                ws.Cell("CK" + (recNo + u)).SetFormulaA1("=IFERROR(BB" + (recNo + u) + "/R" + (recNo + u) + ",\"0%\")");
                ws.Cell("CL" + (recNo + u)).SetFormulaA1("=IFERROR(BC" + (recNo + u) + "/S" + (recNo + u) + ",\"0%\")");
                ws.Cell("CM" + (recNo + u)).SetFormulaA1("=IFERROR(BD" + (recNo + u) + "/T" + (recNo + u) + ",\"0%\")");
                ws.Cell("CN" + (recNo + u)).SetFormulaA1("=IFERROR(BE" + (recNo + u) + "/U" + (recNo + u) + ",\"0%\")");
                ws.Cell("CO" + (recNo + u)).SetFormulaA1("=IFERROR(BF" + (recNo + u) + "/V" + (recNo + u) + ",\"0%\")");
                ws.Cell("CP" + (recNo + u)).SetFormulaA1("=IFERROR(BG" + (recNo + u) + "/W" + (recNo + u) + ",\"0%\")");
                ws.Cell("CQ" + (recNo + u)).SetFormulaA1("=IFERROR(BH" + (recNo + u) + "/X" + (recNo + u) + ",\"0%\")");
                ws.Cell("CR" + (recNo + u)).SetFormulaA1("=IFERROR(BI" + (recNo + u) + "/Y" + (recNo + u) + ",\"0%\")");
                ws.Cell("CS" + (recNo + u)).SetFormulaA1("=IFERROR(BJ" + (recNo + u) + "/Z" + (recNo + u) + ",\"0%\")");
                ws.Cell("CT" + (recNo + u)).SetFormulaA1("=IFERROR(BK" + (recNo + u) + "/AA" + (recNo + u) + ",\"0%\")");
                ws.Cell("CU" + (recNo + u)).SetFormulaA1("=IFERROR(BL" + (recNo + u) + "/AB" + (recNo + u) + ",\"0%\")");
                ws.Cell("CV" + (recNo + u)).SetFormulaA1("=IFERROR(BM" + (recNo + u) + "/AC" + (recNo + u) + ",\"0%\")");
                ws.Cell("CW" + (recNo + u)).SetFormulaA1("=IFERROR(BN" + (recNo + u) + "/AD" + (recNo + u) + ",\"0%\")");
                ws.Cell("CX" + (recNo + u)).SetFormulaA1("=IFERROR(BO" + (recNo + u) + "/AE" + (recNo + u) + ",\"0%\")");
                ws.Cell("CY" + (recNo + u)).SetFormulaA1("=IFERROR(BP" + (recNo + u) + "/AF" + (recNo + u) + ",\"0%\")");
                ws.Cell("CZ" + (recNo + u)).SetFormulaA1("=IFERROR(BQ" + (recNo + u) + "/AG" + (recNo + u) + ",\"0%\")");
                ws.Cell("DA" + (recNo + u)).SetFormulaA1("=IFERROR(BR" + (recNo + u) + "/AH" + (recNo + u) + ",\"0%\")");
                ws.Cell("DB" + (recNo + u)).SetFormulaA1("=IFERROR(BS" + (recNo + u) + "/AI" + (recNo + u) + ",\"0%\")");
                ws.Cell("DC" + (recNo + u)).SetFormulaA1("=IFERROR(BT" + (recNo + u) + "/AJ" + (recNo + u) + ",\"0%\")");
                ws.Cell("DD" + (recNo + u)).SetFormulaA1("=IFERROR(BU" + (recNo + u) + "/AK" + (recNo + u) + ",\"0%\")");
                ws.Cell("DE" + (recNo + u)).SetFormulaA1("=IFERROR(BV" + (recNo + u) + "/AL" + (recNo + u) + ",\"0%\")");                 
            }
                                                          
                                                               


            ws.Range("A" + recNo, "AM" + recNo).Style.Fill.SetBackgroundColor(XLColor.FromArgb(140, 242, 126));
            ws.Range("AO" + recNo, "BV" + recNo).Style.Fill.SetBackgroundColor(XLColor.FromArgb(140, 242, 126));
            ws.Range("BX" + recNo, "DE" + recNo).Style.Fill.SetBackgroundColor(XLColor.FromArgb(140, 242, 126));
            ws.Range("BZ" + recNo, "DE" + (recNo + 3)).Style.NumberFormat.Format = "0%";  
            recNo++;
            ws.Cell("B" + recNo).Value = "Total "+prevstart2.ToString("MMMM yyyy");            
            ws.Cell("AP" + recNo).Value = "Total " + prevstart2.ToString("MMMM yyyy");
            ws.Cell("BY" + recNo).Value = "Total " + prevstart2.ToString("MMMM yyyy");

            ws.Range("A" + recNo, "AM" + (recNo+1 )).Style.Fill.SetBackgroundColor(XLColor.FromArgb(146, 208, 80));
            ws.Range("AO" + recNo, "BV" + (recNo+1)).Style.Fill.SetBackgroundColor(XLColor.FromArgb(146, 208, 80));
            ws.Range("BX" + recNo, "DE" + (recNo+1)).Style.Fill.SetBackgroundColor(XLColor.FromArgb(146, 208, 80));


            int ii = 0;
            foreach(DataRow dr  in dtprev1.Rows)
            {
                ws.Cell(recNo, ii+7).Value = dr["INQ"];
                ws.Cell(recNo , ii + 43).Value = dr["SPK"];                
                if((DateTime.Parse(dr["inqDate"].ToString().Substring(0,4)+"-"+dr["inqDate"].ToString().Substring(4,2)+"-"+dr["inqDate"].ToString().Substring(6,2)))
                    .DayOfWeek.ToString().ToLower()=="sunday")
                {
                    ws.Cell(recNo, ii + 7).Style.Fill.SetBackgroundColor(XLColor.FromArgb(128, 128, 128));
                    ws.Cell(recNo, ii + 43).Style.Fill.SetBackgroundColor(XLColor.FromArgb(128, 128, 128));
                    ws.Cell(recNo, ii + 78).Style.Fill.SetBackgroundColor(XLColor.FromArgb(128, 128, 128));
                }                
                ii++;
                if (ii  == tdays)
                    break;
            }
            ws.Cell("AL" + recNo).SetFormulaA1(string.Format("=SUM(G{0}:AK{0})",recNo));
            ws.Cell("BV" + recNo).SetFormulaA1(string.Format("=SUM(AQ{0}:BU{0})", recNo));
            


            ii = 0;
            recNo++;
            ws.Cell("B" + recNo).Value = "Total " + prevstart1.ToString("MMMM yyyy");
            ws.Cell("AP" + recNo).Value = "Total " + prevstart1.ToString("MMMM yyyy");
            ws.Cell("BY" + recNo).Value = "Total " + prevstart1.ToString("MMMM yyyy");
            foreach (DataRow dr in dtprev2.Rows)
            {
                
                ws.Cell(recNo, ii + 7).Value = dr["INQ"];
                ws.Cell(recNo , ii + 43).Value = dr["SPK"];
                if ((DateTime.Parse(dr["inqDate"].ToString().Substring(0, 4) + "-" + dr["inqDate"].ToString().Substring(4, 2) + "-" + dr["inqDate"].ToString().Substring(6, 2)))
                    .DayOfWeek.ToString().ToLower() == "sunday")
                {
                    ws.Cell(recNo, ii + 7).Style.Fill.SetBackgroundColor(XLColor.FromArgb(128, 128, 128));
                    ws.Cell(recNo, ii + 43).Style.Fill.SetBackgroundColor(XLColor.FromArgb(128, 128, 128));
                    ws.Cell(recNo, ii + 78).Style.Fill.SetBackgroundColor(XLColor.FromArgb(128, 128, 128));
                }
                ii++;
                if (ii == tdays)
                    break;
            }

            ws.Cell("AL" + recNo).SetFormulaA1(string.Format("=SUM(G{0}:AK{0})",recNo));
            ws.Cell("BV" + recNo).SetFormulaA1(string.Format("=SUM(AQ{0}:BU{0})",recNo));
           
            
            ws.Range("C3", "AM" + recNo).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Range("AQ3", "BV" + recNo).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Range("BZ3", "DE" + recNo).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Range("A3", "AM" + recNo).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            ws.Range("A3", "AM" + recNo).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            ws.Range("AO3", "BV" + recNo).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            ws.Range("AO3", "BV" + recNo).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            ws.Range("BX3", "DE" + recNo).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            ws.Range("BX3", "DE" + recNo).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);


            foreach(int isndy in lstSunday)
            {
                ws.Range(5,isndy+6, recNo-2,isndy+6).Style.Fill.SetBackgroundColor(XLColor.FromArgb(128, 128, 128));
                ws.Range(5, isndy + 42, recNo - 2, isndy + 42).Style.Fill.SetBackgroundColor(XLColor.FromArgb(128, 128, 128));
                ws.Range(5,isndy+77, recNo-2,isndy+77).Style.Fill.SetBackgroundColor(XLColor.FromArgb(128, 128, 128));
            }

           


            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }


        public ActionResult GenerateITSIndent(DateTime Date)
        {
            DateTime now = DateTime.Now;
            string fileName = "GenerateITSIndent_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var data = ctxR4.Database.SqlQuery<GenerateITSIndentModel>(string.Format("exec uspfn_GenerateITSIndent '{0}'", Date.ToString("yyyyMMdd")));

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_GenerateITSIndent";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@Date", Date.ToString("yyyyMMdd"));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            DataTable dt2 = ds.Tables[1];

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Duplicate Data");
            var ws2 = wb.Worksheets.Add("Non Duplicate Data");

            foreach (var worksheet in wb.Worksheets)
            {
                //First Names
                worksheet.Cell("A" + recNo).Value = "DealerAbbreviation";
                worksheet.Cell("B" + recNo).Value = "OutletAbbreviation";
                worksheet.Cell("C" + recNo).Value = "CompanyCode";
                worksheet.Cell("D" + recNo).Value = "BranchCode";
                worksheet.Cell("E" + recNo).Value = "InquiryNumber";
                worksheet.Cell("F" + recNo).Value = "InquiryDate";
                worksheet.Cell("G" + recNo).Value = "OutletID";
                worksheet.Cell("H" + recNo).Value = "BranchHead";
                worksheet.Cell("I" + recNo).Value = "SalesHead";
                worksheet.Cell("J" + recNo).Value = "SalesCoordinator";
                worksheet.Cell("K" + recNo).Value = "Wiraniaga";
                worksheet.Cell("L" + recNo).Value = "StatusProspek";
                worksheet.Cell("M" + recNo).Value = "PerolehanData";
                worksheet.Cell("N" + recNo).Value = "NamaProspek";
                worksheet.Cell("O" + recNo).Value = "AlamatProspek";
                worksheet.Cell("P" + recNo).Value = "TelpRumah";
                worksheet.Cell("Q" + recNo).Value = "City";
                worksheet.Cell("R" + recNo).Value = "NamaPerusahaan";
                worksheet.Cell("S" + recNo).Value = "AlamatPerusahaan";
                worksheet.Cell("T" + recNo).Value = "Jabatan";
                worksheet.Cell("U" + recNo).Value = "Handphone";
                worksheet.Cell("V" + recNo).Value = "Faximile";
                worksheet.Cell("W" + recNo).Value = "Email";
                worksheet.Cell("X" + recNo).Value = "TipeKendaraan";
                worksheet.Cell("Y" + recNo).Value = "Variant";
                worksheet.Cell("Z" + recNo).Value = "Transmisi";
                worksheet.Cell("AA" + recNo).Value = "ColourCode";
                worksheet.Cell("AB" + recNo).Value = "ColourDescription";
                worksheet.Cell("AC" + recNo).Value = "CaraPembayaran";
                worksheet.Cell("AD" + recNo).Value = "TestDrive";
                worksheet.Cell("AE" + recNo).Value = "QuantityInquiry";
                worksheet.Cell("AF" + recNo).Value = "LastProgress";
                worksheet.Cell("AG" + recNo).Value = "LastUpdateStatus";
                worksheet.Cell("AH" + recNo).Value = "ProspectDate";
                worksheet.Cell("AI" + recNo).Value = "HotDate";
                worksheet.Cell("AJ" + recNo).Value = "SPKDate";
                worksheet.Cell("AK" + recNo).Value = "DeliveryDate";
                worksheet.Cell("AL" + recNo).Value = "Leasing";
                worksheet.Cell("AM" + recNo).Value = "DownPayment";
                worksheet.Cell("AN" + recNo).Value = "Tenor";
                worksheet.Cell("AO" + recNo).Value = "LostCaseDate";
                worksheet.Cell("AP" + recNo).Value = "LostCaseCategory";
                worksheet.Cell("AQ" + recNo).Value = "LostCaseReasonID";
                worksheet.Cell("AR" + recNo).Value = "LostCaseOtherReason";
                worksheet.Cell("AS" + recNo).Value = "LostCaseVoiceOfCustomer";
                worksheet.Cell("AT" + recNo).Value = "MerkLain";
                worksheet.Cell("AU" + recNo).Value = "CreatedBy";
                worksheet.Cell("AV" + recNo).Value = "CreatedDate";
                worksheet.Cell("AW" + recNo).Value = "LastUpdateBy";
                worksheet.Cell("AX" + recNo).Value = "LastUpdateDate";
            }
            recNo++;

            int cSheet = 1;
            int loop = -1;
            foreach (DataTable dataTables in ds.Tables)
            {
                loop += 1;
                if (loop > 1) break;
                recNo = 2;
                var worksheet = wb.Worksheet(cSheet);
                foreach (DataRow row in dataTables.Rows)
                {
                    worksheet.Cell("A" + recNo).Value = row["DealerAbbreviation"].ToString();
                    worksheet.Cell("B" + recNo).Value = row["OutletAbbreviation"].ToString();
                    worksheet.Cell("C" + recNo).Value = row["CompanyCode"].ToString();
                    worksheet.Cell("D" + recNo).Value = row["BranchCode"].ToString();
                    worksheet.Cell("E" + recNo).Value = row["InquiryNumber"].ToString();
                    worksheet.Cell("F" + recNo).Value = (row["InquiryDate"] is DBNull) ? "1900/01/01" : Convert.ToDateTime(row["InquiryDate"]).ToString("yyyy/MM/dd");
                    worksheet.Cell("G" + recNo).Value = row["OutletID"].ToString();
                    worksheet.Cell("H" + recNo).Value = row["BranchHead"].ToString();
                    worksheet.Cell("I" + recNo).Value = row["SalesHead"].ToString();
                    worksheet.Cell("J" + recNo).Value = row["SalesCoordinator"].ToString();
                    worksheet.Cell("K" + recNo).Value = row["Wiraniaga"].ToString();
                    worksheet.Cell("L" + recNo).Value = row["StatusProspek"].ToString();
                    worksheet.Cell("M" + recNo).Value = row["PerolehanData"].ToString();
                    worksheet.Cell("N" + recNo).Value = row["NamaProspek"].ToString();
                    worksheet.Cell("O" + recNo).Value = row["AlamatProspek"].ToString();
                    worksheet.Cell("P" + recNo).Value = row["TelpRumah"].ToString();
                    worksheet.Cell("Q" + recNo).Value = row["City"].ToString();
                    worksheet.Cell("R" + recNo).Value = row["NamaPerusahaan"].ToString();
                    worksheet.Cell("S" + recNo).Value = row["AlamatPerusahaan"].ToString();
                    worksheet.Cell("T" + recNo).Value = row["Jabatan"].ToString();
                    worksheet.Cell("U" + recNo).Value = row["Handphone"].ToString();
                    worksheet.Cell("V" + recNo).Value = row["Faximile"].ToString();
                    worksheet.Cell("W" + recNo).Value = row["Email"].ToString();
                    worksheet.Cell("X" + recNo).Value = row["TipeKendaraan"].ToString();
                    worksheet.Cell("Y" + recNo).Value = row["Variant"].ToString();
                    worksheet.Cell("Z" + recNo).Value = row["Transmisi"].ToString();
                    worksheet.Cell("AA" + recNo).Value = row["ColourCode"].ToString();
                    worksheet.Cell("AB" + recNo).Value = row["ColourDescription"].ToString();
                    worksheet.Cell("AC" + recNo).Value = row["CaraPembayaran"].ToString();
                    worksheet.Cell("AD" + recNo).Value = row["TestDrive"].ToString();
                    worksheet.Cell("AE" + recNo).Value = row["QuantityInquiry"].ToString();
                    worksheet.Cell("AF" + recNo).Value = row["LastProgress"].ToString();
                    worksheet.Cell("AG" + recNo).Value = (row["LastUpdateStatus"] is DBNull) ? "1900/01/01" : Convert.ToDateTime(row["LastUpdateStatus"]).ToString("yyyy/MM/dd");
                    worksheet.Cell("AH" + recNo).Value = (row["ProspectDate"] is DBNull) ? "1900/01/01" : Convert.ToDateTime(row["ProspectDate"]).ToString("yyyy/MM/dd");
                    worksheet.Cell("AI" + recNo).Value = (row["HotDate"] is DBNull) ? "1900/01/01" : Convert.ToDateTime(row["HotDate"]).ToString("yyyy/MM/dd");
                    worksheet.Cell("AJ" + recNo).Value = (row["SPKDate"] is DBNull) ? "1900/01/01" : Convert.ToDateTime(row["SPKDate"]).ToString("yyyy/MM/dd");
                    worksheet.Cell("AK" + recNo).Value = (row["DeliveryDate"] is DBNull) ? "1900/01/01" : Convert.ToDateTime(row["DeliveryDate"]).ToString("yyyy/MM/dd");
                    worksheet.Cell("AL" + recNo).Value = row["Leasing"].ToString();
                    worksheet.Cell("AM" + recNo).Value = row["DownPayment"].ToString();
                    worksheet.Cell("AN" + recNo).Value = row["Tenor"].ToString();
                    worksheet.Cell("AO" + recNo).Value = (row["LostCaseDate"] is DBNull) ? "1900/01/01" : Convert.ToDateTime(row["LostCaseDate"]).ToString("yyyy/MM/dd");
                    worksheet.Cell("AP" + recNo).Value = row["LostCaseCategory"].ToString();
                    worksheet.Cell("AQ" + recNo).Value = row["LostCaseReasonID"].ToString();
                    worksheet.Cell("AR" + recNo).Value = row["LostCaseOtherReason"].ToString();
                    worksheet.Cell("AS" + recNo).Value = row["LostCaseVoiceOfCustomer"].ToString();
                    worksheet.Cell("AT" + recNo).Value = row["MerkLain"].ToString();
                    worksheet.Cell("AU" + recNo).Value = row["CreatedBy"].ToString();
                    worksheet.Cell("AV" + recNo).Value = (row["CreatedDate"] is DBNull) ? "1900/01/01" : Convert.ToDateTime(row["CreatedDate"]).ToString("yyyy/MM/dd");
                    worksheet.Cell("AW" + recNo).Value = row["LastUpdateBy"].ToString();
                    worksheet.Cell("AX" + recNo).Value = (row["LastUpdateDate"] is DBNull) ? "1900/01/01" : Convert.ToDateTime(row["LastUpdateDate"]).ToString("yyyy/MM/dd");
                    recNo++;
                }
                cSheet += 1;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult GeneratePartSalesData()
        {
            DateTime now = DateTime.Now;
            string fileName = "Part_Sales_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var qry = ctx.PartSalesViews.AsQueryable();

            var date1 = Request.Params["DateFrom"];
            var date2 = Request.Params["DateTo"];
            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var tpgo = Request.Params["TypeOfGoods"];

            if (!string.IsNullOrWhiteSpace(date1))
            {
                var DateFrom = Convert.ToDateTime(date1).Date;
                qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) >= DateFrom);
            }
            if (!string.IsNullOrWhiteSpace(date2))
            {
                var DateTo = Convert.ToDateTime(date2).Date;
                qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) <= DateTo);
            }

            if (!string.IsNullOrWhiteSpace(area)) qry = qry.Where(p => p.Area == area);
            if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.DealerCode == dealer);
            if (!string.IsNullOrWhiteSpace(tpgo)) qry = qry.Where(p => p.TypeOfGoods == tpgo);

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("ITSReport");

            //First Names
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "DealerCode";
            ws.Cell("C" + recNo).Value = "DealerName";
            ws.Cell("D" + recNo).Value = "DealerAbbreviation";
            ws.Cell("E" + recNo).Value = "BranchCode";
            ws.Cell("F" + recNo).Value = "BranchName";
            ws.Cell("G" + recNo).Value = "BranchAbbreviation";
            ws.Cell("H" + recNo).Value = "InvoiceNo";
            ws.Cell("I" + recNo).Value = "InvoiceDate";
            ws.Cell("J" + recNo).Value = "FPJNo";
            ws.Cell("K" + recNo).Value = "FPJDate";
            ws.Cell("L" + recNo).Value = "CustomerCode";
            ws.Cell("M" + recNo).Value = "CustomerName";
            ws.Cell("N" + recNo).Value = "CustomerClass";
            ws.Cell("O" + recNo).Value = "PartNo";
            ws.Cell("P" + recNo).Value = "PartName";
            ws.Cell("Q" + recNo).Value = "TypeOfGoods";
            ws.Cell("R" + recNo).Value = "TypeOfGoodsDesc";
            ws.Cell("S" + recNo).Value = "QtyBill";
            ws.Cell("T" + recNo).Value = "CostPrice";
            ws.Cell("U" + recNo).Value = "RetailPrice";
            ws.Cell("V" + recNo).Value = "DiscPct";
            ws.Cell("W" + recNo).Value = "DiscAmt";
            ws.Cell("X" + recNo).Value = "NetSalesAmt";

            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("B" + recNo).Value = row.DealerCode;
                ws.Cell("C" + recNo).Value = row.DealerName;
                ws.Cell("D" + recNo).Value = row.DealerAbbreviation;
                ws.Cell("E" + recNo).Value = row.BranchCode;
                ws.Cell("F" + recNo).Value = row.BranchName;
                ws.Cell("G" + recNo).Value = row.BranchAbbreviation;
                ws.Cell("H" + recNo).Value = row.InvoiceNo;
                ws.Cell("I" + recNo).Value = row.InvoiceDate;
                ws.Cell("J" + recNo).Value = row.FPJNo;
                ws.Cell("K" + recNo).Value = row.FPJDate;
                ws.Cell("L" + recNo).Value = row.CustomerCode;
                ws.Cell("M" + recNo).Value = row.CustomerName;
                ws.Cell("N" + recNo).Value = row.CustomerClass;
                ws.Cell("O" + recNo).Value = row.PartNo;
                ws.Cell("P" + recNo).Value = row.PartName.Replace((char)(0x19), ' ');
                //ws.Cell("P" + recNo).Value = ReplaceHexadecimalSymbols(row.PartName);                  
                ws.Cell("Q" + recNo).Value = row.TypeOfGoods;
                ws.Cell("R" + recNo).Value = row.TypeOfGoodsDesc;
                ws.Cell("S" + recNo).Value = row.QtyBill;
                ws.Cell("T" + recNo).Value = row.CostPrice;
                ws.Cell("U" + recNo).Value = row.RetailPrice;
                ws.Cell("V" + recNo).Value = row.DiscPct;
                ws.Cell("W" + recNo).Value = row.DiscAmt;
                ws.Cell("X" + recNo).Value = row.NetSalesAmt;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult GenerateUnitIntake(DateTime? DateFrom, DateTime? DateTo)
        {
            DateTime now = DateTime.Now;
            string fileName = "Unit_Intake_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var data = ctx.UnitIntakeViews.AsQueryable();

            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var outlet = Request.Params["Outlet"];

            int areaCode = 0;
            try
            {
                areaCode = Convert.ToInt32(area);
            }
            catch (Exception)
            {
            }

            if (!string.IsNullOrWhiteSpace(area)) data = data.Where(p => p.AreaCode == areaCode);
            if (!string.IsNullOrWhiteSpace(dealer)) data = data.Where(p => p.CompanyCode == dealer);
            if (!string.IsNullOrWhiteSpace(outlet)) data = data.Where(p => p.BranchCode == outlet);

            if (DateFrom != null && DateTo != null)
            {
                data = data.Where(x => x.JobOrderClosed >= EntityFunctions.TruncateTime(DateFrom) && EntityFunctions.TruncateTime(x.JobOrderClosed) <= DateTo);
            }

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Service Unit Intake");

            var rngTable = ws.Range("A1:AF1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.DarkBlue;
            rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

            ws.Columns("1").Width = 30;
            ws.Columns("2").Width = 20;
            ws.Columns("3").Width = 23;
            ws.Columns("4").Width = 50;
            ws.Columns("5").Width = 20;
            ws.Columns("6").Width = 50;
            ws.Columns("7").Width = 15;
            ws.Columns("8").Width = 10;
            ws.Columns("9").Width = 30;
            ws.Columns("10").Width = 20;
            ws.Columns("11").Width = 15;
            ws.Columns("12").Width = 20;
            ws.Columns("13").Width = 15;
            ws.Columns("14").Width = 15;
            ws.Columns("15").Width = 15;
            ws.Columns("16").Width = 15;
            ws.Columns("17").Width = 25;
            ws.Columns("18").Width = 15;
            ws.Columns("19").Width = 15;
            ws.Columns("20").Width = 15;
            ws.Columns("21").Width = 25;
            ws.Columns("22").Width = 15;
            ws.Columns("23").Width = 15;
            ws.Columns("24").Width = 15;
            ws.Columns("25").Width = 100;
            ws.Columns("26").Width = 50;
            ws.Columns("27").Width = 30;
            ws.Columns("28").Width = 30;
            ws.Columns("29").Width = 30;
            //ws.Columns("30").Width = 20;

            //First Names
            ws.Cell("A" + recNo).Value = "Vin No";
            ws.Cell("B" + recNo).Value = "Tanggal Tutup SPK";
            ws.Cell("C" + recNo).Value = "Kode Dealer (Purchase)";
            ws.Cell("D" + recNo).Value = "Nama Dealer (Purchase)";
            ws.Cell("E" + recNo).Value = "Kode Dealer (Service)";
            ws.Cell("F" + recNo).Value = "CompanyName (Service)";
            ws.Cell("G" + recNo).Value = "Area";
            ws.Cell("H" + recNo).Value = "Odometer";
            ws.Cell("I" + recNo).Value = "Car Type";
            ws.Cell("J" + recNo).Value = "Sales Model Code";
            ws.Cell("K" + recNo).Value = "Basic Model";
            ws.Cell("L" + recNo).Value = "Sales Model Year";
            ws.Cell("M" + recNo).Value = "DO Date";
            ws.Cell("N" + recNo).Value = "Nomor Polisi";
            ws.Cell("O" + recNo).Value = "Engine No";
            ws.Cell("P" + recNo).Value = "Chassis No";
            ws.Cell("Q" + recNo).Value = "Customer Name";
            ws.Cell("R" + recNo).Value = "Phone No";
            ws.Cell("S" + recNo).Value = "Office Phone No";
            ws.Cell("T" + recNo).Value = "HP No";
            ws.Cell("U" + recNo).Value = "Additional Contact";
            ws.Cell("V" + recNo).Value = "Email";
            ws.Cell("W" + recNo).Value = "Birth Date";
            ws.Cell("X" + recNo).Value = "Gender";
            ws.Cell("Y" + recNo).Value = "Address";
            ws.Cell("Z" + recNo).Value = "Group Job Type";
            ws.Cell("AA" + recNo).Value = "Job Type Code";
            ws.Cell("AB" + recNo).Value = "Job Type";
            ws.Cell("AC" + recNo).Value = "Service Advisor Name";
            ws.Cell("AD" + recNo).Value = "Service Advisor NIK";
            //ws.Cell("AE" + recNo).Value = "Branch Code";
            //ws.Cell("AF" + recNo).Value = "Branch Name";

            recNo++;

            foreach (var row in data)
            {
                ws.Cell("A" + recNo).Value = row.VinNo;

                ws.Cell("B" + recNo).Value = (row.JobOrderClosed == null ? "" : row.JobOrderClosed.Value.ToString("dd-MMM-yyyy"));
                ws.Cell("B" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";

                ws.Cell("C" + recNo).Value = (row.DealerCode == null ? "" : row.DealerCode.ToString());
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("D" + recNo).Value = row.DealerName;

                ws.Cell("E" + recNo).Value = row.CompanyCode;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("F" + recNo).Value = row.CompanyName;

                ws.Cell("G" + recNo).Value = row.Area;

                ws.Cell("H" + recNo).Value = row.Odometer;
                ws.Cell("H" + recNo).Style.NumberFormat.Format = "#,##0";

                ws.Cell("I" + recNo).Value = row.SalesModelDesc;

                ws.Cell("J" + recNo).Value = row.SalesModelCode;

                ws.Cell("K" + recNo).Value = row.BasicModel;

                ws.Cell("L" + recNo).Value = row.ProductionYear;
                ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("M" + recNo).Value = (row.DoDate == null ? "" : row.DoDate.Value.ToString("dd-MMM-yyyy"));
                ws.Cell("M" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";

                ws.Cell("N" + recNo).Value = (row.PoliceRegNo == null ? "" : "'" + row.PoliceRegNo.ToString());

                ws.Cell("O" + recNo).Value = (row.EngineNo == null ? "" : "'" + row.EngineNo.ToString());

                ws.Cell("P" + recNo).Value = (row.ChassisNo == null ? "" : "'" + row.ChassisNo.ToString());

                ws.Cell("Q" + recNo).Value = row.CustomerName;

                ws.Cell("R" + recNo).Value = (row.PhoneNo == null ? "" : "'" + row.PhoneNo.ToString());

                ws.Cell("S" + recNo).Value = (row.OfficePhoneNo == null ? "" : "'" + row.OfficePhoneNo.ToString());

                ws.Cell("T" + recNo).Value = (row.HPNo == null ? "" : "'" + row.HPNo.ToString());

                ws.Cell("U" + recNo).Value = row.ContactName;

                ws.Cell("V" + recNo).Value = row.Email;

                ws.Cell("W" + recNo).Value = (row.BirthDate == null ? "" : row.BirthDate.Value.ToString("dd-MMM-yyyy"));
                ws.Cell("W" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";

                ws.Cell("X" + recNo).Value = row.Gender;

                ws.Cell("Y" + recNo).Value = row.Address.Replace((char)0x13, '\t');

                ws.Cell("Z" + recNo).Value = row.GroupJobTypeDesc;

                ws.Cell("AA" + recNo).Value = row.JobType;

                ws.Cell("AB" + recNo).Value = row.JobTypeDesc;

                ws.Cell("AC" + recNo).Value = row.SaName;

                ws.Cell("AD" + recNo).Value = row.SaNik;
                ws.Cell("AD" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //ws.Cell("AE" + recNo).Value = row.BranchCode;
                //ws.Cell("AE" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                //ws.Cell("AF" + recNo).Value = row.BranchName;
                //ws.Cell("AF" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult SvRegisterSpk()
        {
            DateTime now = DateTime.Now;
            string fileName = "Register_SPK_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var qry = from p in ctx.SvRegisterSpkList
                      join q in ctx.GnMstDealerMappings on p.CompanyCode equals q.DealerCode
                      join r in ctx.DealerInfos on p.CompanyCode equals r.DealerCode
                      join s in ctx.OutletInfos on p.BranchCode equals s.BranchCode
                      where p.SupplyQty > 0
                      select new
                      {
                          p.CompanyCode,
                          p.BranchCode,
                          p.ProductType,
                          p.ServiceNo,
                          p.TaskPartNo,
                          TaskPartName = (p.TaskPartType == "T") ? p.OperationName : p.PartName,
                          p.TaskPartSeq,
                          p.JobOrderNo,
                          p.JobOrderDate,
                          p.BasicModel,
                          p.PoliceRegNo,
                          p.Odometer,
                          p.CustomerCode,
                          p.CustomerName,
                          p.GroupJobType,
                          p.GroupJobTypeDesc,
                          p.JobType,
                          p.JobTypeDesc,
                          p.OperationNo,
                          p.OperationName,
                          p.OperationHour,
                          p.FmID,
                          p.FmName,
                          p.SaID,
                          p.SaName,
                          p.MechanicID,
                          p.MechanicName,
                          p.PartNo,
                          p.PartName,
                          p.DemandQty,
                          p.SupplyQty,
                          p.ReturnQty,
                          p.SupplySlipNo,
                          p.SSReturnNo,
                          p.TaskPartType,
                          p.ServiceRequestDesc,
                          p.ServiceStatus,
                          p.ServiceStatusDesc,
                          p.TotalSrvAmount,
                          p.InvoiceNo,
                          q.Area,
                          r.DealerName,
                          ShortDealerName = r.ShortName,
                          s.BranchName,
                          s.ShortBranchName
                      };

            var date1 = Request.Params["DateFrom"];
            var date2 = Request.Params["DateTo"];
            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var outlet = Request.Params["Outlet"];

            if (!string.IsNullOrWhiteSpace(date1))
            {
                var DateFrom = Convert.ToDateTime(date1).Date;
                qry = qry.Where(p => EntityFunctions.TruncateTime(p.JobOrderDate) >= DateFrom);
            }
            if (!string.IsNullOrWhiteSpace(date2))
            {
                var DateTo = Convert.ToDateTime(date2).Date;
                qry = qry.Where(p => EntityFunctions.TruncateTime(p.JobOrderDate) <= DateTo);
            }

            if (!string.IsNullOrWhiteSpace(area)) qry = qry.Where(p => p.Area == area);
            if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.CompanyCode == dealer);
            if (!string.IsNullOrWhiteSpace(outlet)) qry = qry.Where(p => p.BranchCode == outlet);

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Register SPK");

            var rngTable = ws.Range("A1:AJ1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.DarkBlue;
            rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

            ws.Columns("1").Width = 15;
            ws.Columns("2").Width = 50;
            ws.Columns("3").Width = 15;
            ws.Columns("4").Width = 50;
            ws.Columns("5").Width = 20;
            ws.Columns("6").Width = 13;
            ws.Columns("7").Width = 15;
            ws.Columns("8").Width = 10;
            ws.Columns("9").Width = 10;
            ws.Columns("10").Width = 20;
            ws.Columns("11").Width = 20;
            ws.Columns("12").Width = 20;
            ws.Columns("13").Width = 15;
            ws.Columns("14").Width = 25;
            ws.Columns("15").Width = 25;
            ws.Columns("16").Width = 12;
            ws.Columns("17").Width = 12;
            ws.Columns("18").Width = 12;
            ws.Columns("19").Width = 15;
            ws.Columns("20").Width = 15;
            ws.Columns("21").Width = 15;
            ws.Columns("22").Width = 30;
            ws.Columns("23").Width = 15;
            ws.Columns("24").Width = 30;
            ws.Columns("25").Width = 15;
            ws.Columns("26").Width = 30;
            ws.Columns("27").Width = 30;
            ws.Columns("28").Width = 50;
            ws.Columns("29").Width = 30;
            ws.Columns("30").Width = 20;
            ws.Columns("31").Width = 15;
            ws.Columns("32").Width = 17;
            ws.Columns("33").Width = 25;
            ws.Columns("34").Width = 20;
            ws.Columns("35").Width = 15;
            ws.Columns("36").Width = 25;
            ws.Columns("37").Width = 10;
            ws.Columns("38").Width = 10;
            ws.Columns("39").Width = 10;
            ws.Columns("40").Width = 10;
            ws.Columns("41").Width = 10;
            ws.Columns("42").Width = 10;
            ws.Columns("43").Width = 10;
            ws.Columns("44").Width = 15;
            ws.Columns("45").Width = 25;

            //First Names
            ws.Cell("A" + recNo).Value = "Dealer Code";
            ws.Cell("B" + recNo).Value = "Dealer Name";
            ws.Cell("C" + recNo).Value = "Outlet Code";
            ws.Cell("D" + recNo).Value = "Outlet Name";
            ws.Cell("E" + recNo).Value = "No SPK";
            ws.Cell("F" + recNo).Value = "Tanggal SPK";
            ws.Cell("G" + recNo).Value = "Model";
            ws.Cell("H" + recNo).Value = "No. Polisi";
            ws.Cell("I" + recNo).Value = "Odometer";
            ws.Cell("J" + recNo).Value = "Kode Pekerjaan";
            ws.Cell("K" + recNo).Value = "Nama Pekerjaan";
            ws.Cell("L" + recNo).Value = "Urutan Jasa / Part";
            ws.Cell("M" + recNo).Value = "Tipe Jasa / Part";
            ws.Cell("N" + recNo).Value = "Kode Jasa / Part";
            ws.Cell("O" + recNo).Value = "Nama Jasa / Part";
            ws.Cell("P" + recNo).Value = "Demand Qty";
            ws.Cell("Q" + recNo).Value = "Supply Qty";
            ws.Cell("R" + recNo).Value = "Return Qty";
            ws.Cell("S" + recNo).Value = "Supply Slip No";
            ws.Cell("T" + recNo).Value = "SS Return No";
            ws.Cell("U" + recNo).Value = "Service Advisor ID";
            ws.Cell("V" + recNo).Value = "Service Advisor Name";
            ws.Cell("W" + recNo).Value = "Foreman ID";
            ws.Cell("X" + recNo).Value = "Foreman Name";
            ws.Cell("Y" + recNo).Value = "Mechanic ID";
            ws.Cell("Z" + recNo).Value = "Mechanic Name";
            ws.Cell("AA" + recNo).Value = "Status";
            ws.Cell("AB" + recNo).Value = "Request";
            ws.Cell("AC" + recNo).Value = "Operation No";
            ws.Cell("AD" + recNo).Value = "Operation Name";
            ws.Cell("AE" + recNo).Value = "Operation Hour";
            ws.Cell("AF" + recNo).Value = "Customer Code";
            ws.Cell("AG" + recNo).Value = "Customer Name";
            ws.Cell("AH" + recNo).Value = "Invoice No";
            ws.Cell("AI" + recNo).Value = "Total Service Amount";
            ws.Cell("AJ" + recNo).Value = "Grup Job Type";

            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = "'" + row.CompanyCode;
                ws.Cell("B" + recNo).Value = row.DealerName;
                ws.Cell("C" + recNo).Value = "'" + row.BranchCode;
                ws.Cell("D" + recNo).Value = row.ShortBranchName;
                ws.Cell("E" + recNo).Value = "'" + row.JobOrderNo;
                ws.Cell("F" + recNo).Value = row.JobOrderDate;
                ws.Cell("G" + recNo).Value = "'" + row.BasicModel;
                ws.Cell("H" + recNo).Value = "'" + row.PoliceRegNo;
                ws.Cell("I" + recNo).Value = row.Odometer;
                ws.Cell("J" + recNo).Value = row.JobType;
                ws.Cell("K" + recNo).Value = row.JobTypeDesc;
                ws.Cell("L" + recNo).Value = row.TaskPartSeq;
                ws.Cell("M" + recNo).Value = row.TaskPartType;
                ws.Cell("N" + recNo).Value = row.TaskPartNo;
                ws.Cell("O" + recNo).Value = row.TaskPartName;
                ws.Cell("P" + recNo).Value = row.DemandQty;
                ws.Cell("Q" + recNo).Value = row.SupplyQty;
                ws.Cell("R" + recNo).Value = row.ReturnQty;
                ws.Cell("S" + recNo).Value = "'" + row.SupplySlipNo;
                ws.Cell("T" + recNo).Value = row.SSReturnNo;
                ws.Cell("U" + recNo).Value = "'" + row.SaID;
                ws.Cell("V" + recNo).Value = row.SaName;
                ws.Cell("W" + recNo).Value = "'" + row.FmID;
                ws.Cell("X" + recNo).Value = row.FmName;
                ws.Cell("Y" + recNo).Value = "'" + row.MechanicID;
                ws.Cell("Z" + recNo).Value = row.MechanicName;
                ws.Cell("AA" + recNo).Value = row.ServiceStatusDesc;
                ws.Cell("AB" + recNo).Value = row.ServiceRequestDesc;
                ws.Cell("AC" + recNo).Value = row.OperationNo;
                ws.Cell("AD" + recNo).Value = row.OperationName;
                ws.Cell("AE" + recNo).Value = row.OperationHour;
                ws.Cell("AF" + recNo).Value = "'" + row.CustomerCode;
                ws.Cell("AG" + recNo).Value = row.CustomerName;
                ws.Cell("AH" + recNo).Value = row.InvoiceNo;
                ws.Cell("AI" + recNo).Value = row.TotalSrvAmount;
                ws.Cell("AJ" + recNo).Value = row.GroupJobTypeDesc;

                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("F" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("M" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("S" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("AI" + recNo).Style.NumberFormat.Format = "#,##0";





                //    ws.Cell("A" + recNo).Value = row.VinNo;

                //    ws.Cell("B" + recNo).Value = (row.JobOrderClosed == null ? "" : row.JobOrderClosed.Value.ToString("dd-MMM-yyyy"));
                //    ws.Cell("B" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";

                //    ws.Cell("C" + recNo).Value = (row.DealerCode == null ? "" : row.DealerCode.ToString());
                //    ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //    ws.Cell("D" + recNo).Value = row.DealerName;

                //    ws.Cell("E" + recNo).Value = row.CompanyCode;
                //    ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //    ws.Cell("F" + recNo).Value = row.CompanyName;

                //    ws.Cell("G" + recNo).Value = row.Area;

                //    ws.Cell("H" + recNo).Value = row.Odometer;
                //    ws.Cell("H" + recNo).Style.NumberFormat.Format = "#,##0";

                //    ws.Cell("I" + recNo).Value = row.SalesModelDesc;

                //    ws.Cell("J" + recNo).Value = row.SalesModelCode;

                //    ws.Cell("K" + recNo).Value = row.BasicModel;

                //    ws.Cell("L" + recNo).Value = row.ProductionYear;
                //    ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //    ws.Cell("M" + recNo).Value = (row.DoDate == null ? "" : row.DoDate.Value.ToString("dd-MMM-yyyy"));
                //    ws.Cell("M" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";

                //    ws.Cell("N" + recNo).Value = (row.PoliceRegNo == null ? "" : "'" + row.PoliceRegNo.ToString());

                //    ws.Cell("O" + recNo).Value = (row.EngineNo == null ? "" : "'" + row.EngineNo.ToString());

                //    ws.Cell("P" + recNo).Value = (row.ChassisNo == null ? "" : "'" + row.ChassisNo.ToString());

                //    ws.Cell("Q" + recNo).Value = row.CustomerName;

                //    ws.Cell("R" + recNo).Value = (row.PhoneNo == null ? "" : "'" + row.PhoneNo.ToString());

                //    ws.Cell("S" + recNo).Value = (row.OfficePhoneNo == null ? "" : "'" + row.OfficePhoneNo.ToString());

                //    ws.Cell("T" + recNo).Value = (row.HPNo == null ? "" : "'" + row.HPNo.ToString());

                //    ws.Cell("U" + recNo).Value = row.ContactName;

                //    ws.Cell("V" + recNo).Value = row.Email;

                //    ws.Cell("W" + recNo).Value = (row.BirthDate == null ? "" : row.BirthDate.Value.ToString("dd-MMM-yyyy"));
                //    ws.Cell("W" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";

                //    ws.Cell("X" + recNo).Value = row.Gender;

                //    ws.Cell("Y" + recNo).Value = row.Address.Replace((char)0x13, '\t');

                //    ws.Cell("Z" + recNo).Value = row.GroupJobTypeDesc;

                //    ws.Cell("AA" + recNo).Value = row.JobType;

                //    ws.Cell("AB" + recNo).Value = row.JobTypeDesc;

                //    ws.Cell("AC" + recNo).Value = row.SaName;

                //    ws.Cell("AD" + recNo).Value = row.SaNik;
                //    ws.Cell("AD" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult GeneratePartSalesData4W()
        {
            DateTime now = DateTime.Now;
            string fileName = "Part_Sales_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");



            //var qry = from p in ctx.SpHstPartSalesList
            //          join q in ctx.GnMstDealerMappings on p.CompanyCode equals q.DealerCode
            //          join r in ctx.OutletInfos on
            //          new { CompanyCode = p.CompanyCode, BranchCode = p.BranchCode } equals
            //          new { CompanyCode = r.CompanyCode, BranchCode = r.BranchCode }
            //          join s in ctx.DealerInfos on p.CompanyCode equals s.DealerCode
            //          where s.ProductType == "4W"
            //          select new
            //          {
            //              p.RecordID,
            //              p.RecordDate,
            //              p.CompanyCode,
            //              p.BranchCode,
            //              p.InvoiceNo,
            //              p.InvoiceDate,
            //              p.FPJNo,
            //              p.FPJDate,
            //              p.CustomerCode,
            //              p.CustomerName,
            //              p.CustomerClass,
            //              p.PartNo,
            //              p.PartName,
            //              p.TypeOfGoods,
            //              p.TypeOfGoodsDesc,
            //              p.QtyBill,
            //              p.CostPrice,
            //              p.RetailPrice,
            //              p.DiscPct,
            //              p.DiscAmt,
            //              p.NetSalesAmt,
            //              q.Area,
            //              q.DealerCode,
            //              q.DealerAbbreviation,
            //              r.BranchName,
            //              s.DealerName
            //          };

            var date1 = Request.Params["DateFrom"];
            var date2 = Request.Params["DateTo"];
            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var tpgo = Request.Params["TypeOfGoods"];

            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandTimeout = 3600;
            //cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_GeneratePartSales";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("StartDate", date1);
            //cmd.Parameters.AddWithValue("EndDate", date2);
            //cmd.Parameters.AddWithValue("Area", area ?? "");
            //cmd.Parameters.AddWithValue("Dealer", dealer ?? "");
            //cmd.Parameters.AddWithValue("PartType", tpgo ?? "");


            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataSet ds = new DataSet();
            //da.Fill(ds);
            //var qry = ds.Tables[0];

            (ctx as IObjectContextAdapter).ObjectContext.CommandTimeout = 3600;
            //var qry = ctx.Database.SqlQuery<GenPartSales>("exec uspfn_GeneratePartSales @StartDate=@p0, @EndDate=@p1, @Area=@p2, @Dealer=@p3, @PartType=@p4", date1, date2, area ?? "", dealer ?? "", tpgo ?? "");
            var qry = ctx.Database.SqlQuery<GenPartSales>("exec uspfn_GeneratePartSales_New @StartDate=@p0, @EndDate=@p1, @Area=@p2, @Dealer=@p3, @PartType=@p4", date1, date2, area ?? "", dealer ?? "", tpgo ?? "");

            //if (!string.IsNullOrWhiteSpace(date1))
            //{
            //    var DateFrom = Convert.ToDateTime(date1).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) >= DateFrom);
            //}
            //if (!string.IsNullOrWhiteSpace(date2))
            //{
            //    var DateTo = Convert.ToDateTime(date2).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) <= DateTo);
            //}

            //if (!string.IsNullOrWhiteSpace(area)) qry = qry.Where(p => p.Area == area);
            //if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.DealerCode == dealer);
            //if (!string.IsNullOrWhiteSpace(tpgo)) qry = qry.Where(p => p.TypeOfGoods == tpgo);

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Part Sales");

            var rngTable = ws.Range("A1:W1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.DarkBlue;
            rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

            ws.Columns("1").Width = 15;
            ws.Columns("2").Width = 55;
            ws.Columns("3").Width = 15;
            ws.Columns("4").Width = 50;
            ws.Columns("5").Width = 15;
            ws.Columns("6").Width = 15;
            ws.Columns("7").Width = 15;
            ws.Columns("8").Width = 30;
            ws.Columns("9").Width = 50;
            ws.Columns("10").Width = 30;
            ws.Columns("11").Width = 30;
            ws.Columns("12").Width = 50;
            ws.Columns("13").Width = 15;
            ws.Columns("14").Width = 15;
            ws.Columns("15").Width = 15;
            ws.Columns("16").Width = 15;
            ws.Columns("17").Width = 15;
            ws.Columns("18").Width = 15;
            ws.Columns("19").Width = 15;
            ws.Columns("20").Width = 30;
            ws.Columns("21").Width = 30;
            ws.Columns("22").Width = 30;
            ws.Columns("23").Width = 30;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Dealer Code";
            ws.Cell("B" + recNo).Value = "Dealer Name";
            ws.Cell("C" + recNo).Value = "Outlet Code";
            ws.Cell("D" + recNo).Value = "Outlet Name";
            ws.Cell("E" + recNo).Value = "Invoice No";
            ws.Cell("F" + recNo).Value = "Invoice Date";
            ws.Cell("G" + recNo).Value = "FPJ No";
            ws.Cell("H" + recNo).Value = "FPJ Date";
            ws.Cell("I" + recNo).Value = "Customer Name";
            ws.Cell("J" + recNo).Value = "Customer Class";
            ws.Cell("K" + recNo).Value = "Part No";
            ws.Cell("L" + recNo).Value = "Part Name";
            ws.Cell("M" + recNo).Value = "Type of Goods";
            ws.Cell("N" + recNo).Value = "Qty Bill";
            ws.Cell("O" + recNo).Value = "Cost Price";
            ws.Cell("P" + recNo).Value = "Retail Price";
            ws.Cell("Q" + recNo).Value = "Disc Pct";
            ws.Cell("R" + recNo).Value = "Disc Amt";
            ws.Cell("S" + recNo).Value = "Net Sales Amount";
            ws.Cell("T" + recNo).Value = "Record Date";

            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = row.DealerCode;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Value = row.DealerName;
                ws.Cell("C" + recNo).Value = row.BranchCode;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Value = row.BranchName;
                ws.Cell("E" + recNo).Value = row.InvoiceNo;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("F" + recNo).Value = row.InvoiceDate;
                ws.Cell("F" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("G" + recNo).Value = row.FPJNo;
                ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("H" + recNo).Value = row.FPJDate;
                ws.Cell("H" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("I" + recNo).Value = row.CustomerName;
                ws.Cell("J" + recNo).Value = row.CustomerClass;
                ws.Cell("K" + recNo).Value = row.PartNo;
                ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("L" + recNo).Value = row.PartName.Replace((char)(0x19), ' ');
                ws.Cell("M" + recNo).Value = row.TypeOfGoodsDesc;
                ws.Cell("N" + recNo).Value = row.QtyBill;
                ws.Cell("N" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("N" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("O" + recNo).Value = row.CostPrice;
                ws.Cell("O" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("O" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("P" + recNo).Value = row.RetailPrice;
                ws.Cell("P" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("P" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("Q" + recNo).Value = row.DiscPct;
                ws.Cell("Q" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("Q" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("R" + recNo).Value = row.DiscAmt;
                ws.Cell("R" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("R" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("S" + recNo).Value = row.NetSalesAmt;
                ws.Cell("S" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("S" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("T" + recNo).Value = row.RecordDate;
                ws.Cell("T" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("T" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult GeneratePartSalesData2W()
        {
            DateTime now = DateTime.Now;
            string fileName = "Part_Sales_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            //var qry = from p in ctx.SpHstPartSalesList
            //          join q in ctx.GnMstDealerMappings on p.CompanyCode equals q.DealerCode
            //          join r in ctx.OutletInfos on
            //          new { CompanyCode = p.CompanyCode, BranchCode = p.BranchCode } equals
            //          new { CompanyCode = r.CompanyCode, BranchCode = r.BranchCode }
            //          join s in ctx.DealerInfos on p.CompanyCode equals s.DealerCode
            //          where s.ProductType == "2W"
            //          select new
            //          {
            //              p.RecordID,
            //              p.RecordDate,
            //              p.CompanyCode,
            //              p.BranchCode,
            //              p.InvoiceNo,
            //              p.InvoiceDate,
            //              p.FPJNo,
            //              p.FPJDate,
            //              p.CustomerCode,
            //              p.CustomerName,
            //              p.CustomerClass,
            //              p.PartNo,
            //              p.PartName,
            //              p.TypeOfGoods,
            //              p.TypeOfGoodsDesc,
            //              p.QtyBill,
            //              p.CostPrice,
            //              p.RetailPrice,
            //              p.DiscPct,
            //              p.DiscAmt,
            //              p.NetSalesAmt,
            //              q.Area,
            //              q.DealerCode,
            //              q.DealerAbbreviation,
            //              r.BranchName,
            //              OutletAbbreviation = r.ShortBranchName,
            //              s.DealerName,
            //              s.ShortName
            //          };

            //var date1 = Request.Params["DateFrom"];
            //var date2 = Request.Params["DateTo"];
            //var area = Request.Params["Area"];
            //var dealer = Request.Params["Dealer"];
            //var tpgo = Request.Params["TypeOfGoods"];

            //if (!string.IsNullOrWhiteSpace(date1))
            //{
            //    var DateFrom = Convert.ToDateTime(date1).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) >= DateFrom);
            //}
            //if (!string.IsNullOrWhiteSpace(date2))
            //{
            //    var DateTo = Convert.ToDateTime(date2).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) <= DateTo);
            //}

            //if (!string.IsNullOrWhiteSpace(area)) qry = qry.Where(p => p.Area == area);
            //if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.DealerCode == dealer);
            //if (!string.IsNullOrWhiteSpace(tpgo)) qry = qry.Where(p => p.TypeOfGoods == tpgo);


            var date1 = Request.Params["DateFrom"];
            var date2 = Request.Params["DateTo"];
            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var tpgo = Request.Params["TypeOfGoods"];
            ctx.Database.CommandTimeout = 3600;
            var qry = ctx.Database.SqlQuery<GenPartSales>("exec uspfn_GeneratePartSales2W_New @StartDate=@p0, @EndDate=@p1, @Area=@p2, @Dealer=@p3, @PartType=@p4", date1, date2, area ?? "", dealer ?? "", tpgo ?? "");

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Part Sales");

            var rngTable = ws.Range("A1:W1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.DarkBlue;
            rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

            ws.Columns("1").Width = 15;
            ws.Columns("2").Width = 55;
            ws.Columns("3").Width = 15;
            ws.Columns("4").Width = 50;
            ws.Columns("5").Width = 15;
            ws.Columns("6").Width = 15;
            ws.Columns("7").Width = 15;
            ws.Columns("8").Width = 30;
            ws.Columns("9").Width = 50;
            ws.Columns("10").Width = 30;
            ws.Columns("11").Width = 30;
            ws.Columns("12").Width = 50;
            ws.Columns("13").Width = 15;
            ws.Columns("14").Width = 15;
            ws.Columns("15").Width = 15;
            ws.Columns("16").Width = 15;
            ws.Columns("17").Width = 15;
            ws.Columns("18").Width = 15;
            ws.Columns("19").Width = 15;
            ws.Columns("20").Width = 30;
            ws.Columns("21").Width = 30;
            ws.Columns("22").Width = 30;
            ws.Columns("23").Width = 30;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Dealer Code";
            ws.Cell("B" + recNo).Value = "Dealer Name";
            ws.Cell("C" + recNo).Value = "Outlet Code";
            ws.Cell("D" + recNo).Value = "Outlet Name";
            ws.Cell("E" + recNo).Value = "Invoice No";
            ws.Cell("F" + recNo).Value = "Invoice Date";
            ws.Cell("G" + recNo).Value = "FPJ No";
            ws.Cell("H" + recNo).Value = "FPJ Date";
            ws.Cell("I" + recNo).Value = "Customer Name";
            ws.Cell("J" + recNo).Value = "Customer Class";
            ws.Cell("K" + recNo).Value = "Part No";
            ws.Cell("L" + recNo).Value = "Part Name";
            ws.Cell("M" + recNo).Value = "Type of Goods";
            ws.Cell("N" + recNo).Value = "Qty Bill";
            ws.Cell("O" + recNo).Value = "Cost Price";
            ws.Cell("P" + recNo).Value = "Retail Price";
            ws.Cell("Q" + recNo).Value = "Disc Pct";
            ws.Cell("R" + recNo).Value = "Disc Amt";
            ws.Cell("S" + recNo).Value = "Net Sales Amount";
            ws.Cell("T" + recNo).Value = "Record Date";

            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = row.DealerCode;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Value = row.DealerName;
                ws.Cell("C" + recNo).Value = row.BranchCode;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Value = row.BranchName;
                ws.Cell("E" + recNo).Value = row.InvoiceNo;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("F" + recNo).Value = row.InvoiceDate;
                ws.Cell("F" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("G" + recNo).Value = row.FPJNo;
                ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("H" + recNo).Value = row.FPJDate;
                ws.Cell("H" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("I" + recNo).Value = row.CustomerName;
                ws.Cell("J" + recNo).Value = row.CustomerClass;
                ws.Cell("K" + recNo).Value = row.PartNo;
                ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("L" + recNo).Value = row.PartName;
                ws.Cell("M" + recNo).Value = row.TypeOfGoodsDesc;
                ws.Cell("N" + recNo).Value = row.QtyBill;
                ws.Cell("N" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("N" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("O" + recNo).Value = row.CostPrice;
                ws.Cell("O" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("O" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("P" + recNo).Value = row.RetailPrice;
                ws.Cell("P" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("P" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("Q" + recNo).Value = row.DiscPct;
                ws.Cell("Q" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("Q" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("R" + recNo).Value = row.DiscAmt;
                ws.Cell("R" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("R" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("S" + recNo).Value = row.NetSalesAmt;
                ws.Cell("S" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("S" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("T" + recNo).Value = row.RecordDate;
                ws.Cell("T" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("T" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult GenerateITSByLeadTime()
        {
            //var qry = from p in ctx.PmItsByLeadTimes
            //          select new
            //          {
            //              p.Area,
            //              p.CompanyCode,
            //              p.BranchCode,
            //              p.InquiryNumber,
            //              p.InquiryDate,
            //              p.DealerAbbreviation,
            //              p.OutletAbbreviation,
            //              p.TipeKendaraan,
            //              p.Variant,
            //              p.Transmisi,
            //              p.Period,
            //              p.PDate,
            //              p.HPDate,
            //              p.SPKDate,
            //              p.LeadTimeHp,
            //              p.LeadTimeSpk,
            //              p.LastProgress,
            //              p.CreatedDate,
            //              p.CreatedBy,
            //              p.LastUpdateDate,
            //              p.LastUpdateBy
            //          };

            //var date1 = Request.Params["DateFrom"];
            //var date2 = Request.Params["DateTo"];
            //var area = Request.Params["Area"];
            
            //var groupNo = "";
            //var dealer = Request.Params["Dealer"];
            //if (dealer.IndexOf("|") > 0)
            //{
            //    string[] xCode = dealer.Split('|');
            //    groupNo = xCode[0];
            //    dealer = xCode[1];
            //}
            //else
            //{
            //    if (dealer == "")
            //    {
            //        groupNo = area;
            //    }
            //}
            //var outlet = Request.Params["Outlet"];
            
            //if (!string.IsNullOrWhiteSpace(date1))
            //{
            //    var DateFrom = Convert.ToDateTime(date1).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InquiryDate) >= DateFrom);
            //}
            //if (!string.IsNullOrWhiteSpace(date2))
            //{
            //    var DateTo = Convert.ToDateTime(date2).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InquiryDate) <= DateTo);
            //}

            //if (!string.IsNullOrWhiteSpace(groupNo)) qry = qry.Where(p => p.Area == groupNo);
            //if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.CompanyCode == dealer);
            //if (!string.IsNullOrWhiteSpace(outlet)) qry = qry.Where(p => p.BranchCode == outlet);

            string areaRaw = Request.Params["Area"] ?? "";
            string dealer = Request.Params["Dealer"] ?? "";
            string outlet = Request.Params["Outlet"] ?? "";
            string dateFrom = Convert.ToDateTime(Request.Params["DateFrom"]).ToString("yyyyMMdd");
            string dateTo = Convert.ToDateTime(Request.Params["DateTo"]).ToString("yyyyMMdd");

            string groupNo = "";
            if (dealer.IndexOf("|") > 0)
            {
                string[] dd = dealer.Split('|');
                groupNo = dd[0];
                dealer = dd[1];
            }

            string query = "";
            if (!string.IsNullOrWhiteSpace(outlet))
            {
                query = String.Format(@"select a.Area, a.CompanyCode, a.BranchCode, a.InquiryNumber, a.InquiryDate, a.DealerAbbreviation, a.OutletAbbreviation,
                                    a.TipeKendaraan, a.Variant, a.Transmisi, a.Period, a.PDate, a.HPDate, a.SPKDate, a.LeadTimeHp, a.LeadTimeSpk, 
                                    a.LastProgress, a.CreatedDate, a.CreatedBy, a.LastUpdateDate, a.LastUpdateBy from PmItsByLeadTime a 
                                    inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o
	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode and o.OutletCode ='{3}'
                                    where convert(varchar, a.InquiryDate, 112) between '{4}' and '{5}'", areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);

            }
            else
            {
                query = String.Format(@"select a.Area, a.CompanyCode, a.BranchCode, a.InquiryNumber, a.InquiryDate, a.DealerAbbreviation, a.OutletAbbreviation,
                                    a.TipeKendaraan, a.Variant, a.Transmisi, a.Period, a.PDate, a.HPDate, a.SPKDate, a.LeadTimeHp, a.LeadTimeSpk, 
                                    a.LastProgress, a.CreatedDate, a.CreatedBy, a.LastUpdateDate, a.LastUpdateBy from PmItsByLeadTime a 
                                    inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o
	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode
                                    where convert(varchar, a.InquiryDate, 112) between '{3}' and '{4}'", areaRaw, dealer, groupNo, dateFrom, dateTo);
            }


            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<PmItsByLeadTime>(query).AsQueryable();

            DateTime now = DateTime.Now;
            string fileName = "ITS_Report_By_Lead_Time_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("ITS Report By Lead Time");

            var rngTable = ws.Range("A1:Q1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.DarkBlue;
            rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

            ws.Columns("1").Width = 35;
            ws.Columns("2").Width = 15;
            ws.Columns("3").Width = 35;
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
            ws.Columns("16").Width = 15;
            ws.Columns("17").Width = 15;

            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer Code";
            ws.Cell("C" + recNo).Value = "Dealer Abbr.";
            ws.Cell("D" + recNo).Value = "Outlet Code";
            ws.Cell("E" + recNo).Value = "Outlet Abbr.";
            ws.Cell("F" + recNo).Value = "Inquiry No.";
            ws.Cell("G" + recNo).Value = "Tipe Kendaraan";
            ws.Cell("H" + recNo).Value = "Variant";
            ws.Cell("I" + recNo).Value = "Transmisi";
            ws.Cell("J" + recNo).Value = "Period";
            ws.Cell("K" + recNo).Value = "Inquiry Date";
            ws.Cell("L" + recNo).Value = "P Date";
            ws.Cell("M" + recNo).Value = "HP Date";
            ws.Cell("N" + recNo).Value = "SPK Date";
            ws.Cell("O" + recNo).Value = "Lead Time HP";
            ws.Cell("P" + recNo).Value = "Lead Time SPK";
            ws.Cell("Q" + recNo).Value = "Last Progress";

            recNo++;

            foreach (var row in data)
            {
                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("B" + recNo).Value = row.CompanyCode;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("C" + recNo).Value = row.DealerAbbreviation;
                ws.Cell("D" + recNo).Value = row.BranchCode;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("E" + recNo).Value = row.OutletAbbreviation;
                ws.Cell("F" + recNo).Value = "'" + row.InquiryNumber;
                ws.Cell("G" + recNo).Value = row.TipeKendaraan;
                ws.Cell("H" + recNo).Value = row.Variant;
                ws.Cell("I" + recNo).Value = row.Transmisi;
                ws.Cell("J" + recNo).Value = row.Period;
                ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("K" + recNo).Value = row.InquiryDate;
                ws.Cell("K" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("L" + recNo).Value = row.PDate;
                ws.Cell("L" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("M" + recNo).Value = row.HPDate;
                ws.Cell("M" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("N" + recNo).Value = row.SPKDate;
                ws.Cell("N" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("O" + recNo).Value = row.LeadTimeHp;
                ws.Cell("P" + recNo).Value = row.LeadTimeSpk;
                ws.Cell("Q" + recNo).Value = row.LastProgress;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult GenerateITSByTestDrive()
        {
            //var qry = from p in ctx.PmItsByTestDrives
            //          select new
            //          {
            //              p.Area,
            //              p.CompanyCode,
            //              p.BranchCode,
            //              p.InquiryNumber,
            //              p.DealerAbbreviation,
            //              p.OutletAbbreviation,
            //              p.InquiryDate,
            //              p.SPKDate,
            //              p.TipeKendaraan,
            //              p.Variant,
            //              p.ColourCode,
            //              p.Transmisi,
            //              p.LastProgress,
            //              p.Inq,
            //              p.InqTestDrive,
            //              p.OutsSPK,
            //              p.OutsSPKTestDrive,
            //              p.NewSPK,
            //              p.NewSPKTestDrive,
            //              p.TotalSPK,
            //              p.TotalSPKTestDrive,
            //          };

            //var date1 = Request.Params["DateFrom"];
            //var date2 = Request.Params["DateTo"];
            //var area = Request.Params["Area"];
            //var groupNo = "";
            //string dealer = Request["Dealer"] ?? "";
            //if (dealer.IndexOf("|") > 0)
            //{
            //    string[] xCode = dealer.Split('|');
            //    groupNo = xCode[0];
            //    dealer = xCode[1];
            //}
            //else
            //{
            //    if (dealer == "")
            //    {
            //        groupNo = area;
            //    }
            //}
            //var outlet = Request.Params["Outlet"];

            //if (!string.IsNullOrWhiteSpace(date1))
            //{
            //    var DateFrom = Convert.ToDateTime(date1).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InquiryDate) >= DateFrom);
            //}
            //if (!string.IsNullOrWhiteSpace(date2))
            //{
            //    var DateTo = Convert.ToDateTime(date2).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InquiryDate) <= DateTo);
            //}

            //if (!string.IsNullOrWhiteSpace(groupNo)) qry = qry.Where(p => p.Area == groupNo);
            //if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.CompanyCode == dealer);
            //if (!string.IsNullOrWhiteSpace(outlet)) qry = qry.Where(p => p.BranchCode == outlet);

            string areaRaw = Request.Params["Area"] ?? "";
            string dealer = Request.Params["Dealer"] ?? "";
            string outlet = Request.Params["Outlet"] ?? "";
            string dateFrom = Convert.ToDateTime(Request.Params["DateFrom"]).ToString("yyyyMMdd");
            string dateTo = Convert.ToDateTime(Request.Params["DateTo"]).ToString("yyyyMMdd");

            string groupNo = "";
            if (dealer.IndexOf("|") > 0)
            {
                string[] dd = dealer.Split('|');
                groupNo = dd[0];
                dealer = dd[1];
            }

            string query = "";
            if (!string.IsNullOrWhiteSpace(outlet))
            {
                query = String.Format(@"select a.Area, a.CompanyCode, a.BranchCode, a.InquiryNumber, a.DealerAbbreviation, a.OutletAbbreviation, a.InquiryDate,
                                    a.SPKDate, a.TipeKendaraan, a.Variant, a.ColourCode, a.Transmisi, a.LastProgress, a.Inq, a.InqTestDrive, a.OutsSPK, a.OutsSPKTestDrive, a.NewSPK, a.NewSPKTestDrive, 
                                    a.TotalSPK, a.TotalSPKTestDrive from PmItsByTestDrive a 
                                    inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o
	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode and o.OutletCode ='{3}'
                                    where convert(varchar, a.InquiryDate, 112) between '{4}' and '{5}'", areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);
            }
            else
            {
                query = String.Format(@"select a.Area, a.CompanyCode, a.BranchCode, a.InquiryNumber, a.DealerAbbreviation, a.OutletAbbreviation, a.InquiryDate,
                                    a.SPKDate, a.TipeKendaraan, a.Variant, a.ColourCode, a.Transmisi, a.LastProgress, a.Inq, a.InqTestDrive, a.OutsSPK, a.OutsSPKTestDrive, a.NewSPK, a.NewSPKTestDrive, 
                                    a.TotalSPK, a.TotalSPKTestDrive from PmItsByTestDrive a  
                                    inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o
	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode
                                    where convert(varchar, a.InquiryDate, 112) between '{3}' and '{4}'", areaRaw, dealer, groupNo, dateFrom, dateTo);
            }


            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<PmItsByTestDrive3>(query).AsQueryable();


            DateTime now = DateTime.Now;
            string fileName = "ITS_Report_By_Test_Drive_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("ITS Report By Test Drive");

            var rngTable = ws.Range("A1:T1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.DarkBlue;
            rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

            ws.Columns("1").Width = 35;
            ws.Columns("2").Width = 15;
            ws.Columns("3").Width = 15;
            ws.Columns("4").Width = 15;
            ws.Columns("5").Width = 25;
            ws.Columns("6").Width = 20;
            ws.Columns("7").Width = 15;
            ws.Columns("8").Width = 15;
            ws.Columns("9").Width = 15;
            ws.Columns("10").Width = 15;
            ws.Columns("11").Width = 15;
            ws.Columns("12").Width = 15;
            ws.Columns("13").Width = 15;
            ws.Columns("14").Width = 15;
            ws.Columns("15").Width = 15;
            ws.Columns("16").Width = 18;
            ws.Columns("17").Width = 18;
            ws.Columns("18").Width = 18;
            ws.Columns("19").Width = 18;
            ws.Columns("20").Width = 18;
            ws.Columns("21").Width = 18;

            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer Code";
            ws.Cell("C" + recNo).Value = "Dealer Abbr.";
            ws.Cell("D" + recNo).Value = "Outlet Code";
            ws.Cell("E" + recNo).Value = "Outlet Abbr.";
            ws.Cell("F" + recNo).Value = "Inquiry No.";
            ws.Cell("G" + recNo).Value = "Inquiry Date";
            ws.Cell("H" + recNo).Value = "SPK Date";
            ws.Cell("I" + recNo).Value = "Tipe Kendaraan";
            ws.Cell("J" + recNo).Value = "Variant";
            ws.Cell("K" + recNo).Value = "Colour Code";
            ws.Cell("L" + recNo).Value = "Transmisi";
            ws.Cell("M" + recNo).Value = "Last Progress";
            ws.Cell("N" + recNo).Value = "Inq";
            ws.Cell("O" + recNo).Value = "Inq Test Drive";
            ws.Cell("P" + recNo).Value = "Outs SPK";
            ws.Cell("Q" + recNo).Value = "Outs SPK Test Drive,";
            ws.Cell("R" + recNo).Value = "New SPK";
            ws.Cell("S" + recNo).Value = "New SPK Test Drive,";
            ws.Cell("T" + recNo).Value = "Total SPK";
            ws.Cell("U" + recNo).Value = "Total SPK Test Drive";

            recNo++;

            foreach (var row in data)
            {
                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("B" + recNo).Value = row.CompanyCode;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("C" + recNo).Value = row.DealerAbbreviation;
                ws.Cell("D" + recNo).Value = row.BranchCode;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("E" + recNo).Value = row.OutletAbbreviation;
                ws.Cell("F" + recNo).Value = "'" + row.InquiryNumber;
                ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("G" + recNo).Value = row.InquiryDate;
                ws.Cell("G" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("H" + recNo).Value = row.SPKDate;
                ws.Cell("H" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("I" + recNo).Value = row.TipeKendaraan;
                ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("J" + recNo).Value = row.Variant;
                ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("K" + recNo).Value = row.ColourCode;
                ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("L" + recNo).Value = row.Transmisi;
                ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("M" + recNo).Value = row.LastProgress;
                ws.Cell("M" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("N" + recNo).Value = row.Inq;
                ws.Cell("O" + recNo).Value = row.InqTestDrive;
                ws.Cell("P" + recNo).Value = row.OutsSPK;
                ws.Cell("Q" + recNo).Value = row.OutsSPKTestDrive;
                ws.Cell("R" + recNo).Value = row.NewSPK;
                ws.Cell("S" + recNo).Value = row.NewSPKTestDrive;
                ws.Cell("T" + recNo).Value = row.TotalSPK;
                ws.Cell("U" + recNo).Value = row.TotalSPKTestDrive;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult GenerateITSByPerolehanData()
        {
            DateTime now = DateTime.Now;
            string fileName = "ITS_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            string filter = Request["Filter"] ?? "";
            string areaRaw = Request["Area"] ?? "0";
            int? area = null;

            if (!string.IsNullOrWhiteSpace(areaRaw))
            {
                area = Convert.ToInt32(areaRaw);
            }

            string dealer = Request["Dealer"] ?? "";
            string outlet = Request["Outlet"] ?? "";
            DateTime dateFrom = Convert.ToDateTime(Request["DateFrom"]);
            DateTime dateTo = Convert.ToDateTime(Request["DateTo"]);

            if (filter == "0")
            {
                //var data = ctx.PmITSByPerolehanData0.AsQueryable();

                //if (dateFrom != null && dateTo != null)
                //{
                //    data = data.Where(x => x.InquiryDate >= dateFrom && x.InquiryDate <= dateTo);
                //}

                //if (area != null)
                //{
                //    data = data.Where(x => x.AreaCode == area);
                //}

                //if (!string.IsNullOrWhiteSpace(dealer))
                //{
                //    data = data.Where(x => x.CompanyCode == dealer);
                //}

                //if (!string.IsNullOrWhiteSpace(outlet))
                //{
                //    data = data.Where(x => x.BranchCode == outlet);
                //}

                //data = data.OrderBy(x => new { x.Area, x.DealerAbbreviation, x.OutletAbbreviation });

                //var data = ctx.Database.SqlQuery<PmITSByPerolehanData0>("exec uspfn_PmITSByPerolehanData @StartDate=@p0, @EndDate=@p1, @FilterBy=@p2", dateFrom, dateTo, 0);
                var data = ctx.Database.SqlQuery<PmITSByPerolehanData0>("exec uspfn_PmITSByPerolehanData_Grid_New @filter=@p0, @period1=@p1, @period2=@p2,@area=@p3, @company=@p4, @outlet=@p5", filter, dateFrom, dateTo, area, dealer,outlet);

                int recNo = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Part Sales");

                var rngTable = ws.Range("A1:M1");

                rngTable.Style
                    .Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;
                rngTable.Style.Font.FontColor = XLColor.DarkBlue;
                rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

                ws.Columns("1").Width = 25;
                ws.Columns("2").Width = 25;
                ws.Columns("3").Width = 55;
                ws.Columns("4").Width = 25;
                ws.Columns("5").Width = 55;
                ws.Columns("6").Width = 15;
                ws.Columns("7").Width = 15;
                ws.Columns("8").Width = 15;
                ws.Columns("9").Width = 15;
                ws.Columns("10").Width = 15;
                ws.Columns("11").Width = 15;
                ws.Columns("12").Width = 15;

                //First Names                                              
                ws.Cell("A" + recNo).Value = "Area";
                ws.Cell("B" + recNo).Value = "Company Code";
                ws.Cell("C" + recNo).Value = "Dealer Abbr.";
                ws.Cell("D" + recNo).Value = "Branch Code";
                ws.Cell("E" + recNo).Value = "Outlet Abbr.";
                ws.Cell("F" + recNo).Value = "Periode";
                ws.Cell("G" + recNo).Value = "Inquiry Date";
                ws.Cell("H" + recNo).Value = "Tipe Kendaraan";
                ws.Cell("I" + recNo).Value = "Varian";
                ws.Cell("J" + recNo).Value = "Transmisi";
                ws.Cell("K" + recNo).Value = "Perolehan Data";
                ws.Cell("L" + recNo).Value = "Last Progress";
                ws.Cell("M" + recNo).Value = "Total";

                recNo++;

                foreach (var row in data)
                {
                    ws.Cell("A" + recNo).Value = row.Area;
                    ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("B" + recNo).Value = row.CompanyCode;
                    ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("C" + recNo).Value = row.DealerAbbreviation;
                    ws.Cell("V" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("D" + recNo).Value = row.BranchCode;
                    ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("E" + recNo).Value = row.OutletAbbreviation;
                    ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("F" + recNo).Value = row.Period;
                    ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("F" + recNo).Style.DateFormat.Format = "YYYY-MMM";
                    ws.Cell("G" + recNo).Value = row.InquiryDate;
                    ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("G" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                    ws.Cell("H" + recNo).Value = row.TipeKendaraan;
                    ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("I" + recNo).Value = row.Variant;
                    ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("J" + recNo).Value = row.Transmisi;
                    ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("K" + recNo).Value = row.PerolehanData;
                    ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("L" + recNo).Value = row.LastProgress;
                    ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("M" + recNo).Value = row.Total;
                    ws.Cell("M" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    recNo++;
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
            else if (filter == "1")
            {
                //var data = ctx.PmITSByPerolehanData1.AsQueryable();

                //if (dateFrom != null && dateTo != null)
                //{
                //    data = data.Where(x => x.UpdateDate >= dateFrom && x.UpdateDate <= dateTo);
                //}

                //if (area != null)
                //{
                //    data = data.Where(x => x.AreaCode == area);
                //}

                //if (!string.IsNullOrWhiteSpace(dealer))
                //{
                //    data = data.Where(x => x.CompanyCode == dealer);
                //}

                //if (!string.IsNullOrWhiteSpace(outlet))
                //{
                //    data = data.Where(x => x.BranchCode == outlet);
                //}

                //data = data.OrderBy(x => new { x.Area, x.DealerAbbreviation, x.OutletAbbreviation });

                var data = ctx.Database.SqlQuery<PmITSByPerolehanData1>("exec uspfn_PmITSByPerolehanData_Grid_New @filter=@p0, @period1=@p1, @period2=@p2,@area=@p3, @company=@p4, @outlet=@p5", filter, dateFrom, dateTo, area, dealer, outlet);

                int recNo = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Part Sales");

                var rngTable = ws.Range("A1:L1");

                rngTable.Style
                    .Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;
                rngTable.Style.Font.FontColor = XLColor.DarkBlue;
                rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

                ws.Columns("1").Width = 25;
                ws.Columns("2").Width = 25;
                ws.Columns("3").Width = 55;
                ws.Columns("4").Width = 25;
                ws.Columns("5").Width = 55;
                ws.Columns("6").Width = 15;
                ws.Columns("7").Width = 15;
                ws.Columns("8").Width = 15;
                ws.Columns("9").Width = 15;
                ws.Columns("10").Width = 15;
                ws.Columns("11").Width = 15;
                ws.Columns("12").Width = 15;

                //First Names                                              
                ws.Cell("A" + recNo).Value = "Area";
                ws.Cell("B" + recNo).Value = "Dealer Code";
                ws.Cell("C" + recNo).Value = "Dealer Abbr.";
                ws.Cell("D" + recNo).Value = "Outlet Code";
                ws.Cell("E" + recNo).Value = "Outlet Abbr.";
                ws.Cell("F" + recNo).Value = "Periode";
                ws.Cell("G" + recNo).Value = "Vehicle Type";
                ws.Cell("H" + recNo).Value = "Variant";
                ws.Cell("I" + recNo).Value = "Transmition";
                ws.Cell("J" + recNo).Value = "Obtained Data";
                ws.Cell("K" + recNo).Value = "Last Progress";
                ws.Cell("L" + recNo).Value = "Total";

                recNo++;

                foreach (var row in data)
                {
                    ws.Cell("A" + recNo).Value = row.Area;
                    ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("B" + recNo).Value = row.CompanyCode;
                    ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("C" + recNo).Value = row.DealerAbbreviation;
                    ws.Cell("V" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("D" + recNo).Value = row.BranchCode;
                    ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("E" + recNo).Value = row.OutletAbbreviation;
                    ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("F" + recNo).Value = row.Period;
                    //ws.Cell("F" + recNo).Style.DateFormat.Format = "YYYY-MMM";
                    ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("G" + recNo).Value = row.TipeKendaraan;
                    ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("H" + recNo).Value = row.Variant;
                    ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("I" + recNo).Value = row.Transmisi;
                    ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("J" + recNo).Value = row.PerolehanData;
                    ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("K" + recNo).Value = row.LastProgress;
                    ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("L" + recNo).Value = row.Total;
                    ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    recNo++;
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }

            return null;
        }

        public ActionResult GenerateITSByLostCase()
        {
            DateTime now = DateTime.Now;
            string fileName = "ITS_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            string filter = Request["Filter"] ?? "";
            string areaRaw = Request["Area"] ?? "";
            
            var groupNo = "";
            string dealer = Request["Dealer"] ?? "";
            if (dealer.IndexOf("|") > 0)
            {
                string[] xCode = dealer.Split('|');
                groupNo = xCode[0];
                dealer = xCode[1];
            }
            string outlet = Request["Outlet"] ?? "";

            string dateFrom = Convert.ToDateTime(Request["DateFrom"]).ToString("yyyyMMdd");
            string dateTo = Convert.ToDateTime(Request["DateTo"]).ToString("yyyyMMdd");

            IQueryable<IPmITSByLostCase> qry = null;

            if (filter == "0")
            {
                //string query = "select * from PmITSByLostCase0 with(nolock, nowait) where CONVERT(date,InquiryDate) BETWEEN '" + dateFrom + "' AND '" + dateTo + "'";

                //if (!string.IsNullOrWhiteSpace(groupNo)) query += " and AreaCode = " + Convert.ToInt32(groupNo);
                //if (!string.IsNullOrWhiteSpace(dealer)) query += " and CompanyCode = " + dealer;
                //if (!string.IsNullOrWhiteSpace(outlet)) query += " and BranchCode = " + outlet;

                //query += "order by Area, DealerAbbreviation, OutletAbbreviation";

                string query = "";
                if (!string.IsNullOrWhiteSpace(outlet))
                {
                    query = String.Format(@"select a.* from PmITSByLostCase0 a with(nolock, nowait) 
                                inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}','{1}','{2}') 
                                o on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode and o.OutletCode = '{3}'
                                where  CONVERT(varchar,a.InquiryDate, 112) between '{4}' and '{5}' order by a.Area, a.DealerAbbreviation, a.OutletAbbreviation",
                                    areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);

                }
                else
                {
                    query = String.Format(@"select a.* from PmITSByLostCase0 a with(nolock, nowait) 
                                inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}','{1}','{2}') 
                                o on o.DealerCode = a.CompanyCode 
                                where  CONVERT(varchar,a.InquiryDate, 112) between '{3}' and '{4}' order by a.Area, a.DealerAbbreviation, a.OutletAbbreviation",
                                areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);
                }

                ctx.Database.CommandTimeout = 3600;
                qry = ctx.Database.SqlQuery<PmITSByLostCase0>(query).AsQueryable();
            }
            else if (filter == "1")
            {
                //string query = "select * from PmITSByLostCase1 with(nolock, nowait) where CONVERT(date,LostCaseDate) BETWEEN '" + dateFrom + "' AND '" + dateTo + "'";

                //if (!string.IsNullOrWhiteSpace(groupNo)) query += " and AreaCode = " + Convert.ToInt32(groupNo);
                //if (!string.IsNullOrWhiteSpace(dealer)) query += " and CompanyCode = " + dealer;
                //if (!string.IsNullOrWhiteSpace(outlet)) query += " and BranchCode = " + outlet;

                //query += "order by Area, DealerAbbreviation, OutletAbbreviation";

                string query = "";
                if (!string.IsNullOrWhiteSpace(outlet))
                {
                    query = String.Format(@"select a.* from PmITSByLostCase1 a with(nolock, nowait) 
                                inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}','{1}','{2}') 
                                o on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode and o.OutletCode = '{3}'
                                where  CONVERT(varchar,a.InquiryDate, 112) between '{4}' and '{5}' order by a.Area, a.DealerAbbreviation, a.OutletAbbreviation",
                                    areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);
                }
                else
                {
                    query = String.Format(@"select a.* from PmITSByLostCase1 a with(nolock, nowait) 
                                inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}','{1}','{2}') 
                                o on o.DealerCode = a.CompanyCode 
                                where  CONVERT(varchar,a.InquiryDate, 112) between '{3}' and '{4}' order by a.Area, a.DealerAbbreviation, a.OutletAbbreviation",
                                areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);
                }

                ctx.Database.CommandTimeout = 3600;
                qry = ctx.Database.SqlQuery<PmITSByLostCase1>(query).AsQueryable();

            }

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Part Sales");

            var rngTable = ws.Range("A1:Z1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.DarkBlue;
            rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

            ws.Columns("1").Width = 25;
            ws.Columns("2").Width = 25;
            ws.Columns("3").Width = 55;
            ws.Columns("4").Width = 25;
            ws.Columns("5").Width = 55;
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
            ws.Columns("16").Width = 15;
            ws.Columns("17").Width = 15;
            ws.Columns("18").Width = 15;
            ws.Columns("19").Width = 25;
            ws.Columns("20").Width = 30;
            ws.Columns("21").Width = 30;
            ws.Columns("22").Width = 30;
            ws.Columns("23").Width = 25;
            ws.Columns("24").Width = 30;
            ws.Columns("25").Width = 80;
            ws.Columns("26").Width = 80;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Company Code";
            ws.Cell("C" + recNo).Value = "Dealer Abbr.";
            ws.Cell("D" + recNo).Value = "Branch Code";
            ws.Cell("E" + recNo).Value = "Outlet Abbr.";
            ws.Cell("F" + recNo).Value = "Inquiry Number";
            ws.Cell("G" + recNo).Value = "Inquiry Date";
            ws.Cell("H" + recNo).Value = "Prospect Date";
            ws.Cell("I" + recNo).Value = "Hot Prospect Date";
            ws.Cell("J" + recNo).Value = "SPK Date";
            ws.Cell("K" + recNo).Value = "Lost Case Date";
            ws.Cell("L" + recNo).Value = "Status Before Lost";
            ws.Cell("M" + recNo).Value = "P Outs";
            ws.Cell("N" + recNo).Value = "P New";
            ws.Cell("O" + recNo).Value = "HP Outs";
            ws.Cell("P" + recNo).Value = "HP New";
            ws.Cell("Q" + recNo).Value = "SPK Outs";
            ws.Cell("R" + recNo).Value = "SPK New";
            ws.Cell("S" + recNo).Value = "Tipe Kendaraan";
            ws.Cell("T" + recNo).Value = "Varian";
            ws.Cell("U" + recNo).Value = "Transmisi";
            ws.Cell("V" + recNo).Value = "Colour Code";
            ws.Cell("W" + recNo).Value = "Lost Case Category";
            ws.Cell("X" + recNo).Value = "Lost Case Reason";
            ws.Cell("Y" + recNo).Value = "Lost Case Other Reason";
            ws.Cell("Z" + recNo).Value = "Lost Case Voice of Customer";

            recNo++;

            foreach (var row in qry)
            {

                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Value = row.CompanyCode;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("C" + recNo).Value = row.DealerAbbreviation;
                ws.Cell("V" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Value = row.BranchCode;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("E" + recNo).Value = row.OutletAbbreviation;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("F" + recNo).Value = row.InquiryNumber;
                ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("G" + recNo).Value = row.InquiryDate;
                ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("G" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("H" + recNo).Value = row.ProspectDate;
                ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("H" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("I" + recNo).Value = row.HotProspectDate;
                ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("I" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("J" + recNo).Value = row.SPKDate;
                ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("J" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("K" + recNo).Value = row.LostCaseDate;
                ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("K" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("L" + recNo).Value = row.StatusbeforeLOST;
                ws.Cell("M" + recNo).Value = row.P_Outs;
                ws.Cell("M" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("M" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("N" + recNo).Value = row.P_New;
                ws.Cell("N" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("N" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("O" + recNo).Value = row.HP_Outs;
                ws.Cell("O" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("O" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("P" + recNo).Value = row.HP_New;
                ws.Cell("P" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("P" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("Q" + recNo).Value = row.SPK_Outs;
                ws.Cell("Q" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("Q" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("R" + recNo).Value = row.SPK_New;
                ws.Cell("R" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("R" + recNo).Style.NumberFormat.Format = "#,##0";
                ws.Cell("S" + recNo).Value = row.TipeKendaraan;
                ws.Cell("S" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("T" + recNo).Value = row.Variant;
                ws.Cell("T" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("U" + recNo).Value = row.Transmisi;
                ws.Cell("U" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("V" + recNo).Value = row.ColourCode;
                ws.Cell("V" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("W" + recNo).Value = row.LostCaseCategory;
                ws.Cell("W" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("X" + recNo).Value = row.LostCaseReasonDesc;
                ws.Cell("X" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("Y" + recNo).Value = row.LostCaseOtherReason;
                ws.Cell("Y" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("Z" + recNo).Value = row.LostCaseVoiceOfCustomer;
                ws.Cell("Z" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult DealerInfo()
        {
            DateTime now = DateTime.Now;
            string fileName = "Dealer_Info_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            string ptype = Request["ProductType"] ?? "";

            var qry = ctx.Database.SqlQuery<UtilDealerInfo>("exec uspfn_InqDealerInfo @ProductType=@p0", ptype);

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Dealer Info");

            var rngTable = ws.Range("A1:K1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.DarkBlue;
            rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

            ws.Columns("1").Width = 10;
            ws.Columns("2").Width = 15;
            ws.Columns("3").Width = 55;
            ws.Columns("4").Width = 15;
            ws.Columns("5").Width = 55;
            ws.Columns("6").Width = 15;
            ws.Columns("7").Width = 15;
            ws.Columns("8").Width = 15;
            ws.Columns("9").Width = 15;
            ws.Columns("10").Width = 15;
            ws.Columns("11").Width = 15;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Type";
            ws.Cell("B" + recNo).Value = "Dealer Code";
            ws.Cell("C" + recNo).Value = "Dealer Name";
            ws.Cell("D" + recNo).Value = "Outlet Code";
            ws.Cell("E" + recNo).Value = "Outlet Name";
            ws.Cell("F" + recNo).Value = "Sales Date";
            ws.Cell("G" + recNo).Value = "Service Date";
            ws.Cell("H" + recNo).Value = "Sparepart Date";
            ws.Cell("I" + recNo).Value = "AP Date";
            ws.Cell("J" + recNo).Value = "AR Date";
            ws.Cell("K" + recNo).Value = "GL Date";

            recNo++;

            foreach (var row in qry)
            {

                ws.Cell("A" + recNo).Value = row.ProductType;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("B" + recNo).Value = row.DealerCode;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("C" + recNo).Value = row.DealerName;
                ws.Cell("V" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Value = row.OutletCode;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("E" + recNo).Value = row.OutletName;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("F" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("F" + recNo).Value = row.SalesDate;
                ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("G" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("G" + recNo).Value = row.ServiceDate;
                ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("H" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("H" + recNo).Value = row.SparepartDate;
                ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("I" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("I" + recNo).Value = row.ApDate;
                ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("J" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("J" + recNo).Value = row.ArDate;
                ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("K" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                ws.Cell("K" + recNo).Value = row.GlDate;
                ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        //public ActionResult GenerateSlSalesNStock(string Area, string Dealer, string Outlet, string GroupModel, string ModelType, int Year)
        //{
        //    string fileName = "";
        //    string group = Area;
        //    int? area;
        //    if (group == "") { area = 0; } else { area = Convert.ToInt32(group); }
        //    fileName = "SlSalesReportNStock_";
        //    fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");
        //    SqlCommand cmd1 = ctx.Database.Connection.CreateCommand() as SqlCommand;
        //    cmd1.CommandTimeout = 1800;
        //    //cmd1.CommandText = "select AreaDealer from CompaniesGroupMappingView where GroupNo='"+Area+"'";
        //    //cmd1.CommandType = CommandType.Text;
        //    cmd1.CommandText = "uspfn_GetAreaDealerOutlet";

        //    cmd1.CommandType = CommandType.StoredProcedure;
        //    cmd1.Parameters.Clear();
        //    cmd1.Parameters.AddWithValue("@Groupno", area);
        //    cmd1.Parameters.AddWithValue("@DealerCode", Dealer);
        //    cmd1.Parameters.AddWithValue("@BranchCode", Outlet);
        //    SqlDataAdapter ga = new SqlDataAdapter(cmd1);
        //    DataSet gt = new DataSet();
        //    ga.Fill(gt);

        //    foreach (var row1 in gt.Tables[0].Rows)
        //    {
        //        ws.Cell("B1").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[0];
        //        ws.Cell("B2").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[1];
        //        ws.Cell("B3").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[2];
        //    }
        //    ws.Columns("1").Width = 10;
        //    ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    lastRow = 2;
        //    ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    lastRow = 3;
        //    ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    lastRow = 4;
        //    ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    ws.Cell("B" + lastRow).Value = ": " + GroupModel;
        //    lastRow = 5;
        //    ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    ws.Cell("B" + lastRow).Value = ": " + ModelType;
        //    lastRow = 6;
        //    ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    ws.Cell("B" + lastRow).Value = ": " + Year;
        //    lastRow = 7;
        //    ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    ws.Cell("B" + lastRow).Value = "";

        //    DateTime now = DateTime.Now;
        //    string fileName = "Sales_And_Stock_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
        //    int lastRow = 1;
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add("Sales & Stock");

        //    ws.Columns("1").Width = 10;
        //    ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    ws.Cell("A" + lastRow).Value = "Area";
        //    ws.Cell("B" + lastRow).Value = ": ";
        //    lastRow = 2;
        //    ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    ws.Cell("A" + lastRow).Value = "Dealer";
        //    lastRow = 3;
        //    ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    ws.Cell("A" + lastRow).Value = "Showroom";
        //    lastRow = 4;
        //    ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    ws.Cell("A" + lastRow).Value = "Series";
        //    lastRow = 5;
        //    ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    ws.Cell("A" + lastRow).Value = "Model";
        //    lastRow = 6;
        //    ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
        //    ws.Cell("A" + lastRow).Value = "Year";

        //    SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
        //    cmd.CommandTimeout = 1800;
        //    cmd.CommandTimeout = 3600;  cmd.CommandText = "uspRpt_SlReportSalesNStock";

        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.Clear();
        //    cmd.Parameters.AddWithValue("@Groupno", area);
        //    cmd.Parameters.AddWithValue("@CompanyCode", Dealer);
        //    cmd.Parameters.AddWithValue("@BranchCode", Outlet);
        //    cmd.Parameters.AddWithValue("@GroupModel", GroupModel);
        //    cmd.Parameters.AddWithValue("@ModelType", ModelType);
        //    cmd.Parameters.AddWithValue("@Year", Year);

        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    DataSet dt = new DataSet();
        //    da.Fill(dt);

        //    lastRow = 7;
        //    ws.Cell("A" + lastRow).Value = "";
        //    lastRow = 8;

        //    return GenerateExcel(wb, dt, lastRow, fileName);
        //}
        public ActionResult GenerateSlSalesNStock(string Area, string Dealer, string Outlet, string GroupModel, string ModelType, int Year)
        {
            string fileName = "";
            string group = Area;
            int? area;
            if (group == "") { area = 0; } else { area = Convert.ToInt32(group); }
            fileName = "Sales_and_Stock_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");
            SqlCommand cmd1 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd1.CommandTimeout = 1800;
            cmd1.CommandText = "uspfn_GetAreaDealerOutlet";

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.Clear();
            cmd1.Parameters.AddWithValue("@Groupno", area);
            cmd1.Parameters.AddWithValue("@DealerCode", Dealer);
            cmd1.Parameters.AddWithValue("@BranchCode", Outlet);
            SqlDataAdapter ga = new SqlDataAdapter(cmd1);
            DataSet gt = new DataSet();
            ga.Fill(gt);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspRpt_SlReportSalesNStock";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Groupno", area);
            cmd.Parameters.AddWithValue("@CompanyCode", Dealer);
            cmd.Parameters.AddWithValue("@BranchCode", Outlet);
            cmd.Parameters.AddWithValue("@GroupModel", GroupModel);
            cmd.Parameters.AddWithValue("@ModelType", ModelType);
            cmd.Parameters.AddWithValue("@Year", Year);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            da.Fill(dt);

            if (dt.Tables[0].Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int recNo = 7;

                var wb = new XLWorkbook();
                //ada and rename sheets
                var ws = wb.Worksheets.Add("Sales&StockR2");
                //Header Table and rule or modification
                var hdrTable = ws.Range("A1:C6");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                var rngTable = ws.Range("A7:AM8");
                rngTable.Style
                      .Font.SetBold()
                      .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                      .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                      .Alignment.SetWrapText();
                rngTable.Style.Font.Bold = true;

                ws.Range("A7:B7").Merge();
                ws.Range("A8:B8").Merge();
                ws.Range("A7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                //ws.Range("A7:A8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Range("A8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                //ws.Range("A7:A8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("B7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("B7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("B7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("B8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("B8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("B8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("C7:C8").Merge();
                ws.Range("C7:C8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("C7:C8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("C7:C8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("C7:C8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                int j = 6;
                for (int i = 4; i <= 39; )
                {
                    ws.Range(7, i, 7, j).Merge();
                    ws.Range(7, i, 7, j).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range(7, i, 7, j).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range(7, i, 7, j).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range(7, i, 7, j).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    i++; j++;
                    i++; j++;
                    i++; j++;
                }

                //column width

                for (int i = 4; i < 39; i++)
                {
                    ws.Columns(Convert.ToString(i)).Width = 10;
                }
                ws.Columns("1").Width = 25;
                ws.Columns("2").Width = 30;
                ws.Columns("3").Width = 30;

                //First Title   
                ws.Cell("A1").Value = "Area";
                ws.Cell("A2").Value = "Dealer";
                ws.Cell("A3").Value = "Showroom";
                ws.Cell("A4").Value = "Series";
                ws.Cell("A5").Value = "Model";
                ws.Cell("A6").Value = "Year";

                string GroupSeries;
                string modelSeries;
                foreach (var row1 in gt.Tables[0].Rows)
                {
                    ws.Cell("B1").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[0];
                    ws.Cell("B2").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[1];
                    ws.Cell("B3").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[2];
                }
                if (GroupModel == "") { GroupSeries = "All Series"; } else { GroupSeries = GroupModel; }
                if (ModelType == "") { modelSeries = "All Model"; } else { modelSeries = ModelType; }
                ws.Cell("B4").Value = ": " + GroupSeries;
                ws.Cell("B5").Value = ": " + modelSeries;
                ws.Cell("B6").Value = ": " + Year;

                ws.Cell("A7").Value = "Status ";
                ws.Cell("A8").Value = "Cabang/ Sub Dealer";
                ws.Cell("C7").Value = "Model";
                ws.Cell("D7").Value = "Jan";
                ws.Cell("G7").Value = "Feb";
                ws.Cell("J7").Value = "Mar";
                ws.Cell("M7").Value = "Apr";
                ws.Cell("P7").Value = "May";
                ws.Cell("S7").Value = "Jun";
                ws.Cell("V7").Value = "Jul";
                ws.Cell("Y7").Value = "Aug";
                ws.Cell("AB7").Value = "Sep";
                ws.Cell("AE7").Value = "Oct";
                ws.Cell("AH7").Value = "Nov";
                ws.Cell("AK7").Value = "Dec";

                for (int i = 4; i <= 39; i++)
                {
                    ws.Cell(8, i).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell(8, i).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell(8, i).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell(8, i).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                }
                int y = 5;
                int z = 6;
                for (int i = 4; i <= 39; )
                {
                    ws.Cell(8, i).Value = "Sales";
                    ws.Cell(8, y).Value = "Stock";
                    ws.Cell(8, z).Value = "Stock Ratio";
                    i++; y++; z++;
                    i++; y++; z++;
                    i++; y++; z++;
                }

                recNo = recNo + 1;
                recNo++;
                int lastRow = recNo;

                foreach (var row in dt.Tables[0].Rows)
                {
                    for (int i = 1; i <= 39; i++)
                    {
                        ws.Cell(recNo, i).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(recNo, i).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(recNo, i).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(recNo, i).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    }

                    ws.Cell("A" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[0];
                    ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range("A" + recNo + ":B" + recNo).Merge();

                    ws.Cell("C" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[1];
                    ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("D" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[2];
                    ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("E" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                    ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("E" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("F" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                    ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("F" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    ws.Cell("G" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                    ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("G" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("H" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                    ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("H" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("I" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                    ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("I" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    ws.Cell("J" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                    ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("J" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("K" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                    ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("K" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("L" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                    ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("L" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    ws.Cell("M" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                    ws.Cell("M" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("M" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("N" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                    ws.Cell("N" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("N" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("O" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                    ws.Cell("O" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("O" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    ws.Cell("P" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                    ws.Cell("P" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("P" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("Q" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("Q" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("Q" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("R" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                    ws.Cell("R" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("R" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    ws.Cell("S" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("S" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("S" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("T" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                    ws.Cell("T" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("T" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("U" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                    ws.Cell("U" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("U" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    ws.Cell("V" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                    ws.Cell("V" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("V" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("W" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[21];
                    ws.Cell("W" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("W" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("X" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[22];
                    ws.Cell("X" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("X" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    ws.Cell("Y" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[23];
                    ws.Cell("Y" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("Y" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("Z" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[24];
                    ws.Cell("Z" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("Z" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("AA" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[25];
                    ws.Cell("AA" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AA" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    ws.Cell("AB" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[26];
                    ws.Cell("AB" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AB" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("AC" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[27];
                    ws.Cell("AC" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AC" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("AD" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[28];
                    ws.Cell("AD" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AD" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    ws.Cell("AE" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[29];
                    ws.Cell("AE" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AE" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("AF" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[30];
                    ws.Cell("AF" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AF" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("AG" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[31];
                    ws.Cell("AG" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AG" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    ws.Cell("AH" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AH" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[32];
                    ws.Cell("AH" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AH" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("AI" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[33];
                    ws.Cell("AI" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AI" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("AJ" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[34];
                    ws.Cell("AJ" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AJ" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    ws.Cell("AK" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[35];
                    ws.Cell("AK" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AK" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("AL" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[36];
                    ws.Cell("AL" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AL" + recNo).Style.NumberFormat.Format = "#,##0";


                    ws.Cell("AM" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[37];
                    ws.Cell("AM" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("AM" + recNo).Style.NumberFormat.Format = "#,##0.0";


                    recNo++;
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
        }
        public ActionResult GenerateItsSalesProspect(string Area, string Dealer, string Outlet, string GroupModel, string ModelType, int Year)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "ItsSalesProspect_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            SqlCommand cmd1 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd1.CommandTimeout = 1800;
            //cmd1.CommandText = "select AreaDealer from CompaniesGroupMappingView where GroupNo='"+Area+"'";
            //cmd1.CommandType = CommandType.Text;
            cmd1.CommandText = "uspfn_GetAreaDealerOutlet";

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.Clear();
            cmd1.Parameters.AddWithValue("@Groupno", Area);
            cmd1.Parameters.AddWithValue("@DealerCode", Dealer);
            cmd1.Parameters.AddWithValue("@BranchCode", Outlet);
            SqlDataAdapter ga = new SqlDataAdapter(cmd1);
            DataSet gt = new DataSet();
            ga.Fill(gt);


            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspRpt_ItsSalesProspect";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Groupno", Area);
            cmd.Parameters.AddWithValue("@CompanyCode", Dealer);
            cmd.Parameters.AddWithValue("@BranchCode", Outlet);
            cmd.Parameters.AddWithValue("@GroupModel", GroupModel);
            cmd.Parameters.AddWithValue("@ModelType", ModelType);
            cmd.Parameters.AddWithValue("@Year", Year);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            da.Fill(dt);

            if (dt.Tables[0].Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int recNo = 8;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("ITS");
                var hdrTable = ws.Range("A1:C5");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                var rngTable = ws.Range("A7:R9");
                ws.Range("A7:A9").Merge();
                ws.Range("A7:A9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A7:A9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A7:A9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A7:A9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("B7:G7").Merge();
                ws.Range("B7:G7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("B7:G7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("B7:G7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("B7:G7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("B8:C8").Merge();
                ws.Range("B8:C8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("B8:C8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("B8:C8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("B8:C8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("D8:E8").Merge();
                ws.Range("D8:E8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("D8:E8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("D8:E8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("D8:E8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("O7:Q7").Merge();
                ws.Range("O7:Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("O7:Q7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("O7:Q7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("O7:Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("F8:F9").Merge();
                ws.Range("F8:F9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("F8:F9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("F8:F9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("F8:F9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("G8:G9").Merge();
                ws.Range("G8:G9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("G8:G9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("G8:G9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("G8:G9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("H7:H9").Merge();
                ws.Range("H7:H9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("H7:H9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("H7:H9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("H7:H9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("I7:I9").Merge();
                ws.Range("I7:I9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("I7:I9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("I7:I9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("I7:I9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("J7:J9").Merge();
                ws.Range("J7:J9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("J7:J9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("J7:J9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("J7:J9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("L7:R7").Merge();
                ws.Range("L7:R7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("L7:R7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("L7:R7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("L7:R7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("L8:L9").Merge();
                ws.Range("L8:L9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("L8:L9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("L8:L9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("L8:L9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("M8:M9").Merge();
                ws.Range("M8:M9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("M8:M9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("M8:M9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("M8:M9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("N8:N9").Merge();
                ws.Range("N8:N9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("N8:N9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("N8:N9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("N8:N9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("O8:O9").Merge();
                ws.Range("O8:O9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("O8:O9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("O8:O9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("O8:O9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("P8:P9").Merge();
                ws.Range("P8:P9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("P8:P9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("P8:P9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("P8:P9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("Q8:Q9").Merge();
                ws.Range("Q8:Q9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("Q8:Q9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("Q8:Q9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("Q8:Q9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("R8:R9").Merge();
                ws.Range("R8:R9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("R8:R9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("R8:R9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("R8:R9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText();

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;
                //rngTable.Style.Font.FontColor = XLColor.DarkBlue;
                //rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;
                //ws.Columns().AdjustToContents();

                ws.Columns("1").Width = 10;
                ws.Columns("2").Width = 10;
                ws.Columns("3").Width = 10;
                ws.Columns("4").Width = 10;
                ws.Columns("5").Width = 10;
                ws.Columns("6").Width = 10;
                ws.Columns("7").Width = 10;
                ws.Columns("8").Width = 10;
                ws.Columns("9").Width = 10;
                ws.Columns("10").Width = 10;
                ws.Columns("11").Width = 5;
                ws.Columns("12").Width = 10;
                ws.Columns("13").Width = 10;
                ws.Columns("14").Width = 10;
                ws.Columns("15").Width = 10;
                ws.Columns("16").Width = 10;
                ws.Columns("17").Width = 10;
                ws.Columns("18").Width = 10;


                //First Names

                ws.Cell("A1").Value = "Area";
                ws.Cell("A2").Value = "Dealer";
                ws.Cell("A3").Value = "Showroom";
                ws.Cell("A4").Value = "Series";
                ws.Cell("A5").Value = "Model";

                foreach (var row1 in gt.Tables[0].Rows)
                {
                    //ws.Cell("B1").Value = ": " + Area + '-' + ((System.Data.DataRow)(row1)).ItemArray[0];
                    ws.Cell("B1").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[0];
                    //ws.Cell("B2").Value = ": " + Dealer + '-' + ((System.Data.DataRow)(row1)).ItemArray[1];
                    ws.Cell("B2").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[1];
                    //ws.Cell("B3").Value = ": " + Outlet + '-' + ((System.Data.DataRow)(row1)).ItemArray[2];
                    ws.Cell("B3").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[2];
                }

                string GroupSeries;
                string modelSeries;

                if (GroupModel == "")
                {
                    GroupSeries = "All";
                }
                else
                {
                    GroupSeries = GroupModel;
                }

                if (ModelType == "")
                {
                    modelSeries = "All";
                }
                else
                {
                    modelSeries = ModelType;
                }
                ws.Cell("B4").Value = ": " + GroupSeries;
                ws.Cell("B5").Value = ": " + modelSeries;

                ws.Cell("A7").Value = "Year / Month";
                ws.Cell("B7").Value = Year;
                ws.Cell("B8").Value = "Prospect";

                ws.Cell("B9").Value = "New";
                ws.Cell("B9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("B9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("B9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("B9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("C9").Value = "Carry Over";
                ws.Cell("C9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("C9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("C9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("C9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("D8").Value = "Hot Prospect";
                ws.Cell("D9").Value = "New";
                ws.Cell("D9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("D9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("D9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("D9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("E9").Value = "Carry Over";
                ws.Cell("E9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("E9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("E9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("E9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("F8").Value = "Sales";
                ws.Cell("G8").Value = "Lost";
                ws.Cell("H7").Value = "NEW HP / Prospect (%)";
                ws.Cell("I7").Value = "Sales/ Prospect (%)";
                ws.Cell("J7").Value = "Sales/ H.Prospect (%)";
                ws.Cell("L7").Value = "Lost Reason";
                ws.Cell("L8").Value = "Brand Related";
                ws.Cell("M8").Value = "Price";
                ws.Cell("N8").Value = "Product Feature";
                ws.Cell("O8").Value = "Higher Discount";
                ws.Cell("P8").Value = "Unit Availabilty";
                ws.Cell("Q8").Value = "Better Service";
                ws.Cell("R8").Value = "Others";

                recNo = recNo + 1;
                recNo++;

                foreach (var row in dt.Tables[0].Rows)
                {
                    ws.Cell("A" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[0];
                    ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("B" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[1];
                    ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("B" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("C" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[2];
                    ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("C" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("D" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                    ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("E" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                    ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("E" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("F" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                    ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("F" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("G" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                    ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("G" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("H" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                    ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("H" + recNo).Style.NumberFormat.Format = "#0.0";

                    ws.Cell("I" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                    ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("I" + recNo).Style.NumberFormat.Format = "#0.0";

                    ws.Cell("J" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                    ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("J" + recNo).Style.NumberFormat.Format = "#0.0";

                    // ws.Cell("K" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                    // ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("L" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                    ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("L" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("M" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                    ws.Cell("M" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("M" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("N" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                    ws.Cell("N" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("N" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("O" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                    ws.Cell("O" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("O" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("P" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("P" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("P" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("Q" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                    ws.Cell("Q" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("Q" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("R" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("R" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("R" + recNo).Style.NumberFormat.Format = "#,##0";

                    recNo++;
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
        }

        public ActionResult SfmPersInfo()
        {
            string dept = "SALES";
            string comp = Request["CompanyCode"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];
            string branch = Request["BranchCode"];
            DateTime now = DateTime.Now;
            string fileName = "Man_Power_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var data = ctx.Database.SqlQuery<SfmPersInfo>("exec uspfn_HrInqPersInfo @CompanyCode=@p0, @DeptCode=@p1, @PosCode=@p2, @Status=@p3, @BranchCode=@p4", comp, dept, post, status, branch);

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Man Power");

            var rngTable = ws.Range("A1:AC1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.DarkBlue;
            rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

            ws.Columns("1").Width = 50;
            ws.Columns("2").Width = 50;
            ws.Columns("3").Width = 50;
            ws.Columns("4").Width = 50;
            ws.Columns("5").Width = 50;
            ws.Columns("6").Width = 50;
            ws.Columns("7").Width = 50;
            ws.Columns("8").Width = 50;
            ws.Columns("9").Width = 50;
            ws.Columns("10").Width = 50;
            ws.Columns("11").Width = 50;
            ws.Columns("12").Width = 50;
            ws.Columns("13").Width = 50;
            ws.Columns("14").Width = 50;
            ws.Columns("15").Width = 50;
            ws.Columns("16").Width = 50;
            ws.Columns("17").Width = 50;
            ws.Columns("18").Width = 50;
            ws.Columns("19").Width = 50;
            ws.Columns("20").Width = 50;
            ws.Columns("21").Width = 50;
            ws.Columns("22").Width = 50;
            ws.Columns("23").Width = 50;
            ws.Columns("24").Width = 50;
            ws.Columns("25").Width = 50;
            ws.Columns("26").Width = 50;
            ws.Columns("27").Width = 50;
            ws.Columns("28").Width = 50;
            ws.Columns("29").Width = 50;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Outlet";
            ws.Cell("B" + recNo).Value = "Department";
            ws.Cell("C" + recNo).Value = "NIK";
            ws.Cell("D" + recNo).Value = "Nama";
            ws.Cell("E" + recNo).Value = "Jabatan";
            ws.Cell("F" + recNo).Value = "Grade";
            ws.Cell("G" + recNo).Value = "Additional Job";
            ws.Cell("H" + recNo).Value = "Status";
            ws.Cell("I" + recNo).Value = "Join Date";
            ws.Cell("J" + recNo).Value = "Team Leader";
            ws.Cell("K" + recNo).Value = "Resign Date";
            ws.Cell("L" + recNo).Value = "Resign Description";
            ws.Cell("M" + recNo).Value = "Marital Status";
            ws.Cell("N" + recNo).Value = "Religion";
            ws.Cell("O" + recNo).Value = "Gender";
            ws.Cell("P" + recNo).Value = "Education";
            ws.Cell("Q" + recNo).Value = "Birth Place";
            ws.Cell("R" + recNo).Value = "Birth Date";
            ws.Cell("S" + recNo).Value = "Address";
            ws.Cell("T" + recNo).Value = "City";
            ws.Cell("U" + recNo).Value = "Province";
            ws.Cell("V" + recNo).Value = "Zip Code";
            ws.Cell("W" + recNo).Value = "Identity No.";
            ws.Cell("X" + recNo).Value = "NPWP";
            ws.Cell("Y" + recNo).Value = "Email";
            ws.Cell("Z" + recNo).Value = "Height";
            ws.Cell("AA" + recNo).Value = "Weight";
            ws.Cell("AB" + recNo).Value = "Size";
            ws.Cell("AC" + recNo).Value = "Size Alt.";

            recNo++;

            foreach (var row in data)
            {

                //ws.Cell("A" + recNo).Value = row.Area;
                //ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                //ws.Cell("G" + recNo).Style.DateFormat.Format = "DD-MMM-YYYY";
                //ws.Cell("H" + recNo).Value = row.ProspectDate;

                //ws.Cell("R" + recNo).Value = row.SPK_New;
                //ws.Cell("R" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                //ws.Cell("R" + recNo).Style.NumberFormat.Format = "#,##0";
                //ws.Cell("S" + recNo).Value = row.TipeKendaraan;

                ws.Cell("A" + recNo).Value = row.BranchName;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("B" + recNo).Value = row.DeptCode;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("C" + recNo).Value = row.EmployeeID;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("D" + recNo).Value = row.EmployeeName;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("E" + recNo).Value = row.PosName;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("F" + recNo).Value = row.Grade;
                ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("G" + recNo).Value = "";
                ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("H" + recNo).Value = row.Status;
                ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("I" + recNo).Value = row.JoinDate;
                ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("J" + recNo).Value = row.TeamLeader;
                ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("K" + recNo).Value = row.ResignDate;
                ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("L" + recNo).Value = row.ResignDescription;
                ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("M" + recNo).Value = row.MaritalStatus;
                ws.Cell("M" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("N" + recNo).Value = row.Religion;
                ws.Cell("N" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("O" + recNo).Value = row.Gender;
                ws.Cell("O" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("P" + recNo).Value = row.Education;
                ws.Cell("P" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("Q" + recNo).Value = row.BirthPlace;
                ws.Cell("Q" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("R" + recNo).Value = row.BirthDate;
                ws.Cell("R" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("S" + recNo).Value = row.Address;
                ws.Cell("S" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("T" + recNo).Value = row.District;
                ws.Cell("T" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("U" + recNo).Value = row.Province;
                ws.Cell("U" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("V" + recNo).Value = row.ZipCode;
                ws.Cell("V" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("W" + recNo).Value = "'" + row.IdentityNo;
                ws.Cell("W" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("X" + recNo).Value = row.NPWPNo;
                ws.Cell("X" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("Y" + recNo).Value = row.Email;
                ws.Cell("Y" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("Z" + recNo).Value = row.Height;
                ws.Cell("Z" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("AA" + recNo).Value = row.Weight;
                ws.Cell("AA" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("AB" + recNo).Value = row.UniformSize;
                ws.Cell("AB" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("AC" + recNo).Value = row.UniformSizeAlt;
                ws.Cell("AC" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_HrInqPersInfo";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", comp);
            //cmd.Parameters.AddWithValue("@DeptCode", dept);
            //cmd.Parameters.AddWithValue("@PosCode", post);
            //cmd.Parameters.AddWithValue("@Status", status);
            //cmd.Parameters.AddWithValue("@BranchCode", branch);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //da.Fill(dt);
            //return Json(GetJson(dt));
        }

        public ActionResult GenerateSalesPersonContribution(DateTime DateFrom, DateTime DateTo, string Area, string Dealer, string Outlet, string Filter)
        {
            string groupNo = "";
            if (Dealer.IndexOf("|") > 0)
            {
                string[] xCode = Dealer.Split('|');
                groupNo = xCode[0];
                Dealer = xCode[1];
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "usprpt_GenerateSalesPersonContribution_v2New";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@Area", Area);
            cmd.Parameters.AddWithValue("@CompanyCode", Dealer);
            cmd.Parameters.AddWithValue("@BranchCode", Outlet);
            cmd.Parameters.AddWithValue("@FilterBy", Filter);
            cmd.Parameters.AddWithValue("@PeriodStart", DateFrom.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@PeriodEnd", DateTo.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@NewArea", groupNo);

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "SalesPersonContribution_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sales Person Contribution");

            ws.Columns("1").Width = 10;
            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Area";
            lastRow = 2;
            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Dealer";
            lastRow = 3;
            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Outlet";
            lastRow = 4;
            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Filter";
            lastRow = 5;
            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Period";//+ DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");

            SqlCommand cmd1 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd1.CommandTimeout = 1800;
            //cmd1.CommandText = "select AreaDealer from CompaniesGroupMappingView where GroupNo='"+Area+"'";
            //cmd1.CommandType = CommandType.Text;
            cmd1.CommandText = "uspfn_GetAreaDealerOutlet";

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.Clear();
            if (Area == "100")
            {
                cmd1.Parameters.AddWithValue("@Groupno", groupNo);
            }
            else
            {
                cmd1.Parameters.AddWithValue("@Groupno", Area);
            }
            cmd1.Parameters.AddWithValue("@DealerCode", Dealer);
            cmd1.Parameters.AddWithValue("@BranchCode", Outlet);
            SqlDataAdapter ga = new SqlDataAdapter(cmd1);
            DataSet gt = new DataSet();

            ga.Fill(gt);

            foreach (var row1 in gt.Tables[0].Rows)
            {
                ws.Cell("B1").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[0];
                ws.Cell("B2").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[1];
                ws.Cell("B3").Value = ": " + ((System.Data.DataRow)(row1)).ItemArray[2];
            }
            ws.Columns("1").Width = 10;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            lastRow = 2;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            lastRow = 3;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            lastRow = 4;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("B" + lastRow).Value = ": " + Filter;
            lastRow = 5;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("B" + lastRow).Value = ": " + DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");

            lastRow = 6;
            ws.Cell("A" + lastRow).Value = "";
            lastRow = 7;

            return GenerateExcel(wb, dt, lastRow, fileName);
        }

        public ActionResult SvMsiR2(string Dealer, string Outlet, int Year)
        {
            //string comp = Request["CompanyCode"] ?? "--";
            //string bran = Request["BranchCode"] ?? "--";
            //string year = string.IsNullOrWhiteSpace(Request["Year"]) ? "2100" : Request["Year"];

            SqlCommand cmd1 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd1.CommandTimeout = 1800;
            cmd1.CommandText = "uspfn_GetAreaDealerOutlet";

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.Clear();
            cmd1.Parameters.AddWithValue("@Groupno", "");
            cmd1.Parameters.AddWithValue("@DealerCode", Dealer);
            cmd1.Parameters.AddWithValue("@BranchCode", Outlet);
            cmd1.Parameters.AddWithValue("@istype", 1);
            SqlDataAdapter ga = new SqlDataAdapter(cmd1);
            DataTable gt = new DataTable();

            ga.Fill(gt);

            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("MSI DATA");
            DateTime now = DateTime.Now;
            string fileName = "MSI DATA_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            ws.Columns("1").Width = 5;
            ws.Rows("1").Height = 40;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(26);
            ws.Cell("A" + lastRow).Value = "Suzuki Major Service Indicators (MSI) 2 WHEELS - " + Year;
            ws.Range("A1", "F1").Merge();
            ws.Range("A1", "F1").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

            //ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            lastRow = 2;
            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(12);
            ws.Row(2).Height = 30;

            ws.Cell("A" + lastRow).Value = "DEALER  :   " + gt.Rows[0]["Dealer"];
            lastRow = 3;
            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(12);
            ws.Row(3).Height = 30;

            ws.Cell("A" + lastRow).Value = "OUTLET  :   " + gt.Rows[0]["Showroom"];
            lastRow = 4;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_WhInqMsiR2";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", Dealer);
            cmd.Parameters.AddWithValue("@BranchCode", Outlet);
            cmd.Parameters.AddWithValue("@PeriodYear", Year);
            cmd.Parameters.AddWithValue("@isType", 1);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return GenerateExcelR2(wb, dt, lastRow, fileName);
        }

        public ActionResult SvMsiR2V2(string Dealer, string Outlet, int Year)
        {
            //string comp = Request["CompanyCode"] ?? "--";
            //string bran = Request["BranchCode"] ?? "--";
            //string year = string.IsNullOrWhiteSpace(Request["Year"]) ? "2100" : Request["Year"];

            SqlCommand cmd1 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd1.CommandTimeout = 1800;
            cmd1.CommandText = "uspfn_GetAreaDealerOutlet";

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.Clear();
            cmd1.Parameters.AddWithValue("@Groupno", "");
            cmd1.Parameters.AddWithValue("@DealerCode", Dealer);
            cmd1.Parameters.AddWithValue("@BranchCode", Outlet);
            cmd1.Parameters.AddWithValue("@istype", 1);
            SqlDataAdapter ga = new SqlDataAdapter(cmd1);
            DataTable gt = new DataTable();

            ga.Fill(gt);

            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("MSI DATA");
            DateTime now = DateTime.Now;
            string fileName = "MSI DATA V2_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            ws.Column(1).Width = 3;
            ws.Row(1).Height = 60;
            ws.Range("A1", "R1").Merge();
            ws.Cell("A" + lastRow).Value = "Major Service Indicators (MSI) - Motorcycle 2W";
            ws.Cell("A" + lastRow).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
            ws.Cell("A" + lastRow).Style.Font.Bold = true;
            ws.Cell("A" + lastRow).Style.Font.SetFontColor(XLColor.White);

            //ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            lastRow = 3;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Column(2).Width = 30;
            ws.Column(3).Width = 30;
            ws.Row(2).Height = 13;
            ws.Cell("B" + lastRow).Value = "Kode Dealer SIS ";
            ws.Cell("C" + lastRow).Value = ": " + gt.Rows[0]["CompanyCode"];

            ws.Cell("E" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("E" + lastRow).Value = "Kota ";
            ws.Cell("F" + lastRow).Value = ": " + gt.Rows[0]["CITY"];

            lastRow = 4;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("B" + lastRow).Value = "Dealer Name " ;
            ws.Cell("C" + lastRow).Value = ": " + gt.Rows[0]["Dealer"];

            ws.Cell("E" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("E" + lastRow).Value = "Region " ;
            ws.Cell("F" + lastRow).Value = ": " + gt.Rows[0]["Area"];

            lastRow = 5;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("B" + lastRow).Value = "Outlet Name " ;
            ws.Cell("C" + lastRow).Value = ": " + gt.Rows[0]["Showroom"];

            ws.Cell("E" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("E" + lastRow).Value = "Status Dealer " ;
            ws.Cell("F" + lastRow).Value = ": " + gt.Rows[0]["StatusDealer"];

            lastRow = 6;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("B" + lastRow).Value = "Alamat " ;
            ws.Cell("C" + lastRow).Value = ": " + gt.Rows[0]["Address1"];

            ws.Cell("E" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("E" + lastRow).Value = "Tahun ";
            ws.Cell("F" + lastRow).Value = ": " + Year;

            ws.Cell("P" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("P" + lastRow).Value = "Print Date ";
            ws.Cell("Q" + lastRow).Value = ": " + String.Format("{0:g}", DateTime.Now);
            lastRow = 8;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_WhInqMsiR2V2";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", Dealer);
            cmd.Parameters.AddWithValue("@BranchCode", Outlet);
            cmd.Parameters.AddWithValue("@PeriodYear", Year);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.Username);
            cmd.Parameters.AddWithValue("@istype", 1);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return GenerateExcelR2V2(wb, dt, lastRow, fileName);
        }


        public ActionResult Rawdata(string tablename)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "Select top 100000 * from " + tablename;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@Area", Area);
            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = tablename + "_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(tablename);

            return GenerateExcelnoborder(wb, dt, lastRow, fileName);
        }

        public ActionResult LiveStockPartProd()
        {
            DateTime now = DateTime.Now;
            string fileName = "Live_Stock_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var PartNo = Request.Params["PartNo"];
            var Area = Request.Params["Area"];

            var qry = ctx.Database.SqlQuery<LiveStockPartDetail>("exec uspfn_spLiveStockPartDetail @partno=@p0, @area=@p1", string.IsNullOrEmpty(PartNo) ? "%" : PartNo, string.IsNullOrEmpty(Area) ? "%" : Area).AsQueryable();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Live Stock Available");

            ws.Cell("A" + recNo).Value = "Part No : ";
            ws.Cell("B" + recNo).Value = PartNo;
            ws.Cell("A" + recNo).Style.Font.Bold = true;
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("B" + recNo).Style.Font.FontSize = 12;
            recNo += 2;

            var rngTable = ws.Range("A3:D3");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.Black;
            rngTable.Style.Fill.BackgroundColor = XLColor.FromArgb(219, 229, 241);

            ws.Columns("1").Width = 15;
            ws.Columns("2").Width = 50;
            ws.Columns("3").Width = 50;
            ws.Columns("4").Width = 30;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer";
            ws.Cell("C" + recNo).Value = "Outlet";
            ws.Cell("D" + recNo).Value = "Qty-Avail";

            BorderAround(ws.Cell("A" + recNo));
            BorderAround(ws.Cell("B" + recNo));
            BorderAround(ws.Cell("C" + recNo));
            BorderAround(ws.Cell("D" + recNo));
            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Value = row.Dealer;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("C" + recNo).Value = row.Outlet;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Value = row.QtyAvail;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                BorderAround(ws.Cell("A" + recNo));
                BorderAround(ws.Cell("B" + recNo));
                BorderAround(ws.Cell("C" + recNo));
                BorderAround(ws.Cell("D" + recNo));
                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult LiveStockPartProd2()
        {
            DateTime now = DateTime.Now;
            string fileName = "Live_Stock_2W_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var PartNo = Request.Params["PartNo"];
            var Area = Request.Params["Area"];

            var qry = ctx.Database.SqlQuery<LiveStockPartDetail>("exec uspfn_spLiveStockPartDetail2 @partno=@p0, @area=@p1", string.IsNullOrEmpty(PartNo) ? "%" : PartNo, string.IsNullOrEmpty(Area) ? "%" : Area).AsQueryable();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Live Stock Available");

            ws.Cell("A" + recNo).Value = "Part No : ";
            ws.Cell("B" + recNo).Value = PartNo;
            ws.Cell("A" + recNo).Style.Font.Bold = true;
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("B" + recNo).Style.Font.FontSize = 12;
            recNo += 2;

            var rngTable = ws.Range("A3:D3");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.Black;
            rngTable.Style.Fill.BackgroundColor = XLColor.FromArgb(219, 229, 241);

            ws.Columns("1").Width = 15;
            ws.Columns("2").Width = 50;
            ws.Columns("3").Width = 50;
            ws.Columns("4").Width = 30;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer";
            ws.Cell("C" + recNo).Value = "Outlet";
            ws.Cell("D" + recNo).Value = "Qty-Avail";

            BorderAround(ws.Cell("A" + recNo));
            BorderAround(ws.Cell("B" + recNo));
            BorderAround(ws.Cell("C" + recNo));
            BorderAround(ws.Cell("D" + recNo));
            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Value = row.Dealer;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("C" + recNo).Value = row.Outlet;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Value = row.QtyAvail;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                BorderAround(ws.Cell("A" + recNo));
                BorderAround(ws.Cell("B" + recNo));
                BorderAround(ws.Cell("C" + recNo));
                BorderAround(ws.Cell("D" + recNo));
                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult LiveStockPartInqProd()
        {
            DateTime now = DateTime.Now;
            string fileName = "Live_Stock_Inquery_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var PartNo = Request.Params["PartNo"];
            var Area = Request.Params["Area"];
            var Qty = Request.Params["Qty"];
            int Quantity;
            if (int.TryParse(Qty, out Quantity) == false || string.IsNullOrEmpty(Qty))
            {
                Quantity = 0;
            }
            else
            {
                Quantity = Convert.ToInt32(Qty);
            }
            var qry = ctx.Database.SqlQuery<LiveStockPartInqDetail>(string.Format("exec uspfn_spLiveStockPartInqDetail '{0}', '{1}', {2}", PartNo, Area, Quantity)).AsQueryable();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Live Stock Inquery Available");

            ws.Cell("A" + recNo).Value = "Part No : ";
            ws.Cell("B" + recNo).Value = PartNo;
            ws.Cell("A" + recNo).Style.Font.Bold = true;
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("B" + recNo).Style.Font.FontSize = 12;
            recNo += 2;

            var rngTable = ws.Range("A3:D3");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.Black;
            rngTable.Style.Fill.BackgroundColor = XLColor.FromArgb(219, 229, 241);

            ws.Columns("1").Width = 15;
            ws.Columns("2").Width = 50;
            ws.Columns("3").Width = 50;
            ws.Columns("4").Width = 30;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer";
            ws.Cell("C" + recNo).Value = "Outlet";
            ws.Cell("D" + recNo).Value = "Qty-Avail";
            BorderAround(ws.Cell("A" + recNo));
            BorderAround(ws.Cell("B" + recNo));
            BorderAround(ws.Cell("C" + recNo));
            BorderAround(ws.Cell("D" + recNo));
            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Value = row.Dealer;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("C" + recNo).Value = row.Outlet;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Value = row.QtyAvail;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                BorderAround(ws.Cell("A" + recNo));
                BorderAround(ws.Cell("B" + recNo));
                BorderAround(ws.Cell("C" + recNo));
                BorderAround(ws.Cell("D" + recNo));
                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }


        public ActionResult LiveStockPartInqProd2()
        {
            DateTime now = DateTime.Now;
            string fileName = "Live_Stock_2W_Inquery_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var PartNo = Request.Params["PartNo"];
            var Area = Request.Params["Area"];
            var Qty = Request.Params["Qty"];
            int Quantity;
            if (int.TryParse(Qty, out Quantity) == false || string.IsNullOrEmpty(Qty))
            {
                Quantity = 0;
            }
            else
            {
                Quantity = Convert.ToInt32(Qty);
            }
            var qry = ctx.Database.SqlQuery<LiveStockPartInqDetail>(string.Format("exec uspfn_spLiveStockPartInqDetail2 '{0}', '{1}', {2}", PartNo, Area, Quantity)).AsQueryable();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Live Stock Inquery Available");

            ws.Cell("A" + recNo).Value = "Part No : ";
            ws.Cell("B" + recNo).Value = PartNo;
            ws.Cell("A" + recNo).Style.Font.Bold = true;
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("B" + recNo).Style.Font.FontSize = 12;
            recNo += 2;

            var rngTable = ws.Range("A3:D3");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.Black;
            rngTable.Style.Fill.BackgroundColor = XLColor.FromArgb(219, 229, 241);

            ws.Columns("1").Width = 15;
            ws.Columns("2").Width = 50;
            ws.Columns("3").Width = 50;
            ws.Columns("4").Width = 30;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer";
            ws.Cell("C" + recNo).Value = "Outlet";
            ws.Cell("D" + recNo).Value = "Qty-Avail";
            BorderAround(ws.Cell("A" + recNo));
            BorderAround(ws.Cell("B" + recNo));
            BorderAround(ws.Cell("C" + recNo));
            BorderAround(ws.Cell("D" + recNo));
            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Value = row.Dealer;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("C" + recNo).Value = row.Outlet;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Value = row.QtyAvail;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                BorderAround(ws.Cell("A" + recNo));
                BorderAround(ws.Cell("B" + recNo));
                BorderAround(ws.Cell("C" + recNo));
                BorderAround(ws.Cell("D" + recNo));
                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult LiveStockPartTransProd()
        {
            DateTime now = DateTime.Now;
            string fileName = "Live_Stock_Transaction_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var PartNo = Request.Params["PartNo"];
            var Area = Request.Params["Area"];

            var qry = ctx.Database.SqlQuery<LiveStockPartTransDetail>(string.Format("exec uspfn_spLiveStockPartTransDetail '{0}', '{1}'", PartNo, Area)).AsQueryable();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Live Stock Buy_Sell Mode");

            ws.Cell("A" + recNo).Value = "Part No : ";
            ws.Cell("B" + recNo).Value = PartNo;
            ws.Cell("A" + recNo).Style.Font.Bold = true;
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("B" + recNo).Style.Font.FontSize = 12;
            recNo += 2;

            var rngTable = ws.Range("A3:E3");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.Black;
            rngTable.Style.Fill.BackgroundColor = XLColor.FromArgb(219, 229, 241);

            ws.Columns("1").Width = 15;
            ws.Columns("2").Width = 50;
            ws.Columns("3").Width = 50;
            ws.Columns("4").Width = 15;
            ws.Columns("5").Width = 15;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer";
            ws.Cell("C" + recNo).Value = "Outlet";
            ws.Cell("D" + recNo).Value = "D.AVG/Day";
            ws.Cell("E" + recNo).Value = "MC";
            BorderAround(ws.Cell("A" + recNo));
            BorderAround(ws.Cell("B" + recNo));
            BorderAround(ws.Cell("C" + recNo));
            BorderAround(ws.Cell("D" + recNo));
            BorderAround(ws.Cell("E" + recNo));
            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Value = row.Dealer;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("C" + recNo).Value = row.Outlet;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Value = row.Average;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("E" + recNo).Value = row.MovingCode;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                BorderAround(ws.Cell("A" + recNo));
                BorderAround(ws.Cell("B" + recNo));
                BorderAround(ws.Cell("C" + recNo));
                BorderAround(ws.Cell("D" + recNo));
                BorderAround(ws.Cell("E" + recNo));
                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }


        public ActionResult LiveStockPartTransProd2()
        {
            DateTime now = DateTime.Now;
            string fileName = "Live_Stock_2W_Transaction_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var PartNo = Request.Params["PartNo"];
            var Area = Request.Params["Area"];

            var qry = ctx.Database.SqlQuery<LiveStockPartTransDetail>(string.Format("exec uspfn_spLiveStockPartTransDetail2 '{0}', '{1}'", PartNo, Area)).AsQueryable();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Live Stock Buy_Sell Mode");

            ws.Cell("A" + recNo).Value = "Part No : ";
            ws.Cell("B" + recNo).Value = PartNo;
            ws.Cell("A" + recNo).Style.Font.Bold = true;
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("B" + recNo).Style.Font.FontSize = 12;
            recNo += 2;

            var rngTable = ws.Range("A3:E3");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.Black;
            rngTable.Style.Fill.BackgroundColor = XLColor.FromArgb(219, 229, 241);

            ws.Columns("1").Width = 15;
            ws.Columns("2").Width = 50;
            ws.Columns("3").Width = 50;
            ws.Columns("4").Width = 15;
            ws.Columns("5").Width = 15;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer";
            ws.Cell("C" + recNo).Value = "Outlet";
            ws.Cell("D" + recNo).Value = "D.AVG/Day";
            ws.Cell("E" + recNo).Value = "MC";
            BorderAround(ws.Cell("A" + recNo));
            BorderAround(ws.Cell("B" + recNo));
            BorderAround(ws.Cell("C" + recNo));
            BorderAround(ws.Cell("D" + recNo));
            BorderAround(ws.Cell("E" + recNo));
            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Value = row.Dealer;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("C" + recNo).Value = row.Outlet;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Value = row.Average;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("E" + recNo).Value = row.MovingCode;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                BorderAround(ws.Cell("A" + recNo));
                BorderAround(ws.Cell("B" + recNo));
                BorderAround(ws.Cell("C" + recNo));
                BorderAround(ws.Cell("D" + recNo));
                BorderAround(ws.Cell("E" + recNo));
                recNo++;
            }
                         
            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        #endregion

        public ActionResult MonitoringDealer(string ProductType, string Dealer, string IsWhat, string Caption, string ExportDetails)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;

            if (ExportDetails == "0")
            {
                cmd.CommandText = "uspfn_MonitoringDealer";

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ProductType", ProductType);
                cmd.Parameters.AddWithValue("@DealerCode", Dealer);
                cmd.Parameters.AddWithValue("@IsWhat", IsWhat);
            }
            else
            {
                cmd.CommandText = "uspfn_MonitoringDealerDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@DealerCode", Dealer);
                //cmd.Parameters.AddWithValue("@OutletCode", OutletCode);
                cmd.Parameters.AddWithValue("@ModuleName", IsWhat);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (IsWhat == "SNIS")
            {
                //dt.Columns.Remove("OutletCode");
                //dt.Columns.Remove("OutletName");
                dt.Columns["DealerCode"].ColumnName = "Dealer Code";
                dt.Columns["DealerName"].ColumnName = "Dealer Name";
                dt.Columns["OutletCode"].ColumnName = "Outlet Code";
                dt.Columns["OutletName"].ColumnName = "Outlet Name";

                if (ExportDetails == "1") dt.Columns["TableName"].ColumnName = "Table Name";

                dt.Columns["UserName"].ColumnName = "User Name SNIS";
                dt.Columns["LastLogin"].ColumnName = "Last Login Date SNIS";
                dt.Columns["LastUpdated"].ColumnName = "Last Update Date SNIS";
            }
            else if (IsWhat == "MS")
            {
                //dt.Columns.Remove("OutletCode");
                //dt.Columns.Remove("OutletName");
                dt.Columns["DealerCode"].ColumnName = "Dealer Code";
                dt.Columns["DealerName"].ColumnName = "Dealer Name";
                dt.Columns["OutletCode"].ColumnName = "Outlet Code";
                dt.Columns["OutletName"].ColumnName = "Outlet Name";
                if (ExportDetails == "1") dt.Columns["TableName"].ColumnName = "Table Name";
                dt.Columns["UserName"].ColumnName = "User Name Market Share";
                dt.Columns["LastLogin"].ColumnName = "Last Login Date Market Share";
                dt.Columns["LastUpdated"].ColumnName = "Last Update Date Market Share";
            }
            else if (IsWhat == "MM")
            {
                //dt.Columns.Remove("OutletCode");
                //dt.Columns.Remove("OutletName");
                dt.Columns.Remove("UserName");
                dt.Columns["DealerCode"].ColumnName = "Dealer Code";
                dt.Columns["DealerName"].ColumnName = "Dealer Name";
                dt.Columns["OutletCode"].ColumnName = "Outlet Code";
                dt.Columns["OutletName"].ColumnName = "Outlet Name";
                if (ExportDetails == "1") dt.Columns["TableName"].ColumnName = "Table Name";
                dt.Columns["LastLogin"].ColumnName = "Last Login Date Manpower Management";
                dt.Columns["LastUpdated"].ColumnName = "Last Update Date Man Power Management";
            }
            else
            {
                dt.Columns.Remove("UserName");
                dt.Columns["DealerCode"].ColumnName = "Dealer Code";
                dt.Columns["DealerName"].ColumnName = "Dealer Name";
                dt.Columns["OutletCode"].ColumnName = "Outlet Code";
                dt.Columns["OutletName"].ColumnName = "Outlet Name";
                if (ExportDetails == "1") dt.Columns["TableName"].ColumnName = "Table Name";
                dt.Columns["LastLogin"].ColumnName = "Last Login Date CS Aplication";
                dt.Columns["LastUpdated"].ColumnName = "Last Update Date CS Aplication";
            }

            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(IsWhat);
            DateTime now = DateTime.Now;
            string fileName = IsWhat + "_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(12);
            ws.Cell("A" + lastRow).Value = Caption;
            lastRow = 3;
            return GenerateExcel(wb, dt, lastRow, fileName);
        }

        private void BorderAround(IXLCell cell)
        {
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
        }

        public ActionResult GenerateInqDataCoupon()
        {
            string area = Request["Area"] ?? "";
            string dealer = Request["Dealer"] ?? "";
            string outlet = Request["Outlet"] ?? "";
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string BeginCoupon = Request["BeginCoupon"] ?? "";
            string EndCoupon = Request["EndCoupon"] ?? "";

            string fileName = "";
            fileName = "Inquiry Data Coupon";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = String.Format(@"SELECT ROW_NUMBER() OVER(ORDER BY a.CoupunNumber ASC) as No, a.CoupunNumber as [No Kupon], a.TestDriveDate as [Tanggal Test Drive] , b.NamaProspek as [Nama], Convert(varchar(50),a.ProspekIdentityNo) as [No SIM A]
                , b.AlamatProspek as [Alamat], b.TelpRumah as [Telp/Hp] , a.Email as [Email], c.EmployeeName as [Nama Salesman], d.SalesID as [ID SIS (ITS)]
                , '''' + c.IdentityNo as [No KTP] , e.OutletName as [Dealer], e.OutletArea as [Daerah], a.Remark as [Remark]
                FROM pmKDPCoupon a
                INNER JOIN pmKDP b
	                ON a.CompanyCode = b.CompanyCode
	                and a.InquiryNumber = b.InquiryNumber
	                AND a.NamaProspek = b.NamaProspek
                INNER JOIN HrEmployee c
	                ON a.CompanyCode = c.CompanyCode
	                and a.EmployeeID = c.EmployeeID
                LEFT JOIN HrEmployeeSales d
	                ON c.CompanyCode = d.CompanyCode
	                AND c.EmployeeID = d.EmployeeID
                INNER JOIN gnMstDealerOutletMapping e
	                ON b.CompanyCode = e.DealerCode
	                AND b.BranchCode = e.OutletCode
                INNER JOIN gnMstDealerMapping f
	                ON e.DealerCode = f.DealerCode
	                AND e.GroupNo = f.GroupNo
                WHERE a.TestDriveDate BETWEEN '{0}' AND '{1}'
                AND f.GroupNo = (CASE WHEN '{2}'='' THEN f.GroupNo ELSE '{2}' end)
                AND b.CompanyCode = (CASE WHEN '{3}'='' THEN b.CompanyCode ELSE '{3}' end)
                AND b.BranchCode = (CASE WHEN '{4}'='' THEN b.BranchCode ELSE '{4}' end)
                AND {5}", dateFrom, dateTo, area, dealer, outlet, (BeginCoupon != "" && EndCoupon != "") ? "a.CoupunNumber BETWEEN '" + BeginCoupon + "' AND '" + EndCoupon + "'" : "1=1");

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 3;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Inquiry Data Coupon");
                //First Names 
                ws.Range("A1:N1").Merge();
                ws.Cell("A1").Value = "REKAP KUPON TEST DRIVE " + dateFrom + " s/d " + dateTo;


                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public ActionResult inqIndentXls()
        {
            string area = Request["GroupArea"] ?? "";
            string comp = Request["CompanyCode"] ?? "";
            //string IndentNumber = "";
            string outl = Request["Outlet"];
            string dateFr = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string dateFrName = Request["DateFrName"];
            string dateToName = Request["DateToName"];
            string fileName = "";
            string areaName = "";
            string compName = "";
            //string outlName = "";

            var PeriodYear = Request["PeriodYear"];
            var PeriodMonth = Request["PeriodMonth"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal Month = Convert.ToInt32(PeriodMonth != "" ? PeriodMonth : "0");

            fileName = "Inquiry Indent";

            //if (outl != "")
            //{
            //    var Outlet = ctx.GnMstDealerOutletMappings.Where(a => a.DealerCode == comp && a.OutletCode == outl).FirstOrDefault();
            //    outlName = Outlet.OutletName.ToString();
            //}

            if (area != "")
            {
                var Outlet = ctx.GnMstDealerMappings.Where(a => a.GroupNo.ToString() == area).FirstOrDefault();
                areaName = Outlet.Area.ToString();
            }

            if (comp != "")
            {
                var Outlet = ctx.GnMstDealerMappings.Where(a => a.DealerCode == comp).FirstOrDefault();
                compName = Outlet.DealerName.ToString();
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_InqIndent_rev";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@GroupNo", area);
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            //cmd.Parameters.AddWithValue("@Outlet", outl);
            //cmd.Parameters.AddWithValue("@DateFrom", dateFr);
            //cmd.Parameters.AddWithValue("@DateTo", dateTo);
            //cmd.Parameters.AddWithValue("@IndentNumber", IndentNumber);
            cmd.Parameters.AddWithValue("@PeriodeYear", Year);
            cmd.Parameters.AddWithValue("@PeriodeMonth", Month);
            cmd.Parameters.AddWithValue("@Table", 3);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //return Json(GetJson(dt));
            //var header = new List<List<dynamic>>() { };
            //header.Add(new List<dynamic> { "Dealer", (comp == "") ? "ALL" : ctx.DealerInfos.Find(new string[] { comp }).DealerName });
            //header.Add(new List<dynamic> { "Cut Off", string.Format("{0}", DateTime.Now.ToString("dd-MMM-yyy HH:mm")) });
            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Inquiry Indent");
                ws.Columns("1").Width = 10;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
                ws.Cell("A" + lastRow).Value = "Area";
                ws.Cell("B" + lastRow).Value = area == "" ? ": ALL" : ": " + areaName;
                lastRow = 2;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
                ws.Cell("A" + lastRow).Value = "Dealer";
                ws.Cell("B" + lastRow).Value = comp == "" ? ": ALL" : ": " + compName; 
                lastRow = 3;
                //ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
                //ws.Cell("A" + lastRow).Value = "Outlet";
                //ws.Cell("B" + lastRow).Value =  outl == "" ? ": ALL" : ": " + outlName; 
                //lastRow = 4;
                //ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
                //ws.Cell("A" + lastRow).Value = "Period";
                //ws.Cell("B" + lastRow).Value = ": " + dateFrName + " s/d " + dateToName;
                //lastRow = 6;

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public ActionResult inqQuotaXls()
        {
            string area = Request["GroupArea"] ?? "";
            string comp = Request["CompanyCode"] ?? "";
            string outl = Request["Outlet"];
            string dateFr = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string dateFrName = Request["DateFrName"];
            string dateToName = Request["DateToName"];
            string fileName = "";
            string areaName = "";
            string compName = "";

            var PeriodYear = Request["PeriodYear"];
            var PeriodMonth = Request["PeriodMonth"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal Month = Convert.ToInt32(PeriodMonth != "" ? PeriodMonth : "0");

            fileName = "Inquiry Quota";

            if (area != "")
            {
                var Outlet = ctx.GnMstDealerMappings.Where(a => a.GroupNo.ToString() == area).FirstOrDefault();
                areaName = Outlet.Area.ToString();
            }

            if (comp != "")
            {
                var Outlet = ctx.GnMstDealerMappings.Where(a => a.DealerCode == comp).FirstOrDefault();
                compName = Outlet.DealerName.ToString();
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_InqIndent_rev";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@GroupNo", area);
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@PeriodeYear", Year);
            cmd.Parameters.AddWithValue("@PeriodeMonth", Month);
            cmd.Parameters.AddWithValue("@Table", 1);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Inquiry Indent");
                ws.Columns("1").Width = 10;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
                ws.Cell("A" + lastRow).Value = "Area";
                ws.Cell("B" + lastRow).Value = area == "" ? ": ALL" : ": " + areaName;
                lastRow = 2;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
                ws.Cell("A" + lastRow).Value = "Dealer";
                ws.Cell("B" + lastRow).Value = comp == "" ? ": ALL" : ": " + compName;
                lastRow = 3;

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public ActionResult SpLogReportXls() 
        {
            string area = Request["GroupArea"] ?? "";
            string comp = Request["CompanyCode"] ?? "";
            string mode = Request["Mode"];
            string outl = Request["Outlet"];
            string dateFr = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string dateFrName = Request["DateFrName"];
            string dateToName = Request["DateToName"];
            string fileName = "";
            string areaName = "";
            string compName = "";
            string outlName = "";
            var modeName = mode== "INQ" ? " Inquiry Mode" : " Selling Mode";
            fileName = "Log Report" + modeName;

            if (outl != "")
            {
                var Outlet = ctx.GnMstDealerOutletMappingNews.Where(a => a.DealerCode == comp && a.OutletCode == outl).FirstOrDefault();
                outlName = Outlet.OutletName.ToString();
            }

            if (area != "")
            {
                var Area = ctx.GroupAreas.Where(a => a.GroupNo.ToString() == area).FirstOrDefault();
                areaName = Area.AreaDealer.ToString();
            }

            if (comp != "")
            {
                var Outlet = ctx.GnMstDealerMappingNews.Where(a => a.DealerCode == comp).FirstOrDefault();
                compName = Outlet.DealerName.ToString();
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_SpGnrLogReport";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@GroupNo", area);
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@Outlet", outl);
            cmd.Parameters.AddWithValue("@DateFrom", dateFr);
            cmd.Parameters.AddWithValue("@DateTo", dateTo);
            cmd.Parameters.AddWithValue("@Mode", mode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            
            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Log Report");
                ws.Columns("1").Width = 10;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
                ws.Range("A" + lastRow + ":B" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = "Area";
                ws.Cell("C" + lastRow).Value = area == "" ? ": ALL" : ": " + areaName;
                lastRow = 2;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
                ws.Range("A" + lastRow + ":B" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = "Dealer";
                ws.Cell("C" + lastRow).Value = comp == "" ? ": ALL" : ": " + compName;
                lastRow = 3;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
                ws.Range("A" + lastRow + ":B" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = "Outlet";
                ws.Cell("C" + lastRow).Value = outl == "" ? ": ALL" : ": " + outlName;
                lastRow = 4;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
                ws.Range("A" + lastRow + ":B" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = "Period";
                ws.Cell("C" + lastRow).Value = ": " + dateFrName + " s/d " + dateToName;
                lastRow = 5;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
                ws.Range("A" + lastRow + ":B" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = mode == "INQ" ? "Inquiry Mode Log Report" : "Selling Mode Log Report";
                ws.Cell("C" + lastRow).Value = ": Generate on " + dateFrName ;
                lastRow = 7;

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public ActionResult GenerateITSReportAccumByPeriode()
        {
            string GroupArea = Request["GroupArea"] ?? "";
            string CompanyCode = Request["CompanyCode"] ?? "";
            string NewArea = "";

            if (CompanyCode.IndexOf("|") > 0)
            {
                string[] dd = CompanyCode.Split('|');
                NewArea = dd[0];
                CompanyCode = dd[1];
            }
            else
            {
                //if (CompanyCode == "")
                //{
                //    groupNo = Convert.ToInt32(GroupArea);
                //}
            }

            string AccumFrom1 = Request["AccumFrom1"]??"";
            string AccumTo1 = Request["AccumTo1"]??"";
            string AccumFrom2 = Request["AccumFrom2"]??"";
            string AccumTo2 = Request["AccumTo2"]??"";
            string AccumTo1Name = Request["AccumTo1Name"]??"";
            string AccumTo2Name = Request["AccumTo2Name"]??"";
            string fileName = "Inquiry Data Accum";

            string DateAccum1 = Request["DateAccum1"] ??"";
            string DateAccum2 = Request["DateAccum2"] ?? "";
            string DateAccum3 = Request["DateAccum3"] ?? "";
            var CompanyName = "";
            var AreaName = "";
            string dealercode="";

            var datefrom1 = DateTime.ParseExact(DateAccum1.Substring(0, 8).ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy");
            var dateto1 = DateTime.ParseExact(DateAccum1.Substring(DateAccum1.Length - 8, 8).ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy");
            var dateto1Label = DateTime.ParseExact(DateAccum1.Substring(DateAccum1.Length - 8, 8).ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MMM yyyy");
            var datefrom2 = DateTime.ParseExact(DateAccum2.Substring(0, 8).ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy");
            var dateto2 = DateTime.ParseExact(DateAccum2.Substring(DateAccum2.Length - 8, 8).ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy");
            var dateto2Label = DateTime.ParseExact(DateAccum2.Substring(DateAccum2.Length - 8, 8).ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MMM yyyy");

            int Area = Convert.ToInt32(GroupArea != "" ? GroupArea : "0");

            if (GroupArea != "" )
            {
                if (GroupArea == "100")
                {
                    var Outlet = ctx.GroupAreas.Where(a => a.GroupNo == GroupArea).FirstOrDefault();
                    AreaName = Outlet.AreaDealer.ToString();
                }
                else
                {
                    var Outlet = ctx.GnMstDealerMappingNews.Where(a => a.GroupNo == Area).FirstOrDefault();
                    AreaName = Outlet.Area.ToString();
                }
            }

            if (CompanyCode != "")
            {
                //var Outlet = ctx.GnMstDealerMappingNews.Where(a => a.DealerCode == CompanyCode && a.GroupNo.ToString() == NewArea).FirstOrDefault();
                //string[] Code = CompanyCode.Split('|');

                //dealercode = Code[1].ToString();
                var Outlet = ctx.GnMstDealerMappings.Where(a => a.DealerCode == CompanyCode).FirstOrDefault();
                //var Outlet = ctx.GnMstDealerMappings.Where(a => a.DealerCode == dealercode).FirstOrDefault();
                CompanyName = Outlet.DealerName.ToString();
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GenerateITSReportAccumNew";
            //cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GenerateITSReportAccum";
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GenerateITSReportAccum_New";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Area", GroupArea);
            //cmd.Parameters.AddWithValue("@Dealer", CompanyCode);
            cmd.Parameters.AddWithValue("@Company", CompanyCode);
            cmd.Parameters.AddWithValue("@DateAccum1", DateAccum1);
            cmd.Parameters.AddWithValue("@DateAccum2", DateAccum2);
            cmd.Parameters.AddWithValue("@DateAccum3", DateAccum3);
            //cmd.Parameters.AddWithValue("@AreaNew", NewArea);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            da.Fill(ds);
            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];
            dt3 = ds.Tables[2];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Inquiry Data Accum");

                ws.Cell("A" + lastRow).Value = "Tanggal Cetak: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + CurrentUser.Username;
                lastRow = 3;

                ws.Columns("1").Width = 10;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A" + lastRow).Value = "Area";
                ws.Cell("B" + lastRow).Value = GroupArea == "" ? ": ALL" : ": " + AreaName;
                ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                lastRow = 4;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A" + lastRow).Value = "Dealer";
                ws.Cell("B" + lastRow).Value = CompanyCode == "" ? ": ALL" : ": " + CompanyName;
                //ws.Cell("B" + lastRow).Value = dealercode == "" ? ": ALL" : ": " + CompanyName;
                ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                lastRow = 5;
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A" + lastRow).Value = "Periode";
                ws.Cell("B" + lastRow).Value = ": Accum " + datefrom2 + " s/d " + dateto2 + " & Accum " + datefrom1 + " s/d " + dateto1;
                ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                lastRow = 7;

                #region ALL Area
                //Table Header
                ws.Range("A" + lastRow + ":T" + (lastRow + 1)).Style.Fill.SetBackgroundColor(XLColor.FromArgb(184, 204, 228));
                ws.Range("A" + lastRow + ":T" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Range("A" + lastRow + ":T" + (lastRow + 1)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                ws.Column("A").Width = 30.00;
                ws.Column("B").Width = 30.00;
                ws.Columns(3, 10).Width = 15.00;
                ws.Columns(11, 12).Width = 20.00;
                ws.Columns(14, 15).Width = 20.00;
                ws.Columns(16, 17).Width = 12.00;
                ws.Columns(18, 19).Width = 20.00;

                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.BottomBorder = XLBorderStyleValues.Double;

                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Font.FontSize = 12;
                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Font.SetBold();
                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Font.FontSize = 12;
                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Font.SetBold();

                ws.Cell("A" + lastRow).Value = "AREA";
                ws.Range("A" + lastRow + ":A" + (lastRow + 1)).Merge();
                ws.Cell("A" + lastRow).Style.Alignment.WrapText = true;

                ws.Cell("B" + lastRow).Value = "MODEL";
                ws.Range("B" + lastRow + ":B" + (lastRow + 1)).Merge();
                ws.Cell("B" + lastRow).Style.Alignment.WrapText = true;

                ws.Cell("C" + lastRow).Value = "(Accum " + dateto1Label + ")";
                ws.Range("C" + lastRow + ":D" + lastRow).Merge();

                ws.Cell("C" + (lastRow + 1)).Value = "INQUIRY";
                ws.Cell("D" + (lastRow + 1)).Value = "SPK";

                ws.Cell("E" + lastRow).Value = "(Accum " + dateto2Label + ")";
                ws.Range("E" + lastRow + ":F" + lastRow).Merge();

                ws.Cell("E" + (lastRow + 1)).Value = "INQUIRY";
                ws.Cell("F" + (lastRow + 1)).Value = "SPK";

                ws.Cell("G" + lastRow).Value = "(" + dateto2Label + " - " + dateto1Label + ")";
                ws.Range("G" + lastRow + ":H" + lastRow).Merge();

                ws.Cell("G" + (lastRow + 1)).Value = "Diff INQ";
                ws.Cell("H" + (lastRow + 1)).Value = "Diff SPK";

                ws.Cell("I" + lastRow).Value = "Avg 3 Month";
                ws.Range("I" + lastRow + ":J" + lastRow).Merge();

                ws.Cell("I" + (lastRow + 1)).Value = "INQUIRY";
                ws.Cell("J" + (lastRow + 1)).Value = "SPK";

                ws.Cell("K" + lastRow).Value = "%SPK to INQ";
                ws.Range("K" + lastRow + ":L" + lastRow).Merge();

                ws.Cell("K" + (lastRow + 1)).Value = "(Accum " + dateto1Label + ")";
                ws.Cell("L" + (lastRow + 1)).Value = "(Accum " + dateto2Label + ")";

                ws.Cell("M" + lastRow).Value = "Diff";
                ws.Cell("M" + (lastRow + 1)).Value = "(%)";

                ws.Cell("N" + lastRow).Value = "FAKTUR";
                ws.Range("N" + lastRow + ":O" + lastRow).Merge();

                ws.Cell("N" + (lastRow + 1)).Value = "(Accum " + dateto1Label + ")";
                ws.Cell("O" + (lastRow + 1)).Value = "(Accum " + dateto2Label + ")";

                ws.Cell("P" + lastRow).Value = "DIFF FAKTUR";
                ws.Range("P" + lastRow + ":P" + (lastRow + 1)).Merge();

                ws.Cell("Q" + lastRow).Value = "Avg 3 Month";
                ws.Cell("Q" + (lastRow + 1)).Value = "Faktur";

                ws.Cell("R" + lastRow).Value = "%FAKTUR to SPK";
                ws.Range("R" + lastRow + ":S" + lastRow).Merge();

                ws.Cell("R" + (lastRow + 1)).Value = "(Accum " + dateto1Label + ")";
                ws.Cell("S" + (lastRow + 1)).Value = "(Accum " + dateto2Label + ")";

                ws.Cell("T" + lastRow).Value = "Diff";
                ws.Cell("T" + (lastRow + 1)).Value = "(%)";

                lastRow++;
                int Count = 0;
                var AreaGroup = "";
                var Grouping1 = lastRow + 1;
                var Grouping2 = 0;
                decimal TotalInq1 = 0, TotalInq2 = 0, TotalSpk1 = 0, TotalSpk2 = 0, TotalFak1 = 0, TotalFak2 = 0, TotalAvgInq = 0, TotalAvgSpk = 0, TotalAvgFak = 0; 
                foreach (DataRow row in dt1.Rows)
                {
                    if (Count == 0) { Count = dt1.Rows.Count; }
                    if (AreaGroup != "" && AreaGroup != (row["Area"].ToString()))
                    {
                        Grouping2 = lastRow;
                        ws.Range("A" + Grouping1 + ":A" + Grouping2).Merge();
                        ws.Cell("A" + Grouping1).Style.Alignment.WrapText = true;
                        ws.Range("A" + Grouping1 + ":A" + Grouping2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Range("A" + Grouping1 + ":A" + Grouping2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        ws.Range("K" + Grouping1, "K" + Grouping2).AddConditionalFormat().DataBar(XLColor.Red, false)
                        .LowestValue()
                        .Maximum(XLCFContentType.Percent, "100");
                        ws.Range("L" + Grouping1, "L" + Grouping2).AddConditionalFormat().DataBar(XLColor.Blue, false)
                        .LowestValue()
                        .Maximum(XLCFContentType.Percent, "100");

                        ws.Range("R" + Grouping1, "R" + Grouping2).AddConditionalFormat().DataBar(XLColor.Red, false)
                        .LowestValue()
                        .Maximum(XLCFContentType.Percent, "100");
                        ws.Range("S" + Grouping1, "S" + Grouping2).AddConditionalFormat().DataBar(XLColor.Blue, false)
                        .LowestValue()
                        .Maximum(XLCFContentType.Percent, "100");

                        ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("A" + (lastRow + 1)).Value = "Total " + AreaGroup;
                        ws.Cell("A" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        ws.Cell("B" + (lastRow + 1)).Value = "";
                        ws.Cell("C" + (lastRow + 1)).Value = TotalInq1;
                        ws.Cell("D" + (lastRow + 1)).Value = TotalSpk1;
                        ws.Cell("E" + (lastRow + 1)).Value = TotalInq2;
                        ws.Cell("F" + (lastRow + 1)).Value = TotalSpk2;
                        ws.Cell("G" + (lastRow + 1)).SetFormulaA1("=C" + (lastRow + 1) + "-E" + (lastRow + 1));
                        ws.Cell("H" + (lastRow + 1)).SetFormulaA1("=D" + (lastRow + 1) + "-F" + (lastRow + 1));
                        ws.Cell("I" + (lastRow + 1)).Value = TotalAvgInq;
                        ws.Cell("J" + (lastRow + 1)).Value = TotalAvgSpk;
                        ws.Cell("K" + (lastRow + 1)).SetFormulaA1("=IFERROR(D" + (lastRow + 1) + "/C" + (lastRow + 1) + ",0%)");
                        ws.Cell("L" + (lastRow + 1)).SetFormulaA1("=IFERROR(F" + (lastRow + 1) + "/E" + (lastRow + 1) + ",0%)");

                        ws.Range("K" + (lastRow + 1), "L" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                        ws.Range("K" + (lastRow + 1), "L" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        ws.Cell("M" + (lastRow + 1)).SetFormulaA1("=K" + (lastRow + 1) + "-L" + (lastRow + 1));
                        ws.Cell("M" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                        ws.Cell("M" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        ws.Cell("N" + (lastRow + 1)).Value = TotalFak1;
                        ws.Cell("O" + (lastRow + 1)).Value = TotalFak2;
                        ws.Cell("P" + (lastRow + 1)).SetFormulaA1("=N" + (lastRow + 1) + "-O" + (lastRow + 1));
                        ws.Cell("Q" + (lastRow + 1)).Value = TotalAvgFak;
                        ws.Cell("R" + (lastRow + 1)).SetFormulaA1("=IFERROR(N" + (lastRow + 1) + "/D" + (lastRow + 1) + ",0%)");
                        ws.Cell("S" + (lastRow + 1)).SetFormulaA1("=IFERROR(O" + (lastRow + 1) + "/F" + (lastRow + 1) + ",0%)");

                        ws.Range("R" + (lastRow + 1), "S" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                        ws.Range("R" + (lastRow + 1), "S" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        ws.Cell("T" + (lastRow + 1)).SetFormulaA1("=R" + (lastRow + 1) + "-S" + (lastRow + 1));
                        ws.Cell("T" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                        ws.Cell("T" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        ws.Range("A" + (lastRow + 1), "T" + (lastRow + 1)).Style.Fill.BackgroundColor = XLColor.Gray;

                        TotalInq1 = 0; TotalInq2 = 0; TotalSpk1 = 0; TotalSpk2 = 0; TotalFak1 = 0; TotalFak2 = 0; TotalAvgInq = 0; TotalAvgSpk = 0; TotalAvgFak = 0; 
                        lastRow++;
                        Count++;
                        Grouping1 = lastRow + 1;

                    }
                    ws.Cell("A" + (lastRow + 1)).Value = row["Area"].ToString();
                    ws.Cell("A" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Cell("A" + (lastRow + 1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

                    ws.Cell("B" + (lastRow + 1)).Value = row["TipeKendaraan"].ToString();
                    ws.Cell("C" + (lastRow + 1)).Value = row["inq1"].ToString();
                    ws.Cell("D" + (lastRow + 1)).Value = row["spk1"].ToString();
                    ws.Cell("E" + (lastRow + 1)).Value = row["inq2"].ToString();
                    ws.Cell("F" + (lastRow + 1)).Value = row["spk2"].ToString();
                    ws.Cell("G" + (lastRow + 1)).SetFormulaA1("=C" + (lastRow + 1) + "-E" + (lastRow + 1));
                    ws.Cell("H" + (lastRow + 1)).SetFormulaA1("=D" + (lastRow + 1) + "-F" + (lastRow + 1));
                    ws.Cell("I" + (lastRow + 1)).Value = row["AvgInq"].ToString();
                    ws.Cell("J" + (lastRow + 1)).Value = row["AvgSpk"].ToString();
                    ws.Cell("K" + (lastRow + 1)).SetFormulaA1("=IFERROR(D" + (lastRow + 1) + "/C" + (lastRow + 1) + ",0%)");
                    ws.Cell("L" + (lastRow + 1)).SetFormulaA1("=IFERROR(F" + (lastRow + 1) + "/E" + (lastRow + 1) + ",0%)");

                    ws.Range("K" + (lastRow + 1), "L" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                    ws.Range("K" + (lastRow + 1), "L" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell("M" + (lastRow + 1)).SetFormulaA1("=K" + (lastRow + 1) + "-L" + (lastRow + 1));
                    ws.Cell("M" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                    ws.Cell("M" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell("N" + (lastRow + 1)).Value = row["Fak1"].ToString();
                    ws.Cell("O" + (lastRow + 1)).Value = row["Fak2"].ToString();
                    ws.Cell("P" + (lastRow + 1)).SetFormulaA1("=N" + (lastRow + 1) + "-O" + (lastRow + 1));
                    ws.Cell("Q" + (lastRow + 1)).Value = row["AvgFak"].ToString();
                    ws.Cell("R" + (lastRow + 1)).SetFormulaA1("=IFERROR(N" + (lastRow + 1) + "/D" + (lastRow + 1) + ",0%)");
                    ws.Cell("S" + (lastRow + 1)).SetFormulaA1("=IFERROR(O" + (lastRow + 1) + "/F" + (lastRow + 1) + ",0%)");

                    ws.Range("R" + (lastRow + 1), "S" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                    ws.Range("R" + (lastRow + 1), "S" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell("T" + (lastRow + 1)).SetFormulaA1("=R" + (lastRow + 1) + "-S" + (lastRow + 1));
                    ws.Cell("T" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                    ws.Cell("T" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Cell("T" + (lastRow + 1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                    AreaGroup = row["Area"].ToString();
                    TotalInq1 += Convert.ToDecimal(row["inq1"].ToString());
                    TotalInq2 += Convert.ToDecimal(row["inq2"].ToString());
                    TotalSpk1 += Convert.ToDecimal(row["spk1"].ToString());
                    TotalSpk2 += Convert.ToDecimal(row["spk2"].ToString());
                    TotalFak1 += Convert.ToDecimal(row["Fak1"].ToString());
                    TotalFak2 += Convert.ToDecimal(row["Fak2"].ToString());
                    TotalAvgInq += Convert.ToDecimal(row["AvgInq"].ToString());
                    TotalAvgSpk += Convert.ToDecimal(row["AvgSpk"].ToString());
                    TotalAvgFak += Convert.ToDecimal(row["AvgFak"].ToString());

                    lastRow++;
                    var BatasAkhir = lastRow - 8;
                    if (BatasAkhir == Count)
                    {
                        Grouping2 = lastRow;
                        ws.Range("A" + Grouping1 + ":A" + Grouping2).Merge();
                        ws.Cell("A" + Grouping1).Style.Alignment.WrapText = true;
                        ws.Range("A" + Grouping1 + ":A" + Grouping2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Range("A" + Grouping1 + ":A" + Grouping2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        ws.Range("K" + Grouping1, "K" + Grouping2).AddConditionalFormat().DataBar(XLColor.Red, false)
                        .LowestValue()
                        .Maximum(XLCFContentType.Percent, "100");
                        ws.Range("L" + Grouping1, "L" + Grouping2).AddConditionalFormat().DataBar(XLColor.Blue, false)
                        .LowestValue()
                        .Maximum(XLCFContentType.Percent, "100");
                        ws.Range("R" + Grouping1, "R" + Grouping2).AddConditionalFormat().DataBar(XLColor.Red, false)
                        .LowestValue()
                        .Maximum(XLCFContentType.Percent, "100");
                        ws.Range("S" + Grouping1, "S" + Grouping2).AddConditionalFormat().DataBar(XLColor.Blue, false)
                        .LowestValue()
                        .Maximum(XLCFContentType.Percent, "100");

                        ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("A" + (lastRow + 1)).Value = "Total " + AreaGroup;
                        ws.Cell("A" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        ws.Cell("B" + (lastRow + 1)).Value = "";
                        ws.Cell("C" + (lastRow + 1)).Value = TotalInq1;
                        ws.Cell("D" + (lastRow + 1)).Value = TotalSpk1;
                        ws.Cell("E" + (lastRow + 1)).Value = TotalInq2;
                        ws.Cell("F" + (lastRow + 1)).Value = TotalSpk2;
                        ws.Cell("G" + (lastRow + 1)).SetFormulaA1("=C" + (lastRow + 1) + "-E" + (lastRow + 1));
                        ws.Cell("H" + (lastRow + 1)).SetFormulaA1("=D" + (lastRow + 1) + "-F" + (lastRow + 1));
                        ws.Cell("I" + (lastRow + 1)).Value = TotalAvgInq;
                        ws.Cell("J" + (lastRow + 1)).Value = TotalAvgSpk;
                        ws.Cell("K" + (lastRow + 1)).SetFormulaA1("=IFERROR(D" + (lastRow + 1) + "/C" + (lastRow + 1) + ",0%)");
                        ws.Cell("L" + (lastRow + 1)).SetFormulaA1("=IFERROR(F" + (lastRow + 1) + "/E" + (lastRow + 1) + ",0%)");

                        ws.Range("K" + (lastRow + 1), "L" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                        ws.Range("K" + (lastRow + 1), "L" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        ws.Cell("M" + (lastRow + 1)).SetFormulaA1("=K" + (lastRow + 1) + "-L" + (lastRow + 1));
                        ws.Cell("M" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                        ws.Cell("M" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        ws.Cell("N" + (lastRow + 1)).Value = TotalFak1;
                        ws.Cell("O" + (lastRow + 1)).Value = TotalFak2;
                        ws.Cell("P" + (lastRow + 1)).SetFormulaA1("=N" + (lastRow + 1) + "-O" + (lastRow + 1));
                        ws.Cell("Q" + (lastRow + 1)).Value = TotalAvgFak;
                        ws.Cell("R" + (lastRow + 1)).SetFormulaA1("=IFERROR(N" + (lastRow + 1) + "/D" + (lastRow + 1) + ",0%)");
                        ws.Cell("S" + (lastRow + 1)).SetFormulaA1("=IFERROR(O" + (lastRow + 1) + "/F" + (lastRow + 1) + ",0%)");

                        ws.Range("R" + (lastRow + 1), "S" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                        ws.Range("R" + (lastRow + 1), "S" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        ws.Cell("T" + (lastRow + 1)).SetFormulaA1("=R" + (lastRow + 1) + "-S" + (lastRow + 1));
                        ws.Cell("T" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                        ws.Cell("T" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        ws.Range("A" + (lastRow + 1), "T" + (lastRow + 1)).Style.Fill.BackgroundColor = XLColor.Gray;

                        TotalInq1 = 0; TotalInq2 = 0; TotalSpk1 = 0; TotalSpk2 = 0; TotalFak1 = 0; TotalFak2 = 0; TotalAvgInq = 0; TotalAvgSpk = 0; TotalAvgFak = 0;
                        lastRow++;
                        Count++;
                        Grouping1 = lastRow + 1;

                    }

                }

                #endregion

                #region Nasional
                lastRow = lastRow + 2;
                //Table Header
                ws.Range("A" + lastRow + ":T" + (lastRow + 1)).Style.Fill.SetBackgroundColor(XLColor.FromArgb(184, 204, 228));
                ws.Range("A" + lastRow + ":T" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Range("A" + lastRow + ":T" + (lastRow + 1)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //ws.Column("A").Width = 30.00;
                //ws.Column("B").Width = 30.00;
                //ws.Columns(3, 10).Width = 15.00;
                //ws.Columns(11, 12).Width = 20.00;
                //ws.Columns(14, 15).Width = 20.00;
                //ws.Columns(16, 17).Width = 12.00;
                //ws.Columns(18, 19).Width = 20.00;

                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.BottomBorder = XLBorderStyleValues.Double;

                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Font.FontSize = 12;
                ws.Range("A" + lastRow + ":" + "T" + lastRow).Style.Font.SetBold();
                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Font.FontSize = 12;
                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Font.SetBold();

                ws.Cell("A" + lastRow).Value = "AREA";
                ws.Range("A" + lastRow + ":A" + (lastRow + 1)).Merge();
                ws.Cell("A" + lastRow).Style.Alignment.WrapText = true;

                ws.Cell("B" + lastRow).Value = "MODEL";
                ws.Range("B" + lastRow + ":B" + (lastRow + 1)).Merge();
                ws.Cell("B" + lastRow).Style.Alignment.WrapText = true;

                ws.Cell("C" + lastRow).Value = "(Accum " + dateto1Label + ")";
                ws.Range("C" + lastRow + ":D" + lastRow).Merge();

                ws.Cell("C" + (lastRow + 1)).Value = "INQUIRY";
                ws.Cell("D" + (lastRow + 1)).Value = "SPK";

                ws.Cell("E" + lastRow).Value = "(Accum " + dateto2 + ")";
                ws.Range("E" + lastRow + ":F" + lastRow).Merge();

                ws.Cell("E" + (lastRow + 1)).Value = "INQUIRY";
                ws.Cell("F" + (lastRow + 1)).Value = "SPK";

                ws.Cell("G" + lastRow).Value = "(" + dateto1Label + " - " + dateto2Label + ")";
                ws.Range("G" + lastRow + ":H" + lastRow).Merge();

                ws.Cell("G" + (lastRow + 1)).Value = "Diff INQ";
                ws.Cell("H" + (lastRow + 1)).Value = "Diff SPK";

                ws.Cell("I" + lastRow).Value = "Avg 3 Month";
                ws.Range("I" + lastRow + ":J" + lastRow).Merge();

                ws.Cell("I" + (lastRow + 1)).Value = "INQUIRY";
                ws.Cell("J" + (lastRow + 1)).Value = "SPK";

                ws.Cell("K" + lastRow).Value = "%SPK to INQ";
                ws.Range("K" + lastRow + ":L" + lastRow).Merge();

                ws.Cell("K" + (lastRow + 1)).Value = "(Accum " + dateto1Label + ")";
                ws.Cell("L" + (lastRow + 1)).Value = "(Accum " + dateto2Label + ")";

                ws.Cell("M" + lastRow).Value = "Diff";
                ws.Cell("M" + (lastRow + 1)).Value = "(%)";

                ws.Cell("N" + lastRow).Value = "FAKTUR";
                ws.Range("N" + lastRow + ":O" + lastRow).Merge();

                ws.Cell("N" + (lastRow + 1)).Value = "(Accum " + dateto1Label + ")";
                ws.Cell("O" + (lastRow + 1)).Value = "(Accum " + dateto2Label + ")";

                ws.Cell("P" + lastRow).Value = "DIFF FUKT";
                ws.Range("P" + lastRow + ":P" + (lastRow + 1)).Merge();

                ws.Cell("Q" + lastRow).Value = "Avg 3 Month";
                ws.Cell("Q" + (lastRow + 1)).Value = "Faktur";

                ws.Cell("R" + lastRow).Value = "%FAKTUR to SPK";
                ws.Range("R" + lastRow + ":S" + lastRow).Merge();

                ws.Cell("R" + (lastRow + 1)).Value = "(Accum " + dateto1Label + ")";
                ws.Cell("S" + (lastRow + 1)).Value = "(Accum " + dateto2Label + ")";

                ws.Cell("T" + lastRow).Value = "Diff";
                ws.Cell("T" + (lastRow + 1)).Value = "(%)";

                lastRow++;
                foreach (DataRow row in dt2.Rows)
                {

                    ws.Cell("A" + (lastRow + 1)).Value = row["Area"].ToString();
                    ws.Cell("A" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Cell("A" + (lastRow + 1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

                    ws.Cell("B" + (lastRow + 1)).Value = row["TipeKendaraan"].ToString();
                    ws.Cell("C" + (lastRow + 1)).Value = row["inq1"].ToString();
                    ws.Cell("D" + (lastRow + 1)).Value = row["spk1"].ToString();
                    ws.Cell("E" + (lastRow + 1)).Value = row["inq2"].ToString();
                    ws.Cell("F" + (lastRow + 1)).Value = row["spk2"].ToString();
                    ws.Cell("G" + (lastRow + 1)).SetFormulaA1("=C" + (lastRow + 1) + "-E" + (lastRow + 1));
                    ws.Cell("H" + (lastRow + 1)).SetFormulaA1("=D" + (lastRow + 1) + "-F" + (lastRow + 1));
                    ws.Cell("I" + (lastRow + 1)).Value = row["AvgInq"].ToString();
                    ws.Cell("J" + (lastRow + 1)).Value = row["AvgSpk"].ToString();
                    ws.Cell("K" + (lastRow + 1)).SetFormulaA1("=IFERROR(D" + (lastRow + 1) + "/C" + (lastRow + 1) + ",0%)");
                    ws.Cell("L" + (lastRow + 1)).SetFormulaA1("=IFERROR(F" + (lastRow + 1) + "/E" + (lastRow + 1) + ",0%)");

                    ws.Range("K" + (lastRow + 1), "L" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                    ws.Range("K" + (lastRow + 1), "L" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell("M" + (lastRow + 1)).SetFormulaA1("=K" + (lastRow + 1) + "-L" + (lastRow + 1));
                    ws.Cell("M" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                    ws.Cell("M" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell("N" + (lastRow + 1)).Value = row["Fak1"].ToString();
                    ws.Cell("O" + (lastRow + 1)).Value = row["Fak2"].ToString();
                    ws.Cell("P" + (lastRow + 1)).SetFormulaA1("=N" + (lastRow + 1) + "-O" + (lastRow + 1));
                    ws.Cell("Q" + (lastRow + 1)).Value = row["AvgFak"].ToString();
                    ws.Cell("R" + (lastRow + 1)).SetFormulaA1("=IFERROR(N" + (lastRow + 1) + "/D" + (lastRow + 1) + ",0%)");
                    ws.Cell("S" + (lastRow + 1)).SetFormulaA1("=IFERROR(O" + (lastRow + 1) + "/F" + (lastRow + 1) + ",0%)");

                    ws.Range("R" + (lastRow + 1), "S" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                    ws.Range("R" + (lastRow + 1), "S" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell("T" + (lastRow + 1)).SetFormulaA1("=R" + (lastRow + 1) + "-S" + (lastRow + 1));
                    ws.Cell("T" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                    ws.Cell("T" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Cell("T" + (lastRow + 1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                    AreaGroup = row["Area"].ToString();
                    TotalInq1 += Convert.ToDecimal(row["inq1"].ToString());
                    TotalInq2 += Convert.ToDecimal(row["inq2"].ToString());
                    TotalSpk1 += Convert.ToDecimal(row["spk1"].ToString());
                    TotalSpk2 += Convert.ToDecimal(row["spk2"].ToString());
                    TotalFak1 += Convert.ToDecimal(row["Fak1"].ToString());
                    TotalFak2 += Convert.ToDecimal(row["Fak2"].ToString());
                    TotalAvgInq += Convert.ToDecimal(row["AvgInq"].ToString());
                    TotalAvgSpk += Convert.ToDecimal(row["AvgSpk"].ToString());
                    TotalAvgFak += Convert.ToDecimal(row["AvgFak"].ToString());

                    lastRow++;
                }
                Grouping1 = (lastRow - dt2.Rows.Count) + 1;
                Grouping2 = lastRow;
                ws.Range("A" + Grouping1 + ":A" + Grouping2).Merge();
                ws.Cell("A" + Grouping1).Style.Alignment.WrapText = true;
                ws.Range("A" + Grouping1 + ":A" + Grouping2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Range("A" + Grouping1 + ":A" + Grouping2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                ws.Range("K" + Grouping1, "K" + Grouping2).AddConditionalFormat().DataBar(XLColor.Red, false)
                .LowestValue()
                .Maximum(XLCFContentType.Percent, "100");
                ws.Range("L" + Grouping1, "L" + Grouping2).AddConditionalFormat().DataBar(XLColor.Blue, false)
                .LowestValue()
                .Maximum(XLCFContentType.Percent, "100");
                ws.Range("R" + Grouping1, "R" + Grouping2).AddConditionalFormat().DataBar(XLColor.Red, false)
                .LowestValue()
                .Maximum(XLCFContentType.Percent, "100");
                ws.Range("S" + Grouping1, "S" + Grouping2).AddConditionalFormat().DataBar(XLColor.Blue, false)
                .LowestValue()
                .Maximum(XLCFContentType.Percent, "100");

                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + (lastRow + 1) + ":" + "T" + (lastRow + 1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("A" + (lastRow + 1)).Value = "Total " + AreaGroup;
                ws.Cell("A" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("B" + (lastRow + 1)).Value = "";
                ws.Cell("C" + (lastRow + 1)).Value = TotalInq1;
                ws.Cell("D" + (lastRow + 1)).Value = TotalSpk1;
                ws.Cell("E" + (lastRow + 1)).Value = TotalInq2;
                ws.Cell("F" + (lastRow + 1)).Value = TotalSpk2;
                ws.Cell("G" + (lastRow + 1)).SetFormulaA1("=C" + (lastRow + 1) + "-E" + (lastRow + 1));
                ws.Cell("H" + (lastRow + 1)).SetFormulaA1("=D" + (lastRow + 1) + "-F" + (lastRow + 1));
                ws.Cell("I" + (lastRow + 1)).Value = TotalAvgInq;
                ws.Cell("J" + (lastRow + 1)).Value = TotalAvgSpk;
                ws.Cell("K" + (lastRow + 1)).SetFormulaA1("=IFERROR(D" + (lastRow + 1) + "/C" + (lastRow + 1) + ",0%)");
                ws.Cell("L" + (lastRow + 1)).SetFormulaA1("=IFERROR(F" + (lastRow + 1) + "/E" + (lastRow + 1) + ",0%)");

                ws.Range("K" + (lastRow + 1), "L" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                ws.Range("K" + (lastRow + 1), "L" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("M" + (lastRow + 1)).SetFormulaA1("=K" + (lastRow + 1) + "-L" + (lastRow + 1));
                ws.Cell("M" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                ws.Cell("M" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("N" + (lastRow + 1)).Value = TotalFak1;
                ws.Cell("O" + (lastRow + 1)).Value = TotalFak2;
                ws.Cell("P" + (lastRow + 1)).SetFormulaA1("=N" + (lastRow + 1) + "-O" + (lastRow + 1));
                ws.Cell("Q" + (lastRow + 1)).Value = TotalAvgFak;
                ws.Cell("R" + (lastRow + 1)).SetFormulaA1("=IFERROR(N" + (lastRow + 1) + "/D" + (lastRow + 1) + ",0%)");
                ws.Cell("S" + (lastRow + 1)).SetFormulaA1("=IFERROR(O" + (lastRow + 1) + "/F" + (lastRow + 1) + ",0%)");

                ws.Range("R" + (lastRow + 1), "S" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                ws.Range("R" + (lastRow + 1), "S" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("T" + (lastRow + 1)).SetFormulaA1("=R" + (lastRow + 1) + "-S" + (lastRow + 1));
                ws.Cell("T" + (lastRow + 1)).Style.NumberFormat.Format = "0.0%";
                ws.Cell("T" + (lastRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Range("A" + (lastRow + 1), "T" + (lastRow + 1)).Style.Fill.BackgroundColor = XLColor.Gray;
                lastRow++;
                #endregion

                lastRow = lastRow + 2;
                foreach (DataRow row in dt3.Rows)
                {
                    ws.Cell("A" + lastRow).Value = row["Accum"].ToString();
                    ws.Cell("B" + lastRow).Value = row["Tanggal"].ToString();
                    lastRow++;
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
        }

        public ActionResult GenerateITSReportRekapInqSpkFak()
        {
            string StartDate = Request["StartDate"];
            string EndDate = Request["EndDate"];
            string TipeKendaraan = Request["TipeKendaraan"] ?? "";
            string GroupArea = Request["GroupArea"] ?? "";
            string CompanyCode = Request["CompanyCode"] ?? "";
            string AccumTo1Name = Request["AccumTo1Name"];
            string fileName = "Rekap Inquiry SPK Faktur";
            //var CompanyName = "";
            //var AreaName = "";

            //int Area = Convert.ToInt32(GroupArea != "" ? GroupArea : "0");

            //if (GroupArea != "")
            //{
            //    var Outlet = ctx.GnMstDealerMappings.Where(a => a.GroupNo.ToString() == GroupArea).FirstOrDefault();
            //    AreaName = Outlet.Area.ToString();
            //}

            //if (CompanyCode != "")
            //{
            //    var Outlet = ctx.GnMstDealerMappings.Where(a => a.DealerCode == CompanyCode).FirstOrDefault();
            //    CompanyName = Outlet.DealerName.ToString();
            //}

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GenerateITSRekapInqSpkFak";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@TipeKendaraan", TipeKendaraan);
            cmd.Parameters.AddWithValue("@Area", "0");
            cmd.Parameters.AddWithValue("@Dealer", "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            DataTable dt4 = new DataTable();
            DataTable dt5 = new DataTable();
            DataTable dt6 = new DataTable();
            da.Fill(ds);
            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];
            dt3 = ds.Tables[2];
            dt4= ds.Tables[3];
            dt5 = ds.Tables[4];
            dt6 = ds.Tables[5];

            if (dt3.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;
                int lastColHeaderType = 1;
                int FristColType = 0;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Rekap Inquiry SPK Faktur");

                ws.Cell("A" + lastRow).Value = "Tanggal Cetak: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + CurrentUser.Username;
                lastRow = lastRow + 2;

                ws.Columns("1").Width = 10;
                ws.Range("A" + lastRow + ":J" + lastRow).Merge();
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A" + lastRow).Value = "Rekap Inquiry SPK Faktur " + TipeKendaraan + " Acc " + AccumTo1Name;
                lastRow = 5;

                ws.Column(lastColHeaderType).Width = 50;
                ws.Cell(lastRow, lastColHeaderType).Value = "DEALER";
                ws.Range(lastRow, lastColHeaderType, lastRow + 1, lastColHeaderType).Merge();
                ws.Range(lastRow, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Range(lastRow, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                int awalRow = lastRow;
                int awalcOL = lastColHeaderType;
                lastColHeaderType++;

                foreach (DataRow row1 in dt1.Rows)
                {
                    FristColType = lastColHeaderType;
                    ws.Cell(lastRow, lastColHeaderType).Value = row1["Type"].ToString();
                    foreach (DataRow row2 in dt2.Rows)
                    {
                        ws.Cell(lastRow + 1, lastColHeaderType).Value = row2["TypeCode"].ToString();
                        ws.Cell(lastRow + 1, lastColHeaderType).Style.Alignment.WrapText = true;
                        ws.Range(lastRow + 1, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        ws.Range(lastRow + 1, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        lastColHeaderType++;
                    }
                    ws.Range(lastRow, FristColType, lastRow, lastColHeaderType - 1).Merge();
                    ws.Range(lastRow, FristColType, lastRow, lastColHeaderType - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Fill.BackgroundColor = XLColor.BabyBlue;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                lastRow = lastRow + 2;

                var seqno = lastRow - 1;
                var OldDealer = "";
                var Type = "";
                var AreaGroup = "";
                var a = 0;
                var pertama = 0;
                var TtlDlr = 0;
                foreach (DataRow row3 in dt3.Rows)
                {
                    Type = "" + row3[dt3.Columns[1].Caption];
                    if (AreaGroup == "" || AreaGroup == "'" + row3[dt3.Columns[0].Caption])
                    {
                        #region RincianAllDealer
                        if (OldDealer == "'" + row3[dt3.Columns[2].Caption])
                        {
                            if (Type == "2-SPK")
                            {
                                for (int i = 3; i < dt3.Columns.Count; i++)
                                {
                                    var caption = dt3.Columns[i].Caption;
                                    a = dt2.Rows.Count + i;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Value = "" + row3[caption];
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                }
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).SetFormulaA1("=SUM(" + GetExcelColumnName(dt2.Rows.Count + 2) + seqno + ":" + GetExcelColumnName(a - 1) + seqno + ")");
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Font.SetBold();
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            }
                            if (Type == "3-FAK")
                            {
                                for (int i = 3; i < dt3.Columns.Count; i++)
                                {
                                    var caption = dt3.Columns[i].Caption;
                                    a = dt2.Rows.Count + dt2.Rows.Count + i;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Value = "" + row3[caption];
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                }
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).SetFormulaA1("=SUM(" + GetExcelColumnName(dt2.Rows.Count + dt2.Rows.Count + 2) + seqno + ":" + GetExcelColumnName(a - 1) + seqno + ")");
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Font.SetBold();
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            }
                        }
                        else
                        {
                            if (Type == "3-FAK")
                            {
                                seqno++;
                                TtlDlr++;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Value = "" + row3[dt3.Columns[2].Caption];
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                var count = dt3.Columns.Count + dt2.Rows.Count + dt2.Rows.Count;
                                for (int i = 2; i < count; i++)
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Value = 0;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                }

                                if (AreaGroup == "" || AreaGroup != "'" + row3[dt3.Columns[0].Caption]) { pertama = seqno; }
                                for (int i = 3; i < dt3.Columns.Count; i++)
                                {
                                    var caption = dt3.Columns[i].Caption;
                                    a = dt2.Rows.Count + dt2.Rows.Count + i;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Value = "" + row3[caption];
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                }
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).SetFormulaA1("=SUM(" + GetExcelColumnName(dt2.Rows.Count + dt2.Rows.Count + 2) + seqno + ":" + GetExcelColumnName(a - 1) + seqno + ")");
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Font.SetBold();
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            }

                            if (Type == "1-INQ")
                            {
                                seqno++;
                                TtlDlr++;
                                if (AreaGroup == "" || AreaGroup != "'" + row3[dt3.Columns[0].Caption]) { pertama = seqno; }
                                for (int i = 2; i < dt3.Columns.Count; i++)
                                {
                                    var caption = dt3.Columns[i].Caption;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Value = "" + row3[caption];
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                }
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).SetFormulaA1("=SUM(B" + seqno + ":" + GetExcelColumnName(dt3.Columns.Count - 2) + seqno + ")");
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Font.SetBold();
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                
                                var count = dt3.Columns.Count + dt2.Rows.Count + dt2.Rows.Count + 1;
                                for (int i = dt3.Columns.Count + 1; i < count; i++)
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Value = 0;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                }
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region SubtotalByArea
                        seqno++;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Value = AreaGroup + " (" + TtlDlr + ")";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        TtlDlr = 0;

                        var jml = dt2.Rows.Count * 3;
                        for (int i = 2; i < jml + 2; i++)
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).SetFormulaA1("=SUM(" + GetExcelColumnName(i) + pertama + ":" + GetExcelColumnName(i) + (seqno - 1) + ")");
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        }

                        if (Type == "3-FAK")
                        {
                            seqno++;
                            TtlDlr++;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Value = "" + row3[dt3.Columns[2].Caption];
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            var count = dt3.Columns.Count + dt2.Rows.Count + dt2.Rows.Count;
                            for (int i = 2; i < count; i++)
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Value = 0;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            }

                            if (AreaGroup == "" || AreaGroup != "'" + row3[dt3.Columns[0].Caption]) { pertama = seqno; }
                            for (int i = 3; i < dt3.Columns.Count; i++)
                            {
                                var caption = dt3.Columns[i].Caption;
                                a = dt2.Rows.Count + dt2.Rows.Count + i;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Value = "" + row3[caption];
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a - 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            }
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).SetFormulaA1("=SUM(" + GetExcelColumnName(dt2.Rows.Count + dt2.Rows.Count + 2) + seqno + ":" + GetExcelColumnName(a - 1) + seqno + ")");
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Font.SetBold();
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        }

                        if (Type == "1-INQ")
                        {
                            seqno++;
                            TtlDlr++;
                            if (AreaGroup == "" || AreaGroup != "'" + row3[dt3.Columns[0].Caption]) { pertama = seqno; }
                            for (int i = 2; i < dt3.Columns.Count; i++)
                            {
                                var caption = dt3.Columns[i].Caption;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Value = "" + row3[caption];
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            }
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).SetFormulaA1("=SUM(B" + seqno + ":" + GetExcelColumnName(dt3.Columns.Count - 2) + seqno + ")");
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Font.SetBold();
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                            var count = dt3.Columns.Count + dt2.Rows.Count + dt2.Rows.Count + 1;
                            for (int i = dt3.Columns.Count + 1; i < count; i++)
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Value = 0;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i - 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            }
                        }
                        #endregion
                    }
                    AreaGroup = "'" + row3[dt3.Columns[0].Caption];
                    OldDealer = "'" + row3[dt3.Columns[2].Caption];
                }

                #region SubtotalLastArea
                seqno++;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Value = AreaGroup + " (" + TtlDlr + ")";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                var jumlah = dt2.Rows.Count * 3;
                for (int i = 2; i < jumlah + 2; i++)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).SetFormulaA1("=SUM(" + GetExcelColumnName(i) + pertama + ":" + GetExcelColumnName(i) + (seqno - 1) + ")");
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                }
                #endregion

                #region Header2
                lastRow = seqno + 3;
                lastColHeaderType = 1;
                FristColType = 0;

                ws.Column(lastColHeaderType).Width = 50;
                ws.Cell(lastRow, lastColHeaderType).Value = "DEALER";
                ws.Range(lastRow, lastColHeaderType, lastRow + 1, lastColHeaderType).Merge();
                ws.Range(lastRow, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Range(lastRow, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                awalRow = lastRow;
                awalcOL = lastColHeaderType;
                lastColHeaderType++;

                foreach (DataRow row1 in dt1.Rows)
                {
                    FristColType = lastColHeaderType;
                    ws.Cell(lastRow, lastColHeaderType).Value = row1["Type"].ToString();
                    foreach (DataRow row2 in dt2.Rows)
                    {
                        ws.Cell(lastRow + 1, lastColHeaderType).Value = row2["TypeCode"].ToString();
                        ws.Cell(lastRow + 1, lastColHeaderType).Style.Alignment.WrapText = true;
                        ws.Range(lastRow + 1, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        ws.Range(lastRow + 1, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        lastColHeaderType++;
                    }
                    ws.Range(lastRow, FristColType, lastRow, lastColHeaderType - 1).Merge();
                    ws.Range(lastRow, FristColType, lastRow, lastColHeaderType - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Fill.BackgroundColor = XLColor.BabyBlue;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                lastRow = lastRow + 2;
                #endregion

                #region AllArea
                seqno = lastRow;
                foreach (DataRow row4 in dt4.Rows)
                {
                    Type = "" + row4[dt4.Columns[0].Caption];
                    var AllArea = "" + row4[dt4.Columns[1].Caption];
                    if (AllArea == "NONJABODETABEK" && Type == "1-INQ") { seqno = seqno + 1; }
                    if (Type == "2-SPK")
                    {
                        for (int i = 2; i < dt4.Columns.Count; i++)
                        {
                            var caption = dt4.Columns[i].Caption;
                            a = dt2.Rows.Count + i;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Value = "" + row4[caption];
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).SetFormulaA1("=SUM(" + GetExcelColumnName(dt2.Rows.Count + 2) + seqno + ":" + GetExcelColumnName(a) + seqno + ")");
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).Style.Font.SetBold();
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    }
                    if (Type == "3-FAK")
                    {
                        for (int i = 2; i < dt4.Columns.Count; i++)
                        {
                            var caption = dt4.Columns[i].Caption;
                            a = dt2.Rows.Count + dt2.Rows.Count + i;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Value = "" + row4[caption];
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).SetFormulaA1("=SUM(" + GetExcelColumnName(dt2.Rows.Count + dt2.Rows.Count + 2) + seqno + ":" + GetExcelColumnName(a) + seqno + ")");
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).Style.Font.SetBold();
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(a + 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    }
                    if (Type == "1-INQ")
                    {
                        for (int i = 1; i < dt4.Columns.Count; i++)
                        {
                            var caption = dt4.Columns[i].Caption;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Value = "" + row4[caption];
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).SetFormulaA1("=SUM(B" + seqno + ":" + GetExcelColumnName(dt3.Columns.Count - 2) + seqno + ")");
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Font.SetBold();
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(dt3.Columns.Count - 1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        var count = dt4.Columns.Count + dt2.Rows.Count + dt2.Rows.Count + 1;
                        for (int i = dt4.Columns.Count + 1; i < count; i++)
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Value = 0;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;                            
                        }
                    }
                }
                #endregion

                #region LastTotalArea
                seqno++;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Value = "NASIONAL";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                jumlah = dt2.Rows.Count * 3;
                for (int i = 2; i < jumlah + 2; i++)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).SetFormulaA1("=SUM(" + GetExcelColumnName(i) + (seqno - 2) + ":" + GetExcelColumnName(i) + (seqno - 1) + ")");
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                }

                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno - 6)).Value = "NASIONAL";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno - 6)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno - 6)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno - 6)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno - 6)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), seqno - 6)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                jumlah = dt2.Rows.Count * 3;
                for (int i = 2; i < jumlah + 2; i++)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno - 6)).SetFormulaA1("=SUM(" + GetExcelColumnName(i) + (seqno - 2) + ":" + GetExcelColumnName(i) + (seqno - 1) + ")");
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno - 6)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno - 6)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno - 6)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno - 6)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), seqno - 6)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                }
                #endregion

                #region Header3
                lastRow = seqno + 2;
                lastColHeaderType = 1;
                FristColType = 0;

                ws.Column(lastColHeaderType).Width = 50;
                ws.Cell(lastRow, lastColHeaderType).Value = "RATIO";
                ws.Range(lastRow, lastColHeaderType, lastRow + 1, lastColHeaderType).Merge();
                ws.Range(lastRow, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Range(lastRow, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                awalRow = lastRow;
                awalcOL = lastColHeaderType;
                lastColHeaderType++;

                foreach (DataRow row1 in dt5.Rows)
                {
                    FristColType = lastColHeaderType;
                    ws.Cell(lastRow, lastColHeaderType).Value = row1["Type"].ToString();
                    foreach (DataRow row2 in dt2.Rows)
                    {
                        ws.Cell(lastRow + 1, lastColHeaderType).Value = row2["TypeCode"].ToString();
                        ws.Cell(lastRow + 1, lastColHeaderType).Style.Alignment.WrapText = true;
                        ws.Range(lastRow + 1, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        ws.Range(lastRow + 1, lastColHeaderType, lastRow + 1, lastColHeaderType).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        lastColHeaderType++;
                    }
                    ws.Range(lastRow, FristColType, lastRow, lastColHeaderType - 1).Merge();
                    ws.Range(lastRow, FristColType, lastRow, lastColHeaderType - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Fill.BackgroundColor = XLColor.BabyBlue;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(awalRow, awalcOL, lastRow + 1, lastColHeaderType - 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                lastRow = lastRow + 2;
                #endregion

                foreach (DataRow row1 in dt6.Rows)
                {
                    var LastCol = 1;
                    ws.Cell(lastRow, LastCol).Value = row1["RATIO"].ToString();
                    ws.Cell(lastRow, LastCol).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(lastRow, LastCol).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(lastRow, LastCol).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(lastRow, LastCol).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    Type = "" + row1[dt6.Columns[0].Caption];
                    #region "SPK/INQ"
                    if (Type == "SPK/INQ")
                    {
                        foreach (DataRow row2 in dt5.Rows)
                        {
                            LastCol++;
                            var Col = 2;
                            Type = "" + row2[dt5.Columns[0].Caption];
                            if (Type == "JABODETABEK")
                            {
                                foreach (DataRow row3 in dt2.Rows)
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).SetFormulaA1("=IFERROR(" + GetExcelColumnName(Col + dt2.Rows.Count) + (lastRow - 6) + "/" + GetExcelColumnName(Col) + (lastRow - 6) + ",0%)");
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.NumberFormat.Format = "0.0%";
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    LastCol++;
                                    Col++;
                                }
                            }
                            if (Type == "NONJABODETABEK")
                            {
                                foreach (DataRow row3 in dt2.Rows)
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).SetFormulaA1("=IFERROR(" + GetExcelColumnName(Col + dt2.Rows.Count) + (lastRow - 5) + "/" + GetExcelColumnName(Col) + (lastRow - 5) + ",0%)");
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.NumberFormat.Format = "0.0%";
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    LastCol++;
                                    Col++;
                                }
                            }
                            if (Type == "NASIONAL")
                            {
                                foreach (DataRow row3 in dt2.Rows)
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).SetFormulaA1("=IFERROR(" + GetExcelColumnName(Col + dt2.Rows.Count) + (lastRow - 4) + "/" + GetExcelColumnName(Col) + (lastRow - 4) + ",0%)");
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.NumberFormat.Format = "0.0%";
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    LastCol++;
                                    Col++;
                                }
                            }
                            LastCol = LastCol - 1;
                        }
                    }
                    #endregion
                    #region "FAK/SPK"
                    if (Type == "FAK/SPK")
                    {
                        foreach (DataRow row2 in dt5.Rows)
                        {
                            LastCol++;
                            var Col = 2;
                            Type = "" + row2[dt5.Columns[0].Caption];
                            if (Type == "JABODETABEK")
                            {
                                foreach (DataRow row3 in dt2.Rows)
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).SetFormulaA1("=IFERROR(" + GetExcelColumnName(Col + dt2.Rows.Count + dt2.Rows.Count) + (lastRow - 7) + "/" + GetExcelColumnName(Col + dt2.Rows.Count) + (lastRow - 7) + ",0%)");
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.NumberFormat.Format = "0.0%";
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    LastCol++;
                                    Col++;
                                }
                            }
                            if (Type == "NONJABODETABEK")
                            {
                                foreach (DataRow row3 in dt2.Rows)
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).SetFormulaA1("=IFERROR(" + GetExcelColumnName(Col + dt2.Rows.Count + dt2.Rows.Count) + (lastRow - 6) + "/" + GetExcelColumnName(Col + dt2.Rows.Count) + (lastRow - 6) + ",0%)");
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.NumberFormat.Format = "0.0%";
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    LastCol++;
                                    Col++;
                                }
                            }
                            if (Type == "NASIONAL")
                            {
                                foreach (DataRow row3 in dt2.Rows)
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).SetFormulaA1("=IFERROR(" + GetExcelColumnName(Col + dt2.Rows.Count + dt2.Rows.Count) + (lastRow - 5) + "/" + GetExcelColumnName(Col + dt2.Rows.Count) + (lastRow - 5) + ",0%)");
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.NumberFormat.Format = "0.0%";
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    LastCol++;
                                    Col++;
                                }
                            }
                            LastCol = LastCol - 1;
                        }
                    }
                    #endregion
                    #region "FAK/INQ
                    if (Type == "FAK/INQ")
                    {
                        foreach (DataRow row2 in dt5.Rows)
                        {
                            LastCol++;
                            var Col = 2;
                            Type = "" + row2[dt5.Columns[0].Caption];
                            if (Type == "JABODETABEK")
                            {
                                foreach (DataRow row3 in dt2.Rows)
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).SetFormulaA1("=IFERROR(" + GetExcelColumnName(Col + dt2.Rows.Count + dt2.Rows.Count) + (lastRow - 8) + "/" + GetExcelColumnName(Col) + (lastRow - 8) + ",0%)");
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.NumberFormat.Format = "0.0%";
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    LastCol++;
                                    Col++;
                                }
                            }
                            if (Type == "NONJABODETABEK")
                            {
                                foreach (DataRow row3 in dt2.Rows)
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).SetFormulaA1("=IFERROR(" + GetExcelColumnName(Col + dt2.Rows.Count + dt2.Rows.Count) + (lastRow - 7) + "/" + GetExcelColumnName(Col) + (lastRow - 7) + ",0%)");
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.NumberFormat.Format = "0.0%";
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    LastCol++;
                                    Col++;
                                }
                            }
                            if (Type == "NASIONAL")
                            {
                                foreach (DataRow row3 in dt2.Rows)
                                {
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).SetFormulaA1("=IFERROR(" + GetExcelColumnName(Col + dt2.Rows.Count + dt2.Rows.Count) + (lastRow - 6) + "/" + GetExcelColumnName(Col) + (lastRow - 6) + ",0%)");
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.NumberFormat.Format = "0.0%";
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(LastCol), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    LastCol++;
                                    Col++;
                                }
                            }
                            LastCol = LastCol - 1;
                        }
                    }
                    #endregion
                    lastRow++;
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
        }

        public ActionResult Generatechart()
        {
            
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            Excel.Range chartRange;

            Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
            Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, 80, 300, 250);
            Excel.Chart chartPage = myChart.Chart;

            chartRange = xlWorkSheet.get_Range("A1", "d5");
            chartPage.SetSourceData(chartRange, misValue);
            chartPage.ChartType = Excel.XlChartType.xl3DBarStacked;

            xlWorkBook.SaveAs(Server.MapPath("~/ReportTemp/" + "chart1" + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + "chart1" + ".xlsx"));
        }

        public ActionResult GenerateITSReportSPKWorkDay() 
        {
            string StartDate = Request["StartDate"];
            string EndDate = Request["EndDate"];
            string TipeKendaraan = Request["TipeKendaraan"] ?? "";
            string StartDateName = Request["StartDateName"];
            string EndDateName = Request["EndDateName"];
            string fileName = "SPK Work Day";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GenerateITSSPKWorkDay";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@TipeKendaraan", TipeKendaraan);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            da.Fill(ds);
            dt1 = ds.Tables[0];

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
                var ws = wb.Worksheets.Add("Work Day SPK Nasional"); 

                ws.Columns("1").Width = 10;
                ws.Range("A" + lastRow + ":J" + lastRow).Merge();
                ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A" + lastRow).Value = "WORK DAY SPK NASIONAL";
                lastRow = 3; 
                ws.Columns("1").Width = 20;
                ws.Range("A" + lastRow + ":J" + lastRow).Merge();
                ws.Cell("A" + lastRow).Value = "Periode " + StartDateName + " S/D " + EndDateName;
                lastRow++;

                #region Work Day
                var tgl = 0;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Work Day";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                for (int i = 2; i <= 25; i++)
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
                lastRow++;

                var x = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = row[dt1.Columns[0].Caption];
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i < dt1.Columns.Count; i++)
                    {
                        var a = i - 1;
                        var inq = Convert.ToInt32(row[dt1.Columns[a].Caption]);
                        accum = accum + inq;
                        if (x != accum || x == 0 || inq == 0) { accum = accum; } else { accum = 0; }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = row[dt1.Columns[a].Caption]; ;
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
                #endregion

                #region Work Day (Acumm)
                lastRow++;
                tgl = 0;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = "Work Day (Accum)";
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                for (int i = 2; i <= 25; i++)
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
                lastRow++;

                x = 0;
                var LastAccum = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Value = row[dt1.Columns[0].Caption];
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(1), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    for (int i = 2; i < dt1.Columns.Count; i++)
                    {
                        var a = i - 1;
                        var inq = Convert.ToInt32(row[dt1.Columns[a].Caption]);
                        accum = accum + inq;
                        LastAccum = LastAccum + inq;
                        if (x != accum || x == 0) { accum = accum; } else { accum = 0; }
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Value = accum;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i), lastRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        x = accum;
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
                #endregion

                //Excel.Application xlApp;
                //Excel.Workbook xlWorkBook;
                //Excel.Worksheet xlWorkSheet;
                //object misValue = System.Reflection.Missing.Value;

                //xlApp = new Excel.Application();
                //xlWorkBook = xlApp.Workbooks.Add(misValue);
                //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                //Excel.Range chartRange;

                //Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                //Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, 80, 300, 250);
                //Excel.Chart chartPage = myChart.Chart;

                //chartRange = xlWorkSheet.get_Range("A1", "d5");
                //chartPage.SetSourceData(chartRange, misValue);
                //chartPage.ChartType = Excel.XlChartType.xl3DBarStacked;

                //xlWorkBook.SaveAs(Server.MapPath("~/ReportTemp/" + "chart1" + ".xlsx"));
                //return Redirect(Url.Content("~/ReportTemp/" + "chart1" + ".xlsx"));


                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
        }

        public ActionResult GenerateITSToExcel(int Year, int Month)
        {
            HttpContext.Server.ScriptTimeout = 13600;
            DateTime now = DateTime.Now;
            string fileName = "ITS_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; 
            cmd.CommandText = "uspfn_GenerateITStoExecl";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Year", Year);
            cmd.Parameters.AddWithValue("@Month", Month);
            cmd.Parameters.AddWithValue("@Type", 0);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            da.Fill(ds);
            dt1 = ds.Tables[0];

            int lastRow = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("GenerateITSwithStatusAndTD");

            return GenerateExcel(wb, dt1, lastRow, fileName);
        }
        
        #region Sales Force ID
        private class prmSLSFRC
        {
            public string area { get; set; }
            public string dealerCode { get; set; }
            public string outletCode { get; set; }
            public string branchHead { get; set; }
            public string salesHead { get; set; }
            public string salesman { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string createBy { get; set; }
            public int typeReport { get; set; }
        }

        private class SalesForceID 
        {
            public string tipekendaraan { get; set; }
            public int spkminsatu { get; set; }
            public int p { get; set; }
            public int hp { get; set; }
            public int spk { get; set; }
            public int doo { get; set; }
            public int delivery { get; set; }
            public int lost { get; set; }
            public int gt { get; set; }
            public int outp { get; set; }
            public int outhp { get; set; }
            public int outspk { get; set; } 
        }

        private static char GetChar(int colnumber)
        {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            return alpha[colnumber];
        }

        private DataTable CreateTable(prmSLSFRC param, string SPName)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = SPName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Salesman", param.salesman);
            cmd.Parameters.AddWithValue("@StartDate", param.startDate);
            cmd.Parameters.AddWithValue("@EndDate", param.endDate);
            cmd.Parameters.AddWithValue("@TypeReport", param.typeReport);
            cmd.Parameters.AddWithValue("@GroupNo", param.area);
            cmd.Parameters.AddWithValue("@DealerCode", param.dealerCode);
            cmd.Parameters.AddWithValue("@OutletCode", param.outletCode);
            cmd.Parameters.AddWithValue("@BranchHead", param.branchHead);
            cmd.Parameters.AddWithValue("@SalesHead", param.salesHead);
            cmd.Parameters.AddWithValue("@CreateBy", param.createBy);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public JsonResult SalesForceId() 
        {
            var data = ctx.SvUnitIntakeViews.AsQueryable();
            var DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var DateTo = DateTime.Now;
            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var outlet = Request.Params["Outlet"];

            var sl = Request.Params["Salesman"] == "" ? "21222" : Request.Params["Salesman"]; 
            //var novin = Request.Params["NOVIN"];
            //var nopol = Request.Params["NOPOL"];
            //var pelanggan = Request.Params["PELANGGAN"];
            //var rework = Request.Params["Rework"];

            DateFrom = new DateTime(DateTime.Now.Year, (DateTime.Now.Month-1), 1);
            DateTo = new DateTime(DateTime.Now.Year, (DateTime.Now.Month-1), 31);
            string query = "exec usprpt_InqSalesForceID '" + sl + "','" + String.Format("{0:yyyyMMdd}", DateFrom) +"','" + String.Format("{0:yyyyMMdd}", DateTo) + "'";
            
            //if (!string.IsNullOrWhiteSpace(area)) query += " and GroupNo = " + area;
            //if (!string.IsNullOrWhiteSpace(dealer)) query += " and GNDealerCode = '" + dealer + "'";
            ////if (!string.IsNullOrWhiteSpace(outlet)) data = data.Where(p => p.BranchCode == outlet.Substring(6));
            //if (!string.IsNullOrWhiteSpace(outlet)) query += " and GNOutletCode = '" + outlet + "'";
            //if (!string.IsNullOrWhiteSpace(novin)) query += " and VinNo like '%" + novin + "%'";
            //if (!string.IsNullOrWhiteSpace(nopol)) query += " and PoliceRegNo like '%" + nopol + "%'";
            //if (!string.IsNullOrWhiteSpace(pelanggan)) query += " and CustomerName like '%" + pelanggan + "%'";
            //if (!string.IsNullOrWhiteSpace(rework)) query += " and JobType='Rework'";

            //if (!string.IsNullOrWhiteSpace(DateFrom) && !string.IsNullOrWhiteSpace(DateTo)) { query += " and left(convert(nvarchar, JobOrderClosed, 121), 10) >= '" + DateFrom + "' and left(convert(nvarchar, JobOrderClosed, 121), 10) <= '" + DateTo + "'"; }

            ctx.Database.CommandTimeout = 3600;
            var data2 = ctx.Database.SqlQuery<SalesForceID>(query).AsQueryable();

            return Json(data2.KGrid());
        }
        public ActionResult GenerateSalesForceId
       (string area, string dealer, string outlet, string bm, string sh, string sl, string startdate, string enddate, string createby, string typerpt )
        {
            var data = ctx.Database.SqlQuery<getAreaDealerOutlet>("exec uspfn_GetAreaDealerOutletSvr @GroupNo=@p0, @dealercode=@p1, @BranchCode=@p2", area, dealer, outlet);
            var areaName = data.FirstOrDefault().Area;
            var dealerName = data.FirstOrDefault().Dealer;
            var outletName = data.FirstOrDefault().Showroom;

            DateTime now = DateTime.Now;
            string fileName = "Sales Force ID";
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sales Force");
            ws.Style.Font.SetFontName("calibri");
            ws.Style.Font.SetFontSize(9);
            int row = 1;

            #region Title
            ws.Cell("A1").Value = "UPDATE REATAIL SALES SALES FORCE ID CARD";
            ws.Row(1).Height = 25;
            ws.Row(1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            
            var Title = ws.Range(1, 1, 1,12);
            Title.Merge();
            Title.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            Title.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            Title.Style.Font.FontSize = 14;
            Title.Style.Font.SetBold(true);

            ws.Cell("A2").Value = "NAMA";
            ws.Cell("A3").Value = "AREA";
            ws.Cell("A4").Value = "DEALER";
            ws.Cell("A5").Value = "OUTLET";
            ws.Cell("A6").Value = "ATPM ID";

            ws.Cell("B2").Value = ": " + createby;
            ws.Cell("B3").Value = ": " + areaName;
            ws.Cell("B4").Value = ": " + dealerName;
            ws.Cell("B5").Value = ": " + outletName;
            ws.Cell("B6").Value = ": " + (sl == ""?"ALL": sl);

            ws.Range(2, 1, 6, 1).Style.Font.Bold = true;
            row = 8;
            #endregion

            #region Header Table
            ws.Cell("A" + row).Value = "TYPE";
            ws.Cell("B" + row).Value = "FRESH\n SPK(N-1)";
            ws.Cell("C" + row).Value = "PERIODE";
            ws.Cell("J" + row).Value = "OUTSTANDING";
            row++;
            ws.Cell("C" + row).Value = "P";
            ws.Cell("D" + row).Value = "HP";
            ws.Cell("E" + row).Value = "SPK";
            ws.Cell("F" + row).Value = "DO";
            ws.Cell("G" + row).Value = "DELEVERY";
            ws.Cell("H" + row).Value = "LOST";
            ws.Cell("I" + row).Value = "GRAND TOTAL";
            ws.Cell("J" + row).Value = "P";
            ws.Cell("K" + row).Value = "HP";
            ws.Cell("L" + row).Value = "SPK";

            ws.Range("A" + (row - 1).ToString() + ":A" + row).Merge();
            ws.Range("B" + (row - 1).ToString() + ":B" + row).Merge();
            ws.Range("C" + (row - 1).ToString() + ":I" + (row - 1).ToString()).Merge();
            ws.Range("J" + (row - 1).ToString() + ":L" + (row - 1).ToString()).Merge();
            
            ws.Range(row-1, 1, row, 12).Style.Font.Bold = true;
            var Titles = ws.Range("A" + (row - 1).ToString() + ":L" + row);
            Titles.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            Titles.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            Titles.Style.Font.FontSize = 10;
            Titles.Style.Font.SetBold(true);
            Titles.Style.Fill.SetBackgroundColor(XLColor.FromArgb(192, 0, 0));
            Titles.Style.Font.SetFontColor(XLColor.White);
            Titles.Style.Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);
            row++;
            #endregion
            prmSLSFRC param = new prmSLSFRC
            {
                area = area,
                dealerCode = dealer,
                outletCode = outlet,
                branchHead = bm,
                salesHead = sh,
                salesman = sl,
                startDate = startdate,
                endDate = enddate,
                typeReport = 0,
                createBy = CurrentUser.Username
            };
            DataTable dt = CreateTable(param, "usprpt_InqSalesForceID");
            GenerateExcelSFIC(wb, dt, row, fileName, true);
            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        private XLWorkbook GenerateExcelSFIC(XLWorkbook wb, DataTable dt, int lastRow, string fileName, bool isCustomize = false, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var MRow = lastRow;

            #region Summary Vin
            var ws = wb.Worksheet(1);
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
            var column = 2;
            if (iCol > 26)
            {
                char alphaStart = GetChar(column - 1);
                for (char i = alphaStart; i <= 'Z'; i++)
                {
                  ws.Cell(lastRow, column).FormulaA1 = "=SUM(" + i + (tmpLastRow) + ":" + i + (lastRow - 1) + ")";
                  column++;
                }

                char alphaEnd = GetChar((iCol - 26) - 2);
                for (char i = 'A'; i <= alphaEnd; i++)
                {
                    ws.Cell(lastRow, column).FormulaA1 = "=SUM(A" + i + (tmpLastRow) + ":A" + i + (lastRow - 1) + ")";
                    column++;
                }
            }
            else
            {
                char alphaStart = GetChar(column - 1);
                char alphaEnd = GetChar((26 - iCol) - 2);
                for (char i = alphaStart; i <= alphaEnd; i++)
                {
                    ws.Cell(lastRow, column).FormulaA1 = "=SUM(" + i + (tmpLastRow) + ":" + i + (lastRow - 1) + ")";
                    column++;
                }
            }
            var rngTable = ws.Range(tmpLastRow, 1, lastRow, iCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);
            rngTable.Style.Font.FontSize = 9;
            var IsiDatas = ws.Range(tmpLastRow, 1, lastRow, iCol - 1);
            IsiDatas.Style.Fill.SetBackgroundColor(XLColor.FromArgb(234, 241, 221));

            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.Font.Bold = true;
            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));
            ws.Columns().Style.Alignment.SetWrapText(true);
            //ws.Columns(1, 2).AdjustToContents();
            #endregion
            return wb;
        }
        #endregion
    }
}
            
