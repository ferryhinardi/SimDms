using SimDms.PreSales.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.PreSales.Controllers.Api
{
    public class InquiryOutStandingProspekController : BaseController
    {
        public JsonResult GetData4InqOutStanding(string Priode, string NikBM, string NikSH, string NikSC, string NikSL, string x)
        {
            var COO = "";
            //var Position = ctx.HrEmployees.Where(a => a.CompanyCode == CompanyCode && a.RelatedUser == CurrentUser.UserId);

            //if (Position == null) { COO = ""; } else { COO = "1"; }
            if (NikBM == "" && NikSH == "" && NikSC == "" && NikSL == "") { COO = "1"; } else { COO = ""; }

            var query = string.Format(@"select * from pmkdp'");
            if (x=="1"){
                query = string.Format(@"exec usprpt_PmRpInqOutStanding_NewBySalesman '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'
            ", CompanyCode, BranchCode, Priode, COO, NikBM, NikSH, NikSC, NikSL);
            }
            if (x == "2")
            {
                query = string.Format(@"exec usprpt_PmRpInqOutStanding_NewByType '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'
            ", CompanyCode, BranchCode, Priode, COO, NikBM, NikSH, NikSC, NikSL);
            }
            if (x == "3")
            {
                query = string.Format(@"exec usprpt_PmRpInqOutStanding_NewByData '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'
            ", CompanyCode, BranchCode, Priode, COO, NikBM, NikSH, NikSC, NikSL);
            }
            var queryable = ctx.Database.SqlQuery<OutStandingProspekBySalesman>(query);
            return Json(new { queryable = queryable });
        }
    }
}