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
    public class MessageController : BaseController 
    {          
        [HttpPost]
        public JsonResult Save(SysMessageBoard model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var message = ctx.SysMessageBoards.Find(model.MessageID);

            if (message == null)
            {
                message = new SysMessageBoard();
                message.CreatedDate = currentTime;
                message.LastUpdateDate = currentTime;
                message.CreatedBy = userID;
                ctx.SysMessageBoards.Add(message);
            }
                else{
                    message.LastUpdateDate = currentTime;
                    message.LastUpdateBy = userID;
            }
            message.MessageHeader = model.MessageHeader;
            message.MessageText = model.MessageText;       
               
                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data message berhasil disimpan.";
                    result.data = new
                    {
                        MessageHeader = model.MessageHeader,
                        MessageText = model.MessageText
                    };
                }
                catch (Exception Ex)
                {
                    result.message = "Data message tidak bisa disimpan.";
                    MyLogger.Info("Error on message saving: " + Ex.Message);
                }
            
            return Json(result);
        }

        public JsonResult Delete(SysMessageBoard model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var message = ctx.SysMessageBoards.Find(model.MessageID);
                    if (message != null)
                    {
                        ctx.SysMessageBoards.Remove(message);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data message berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete message , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete message , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

    }
}
