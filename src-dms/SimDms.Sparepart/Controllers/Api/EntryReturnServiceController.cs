using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using SimDms.Sparepart.Models;
using System.Transactions;


namespace SimDms.Sparepart.Controllers.Api
{
    public class EntryReturnServiceController : BaseController
    {
        private const string STATUS_OPEN = "OPEN RETURN";
        private const string STATUS_OUTSTANDING = "OUTSTANDING RETURN";
        private const string STATUS_POSTING = "POSTING RETURN";

        public JsonResult getReturnServiceDetail( SpTrnSRturSrvHdr model)
        {
            var status = "";
            var statusCode = "";
            var customerCode = "";
            IEnumerable<PartInvoiceReturn> table;
            var SpTrnSRturSrvHdr = ctx.SpTrnSRturSrvHdrs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.InvoiceNo == model.InvoiceNo).FirstOrDefault();
            if (SpTrnSRturSrvHdr != null)
            {
                statusCode = SpTrnSRturSrvHdr.Status;
            }
            else
            {
                statusCode = "0";
            }
            var InvoiceReturn = ctx.SpTrnSRturSrvHdrs.Where(a=>a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.InvoiceNo == model.InvoiceNo && a.Status == "2");

            if (InvoiceReturn.Count() == 0)
            {
                customerCode = ctx.SvTrnReturns.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.InvoiceNo == model.InvoiceNo).CustomerCode;
            }
            else
            {
                customerCode = InvoiceReturn.FirstOrDefault().CustomerCode;
            }

            if (InvoiceReturn.Count() > 0)
            {
                table = SelectPartInvoiceReturn(model.InvoiceNo);
                status = STATUS_POSTING;
            }
            else
            {
                table = SelectPartInvoice(model.InvoiceNo);
                if (statusCode == "1")
                {
                    status = STATUS_OUTSTANDING;
                }
                else
                {
                    status = STATUS_OPEN;
                }
            }

            var totalPartCost = table.Sum(a => a.CostPrice);
            var totalPartRetail = table.Sum(a => a.RetailPrice);

            var invoice = ctx.SvTrnInvoices.Find(CompanyCode,BranchCode,ProductType,model.InvoiceNo);
            var customer = ctx.GnMstCustomers.Find(CompanyCode, customerCode);
            var vehicle = ctx.SvTrnServices.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType && a.JobOrderNo == invoice.JobOrderNo);

            return Json(new { ReturnStatus = status, Table = table, Customer = customer, Vehicle = vehicle, Invoice = invoice, TotalPartCost = totalPartCost, TotalPartRetail = totalPartRetail, Status = statusCode });
        }

        public JsonResult OpenReturn(string InvoiceNo, List<SpTrnReturn>model)
        {
            bool result = false;
            string msg = "";
            var returnNo = ctx.SvTrnReturns.FirstOrDefault(a=>a.CompanyCode== CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType && a.InvoiceNo == InvoiceNo).ReturnNo;

            if (string.IsNullOrEmpty(returnNo))
            {
                msg = "Nomor return tidak diketemukan ! Harap periksa data return service anda !";
                return Json(new { success = false, message = msg });
            }

            using (var trx = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (model.Count() > 0)
                    {
                        foreach (SpTrnReturn recDtl in model)
                        {
                            if (recDtl.Check == 1)
                            {
                                InsertReturnService(InvoiceNo, returnNo, recDtl.PartNo);
                                msg = "Sukses posting return service";
                                result = true;
                            }
                        }
                    }
                    trx.Commit();
                }
                catch(Exception e)
                {
                    trx.Rollback();
                    msg = "Gagal posting return service";
                    result = false;
                }


                return Json(new { success = result, message = msg, InvoiceNo = InvoiceNo });
            }
        }

        private void InsertReturnService(string invoiceNo, string returnNo, string PartNo)
        {
            object[] parameters = { CompanyCode, BranchCode, CurrentUser.CoProfile.ProductType, invoiceNo, returnNo, CurrentUser.UserId, PartNo };
            var query = "exec uspfn_GenerateReturnServiceWeb {0},{1},{2},{3},{4},{5},{6}";
            //var query = "exec uspfn_GenerateReturnService {0},{1},{2},{3},{4},{5}";

            //if (DealerCode() == "MD")
            //{
            //    ctxMD.Database.ExecuteSqlCommand(query, parameters);
            //}
            //else
            //{
                ctx.Database.ExecuteSqlCommand(query, parameters);
            //}
        }

        private IEnumerable<PartInvoiceReturn> SelectPartInvoiceReturn(string invoiceNo)
        {
            var query = string.Format(@"SELECT
	                    row_number() over(order by a.ReturnNo, a.PartNo ASC) No
	                    , a.ReturnNo
	                    , a.PartNo
                        , (SELECT TOP 1 PartName FROM SpMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) PartName
	                    , a.CostPrice
	                    , a.RetailPrice
	                    , a.QtyReturn QtyBill
	                    , a.QtyReturn 
                        FROM SpTrnSRturSrvDtl a
                        WHERE a.CompanyCode = '{0}'
                          AND a.BranchCode = '{1}'
	                    	AND EXISTS
	                    	(	
	                    		SELECT ReturnNo FROM SpTrnSRturSrvHdr
	                    		WHERE CompanyCode = a.CompanyCode
	                    			AND BranchCode = a.BranchCode
                                  AND ReturnNo = a.ReturnNo 
	                    			AND InvoiceNo = '{2}'
	                    	)", CompanyCode, BranchCode, invoiceNo);

            return  ctx.Database.SqlQuery<PartInvoiceReturn>(query);
        }

        private IEnumerable<PartInvoiceReturn> SelectPartInvoice(string invoiceNo)
        {
            string query = string.Format(@"SELECT
                	row_number() over(order by a.PartNo ASC) No
                	, '' ReturnNo
                	, a.PartNo    
                    , (SELECT TOP 1 PartName FROM SpMstItemInfo WHERE CompanyCode = '{0}' AND PartNo = a.PartNo) PartName
                	, a.CostPrice
                	, a.RetailPrice
                	, a.SupplyQty QtyBill
                	, a.SupplyQty QtyReturn
                    FROM SvTrnInvItem a
                    WHERE a.CompanyCode = '{0}'
                	AND a.BranchCode = '{1}'
                	AND a.InvoiceNo = '{2}'
                    AND a.PartNo NOT IN (SELECT PartNo FROM spTrnSRturSrvHdr z
                					INNER JOIN spTrnSRturSrvDtl y
                						ON z.CompanyCode = y.CompanyCode AND z.BranchCode = y.BranchCode AND z.ReturnNo = y.ReturnNo
                					WHERE y.CompanyCode = '{0}' AND y.BranchCode = '{1}' AND z.InvoiceNo = '{2}')
                ", CompanyCode, BranchCode, invoiceNo);

            return ctx.Database.SqlQuery<PartInvoiceReturn>(query);
        }


    }
}
