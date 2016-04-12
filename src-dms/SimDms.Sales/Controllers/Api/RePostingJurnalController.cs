using SimDms.Common.Models;
using SimDms.Sales.Models.Result;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.Models;

namespace SimDms.Sales.Controllers.Api
{
    public class RePostingJurnalController : BaseController
    {
        public JsonResult PopulateData()
        {
            var query = string.Format(@"uspfn_OmRePostingInquiry '{0}' ,'{1}'
                ", CompanyCode, BranchCode);

            var queryable = ctx.Database.SqlQuery<JurnalInventory>(query).AsQueryable();
            return Json(new { success = true, data = queryable });
        }

        public JsonResult RePostingJurnal(List<JurnalPurchase> model)
        {
            var query = "";
            int queryable = 0;
            try
            {
                if (model.Count() > 0)
                {
                    foreach (JurnalPurchase recDtl in model)
                    {
                        query = string.Format(@"uspfn_OmRePosting '{0}','{1}','{2}','{3}','{4}'
                                    ", CompanyCode, BranchCode, recDtl.TypeJournal, recDtl.DocNo, CurrentUser.UserId);

                        queryable = ctx.Database.ExecuteSqlCommand(query);
                    }
                }

                return Json(new { success = true, message = "Sebanyak " +model.Count+ " data berhasil di RePosting" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, error_log = ex.Message });
            }
        }

    }
}
