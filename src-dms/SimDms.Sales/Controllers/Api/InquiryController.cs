using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Diagnostics;
using ClosedXML.Excel;
using System.Data;
using System.Data.SqlClient;
using RestSharp;

namespace SimDms.Sales.Controllers.Api
{
    public class InquiryController : BaseController
    {
        public ActionResult PrintInqProdSales(DateTime StartDate, DateTime EndDate, string Area, string Dealer, string Outlet, string BranchHead, string SalesHead, string SalesCoordinator, string Salesman, string OutletStat)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "ProductivitySalesForce";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_OmRpSalRgs047";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@Area", Area);
            cmd.Parameters.AddWithValue("@Dealer", Dealer);
            cmd.Parameters.AddWithValue("@Outlet", Outlet);
            cmd.Parameters.AddWithValue("@BranchHead", BranchHead);
            cmd.Parameters.AddWithValue("@SalesHead", SalesHead);
            cmd.Parameters.AddWithValue("@SalesCoordinator", SalesCoordinator);
            cmd.Parameters.AddWithValue("@Salesman", Salesman);
            cmd.Parameters.AddWithValue("@OutletStat", OutletStat);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 12;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Productivity Sales Force");
                var hdrTable = ws.Range("A12:AE12");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A12:AE12");
                ws.Range("A12", "AE12").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A12", "AE12").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A12", "AE12").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A12", "AE12").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText(); ;

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                //ws.Columns("1").Width = 50;
                //ws.Columns("2").Width = 10;
                //ws.Columns("3").Width = 5;

                //First Names  
                ws.Range("A1", "C1").Merge();
                ws.Cell("A1").Value = "Productivity Sales Report";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(12);
                ws.Cell("A2").Value = "Date : " + StartDate + " s/d " + EndDate ;
                ws.Cell("A3").Value = "";
                ws.Cell("A4").Value = "Area";
                if (Area == "")
                    ws.Cell("B4").Value = ":  ALL";
                else
                    ws.Cell("B4").Value = ":  " + Area;

                ws.Cell("A5").Value = "Dealer";
                if (Dealer == "")
                    ws.Cell("B5").Value = ":  ALL";
                else
                    ws.Cell("B5").Value = ":  " + Dealer;

                ws.Cell("A6").Value = "OutLet";
                if (Outlet == "")
                    ws.Cell("B6").Value = ":  ALL   s/d   ALL";
                else
                    ws.Cell("B6").Value = ":  " + Outlet;

                ws.Cell("A7").Value = "SalesCoordinator";
                if (SalesCoordinator == "")
                    ws.Cell("B7").Value = ":  ALL";
                else
                    ws.Cell("B7").Value = ":  " + SalesCoordinator;

                ws.Cell("A8").Value = "Salesman";
                if (Salesman == "")
                    ws.Cell("B8").Value = ":  ALL";
                else
                    ws.Cell("B8").Value = ":  " + Salesman;

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public ActionResult PrintInqSales(string startDate, string endDate, string area, string dealer, string outlet, string branchHead, string salesHead, string salesCoordinator, string salesman, string SalesType, bool isOutlet = false)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "ITS Reports";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_OmRpSalRgs045";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@Dealer", dealer);
            cmd.Parameters.AddWithValue("@Outlet", outlet);
            cmd.Parameters.AddWithValue("@BranchHead", branchHead);
            cmd.Parameters.AddWithValue("@SalesHead", salesHead);
            cmd.Parameters.AddWithValue("@SalesCoordinator", salesCoordinator);
            cmd.Parameters.AddWithValue("@Salesman", salesman);
            cmd.Parameters.AddWithValue("@SalesType", SalesType);
            cmd.Parameters.AddWithValue("@SalesOutlet", isOutlet);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 12;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("ITS Report");
                var hdrTable = ws.Range("A12:AE12");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A12:AE12");
                ws.Range("A12", "AE12").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A12", "AE12").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A12", "AE12").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A12", "AE12").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText(); ;

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                ws.Range("A1", "C1").Merge();
                ws.Cell("A1").Value = "ITS Report";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(12);
                ws.Cell("A2").Value = "Date : " + DateTime.Now.ToString("dd-MMM-yyyy"); ;
                ws.Cell("A3").Value = "";
                ws.Cell("A4").Value = "Area";
                if (area == "")
                    ws.Cell("B4").Value = ":  ALL";
                else
                    ws.Cell("B4").Value = ":  " + area;

                ws.Cell("A5").Value = "Dealer";
                if (dealer == "")
                    ws.Cell("B5").Value = ":  ALL";
                else
                    ws.Cell("B5").Value = ":  " + dealer;

                ws.Cell("A6").Value = "OutLet";
                if (outlet == "")
                    ws.Cell("B6").Value = ":  ALL   s/d   ALL";
                else
                    ws.Cell("B6").Value = ":  " + outlet;

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public ActionResult PrintInqITS(DateTime StartDate, DateTime EndDate, string Area, string Dealer, string Outlet)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "ITS Reports";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "uspfn_InquiryITS";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@Area", Area);
            cmd.Parameters.AddWithValue("@DealerCode", Dealer);
            cmd.Parameters.AddWithValue("@OutletCode", Outlet);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 12;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("ITS Report");
                var hdrTable = ws.Range("A12:AE12");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A12:AE12");
                ws.Range("A12", "AE12").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A12", "AE12").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A12", "AE12").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A12", "AE12").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText(); ;

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                ws.Range("A1", "C1").Merge();
                ws.Cell("A1").Value = "ITS Report";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(12);
                ws.Cell("A2").Value = "Date : " + DateTime.Now.ToString("dd-MMM-yyyy"); ;
                ws.Cell("A3").Value = "";
                ws.Cell("A4").Value = "Area";
                if (Area == "")
                    ws.Cell("B4").Value = ":  ALL";
                else
                    ws.Cell("B4").Value = ":  " + Area;

                ws.Cell("A5").Value = "Dealer";
                if (Dealer == "")
                    ws.Cell("B5").Value = ":  ALL";
                else
                    ws.Cell("B5").Value = ":  " + Dealer;

                ws.Cell("A6").Value = "OutLet";
                if (Outlet == "")
                    ws.Cell("B6").Value = ":  ALL   s/d   ALL";
                else
                    ws.Cell("B6").Value = ":  " + Outlet;

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
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
                        // ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.TeaRoseRose).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
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

        public string GetComboLiveStock(string cboType)
        {

            var sql = "select paramvalue from sysparameter where paramid='simdms_api'";
            var simdmsUrl = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();

            var client = new RestClient(simdmsUrl);
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("api/utilinfo1", Method.POST);
            request.AddParameter("TypeCombo", cboType); // adds to POST or URL querystring based on Method
            request.AddParameter("IsVisible", 1); // adds to POST or URL querystring based on Method

            // execute the request
            IRestResponse response = client.Execute(request);

            int n = response.Content.Length;

            return response.Content.Substring(3, n - 4); // raw content as string
        }

        public JsonResult GetLiveStock(string Type, string Variant, string Transmission, string Colour)
        {

            var sql = "select paramvalue from sysparameter where paramid='simdms_api'";
            var simdmsUrl = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();

            var client = new RestClient(simdmsUrl);
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("api/utilinfo2", Method.POST);
            request.AddParameter("Type", Type); // adds to POST or URL querystring based on Method
            request.AddParameter("Variant", Variant); // adds to POST or URL querystring based on Method
            request.AddParameter("Transmission", Transmission); // adds to POST or URL querystring based on Method
            request.AddParameter("Colour", Colour); // adds to POST or URL querystring based on Method
            request.AddParameter("IsVisible", 1); // adds to POST or URL querystring based on Method

            // execute the request
            IRestResponse response = client.Execute(request);

            int n = response.Content.Length;

            return Json(new { success = true, grid = response.Content.Substring(3, n - 4) }); // raw content as string
        }
    }
}
