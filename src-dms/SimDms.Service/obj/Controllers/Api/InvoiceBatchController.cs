using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using SimDms.Common.Models;

namespace SimDms.Service.Controllers.Api
{
    public class InvoiceBatchController : BaseController
    {
        //constant
        public const string GnMstDocumentINC = "INC";
        public const string GnMstDocumentINA = "INA";
        public const string GnMstDocumentINI = "INI";
        public const string GnMstDocumentINF = "INF";
        public const string GnMstDocumentINW = "INW";
        public const string GnMstDocumentINP = "INP";
        public const string GnMstDocumentFPS = "FPS";
        public const string GnMstDocumentSHN = "SHN";
        private string companyMD = "";
        private string branchMD = "";
        private string warehouseMD = "";

        // GET: /InvoiceBatch/

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

        public JsonResult InquiryInvoiceBatch(string groupJobType)
        {
            if (groupJobType == string.Empty) groupJobType = "ALL";

            DataSet ds = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvTrnInvInquiryBatch";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@GroupJobType", groupJobType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            if (ds.Tables.Count > 0)
            {
                var listJobOrder = GetJson(ds.Tables[0]);
                var listInvoice = GetJson(ds.Tables[1]);

                return Json(new { success = true, listJobOrder = listJobOrder, listInvoice = listInvoice });
            }
            else
            {
                return Json(new { success = false, message = "data not found" }); ;
            }
        }

        public JsonResult SaveInquiryInvoiceBatch(string groupJobType, string[] jobOrderNo, string remarks)
        {
            if (groupJobType == string.Empty) groupJobType = "ALL";

            DataSet ds = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvTrnInvInquiryBatch";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@GroupJobType", groupJobType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            if (ds.Tables.Count > 0)
            {
                var listJobOrder = GetJson(ds.Tables[0]);

                string[] listJobOrderNo = jobOrderNo[0].Split(',');

                for (int i = 0; i < listJobOrder.Count; i++)
                {
                    if (listJobOrderNo.Contains(listJobOrder[i]["JobOrderNo"].ToString()))
                    {
                        listJobOrder[i]["Remarks"] = remarks;
                        listJobOrder[i]["IsSelect"] = true;
                    }
                }

                var listInvoice = GetJson(ds.Tables[1]);

                return Json(new { success = true, listJobOrder = listJobOrder, listInvoice = listInvoice });
            }
            else
            {
                return Json(new { success = false, message = "data not found" });
            }
        }

        public JsonResult ProcessInvoiceBatch()
        {
            string[] JobOrderNumbers = (string.IsNullOrWhiteSpace(Request["JobOrderNumbers"]) ? new string[] { "" } : Request["JobOrderNumbers"].Split(','));
            string[] Remarks = (string.IsNullOrWhiteSpace(Request["Remarks"]) ? new string[] { "" } : Request["Remarks"].Split(','));
            int idx = 0;
            var user = CurrentUser;
            foreach (var JobOrderNo in JobOrderNumbers)
            {
                var service = ctx.Services.Where(m => m.JobOrderNo == JobOrderNo && m.BranchCode == user.BranchCode && m.CompanyCode == user.CompanyCode && m.ProductType == ProductType).FirstOrDefault();
                ProcessInvoice(service, Remarks[idx]);
                idx++;
            }
            return Json("");
        }

