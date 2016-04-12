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
    public class MstModelPerlengkapanController : BaseController
    {
        public JsonResult ModelCode(MstModelPerlengkapan model)
        {
            var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult PerlengkapanCode(PerlengkapanCodeLookup model)
        {
            if ((model.PerlengkapanCode != null || model.PerlengkapanCode == model.PerlengkapanCodeTo) && model.PerlengkapanCodeTo == null)
            {
                var record = ctx.MstPerlengkapan.Find(CompanyCode, BranchCode, model.PerlengkapanCode);
                return Json(new { success = record != null, data = record });
            }
            else
            {
                var record = ctx.MstPerlengkapan.Find(CompanyCode, BranchCode, model.PerlengkapanCodeTo);
                return Json(new { success = record != null, data = record });
            }
        }

        public JsonResult ModelPerlengkapanCode(MstModelPerlengkapan model)
        {
            var record = ctx.MstModelPerlengkapan.Find(CompanyCode, BranchCode, model.SalesModelCode, model.PerlengkapanCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult ModelPerlengkapanDetailsLoad(string SalesModelCode)
        {
            return Json(ctx.Database.SqlQuery<MstPerlengkapanView>("sp_omMstModelPerlengkapan '" + CompanyCode + "','" + BranchCode + "','" + SalesModelCode + "'").AsQueryable());
        }

        public JsonResult Save2(MstModelPerlengkapan model)
        {
            string msg = "";
            var record = ctx.MstModelPerlengkapan.Find(CompanyCode, BranchCode, model.SalesModelCode, model.PerlengkapanCode);

            if (record == null)
            {
                record = new MstModelPerlengkapan
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    SalesModelCode = model.SalesModelCode,
                    PerlengkapanCode = model.PerlengkapanCode,
                    Quantity = model.Quantity == null ? 0 : model.Quantity,
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

                ctx.MstModelPerlengkapan.Add(record);
                msg = "Model Year details added...";
            }
            else
            {
                ctx.MstModelPerlengkapan.Attach(record);
                msg = "Model Year updated";
            }

            record.PerlengkapanCode = model.PerlengkapanCode;
            record.Quantity = model.Quantity == null ? 0 : model.Quantity;
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

                var records = ctx.Database.SqlQuery<MstPerlengkapanView>("sp_omMstModelPerlengkapan '" + CompanyCode + "','" + BranchCode + "','" + model.SalesModelCode + "'").AsQueryable();

                return Json(new { success = true, message = msg, data = record, result = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(MstModelPerlengkapan model)
        {
            var record = ctx.MstModelPerlengkapan.Where(a => a.SalesModelCode == model.SalesModelCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.Database.ExecuteSqlCommand("DELETE omMstModelPerlengkapan WHERE CompanyCode='" + CompanyCode + "' and SalesModelCode='" + model.SalesModelCode + "'");
                //ctx.MstModelColours.Remove(record);
            }

            try
            {
                ctx.SaveChanges();

                var records = ctx.Database.SqlQuery<MstPerlengkapanView>("sp_omMstModelPerlengkapan '" + CompanyCode + "','" + BranchCode + "','" + model.SalesModelCode + "'").AsQueryable();

                return Json(new { success = true, data = record, result = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete2(MstModelPerlengkapan model)
        {
            var record = ctx.MstModelPerlengkapan.Find(CompanyCode, BranchCode, model.SalesModelCode, model.PerlengkapanCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstModelPerlengkapan.Remove(record);
            }

            try
            {
                ctx.SaveChanges();

                var records = ctx.Database.SqlQuery<MstPerlengkapanView>("sp_omMstModelPerlengkapan '" + CompanyCode + "','" + BranchCode + "','" + model.SalesModelCode + "'").AsQueryable();

                return Json(new { success = true, data = record, result = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }

}
