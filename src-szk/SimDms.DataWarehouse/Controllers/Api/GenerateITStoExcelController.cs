using System;
using System.Collections.Generic;
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
using System.IO;
using System.Net;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class GenerateITStoExcelController : BaseController
    {
        public JsonResult GenerateITS()
        {
            List<object> listOfMonths = new List<object>();
            int Now = DateTime.Now.Year;
            int Past = DateTime.Now.Year - 10;

            listOfMonths.Add(new { value = 0, text = "With Status And Test Drive" });
            listOfMonths.Add(new { value = 1, text = "By Periode" });
            listOfMonths.Add(new { value = 2, text = "By FollowUp" });
            return Json(listOfMonths);
        }

        private class prmProc
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Type { get; set; }
            public string GroupArea { get; set; }
        }

        private DataTable CreateTable(prmProc param, string SPName)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = SPName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Year", param.Year);
            cmd.Parameters.AddWithValue("@Month", param.Month);
            cmd.Parameters.AddWithValue("@Type", param.Type); ;
            cmd.Parameters.AddWithValue("@GroupAre", param.GroupArea); ;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        //public ActionResult GenerateITSWithStatusAndTestDrive(string Year, string Month, string Type)
        //{
        //    DateTime now = DateTime.Now;
        //    string fileName = "ITS_Report_GenerateITSWithStatusAndTestDrive" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd");

        //    System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
        //    response.ClearContent();
        //    response.Clear();
        //    response.ContentType = "text/plain";
        //    response.AddHeader("Content-Disposition",
        //                       "attachment; filename=" + fileName + ";");
        //    response.TransmitFile("D:/SimDms/src-szk/SimDms.Web");
        //    response.Flush();
        //    response.End();

        //    return Redirect(Url.Content("~/Reports/" + fileName + ".xlsx"));
        //}

        public FileContentResult GenerateITSWithStatusAndTestDrive(string Year, string Month, string Type) 
        {
            string file = "";
            string fileName = "";

            if (Type == "0") {
                file = "pmHstGenerateITSwithStatusAndTD";
            }
            else if (Type == "1")
            {
                file = "pmHstGenerateITS_ByPeriode";
            }
            else if (Type == "2") {
                file = "pmHstGenerateITS_ByFollowUp";
            }

            fileName = "ITS_Report_GenerateITS_" + file + ".xls";

            //try
            //{
            WebClient req = new WebClient();
            var contentType = "text/csv";
            HttpResponse response = System.Web.HttpContext.Current.Response;
            string filePath = "D:/SimDms/src-szk/SimDms.Web/Reports/" + file + "_" + Year + "_" + Month + ".xls";
            //string filePath = "E:/ManagementReports/DailySalesReport/ITS_Report_GenerateITSWithStatusAndTD_2016_1.xls";
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Buffer = true;
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            byte[] data = req.DownloadData(filePath);
            response.BinaryWrite(data);
            response.End();

            return File(data, contentType, "");
            //}
            //catch
            //{
            //    return new HttpNotFoundResult();
            //}

        }

        public class HttpNotFoundResult : ActionResult
        {
            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                context.HttpContext.Response.StatusCode = 404;
            }
        }

        //public ActionResult GenerateITSWithStatusAndTestDrive(string Year, string Month, string Type)
        //{
        //    HttpContext.Server.ScriptTimeout = 13600;
        //    DateTime now = DateTime.Now;
        //    string fileName = "ITS_Report_GenerateITSWithStatusAndTestDrive" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd");

        //    SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
        //    cmd.CommandTimeout = 13600;
        //    cmd.CommandText = "SELECT * FROM GroupArea";
        //    cmd.CommandType = CommandType.Text;

        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();
        //    da.Fill(ds);
        //    DataTable GroupArea = ds.Tables[0];
        //    var wb = new XLWorkbook();

        //    foreach (DataRow Area in GroupArea.Rows)
        //    {
        //        prmProc param = new prmProc
        //        {
        //            Year = Convert.ToInt32(Year),
        //            Month = Convert.ToInt32(Month),
        //            Type = Convert.ToInt32(Type),
        //            GroupArea = Area["AreaDealer"].ToString()
        //        };
        //        DataTable dt = CreateTable(param, "uspfn_GenerateITStoExecl");

        //        int recNo = 1;
        //        var ws = wb.Worksheets.Add(Area["AreaDealer"].ToString().Replace('/',' '));

        //        //First Names
        //        ws.Cell("A" + recNo).Value = "Area";
        //        ws.Cell("B" + recNo).Value = "Dealer";
        //        ws.Cell("C" + recNo).Value = "Abbr";
        //        ws.Cell("D" + recNo).Value = "Outlet";
        //        ws.Cell("E" + recNo).Value = "OutletAbbr";
        //        ws.Cell("F" + recNo).Value = "CityId";
        //        ws.Cell("G" + recNo).Value = "CityName";
        //        ws.Cell("H" + recNo).Value = "Date";
        //        ws.Cell("I" + recNo).Value = "WorkDay";
        //        ws.Cell("J" + recNo).Value = "Model";
        //        ws.Cell("K" + recNo).Value = "Var";
        //        ws.Cell("L" + recNo).Value = "ColourName";
        //        ws.Cell("M" + recNo).Value = "Inq";
        //        ws.Cell("N" + recNo).Value = "InqTestDrive";
        //        ws.Cell("O" + recNo).Value = "HP";
        //        ws.Cell("P" + recNo).Value = "HPTestDrive";
        //        ws.Cell("Q" + recNo).Value = "SPK";
        //        ws.Cell("R" + recNo).Value = "SPKTesDrive";
        //        ws.Cell("S" + recNo).Value = "DO";
        //        ws.Cell("T" + recNo).Value = "DOTesDrive";
        //        ws.Cell("U" + recNo).Value = "DELIVERY";
        //        ws.Cell("V" + recNo).Value = "DELIVERYTesDrive";
        //        ws.Cell("W" + recNo).Value = "INVOICE";
        //        ws.Cell("X" + recNo).Value = "INVOICETesDrive";
        //        ws.Cell("Y" + recNo).Value = "LOST";

        //        recNo++;

        //        foreach (DataRow row in dt.Rows)
        //        {
        //            ws.Cell("A" + recNo).Value = row["Area"].ToString();
        //            ws.Cell("B" + recNo).Value = row["Dealer"].ToString();
        //            ws.Cell("C" + recNo).Value = row["Abbr"].ToString();
        //            ws.Cell("D" + recNo).Value = row["OutletCode"].ToString();
        //            ws.Cell("E" + recNo).Value = row["OutletAbbreviation"].ToString();
        //            ws.Cell("F" + recNo).Value = row["CityId"].ToString();
        //            ws.Cell("G" + recNo).Value = row["CityName"].ToString();
        //            ws.Cell("H" + recNo).Value = row["Date"].ToString();
        //            ws.Cell("I" + recNo).Value = row["WorkDay"].ToString();
        //            ws.Cell("J" + recNo).Value = row["Model"].ToString();
        //            ws.Cell("K" + recNo).Value = row["Var"].ToString();
        //            ws.Cell("L" + recNo).Value = row["ColourName"].ToString();
        //            ws.Cell("M" + recNo).Value = row["INQ"].ToString();
        //            ws.Cell("N" + recNo).Value = row["InqTestDrive"].ToString();
        //            ws.Cell("O" + recNo).Value = row["HP"].ToString();
        //            ws.Cell("P" + recNo).Value = row["HPTestDrive"].ToString();
        //            ws.Cell("Q" + recNo).Value = row["SPK"].ToString();
        //            ws.Cell("R" + recNo).Value = row["SPKTestDrive"].ToString();
        //            ws.Cell("S" + recNo).Value = row["DO"].ToString();
        //            ws.Cell("T" + recNo).Value = row["DOTestDrive"].ToString();
        //            ws.Cell("U" + recNo).Value = row["DELIVERY"].ToString();
        //            ws.Cell("V" + recNo).Value = row["DELIVERYTestDrive"].ToString();
        //            ws.Cell("W" + recNo).Value = row["INVOICE"].ToString();
        //            ws.Cell("X" + recNo).Value = row["INVOICETestDrive"].ToString();
        //            ws.Cell("Y" + recNo).Value = row["LOST"].ToString();
        //            recNo++;
        //        }
        //    }

        //    Response.Clear();
        //    Response.Buffer = true;
        //    Response.Charset = "";
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.AddHeader("content-disposition", "attachment;filename="+fileName+".xlsx");
        //    using (MemoryStream MyMemoryStream = new MemoryStream())
        //    {
        //        wb.SaveAs(MyMemoryStream);
        //        MyMemoryStream.WriteTo(Response.OutputStream);
        //        Response.Flush();
        //        Response.End();
        //    }

        //    //wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
        //    return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        //}
    }
}