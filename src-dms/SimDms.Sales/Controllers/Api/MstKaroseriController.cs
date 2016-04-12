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
    public class MstKaroseriController : BaseController
    {
        public JsonResult SupplierCode(KaroseriLookup model)
        {
            if ((model.SupplierCode != null || model.SupplierCode == model.SupplierCodeTo) && model.SupplierCodeTo == null)
            {
                var record = ctx.Supplier.Find(CompanyCode, model.SupplierCode);
                return Json(new { success = record != null, data = record });
            }
            else
            {
                var record = ctx.Supplier.Find(CompanyCode, model.SupplierCodeTo);
                return Json(new { success = record != null, data = record });
            }
        }

        public JsonResult ModelCodeBaru(KaroseriLookup model)
        {
            if ((model.SalesModelCodeBaru != null || model.SalesModelCodeBaru == model.SalesModelCodeBaruTo) && model.SalesModelCodeBaruTo == null)
            {
                var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCodeBaru);
                return Json(new { success = record != null, data = record });
            }
            else
            {
                var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCodeBaruTo);
                return Json(new { success = record != null, data = record });
            }
        }

        public JsonResult ModelCode(MstKaroseri model)
        {
            var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult KaroseriView(MstKaroseri model)
        {
            var record = ctx.KaroseriBrowseView.Find(CompanyCode, BranchCode, model.SupplierCode, model.SalesModelCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult Save(MstKaroseri model)
        {
            string msg = "";
            var record = ctx.MstKaroseri.Find(CompanyCode, BranchCode, model.SupplierCode, model.SalesModelCode);

            if (record == null)
            {
                record = new MstKaroseri
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    SupplierCode = model.SupplierCode,
                    SalesModelCode = model.SalesModelCode,
                    SalesModelCodeNew = model.SalesModelCodeNew,
                    DPPMaterial = model.DPPMaterial,
                    DPPFee = model.DPPFee == null ? 0 : model.DPPFee,
                    DPPOthers = model.DPPOthers == null ? 0 : model.DPPOthers,
                    PPn = model.PPn,
                    Total = model.Total,
                    Remark = model.Remark == null ? "" : model.Remark,
                    Status = model.Status == "True" ? "1" : "0",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    isLocked = false,
                    LockingBy = "",
                    LockingDate = Convert.ToDateTime("1900-01-01")

                };
                ctx.MstKaroseri.Add(record);
                msg = "New Karoseri added...";
            }
            else
            {
                record.SalesModelCodeNew = model.SalesModelCodeNew;
                record.DPPMaterial = model.DPPMaterial;
                record.DPPFee = model.DPPFee == null ? 0 : model.DPPFee;
                record.DPPOthers = model.DPPOthers == null ? 0 : model.DPPOthers;
                record.PPn = model.PPn;
                record.Total = model.Total;
                record.Remark = model.Remark == null ? "" : model.Remark;
                record.Status = model.Status == "True" ? "1" : "0";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctx.CurrentTime;
                record.isLocked = false;
                record.LockingBy = "";
                record.LockingDate = Convert.ToDateTime("1900-01-01");
                msg = "Karoseri updated";
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

        public JsonResult Delete(MstKaroseri model)
        {
            var record = ctx.MstKaroseri.Find(CompanyCode, BranchCode, model.SupplierCode, model.SalesModelCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstKaroseri.Remove(record);
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
