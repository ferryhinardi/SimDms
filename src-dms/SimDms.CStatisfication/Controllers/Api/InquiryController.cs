using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.CStatisfication.Models;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class InquiryController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
            });
        }

        public JsonResult TDayCall()
        {
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];
            string branchCode = Request["BranchCode"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsInqTDayCall";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult CsTDaysCall()
        {
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsInqTDaysCall";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            cmd.Parameters.AddWithValue("@Outstanding", "N");
            cmd.Parameters.AddWithValue("@Status", "Inquiry");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult StnkExt()
        {
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];
            string branchCode = Request["BranchCode"] ?? "";
            string isStnkExtension = Request["IsStnkExtension"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsInqStnkExt";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@StnkExt", isStnkExtension);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult CsStnkExtension()
        {
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];
            string branchCode = BranchCode;
            string isStnkExtension = Request["IsStnkExtension"] ?? "";
            string outstanding = Request["Outstanding"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsStnkExtension";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@IsStnkExtension", isStnkExtension);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            cmd.Parameters.AddWithValue("@Outstanding", outstanding);
            cmd.Parameters.AddWithValue("@Status", "Inquiry");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult CustBirthday()
        {
            var year = Request["Year"] ?? "1900";
            var monthFrom = Request["MonthFrom"] ?? DateTime.Now.Month.ToString();
            var monthTo = Request["MonthTo"] ?? DateTime.Now.Month.ToString();
            var status = Request["Status"] ?? "0";
            var branchCode = Request["BranchCode"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsInqCustBirthday";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            cmd.Parameters.AddWithValue("@MonthFrom", monthFrom);
            cmd.Parameters.AddWithValue("@MonthTo", monthTo);
            cmd.Parameters.AddWithValue("@ParStatus", status);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(GetJson(dt));
        }

        public JsonResult CsCustomerBirthday()
        {
            var year = Request["Year"] ?? "1900";
            var monthFrom = Request["MonthFrom"] ?? DateTime.Now.Month.ToString();
            var monthTo = Request["MonthTo"] ?? DateTime.Now.Month.ToString();
            var status = Request["Status"] ?? "0";
            var branchCode = Request["BranchCode"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsInqCustomerBirthday";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@MonthFrom", monthFrom);
            cmd.Parameters.AddWithValue("@MonthTo", monthTo);
            cmd.Parameters.AddWithValue("@Outstanding", status);
            cmd.Parameters.AddWithValue("@Status", "Inquiry");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(GetJson(dt));
        }

        public JsonResult BpkbReminder()
        {
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];
            string branchCode = Request["BranchCode"] ?? "";
                                                            
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsInqBpkbReminder";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult CsBpkbReminder()
        {
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];
            string branchCode = BranchCode;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsInqBpkbReminder";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            cmd.Parameters.AddWithValue("@Outstanding", "N");
            cmd.Parameters.AddWithValue("@Status", "Inquiry");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult OutstandingDelivery()
        {
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];
            string branchCode = Request["BranchCode"] ?? "";
            string status = Request["Status"] ?? "";

            int outputStatus;
            if (!Int32.TryParse(status, out outputStatus))
                outputStatus = -1;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsOutstandingDlvryReport";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            if (outputStatus != -1)
                cmd.Parameters.AddWithValue("@Status", outputStatus);
            else
                cmd.Parameters.AddWithValue("@Status", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult CustHoliday()
        {
            string pReligion = Request["Religion"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsInqCustHoliday";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@Religion", pReligion);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult Feedback()
        {
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsInqCustFeedback";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        //Customer Data, Customer Data With transaction, cutomer suzuki
        public JsonResult CustomerStatus()
        {
            DataTable dt = new DataTable();
            if (Request["InqType"].ToString().ToLower().Equals("d"))
            {
                string CompanyCode = (string.IsNullOrWhiteSpace(Request["CompanyCode"]) ? "" : Request["CompanyCode"].ToString());
                string BranchCode = (string.IsNullOrWhiteSpace(Request["BranchCode"]) ? "" : Request["BranchCode"].ToString());
                string DateFrom = (string.IsNullOrWhiteSpace(Request["DateFrom"]) ? "" : Request["DateFrom"].ToString());
                string DateTo = (String.IsNullOrWhiteSpace(Request["DateTo"]) ? "" : Request["DateTo"].ToString());

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "uspfn_CsCustSzkLastTrans";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                cmd.Parameters.AddWithValue("@DateTo", DateTo);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            else
            {
                string BranchCode = (string.IsNullOrWhiteSpace(Request["BranchCode"]) ? "" : Request["BranchCode"].ToString());
                string InqType = (string.IsNullOrWhiteSpace(Request["InqType"]) ? "" : Request["InqType"].ToString());
                string Month = (string.IsNullOrWhiteSpace(Request["Month"]) ? "" : Request["Month"].ToString());
                string YearTo = (string.IsNullOrWhiteSpace(Request["YearTo"]) ? "" : Request["YearTo"].ToString());
                string YearFrom = (string.IsNullOrWhiteSpace(Request["YearFrom"]) ? "" : Request["YearFrom"].ToString());

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "uspfn_CsCustomerDataInq";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SelectCode", InqType);
                cmd.Parameters.AddWithValue("@Month", Month);
                cmd.Parameters.AddWithValue("@YearTo", YearTo);
                cmd.Parameters.AddWithValue("@YearFrom", YearFrom);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return Json(GetJson(dt));
        }

        public JsonResult CustomerStatusDetail()
        {
            string CompanyCode = (string.IsNullOrWhiteSpace(Request["CompanyCode"]) ? "" : Request["CompanyCode"].ToString());
            string BranchCode = (string.IsNullOrWhiteSpace(Request["BranchCode"]) ? "" : Request["BranchCode"].ToString());
            string CustType = (string.IsNullOrWhiteSpace(Request["CustType"]) ? "" : Request["CustType"].ToString());
            string TransDateStart = (string.IsNullOrWhiteSpace(Request["TransDateStart"]) ? "" : Request["TransDateStart"].ToString());
            string TransDateEnd = (string.IsNullOrWhiteSpace(Request["TransDateEnd"]) ? "" : Request["TransDateEnd"].ToString());

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CustSuzukiDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@CustType", CustType);
            cmd.Parameters.AddWithValue("@TransDateStart", TransDateStart);
            cmd.Parameters.AddWithValue("@TransDateEnd", TransDateEnd);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }
    }
}
