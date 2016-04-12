using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class SalesCampaignController : BaseController
    {




        public JsonResult Save(spMstSalesCampaign model)
        {
            string msg = "";
            var record = ctx.spMstSalesCampaigns.Find(CompanyCode, BranchCode,model.SupplierCode, model.PartNo,model.BegDate);

            if (record == null)
            {
                record = new spMstSalesCampaign
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    PartNo = model.PartNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spMstSalesCampaigns.Add(record);
                msg = "New item price added...";
            }
            else
            {
                ctx.spMstSalesCampaigns.Attach(record);
                msg = "Item price updated";
            }

            record.SupplierCode = model.SupplierCode;
            record.BegDate = model.BegDate;
            record.EndDate = model.EndDate;
            record.DiscPct = model.DiscPct;
 
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

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




        public JsonResult Delete(spMstSalesCampaign model)
        {
            var record = ctx.spMstSalesCampaigns.Find(CompanyCode, BranchCode, model.SupplierCode, model.PartNo, model.BegDate);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spMstSalesCampaigns.Remove(record);
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

        public string CheckRecord(string PartNo)
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
