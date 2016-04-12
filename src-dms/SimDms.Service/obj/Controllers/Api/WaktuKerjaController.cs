using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class WaktuKerjaController : BaseController
    {
        //
        // GET: /WaktuKerja/

        public JsonResult Default()
        {
            return Json(new
            {

                CompanyCode = CompanyCode,
                ProductType = ProductType,
                BeginWorkTime = "00:00", 
                EndWorkTime = "00:00",
                BeginLunchTime ="00:00",
                EndLunchTime = "00:00",
            });
        }

        private void InitFirst()
        {
            string[] strDay = new string[] { "Minggu", "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu" };

            for (int i = 1; i <= 7; i++)
            {
                var record = new SvMstWaktuKerja
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    DayCode = i.ToString(),
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    Description = strDay[i - 1].ToString(),
                    BeginWorkTime = new DateTime(1900,1,1,0,0,0),
                    EndWorkTime = new DateTime(1900, 1, 1, 0, 0, 0),
                    BeginLunchTime = new DateTime(1900, 1, 1, 0, 0, 0),
                    EndLunchTime = new DateTime(1900, 1, 1, 0, 0, 0),
                    IsActive = true,
                    LastupdateBy = CurrentUser.UserId,
                    LastupdateDate = DateTime.Now,
                    IsLocked = false,
                    LockingBy = CurrentUser.UserId,
                    LockingDate = DateTime.Now
                };

                ctx.SvMstWaktuKerjas.Add(record);

                ctx.SaveChanges();
            }
        }

        public JsonResult Save(SvMstWaktuKerja model)
        {
            var record = ctx.SvMstWaktuKerjas.Find(CompanyCode, BranchCode, model.DayCode);
                       
            if (record == null)
            {
                record = new SvMstWaktuKerja
                {
                    CompanyCode=CompanyCode, 
                    BranchCode=BranchCode, 
                    DayCode=model.DayCode,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,

                };
                ctx.SvMstWaktuKerjas.Add(record);
            }

            record.Description = model.Description;
            record.BeginWorkTime = model.BeginWorkTime;
            record.EndWorkTime = model.EndWorkTime;
            record.BeginLunchTime = model.BeginLunchTime;
            record.EndLunchTime = model.EndLunchTime;
            record.IsActive = model.IsActive;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;

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

        public JsonResult WorkingTime()
        {
            var queryable = ctx.SvMstWaktuKerjaViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).OrderBy(x => x.DayCode);
            if (queryable.ToList().Count <= 0)
            {
                InitFirst();
            }
            return Json(new { success = true, data = queryable });
        }
    }
}