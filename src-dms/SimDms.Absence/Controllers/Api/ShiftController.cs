using SimDms.Absence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class ShiftController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                HolidayYear = DateTime.Now.Year,
                OnDutyTime = "08:00",
                OffDutyTime = "17:00",
                OnRestTime = "12:00",
                OffRestTime = "13:00",
                IsActive = true,
            });
        }

        public JsonResult Save(Shift model)
        {
            var record = ctx.Shifts.Find(CompanyCode, model.ShiftCode);
            if (record == null)
            {
                record = model;
                record.CompanyCode = CompanyCode;
                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
                ctx.Shifts.Add(record);
            }
            record.ShiftName = model.ShiftName;
            record.OnDutyTime = model.OnDutyTime;
            record.OffDutyTime = model.OffDutyTime;
            record.OnRestTime = model.OnRestTime;
            record.OffRestTime = model.OffRestTime;
            record.IsActive = model.IsActive;
            record.WorkingHour = model.WorkingHour;
            record.UpdatedBy = CurrentUser.UserId;
            record.UpdatedDate = DateTime.Now;

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

        public JsonResult Delete(Shift model)
        {
            var record = ctx.Shifts.Find(CompanyCode, model.ShiftCode);
            if (record != null)
            {
                ctx.Shifts.Remove(record);

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
            return Json(new { success = false, message = "data not valid" });
        }
    }
}