        private void ProcessInvoice(TrnService service, string remark)
        {
            try
            {
               var user = CurrentUser;
               var query = @"
            select BillType as BillType
              from svTrnSrvTask
             where CompanyCode = {0}
               and BranchCode  = {1}
               and ProductType = {2} 
               and ServiceNo   = {3}
            union
            select BillType as BillType
              from svTrnSrvItem b
             where CompanyCode = {0}
               and BranchCode  = {1}
               and ProductType = {2}
               and ServiceNo   = {3}
               and  (SupplyQty - ReturnQty) > 0
            "; 
                object[] parameters = { service.CompanyCode, service.BranchCode, service.ProductType, service.ServiceNo};
                var listBillType = ctx.Database.SqlQuery<string>(query, parameters);

                foreach (var billType in listBillType)
                {
                    ProcessInvoice(service, remark, billType);
                }

                //update service
                var invoices = ctx.Invoices.Where(m => m.CompanyCode == service.CompanyCode && m.BranchCode == service.BranchCode && m.ProductType == service.ProductType && m.JobOrderNo == service.JobOrderNo).ToList();
                if (invoices.Count == 1)
                {
                    service.InvoiceNo = invoices[0].InvoiceNo;
                }
                else
                {
                    var incInvoices = invoices.Where(m => m.InvoiceNo.Contains("INC")).ToList();
                    if (incInvoices.Count > 0)
                        service.InvoiceNo = incInvoices[0].InvoiceNo;
                }

                //update service
                try
                {
                    var oService = ctx.Services.Where(m => m.CompanyCode == service.CompanyCode && m.BranchCode == service.BranchCode && m.ProductType == service.ProductType && m.JobOrderNo == service.JobOrderNo).FirstOrDefault();
                    if (oService != null)
                        oService = service;
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when updating Service No, Message=" + ex.ToString());
                }

                //update Remark
                try
                {
                    var custVehicle = ctx.CustomerVehicles.Find(user.CompanyCode, service.ChassisCode, service.ChassisNo);
                    custVehicle.RemainderDescription = remark;
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when updating Customer Vehicle Reminder, Message=" + ex.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in function ProcessInvoice, Message=" + ex.ToString());
            }

        }

        public void ProcessInvoice(TrnService oService, string remark, string billType)
        {
            try
            {
                DateTime invDate = DateTime.Now;
                var user = CurrentUser;
                string InvoiceNo = ""; 
                switch (billType)
                {
                    case "C":
                        InvoiceNo = GetNewDocumentNo(GnMstDocumentINC, invDate);
                        break;
                    case "A":
                        InvoiceNo = GetNewDocumentNo(GnMstDocumentINA, invDate);
                        break;
                    case "I":
                        InvoiceNo = GetNewDocumentNo(GnMstDocumentINI, invDate);
                        break;
                    case "F":
                        InvoiceNo = GetNewDocumentNo(GnMstDocumentINF, invDate);
                        break;
                    case "W":
                        InvoiceNo = GetNewDocumentNo(GnMstDocumentINW, invDate);
                        break;
                    case "P":
                        InvoiceNo = GetNewDocumentNo(GnMstDocumentINP, invDate);
                        break;
                    default:
                        break;
                }

                //Create Invoice Document
                if (!string.IsNullOrWhiteSpace(InvoiceNo))
                {
                    //Create Invoice Document
                        ctx.Database.ExecuteSqlCommand("uspfn_SvTrnInvoiceCreate {0},{1},{2},{3},{4}, {5}, {6},{7}", user.CompanyCode, user.BranchCode, ProductType, oService.ServiceNo, billType, InvoiceNo, remark, user.UserId);
                }

                var _ctx = new DataContext();
                var _ctxMD = new MDContext();
                oService = _ctx.Services.Where(m => m.JobOrderNo == oService.JobOrderNo && m.BranchCode == user.BranchCode && m.CompanyCode == user.CompanyCode && m.ProductType == ProductType).FirstOrDefault();

                var oInvoice = _ctx.Invoices.Find(user.CompanyCode, user.BranchCode, ProductType, InvoiceNo);

                if (oInvoice.IsPKP.Value)
                {
                    var oLookup = _ctx.LookUpDtls.FirstOrDefault(m => m.CompanyCode == user.CompanyCode && m.CodeID == "SVFPJG" && m.LookUpValue == oInvoice.CustomerCodeBill);
                    if (billType == "F" || billType == "W" || oLookup != null)
                    {
                        oService.ServiceStatus = "7";
                        oInvoice.InvoiceStatus = "0";
                    }
                    else
                    {
                        oService.ServiceStatus = "9";
                        oInvoice.InvoiceStatus = "2";
                        oInvoice.FPJDate = DateTime.Now;
                        oInvoice.FPJNo = GetNewDocumentNo(GnMstDocumentFPS, invDate);
                        SaveFakturPajak(oInvoice);
                    }
                }
                else
                {
                    oService.ServiceStatus = "9";
                    oInvoice.InvoiceStatus = "1";
                    oInvoice.FPJDate = DateTime.Now;
                    oInvoice.FPJNo = GetNewDocumentNo(GnMstDocumentSHN, invDate);
                    SaveFakturPajak(oInvoice);
                }

                //updating Invoice
                ctx.SaveChanges(); 
                //Posting Invoice
                PostingInvoice(oInvoice);

                //if (!(DealerCode() == "MD"))
                //{
                //    SDMovement(oInvoice);
                //}
            }
            catch (Exception ex)
            {
                throw new Exception("Error in function ProcessInvoice, Message=" + ex.ToString());
            }

        }

