using SimDms.CStatisfication.Models;
using SimDms.CStatisfication.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class GridController : BaseController
    {
        public JsonResult Holidays()
        {
            var queryable = ctx.Holidays;
            return Json(GeLang.DataTables<Holiday>.Parse(queryable, Request));
        }

        public JsonResult Customers(int paramException = 0)
        {
            IQueryable<CustomerView> queryable;
            IQueryable<string> currentData;
            DateTime currentDateTime = DateTime.Now;

            switch (paramException)
            {
                case 1: //Showing customer data that not exist in CsCustBirthDay
                    IQueryable<string> customerCode = ctx.CsCustBirthDays.Select(x => x.CustomerCode);
                    queryable = ctx.CustomerViews.Where(p =>
                        p.CustomerName != "" &&
                        p.CustomerName != null &&
                        !customerCode.Contains(p.CustomerCode)
                    );
                    break;
                //return Json(GeLang.DataTables<CustomerView>.Parse(queryable, Request));
                case 2:
                    queryable = ctx.CustomerViews.Where(p => p.CustomerName != "" && p.BirthDate != null && p.BirthDate.Value.Month >= currentDateTime.Month);
                    //return Json(GeLang.DataTables<CustomerView>.Parse(queryable, Request));
                    break;
                default: //All customer data showed
                    queryable = ctx.CustomerViews.Where(p => p.CustomerName != "");
                    //return Json(GeLang.DataTables<CustomerView>.Parse(queryable, Request));
                    break;
            }

            return Json(queryable);
        }

        public JsonResult CustomerList()
        {
           
            //if (!string.IsNullOrWhiteSpace(customerCode)) { qry = qry.Where(p => p.CustomerName.Contains(customerCode)); };
            //if (!string.IsNullOrWhiteSpace(customerName)) { qry = qry.Where(p => p.CustomerName.Contains(customerName)); };
            //if (!string.IsNullOrWhiteSpace(outStanding)) { qry = qry.Where(p => p.CustomerName.Contains(outStanding)); };
            
            //return Json(qry.KGrid());
            return null;
        }

        public JsonResult CustomerBirthDays()
        {
            string customerName = Request["CustomerName"] ?? "";
            string customerCode = Request["CustomerCode"] ?? "";
            string branchCode = Request["BranchCode"] ?? "";
            string outStanding = Request["OutStanding"] ?? "";

            //var qry = ctx.CsLkuBirthdayViews.Where(p =>
            //                p.CompanyCode == CompanyCode
            //                &&
            //                p.BranchCode == BranchCode
            //          );
            //if (!string.IsNullOrWhiteSpace(outStanding))
            //{
            //    var setting = ctx.CsSettings.Find(CompanyCode, "REMBDAYS");

            //    if (setting != null)
            //    {
            //        var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
            //        var date1 = DateTime.Now.AddMonths(-month);
            //        var date2 = new DateTime(date1.Year, date1.Month, 1);

            //        var date3 = DateTime.Now.AddMonths(1);
            //        var date4 = new DateTime(date3.Year, date3.Month, 1);

            //        qry = qry.Where(p => p.Outstanding == outStanding && p.CustomerBirthDate.Value.Month >= date2.Month && p.CustomerBirthDate.Value.Month < date4.Month);
            //    }
            //};

            var qry = ctx.Database.SqlQuery<CsLkuBirthdayView>("exec uspfn_CsLkuCustBday @CompanyCode=@p0, @BranchCode=@p1, @OutStanding=@p2, @CustomerCode=@p3, @CustomerName=@p4", CompanyCode, BranchCode, outStanding, customerCode, customerName).AsQueryable();
            //var qry = ctx.Database.SqlQuery<CsLkuBirthdayView>("exec uspfn_CsLkuCustBday @CompanyCode=@p0, @BranchCode=@p1, @OutStanding=@p2, @CustomerCode=@p3, @CustomerName=@p4", CompanyCode, BranchCode, outStanding, customerCode, customerName);

            if (!string.IsNullOrWhiteSpace(customerCode)) { qry = qry.Where(p => p.CustomerCode.Contains(customerCode)); };
            if (!string.IsNullOrWhiteSpace(customerName)) { qry = qry.Where(p => p.CustomerName.Contains(customerName)); };
            if (!string.IsNullOrWhiteSpace(branchCode)) { qry = qry.Where(p =>  p.BranchCode != null && p.BranchCode.Contains(branchCode)); };

            return Json(qry.KGrid());
            //return Json(qry);
        }

        public JsonResult CustomerBuys()
        {
            var queryable = ctx.CustomerBuyViews.Where(p => p.CustomerName != "");  // retrieve customer from database using EF
            return Json(GeLang.DataTables<CustomerBuyView>.Parse(queryable, Request));
        }

        public JsonResult CsDlvryOutstanding()
        {
            string companyCode = CompanyCode;
            string branchCode = BranchCode;

            var data = ctx.Database.SqlQuery<CsDlvryOutstanding>("exec uspfn_CsDlvryOutstanding @CompanyCode=@p0, @BranchCode=@p1", companyCode, branchCode).AsQueryable();
            return Json(data.KGrid());
        }

        public JsonResult CsTDayCalls()
        {
            var outstanding = Request["OutStanding"] ?? "N";
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            var dateFrom = Request["DateFrom"];
            var dateTo = Request["DateTo"];

            var qry = ctx.Database.SqlQuery<CsLkuTDayCallView>("exec uspfn_CsInqTDaysCall @CompanyCode=@p0, @BranchCode=@p1, @DateFrom=@p2, @DateTo=@p3, @Outstanding=@p4, @Status=@p5", companyCode, branchCode, dateFrom, dateTo, outstanding, "Lookup").AsQueryable();

            //var qry = ctx.CsLkuTDayCallViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode);
            //if (oustanding == "Y")
            //{
            //    var setting = ctx.CsSettings.Find(CompanyCode, "REM3DAYSCALL");
            //    var date1 = DateTime.Now;
            //    if (setting.SettingParam1.Length == 10)
            //    {
            //        date1 = Convert.ToDateTime(setting.SettingParam1);
            //    }
            //    else
            //    {
            //        var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
            //        date1 = DateTime.Now.AddMonths(-month);
            //    }    
            //    var date2 = new DateTime(date1.Year, date1.Month, 1);
            //    qry = qry.Where(p => p.Outstanding == "Y" && p.DODate >= date2);
            //}
            return Json(qry.KGrid());
        }

        public JsonResult StnkExt()
        {
            var outstanding = Request["OutStanding"] ?? "N";

            var qry = ctx.Database.SqlQuery<CsLkuStnkExtensionView>("exec uspfn_CsStnkExtension @CompanyCode=@p0, @BranchCode=@p1, @IsStnkExtension=@p2, @DateFrom=@p3, @DateTo=@p4, @Outstanding=@p5, @Status=@p6", CompanyCode, BranchCode, null, null, null, outstanding, "Lookup").AsQueryable();

            //var qry = ctx.CsLkuStnkExtensionViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode);
            //if (oustanding == "Y")
            //{
            //    var setting = ctx.CsSettings.Find(CompanyCode, "REMSTNKEXT");
            //    var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
            //    var date1 = DateTime.Now.AddMonths(-month);
            //    var date2 = new DateTime(date1.Year, date1.Month, 1);
            //    qry = qry.Where(p => p.Outstanding == "Y" && p.StnkExpiredDate >= date2);
            //}
            return Json(qry.KGrid());
        }

        public JsonResult CsBpkbs()
        {
            var outstanding = Request["OutStanding"] ?? "N";

            var qry = ctx.Database.SqlQuery<CsLkuBpkbReminderView>("exec uspfn_CsInqBpkbReminder @CompanyCode=@p0, @BranchCode=@p1, @DateFrom=@p2, @DateTo=@p3, @Outstanding=@p4, @Status=@p5", CompanyCode, BranchCode, null, null, outstanding, "Lookup").AsQueryable();
            
            //var qry = ctx.CsLkuBpkbReminderViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode);
            //if (oustanding == "Y")
            //{
            //    try
            //    {
            //        var setting = ctx.CsSettings.Find(CompanyCode, "REMBPKB");
            //        var month = (dynamic)null;
            //        DateTime tempSetting;
            //        if (DateTime.TryParse(setting.SettingParam1, out tempSetting))
            //        {
            //            month = Convert.ToInt32(tempSetting.ToString("MM"));
            //        }
            //        else
            //        {
            //            month = Convert.ToInt32(setting.SettingParam1 ?? "0");
            //        }
            //        //var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
            //        var date1 = DateTime.Now.AddMonths(-month);
            //        var date2 = new DateTime(date1.Year, date1.Month, 1);
            //        qry = qry.Where(p => p.Outstanding == "Y" && p.BpkbDate >= date2);
            //    }
            //    catch(Exception e)
            //    {
            //        throw e;
            //    }
            //}
            return Json(qry.KGrid());
        }

        public JsonResult CustHolidays()
        {
            var list = ctx.CustHolidayViews;
            return Json(GeLang.DataTables<CustHolidayView>.Parse(list, Request));
        }

        public JsonResult Settings()
        {
            string companyCode = CompanyCode;
            string settingCode = Request["filterSettingCode"] ?? "";
            string filterSettingDesc = Request["filterSettingDesc"] ?? "";

            var qry = ctx.CsSettings.Where(p => p.CompanyCode == CompanyCode);

            if (string.IsNullOrEmpty(settingCode) == false)
            {
                qry = qry.Where(x => x.SettingCode.Contains(settingCode));
            }
            if (string.IsNullOrEmpty(filterSettingDesc) == false)
            {
                qry = qry.Where(x => x.SettingDesc.Contains(filterSettingDesc));
            }

            return Json(qry.KGrid());
        }

    }
}
