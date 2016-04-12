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
    public class GenFPJStdController : BaseController
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

        public JsonResult GetData(string FPJNo, string taxInvoiceNoStart, string taxInvoiceNoEnd)
        {
            DataTable dt = new DataTable();
            var da = Sda(FPJNo, taxInvoiceNoStart, taxInvoiceNoEnd);

            da.Fill(dt);
            var list = GetJson(dt);

            if (list != null)
            {
                return Json(new { success = true, list = list });
            }
            else
            {
                return Json(new { success = false, message = "data not found" }); ;
            }
        }

        public ActionResult Save(string FPJNo, string taxInvoiceNoStart, string taxInvoiceNoEnd)
        {
            var query = "exec uspfn_SvInqFpjStdData {0}, {1}, {2}, {3}, {4}, {5}";
            object[] parameters = { CompanyCode, BranchCode, ProductType, FPJNo, taxInvoiceNoStart, taxInvoiceNoEnd };
            var data = ctx.Database.SqlQuery<TaxInvoiceSave>(query, parameters).ToList();
            
            try
            {
                var taxInvoice = new TaxInvoice();
                taxInvoice.CompanyCode = CompanyCode;
                taxInvoice.BranchCode = BranchCode;
                taxInvoice.FPJNo = GetNewDocumentNo("FPS");
                taxInvoice.FPJDate = DateTime.Now;
                taxInvoice.FPJGovNo = "";
                taxInvoice.FPJCentralNo = "";
                taxInvoice.SignedDate = DateTime.Now;
                taxInvoice.CustomerCode = data.FirstOrDefault().CustomerCodeBill;
                taxInvoice.CustomerCodeBill = data.FirstOrDefault().CustomerCodeBill;
                taxInvoice.IsPKP = data.FirstOrDefault().IsPKP;

                var cust = ctx.Customers.Find(CompanyCode, data.FirstOrDefault().CustomerCodeBill);

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

                taxInvoice.TOPCode = data.FirstOrDefault().TOPCode;
                taxInvoice.TOPDays = data.FirstOrDefault().TOPDays;
                taxInvoice.DueDate = data.FirstOrDefault().DueDate;
                taxInvoice.NoOfInvoice = data.Count();
                taxInvoice.GenerateStatus = "0";
                taxInvoice.CreatedBy = CurrentUser.UserId;
                taxInvoice.CreatedDate = DateTime.Now;
                taxInvoice.LastupdateBy = CurrentUser.UserId;
                taxInvoice.LastupdateDate = DateTime.Now;

                ctx.TaxInvoices.Add(taxInvoice);

                try
                {
                    ctx.SaveChanges();
                    foreach (var item in data)
                    {
                        UpdateInvoice(item.BranchCode, item.InvoiceNo, taxInvoice.FPJNo, taxInvoice.FPJDate);
                    }
                    return Json(new { success = true, FPJNo = taxInvoice.FPJNo, message = "Data Saved" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        protected SqlDataAdapter Sda(string FPJNo, string taxInvoiceNoStart, string taxInvoiceNoEnd)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvInqFpjStdData";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@FPJNo", FPJNo);
            cmd.Parameters.AddWithValue("@InvoiceNoStart", taxInvoiceNoStart);
            cmd.Parameters.AddWithValue("@InvoiceNoEnd", taxInvoiceNoEnd);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            return da;
        }

        private string GetNewDocumentNo(string docType)
        {
            var query = "exec uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            object[] parameters = { CompanyCode, BranchCode, docType, CurrentUser.UserId, DateTime.Now };

            var newDocument = ctx.Database.SqlQuery<string>(query, parameters).First();

            return newDocument;
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
    }
}