        private void PostingInvoice(Invoice oInvoice)
        {
            try
            {
                var user = CurrentUser;
                ctx.Database.ExecuteSqlCommand("uspfn_SvTrnInvoicePosting {0},{1},{2},{3},{4}", user.CompanyCode, user.BranchCode, ProductType, oInvoice.InvoiceNo, user.UserId); 
                SaveBankBook(oInvoice);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in function PostingInvoice, Message="+ex.ToString());
            }
        }

        private void SaveBankBook(Invoice oInvoice)
        {
            ctx = new DataContext();
            ctxMD = new MDContext();
            try
            {
                var user = CurrentUser;
                var oBankBook =ctx.BankBooks.Find(CompanyCode, BranchCode, oInvoice.CustomerCodeBill, ProfitCenter);

                if (oBankBook == null)
                {
                    oBankBook = new BankBook();
                    oBankBook.CompanyCode = user.CompanyCode;
                    oBankBook.BranchCode = user.BranchCode;
                    oBankBook.CustomerCode = oInvoice.CustomerCodeBill;
                    oBankBook.ProfitCenterCode = ProfitCenter;
                    oBankBook.SalesAmt = oInvoice.TotalSrvAmt;
                    oBankBook.ReceivedAmt = 0;
                    ctx.BankBooks.Add(oBankBook);
                }
                else
                {
                    oBankBook.SalesAmt = oBankBook.SalesAmt + oInvoice.TotalSrvAmt;
                }
                ctx.SaveChanges(); 
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Process SaveBankBook, Message = "+ ex.ToString());
            }
        }

        private void SaveFakturPajak(Invoice oInvoice)
        {
            try
            {
                TaxInvoice oTaxInvoice = new TaxInvoice();
                oTaxInvoice.CompanyCode = oInvoice.CompanyCode;
                oTaxInvoice.BranchCode = oInvoice.BranchCode;
                oTaxInvoice.FPJNo = oInvoice.FPJNo;
                if (!oInvoice.IsPKP.Value) oTaxInvoice.FPJGovNo = oTaxInvoice.FPJNo;
                oTaxInvoice.FPJCentralNo = "";
                oTaxInvoice.NoOfInvoice = 1;
                oTaxInvoice.SignedDate = DateTime.Now;
                oTaxInvoice.FPJDate = DateTime.Now;
                oTaxInvoice.CustomerCode = oInvoice.CustomerCode;
                oTaxInvoice.CustomerCodeBill = oInvoice.CustomerCodeBill;
                var oCustomer = ctx.Customers.Find(oInvoice.CompanyCode, oInvoice.CustomerCodeBill);
                if (oCustomer != null)
                {
                    oTaxInvoice.CustomerName = oCustomer.CustomerGovName;
                    oTaxInvoice.Address1 = oCustomer.Address1;
                    oTaxInvoice.Address2 = oCustomer.Address2;
                    oTaxInvoice.Address3 = oCustomer.Address3;
                    oTaxInvoice.Address4 = oCustomer.Address4;
                    oTaxInvoice.PhoneNo = oCustomer.PhoneNo;
                    oTaxInvoice.HPNo = oCustomer.HPNo;
                    oTaxInvoice.IsPKP = oCustomer.isPKP.Value;
                    oTaxInvoice.SKPNo = oCustomer.SKPNo;
                    oTaxInvoice.SKPDate = oCustomer.SKPDate;
                    oTaxInvoice.NPWPNo = oCustomer.NPWPNo;
                    oTaxInvoice.NPWPDate = oCustomer.NPWPDate;

                }
                oTaxInvoice.TOPCode = oInvoice.TOPCode;
                oTaxInvoice.TOPDays = oInvoice.TOPDays;
                oTaxInvoice.DueDate = oInvoice.DueDate.Value;
                oTaxInvoice.PrintSeq = 0;
                oTaxInvoice.GenerateStatus = "0";
                oTaxInvoice.IsLocked = false;
                oTaxInvoice.CreatedBy = oInvoice.CreatedBy;
                oTaxInvoice.CreatedDate = oInvoice.CreatedDate;
                oTaxInvoice.LastupdateBy = oInvoice.LastupdateBy;
                oTaxInvoice.LastupdateDate = oInvoice.LastupdateDate;
                ctx.TaxInvoices.Add(oTaxInvoice);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in function SaveFakturPajak, Message=" + ex.ToString());
            }
        }

