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
    public class ReportSettingController : BaseController  
    {
        public JsonResult DetailLoad(string ReportID) 
        {
            return Json(ctx.Database.SqlQuery<SysReportSettings>("select * from SysReportSettings a where ReportID ='" + ReportID + "'").AsQueryable());
        }

        [HttpPost]
        public JsonResult Save(SysReportSettings model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;
            var me = ctx.SysReportSettings.Find(model.ReportID, model.Keyword);

            if (me == null)
            {
                me = new SysReportSettings();
                ctx.SysReportSettings.Add(me);
            }
            me.ReportID = model.ReportID;
            me.Keyword = model.Keyword;
            me.IsVisible = model.IsVisible;
            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Security Report Setting berhasil disimpan.";
                var records = ctx.Database.SqlQuery<SysReportSettings>("select ReportID, Keyword, IsVisible from SysReportSettings where ReportID='" + model.ReportID + "'").AsQueryable();
                result.data = records;
            }
            catch (Exception Ex)
            {
                result.message = "Data Security Report Setting tidak bisa disimpan.";
                MyLogger.Info("Error on Security Report Setting saving: " + Ex.Message);
            }

            return Json(result);
        }

        public JsonResult Delete(SysReportSettings model)
        {
            object returnObj = null;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.SysReportSettings.Find(model.ReportID, model.Keyword);
                    if (me != null)
                    {
                        ctx.SysReportSettings.Remove(me);
                        ctx.SaveChanges();
                        var records = ctx.Database.SqlQuery<SysReportSettings>("select ReportID, Keyword, IsVisible from SysReportSettings where ReportID='" + model.ReportID + "'").AsQueryable();
                        returnObj = new { success = true, message = "Data Security Report Setting berhasil di delete.", result = records };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Security Report Setting, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Security Report Setting , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
    }
}