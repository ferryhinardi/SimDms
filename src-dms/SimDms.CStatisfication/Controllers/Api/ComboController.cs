using SimDms.CStatisfication.Models;
using SimDms.CStatisfication.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class ComboController : BaseController
    {
        public JsonResult ListBranchCode()
        {
            var listBranch = ctx.GnMstOrganizationDtls.Select(m => new {value=m.BranchCode, text= m.BranchCode + " - " + m.BranchName }).OrderBy(x => x.value);
            return Json(listBranch) ;
        }

        public JsonResult CurrentBranchCode()
        {
            string branchcode = CurrentUser.BranchCode;
            var currBranch = ctx.GnMstOrganizationDtls.Where(p => p.BranchCode == branchcode).OrderBy(p => p.SeqNo).Select(m => new { value = m.BranchCode, text = m.BranchCode + " - " + m.BranchName }).OrderBy(x => x.value);
            return Json(currBranch);
        }

        public JsonResult isGM()
        {
            string isGM = "2";
            return Json(isGM);
        }

        public JsonResult ListOfMonth()
        {
            List<Object> listObj = new List<Object>();
            string[] listMonth = new string[]{ "Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember" };
            int idx = 1;
            foreach (var month in listMonth)
            {
                listObj.Add(new {value=idx,text=month});
                idx++;
            }
            return Json(listObj);
        }

        public JsonResult ListOfYear()
        {
            var year = DateTime.Now.Year;
            List<Object> listObj = new List<Object>();
            for (int i = year-5; i <= year; i++)
            {
                listObj.Add(new {value= i.ToString(), text=i.ToString() });
            }
            return Json(listObj);
        }

        public JsonResult Holidays()
        {
            var queryable = ctx.Holidays.Where(p => p.HolidayYear == DateTime.Now.Year);
            var list = ctx.Holidays.OrderBy(p => p.DateFrom).Select(p => new { value = p.HolidayCode, text = p.HolidayDesc }).ToList();
            return Json(list);
        }

        public JsonResult CustLeasing()
        {
            //var ListCust = ctx.GnMstCustomers.ToList().Where(m => m.CategoryCode == "32").Select(m => new { value =m.CustomerCode, text = m.CustomerName});
            var ListCust = ctx.Database.SqlQuery<ComboModel>("exec uspfn_CsCustLeasingList @CompanyCode=@p0", CompanyCode);
            return Json(ListCust);
        }

        public JsonResult Lookups(string id = "")
        {
            var list = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == id).OrderBy(p => p.SeqNo).Select(p => new { value = p.LookUpValue, text = p.LookUpValueName.ToUpper() }).OrderBy(x => x.text).ToList();
            return Json(list);
        }

        public JsonResult MonthList()
        {
            var data = new List<object>();
            data.Add(new { text = "January", value = "1" });
            data.Add(new { text = "February", value = "2" });
            data.Add(new { text = "March", value = "3" });
            data.Add(new { text = "April", value = "4" });
            data.Add(new { text = "May", value = "5" });
            data.Add(new { text = "June", value = "6" });
            data.Add(new { text = "July", value = "7" });
            data.Add(new { text = "August", value = "8" });
            data.Add(new { text = "September", value = "9" });
            data.Add(new { text = "October", value = "10" });
            data.Add(new { text = "November", value = "11" });
            data.Add(new { text = "December", value = "12" });

            return Json(data);
        }

        [HttpPost]
        public JsonResult Status()
        {
            var list = new List<listitem>();
            list.Add(new listitem() { value = "0", text = "In Progress" });
            list.Add(new listitem() { value = "2", text = "Finish" });
            return Json(list);
        }

        public class listitem
        {
            public string value;
            public string text;
        }

        public JsonResult Reasons()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("CSREASON") == true
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            text = x.LookUpValueName.ToUpper(),
                            value = x.LookUpValueName.ToUpper()
                        }).ToList();

            return Json(data);
        }

        public JsonResult ReviewPlans()
        {
            var data = (from x in ctx.CsSettings
                        where
                        x.SettingLink2 != null && x.SettingLink3 != null
                        orderby x.SettingLink2 ascending
                        select new
                        {
                            text = x.SettingLink2.ToUpper(),
                            value = x.SettingLink2.ToUpper()
                        }).ToList();

            return Json(data);
        }

        public JsonResult IsHolding()
        {
            string branchcode = CurrentUser.BranchCode;
            var currBranch = ctx.GnMstOrganizationDtls.Where(p => p.BranchCode == branchcode).ToList().SingleOrDefault();
            if (currBranch.IsBranch == false)
                return Json(true);
            else
                return Json(false);
        }
    }
}
