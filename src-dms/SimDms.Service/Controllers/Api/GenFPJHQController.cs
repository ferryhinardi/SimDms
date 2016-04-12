using SimDms.Common;
using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class GenFPJHQController : BaseController
    {
        public JsonResult Default()
        {
            var IsCentral = false;
            var data = ctx.LookUpDtls.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.CodeID == GnMstLookUpHdr.FPGabungan);
            if (data != null && data.LookUpValue.Equals("1") && !IsBranch) IsCentral = true;

            if (!IsCentral) return Json(new { success = false, message = "Set konfigurasi Pajak Gabungan di Master Lookup" });
            if (IsBranch) return Json(new { success = false, message = "User anda bukan holding." });

            return Json(new
            {
                success = true,
                BranchFrom = BranchFrom,
                BranchTo = BranchTo
            });
        }

        public JsonResult GetData(string isPdi, string isFsc, string isFscCampaign)
        {
            object[] parameters = { CompanyCode, BranchFrom, BranchTo, ProductType, isPdi, isFsc, isFscCampaign };
            var query = "exec uspfn_Select4ListFpjHQ @p0,@p1,@p2,@p3,@p4,@p5,@p6";

            var data = ctx.Database.SqlQuery<InqFPJData>(query, parameters);

            return Json(data);
        }

        public JsonResult LoadData(string fpjNo, string branchFrom, string branchTo)
        {
            var docPrefix = "";
            string query = string.Format("exec uspfn_SvInqFpjHQGet '{0}','{1}','{2}','{3}','{4}'", CompanyCode, ProductType, branchFrom, branchTo, fpjNo);
            var data = ctx.Database.SqlQuery<InqFPJGet>(query);

            if (data.Count() > 0)
            {
                string prefix = data.FirstOrDefault().InvoiceNo.Substring(0, 3);
                if (prefix.Equals("INF"))
                    docPrefix = "0";
                else if (prefix.Equals("INW"))
                    docPrefix = "1";
                else
                    docPrefix = "2";
            }
            return Json(new { docPrefix = docPrefix, data = data });
        }

        public JsonResult GetDataFPJHQ(string isPdi, string isFsc, string isFscCampaign, string isSprClaim, string CustBill)
        {
            object[] parameters = { CompanyCode, BranchFrom, BranchTo, ProductType, isPdi, isFsc, isFscCampaign };
            var query = "exec uspfn_Select4ListFpjHQ @p0,@p1,@p2,@p3,@p4,@p5,@p6";

            var data = ctx.Database.SqlQuery<InqFPJGet>(query,parameters);

           return Json(data);
        }

        public JsonResult GetDataClaimHQ(string isSprClaim)
        {
            object[] parameters = { CompanyCode, BranchFrom, BranchTo, ProductType, isSprClaim};
            var query = "exec uspfn_Select4ListFpjClaimHQ @p0,@p1,@p2,@p3,@p4";

            var data = ctx.Database.SqlQuery<InqFPJGet>(query, parameters);

            return Json(data);
        }

        public JsonResult GetDataCustHQ(string CustBill)
        {
            object[] parameters = { CompanyCode, BranchFrom, BranchTo, ProductType, CustBill };
            var query = "exec uspfn_Select4ListFpjCustHQ @p0,@p1,@p2,@p3,@p4";

            var data = ctx.Database.SqlQuery<InqFPJGet>(query, parameters);

            return Json(data);
        }

        public ActionResult Save(List<TaxInvoicePreSave> model)
        {
            var invModel = model.FirstOrDefault();
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var item = ctx.Invoices.Find(CompanyCode, invModel.BranchCode, ProductType, invModel.InvoiceNo);

                    var taxInvoice = new TaxInvoice();
                    taxInvoice.CompanyCode = CompanyCode;
                    taxInvoice.BranchCode = BranchCode;
                    taxInvoice.FPJNo = GetNewDocumentNo("FPH", DateTime.Now);
                    taxInvoice.FPJDate = DateTime.Now;
                    taxInvoice.FPJCentralNo = "";
                    taxInvoice.SignedDate = DateTime.Now;
                    taxInvoice.CustomerCode = item.CustomerCode;
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
                    taxInvoice.DueDate = taxInvoice.FPJDate.AddDays(Convert.ToDouble(taxInvoice.TOPDays));
                    taxInvoice.NoOfInvoice = (model.Count < 2) ? 2 : model.Count;
                    taxInvoice.GenerateStatus = "0";
                    taxInvoice.CreatedBy = CurrentUser.UserId;
                    taxInvoice.CreatedDate = DateTime.Now;
                    taxInvoice.LastupdateBy = CurrentUser.UserId;
                    taxInvoice.LastupdateDate = DateTime.Now;

                    ctx.TaxInvoices.Add(taxInvoice);

                    ctx.SaveChanges();

                    foreach (var data in model)
                    {
                        UpdateInvoice(data.BranchCode, data.InvoiceNo, taxInvoice.FPJNo, taxInvoice.FPJDate);
                    }
                    trans.Commit();
                    return Json(new { success = true, data = taxInvoice, message = "Data Saved" });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    trans.Dispose();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        private void UpdateInvoice(string branchCode, string invoiceNo, string fpjNo, DateTime fpjDate)
        {
            var invoices = ctx.Invoices.Find(CompanyCode, branchCode, ProductType, invoiceNo);

            invoices.InvoiceStatus = "3";
            invoices.FPJNo = fpjNo;
            invoices.FPJDate = fpjDate;
            invoices.IsLocked = !IsBranch;
            invoices.LockingBy = CurrentUser.UserId;
            invoices.LastupdateDate = DateTime.Now;

            ctx.SaveChanges();
        }

        protected string BranchFrom
        {
            get
            {
                return ctx.OrganizationDtls.FirstOrDefault(c => c.CompanyCode == CompanyCode).BranchCode;
            }
        }

        protected string BranchTo
        {
            get
            {
                return ctx.OrganizationDtls.Where(c => c.CompanyCode == CompanyCode).OrderByDescending(c => c.SeqNo).FirstOrDefault().BranchCode;
            }
        }
    }
}