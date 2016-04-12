using SimDms.CStatisfication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class CustHolidayController : BaseController
    {
        public JsonResult Save(CustHoliday model)
        {
            //var record = ctx.CustHolidays.Find(model.CompanyCode, model.CustomerCode, model.PeriodYear, model.GiftSeq);

            var length = ctx.CustHolidays.Where(p => p.CompanyCode == model.CompanyCode &&
                                p.CustomerCode == model.CustomerCode &&
                                p.PeriodYear == model.PeriodYear
                            ).Count();

            var record = ctx.CustHolidays.Where(p => p.CompanyCode == model.CompanyCode &&
                                p.CustomerCode == model.CustomerCode &&
                                p.PeriodYear == model.PeriodYear &&
                                p.HolidayCode == model.HolidayCode
                            ).FirstOrDefault();

            if (record == null)
            {
                record = model;
                record.PeriodYear = DateTime.Now.Year;
                record.GiftSeq = length + 1;
                ctx.CustHolidays.Add(record);
            }

            record.ReligionCode = model.ReligionCode;
            record.HolidayCode = model.HolidayCode;
            record.IsGiftCard = model.IsGiftCard;
            record.IsGiftLetter = model.IsGiftLetter;
            record.IsGiftSms = model.IsGiftSms;
            record.IsGiftSouvenir = model.IsGiftSouvenir;
            record.SouvenirSent = model.SouvenirSent;
            record.SouvenirReceived = model.SouvenirReceived;
            record.Comment = model.Comment;
            record.Additional = model.Additional;
            record.Status = ((model.SouvenirReceived != null) ? 2 : 0);

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = new { PeriodYear = model.PeriodYear, GiftSeq = model.GiftSeq } });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Get(CustHoliday model)
        {
            var record = ctx.CustHolidays.Find(model.CompanyCode, model.CustomerCode, model.PeriodYear, model.GiftSeq);
            if (record != null)
            {
                return Json(new { success = true, data = record, isNew = false });
            }
            else
            {
                var data = ctx.Customers.Find(model.CompanyCode, model.CustomerCode);
                return Json(new { success = true, data = data, isNew = true }); ;
            }
        }

        public JsonResult Delete(CustHoliday model)
        {
            var record = ctx.CustHolidays.Find(model.CompanyCode, model.CustomerCode, model.PeriodYear, model.GiftSeq);
            if (record != null)
            {
                ctx.CustHolidays.Remove(record);

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

        public JsonResult GetData(CustHoliday model)
        {
            var record = ctx.CustHolidays.Where(p =>
                    p.CompanyCode == model.CompanyCode &&
                    p.CustomerCode == model.CustomerCode &&
                    p.PeriodYear == model.PeriodYear &&
                    p.HolidayCode == model.HolidayCode
                ).FirstOrDefault();
            if (record != null)
            {
                return Json(new { success = true, data = record, isNew = false });
            }
            else
            {
                return Json(new { success = false }); ;
            }
        }
    }
}
