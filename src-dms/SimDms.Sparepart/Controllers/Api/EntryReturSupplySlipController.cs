using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using GeLang;
using SimDms.Sparepart.Models;
using System.Transactions;
using System.Data;
using SimDms.Common.Models;
using SimDms.Common;

namespace SimDms.Sparepart.Controllers.Api
{
    public class EntryReturSupplySlipController : BaseController
    {
        [HttpPost]
        public JsonResult GetDetailsBrowse(string ReturnNo)
        {
            var sql = string.Format("exec uspfn_spGetReturnSSDetails '{0}','{1}','{2}'", CompanyCode, BranchCode, ReturnNo);
            var dataDetails = ctx.Database.SqlQuery<SPKNoService>(sql).FirstOrDefault();
            return Json(dataDetails);
        }

        [HttpPost]
        public JsonResult GetDetailsReturnSS(string ReturnNo, string LampiranNo)
        {
            //var sql = string.Format("exec uspfn_spGetReturnSSDtl '{0}','{1}','{2}','{3}'", CompanyCode, BranchCode, ReturnNo, LampiranNo);
            var sql = string.Format("exec uspfn_spGetReturnSSDtl '{0}','{1}','{2}'", CompanyCode, BranchCode, ReturnNo);
            var dataDetails = ctx.Database.SqlQuery<ReturnSSDetails>(sql).AsQueryable();
            return Json(dataDetails);
        }

        [HttpPost]
        public JsonResult GetDetailsLookup(string LmpNo)
        {
            var sql = string.Format("exec uspfn_spGetSPKNoService '{0}','{1}','{2}'", CompanyCode, BranchCode, LmpNo);
            var dataDetails = ctx.Database.SqlQuery<SPKNoService>(sql).FirstOrDefault();
            return Json(dataDetails);
        }

        public JsonResult SaveDtl()
        {
            return Json("");
        }

        public JsonResult DeleteRtrSS(SPKNoService model)
        {
            var ErrorMsg = IsValidStatus(model.ReturnNo);
            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                return Json(new { success = false, message = ErrorMsg });
            }
            try
            {
                SpTrnSRturSSHdr oRSS = ctx.SpTrnSRturSSHdrs.Find(CompanyCode, BranchCode, model.ReturnNo);
                if (oRSS != null)
                {
                    if (oRSS.Status == "2" || oRSS.Status == "3")
                    {
                        return Json(new { success = false, message = "Nomor dokumen ini sudah tidak bisa dihapus." });
                    }
                    
                    if (oRSS.Status == "2")
                    {
                        return Json(new { success = false, message = "Nomor dokumen ini sudah ter-posting !!!" });
                    }
                    oRSS.Status = "3";
                    oRSS.LastUpdateBy = CurrentUser.UserId;
                    oRSS.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                }
                else
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Proses delete data Return Supply Slip Gagal" });
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult Save(SPKNoService model)
        {
            string ErrorMsg = DateTransValidation(model.ReturnDate.Date);
            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                return Json(new { success = false, message = ErrorMsg });
            }

            //if (ctx.CompanyMappings.Count() != 0)
            //{
            //    if (model.LmpDate.ToShortDateString() != model.ReturnDate.ToShortDateString())
            //    {
            //        return Json(new
            //        {
            //            success = false,
            //            message = "Proses retur supply slip tidak dapat dilakukan karena tanggal Lampiran dan tanggal SSR tidak sama. Harap dibuatkan retur penjualan."
            //        });
            //    }
            //}

            try
            {
                var oCustomer = ctx.GnMstCustomers.Find(CompanyCode, model.CustomerCode);
                var oLmpHdr = ctx.SpTrnSLmpHdrs.Find(CompanyCode, BranchCode, model.LmpNo);

                //check existing record of SpTrnSRturSSHdr
                SpTrnSRturSSHdr oRSS = ctx.SpTrnSRturSSHdrs.Find(CompanyCode, BranchCode, model.ReturnNo);
                if (oRSS == null)
                {
                    oRSS = new SpTrnSRturSSHdr();
                    oRSS.CompanyCode = CompanyCode;
                    oRSS.BranchCode = BranchCode;
                    oRSS.ReturnNo = model.ReturnNo = GetNewDocumentNo("STR", model.ReturnDate);
                    oRSS.CreatedBy = CurrentUser.UserId;
                    oRSS.CreatedDate = DateTime.Now;
                    ctx.SpTrnSRturSSHdrs.Add(oRSS);
                }
                oRSS.ReturnDate = model.ReturnDate;
                oRSS.CustomerCode = model.CustomerCode;
                oRSS.DocNo = model.LmpNo;
                oRSS.DocDate = model.LmpDate;
                oRSS.ReferenceNo = model.ReferenceNo;
                oRSS.ReferenceDate = model.ReferenceDate;
                oRSS.isPKP = oLmpHdr.isPKP;
                oRSS.NPWPNo = oCustomer.NPWPNo;
                oRSS.SalesType = oLmpHdr.TransType.Substring(0, 1);
                oRSS.TransType = oLmpHdr.TransType;
                oRSS.Status = "0";
                oRSS.TypeOfGoods = CurrentUser.TypeOfGoods;
                oRSS.SPKDate = model.JobOrderDate;
                oRSS.SKPNo = model.JobOrderNo;
                oRSS.LastUpdateBy = CurrentUser.UserId;
                oRSS.LastUpdateDate = DateTime.Now;
                ctx.SaveChanges();
            }
            
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error Ketika Save ke Table SpTrnSRturSSHdr, Message=" + ex.ToString() });
            }
            return Json(new { success = true, message = "", returnNo = model.ReturnNo });
        }

