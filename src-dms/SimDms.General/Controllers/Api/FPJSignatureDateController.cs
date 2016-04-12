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
    public class FPJSignatureDateController : BaseController 
    {
        [HttpPost]
        public JsonResult Save(GnMstFPJSignDate model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.GnMstFPJSignDates.Find(companyCode,branchCode, model.ProfitCenterCode);

            if (me == null)
            {
                me = new GnMstFPJSignDate(); 
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.GnMstFPJSignDates.Add(me); 
            }
                else{
                    me.LastUpdateDate = currentTime;
                    me.LastUpdateBy = userID;
            }
            me.CompanyCode = companyCode;
            me.BranchCode = branchCode;
            me.ProfitCenterCode = model.ProfitCenterCode;
            me.FPJOption = model.FPJOption;
            me.FPJOptionDescription = model.FPJOptionDescription;
                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data FPJ Signature Date berhasil disimpan.";
                    result.data = new
                    {
                        ProfitCenterCode = me.ProfitCenterCode,
                        FPJOptionDescription = me.FPJOptionDescription
                    };
                }
                catch (Exception Ex)
                {
                    result.message = "Data FPJ Signature Date tidak bisa disimpan.";
                    MyLogger.Info("Error on FPJ Signature Date saving: " + Ex.Message);
                }
            
            return Json(result);
        }

        public JsonResult Delete(GnMstFPJSignDate model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.GnMstFPJSignDates.Find(companyCode, branchCode, model.ProfitCenterCode);
                    if (me != null)
                    {
                        ctx.GnMstFPJSignDates.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data FPJ Signature Date berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete FPJ Signature Date , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete FPJ Signature Date , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        public JsonResult checkData(string ProfitCenterCode)
        {
            var me = ctx.GnMstFPJSignDates.Find(CompanyCode, BranchCode, ProfitCenterCode);
            if (me != null)
            {
                return Json(new
                {
                    success = true,
                    ProfitCenterCode = me.ProfitCenterCode,
                    FPJOption = me.FPJOption,
                    FPJOptionDescription = me.FPJOptionDescription
                }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new
                {
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
