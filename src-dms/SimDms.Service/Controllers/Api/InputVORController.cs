using SimDms.Common;
using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Drawing;

//using System.Drawing;

using DoddleReport;
using DoddleReport.Web;
using GeLang;

using DoddleReport.OpenXml;
using TracerX;
//using Newtonsoft.Json.Linq;

namespace SimDms.Service.Controllers.Api
{
    public class InputVORController : BaseController
    {
        public JsonResult GetGridVOR(long ServiceNo)
        {
            var gridMechanic = from c in ctx.Mechanics
                               join d in ctx.Employees
                               on new { c.CompanyCode, c.BranchCode, c.MechanicID }
                               equals new { d.CompanyCode, d.BranchCode, MechanicID = d.EmployeeID } into _d
                               from d in _d.DefaultIfEmpty()
                               where c.CompanyCode == CompanyCode
                               && c.BranchCode == BranchCode
                               && c.ProductType == ProductType
                               && c.ServiceNo == ServiceNo
                               select new GridMechanic
                               {
                                   MechanicID = c.MechanicID,
                                   MechanicName = d.EmployeeName
                               };

            int seqNo = 0;
            var gridPart = from a in ctx.VORDtls
                           join d in ctx.ItemInfos
                           on new { a.CompanyCode, a.PartNo }
                           equals new { d.CompanyCode, d.PartNo }
                           where a.CompanyCode == CompanyCode
                           && a.BranchCode == BranchCode
                           && a.ServiceNo == ServiceNo
                           select new VORPart
                           {
                               SeqNo = seqNo + 1,
                               POSNo = a.POSNo,
                               PartNo = a.PartNo,
                               PartName = d.PartName,
                               PartQty = a.PartQty
                           };


            return Json(new { success = true, gridMechanic = gridMechanic, gridPart = gridPart});
        }

        public JsonResult NoVOR(int novor) 
        {
            if (novor == 1)
            {
                ctx.NoVORs.Add(new svTrnSrvNoVOR()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    CreatedDate = DateTime.Now.Date,
                    CreatedBy = CurrentUser.UserId
                });
            }
            else
            {
                ctx.NoVORs.Remove(ctx.NoVORs.Find(CompanyCode, BranchCode, DateTime.Now.Date));
            }
            var result = false;
            var msg = "";
            result = ctx.SaveChanges() > 0;

            if (result) { msg = "berhasil"; }
            else { msg = string.Format(Message(SysMessages.MSG_5039), "No VOR", ""); }

            return Json(new { success = result, message = msg });
        }

        public JsonResult Save(VORBrowse model)
        {
            var result = false;
            var msg = "";
            var record = ctx.VORs.Find(CompanyCode, BranchCode, model.ServiceNo);
            if(record == null)
            {
                record = new VOR()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ServiceNo = model.ServiceNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };
                ctx.VORs.Add(record);
            }

            record.IsActive = model.IsActive;
            record.JobOrderNo = model.JobOrderNo;
            record.JobDelayCode = model.JobDelayCode;
            record.JobReasonDesc = model.JobReasonDesc.ToUpper();
            record.ClosedDate = model.ClosedDate;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.IsSparepart = model.IsSparepart;

            result = ctx.SaveChanges() > 0;

            if (result) { msg = "Data VOR berhasil disimpan"; }
            else { msg = string.Format(Message(SysMessages.MSG_5039), "Input VOR", ""); }

