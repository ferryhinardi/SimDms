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
    public class FakturPajakSeqController : BaseController  
    {
        [HttpPost]
        public JsonResult Save(GnMstFPJSeqNo model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.GnMstFPJSeqNos.Find(companyCode, model.BranchCode, model.Year, model.SeqNo);

            if (me == null)
            {
                me = new GnMstFPJSeqNo(); 
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.GnMstFPJSeqNos.Add(me); 
            }
                else{
                    me.LastUpdateDate = currentTime;
                    me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = model.BranchCode;
            me.Year = model.Year;
            me.FPJSeqNo = model.FPJSeqNo;
            me.LockedTime = model.LockedTime;
            me.SeqNo = model.SeqNo;
            me.BeginTaxNo = model.BeginTaxNo;
            me.EndTaxNo = model.EndTaxNo;
            me.EffectiveDate = model.EffectiveDate;
            me.isActive = model.isActive;

            try
            {
                Helpers.ReplaceNullable(me);
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data FPJ Sequence berhasil disimpan.";
                result.data = new
                {
                    FPJSeqNo = me.FPJSeqNo,
                    EffectiveDate = me.EffectiveDate
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data FPJ Sequence tidak bisa disimpan.";
                MyLogger.Info("Error on FPJ Sequence saving: " + Ex.Message);
            }
            
            return Json(result);
        }

        public JsonResult Delete(GnMstFPJSeqNo model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.GnMstFPJSeqNos.Find(companyCode, model.BranchCode, model.Year, model.SeqNo);
                    if (me != null)
                    {
                        ctx.GnMstFPJSeqNos.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data FPJ Sequence berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete FPJ Sequence , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete FPJ Sequence , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        public JsonResult LastSeqNo(string branchCode, string year)
        {
            string cmdText = string.Format(@"
SELECT  ISNULL(MAX(SeqNo), 0) + 1
FROM GnMstFPJSeqNo a
where CompanyCode = '{0}'
  and BranchCode = '{1}'
  and Year = {2}", CompanyCode, branchCode, year);

            Int32 lastSeqNo = ctx.Database.SqlQuery<Int32>(cmdText).FirstOrDefault();

            return Json(new { data = lastSeqNo });
        }
    }
}
