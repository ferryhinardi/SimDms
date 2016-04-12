using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Sales.Controllers.Api
{
    public class PermohonanFktrPlsDCSController : BaseController
    {
        public JsonResult SelectBatchByLockedBy()
        {
            var query = String.Format(@"select * from omTrSalesReq where CompanyCode = '{0}' and BranchCode = '{1}' 
            ",CompanyCode, BranchCode);

            var Result = ctx.Database.SqlQuery<SalesReqView>(query).AsQueryable();

            return Json(new { success = true, Result = Result });

        }

    }
}
