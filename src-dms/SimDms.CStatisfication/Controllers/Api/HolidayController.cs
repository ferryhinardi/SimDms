using SimDms.CStatisfication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers.Api
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
            var record = ctx.Holidays.Find(model.CompanyCode, model.HolidayYear, model.HolidayCode);
            if (record == null)
            {
                record = model;
                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
                ctx.Holidays.Add(record);
            }
            record.HolidayDesc = model.HolidayDesc;
            record.DateFrom = model.DateFrom;
            record.DateTo = model.DateTo;
            record.IsHoliday = model.IsHoliday;
            record.ReligionCode = model.ReligionCode ?? "";
            record.HolidayCode = model.HolidayCode;
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
