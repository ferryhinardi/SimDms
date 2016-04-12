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
    public class MstPriceListJualController : BaseController
    {
        public JsonResult GroupPriceCode(PriceListJualLookup model)
        {
            var RefferenceType = "GRPR";

            if ((model.GroupPriceCode != null || model.GroupPriceCode == model.GroupPriceCodeTo) && model.GroupPriceCodeTo == null)
            {
                var record = ctx.MstRefferences.Find(CompanyCode, RefferenceType, model.GroupPriceCode);
                return Json(new { success = record != null, data = record });
            }
            else
            {
                var record = ctx.MstRefferences.Find(CompanyCode, RefferenceType, model.GroupPriceCodeTo);
                return Json(new { success = record != null, data = record });
            }
        }

        public JsonResult ModelCode(OmMstPricelistSell model)
        {
            var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult TaxCode(OmMstPricelistSell model)
        {
            var record = ctx.Taxs.Find(CompanyCode, model.TaxCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult CekData(OmMstPricelistSell model)
        {
            var record = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode, model.GroupPriceCode, model.SalesModelCode, model.SalesModelYear);

            //if (record != null)
            //{
            //    //var data = ctx.Database.SqlQuery<OmMstPricelistSell>("sp_MstPriceListJualView '" + CompanyCode + "','" + BranchCode + "','" + model.GroupPriceCode + "','" + model.SalesModelCode + "','" + model.SalesModelYear + "'").AsQueryable();
            //    return Json(new { success = true, data = record });
            //    //return Json(record);
            //}

            return Json(new { success = record != null, data = record});
        }

        public JsonResult txtTotal_Validated(OmMstPricelistSell model)
        {
            var MstModel = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);
            var PPnBMPct = (MstModel == null) ? 0 : Convert.ToDecimal(MstModel.PpnBmPctSell);
            var dppAmt = model.Total / ((100 + model.TaxPct + PPnBMPct) / 100);

            decimal x = Convert.ToDecimal(dppAmt);
            decimal y = Convert.ToDecimal(dppAmt * (PPnBMPct / 100));
            decimal z = Convert.ToDecimal(model.Total - (dppAmt + y));

            return Json(new { success = true, DPP = Math.Round(x), PPn = Math.Round(z), PPnBM = Math.Round(y) });
        }

        public JsonResult Save(OmMstPricelistSell model)
        {
            string msg = "";
            var record = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode, model.GroupPriceCode, model.SalesModelCode, model.SalesModelYear);

            if (record == null)
            {
                record = new OmMstPricelistSell
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    GroupPriceCode = model.GroupPriceCode,
                    SalesModelCode = model.SalesModelCode,
                    SalesModelYear = model.SalesModelYear,
                    DPP = model.DPP,
                    PPn = model.PPn,
                    PPnBM = model.PPnBM == null ? 0 : model.PPnBM,
                    Total = model.Total,
                    TotalMinStaff = model.TotalMinStaff == null ? 0 : model.TotalMinStaff,
                    TaxCode = model.TaxCode,
                    TaxPct = model.TaxPct,
                    Remark = model.Remark == null ? "" : model.Remark,
                    Status = model.Status == "true" ? "1" : "0",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    isLocked = false,
                    LockingBy = "",
                    LockingDate = Convert.ToDateTime("1900-01-01"),
                    TotalMinSpv = 0,
                    FromDate = Convert.ToDateTime("1900-01-01"),
                    ToDate = Convert.ToDateTime("1900-01-01")

                };
                ctx.OmMstPricelistSells.Add(record);
                msg = "New PriceList Sell added...";
            }
            else
            {
                record.DPP = model.DPP;
                record.PPn = model.PPn;
                record.PPnBM = model.PPnBM == null ? 0 : model.PPnBM;
                record.Total = model.Total;
                record.TotalMinStaff = model.TotalMinStaff == null ? 0 : model.TotalMinStaff;
                record.TaxCode = model.TaxCode;
                record.TaxPct = model.TaxPct;
                record.Remark = model.Remark == null ? "" : model.Remark;
                record.Status = model.Status == "True" ? "1" : "0";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctx.CurrentTime;
                record.isLocked = false;
                record.LockingBy = "";
                record.LockingDate = Convert.ToDateTime("1900-01-01");
                record.TotalMinSpv = 0;
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

        public JsonResult Delete(OmMstPricelistSell model)
        {
            var record = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode, model.GroupPriceCode, model.SalesModelCode, model.SalesModelYear);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.OmMstPricelistSells.Remove(record);
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
