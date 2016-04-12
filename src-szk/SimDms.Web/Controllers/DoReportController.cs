

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
using SimDms.DataWarehouse.Helpers;
using System.Data.SqlClient;
using DoddleReport.OpenXml;
using SimDms.Web.Models;
using TracerX;

namespace SimDms.Web.Controllers
{
    public class DoReportController : BaseController
    {

        public JsonResult CreateReportSession(ReportSession data)
        {
            var UserId = CurrentUser.Username;
            data.SessionId = MyLogger.GetMD5(UserId + DateTime.Now.ToString("G"));
            data.CreatedBy = UserId;
            data.CreatedDate = DateTime.Now;

            ctx.ReportSessions.Add(data);
            ctx.SaveChanges();

            var sql = "delete from reportsession where createdby='" + UserId + "' and sessionid !='" + data.SessionId + "'";
            ctx.Database.SqlQuery<object>(sql).FirstOrDefault();

            return Json(new { success = true, SessionId = data.SessionId }, JsonRequestBehavior.AllowGet);
        }

        private void SetHeaderReport(Report mainReport, string Area, string Filter, DateTime Period, string C, string B)
        {
            // Customize the Text Fields
            mainReport.TextFields.Title = "Generate Summary Unit Intake";
            mainReport.TextFields.SubTitle = "Daily Report";
            mainReport.TextFields.Header = string.Format(@"{0}
    Period          : {1}
    Filter By       : {2}
    Area              : {3}
    Dealer          : {4}
    Outlet          : {5}", "",
    Period.Year.ToString() + @"/" + Period.Month.ToString(),
    Filter, Area, C,B);

            mainReport.TextFields.Footer = @"

    Report Generated On " + DateTime.Now.ToString();

            // Render hints allow you to pass additional hints to the reports as they are being rendered
            mainReport.RenderHints.BooleanCheckboxes = true;
            mainReport.RenderHints.BooleansAsYesNo = true;
            mainReport.RenderHints.FreezeRows =10;
            mainReport.RenderHints.FreezeColumns = 5;


            mainReport.RenderingRow += report_RenderingRow;

            //int nFields = mainReport.DataFields.Count;
            //for (int i = 7; i < nFields; i++)
            //{
            //    mainReport.DataFields[i].ShowTotals = true;
            //}


        }


        [HttpPost, FileDownload]
        public ReportResult SumUnitIntake()
        {
            DataTable dt = new DataTable();
            string Area = Request["Area"];
            string Dealer = Request["Dealer"];
            string Outlet = Request["Outlet"];
            string DatePeriod = Request["DatePeriod"];
            string DateTo = Request["DateTo"];
            string ReportBy = Request["ReportBy"];
            string FilterBy = Request["FilterBy"];
            string ProductType = Request["ProductType"];

            string AreaName = Request["AreaName"];
            string FilterName = Request["FilterName"];
            string C = Request["CompanyName"];
            string B = Request["BranchName"];

            string PDI = Request["PDI"];

            if (string.IsNullOrEmpty(C)) C = "ALL";
            if (string.IsNullOrEmpty(B)) B = "ALL";
            if (string.IsNullOrEmpty(AreaName)) AreaName = "ALL";


            DateTime Period = Convert.ToDateTime(DatePeriod);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_SrvSummaryUnitIntake";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("GroupNo", Area);
            cmd.Parameters.AddWithValue("CompanyCode", Dealer);
            cmd.Parameters.AddWithValue("BranchCode", Outlet);
            cmd.Parameters.AddWithValue("ProductType", ProductType);
            cmd.Parameters.AddWithValue("ReportBy", ReportBy);
            cmd.Parameters.AddWithValue("PeriodYear", Period.Year );
            cmd.Parameters.AddWithValue("PeriodMonth", Period.Month );
            cmd.Parameters.AddWithValue("FilterBy", FilterBy);
            cmd.Parameters.AddWithValue("PDI", PDI);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            var results1 = ds.Tables[0];
            var results2 = ds.Tables[1];
            var results3 = ds.Tables[2];

            var mainReport = new Report(results1.ToReportSource(), new ExcelReportWriter());
            mainReport.RenderHints["SheetName"] = "Daily";

            SetHeaderReport(mainReport, AreaName, FilterName, Period,C,B);

            var report2 = new Report(results2.ToReportSource(), new ExcelReportWriter());
            report2.RenderHints["SheetName"] = "Weekly";

            SetHeaderReport(report2, AreaName, FilterName, Period, C, B);
            report2.TextFields.SubTitle = "Weekly Report";

            var report3 = new Report(results3.ToReportSource(), new ExcelReportWriter());
            report3.RenderHints["SheetName"] = "Monthly";

            SetHeaderReport(report3, AreaName, FilterName, Period, C, B);
            report3.TextFields.SubTitle = "Monthly Report";

            var writer = new ExcelReportWriter();
            writer.AppendReport(mainReport, report2);
            writer.AppendReport(mainReport, report3);

            return new ReportResult(mainReport, writer);

        }


