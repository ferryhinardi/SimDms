using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
//using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common;
using SimDms.Common.Models;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;


namespace SimDms.General.Controllers.Api
{
    public class MasterLookupController : BaseController
    {
        public JsonResult LookUpDetailsLoad(string CodeID)  
        {
            return Json(ctx.Database.SqlQuery<LookUpDtls>("Select LookUpValue, ParaValue, LookUpValueName from gnMstLookUpDtl where CompanyCode='" + CompanyCode + "' and CodeID='" + CodeID + "'").AsQueryable());
        }

        [HttpPost]
        public JsonResult Save(LookUpHdr model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.GnMstLookUpHdrs.Find(companyCode, model.CodeID);

            if (me == null)
            {
                me = new LookUpHdr(); 
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.GnMstLookUpHdrs.Add(me); 
            }
                else{
                    me.LastUpdateDate = currentTime;
                    me.LastUpdateBy = userID;
            }
             me.CodeID= model.CodeID;
             me.CodeName = model.CodeName;
             me.FieldLength = model.FieldLength;
             me.isNumber = model.isNumber;
             me.CompanyCode = companyCode;
              
                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data Lookup Header berhasil disimpan.";
                    result.data = new
                    {
                        CodeID = me.CodeID,
                        CodeName = me.CodeName
                    };
                }
                catch (Exception Ex)
                {
                    result.message = "Data Lookup Header tidak bisa disimpan.";
                    MyLogger.Info("Error on Lookup Header saving: " + Ex.Message);
                }
            
            return Json(result);
        }

        public JsonResult Delete(LookUpHdr model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.GnMstLookUpHdrs.Find(companyCode, model.CodeID);
                    if (me != null)
                    {
                        ctx.GnMstLookUpHdrs.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Lookup Header berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Lookup Header , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Lookup Header , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        [HttpPost]
        public JsonResult SaveDtl(LookUpDtl model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;
            string msg = "";
            var me = ctx.LookUpDtls.Find(companyCode, model.CodeID, model.LookUpValue);

            if (me == null)
            {
                me = new LookUpDtl();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.LookUpDtls.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.ParaValue = model.ParaValue;
            me.LookUpValue = model.LookUpValue;
            me.LookUpValueName = model.LookUpValueName;
            me.SeqNo = model.SeqNo;
            me.CodeID = model.CodeID;
            me.CompanyCode = companyCode;

            try
            {
                ctx.SaveChanges();
                msg = "Data Lookup Detail berhasil disimpan.";
                var records = ctx.Database.SqlQuery<LookUpDtls>("select LookUpValue, ParaValue, LookUpValueName from GnMstLookUpDtl where CompanyCode='" + CompanyCode + "'and CodeID='" + model.CodeID + "'").AsQueryable();
                return Json(new { status = true, message = msg, data = me, result = records });

            }
            catch (Exception Ex)
            {
                msg = "Data Lookup Detail tidak bisa disimpan.";
                MyLogger.Info("Error on Lookup Header saving: " + Ex.Message);
                return Json(new { status = false, message = Ex.Message });
            }

            return Json(result);
        }

        public JsonResult DeleteDtl(LookUpDtl model) 
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.LookUpDtls.Find(companyCode, model.CodeID, model.LookUpValue);
                    if (me != null)
                    {
                        ctx.LookUpDtls.Remove(me);
                        ctx.SaveChanges();
                        var records = ctx.Database.SqlQuery<LookUpDtls>("select LookUpValue, ParaValue, LookUpValueName from GnMstLookUpDtl where CompanyCode='" + CompanyCode + "'and CodeID='" + model.CodeID + "'").AsQueryable();
                        returnObj = new { success = true, message = "Data Lookup Header berhasil di delete.", result = records };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Lookup Header , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Lookup Header , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
    }
}
