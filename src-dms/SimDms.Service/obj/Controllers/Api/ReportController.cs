using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.IO;
using SimDms.Service.Models;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace SimDms.Service.Controllers.Api
{

    public class ReportController : BaseController
    {

        DateTimeFormatInfo mfi = new DateTimeFormatInfo();

        [HttpPost]
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                BranchTo = BranchCode,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                UserId = CurrentUser.UserId,
                IsHolding = !IsBranch
            });
        }

        [HttpPost]
        public JsonResult FSCCamp()
        {
            var LookUpstartDate = ctx.LookUpDtls.Find(CompanyCode, "CAMP_DATE", "START_DATE");
            var LookUpendDate = ctx.LookUpDtls.Find(CompanyCode, "CAMP_DATE", "END_DATE");

            var startDate = LookUpstartDate != null ? Convert.ToDateTime(LookUpstartDate.ParaValue) : DateTime.Now;
            var endDate = LookUpendDate != null ? Convert.ToDateTime(LookUpendDate.ParaValue) : DateTime.Now.AddDays(1);
            
            return Json(new
            {
                FirstPeriod = startDate,
                EndPeriod = endDate,
                Enable = (LookUpstartDate != null && LookUpendDate != null) ? true : false
            });
        }


        [HttpPost]
        public JsonResult ClaimDetailListDef()
        {
            bool ChkBranch=false;
            bool isChkBranch = false;
            bool chkClaim = true;

            var lookupDtl = ctx.LookUpDtls.Find(CurrentUser.CompanyCode, "SRV_FLAG", "CLM_HOLDING");
            var OrgDtl = ctx.OrganizationDtls.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode);
            if(!OrgDtl.IsBranch)
            {
               ChkBranch = false;
               isChkBranch = true;
            }
            else
            {
                if(lookupDtl.ParaValue =="1")
                {
                    ChkBranch = false;
                    isChkBranch = true;
                    chkClaim = false;
                }
                else{
                    ChkBranch = false;
                    isChkBranch = true;
                }
            }
            return Json(new {CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                BranchTo = BranchCode,
                BranchFrom = BranchCode,
                ChkBranch = ChkBranch,
                isChkBranch = isChkBranch,
                chkClaim = chkClaim
                });
        }
        //
        // GET: /Report/
        [ValidateInput(false)]
        public ActionResult SvRpReport014(string SPKDateFrom, string SPKDateTo, string Outstanding, string GroupJobType)
        {
            var user = CurrentUser;
            var CoProfile = ctx.CoProfiles.Find(user.CompanyCode, user.BranchCode);
            string reportUrl = ConfigurationSettings.AppSettings["reportUrl"].ToString();
            SPKDateFrom = Convert.ToDateTime(SPKDateFrom).ToString("yyyyddMM");
            SPKDateTo = Convert.ToDateTime(SPKDateTo).ToString("yyyyddMM");
            reportUrl = reportUrl + "?id=SvRpReport014&pparam='{0}', '{1}', '{2}', '{3}', '{4}', {5}, '{6}', {7}&rparam={8}";
            reportUrl = string.Format(reportUrl, user.CompanyCode, user.BranchCode, CoProfile.ProductType, SPKDateFrom, SPKDateTo, Outstanding, GroupJobType, 0 , user.FullName);
            return Redirect(reportUrl);
        }

        public JsonResult GnRpMst004()
        {
            DataTable dt = new DataTable();
            var year = Request["Year"];
            var status = Request["Status"];

            var strReport = @"select a.*,b.IsLanscape from SysReport a 
                                          left join SysReportDevice b on a.ReportDeviceID = b.ReportDeviceID
                                          where a.ReportID='GnRpMst004'";

            var report = ctx.Database.SqlQuery<Report>(strReport).FirstOrDefault();

            var reportInfo = report.ReportInfo;

            try
            {

                //string reportpath = report.ReportPath ?? "";
                //string[] classes = reportpath.Split(',');
                //Assembly ass = Assembly.Load(classes[0]);
                //Type type = null;
                //if (classes[1].StartsWith("Isi."))
                //{
                //    type = ass.GetType(classes[1]);
                //}
                //else
                //{
                //    type = ass.GetType(reportpath.Replace(',', '.'));
                //}
                //report = (XtraReport)Activator.CreateInstance(type);
                //if (DeviceFormat != null && DeviceFormat.ForceFormat)
                //{
                //    if (DeviceFormat.ForcePaperKind)
                //    {
                //        SetPaperKind(report, DeviceFormat.PaperKind);
                //        bool isLanscape = (row["IsLanscape"] is DBNull) ? false : Convert.ToBoolean(row["IsLanscape"]);
                //        report.Landscape = isLanscape;
                //    }

                //    report.Margins.Top = DeviceFormat.MarginTop;
                //    report.Margins.Left = DeviceFormat.MarginLeft;
                //    report.Margins.Right = DeviceFormat.MarginRight;
                //    report.Margins.Bottom = DeviceFormat.MarginBottom;
                //}
            }
            catch (Exception ex)
            {
              
            }

            var strReportDevice = @"select a.* from SysReportDevice a 
                                                inner join SysReport b on a.ReportDeviceID=b.ReportDeviceID
                                                where b.ReportID='GnRpMst004'";

            var reportDevice = ctx.Database.SqlQuery<ReportDevice>(strReportDevice);

            object[] parameters = { CompanyCode, BranchCode, year, status };
            var strRpt = "exec usprpt_GnRpMst004 @p0,@p1,@p2,@p3";

            var rptMst004 = ctx.Database.SqlQuery<GnRpMst004>(strRpt, parameters);

            var custAct = rptMst004.Where(a => a.Keterangan.Equals("Aktif")).Count();
            var custPsv = rptMst004.Where(a => a.Keterangan.Equals("Pasif")).Count();

            //var path = Path.GetTempFileName().Replace(".tmp", ".xls");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "usprpt_GnRpMst004";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode); 
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Status", status);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var header = new List<List<dynamic>>(){};
            header.Add(new List<dynamic> { reportInfo });
            header.Add(new List<dynamic> { "Periode Tahun :", year});
            header.Add(new List<dynamic> { "Total Customer Aktif :", custAct });
            header.Add(new List<dynamic> { "Total Customer Pasif :", custPsv });

            return GenerateReportXls(dt, "GnRpMst004", "GnRpMst004", header);
        }

        public JsonResult PerformaService()
        {
            DataTable dt = new DataTable();
            var month = Request["month"];
            var year = Request["year"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "usprpt_SvRpReport009";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return GenerateReportXls(dt, "usprpt_SvRpReport009", "usprpt_SvRpReport009");
        }

        public JsonResult ReviewServiceAdvisor()
        {
            DataTable dt = new DataTable();
            var month = Request["month"];
            var year = Request["year"];
            var options = Request["options"];
            var employeeId = Request["employeeId"];

            var ReportId="";
            var parMonth = "@Month";

            switch(options){
                case "0": 
                    ReportId = "usprpt_SvRpReport020";
                    break;
                case "1":
                    ReportId ="usprpt_SvRpReport019";
                    break;
                case "2" :
                    ReportId ="usprpt_SvRpReport018";
                    parMonth = "@Periode";
                    break;
                case "3" :
                    ReportId ="usprpt_SvRpReport019A";
                    break;
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = ReportId;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
            if (options != "2") cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue(parMonth, month);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { CompanyName });
            header.Add(new List<dynamic> { "Monthly Sales Review Service Advisor" });
            header.Add(new List<dynamic> { (options != "2") ? "BULAN : " + GetString(int.Parse(month), "MMM") + " - " +year : "TAHUN : " + year  });
            header.Add(new List<dynamic> { "Target Penjualan By Tanggal (Rp)" });

            return GenerateReportXls(dt, ReportId, ReportId, header);
        }

        public JsonResult ActivityService()
        {
            DataTable dt = new DataTable();
            var branchFrom = Request["branchFrom"] ?? "";
            var branchTo = Request["branchTo"] ?? "";
            var month = Request["month"];
            var year = Request["year"];
            var options = Request["options"];

            var ReportId="usprpt_SvRpReport030";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = ReportId;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCodeFrom", branchFrom);
            cmd.Parameters.AddWithValue("@BranchCodeTo", branchTo);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@MonthPeriod", month);
            cmd.Parameters.AddWithValue("@YearPeriod", year);
            cmd.Parameters.AddWithValue("@ParamReport", options);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var cabang = !IsBranch ? 
                (branchFrom == "" && branchTo =="" ? "CABANG : SEMUA CABANG" 
                : "CABANG : " + GetCompanyName(branchFrom) + " sd " + GetCompanyName(branchTo))
                : "CABANG : " + GetCompanyName(branchFrom);
                //? "CABANG : " + ctx.CoProfiles

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { CompanyName });
            header.Add(new List<dynamic> { CurrentUser.CoProfile.Address1 });
            header.Add(new List<dynamic> { CurrentUser.CoProfile.Address2 });
            header.Add(new List<dynamic> { });
            header.Add(new List<dynamic> { "Report Aktifitas Service By Mekanik & Unit" });
            header.Add(new List<dynamic> { "BULAN : " + GetString(int.Parse(month), "MMM") + " - " + year });
            header.Add(new List<dynamic> { });
            header.Add(new List<dynamic> { cabang});

            return GenerateReportXls(dt, ReportId, ReportId,header);
        }

        public static string GetString(int month, string format)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            //return new DateTime(1900, month, 1).ToString(format, CultureInfo.GetCultureInfo("id-ID"));
        }

        private string GetCompanyName(string branchCode)
        {
            string name = "";
            if (branchCode == "") { name = ""; }
            else { name = ctx.CoProfiles.Find(CompanyCode, branchCode).CompanyName; }
            return name;
        }
    }
}
