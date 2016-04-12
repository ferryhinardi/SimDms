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
    public class ReminderController : BaseController 
    {
        public JsonResult ReminderLoad(string ReminderKey)
        {
            return Json(ctx.Database.SqlQuery<ReminderView>("select a.ReminderKey, a.ReminderCode, a.ReminderDescription, CONVERT(VARCHAR(50),a.ReminderDate,111) ReminderDate, a.ReminderTimePeriod, a.ReminderTimeDim, a.ReminderStatus from GnMstReminder a where ReminderKey ='" + ReminderKey + "'").AsQueryable());
        }

        public int maxkey()
        {
            var max = ctx.Database.SqlQuery<int>("select max(a.ReminderKey) from GnMstReminder a").FirstOrDefault();
            return max;
        }

        [HttpPost]
        public JsonResult Save(GnMstReminder model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.GnMstReminders.Find(CompanyCode, model.ReminderKey);

            if (me == null)
            {
                me = new GnMstReminder();
                me.CreatedBy = userID;
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
                var mxkey = maxkey();
                me.ReminderKey = mxkey + 1;
                ctx.GnMstReminders.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.ReminderCode = model.ReminderCode;
            me.ReminderDescription = model.ReminderDescription;
            me.ReminderDate = model.ReminderDate;
            me.ReminderTimePeriod = model.ReminderTimePeriod;
            me.ReminderTimeDim = model.ReminderTimeDim;
            me.ReminderStatus = model.ReminderStatus; 

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Reminder berhasil disimpan.";
                result.data = new
                {
                    ReminderCode = me.ReminderCode, 
                    ReminderDescription = me.ReminderDescription
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Reminder tidak bisa disimpan.";
                MyLogger.Info("Error on Reminder saving: " + Ex.Message);
            }

            return Json(result);
        }

        public JsonResult Delete(GnMstReminder model)
        {
            object returnObj = null;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.GnMstReminders.Find(CompanyCode, model.ReminderKey);
                    if (me != null)
                    {
                        ctx.GnMstReminders.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Reminder berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Reminder, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Reminder , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
    }
}