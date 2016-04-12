using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoddleReport;
using DoddleReport.Web;
using SimDms.Web;
using GeLang;

using System.Data.SqlClient;
using DoddleReport.OpenXml;
using TracerX;
using Newtonsoft.Json.Linq;

namespace SimDms.Web.Controllers
{
    public class DoReportController : BaseController
    {
       
        [HttpPost, FileDownload]
        public ReportResult Customers()
        {

            string flag1 = Convert.ToBoolean(Request["AllowPeriod"]) ? "1" : "0";
            DateTime StartDate = Convert.ToDateTime(Request["StartDate"]);
            DateTime EndDate = Convert.ToDateTime(Request["EndDate"]);

            string dtFirstDate = StartDate.ToString("yyyy-MM-dd");
            string dtLastDate = EndDate.ToString("yyyy-MM-dd");

            string sPeriod = "ALL";

            if (flag1=="1")
            {
                sPeriod = StartDate.ToString("dd-MMM-yyyy") + @" s/d " + EndDate.ToString("dd-MMM-yyyy");
            }

            string Area = Request["Area"] ?? "";
            string DealerName = Request["DealerName"] ?? "";
            string Branch = Request["Branch"] ?? "";
            string BranchName = Request["BranchName"] ?? "";
            string Abbr = Request["Abbr"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "usprpt_QueryCustomerDealer2 '" + flag1 + "','" + dtFirstDate + "','" + dtLastDate + "','','','" + Branch + "'";

            MyLogger.Log.Info("Inquiry Customers: EXEC " + cmd.CommandText);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            da.Fill(dt);
            var records = dt.Tables[0];
            var rows = records.Rows.Count;

            var mainReport = new Report(records.ToReportSource(), new ExcelReportWriter());
            mainReport.RenderHints["SheetName"] = "Customer List - " + Abbr;

            mainReport.DataFields["CompanyCode"].Hidden = true;

            //mainReport.TextFields.Title = "Inquiry Customers";
            //mainReport.TextFields.SubTitle = "Detail Report";
            mainReport.TextFields.Header = string.Format(@"I N Q U I R Y   C U S T O M E R S

Report Generated On {0}

    Period          : {1}
    Area              : {2}
    Dealer          : {3}
    Outlet          : {4}",
    DateTime.Now.ToString(), sPeriod, Area, DealerName, BranchName);


            //mainReport.RenderHints.FreezeRows = 10;
            //mainReport.RenderHints.FreezeColumns = 4;
           
            mainReport.RenderingRow += mainReport_RenderingRow;
                      
            return new ReportResult(mainReport);
        }

        void mainReport_RenderingRow(object sender, ReportRowEventArgs e)
        {
            int n = e.Row.Fields.Count();

            switch (e.Row.RowType)
            {
                case ReportRowType.HeaderRow:
                    for (int i=0;i<=n;i++)
                    {
                        e.Row.Fields[i].HeaderStyle.BackColor = Color.DarkBlue;
                        e.Row.Fields[i].HeaderStyle.ForeColor = Color.White;
                        e.Row.Fields[i].HeaderStyle.FontSize = 10;
                        e.Row.Fields[i].HeaderStyle.Underline = false;
                    }        
                    break;
                //case ReportRowType.DataRow:
                //    {
                //        for (int i = 8; i < nFields; i++)
                //        {
                //            var value = (int)e.Row[e.Row.Fields[i]];
                //            e.Row.Fields[i].DataStyle.HorizontalAlignment = HorizontalAlignment.Right;
                //            e.Row.Fields[i].DataFormatString = "#,#";
                //            if (value == 0)
                //            {
                //                e.Row.Fields[i].DataStyle.BackColor = Color.LightGray;
                //            }
                //        }
                //    }
                //    break;
            }
        }

        [HttpPost, FileDownload]
        public ReportResult InquiryITS()
        {

            DateTime StartDate = Convert.ToDateTime(Request["dtpFrom"]);
            DateTime EndDate = Convert.ToDateTime(Request["dtpTo"]);

            string dtFirstDate = StartDate.ToString("yyyy-MM-dd");
            string dtLastDate = EndDate.ToString("yyyy-MM-dd");

            string sPeriod = "";

            sPeriod = StartDate.ToString("dd-MMM-yyyy") + @" s/d " + EndDate.ToString("dd-MMM-yyyy");

            string Area = Request["Area"] ?? "";
            string Dealer = Request["Dealer"] ?? "";
            string DealerName = Request["DealerName"] ?? "";
            string Outlet = Request["Outlet"] ?? "";
            string OutletName = Request["OutletName"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_InquiryITS '" + dtFirstDate + "','" + dtLastDate + "','" + Area + "','" + Dealer + "','" + Outlet + "'";

            MyLogger.Log.Info("Inquiry ITS: EXEC " + cmd.CommandText);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            da.Fill(dt);
            var records = dt.Tables[0];
            var rows = records.Rows.Count;

            var mainReport = new Report(records.ToReportSource(), new ExcelReportWriter());
            mainReport.RenderHints["SheetName"] = "Inquiry ITS - ";

            //mainReport.DataFields["CompanyCode"].Hidden = true;

            //mainReport.TextFields.Title = "Inquiry Customers";
            //mainReport.TextFields.SubTitle = "Detail Report";
            mainReport.TextFields.Header = string.Format(@"I T S R e p o r t  
Report Date: {0}

    Period          : {1}
    Area              : {2}
    Dealer          : {3}
    Outlet          : {4}",
    DateTime.Now.ToString(), sPeriod, Area, DealerName, OutletName);


            //mainReport.RenderHints.FreezeRows = 10;
            //mainReport.RenderHints.FreezeColumns = 4;

            mainReport.RenderingRow += inqITS_RenderingRow;

            return new ReportResult(mainReport);

        }

        [HttpPost, FileDownload]
        public ReportResult AosItemSpList()
        {
            var user = CurrentUser;
            string typeOfGood = Request["typeOfGood"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_spAutomaticOrderSparepartList '" + user.CompanyCode + "','" + BranchCode + "', "+ typeOfGood;

            MyLogger.Log.Info("AOS Item Sparepart List: EXEC " + cmd.CommandText);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            da.Fill(dt);
            var records = dt.Tables[0];
            var rows = records.Rows.Count;

            var mainReport = new Report(records.ToReportSource(), new ExcelReportWriter());
            mainReport.RenderHints["SheetName"] = "AOS Item SP List ";
            mainReport.TextFields.SubTitle = "Generate AOS Item Sparepart List";
            mainReport.TextFields.Header = string.Format(@" Report Date: {0}

    Dealer          : {1}
    Outlet          : {2}",
    DateTime.Now.ToString(), user.CompanyCode, user.BranchCode);

            mainReport.RenderingRow += AosItemSp_RenderingRow;

            return new ReportResult(mainReport);

        }

        void inqITS_RenderingRow(object sender, ReportRowEventArgs e)
        {
            int n = e.Row.Fields.Count();

            switch (e.Row.RowType)
            {
                case ReportRowType.HeaderRow:
                    for (int i = 0; i <= n-1; i++)
                    {
                        e.Row.Fields[i].HeaderStyle.BackColor = Color.DarkBlue;
                        e.Row.Fields[i].HeaderStyle.ForeColor = Color.White;
                        e.Row.Fields[i].HeaderStyle.FontSize = 10;
                        e.Row.Fields[i].HeaderStyle.Underline = false;
                    }
                    break;
            }
        }

        void AosItemSp_RenderingRow(object sender, ReportRowEventArgs e)
        {
            int n = e.Row.Fields.Count();

            switch (e.Row.RowType)
            {
                case ReportRowType.HeaderRow:
                    for (int i = 0; i <= n - 1; i++)
                    {
                        e.Row.Fields[i].HeaderStyle.FontSize = 10;
                        e.Row.Fields[i].HeaderStyle.Underline = false;
                    }
                    break;

                case ReportRowType.DataRow:
                    for (int i = 3; i <= n - 3; i++)
                    {
                        e.Row.Fields[i].DataStyle.HorizontalAlignment = HorizontalAlignment.Right;
                    }
                    break;

                
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class FileDownloadAttribute : ActionFilterAttribute
    {
        public FileDownloadAttribute(string cookieName = "fileDownload", string cookiePath = "/")
        {
            CookieName = cookieName;
            CookiePath = cookiePath;
        }

        public string CookieName { get; set; }

        public string CookiePath { get; set; }

        /// <summary>
        /// If the current response is a FileResult (an MVC base class for files) then write a
        /// cookie to inform jquery.fileDownload that a successful file download has occured
        /// </summary>
        /// <param name="filterContext"></param>
        private void CheckAndHandleFileResult(ActionExecutedContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var response = httpContext.Response;

            if (filterContext.Result is ReportResult)
                //jquery.fileDownload uses this cookie to determine that a file download has completed successfully
                response.AppendCookie(new HttpCookie(CookieName, "true") { Path = CookiePath });
            else
                //ensure that the cookie is removed in case someone did a file download without using jquery.fileDownload
                if (httpContext.Request.Cookies[CookieName] != null)
                {
                    response.AppendCookie(new HttpCookie(CookieName, "true") { Expires = DateTime.Now.AddYears(-1), Path = CookiePath });
                }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            CheckAndHandleFileResult(filterContext);

            base.OnActionExecuted(filterContext);
        }
    }

}