        void report_RenderingRow(object sender, ReportRowEventArgs e)
        {

            int nFields = e.Row.Fields.Count();


            switch (e.Row.RowType)
            {
                case ReportRowType.HeaderRow:
                    for (int i = 8; i < nFields; i++)
                    {
                        e.Row.Fields[i].HeaderStyle.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                    break;
                case ReportRowType.DataRow:
                    {
                        for (int i = 8; i < nFields; i++)
                        {
                            var value = (int)e.Row[e.Row.Fields[i]];
                            e.Row.Fields[i].DataStyle.HorizontalAlignment = HorizontalAlignment.Right;
                            e.Row.Fields[i].DataFormatString = "#,#";                            
                            if (value == 0)
                            {
                                e.Row.Fields[i].DataStyle.BackColor = Color.LightGray;
                            }
                        }
                    }
                    break;
            }
        }


        [HttpPost, FileDownload]
        public ReportResult DatabaseSubmission()
        {
            DataTable dt = new DataTable();
            string Area = Request["Area"];
            string Dealer = Request["Dealer"];
            string Outlet = Request["Outlet"];
            string DatePeriodStart = Request["PeriodStart"];
            string DatePeriodEnd = Request["PeriodEnd"];
            string ReportBy = Request["ReportBy"];
            string FilterBy = Request["FilterBy"];
            string ProductType = Request["ProductType"];
            string JobGroup = Request["JobGroup"];

            string AreaName = Request["AreaName"];
            string FilterName = Request["FilterName"];
            string C = Request["CompanyName"];
            string B = Request["BranchName"];
            string JobName = Request["JobName"];

            if (string.IsNullOrEmpty(C)) C = "ALL";
            if (string.IsNullOrEmpty(B)) B = "ALL";
            if (string.IsNullOrEmpty(AreaName)) AreaName = "ALL";


            DateTime PeriodStart = Convert.ToDateTime(DatePeriodStart);
            DateTime PeriodEnd = Convert.ToDateTime(DatePeriodEnd);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "[uspfn_SvDatabaseSubmissionBasedOnUnitEntry]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Area", AreaName);
            cmd.Parameters.AddWithValue("CompanyCode", Dealer);
            cmd.Parameters.AddWithValue("BranchCode", Outlet);
            cmd.Parameters.AddWithValue("ProductType", ProductType);
            cmd.Parameters.AddWithValue("PeriodStart", DatePeriodStart.Replace('-', '/'));
            cmd.Parameters.AddWithValue("PeriodEnd", DatePeriodEnd.Replace('-', '/'));
            cmd.Parameters.AddWithValue("FilterBy", FilterBy);
            cmd.Parameters.AddWithValue("JobGroup", JobGroup);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            var results1 = ds.Tables[0];

            var mainReport = new Report(results1.ToReportSource(), new ExcelReportWriter());
            mainReport.RenderHints["SheetName"] = "Database Submission";

            mainReport.TextFields.Title = "Database Submission Based On Unit Intake";
            mainReport.TextFields.SubTitle = "Detail Report";
            mainReport.TextFields.Header = string.Format(@"{0}
    Period          : {1}
    Filter By       : {2} / {6}
    Area              : {3}
    Dealer          : {4}
    Outlet          : {5}", "",    
    DatePeriodStart + @" to " + DatePeriodEnd,
    FilterName, AreaName, C, B, JobName);

            mainReport.RenderHints.FreezeRows = 10;
            mainReport.RenderHints.FreezeColumns = 5;

            mainReport.TextFields.Footer = @"

    Report Generated On " + DateTime.Now.ToString();

            return new ReportResult(mainReport);

        }


