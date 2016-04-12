using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using SimDms.Common;
using SimDms.Common.Models;
using System.Collections;

namespace SimDms.Sparepart.Controllers.Api
{
    public class EntryClaimSupplierController : BaseController
    {
        private const string GUDANG_CLAIMSHORTAGE = "X2";
        private const string GUDANG_CLAIMWRONG = "X3";
        private const string GUDANG_CLAIMDAMAGE = "X1";
        private const string GUDANG_CLAIMDOVER = "X4";

        public string getData()
        {
            var transdate = ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate;
            //return GetNewDocumentNoHpp("CLM", DateTime.Now.ToString("yyyyMMdd"));
            return GetNewDocumentNoHpp("CLM", (Convert.ToDateTime(transdate)).ToString("yyyyMMdd"));
        }

        public SpTrnPRcvHdr getBinning(string WRSNo)
        {
            return ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, WRSNo);
        }

        public JsonResult Save(spTrnPClaimHdr model, bool ClaimDateEnabled)
        {
            if (IsClosedDocNo(model.ClaimNo))
            {
                var msg = "No. dokumen ini telah diubah oleh user lain";
                var data = PopulateRecordClaim(model.ClaimNo);

                return Json(new { 
                    success = false, 
                    message = msg,
                    claimData = new {
                        isi = data[0],
                        WRSDet = data[1],
                        SuppDet = data[2],
                        TransDet = data[3],
                        GridPartNo = data[4],
                        GridPartNoWrong = data[5]
                    }
                });
            }
            var chekTransDate = DateTransValidation(model.ClaimDate.Value);
            if (!string.IsNullOrEmpty(chekTransDate) && ClaimDateEnabled)
            {
                return Json(new
                {
                    success = false,
                    message = chekTransDate
                });
            }


            var record = ctx.spTrnPClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimNo);
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var ReceiveHdr = getBinning(model.WRSNo);
                    if (record == null)
                    {
                        record = new spTrnPClaimHdr
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            ClaimNo = getData(),
                            ClaimDate = model.ClaimDate,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                            TypeOfGoods = model.TypeOfGoods

                        };
                        ctx.spTrnPClaimHdrs.Add(record);
                    }

                    record.BinningDate = ReceiveHdr.BinningDate;
                    record.BinningNo = ReceiveHdr.BinningNo;
                    record.ReferenceDate = model.ReferenceDate;
                    record.ReferenceNo = model.ReferenceNo;
                    record.SupplierCode = model.SupplierCode;
                    record.TransType = model.TransType;
                    record.WRSDate = model.WRSDate;
                    record.WRSNo = model.WRSNo;
                    record.Status = "0";
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;

                    Helpers.ReplaceNullable(record);

                    ctx.SaveChanges();
                    trans.Commit();

                    return Json(new { success = true, clm = record.ClaimNo, status = record.Status });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        public SpTrnPRcvDtl getDetail(string WRSNo, string PartNo, string DocNo)
        {
            return ctx.SpTrnPRcvDtls.Find(CompanyCode, BranchCode, WRSNo, PartNo, DocNo);
        }

        public JsonResult SaveDetail(SpTrnPClaimDtl model, string WRSNo, string pil)
        {
            var record = ctx.SpTrnPClaimDtls.Find(CompanyCode, BranchCode, model.ClaimNo, model.PartNo, model.DocNo);
            var recordRcvDtl = getDetail(WRSNo, model.PartNo, model.DocNo);
            
            if (record == null)
            {
                record = new SpTrnPClaimDtl
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ClaimNo = model.ClaimNo,
                    PartNo = model.PartNo,
                    DocNo = model.DocNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now

                };
                ctx.SpTrnPClaimDtls.Add(record);
            }

            record.DocDate = recordRcvDtl.DocDate;
            record.PurchasePrice = recordRcvDtl.PurchasePrice;
            record.CostPrice = recordRcvDtl.CostPrice;
            record.ABCClass = recordRcvDtl.ABCClass;
            record.MovingCode = recordRcvDtl.MovingCode;
            record.ProductType = recordRcvDtl.ProductType;
            record.PartCategory = recordRcvDtl.PartCategory;
            record.ReceivedQty = recordRcvDtl.ReceivedQty;
            record.Status = "0";
            record.ReasonCode = model.ReasonCode;
            if (pil == "v1")
            { 
                record.ClaimType = "0";
                record.OvertageQty = Convert.ToDecimal(model.OvertageQty);
                record.ShortageQty = Convert.ToDecimal(model.ShortageQty);
                record.DemageQty = Convert.ToDecimal(model.DemageQty);
                record.TotClaimQty = Convert.ToDecimal(model.OvertageQty) +
                                          Convert.ToDecimal(model.ShortageQty) +
                                          Convert.ToDecimal(model.DemageQty);
            }
            else {
                record.ClaimType = "1";
                record.PartNoWrong = model.PartNoWrong;
                record.WrongQty = Convert.ToDecimal(model.WrongQty);
                record.TotClaimQty = Convert.ToDecimal(model.DemageQty);
            }

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                Helpers.ReplaceNullable(record);
                ctx.SaveChanges();
                var datadetail = ctx.SpGridPartNos.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimNo == model.ClaimNo && a.ClaimType == record.ClaimType );
                return Json(new { success = true, data = datadetail });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult deleteData(spTrnPClaimHdr model)
        {
            var record = ctx.spTrnPClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimNo);
            if (record != null) {
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

        public JsonResult delete(SpTrnPClaimDtl model, string pil)
        {
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var ClaimType = "";
                    var record = ctx.SpTrnPClaimDtls.Find(CompanyCode, BranchCode, model.ClaimNo, model.PartNo, model.DocNo);
                    if (record != null)
                    {
                        ctx.SpTrnPClaimDtls.Remove(record);
                    }

                    ctx.SaveChanges();
                    if (pil == "v1")
                    {
                        ClaimType = "0";
                    }
                    else
                    {
                        ClaimType = "1";
                    }

                    var recordHdr = ctx.spTrnPClaimHdrs.Find(CompanyCode, BranchCode, record.ClaimNo);
                    recordHdr.Status = "0";
                    recordHdr.LastUpdateBy = CurrentUser.UserId;
                    recordHdr.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                    
                    trans.Commit();

                    var datadetail = ctx.SpGridPartNos.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimNo == model.ClaimNo && a.ClaimType == ClaimType);

                    return Json(new { success = true, data = datadetail, status = recordHdr.Status });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        public JsonResult Print(spTrnPClaimHdr model)
        {
            var recordHdr = ctx.spTrnPClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimNo);
            if (recordHdr != null)
            {
                recordHdr.Status = "1";
                recordHdr.PrintSeq += 1;
                recordHdr.LastUpdateBy = CurrentUser.UserId;
                recordHdr.LastUpdateDate = DateTime.Now;
            }
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, status = recordHdr.Status});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }


        }

        public JsonResult CreateClaim(spTrnPClaimHdr model, string pil, SpTrnPClaimDtl model2, bool ClaimDateEnabled)
        {
            var pesan = "";
            var datadtl1 = ctx.SpGridPartNos.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimNo == model.ClaimNo && a.ClaimType == "0").ToList();
            if (pil == "v1")
            {
                if (datadtl1.Count() == 0)
                {
                    pesan = "Tidak ada data details";
                    return Json(new { success = false, message = pesan });
                }
            }
            else
            {
                var datadtl2 = ctx.SpGridPartNos.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimNo == model.ClaimNo && a.ClaimType == "1").ToList();
                if (datadtl2.Count() == 0)
                {
                    pesan = "Tidak ada data details";
                    return Json(new { success = false, message = pesan });
                }
            }

            var recordHdr = ctx.spTrnPClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimNo);
            if (recordHdr != null)
            {
                string errMsg = string.Empty;

                // TO DO : Checking for Claim that was claimed or not
                if (recordHdr.Status.Equals("2"))
                {
                    var msg = string.Format("Dokumen No {0} sudah di {1}", recordHdr.ClaimNo, "Closing");

                    return Json(new { success = false, message = msg, status = recordHdr.Status });
                }

                errMsg = DateTransValidation(recordHdr.ClaimDate.Value);
                if (!string.IsNullOrEmpty(errMsg) && ClaimDateEnabled)
                {
                    return Json(new { success = false, message = errMsg });
                }

                List<SpTrnPClaimDtl> lstClaimDtl = SelectClaimDetail(model.ClaimNo);

                if (p_CreateClaim(recordHdr, lstClaimDtl, ref errMsg))
                {
                    recordHdr = ctx.spTrnPClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimNo);

                    return Json(new { success = true, message = "Claim berhasil dibuat", status = recordHdr.Status });
                }
                else
                    return Json(new { success = false, message = "Proses Pembuatan Claim gagal. " + errMsg });

            }
            else
            {
                return Json(new { success = false, message = "Tidak ada Data yang akan dibuat Claim" });
            }
        }

        public JsonResult CreateClaim_ORI(spTrnPClaimHdr model, string pil, SpTrnPClaimDtl model2)
        {
            var pesan = "";
            var pesan_error = "";
            DateTime transDate;
            var datadtl1 = ctx.SpGridPartNos.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimNo == model.ClaimNo && a.ClaimType == "0").ToList();
            var datadtl2 = ctx.SpGridPartNos.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimNo == model.ClaimNo && a.ClaimType == "1").ToList();
            if (pil == "v1")
            {
                if (datadtl1 == null)
                {
                    pesan = "Tidak ada data details";
                    return Json(new { success = false, message = pesan });
                }
            }
            else {
                if (datadtl2 == null)
                {
                    pesan = "Tidak ada data details";
                    return Json(new { success = false, message = pesan });
                }
            }
            var recordHdr = ctx.spTrnPClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimNo);
            var recordDtl = ctx.SpTrnPClaimDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimNo == model.ClaimNo);
            if (recordHdr == null)
            {
                return Json(new { success = false, message = pesan });
            }
            if (recordHdr.Status == "2") {
                pesan = "No Claim = " + recordHdr.ClaimNo + " telah di close";
                return Json(new { success = false, message = pesan });
            }

            if (recordHdr != null) {
                var getCoProfile = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                if (getCoProfile == null)
                {
                    pesan = "Proses Penyimpanan data Warehouse Transfer Headear Gagal \r\n Karena Company Profile belum disetup";
                    return Json(new { success = false, message = pesan });
                }
                else {
                    transDate = getCoProfile.TransDate;
                    //var recWHH = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, recordHdr.ClaimNo);
                    var recWHH = new spTrnIWHTrfHdr();
                   
                    recWHH.CompanyCode = CompanyCode;
                    recWHH.BranchCode = BranchCode;
                    recWHH.WHTrfNo = GetNewDocumentNo(GnMstDocumentConstant.WTR, transDate);
                    recWHH.WHTrfDate = transDate;
                    recWHH.ReferenceNo = recordHdr.ClaimNo;
                    recWHH.ReferenceDate = recordHdr.ClaimDate;
                    recWHH.TypeOfGoods = TypeOfGoods;
                    recWHH.Status = "2";
                    recWHH.CreatedBy = CurrentUser.UserId;
                    recWHH.CreatedDate = DateTime.Now;
                    recWHH.LastUpdateBy = CurrentUser.UserId;
                    recWHH.LastUpdateDate = DateTime.Now;
                        ctx.spTrnIWHTrfHdrs.Add(recWHH);

                    //insert header 
                    ctx.SaveChanges();
                    
                }
            }
            foreach(var trn in (pil == "v1")?datadtl1:datadtl2)
            {
                var recordItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, trn.PartNo, "00");
                var recordItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, trn.PartNo);
                var recordItem = ctx.spMstItems.Find(CompanyCode, BranchCode, trn.PartNo);
                spTrnIWHTrfDtl TrnIWHT = new spTrnIWHTrfDtl();
                if (trn.DemageQty > 0 || trn.ShortageQty > 0 || trn.WrongQty > 0 || trn.OvertageQty > 0)
                {
                    decimal Qty = 0;
                    string typeClaim = trn.ClaimType;


                    //var recItemLocTemp = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, trn.PartNo, "00");
                    /*if (recItemLocTemp != null)
                    {

                        TrnIWHT = new spTrnIWHTrfDtl
                        {
                            ToLocationCode = recItemLocTemp.LocationCode
                        };
                        ctx.spTrnIWHTrfDtls.Add(TrnIWHT);
                    }
                    TrnIWHT.ToLocationCode = "-";*/
                    switch (typeClaim)
                    {
                        case "0":
                            if (trn.ShortageQty > 0)
                            {
                                var recItemLocTemp = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, trn.PartNo, "00");
                                var TrnIWHTA = ctx.spTrnIWHTrfDtls.Find(CompanyCode, BranchCode, "", trn.PartNo, "00", "X2");
                                Qty = Convert.ToDecimal(trn.ShortageQty);
                                
                                if (TrnIWHTA == null)
                                {
                                    TrnIWHTA = new spTrnIWHTrfDtl
                                    {
                                        CompanyCode = CompanyCode,
                                        BranchCode = BranchCode,
                                        WHTrfNo = string.Empty,
                                    };
                                    ctx.spTrnIWHTrfDtls.Add(TrnIWHTA);
                                }
                                TrnIWHTA.PartNo = trn.PartNo;
                                TrnIWHTA.RetailPrice = recordItemPrice.RetailPrice;
                                TrnIWHTA.RetailPriceInclTax = recordItemPrice.RetailPriceInclTax;
                                TrnIWHTA.CostPrice = trn.CostPrice;
                                TrnIWHTA.FromWarehouseCode = "00";
                                TrnIWHTA.FromLocationCode = recordItemLoc.LocationCode;
                                TrnIWHTA.ToLocationCode = (recItemLocTemp != null) ? recItemLocTemp.LocationCode : "-";
                                TrnIWHTA.ReasonCode = trn.ReasonCode;
                                TrnIWHTA.MovingCode = trn.MovingCode;
                                TrnIWHTA.CreatedBy = CurrentUser.UserId;
                                TrnIWHTA.CreatedDate = DateTime.Now;
                                TrnIWHTA.LastUpdateBy = CurrentUser.UserId;
                                TrnIWHTA.LastUpdateDate = DateTime.Now;

                                TrnIWHTA.Qty = Qty; // global

                                TrnIWHTA.ToWarehouseCode = "X2";
                                pesan_error = "Warehouse Transfer Untuk Claim Shortage Gagal";
                            

                            }
                            if (trn.DemageQty > 0)
                            {
                                Qty = Convert.ToDecimal(trn.DemageQty); // damage
                                var recItemLocTemp = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, trn.PartNo, "00");
                                var TrnIWHTA = ctx.spTrnIWHTrfDtls.Find(CompanyCode, BranchCode, "", trn.PartNo, "00", "X1");
                                if (TrnIWHTA == null)
                                {
                                    TrnIWHTA = new spTrnIWHTrfDtl
                                    {
                                        CompanyCode = CompanyCode,
                                        BranchCode = BranchCode,
                                        WHTrfNo = string.Empty,
                                    };
                                    ctx.spTrnIWHTrfDtls.Add(TrnIWHTA);
                                }
                                TrnIWHTA.PartNo = trn.PartNo;
                                TrnIWHTA.RetailPrice = recordItemPrice.RetailPrice;
                                TrnIWHTA.RetailPriceInclTax = recordItemPrice.RetailPriceInclTax;
                                TrnIWHTA.CostPrice = trn.CostPrice;
                                TrnIWHTA.FromWarehouseCode = "00";
                                TrnIWHTA.FromLocationCode = recordItemLoc.LocationCode;
                                TrnIWHTA.ToLocationCode = (recItemLocTemp != null) ? recItemLocTemp.LocationCode : "-";
                                TrnIWHTA.ReasonCode = trn.ReasonCode;
                                TrnIWHTA.MovingCode = trn.MovingCode;
                                TrnIWHTA.CreatedBy = CurrentUser.UserId;
                                TrnIWHTA.CreatedDate = DateTime.Now;
                                TrnIWHTA.LastUpdateBy = CurrentUser.UserId;
                                TrnIWHTA.LastUpdateDate = DateTime.Now;

                                TrnIWHTA.Qty = Qty; // global
                                TrnIWHTA.ToWarehouseCode = "X1";
                                pesan_error = "Warehouse Transfer Untuk Claim Damage Gagal";
                            }
                            if (trn.WrongQty > 0)
                            {
                                Qty = Convert.ToDecimal(trn.WrongQty); // wrong
                                var recItemLocTemp = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, trn.PartNo, "00");
                                var TrnIWHTA = ctx.spTrnIWHTrfDtls.Find(CompanyCode, BranchCode, "", trn.PartNo, "00", "X3");
                                if (TrnIWHTA == null)
                                {
                                    TrnIWHTA = new spTrnIWHTrfDtl
                                    {
                                        CompanyCode = CompanyCode,
                                        BranchCode = BranchCode,
                                        WHTrfNo = string.Empty,
                                    };
                                    ctx.spTrnIWHTrfDtls.Add(TrnIWHTA);
                                }
                                TrnIWHTA.PartNo = trn.PartNo;
                                TrnIWHTA.RetailPrice = recordItemPrice.RetailPrice;
                                TrnIWHTA.RetailPriceInclTax = recordItemPrice.RetailPriceInclTax;
                                TrnIWHTA.CostPrice = trn.CostPrice;
                                TrnIWHTA.FromWarehouseCode = "00";
                                TrnIWHTA.FromLocationCode = recordItemLoc.LocationCode;
                                TrnIWHTA.ToLocationCode = (recItemLocTemp != null) ? recItemLocTemp.LocationCode : "-";
                                TrnIWHTA.ReasonCode = trn.ReasonCode;
                                TrnIWHTA.MovingCode = trn.MovingCode;
                                TrnIWHTA.CreatedBy = CurrentUser.UserId;
                                TrnIWHTA.CreatedDate = DateTime.Now;
                                TrnIWHTA.LastUpdateBy = CurrentUser.UserId;
                                TrnIWHTA.LastUpdateDate = DateTime.Now;

                                TrnIWHTA.Qty = Qty; // global
                                TrnIWHTA.ToWarehouseCode = "X3";
                                pesan_error = "Warehouse Transfer Untuk Claim Wrong Gagal";
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            recordHdr.Status = "2";
            recordHdr.LastUpdateBy = CurrentUser.UserId;
            recordHdr.LastUpdateDate = DateTime.Now;
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = "Claim berhasil dibuat", status = recordHdr.Status });
            }
            catch
            {
                return Json(new { success = false, message = pesan_error });
            }
        }
        
        public JsonResult EntryCSLoad(string ClaimNo)
        {
            var data = PopulateRecordClaim(ClaimNo);
            return Json(new
            {
                isi = data[0],
                WRSDet = data[1],
                SuppDet = data[2],
                TransDet = data[3],
                GridPartNo = data[4],
                GridPartNoWrong = data[5]
            });
        }

        public JsonResult WRSNoDetail(string WRSNo)
        {
            var record = ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, WRSNo);
            return Json(new
            {
                WRSDet = record,
                SuppDet = _SupplierDetail(record.SupplierCode),
                TransDet = _TransDetail(record.TransType)
            });
            //return Json(record);
        }

        public SpTrnPRcvHdr _WRSNoDetail(string WRSNo)
        {
            return ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, WRSNo);
        }

        public gnMstSupplierView _SupplierDetail(string SupplierCode)
        {
            return ctx.gnMstSupplierViews.Find(CompanyCode, SupplierCode);
        }

        public LookUpDtl _TransDetail(string LookUpValue)
        {
            var CodeID = "TTWR";
            return ctx.LookUpDtls.Find(CompanyCode, CodeID, LookUpValue);
        }

        private bool IsClosedDocNo(string ClaimNo)
        {
            var recordHdr = ctx.spTrnPClaimHdrs.Find(CompanyCode, BranchCode, ClaimNo);
            if (recordHdr == null) return false;
            if (int.Parse(recordHdr.Status) > 1)
            {
                return true;
            }
            return false;
        }

        private ArrayList PopulateRecordClaim(string ClaimNo)
        {
            var arrData = new ArrayList();

            var record = ctx.spTrnPClaimHdrs.Find(CompanyCode, BranchCode, ClaimNo);
            var datadtl = ctx.SpGridPartNos.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimNo == ClaimNo && a.ClaimType == "0");
            var datadtl2 = ctx.SpGridPartNos.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimNo == ClaimNo && a.ClaimType == "1");
            
            arrData.Add(record);
            arrData.Add(_WRSNoDetail(record.WRSNo));
            arrData.Add(_SupplierDetail(record.SupplierCode));
            arrData.Add(_TransDetail(record.TransType));
            arrData.Add(datadtl);
            arrData.Add(datadtl2);

            return arrData;
            
        }

        private List<SpTrnPClaimDtl> SelectClaimDetail(string claimNo)
        {
            List<SpTrnPClaimDtl> lstDetails = new List<SpTrnPClaimDtl>();
            var sql = string.Format(@"SELECT CompanyCode, BranchCode, ClaimNo, PartNo, ISNULL(SUM(OvertageQty),0) AS OvertageQty,
                                ISNULL(SUM(ShortageQty),0) AS ShortageQty, ISNULL(SUM(DemageQty),0) AS DemageQty, 
                                ISNULL(SUM(WrongQty),0) AS WrongQty, MovingCode, CostPrice
                                FROM spTrnPClaimDtl 
                                WHERE 
                                CompanyCode = '{0}'
                                AND BranchCode = '{1}'
                                AND ClaimNo = '{2}'
                                GROUP BY PartNo, CompanyCode, BranchCode, ClaimNo, MovingCode, CostPrice",
                                CompanyCode, BranchCode, claimNo);

            var dtl = ctx.Database.SqlQuery<SpTrnPClaimDtlViewModel>(sql).ToList();
            dtl.ForEach(x =>
            {
                var recordDetails = new SpTrnPClaimDtl();
                recordDetails.CompanyCode = x.CompanyCode;
                recordDetails.BranchCode = x.BranchCode;
                recordDetails.ClaimNo = x.ClaimNo;
                recordDetails.PartNo = x.PartNo;
                var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                            GetDbMD(), CompanyMD, BranchMD, recordDetails.PartNo);
                spMstItemPrice itemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

                if (itemPrice != null)
                    recordDetails.CostPrice = itemPrice.CostPrice;
                else
                    recordDetails.CostPrice = x.CostPrice;
                
                if (itemPrice != null)
                    recordDetails.CostPrice = itemPrice.CostPrice;
                else
                    recordDetails.CostPrice = x.CostPrice ?? 0;

                recordDetails.MovingCode = x.MovingCode;
                recordDetails.OvertageQty = x.OvertageQty ?? 0;
                recordDetails.ShortageQty = x.ShortageQty ?? 0;
                recordDetails.DemageQty = x.DemageQty ?? 0;
                recordDetails.WrongQty = x.WrongQty ?? 0;
                recordDetails.ClaimType = "0";

                lstDetails.Add(recordDetails);

            });
            
            return lstDetails;
        }

        private bool p_CreateClaim(spTrnPClaimHdr recordHdr, List<SpTrnPClaimDtl> dtDetail, ref string errorMsg)
        {
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                bool result = false;
                try
                {
                    // [Add by Beny on Dec-04-2008]
                    spTrnPClaimHdr recordClaim = ctx.spTrnPClaimHdrs.Find(CompanyCode, recordHdr.BranchCode, recordHdr.ClaimNo);

                    if (recordClaim == null)
                        return result = false;

                    if (recordClaim.Status == "2")
                    {
                        throw new Exception("Proses create claim gagal. No. Claim = " + recordHdr.ClaimNo + " telah di close");
                    }
                    // [End Add by Beny on Dec-04-2008]

                    result = p_WarehouseTransferForClaim(recordHdr, dtDetail, "00");
                }
                catch (Exception ex)
                {
                    result = false;
                    errorMsg = ex.Message;
                }
                finally
                {
                    if (result)
                        trans.Commit();
                    else
                    {
                        trans.Rollback();
                    }
                }

                return result;
            }
        }

        public bool p_WarehouseTransferForClaim(spTrnPClaimHdr recordHdr,
            List<SpTrnPClaimDtl> lstSTDetail, string warehouseCode)
        {
            if (lstSTDetail.Count == 0)
                return false;

            //var oSpTrnPRcvDtl = new SpTrnPRcvDtl();
            //var oSpTrnPClaimHdr = new spTrnPClaimHdr();
            //var oSpMstItems = new spMstItem();
            //var oSpMstItemLoc = new SpMstItemLoc();
            //var oSpMstItemPrice = new spMstItemPrice();


            spTrnIWHTrfHdr recWHHeader = null;

            // TO DO : This line code below will insert the record details in 
            //         list object to the warehouse transfer tables

            if (recordHdr != null)
            {
                GnMstCoProfileSpare getCoProfile = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                if (getCoProfile == null)
                {
                    throw new Exception ("Proses Penyimpanan data Warehouse Transfer Headear Gagal \r\n" + "Karena Company Profile belum disetup");
                }
                                
                recWHHeader = new spTrnIWHTrfHdr();
                recWHHeader.CompanyCode = CompanyCode;
                recWHHeader.BranchCode = BranchCode;
                recWHHeader.WHTrfNo = null; //string.Empty;
                recWHHeader.WHTrfDate = getCoProfile.TransDate;
                recWHHeader.ReferenceNo = recordHdr.ClaimNo;
                recWHHeader.ReferenceDate = recordHdr.ClaimDate;
                recWHHeader.TypeOfGoods = TypeOfGoods;
                recWHHeader.Status = "2";
                recWHHeader.CreatedBy = CurrentUser.UserId;
                recWHHeader.CreatedDate = DateTime.Now;
                recWHHeader.LastUpdateBy = CurrentUser.UserId;
                recWHHeader.LastUpdateDate = DateTime.Now;
                if (!p_SaveSpTrnIWHTrfHdr(recWHHeader))
                {
                    throw new Exception("Proses Penyimpanan data Warehouse Transfer Headear Gagal");
                }
            }

            foreach (SpTrnPClaimDtl recordDtl in lstSTDetail)
            {
                //var recordItemLoc = ctx.SpMstItemLocs.Find(recordDtl.CompanyCode, recordDtl.BranchCode, recordDtl.PartNo, warehouseCode);
                //var recordItemPrice = ctx.spHstItemPrices.Find(recordDtl.CompanyCode, recordDtl.BranchCode, recordDtl.PartNo);
                //var recordItem = ctx.spMstItems.Find(recordDtl.CompanyCode, recordDtl.BranchCode, recordDtl.PartNo);

                var sqlItemLoc = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                    GetDbMD(), CompanyMD, BranchMD, recordDtl.PartNo, warehouseCode);
                SpMstItemLoc recordItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

                var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                            GetDbMD(), CompanyMD, BranchMD, recordDtl.PartNo);
                spMstItemPrice recordItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

                var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                    GetDbMD(), CompanyMD, BranchMD, recordDtl.PartNo);
                spMstItem recordItem = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();


                // NOTES : check the quantity of damage, shortage and wrong
                if (recordDtl.DemageQty > 0 || recordDtl.ShortageQty > 0 || recordDtl.WrongQty > 0 || recordDtl.OvertageQty > 0)
                {
                    decimal Qty = 0;
                    string typeClaim = recordDtl.ClaimType;

                    // NOTES : based on the condition of either shortageQty or DamageQty or WrongQty,
                    //         declare the warehouseTo destination for transfer
                    switch (typeClaim)
                    {
                        case "0":
                            if (recordDtl.ShortageQty > 0)
                            {
                                Qty = recordDtl.ShortageQty ?? 0; // short
                                if (!p_SetWTrfDtlForClaim(recWHHeader, recordDtl, recordItemLoc, recordItemPrice, warehouseCode, Qty, GUDANG_CLAIMSHORTAGE))
                                {
                                    throw new Exception("Warehouse Transfer Untuk Claim Shortage Gagal");
                                }

                            }
                            if (recordDtl.DemageQty > 0)
                            {
                                Qty = recordDtl.DemageQty ?? 0; // damage
                                if (!p_SetWTrfDtlForClaim(recWHHeader, recordDtl, recordItemLoc, recordItemPrice, warehouseCode, Qty, GUDANG_CLAIMDAMAGE))
                                {
                                    throw new Exception("Warehouse Transfer Untuk Claim Damage Gagal");
                                }
                            }
                            if (recordDtl.WrongQty > 0)
                            {
                                Qty = recordDtl.WrongQty ?? 0; // wrong
                                if (!p_SetWTrfDtlForClaim(recWHHeader, recordDtl, recordItemLoc, recordItemPrice, warehouseCode, Qty, GUDANG_CLAIMWRONG))
                                {
                                    throw new Exception("Warehouse Transfer Untuk Claim Wrong Gagal");
                                }
                            }
                            if (recordDtl.OvertageQty > 0)
                            {
                                Qty = recordDtl.OvertageQty ?? 0; // over
                                if (!p_SetWTrfDtlForClaim(recWHHeader, recordDtl, recordItemLoc, recordItemPrice, warehouseCode, Qty, GUDANG_CLAIMDOVER))
                                {
                                    throw new Exception("Warehouse Transfer Untuk Claim Overtage Gagal");
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    var spTrnIWHTrfDtl = ctx.spTrnIWHTrfDtls.Where(x => x.CompanyCode == recordDtl.CompanyCode && x.BranchCode == recordDtl.BranchCode && x.WHTrfNo == recWHHeader.WHTrfNo && x.PartNo == recordDtl.PartNo).ToList();
                    string epesan = "";
                    if (!p_UpdateQuantitySpTrnIWHTrfDtl(spTrnIWHTrfDtl, ref epesan))
                    {
                        throw new Exception(epesan);
                    }
                }
            }
            recordHdr.Status = "2";
            recordHdr.LastUpdateBy = CurrentUser.UserId;
            recordHdr.LastUpdateDate = DateTime.Now;

            if (ctx.SaveChanges() < 0)
            {
                throw new Exception("Update Claim Header Gagal");
            }

            return true;
        }

        private bool p_SaveSpTrnIWHTrfHdr(spTrnIWHTrfHdr oRecord)
        {
            // check mandatory
            if (oRecord == null)
                return false;

            int result = 0;
            spTrnIWHTrfHdr oTempRecord = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, oRecord.WHTrfNo);
            if (oTempRecord == null)
            {
                oRecord.WHTrfNo = GetNewDocumentNo(GnMstDocumentConstant.WTR, oRecord.WHTrfDate.Value);
                oRecord.CreatedBy = CurrentUser.UserId;
                oRecord.CreatedDate = DateTime.Now;
                oRecord.LastUpdateBy = CurrentUser.UserId;
                oRecord.LastUpdateDate = DateTime.Now;
                
                Helpers.ReplaceNullable(oRecord);
                
                ctx.spTrnIWHTrfHdrs.Add(oRecord);
                result = ctx.SaveChanges();
            }
            else
            {
                oTempRecord.WHTrfNo = oRecord.WHTrfNo;
                oTempRecord.WHTrfDate = oRecord.WHTrfDate;
                oTempRecord.ReferenceNo = oRecord.ReferenceNo;
                oTempRecord.ReferenceDate = oRecord.ReferenceDate;
                oTempRecord.LastUpdateBy = CurrentUser.UserId;
                oTempRecord.LastUpdateDate = DateTime.Now;

                Helpers.ReplaceNullable(oRecord);
                result = ctx.SaveChanges();
            }
            oTempRecord = null;

            return result > 0;

        }// end-of-Save Hdr


        private bool p_SaveSpTrnIWHTrfDtl(spTrnIWHTrfDtl oRecord)
        {
            if (oRecord == null)
                return false;
            int result = 0;

            ctx.spTrnIWHTrfDtls.Add(oRecord);
            result = ctx.SaveChanges();

            return result > 0;
        }
        // end-of-Save Dtl

        private bool p_SetWTrfDtlForClaim(spTrnIWHTrfHdr recordHdr, SpTrnPClaimDtl recordDtl,
            SpMstItemLoc recordItemLoc, spMstItemPrice recordItemPrice, string warehouseCode, decimal Qty, string warehouseCodeTo)
        {
            spTrnIWHTrfDtl recWHDetail = new spTrnIWHTrfDtl();
            SpMstItemLoc recItemLocTemp = new SpMstItemLoc();

            recWHDetail.CompanyCode = CompanyCode;
            recWHDetail.BranchCode = BranchCode;
            recWHDetail.WHTrfNo = recordHdr.WHTrfNo;
            recWHDetail.PartNo = recordDtl.PartNo;
            recWHDetail.RetailPrice = recordItemPrice.RetailPrice;
            recWHDetail.RetailPriceInclTax = recordItemPrice.RetailPriceInclTax;
            recWHDetail.CostPrice = recordDtl.CostPrice;
            recWHDetail.FromWarehouseCode = warehouseCode;
            recWHDetail.FromLocationCode = recordItemLoc.LocationCode;
            recWHDetail.ReasonCode = recordDtl.ReasonCode;
            recWHDetail.MovingCode = recordDtl.MovingCode;
            recWHDetail.CreatedBy = CurrentUser.UserId;
            recWHDetail.CreatedDate = DateTime.Now;
            recWHDetail.LastUpdateBy = CurrentUser.UserId;
            recWHDetail.LastUpdateDate = DateTime.Now;

            recWHDetail.Qty = Qty; // global
            recWHDetail.ToWarehouseCode = warehouseCodeTo;
            
            var sqlItemLoc = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                GetDbMD(), CompanyMD, BranchMD, recordDtl.PartNo, recWHDetail.ToWarehouseCode);
            recItemLocTemp = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

            if (recItemLocTemp != null)
                recWHDetail.ToLocationCode = recItemLocTemp.LocationCode;
            else
                recWHDetail.ToLocationCode = "-";

            Helpers.ReplaceNullable(recWHDetail);
            ctx.spTrnIWHTrfDtls.Add(recWHDetail);
            if (!p_SaveSpTrnIWHTrfDtl(recWHDetail)) return false;
            return true;
        }

        private bool p_UpdateQuantitySpTrnIWHTrfDtl(List<spTrnIWHTrfDtl> listSpTrnIWHTrfDtl, ref string ePesan)
        {
            bool stat = false;
            int caseStock = 0;
            bool result = false;
            //decimal QtyTujuan = 0;
            decimal QtyReal = 0;
            string[] warehouseRest = { "97", "98", "99", "X1", "X2", "X3", "X4" };

            SpMstItemLoc recordLocTo;
            SpMstItemLoc recordLocFrom;
            spMstItem recordItem;
            spTrnIWHTrfHdr recordHeader;
            spTrnIWHTrfDtl recordDetail;

            var rocCount = listSpTrnIWHTrfDtl.Count();
            if (rocCount > 0)
            {
                for (int x = 0; x < rocCount; x++)
                {
                    if (x == rocCount - 1)
                        stat = true;

                    // NOTES : This line will insert new item locations based on the the "events" value
                    var sqlItemLocTo = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                        GetDbMD(), CompanyMD, BranchMD, listSpTrnIWHTrfDtl[x].PartNo, listSpTrnIWHTrfDtl[x].ToWarehouseCode);
                    recordLocTo = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLocTo).FirstOrDefault();


                    var sqlItemLocFrom = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                        GetDbMD(), CompanyMD, BranchMD, listSpTrnIWHTrfDtl[x].PartNo, listSpTrnIWHTrfDtl[x].FromWarehouseCode);
                    recordLocFrom = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLocFrom).FirstOrDefault();

                    // TO DO : This line of code below will prepare the data
                    var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                        GetDbMD(), CompanyMD, BranchMD, listSpTrnIWHTrfDtl[x].PartNo);
                    recordItem = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();

                    recordDetail = ctx.spTrnIWHTrfDtls.Find(CompanyCode, BranchCode, listSpTrnIWHTrfDtl[x].WHTrfNo,
                                   listSpTrnIWHTrfDtl[x].PartNo, listSpTrnIWHTrfDtl[x].FromWarehouseCode, listSpTrnIWHTrfDtl[x].ToWarehouseCode);

                    recordHeader = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, listSpTrnIWHTrfDtl[x].WHTrfNo);

                    if (recordLocTo == null)
                    {
                        recordLocTo = new SpMstItemLoc();
                        recordLocTo.CompanyCode = CompanyCode;
                        recordLocTo.BranchCode = BranchCode;
                        recordLocTo.WarehouseCode = listSpTrnIWHTrfDtl[x].ToWarehouseCode;
                        recordLocTo.LocationCode = "-";
                        recordLocTo.PartNo = listSpTrnIWHTrfDtl[x].PartNo;
                        recordLocTo.CreatedBy = CurrentUser.UserId;
                        recordLocTo.CreatedDate = DateTime.Now;

                        p_SetNewLocation(recordLocTo);
                    }

                    // TO DO : This line of code will update the items
                    if (recordDetail != null)
                    {
                        var qty = listSpTrnIWHTrfDtl[x].Qty ?? 0;

                        // TO DO : This line of code will check again the Qty
                        decimal avaQty = recordLocFrom.OnHand.Value - (recordLocFrom.AllocationSP.Value
                            + recordLocFrom.AllocationSR.Value + recordLocFrom.AllocationSL.Value
                            + recordLocFrom.ReservedSP.Value + recordLocFrom.ReservedSL.Value
                            + recordLocFrom.ReservedSR).Value;

                        QtyReal = qty;

                        if (avaQty < QtyReal)
                        {
                            // TO DO : This line of code will handle the error that caused not by exception
                            ePesan = "Qty yang tersedia tidak mencukupi untuk Claim";
                            return false;
                        }

                        // TO DO : This line of code below will update the spMstItems
                        // ==========================================================

                        #region Validation Update Stock
                        caseStock = p_SetCaseUpdateStock(listSpTrnIWHTrfDtl[x].FromWarehouseCode,
                                    listSpTrnIWHTrfDtl[x].ToWarehouseCode, warehouseRest);

                        switch (caseStock)
                        {   /*  Claim Part [Shortage, Damage, Wrong Part]*/
                            case 1:
                                UpdateStock(listSpTrnIWHTrfDtl[x].PartNo, listSpTrnIWHTrfDtl[x].FromWarehouseCode, (QtyReal * -1), 0, 0, 0, string.Empty);
                                UpdateStockWarehouse(listSpTrnIWHTrfDtl[x].PartNo, listSpTrnIWHTrfDtl[x].ToWarehouseCode, QtyReal, 0, 0, 0);
                                result = true;         
                                break;
                            /*  Receiving Claim Part [Shortage, Damage, Wrong Part]*/
                            case 2:
                                UpdateStock(listSpTrnIWHTrfDtl[x].PartNo,listSpTrnIWHTrfDtl[x].ToWarehouseCode, QtyReal, 0, 0, 0, string.Empty);
                                UpdateStockWarehouse(listSpTrnIWHTrfDtl[x].PartNo, listSpTrnIWHTrfDtl[x].FromWarehouseCode, (QtyReal * -1), 0, 0, 0);
                                result = true;
                                break;
                            /*  Warehouse Transfer */
                            case 3:
                                UpdateStockWarehouse(listSpTrnIWHTrfDtl[x].PartNo,listSpTrnIWHTrfDtl[x].FromWarehouseCode, (QtyReal * -1), 0, 0, 0);
                                UpdateStockWarehouse(listSpTrnIWHTrfDtl[x].PartNo, listSpTrnIWHTrfDtl[x].ToWarehouseCode, QtyReal, 0, 0, 0);
                                result = true;
                                break;
                        }

                        #endregion

                        if (!result)
                        {
                            ePesan = "Proses Update Persediaan Gagal";
                            result = false;
                            return result;
                        }

                        if (result)
                            MovementLog(recordHeader.WHTrfNo, recordHeader.WHTrfDate.Value, listSpTrnIWHTrfDtl[x].PartNo, listSpTrnIWHTrfDtl[x].ToWarehouseCode,
                                listSpTrnIWHTrfDtl[x].FromWarehouseCode, false, "", Convert.ToDecimal(listSpTrnIWHTrfDtl[x].Qty.Value));

                        if (!result)
                        {
                            ePesan = "Update Movement Log Transaksi Gagal";
                            result = false;
                            return result;
                        }

                        if (stat)
                        {
                            recordHeader.Status = "2";
                            recordHeader.LastUpdateBy = CurrentUser.UserId;
                            recordHeader.LastUpdateDate = DateTime.Now;

                            result = ctx.SaveChanges() >= 0;

                            if (!result)
                            {
                                ePesan = "Update Status Warehouse TRF Header Gagal";
                                result = false;
                                return result;
                            }
                        }

                    }
                    else
                    {
                        ePesan = "Detail Data transaksi tidak ada";
                        result = false;
                        return result;
                    }
                }
            }
            return result;
        }

        private void p_SetNewLocation(SpMstItemLoc recordLoc)
        {
            if (recordLoc == null)
                return;


            var sqlItemLocUpdate = string.Format(@"INSERT INTO {0}..SpMstItemLoc(CompanyCode, BranchCode, WarehouseCode, LocationCode, PartNo, CreatedBy, CreatedDate)  
                VALUES('{1}','{2}','{3}','{4}','{5}','{6}','{7}')", GetDbMD(), recordLoc.CompanyCode, recordLoc.BranchCode, recordLoc.WarehouseCode, recordLoc.LocationCode,
            recordLoc.PartNo, recordLoc.CreatedBy, recordLoc.CreatedDate); 
            
            ctx.Database.ExecuteSqlCommand(sqlItemLocUpdate);
            ctx.SaveChanges();
        }

        private int p_SetCaseUpdateStock(string fromWH, string toWH, string[] whValid)
        {
            if (fromWH.StartsWith("X"))
            {
                return 2;
            }
            else if (toWH.StartsWith("X"))
            {
                return 1;
            }
            else
            {
                try
                {
                    if (Convert.ToInt32(fromWH) < 97)
                    {
                        for (int x = 0; x < whValid.Length; x++)
                        {
                            if (toWH == whValid[x].ToString())
                                return 1;
                        }
                        return 3;
                    }
                    else
                    {
                        for (int x = 0; x < whValid.Length; x++)
                        {
                            if (toWH == whValid[x].ToString())
                                return 3;
                        }
                        return 2;
                    }
                }
                catch (Exception)
                {

                    return 3;
                }
            }
        }

    }
}