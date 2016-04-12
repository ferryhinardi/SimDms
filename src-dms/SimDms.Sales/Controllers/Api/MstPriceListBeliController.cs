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
    public class MstPriceListBeliController : BaseController
    {
        public JsonResult SupplierCode(MstPriceListBeli model)
        {
            var record = ctx.Supplier.Find(CompanyCode, model.SupplierCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult ModelCode(MstPriceListBeli model)
        {
            var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult PriceListBeli(MstPriceListBeli model)
        {
            var record = ctx.MstPriceListBeli.Find(CompanyCode, BranchCode, model.SupplierCode, model.SalesModelCode, model.SalesModelYear);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult txtTotal_Validated(MstPriceListBeli model)
        {
            var SupplierProfitCenter = ctx.SupplierProfitCenter.Find(CompanyCode, BranchCode, model.SupplierCode, "100");
            var ptPPn = (SupplierProfitCenter == null) ? "NON" : SupplierProfitCenter.TaxCode;
            var MstTax = ctx.Taxs.Find(CompanyCode, ptPPn);
            var ppPPn = (MstTax == null) ? 0 : MstTax.TaxPct;
            var MstModel = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);
            var ppPPnBM = (MstModel == null) ? 0 : MstModel.PpnBmPctBuy;
            
            decimal ptDPP = Convert.ToDecimal(model.Total / ((100 + ppPPn + ppPPnBM) / 100));
            decimal tPPn = Convert.ToDecimal(ptDPP * (ppPPn / 100));
            decimal tPPnBM = Convert.ToDecimal(ptDPP * (ppPPnBM / 100));

            return Json(new { success = true, DPP = Math.Round(ptDPP), PPn = Math.Round(tPPn), PPnBM = Math.Round(tPPnBM) });
        }

        public JsonResult Save(MstPriceListBeli model)
        {
            string msg = "";
            var record = ctx.MstPriceListBeli.Find(CompanyCode, BranchCode, model.SupplierCode, model.SalesModelCode, model.SalesModelYear);

            if (record == null)
            {
                record = new MstPriceListBeli
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    SupplierCode = model.SupplierCode,
                    SalesModelCode = model.SalesModelCode,
                    SalesModelYear = model.SalesModelYear,
                    PPnBMPaid = model.PPnBMPaid,
                    DPP = model.DPP,
                    PPn = model.PPn,
                    PPnBM = model.PPnBM == null ? 0 : model.PPnBM,
                    Total = model.Total,
                    Remark = model.Remark,
                    Status = model.Status == "True" ? "1" : "0",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    isLocked = false,
                    LockingBy = "",
                    LockingDate = Convert.ToDateTime("1900-01-01"),
                    FromDate = Convert.ToDateTime("1900-01-01"),
                    ToDate = Convert.ToDateTime("1900-01-01")

                };
                ctx.MstPriceListBeli.Add(record);
                msg = "New PriceList Buy added...";
            }
            else
            {
                record.DPP = model.DPP;
                record.PPn = model.PPn;
                record.PPnBM = model.PPnBM == null ? 0 : model.PPnBM;
                record.Total = model.Total;
                record.Remark = model.Remark;
                record.Status = model.Status == "True" ? "1" : "0";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctx.CurrentTime;
                record.isLocked = false;
                record.LockingBy = "";
                record.LockingDate = Convert.ToDateTime("1900-01-01");
                record.FromDate = Convert.ToDateTime("1900-01-01");
                record.ToDate = Convert.ToDateTime("1900-01-01");
                msg = "PriceList Buy updated";
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

        public JsonResult Delete(MstPriceListBeli model)
        {
            var record = ctx.MstPriceListBeli.Find(CompanyCode, BranchCode, model.SupplierCode, model.SalesModelCode, model.SalesModelYear);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstPriceListBeli.Remove(record);
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