        [HttpPost, FileDownload]
        public ReportResult CustomerDealerInfo()
        {
            DataTable dt = new DataTable();
            string Area = Request["Area"];
            string Dealer = Request["Dealer"];
            string Outlet = Request["Outlet"];

            string DatePeriodStart = Request["DateFrom"];
            string DatePeriodEnd = Request["DateTo"];


            string AreaName = Request["AreaName"];
            string C = Request["CompanyName"];
            string B = Request["BranchName"];


            if (string.IsNullOrEmpty(C)) C = "ALL";
            if (string.IsNullOrEmpty(B)) B = "ALL";
            if (string.IsNullOrEmpty(AreaName)) AreaName = "ALL";


            DateTime PeriodStart = Convert.ToDateTime(DatePeriodStart);
            DateTime PeriodEnd = Convert.ToDateTime(DatePeriodEnd);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            if (Dealer != "")
            {
                //cmd.CommandTimeout = 3600;
                //cmd.CommandText = "select GroupNo,Dealercode,DealerName from gnmstDealerMappingNew where Dealername='" + Dealer + "'";
                //cmd.CommandType = CommandType.Text;
                //cmd.Parameters.Clear();

                //SqlDataAdapter sda = new SqlDataAdapter(cmd);
                //DataSet ds1 = new DataSet();
                //sda.Fill(ds1);
                string[] code = Dealer.Split('|');
                Dealer = code[1].ToString();
            }
            
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "[usprpt_QueryCustomerDealer3]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("CheckDate", "1");
            cmd.Parameters.AddWithValue("StartDate", DatePeriodStart.Replace('-', '/'));
            cmd.Parameters.AddWithValue("EndDate", DatePeriodEnd.Replace('-', '/'));
            cmd.Parameters.AddWithValue("Area", AreaName);
            cmd.Parameters.AddWithValue("Dealer", Dealer);
            cmd.Parameters.AddWithValue("Outlet", Outlet);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            var results1 = ds.Tables[0];

            var mainReport = new Report(results1.ToReportSource(), new ExcelReportWriter());
            mainReport.RenderHints["SheetName"] = "Customer Dealer Info";

            mainReport.TextFields.Header = string.Format(@"{0}
    Period          : {1}
    Area              : {2}
    Dealer          : {3}
    Outlet          : {4}", "",
    DatePeriodStart + @" to " + DatePeriodEnd,
    AreaName, C, B);

            //mainReport.RenderHints.FreezeRows = 10;
            //mainReport.RenderHints.FreezeColumns = 5;

//            mainReport.TextFields.Footer = @"
//    Report Generated On " + DateTime.Now.ToString();

            return new ReportResult(mainReport);

        }

        [HttpPost, FileDownload]
        public ReportResult CustomerDealerInfo2()
        {
            DataTable dt = new DataTable();
            string Area = Request["Area"];
            string Dealer = Request["Dealer"];
            string Outlet = Request["Outlet"];

            string DatePeriodStart = Request["DateFrom"];
            string DatePeriodEnd = Request["DateTo"];


            string AreaName = Request["AreaName"];
            string C = Request["CompanyName"];
            string B = Request["BranchName"];


            if (string.IsNullOrEmpty(C)) C = "ALL";
            if (string.IsNullOrEmpty(B)) B = "ALL";
            if (string.IsNullOrEmpty(AreaName)) AreaName = "ALL";


            DateTime PeriodStart = Convert.ToDateTime(DatePeriodStart);
            DateTime PeriodEnd = Convert.ToDateTime(DatePeriodEnd);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "[usprpt_QueryCustomerDealer2]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("CheckDate", "1");
            cmd.Parameters.AddWithValue("StartDate", DatePeriodStart.Replace('-', '/'));
            cmd.Parameters.AddWithValue("EndDate", DatePeriodEnd.Replace('-', '/'));
            cmd.Parameters.AddWithValue("Area", AreaName);
            cmd.Parameters.AddWithValue("Dealer", Dealer);
            cmd.Parameters.AddWithValue("Outlet", Outlet);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            var results1 = ds.Tables[0];

            var mainReport = new Report(results1.ToReportSource(), new ExcelReportWriter());
            mainReport.RenderHints["SheetName"] = "Customer Dealer Info";

            mainReport.TextFields.Header = string.Format(@"{0}
    Period          : {1}
    Area              : {2}
    Dealer          : {3}
    Outlet          : {4}", "",
    DatePeriodStart + @" to " + DatePeriodEnd,
    AreaName, C, B);

            //mainReport.RenderHints.FreezeRows = 10;
            //mainReport.RenderHints.FreezeColumns = 5;

            //            mainReport.TextFields.Footer = @"
            //    Report Generated On " + DateTime.Now.ToString();

            return new ReportResult(mainReport);

        }

