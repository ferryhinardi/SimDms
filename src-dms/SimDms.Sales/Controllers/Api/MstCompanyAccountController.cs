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
    public class MstCompanyAccountController : BaseController
    {
        public JsonResult AccNo(MstCompanyAccount model)
        {
            var record = ctx.GnMstAccounts.Find(CompanyCode, BranchCode, model.InterCompanyAccNoTo);

            return Json(new { success = record != null, data = record });
        }

        public JsonResult Save(MstCompanyAccount model)
        {
            string msg = "";
            var record = ctx.MstCompanyAccount.Find(CompanyCode, model.CompanyCodeTo);

            if (record == null)
            {
                record = new MstCompanyAccount
                {
                    CompanyCode = CompanyCode,
                    CompanyCodeTo = model.CompanyCodeTo ?? "",
                    CompanyCodeToDesc = model.CompanyCodeToDesc ?? "",
                    BranchCodeTo = model.BranchCodeTo ?? "",
                    BranchCodeToDesc = model.BranchCodeToDesc ?? "",
                    WarehouseCodeTo = model.WarehouseCodeTo,
                    WarehouseCodeToDesc = model.WarehouseCodeToDesc,
                    InterCompanyAccNoTo = model.InterCompanyAccNoTo,
                    UrlAddress = model.UrlAddress,
                    isActive = model.isActive,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime
                };
                
                ctx.MstCompanyAccount.Add(record);
                msg = "New Company Account added...";
            }
            else
            {
                record.CompanyCodeToDesc = model.CompanyCodeToDesc ?? "";
                record.BranchCodeTo = model.BranchCodeTo ?? "";
                record.BranchCodeToDesc = model.BranchCodeToDesc ?? "";
                record.WarehouseCodeTo = model.WarehouseCodeTo;
                record.WarehouseCodeToDesc = model.WarehouseCodeToDesc;
                record.InterCompanyAccNoTo = model.InterCompanyAccNoTo;
                record.UrlAddress = model.UrlAddress;
                record.isActive = model.isActive;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctx.CurrentTime;
                msg = "Company Account updated";
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

        public JsonResult Delete(MstCompanyAccount model)
        {
            var record = ctx.MstCompanyAccount.Find(CompanyCode, model.CompanyCodeTo);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.MstCompanyAccount.Remove(record);
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
