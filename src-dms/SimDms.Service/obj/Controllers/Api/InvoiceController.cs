using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using System.Transactions;

using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Service.Controllers.Api
{
    public class InvoiceController : BaseController
    {
        //
        // GET: /Invoice/
        private string err = "Proses pembuatan faktur gagal ";
        private string companyMD = "";
        private string branchMD = "";
        private string warehouseMD = "";
        private string ProfitCenter = "200";

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                ServiceType = 2,
                JobOrderDate = DateTime.Now,
                Recommendation = "TERIMA KASIH ATAS KEPERCAYAAN ANDA TELAH MELAKUKAN PERAWATAN DAN PERBAIKAN DI BERES KAMI.\nSERVIS BERIKUT: \nSARAN SERVIS: \n \nTERIMA KASIH"
            });
        }

        public JsonResult LookUpInvoice()
        {
            string InvoiceNo = Request["InvoiceNo"] != null ? Request["InvoiceNo"].ToString() : "";
            string JobOrderNo = Request["JobOrderNo"] != null ? Request["JobOrderNo"].ToString() != "undefined" ? Request["JobOrderNo"].ToString() : "" : "";
            string FPJNo = Request["FPJNo"] != null ? Request["FPJNo"].ToString() : "";

            string dynamicFilter = "";
            dynamicFilter = InvoiceNo != "" ? " AND InvoiceNo LIKE ''%" + InvoiceNo + "%'" : "";
            if(dynamicFilter == ""){
                dynamicFilter += JobOrderNo != "" ? " AND JobOrderNo LIKE ''%" + JobOrderNo + "%'" : "";
            }
            else
            {
                dynamicFilter += JobOrderNo != "" ? "' AND JobOrderNo LIKE ''%" + JobOrderNo + "%'" : "";
            }
            if (dynamicFilter == "")
            {
                dynamicFilter += FPJNo != "" ? " AND FPJNo LIKE ''%" + FPJNo + "%'" : "";
            }
            else
            {
                dynamicFilter += FPJNo != "" ? "' AND FPJNo LIKE ''%" + FPJNo + "%'" : "";
            }

            dynamicFilter = dynamicFilter != "" ? dynamicFilter += "'" : "";
            string sql = string.Format(@"EXEC uspfn_svInvoiceForLookUp '{0}', '{1}', '{2}', '{3}'",
                CompanyCode, BranchCode, ProductType, dynamicFilter);

            var records = ctx.Database.SqlQuery<InvoiceGetView>(sql).AsQueryable();

            return Json(records.toKG());
        }

        public JsonResult Get(string jobOrderNo)
        {
            // penambahan InvoiceNo di spnya
            DataTable dt = new DataTable();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvTrnInvoiceDraft";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@JobOrderNo", jobOrderNo);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            var list = GetJson(dt);

            string invoiceNo = "";
            var rec = ctx.Services.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType && e.JobOrderNo == jobOrderNo);
            if(rec != null){
                invoiceNo = rec.InvoiceNo;
            }
            else
            {
                return Json(new { success = false, message = "No SPK \"" + jobOrderNo+ "\" tidak ditemukan" });
            }

            var btnproses = false;

            if (rec.ServiceStatus == "5")
            {
                if (rec.JobType.Contains("PDI") || rec.JobType.Contains("FS"))
                {
                    if (rec.IsLocked == false)
                    {
                        if (rec.InvoiceNo == "")
                        {
                            btnproses = true;
                        }
                        else
                            btnproses = false;
                    }
                    else
                    {
                        btnproses = true;
                    }
                }
                else
                {
                    btnproses = true;
                }
            }
            else
            {
                btnproses = false;
            }

            if (list != null)
            {
                list[0]["InvoiceNo"] = invoiceNo;
                return Json(new { success = true, list = list.FirstOrDefault(), btnproses = btnproses });
            }
            else
            {
                return Json(new { success = false, message = "No SPK \"" + jobOrderNo + "\" tidak ditemukan" });
            }
        }

        public JsonResult GetTable(string invoiceNo, string jobOrderNo, string JobType, long serviceNo)
        {
            string billType = "";

            if (string.IsNullOrEmpty(invoiceNo))
            {
                if (JobType.StartsWith("FSC01") || JobType.StartsWith("PDI")) billType = "F";
                else if (JobType.StartsWith("CLAIM")) billType = "W";
                else
                {
                    billType = GetFirstBillType(serviceNo);
                }
            }
            else
            {
                billType = invoiceNo.Substring(2, 1);
            }

            DataTable dt = new DataTable();
            SqlCommand cmd =  ctx.Database.Connection.CreateCommand() as SqlCommand ; 
            cmd.CommandText = "uspfn_SvTrnInvoiceSelectDtl";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@JobOrderNo", jobOrderNo);
            cmd.Parameters.AddWithValue("@BillType", billType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            var list = GetJson(dt);

            if (list != null)
            {
                return Json(new { success = true, list = list, bill = billType });
            }
            else
            {
                return Json(new { success = false, message = "Tidak terdapat No. SPK" }); ;
            }

        }

        private string GetFirstBillType(long ServiceNo)
        {
            var query = "exec uspfn_SvGetFirstBillType @p0,@p1,@p2,@p3";
            object[] parameters = { CompanyCode, BranchCode, ProductType, ServiceNo };
            var billType = ctx.Database.SqlQuery<string>(query, parameters).FirstOrDefault();

            return billType;
        }

        public JsonResult ProcessInvoice(string jobOrderNo, string remark)
        {
            companyMD = CompanyMD;
            branchMD = BranchMD;
            warehouseMD = WarehouseMD;

            try
            {
                var record = ctx.Services.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType && e.ServiceType == "2" && e.JobOrderNo == jobOrderNo);
                
                if (record == null) throw new Exception(err + ", data tidak ditemukan");

                var cust=ctx.Customers.Find(CompanyCode,record.CustomerCode);
                if(cust!= null)
                {
                    if(cust.isPKP==null)
                    {
                         throw new Exception("Faktur Pajak Customer belum disetting");
                    }
                }
                else
                {
                    throw new Exception("Customer Tidak ditemukan");
                }

                

                //Check for Fiscal Year and Fiscal Month of Profile service to current Date
                if (!DateTransValidation()) throw new Exception(err + ", Tanggal transaksi tidak sesuai dengan periode transaksi");

                //Check for Document Year to Current Date
                var doc = ctx.Documents.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.DocumentType == "SPK");

                if (doc == null) throw new Exception(err + ", Master Document SPK tidak ditemukan");
                if (doc.DocumentYear != DateTime.Now.Year) throw new Exception(err + ", Apakah sudah dilakukan Tutup Tahun / Bulan? Tahun Fiscal, Tahun Transaksi");

                //Check Concorenccy data based on status.
                if (record.ServiceStatus == "7" || record.ServiceStatus == "9")
                    throw new Exception(err + ", Doc no " + record.JobOrderNo + " sudah di Faktur oleh " + record.LastUpdateBy);

                //Check Concorenccy data based on status.
                if (record.ServiceStatus != "5") throw new Exception(string.Format(err + ", Proses tidak dapat dilanjutkan karena status SPK sudah berubah"));

                var outQuery = @"select
                                    CompanyCode
                                   ,BranchCode
                                   ,ReturnNo
                                   ,SKPNo 
                                   ,SPKDate 
                                   from spTrnSRturSSHdr
                                   where Status < '2'
                                   and CompanyCode = '{0}'
                                   and BranchCode  = '{1}'
                                   and SKPNo       = '{2}'";

                object[] outParams = { CompanyCode, BranchCode, record.JobOrderNo };
                var outstanding = ctx.Database.SqlQuery<Outstanding>(outQuery, outParams);
                if (outstanding.Count() > 0)
                    throw new Exception(err + ", Proses tidak dapat dilanjutkan karena masih ada " + outstanding.Count().ToString() + " outstanding Supply Slip return");

                if (!CheckSubConPrice(record.ServiceNo))
                    throw new Exception(err + ", Harga Sub-Con lebih besar dari nilai yang ditagihkan. Mohon diubah nilai yang ditagih di master pekerjaan...");

                var recordItem =  ctx.ServiceItems.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType && p.ServiceNo == record.ServiceNo);
                string billType = "F";
                if (recordItem.Count() > 0)
                {
                    foreach (var item in recordItem)
                    {
                        if (item.BillType != billType)
                        {
                            billType = item.BillType;
                            break;
                        }
                    }
                    if (billType == "F")
                    {
                        if (!record.IsLocked)
                            throw new Exception("Proses Invoice tidak bisa dilanjutkan, silahkan lakukan Approve PDI/FSC Claim terlebih dahulu");
                    }
                }
                else
                {
                    var recordTask = ctx.ServiceTasks.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType && p.ServiceNo == record.ServiceNo);
                    if (recordTask.Count() > 0)
                    {
                        foreach (var item in recordTask)
                        {
                            if (item.BillType != billType)
                            {
                                billType = item.BillType;
                                break;
                            }
                        }
                        if (billType == "F")
                        {
                            if (!record.IsLocked)
                                throw new Exception("Proses Invoice tidak bisa dilanjutkan, silahkan lakukan Approve PDI/FSC Claim terlebih dahulu");
                        }
                    }
                }

                //using (TransactionScope trans = new TransactionScope())
                //{
                    if (!Process(record, record.ServiceNo, remark))
                        throw new Exception(err);
                    //trans.Complete();
                //}

                var invoiceNo = ctx.Services.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType && e.JobOrderNo == jobOrderNo).InvoiceNo;
                
                return Json(new { Success = true, Message = "Process invoice berhasil", invNo = invoiceNo});
            }
            catch (Exception ex)
            {

                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult GetDataByJobOrder(string SpkNo)
        {
            var record = ctx.InvoiceViews.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType && e.JobOrderNo == SpkNo);
            var count = record.Count();
            if (count != 0)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult GetDataByInvNo(string InvNo)
        {
            var record = ctx.InvoiceViews.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType && e.InvoiceNo == InvNo).Select(p => new
            {
                JobOrderNo = p.JobOrderNo
            });
            var count = record.Count();
            if (count != 0)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        #region Validation

        private bool DateTransValidation()
        {
            // 200 : Check for Service 
            var oService = ctx.CoProfileServices.Find(CompanyCode, BranchCode);
            var date = DateTime.Now.Date;

            if (oService != null)
            {
                if (oService.TransDate.Equals(DBNull.Value) || oService.TransDate < new DateTime(1900, 1, 2)) oService.TransDate = DateTime.Now.Date;
                if (date >= oService.PeriodBeg.Date && date <= oService.PeriodEnd.Date)
                {
                    if (date <= DateTime.Now.Date)
                    {
                        if (date >= oService.TransDate.Value.Date)
                        {
                            if (oService.isLocked == true)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private bool CheckSubConPrice(long serviceNo)
        {
            object[] parameters = { CompanyCode, BranchCode, ProductType, serviceNo };
            
            bool bReturn = false;

            var decSumQuery = @"
select 
	isnull(sum (OperationHour * OperationCost), 0) 
from svTrnSrvTask 
	where ServiceNo = {3} and 
	PONo != '' and
    CompanyCode = {0} and
    BranchCode = {1} and
    ProductType = {2} and 
    IsSubCon = 1
";
            var decSumTagih = ctx.Database.SqlQuery<decimal>(decSumQuery, parameters).FirstOrDefault();

            var decSubConPriceQuery= @"
select 
	isnull(sum (SubConPrice), 0) 
from svTrnSrvTask 
	where ServiceNo = {3} and 
	PONo != '' and
    CompanyCode = {0} and
    BranchCode = {1} and
    ProductType = {2}
";
            
          var decSubConPrice = ctx.Database.SqlQuery<decimal>(decSubConPriceQuery, parameters).FirstOrDefault();
         
          if (decSumTagih >= decSubConPrice) bReturn = true;

          return bReturn;
        }

        #endregion

        #region Process Invoice

        private bool Process(TrnService service, long serviceNo, string remark)
        {
            string query = string.Format(@"
            select BillType as BillType
              from svTrnSrvTask
             where CompanyCode = {0}
               and BranchCode  = {1}
               and ProductType = '{2}' 
               and ServiceNo   = {3}
            union
            select BillType as BillType
              from svTrnSrvItem b
             where CompanyCode = {0}
               and BranchCode  = {1}
               and ProductType = '{2}'
               and ServiceNo   = {3}
               and  (SupplyQty - ReturnQty) > 0
            ",CompanyCode, BranchCode, ProductType, serviceNo );

            //object[] parameters = { CompanyCode, BranchCode, ProductType, serviceNo };
            var billTypes = ctx.Database.SqlQuery<string>(query);
            bool result = true;

            try
            {
                for (int i = 0; i < billTypes.Count(); i++)
                {
                    Process(billTypes.ElementAt(i), service, remark);
                }

                var invoiceNoList = ctx.Invoices.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.ProductType == ProductType && e.JobOrderNo == service.JobOrderNo);

                if (invoiceNoList.Count() == 1) { service.InvoiceNo = invoiceNoList.FirstOrDefault().InvoiceNo; }
                if (invoiceNoList.Count() > 1)
                {
                    invoiceNoList = invoiceNoList.Where(e => e.InvoiceNo.StartsWith("INC"));
                    if (invoiceNoList.Count() > 0) { service.InvoiceNo = invoiceNoList.FirstOrDefault().InvoiceNo; }
                }
                else if (invoiceNoList.Count() == 0) result = false;
                ctx.SaveChanges();

                // update Remarks on SvMstCustomerVehicle
                var cv = new CustomerVehicle();
                    cv = ctx.CustomerVehicles.Find(CompanyCode, service.ChassisCode, service.ChassisNo);
                    cv.RemainderDescription = remark;
                    ctx.SaveChanges();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private bool Process(string billType, TrnService oService, string remark)
        {
            bool result = false;

            string InvoiceNo = ""; 

            switch (billType)
            {
                case "C":
                    if (string.IsNullOrEmpty(oService.InvoiceNo))
                        InvoiceNo = GetNewDocumentNo("INC", DateTime.Now);
                    break;                       
                case "A":
                    InvoiceNo = GetNewDocumentNo("INA", DateTime.Now);
                    break;                     
                case "I":
                    InvoiceNo = GetNewDocumentNo("INI", DateTime.Now);
                    break;                       
                case "F":
                    if (oService.JobType.Contains("FS") || oService.JobType.Contains("PDI"))
                    {
                        if (oService.IsLocked == true && oService.ServiceStatus == "5")
                            InvoiceNo = GetNewDocumentNo("INF", DateTime.Now);
                    }
                    break;                       
                case "W":
                    InvoiceNo = GetNewDocumentNo("INW", DateTime.Now);
                    break;                     
                case "P":
                    InvoiceNo = GetNewDocumentNo("INP", DateTime.Now);
                    break;
                default:
                    break;
            }

            if (InvoiceNo != "")
            {
                string query = "exec uspfn_SvTrnInvoiceCreate  @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7";
                object[] parameters = { CompanyCode, BranchCode, ProductType, oService.ServiceNo, billType, InvoiceNo, remark, CurrentUser.UserId };

                ctx.Database.ExecuteSqlCommand(query, parameters);
            }

            var oInvoice = new Invoice();
            oInvoice = ctx.Invoices.Find(CompanyCode, BranchCode, ProductType, InvoiceNo); 

            if ((oService.JobType.Contains("FS") || oService.JobType.Contains("PDI")) && billType == "F")
            {
                var lookupDtl = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "SRV_FLAG" && p.LookUpValue == "CLM_MODE").FirstOrDefault();
                if (lookupDtl != null)
                {
                    if (lookupDtl.ParaValue == "1")
                    {
                        result = UpdateInvoice(oInvoice, oService, billType);
                    }
                    else
                    {
                        if (oService.IsLocked == false)
                        {
                            // Do nothing
                        }
                        if (oService.IsLocked == true && oService.ServiceStatus == "5")
                        {
                            result = UpdateInvoice(oInvoice, oService, billType);
                        }
                    }
                }
            }
            else
            {
                if ((oService.JobType.Contains("FS") || oService.JobType.Contains("PDI")) && oService.IsLocked == true && billType == "C")
                {
                    // Do Nothing
                }
                else
                    result = UpdateInvoice(oInvoice, oService, billType);
            }

            //SDMovement 
            //if (!(DealerCode() == "MD"))
            //{
            //    SDMovement(oInvoice);
            //}

            return result;
        }

        private bool UpdateInvoice(Invoice oInvoice, TrnService oService, string billType)
        {
            bool result = false;
            if (oInvoice.IsPKP.Value)
            {
                var oLookup = ctx.LookUpDtls.Find(CompanyCode, "SVFPJG", oInvoice.CustomerCodeBill);

                if (billType == "F" || billType == "W" || oLookup != null)
                {
                    if (oService.JobType.Contains("PDI") || oService.JobType.Contains("FS"))
                    {
                        if (oService.IsLocked == false)
                            oService.ServiceStatus = "5";
                        else
                            oService.ServiceStatus = "7";
                    }
                    else 
                        oService.ServiceStatus = "7";

                    oInvoice.InvoiceStatus = "0";
                }
                else
                {
                    if ((oService.JobType.Contains("PDI") || oService.JobType.Contains("FS")) && billType == "C")
                    {
                        if (oService.IsLocked == false)
                            oService.ServiceStatus = "5";
                        else
                            oService.ServiceStatus = "9";
                    }
                    else oService.ServiceStatus = "9";

                    oInvoice.InvoiceStatus = "2";
                    oInvoice.FPJDate = DateTime.Now;
                    oInvoice.FPJNo = GetNewDocumentNo("FPS", oInvoice.FPJDate.Value);
                    SaveFakturPajak(oInvoice);
                }
            }
            else
            {
                if ((oService.JobType.Contains("PDI") || oService.JobType.Contains("FS")) && billType == "C")
                {
                    if (oService.IsLocked == false)
                        oService.ServiceStatus = "5";
                    else
                        oService.ServiceStatus = "9";
                }
                else oService.ServiceStatus = "9";

                oInvoice.InvoiceStatus = "1";
                oInvoice.FPJDate = DateTime.Now;
                oInvoice.FPJNo = GetNewDocumentNo("SHN", oInvoice.FPJDate.Value);
                SaveFakturPajak(oInvoice);
            }

            // Proses posting.
            return result = PostingInvoice(oInvoice);
        }

        private void SaveFakturPajak(Invoice oInvoice)
        {
            try
            {
                TaxInvoice taxInvoice = new TaxInvoice();

                taxInvoice.CompanyCode = oInvoice.CompanyCode;
                taxInvoice.BranchCode = oInvoice.BranchCode;
                taxInvoice.FPJNo = oInvoice.FPJNo;
                if (!oInvoice.IsPKP.Value) taxInvoice.FPJGovNo = taxInvoice.FPJNo;
                taxInvoice.FPJCentralNo = "";
                taxInvoice.NoOfInvoice = 1;
                taxInvoice.SignedDate = DateTime.Now;
                taxInvoice.FPJDate = DateTime.Now;
                taxInvoice.CustomerCode = oInvoice.CustomerCode;
                taxInvoice.CustomerCodeBill = oInvoice.CustomerCodeBill;
                var customer = ctx.Customers.Find(CompanyCode, oInvoice.CustomerCodeBill);
                if (customer != null)
                {
                    taxInvoice.CustomerName = customer.CustomerGovName;
                    taxInvoice.Address1 = customer.Address1;
                    taxInvoice.Address2 = customer.Address2;
                    taxInvoice.Address3 = customer.Address3;
                    taxInvoice.Address4 = customer.Address4;
                    taxInvoice.PhoneNo = customer.PhoneNo;
                    taxInvoice.HPNo = customer.HPNo;
                    taxInvoice.IsPKP = customer.isPKP.Value;
                    taxInvoice.SKPNo = customer.SKPNo;
                    taxInvoice.SKPDate = customer.SKPDate;
                    taxInvoice.NPWPNo = customer.NPWPNo;
                    taxInvoice.NPWPDate = customer.NPWPDate;

                }
                taxInvoice.TOPCode = oInvoice.TOPCode;
                taxInvoice.TOPDays = oInvoice.TOPDays;
                taxInvoice.DueDate = oInvoice.DueDate.Value;
                taxInvoice.PrintSeq = 0;
                taxInvoice.GenerateStatus = "0";
                taxInvoice.IsLocked = false;
                taxInvoice.CreatedBy = oInvoice.CreatedBy;
                taxInvoice.CreatedDate = oInvoice.CreatedDate;
                taxInvoice.LastupdateBy = oInvoice.LastupdateBy;
                taxInvoice.LastupdateDate = oInvoice.LastupdateDate;
                ctx.TaxInvoices.Add(taxInvoice);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        private bool PostingInvoice(Invoice oInvoice)
        {
            string query = "exec uspfn_SvTrnInvoicePosting @p0,@p1,@p2,@p3,@p4";
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

        private void SDMovement(Invoice oInvoice)
        {
            try
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
                    oSDMovement.RetailPriceInclTaxMD = ctxMD.ItemPrices.FirstOrDefault(a => a.CompanyCode == companyMD && a.BranchCode == branchMD && a.PartNo == data.PartNo).RetailPriceInclTax.Value;
                    oSDMovement.RetailPriceMD = ctxMD.ItemPrices.FirstOrDefault(a => a.CompanyCode == companyMD && a.BranchCode == branchMD && a.PartNo == data.PartNo).RetailPrice.Value;
                    oSDMovement.CostPriceMD = ctxMD.ItemPrices.FirstOrDefault(a => a.CompanyCode == companyMD && a.BranchCode == branchMD && a.PartNo == data.PartNo).CostPrice.Value;
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
            catch (Exception ex)
            {
                throw new Exception(err);
            }
        }
        #endregion
    }
}
