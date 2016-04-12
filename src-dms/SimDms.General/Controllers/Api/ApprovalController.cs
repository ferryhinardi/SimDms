using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common.Models;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;

namespace SimDms.General.Controllers.Api
{
    public class ApprovalController : BaseController 
    {
        public JsonResult ApprovalLoad(string ApprovalKey)
        {
            return Json(ctx.Database.SqlQuery<ApprovalView>("select a.DocumentType, a.ApprovalNo from GnMstApproval a where ApprovalKey ='" + ApprovalKey + "'").AsQueryable());
        }

        [HttpPost]
        public JsonResult Save(GnMstApproval model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;
            var me = ctx.GnMstApprovals.Find(CompanyCode, BranchCode, model.DocumentType, model.ApprovalNo, model.SeqNo);

            if (me == null)
            {
                me = new GnMstApproval();
                me.CreatedBy = userID;
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
                ctx.GnMstApprovals.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.DocumentType = model.DocumentType;
            me.ApprovalNo = model.ApprovalNo;
            me.SeqNo = model.SeqNo;
            me.UserID = model.UserID;
            me.IsActive = model.IsActive;
            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Approval berhasil disimpan.";
                result.data = new
                {
                    ApprovalNo = me.ApprovalNo,
                    DocumentType = me.DocumentType
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Approval tidak bisa disimpan.";
                MyLogger.Info("Error on Approval saving: " + Ex.Message);
            }

            return Json(result);
        }

        public JsonResult Delete(GnMstApproval model)
        {
            object returnObj = null;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.GnMstApprovals.Find(CompanyCode, BranchCode, model.DocumentType, model.ApprovalNo, model.SeqNo);
                    if (me != null)
                    {
                        ctx.GnMstApprovals.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Approval berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Approval, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Approval , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
    }
}