using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Web.Mvc;
using GeLang;
using System.Diagnostics;
using SimDms.CStatisfication.Models;
using SimDms.Common;
using System.Collections.Generic;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class LookupController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                HolidayYear = DateTime.Now.Year
            });
        }

        //public JsonResult TDayCalls()
        //{
        //    var outstanding = Request["OutStanding"];
        //    var fltCustName = Request["fltCustName"];
        //    var fltVinNo = Request["fltVinNo"];
        //    var fltPolReg = Request["fltPolReg"];

        //    var qry = ctx.CsLkuTDayCallViews.Where(p =>
        //                p.CompanyCode == CompanyCode &&
        //                p.BranchCode == BranchCode &&
        //                p.Outstanding == outstanding
        //            );

        //    if (!string.IsNullOrWhiteSpace(outstanding))
        //    {
        //        var setting = ctx.CsSettings.Find(CompanyCode, "REM3DAYSCALL");
        //        var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
        //        var date1 = DateTime.Now.AddMonths(-month);
        //        var date2 = new DateTime(date1.Year, date1.Month, 1);
        //        qry = qry.Where(p => p.DODate >= date2);
        //    }

        //    if (!string.IsNullOrWhiteSpace(fltCustName)) { qry = qry.Where(p => p.CustomerName.Contains(fltCustName)); };
        //    if (!string.IsNullOrWhiteSpace(fltVinNo)) { qry = qry.Where(p => p.Chassis.Contains(fltVinNo)); };
        //    if (!string.IsNullOrWhiteSpace(fltPolReg)) { qry = qry.Where(p => p.PoliceRegNo.Contains(fltPolReg)); };

        //    return Json(qry.KGrid());
        //}

        //public JsonResult StnkExts()
        //{
        //    var outstanding = Request["OutStanding"];
        //    var fltCustName = Request["fltCustName"];
        //    var fltVinNo = Request["fltVinNo"];
        //    var fltPolReg = Request["fltPolReg"];

        //    var qry = ctx.CsLkuStnkExtViews.Where(p =>
        //                p.CompanyCode == CompanyCode &&
        //                p.BranchCode == BranchCode &&
        //                p.Outstanding == outstanding
        //            );

        //    //if (outstanding == "N")
        //    //{
        //    //    qry = qry.Where(x => x.Ownership == true);
        //    //}


        //    if (!string.IsNullOrWhiteSpace(outstanding))
        //    {
        //        var setting = ctx.CsSettings.Find(CompanyCode, "REMSTNKEXT");
        //        var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
        //        var date1 = DateTime.Now.AddMonths(-month);
        //        var date2 = new DateTime(date1.Year, date1.Month, 1);
        //        qry = qry.Where(p => p.StnkExpiredDate >= date2);
        //    }

        //    if (!string.IsNullOrWhiteSpace(fltCustName)) { qry = qry.Where(p => p.CustomerName.Contains(fltCustName)); };
        //    if (!string.IsNullOrWhiteSpace(fltVinNo)) { qry = qry.Where(p => p.Chassis.Contains(fltVinNo)); };
        //    if (!string.IsNullOrWhiteSpace(fltPolReg)) { qry = qry.Where(p => p.PoliceRegNo.Contains(fltPolReg)); };

        //    return Json(qry.KGrid());
        //}

        public JsonResult Bpkbs()
        {
            var outstanding = Request["OutStanding"];
            var fltCustName = Request["fltCustName"];
            var fltVinNo = Request["fltVinNo"];
            var fltPolReg = Request["fltPolReg"];

            var qry = ctx.CsLkuBpkbReminderViews.Where(p =>
                        p.CompanyCode == CompanyCode &&
                        p.BranchCode == BranchCode &&
                        p.Outstanding == outstanding
                    );

            if (!string.IsNullOrWhiteSpace(outstanding))
            {
                var setting = ctx.CsSettings.Find(CompanyCode, "REMBPKB");
                var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
                var date1 = DateTime.Now.AddMonths(-month);
                var date2 = new DateTime(date1.Year, date1.Month, 1);
                qry = qry.Where(p => p.BpkbDate >= date2);
            }

            //qry = qry.Where( x => x.DelayedRetrievalDate < DateTime.Now || x.DelayedRetrievalDate == null);

            if (!string.IsNullOrWhiteSpace(fltCustName)) { qry = qry.Where(p => p.CustomerName.Contains(fltCustName)); };
            if (!string.IsNullOrWhiteSpace(fltVinNo)) { qry = qry.Where(p => p.Chassis.Contains(fltVinNo)); };
            if (!string.IsNullOrWhiteSpace(fltPolReg)) { qry = qry.Where(p => p.PoliceRegNo.Contains(fltPolReg)); };
            

            return Json(qry.KGrid());
        }

        public JsonResult PendingBpkbRetrieval()
        {
            string customerCode = Request["CustomerCode"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsBpkbRetrievalInformation";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@CustomerCode", customerCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(GetJson(dt));
        }

        public JsonResult Feedback()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsLkuFeedback";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@OutStanding", Request["OutStanding"]);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(GetJson(dt));
        }

        public JsonResult Feedbacks()
        {
            var outstanding = Request["OutStanding"];
            var fltCustName = Request["fltCustName"];
            var fltVinNo = Request["fltVinNo"];
            var fltPolReg = Request["fltPolReg"];
            /*
            var qry = ctx.CsLkuFeedbackViews.Where(p =>
                        p.CompanyCode == CompanyCode &&
                        p.BranchCode == BranchCode &&
                        p.OutStanding == outstanding
                    );

            if (!string.IsNullOrWhiteSpace(fltCustName)) { qry = qry.Where(p => p.CustomerName.Contains(fltCustName)); };
            if (!string.IsNullOrWhiteSpace(fltVinNo)) { qry = qry.Where(p => p.Chassis.Contains(fltVinNo)); };
            if (!string.IsNullOrWhiteSpace(fltPolReg)) { qry = qry.Where(p => p.PoliceRegNo.Contains(fltPolReg)); };
            */
            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<CsLkuFeedbackView>("exec uspfn_CsLkuFeedback2 @CompanyCode=@p0, @BranchCode=@p1, @OutStanding=@p2, @CustomerName=@p3, @VinNo=@p4, @PolReg=@p5", CompanyCode, BranchCode, outstanding, fltCustName, fltVinNo, fltPolReg).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomerBirthdays()
        {
            var qry = ctx.CsLkuBirthdayViews.Where(p =>
                        p.CompanyCode == CompanyCode &&
                        p.BranchCode == BranchCode
                    );

            return Json(qry.KGrid());
        }

        //public JsonResult Customers()
        //{
        //    var qry = ctx.GnMstCustomers.Where(p =>
        //                p.CompanyCode == CompanyCode &&
        //            );

        //    return Json(qry.KGrid());
        //}

        public JsonResult CsTDaysCall()
        {
            var outstanding = Request["Outstanding"] ?? "N";
            var customerName = Request["fltCustName"] ?? "";
            var vinNo = Request["fltVinNo"] ?? "";
            var policeRegNo = Request["fltPolReg"] ?? "";
            var BPKDate = Request["fltBPKDate"] ?? "";
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            var dateFrom = Request["DateFrom"];
            var dateTo = Request["DateTo"];

            var data = ctx.Database.SqlQuery<CsLkuTDayCallView>("exec uspfn_CsInqTDaysCall @CompanyCode=@p0, @BranchCode=@p1, @DateFrom=@p2, @DateTo=@p3, @Outstanding=@p4, @Status=@p5", companyCode, branchCode, dateFrom, dateTo, outstanding, "Lookup").AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                data = data.Where(x => x.CustomerName.ToLower().Contains(customerName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(vinNo))
            {
                data = data.Where(x => x.Chassis.ToLower().Contains(vinNo.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(policeRegNo))
            {
                data = data.Where(x => x.PoliceRegNo.ToLower().Contains(policeRegNo.ToLower()));
            }

            //if (!string.IsNullOrEmpty(BPKDate))
            //{
            //    data = data.Where( x=> x.BPKDate.)
            //}

            return Json(data.KGrid());
        }

        public JsonResult CsDlvryOutstanding()
        {
            string companyCode = CompanyCode;
            string branchCode = BranchCode;

            var data = ctx.Database.SqlQuery<CsDlvryOutstanding>("exec uspfn_CsDlvryOutstanding @CompanyCode=@p0, @BranchCode=@p1", companyCode, branchCode).OrderByDescending(x => x.BPKDate).AsQueryable();
            return Json(data.KGrid());
        }

        public JsonResult CsCustBirthdays()
        {
            string branchCode = BranchCode;
            var outstanding = Request["Outstanding"] ?? "N";
            var customerCode = Request["CustomerCode"] ?? "";
            var customerName = Request["CustomerName"] ?? "";

            var data = ctx.Database.SqlQuery<CsLkuBirthdayView>("exec uspfn_CsInqCustomerBirthday @CompanyCode=@p0, @BranchCode=@p1, @Year=@p2, @MonthFrom=@p3, @MonthTo=@p4, @Outstanding=@p5, @Status=@p6", CompanyCode, BranchCode, null, null, null, outstanding, "Lookup").AsQueryable();
            var distinctedData = data; //.Select(x => new { x.CustomerCode, x.CustomerName, x.CustomerTelephone, x.CustomerBirthDate, x.CustomerBirthDay, x.Status, x.Reason }).Distinct();

            //var data = ctx.CsLkuBirthdayViews.Where(x => x.BranchCode == branchCode && x.Outstanding == outstanding).AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerCode))
            {
                distinctedData = distinctedData.Where(x => x.CustomerCode.ToLower().Contains(customerCode.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                distinctedData = distinctedData.Where(x => x.CustomerName.ToLower().Contains(customerName.ToLower()));
            }

            return Json(distinctedData.OrderBy(x => x.CustomerBirthDate.Value.Month).ThenBy(x => x.CustomerBirthDay).KGrid());
        }

        public JsonResult CsStnkExtensions()
        {
            var outstanding = Request["Outstanding"] ?? "N";
            var customerName = Request["fltCustName"] ?? "";
            var vinNo = Request["fltVinNo"] ?? "";
            var policeRegNo = Request["fltPolReg"] ?? "";
            string branchCode = BranchCode;

            //var data = ctx.CsLkuStnkExtensionViews.Where(x => x.Outstanding == outstanding && x.BranchCode == branchCode).AsQueryable();
            var data = ctx.Database.SqlQuery<CsLkuStnkExtensionView>("exec uspfn_CsStnkExtension @CompanyCode=@p0, @BranchCode=@p1, @IsStnkExtension=@p2, @DateFrom=@p3, @DateTo=@p4, @Outstanding=@p5, @Status=@p6", CompanyCode, BranchCode, null, null, null, outstanding, "Lookup").AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                data = data.Where(x => x.CustomerName.ToLower().Contains(customerName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(vinNo))
            {
                data = data.Where(x => x.Chassis.ToLower().Contains(vinNo.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(policeRegNo))
            {
                data = data.Where(x => x.PoliceRegNo.ToLower().Contains(policeRegNo.ToLower()));
            }

            return Json(data.KGrid());
        }

        public JsonResult CsBpkbReminders()
        {
            var outstanding = Request["Outstanding"] ?? "N";
            var customerName = Request["fltCustName"] ?? "";
            var vinNo = Request["fltVinNo"] ?? "";
            var policeRegNo = Request["fltPolReg"] ?? "";
            string branchCode = BranchCode;

            //var data = ctx.CsLkuBpkbReminderViews.Where(x => x.Outstanding == outstanding && x.BranchCode == branchCode).AsQueryable();
            var data = ctx.Database.SqlQuery<CsLkuBpkbReminderView>("exec uspfn_CsInqBpkbReminder @CompanyCode=@p0, @BranchCode=@p1, @DateFrom=@p2, @DateTo=@p3, @Outstanding=@p4, @Status=@p5", CompanyCode, BranchCode, null, null, outstanding, "Lookup").AsQueryable();
                        
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                data = data.Where(x => x.CustomerName.ToLower().Contains(customerName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(vinNo))
            {
                data = data.Where(x => x.Chassis.ToLower().Contains(vinNo.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(policeRegNo))
            {
                data = data.Where(x => x.PoliceRegNo.ToLower().Contains(policeRegNo.ToLower()));
            }

            return Json(data.toKG());
        }

        public JsonResult CsReviews()
        {
            string qry = @"select a.*, dom.OutletCode, dom.OutletAbbreviation 
                             from CsReviews a
                       inner join gnMstDealerOutletMapping dom 
                               on dom.DealerCode = a.CompanyCode and dom.OutletCode = a.BranchCode
                            where companycode='" + CompanyCode + "'";
            
            var Employee = ctx.HrEmployees.Where(a => a.CompanyCode == CompanyCode && a.RelatedUser == CurrentUser.UserId).FirstOrDefault();

            if (Employee != null)
            {
                if (Employee.Position != "GM" && CurrentUser.UserId != "ga")
                {
                    qry += " and branchcode='" + BranchCode + "'";
                    qry += " and employeeid='" + Employee.EmployeeID + "'";
                }
            }
            else if (CurrentUser.UserId != "ga")
            {
                return Json(new List<CsReviewModel>().AsQueryable().KGrid());
            }

            object[] paramz = new object[2];
            if (!string.IsNullOrEmpty(Request["fltFrom"]))
            {
                DateTime pDateFrom;
                DateTime.TryParse(Request["fltFrom"], out pDateFrom);
                qry += " and DateFrom>=@p0";
                paramz[0] = pDateFrom;
            }
            if (!string.IsNullOrEmpty(Request["fltTo"]))
            {
                DateTime pDateTo;
                DateTime.TryParse(Request["fltTo"], out pDateTo);
                qry += " and DateTo>=" + (Request["fltTo"] != "" ? "@p1" : "@p0");
                paramz[1] = pDateTo;
            }
            var data = ctx.Database.SqlQuery<CsReviewModel>(qry, paramz).AsQueryable();
            
            //var data = ctx.CsReviews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.EmployeeID == CurrentUser.UserId);
            return Json(data.KGrid());
        }

    }
}
