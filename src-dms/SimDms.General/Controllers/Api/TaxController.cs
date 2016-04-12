using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common;
using SimDms.Common.Models;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;


namespace SimDms.General.Controllers.Api
{
    public class TaxController : BaseController  
    {          
        [HttpPost]
        public JsonResult Save(Tax model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var tax = ctx.Taxs.Find(companyCode, model.TaxCode);

            if (tax == null)
            {
                tax = new Tax();
                tax.CreatedDate = currentTime;
                tax.LastUpdateDate = currentTime;
                tax.CreatedBy = userID;
                ctx.Taxs.Add(tax);
            }
                else{
                    tax.LastUpdateDate = currentTime;
                    tax.LastUpdateBy = userID;
            }               
                tax.TaxCode = model.TaxCode.ToUpper();
                tax.TaxPct = model.TaxPct;
                tax.Description = model.Description.ToUpper();
                tax.CompanyCode = companyCode;
               

                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data Customer Class berhasil disimpan.";
                    result.data = new
                    {
                        TaxCode = tax.TaxCode,
                        TaxPct = tax.TaxPct
                    };
                }
                catch (Exception Ex)
                {
                    result.message = "Data customer tidak bisa disimpan.";
                    MyLogger.Info("Error on customer saving: " + Ex.Message);
                }
            
            return Json(result);
        }

        public JsonResult Delete(Tax model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var tax = ctx.Taxs.Find(companyCode, model.TaxCode);
                    if (tax != null)
                    {
                        ctx.Taxs.Remove(tax);
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
