using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;

namespace SimDms.Service.Controllers.Api
{
    public class GenFPJController : BaseController
    {
        public JsonResult GetData(string docPrefix, string isPdi, string isFsc, string isFscCampaign, string isSprClaim, string CustBill)
        {
            object[] parameters = new Object[] { 
                new SqlParameter("@CompanyCode", CompanyCode),
                new SqlParameter("@BranchCode", BranchCode),
                new SqlParameter("@DocPrefix", docPrefix),
                new SqlParameter("@IsPdi", isPdi),
                new SqlParameter("@IsFsc", isFsc),
                new SqlParameter("@IsFSCCampaign", isFscCampaign),
                new SqlParameter("@IsSprClaim", isSprClaim),
                new SqlParameter("@CustBill", CustBill)};
            var query = "exec uspfn_SvInqFpjData @CompanyCode, @BranchCode, @DocPrefix, @IsPdi, @IsFsc, @IsFSCCampaign, @IsSprClaim, @CustBill";

            var data = ctx.Database.SqlQuery<InqGenFPJData>(query, parameters).ToList();

            return Json(data);
        }

        public JsonResult LoadData(string fpjNo)
        {
            string query = string.Format("exec uspfn_SvInqFpjGet {0},{1},'{2}'", CompanyCode, BranchCode, fpjNo);
            var data = ctx.Database.SqlQuery<InqFPJGet>(query);

            return Json(data);
        }

        public ActionResult Save(List<TaxInvoicePreSave> model)
        {
            var item = ctx.Invoices.Find(CompanyCode, model.FirstOrDefault().BranchCode, ProductType, model.FirstOrDefault().InvoiceNo);

            var taxInvoice = new TaxInvoice();
            taxInvoice.CompanyCode = CompanyCode;
            taxInvoice.BranchCode = item.BranchCode;
            taxInvoice.FPJNo = GetNewDocumentNo("FPS", DateTime.Now);
            taxInvoice.FPJDate = DateTime.Now;
            taxInvoice.FPJCentralNo = "";
            taxInvoice.SignedDate = DateTime.Now;
            taxInvoice.CustomerCode = item.CustomerCodeBill;
            taxInvoice.CustomerCodeBill = item.CustomerCodeBill;
            taxInvoice.IsPKP = item.IsPKP.Value;

            var cust = ctx.Customers.Find(CompanyCode, item.CustomerCodeBill);

            if (cust != null)
            {
                taxInvoice.CustomerName = cust.CustomerName;
                taxInvoice.Address1 = cust.Address1;
                taxInvoice.Address2 = cust.Address2;
                taxInvoice.Address3 = cust.Address3;
                taxInvoice.Address4 = cust.Address4;
                taxInvoice.SKPNo = cust.SKPNo;
                taxInvoice.SKPDate = cust.SKPDate;
                taxInvoice.NPWPNo = cust.NPWPNo;
                taxInvoice.NPWPDate = cust.NPWPDate;
                taxInvoice.HPNo = cust.HPNo;
                taxInvoice.PhoneNo = cust.PhoneNo;
            }

            taxInvoice.TOPCode = item.TOPCode;
            taxInvoice.TOPDays = item.TOPDays;
            taxInvoice.DueDate = item.DueDate.Value;
            taxInvoice.NoOfInvoice = (model.Count < 2) ? 2 : model.Count;
            taxInvoice.GenerateStatus = "0";
            taxInvoice.CreatedBy = CurrentUser.UserId;
            taxInvoice.CreatedDate = DateTime.Now;
            taxInvoice.LastupdateBy = CurrentUser.UserId;
            taxInvoice.LastupdateDate = DateTime.Now;

            ctx.TaxInvoices.Add(taxInvoice);

            try
            {
                ctx.SaveChanges();

                foreach (var data in model)
                {
                    UpdateInvoice(data.BranchCode, data.InvoiceNo, taxInvoice.FPJNo, taxInvoice.FPJDate);
                    UpdateAR(data.BranchCode, data.InvoiceNo, taxInvoice.FPJNo, taxInvoice.FPJDate);
                }
                return Json(new { success = true, FPJNo = taxInvoice.FPJNo, message = "Data Saved" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        private void UpdateInvoice(string branchCode, string invoiceNo, string fpjNo, DateTime fpjDate)
        {
            var invoices = ctx.Invoices.Find(CompanyCode, branchCode, ProductType, invoiceNo);

            invoices.InvoiceStatus = "3";
            invoices.FPJNo = fpjNo;
            invoices.FPJDate = fpjDate;
            invoices.IsLocked = true;
            invoices.LockingBy = CurrentUser.UserId;
            invoices.LastupdateDate = DateTime.Now;

            ctx.SaveChanges();
        }

        private void UpdateAR(string branchCode, string invoiceNo, string fpjNo, DateTime fpjDate)
        {
            var ar = ctx.ARInterfaces.Find(CompanyCode, branchCode, invoiceNo);

            ar.FakturPajakNo = fpjNo;
            ar.FakturPajakDate = fpjDate;

            ctx.SaveChanges();
        }
    }
}
