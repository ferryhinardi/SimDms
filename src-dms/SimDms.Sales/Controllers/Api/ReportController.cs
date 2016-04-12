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
using SimDms.Common;

namespace SimDms.Sales.Controllers.Api
{
    public class ReportController : BaseController
    {
        [HttpPost]
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                Dealer = CompanyName,
                Outlet = BranchName,
                Year = DateTime.Now.Year,
                dateFrom = Helpers.StartOfMonth(),
                dateTo = Helpers.EndOfMonth(),
            });
        }

        public JsonResult SalesOrder()
        {
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string soNumber = Request["SONumber"] ?? "";

            var ds_0 = ctx.CoProfiles.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode).ToList();
            var ds_1 = ctx.Database.SqlQuery<uspfn_RptSO_A01_Rpt>("exec uspfn_RptSO_A01_Rpt @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", companyCode, branchCode, soNumber).ToList();
            var ds_2 = ctx.Database.SqlQuery<uspfn_RptSO_A01_Dtl>("exec uspfn_RptSO_A01_Dtl @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", companyCode, branchCode, soNumber).ToList();

            string reportPath = Server.MapPath("~/Reports/rdlc/om/SO-A01.rdlc");

            RenderReport(reportPath, "SalesOrder", 13.0, 31.0, "pdf", ds_0, ds_1, ds_2);

            return Json(null);
        }

        public JsonResult CatatanPenjualan()
        {
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string soNumber = Request["SONumber"] ?? "";
            var ds_0 = ctx.CoProfiles.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode).ToList();
            var ds_1 = ctx.Database.SqlQuery<usprpt_OmRpSalesTrn001B>("exec uspfn_OmRpSalesTrn001B @CompanyCode=@p0, @BranchCode=@p1, @SONoFrom=@p2, @SONoEnd=@p3, @param=@p4", companyCode, branchCode, soNumber, soNumber, false).ToList();
            string reportPath = Server.MapPath("~/Reports/rdlc/om/SO-A02.rdlc");
            RenderReport(reportPath, "CatatanPenjualan", 13.0, 31.0, "pdf", ds_0, ds_1);
            return Json(null);
        }

        public JsonResult Series()
        {
            string sql = "select distinct GroupCode as value, GroupCode as text from omMstModel";

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
            string sql = string.Format("select distinct SalesModelcode as value, SalesModelcode as text from omMstModel where GroupCode = '{0}'", id);

            var list = (from p in ctx.Database.SqlQuery<Combo>(sql)
                        select new Combo()
                        {
                            Value = p.Value,
                            Text = p.Text
                        }).Select(p => new { value = p.Value, text = p.Text }).ToList();

            return Json(list);
        }

        public JsonResult Years()
        {
            var list = new List<int>();
            for (int i = DateTime.Now.Year - 3; i <= DateTime.Now.Year; i++)
            {
                list.Add(i);
            }
            return Json(list.OrderByDescending(p => p).Select(p => new { value = p, text = p }));
        }

        public ActionResult GenerateOmSalesNStock(string Area, string CompanyCode, string BranchCode, string Series, string ModelType, int Year)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "OmSalesReportNStock_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            SqlCommand cmd1 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd1.CommandTimeout = 1800;
            //cmd1.CommandText = "select AreaDealer from CompaniesGroupMappingView where GroupNo='"+Area+"'";
            //cmd1.CommandType = CommandType.Text;
            cmd1.CommandText = "uspfn_GetAreaDealerOutlet";

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.Clear();
            //cmd1.Parameters.AddWithValue("@Groupno", Area);
            cmd1.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd1.Parameters.AddWithValue("@BranchCode", BranchCode);
            SqlDataAdapter ga = new SqlDataAdapter(cmd1);
            DataSet gt = new DataSet();
            ga.Fill(gt);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "uspRpt_OmReportSalesNStock";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Series", Series);
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
                var ws = wb.Worksheets.Add("Sales & Stock R2");
                var hdrTable = ws.Range("A1:C6");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A7:AL8");
                ws.Range("A7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                //ws.Range("A7:A8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Range("A8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                //ws.Range("A7:A8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;


                ws.Range("B7:B8").Merge();
                ws.Range("B7:B8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("B7:B8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("B7:B8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("B7:B8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("C7:E7").Merge();
                ws.Range("C7:E7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("C7:E7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("C7:E7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("C7:E7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("F7:H7").Merge();
                ws.Range("F7:H7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("F7:H7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("F7:H7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("F7:H7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("I7:K7").Merge();
                ws.Range("I7:K7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("I7:K7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("I7:K7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("I7:K7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("L7:N7").Merge();
                ws.Range("L7:N7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("L7:N7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("L7:N7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("L7:N7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("O7:Q7").Merge();
                ws.Range("O7:Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("O7:Q7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("O7:Q7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("O7:Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Range("R7:T7").Merge();
                ws.Range("R7:T7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("R7:T7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("R7:T7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("R7:T7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Range("U7:W7").Merge();
                ws.Range("U7:W7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("U7:W7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("U7:W7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("U7:W7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Range("X7:Z7").Merge();
                ws.Range("X7:Z7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("X7:Z7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("X7:Z7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("X7:Z7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Range("AA7:AC7").Merge();
                ws.Range("AA7:AC7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("AA7:AC7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("AA7:AC7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("AA7:AC7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Range("AD7:AF7").Merge();
                ws.Range("AD7:AF7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("AD7:AF7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("AD7:AF7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("AD7:AF7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Range("AD7:AF7").Merge();
                ws.Range("AD7:AF7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("AD7:AF7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("AD7:AF7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("AD7:AF7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Range("AG7:AI7").Merge();
                ws.Range("AG7:AI7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("AG7:AI7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("AG7:AI7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("AG7:AI7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Range("AJ7:AL7").Merge();
                ws.Range("AJ7:AL7").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("AJ7:AL7").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("AJ7:AL7").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("AJ7:AL7").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText(); ;

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;
                //rngTable.Style.Font.FontColor = XLColor.DarkBlue;
                //rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

                ws.Columns("1").Width = 55;
                ws.Columns("2").Width = 30;
                ws.Columns("3").Width = 10;
                ws.Columns("4").Width = 10;
                ws.Columns("5").Width = 10;
                ws.Columns("6").Width = 10;
                ws.Columns("7").Width = 10;
                ws.Columns("8").Width = 10;
                ws.Columns("9").Width = 10;
                ws.Columns("10").Width = 10;
                ws.Columns("11").Width = 10;
                ws.Columns("12").Width = 10;
                ws.Columns("13").Width = 10;
                ws.Columns("14").Width = 10;
                ws.Columns("15").Width = 10;
                ws.Columns("16").Width = 10;
                ws.Columns("17").Width = 10;
                ws.Columns("18").Width = 10;
                ws.Columns("19").Width = 10;
                ws.Columns("20").Width = 10;
                ws.Columns("21").Width = 10;
                ws.Columns("22").Width = 10;
                ws.Columns("23").Width = 10;
                ws.Columns("24").Width = 10;
                ws.Columns("25").Width = 10;
                ws.Columns("26").Width = 10;
                ws.Columns("27").Width = 10;
                ws.Columns("28").Width = 10;
                ws.Columns("29").Width = 10;
                ws.Columns("30").Width = 10;
                ws.Columns("31").Width = 10;
                ws.Columns("32").Width = 10;
                ws.Columns("33").Width = 10;
                ws.Columns("34").Width = 10;
                ws.Columns("35").Width = 10;
                ws.Columns("36").Width = 10;
                ws.Columns("37").Width = 10;
                ws.Columns("38").Width = 10;


                //First Names   

                ws.Cell("A1").Value = "Dealer";
                ws.Cell("A2").Value = "Showroom";
                ws.Cell("A3").Value = "Series";
                ws.Cell("A4").Value = "Model";
                ws.Cell("A5").Value = "Year";

                foreach (var row1 in gt.Tables[0].Rows)
                {
                    //ws.Cell("B1").Value = ": " + Area + '-' + ((System.Data.DataRow)(row1)).ItemArray[0];
                    ws.Cell("B1").Value = ": " + CompanyCode + '-' + ((System.Data.DataRow)(row1)).ItemArray[0];
                    ws.Cell("B2").Value = ": " + BranchCode + '-' + ((System.Data.DataRow)(row1)).ItemArray[1];
                }

                string GroupSeries;
                string modelSeries;

                if (Series == "")
                {
                    GroupSeries = "All Series";
                }
                else
                {
                    GroupSeries = Series;
                }

                if (ModelType == "")
                {
                    modelSeries = "All Model";
                }
                else
                {
                    modelSeries = ModelType;
                }
                ws.Cell("B3").Value = ": " + GroupSeries;
                ws.Cell("B4").Value = ": " + modelSeries;
                ws.Cell("B5").Value = ": " + Year;

                ws.Cell("A7").Value = "Status ";
                ws.Cell("A8").Value = "Branches/ Sub Dealer";
                ws.Cell("B7").Value = "Model";
                ws.Cell("C7").Value = "Jan";
                ws.Cell("F7").Value = "Feb";
                ws.Cell("I7").Value = "Mar";
                ws.Cell("L7").Value = "Apr";
                ws.Cell("O7").Value = "May";
                ws.Cell("R7").Value = "Jun";
                ws.Cell("U7").Value = "Jul";
                ws.Cell("X7").Value = "Aug";
                ws.Cell("AA7").Value = "Sep";
                ws.Cell("AD7").Value = "Oct";
                ws.Cell("AG7").Value = "Nov";
                ws.Cell("AJ7").Value = "Dec";

                //ws.Cell("A2").Value = "Cabang/ Sub Dealer"; //0
                //ws.Cell("B2").Value = ""; //1

                ws.Cell("C8").Value = "Sales";
                ws.Cell("C8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("C8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("C8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("C8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("D8").Value = "Stock";
                ws.Cell("D8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("D8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("D8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("D8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("E8").Value = "Stock Ratio";
                ws.Cell("E8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("E8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("E8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("E8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("F8").Value = "Sales";
                ws.Cell("F8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("F8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("F8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("F8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("G8").Value = "Stock";
                ws.Cell("G8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("G8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("G8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("G8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("H8").Value = "Stock Ratio";
                ws.Cell("H8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("H8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("H8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("H8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("I8").Value = "Sales";
                ws.Cell("I8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("I8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("I8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("I8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("J8").Value = "Stock";
                ws.Cell("J8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("J8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("J8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("J8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("K8").Value = "Stock Ratio";
                ws.Cell("K8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("K8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("K8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("K8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("L8").Value = "Sales";
                ws.Cell("L8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("L8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("L8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("L8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("M8").Value = "Stock";
                ws.Cell("M8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("M8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("M8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("M8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("N8").Value = "Stock Ratio";
                ws.Cell("N8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("N8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("N8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("N8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("O8").Value = "Sales";
                ws.Cell("O8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("O8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("O8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("O8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("P8").Value = "Stock";
                ws.Cell("P8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("P8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("P8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("P8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("Q8").Value = "Stock Ratio";
                ws.Cell("Q8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("Q8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("Q8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("Q8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("R8").Value = "Sales";
                ws.Cell("R8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("R8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("R8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("R8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("S8").Value = "Stock";
                ws.Cell("S8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("S8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("S8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("S8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("T8").Value = "Stock Ratio";
                ws.Cell("T8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("T8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("T8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("T8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("U8").Value = "Sales";
                ws.Cell("U8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("U8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("U8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("U8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("V8").Value = "Stock";
                ws.Cell("V8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("V8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("V8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("V8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("W8").Value = "Stock Ratio";
                ws.Cell("W8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("W8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("W8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("W8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("X8").Value = "Sales";
                ws.Cell("X8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("X8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("X8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("X8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("Y8").Value = "Stock";
                ws.Cell("Y8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("Y8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("Y8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("Y8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("Z8").Value = "Stock Ratio";
                ws.Cell("Z8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("Z8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("Z8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("Z8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AA8").Value = "Sales";
                ws.Cell("AA8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AA8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AA8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AA8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AB8").Value = "Stock";
                ws.Cell("AB8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AB8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AB8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AB8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AC8").Value = "Stock Ratio";
                ws.Cell("AC8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AC8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AC8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AC8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AD8").Value = "Sales";
                ws.Cell("AD8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AD8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AD8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AD8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AE8").Value = "Stock";
                ws.Cell("AE8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AE8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AE8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AE8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AF8").Value = "Stock Ratio";
                ws.Cell("AF8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AF8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AF8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AF8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AG8").Value = "Sales";
                ws.Cell("AG8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AG8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AG8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AG8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AH8").Value = "Stock";
                ws.Cell("AH8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AH8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AH8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AH8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AI8").Value = "Stock Ratio";
                ws.Cell("AI8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AI8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AI8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AI8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AJ8").Value = "Sales";
                ws.Cell("AJ8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AJ8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AJ8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AJ8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AK8").Value = "Stock";
                ws.Cell("AK8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AK8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AK8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AK8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("AL8").Value = "Stock Ratio";
                ws.Cell("AL8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("AL8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("AL8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("AL8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;


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
                    ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("C" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[2];
                    ws.Cell("C" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[2];
                    ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("C" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("D" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                    ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("D" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("E" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                    ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("E" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    ws.Cell("F" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                    ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("F" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("G" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                    ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("G" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("H" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                    ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("H" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    ws.Cell("I" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                    ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("I" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("J" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                    ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("J" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("k" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("k" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("k" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("k" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                    ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("K" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    ws.Cell("L" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                    ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("L" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("M" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                    ws.Cell("M" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("M" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("N" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                    ws.Cell("N" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("N" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    ws.Cell("O" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                    ws.Cell("O" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("O" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("P" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("P" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("P" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("Q" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                    ws.Cell("Q" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("Q" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    ws.Cell("R" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("R" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("R" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("S" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                    ws.Cell("S" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("S" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("T" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                    ws.Cell("T" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("T" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    ws.Cell("U" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                    ws.Cell("U" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("U" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("V" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[21];
                    ws.Cell("V" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("V" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("W" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("W" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("W" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("W" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("W" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[22];
                    ws.Cell("W" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("W" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    ws.Cell("X" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("X" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("X" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("X" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("X" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[23];
                    ws.Cell("X" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("X" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("Y" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Y" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Y" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Y" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Y" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[24];
                    ws.Cell("Y" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("Y" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("Z" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Z" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Z" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Z" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Z" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[25];
                    ws.Cell("Z" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("Z" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    ws.Cell("AA" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AA" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AA" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AA" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AA" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[26];
                    ws.Cell("AA" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AA" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("AB" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AB" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AB" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AB" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AB" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[27];
                    ws.Cell("AB" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AB" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("AC" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AC" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AC" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AC" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AC" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[28];
                    ws.Cell("AC" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AC" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    ws.Cell("AD" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AD" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AD" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AD" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AD" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[29];
                    ws.Cell("AD" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AD" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("AE" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AE" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AE" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AE" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AE" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[30];
                    ws.Cell("AE" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AE" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("AF" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AF" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AF" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AF" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AF" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[31];
                    ws.Cell("AF" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AF" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    ws.Cell("AG" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AG" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AG" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AG" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AG" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[32];
                    ws.Cell("AG" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AG" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("AH" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AH" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AH" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AH" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AH" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[33];
                    ws.Cell("AH" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AH" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("AI" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AI" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AI" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AI" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AI" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[34];
                    ws.Cell("AI" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AI" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    ws.Cell("AJ" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AJ" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AJ" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AJ" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AJ" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[35];
                    ws.Cell("AJ" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AJ" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("AK" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AK" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AK" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AK" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AK" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[36];
                    ws.Cell("AK" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AK" + recNo).Style.NumberFormat.Format = "#,##0";

                    ws.Cell("AL" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AL" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AL" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AL" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("AL" + recNo).Value = ((System.Data.DataRow)(row)).ItemArray[37];
                    ws.Cell("AL" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; ws.Cell("AL" + recNo).Style.NumberFormat.Format = "#,##0.0";

                    recNo++;
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
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

        public ActionResult LaporanFaktur(string CompanyCode, string BranchCode, string DateFrom, string DateTo, string ReportID, string SDate, string EDate)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = ReportID + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_" + ReportID;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ReqDateFrom", SDate);
            cmd.Parameters.AddWithValue("@ReqDateTo", EDate);

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
                var ws = wb.Worksheets.Add("Laporan Faktur");
                var hdrTable = ws.Range("A1:C5");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A5:A5");
                ws.Range("A5", "C5").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A5", "C5").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A5", "C5").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A5", "C5").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText(); ;

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                ws.Columns("1").Width = 50;
                ws.Columns("2").Width = 10;
                ws.Columns("3").Width = 5;

                //First Names   
                ws.Cell("A1").Value = "LAPORAN FAKTUR";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                if (ReportID == "OmRpSalRgs020")
                {
                    ws.Cell("A2").Value = "Per Dealer dan Wilayah";
                }
                else if (ReportID == "OmRpSalRgs021")
                {
                    ws.Cell("A2").Value = "Per Dealer dan Kecamatan";
                }
                else if (ReportID == "OmRpSalRgs022")
                {
                    ws.Cell("A2").Value = "Per Tipe dan Dealer";
                }
                else if (ReportID == "OmRpSalRgs023")
                {
                    ws.Cell("A2").Value = "Per Tipe dan Wilayah ";
                }
                else if (ReportID == "OmRpSalRgs024")
                {
                    ws.Cell("A2").Value = "Per Tipe dan Kecamatan";
                }
                ws.Cell("A3").Value = "Periode : " + SDate + " s/d " + EDate;
                ws.Cell("A4").Value = "";

                return GenerateExcel(wb, dt, lastRow, fileName, false, true);
            }
        }

        public ActionResult CustomerListSalesUnit(string CompanyCode, string BranchCode, string SalesModelCode, string PaidDateFrom, string PaidDateTo, string SODateFrom, string SODateTo, string BirthDateFrom, string BirthDateTo, string Employee, string Status, string SortBy, string SOFrom, string SOTo, string PaidFrom, string PaidTo, string BirthFrom, string BirthTo)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "OmRpSalRgs027_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_OmRpSalRgs027";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@SalesModelCode", SalesModelCode);
            cmd.Parameters.AddWithValue("@PaidDateFrom", PaidFrom);
            cmd.Parameters.AddWithValue("@PaidDateTo", PaidTo);
            cmd.Parameters.AddWithValue("@BirthDateFrom", BirthFrom);
            cmd.Parameters.AddWithValue("@BirthDateTo", BirthTo);
            cmd.Parameters.AddWithValue("@SODateFrom", SOFrom);
            cmd.Parameters.AddWithValue("@SODateTo", SOTo);
            cmd.Parameters.AddWithValue("@EmployeeName", Employee);
            cmd.Parameters.AddWithValue("@Status", Status);
            cmd.Parameters.AddWithValue("@SortBy", SortBy);

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
                var ws = wb.Worksheets.Add("Customer List - Sales Unit");
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
                ws.Cell("A1").Value = "CUSTOMER LIST - SALES UNIT";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(12);
                ws.Cell("A2").Value = "Proses Date : " + DateTime.Now.ToString("dd-MMM-yyyy"); ;
                ws.Cell("A3").Value = "";
                ws.Cell("A4").Value = "SALES MODEL";
                if (SalesModelCode == "")
                    ws.Cell("B4").Value = ":  ALL";
                else
                    ws.Cell("B4").Value = ":  " + SalesModelCode;

                ws.Cell("A5").Value = "SO DATE";
                if (SODateFrom == "")
                    ws.Cell("B5").Value = ":  ALL   s/d   ALL";
                else
                    ws.Cell("B5").Value = ":  " + SODateFrom + "   s/d   " + SODateTo;

                ws.Cell("A6").Value = "FINAL PAYMENT DATE";
                if (PaidDateFrom == "")
                    ws.Cell("B6").Value = ":  ALL   s/d   ALL";
                else
                    ws.Cell("B6").Value = ":  " + PaidDateFrom + "   s/d   " + PaidDateTo;

                ws.Cell("A7").Value = "BIRTH DATE";
                if (BirthDateFrom == "")
                    ws.Cell("B7").Value = ":  ALL   s/d   ALL";
                else
                    ws.Cell("B7").Value = ":  " + BirthDateFrom + "   s/d   " + BirthDateTo;

                ws.Cell("A8").Value = "SALESMAN";
                if (Employee == "")
                    ws.Cell("B8").Value = ":  ALL";
                else
                    ws.Cell("B8").Value = ":  " + Employee;

                ws.Cell("A9").Value = "STATUS";
                if (Status == "")
                    ws.Cell("B9").Value = ":  ALL";
                else
                    if (Status == "1")
                        ws.Cell("B9").Value = ":  Aktif";
                    else ws.Cell("B9").Value = ":  Tidak Aktif";
                ws.Cell("A10").Value = "SORTING BY";
                if (SortBy == "")
                    ws.Cell("B10").Value = ":  ALL";
                else
                    ws.Cell("B10").Value = ":  " + SortBy;

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public ActionResult SDMSINVDoAttribute(string CompanyCode, string BranchCode, string FromDate, string EndDate, string sFromDate, string sEndDate)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "OmRpPurRgs014_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_OmRpPurRgs014";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DoNo", "");
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 8;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("SDMS INV Do Attribute");
                var hdrTable = ws.Range("A8:U8");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A8:U8");
                ws.Range("A8", "U8").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A8", "U8").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A8", "U8").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A8", "U8").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText(); ;

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                //First Names  
                ws.Range("A1", "F1").Merge();
                ws.Range("A2", "F2").Merge();
                ws.Range("A3", "F3").Merge();
                ws.Range("A4", "F4").Merge();
                ws.Range("A5", "U5").Merge();
                ws.Range("A5", "U5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Range("A6", "U6").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell("A1").Value = "PT. BUANA INDOMOBIL TRADA (PKU) - HOLDING (01)";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(12);
                ws.Cell("A2").Value = "JL. SOEKARNO HATTA NO. 34-40 RT/RW: 008/002 SIDOMULYO TIMUR - MARPOYAN DAMAI ";
                ws.Cell("A3").Value = "PEKAN BARU";
                ws.Cell("A4").Value = "";
                ws.Cell("A5").Value = "SDMS-INV DO Attributes";
                ws.Cell("A6").Value = "PERIODE : " + sFromDate + "  s/d  " + sEndDate;

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public ActionResult DataCustomerProfile(string CompanyCode, string BranchCode, string StartDate, string ToDate, string Order, string ReportID, string From, string To)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = ReportID + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            SqlCommand cmd1 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd1.CommandTimeout = 1800;
            cmd1.CommandText = "uspfn_GetAreaDealerOutlet";

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.Clear();
            cmd1.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd1.Parameters.AddWithValue("@BranchCode", BranchCode);
            SqlDataAdapter ga = new SqlDataAdapter(cmd1);
            DataSet gt = new DataSet();
            ga.Fill(gt);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_" + ReportID;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@InvoiceDateFrom", StartDate);
            cmd.Parameters.AddWithValue("@InvoiceDateTo", ToDate);
            cmd.Parameters.AddWithValue("@OrderBy", Order);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 13;
                int no = 0;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Laporan Data Customer Profile");
                var hdrTable = ws.Range("A1:G13");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A13:R5");
                ws.Range("A13", "R13").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A13", "R13").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A13", "R13").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A13", "R13").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText(); ;

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                ws.Columns("1").Width = 10;
                ws.Columns("2").Width = 30;
                ws.Columns("3").Width = 30;
                ws.Columns("4").Width = 30;
                ws.Columns("5").Width = 30;
                ws.Columns("6").Width = 50;
                ws.Columns("7").Width = 20;
                ws.Columns("8").Width = 20;
                ws.Columns("9").Width = 50;
                ws.Columns("10").Width = 100;
                ws.Columns("11").Width = 40;
                ws.Columns("12").Width = 40;
                ws.Columns("13").Width = 30;
                ws.Columns("14").Width = 30;
                ws.Columns("15").Width = 50;
                ws.Columns("16").Width = 100;
                ws.Columns("17").Width = 30;
                ws.Columns("18").Width = 20;
                ws.Columns("19").Width = 30;
                ws.Columns("20").Width = 20;
                ws.Columns("21").Width = 100;

                //First Names   
                ws.Cell("A1").Value = "Laporan Data Customer Profile";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A2").Value = "Dealer";
                ws.Cell("A3").Value = "Showroom";

                ws.Cell("A4").Value = "Periode";
                ws.Cell("A5").Value = "";

                foreach (var row1 in gt.Tables[0].Rows)
                {
                    ws.Cell("B2").Value = " : " + CompanyCode + " - " + ((System.Data.DataRow)(row1)).ItemArray[0];
                    ws.Cell("B3").Value = " : " + BranchCode + " - " + ((System.Data.DataRow)(row1)).ItemArray[1];
                }
                ws.Cell("B4").Value = " : " + From + " s/d " + To;


                ws.Cell("A13").Value = "No ";
                ws.Cell("B13").Value = "NO SKPK ";
                ws.Cell("C13").Value = "TGL SKPK ";
                ws.Cell("D13").Value = "NO. RANGKA ";
                ws.Cell("E13").Value = "NO. MESIN ";
                ws.Cell("F13").Value = "MODEL ";
                ws.Cell("G13").Value = "WARNA ";
                ws.Cell("H13").Value = "NAMA CUSTOMER ";
                ws.Cell("I13").Value = "ALAMAT ";
                ws.Cell("J13").Value = "KOTA ";
                ws.Cell("K13").Value = "TELPON ";
                ws.Cell("L13").Value = "GENDER ";
                ws.Cell("M13").Value = "TGL. LAHIR ";
                ws.Cell("N13").Value = "NAMA DI STNK ";
                ws.Cell("O13").Value = "ALAMAT ";
                ws.Cell("P13").Value = "KOTA ";
                ws.Cell("Q13").Value = "TELPON ";
                ws.Cell("R13").Value = "NAMA SALESMAN ";

                lastRow = lastRow + 1;
                no = no + 1;

                foreach (var row in ds.Tables[0].Rows)
                {
                    ws.Cell("A" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + lastRow).Value = no;
                    ws.Cell("A" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[1];
                    ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9].ToString();
                    ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                    ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                    ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                    ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[2];
                    ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                    ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                    ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                    ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("L" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                    ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("M" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                    ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("N" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("O" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                    ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("P" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                    ws.Cell("P" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("Q" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[21];
                    ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("R" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                    ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    lastRow++;
                    no++;
                }
                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));

            }
        }

        public ActionResult vehicleownershipinfo(string FromDate, string EndDate, string sFromDate, string sEndDate)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "SalesReportwithVehicleOwnershipInfo_";
            fileName = fileName + sFromDate + '_' + sEndDate;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_OmRpSalRgs048";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@StartDate", FromDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Sales Report With Vehicle Ownership Info");
                var hdrTable = ws.Range("A1:J1");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A1:J1");
                ws.Range("A1", "J1").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A1", "J1").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A1", "J1").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A1", "J1").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText(); ;

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public ActionResult GenDailySalesBranchV2(string CompanyCode, string BranchCode, string StartDate, string ToDate, int ID, string Tipe, string ReportId, string Period1, string Period2)
        {
            string fileName = "";
            string SpId = "";
            fileName = "DailySalesBranchV2";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");
            SpId = "usprpt_" + ReportId;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = SpId;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@companycode", CompanyCode);
            cmd.Parameters.AddWithValue("@branchcode", BranchCode);
            cmd.Parameters.AddWithValue("@period1", StartDate);
            cmd.Parameters.AddWithValue("@period2", ToDate);
            cmd.Parameters.AddWithValue("@ID", ID);
            cmd.Parameters.AddWithValue("@tipe", Tipe);

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);

            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            dt1 = ds.Tables[0];
            dt2 = ds.Tables[2];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                string sheetName = "Sheet1";
                var wb = new XLWorkbook();

                var ws = wb.Worksheets.Add(sheetName);
                ws.ShowGridLines = false;

                ws.Columns("1").Width = 45;
                ws.Columns("2").Width = 15;
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
                ws.Columns("16").Width = 15;
                ws.Columns("17").Width = 15;
                ws.Columns("18").Width = 15;
                ws.Columns("19").Width = 15;
                ws.Columns("20").Width = 15;
                ws.Columns("21").Width = 15;
                ws.Columns("22").Width = 15;

                var hdrTable = ws.Range("A1:V8");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A7:V8");

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText();

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                //First Names   
                ws.Cell("A1").Value = ds.Tables[0].Rows[0][0].ToString();
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(12);

                ws.Cell("A2").Value = ds.Tables[0].Rows[0][1].ToString();
                ws.Cell("A2").Style.Font.SetBold().Font.SetFontSize(12);

                ws.Cell("A3").Value = ds.Tables[0].Rows[0][2].ToString();
                ws.Cell("A3").Style.Font.SetBold().Font.SetFontSize(12);

                ws.Cell("H1").Value = "TGL : " + DateTime.Now.ToString("dd MMM yyyy");
                ws.Cell("H2").Value = "JAM : " + DateTime.Now.ToString("HH:mm:ss");
                ws.Cell("H3").Value = "PID : " + ReportId;

                ws.Range("A5:I5").Merge();
                ws.Range("A5:I5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A5").Value = "DAILY SALES ALL BRANCH ";

                ws.Range("A6:I6").Merge();
                ws.Range("A6:I6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A6").Value = Period1 + " s/d " + Period2;

                ws.Range("A8:A9").Merge();
                ws.Range("A8:A9").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A8:A9").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A8:A9").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A8:A9").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("A8").Value = "TIPE ";

                ws.Range("B8:B9").Merge();
                ws.Range("B8:B9").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("B8:B9").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("B8:B9").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("B8:B9").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Range("B8:B9").Style.Fill.BackgroundColor = XLColor.LightBlue;
                ws.Cell("B8").Value = "TOTAL ALL ";

                string compcd1 = "";
                string compcd2 = "";
                int noc = 1;
                int noctemp = 0;
                int nocclm = 0;
                int lastrow = 10;
                string cell0 = "";
                string cell = "";
                int total = 0;
                int totalSub = 0;
                int grandTotal = 0;

                foreach (var row in ds.Tables[1].Rows)
                {
                    lastrow = 10;
                    cell = CheckCell(noc);

                    compcd1 = ((System.Data.DataRow)(row)).ItemArray[1].ToString();

                    #region ** create header columns dinamis **
                    ws.Cell(cell + "9").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + "9").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + "9").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + "9").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + "9").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(cell + "9").Value = ((System.Data.DataRow)(row)).ItemArray[1];
                    #endregion

                    #region ** field values dinamis columns **
                    nocclm = noc + 1;
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        ws.Cell(cell + lastrow).Value = dt2.Rows[i][nocclm].ToString();

                        total += Convert.ToInt32(dt2.Rows[i][nocclm].ToString());
                        lastrow++;
                    }
                    

                    ws.Cell(cell + lastrow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + lastrow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + lastrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + lastrow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + lastrow).Value = total;

                    #endregion

                    #region ** create header sub total **
                    if (compcd2 != compcd1)
                    {
                        noctemp = noc;
                        noc = noc + 1;
                        cell = CheckCell(noc);
                        ws.Cell(cell + "9").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(cell + "9").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(cell + "9").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(cell + "9").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(cell + "9").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(cell + "9").Style.Fill.BackgroundColor = XLColor.LightBlue;
                        ws.Cell(cell + "9").Value = "Total";

                        cell0 = CheckCell(noctemp);
                        cell = CheckCell(noc);

                        ws.Range(cell0 + "8:" + cell + "8").Merge();
                        ws.Range(cell0 + "8:" + cell + "8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range(cell0 + "8:" + cell + "8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range(cell0 + "8:" + cell + "8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range(cell0 + "8:" + cell + "8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range(cell0 + "8:" + cell + "8").Value = "Company [" + ((System.Data.DataRow)(row)).ItemArray[0] + "]";

                        compcd2 = compcd1;
                    }

                    #endregion

                    #region ** field values sub total **

                    lastrow = 10;
                    nocclm = noc + 1;
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        ws.Cell(cell + lastrow).Value = dt2.Rows[i][nocclm].ToString();
                        ws.Cell(cell + lastrow).Style.Fill.BackgroundColor = XLColor.LightBlue;

                        totalSub += Convert.ToInt32(dt2.Rows[i][nocclm].ToString());

                        lastrow++;
                    }

                    ws.Cell(cell + lastrow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + lastrow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + lastrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + lastrow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + lastrow).Value = totalSub;
                    ws.Cell(cell + lastrow).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    #endregion

                    noc++;
                }

                #region ** Grand Total **
                lastrow = 10;
                foreach (var row in ds.Tables[2].Rows)
                {
                    ws.Cell("A" + lastrow).Value = ((System.Data.DataRow)(row)).ItemArray[0];

                    ws.Cell("B" + lastrow).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("B" + lastrow).Value = ((System.Data.DataRow)(row)).ItemArray[1];

                    grandTotal += Convert.ToInt32(((System.Data.DataRow)(row)).ItemArray[1]);
                    lastrow++;
                }

                ws.Cell("A" + lastrow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + lastrow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + lastrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + lastrow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + lastrow).Value = "GRAND TOTAL";

                ws.Cell("B" + lastrow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + lastrow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + lastrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + lastrow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + lastrow).Style.Fill.BackgroundColor = XLColor.LightBlue;
                ws.Cell("B" + lastrow).Value = grandTotal;

                #endregion

                ws.Cell("A" + (lastrow + 1)).Value = "* Sudah termasuk pengurangan return";

                lastrow = lastrow + 2;
                foreach (var row in ds.Tables[3].Rows)
                {
                    ws.Cell("A" + lastrow).Value = ((System.Data.DataRow)(row)).ItemArray[0]; ;
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));

            }

        }

        public ActionResult GenDailySalesBranchV(string StartDate, string ToDate, string ReportId, string Period1, string Period2)
        {
            string fileName = "";
            string SpId = "";
            fileName = "DailySalesBranchV";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");
            SpId = "usprpt_" + ReportId;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = SpId;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@companycode", CompanyCode);
            cmd.Parameters.AddWithValue("@branchcode", BranchCode);
            cmd.Parameters.AddWithValue("@period1", StartDate);
            cmd.Parameters.AddWithValue("@period2", ToDate);

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);

            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            dt1 = ds.Tables[0];
            dt2 = ds.Tables[2];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                string sheetName = "Sheet1";
                var wb = new XLWorkbook();

                var ws = wb.Worksheets.Add(sheetName);
                ws.ShowGridLines = false;

                ws.Columns("1").Width = 45;
                ws.Columns("2").Width = 15;
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
                ws.Columns("16").Width = 15;
                ws.Columns("17").Width = 15;
                ws.Columns("18").Width = 15;
                ws.Columns("19").Width = 15;
                ws.Columns("20").Width = 15;
                ws.Columns("21").Width = 15;
                ws.Columns("22").Width = 15;

                var hdrTable = ws.Range("A1:V8");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A7:V8");

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText();

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //First Names   
                ws.Cell("A1").Value = ds.Tables[0].Rows[0][0].ToString();
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(12);

                ws.Cell("A2").Value = ds.Tables[0].Rows[0][1].ToString();
                ws.Cell("A2").Style.Font.SetFontSize(12);

                ws.Cell("A3").Value = ds.Tables[0].Rows[0][2].ToString();
                ws.Cell("A3").Style.Font.SetFontSize(12);

                ws.Cell("A3").Value = ds.Tables[0].Rows[0][4].ToString();
                ws.Cell("A3").Style.Font.SetFontSize(12);

                ws.Range("A5:I5").Merge();
                ws.Range("A5:I5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A5").Value = "DAILY SALES ALL BRANCH ";
                ws.Cell("A5").Style.Font.SetBold().Font.SetFontSize(12);

                ws.Range("A6:I6").Merge();
                ws.Range("A6:I6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A6").Value = Period1 + " s/d " + Period2;

                ws.Range("A8:B8").Merge();
                ws.Range("A8:B8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A8:B8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("A8").Value = "TIPE ";

                int noc = 1;
                int nocclm = 0;
                int lastrow = 9;
                string cell = "";
                int total = 0;
                int grandTotal = 0;

                foreach (var row in ds.Tables[1].Rows)
                {
                    lastrow = 9;
                    cell = CheckCell(noc);
                    #region ** create header colom dinamis **
                    ws.Cell(cell + "8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + "8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + "8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(cell + "8").Style.NumberFormat.Format = "0#";
                    ws.Cell(cell + "8").Value = "["+((System.Data.DataRow)(row)).ItemArray[0]+"]";
                    #endregion

                    #region ** field value dinamic columns **
                    nocclm = noc;
                    total = 0;
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        ws.Cell(cell + lastrow).Value = dt2.Rows[i][nocclm].ToString();

                        total += Convert.ToInt32(dt2.Rows[i][nocclm].ToString());
                        lastrow++;
                    }

                    ws.Cell(cell + lastrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + lastrow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell(cell + lastrow).Value = total;

                    #endregion

                    noc++;
                }
                
                lastrow = 9;
                foreach (var row in ds.Tables[2].Rows)
                {
                    ws.Cell("A" + lastrow).Value = ((System.Data.DataRow)(row)).ItemArray[0];
                    lastrow++;
                }

                #region ** create header sub total **
                cell = CheckCell(noc);
                ws.Cell(cell + "8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(cell + "8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell(cell + "8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(cell + "8").Value = "TOTAL";
                #endregion

                #region ** field values sub total **

                lastrow = 9;
                nocclm = nocclm + 1;
                
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    ws.Cell(cell + lastrow).Value = dt2.Rows[i][nocclm].ToString();

                    grandTotal += Convert.ToInt32(dt2.Rows[i][nocclm].ToString());
                    lastrow++;
                }

                ws.Cell(cell + lastrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(cell + lastrow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell(cell + lastrow).Value = grandTotal;

                #endregion

                ws.Range("A" + lastrow + ":" + "B" + lastrow).Merge();
                ws.Range("A" + lastrow + ":" + "B" + lastrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + lastrow + ":" + "B" + lastrow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + lastrow).Value = "GRAND TOTAL ";

                ws.Cell("A" + (lastrow + 1)).Value = "* Note : ";

                lastrow = lastrow + 2;
                int irs = 1;
                foreach (var row in ds.Tables[3].Rows)
                {
                    if ((irs % 2) == 0)
                    {
                        ws.Cell("D" + lastrow).Value = ((System.Data.DataRow)(row)).ItemArray[0];
                        lastrow = lastrow + 1;
                    }
                    else
                    {
                        ws.Cell("A" + lastrow).Value = ((System.Data.DataRow)(row)).ItemArray[0];
                    }
                    irs = irs + 1;
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
        }

        public string CheckCell(int noc)
        {
            string cell = "";
            if (noc == 1) { cell = "C"; }
            if (noc == 2) { cell = "D"; }
            if (noc == 3) { cell = "E"; }
            if (noc == 4) { cell = "F"; }
            if (noc == 5) { cell = "G"; }
            if (noc == 6) { cell = "H"; }
            if (noc == 7) { cell = "I"; }
            if (noc == 8) { cell = "J"; }
            if (noc == 9) { cell = "K"; }
            if (noc == 10) { cell = "L"; }
            if (noc == 11) { cell = "M"; }
            if (noc == 12) { cell = "N"; }
            if (noc == 13) { cell = "O"; }
            if (noc == 14) { cell = "P"; }
            if (noc == 15) { cell = "Q"; }
            if (noc == 16) { cell = "R"; }
            if (noc == 17) { cell = "S"; }
            if (noc == 18) { cell = "T"; }
            if (noc == 19) { cell = "U"; }
            if (noc == 20) { cell = "V"; }
            if (noc == 21) { cell = "W"; }
            if (noc == 22) { cell = "X"; }
            if (noc == 23) { cell = "Y"; }
            if (noc == 24) { cell = "Z"; }
            return cell;
        }
    }
}
