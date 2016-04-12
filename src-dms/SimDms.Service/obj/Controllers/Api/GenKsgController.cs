using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class GenKsgController : BaseController
    {
        //
        // GET: /GenKsg/

        public ActionResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                ProductType = ProductType
            });
        }


        public JsonResult Get(GenSpkFilter filter)
        {
            #region -- Query --
            var query = @"
select (row_number() over (order by a.GenerateNo)) RecNo
,IsSelected = 1
,a.GenerateNo
,a.GenerateDate
,a.RefferenceNo
,RefferenceDate = (case a.RefferenceDate when '19000101' then null else a.RefferenceDate end)
,a.SenderDealerCode
,a.SenderDealerName
,a.TotalNoOfItem
,a.TotalLaborAmt
,a.TotalMaterialAmt
,a.TotalAmt
from svTrnPdiFsc a
where 1 = 1
 and a.CompanyCode = @p0
 and a.BranchCode = @p1
 and a.ProductType = @p2
 and a.BatchNo = @p3
";
            #endregion

            var qry = ctx.Database.SqlQuery<GenSpkView>(query, CompanyCode, BranchCode, ProductType, filter.NoBatch);

            return Json(new { success = true, branchInfo = qry });
        }

    }

    public class GenSpkFilter
    {
        public string NoBatch { get; set; }
        public string ReceiptNo { get; set; }
        public string FPJNo { get; set; }
        public string FPJGovNo { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime? FPJDate { get; set; }
    }

    public class GenSpkView
    {
        public string BranchCode { get; set; }
        public string GenerateNo { get; set; }
        public DateTime? GenerateDate { get; set; }
        public string SenderDealerCode { get; set; }
        public string SenderDealerName { get; set; }
        public decimal? TotalNoOfItem { get; set; }
        public decimal? TotalLaborAmt { get; set; }
        public decimal? TotalMaterialAmt { get; set; }
        public decimal? TotalAmt { get; set; }
    }
}
