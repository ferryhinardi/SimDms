using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Mvc; 
namespace SimDms.Tax.Controllers.Api
{
    public class ComboController : BaseController
    {
        public JsonResult Years()
        {
            List<object> listOfYears = new List<object>();
            for (int i = 1900; i <= 2100; i++)
            {
                listOfYears.Add(new { value = i, text = i });
            }
            return Json(listOfYears);
        }

        public JsonResult Months()
        {
            List<object> listOfMonths = new List<object>();
            int Now = DateTime.Now.Year;
            int Past = DateTime.Now.Year - 10;

            listOfMonths.Add(new { value = 1, text = "January" });
            listOfMonths.Add(new { value = 2, text = "February" });
            listOfMonths.Add(new { value = 3, text = "March" });
            listOfMonths.Add(new { value = 4, text = "April" });
            listOfMonths.Add(new { value = 5, text = "May" });
            listOfMonths.Add(new { value = 6, text = "June" });
            listOfMonths.Add(new { value = 7, text = "July" });
            listOfMonths.Add(new { value = 8, text = "August" });
            listOfMonths.Add(new { value = 9, text = "September" });
            listOfMonths.Add(new { value = 10, text = "October" });
            listOfMonths.Add(new { value = 11, text = "November" });
            listOfMonths.Add(new { value = 12, text = "December" });

            return Json(listOfMonths);
        }

        public JsonResult SignName()
        {
            var list = ctx.gnMstSignatures.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).OrderBy(p => p.SeqNo).Select(p => new { value = p.SignName, text = p.SignName.ToUpper() }).ToList();
            return Json(list);
        }

        public JsonResult SignNameSeq(string ProfitCenter)
        {

            string doctyp = ProfitCenter == "300" ? "FPJ" : ProfitCenter == "200" ? "FPS" : ProfitCenter == "000" ? "PJK" : ProfitCenter == "100" ? "PJU" : "";
            var lst= ctx.gnMstSignatures
                .Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProfitCenterCode==ProfitCenter);

            if(doctyp!="")
                lst=lst.Where(x=>x.DocumentType==doctyp);


            var list=lst
                    .OrderBy(p => p.SeqNo)
                    .Select(p => new { value = p.SeqNo, text = p.SignName.ToUpper() }).ToList();
            return Json(list);

        }

        public JsonResult LoadLookup(string CodeID)
        {
            var user = CurrentUser;
            var listLkp = ctx.LookUpDtls.Where(m => m.CompanyCode == user.CompanyCode && m.CodeID == CodeID).OrderBy(x => x.ParaValue).Select(m => new { value = m.ParaValue, text = m.LookUpValueName }).ToList();
            return Json(listLkp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Branch()
        {
            var user = CurrentUser;
            var isBranch = ctx.OrganizationDtls.Where(a => a.CompanyCode == user.CompanyCode && a.BranchCode == user.BranchCode).FirstOrDefault();
            var branchlist = ctx.CoProfiles.Where(m => m.CompanyCode == user.CompanyCode)
                .OrderBy(x => x.BranchCode)
                .Select(m => new { value = m.BranchCode, text = m.BranchCode }).ToList();
            return Json(new { branchlist = branchlist, isBranch = isBranch.IsBranch, Branch = isBranch.BranchCode});
        }
    }
}
