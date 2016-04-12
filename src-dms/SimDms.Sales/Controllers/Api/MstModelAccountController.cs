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
using Newtonsoft.Json.Linq;


namespace SimDms.Sales.Controllers.Api
{
    public class MstModelAccountController : BaseController
    {
        public JsonResult ModelCode(MstModelAccount model)
        {
            var record = ctx.MstModels.Find(CompanyCode, model.SalesModelCode);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult ModelAccount(MstModelAccount model)
        {
            var record = ctx.Database.SqlQuery<ModelAccountBrowse>("sp_ModelAccountLookup '" + CompanyCode + "','" + BranchCode + "','" + model.SalesModelCode + "'");

            return Json(new { success = record != null, record = record });
        }

        public JsonResult Save(MstModelAccount model)
        {
            string msg = "";
            var record = ctx.MstModelAccount.Find(CompanyCode, BranchCode, model.SalesModelCode);

            if (record == null)
            {
                record = new MstModelAccount
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    SalesModelCode = model.SalesModelCode,
                    SalesAccNo = model.SalesAccNo == null ? "" : model.SalesAccNo,
                    DiscountAccNo = model.DiscountAccNo == null ? "" : model.DiscountAccNo,
                    ReturnAccNo = model.ReturnAccNo == null ? "" : model.ReturnAccNo,
                    COGsAccNo = model.COGsAccNo == null ? "" : model.COGsAccNo,
                    InventoryAccNo = model.InventoryAccNo == null ? "" : model.InventoryAccNo,
                    SalesAccNoAks = model.SalesAccNoAks == null ? "" : model.SalesAccNoAks,
                    ReturnAccNoAks = model.ReturnAccNoAks == null ? "" : model.ReturnAccNoAks,
                    COGsAccNoAks = model.COGsAccNoAks == null ? "" : model.COGsAccNoAks,
                    InventoryAccNoAks = model.InventoryAccNoAks == null ? "" : model.InventoryAccNoAks,
                    BBNAccNo = model.BBNAccNo == null ? "" : model.BBNAccNo,
                    KIRAccNo = model.KIRAccNo == null ? "" : model.KIRAccNo,
                    HReturnAccNo = model.HReturnAccNo == null ? "" : model.HReturnAccNo,
                    PReturnAccNo = model.PReturnAccNo == null ? "" : model.PReturnAccNo,
                    DiscountAccNoAks = model.DiscountAccNoAks == null ? "" : model.DiscountAccNoAks,
                    ShipAccNo = model.ShipAccNo == null ? "" : model.ShipAccNo,
                    DepositAccNo = model.DepositAccNo == null ? "" : model.DepositAccNo,
                    OthersAccNo = model.OthersAccNo == null ? "" : model.OthersAccNo,
                    InTransitTransferStockAccNo = model.InTransitTransferStockAccNo == null ? "" : model.InTransitTransferStockAccNo,
                    Remark = model.Remark == null ? "" : model.Remark,
                    IsActive = model.IsActive,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime

                };
                ctx.MstModelAccount.Add(record);
                msg = "New Model Account added...";
            }
            else
            {
                    record.SalesAccNo = model.SalesAccNo == null ? "" : model.SalesAccNo;
                    record.DiscountAccNo = model.DiscountAccNo == null ? "" : model.DiscountAccNo;
                    record.ReturnAccNo = model.ReturnAccNo == null ? "" : model.ReturnAccNo;
                    record.COGsAccNo = model.COGsAccNo == null ? "" : model.COGsAccNo;
                    record.InventoryAccNo = model.InventoryAccNo == null ? "" : model.InventoryAccNo;
                    record.SalesAccNoAks = model.SalesAccNoAks == null ? "" : model.SalesAccNoAks;
                    record.ReturnAccNoAks = model.ReturnAccNoAks == null ? "" : model.ReturnAccNoAks;
                    record.COGsAccNoAks = model.COGsAccNoAks == null ? "" : model.COGsAccNoAks;
                    record.InventoryAccNoAks = model.InventoryAccNoAks == null ? "" : model.InventoryAccNoAks;
                    record.BBNAccNo = model.BBNAccNo == null ? "" : model.BBNAccNo;
                    record.KIRAccNo = model.KIRAccNo == null ? "" : model.KIRAccNo;
                    record.HReturnAccNo = model.HReturnAccNo == null ? "" : model.HReturnAccNo;
                    record.PReturnAccNo = model.PReturnAccNo == null ? "" : model.PReturnAccNo;
                    record.DiscountAccNoAks = model.DiscountAccNoAks == null ? "" : model.DiscountAccNoAks;
                    record.ShipAccNo = model.ShipAccNo == null ? "" : model.ShipAccNo;
                    record.DepositAccNo = model.DepositAccNo == null ? "" : model.DepositAccNo;
                    record.OthersAccNo = model.OthersAccNo == null ? "" : model.OthersAccNo;
                    record.InTransitTransferStockAccNo = model.InTransitTransferStockAccNo == null ? "" : model.InTransitTransferStockAccNo;
                    record.Remark = model.Remark == null ? "" : model.Remark;
                    record.IsActive = model.IsActive;
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = ctx.CurrentTime;
                    msg = "Model Account updated";
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

        public JsonResult Delete(MstModelAccount model)
        {
            var record = ctx.MstModelAccount.Find(CompanyCode, BranchCode, model.SalesModelCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstModelAccount.Remove(record);
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
