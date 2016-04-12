using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class InputApprovalController : BaseController
    {
        public JsonResult GetSPKforApprovalPDIFSC(bool isPDI)
        {
            var query = "exec uspfn_GetSPKForApprovalPdiFsc @p0, @p1, @p2, @p3";
            var result = ctx.Database.SqlQuery<InputApproval>(query, CompanyCode, BranchCode, ProductType, isPDI);

            return Json(new { result = result });
        }

        public JsonResult GetSPKForUnApprovalPdiFsc(bool isPDI)
        {
            var query = "exec uspfn_GetSPKForUnApprovalPdiFsc @p0, @p1, @p2, @p3";
            var result = ctx.Database.SqlQuery<InputApproval>(query, CompanyCode, BranchCode, ProductType, isPDI);

            return Json(new { result = result });
        }

        public JsonResult ApproveSPKPdiFsc(IEnumerable<InputApproval> data)
        {
            var message = "";
            
            try
            {
                foreach (var item in data)
                {
                    var service = ctx.Services.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                        x.BranchCode == BranchCode && x.ProductType == ProductType && x.ServiceNo == item.ServiceNo);
                    service.IsLocked = true;
                    service.LockingBy = CurrentUser.UserId;
                    service.LockingDate = DateTime.Now;
                    service.LastUpdateBy = CurrentUser.UserId;
                    service.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }
    }
}