using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;


namespace SimDms.General.Controllers.Api
{
    public class CollectorController : BaseController   
    {
        public JsonResult CollectorLoad(GnMstCollector model) 
        {
            //var data = ctx.GnMstCollectors.Find(CompanyCode, BranchCode, model.CollectorCode, model.ProfitCenterCode);
            var data = from p in ctx.GnMstCollectors
                       join p1 in ctx.LookUpDtls
                       on p.ProfitCenterCode equals p1.LookUpValue
                       where p.CompanyCode == CompanyCode &&
                             p.BranchCode == BranchCode &&
                             p.CollectorCode == model.CollectorCode &&
                             p.ProfitCenterCode == model.ProfitCenterCode &&
                             p1.CodeID == "PFCN"
                       select new GnMstCollectorView()
                       {
                           CollectorCode = p.CollectorCode,
                           ProfitCenterCode = p.ProfitCenterCode,
                           CollectorName = p.CollectorName,
                           ProfitCenterNameDisc = p1.LookUpValueName
                       };
            if (data != null)
            {
                return Json(new { success = true, data = data });
            }
            else {
                return Json(new { success = true, data = data });
            }
            
        }
        [HttpPost]
        public JsonResult Save(GnMstCollector model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var collector = ctx.GnMstCollectors.Find(companyCode, branchCode, model.CollectorCode, model.ProfitCenterCode);

            if (collector == null)
            {
                collector = new GnMstCollector();
                collector.CreatedDate = currentTime;
                collector.LastUpdateDate = currentTime;
                collector.CreatedBy = userID;
                ctx.GnMstCollectors.Add(collector);
            }
                else{
                    collector.LastUpdateDate = currentTime;
                    collector.LastUpdateBy = userID;
            }               
                collector.CollectorCode = model.CollectorCode;
                collector.ProfitCenterCode = model.ProfitCenterCode;
                collector.CollectorName = model.CollectorName;
                collector.CompanyCode = companyCode;
                collector.BranchCode = branchCode;
               

                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data Customer Class berhasil disimpan.";
                    result.data = new
                    {
                        CollectorCode = collector.CollectorCode, 
                        ProfitCenterCode = collector.ProfitCenterCode
                    };
                }
                catch (Exception Ex)
                {
                    result.message = "Data customer tidak bisa disimpan.";
                    MyLogger.Info("Error on customer saving: " + Ex.Message);
                }
            
            return Json(result);
        }

        public JsonResult Delete(GnMstCollector model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var collector = ctx.GnMstCollectors.Find(companyCode, branchCode, model.CollectorCode, model.ProfitCenterCode);
                    if (collector != null)
                    {
                        ctx.GnMstCollectors.Remove(collector);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data customer class berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete customer class , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete customer class , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

    }
}
