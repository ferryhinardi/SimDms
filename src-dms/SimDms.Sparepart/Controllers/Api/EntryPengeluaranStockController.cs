using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using SimDms.Sparepart.Models;
using SimDms.Common.Models;
using System.Data;

namespace SimDms.Sparepart.Controllers.Api
{
    public class EntryPengeluaranStockController : BaseController
    {
        private bool enablePrint = true;
        //private bool success = false;
        private const string SUBSIGN_CODE_SALES = "SA-PJUAL";
        private const string SUBSIGN_CODE_NON_SALES = "SA-NPJUAL";
        private const string BOSUBSIGN_CODE_SALES = "BR-PJUAL";
        private const string BOSUBSIGN_CODE_NON_SALES = "BR-NPJUAL";
        private string msg = "";
        private bool md = false;
        private const string ProfitCenter = "300";
        private const string ProfitCenterName = "SPARE PART";

        public JsonResult Default()
        {
            
            return Json(new { DocDate = ctx.CurrentTime});
        }

        public JsonResult DefaultNonSales()
        {
            var lookUpTREX = ctx.LookUpDtls.Find(CompanyCode, "TREX", "STATUS");
            bool isTrex = false;
            if (lookUpTREX != null)
            {
                if (lookUpTREX.ParaValue == "1") isTrex = true;
            }
            return Json(new { isTrex = isTrex });
        }

        public JsonResult GetPartName(string PartNo, string SONo)
        {
            string qry = String.Empty;
            qry = "select PartNo from omTrSalesSOAccsSeq a where a.Sono='" + SONo + "' and " +
                  "a.PartNo in (select c.PartNo from spTrnSORDHdr b inner join SpTrnSORDDtl c on (b.DocNo = c.DocNo) where b.UsageDocNo=a.SONo) and a.PartNo='" + PartNo + "'";
            
            
            
            
            //var partExist = (from a in ctx.SpTrnSORDDtls
            //                 where a.CompanyCode == CompanyCode
            //                     && a.BranchCode == BranchCode && a.PartNo == PartNo
            //                 select a.PartNo).FirstOrDefault();

            string partExist = ctx.Database.SqlQuery<string>(qry).FirstOrDefault();
            if (partExist != null)
            {
                return Json(new { success = false, exist = true, message = "Part sudah ada di list pengeluaran!" });
            }
           
            var PartName = (from a in ctx.SpMstItemInfos
                             where a.CompanyCode == CompanyCode
                             && a.PartNo == PartNo
                             select a.PartName
                                 ).FirstOrDefault();

            var itemPrice = ctx.spMstItemPrices.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.PartNo == PartNo).FirstOrDefault();

            if (PartName != null && itemPrice != null)
                return Json(new { success = true, data = new {iName = PartName, iPrice = itemPrice.RetailPrice}, message="" } );
            else
                return Json(new { success = false, data = "", message="Part tidak ditemukan!" });
        }

        public JsonResult Save(SpTrnSORDHdr model, SalesPartLookUp emp, bool isExternal)
        {
            md = DealerCode() == "MD";
            if (!IsValidStatus(model.DocNo)) { return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" }); }

            if (string.IsNullOrEmpty(CurrentUser.TypeOfGoods)) { return Json(new { success = false, message = "Type Of Goods User Belum Di Setting !" }); }

            if (!isCheckStatus(model.DocNo)) { return Json(new { success = false, message = "Status Transaksi sudah di alokasi, data tidak dapat diupdate" }); }

            var dtv = DateTransValidation(model.DocDate.Value);
            if (dtv != "") { return Json(new { success = false, message = dtv }); }

            string taxCode = ctx.ProfitCenters.Find(CompanyCode, BranchCode, model.CustomerCode, ProfitCenter).TaxCode;

            var record = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, model.DocNo) ;
            var recProfit = ctx.ProfitCenters.Find(CompanyCode, BranchCode, model.CustomerCode, ProfitCenter);

            if (model.IsPORDD.Value)
            {
                model.TransType = ctx.LookUpDtls.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.CodeID == "TTPJ" && a.LookUpValueName == model.TransType).LookUpValue;
                var utlRec = ctx.SpTrnSORDHdrs.Where(a =>
                    a.CompanyCode == CompanyCode
                    && a.BranchCode == BranchCode
                    && a.CustomerCode == model.CustomerCode
                    && a.OrderNo == model.OrderNo);

                if (utlRec == null) { return Json(new { success = false }); }
                if (!CheckCustomerShipBill(model.CustomerCodeBill, model.CustomerCodeShip)) { return Json(new { success = false }); }
                if (recProfit == null) { return Json(new { success = false, message = string.Format("Customer tidak memiliki profit center") }); }
            }

            if (model.SalesType == "2")
            {
                // validate
                var outstandingSSNo = ctx.SpTrnSORDHdrs.Where(
                    a => a.CompanyCode == CompanyCode
                    && a.BranchCode == BranchCode
                    && a.UsageDocNo == model.UsageDocNo
                    && a.SalesType == model.SalesType).ToList().FirstOrDefault(a => Convert.ToInt32(a.Status) < 2);

                if (outstandingSSNo != null && outstandingSSNo.DocNo != model.DocNo)
                { return Json(new { success = false, message = string.Format("Pastikan status No. Supply Slip : {0} dengan No. SPK : \n{1} adalah CLOSED atau CANCEL", outstandingSSNo.DocNo, model.UsageDocNo) }); }

                if (recProfit == null) { return Json(new { success = false, message = string.Format("Pelanggan ini belum terdaftar di profit center {0}.", ProfitCenterName) }); }
                if (string.IsNullOrEmpty(recProfit.TaxCode)) { return Json(new { success = false, message = string.Format("Kode pajak pelanggan ini belum terisi di profit center {0}.", ProfitCenterName) }); }

                isOverdueOrderExist(model.CustomerCode, recProfit);
            }

            if (model.SalesType == "3")
            {
                if (recProfit == null)
                {
                    msg = string.Format("Proses pembuatan Draft Pesanan Sparepart gagal \nPastikan pelanggan ini telah terdaftar di profit center {0}.", ProfitCenterName);
                    return Json(new { success = false, message = msg });
                }

                //// Check if tax code is empty
                if (string.IsNullOrEmpty(recProfit.TaxCode))
                {
                    msg = string.Format("Proses pembuatan Draft Pesanan Sparepart gagal \nPastikan pelanggan ini memiliki kode pajak untuk profit center {0}", ProfitCenterName);
                    return Json(new { success = false, message = msg });
                }

                var recLookUp = ctx.LookUpDtls.Find(CompanyCode, "TOPC", recProfit.TOPCode);
                if (recLookUp == null)
                {
                    msg = string.Format("Proses pembuatan Draft Pesanan Sparepart \nPastikan kode TOP {0} telah tersimpan pada Master Look Up", recProfit.TOPCode);
                    return Json(new { success = false, message = msg });
                }

                // Check for Overdue Allowed
                if (!recProfit.isOverDueAllowed.Value && CurrentUser.CoProfile.IsLinkToFinance.Value && int.Parse(recLookUp.ParaValue) > 1 &&
                    isOverdueOrder(model.CustomerCode))
                {
                    msg = string.Format("Proses pembuatan Draft Pesanan Sparepart gagal \nTransasi tidak dapat dilanjutkan karena ada transaksi sebelumnya yang telah jatuh tempo");
                    return Json(new { success = false, message = msg });
                }

                var recTax = ctx.Taxes.Find(CompanyCode, recProfit.TaxCode);
                if (recTax == null)
                {
                    msg = string.Format("Proses pembuatan Draft Pesanan Sparepart \nPastikan kode pajak {0} telah tersimpan di Master Pajak", recProfit.TaxCode);
                    return Json(new { success = false, message = msg });
                }
            }
            try
            {
                var docType = model.SalesType == "0" ? "SOC" : model.SalesType == "1" ? "BPS" : model.SalesType == "2" ? "SSS" : "SSU";
                if (record == null)
                {
                    record = new SpTrnSORDHdr
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        DocNo = GetNewDocumentNo(docType, model.DocDate.Value),
                        DocDate = model.DocDate,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now
                    };
                    ctx.SpTrnSORDHdrs.Add(record);
                }
                record.UsageDocNo = model.SalesType == "2" || model.SalesType == "3" ? model.UsageDocNo : "";
                record.UsageDocDate = model.SalesType == "2" || model.SalesType == "3" ? model.UsageDocDate : null;
                record.CustomerCode = model.CustomerCode;
                record.CustomerCodeBill = model.CustomerCodeBill;
                record.CustomerCodeShip = model.CustomerCode;
                record.isBO = model.IsPORDD.Value ? true : model.isBO;
                record.isSubstitution = model.IsPORDD.Value ? true : model.isSubstitution;
                record.TransType = model.TransType;
                record.SalesType = model.SalesType;
                record.IsPORDD = model.IsPORDD;
                record.OrderNo = model.OrderNo;
                record.OrderDate = model.OrderDate;
                record.TOPCode = model.TOPCode;
                record.TOPDays = model.TOPDays;
                record.PaymentCode = model.SalesType == "1" ? "" : GetPaymentValue(model.PaymentCode);
                record.PaymentRefNo = model.PaymentRefNo;
                record.Status = enablePrint ? "0" : "1";
                record.TypeOfGoods = TypeOfGoods;
                record.LockingBy = emp.EmployeeID != "" ? emp.EmployeeID : string.Empty;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
                record.isDropSign = false;
                record.TotSalesQty = 0;
                record.TotSalesAmt = 0;
                record.TotDiscAmt = 0;
                record.TotDPPAmt = 0;
                record.TotPPNAmt = 0;
                record.TotFinalSalesAmt = record.TotDPPAmt + record.TotPPNAmt;
                record.PrintSeq = 0;

                record.isIncludePPN = model.IsPORDD.Value ? true  : isExternal ? true : GetIsPPNPct(taxCode);
                record.isPKP = model.IsPORDD.Value ? true : isExternal ? true : GetIsPKP(model.CustomerCode);
                record.isLocked = isExternal ? true : false;

