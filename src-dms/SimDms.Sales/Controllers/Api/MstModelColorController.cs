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
    public class MstModelColorController : BaseController 
    {

        public JsonResult ModelCode(omMstModelColourBrowse model)
        {
            if ((model.SalesModelCode != null || model.SalesModelCode == model.SalesModelCodeTo) && model.SalesModelCodeTo == null)
            {
                var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);
                return Json(new { success = record != null, data = record });
            }
            else
            {
                var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCodeTo);
                return Json(new { success = record != null, data = record });
            }
        }

        public JsonResult ColourCode(omMstModelColourBrowse model)
        {
            var RefferenceType = "COLO";

            if ((model.ColourCode != null || model.ColourCode == model.ColourCodeTo) && model.ColourCodeTo == null)
            {
                var record = ctx.MstRefferences.Find(CompanyCode, RefferenceType, model.ColourCode);
                return Json(new { success = record != null, data = record });
            }
            else
            {
                var record = ctx.MstRefferences.Find(CompanyCode, RefferenceType, model.ColourCodeTo);
                return Json(new { success = record != null, data = record });
            }
        }

        public JsonResult ModelColorDetailsLoad(string SalesModelCode)
        {
            return Json(ctx.Database.SqlQuery<omMstModelColourView>("sp_omMstModelColour '" + CompanyCode + "','" + SalesModelCode + "'").AsQueryable());
        }

        public JsonResult ModelColourCode(MstModelColour model)
        {
            var record = ctx.MstModelColours.Find(CompanyCode, model.SalesModelCode, model.ColourCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult Save2(MstModelColour model)
        {
            string msg = "";
            var record = ctx.MstModelColours.Find(CompanyCode, model.SalesModelCode, model.ColourCode);

            if (record == null)
            {
                record = new MstModelColour
                {
                    CompanyCode = CompanyCode,
                    SalesModelCode = model.SalesModelCode,
                    ColourCode = model.ColourCode,
                    Remark = model.Remark == null ? "" : model.Remark,
                    Status = model.Status == "True" ? "1" : "0",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    isLocked = false,
                    LockingBy = "",
                    LockingDate = Convert.ToDateTime("1900-01-01")
                };

                ctx.MstModelColours.Add(record);
                msg = "Model Colour details added...";
            }
            else
            {
                ctx.MstModelColours.Attach(record);
                msg = "Model Colour updated";
            }

            record.ColourCode = model.ColourCode;
            record.Remark = model.Remark == null ? "" : model.Remark;
            record.Status = model.Status == "True" ? "1" : "0";
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.isLocked = false;
            record.LockingBy = "";
            record.LockingDate = Convert.ToDateTime("1900-01-01");

            try
            {
                ctx.SaveChanges();

                var records = ctx.Database.SqlQuery<omMstModelColourView>("sp_omMstModelColour '" + CompanyCode + "','" + model.SalesModelCode + "'").AsQueryable();

                return Json(new { success = true, message = msg, data = record, result = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(MstModelColour model)
        {
            var record = ctx.MstModelColours.Where(a=> a.SalesModelCode == model.SalesModelCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.Database.ExecuteSqlCommand("DELETE omMstMOdelColour WHERE CompanyCode='" + CompanyCode + "' and SalesModelCode='" + model.SalesModelCode + "'");
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

        public JsonResult Delete2(MstModelColour model)
        {
            var record = ctx.MstModelColours.Find(CompanyCode, model.SalesModelCode, model.ColourCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstModelColours.Remove(record);
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

    }

}
