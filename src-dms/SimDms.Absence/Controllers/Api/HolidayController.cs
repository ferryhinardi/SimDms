using SimDms.Absence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class HolidayController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                HolidayYear = DateTime.Now.Year
            });
        }

        public JsonResult Save(Holiday model)
        {
            var userId = CurrentUser.UserId;
            var curTime = DateTime.Now;
            var record = ctx.Holidays.Find(model.CompanyCode, model.HolidayYear, model.HolidayCode);
            if (record == null)
            {
                record = model;
                record.CreatedBy = userId;
                record.CreatedDate = curTime;
                ctx.Holidays.Add(record);
            }
            record.HolidayDesc = model.HolidayDesc;
            record.DateFrom = model.DateFrom;
            record.DateTo = model.DateTo;
            record.IsHoliday = model.IsHoliday;
            record.ReligionCode = model.ReligionCode ?? "";
            record.HolidayCode = model.HolidayCode;
            record.UpdatedBy = userId;
            record.UpdatedDate = curTime;

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

        public JsonResult Delete(Holiday model)
        {
            var record = ctx.Holidays.Find(model.CompanyCode, model.HolidayYear, model.HolidayCode);
            if (record != null)
            {
                ctx.Holidays.Remove(record);

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
