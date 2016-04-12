using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using SimDms.Common.Models;

namespace SimDms.Service.Controllers.Api
{
    public class MaintainFPJController : BaseController
    {       
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName
            });
        }

        public JsonResult Get(string FPJNo)
        {
            var qry = from p in ctx.TaxInvoices
                      where p.CompanyCode == p.CompanyCode && p.BranchCode == BranchCode && p.FPJNo == FPJNo
                      select p;
            var record = qry.FirstOrDefault();

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false, message = "data not found" });
        }

        public ActionResult Save(string FPJNo, string customerCodeBill)
        {
            TaxInvoice taxInvoice = ctx.TaxInvoices.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.FPJNo == FPJNo).FirstOrDefault();
            GnMstCustomer customer = ctx.Customers.Where(p => p.CompanyCode == CompanyCode && p.CustomerCode == customerCodeBill).FirstOrDefault();
            taxInvoice.CustomerCode = taxInvoice.CustomerCodeBill = customer.CustomerCode;
            taxInvoice.CustomerName = customer.CustomerName;
            taxInvoice.Address1 = customer.Address1;
            taxInvoice.Address2 = customer.Address2;
            taxInvoice.Address3 = customer.Address3;
            taxInvoice.Address4 = customer.Address4;
            taxInvoice.NPWPNo = customer.NPWPNo;
            taxInvoice.NPWPDate = customer.NPWPDate;
            taxInvoice.SKPNo = customer.SKPNo;
            taxInvoice.SKPDate = customer.SKPDate;
            taxInvoice.LastupdateBy = CurrentUser.UserId;
            taxInvoice.LastupdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = "Save Succeeded", data = taxInvoice });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
