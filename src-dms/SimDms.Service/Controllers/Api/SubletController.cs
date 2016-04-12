using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using SimDms.Common.DcsWs;
using SimDms.Common.Models;
using System.Text;
using System.IO;
using ClosedXML.Excel;


namespace SimDms.Service.Controllers.Api
{
    public class SubletController : BaseController
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
                Now=DateTime.Now
            });
               
        }

        public ActionResult GenerateSubletXls(DateTime startdate,DateTime enddate,string rtype)
        {

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("EXEC usprpt_SvRpSublet '{0}','{1}','{2}','{3}'",CompanyCode,BranchCode,startdate.ToString("yyyyMMdd"),enddate.ToString("yyyyMMdd"));
            cmd.CommandTimeout = 360;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt1 = new DataTable();
            dt1 = ds.Tables[0];
            int indexrow = 0;

            

            var wb = new XLWorkbook();
            string snotfound = "<h1>Tidak ada data</h1>";

            if (dt1.Rows.Count == 0)
            {
                return Content(snotfound);
            }
            string fileName = "";
            fileName = "List_Sublet_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");


            var sheetName = "Sublet";
            var ws = wb.Worksheets.Add(sheetName);


            ws.Cell("A"+ 1).Value = "List Sublet";
            ws.Cell("A" + 2).Value = "Periode:" + startdate.ToString("dd MMM yyyy") + " s/d " + enddate.ToString("dd MMM yyyy");

            indexrow = 3;
            ws.Cell("A" + indexrow).Value = "Nomor";
            ws.Cell("B" + indexrow).Value = "InvoiceNo";
            ws.Cell("C" + indexrow).Value = "InvoiceDate";
            ws.Cell("D" + indexrow).Value = "Kode Pekerjaan/Nomor Item";
            ws.Cell("E" + indexrow).Value = "Nama Pekerjaan/Nama Item";
            ws.Cell("F" + indexrow).Value = "QTY";
            ws.Cell("G" + indexrow).Value = "Retail Price";
            ws.Cell("H" + indexrow).Value = "Discount%";
            ws.Cell("I" + indexrow).Value = "Discount Price";
            ws.Cell("J" + indexrow).Value = "DPP";
            ws.Cell("K" + indexrow).Value = "PPN";
            ws.Cell("L" + indexrow).Value = "Total Sales";
            ws.Cell("M" + indexrow).Value = "Total Cost";

            int ino=1;
            foreach (DataRow dr in dt1.Rows)
            {
                
                indexrow++;
                ws.Cell("A" + indexrow).Value = ino;
                ws.Cell("B" + indexrow).Value = dr["InvoiceNo"].ToString();
                ws.Cell("C" + indexrow).Value = ((DateTime)dr["InvoiceDate"]).ToString("dd/MM/yyyy");
                ws.Cell("D" + indexrow).Value = dr["TaskPartID"].ToString();
                ws.Cell("E" + indexrow).Value = dr["TaskPartDesc"].ToString();
                ws.Cell("F" + indexrow).Value = (decimal)dr["Qty"];
                ws.Cell("G" + indexrow).Value = (decimal)dr["RetailPrice"];
                ws.Cell("H" + indexrow).Value = (decimal)dr["Discount"];
                ws.Cell("I" + indexrow).Value = (decimal)dr["DiscountPrice"];
                ws.Cell("J" + indexrow).Value = (decimal)dr["DPP"];
                ws.Cell("K" + indexrow).Value =(decimal)dr["PPN"];
                ws.Cell("L" + indexrow).Value = (decimal)dr["TotalSales"];
                ws.Cell("M" + indexrow).Value = (decimal)dr["TotalCost"];

                ino++;
            }

            ws.Range("A1", "M1").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Range("A2", "M2").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Range("A3", "M3").Style.Font.SetBold();
            ws.Range("A3", "M"+indexrow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A3", "M"+indexrow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A3", "M"+indexrow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("A3", "M"+indexrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;

            ws.Range("G4", "G" + indexrow).Style.NumberFormat.Format = "#,##0";
            ws.Range("I4", "M" + indexrow).Style.NumberFormat.Format = "#,##0";

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }
    }
}