        public JsonResult getCurrentDate()
        {
            return Json(new { success = true, message="", cDate = ctx.CurrentTime });
        }


        public JsonResult SaveDetailsSS(ReturnSSDetails model)
        {
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    string ErrorMsg = IsValidStatus(model.ReturnNo);
                    if (!string.IsNullOrEmpty(ErrorMsg))
                    {
                        return Json(new { success = false, message = ErrorMsg });
                    }

                    ///max qty retur
                    string sql = string.Format("exec uspfn_spGetMaxQtyReturSupplySlip '{0}','{1}','{2}','{3}','{4}','{5}','{6}'", CompanyCode, BranchCode, model.PartNo, model.PartNoOriginal, model.DocNo, model.LmpNo, model.ReturnNo);
                    var MaxQtyRtr = ctx.Database.SqlQuery<decimal>(sql).FirstOrDefault();

                    if (model.QtyBill > MaxQtyRtr)
                    {
                        return Json(new { success = false, message = ctx.SysMsgs.Find("5018").MessageCaption + ", Maximum Quantity Return : " + MaxQtyRtr });
                    }

                    var oLmpDtl = ctx.SpTrnSLmpDtls.Find(CompanyCode, BranchCode, model.LmpNo, "00", model.PartNo, model.PartNoOriginal, model.DocNo);

                    if (oLmpDtl == null)
                    {
                        return Json(new { success = false, message = ctx.SysMsgs.Find("5039").MessageCaption + ", Pastikan data tersebut tersimpan di tabel Lampiran Detail" });
                    }

                    var oRtrHdr = ctx.SpTrnSRturSSHdrs.Find(CompanyCode, BranchCode, model.ReturnNo);
                    if (oRtrHdr != null)
                    {
                        if (oRtrHdr.SalesType == "3" && CurrentUser.CoProfile.IsLinkToSales.Value)
                        {
                            if (model.QtyLmp != model.QtyBill)
                            {
                                return Json(new { success = false, message = "Quantity return harus sama dengan quantity lampiran !" });
                            }
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Data Return Supply Slip tidak ditemukan." });
                    }

                    var oRtrSSDtl = ctx.SpTrnSRTurSSDtls.Find(CompanyCode, BranchCode, model.ReturnNo, model.PartNo, model.PartNoOriginal, model.WarehouseCode, model.DocNo);
                    if (oRtrSSDtl == null)
                    {
                        oRtrSSDtl = new SpTrnSRTurSSDtl();
                        oRtrSSDtl.CompanyCode = CompanyCode;
                        oRtrSSDtl.BranchCode = BranchCode;
                        oRtrSSDtl.ReturnNo = model.ReturnNo;
                        oRtrSSDtl.PartNo = model.PartNo;
                        oRtrSSDtl.PartNoOriginal = model.PartNoOriginal;
                        oRtrSSDtl.WareHouseCode = string.IsNullOrEmpty(model.WarehouseCode) ? "00" : model.WarehouseCode;
                        oRtrSSDtl.DocNo = model.DocNo;
                        ctx.SpTrnSRTurSSDtls.Add(oRtrSSDtl);
                    }
                    oRtrSSDtl.ReturnDate = model.LmpDate;
                    oRtrSSDtl.QtyReturn = model.QtyBill;
                    oRtrSSDtl.RetailPriceInclTax = oLmpDtl.RetailPriceInclTax;
                    oRtrSSDtl.RetailPrice = oLmpDtl.RetailPrice;
                    oRtrSSDtl.CostPrice = oLmpDtl.CostPrice;
                    oRtrSSDtl.DiscPct = oLmpDtl.DiscPct;
                    oRtrSSDtl.ReturAmt = oRtrSSDtl.QtyReturn * oRtrSSDtl.RetailPrice;
                    oRtrSSDtl.DiscAmt = oRtrSSDtl.DiscPct * oRtrSSDtl.ReturAmt / 100;
                    oRtrSSDtl.NetReturAmt = oRtrSSDtl.ReturAmt - oRtrSSDtl.DiscAmt;
                    oRtrSSDtl.PPNAmt = 0;
                    oRtrSSDtl.TotReturAmt = oRtrSSDtl.NetReturAmt + oRtrSSDtl.PPNAmt;
                    oRtrSSDtl.CostAmt = oRtrSSDtl.QtyReturn * oRtrSSDtl.CostPrice;
                    oRtrSSDtl.LocationCode = oLmpDtl.LocationCode;
                    oRtrSSDtl.ProductType = oLmpDtl.ProductType;
                    oRtrSSDtl.PartCategory = oLmpDtl.PartCategory;
                    oRtrSSDtl.MovingCode = oLmpDtl.MovingCode;
                    oRtrSSDtl.ABCClass = oLmpDtl.ABCClass;

                    Helpers.ReplaceNullable(oRtrSSDtl);

                    ctx.SaveChanges();
                    oRtrHdr = ctx.SpTrnSRturSSHdrs.Find(CompanyCode, BranchCode, model.ReturnNo);

                    var SqlReturSum = string.Format("exec uspfn_spGetRturSumAmtSupplySlip '{0}','{1}','{2}'", CompanyCode, BranchCode, model.ReturnNo);
                    var ReturnSumAmt = ctx.Database.SqlQuery<ReturnSumAmtSS>(SqlReturSum).FirstOrDefault();
                    //var UserPc = ctx.SysUserProfitCenters.Find(CurrentUser.UserId);
                    if (ReturnSumAmt != null)
                    {
                        oRtrHdr.TotReturQty = ReturnSumAmt.TotReturQty;
                        oRtrHdr.TotReturAmt = ReturnSumAmt.TotReturAmt;
                        oRtrHdr.TotDiscAmt = ReturnSumAmt.TotDiscAmt;
                        oRtrHdr.TotDPPAmt = ReturnSumAmt.TotDPPAmt;
                        oRtrHdr.TotCostAmt = ReturnSumAmt.TotCostAmt;
                       if(ProfitCenter != "300"){ // if (UserPc.ProfitCenter != "300"){
                            trans.Rollback();
                            return Json(new { success = false, message = "User Login Belum di Setup Profit Center Sparepart!" });
                        }
                        var CustPC = ctx.ProfitCenters.Find(CompanyCode, BranchCode, oRtrHdr.CustomerCode, ProfitCenter);
                        var oTax = string.IsNullOrEmpty(CustPC.TaxCode) ? null : ctx.Taxes.Find(CompanyCode, CustPC.TaxCode);

                        oRtrHdr.TotPPNAmt = (oTax != null) ? Decimal.Parse(Math.Truncate(Double.Parse((oRtrHdr.TotDPPAmt * oTax.TaxPct / 100).ToString())).ToString()) : 0;
                        oRtrHdr.TotFinalReturAmt = oRtrHdr.TotDPPAmt + oRtrHdr.TotPPNAmt;
                    }
                    else
                    {
                        oRtrHdr.TotReturQty = 0;
                        oRtrHdr.TotReturAmt = 0;
                        oRtrHdr.TotDiscAmt = 0;
                        oRtrHdr.TotDPPAmt = 0;
                        oRtrHdr.TotCostAmt = 0;
                        oRtrHdr.TotPPNAmt = 0;
                        oRtrHdr.TotFinalReturAmt = 0;
                        oRtrHdr.TotCostAmt = 0;
                    }

                    oRtrHdr.Status = "0";
                    oRtrHdr.LastUpdateBy = CurrentUser.UserId;
                    oRtrHdr.LastUpdateDate = DateTime.Now;

                    Helpers.ReplaceNullable(oRtrHdr);
                    ctx.SaveChanges();

                    var isLinkToService = (CurrentUser.CoProfile.IsLinkToService == null ? false : CurrentUser.CoProfile.IsLinkToService.Value);
                    if (oRtrHdr.SalesType == "2" && isLinkToService)
                    {
                        //UpdateSSReturnNo
                        var sqlUpdateSSR = string.Format("exec Uspfn_SrvItemUpdateSSReturnNo '{0}','{1}','{2}','{3}','{4}','{5}','{6}'", CompanyCode, BranchCode, ProductType, model.ReturnNo, model.PartNo, true, CurrentUser.UserId);
                        ctx.Database.ExecuteSqlCommand(sqlUpdateSSR);
                    }

                    var isLinkToSales = (CurrentUser.CoProfile.IsLinkToSales == null ? false : CurrentUser.CoProfile.IsLinkToSales.Value);
                    if (oRtrHdr.SalesType == "3" && isLinkToSales)
                    {
                        //UpdateSSReturnNo
                        var sqlUpdateSSR = string.Format("exec Uspfn_spSOAccsUpdateSSReturnNo '{0}','{1}','{2}','{3}','{4}','{5}','{6}'", CompanyCode, BranchCode, ProductType, model.ReturnNo, model.PartNo, true, CurrentUser.UserId);
                        ctx.Database.ExecuteSqlCommand(sqlUpdateSSR);
                    }

                    trans.Commit();
                    
                    return Json(new { success = true, message = "" });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    var exMsg = ex.InnerException != null ? ex.Message + ", " + ex.InnerException.Message : ex.Message;
 
                    return Json(new { success = false, message = "Error di function SaveDetailsSS", error_log = exMsg });
                }
            }
        }

        private string IsValidStatus(string ReturnNo)
        {
            var returnMsg = "";
            var oRtrSSHdr = ctx.SpTrnSRturSSHdrs.Find(CompanyCode, BranchCode, ReturnNo);
            if (oRtrSSHdr != null && int.Parse(oRtrSSHdr.Status) > 1)
            {
                returnMsg = "Nomor dokumen ini sudah ter-posting !!!";
            }
            return returnMsg;
        }

        public JsonResult DeleteDetailsSS(ReturnSSDetails model)
        {
            var result = false;
            var msg = "";
            var oRTRSS = ctx.SpTrnSRTurSSDtls.Find(CompanyCode, BranchCode, model.ReturnNo, model.PartNo, model.PartNoOriginal, model.WarehouseCode, model.DocNo);
            if (oRTRSS != null)
            {
                using (var tran = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var recHdr = ctx.SpTrnSRturSSHdrs.Find(CompanyCode, BranchCode, model.ReturnNo);
                        if (recHdr != null)
                        {
                            // Update SvTrnSrvItem.SSReturnNo
                            var isLinkToService = (CurrentUser.CoProfile.IsLinkToService == null ? false : CurrentUser.CoProfile.IsLinkToService.Value);
                            if (recHdr.SalesType == "2" && isLinkToService)
                            {
                                var sqlUpdateSSR = string.Format("exec Uspfn_SrvItemUpdateSSReturnNo '{0}','{1}','{2}','{3}','{4}','{5}','{6}'", CompanyCode, BranchCode, ProductType, model.ReturnNo, model.PartNo, false, CurrentUser.UserId);
                                result = ctx.Database.ExecuteSqlCommand(sqlUpdateSSR) > 0;
                            }

                            // Update OmTrSalesSOAccs.SSReturnNo
                            var isLinkToSales = (CurrentUser.CoProfile.IsLinkToSales == null ? false : CurrentUser.CoProfile.IsLinkToSales.Value);
                            if (recHdr.SalesType == "3" && isLinkToSales)
                            {
                                var sqlUpdateSSR = string.Format("exec Uspfn_spSOAccsUpdateSSReturnNo '{0}','{1}','{2}','{3}','{4}','{5}','{6}'", CompanyCode, BranchCode, ProductType, model.ReturnNo, model.PartNo, false, CurrentUser.UserId);
                                result = ctx.Database.ExecuteSqlCommand(sqlUpdateSSR) > 0;
                            }

                            if (result && (isLinkToSales || isLinkToService))
                            {
                                ctx.SpTrnSRTurSSDtls.Remove(oRTRSS);
                                result = ctx.SaveChanges() > 0;
                            }

                            if (!isLinkToService || !isLinkToSales)
                            {
                                ctx.SpTrnSRTurSSDtls.Remove(oRTRSS);
                                result = ctx.SaveChanges() > 0;
                            }

                            if (result)
                            {
                                var SqlReturSum = string.Format("exec uspfn_spGetRturSumAmtSupplySlip '{0}','{1}','{2}'", CompanyCode, BranchCode, model.ReturnNo);
                                var ReturnSumAmt = ctx.Database.SqlQuery<ReturnSumAmtSS>(SqlReturSum).FirstOrDefault();

                                if (ReturnSumAmt != null)
                                {
                                    recHdr.TotReturQty = ReturnSumAmt.TotReturQty;
                                    recHdr.TotReturAmt = ReturnSumAmt.TotReturAmt;
                                    recHdr.TotDiscAmt = ReturnSumAmt.TotDiscAmt;
                                    recHdr.TotDPPAmt = ReturnSumAmt.TotDPPAmt;
                                    recHdr.TotPPNAmt = ReturnSumAmt.TotPPNAmt;
                                    recHdr.TotFinalReturAmt = ReturnSumAmt.TotFinalReturAmt;
                                    recHdr.TotCostAmt = ReturnSumAmt.TotCostAmt;
                                }
                                else
                                {
                                    recHdr.TotReturQty = 0;
                                    recHdr.TotReturAmt = 0;
                                    recHdr.TotDiscAmt = 0;
                                    recHdr.TotDPPAmt = 0;
                                    recHdr.TotPPNAmt = 0;
                                    recHdr.TotFinalReturAmt = 0;
                                    recHdr.TotCostAmt = 0;
                                }

                                recHdr.Status = "0";
                                recHdr.LastUpdateBy = CurrentUser.UserId;
                                recHdr.LastUpdateDate = DateTime.Now;
                                result = ctx.SaveChanges() > 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json(new { success = false, message = "" });
                    }
                    finally {
                        if (result)
                        {
                            tran.Commit();
                        }
                        else
                        {
                            tran.Rollback();
                        }
                    }

                    return Json(new { success = result, message = result ? "" : "Proses Hapus data detail gagal." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Proses Hapus data detail gagal. Data Return Supply Slip Detail tidak ditemukan." });
            }
        }

        [HttpPost]
        public JsonResult UpdateStatus(string ReturnNo)
        {
            try
            {
                var sql = string.Format("Update spTrnSRturSSHdr Set Status=0 where CompanyCode='{0}' and BranchCode='{1}' and ReturnNo='{2}'", CompanyCode, BranchCode, ReturnNo);
                ctx.Database.ExecuteSqlCommand(sql);
                return Json(new { success = true, message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error pada function UpdateStatus, Message=" + ex.ToString() });
            }
        }

        [HttpPost]
        public JsonResult Print(ReturnSSDetails model)
        {
            try
            {
                var errorMsg = IsValidStatus(model.ReturnNo);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    return Json(new { success = false, message = errorMsg });
                }
                var oRtrSS = ctx.SpTrnSRturSSHdrs.Find(CompanyCode, BranchCode, model.ReturnNo);
                if (oRtrSS != null)
                {
                    oRtrSS.PrintSeq += 1;
                    oRtrSS.Status = "1";
                    oRtrSS.LastUpdateDate = DateTime.Now;
                    oRtrSS.LastUpdateBy = CurrentUser.UserId;
                    ctx.SaveChanges();
                }
                return Json(new { success = true, message = errorMsg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error pada function Print, Message=" + ex.ToString() });
            }
        }

        [HttpPost]
        public JsonResult PostRtrSS(SPKNoService model)
        {
            string errorMsg = "";
            try
            {
                errorMsg = IsValidStatus(model.ReturnNo);
                if (!string.IsNullOrWhiteSpace(errorMsg))
                {
                    return Json(new { success = false, message = errorMsg });
                }

                errorMsg = DateTransValidation(model.ReturnDate.Date);
                if (!string.IsNullOrWhiteSpace(errorMsg))
                {
                    return Json(new { success = false, message = errorMsg });
                }

                errorMsg = PostingReturnSupplySlip(model.ReturnNo, model.LmpNo);
                if (!string.IsNullOrWhiteSpace(errorMsg))
                {
                    return Json(new { success = false, message = errorMsg });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = errorMsg + ". Exception message : " + ex.Message });
            }

            return Json(new { success = true, message = errorMsg });
        }

        public string PostingReturnSupplySlip(string ReturnNo, string LmpNo)
        {
            var oRtrHdr = ctx.SpTrnSRturSSHdrs.Find(CompanyCode, BranchCode, ReturnNo);
            //get SpTrnSRturSSHdr
            string errorMsg = "";
            if (oRtrHdr.Status == "1")
            {
                using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // Update tabel spTrnSRturSSHdr
                        oRtrHdr.Status = "2";
                        oRtrHdr.LastUpdateBy = CurrentUser.UserId;
                        oRtrHdr.LastUpdateDate = DateTime.Now;
                        ctx.SaveChanges();

                        List<SpTrnSRTurSSDtl> ListRturDtl = ctx.SpTrnSRTurSSDtls.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.ReturnNo == oRtrHdr.ReturnNo).ToList();
                        List<RturItem> ListRturItem = new List<RturItem>();
                        SpTrnSRturSSHdr recordSSHdr = ctx.SpTrnSRturSSHdrs.Find(CompanyCode, BranchCode, ReturnNo);
                        RturItem RturItem = null;
                        foreach (var RturDtl in ListRturDtl)
                        {
                            RturItem = new RturItem();
                            RturItem.PartNo = RturDtl.PartNo;
                            RturItem.QtyReturn = RturDtl.QtyReturn.Value;
                            RturItem.CostPrice = RturDtl.CostPrice.Value;
                            ListRturItem.Add(RturItem);
                        }

                        // Update tabel spMstItemPrice 
                        UpdateItemPriceAvgCost(ReturnNo, "RSSLIP", ListRturItem);

                        foreach (var RturDtl in ListRturDtl)
                        {
                            //Update Stock
                            //following the instruction 
                            //FOR SD : Return Quantity doesn't add to Stock On Hand Quantity
                            //Stock On Hand Quantity & Allocation Quantity will add automatically after MD make SD Invoice
                            UpdateStock(RturDtl.PartNo, RturDtl.WareHouseCode, (RturDtl.QtyReturn == null ? 0 : RturDtl.QtyReturn.Value), 0, 0, 0, string.Empty);

                            MovementLog(ReturnNo, oRtrHdr.DocDate.Value, RturDtl.PartNo, RturDtl.WareHouseCode, "IN", "RSSLIP", RturDtl.QtyReturn.Value);
                        }

                        if (CurrentUser.CoProfile.IsLinkToService.Value && (oRtrHdr.TransType.Substring(0, 1) == "2"))
                        {
                            //update return Quantity
                            var sql = string.Format("exec uspfn_spInsertReturnQtyService '{0}','{1}','{2}','{3}','{4}'", CompanyCode, BranchCode, ProductType, oRtrHdr.ReturnNo, CurrentUser.UserId);
                            ctx.Database.ExecuteSqlCommand(sql);
                        }

                        if (CurrentUser.CoProfile.IsLinkToSales.Value && (oRtrHdr.TransType.Substring(0, 1) == "3"))
                        {
                            //update return Quantity
                            var sql = string.Format("exec uspfn_spInsertReturnQtyUnit '{0}','{1}','{2}','{3}','{4}'", CompanyCode, BranchCode, oRtrHdr.DocNo, oRtrHdr.ReturnNo, CurrentUser.UserId);
                            ctx.Database.ExecuteSqlCommand(sql);
                        }

                        if (!CurrentUser.CoProfile.IsLinkToService.Value)
                        {
                            JournalSpReturn(recordSSHdr.ReturnNo, recordSSHdr.ReturnDate.Value, recordSSHdr.DocNo, 0, 0, 0,
                                (recordSSHdr.TotCostAmt == null ? 0 : recordSSHdr.TotCostAmt.Value),
                                TypeOfGoods, recordSSHdr.CustomerCode);
                        }

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        errorMsg = "Error pada function PostingReturnSupplySlip, message=" + ex.ToString(); ;
                    }
                }
            }

            return errorMsg;
        }

        private void GenerateGLInterface(GLInterface oJournalParam, MstCustomerProfitCenter userProfitCenter)
        {
            GLInterface oJournal = new GLInterface();
            oJournal.CompanyCode = CompanyCode;
            oJournal.BranchCode = BranchCode;
            oJournal.ProfitCenterCode = userProfitCenter.ProfitCenterCode;
            oJournal.AccDate = oJournalParam.CreateDate;
            oJournal.BatchNo = string.Empty;
            oJournal.BatchDate = DateTime.Parse("1900/01/01");
            oJournal.StatusFlag = "0";
            oJournal.CreateBy = CurrentUser.UserId;
            oJournal.LastUpdateBy = CurrentUser.UserId;
            oJournal.CreateDate = DateTime.Now;
            oJournal.LastUpdateDate = DateTime.Now;
            ctx.GLInterfaces.Add(oJournal);
            ctx.SaveChanges();
        }

        //private string UpdateStock(SpTrnSRTurSSDtl RturDtl)
        //{
        //    bool md = DealerCode() == "MD";

        //    var returnMsg = "";
        //    spMstItem oItem = md ? ctx.spMstItems.Find(CompanyCode, BranchCode, RturDtl.PartNo) : ctxMD.spMstItems.Find(CompanyCode, BranchCode, RturDtl.PartNo);
        //    if (oItem != null)
        //    {
        //        SpMstItemLoc oItemLoc = md ? ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, RturDtl.PartNo, RturDtl.WareHouseCode) : ctxMD.SpMstItemLocs.Find(CompanyCode, BranchCode, RturDtl.PartNo, RturDtl.WareHouseCode);
        //        if (oItemLoc != null)
        //        {
        //            decimal QtyRturn = RturDtl.QtyReturn.Value;
        //            if (Math.Abs(QtyRturn) > 0)
        //            {
        //                oItemLoc.OnHand += QtyRturn;
        //                oItem.OnHand += QtyRturn;

        //                if (oItemLoc.OnHand < 0)
        //                {
        //                    returnMsg += string.Format("OnHand untuk Part = {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.OnHand);
        //                }

        //                if (oItem.OnHand < 0)
        //                {
        //                    returnMsg += string.Format("OnHand untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.OnHand);
        //                }
        //            }
        //            oItemLoc.LastUpdateBy = CurrentUser.UserId;
        //            oItemLoc.LastUpdateDate = DateTime.Now;
        //            oItem.LastUpdateBy = CurrentUser.UserId;
        //            oItem.LastUpdateDate = DateTime.Now;
        //            ctx.SaveChanges();
        //        }
        //    }
        //    return returnMsg;
        //}

        #region "Move to BaseController using SD MD Configuration"
        //private bool MovementLog(SpTrnSRturSSHdr RturHdr,SpTrnSRTurSSDtl RturDtl)
        //{
        //    bool returnVal = false;
        //    try
        //    {
        //        var oItem = ctx.spMstItems.Find(CompanyCode, BranchCode, RturDtl.PartNo);
        //        var oItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, RturDtl.PartNo, RturDtl.WareHouseCode);
        //        var oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, RturDtl.PartNo, RturDtl.WareHouseCode);
        //        SpTrnIMovement oIMovement = new SpTrnIMovement();

        //        if (oItem != null && oItemLoc != null && oItemPrice != null)
        //        {
        //            oIMovement.CompanyCode = CompanyCode;
        //            oIMovement.BranchCode = BranchCode;
        //            oIMovement.DocNo = RturHdr.DocNo;
        //            oIMovement.DocDate = RturHdr.DocDate.Value;
        //            oIMovement.CreatedDate = DateTime.Now;

        //            oIMovement.WarehouseCode = oItemLoc.WarehouseCode;
        //            oIMovement.LocationCode = oItemLoc.LocationCode;
        //            oIMovement.PartNo = oItemLoc.PartNo;
        //            oIMovement.Signcode = "IN";
        //            oIMovement.SubSignCode = "RSSLIP";
        //            oIMovement.Qty = RturDtl.QtyReturn.Value;
        //            oIMovement.Price = oItemPrice.RetailPrice;
        //            oIMovement.CostPrice = oItemPrice.CostPrice;
        //            oIMovement.ABCClass = oItem.ABCClass;
        //            oIMovement.MovingCode = oItem.MovingCode;
        //            oIMovement.ProductType = oItem.ProductType;
        //            oIMovement.PartCategory = oItem.PartCategory;
        //            oIMovement.CreatedBy = CurrentUser.UserId;

        //            ctx.SpTrnIMovements.Add(oIMovement);
        //        }
        //        ctx.SaveChanges();
        //        returnVal = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        returnVal = false;
        //    }
        //    return returnVal;
        //}
        #endregion
    }
}
