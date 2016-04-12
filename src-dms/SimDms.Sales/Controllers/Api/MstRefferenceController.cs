using SimDms.Sales.Models;
using SimDms.Sales.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Common.Models;
using SimDms.Common;

namespace SimDms.Sales.Controllers.Api
{
    public class MstRefferenceController : BaseController
    {
        public JsonResult tiperef(MstRefferenceView model)
        {
            var record = ctx.MstRefferences.Where(p => p.CompanyCode == CompanyCode && p.RefferenceType == model.RefferenceType).FirstOrDefault();

            return Json(new { success = record != null, data = record });
        }

        public JsonResult koderef(MstRefferenceView model)
        {
            if ((model.RefferenceCode != null || model.RefferenceCode == model.RefferenceCodeTo) && model.RefferenceCodeTo == null)
            {
                var record = ctx.MstRefferences.Where(p => p.CompanyCode == CompanyCode && p.RefferenceType == model.RefferenceType && p.RefferenceCode == model.RefferenceCode).FirstOrDefault();
                return Json(new { success = record != null, data = record });
            }
            else
            {
                var record = ctx.MstRefferences.Where(p => p.CompanyCode == CompanyCode && p.RefferenceType == model.RefferenceType && p.RefferenceCode == model.RefferenceCodeTo).FirstOrDefault();
                return Json(new { success = record != null, data = record });
            }

        }

        public JsonResult Save(MstRefference model)
        {
            string msg = "";
            var record = ctx.MstRefferences.Find(CompanyCode, model.RefferenceType, model.RefferenceCode);

            if (record == null)
            {
                record = new MstRefference
                {
                    CompanyCode = CompanyCode,
                    RefferenceType = model.RefferenceType,
                    RefferenceCode = model.RefferenceCode,
                    RefferenceDesc1 = model.RefferenceDesc1,
                    RefferenceDesc2 = model.RefferenceDesc2 == null ? "" : model.RefferenceDesc2,
                    Remark = model.Remark == null ? "" : model.Remark,
                    Status = model.Status == "True" ? "1" : "0",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    IsLocked = false,
                    LockedBy = "",
                    LockedDate = Convert.ToDateTime("1900-01-01")
                    //RefferenceDesc = ""
                };
                ctx.MstRefferences.Add(record);
                msg = "New Refference added...";
            }
            else
            {
                record.RefferenceDesc1 = model.RefferenceDesc1;
                record.RefferenceDesc2 = model.RefferenceDesc2 == null ? "" : model.RefferenceDesc2;
                record.Remark = model.Remark == null ? "" : model.Remark;
                record.Status = model.Status;
                record.Status = model.Status == "True" ? "1" : "0";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctx.CurrentTime;
                record.IsLocked = false;
                record.LockedBy = model.IsLocked == true ? CurrentUser.UserId : "";
                record.LockedDate = model.IsLocked == true ? ctx.CurrentTime : Convert.ToDateTime("1900-01-01");
                //record.RefferenceDesc = "";
                //ctx.MstRefferences.Add(record);
                msg = "Refference updated";
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(MstRefference model)
        {
            var record = ctx.MstRefferences.Find(CompanyCode, model.RefferenceType, model.RefferenceCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstRefferences.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
