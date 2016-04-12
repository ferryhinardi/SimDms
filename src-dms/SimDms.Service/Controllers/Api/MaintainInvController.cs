using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;

namespace SimDms.Service.Controllers.Api
{
    public class MaintainInvController : BaseController
    {
        //
        // GET: /MaintainInv/

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

        public JsonResult TaskPartList(string invoiceNo)
        {
            string query = string.Format(@"
            select * into #t1 from(
             select 0 OrderGroup
             ,convert(varchar(100), a.OperationNo) as PartJobNo
             ,a.OperationHour as QtyNK
             ,convert(numeric(18,2), a.OperationCost) as Price
             ,convert(numeric(18,2), a.DiscPct) as DiscPct
             ,convert(numeric(18,2), (a.OperationHour * a.OperationCost) * (100.0 - a.DiscPct) * 0.01) as PriceNet
             ,convert(varchar(100), isnull((
		            select top 1 Description 
		              from svMstTask with (nowait,nolock)
                     where CompanyCode = a.CompanyCode
		               and ProductType = a.ProductType
		               and BasicModel  = b.BasicModel
		               and OperationNo = a.OperationNo
		               and JobType in (b.JobType,'CLAIM','OTHER')
		            ), '')
	            ) as PartName
             from svTrnInvTask a with (nowait,nolock)
             left join svTrnInvoice b with (nowait,nolock)
               on b.CompanyCode = a.CompanyCode
              and b.BranchCode = a.BranchCode
              and b.ProductType = a.ProductType
              and b.InvoiceNo = a.InvoiceNo
             where 1 = 1
              and a.CompanyCode = '{0}'
              and a.BranchCode = '{1}'
              and a.ProductType = '{2}'
              and a.InvoiceNo = '{3}'
             )#t1

            insert into #t1
            select 1 OrderGroup
             ,a.PartNo as PartJobNo
             ,(a.SupplyQty - ReturnQty) as QtyNK
             ,a.RetailPrice as Price
             ,convert(numeric(18,2), a.DiscPct) as DiscPct
             ,(a.SupplyQty - ReturnQty) * a.RetailPrice * (100.0 - a.DiscPct) * 0.01 as PriceNet
             ,b.PartName
            from svTrnInvItem a with (nowait,nolock)
            left join spMstItemInfo b with (nowait,nolock)
               on b.CompanyCode = a.CompanyCode
              and b.PartNo = a.PartNo
            where 1 = 1
              and a.CompanyCode = '{0}'
              and a.BranchCode = '{1}'
              and a.ProductType = '{2}'
              and a.InvoiceNo = '{3}'

            select OrderGroup, (row_number() over (order by OrderGroup, PartJobNo)) RecNo, PartJobNo, QtyNK, Price, DiscPct, PriceNet, PartName from #t1
            drop table #t1
            ", CompanyCode, BranchCode, ProductType, invoiceNo);

            return Json(ctx.Database.SqlQuery<InvoiceItem>(query));
        }

        public ActionResult Save(string invoiceNo,string custCodeBill, decimal totSrvAmt, InvoiceItem model)
        {
            if (model != null)
            {
                if (model.OrderGroup == 0)
                {
                    UpdateInvTask(invoiceNo,model.PartJobNo,model.QtyNK,model.Price,custCodeBill,totSrvAmt,model.DiscPct);
                }
                else
                {
                    UpdateRetailPrice(invoiceNo, model.PartJobNo, model.Price, custCodeBill, totSrvAmt, model.QtyNK, model.DiscPct);
                }
            }
            return Json(new { success = true, message = "Data Saved" });
        }

        public bool UpdateInvTask(string invoiceNo, string partNo, decimal nk, decimal price, string custCodeBill, decimal totSrvAmt, decimal discPct)
        {
            bool retVal = false;

            string query = string.Format(@"
            select * into #t1 from(
            select * from svTrnInvMechanic
             where 1 = 1
               and CompanyCode = '{0}'
               and BranchCode  = '{1}'
               and ProductType = '{2}'
               and InvoiceNo   = '{3}'
               and OperationNo = '{4}'
            )#t1
            
            delete svTrnInvMechanic
             where 1 = 1
               and CompanyCode = '{0}'
               and BranchCode  = '{1}'
               and ProductType = '{2}'
               and InvoiceNo   = '{3}'
               and OperationNo = '{4}'
            
            update svTrnInvTask
               set OperationNo = '{5}', OperationHour = {6}, OperationCost = {7}, DiscPct = {8}
             where 1 = 1
               and CompanyCode = '{0}'
               and BranchCode  = '{1}'
               and ProductType = '{2}'
               and InvoiceNo   = '{3}'
               and OperationNo = '{4}'
            
            update #t1 set OperationNo = '{5}'
            
            insert into svTrnInvMechanic
            select * from #t1
            
            drop table #t1
            ", CompanyCode, BranchCode, ProductType, invoiceNo, partNo, partNo, nk, price, discPct);

            try
            {
                int i = -1;
                i = ctx.Database.ExecuteSqlCommand(query);
                if (i > 0) i += RecalculateInvoice(invoiceNo);

                // Reposting Journal
                var oInvoice = ctx.Invoices.Find(CompanyCode, BranchCode, ProductType, invoiceNo);
                if (oInvoice != null) RePosting(oInvoice, custCodeBill, totSrvAmt);

                retVal = (i > 0);
            }
            catch
            {
            }
            return retVal;
        }