        private void SDMovement(Invoice oInvoice)
        {
            int index = 1;
            var srvNo = ctx.Services.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.JobOrderNo == oInvoice.JobOrderNo).ServiceNo;

            var datas = ctx.ServiceItems.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ServiceNo == srvNo).ToList();

            foreach (var data in datas)
            {
                var oSDMovement = new SvSDMovement();

                oSDMovement.CompanyCode = CompanyCode;
                oSDMovement.BranchCode = BranchCode;
                oSDMovement.DocNo = oInvoice.InvoiceNo;
                oSDMovement.DocDate = oInvoice.InvoiceDate;
                oSDMovement.PartNo = data.PartNo;
                oSDMovement.PartSeq = index++;
                oSDMovement.WarehouseCode = "00";
                oSDMovement.QtyOrder = data.DemandQty.Value;
                oSDMovement.Qty = data.DemandQty.Value;
                oSDMovement.DiscPct = data.DiscPct.Value;
                oSDMovement.CostPrice = data.CostPrice.Value;
                oSDMovement.RetailPrice = data.RetailPrice.Value;
                oSDMovement.TypeOfGoods = data.TypeOfGoods.Value.ToString();
                oSDMovement.CompanyMD = companyMD;
                oSDMovement.BranchMD = branchMD;
                oSDMovement.WarehouseMD = warehouseMD;
                oSDMovement.RetailPriceInclTaxMD = ctxMD.ItemPrices.FirstOrDefault(a => a.CompanyCode == CompanyMD && a.BranchCode == BranchMD && a.PartNo == data.PartNo).RetailPriceInclTax.Value;
                oSDMovement.RetailPriceMD = ctxMD.ItemPrices.FirstOrDefault(a => a.CompanyCode == CompanyMD && a.BranchCode == BranchMD && a.PartNo == data.PartNo).RetailPrice.Value;
                oSDMovement.CostPriceMD = ctxMD.ItemPrices.FirstOrDefault(a => a.CompanyCode == CompanyMD && a.BranchCode == BranchMD && a.PartNo == data.PartNo).CostPrice.Value;
                oSDMovement.QtyFlag = "x";
                oSDMovement.ProductType = ProductType;
                oSDMovement.ProfitCenterCode = ProfitCenter;
                oSDMovement.ProcessStatus = "0";
                oSDMovement.ProcessDate = DateTime.Now;
                oSDMovement.Status = "8";
                oSDMovement.CreatedBy = CurrentUser.UserId;
                oSDMovement.CreatedDate = DateTime.Now;
                oSDMovement.LastUpdateBy = CurrentUser.UserId;
                oSDMovement.LastUpdateDate = DateTime.Now;
                ctxMD.SvSDMovements.Add(oSDMovement);
                ctxMD.SaveChanges();
            }
        }
    }
}
