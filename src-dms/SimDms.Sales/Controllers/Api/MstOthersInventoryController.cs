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
    public class MstOthersInventoryController : BaseController
    {
        public JsonResult AccNo(MstCompanyAccount model)
        {
            var record = ctx.GnMstAccounts.Find(CompanyCode, BranchCode, model.InterCompanyAccNoTo);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult Save(MstOthersInventory model)
        {
            string msg = "";
            var record = ctx.MstOthersInventory.Find(CompanyCode, BranchCode, model.OthersNonInventory);

            if (record == null)
            {
                record = new MstOthersInventory
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    OthersNonInventory = model.OthersNonInventory,
                    OthersNonInventoryDesc = model.OthersNonInventoryDesc == null ? "" : model.OthersNonInventoryDesc,
                    OthersNonInventoryAccNo = model.OthersNonInventoryAccNo,
                    IsActive = model.IsActive,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    Remark = ""
                };
                ctx.MstOthersInventory.Add(record);
                msg = "New Others Inventory added...";
            }
            else
            {
                record.OthersNonInventoryDesc = model.OthersNonInventoryDesc == null ? "" : model.OthersNonInventoryDesc;
                record.Remark = model.Remark;
                record.IsActive = model.IsActive;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctx.CurrentTime;
                record.Remark = "";
                //ctx.MstRefferences.Add(record);
                msg = "Others Inventory updated";
            }

            //record.IsLocked = false;
            //record.LockedBy = CurrentUser.UserId;
            //record.LockedDate = ctx.CurrentTime;

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

        public JsonResult Delete(MstOthersInventory model)
        {
            var record = ctx.MstOthersInventory.Find(CompanyCode, BranchCode, model.OthersNonInventory);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstOthersInventory.Remove(record);
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
