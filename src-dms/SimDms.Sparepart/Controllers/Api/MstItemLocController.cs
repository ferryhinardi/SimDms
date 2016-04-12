using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Common.Models;
using SimDms.Common;

namespace SimDms.Sparepart.Controllers.Api
{
    public class MstItemLocController : BaseController
    {

        public JsonResult Save(SpMstItemLoc model)
        {
            string msg = "";
            var record = ctx.SpMstItemLocs.Find(CompanyCode,BranchCode,model.PartNo,model.WarehouseCode);

            if (record == null)
            {
                record = new SpMstItemLoc
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    PartNo = model.PartNo,
                    WarehouseCode = model.WarehouseCode
                };                
                ctx.SpMstItemLocs.Add(record);
                msg = "New item location added...";
            }
            else
            {
                ctx.SpMstItemLocs.Attach(record);
                msg = "Item location updated";
            }

            record.LocationCode = model.LocationCode;
            record.LocationSub1 = model.LocationSub1;
            record.LocationSub2 = model.LocationSub2;
            record.LocationSub3 = model.LocationSub3;
            record.LocationSub4 = model.LocationSub4;
            record.LocationSub5 = model.LocationSub5;
            record.LocationSub6 = model.LocationSub6;

            record.isLocked = false;
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = ctx.CurrentTime;

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


        public JsonResult Delete(SpMstItemLoc model)
        {
             var record = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, model.PartNo, model.WarehouseCode);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.SpMstItemLocs.Remove(record);
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

        public string CheckRecord(string PartNo, string WhCode)
        {
            var data = ctx.spMstItems.Find(CompanyCode, BranchCode, PartNo);
            if (data == null)
            {
                return "0";
            }
            return "1";
        }

    }
}
