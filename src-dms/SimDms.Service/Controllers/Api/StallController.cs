using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class StallController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                UserId = CurrentUser.UserId
            });
        }

        public JsonResult Save(svMstStall model)
        {
            var record = ctx.svMstStalls.Find(CompanyCode, ProductType, model.StallCode, BranchCode);
            if (record == null)
            {
                record = new svMstStall
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ProductType = ProductType,
                    StallCode = model.StallCode,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                };
                ctx.svMstStalls.Add(record);
            }

            record.Description = model.Description;
            record.HaveLift = model.HaveLift;
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

        public JsonResult deleteData(svMstStall model)
        {

            var record = ctx.svMstStalls.Find(CompanyCode, ProductType, model.StallCode, BranchCode);
            if (record != null)
            {
                ctx.svMstStalls.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Proses Delete Stall Gagal!!!" });
            }


        }

        public JsonResult Get(svMstStall model)
        {
            var record = ctx.svMstStalls.Find(CompanyCode, ProductType, model.StallCode, BranchCode);
            if (record != null)
            {
                return Json(new { success = true, data = record });
            }
            else
            {
                return Json(new { success = false });
            }
        }

    }
}