            return Json(new { success = result, record = record, message = msg });
        }

        public JsonResult Delete(VORBrowse model)
        {
            var result = false;
            var msg = "";
            var Dtl = ctx.VORDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ServiceNo == model.ServiceNo);
            if(Dtl.Count() > 0)
            {
                msg = "Data tidak dapat dihapus karena masih ada data detail";
            }
            else
            {
                var Hdr = ctx.VORs.Find(CompanyCode, BranchCode, model.ServiceNo);
                ctx.VORs.Remove(Hdr);
                result = ctx.SaveChanges() > 0;
                msg = "Data VOOR berhasil dihapus";
            }

            return Json(new { success = result, message = msg });
        }

        public JsonResult SavePart(VORBrowse model, VORPart partModel)
        {
            var result = false;
            var msg = "";
            var record = ctx.VORDtls.Find(CompanyCode, BranchCode, model.ServiceNo, partModel.POSNo, partModel.PartNo);

            if (record == null)
            {
                record = new VORDtl()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ServiceNo = model.ServiceNo,
                    POSNo = partModel.POSNo,
                    PartNo = partModel.PartNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };
                ctx.VORDtls.Add(record);
            }
            record.PartQty = partModel.PartQty;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            result = ctx.SaveChanges() > 0;
            if (result) { msg = "Data VOR Detail berhasil disimpan."; }
            else { msg = string.Format(Message(SysMessages.MSG_5039), "Input VOR Detail", ""); }
            
            return Json(new { success = result, record = record, message = msg });
        }

        public JsonResult DeletePart(VORBrowse model, VORPart partModel)
        {
            var result = false;
            var msg = "";
             var record = ctx.VORDtls.Find(CompanyCode, BranchCode, model.ServiceNo, partModel.POSNo, partModel.PartNo);

             if (record != null) ctx.VORDtls.Remove(record);

             result = ctx.SaveChanges() > 0;

             if (result) { msg = "Data Vor Detail berhasil dihapus."; }
             else { msg = string.Format(Message(SysMessages.MSG_5039), "Delete VOR Detail", ""); }
             
            return Json(new { success = result, record = record, message = msg });
        }

        public JsonResult CountWIP()
        {
            DataSet dt = new DataSet();
            DateTime startDate;
            DateTime endDate;
            DateTime.TryParse(Request["startDate"], out startDate);
            DateTime.TryParse(Request["endDate"], out endDate);

            string qry = @"
            select CASE 
		             WHEN cast(ClosedDate as date) = '1900-01-01' 
		             THEN DATEDIFF(day, CreatedDate, GETDATE()) 
		             ELSE 0
    	           END CountWIP
            INTO #temp
            FROM svTrnSrvVOR
           WHERE CompanyCode = @CompanyCode
	         and BranchCode = @BranchCode
	         and convert(varchar,CreatedDate,112) between convert(varchar, @StartDate,112) and convert(varchar,@EndDate,112)
	         and IsActive = 1
           
          select Count(CountWIP) FROM #temp WHERE CountWIP >= 3
            drop table #temp";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = qry;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var records = dt.Tables[0];
            var rows = (int)records.Rows[0].ItemArray[0];

            return Json(new { success = (rows > 0) });
        }

        public JsonResult Print()
        {
            DataTable dt = new DataTable();
            var startDate = Request["startDate"]; 
            var endDate = Request["endDate"];

            var ReportId = "usprpt_SvRpTrn021";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = ReportId;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "VEHICLE OFF THE ROAD PROJECT (VOR REPORT)" });
            header.Add(new List<dynamic> { "LAPORAN PEKERJAAN TUNDA" });
            header.Add(new List<dynamic> { });
            header.Add(new List<dynamic> { "Tanggal Laporan : " + DateTime.Now.ToString("dd-MMM-yyyy") });
            header.Add(new List<dynamic> { "Nama Dealer : " + BranchName });

            return GenerateReportXls(dt, ReportId, ReportId,header);
        }

        [HttpPost, FileDownload]
        public ReportResult Print1()
        {
            DataSet dt = new DataSet();
            var startDate = Request["startDate"];
            var endDate = Request["endDate"];

            var ReportId = "usprpt_SvRpTrn021";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = ReportId;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            MyLogger.Log.Info("Inquiry Customers: EXEC " + cmd.CommandText);

            var records = dt.Tables[0];
            var rows = records.Rows.Count;

            var mainReport = new DoddleReport.Report(records.ToReportSource(), new ExcelReportWriter());
            mainReport.RenderHints["SheetName"] = "svRpTrn021";

            mainReport.DataFields["ReportDate"].Hidden = true;
            mainReport.DataFields["CountMechAbs"].Hidden = true;
            mainReport.DataFields["CountMechAlpha"].Hidden = true;
            mainReport.DataFields["CountMechOvrTime"].Hidden = true;
            mainReport.DataFields["CountMechDelay"].Hidden = true;
            mainReport.DataFields["CountMechNextDelay"].Hidden = true;

            mainReport.TextFields.Header = string.Format(@"
                VEHICLE OFF THE ROAD PROJECT (VOR REPORT)
                LAPORAN PEKERJAAN TUNDA
                
                Tanggal Laporan : {0}		
                Nama Dealer : {1}		
                ",
                    DateTime.Now.ToString("dd-MMM-yyyy"), BranchName);
            
            mainReport.RenderingRow += mainReport_RenderingRow;

            return new ReportResult(mainReport) { FileName = "svRpTrn021.xlsx" };
        }

        void mainReport_RenderingRow(object sender, ReportRowEventArgs e)
        {
            int n = e.Row.Fields.Count();

            switch (e.Row.RowType)
            {
                case ReportRowType.HeaderRow:
                    for (int i = 0; i <= n; i++)
                    {
                        e.Row.Fields[i].HeaderStyle.BackColor = Color.DarkBlue;
                        e.Row.Fields[i].HeaderStyle.ForeColor = Color.White;
                        e.Row.Fields[i].HeaderStyle.FontSize = 10;
                        e.Row.Fields[i].HeaderStyle.Underline = false;
                    }
                    break;
                case ReportRowType.DataRow:
                    {
                        string closed = e.Row[e.Row.Fields[22]].ToString();
                        DateTime opened;
                        DateTime.TryParse(e.Row[e.Row.Fields[13]].ToString(), out opened);

                        if (closed == "1/1/1900 12:00:00 AM" && (DateTime.Now.Date - opened.Date).Days >= 3)
                        {
                            for (int i = 0; i <= n; i++)
                            {
                                e.Row.Fields[i].DataStyle.BackColor = Color.Red;
                                //var value = (int)e.Row[e.Row.Fields[i]];
                                //e.Row.Fields[i].DataStyle.HorizontalAlignment HorizontalAlignment.Right;
                                //e.Row.Fields[i].DataFormatString = "#,#";
                                //if (value == 0)
                                //{
                                //    e.Row.Fields[i].DataStyle.BackColor = Color.LightGray;
                                //}
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