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
    public class CalenderController : BaseController
    {
        public JsonResult CalenderLoad(string ProfitCenterCode)
        {
            return Json(ctx.Database.SqlQuery<CalenderView>("select CONVERT(VARCHAR(50),CalendarDate,106) CalendarDate, CalendarDescription from GnMstCalendar where ProfitCenterCode ='" + ProfitCenterCode + "'").AsQueryable());
        }

        [HttpPost]
        public JsonResult Save(GnMstCalender model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.Calenders.Find(CompanyCode, BranchCode, model.ProfitCenterCode, model.CalendarDate);

            if (me == null)
            {
                me = new GnMstCalender();
                me.CreatedBy = userID;
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                ctx.Calenders.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.ProfitCenterCode = model.ProfitCenterCode;
            me.CalendarDate = model.CalendarDate; 
            me.CalendarDescription = model.CalendarDescription;
            me.CalendarType = model.CalendarType; 

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Kalender berhasil disimpan.";
                result.data = new
                {
                    CalendarDate = me.CalendarDate,
                    CalendarDescription = me.CalendarDescription
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Kalender tidak bisa disimpan.";
                MyLogger.Info("Error on calender saving: " + Ex.Message);
            }

            return Json(result);
        }

        public JsonResult Delete(GnMstCalender model)
        {
            object returnObj = null;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.Calenders.Find(CompanyCode, BranchCode, model.ProfitCenterCode, model.CalendarDate);
                    if (me != null)
                    {
                        ctx.Calenders.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data kalender berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete kalender, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete kalender , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
    }
}