        public int RecalculateInvoice(string invoiceNo)
        {
            int retVal = -1;
            string query = "exec uspfn_SvTrnInvoiceReCalculate @p0, @p1, @p2, @p3, @p4, @p5, @p6";
            object[] parameters = { CompanyCode, BranchCode, ProductType, invoiceNo, 0, 0, CurrentUser.UserId };

            retVal = ctx.Database.ExecuteSqlCommand(query, parameters);
            return retVal;
        }

        public bool UpdateRetailPrice(string invoiceNo, string partNo, decimal Price, string custCodeBill, decimal totSrvAmt, decimal nk, decimal discPct)
        {
            bool retVal = false;

            var query = "exec uspfn_SvTrnInvUpdPrice @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7";
            object[] parameters = { Price, CompanyCode, BranchCode, ProductType, invoiceNo, partNo, nk, discPct };

            try
            {
                ctx.Database.ExecuteSqlCommand(query, parameters);
                RecalculateInvoice(invoiceNo);

                //// Reposting Journal
                var oInvoice = ctx.Invoices.Find(CompanyCode, BranchCode, ProductType, invoiceNo);
                if (oInvoice != null) retVal = RePosting(oInvoice, custCodeBill, totSrvAmt);
            }
            catch 
            {
            }

            return retVal;
        }

        public bool RePosting(Invoice oInvoice, string oldCustCode, decimal oldTotSrvAmt)
        {
            bool retVal = false;

            string query = string.Format(@"
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
", CompanyCode, BranchCode, oInvoice.InvoiceNo, oldCustCode, oldTotSrvAmt);

            int i = ctx.Database.ExecuteSqlCommand(query);
            if (i >= 0) retVal = PostingInvoice(oInvoice);

            return retVal;
        }

        public bool PostingInvoice(Invoice oInvoice)
        {

            string query = "exec uspfn_SvTrnInvoicePosting @p0, @p1, @p2, @p3, @p4";  
            object[] parameters = { CompanyCode, BranchCode, ProductType, oInvoice.InvoiceNo, CurrentUser.UserId };
            int i = ctx.Database.ExecuteSqlCommand(query, parameters);
            if (i > 0) i += SaveBankBook(oInvoice);
            return i > 0;
        }

        private int SaveBankBook(Invoice oInvoice)
        {
            int retVal = -1;

            var book = ctx.BankBooks.Find(CompanyCode, BranchCode, oInvoice.CustomerCodeBill, ProfitCenter);
            if (book == null)
            {
                book = new BankBook();
                book.CompanyCode = CompanyCode;
                book.BranchCode = BranchCode;
                book.CustomerCode = oInvoice.CustomerCodeBill;
                book.ProfitCenterCode = ProfitCenter;
                book.SalesAmt = oInvoice.TotalSrvAmt;
                book.ReceivedAmt = 0;
                ctx.BankBooks.Add(book);
            }
            else
            {
                book.SalesAmt = book.SalesAmt + oInvoice.TotalSrvAmt;
            }

            try
            {
                retVal = ctx.SaveChanges();
            }
            catch 
            {
            }
           
            return retVal;
        }

        public JsonResult GetCustomerInfo(string custCode, string invoiceNo)
        {
            var customer = ctx.Customers.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CustomerCode == custCode);
            var invoice = ctx.Invoices.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode
                && x.ProductType == ProductType && x.InvoiceNo == invoiceNo);

            var info = new CustomerExtraInfoItem
            {
                CustomerCode = custCode,
                NPWPNo = customer.NPWPNo,
                NPWPDate = customer.NPWPDate
            };
            
            if (invoice != null)
            {
                var query = "exec uspfn_SvListDicount @p0, @p1, @p2, @p3, @p4, @p5, @p6";
                var discount = ctx.Database.SqlQuery<ListDiscountItem>(query,
                    CompanyCode, BranchCode,
                    invoice.CustomerCodeBill, invoice.ChassisCode, invoice.ChassisNo,
                    DateTime.Now, invoice.JobType).FirstOrDefault();
                if (discount != null)
                {
                    info.LaborDiscPct = discount.LaborDiscPct;
                    info.PartsDiscPct = discount.PartDiscPct;
                    info.MaterialDiscPct = discount.MaterialDiscPct;
                }
            }

            return Json(new { info = info });
        }

        public JsonResult GetInvoiceData(string invoiceNo)
        {
            var fiscalYear = ctx.CoProfileServices.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).FirstOrDefault().FiscalYear;
            var periodeDate = ctx.Periodes.Where(p => p.CompanyCode == p.CompanyCode && p.BranchCode == BranchCode && p.FiscalYear == fiscalYear).OrderBy(p => p.FromDate).FirstOrDefault().FromDate;
            var invoice1 = (from p in ctx.GLInterfaces
                            where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProfitCenterCode == ProfitCenter && p.StatusFlag == "0"
                            select p.DocNo).ToList();
            var invoice2 = (from p in ctx.ARInterfaces
                            where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProfitCenterCode == ProfitCenter && p.StatusFlag == "0"
                            select p.DocNo).ToList();

            var queryable = ctx.InvoiceViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType &&
                p.InvoiceDate >= periodeDate && invoice1.Contains(p.InvoiceNo) && invoice2.Contains(p.InvoiceNo));

            var result = queryable.FirstOrDefault(x => x.InvoiceNo == invoiceNo);
            return Json(new { invoice = result });
        }
    }
}