        [HttpPost, FileDownload]
        public ReportResult DealerInfo()
        {
            DataTable dt = new DataTable();

            string ptype = Request["ProductType"] ?? "";
            string dName = Request["Dealer"] ?? "";

            var rawData = ctx.TransactionDateInfo.AsQueryable();

            if (!string.IsNullOrEmpty(ptype))
            {
                rawData = rawData.Where(x => x.ProductType == ptype);
            }

            if (!string.IsNullOrEmpty(dName))
            {
                rawData = rawData.Where(x => x.DealerCode == dName);
            }

            rawData = rawData.OrderBy(x => x.DealerCode).ThenBy(x => x.DealerAbbr).ThenBy(x => x.BranchCode);

            var results1 = rawData.ToList();



            var mainReport = new Report(results1.ToReportSource(), new ExcelReportWriter());
            mainReport.RenderHints["SheetName"] = "List Of Dealer Info";
            //mainReport.RenderingRow += reportDealerInfo_RenderingRow;

            //mainReport.DataFields["GoLiveDate"].DataFormatString = @"{0:dd/MMM/yyyy}";
            //mainReport.DataFields["LastSalesDate"].DataFormatString = "{0:yyyy-MMM-dd}";
            //mainReport.DataFields["LastSpareDate"].DataFormatString = "{0:dd-MMM-yyyy}";
            //mainReport.DataFields["LastServiceDate"].DataFormatString = "{0:dd-MMM-yyyy}";
            //mainReport.DataFields["LastAPDate"].DataFormatString = "{0:dd-MMM-yyyy}";
            //mainReport.DataFields["LastARDate"].DataFormatString = "{0:dd-MMM-yyyy}";
            //mainReport.DataFields["LastGLDate"].DataFormatString = "{0:dd-MMM-yyyy}";
            //mainReport.DataFields["LastupdateDate"].DataFormatString = "{0:dd-MMM-yyyy}";

            mainReport.DataFields["LastSalesDate"].FormatAs<string>(value =>  value);


            return new ReportResult(mainReport);

        }

        [HttpPost, FileDownload]
        public ReportResult GenerateITSToExcel()
        {
            DataTable dt = new DataTable();
            string Year = Request["Year"];
            string Month = Request["Month"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_GenerateITStoExecl";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Year", Convert.ToInt32(Year));
            cmd.Parameters.AddWithValue("@Month", Convert.ToInt32(Month));
            cmd.Parameters.AddWithValue("@Type", 0);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            var results1 = ds.Tables[0];

            var mainReport = new Report(results1.ToReportSource(), new ExcelReportWriter());
            mainReport.RenderHints["SheetName"] = "pmHstGenerateITSwithStatusAndTD";

            //var report2 = new Report(results2.ToReportSource(), new ExcelReportWriter());
            //report2.RenderHints["SheetName"] = "pmHstGenerateITS_ByPeriode";

            //var report3 = new Report(results3.ToReportSource(), new ExcelReportWriter());
            //report3.RenderHints["SheetName"] = "pmHstGenerateITS_ByFollowUp";

            //var writer = new ExcelReportWriter();
            //writer.AppendReport(mainReport, report2);
            //writer.AppendReport(mainReport, report3);

            return new ReportResult(mainReport);

        }

        [HttpPost, FileDownload]
        public ReportResult Consolidation()
        {
            DataTable dt = new DataTable();
            string area = Request["GroupArea"] ?? "--";
            string comp = Request["CompanyCode"] ?? "--";

            string AreaName = Request["AreaName"];
            string C = Request["CompanyName"];

            if (string.IsNullOrEmpty(C)) C = "ALL DEALER";
            if (string.IsNullOrEmpty(AreaName)) AreaName = "ALL AREA";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_SummaryOfValidEmployee_New";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@AreaCode", area);
            cmd.Parameters.AddWithValue("@Company", comp);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            var results1 = ds.Tables[0];

            var mainReport = new Report(results1.ToReportSource(), new ExcelReportWriter());
            mainReport.RenderHints["SheetName"] = "SummaryOfValidEmployee";

            mainReport.TextFields.Header = string.Format(@"
    Area              : {0}
    Dealer          : {1}" ,
    AreaName, C);
            
            return new ReportResult(mainReport);

        }

        void reportDealerInfo_RenderingRow(object sender, ReportRowEventArgs e)
        {

            int nFields = e.Row.Fields.Count();


            switch (e.Row.RowType)
            {
                case ReportRowType.HeaderRow:
                    for (int i = 8; i < nFields; i++)
                    {
                        e.Row.Fields[i].HeaderStyle.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                    break;
                case ReportRowType.DataRow:
                    {
                        for (int i = 8; i < nFields; i++)
                        {
                            var value = (string)e.Row[e.Row.Fields[i]];
                            e.Row.Fields[i].DataStyle.HorizontalAlignment = HorizontalAlignment.Right;
                            //e.Row.Fields[i].d = "{0:dd-MM-yyyy}";
                            if (value == "1900-01-01")
                            {
                                e.Row.Fields[i].DataStyle.BackColor = Color.LightGray;
                            }
                        }
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

            if (filterContext.Result is  ReportResult )
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
