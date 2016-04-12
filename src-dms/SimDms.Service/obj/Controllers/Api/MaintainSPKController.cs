using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;

namespace SimDms.Service.Controllers.Api
{
    public class MaintainSPKController : BaseController
    {
        //
        // GET: /MaintainSPK/

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName
            });
        }

        public ActionResult Save(TrnService model)
        {
            TrnService trnService = ctx.Services.Find(CompanyCode, BranchCode, ProductType, model.ServiceNo);

            //trnService.LaborGrossAmt = ctx.ServiceTasks.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType && e.ServiceNo == model.ServiceNo).Sum(e => e.OperationHour * e.OperationCost);
            //trnService.PartsGrossAmt = ctx.ServiceItems.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType && e.ServiceNo == model.ServiceNo && e.TypeOfGoods != 1).Sum(e => e.DemandQty * e.RetailPrice);
            //trnService.MaterialGrossAmt = ctx.ServiceItems.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType && e.ServiceNo == model.ServiceNo && e.TypeOfGoods == 1).Sum(e => e.DemandQty * e.RetailPrice);
            //trnService.LaborDiscAmt = trnService.LaborGrossAmt * trnService.LaborDiscPct / 100;
            //trnService.PartsDiscAmt = trnService.PartsGrossAmt * trnService.PartDiscPct / 100;
            //trnService.MaterialDiscAmt = trnService.MaterialGrossAmt * trnService.MaterialDiscPct / 100;
            //trnService.LaborDppAmt = trnService.LaborGrossAmt - trnService.LaborDiscAmt;
            //trnService.PartsDppAmt = trnService.PartsGrossAmt - trnService.PartsDiscAmt;
            //trnService.MaterialDppAmt = trnService.MaterialGrossAmt - trnService.MaterialDiscAmt;
            //trnService.TotalDPPAmount = trnService.LaborDppAmt + trnService.PartsDppAmt + trnService.MaterialDppAmt;
            //trnService.TotalPphAmount = trnService.LaborDppAmt * trnService.PPHPct / 100;
            //trnService.TotalPpnAmount = trnService.TotalDPPAmount * trnService.PPNPct / 100;
            //trnService.TotalSrvAmount = trnService.TotalDPPAmount + trnService.TotalPphAmount + trnService.TotalPpnAmount;
            trnService.ServiceStatus = model.ServiceStatus;
            trnService.LastUpdateBy = CurrentUser.UserId;
            trnService.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = "Save Succeeded", serviceStatus = trnService.ServiceStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
