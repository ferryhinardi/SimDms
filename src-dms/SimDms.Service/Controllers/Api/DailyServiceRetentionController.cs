using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Service.Models;
using System.Transactions;

namespace SimDms.Service.Controllers.Api
{
    public class DailyServiceRetentionController : BaseController
    {
        public JsonResult isCallInformation(string RefferenceType)
        {
            var trans = ctx.svMstRefferenceServices
                .Where(x => (x.CompanyCode == CompanyCode && x.ProductType == "4W" && x.RefferenceType == RefferenceType))
                .OrderBy(x => x.RefferenceCode)
                .Select(x => new { value = x.RefferenceCode, text = x.Description }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Kedatangan(string CodeID)
        {
            var trans = ctx.LookUpDtls
                .Where(x => (x.CompanyCode == CompanyCode && x.CodeID == CodeID))
                .OrderBy(x => x.SeqNo)
                .Select(x => new { value = x.ParaValue, text = x.LookUpValueName }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        public JsonResult isTidakPuasan(string RefferenceType)
        {
            var trans = ctx.svMstRefferenceServices
                .Where(x => (x.CompanyCode == CompanyCode && x.ProductType == "4W" && x.RefferenceType == RefferenceType))
                .OrderBy(x => x.RefferenceCode)
                .Select(x => new { value = x.RefferenceCode, text = x.Description }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PopulateData(DateTime date, string optionType, int interval, int range, bool InclPdi, bool IsOdom)
        {
            var query = string.Format(@"uspfn_SelectDailyRetentionWeb '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}', {8}
                ", CompanyCode, BranchCode, date, optionType, interval, range, InclPdi, CurrentUser.UserId, IsOdom);

            var queryable = ctx.Database.SqlQuery<DailyServiceRetentionPopulate>(query).AsQueryable();
            var x = queryable.ToList();

            return Json(queryable);
        }

        public JsonResult GetKendaraan(string Year, string Month, Int64 RetentionNo, string CustomerCode)
        {
            var query = string.Format(@"uspfn_GetVehicleData '{0}','{1}','{2}','{3}','{4}','{5}'
                ", CompanyCode, BranchCode, Year, Month, RetentionNo, CustomerCode);

            var queryable = ctx.Database.SqlQuery<GetVehicleData>(query).AsQueryable();
            return Json(new {success = queryable == null ? false : true,  data = queryable });
        }

        public JsonResult GetCustomer(string CustomerCode)
        {
            var query = string.Format(@"SELECT
                    a.CustomerCode
	                , a.CustomerName
	                , a.Gender 
	                , a.BirthDate
	                , a.CityCode
	                , b.LookUpValueName City
	                , a.PhoneNo
                    , a.OfficePhoneNo
	                , a.HPNo
	                , a.Address1
	                , a.Address2
	                , a.Address3
	                , a.Address4
                FROM 
	                gnMstCustomer a
                LEFT JOIN gnMstLookUpDtl b ON b.CompanyCode = a.CompanyCode
	                AND b.CodeID = 'CITY' AND b.LookUpValue = a.CityCode 
                WHERE
                    a.CompanyCode = '{0}'
                    AND a.CustomerCode = '{1}' 
                ", CompanyCode, CustomerCode);

            var queryable = ctx.Database.SqlQuery<GetCustomerData>(query).AsQueryable();
            return Json(new { success = queryable == null ? false : true, data = queryable });
        }

        public JsonResult SaveDataRDH(decimal PeriodYear, decimal PeriodMonth, long RetentionNo, string CustomerCode, GetVehicleData model)
        {
            string msg = "";
            var record = ctx.SvTrnDailyRetention.Find(CompanyCode, BranchCode, PeriodYear, PeriodMonth, RetentionNo, CustomerCode);
            Int64 seqNo = getMaxSeqNo(record.PeriodYear, record.PeriodMonth, record.RetentionNo);
            try{
                if (record != null){
                    // Set Old Value First Before Update
                    var visitDate = record.VisitDate;
                    var estimationNextVisit = record.EstimationNextVisit;

                    record.IsReminder = model.IsReminder;
                    record.IsFollowUp = model.IsFollowUp;
                    record.IsConfirmed = model.IsConfirmed;
                    record.IsBooked = model.IsBooked;
                    record.IsVisited = model.IsVisited;
                    record.IsSatisfied = model.IsSatisfied;
                    record.IsClosed = model.isClosed;
                    record.StatisfyReasonGroup = model.StatisfyReasonGroup; //(rbReasonStatify.Checked) ? "T" : "P";
                    record.StatisfyReasonCode = model.StatisfyReasonCode;//(lkuStatifyInfo.EditValue == null) ? "" : lkuStatifyInfo.EditValue.ToString();
                    record.CannotCallCode = model.CannotCallCode; //(lkuCustNotCall.EditValue == null) ? "" : lkuCustNotCall.EditValue.ToString();

                    record.ReminderDate = model.IsReminder == true ? model.ReminderDate : Convert.ToDateTime("1900-01-01");
                    record.BookingDate = model.IsBooked == true ? model.BookingDate : Convert.ToDateTime("1900-01-01");
                    record.FollowUpDate = model.IsFollowUp == true ? model.FollowUpDate : Convert.ToDateTime("1900-01-01");

                    record.VisitInitial = model.VisitInitial;
                    record.Reason = model.Reason;

                    // update svTrnDailyRetention
                    int result = ctx.SaveChanges();

                    if (result > -1)
                    {
                        SvHstDailyRetention oSvHstDailyRetention = new SvHstDailyRetention();

                        oSvHstDailyRetention.CompanyCode = CompanyCode;
                        oSvHstDailyRetention.BranchCode = BranchCode;
                        oSvHstDailyRetention.PeriodYear = record.PeriodYear;
                        oSvHstDailyRetention.PeriodMonth = record.PeriodMonth;
                        oSvHstDailyRetention.RetentionNo = record.RetentionNo;
                        oSvHstDailyRetention.SeqNo = seqNo;
                        oSvHstDailyRetention.VisitInitial = record.VisitInitial;

                        oSvHstDailyRetention.VisitDate = visitDate;
                        oSvHstDailyRetention.EstimationNextVisit = estimationNextVisit;
                        oSvHstDailyRetention.PMNow = model.PMNow;
                        oSvHstDailyRetention.PMNext = model.PMNow;
                        oSvHstDailyRetention.ReminderDate = model.IsReminder == true ? model.ReminderDate : Convert.ToDateTime("1900-01-01");

                        oSvHstDailyRetention.IsConfirmed = record.IsConfirmed;
                        oSvHstDailyRetention.IsBooked = record.IsBooked;
                        oSvHstDailyRetention.IsVisited = record.IsVisited;
                        oSvHstDailyRetention.IsSatisfied = record.IsSatisfied;
                        oSvHstDailyRetention.BookingDate = record.BookingDate;
                        oSvHstDailyRetention.FollowUpDate = record.FollowUpDate;
                        oSvHstDailyRetention.Reason = record.Reason;
                        oSvHstDailyRetention.RefferenceDate = model.RefferenceDate;
                        oSvHstDailyRetention.LastServiceDate = model.LastServiceDate;
                        oSvHstDailyRetention.LastRemark = model.LastRemark;
                        oSvHstDailyRetention.CreatedBy = CurrentUser.UserId;
                        oSvHstDailyRetention.CreatedDate = DateTime.Now;
                        oSvHstDailyRetention.LastUpdateBy = CurrentUser.UserId;
                        oSvHstDailyRetention.LastUpdateDate = DateTime.Now;

                        ctx.SvHstDailyRetentions.Add(oSvHstDailyRetention);
                        result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            msg = "Data Berhasil Di Simpan";
                        }
                        else
                        {
                            msg = "Data Gagal Di Simpan !";
                        }
                    }
                }

                return Json(new { success = true, message = msg, data = record });
            }
            catch(Exception ex){
                return Json(new { success = false, message = ex.Message });
            }
        }

        private Int64 getMaxSeqNo(decimal year, decimal month, long retNo)
        {
            Int64 seqNo = 1;
            var record = ctx.SvHstDailyRetentions.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode 
                && x.PeriodYear == year && x.PeriodMonth == month && x.RetentionNo == retNo);

            if (record != null)
            {
                seqNo = record.Count() + 1;
            }

            return seqNo;
        }
    }
}
