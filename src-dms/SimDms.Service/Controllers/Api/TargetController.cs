using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace SimDms.Service.Controllers.Api
{
    public class TargetController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {

                TotalUnitService = "0.00",
                TotalStall = "0",
                TotalMechanic = "0.00",
                TotalWorkingDays = "0.00",
                ProductivityMechanic = "0.00",
                ProductivityStall = "0.00",
                TotalLift = "0",
                HourlyLaborRate = "0.00",
                OverheadCost = "0.00",
                ServiceAmount = "0.00",
                SMRTarget = "0.00",
                DasMonthTarget = "0",
                DasDailyTarget = "0",
                tanda = "                   /",

            });
        }

        public JsonResult Save(svMstTarget model, string ProductivityMechanic, string ProductivityStall)
        {
          
            var record = ctx.svMstTargets.Find(CompanyCode, BranchCode, ProductType, model.PeriodYear, model.PeriodMonth);
            if (record == null)
            {
                record = new svMstTarget
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ProductType = ProductType,
                    PeriodYear = model.PeriodYear,
                    PeriodMonth = model.PeriodMonth,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,

                };
                ctx.svMstTargets.Add(record);
            } 
            record.ProductivityMechanic = decimal.Parse(ProductivityMechanic, CultureInfo.InvariantCulture);
            record.ProductivityStall = decimal.Parse(ProductivityStall, CultureInfo.InvariantCulture);
            record.TotalUnitService = model.TotalUnitService;
            record.TotalWorkingDays = model.TotalWorkingDays;
            record.TotalMechanic = model.TotalMechanic;
            record.TotalStall = model.TotalStall;
            record.TotalLift = model.TotalLift;
            record.HourlyLaborRate = model.HourlyLaborRate;
            record.OverheadCost = model.OverheadCost;
            record.ServiceAmount = model.ServiceAmount;
            record.IsActive = true;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = "";
            record.DasDailyTarget = model.DasDailyTarget;
            record.DasMonthTarget = model.DasMonthTarget;
            record.SMRTarget = model.SMRTarget;

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

        public JsonResult deleteData(svMstTarget model)
        {

            var record = ctx.svMstTargets.Find(CompanyCode, BranchCode, ProductType, model.PeriodYear, model.PeriodMonth);
            if (record != null)
            {
                ctx.svMstTargets.Remove(record);
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