                ctx.SaveChanges();

                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Simpan Data Gagal!!! " + ex.Message });
            }
        }

        public JsonResult Delete(SpTrnSORDHdr model, SalesPartLookUp emp, string saveType)
        {
            if (!IsValidStatus(model.DocNo)) { return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" }); }

            if (!isCheckStatus(model.DocNo)) { return Json(new { success = false, message = "Status Transaksi sudah di alokasi, data tidak dapat diupdate" }); }

            if (GetPartTable(model.DocNo).Count() > 0) { return Json(new { success = false, message = "Data tidak dapat dihapus karena memiliki data detail" }); }

            var record = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, model.DocNo);
            if (record != null)
            {
                record.PrintSeq += 1;
                record.Status = "3";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
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

        public JsonResult ConfirmBO(SpTrnSORDDtl modelDtl)
        {
            var Hdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, modelDtl.DocNo);
            var CustomerCode = Hdr.CustomerCode;

            decimal qtyBO = GetBOQuantityByCustomer(CustomerCode, modelDtl.PartNo);
            if (qtyBO > 0) //{ return Json(new { success = false, status = "BO", message = "Customer ini mempunyai BO Part ini sejumlah " + qtyBO.ToString() + "\n Apakah mau dilanjutkan ??" }); }
                return Json(new { success = false, QtyBO = qtyBO });
            else
                return Json(new { success = true });
        }

        public JsonResult ValidatePart(SpTrnSORDDtl modelDtl)
        {
            var Hdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, modelDtl.DocNo);
            var CustomerCode = Hdr.CustomerCode;
            var DocNo = Hdr.DocNo;
            var DocDate = Hdr.DocDate;

            if (!IsValidStatus(DocNo)) { return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" }); }

            if (modelDtl.QtyOrder.Value.Equals(0)) { return Json(new { success = false, message = "Jumlah part yang dipesan harus lebih besar dari 0" }); }

            //decimal qtyBO = GetBOQuantityByCustomer(CustomerCode, modelDtl.PartNo);
            //if (qtyBO > 0) { return Json(new { success = false, status = "BO", message = "Customer ini mempunyai BO Part ini sejumlah " + qtyBO.ToString() + "\n Apakah mau dilanjutkan ??" }); }

            return Json(new { success = true });
        }

        public JsonResult SavePart(SpTrnSORDDtl modelDtl)
        {
            using (var transPart = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                md = DealerCode() == "MD";
                var Hdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, modelDtl.DocNo);
                var CustomerCode = Hdr.CustomerCode;
                var DocNo = Hdr.DocNo;
                var DocDate = Hdr.DocDate;
               
                var oCustProfitCenter = ctx.ProfitCenters.Find(CompanyCode, BranchCode, CustomerCode, ProfitCenter);

                var oSORHdrInq = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, DocNo);
                if (Convert.ToInt32(oSORHdrInq.Status) > 1) { return Json(new { success = false, message = "Status Transaksi sudah di alokasi, data tidak dapat diupdate" }); }

                var recordDtl = ctx.SpTrnSORDDtls.Find(CompanyCode, BranchCode, DocNo, modelDtl.PartNo, WarehouseMD, modelDtl.PartNo);
                if (recordDtl == null)
                {
                    var oItems = new spMstItem();
                    var oItemLoc = new SpMstItemLoc();
                    var oItemPrice = new spMstItemPrice();
                    //if (md)
                    //{
                    //oItems = ctxMD.spMstItems.Find(CompanyMD, BranchMD, modelDtl.PartNo);
                    var sqlItem = string.Format("SELECT * FROM " + GetDbMD() + @"..spMstItems WHERE CompanyCode='{0}' AND BranchCode ='{1}' AND PartNo ='{2}'",
                        CompanyMD, BranchMD, modelDtl.PartNo);
                    oItems = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();

                    //oItemLoc = ctxMD.SpMstItemLocs.Find(CompanyMD, BranchMD, modelDtl.PartNo, WarehouseMD);
                    var sqlItemLoc = string.Format("SELECT * FROM " + GetDbMD() + @"..SpMstItemLoc WHERE CompanyCode='{0}' AND BranchCode ='{1}' AND PartNo ='{2}' AND WarehouseCode ='{3}'",
                        CompanyMD, BranchMD, modelDtl.PartNo, WarehouseMD);
                    oItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();
                    
                    //oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, modelDtl.PartNo); //ctxMD.spMstItemPrices.Find(CompanyMD, BranchMD, modelDtl.PartNo);
                    var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                        GetDbMD(), CompanyMD, BranchMD, modelDtl.PartNo);
                    oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();            
                    
                    if (oItems != null)
                    {
                        //Insert Into spMstItems, spMstItemLoc, spMstItemPrice 
                        InsertItemsLocPriceFromMD(oItems.PartNo);
                    }
                    //}
                    //else
                    //{
                    //    oItems = ctx.spMstItems.Find(CompanyCode, BranchCode, modelDtl.PartNo);
                    //    oItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, modelDtl.PartNo, WarehouseMD);
                    //    oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, modelDtl.PartNo);
                    //}

                    recordDtl = new SpTrnSORDDtl();
                    recordDtl.CompanyCode = CompanyCode;
                    recordDtl.BranchCode = BranchCode;
                    recordDtl.DocNo = DocNo;
                    recordDtl.PartNo = modelDtl.PartNo;
                    recordDtl.WarehouseCode = WarehouseMD;
                    recordDtl.PartNoOriginal = modelDtl.PartNo;
                    recordDtl.ReferenceNo = DocNo;
                    recordDtl.ReferenceDate = DocDate;
                    recordDtl.LocationCode = (oItemLoc == null) ? "" : oItemLoc.LocationCode ?? "";
                    recordDtl.QtyOrder = modelDtl.QtyOrder;
                    recordDtl.QtySupply = 0;
                    recordDtl.QtyBO = 0;
                    recordDtl.QtyBOSupply = 0;
                    recordDtl.QtyBOCancel = 0;
                    recordDtl.QtyBill = 0;
                    recordDtl.RetailPriceInclTax = GetRetailPriceInclTax(modelDtl.PartNo, oItemPrice.RetailPriceInclTax.Value); // oItemPrice == null ? 0 : oItemPrice.RetailPriceInclTax;
                    recordDtl.RetailPrice = GetRetailPrice(modelDtl.PartNo, oItemPrice.RetailPrice.Value); //oItemPrice.RetailPrice; 
                    recordDtl.CostPrice = GetCostPrice(modelDtl.PartNo); //oItemPrice == null ? 0 : oItemPrice.CostPrice;
                    recordDtl.DiscPct = modelDtl.DiscPct == null ? 0 : modelDtl.DiscPct;
                    recordDtl.SalesAmt = modelDtl.QtyOrder * GetRetailPrice(modelDtl.PartNo, oItemPrice.RetailPrice.Value); //modelDtl.RetailPrice;
                    recordDtl.DiscAmt = Math.Round(recordDtl.SalesAmt.Value * (recordDtl.DiscPct.Value / 100), 0, MidpointRounding.AwayFromZero);
                    recordDtl.NetSalesAmt = recordDtl.SalesAmt - recordDtl.DiscAmt;
                    recordDtl.PPNAmt = 0;
                    recordDtl.TotSalesAmt = recordDtl.NetSalesAmt + recordDtl.PPNAmt;
                    recordDtl.MovingCode = oItems.MovingCode;
                    recordDtl.ABCClass = oItems.ABCClass;
                    recordDtl.ProductType = oItems.ProductType;
                    recordDtl.PartCategory = oItems.PartCategory;
                    recordDtl.Status = "0";
                    recordDtl.CreatedBy = CurrentUser.UserId;
                    recordDtl.CreatedDate = DateTime.Now;
                    recordDtl.LastUpdateBy = CurrentUser.UserId;
                    recordDtl.LastUpdateDate = DateTime.Now;
                    recordDtl.FirstDemandQty = modelDtl.QtyOrder;
                    ctx.SpTrnSORDDtls.Add(recordDtl);
                }
                else
                {
                    var oItemPrice = new spMstItemPrice();

                    //oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, modelDtl.PartNo); //ctxMD.spMstItemPrices.Find(CompanyMD, BranchMD, modelDtl.PartNo);
                    var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                        GetDbMD(), CompanyMD, BranchMD, modelDtl.PartNo);
                    oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault(); 

                    recordDtl.QtyOrder = modelDtl.QtyOrder;
                    recordDtl.RetailPriceInclTax = GetRetailPriceInclTax(modelDtl.PartNo, oItemPrice.RetailPriceInclTax.Value); // oItemPrice == null ? 0 : oItemPrice.RetailPriceInclTax;
                    recordDtl.RetailPrice = GetRetailPrice(modelDtl.PartNo, oItemPrice.RetailPrice.Value); //oItemPrice.RetailPrice; 
                    recordDtl.DiscPct = modelDtl.DiscPct;
                    recordDtl.SalesAmt = recordDtl.QtyOrder * GetRetailPrice(modelDtl.PartNo, oItemPrice.RetailPrice.Value);  //modelDtl.RetailPrice;
                    recordDtl.DiscAmt = Math.Round(recordDtl.SalesAmt.Value * (modelDtl.DiscPct.Value / 100), 0, MidpointRounding.AwayFromZero);
                    recordDtl.NetSalesAmt = recordDtl.SalesAmt - recordDtl.DiscAmt;
                    recordDtl.PPNAmt = 0;
                    recordDtl.TotSalesAmt = recordDtl.NetSalesAmt + recordDtl.PPNAmt;
                    recordDtl.LastUpdateBy = CurrentUser.UserId;
                    recordDtl.LastUpdateDate = DateTime.Now;
                }

                ctx.SaveChanges();

                isOverdueOrderExist(CustomerCode, oCustProfitCenter);

                if (string.IsNullOrEmpty(recordDtl.LocationCode))
                { return Json(new { success = false, message = "Proses simpan detail gagal. \n Data tidak dapat disimpan karena item lokasi belum disetting" }); }

                if (modelDtl.RetailPrice == 0 || recordDtl.CostPrice == 0)
                { return Json(new { success = false, message = "Proses simpan detail gagal. \n Data tidak dapat disimpan karena item price belum disetting" }); }

                decimal retailPrice = Math.Round(recordDtl.RetailPrice.Value * (1 - (recordDtl.DiscPct.Value / 100)), 0, MidpointRounding.AwayFromZero);
                decimal ppnAmt = Math.Truncate(retailPrice * (GetPPNPct(recordDtl.DocNo) / 100));

                // Validasi dipindahkan ke method ValidateHppValue
                // By Rudiana
                    
                //if (retailPrice + ppnAmt < recordDtl.CostPrice)
                //{
                //    //if (!IsConfirmHpp)
                //    //{
                //        return Json(new { success = false, cekHpp = true, message = "Pastikan bahwa nilai penjualan > HPP ! Lanjutkan proses simpan ?" });
                //    //}
                //}

                try
                {
                    CalculatePart(DocNo, CustomerCode);
                    transPart.Commit();
                    return Json(new { success = true, data = recordDtl, docDate = DocDate });
                }
                catch (Exception ex)
                {
                    transPart.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        public JsonResult ValidateHppValue(SpTrnSORDDtl modelDtl)
        {
            try
            {
                var Hdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, modelDtl.DocNo);
                var DocNo = Hdr != null ? Hdr.DocNo : "";

                //var oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, modelDtl.PartNo); //ctxMD.spMstItemPrices.Find(CompanyMD, BranchMD, modelDtl.PartNo);
                var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                        GetDbMD(), CompanyMD, BranchMD, modelDtl.PartNo);
                var oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();            

                var costPrice = oItemPrice == null ? 0 : oItemPrice.CostPrice;
                var itmRetailPrice = GetRetailPrice(modelDtl.PartNo, oItemPrice.RetailPrice.Value); //oItemPrice == null ? 0 : oItemPrice.RetailPrice; 

                decimal retailPrice = Math.Round(itmRetailPrice * (1 - (modelDtl.DiscPct.Value / 100)), 0, MidpointRounding.AwayFromZero);
                decimal ppnAmt = Math.Truncate(retailPrice * (GetPPNPct(DocNo) / 100));

                //string msg = "";
                if (retailPrice + ppnAmt < costPrice)
                {
                    msg = "Pastikan bahwa nilai penjualan > HPP ! Lanjutkan proses simpan ?"; 
                }

                return Json(new { success = true, message = msg});
                
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Terjadi kesalahan saat validasi nilai penjualan terhadap HPP. "  + ex.Message}); 
            }
        }

        public JsonResult DeletePart(SpTrnSORDDtl modelDtl)
        {
            
            var Hdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, modelDtl.DocNo);
            var CustomerCode = Hdr.CustomerCode;
            var DocNo = Hdr.DocNo;
            var DocDate = Hdr.DocDate;

            if (!IsValidStatus(DocNo)) { return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" }); }

            var oSORHdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, DocNo);
            if (Convert.ToInt32(oSORHdr.Status) > 1)
            {
                return Json(new { success = false, message = "Status Transaksi sudah di alokasi, data tidak dapat diupdate" });
            }

            var record = ctx.SpTrnSORDDtls.Find(CompanyCode, BranchCode, DocNo, modelDtl.PartNo, WarehouseMD, modelDtl.PartNo);

            using (var tranScope = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ctx.SpTrnSORDDtls.Remove(record);

                    ctx.SaveChanges();
                    CalculatePart(DocNo, CustomerCode);
                    
                    tranScope.Commit();
                    
                    var table = GetPartTable(DocNo);
                    return Json(new { success = true, Data = Hdr, Table = table });
                }
                catch (Exception ex)
                {
                    tranScope.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        #region Allocation Stock

        public JsonResult AllocationStock(SpTrnSORDHdr model)
        {
            try
            {
                md = DealerCode() == "MD";
                var hdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, model.DocNo);

                if (!IsValidStatus(model.DocNo))
                { return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" }); }

                if (model.SalesType == "1")
                {
                    if (hdr != null)
                    {
                        if (hdr.TransType == "10")
                        {
                            var periode = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);

                            if (periode != null)
                            {
                                var perBranchTo = ctx.Periodes.FirstOrDefault(
                                    a => a.CompanyCode == CompanyCode
                                    && a.BranchCode == BranchCode
                                    && a.FiscalYear == periode.FiscalYear
                                    && a.FiscalMonth == periode.FiscalMonth
                                    && a.PeriodeNum == periode.FiscalPeriod
                                    && a.FromDate > hdr.DocDate
                                    && a.EndDate < hdr.DocDate);

                                if (perBranchTo != null)
                                {
                                    if (perBranchTo.StatusSparepart == 2)
                                    {
                                        return Json(new { success = false, message = "Periode Cabang Tujuan Sudah Tutup !!!" });
                                    }
                                }
                            }
                        }
                    }
                }

                using (var transScope = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var result = ProcessSO(model, md);
                        if (result == true)
                        {
                            transScope.Commit();
                        }
                        else
                        {
                            transScope.Rollback();
                            return Json(new { success = false, message = msg });
                        }

                        return Json(new { success = result, message = msg });
                    }
                    catch (Exception ex)
                    {
                        transScope.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, error_log = ex.Message });
            }
        }

        private bool ProcessSO(SpTrnSORDHdr model, bool md)
        {
            bool process = false;

            try
            {
                PreProcessValidation(model.DocNo, true);

                //var dtv = DateTransValidation(model.DocDate.Value);
                //if (dtv != "")
                //{
                //    msg = dtv;
                //    return process;
                //}

                var countSODtl = ctx.SpTrnSORDDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == model.DocNo).Count();
                if (countSODtl < 0)
                {
                    msg = "Dokumen belum memiliki detail pesanan";
                    return process;

                }

                return StockAllocation(model);
            }
            catch(Exception e)
            {
                msg = e.Message;
                return process;
            }
        }

        private bool StockAllocation(SpTrnSORDHdr model)
        {
            bool result = true;
            bool isresult = false;
            decimal counterRows = 0;
            int counterIndex = 0;
            //string msg = "";
            bool md = DealerCode() == "MD";

            var coProfileSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
            decimal fiscalYear = coProfileSpare.PeriodEnd.Year;
            decimal fiscalMonth = coProfileSpare.PeriodEnd.Month;
            try
            {
                var coProfile = ctx.CoProfiles.Find(CompanyCode, BranchCode);
                var recordHdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, model.DocNo);

                if (recordHdr == null)
                {
                    result = false;
                    msg = "Data sedang di Locking, Tunggu beberapa saat lagi.";
                    return result;
                }
                else if (recordHdr.Status == "2")
                {
                    result = false;
                    msg = "Proses alokasi stok gagal \nNo. Sales Order = " + model.DocNo + " telah di close";
                    return result;
                }

                if (recordHdr.Status == "1")
                {
                    recordHdr.Status = "2";
                    recordHdr.LastUpdateBy = CurrentUser.UserId;
                    recordHdr.LastUpdateDate = DateTime.Now;
                    result = ctx.SaveChanges() > 0;

                    if (result)
                    {
                        string[] arrStat = { "0", "1" };
                        var dtl = ctx.SpTrnSORDDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == model.DocNo && arrStat.Contains(a.Status)).ToList();

                        if (dtl.Count() == 0)
                        {
                            msg = "Data Belum diCetak, Silahkan di Cetak terlebih dahulu";
                            return false;
                        }

                        decimal qtyBO = 0;
                        bool resultSubs = false;

                        // loop spTrnSORDDtl data 
                        decimal amtSales = 0;
                        decimal PpnAmt = 0;

                        var PpnPct = ctx.ProfitCenters.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
                                    && a.CustomerCode == model.CustomerCode && a.ProfitCenterCode == ProfitCenter)
                                    .Join(ctx.Taxes,
                                    x => new { x.CompanyCode, x.TaxCode },
                                    y => new { y.CompanyCode, y.TaxCode },
                                    (x, y) => y.TaxPct.Value).Distinct().FirstOrDefault();

                        counterRows = dtl.Count();

                        foreach (var oSpTrnSordDtl in dtl)
                        {
                            counterIndex++;

                            var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode='{2}' AND PartNo='{3}'",
                                GetDbMD(), CompanyMD, BranchMD, oSpTrnSordDtl.PartNo);
                            var oItemPrices = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();
                            var costPriceMD = oItemPrices != null ? oItemPrices.CostPrice : 0;

                            oSpTrnSordDtl.CostPrice = GetCostPrice(oSpTrnSordDtl.PartNo); //oItemPrices != null ? oItemPrices.CostPrice : oSpTrnSordDtl.CostPrice;

                            //var recCostPrice = md ? ctx.spMstItemPrices.Find(CompanyCode, BranchCode, oSpTrnSordDtl.PartNo).CostPrice : costPriceMD;
                            //var recCostPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, oSpTrnSordDtl.PartNo).CostPrice;
                            //oSpTrnSordDtl.CostPrice = recCostPrice != null ? recCostPrice : oSpTrnSordDtl.CostPrice;

                            qtyBO = 0;
                            qtyBO = SaveStockAllocation(oSpTrnSordDtl, oSpTrnSordDtl.QtyOrder.Value, recordHdr,
                                                         0, ref resultSubs, ref amtSales, true, md);

                            if (!resultSubs)
                            {
                                msg = "Proses Update Sales Order Supply Gagal";
                                return false;
                            }

                            if (recordHdr.TransType == "10" && recordHdr.isLocked.Value)
                            {
                                if (qtyBO > 0)
                                {
                                    msg = "Part Harus Ter-Supply Semua !";
                                    return false;
                                }
                            }

                            // update check status dari isBO whether is true or false, if true will proceed                            
                            oSpTrnSordDtl.QtySupply = oSpTrnSordDtl.QtyOrder - (qtyBO - oSpTrnSordDtl.QtyBOSupply - oSpTrnSordDtl.QtyBOCancel);

                            if (qtyBO > 0 && recordHdr.isBO.Value)
                            {
                                // update bo di stock
                                if (result)
                                    UpdateStock(oSpTrnSordDtl.PartNoOriginal, "00", 0, 0, qtyBO, 0, model.SalesType, md, ref isresult);
                                result = isresult;
                                if (!result)
                                {
                                    //infoForm.AddMessage(LoggerType.Info, "Proses Update Saldo Back Order Gagal");
                                    msg = "Proses Update Saldo Back Order Gagal";
                                    return false;
                                }

                                // prepare bo quantity for sord detail data
                                oSpTrnSordDtl.QtyBO = qtyBO;
                            }
                            else if (qtyBO > 0 && !(recordHdr.isBO.Value))
                            {
                                // prepare bo quantity for sord detail data
                                oSpTrnSordDtl.QtyBO = qtyBO;
                                oSpTrnSordDtl.QtyBOCancel = qtyBO;
                            }
                            // update status SpTrnSordDtl menjadi 2
                            oSpTrnSordDtl.Status = "2";
                            oSpTrnSordDtl.StockAllocatedBy = CurrentUser.UserId;
                            oSpTrnSordDtl.StockAllocatedDate = DateTime.Now;

                            result = ctx.SaveChanges() > 0;
                            
                            if (!md && result)
                            {
                                SDMovementAdd(oSpTrnSordDtl, model.DocNo, model.DocDate.Value);
                            }

                            if (result) { result = UpdateHistory(oSpTrnSordDtl, recordHdr, fiscalYear, fiscalMonth); }
                            if (!result)
                            {
                                msg = "Proses Update History Gagal";
                                return false;
                            }

                            // Update LastDemandDate (LDD) di table spMstItems
                            if (result)
                                UpdateLastItemDate(oSpTrnSordDtl.PartNo, "LDD", md);
                        } // end of forech dt.Rows

                        PpnAmt = PpnPct > 0 ? Math.Truncate((amtSales * (PpnPct / 100))) : 0;
                        amtSales = amtSales + PpnAmt;

                        // update header dan tambah bankbook dan Credit Limit
                        string errMsg = string.Empty;
                        if (result && amtSales > 0)
                        {
                            result = UpdateHeaderAndBankBook(recordHdr.CustomerCode, recordHdr.SalesType, amtSales);
                                //, ref errMsg);

                            if (!result)
                            {
                                //msg = errMsg;
                                return false;
                            }
                        }
                    }
                    else
                        msg = "Data sedang di Locking, Tunggu beberapa saat lagi";

                    // If SalesType = 2 then insert/update in SvTrnSrvItem table
                    if (result && recordHdr.SalesType == "2" && CurrentUser.CoProfile.IsLinkToService.Value)
                    {
                        result = UpdateSupplySlipNo(recordHdr.UsageDocNo, recordHdr.DocNo);
                        if (!result)
                        {
                            msg = "Proses update No. Supply Slip gagal";
                            return false;
                        }
                    }

                    // If SalesType = 3 then update OmTrSalesSOAccs.SupplySlipNo
                    if (result && recordHdr.SalesType == "3" && CurrentUser.CoProfile.IsLinkToSales.Value)
                    {
                        // Cek Qty Supply
                        var dtSupp = ctx.SpTrnSORDDtls.Where(a => a.CompanyCode == recordHdr.CompanyCode && a.BranchCode == recordHdr.BranchCode && a.DocNo == recordHdr.DocNo && a.QtySupply != a.QtyOrder);

                        if (dtSupp.Count() > 0)
                        {
                            string part = "";
                            int last = dtSupp.Count();
                            int seq = 1;
                            foreach (var dtl in dtSupp)
                            {
                                if (seq == last)
                                    part += dtl.PartNo;
                                else
                                    part += dtl.PartNo + ", ";
                                seq++;
                            }

                            msg = "Part " + part + " harus tersupply semua !";
                            result = false;
                            return false;
                        }

                        result = ctx.Database.ExecuteSqlCommand("exec uspfn_UpdateSalesSOAccsSeq '" + CompanyCode + "','" + BranchCode + "','" + recordHdr.DocNo + "','" + CurrentUser.UserId + "'") > 0;//CompanyCode, BranchCode, recordHdr.DocNo, CurrentUser.UserId) > 0;
                        if (!result)
                        {
                            msg = "Proses update No. Supply Slip gagal";
                            return false;
                        }
                    }
                }
                else
                    result = false;

            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception("Gagal proses Stock Allocation. Error: " + ex.Message);
            }

            return result;
        }

        private decimal SaveStockAllocation(SpTrnSORDDtl oSpTrnSordDtl, decimal qtyOrder, SpTrnSORDHdr oSpTrnSORDHdr,
            int supSeq, ref bool resultSubs, ref decimal amtSales, bool isAlloc, bool md)
        {
            // loop part mode 
            decimal decQtyOrder = qtyOrder;
            decimal availItem = 0;
            decimal qtyNewSupply = 0;

            SpMstItemLoc oSpMstItemLoc = null;
            spMstItem oSpMstItems = null;
            SpTrnSOSupply oSpTrnSOSupply = null;

            spMstItemMod oSpMstItemMod = null;
            List<spMstItemMod> lstSpMstItemMod = new List<spMstItemMod>();

            //var oSpMstItemLocDao = new 
            //var oSpMstItemsDao = new SpMstItemsDao(ctx);
            bool isGetSusbtitution = false;
            if (oSpTrnSORDHdr.SalesType == "0" || oSpTrnSORDHdr.SalesType == "1")
            {
                isGetSusbtitution = oSpTrnSORDHdr.isSubstitution.Value;
            }
            // check available for curren part no order

            var sqlItemLoc = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                GetDbMD(), CompanyMD, BranchMD, oSpTrnSordDtl.PartNo, "00");
            oSpMstItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

            availItem = oSpMstItemLoc.OnHand.Value -
                        (oSpMstItemLoc.AllocationSP.Value +
                         oSpMstItemLoc.AllocationSR.Value +
                         oSpMstItemLoc.AllocationSL.Value) -
                        (oSpMstItemLoc.ReservedSP.Value +
                         oSpMstItemLoc.ReservedSR.Value +
                         oSpMstItemLoc.ReservedSL.Value);

            if (availItem > 0 && qtyOrder <= availItem)
                isGetSusbtitution = false;

            // prepare listItemMod
            if (isGetSusbtitution)
            {
                lstSpMstItemMod = fc_SelectModifikasi(oSpTrnSordDtl.PartNo);
            }

            if (!isGetSusbtitution || lstSpMstItemMod.Count <= 0)
            {
                oSpMstItemMod = new spMstItemMod();
                oSpMstItemMod.PartNo = oSpTrnSordDtl.PartNo;
                lstSpMstItemMod.Add(oSpMstItemMod);
            }

            resultSubs = false;
            decimal SalesAmt = 0
                    , DiscAmt = 0
                    , DppAmt = 0;

            foreach (spMstItemMod oSpMstItemModTemp in lstSpMstItemMod)
            {
                sqlItemLoc = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                                GetDbMD(), CompanyMD, BranchMD, oSpMstItemModTemp.PartNo, "00");
                oSpMstItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

                var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                    GetDbMD(), CompanyMD, BranchMD, oSpMstItemModTemp.PartNo);
                oSpMstItems = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();

                if (oSpMstItemLoc == null || oSpMstItems == null)
                    availItem = 0;
                else if (!oSpMstItems.TypeOfGoods.Equals(oSpTrnSORDHdr.TypeOfGoods) ||
                         !oSpMstItems.ProductType.Equals(oSpTrnSordDtl.ProductType))
                    availItem = 0;
                else
                    availItem = oSpMstItemLoc.OnHand.Value -
                               (oSpMstItemLoc.AllocationSP.Value +
                                oSpMstItemLoc.AllocationSR.Value +
                                oSpMstItemLoc.AllocationSL.Value) -
                               (oSpMstItemLoc.ReservedSP.Value +
                                oSpMstItemLoc.ReservedSR.Value +
                                oSpMstItemLoc.ReservedSL.Value);

                if (availItem > 0)
                {
                    qtyNewSupply = availItem > decQtyOrder ? decQtyOrder : availItem;
                    // prepare data supply 
                    oSpTrnSOSupply = prepareSupplyData(oSpMstItems, oSpTrnSORDHdr,
                                             oSpMstItemModTemp.PartNo, oSpTrnSordDtl.PartNoOriginal, oSpMstItemLoc.LocationCode,
                                             qtyNewSupply, supSeq, oSpTrnSordDtl.RetailPrice.Value,
                                             oSpTrnSordDtl.RetailPriceInclTax.Value, oSpTrnSordDtl.CostPrice.Value,
                                             oSpTrnSordDtl.DiscPct.Value, md);

                    //insert spTrnSOSupply, update item dan itemloc
                    resultSubs = UpdateStockAndSupply(oSpTrnSOSupply, oSpTrnSORDHdr, qtyNewSupply, isAlloc, md);

                    if (!resultSubs)
                        break;

                    // hitung sales amount untuk part no yang dialokasi
                    SalesAmt = oSpTrnSOSupply.RetailPrice.Value * qtyNewSupply;
                    DiscAmt = Math.Round(SalesAmt * oSpTrnSOSupply.DiscPct.Value / 100, 0, MidpointRounding.AwayFromZero);
                    DppAmt = SalesAmt - DiscAmt;
                    amtSales += DppAmt;
                }
                else
                {
                    qtyNewSupply = 0;
                    resultSubs = true;
                }

                decQtyOrder = decQtyOrder - qtyNewSupply;

                if (decQtyOrder <= 0)
                    return decQtyOrder;

                //end-of-forech lstSpMstItemLoc
            }
            return decQtyOrder;
        } 
        
        public JsonResult AllocationStockBranch(SpTrnSORDHdr model)
        {
            try
            {
                md = DealerCode() == "MD";
                var hdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, model.DocNo);

                if (!IsValidStatus(model.DocNo))
                { return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" }); }

                if (model.SalesType == "1")
                {
                    if (hdr != null)
                    {
                        if (hdr.TransType == "10")
                        {
                            var periode = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);

                            if (periode != null)
                            {
                                var perBranchTo = ctx.Periodes.FirstOrDefault(
                                    a => a.CompanyCode == CompanyCode
                                    && a.BranchCode == BranchCode
                                    && a.FiscalYear == periode.FiscalYear
                                    && a.FiscalMonth == periode.FiscalMonth
                                    && a.PeriodeNum == periode.FiscalPeriod
                                    && a.FromDate > hdr.DocDate
                                    && a.EndDate < hdr.DocDate);

                                if (perBranchTo != null)
                                {
                                    if (perBranchTo.StatusSparepart == 2)
                                    {
                                        return Json(new { success = false, message = "Periode Cabang Tujuan Sudah Tutup !!!" });
                                    }
                                }
                            }
                        }
                    }
                }

                using (var transScope = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var result = ProcessSOBranch(model, md);
                        if (result == true)
                        {
                            transScope.Commit();
                        }
                        else
                        {
                            transScope.Rollback();
                            return Json(new { success = false, message = msg });
                        }

                        return Json(new { success = result, message = msg });
                    }
                    catch (Exception ex)
                    {
                        transScope.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool ProcessSOBranch(SpTrnSORDHdr model, bool md)
        {
            bool process = false;

            try
            {
                PreProcessValidation(model.DocNo, true);

                //var dtv = DateTransValidation(model.DocDate.Value);
                //if (dtv != "")
                //{
                //    msg = dtv;
                //    return process;
                //}

                var countSODtl = ctx.SpTrnSORDDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == model.DocNo).Count();
                if (countSODtl < 0)
                {
                    msg = "Dokumen belum memiliki detail pesanan";
                    return process;

                }

                return StockAllocationBranch(model);
            }
            catch (Exception e)
            {
                msg = e.Message;
                return process;
            }
        }

        private bool StockAllocationBranch(SpTrnSORDHdr model)
        {
            bool result = true;
            bool isresult = false;
            decimal counterRows = 0;
            int counterIndex = 0;
            //string msg = "";
            bool md = DealerCode() == "MD";

            var coProfileSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
            decimal fiscalYear = coProfileSpare.PeriodEnd.Year;
            decimal fiscalMonth = coProfileSpare.PeriodEnd.Month;
            try
            {
                var coProfile = ctx.CoProfiles.Find(CompanyCode, BranchCode);
                var recordHdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, model.DocNo);

                if (recordHdr == null)
                {
                    result = false;
                    msg = "Data sedang di Locking, Tunggu beberapa saat lagi.";
                    return result;
                }
                else if (recordHdr.Status == "2")
                {
                    result = false;
                    msg = "Proses alokasi stok gagal \nNo. Sales Order = " + model.DocNo + " telah di close";
                    return result;
                }

                if (recordHdr.Status == "1")
                {
                    recordHdr.Status = "2";
                    recordHdr.LastUpdateBy = CurrentUser.UserId;
                    recordHdr.LastUpdateDate = DateTime.Now;
                    result = ctx.SaveChanges() > 0;

                    if (result)
                    {
                        string[] arrStat = { "0", "1" };
                        var dtl = ctx.SpTrnSORDDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == model.DocNo && arrStat.Contains(a.Status)).ToList();

                        if (dtl.Count() == 0)
                        {
                            msg = "Data Belum diCetak, Silahkan di Cetak terlebih dahulu";
                            return false;
                        }

                        decimal qtyBO = 0;
                        bool resultSubs = false;

                        // loop spTrnSORDDtl data 
                        decimal amtSales = 0;
                        decimal PpnAmt = 0;

                        var PpnPct = ctx.ProfitCenters.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
                                    && a.CustomerCode == model.CustomerCode && a.ProfitCenterCode == ProfitCenter)
                                    .Join(ctx.Taxes,
                                    x => new { x.CompanyCode, x.TaxCode },
                                    y => new { y.CompanyCode, y.TaxCode },
                                    (x, y) => y.TaxPct.Value).Distinct().FirstOrDefault();

                        counterRows = dtl.Count();

                        foreach (var oSpTrnSordDtl in dtl)
                        {
                            counterIndex++;
                            var sqlItemPrice = string.Format("SELECT * FROM spMstItemPrice WHERE CompanyCode='{0}' AND BranchCode='{1}' AND PartNo='{2}'",
                                CompanyCode, BranchCode, oSpTrnSordDtl.PartNo);
                            var oItemPrices = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();
                            var costPriceMD = oItemPrices != null ? oItemPrices.CostPrice : 0;
                            if (md)
                            {
                                 sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode='{2}' AND PartNo='{3}'",
                                GetDbMD(), CompanyMD, BranchMD, oSpTrnSordDtl.PartNo);
                                 oItemPrices = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();
                                 costPriceMD = oItemPrices != null ? oItemPrices.CostPrice : 0;
                            }
                            
                            oSpTrnSordDtl.CostPrice = GetCostPrice(oSpTrnSordDtl.PartNo); //oItemPrices != null ? oItemPrices.CostPrice : oSpTrnSordDtl.CostPrice;

                            //var recCostPrice = md ? ctx.spMstItemPrices.Find(CompanyCode, BranchCode, oSpTrnSordDtl.PartNo).CostPrice : costPriceMD;
                            //var recCostPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, oSpTrnSordDtl.PartNo).CostPrice;
                            //oSpTrnSordDtl.CostPrice = recCostPrice != null ? recCostPrice : oSpTrnSordDtl.CostPrice;

                            qtyBO = 0;
                            qtyBO = SaveStockAllocationBranch(oSpTrnSordDtl, oSpTrnSordDtl.QtyOrder.Value, recordHdr,
                                                         0, ref resultSubs, ref amtSales, true, md);

                            if (!resultSubs)
                            {
                                msg = "Proses Update Sales Order Supply Gagal";
                                return false;
                            }

                            if (recordHdr.TransType == "10" && recordHdr.isLocked.Value)
                            {
                                if (qtyBO > 0)
                                {
                                    msg = "Part Harus Ter-Supply Semua !";
                                    return false;
                                }
                            }

                            // update check status dari isBO whether is true or false, if true will proceed                            
                            oSpTrnSordDtl.QtySupply = oSpTrnSordDtl.QtyOrder - (qtyBO - oSpTrnSordDtl.QtyBOSupply - oSpTrnSordDtl.QtyBOCancel);

                            if (qtyBO > 0 && recordHdr.isBO.Value)
                            {
                                // update bo di stock
                                if (result)
                                    UpdateStock(oSpTrnSordDtl.PartNoOriginal, "00", 0, 0, qtyBO, 0, model.SalesType, md, ref isresult);
                                result = isresult;
                                if (!result)
                                {
                                    //infoForm.AddMessage(LoggerType.Info, "Proses Update Saldo Back Order Gagal");
                                    msg = "Proses Update Saldo Back Order Gagal";
                                    return false;
                                }

                                // prepare bo quantity for sord detail data
                                oSpTrnSordDtl.QtyBO = qtyBO;
                            }
                            else if (qtyBO > 0 && !(recordHdr.isBO.Value))
                            {
                                // prepare bo quantity for sord detail data
                                oSpTrnSordDtl.QtyBO = qtyBO;
                                oSpTrnSordDtl.QtyBOCancel = qtyBO;
                            }
                            // update status SpTrnSordDtl menjadi 2
                            oSpTrnSordDtl.Status = "2";
                            oSpTrnSordDtl.StockAllocatedBy = CurrentUser.UserId;
                            oSpTrnSordDtl.StockAllocatedDate = DateTime.Now;

                            result = ctx.SaveChanges() > 0;

                            //if (!md && result)
                            //{
                            //    SDMovementAdd(oSpTrnSordDtl, model.DocNo, model.DocDate.Value);
                            //}

                            if (result) { result = UpdateHistory(oSpTrnSordDtl, recordHdr, fiscalYear, fiscalMonth); }
                            if (!result)
                            {
                                msg = "Proses Update History Gagal";
                                return false;
                            }

                            // Update LastDemandDate (LDD) di table spMstItems
                            if (result)
                                UpdateLastItemDate(oSpTrnSordDtl.PartNo, "LDD", md);
                        } // end of forech dt.Rows

                        PpnAmt = PpnPct > 0 ? Math.Truncate((amtSales * (PpnPct / 100))) : 0;
                        amtSales = amtSales + PpnAmt;

                        // update header dan tambah bankbook dan Credit Limit
                        string errMsg = string.Empty;
                        if (result && amtSales > 0)
                        {
                            result = UpdateHeaderAndBankBook(recordHdr.CustomerCode, recordHdr.SalesType, amtSales);
                            //, ref errMsg);

                            if (!result)
                            {
                                //msg = errMsg;
                                return false;
                            }
                        }
                    }
                    else
                        msg = "Data sedang di Locking, Tunggu beberapa saat lagi";

                    // If SalesType = 2 then insert/update in SvTrnSrvItem table
                    if (result && recordHdr.SalesType == "2" && CurrentUser.CoProfile.IsLinkToService.Value)
                    {
                        result = UpdateSupplySlipNo(recordHdr.UsageDocNo, recordHdr.DocNo);
                        if (!result)
                        {
                            msg = "Proses update No. Supply Slip gagal";
                            return false;
                        }
                    }

                    // If SalesType = 3 then update OmTrSalesSOAccs.SupplySlipNo
                    if (result && recordHdr.SalesType == "3" && CurrentUser.CoProfile.IsLinkToSales.Value)
                    {
                        // Cek Qty Supply
                        var dtSupp = ctx.SpTrnSORDDtls.Where(a => a.CompanyCode == recordHdr.CompanyCode && a.BranchCode == recordHdr.BranchCode && a.DocNo == recordHdr.DocNo && a.QtySupply != a.QtyOrder);

                        if (dtSupp.Count() > 0)
                        {
                            string part = "";
                            int last = dtSupp.Count();
                            int seq = 1;
                            foreach (var dtl in dtSupp)
                            {
                                if (seq == last)
                                    part += dtl.PartNo;
                                else
                                    part += dtl.PartNo + ", ";
                                seq++;
                            }

                            msg = "Part " + part + " harus tersupply semua !";
                            result = false;
                            return false;
                        }

                        result = ctx.Database.ExecuteSqlCommand("exec uspfn_UpdateSalesSOAccsSeq '" + CompanyCode + "','" + BranchCode + "','" + recordHdr.DocNo + "','" + CurrentUser.UserId + "'") > 0;//CompanyCode, BranchCode, recordHdr.DocNo, CurrentUser.UserId) > 0;
                        if (!result)
                        {
                            msg = "Proses update No. Supply Slip gagal";
                            return false;
                        }
                    }
                }
                else
                    result = false;

            }
            catch (Exception e)
            {
                result = false;
                throw new Exception("Gagal proses Stock Allocation");
            }

            return result;
        }

        private decimal SaveStockAllocationBranch(SpTrnSORDDtl oSpTrnSordDtl, decimal qtyOrder, SpTrnSORDHdr oSpTrnSORDHdr,
           int supSeq, ref bool resultSubs, ref decimal amtSales, bool isAlloc, bool md)
        {
            // loop part mode 
            decimal decQtyOrder = qtyOrder;
            decimal availItem = 0;
            decimal qtyNewSupply = 0;

            SpMstItemLoc oSpMstItemLoc = null;
            spMstItem oSpMstItems = null;
            SpTrnSOSupply oSpTrnSOSupply = null;

            spMstItemMod oSpMstItemMod = null;
            List<spMstItemMod> lstSpMstItemMod = new List<spMstItemMod>();

            //var oSpMstItemLocDao = new 
            //var oSpMstItemsDao = new SpMstItemsDao(ctx);
            bool isGetSusbtitution = false;
            if (oSpTrnSORDHdr.SalesType == "0" || oSpTrnSORDHdr.SalesType == "1")
            {
                isGetSusbtitution = oSpTrnSORDHdr.isSubstitution.Value;
            }
            // check available for curren part no order
            var sqlItemLoc = string.Format("SELECT * FROM SpMstItemLoc WHERE CompanyCode='{0}' AND BranchCode ='{1}' AND PartNo ='{2}' AND WarehouseCode ='{3}'",
                CompanyCode, BranchCode, oSpTrnSordDtl.PartNo, "00");
            oSpMstItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

            if (md)
            {
                sqlItemLoc = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                GetDbMD(), CompanyMD, BranchMD, oSpTrnSordDtl.PartNo, "00");
                oSpMstItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();
            }
            
            availItem = oSpMstItemLoc.OnHand.Value -
                        (oSpMstItemLoc.AllocationSP.Value +
                         oSpMstItemLoc.AllocationSR.Value +
                         oSpMstItemLoc.AllocationSL.Value) -
                        (oSpMstItemLoc.ReservedSP.Value +
                         oSpMstItemLoc.ReservedSR.Value +
                         oSpMstItemLoc.ReservedSL.Value);

            if (availItem > 0 && qtyOrder <= availItem)
                isGetSusbtitution = false;

            // prepare listItemMod
            if (isGetSusbtitution)
            {
                lstSpMstItemMod = fc_SelectModifikasi(oSpTrnSordDtl.PartNo);
            }

            if (!isGetSusbtitution || lstSpMstItemMod.Count <= 0)
            {
                oSpMstItemMod = new spMstItemMod();
                oSpMstItemMod.PartNo = oSpTrnSordDtl.PartNo;
                lstSpMstItemMod.Add(oSpMstItemMod);
            }

            resultSubs = false;
            decimal SalesAmt = 0
                    , DiscAmt = 0
                    , DppAmt = 0;

            foreach (spMstItemMod oSpMstItemModTemp in lstSpMstItemMod)
            {
                sqlItemLoc = string.Format("SELECT * FROM SpMstItemLoc WHERE CompanyCode='{0}' AND BranchCode ='{1}' AND PartNo ='{2}' AND WarehouseCode ='{3}'",
                               CompanyCode, BranchCode, oSpMstItemModTemp.PartNo, "00");
                oSpMstItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

                var sqlItem = string.Format("SELECT * FROM spMstItems WHERE CompanyCode='{0}' AND BranchCode ='{1}' AND PartNo ='{2}'",
                   CompanyCode, BranchCode, oSpMstItemModTemp.PartNo);
                oSpMstItems = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();
                
                if (md) {
                    sqlItemLoc = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                                GetDbMD(), CompanyMD, BranchMD, oSpMstItemModTemp.PartNo, "00");
                    oSpMstItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

                    sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                        GetDbMD(), CompanyMD, BranchMD, oSpMstItemModTemp.PartNo);
                    oSpMstItems = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();
                }
                
                if (oSpMstItemLoc == null || oSpMstItems == null)
                    availItem = 0;
                else if (!oSpMstItems.TypeOfGoods.Equals(oSpTrnSORDHdr.TypeOfGoods) ||
                         !oSpMstItems.ProductType.Equals(oSpTrnSordDtl.ProductType))
                    availItem = 0;
                else
                    availItem = oSpMstItemLoc.OnHand.Value -
                               (oSpMstItemLoc.AllocationSP.Value +
                                oSpMstItemLoc.AllocationSR.Value +
                                oSpMstItemLoc.AllocationSL.Value) -
                               (oSpMstItemLoc.ReservedSP.Value +
                                oSpMstItemLoc.ReservedSR.Value +
                                oSpMstItemLoc.ReservedSL.Value);

                if (availItem > 0)
                {
                    qtyNewSupply = availItem > decQtyOrder ? decQtyOrder : availItem;
                    // prepare data supply 
                    oSpTrnSOSupply = prepareSupplyData(oSpMstItems, oSpTrnSORDHdr,
                                             oSpMstItemModTemp.PartNo, oSpTrnSordDtl.PartNoOriginal, oSpMstItemLoc.LocationCode,
                                             qtyNewSupply, supSeq, oSpTrnSordDtl.RetailPrice.Value,
                                             oSpTrnSordDtl.RetailPriceInclTax.Value, oSpTrnSordDtl.CostPrice.Value,
                                             oSpTrnSordDtl.DiscPct.Value, md);

                    //insert spTrnSOSupply, update item dan itemloc
                    resultSubs = UpdateStockAndSupply(oSpTrnSOSupply, oSpTrnSORDHdr, qtyNewSupply, isAlloc, md);

                    if (!resultSubs)
                        break;

                    // hitung sales amount untuk part no yang dialokasi
                    SalesAmt = oSpTrnSOSupply.RetailPrice.Value * qtyNewSupply;
                    DiscAmt = Math.Round(SalesAmt * oSpTrnSOSupply.DiscPct.Value / 100, 0, MidpointRounding.AwayFromZero);
                    DppAmt = SalesAmt - DiscAmt;
                    amtSales += DppAmt;
                }
                else
                {
                    qtyNewSupply = 0;
                    resultSubs = true;
                }

                decQtyOrder = decQtyOrder - qtyNewSupply;

                if (decQtyOrder <= 0)
                    return decQtyOrder;

                //end-of-forech lstSpMstItemLoc
            }
            return decQtyOrder;
        } 
        /// <summary>
        /// Proses Stock Alokasi
        /// </summary>
        /// <param name="oSpTrnSordDtl"></param>
        /// <param name="qtyOrder"></param>
        /// <param name="oSpTrnSORDHdr"></param>
        /// <param name="supSeq"></param>
        /// <param name="resultSubs"></param>
        /// <param name="amtSales"></param>
        /// <param name="isAlloc"></param>
        /// <param name="md"></param>
        /// <returns></returns>
        
       
        #endregion

        public JsonResult CancelSO(SpTrnSORDHdr model)
        {
            md = DealerCode() == "MD";
            if (!IsValidStatus(model.DocNo)) { return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!" }); }

            PreProcessValidation(model.DocNo, true);

            var dtv = DateTransValidation(model.DocDate.Value);
            if (dtv != "") { return Json(new { success = false, message = dtv }); }

            if (CancelSODetail(model)) {
                return Json(new { success = true , message = "Proses cancel SO berhasil"});
            }

            return Json(new { success = false, message = "Proses cancel SO gagal"});
        }

        public bool CancelSODetail(SpTrnSORDHdr recordHdr)
        {
            bool result = false;

            try
            {
                result = UpdateHdr(recordHdr, "3", false) && UpdateDtl( recordHdr, "3");
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool UpdateHdr(SpTrnSORDHdr record, String status, bool isPrint)
        {
            SpTrnSORDHdr recordTemp = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, record.DocNo);

            if (isPrint)
                recordTemp.PrintSeq += 1;

            recordTemp.Status = status;
            recordTemp.LastUpdateBy = CurrentUser.UserId;
            recordTemp.LastUpdateDate = DateTime.Now;

            return ctx.SaveChanges() > 0 ? true : false;
        }

        public bool UpdateDtl(SpTrnSORDHdr record, String status)
        {
            int intDataSaved = 0;

            // select all detail  data for current header
            var dtls = ctx.SpTrnSORDDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == record.DocNo).ToList();

            // data ada yang sudah close atau delete 
            var rowTemp = dtls.Where(x => x.Status != "0" && x.Status != "1");
            if (rowTemp.Count() > 0)
                return false;

            foreach (var dtl in dtls)
            {
                SpTrnSORDDtl recDetail = ctx.SpTrnSORDDtls.Find(CompanyCode, BranchCode, dtl.DocNo, dtl.PartNo, dtl.WarehouseCode, dtl.PartNoOriginal);

                recDetail.Status = status;
                recDetail.LastUpdateBy = CurrentUser.UserId;
                recDetail.LastUpdateDate = DateTime.Now;

                var x = ctx.SaveChanges() <= 0;

                if (x)
                {
                    return false;
                }

                intDataSaved += 1;
            }

            if (dtls.Count() == intDataSaved)
                return true;
            else
                return false;

        }

        private void CalculatePart(string docNo, string custCode)
        {
            var dtl = ctx.SpTrnSORDDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == docNo);
            var totSalesQty = dtl.Sum(m => m.QtyOrder) ?? 0;
            var totSalesAmt = dtl.Sum(m => m.SalesAmt) ?? 0;
            var totDiscAmt = dtl.Sum(m => m.DiscAmt) ?? 0;

            string taxCode = ctx.ProfitCenters.Find(CompanyCode, BranchCode, custCode, ProfitCenter).TaxCode;
            decimal taxPct = (string.IsNullOrEmpty(taxCode)) ? 0 : ctx.Taxes.Find(CompanyCode, taxCode).TaxPct.Value;
            var ppn_amt = Math.Truncate((totSalesAmt - totDiscAmt) * taxPct / 100);

            var recordHdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, docNo);

            if (recordHdr != null)
            {
                recordHdr.TotSalesQty = totSalesQty;
                recordHdr.TotSalesAmt = totSalesAmt;
                recordHdr.TotDiscAmt = totDiscAmt;
                recordHdr.TotDPPAmt = totSalesAmt - totDiscAmt;
                recordHdr.TotPPNAmt = ppn_amt;
                recordHdr.TotFinalSalesAmt = totSalesAmt - totDiscAmt + ppn_amt;
                try
                {
                    ctx.SaveChanges();
                }
                catch(Exception ex) {
                    throw new Exception(ex.Message);
                }
            }
        }

        public JsonResult Print(string docNo)
        {
            var hdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, docNo);
            var dtl = ctx.SpTrnSORDDtls.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == docNo);

            if (hdr.Status == "2")
            {
                return Json(new { success = true });
            }
            else
            {
                try
                {
                    hdr.Status = "1";
                    hdr.PrintSeq = hdr.PrintSeq + 1;
                    hdr.LastUpdateBy = CurrentUser.UserId;
                    hdr.LastUpdateDate = DateTime.Now;

                    dtl.Status = "1";
                    dtl.LastUpdateBy = CurrentUser.UserId;
                    dtl.LastUpdateDate = DateTime.Now;

                    ctx.SaveChanges();
                    return Json(new { success = true });
                }
                catch
                {
                    return Json(new { success = false, message = "Dokumen tidak dapat dicetak karena tidak memiliki data detail" });
                }
            }
        }

        public JsonResult GetCustomerDetail(string customerCode, string customerCodeBill, string salesType)
        {
            var custCodeBill = salesType == "2" ? customerCodeBill : customerCode;
             
            var oCustProfitCenter = ctx.ProfitCenters.Find(CompanyCode, BranchCode, customerCode, ProfitCenter);

            if (oCustProfitCenter != null)
            {
                var TopCode = oCustProfitCenter.TOPCode;
                var DiscPct = oCustProfitCenter.DiscPct;

                var oLookUpDtl1 = ctx.LookUpDtls.Find(CompanyCode, "TOPC", oCustProfitCenter.TOPCode);

                if (oLookUpDtl1 != null)
                {
                    var oCustomer = ctx.GnMstCustomers.Find(CompanyCode, customerCode);
                    var oCustomerBill = ctx.GnMstCustomers.Find(CompanyCode, custCodeBill);
                    var TopDays = oLookUpDtl1.ParaValue;

                    var oLookUpDtl2 = ctx.LookUpDtls.Find(CompanyCode, "PYBY", oCustProfitCenter.PaymentCode);
                    var PaymentDesc = (oLookUpDtl2 != null) ? oLookUpDtl2.LookUpValueName : "";
                    return Json(new { topcode = TopCode, topdays = TopDays, paymentdesc = PaymentDesc, customerdtl = oCustomer, customerbilldtl = oCustomerBill, discPct = DiscPct });
                }
            }
            return null;
        }

        public JsonResult GetTransDetail(string docNo)
        {
            var detail = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, docNo) ;
            var table = GetPartTable(docNo);
            var employee =  ctx.Employees.Find(CompanyCode, BranchCode, detail.LockingBy) ;

            var x = table.ToList();
            var z = x;

            return Json(new { Detail = detail, Table = table, Employee = employee });
        }

        public JsonResult GetTransPORDDDetail(string customerCode, string orderNo)
        {
            md = DealerCode() == "MD";
            var hdrPORDD = ctx.SpUtlPORDDHdrs.Find(CompanyCode, BranchCode, customerCode, orderNo);
            var dtlPORDD = ctx.SpUtlPORDDDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DealerCode == customerCode && a.OrderNo == orderNo);
        
            var utlPORDDdtl = GetUtlPORDDdtl(customerCode, orderNo);

            decimal totSales = 0;
            decimal totDisc = 0;
            decimal totDPP = 0;
            decimal totPPN = 0;
            decimal total = 0;
            foreach (var dtl in utlPORDDdtl)
            {
                var oItemInfo = new MasterItemInfo();

                oItemInfo = ctx.MasterItemInfos.Find(CompanyName, dtl.PartNo);

                string supplierCode = (oItemInfo != null) ? oItemInfo.SupplierCode : string.Empty;

                var DiscPct = GetDiscountPct(dtl.DealerCode, supplierCode, dtl.PartNo, hdrPORDD.OrderType, hdrPORDD.OrderDate);
                decimal discAmt = dtl.QtyOrder.Value * dtl.RetailPrice.Value * dtl.DiscPct / 100;
                decimal netSalesAmt = dtl.SalesAmt.Value - discAmt;

                totSales = totSales + dtl.SalesAmt.Value;
                totDisc = totDisc + discAmt;
                totDPP = totDPP + netSalesAmt;
            }

            totPPN = GetIsPKP(customerCode) ? GetPPNAmount(customerCode, orderNo, totDPP) : 0;
            total = totDPP + totPPN;

            if (hdrPORDD != null)
            {
                var TransType = ctx.LookUpDtls.Find(CompanyCode, "TTPJ", hdrPORDD.OrderType).LookUpValueName;
                return Json(new { success = true, transType = TransType, totalSales = totSales, totalDisc = totDisc, totalDPP = totDPP, totalPPN = totPPN, total = total });
            }
            return Json("");
        }

        public IEnumerable<PartTable> GetPartTable(string docNo)
        {
            var query = string.Format(@"
                                SELECT 
                                 row_number () OVER (ORDER BY spTrnSORDDtl.CreatedDate ASC) AS No
                                ,spTrnSORDDtl.PartNo
                                ,spMstItemInfo.PartName
                                ,spTrnSORDDtl.QtyOrder
                                ,spTrnSORDDtl.SalesAmt
                                ,spTrnSORDDtl.DiscAmt
                                ,spTrnSORDDtl.NetSalesAmt
                                ,spTrnSORDDtl.WarehouseCode
                                ,spTrnSORDDtl.QtySupply
                                ,(spTrnSORDDtl.QtyBO - spTrnSORDDtl.QtyBOSupply - spTrnSORDDtl.QtyBOCancel) QtyBO
                                ,l.LookupValueName
                                ,spTrnSORDDtl.RetailPrice
                                ,spTrnSORDDtl.RetailPrice * spTrnSORDDtl.QtyOrder as NetAmt
                                ,spTrnSORDDtl.ReferenceNo
                                ,spTrnSORDDtl.DiscPct
                                ,spTrnSORDDtl.CostPrice
                                FROM spTrnSORDDtl with(nolock, nowait) INNER JOIN
                                spMstItemInfo ON spMstItemInfo.PartNo = spTrnSORDDtl.PartNo AND spMstItemInfo.CompanyCode = spTrnSORDDtl.CompanyCode
                                inner join gnMstLookupDtl l on spTrnSORDDtl.WarehouseCode = l.LookupValue AND spTrnSORDDtl.CompanyCode = l.CompanyCode
                                and l.CodeID = 'WRCD' 
                                WHERE spTrnSORDDtl.CompanyCode = '{0}'
                                AND spTrnSORDDtl.BranchCode = '{1}'
                                AND spTrnSORDDtl.DocNo = '{2}'
                                ", CompanyCode, BranchCode, docNo);

                return ctx.Database.SqlQuery<PartTable>(query);
        }

        private decimal GetPPNPct(string docNo)
        {
            var pct = (ctx.SpTrnSORDHdrs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == docNo)
                .Join(ctx.ProfitCenters, d => new { d.CompanyCode, d.BranchCode, d.CustomerCode }, e => new { e.CompanyCode, e.BranchCode, e.CustomerCode }, (d, e) => new { d, e })
                .Join(ctx.Taxes, x => new { x.e.CompanyCode, x.e.TaxCode }, y => new { y.CompanyCode, y.TaxCode }, (x, y) => new { x, y })
                .Select(z => z.y.TaxPct)).Distinct().FirstOrDefault();

            return pct == null ? 0 : Convert.ToDecimal(pct);
        }

        private bool GetIsPPNPct(string taxCode)
        {
            var percent = ctx.Taxes.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.TaxCode == taxCode).TaxPct;

            return percent == null ? false : percent > 0 ? true : false;
        }

        private bool GetIsPKP(string customerCode)
        {
            var oCust = ctx.GnMstCustomers.Find(CompanyCode, customerCode);
            var isPKP = (oCust.isPKP == null ? false : oCust.isPKP.Value);

            return isPKP;
        }

        private string GetPaymentValue(string paymentDesc)
        {
            var val = ctx.LookUpDtls.FirstOrDefault(m => m.CodeID == "PYBY" && m.LookUpValueName == paymentDesc && m.CompanyCode == CompanyCode).LookUpValue;
            return val;
        }

        private List<UtlPORDDdtlModel> GetUtlPORDDdtl(string customerCode, string orderNo)
        {
            var query = "exec uspfn_spGetUtlPORDDdtl";
            object[] parameters = { CompanyCode, BranchCode, customerCode, orderNo };

            var data = ctx.Database.SqlQuery<UtlPORDDdtlModel>(query, parameters);

            return data.ToList();
        }

        private decimal GetDiscountPct(string customerCode, string supplierCode, string partNo, string transtype, DateTime paramDate)
        {
            decimal discount = 0;

            // discount from master supplier
            var obj1 = ctx.ProfitCenters.Find(CompanyCode, BranchCode, customerCode, ProfitCenter);
            discount = (obj1 == null) ? discount : discount + obj1.DiscPct.Value;

            // discount from master ordertype
            var obj2 = ctx.LookUpDtls.Find(CompanyCode, "TTPJ", transtype);
            discount = (obj2 == null) ? discount : discount + Convert.ToDecimal(obj2.ParaValue);

            // discount from master campaign
            var obj3 = ctx.spMstSalesCampaigns.Find(CompanyCode, BranchCode, supplierCode, partNo, paramDate);
            if (obj3 != null && obj3.BegDate.Date <= paramDate && obj3.EndDate.Value.Date >= paramDate)
            {
                discount = discount + obj3.DiscPct.Value;
            }

            // discount from master spMstItems
            var obj4 = ctx.MasterItemInfos.Find(CompanyCode, partNo) ;
            discount = (obj4 == null) ? discount : discount + obj4.DiscPct.Value;

            // discount from Master GnMstCustomerDisc
            var obj5 =ctx.GnMstCustomerDiscs.Find(CompanyCode, BranchCode, customerCode, ProfitCenter, TypeOfGoods);
            discount = (obj5 == null) ? discount : discount + obj5.DiscPct;

            return (discount < 0 ? 0 : discount);
        }

        private decimal GetPPNAmount(string customerCode, string orderNo, decimal totDPP)
        {
            var ppnAmount = (from a in ctx.SpUtlPORDDHdrs
                                  from b in ctx.ProfitCenters
                                  from c in ctx.Taxes
                                  where a.CompanyCode == b.CompanyCode
                                  && a.BranchCode == b.BranchCode
                                  && a.DealerCode == b.CustomerCode
                                  && b.TaxCode == c.TaxCode
                                  && a.CompanyCode == CompanyCode
                                  && a.BranchCode == BranchCode
                                  && a.DealerCode == customerCode
                                  && a.OrderNo == orderNo
                                  && b.ProfitCenterCode == ProfitCenter
                                  select
                                      c.TaxPct * totDPP
                                 ).FirstOrDefault() / 100 ;

            ppnAmount = Convert.ToDecimal(Math.Floor(Convert.ToDouble(ppnAmount)).ToString());

            return ppnAmount == null ? 0 : ppnAmount.Value;
        }

        #region Validation

        private bool IsValidStatus(string docNo)
        {
            var cek = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, docNo);

            if (cek != null && Convert.ToInt32(cek.Status) > 1)
            {
                return false;
            }
            return true;
        }

        private bool isCheckStatus(string docNo)
        {
            var record = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, docNo);
            if (record != null && Convert.ToInt32(record.Status) > 1)
            {
                return false;
            }
            return true;
        }

        private decimal GetBOQuantityByCustomer(string customerCode, string partNo)
        {
            var x = ctx.SpTrnSORDDtls.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode && e.PartNo == partNo)
                .Join(ctx.SpTrnSORDHdrs.Where(d => d.CustomerCode == customerCode),
                e => new { e.CompanyCode, e.BranchCode, e.DocNo },
                d => new { d.CompanyCode, d.BranchCode, d.DocNo },
                (e, d) => new { e, d }).Select(a => new { BO = a.e.QtyBO - a.e.QtyBOSupply - a.e.QtyBOCancel }).Sum(a => a.BO) ;

            return x ?? 0;
        }

        private JsonResult isOverdueOrderExist(string customerCode, ProfitCenter recCustomer)
        {
            var recLookUpDtl = ctx.LookUpDtls.Find(CompanyCode, "TOPC", recCustomer.TOPCode);
            var islinktofinance = ctx.CoProfiles.Find(CompanyCode, BranchCode).IsLinkToFinance;
            if (recLookUpDtl != null)
            {
                if (recLookUpDtl.ParaValue != "0")
                {
                    if (islinktofinance.Value && !recCustomer.isOverDueAllowed.Value)
                    {
                        if (isOverdueOrder(customerCode))
                        {
                            return Json(new { success = false, message = "Anda memiliki transaksi yang telah jatuh tempo, silahkan selesaikan terlebih dahulu pembayaran untuk transaksi sebelumnya" });
                        }
                    }
                }
            }
            else
            {
                return Json(new { success = false, message = "Kode TOP belum disetting untuk pelanggan ini" });
            }
            return Json(null);
        }

        private bool isOverdueOrder(string CustomerCode)
        {
            var data =  ctx.ArInterfaces.Where(x => x.ProfitCenterCode == ProfitCenter)
                .Join(ctx.SpTrnSORDHdrs.Where(y => y.CompanyCode == CompanyCode && y.BranchCode == BranchCode && y.CustomerCode == CustomerCode),
                a => new { a.CustomerCode, a.CompanyCode, a.BranchCode },
                d => new { d.CustomerCode, d.CompanyCode, d.BranchCode },
                (a, b) => new { a, b }).Count() ;

            return data > 0 ? true : false;
        }

        private bool CheckCustomerShipBill(string custBill, string custShip)
        {
            var recTemp = ctx.GnMstCustomers.Find(CompanyCode, custBill);
            if (recTemp == null)
                return false;
            else
            {
                recTemp = ctx.GnMstCustomers.Find(CompanyCode, custShip);
                if (recTemp == null)
                    return false;
            }
            return true;
        }

        private void PreProcessValidation(string docNo, bool isStockAlloc)
        {
            if (string.IsNullOrEmpty(docNo)) { throw new Exception("Proses alokasi stok gagal \nNo. Dokumen tidak ditemukan."); }

            var hdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, docNo);
            if (hdr == null) { throw new Exception("Proses alokasi stok gagal \nData SO tidak ditemukan."); }

            if (!hdr.isLocked.Value)
            {
                var custProfit = ctx.ProfitCenters.Find(CompanyCode, BranchCode, hdr.CustomerCode, ProfitCenter);
                if (custProfit == null) { throw new Exception("Proses alokasi stok gagal \nkarena data Customer Profit tidak ditemukan."); }

                if (custProfit.isBlackList.Value) { throw new Exception("Proses alokasi stok gagal \nPelanggan ini di Black list"); }

                if (isStockAlloc)
                {
                    if (hdr.Status == "0") { throw new Exception("Proses alokasi stok gagal \nStatus dokumen masih open"); }
                    if (hdr.Status == "2") { throw new Exception("Proses alokasi stok gagal \nStatus dokumen telah diclose"); }

                    var countSODtl = ctx.SpTrnSORDDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == docNo).Count();
                    if (countSODtl < 0) { throw new Exception("Proses alokasi stok \nDokumen tidak memilik detail pesanan"); }
                }
                else
                {
                    if (hdr.Status == "2" || hdr.Status == "3" || hdr.Status == "4") { throw new Exception("SO tidak dapat dibatalkan"); }
                }
            }
        }

        private JsonResult PreProcessValidation_OLD(string docNo, bool isStockAlloc)
        {
            if (string.IsNullOrEmpty(docNo)) { return Json(new { success = false, message = "Proses alokasi stok gagal \nNo. Dokumen tidak ditemukan." }); }

            var hdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, docNo);
            if (hdr == null) { return Json(new { success = false, message = "Proses alokasi stok gagal \nData SO tidak ditemukan." }); }

            if (!hdr.isLocked.Value)
            {
                var custProfit = ctx.ProfitCenters.Find(CompanyCode, BranchCode, hdr.CustomerCode, ProfitCenter);
                if (custProfit == null) { return Json(new { success = false, message = "Proses alokasi stok gagal \nkarena data Customer Profit tidak ditemukan." }); }

                if (custProfit.isBlackList.Value) { return Json(new { success = false, message = "Proses alokasi stok gagal \nPelanggan ini di Black list" }); }

                if (isStockAlloc)
                {
                    if (hdr.Status == "0") { return Json(new { success = false, message = "Proses alokasi stok gagal \nStatus dokumen masih open" }); }
                    if (hdr.Status == "2") { return Json(new { success = false, message = "Proses alokasi stok gagal \nStatus dokumen telah diclose" }); }

                    var countSODtl = ctx.SpTrnSORDDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == docNo).Count();
                    if (countSODtl < 0) { return Json(new { success = false, message = "Proses alokasi stok \nDokumen tidak memilik detail pesanan" }); }
                }
                else
                {
                    if (hdr.Status == "2" || hdr.Status == "3" || hdr.Status == "4") { return Json(new { success = false, message = "SO tidak dapat dibatalkan" }); }
                }
            }

            return Json(null);
        }

        private List<spMstItemMod> fc_SelectModifikasi(string partNo)
        {
            List<spMstItemMod> list = new List<spMstItemMod>();

            var data = ctx.Database.SqlQuery<GetModification>("Select * from GetModifikasi(@p0)", partNo);

            if (data != null && data.Count() > 0)
            {
                foreach (var _data in data)
                {
                    spMstItemMod record = new spMstItemMod();
                    record.PartNo = _data.ID;
                    record.InterChangeCode = _data.InterChangeCode;
                    list.Add(record);
                }
            }
            return list;
        }

        private SpTrnSOSupply prepareSupplyData(spMstItem oSpMstItemsDao, SpTrnSORDHdr oSpTrnSORDHdr, string partNo, string partNoOriginal,
            string locationCode, decimal qtySupply, int supSeq, decimal retailPrice, decimal retailPriceIncTax, decimal costPrice, decimal discPct, bool md)
        {
            if (oSpTrnSORDHdr == null)
                throw new Exception("header data tidak ditemukan");

            var seq = ctx.SpTrnSOSupplys.Where(a => a.CompanyCode == CompanyCode
                && a.BranchCode == BranchCode
                && a.DocNo == oSpTrnSORDHdr.DocNo
                && a.SupSeq == supSeq).Count();

            SpTrnSOSupply oSpTrnSOSupply = new SpTrnSOSupply();
            oSpTrnSOSupply.CompanyCode = oSpTrnSORDHdr.CompanyCode;
            oSpTrnSOSupply.BranchCode = oSpTrnSORDHdr.BranchCode;
            oSpTrnSOSupply.DocNo = oSpTrnSORDHdr.DocNo;
            oSpTrnSOSupply.SupSeq = supSeq;
            oSpTrnSOSupply.PartNo = partNo;
            oSpTrnSOSupply.PartNoOriginal = partNoOriginal;
            oSpTrnSOSupply.PTSeq = seq == 0 ? 1 : seq + 1;
            oSpTrnSOSupply.PickingSlipNo = "";
            oSpTrnSOSupply.ReferenceNo = oSpTrnSORDHdr.OrderNo;
            oSpTrnSOSupply.ReferenceDate = oSpTrnSORDHdr.OrderDate;
            oSpTrnSOSupply.WarehouseCode = "00";
            oSpTrnSOSupply.LocationCode = locationCode;
            oSpTrnSOSupply.QtySupply = qtySupply;
            oSpTrnSOSupply.QtyPicked = 0;
            oSpTrnSOSupply.QtyBill = 0;

            if (partNoOriginal != partNo)
            {
                //spMstItemPrice recItemPrice = md ? ctx.spMstItemPrices.Find(oSpTrnSORDHdr.CompanyCode, oSpTrnSORDHdr.BranchCode, partNo) : 
                //    ctxMD.spMstItemPrices.Find(CompanyMD, BranchMD, partNo);
                //spMstItemPrice recItemPrice = ctx.spMstItemPrices.Find(oSpTrnSORDHdr.CompanyCode, oSpTrnSORDHdr.BranchCode, partNo);

                // Pembetulan COGS
                var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                                GetDbMD(), CompanyMD, BranchMD, partNo);
                spMstItemPrice recItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

                if (recItemPrice != null)
                {
                    oSpTrnSOSupply.RetailPrice = recItemPrice.RetailPrice; 
                    oSpTrnSOSupply.RetailPriceInclTax = recItemPrice.RetailPriceInclTax;
                    oSpTrnSOSupply.CostPrice = GetCostPrice(partNo);
                }
            }
            else
            {
                oSpTrnSOSupply.RetailPrice = retailPrice;
                oSpTrnSOSupply.RetailPriceInclTax = retailPriceIncTax;
                oSpTrnSOSupply.CostPrice = costPrice;
            }

            //spMstItem oSpMstItems = md ? ctx.spMstItems.Find(CompanyCode, BranchCode, partNo) : ctxMD.spMstItems.Find(CompanyMD, BranchMD, partNo);
            var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                GetDbMD(), CompanyMD, BranchMD, partNo);
            spMstItem oSpMstItems = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();
            if (oSpMstItems != null)
            {
                oSpTrnSOSupply.MovingCode = oSpMstItems.MovingCode;
                oSpTrnSOSupply.ABCClass = oSpMstItems.ABCClass;
                oSpTrnSOSupply.ProductType = oSpMstItems.ProductType;
                oSpTrnSOSupply.PartCategory = oSpMstItems.PartCategory;
                //oSpTrnSOSupply.DiscPct = oSpMstItems.PurcDiscPct;
            }

            SpMstItemInfo oSpMstItemInfo = ctx.SpMstItemInfos.Find(CompanyCode, partNo);
            string suppCode = oSpMstItemInfo == null ? string.Empty : oSpMstItemInfo.SupplierCode;

            oSpTrnSOSupply.DiscPct = discPct;
            oSpTrnSOSupply.Status = "0";
            oSpTrnSOSupply.CreatedBy = CurrentUser.UserId;
            oSpTrnSOSupply.CreatedDate = DateTime.Now;
            oSpTrnSOSupply.LastUpdateBy = CurrentUser.UserId;
            oSpTrnSOSupply.LastUpdateDate = DateTime.Now;

            return oSpTrnSOSupply;
        }

        //update item, itemloc dan insert spTrnSOSupply
        private bool UpdateStockAndSupply(SpTrnSOSupply oSpTrnSOSupply, SpTrnSORDHdr recordHeader, decimal itemOnHand, bool isAlloc, bool md)
        {
            // prepare return value 
            bool retVal = false;
            bool isretVal = false;
            ctx.SpTrnSOSupplys.Add(oSpTrnSOSupply);
            retVal = ctx.SaveChanges() > 0;

            // update stock 
            if (retVal)
                UpdateStock(oSpTrnSOSupply.PartNo, "00", 0, itemOnHand, 0, 0, recordHeader.SalesType, md, ref isretVal);
            retVal = isretVal;
            // insert movement log
            if (retVal)
            {
                retVal = MovementLog(recordHeader.DocNo, recordHeader.DocDate.Value, oSpTrnSOSupply.PartNo,
                            "00", "OUT", isAlloc ? (recordHeader.SalesType.Equals("0") ? SUBSIGN_CODE_SALES :
                            SUBSIGN_CODE_NON_SALES) : (recordHeader.SalesType.Equals("0") ? BOSUBSIGN_CODE_SALES :
                            BOSUBSIGN_CODE_NON_SALES), itemOnHand);
            }

            return retVal;
        }

        private JsonResult UpdateStock(string partno, string whcode, decimal onhand, decimal allocation, decimal backorder, decimal reserved, string salesType, bool md, ref bool result)
        {
            //spMstItem oItem = ctxMD.spMstItems.Find(CompanyMD, BranchMD, partno);
            var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                GetDbMD(), CompanyMD, BranchMD, partno);
            spMstItem oItem = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();

            //string msg = "";
            if (oItem != null)
            {
                //TODO : Tambahkan check result untuk yang hasilnya negatif
                //SpMstItemLoc oItemLoc = md ? ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcode) : ctxMD.SpMstItemLocs.Find(CompanyMD, BranchMD, partno, whcode);
                var sqlItemLoc = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                    GetDbMD(), CompanyMD, BranchMD, partno, whcode);
                SpMstItemLoc oItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

                if (oItemLoc != null)
                {
                    if (Math.Abs(onhand) > 0)
                    {
                        oItemLoc.OnHand += onhand;
                        oItem.OnHand += onhand;

                        // Tambahkan check result untuk yang ItemLoc negatif
                        if (oItemLoc.OnHand < 0)
                        {
                            msg = string.Format("OnHand untuk Part = {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.OnHand);
                            return Json(new { success = false, message = msg });
                        }

                        // Tambahkan check result untuk yang Item negatif
                        if (oItem.OnHand < 0)
                        {
                            msg = string.Format("OnHand untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.OnHand);
                            return Json(new { success = false, message = msg });
                        }
                    }

                    if (Math.Abs(allocation) > 0)
                    {
                        if (!string.IsNullOrEmpty(salesType) && (salesType == "0" || salesType == "1"))
                        {
                            oItemLoc.AllocationSP += allocation;
                            oItem.AllocationSP += allocation;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.AllocationSP < 0)
                            {
                                msg = string.Format("AllocationSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSP);
                                return Json(new { success = false, message = msg });
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.AllocationSP < 0)
                            {
                                msg = string.Format("AllocationSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSP);
                                return Json(new { success = false, message = msg });
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "2")
                        {
                            oItemLoc.AllocationSR += allocation;
                            oItem.AllocationSR += allocation;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.AllocationSR < 0)
                            {
                                msg = string.Format("AllocationSR untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSR);
                                return Json(new { success = false, message = msg });
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.AllocationSR < 0)
                            {
                                msg = string.Format("AllocationSR untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSR);
                                return Json(new { success = false, message = msg });
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "3")
                        {
                            oItemLoc.AllocationSL += allocation;
                            oItem.AllocationSL += allocation;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.AllocationSL < 0)
                            {
                                msg = string.Format("AllocationSL untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSL);
                                return Json(new { success = false, message = msg });
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.AllocationSL < 0)
                            {
                                msg = string.Format("AllocationSL untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSL);
                                return Json(new { success = false, message = msg });
                            }
                        }
                    }

                    if (Math.Abs(backorder) > 0)
                    {
                        if (!string.IsNullOrEmpty(salesType) && (salesType == "0" || salesType == "1"))
                        {
                            oItemLoc.BackOrderSP += backorder;
                            oItem.BackOrderSP += backorder;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.BackOrderSP < 0)
                            {
                                msg = string.Format("BackOrderSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSP);

                                return Json(new { success = false, message = msg });
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.BackOrderSP < 0)
                            {
                                msg = string.Format("BackOrderSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSP);
                                return Json(new { success = false, message = msg });
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "2")
                        {
                            oItemLoc.BackOrderSR += backorder;
                            oItem.BackOrderSR += backorder;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.BackOrderSR < 0)
                            {
                                msg = string.Format("BackOrderSR untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSR);
                                return Json(new { success = false, message = msg });
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.BackOrderSR < 0)
                            {
                                msg = string.Format("BackOrderSR untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSR);
                                return Json(new { success = false, message = msg });
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "3")
                        {
                            oItemLoc.BackOrderSL += backorder;
                            oItem.BackOrderSL += backorder;

                            // Tambahkan check result untuk yang ItemLoc negatif
                            if (oItemLoc.BackOrderSL < 0)
                            {
                                msg = string.Format("BackOrderSL untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSL);
                                return Json(new { success = false, message = msg });
                            }

                            // Tambahkan check result untuk yang Item negatif
                            if (oItem.BackOrderSL < 0)
                            {
                                msg = string.Format("BackOrderSL untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSL);
                                return Json(new { success = false, message = msg });
                            }
                        }
                    }

                    if (Math.Abs(reserved) > 0)
                    {
                        oItemLoc.ReservedSP += reserved;
                        oItem.ReservedSP += reserved;

                        // Tambahkan check result untuk yang ItemLoc negatif
                        if (oItemLoc.ReservedSP < 0)
                        {
                            msg = string.Format("ReservedSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.ReservedSP);
                            return Json(new { success = false, message = msg });
                        }

                        // Tambahkan check result untuk yang Item negatif
                        if (oItem.ReservedSP < 0)
                        {
                            msg = string.Format("ReservedSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.ReservedSP);
                            return Json(new { success = false, message = msg });
                        }
                    }
                    
                    //result = md ? ctx.SaveChanges() > 0 : ctxMD.SaveChanges() > 0;
                    ctx.SaveChanges();

                    //oItemLoc.LastUpdateDate = DateTime.Now;
                    //oItemLoc.LastUpdateBy = CurrentUser.UserId;
                    //oItem.LastUpdateDate = DateTime.Now;
                    //oItem.LastUpdateBy = CurrentUser.UserId;
                    //ctxMD.SaveChanges();

                    var sqlUpdateItemLocMD = string.Format(@"UPDATE {0}..SpMstItemLoc SET 
                        OnHand ={1}
                        ,AllocationSP ={2}
                        ,AllocationSR ={3}
                        ,AllocationSL ={4}
                        ,BackOrderSP ={5}
                        ,BackOrderSR ={6}
                        ,BackOrderSL ={7}
                        ,ReservedSP ={8}
                        ,LastUpdateDate ='{9}'
                        ,LastUpdateBy ='{10}' 
                    WHERE CompanyCode='{11}' AND BranchCode ='{12}' AND PartNo ='{13}' AND WarehouseCode ='{14}'", 
                        GetDbMD()
                        ,oItemLoc.OnHand        
                        ,oItemLoc.AllocationSP 
                        ,oItemLoc.AllocationSR 
                        ,oItemLoc.AllocationSL 
                        ,oItemLoc.BackOrderSP  
                        ,oItemLoc.BackOrderSR  
                        ,oItemLoc.BackOrderSL  
                        ,oItemLoc.ReservedSP   
                        ,DateTime.Now
                        ,CurrentUser.UserId
                        ,CompanyMD, BranchMD, partno, whcode
                    );
                    ctx.Database.ExecuteSqlCommand(sqlUpdateItemLocMD);

                    var sqlUpdateItemMD = string.Format(@"UPDATE {0}..spMstItems SET 
                        OnHand ={1}
                        ,AllocationSP ={2}
                        ,AllocationSR ={3}
                        ,AllocationSL ={4}
                        ,BackOrderSP ={5}
                        ,BackOrderSR ={6}
                        ,BackOrderSL ={7}
                        ,ReservedSP ={8}
                        ,LastUpdateDate ='{9}'
                        ,LastUpdateBy ='{10}' 
                    WHERE CompanyCode='{11}' AND BranchCode ='{12}' AND PartNo ='{13}'",
                        GetDbMD()
                        ,oItem.OnHand
                        ,oItem.AllocationSP
                        ,oItem.AllocationSR
                        ,oItem.AllocationSL
                        ,oItem.BackOrderSP
                        ,oItem.BackOrderSR
                        ,oItem.BackOrderSL
                        ,oItem.ReservedSP
                        ,DateTime.Now
                        ,CurrentUser.UserId
                        ,CompanyMD, BranchMD, partno
                    );
                    ctx.Database.ExecuteSqlCommand(sqlUpdateItemMD);
                    
                    result = true;
                    //if (result)
                    //{
                        
                    //    result = md ? ctx.SaveChanges() > 0 : ctxMD.SaveChanges() > 0;
                    //}
                }
            }

            return Json(result);
        }

        public bool MovementLog(string docno, DateTime docdate, string partno, string whcode, string signcode, string subsigncode, decimal qty)
        {
            bool result = false;

            bool md = DealerCode() == "MD";

            var oIMovement = new SpTrnIMovement();
            //spMstItemPrice oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, partno);
            var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                        GetDbMD(), CompanyMD, BranchMD, partno);
            spMstItemPrice oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();
            
            //spMstItem oItems = md ? ctx.spMstItems.Find(CompanyCode, BranchCode, partno) : ctxMD.spMstItems.Find(CompanyMD, BranchMD, partno);
            var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                GetDbMD(), CompanyMD, BranchMD, partno);
            spMstItem oItems = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();
            
            //SpMstItemLoc oItemLoc = md ? ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcode) : ctxMD.SpMstItemLocs.Find(CompanyMD, BranchMD, partno, whcode);
            var sqlItemLoc = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                GetDbMD(), CompanyMD, BranchMD, partno, whcode);
            SpMstItemLoc oItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

            if (oItemLoc != null && oItemPrice != null && oItems != null)
            {
                oIMovement.CompanyCode = CompanyCode;
                oIMovement.BranchCode = BranchCode;
                oIMovement.DocNo = docno;
                oIMovement.DocDate = docdate;
                oIMovement.CreatedDate = DateTime.Now;

                oIMovement.WarehouseCode = oItemLoc.WarehouseCode;
                oIMovement.LocationCode = oItemLoc.LocationCode;
                oIMovement.PartNo = oItemLoc.PartNo;
                oIMovement.SignCode = signcode;
                oIMovement.SubSignCode = subsigncode;
                oIMovement.Qty = qty;
                oIMovement.Price = GetRetailPrice(oItemLoc.PartNo, oItemPrice.RetailPrice.Value); //oItemPrice.RetailPrice; 
                oIMovement.CostPrice = GetCostPrice(partno); // oItemPrice.CostPrice;
                oIMovement.ABCClass = oItems.ABCClass;
                oIMovement.MovingCode = oItems.MovingCode;
                oIMovement.ProductType = oItems.ProductType;
                oIMovement.PartCategory = oItems.PartCategory;
                oIMovement.CreatedBy = CurrentUser.UserId;

                ctx.SpTrnIMovements.Add(oIMovement);
                result = ctx.SaveChanges() > 0;
            }
            return result;
        }

        private bool UpdateHistory(SpTrnSORDDtl recordDetail, SpTrnSORDHdr recordHeader, decimal fiscalYear, decimal fiscalMonth)
        {
            bool isHstDemandItemNew = false;
            bool isHstDemandCustNew = false;

            // prepare DAO objects for current transaction            

            // prepare return value 
            bool retVal = true;

            // insert demand history item
            if (retVal)
            {
                SpHstDemandItem recordDemandItem = prepareDemandItemRecord(recordDetail, recordHeader, ref isHstDemandItemNew, fiscalYear, fiscalMonth);
                if (isHstDemandItemNew)
                {
                    ctx.SpHstDemandItems.Add(recordDemandItem);
                }
                retVal = ctx.SaveChanges() > 0;
            }

            // insert demand history customer
            if (retVal)
            {
                SpHstDemandCust recordDemandCust = prepareDemandCustRecord(recordDetail, recordHeader, ref isHstDemandCustNew, fiscalYear, fiscalMonth);
                if (isHstDemandCustNew)
                {
                    ctx.SpHstDemandCusts.Add(recordDemandCust);
                }

                retVal = ctx.SaveChanges() > 0;
            }

            return retVal;
        }

        private SpHstDemandItem prepareDemandItemRecord(SpTrnSORDDtl recordDetail, SpTrnSORDHdr recordHeader,ref bool isNew, decimal fiscalYear, decimal fiscalMonth)
        {
            isNew = false;

            SpHstDemandItem HstDemand = ctx.SpHstDemandItems.Find(CompanyCode, BranchCode, fiscalYear, fiscalMonth, recordDetail.PartNo);

            if (HstDemand == null)
            {
                HstDemand = new SpHstDemandItem();
                HstDemand.CompanyCode = CompanyCode;
                HstDemand.BranchCode = BranchCode;
                HstDemand.Year = fiscalYear;
                HstDemand.Month = fiscalMonth;
                HstDemand.PartNo = recordDetail.PartNo;
                HstDemand.DemandFreq = ((recordHeader.SalesType == "2" || recordHeader.SalesType == "3") && recordDetail.QtySupply == 0) ? 0 : 1;
                HstDemand.DemandQty = (recordHeader.SalesType == "2" || recordHeader.SalesType == "3") ? recordDetail.QtySupply : recordDetail.QtyOrder;
                HstDemand.MovingCode = recordDetail.MovingCode;
                HstDemand.ProductType = recordDetail.ProductType;
                HstDemand.PartCategory = recordDetail.PartCategory;
                HstDemand.ABCClass = recordDetail.ABCClass;
                HstDemand.SalesFreq = 0;
                HstDemand.SalesQty = 0;
                isNew = true;
            }
            else
            {
                HstDemand.DemandFreq += ((recordHeader.SalesType == "2" || recordHeader.SalesType == "3") && recordDetail.QtySupply == 0) ? 0 : 1;
                HstDemand.DemandQty += (recordHeader.SalesType == "2" || recordHeader.SalesType == "3") ? recordDetail.QtySupply : recordDetail.QtyOrder;
            }

            HstDemand.LastUpdateBy = CurrentUser.UserId;
            HstDemand.LastUpdateDate = DateTime.Now;

            return HstDemand;
        }

        private SpHstDemandCust prepareDemandCustRecord(SpTrnSORDDtl recordDetail, SpTrnSORDHdr recordHeader, ref bool isNew, decimal fiscalYear, decimal fiscalMonth)
        {
            isNew = false;

            SpHstDemandCust hstDemandCustomer = ctx.SpHstDemandCusts.Find(CompanyCode, BranchCode, fiscalYear, fiscalMonth, recordHeader.CustomerCode, recordDetail.PartNo);

            if (hstDemandCustomer == null)
            {
                hstDemandCustomer = new SpHstDemandCust();
                hstDemandCustomer.CompanyCode = CompanyCode;
                hstDemandCustomer.BranchCode = BranchCode;
                hstDemandCustomer.Year = fiscalYear;
                hstDemandCustomer.Month = fiscalMonth;
                hstDemandCustomer.CustomerCode = recordHeader.CustomerCode;
                hstDemandCustomer.PartNo = recordDetail.PartNo;
                hstDemandCustomer.DemandFreq = ((recordHeader.SalesType == "2" || recordHeader.SalesType == "3") && recordDetail.QtySupply == 0) ? 0 : 1;
                hstDemandCustomer.DemandQty = (recordHeader.SalesType == "2" || recordHeader.SalesType == "3") ? recordDetail.QtySupply : recordDetail.QtyOrder;
                hstDemandCustomer.MovingCode = recordDetail.MovingCode;
                hstDemandCustomer.ProductType = recordDetail.ProductType;
                hstDemandCustomer.partCategory = recordDetail.PartCategory;
                hstDemandCustomer.ABCClass = recordDetail.ABCClass;
                hstDemandCustomer.SalesFreq = 0;
                hstDemandCustomer.SalesQty = 0;
                isNew = true;
            }
            else
            {
                hstDemandCustomer.DemandFreq += ((recordHeader.SalesType == "2" || recordHeader.SalesType == "3") && recordDetail.QtySupply == 0) ? 0 : 1;
                hstDemandCustomer.DemandQty += (recordHeader.SalesType == "2" || recordHeader.SalesType == "3") ? recordDetail.QtySupply : recordDetail.QtyOrder;
            }

            hstDemandCustomer.LastUpdateBy = CurrentUser.UserId;
            hstDemandCustomer.LastUpdateDate = DateTime.Now;

            return hstDemandCustomer;
        }

        private void UpdateLastItemDate(string partno, string isUpdateWhat, bool md)
        {
            //spMstItem oItem = md ? ctx.spMstItems.Find(CompanyCode, BranchCode, partno) : ctxMD.spMstItems.Find(CompanyMD, BranchMD, partno);
            var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                GetDbMD(), CompanyMD, BranchMD, partno);
            spMstItem oItem = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();
            
            if (oItem != null)
            {
                if (isUpdateWhat == "LDD") // LastDemandDate
                {
                    oItem.LastDemandDate = DateTime.Now;
                }
                else if (isUpdateWhat == "LPD") // LastPurchaseDate
                {
                    oItem.LastPurchaseDate = DateTime.Now;
                }
                else if (isUpdateWhat == "LSD") // LastSalesDate
                {
                    oItem.LastSalesDate = DateTime.Now;
                }
                //oItem.LastUpdateDate = DateTime.Now;
                //oItem.LastUpdateBy = CurrentUser.UserId;

                //if (md) { ctx.SaveChanges(); }
                //else { ctxMD.SaveChanges(); }

                var sqlUpdateItem = string.Format(@"UPDATE {0}..spMstItems SET 
                    LastDemandDate='{1}'
                    ,LastPurchaseDate='{2}'
                    ,LastSalesDate='{3}'
                    ,LastUpdateDate='{4}'
                    ,LastUpdateBy='{5}'
                    WHERE CompanyCode='{6}' AND BranchCode ='{7}' AND PartNo ='{8}'",
                    GetDbMD()
                    , oItem.LastDemandDate
                    , oItem.LastPurchaseDate
                    , oItem.LastSalesDate
                    , DateTime.Now
                    , CurrentUser.UserId,
                    CompanyMD, BranchMD, partno);

               ctx.Database.ExecuteSqlCommand(sqlUpdateItem);

            }
        }

        private bool UpdateHeaderAndBankBook(string custCode, string salesType, decimal totFinalSalesAmt)
            //, ref string errMsg)
        {
            bool retVal = false;

            // calculate final sales amount, used by validation and NetSalesAmt in gnTrnBankBook 
            // validation
            switch (salesType)
            {
                // penjualan
                case "0":
                    #region check credit limit
                    // prepare dao object 

                    // get data transaction for current customer
                    BankBook oGnTrnBankBook = ctx.BankBooks.Find(CompanyCode, BranchCode, custCode, ProfitCenter);

                    // get customer information (credit limit)
                    ProfitCenter oGnMstCustomerProfitCenter = ctx.ProfitCenters.Find(CompanyCode, BranchCode, custCode, ProfitCenter);

                    // calculate credit for current customer 
                    var decCreditAlreadyUsed = oGnTrnBankBook == null ? 0 : oGnTrnBankBook.SalesAmt - oGnTrnBankBook.ReceivedAmt;

                    // check if credit limit is enough for current order 
                    var recLookUpDtl = ctx.LookUpDtls.Find(CompanyCode, "TOPC", oGnMstCustomerProfitCenter.TOPCode);
                    if (recLookUpDtl != null)
                    {
                        if (recLookUpDtl.ParaValue != "0")
                        {
                            if (oGnMstCustomerProfitCenter.CreditLimit < decCreditAlreadyUsed + totFinalSalesAmt
                                && CurrentUser.CoProfile.IsLinkToFinance.Value)
                            {
                                msg = "Sisa kredit anda tidak mencukupi untuk melanjutkan transaksi";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        msg = "Kode TOP belum disetting untuk pelanggan ini";
                        return false;
                    }
                    break;
                    #endregion

                // non penjualan
                case "1":
                    break;

                default:
                    break;
            }


            // insert bank book
            if (salesType == "0")
            {
                retVal = UpdateBankBook(custCode, totFinalSalesAmt, true);
                //retVal = true;
            }
            else
                retVal = true;

            return retVal;
        }

        private bool UpdateBankBook(string custCode, decimal amount, bool isSales)
        {
            bool result = false;
            BankBook oBankBook = ctx.BankBooks.Find(CompanyCode, BranchCode, custCode, ProfitCenter);
            try
            {
                bool isNew = false;
                if (oBankBook == null)
                {
                    isNew = true;
                    oBankBook = new BankBook();
                    oBankBook.CompanyCode = CompanyCode;
                    oBankBook.BranchCode = BranchCode;
                    oBankBook.CustomerCode = custCode;
                    oBankBook.ProfitCenterCode = ProfitCenter;
                    oBankBook.SalesAmt = oBankBook.ReceivedAmt = 0;
                }

                if (isSales) oBankBook.SalesAmt += amount;

                if (isNew)
                {
                    ctx.BankBooks.Add(oBankBook); 
                    result = ctx.SaveChanges() > 0;
                }
                else if (isSales)
                {
                    result = ctx.SaveChanges() > 0; 
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool UpdateSupplySlipNo(string usageDocNo, string docNo)
        {
            var datas = ctx.Database.SqlQuery<ParNoSupplied>("exec uspfn_GetPartNoSuppliedWeb '{0}','{1}','{2}','{3}','{4}'", CompanyCode, BranchCode, ProductType, docNo, usageDocNo);

            foreach (var data in datas)
            {
                decimal latestPartSeq = GetLatestPartSeq(data.ServiceNo, data.PartNo);

                SvTrnSrvItem recSrvItem = ctx.SvTrnSrvItems.Find(CompanyCode, BranchCode, ProductType, data.ServiceNo, data.PartNo, latestPartSeq); 

                if (recSrvItem == null) return false;
                bool result = false;

                // Insert SvTrnSrvItem
                if (!string.IsNullOrEmpty(recSrvItem.SupplySlipNo) && recSrvItem.SupplyQty >= 0)
                {
                    latestPartSeq = GetLatestPartSeq(data.ServiceNo, "");
                    recSrvItem = new SvTrnSrvItem();
                    recSrvItem.CompanyCode = data.CompanyCode;
                    recSrvItem.BranchCode = data.BranchCode;
                    recSrvItem.ProductType = data.ProductType;
                    recSrvItem.ServiceNo = data.ServiceNo;
                    recSrvItem.PartNo = data.PartNo;
                    recSrvItem.PartSeq = latestPartSeq + 1;
                    recSrvItem.SupplyQty = data.QtySupply;
                    recSrvItem.CostPrice = GetCostPrice(data.PartNo);  //data.CostPrice;
                    recSrvItem.RetailPrice = data.RetailPrice;
                    recSrvItem.TypeOfGoods = data.TypeOfGoods;
                    recSrvItem.BillType = data.BillType;
                    recSrvItem.SupplySlipNo = docNo;
                    recSrvItem.SupplySlipDate = data.SupplySlipDate;
                    recSrvItem.DiscPct = data.DiscPct;
                    recSrvItem.CreatedBy = CurrentUser.UserId;
                    recSrvItem.CreatedDate = DateTime.Now;
                    recSrvItem.LastupdateBy = CurrentUser.UserId;
                    recSrvItem.LastupdateDate = DateTime.Now;
                    ctx.SvTrnSrvItems.Add(recSrvItem);
                    result = ctx.SaveChanges() > 0;
                }

                // Update SvTrnSrvItem
                else if (string.IsNullOrEmpty(recSrvItem.SupplySlipNo) && recSrvItem.SupplyQty == 0)
                {
                    recSrvItem.SupplySlipNo = docNo;
                    recSrvItem.SupplySlipDate = data.SupplySlipDate;
                    recSrvItem.LastupdateDate = DateTime.Now;

                    if (recSrvItem.DemandQty < data.QtySupply)
                    {
                        decimal countMinus = data.QtySupply - recSrvItem.DemandQty.Value;
                        recSrvItem.SupplyQty = recSrvItem.DemandQty;
                        result = ctx.SaveChanges() > 0;

                        #region ** Insert New Sequence
                        latestPartSeq = GetLatestPartSeq(data.ServiceNo, "");
                        recSrvItem = new SvTrnSrvItem();
                        recSrvItem.CompanyCode = data.CompanyCode;
                        recSrvItem.BranchCode = data.BranchCode;
                        recSrvItem.ProductType = data.ProductType;
                        recSrvItem.ServiceNo = data.ServiceNo;
                        recSrvItem.PartNo = data.PartNo;
                        recSrvItem.PartSeq = latestPartSeq + 1;
                        recSrvItem.SupplyQty = countMinus;
                        recSrvItem.CostPrice = GetCostPrice(data.PartNo); //data.CostPrice;
                        recSrvItem.RetailPrice = data.RetailPrice;
                        recSrvItem.TypeOfGoods = data.TypeOfGoods;
                        recSrvItem.BillType = data.BillType;
                        recSrvItem.SupplySlipNo = docNo;
                        recSrvItem.SupplySlipDate = data.SupplySlipDate;
                        recSrvItem.CreatedBy = CurrentUser.UserId;
                        recSrvItem.CreatedDate = DateTime.Now;
                        recSrvItem.LastupdateBy = CurrentUser.UserId;
                        recSrvItem.LastupdateDate = DateTime.Now;
                        ctx.SvTrnSrvItems.Add(recSrvItem);
                        result = ctx.SaveChanges() > 0;
                        #endregion
                    }
                    else
                    {
                        recSrvItem.SupplyQty = data.QtySupply;
                        recSrvItem.LastupdateBy = CurrentUser.UserId;
                        result = ctx.SaveChanges() > 0;
                    }

                }

                if (!result) return result;
            }

            return true;
        }

        public decimal GetLatestPartSeq(Int64 serviceNo, string partNo)
        {
            var partSeq = ctx.SvTrnSrvItems.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode 
                && a.ProductType == ProductType && a.ServiceNo == serviceNo && a.PartNo == (partNo != "" ? partNo : a.PartNo))
                .Max(a=>a.PartSeq);

            return partSeq;
        }

        private void SDMovementAdd(SpTrnSORDDtl record, string docno, DateTime docdate)
        {
            string whcode = "00";
            string partno = record.PartNo;
            
            //spMstItemPrice oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, partno);
            var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                GetDbMD(), CompanyMD, BranchMD, partno);
            spMstItemPrice oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();            

            //spMstItem oItems = ctxMD.spMstItems.Find(CompanyMD, BranchMD, partno);
            var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                GetDbMD(), CompanyMD, BranchMD, partno);
            spMstItem oItems = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();

            //SpMstItemLoc oItemLoc = ctxMD.SpMstItemLocs.Find(CompanyMD, BranchMD, partno, whcode);
            var sqlItemLoc = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                GetDbMD(), CompanyMD, BranchMD, partno, whcode);
            SpMstItemLoc oItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();
            
            //var data = ctxMD.SvSDMovements.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == docno);
            //var oSDMovement = data.FirstOrDefault(a => a.PartNo == record.PartNo);
            var sqlSeq = string.Format("SELECT * FROM {0}..svSDMovement WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND DocNo ='{3}'",
                GetDbMD(), CompanyCode, BranchCode, docno);
            var data = ctx.Database.SqlQuery<SvSDMovement>(sqlSeq);

            var seq = data.FirstOrDefault() == null ? 0 : data.Max(a => a.PartSeq);
            //var oSDMovement = new SvSDMovement();
            //oSDMovement.CompanyCode = CompanyCode;
            //oSDMovement.BranchCode = BranchCode;
            //oSDMovement.DocNo = docno;
            //oSDMovement.DocDate = docdate;
            //oSDMovement.PartNo = oItemLoc.PartNo;
            //oSDMovement.PartSeq = seq + 1;
            //oSDMovement.WarehouseCode = oItemLoc.WarehouseCode;
            //oSDMovement.QtyOrder = record.QtyOrder.Value;
            //oSDMovement.Qty = record.QtyOrder.Value;
            //oSDMovement.DiscPct = record.DiscPct.Value;
            //oSDMovement.CostPrice = record.CostPrice.Value;
            //oSDMovement.RetailPrice = record.RetailPrice.Value;
            //oSDMovement.TypeOfGoods = TypeOfGoods;
            //oSDMovement.CompanyMD = CompanyMD;
            //oSDMovement.BranchMD = BranchMD;
            //oSDMovement.WarehouseMD = WarehouseMD;
            //oSDMovement.RetailPriceInclTaxMD = oItemPrice.RetailPriceInclTax ?? 0;
            //oSDMovement.RetailPriceMD = oItemPrice.RetailPrice.Value;
            //oSDMovement.CostPriceMD = oItemPrice.CostPrice.Value;
            //oSDMovement.QtyFlag = "-";
            //oSDMovement.ProductType = ProductType;
            //oSDMovement.ProfitCenterCode = ProfitCenter;
            //oSDMovement.Status = "0";
            //oSDMovement.ProcessStatus = "0";
            //oSDMovement.ProcessDate = DateTime.Now;
            //oSDMovement.CreatedBy = CurrentUser.UserId;
            //oSDMovement.CreatedDate = DateTime.Now;
            //oSDMovement.LastUpdateBy = CurrentUser.UserId;
            //oSDMovement.LastUpdateDate = DateTime.Now;

            //ctxMD.SvSDMovements.Add(oSDMovement);
            //ctxMD.SaveChanges();
            var sqlInsertSDMvt = string.Format(@"INSERT INTO {0}..svSDMovement(
                    CompanyCode,BranchCode,DocNo,DocDate,PartNo,PartSeq,WarehouseCode
                    ,QtyOrder,Qty,DiscPct,CostPrice,RetailPrice,TypeOfGoods,CompanyMD
                    ,BranchMD,WarehouseMD,RetailPriceInclTaxMD,RetailPriceMD,CostPriceMD
                    ,QtyFlag,ProductType,ProfitCenterCode,Status,ProcessStatus
                    ,ProcessDate,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate)
                VALUES(
                    '{1}','{2}','{3}','{4}','{5}','{6}','{7}'
                    ,{8},{9},{10},{11},{12},'{13}','{14}'
                    ,'{15}','{16}',{17},{18},{19}
                    ,'{20}','{21}','{22}','{23}','{24}'
                    ,'{25}','{26}','{27}','{28}','{29}')",
                GetDbMD(),
                CompanyCode, BranchCode, docno, docdate, oItemLoc.PartNo, seq + 1, oItemLoc.WarehouseCode
                , record.QtyOrder.Value, record.QtyOrder.Value, record.DiscPct.Value, Math.Floor(record.CostPrice.Value), record.RetailPrice.Value, TypeOfGoods, CompanyMD
                , BranchMD, WarehouseMD, oItemPrice.RetailPriceInclTax ?? 0, oItemPrice.RetailPrice.Value, oItemPrice.CostPrice.Value
                , "-", ProductType, ProfitCenter, "0", "0"
                , DateTime.Now, CurrentUser.UserId, DateTime.Now, CurrentUser.UserId, DateTime.Now);

            ctx.Database.ExecuteSqlCommand(sqlInsertSDMvt);
        }

        public JsonResult getQtyAccUnitOrder(string SONo, string PartNo)
        {
            decimal? qty = ctx.Database.SqlQuery<decimal?>("SELECT DemandQty FROM omtrSalesSOaccsseq WHERE Sono='" + SONo + "' AND partno='" + PartNo + "'").FirstOrDefault();
            if (qty > 0 || qty != null)
            {
                return Json(new { success = true, qty = qty });
            }
            return Json(new { success = false });
        }

        #endregion

        public JsonResult GetLookupEPS_FLAG()
        {
            string bEnabled = "0";
            var record = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "DISC_EPS_FLAG" && p.LookUpValue == BranchCode).FirstOrDefault();
            if (record != null)
                bEnabled = record.ParaValue;

            return Json(new { success = true, enabled = (bEnabled == "0") ? true : false });
        }

        private class PREQDtl
        {
            public decimal SeqNo {get; set;}
            public string REQNo {get; set;}
            public string PartNo {get; set;}
            public string PartName {get; set;}
            public decimal? QtyOrder { get; set; }
        }

        public JsonResult GetTranPREQ(string REQNo) 
        {
            var header = ctx.spTrnPREQHdr.Where(m => m.CompanyCode == CompanyCode && m.SupplierCode == BranchCode && m.REQNo == REQNo).FirstOrDefault();
            //var detail = ctx.spTrnPREQDtl.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.REQNo == REQNo).ToList();
            var detail = (from p in ctx.spTrnPREQDtl
                         join p1 in ctx.SpMstItemInfos on new { p.CompanyCode, p.PartNo } equals new { p1.CompanyCode, p1.PartNo }
                         where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.REQNo == REQNo
                         select new PREQDtl()
                            {
                                SeqNo = p.SeqNo,
                                REQNo = p.REQNo,
                                PartNo = p.PartNo,
                                PartName = p1.PartName,
                                QtyOrder = p.OrderQty
                            }).ToList();
            var customer = "";
            if (header != null)
            {
                customer = ctx.GnMstCustomers.Where(m => m.CompanyCode == CompanyCode && m.CustomerCode == header.BranchCode).Select(a => a.CustomerName).FirstOrDefault().ToString();
            }
            return Json(new { success = true, Detail = detail, Header = header, Customer = customer });
        }

        public JsonResult SavePartBranch(SpTrnSORDDtl modelDtl) 
        {
            using (var transPart = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                md = DealerCode() == "MD";
                var Hdr = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, modelDtl.DocNo);
                var CustomerCode = Hdr.CustomerCode;
                var DocNo = Hdr.DocNo;
                var DocDate = Hdr.DocDate;

                var oCustProfitCenter = ctx.ProfitCenters.Find(CompanyCode, BranchCode, CustomerCode, ProfitCenter);

                var oSORHdrInq = ctx.SpTrnSORDHdrs.Find(CompanyCode, BranchCode, DocNo);
                if (Convert.ToInt32(oSORHdrInq.Status) > 1) { return Json(new { success = false, message = "Status Transaksi sudah di alokasi, data tidak dapat diupdate" }); }

                var recordDtl = ctx.SpTrnSORDDtls.Find(CompanyCode, BranchCode, DocNo, modelDtl.PartNo, WarehouseMD, modelDtl.PartNo);
                if (recordDtl == null)
                {
                    var oItems = new spMstItem();
                    var oItemLoc = new SpMstItemLoc();
                    var oItemPrice = new spMstItemPrice();
                    //if (md)
                    //{
                    //oItems = ctxMD.spMstItems.Find(CompanyMD, BranchMD, modelDtl.PartNo);
                    var sqlItem = string.Format("SELECT * FROM spMstItems WHERE CompanyCode='{0}' AND BranchCode ='{1}' AND PartNo ='{2}'",
                        CompanyCode, BranchCode, modelDtl.PartNo);
                    oItems = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();

                    var sqlItemLoc = string.Format("SELECT * FROM SpMstItemLoc WHERE CompanyCode='{0}' AND BranchCode ='{1}' AND PartNo ='{2}' AND WarehouseCode ='{3}'",
                      CompanyCode, BranchCode, modelDtl.PartNo, WarehouseMD);
                    oItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

                    var sqlItemPrice = string.Format("SELECT * FROM spMstItemPrice WHERE CompanyCode='{0}' AND BranchCode ='{1}' AND PartNo ='{2}'",
                           CompanyCode, BranchCode, modelDtl.PartNo);
                    oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

                    if (md)
                    {
                        sqlItem = string.Format("SELECT * FROM " + GetDbMD() + @"..spMstItems WHERE CompanyCode='{0}' AND BranchCode ='{1}' AND PartNo ='{2}'",
                        CompanyMD, BranchMD, modelDtl.PartNo);
                        oItems = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();

                        sqlItemLoc = string.Format("SELECT * FROM " + GetDbMD() + @"..SpMstItemLoc WHERE CompanyCode='{0}' AND BranchCode ='{1}' AND PartNo ='{2}' AND WarehouseCode ='{3}'",
                       CompanyMD, BranchMD, modelDtl.PartNo, WarehouseMD);
                        oItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

                        //oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, modelDtl.PartNo); //ctxMD.spMstItemPrices.Find(CompanyMD, BranchMD, modelDtl.PartNo);
                        sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                            GetDbMD(), CompanyMD, BranchMD, modelDtl.PartNo);
                        oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();
                    }

                    if (oItems != null)
                    {
                        //Insert Into spMstItems, spMstItemLoc, spMstItemPrice 
                        InsertItemsLocPriceFromMD(oItems.PartNo);
                    }
                    //}
                    //else
                    //{
                    //    oItems = ctx.spMstItems.Find(CompanyCode, BranchCode, modelDtl.PartNo);
                    //    oItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, modelDtl.PartNo, WarehouseMD);
                    //    oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, modelDtl.PartNo);
                    //}

                    recordDtl = new SpTrnSORDDtl();
                    recordDtl.CompanyCode = CompanyCode;
                    recordDtl.BranchCode = BranchCode;
                    recordDtl.DocNo = DocNo;
                    recordDtl.PartNo = modelDtl.PartNo;
                    recordDtl.WarehouseCode = WarehouseMD;
                    recordDtl.PartNoOriginal = modelDtl.PartNo;
                    recordDtl.ReferenceNo = Hdr.OrderNo;
                    recordDtl.ReferenceDate = DocDate;
                    recordDtl.LocationCode = (oItemLoc == null) ? "" : oItemLoc.LocationCode ?? "";
                    recordDtl.QtyOrder = modelDtl.QtyOrder;
                    recordDtl.QtySupply = 0;
                    recordDtl.QtyBO = 0;
                    recordDtl.QtyBOSupply = 0;
                    recordDtl.QtyBOCancel = 0;
                    recordDtl.QtyBill = 0;
                    recordDtl.RetailPriceInclTax = oItemPrice == null ? 0 : oItemPrice.RetailPriceInclTax;
                    recordDtl.RetailPrice = oItemPrice == null ? 0 : oItemPrice.RetailPrice;
                    recordDtl.CostPrice = oItemPrice == null ? 0 : GetCostPrice(modelDtl.PartNo); //oItemPrice.CostPrice;
                    recordDtl.DiscPct = modelDtl.DiscPct == null ? 0 : modelDtl.DiscPct;
                    recordDtl.SalesAmt = modelDtl.RetailPrice;
                    recordDtl.DiscAmt = Math.Round(recordDtl.SalesAmt.Value * (recordDtl.DiscPct.Value / 100), 0, MidpointRounding.AwayFromZero);
                    recordDtl.NetSalesAmt = recordDtl.SalesAmt - recordDtl.DiscAmt;
                    recordDtl.PPNAmt = 0;
                    recordDtl.TotSalesAmt = recordDtl.NetSalesAmt + recordDtl.PPNAmt;
                    recordDtl.MovingCode = oItems.MovingCode;
                    recordDtl.ABCClass = oItems.ABCClass;
                    recordDtl.ProductType = oItems.ProductType;
                    recordDtl.PartCategory = oItems.PartCategory;
                    recordDtl.Status = "0";
                    recordDtl.CreatedBy = CurrentUser.UserId;
                    recordDtl.CreatedDate = DateTime.Now;
                    recordDtl.LastUpdateBy = CurrentUser.UserId;
                    recordDtl.LastUpdateDate = DateTime.Now;
                    recordDtl.FirstDemandQty = modelDtl.QtyOrder;
                    ctx.SpTrnSORDDtls.Add(recordDtl);
                }
                else
                {
                    var oItemPrice = new spMstItemPrice();

                    //oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, modelDtl.PartNo); //ctxMD.spMstItemPrices.Find(CompanyMD, BranchMD, modelDtl.PartNo);
                    var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                        GetDbMD(), CompanyMD, BranchMD, modelDtl.PartNo);
                    oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

                    recordDtl.QtyOrder = modelDtl.QtyOrder;
                    recordDtl.RetailPriceInclTax = oItemPrice == null ? 0 : oItemPrice.RetailPriceInclTax;
                    recordDtl.RetailPrice = GetRetailPrice(modelDtl.PartNo, oItemPrice.RetailPrice.Value); //oItemPrice.RetailPrice; 
                    recordDtl.DiscPct = modelDtl.DiscPct;
                    recordDtl.SalesAmt = recordDtl.QtyOrder * GetRetailPrice(modelDtl.PartNo, oItemPrice.RetailPrice.Value);  //modelDtl.RetailPrice;
                    recordDtl.DiscAmt = Math.Round(recordDtl.SalesAmt.Value * (modelDtl.DiscPct.Value / 100), 0, MidpointRounding.AwayFromZero);
                    recordDtl.NetSalesAmt = recordDtl.SalesAmt - recordDtl.DiscAmt;
                    recordDtl.PPNAmt = 0;
                    recordDtl.TotSalesAmt = recordDtl.NetSalesAmt + recordDtl.PPNAmt;
                    recordDtl.LastUpdateBy = CurrentUser.UserId;
                    recordDtl.LastUpdateDate = DateTime.Now;
                }

                ctx.SaveChanges();

                isOverdueOrderExist(CustomerCode, oCustProfitCenter);

                if (string.IsNullOrEmpty(recordDtl.LocationCode))
                { return Json(new { success = false, message = "Proses simpan detail gagal. \n Data tidak dapat disimpan karena item lokasi belum disetting" }); }

                if (modelDtl.RetailPrice == 0 || recordDtl.CostPrice == 0)
                { return Json(new { success = false, message = "Proses simpan detail gagal. \n Data tidak dapat disimpan karena item price belum disetting" }); }

                decimal retailPrice = Math.Round(recordDtl.RetailPrice.Value * (1 - (recordDtl.DiscPct.Value / 100)), 0, MidpointRounding.AwayFromZero);
                decimal ppnAmt = Math.Truncate(retailPrice * (GetPPNPct(recordDtl.DocNo) / 100));

                // Validasi dipindahkan ke method ValidateHppValue
                // By Rudiana

                //if (retailPrice + ppnAmt < recordDtl.CostPrice)
                //{
                //    //if (!IsConfirmHpp)
                //    //{
                //        return Json(new { success = false, cekHpp = true, message = "Pastikan bahwa nilai penjualan > HPP ! Lanjutkan proses simpan ?" });
                //    //}
                //}

                try
                {
                    CalculatePart(DocNo, CustomerCode);
                    transPart.Commit();
                    return Json(new { success = true, data = recordDtl, docDate = DocDate });
                }
                catch (Exception ex)
                {
                    transPart.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
    }
}
