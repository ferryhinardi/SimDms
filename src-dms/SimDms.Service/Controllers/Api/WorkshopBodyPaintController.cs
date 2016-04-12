using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class WorkshopBodyPaintController : BaseController
    {
        public JsonResult WorkshopBodyPaintGrid()
        {
            DataTable dt = new DataTable(); 
            string companycode = CompanyCode;
            string branchcode = BranchCode;
            string producttype = ProductType;
            string periodyear = Request["Year"];
            string periodmonth = Request["Month"];
            string user = CurrentUser.UserId;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_srvworkshopbodypaintGrid";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companycode);
            cmd.Parameters.AddWithValue("@BranchCode", branchcode);
            cmd.Parameters.AddWithValue("@ProductType", producttype);
            cmd.Parameters.AddWithValue("@PeriodYear", periodyear);
            cmd.Parameters.AddWithValue("@Month0", periodmonth);
            cmd.Parameters.AddWithValue("@user", user);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];

            return Json(GetJson(dt));
        }

        public ActionResult ExportExcelBodyRepair(string SpID, string periodYear, string periodMonth)
        {
            string fileName = "";
            fileName = "WorkshopBP" + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            string companycode = CompanyCode;
            string branchcode = BranchCode;
            string producttype = ProductType;
            string user = CurrentUser.UserId;
            int periodyear = Convert.ToInt32(periodYear);
            int periodmonth = Convert.ToInt32(periodMonth);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "EXEC " + SpID + " '" + companycode + "','" + branchcode + "','" + producttype + "','" + periodyear + "','" + periodmonth + "','" + user + "'";
            cmd.CommandTimeout = 360;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            var wb = new XLWorkbook();
            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }

            #region ** format Excell  By Branch **
            else
            {
                int lastRow = 9;
                int index = 0;
                int indexRow = 0;

                var noGroupCheck1 = "";
                var noGroupCheck2 = "";

                var sheetName = ds.Tables[0].Rows[0][1].ToString();
                var ws = wb.Worksheets.Add(sheetName);

                #region ** write header **
                var hdrTable = ws.Range("A1:E6");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                var rngTable = ws.Range("A8:E8");
                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText();

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                ws.Columns("1").Width = 10;
                ws.Columns("2").Width = 80;
                ws.Columns("3").Width = 17;
                ws.Columns("4").Width = 17;
                ws.Columns("5").Width = 17;

                //Header Name   
                ws.Range("A1:E1").Merge();
                ws.Range("A1:E1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range("A1:E1").Value = "Laporan Bulanan Workshop Body & Paint Suzuki ";

                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A2").Value = "Dealer";
                ws.Cell("A3").Value = "Alamat";
                ws.Cell("D2").Value = "Cetak ";
                ws.Cell("D2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A4").Value = "Kota ";
                ws.Cell("A5").Value = "Telpon ";
                ws.Cell("A6").Value = "Bln & Thn";

                ws.Cell("B2").Value = ": "+ dt2.Rows[0][0].ToString();
                ws.Cell("B3").Value = ": " + dt2.Rows[0][1].ToString();
                ws.Cell("E2").Value = DateTime.Now.ToString();
                ws.Cell("E2").Style.DateFormat.Format = "DD-MMM-YYYY HH:mm";
                ws.Cell("B4").Value = ": " + dt2.Rows[0][2].ToString();
                ws.Cell("B5").Value = ": " + dt2.Rows[0][3].ToString();
                ws.Cell("B6").Value = ": " + dt2.Rows[0][4].ToString();

                ws.Cell("A8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("A8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("A8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("A8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("A8").Style.Fill.BackgroundColor = XLColor.LightBlue;
                ws.Cell("A8").Value = "Number";

                ws.Cell("B8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("B8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("B8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("B8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("B8").Style.Fill.BackgroundColor = XLColor.LightBlue;
                ws.Cell("B8").Value = "Descriptions ";

                ws.Cell("C8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("C8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("C8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("C8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("C8").Style.Fill.BackgroundColor = XLColor.LightBlue;
                ws.Cell("C8").Value = "Suzuki ";

                ws.Cell("D8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("D8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("D8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("D8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("D8").Style.Fill.BackgroundColor = XLColor.LightBlue;
                ws.Cell("D8").Value = "Non Suzuki ";

                ws.Cell("E8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("E8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("E8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("E8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("E8").Style.Fill.BackgroundColor = XLColor.LightBlue;
                ws.Cell("E8").Value = "Total ";

                #endregion

                #region ** field values **
                foreach (var row in dt1.Rows)
                {
                    noGroupCheck2 = ((System.Data.DataRow)(row)).ItemArray[4].ToString();
                    if (noGroupCheck1 != noGroupCheck2)
                    {
                        indexRow = 0;
                        index = index + 1;
                        if (index == 30)
                        {
                            indexRow = indexRow + (6 - 1);
                        }

                        //No Group
                        ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Merge();
                        ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    }

                    //Description
                    ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                    ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    //Suzuki
                    ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                    ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("C" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                    //Non Suzuki
                    ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                    ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                    //Total
                    ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                    ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("E" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                    noGroupCheck1 = noGroupCheck2;
                    lastRow++;
                }

                #endregion

            }
            #endregion

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));

        }
    }
}