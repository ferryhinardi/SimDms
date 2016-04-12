using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SimDms.Common;

namespace SimDms.Service.Controllers.Api
{
    public class InvoiceCancelController : BaseController
    {
        public JsonResult SelectInqInvCancel(string invoice1, string invoice2)
        {
            var query = "exec uspfn_SvInqInvCancel @p0, @p1, @p2, @p3";
            var result = ctx.Database.SqlQuery<SvInqInvCancel>(query, CompanyCode, BranchCode, invoice1, invoice2).OrderByDescending(a => a.InvoiceDate).ThenByDescending(b=>b.InvoiceNo);
            
            return Json(result);
        }

        public JsonResult RePosting(IEnumerable<SvInqInvCancel> data)
        {
            var message = "";
            
            try
            {
                foreach (var item in data)
                {
                    var invoice = ctx.Invoices.FirstOrDefault(x =>
                        x.CompanyCode == CompanyCode && x.BranchCode == BranchCode &&
                        x.ProductType == ProductType && x.InvoiceNo == item.InvoiceNo);
                    if (invoice == null) throw new Exception("Invoice " + item.InvoiceNo +" tidak ditemukan");

                    #region Update Journal
                    var query1 = string.Format(@"
delete from glInterface
 where ProfitCenterCode = '200'
   and StatusFlag = 0
   and CompanyCode = '{0}'
   and BranchCode = '{1}'
   and DocNo = '{2}'

delete from arInterface
 where ProfitCenterCode = '200'
   and StatusFlag = 0
   and CompanyCode = '{0}'
   and BranchCode = '{1}'
   and DocNo = '{2}'

update gnTrnBankBook set SalesAmt = SalesAmt - {4}
 where ProfitCenterCode = '200'
   and CompanyCode = '{0}'
   and BranchCode = '{1}'
   and CustomerCode = '{3}'
", CompanyCode, BranchCode, invoice.InvoiceNo, invoice.CustomerCodeBill, invoice.TotalSrvAmt);
                    var a = ctx.Database.ExecuteSqlCommand(query1);
                    if (a < 0) throw new Exception("Update Jurnal gagal");
                    #endregion

                    // Posting Invoice
                    var query2 = "exec uspfn_SvTrnInvoicePosting @p0, @p1, @p2, @p3, @p4";
                    var b = ctx.Database.ExecuteSqlCommand(query2,
                        CompanyCode, BranchCode, ProductType, invoice.InvoiceNo, CurrentUser.UserId);
                    
                    #region SaveBankBook
                    if (b > 0)
                    {
                        var book = ctx.BankBooks.FirstOrDefault(x =>
                            x.CompanyCode == CompanyCode && x.BranchCode == BranchCode &&
                            x.CustomerCode == invoice.CustomerCodeBill && x.ProfitCenterCode == ProfitCenter);
                        if (book == null)
                        {
                            book = new BankBook
                            {
                                CompanyCode = CompanyCode,
                                BranchCode = BranchCode,
                                CustomerCode = invoice.CustomerCodeBill,
                                ProfitCenterCode = ProfitCenter,
                                SalesAmt = invoice.TotalSrvAmt,
                                ReceivedAmt = 0
                            };
                            ctx.BankBooks.Add(book);
                        }
                        else book.SalesAmt += invoice.TotalSrvAmt;
                        ctx.SaveChanges();
                    } 
                    #endregion
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }

        public JsonResult CancelInvoice(IEnumerable<SvInqInvCancel> data)
        {
            var message = "";

            try
            {
                foreach (var item in data)
                {
                    var invoice = ctx.Invoices.FirstOrDefault(x =>
                        x.CompanyCode == CompanyCode && x.BranchCode == BranchCode &&
                        x.ProductType == ProductType && x.InvoiceNo == item.InvoiceNo);
                    if (invoice == null) throw new Exception("Invoice " + item.InvoiceNo + " tidak ditemukan");

                    
                    var query = "exec uspfn_SvTrnInvoiceCancel @p0, @p1, @p2, @p3";
                    ctx.Database.ExecuteSqlCommand(query, CompanyCode, BranchCode, invoice.InvoiceNo,
                        string.Format("{0}^{1}", CurrentUser.UserId, Request.UserHostAddress));
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }

        public JsonResult GetDescInvoice(string invoiceNo)
        {
            var query = "exec uspfn_SvTrnInvoiceJournal @p0, @p1, @p2, @p3, @p4";
            var result = ctx.Database.SqlQuery<SvInqInvCancelSubDtl>(query,
                CompanyCode, BranchCode, ProductType, invoiceNo, CurrentUser.UserId);
            return Json(result);
        }

        public JsonResult ValidateInvoiceNo(string InvoiceNo)
        {
            var query = "exec uspfn_svSelectInvoiceForCancellation @p0, @p1, @p2, @p3";
            var result = ctx.Database.SqlQuery<SvUtilInvoiceCancelLookup>(query,
                CompanyCode, BranchCode, ProductType, InvoiceNo);

            bool success = false;
            if (result.Count() > 0)
            {
                success = true;
            }

            return Json(new { Success = success });
        }

        public JsonResult InvoiceForCancellation()
        {
            var query = "exec uspfn_svSelectInvoiceForCancellation @p0, @p1, @p2, @p3";
            var result = ctx.Database.SqlQuery<SvUtilInvoiceCancelLookup>(query,
                CompanyCode, BranchCode, ProductType, "").AsQueryable();

            return Json(result.toKG());
        }
    }
}
