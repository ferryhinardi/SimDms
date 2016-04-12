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
    public class MstBBNKIRController : BaseController
    {
        public JsonResult CekData(MstBBNKIR model)
        {
            var record = ctx.MstBBNKIR.Find(CompanyCode, BranchCode, model.SupplierCode, model.CityCode, model.SalesModelCode, model.SalesModelYear);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult SupplierCode(MstBBNKIR model)
        {
            var record = ctx.Supplier.Find(CompanyCode, model.SupplierCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult CityCode(BBNKIRLookup model)
        {
            var CodeID = "CITY";
            if ((model.CityCode != null || model.CityCode == model.CityCodeTo) && model.CityCodeTo == null)
            {
                var record = ctx.LookUpDtls.Find(CompanyCode, CodeID, model.CityCode);
                return Json(new { success = record != null, data = record });
            }
            else
            {
                var record = ctx.LookUpDtls.Find(CompanyCode, CodeID, model.CityCodeTo);
                return Json(new { success = record != null, data = record });
            }
        }

        public JsonResult ModelCode(MstBBNKIR model)
        {
            var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult ModelYear(MstBBNKIR model)
        {
            var record = ctx.MstModelYear.Find(CompanyCode, model.SalesModelCode, model.SalesModelYear);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult Save(MstBBNKIR model)
        {
            string msg = "";
            var record = ctx.MstBBNKIR.Find(CompanyCode, BranchCode, model.SupplierCode, model.CityCode, model.SalesModelCode, model.SalesModelYear);

            if (record == null)
            {
                record = new MstBBNKIR
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    SupplierCode = model.SupplierCode,
                    CityCode = model.CityCode,
                    SalesModelCode = model.SalesModelCode,
                    SalesModelYear = model.SalesModelYear,
                    BBN = model.BBN,
                    KIR = model.KIR,
                    Remark = model.Remark == null ? "" : model.Remark ,
                    Status = model.Status.ToLower() == "true" ? "1" : "0",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    isLocked = false,
                    LockingBy = "",
                    LockingDate = Convert.ToDateTime("1900-01-01")
                };
                ctx.MstBBNKIR.Add(record);
                msg = "New BBN KIR added...";
            }
            else
            {
                record.BBN = model.BBN;
                record.KIR = model.KIR;
                record.Remark = model.Remark == null ? "" : model.Remark;
                record.Status = model.Status.ToLower() == "true" ? "1" : "0";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctx.CurrentTime;
                record.isLocked = false;
                record.LockingBy = "";
                record.LockingDate = Convert.ToDateTime("1900-01-01");
                msg = "Perlnegkapan updated";
                msg = "BBN KIR updated";
            }



            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(MstBBNKIR model)
        {
            var record = ctx.MstBBNKIR.Find(CompanyCode, BranchCode, model.SupplierCode, model.CityCode, model.SalesModelCode, model.SalesModelYear);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstBBNKIR.Remove(record);
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
