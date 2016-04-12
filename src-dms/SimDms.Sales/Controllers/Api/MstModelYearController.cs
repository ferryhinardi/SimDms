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
    public class MstModelYearController : BaseController
    {
        public JsonResult ModelCode(MstModelYear model)
        {
            var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult ModelYear(MstModelYear model)
        {
            var record = ctx.MstModelYear.Find(CompanyCode, model.SalesModelCode, model.SalesModelYear);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult ModelYearDetailsLoad(string SalesModelCode)
        {
            return Json(ctx.Database.SqlQuery<MstModelYearView>("sp_omMstModelYear '" + CompanyCode + "','" + SalesModelCode + "'").AsQueryable());
        }

        public JsonResult Save2(MstModelYear model)
        {
            string msg = "";
            var record = ctx.MstModelYear.Find(CompanyCode, model.SalesModelCode, model.SalesModelYear);

            if (record == null)
            {
                record = new MstModelYear
                {
                    CompanyCode = CompanyCode,
                    SalesModelCode = model.SalesModelCode,
                    SalesModelYear = model.SalesModelYear,
                    SalesModelDesc = model.SalesModelDesc,
                    ChassisCode = model.ChassisCode,
                    Remark = model.Remark == null ? "" : model.Remark,
                    Status = model.Status == "True" ? "1" : "0",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now,
                    isLocked = false,
                    LockingBy = "",
                    LockingDate = Convert.ToDateTime("1900-01-01")
                };

                ctx.MstModelYear.Add(record);
                msg = "Model Year details added...";
            }
            else
            {
                ctx.MstModelYear.Attach(record);
                msg = "Model Year updated";
            }

            record.SalesModelYear = model.SalesModelYear;
            record.SalesModelDesc = model.SalesModelDesc;
            record.ChassisCode = model.ChassisCode;
            record.Remark = model.Remark == null ? "" : model.Remark;
            record.Status = model.Status == "True" ? "1" : "0";
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.isLocked = false;
            record.LockingBy = "";
            record.LockingDate = Convert.ToDateTime("1900-01-01");
            msg = "Model Year updated";

            try
            {
                ctx.SaveChanges();

                var records = ctx.Database.SqlQuery<MstModelYearView>("sp_omMstModelYear '" + CompanyCode + "','" + model.SalesModelCode + "'").AsQueryable();

                return Json(new { success = true, message = msg, data = record, result = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(MstModelYear model)
        {
            var record = ctx.MstModelYear.Where(a => a.SalesModelCode == model.SalesModelCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.Database.ExecuteSqlCommand("DELETE omMstModelYear WHERE CompanyCode='" + CompanyCode + "' and SalesModelCode='" + model.SalesModelCode + "'");
                //ctx.MstModelColours.Remove(record);
            }

            try
            {
                ctx.SaveChanges();

                var records = ctx.Database.SqlQuery<omMstModelColourView>("sp_omMstModelColour '" + CompanyCode + "','" + model.SalesModelCode + "'").AsQueryable();

                return Json(new { success = true, data = record, result = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete2(MstModelYear model)
        {
            var record = ctx.MstModelYear.Find(CompanyCode, model.SalesModelCode, model.SalesModelYear);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstModelYear.Remove(record);
            }

            try
            {
                ctx.SaveChanges();

                var records = ctx.Database.SqlQuery<MstModelYearView>("sp_omMstModelYear '" + CompanyCode + "','" + model.SalesModelCode + "'").AsQueryable();

                return Json(new { success = true, data = record, result = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }

}
