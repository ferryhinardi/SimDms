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
    public class PurchCampaignController : BaseController
    {




        public JsonResult Save(spMstPurchCampaign model)
        {
            string msg = "";
            var record = ctx.spMstPurchCampaigns.Find(CompanyCode, BranchCode,model.SupplierCode, model.PartNo,model.BegDate);

            if (record == null)
            {
                record = new spMstPurchCampaign
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    PartNo = model.PartNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spMstPurchCampaigns.Add(record);
                msg = "New item price added...";
            }
            else
            {
                ctx.spMstPurchCampaigns.Attach(record);
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




        public JsonResult Delete(spMstPurchCampaign model)
        {
            var record = ctx.spMstPurchCampaigns.Find(CompanyCode, BranchCode, model.SupplierCode, model.PartNo, model.BegDate);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spMstPurchCampaigns.Remove(record);
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

        public JsonResult getRecord(string PartNo)
        {
            var rec = ctx.SpMstItemInfos.Find(CompanyCode, PartNo);
            var record = (from p in ctx.SpMstItemInfos
                       join p1 in ctx.GnMstSuppliers
                        on new { p.CompanyCode, p.SupplierCode } equals new { p1.CompanyCode, p1.SupplierCode }
                        where p.CompanyCode == CompanyCode && p.PartNo == PartNo
                        select new
                        {
                            p.PartNo,
                            p.PartName,
                            p1.SupplierCode,
                            p1.SupplierName
                        });
            //var me = ctx.SpMstItemInfos.Find(CompanyCode, PartNo);
            if (rec != null)
            {
                return Json(new
                {
                    success = true,
                    data = record.FirstOrDefault()//,
                    //info = me
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false });
        }
    }
}
