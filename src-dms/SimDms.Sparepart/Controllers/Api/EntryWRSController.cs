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
using SimDms.Sparepart.BLL;
using System.Transactions;

namespace SimDms.Sparepart.Controllers.Api
{
    public class EntryWRSController : BaseController
    {
        public JsonResult Default()
        {
            var curDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            return Json(new { currentDate = curDate });
        }

        public string getData()
        {
            try
            {
                return GetNewDocumentNoHpp("WRL", DateTime.Now.ToString("yyyyMMdd"));
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                return e.Message;
            }
        }

        public string getData2()
        {
            try
            {
                return GetNewDocumentNoHpp("WRN", DateTime.Now.ToString("yyyyMMdd"));
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                return e.Message;
            }
        }


        public JsonResult Save(SpTrnPRcvHdr model, string pil)
        {
            if (model.ReferenceNo == null)
            {
                model.ReferenceNo = "";
            }

            var validTranMsg = DateTransValidation(model.WRSDate.Value);
            if (!string.IsNullOrEmpty(validTranMsg))
            {
                return Json(new { success = false, message = validTranMsg });
            }


            var recordBinn = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
            if (recordBinn == null)
            {
                return Json(new { success = false, message = "Save WRS," + model.BinningNo + " sudah tercatat dalam WRS record" });
            }
            else if (recordBinn.Status == "4")
            {
                return Json(new { success = false, message = "Binning Status informasi tidak Benar" });
            }

            var record = ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, model.WRSNo);
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (record == null)
                    {
                        if (model.WRSNo.Length > 20)
                        {
                            return Json(new { success = false, message = model.WRSNo });
                        }

                        record = new SpTrnPRcvHdr
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            WRSNo = pil == "1" ? getData() : getData2(),
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                            TypeOfGoods = TypeOfGoods
                        };
                        ctx.SpTrnPRcvHdrs.Add(record);
                    }

                    record.WRSDate = model.WRSDate;
                    record.BinningNo = model.BinningNo;
                    record.BinningDate = model.BinningDate;
                    record.DNSupplierDate = model.DNSupplierDate;
                    record.PrintSeq = 0;
                    record.ReceivingType = recordBinn.ReceivingType;
                    record.TransType = recordBinn.TransType;
                    record.DNSupplierNo = model.DNSupplierNo;
                    record.SupplierCode = model.SupplierCode;
                    record.ReferenceNo = model.ReferenceNo;
                    record.ReferenceDate = model.ReferenceDate;
                    record.TotItem = model.TotItem;
                    record.TotWRSAmt = model.TotWRSAmt;
                    record.Status = "0";
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;

                    ctx.SaveChanges();
                    IQueryable<EntryWRSModel> dataDtl = null;

                    try
                    {
                        dataDtl = RecordDetail(model.BinningNo, record.WRSNo);
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                    }
                    return Json(new { success = true, cod = record.WRSNo, status = record.Status, dataDtl = dataDtl });
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    return Json(new { success = false, message = "Gagal simpan data!!!" });
                }
            }
        }

        public IQueryable<EntryWRSModel> RecordDetail(string BinningNo, string WRSNo)
        {
            ctx.SpTrnPBinnDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == BinningNo).ToList().ForEach(x =>
            {
                var receiveDtl = ctx.SpTrnPRcvDtls.Find(CompanyCode, BranchCode, WRSNo, x.PartNo, x.DocNo, x.BoxNo);
                updateBin(BinningNo);
                if (receiveDtl == null)
                {
                    receiveDtl = new SpTrnPRcvDtl
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        WRSNo = WRSNo,
                        PartNo = x.PartNo,
                        DocNo = x.DocNo,
                        BoxNo = x.BoxNo,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now

                    };
                    ctx.SpTrnPRcvDtls.Add(receiveDtl);
                }
                receiveDtl.DocDate = x.DocDate;
                receiveDtl.WarehouseCode = x.WarehouseCode;
                receiveDtl.LocationCode = x.LocationCode;
                receiveDtl.BoxNo = x.BoxNo;
                receiveDtl.ReceivedQty = x.ReceivedQty;
                receiveDtl.PurchasePrice = x.PurchasePrice;
                receiveDtl.CostPrice = x.CostPrice;
                receiveDtl.DiscPct = x.DiscPct;
                receiveDtl.ABCClass = x.ABCClass;
                receiveDtl.MovingCode = x.MovingCode;
                receiveDtl.ProductType = x.ProductType;
                receiveDtl.PartCategory = x.PartCategory;
                receiveDtl.LastUpdateBy = CurrentUser.UserId;
                receiveDtl.LastUpdateDate = DateTime.Now;
            });

            ctx.SaveChanges();

            var LoadTabel = getDetail(ctx, CompanyCode, BranchCode, WRSNo);
            return LoadTabel;
        }

        public JsonResult SaveRecordDetail(SpTrnPRcvDtl model, string BinningNo, string WRSNo)
        {
            //var record = ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, BinningNo);
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    ctx.SpTrnPBinnDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == BinningNo).ToList().ForEach(x =>
                    {
                        var receiveDtl = ctx.SpTrnPRcvDtls.Find(CompanyCode, BranchCode, WRSNo, x.PartNo, x.DocNo, x.BoxNo);
                        updateBin(BinningNo);
                        if (receiveDtl == null)
                        {
                            receiveDtl = new SpTrnPRcvDtl
                            {
                                CompanyCode = CompanyCode,
                                BranchCode = BranchCode,
                                WRSNo = WRSNo,
                                PartNo = x.PartNo,
                                DocNo = x.DocNo,
                                CreatedBy = CurrentUser.UserId,
                                CreatedDate = DateTime.Now

                            };
                            ctx.SpTrnPRcvDtls.Add(receiveDtl);
                        }
                        receiveDtl.DocDate = x.DocDate;
                        receiveDtl.WarehouseCode = x.WarehouseCode;
                        receiveDtl.LocationCode = x.LocationCode;
                        receiveDtl.BoxNo = x.BoxNo;
                        receiveDtl.ReceivedQty = x.ReceivedQty;
                        receiveDtl.PurchasePrice = x.PurchasePrice;
                        receiveDtl.CostPrice = x.CostPrice;
                        receiveDtl.DiscPct = x.DiscPct;
                        receiveDtl.ABCClass = x.ABCClass;
                        receiveDtl.MovingCode = x.MovingCode;
                        receiveDtl.ProductType = x.ProductType;
                        receiveDtl.PartCategory = x.PartCategory;
                        receiveDtl.LastUpdateBy = CurrentUser.UserId;
                        receiveDtl.LastUpdateDate = DateTime.Now;

                    });

                    ctx.SaveChanges();
                    trans.Commit();

                    var LoadTabel = ctx.GridEntryWRSs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.WRSNo == WRSNo);
                    return Json(new { success = true, dat = LoadTabel });
                }
                catch (Exception)
                {
                    trans.Rollback();
                    return Json(new { success = false, message = "Gagal simpan detail!!!" });
                }
            }
        }

        public JsonResult updateBin(string BinningNo)
        {
            var binnHdr = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, BinningNo);

            if (binnHdr == null)
            {
                binnHdr = new spTrnPBinnHdr
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    BinningNo = BinningNo,

                };
                ctx.spTrnPBinnHdrs.Add(binnHdr);
            }
            binnHdr.Status = "4";
            binnHdr.LastUpdateBy = CurrentUser.UserId;
            binnHdr.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Gagal update Binning!!!" });
            }
        }

        public JsonResult delete(SpTrnPRcvHdr model)
        {
            var receiveHdr = ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, model.WRSNo);

            if (receiveHdr == null)
            {
                receiveHdr = new SpTrnPRcvHdr
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    WRSNo = model.WRSNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    TypeOfGoods = TypeOfGoods

                };
                ctx.SpTrnPRcvHdrs.Add(receiveHdr);
            }
            receiveHdr.Status = "3";
            receiveHdr.LastUpdateBy = CurrentUser.UserId;
            receiveHdr.LastUpdateDate = DateTime.Now;
            // using (var ct = new DataContext())
            //   {
            ctx.SpTrnPBinnDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == model.BinningNo).ToList().ForEach(x =>
            {
                var receiveDtl = ctx.SpTrnPRcvDtls.Find(CompanyCode, BranchCode, model.WRSNo, x.PartNo, x.DocNo);

                if (receiveDtl != null)
                {
                    ctx.SpTrnPRcvDtls.Remove(receiveDtl);
                }
            });
            //  }

            try
            {

                ctx.SaveChanges();
                deleteBin(model.BinningNo);
                return Json(new { success = true });
            }

            catch (Exception)
            {
                return Json(new { success = false, message = "Gagal hapus data!!!" });
            }
        }

        public JsonResult deleteBin(string BinningNo)
        {
            bool isSuccess = true;
            var binnHdr = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, BinningNo);
            if (binnHdr == null)
            {
                binnHdr = new spTrnPBinnHdr
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    BinningNo = BinningNo,

                };
                ctx.spTrnPBinnHdrs.Add(binnHdr);
            }
            binnHdr.Status = "1";
            binnHdr.LastUpdateBy = CurrentUser.UserId;
            binnHdr.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
            }

            if (!isSuccess)
            {
                throw new Exception("Hapus Data Gagal, Status binning header tidak bisa diupdate.");
            }

            // Detail Part                
            isSuccess = UpdateOnOrderAndIntransit(BinningNo);

            if (!isSuccess)
            {
                throw new Exception("hapus data, Update OnOrder Dan InTransit Part Gagal !!!");
            }
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Gagal hapus detail!!!" });
            }
        }

        public bool UpdateOnOrderAndIntransit(string BinningNo)
        {
            try
            {
                SqlConnection con = (SqlConnection)ctx.Database.Connection;
                SqlCommand cmd = con.CreateCommand();


                cmd.CommandText = "uspfn_UpdateInTransitSparePart";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@com", CompanyCode);
                cmd.Parameters.AddWithValue("@bra", BranchCode);
                cmd.Parameters.AddWithValue("@docno", BinningNo);
                cmd.Parameters.AddWithValue("@userid", CurrentUser.UserId);
                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public JsonResult Print(SpTrnPRcvHdr model)
        {
            var record = ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, model.WRSNo);
            if (record != null)
            {
                record.Status = "1";
                record.PrintSeq = record.PrintSeq + 1;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
            }
            try
            {
                ctx.SaveChanges();
                var LoadTabel = ctx.GridEntryWRSs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.WRSNo == model.WRSNo);
                return Json(new { success = true, status = record.Status, datdet = LoadTabel });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Gagal print data!!!" });
            }


        }

        public JsonResult CloseWRS(spTrnPBinnHdr model, SpTrnPRcvHdr model1)
        {
            using (ctx)
            {
                using (var trx = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        SpMstItemLocBLL spMstItemLocBLL = SpMstItemLocBLL.Instance(CurrentUser.UserId);
                        //            spTrnPBinnHdr model = new spTrnPBinnHdr();
                        //          SpTrnPRcvHdr model1 = new SpTrnPRcvHdr();
                        bool result = true;
                        var MSG_5034 = " tidak ditemukan";
                        var MSG_5039 = " gagal";
                        var pesan_error = "";
                        var pesan_error2 = "";
                        var binnHdr = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
                        if (binnHdr == null)
                        {
                            pesan_error = "Data detail dengan binning no " + model.BinningNo + MSG_5034;
                            return Json(new { success = false, message = pesan_error });

                        }

                        var receiveHdr = ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, model1.WRSNo);
                        if (receiveHdr.isLocked == null)
                        {
                            receiveHdr.isLocked = false;
                        }
                        if (receiveHdr == null)
                        {
                            pesan_error = "WRS[" + model1 + "]" + MSG_5034;
                            return Json(new { success = false, message = pesan_error });

                        }
                        else if (receiveHdr.Status == "2")
                        {
                            pesan_error = "Data detail dengan binning no " + model.BinningNo + " sudah terposting";
                            return Json(new { success = false, message = pesan_error });

                        }

                        binnHdr.Status = "4";
                        binnHdr.LastUpdateBy = CurrentUser.UserId;
                        binnHdr.LastUpdateDate = DateTime.Now;

                        if (result)
                        {
                            bool isUpdateOnHandOnly = false;
                            bool isUpdateOnHandAndInTransit = false;
                            bool isUpdateOnHandAndBorrow = false;
                            bool isUpdateOnHandAndBorrowed = false;
                            //bool isPurchasePriceIncPPN = false;
                            decimal avgCost = 0;
                            decimal pPrice = 0;
                            decimal taxPct = 0;
                            bool stat = true;

                            var query = string.Format(@"
            SELECT A.WRSNo, A.DocNo, A.PartNo, A.PurchasePrice, A.WarehouseCode,
            A.DiscPct,  A.ReceivedQty, A.BoxNo, (select PartName from spMstItemInfo C
            where C.CompanyCode=A.CompanyCode and C.PartNo=A.PartNo) as NmPart 
              FROM spTrnPRcvDtl A
             INNER JOIN spTrnPRcvHdr B ON 
             B.CompanyCode = A.CompanyCode
             AND B.BranchCode = A.BranchCode 
             AND B.WRSNo = A.WRSNo 
             WHERE A.CompanyCode = '{0}'
               AND A.BranchCode = '{1}'
               AND A.WRSNo =  '{2}'", CompanyCode, BranchCode, model1.WRSNo);
                            ctx.Database.SqlQuery<QueryCloseWrs>(query).ToList().ForEach(x =>
                            {
                                if (receiveHdr.TransType.Equals("4"))   // pembelian
                                {
                                    // get POS header
                                    var posHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, x.DocNo);
                                    if (posHdr == null)
                                    {
                                        pesan_error = "Data detail dengan POS no " + x.DocNo + MSG_5034;
                                        stat = false;
                                        //return Json(new { success = result, message = pesan_error });
                                    }

                                    // update status POS header
                                    posHdr.Status = "5";    // Receiving Generated
                                    posHdr.LastUpdateBy = CurrentUser.UserId;
                                    posHdr.LastUpdateDate = DateTime.Now;

                                    if (!result)
                                    {
                                        pesan_error = "update status pos header[" + posHdr.POSNo + "]" + MSG_5039;
                                    }
                                }

                                isUpdateOnHandOnly = (receiveHdr.ReceivingType.Equals("1") && receiveHdr.TransType.Equals("0")) ||  // BPS & transfer stock
                                                         (receiveHdr.ReceivingType.Equals("1") && receiveHdr.TransType.Equals("3")) ||  // BPS & lain-lain
                                                         (receiveHdr.ReceivingType.Equals("3") && receiveHdr.TransType.Equals("0")) ||   // transfer stock & transfer stock
                                                         (receiveHdr.ReceivingType.Equals("1") && receiveHdr.TransType.Equals("5"));    // transfer internal

                                isUpdateOnHandAndInTransit = (receiveHdr.ReceivingType.Equals("1") && receiveHdr.TransType.Equals("4")) || // BPS & pembelian
                                                             (receiveHdr.ReceivingType.Equals("2") && receiveHdr.TransType.Equals("4"));   // PINV & pembelian

                                isUpdateOnHandAndBorrow = (receiveHdr.ReceivingType.Equals("1") && receiveHdr.TransType.Equals("1"));  // BPS & peminjaman

                                isUpdateOnHandAndBorrowed = (receiveHdr.ReceivingType.Equals("1") && receiveHdr.TransType.Equals("2"));

                                var qr = string.Format(@"
            SELECT A.CompanyCode, A.BranchCode, A.PartNo,
                   C.SupplierCode, A.OnHand, B.CostPrice
              FROM spMstItems A
                   INNER JOIN spMstItemPrice B 
                        ON (A.CompanyCode = B.CompanyCode) AND (A.BranchCode = B.BranchCode) AND (A.PartNo = B.PartNo)
                    INNER JOIN spMstItemInfo C
                        ON C.CompanyCode = A.CompanyCode AND C.PartNo = A.PartNo
            WHERE A.CompanyCode = '{0}'
            AND A.BranchCode = '{1}'
            AND A.TypeOfGoods = '{2}'
            AND A.ProductType = '{3}'
            AND A.PartNo = '{4}'
             ORDER BY
               A.CompanyCode ASC,
               A.BranchCode ASC,
               A.PartNo ASC", CompanyCode, BranchCode, TypeOfGoods, ProductType, x.PartNo);
                                var rowItem = ctx.Database.SqlQuery<QueryCloseWrs1>(query).ToList();
                                if (rowItem[0].OnHand == null)
                                {
                                    rowItem[0].OnHand = 0;
                                }
                                if (rowItem == null)
                                {
                                    pesan_error = "Cost price part[" + x.PartNo + "]" + MSG_5034;
                                    stat = false;
                                    //return Json(new { success = result, message = pesan_error });
                                }

                                if (isUpdateOnHandOnly)
                                {


                                    //var recItemLoc = spMstItemLocBLL.GetRecord(newPartNo, WAREHOUSECODE);

                                    result = spMstItemLocBLL.UpdateStock(ctx, x.PartNo, x.WarehouseCode, x.ReceivedQty, 0, 0, 0, string.Empty);
                                    if (!result)
                                    {
                                        pesan_error = "Update On Hand Persediaan Gagal";
                                        stat = false;
                                        //return Json(new { success = result, message = pesan_error });
                                    }
                                }

                                if (isUpdateOnHandAndBorrow)
                                {
                                    // update onhand and borrow in SpMstItems table
                                    // update onhand in SpMstItems and SpMstItemLoc
                                    result = spMstItemLocBLL.UpdateStock(ctx, x.PartNo, x.WarehouseCode, x.ReceivedQty, 0, 0, 0, string.Empty);
                                    if (!result)
                                    {
                                        pesan_error = "Update Qty Peminjaman di Persediaan Item Lokasi Gagal";
                                        stat = false;
                                        // return Json(new { success = result, message = pesan_error });
                                    }

                                    // update borrow in SpMstItems
                                    result = spMstItemLocBLL.UpdateStock(ctx, x.PartNo, x.WarehouseCode, x.ReceivedQty, 0, 0, 0, string.Empty);
                                    if (!result)
                                    {
                                        pesan_error = "Update Qty Peminjaman Persediaan Item Gagal";
                                        stat = false;
                                        //return Json(new { success = result, message = pesan_error });
                                    }

                                }

                                if (isUpdateOnHandAndBorrowed)
                                {
                                    // update onhand and borrow in SpMstItems table
                                    // update onhand in SpMstItems and SpMstItemLoc
                                    result = spMstItemLocBLL.UpdateStock(ctx, x.PartNo, x.WarehouseCode, x.ReceivedQty, 0, 0, 0, string.Empty);
                                    if (!result)
                                    {
                                        pesan_error = "Update Qty Dipinjam Persediaan Item Lokasi Gagal";
                                        stat = false;
                                        //return Json(new { success = result, message = pesan_error });
                                    }

                                    // update borrow in SpMstItems
                                    result = spMstItemLocBLL.UpdateStock(ctx, x.PartNo, x.WarehouseCode, x.ReceivedQty, 0, 0, 0, string.Empty);
                                    if (!result)
                                    {
                                        pesan_error = "Update Qty Dipinjam Persediaan Item Gagal";
                                        stat = false;
                                        //return Json(new { success = result, message = pesan_error });
                                    }
                                }

                                if (!(receiveHdr.TransType.Equals("0")))
                                {
                                    // get item price
                                    var itemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, x.PartNo);
                                    if (itemPrice == null)
                                    {
                                        pesan_error = "Item price part [" + x.PartNo + "]" + MSG_5034;
                                        stat = false;
                                        //return Json(new { success = result, message = pesan_error });
                                    }// end-of-itemPrice-null

                                    // update wrs detail cost price 
                                    x.CostPrice = itemPrice.CostPrice;
                                    x.LastUpdateBy = CurrentUser.UserId;
                                    x.LastUpdateDate = DateTime.Now;

                                    if (!result)
                                    {
                                        pesan_error = "update cost price wrs detail part [" + x.PartNo +
                                            "] dan doc[" + x.DocNo + "]" + MSG_5039;
                                        stat = false;
                                        //return Json(new { success = result, message = pesan_error });
                                    }
                                }
                                if (receiveHdr.TransType.Equals("4") || receiveHdr.TransType.Equals("0") || receiveHdr.TransType.Equals("5")) // 4->pembelian ; 0->transfer stock ; 3->Lain-Lain
                                {
                                    SupplierProfitCenter recProcCenter = new SupplierProfitCenter();
                                    if (!(bool)receiveHdr.isLocked)
                                    {
                                        // get is Purchase Include Tax
                                        var recPC = ctx.SupplierProfitCenters.Find(CompanyCode, BranchCode, receiveHdr.SupplierCode, ProfitCenter);

                                        if (recPC == null)
                                        {
                                            pesan_error = "Proses Closing WRS Gagal ! Periksa kembali data master supplier anda !";
                                            stat = false;
                                            //return Json(new { success = result, message = pesan_error });
                                        }
                                    }

                                    string TaxCod = recProcCenter.TaxCode;
                                    if (string.IsNullOrEmpty(TaxCod))
                                        taxPct = 0;
                                    else
                                        taxPct = (decimal)ctx.Taxes.Find(CompanyCode, TaxCod).TaxPct;


                                    pPrice = (decimal)x.PurchasePrice;

                                    // get average cost
                                    if (receiveHdr.TransType.ToString() == "0")
                                        avgCost = AverageCostTransferStock(x.PartNo, (decimal)rowItem[0].OnHand, x.ReceivedQty, (decimal)rowItem[0].CostPrice, pPrice);
                                    else if (receiveHdr.TransType.ToString() == "5")
                                        avgCost = AverageCostOthers(x.PartNo, (decimal)rowItem[0].OnHand, x.ReceivedQty, (decimal)rowItem[0].CostPrice, pPrice);
                                    else
                                        avgCost = AverageCost((decimal)rowItem[0].OnHand, (decimal)rowItem[0].CostPrice, x.ReceivedQty, pPrice, x.DiscPct);

                                    // update average cost in item price table
                                    //decimal Nil = 0;
                                    result = UpdateItemAvgCost(x.PartNo, avgCost, (decimal)rowItem[0].CostPrice) >= 0;
                                    if (!result)
                                    {
                                        pesan_error = "update item average cost part[" + x.PartNo + "]" + MSG_5039;
                                        stat = false;
                                        //return Json(new { success = result, message = pesan_error });
                                    }
                                }

                                if (isUpdateOnHandAndInTransit)
                                {
                                    // update orderbalance & SpMstItems
                                    result = UpdateIntransitOrderBalance(x.DocNo, receiveHdr.SupplierCode, x.PartNo, x.ReceivedQty, x.WRSNo, receiveHdr.WRSDate) >= 0;

                                    if (!result)
                                    {
                                        pesan_error = "update order balance part[" + x.PartNo + "] dan dokumen[ " + x.DocNo + "]" + MSG_5039;
                                        stat = false;
                                        //return Json(new { success = false, message = pesan_error });
                                    }

                                    // update SpMstItemLoc 
                                    result = UpdateItemStockLocation(x.PartNo, x.ReceivedQty, 1, "00") >= 0;
                                    if (!result)
                                    {
                                        pesan_error = "update item location onhand part[" + x.PartNo + "]" + MSG_5039;
                                        stat = false;
                                        //return Json(new { success = false, message = pesan_error });
                                    }
                                }
                                if (result)
                                {
                                    result = MovementLog(receiveHdr.WRSNo, receiveHdr.WRSDate, x.PartNo, x.WarehouseCode, "IN", "WRS", x.ReceivedQty);
                                }
                                if (!result)
                                {
                                    pesan_error = "Proses Insert Data Movement Gagal";
                                }
                            });

                            if (!stat)
                            {
                                // Rollback transaction
                                trx.Rollback();

                                return Json(new { success = false, message = pesan_error });
                            }

                            try
                            {
                                if (receiveHdr == null)
                                {
                                    receiveHdr = new SpTrnPRcvHdr()
                                    {
                                        CompanyCode = CompanyCode,
                                        BranchCode = BranchCode,
                                        WRSNo = model1.WRSNo
                                    };
                                    ctx.SpTrnPRcvHdrs.Add(receiveHdr);
                                }
                                receiveHdr.Status = "2";
                                receiveHdr.LastUpdateBy = CurrentUser.UserId;
                                receiveHdr.LastUpdateDate = DateTime.Now;

                                result = true;
                            }
                            catch
                            {
                                result = false;
                            }

                            if (!result)
                            {
                                // Rollback transaction
                                trx.Rollback();

                                pesan_error = "update status header wrs[" + receiveHdr.WRSNo + "]" + MSG_5039;
                                return Json(new { success = false, message = pesan_error });
                            }
                            else
                            {
                                try
                                {
                                    if (receiveHdr.ReceivingType == "3")
                                    {
                                        decimal cogs = GetSumCOGS(receiveHdr.WRSNo);
                                        string msgWrn = string.Empty;
                                        result = JournalSpIn(receiveHdr.WRSNo, receiveHdr.WRSDate, cogs, receiveHdr.TypeOfGoods, receiveHdr.SupplierCode, true, ref msgWrn);
                                        if (!result)
                                        {
                                            // Rollback transaction
                                            trx.Rollback();

                                            pesan_error = "Proses Simpan Data Jurnal Pembelian Gagal ! " + msgWrn;
                                            return Json(new { success = false, message = pesan_error });
                                        }
                                    }
                                    else if (receiveHdr.ReceivingType == "1" && receiveHdr.TransType == "5")
                                    {
                                        decimal cogs = GetSumCOGS(receiveHdr.WRSNo);
                                        result = JournalSpInOthers(receiveHdr.WRSNo, receiveHdr.WRSDate, cogs, receiveHdr.TypeOfGoods, receiveHdr.SupplierCode, true);
                                        if (!result)
                                        {

                                            // Rollback transaction
                                            trx.Rollback();

                                            pesan_error = "Proses Simpan Data Jurnal Pembelian Gagal ! Periksa kembali data master supplier anda !";
                                            return Json(new { success = false, message = pesan_error });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Rollback transaction
                                    trx.Rollback();

                                    pesan_error = "Proses Simpan Data Jurnal Pembelian Gagal ! " + ex.Message;
                                    return Json(new { success = false, message = pesan_error });
                                }
                            }

                        }
                        else
                        {
                            // Rollback transaction
                            trx.Rollback();

                            return Json(new { success = false, message = "Data sedang di Locking, Tunggu beberapa saat lagi" });
                        }
                        if (result)
                        {
                            ctx.SaveChanges();

                            // Commit transaction
                            trx.Commit();
                            return Json(new { success = true, message = "WRS Berhasil di close" });
                        }
                        else
                        {
                            // Rollback transaction
                            trx.Rollback();

                            return Json(new { success = false, message = pesan_error2 });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction
                        trx.Rollback();
                        var innerMsg = " " + (ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : "");
                        return Json(new { success = false, message = "WRS tidak berhasil di closing. Mohon hubungi SDMS Support", error_log = ex.Message + innerMsg });
                    }
                }
            }
        }

        private bool JournalSpInOthers(string docno, DateTime? docdate, decimal cogs, string typeOfGoods, string supplierCode, bool flag)
        {
            MstSupplierProfitCenter oGnMstSupplierProfitCenterDao = new MstSupplierProfitCenter();
            var recordProf = ctx.MstSupplierProfitCenters.Find(CompanyCode, BranchCode, supplierCode, ProfitCenter);
            if (recordProf == null)
            {
                return false;
            }
            var oSupplierClass = GnMstSupplierClassNew(supplierCode, typeOfGoods);
            if (oSupplierClass == null)
            {
                return false;
            }

            var recordWRS = ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, docno);
            var oAccount = ctx.spMstAccounts.Find(CompanyCode, BranchCode, typeOfGoods);
            var oSenderAccount = ctx.spMstAccounts.Find(CompanyCode, supplierCode, typeOfGoods);
            GnMstCoProfileSpare oGnMstCoProfileSpareDao = new GnMstCoProfileSpare();

            int seqNo = 0;

            GLInterface oJurnal = new GLInterface();
            glJournal recJournal = new glJournal();
            GlJournalDtl recJournalDtl = new GlJournalDtl();
            var recordCPProfitCenter = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);

            oJurnal.DocNo = docno;
            oJurnal.DocDate = docdate;
            oJurnal.JournalCode = "SPAREPART";
            oJurnal.TypeJournal = "INCOMING";
            oJurnal.ApplyTo = recordWRS.DNSupplierNo; // request from Ricky San 13 Des 2010
            oJurnal.StatusFlag = "3";
            oJurnal.CreateDate = DateTime.Now;

            recJournal.CompanyCode = CompanyCode;
            recJournal.BranchCode = BranchCode;
            recJournal.FiscalYear = decimal.Parse(DateTime.Now.Year.ToString());
            recJournal.ProfitCenterCode = ProfitCenter;
            recJournal.GLDate = docdate;
            recJournal.JournalNo = GetNewGLNo("2");
            recJournal.JournalType = "Harian";
            recJournal.JournalDate = docdate;
            recJournal.ReffNo = docno;
            recJournal.ReffDate = docdate;
            recJournal.FiscalMonth = recordCPProfitCenter.FiscalMonth;
            recJournal.FiscalYear = (decimal)recordCPProfitCenter.FiscalYear;
            recJournal.StatusReverse = "0";

            var rowItem = SelectbyPeriodeGL(recordCPProfitCenter.FiscalYear.ToString(), recordCPProfitCenter.FiscalMonth.ToString(), docdate);
            if (rowItem.Count == 1)
            {
                recJournal.Periode = rowItem[0].Periode.ToString();
                recJournal.PeriodeName = rowItem[0].PeriodeName.ToString();
                recJournal.PeriodeNum = Convert.ToDecimal(rowItem[0].PeriodeNum.ToString());
            }
            else
            {
                return false;
            }

            recJournal.BalanceType = "1";
            recJournal.amountDb = recJournal.amountCr = cogs;
            recJournal.DocSource = "SP";
            recJournal.Status = "1";
            recJournal.PrintSeq = 1;

            recJournal.CreatedBy = CurrentUser.UserId;
            recJournal.CreatedDate = DateTime.Now;
            recJournal.LastUpdateBy = CurrentUser.UserId;
            recJournal.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.glJournals.Add(recJournal);
            }
            catch
            {
                return false;
            }

            recJournalDtl.CompanyCode = recJournal.CompanyCode;
            recJournalDtl.BranchCode = recJournal.BranchCode;
            recJournalDtl.FiscalYear = recJournal.FiscalYear;
            recJournalDtl.JournalNo = recJournal.JournalNo;
            recJournalDtl.Description = recordWRS.DNSupplierNo;
            recJournalDtl.DocNo = docno;
            recJournalDtl.TypeTrans = "TRANSFER SP";
            recJournalDtl.JournalType = recJournal.JournalType;
            recJournalDtl.StatusReverse = "0";

            if (cogs > 0)
            {
                // Insert Inventory Journal
                oJurnal.SeqNo = seqNo += 1;
                oJurnal.AccountNo = oAccount.InventoryAccNo;
                oJurnal.AmountDb = cogs;
                oJurnal.AmountCr = decimal.Zero;
                oJurnal.TypeTrans = "INVENTORY";

                recJournalDtl.SeqNo = oJurnal.SeqNo;
                recJournalDtl.AccountNo = oJurnal.AccountNo;
                recJournalDtl.amountDb = cogs;
                recJournalDtl.amountCr = decimal.Zero;
                recJournalDtl.TypeTrans = "INVENTORY";
                recJournalDtl.AccountType = ctx.gnMstAccounts.Find(CompanyCode, BranchCode, oJurnal.AccountNo).AccountType;

                Generate(docno, docdate, recordWRS.DNSupplierNo);

                // Insert COGS Journal
                oJurnal.SeqNo = seqNo += 1;
                oJurnal.AccountNo = oSenderAccount.InTransitAccNo;
                oJurnal.AmountDb = decimal.Zero;
                oJurnal.AmountCr = cogs;
                oJurnal.TypeTrans = "INTRANSIT";

                recJournalDtl.SeqNo = oJurnal.SeqNo;
                recJournalDtl.AccountNo = oJurnal.AccountNo;
                recJournalDtl.amountDb = decimal.Zero;
                recJournalDtl.amountCr = cogs;
                recJournalDtl.TypeTrans = "INTRANSIT";
                recJournalDtl.AccountType = ctx.gnMstAccounts.Find(CompanyCode, BranchCode, oJurnal.AccountNo).AccountType;

                Generate(docno, docdate, recordWRS.DNSupplierNo);
            }
            return true;
        }

        public string GetNewGLNo(string pil)
        {
            //  var transdate = ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate;
            if (pil == "1")
            {
                return GetNewDocumentNoHpp("JTS", DateTime.Now.ToString("yyyyMMdd"));

            }
            else
            {
                return GetNewDocumentNoHpp("JTI", DateTime.Now.ToString("yyyyMMdd"));
            }

        }

        private bool JournalSpIn(string docno, DateTime? docdate, decimal cogs, string typeOfGoods, string supplierCode, bool flag, ref string msgWrn)
        {
            var recordWRS = ctx.SpTrnPRcvHdrs.Find(CompanyCode, BranchCode, docno);
            if (recordWRS.isLocked == null)
            {
                recordWRS.isLocked = false;
            }
            var oAccount = ctx.spMstAccounts.Find(CompanyCode, BranchCode, typeOfGoods);
            var oSenderAccount = ctx.spMstAccounts.Find(CompanyCode, supplierCode, typeOfGoods);
            if (oSenderAccount == null)
            {
                msgWrn = "Master Account untuk supplier " + supplierCode + " belum di-setting";
                return false;
            }
            var query = string.Format(@"
            select * from spMstCompanyAccount 
            where CompanyCode = '{0}' and BranchCodeTo = '{1}'
            ", CompanyCode, recordWRS.SupplierCode);
            var oCompanyAccount = ctx.Database.SqlQuery<spMstCompanyAccount>(query).ToList();

            if (!(bool)recordWRS.isLocked)
            {
                // TO DO : CEK SUPPLIER CLASS AND PROFIT CENTER
                var oSupplierClass = GnMstSupplierClassNew(supplierCode, typeOfGoods);
                if (oSupplierClass == null)
                {
                    msgWrn = "Supplier Class untuk supplier " + supplierCode + " belum di-setting";
                    return false;
                }
            }
            int seqNo = 0;
            //GLInterface oJurnal = new GLInterface();
            //glJournal recJournal = new glJournal();
            //GlJournalDtl recJournalDtl = new GlJournalDtl();
            var recordCPProfitCenter = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);


            var oJurnal = ctx.GLInterfaces.Find(CompanyCode, BranchCode, docno, seqNo + 1);
            var jNo = GetNewGLNo("1");
            var recJournal = ctx.glJournals.Find(CompanyCode, BranchCode, DateTime.Now.Year, jNo);
            var recJournalDtl = ctx.GlJournalDtls.Find(CompanyCode, BranchCode, DateTime.Now.Year, jNo, seqNo + 1);

            if (oJurnal == null)
            {
                oJurnal = new GLInterface()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    DocNo = docno,
                    SeqNo = seqNo + 1,
                    DocDate = docdate,
                    ProfitCenterCode = ProfitCenter,
                    AccDate = DateTime.Now,
                    AccountNo = oAccount.InventoryAccNo,
                    JournalCode = "SPAREPART",
                    TypeJournal = "INCOMING",
                    ApplyTo = recordWRS.DNSupplierNo,
                    AmountDb = cogs,
                    AmountCr = decimal.Zero,
                    TypeTrans = "INVENTORY",
                    BatchNo = string.Empty,
                    BatchDate = new DateTime(1900, 1, 1),
                    StatusFlag = "3",
                    CreateBy = CurrentUser.UserId,
                    CreateDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now
                };
                ctx.GLInterfaces.Add(oJurnal);
            }

            var periode = "";
            var periodename = "";
            decimal periodenum = 0;
            var rowItem = SelectbyPeriodeGL(recordCPProfitCenter.FiscalYear.ToString(), recordCPProfitCenter.FiscalMonth.ToString(), docdate);
            if (rowItem.Count == 1)
            {
                periode = rowItem[0].Periode.ToString();
                periodename = rowItem[0].PeriodeName.ToString();
                periodenum = Convert.ToDecimal(rowItem[0].PeriodeNum.ToString());
            }
            else
            {
                return false;
            }

            if (recJournal == null)
            {
                recJournal = new glJournal()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    FiscalYear = DateTime.Now.Year,
                    ProfitCenterCode = ProfitCenter,
                    JournalNo = jNo,
                    JournalType = "Harian",
                    JournalDate = docdate,
                    DocSource = "SP",
                    ReffNo = docno + "," + GetNoBPSForTransfer(docno),
                    ReffDate = docdate,
                    FiscalMonth = recordCPProfitCenter.FiscalMonth,
                    PeriodeNum = periodenum,
                    Periode = periode,
                    PeriodeName = periodename,
                    GLDate = docdate,
                    BalanceType = "1",
                    amountDb = cogs,
                    amountCr = cogs,
                    Status = "1",
                    StatusRecon = "",
                    PostingDate = new DateTime(1900, 1, 1),
                    StatusReverse = "0",
                    ReverseDate = new DateTime(1900, 1, 1),
                    PrintSeq = 1,
                    FSend = false,
                    SendBy = "",
                    SendDate = new DateTime(1900, 1, 1),
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now
                };
                ctx.glJournals.Add(recJournal);
            }

            if (recJournalDtl == null)
            {
                recJournalDtl = new GlJournalDtl()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    FiscalYear = DateTime.Now.Year,
                    JournalNo = jNo,
                    SeqNo = seqNo + 1,
                    AccountNo = oAccount.InventoryAccNo,
                    Description = GetDocApplyToJournal(recordWRS.DNSupplierNo, recordWRS.SupplierCode),
                    JournalType = "Harian",
                    amountDb = cogs,
                    amountCr = decimal.Zero,
                    TypeTrans = "INVENTORY",
                    AccountType = ctx.gnMstAccounts.Find(CompanyCode, BranchCode, oJurnal.AccountNo).AccountType,
                    DocNo = docno,
                    StatusReverse = "0",
                    ReverseDate = new DateTime(1900, 1, 1),
                    FSend = false,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };
                ctx.GlJournalDtls.Add(recJournalDtl);
            }
            seqNo = seqNo + 1;
            if (cogs > 0)
            {
                var accNo = "";
                if ((bool)recordWRS.isLocked)
                {
                    if (oCompanyAccount.Count == 0)
                    {
                        return false;
                    }
                    string compTo = oCompanyAccount[0].CompanyCodeTo.ToString();
                    var dtlComp = ctx.spMstCompanyAccountdtls.Find(CompanyCode, compTo, typeOfGoods);
                    if (dtlComp != null)
                    {
                        accNo = dtlComp.InterCompanyAccNoTo.ToString();
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    accNo = oSenderAccount.InTransitAccNo;
                }

                oJurnal = new GLInterface()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    DocNo = docno,
                    SeqNo = seqNo + 1,
                    DocDate = docdate,
                    ProfitCenterCode = ProfitCenter,
                    AccDate = DateTime.Now,
                    AccountNo = accNo,
                    JournalCode = "SPAREPART",
                    TypeJournal = "INCOMING",
                    ApplyTo = recordWRS.DNSupplierNo,
                    AmountDb = cogs,
                    AmountCr = decimal.Zero,
                    TypeTrans = "INTRANSIT",
                    BatchNo = string.Empty,
                    BatchDate = new DateTime(1900, 1, 1),
                    StatusFlag = "3",
                    CreateBy = CurrentUser.UserId,
                    CreateDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now
                };
                ctx.GLInterfaces.Add(oJurnal);

                var accNodtl = "";
                var AccountTypedtl = "";
                if ((bool)recordWRS.isLocked)
                {
                    if (oCompanyAccount.Count == 0)
                    { return false; }

                    string compTo = oCompanyAccount[0].CompanyCodeTo.ToString();
                    var dtlComp = ctx.spMstCompanyAccountdtls.Find(CompanyCode, compTo, typeOfGoods);

                    if (dtlComp != null)
                    {
                        accNodtl = dtlComp.InterCompanyAccNoTo.ToString();
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    accNodtl = oJurnal.AccountNo;
                }

                if ((bool)recordWRS.isLocked)
                {
                    AccountTypedtl = ctx.gnMstAccounts.Find(CompanyCode, BranchCode, oJurnal.AccountNo).AccountType;
                }
                else
                {
                    AccountTypedtl = ctx.gnMstAccounts.Find(CompanyCode, recordWRS.SupplierCode, oJurnal.AccountNo).AccountType;
                }

                recJournalDtl = new GlJournalDtl()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    FiscalYear = DateTime.Now.Year,
                    JournalNo = jNo,
                    SeqNo = seqNo + 1,
                    AccountNo = accNodtl,
                    Description = GetDocApplyToJournal(recordWRS.DNSupplierNo, recordWRS.SupplierCode),
                    JournalType = "Harian",
                    amountDb = decimal.Zero,
                    amountCr = cogs,
                    TypeTrans = "INTRANSIT",
                    AccountType = AccountTypedtl,
                    DocNo = docno,
                    StatusReverse = "0",
                    ReverseDate = new DateTime(1900, 1, 1),
                    FSend = false,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };
                ctx.GlJournalDtls.Add(recJournalDtl);
            }
            else
            {
                return true;
            }
            return true;


            //if (cogs > 0) {
            //    oJurnal.SeqNo = seqNo += 1;
            //    oJurnal.AccountNo = oAccount.InventoryAccNo;
            //    oJurnal.AmountDb = cogs;
            //    oJurnal.AmountCr = decimal.Zero;
            //    oJurnal.TypeTrans = "INVENTORY";

            //    recJournalDtl.SeqNo = oJurnal.SeqNo;
            //    recJournalDtl.AccountNo = oJurnal.AccountNo;
            //    recJournalDtl.amountDb = cogs;
            //    recJournalDtl.amountCr = decimal.Zero;
            //    recJournalDtl.TypeTrans = "INVENTORY";
            //    recJournalDtl.AccountType = ctx.gnMstAccounts.Find(CompanyCode, BranchCode, oJurnal.AccountNo).AccountType;

            //    Generate(docno, docdate, recordWRS.DNSupplierNo);

            //    // Insert COGS Journal
            //    oJurnal.SeqNo = seqNo += 1;

            //    if ((bool)recordWRS.isLocked)
            //    {
            //        if (oCompanyAccount.Count == 0) {
            //            return false;
            //        }
            //        string compTo = oCompanyAccount[0].CompanyCodeTo.ToString();
            //        var dtlComp = ctx.spMstCompanyAccountdtls.Find(CompanyCode, compTo, typeOfGoods);
            //        if (dtlComp != null)
            //        {
            //            oJurnal.AccountNo = dtlComp.InterCompanyAccNoTo.ToString();
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //    else {
            //        oJurnal.AccountNo = oSenderAccount.InTransitAccNo;
            //    }
            //    recJournalDtl.amountDb = decimal.Zero;
            //    recJournalDtl.amountCr = cogs;
            //    recJournalDtl.TypeTrans = "INTRANSIT";
            //    recJournalDtl.SeqNo = oJurnal.SeqNo;

            //    if ((bool)recordWRS.isLocked)
            //    {
            //        if (oCompanyAccount.Count == 0)
            //        { return false; }

            //        string compTo = oCompanyAccount[0].CompanyCodeTo.ToString();
            //        var dtlComp = ctx.spMstCompanyAccountdtls.Find(CompanyCode, compTo, typeOfGoods);

            //        if (dtlComp != null)
            //        {
            //            recJournalDtl.AccountNo = dtlComp.InterCompanyAccNoTo.ToString();
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //    else {
            //        recJournalDtl.AccountNo = oJurnal.AccountNo;
            //    }

            //    if ((bool)recordWRS.isLocked)
            //    {
            //        recJournalDtl.AccountType = ctx.gnMstAccounts.Find(CompanyCode, BranchCode, oJurnal.AccountNo).AccountType;
            //    }
            //    else
            //    {
            //        recJournalDtl.AccountType = ctx.gnMstAccounts.Find(CompanyCode, recordWRS.SupplierCode, oJurnal.AccountNo).AccountType;
            //    }
            //    Generate(docno, docdate, recordWRS.DNSupplierNo);
            //} else {
            //    return true;
            //}
            //return true;
            //ctx.GLInterfaces.Add(oJurnal);
            //ctx.glJournals.Add(recJournal);
            //ctx.GlJournalDtls.Add(recJournalDtl);
        }

        public void Generate(string docno, DateTime? docdate, string applyto)
        {


            GLInterface oJurnal = new GLInterface();
            GlJournalDtl oJurnalGL = new GlJournalDtl();
            //var jurnal = ctx.GLInterfaces.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == docno).FirstOrDefault();
            oJurnal.CompanyCode = CompanyCode;
            oJurnal.BranchCode = BranchCode;
            oJurnal.ProfitCenterCode = ProfitCenter;
            oJurnal.AccDate = docdate;
            oJurnal.ApplyTo = docno != applyto ? applyto : docno;
            oJurnal.BatchNo = string.Empty;
            oJurnal.BatchDate = DateTime.Parse("1900/01/01");
            oJurnal.StatusFlag = "3";
            oJurnal.CreateBy = CurrentUser.UserId;
            oJurnal.LastUpdateBy = CurrentUser.UserId;
            oJurnal.LastUpdateDate = DateTime.Now;

            oJurnalGL.CreatedBy = CurrentUser.UserId;
            oJurnalGL.CreatedDate = DateTime.Now;

            //ctx.GLInterfaces.Add(oJurnal);
            //ctx.GlJournalDtls.Add(oJurnalGL);

        }

        private string GetDocApplyToJournal(string docNo, string supplierID)
        {
            var query = string.Format(@"
            SELECT TOP 1 journalNo 
            FROM glJournalDtl
            WHERE
            CompanyCode = '{0}'
            AND DocNo = '{2}'
            AND BranchCode = '{1}'
            ", CompanyCode, supplierID, docNo);
            var dt = ctx.Database.SqlQuery<COGSLev>(query).ToList();
            string result = "";

            if (dt.Count > 0)
            {
                result = dt[0].journalNo.ToString();
            }
            return result;

        }

        private List<FISTCAL> SelectbyPeriodeGL(string fiscalYear, string fiscalMonth, DateTime? docDate)
        {

            var query = string.Format(@"
            SELECT  TOP 1 CONVERT(varchar, FiscalYear) + RIGHT('00' + CONVERT(varchar, PeriodeNum), 2) AS Periode, PeriodeNum, FiscalYear, FiscalMonth, PeriodeName
            FROM gnMstPeriode WHERE FiscalStatus = 1 AND CompanyCode = '{0}'
            AND BranchCode = '{1}' AND FiscalYear = '{2}'
            AND FiscalMonth = '{3}' AND StatusSparepart = 1
            AND (MONTH(FromDate) = MONTH('{4}') AND YEAR(FromDate) = YEAR('{4}'))
            ORDER BY FiscalYear ASC, FiscalMonth ASC, PeriodeNum ASC
            ", CompanyCode, BranchCode, fiscalYear, fiscalMonth, docDate);
            var data = ctx.Database.SqlQuery<FISTCAL>(query).ToList();
            return data;
        }

        private string GetNoBPSForTransfer(string docno)
        {
            var query = string.Format(@"
            SELECT DISTINCT DocNo FROM spTrnPbinnDtl
            WHERE
            CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND BinningNo IN(
            SELECT BinningNo FROM spTrnPRcvHdr
            WHERE 
            CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND WRSNo = '{2}')
            ", CompanyCode, BranchCode, docno);
            var data = ctx.Database.SqlQuery<COGSLev>(query).ToList();
            return data[0].DocNo.ToString();
        }

        private object GnMstSupplierClassNew(string supplierCode, string typeOfGoods)
        {
            var recordProf = ctx.MstSupplierProfitCenters.Find(CompanyCode, BranchCode, supplierCode, ProfitCenter);
            if (recordProf == null)
            {
                return null;
            }
            var dt = ctx.GnMstSupplierClasses.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SupplierClass == recordProf.SupplierClass && a.ProfitCenterCode == ProfitCenter && a.TypeOfGoods == typeOfGoods).FirstOrDefault();
            return dt;

        }

        private decimal GetSumCOGS(string WRSNo)
        {
            var query = string.Format(@"
            select 
            isnull (sum(a.PurchasePrice * a.ReceivedQty), 0) as COGS
            from SpTrnPRcvDtl a
            where 
                WrsNo  = '{2}' and
                CompanyCode = '{0}' and
                BranchCode = '{1}'
            ", CompanyCode, BranchCode, WRSNo);
            var result = ctx.Database.SqlQuery<COGSLev>(query).ToList();
            return result[0].COGS;
        }

        public bool MovementLog(string docno, DateTime? docdate, string partno, string whcode, string signcode, string subsigncode, decimal? qty)
        {
            if (qty == null)
            {
                qty = 0;
            }
            if (docdate == null)
            {
                docdate = DateTime.Now;
            }
            bool result = true;
            //var oItems = ctx.spMstItems.Find(CompanyCode, BranchCode, partno);
            var oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, partno);

            var query = string.Format(@"select * from {0}..spMstItems where CompanyCode = '{1}' and BranchCode = '{2}' and PartNo = '{3}'", GetDbMD(), CompanyMD, BranchMD, partno);
            var oItems = ctx.Database.SqlQuery<spMstItem>(query).FirstOrDefault();

            query = string.Format(@"select * from {0}..SpMstItemLoc where CompanyCode ='{1}' and BranchCode ='{2}' and PartNo = '{3}' and WarehouseCode = '{4}'", GetDbMD(), CompanyMD, BranchMD, partno, whcode);
            var oItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(query).FirstOrDefault();

            //SpTrnIMovement oIMovement = new SpTrnIMovement();
            //var oItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcode);

            if (oItemLoc != null && oItemPrice != null && oItems != null)
            {
                var OiMv = ctx.SpTrnIMovements.Find(CompanyCode, BranchCode, docno, docdate, DateTime.Now);
                if (OiMv == null)
                {
                    OiMv = new SpTrnIMovement()
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        DocNo = docno,
                        DocDate = (DateTime)docdate,
                        CreatedDate = DateTime.Now
                    };
                }
                OiMv.WarehouseCode = oItemLoc.WarehouseCode;
                OiMv.LocationCode = oItemLoc.LocationCode;
                OiMv.PartNo = oItemLoc.PartNo;
                OiMv.SignCode = signcode;
                OiMv.SubSignCode = subsigncode;
                OiMv.Qty = (decimal)qty;
                OiMv.Price = oItemPrice.RetailPrice;
                OiMv.CostPrice = oItemPrice.CostPrice;
                OiMv.ABCClass = oItems.ABCClass;
                OiMv.MovingCode = oItems.MovingCode;
                OiMv.ProductType = oItems.ProductType;
                OiMv.PartCategory = oItems.PartCategory;
                OiMv.CreatedBy = CurrentUser.UserId;
                try
                {
                    ctx.SpTrnIMovements.Add(OiMv);
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }

        public int UpdateItemStockLocation(string partno, decimal? receivedQty, int param, string warehouseCode)
        {
            int result = -1;
            var query = string.Format(@"
            UPDATE spMstItemLoc 
            SET OnHand = OnHand + ({3} * {6}), 
                LastUpdateBy = '{2}',
                LastUpdateDate = '{5}'
            WHERE CompanyCode='{0}'
              AND BranchCode='{1}'
              AND partno='{4}' 
              AND WarehouseCode = '{7}'
            ", CompanyCode, BranchCode, CurrentUser.UserId, receivedQty, partno, DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"), param, warehouseCode);
            try
            {
                ctx.Database.ExecuteSqlCommand(query);
                result = 0;
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return result;
        }

        public int UpdateIntransitOrderBalance(string posNo, string suppCode, string partNo, decimal? rcvQty, string wrsNo, DateTime? wrsDate)
        {
            int result = 0;

            try
            {
                var query = string.Format(@"
            SELECT * FROM spTrnPOrderBalance 
            WHERE CompanyCode='{0}'
              AND BranchCode='{1}'
              AND POSNo='{2}'
              AND SupplierCode='{3}'
              AND PartNo='{4}'
            ORDER BY SeqNo
            ", CompanyCode, BranchCode, posNo, suppCode, partNo);
                var exe = ctx.Database.SqlQuery<spTrnPOrderBalance>(query).ToList();

                if (exe.Count > 1)
                {
                    result = UpdateIntransitOrderBalance1(posNo, suppCode, partNo, rcvQty, wrsNo, wrsDate);
                }
                else if (exe.Count == 1)
                {
                    decimal Seq = exe[0].SeqNo;
                    spTrnPOrderBalance oPOrderBalance = new spTrnPOrderBalance();
                    var oPOrder = ctx.spTrnPOrderBalances.Find(CompanyCode, BranchCode, posNo, suppCode, partNo, Seq);

                    var pRcvDtl = string.Format(@"
            SELECT SUM(ReceivedQty) FROM SpTrnPRcvDtl 
            WHERE CompanyCode='{0}'
              AND BranchCode='{1}'
              AND WRSNo='{2}'
              AND PartNo='{3}'
              AND DocNo='{4}'
            ", CompanyCode, BranchCode, wrsNo, partNo, posNo);

                    decimal rec = ctx.Database.SqlQuery<Decimal>(pRcvDtl).FirstOrDefault();

                    oPOrder.InTransit = oPOrder.InTransit - rcvQty;
                    oPOrder.Received = oPOrder.Received + rcvQty;
                    oPOrder.Located = oPOrder.Located + rcvQty;
                    oPOrder.WRSNo = wrsNo;
                    oPOrder.WRSDate = wrsDate;
                    oPOrder.LastUpdateBy = CurrentUser.UserId;
                    oPOrder.LastUpdateDate = DateTime.Now;
                    UpdateOnOrderStock(partNo, 0, 0, rec);
                }
                // ctx.SaveChanges();

            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }


        private int UpdateIntransitOrderBalance1(string PN, string SC, string PartN, decimal? rcvQty, string wrsNo, DateTime? wrsDate)
        {
            int result = 0;
            decimal qty = Convert.ToDecimal(rcvQty);

            var query = string.Format(@"
                SELECT * FROM spTrnPOrderBalance 
                WHERE CompanyCode='{0}'
                  AND BranchCode='{1}'
                  AND POSNo='{2}'
                  AND SupplierCode='{3}'
                  AND PartNo='{4}'
                ORDER BY SeqNo
            ", CompanyCode, BranchCode, PN, SC, PartN);
            ctx.Database.SqlQuery<spTrnPOrderBalance>(query).ToList().ForEach(
                x =>
                {
                    var oPOrderBalance = ctx.spTrnPOrderBalances.Find(CompanyCode, BranchCode, x.POSNo, x.SupplierCode, x.PartNo, x.SeqNo);


                    if (qty < oPOrderBalance.InTransit)
                    {
                        oPOrderBalance.Received = oPOrderBalance.Received + qty;
                        oPOrderBalance.Located = oPOrderBalance.Located + qty;
                        oPOrderBalance.InTransit = oPOrderBalance.InTransit - qty;
                        oPOrderBalance.WRSNo = wrsNo;
                        oPOrderBalance.WRSDate = wrsDate;
                        oPOrderBalance.LastUpdateBy = CurrentUser.UserId;
                        oPOrderBalance.LastUpdateDate = DateTime.Now;
                        UpdateOnOrderStock(oPOrderBalance.PartNo, 0, 0, qty);
                        result++;
                    }
                    else
                    {
                        decimal qtyint = Convert.ToDecimal(oPOrderBalance.InTransit);
                        if (qtyint > 0)
                        {
                            oPOrderBalance.Received = oPOrderBalance.Received + oPOrderBalance.InTransit;
                            oPOrderBalance.Located = oPOrderBalance.Located + oPOrderBalance.InTransit;
                            oPOrderBalance.InTransit = 0;
                            oPOrderBalance.WRSNo = wrsNo;
                            oPOrderBalance.WRSDate = wrsDate;
                            oPOrderBalance.LastUpdateBy = CurrentUser.UserId;
                            oPOrderBalance.LastUpdateDate = DateTime.Now;
                            UpdateOnOrderStock(oPOrderBalance.PartNo, 0, 0, oPOrderBalance.InTransit);
                            qty = qty - qtyint;
                            result++;
                        }
                    }
                });

            return result;
        }

        private bool UpdateOnOrderStock(string partNo, int onorder, int intransit, decimal? received)
        {
            bool result = false;
            // spMstItem oItemDao = new spMstItem();
            var oItem = ctx.spMstItems.Find(CompanyCode, BranchCode, partNo);

            if (oItem != null)
            {
                if (Math.Abs(onorder) > 0)
                {
                    oItem.OnOrder += onorder;
                }

                if (Math.Abs(intransit) > 0)
                {
                    oItem.OnOrder -= intransit;
                    oItem.InTransit += intransit;
                }

                if (received > 0)
                {
                    oItem.InTransit -= received;
                    oItem.OnHand += received;
                }

                if (oItem.OnOrder < 0 || oItem.InTransit < 0)
                {
                    return false;
                }

                oItem.LastUpdateDate = DateTime.Now;
                oItem.LastUpdateBy = CurrentUser.UserId;
                return result;
            }
            return result;
        }

        private decimal AverageCost(decimal invStock, decimal costPrice, decimal? receivedQty, decimal purchasePrice, decimal? discPct)
        {
            if (receivedQty == null)
            {
                receivedQty = 0;
            }
            if (discPct == null)
            {
                discPct = 0;
            }
            return Convert.ToDecimal(((invStock * costPrice) + ((receivedQty * purchasePrice) * (1 - (discPct / 100)))) / (invStock + receivedQty));
        }

        private decimal AverageCostTransferStock(string partno, decimal invStock, decimal? receivedQty, decimal costPrice, decimal purchasePrice)
        {
            if (receivedQty == null)
            {
                receivedQty = 0;
            }
            var dtHead = ctx.OrganizationDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.IsBranch == false).ToList();
            if (dtHead.Count > 1)
            {
                Console.Write(new Exception("Jumlah Holding lebih dari satu"));
            }
            return Convert.ToDecimal(((receivedQty * purchasePrice) + ((invStock * costPrice))) / (invStock + receivedQty));
        }

        private decimal AverageCostOthers(string partno, decimal invStock, decimal? receivedQty, decimal costPrice, decimal purchasePrice)
        {
            if (receivedQty == null)
            {
                receivedQty = 0;
            }
            var dtHead = ctx.OrganizationDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.IsBranch == false).ToList();
            if (dtHead.Count > 1)
            {
                Console.Write(new Exception("Jumlah Holding lebih dari satu"));
            }
            return Convert.ToDecimal(((receivedQty * purchasePrice) + ((invStock * costPrice))) / (invStock + receivedQty));
        }

        public int UpdateItemAvgCost(string partno, decimal costPrice, decimal oldcostPrice)
        {
            int result = 0;
            var query = string.Format(@"
            SELECT
               CompanyCode,BranchCode,PartNo,UpdateDate,
               ISNULL(RetailPrice,0) AS RetailPrice, 
               ISNULL(RetailPriceInclTax,0) AS RetailPriceInclTax, 
               ISNULL(PurchasePrice,0) AS PurchasePrice,
               ISNULL(OldRetailPrice,0) AS OldRetailPrice, 
               ISNULL(OldPurchasePrice,0) AS OldPurchasePrice,
               ISNULL(Discount,0) AS Discount, ISNULL(OldDiscount,0) AS OldDiscount,
               ISNULL(CostPrice,0) AS CostPrice, ISNULL(OldCostPirce,0) AS OldCostPirce,
               CreatedBy,CreatedDate
                FROM
	            spHstItemPrice
                WHERE
	            CompanyCode = '{0}'
	            AND BranchCode = '{1}'
	            AND PartNo = '{2}'
	            AND UpdateDate = (	SELECT
	            MAX(UpdateDate)
                FROM
	            spHstItemPrice
                WHERE
	            CompanyCode = '{0}'
	            AND BranchCode = '{1}'
	            AND PartNo = '{2}')", CompanyCode, BranchCode, partno);
            var drHst = ctx.Database.SqlQuery<QueryCloseWrs2>(query).ToList();

            if (costPrice != oldcostPrice)
            {
                var code = string.Format(@"
                    UPDATE spMstItemPrice 
                    SET CostPrice = {4}, 
                        OldCostPrice = {5},
                        LastUpdateBy = '{2}',
                        LastUpdateDate = '{6}'
                    WHERE CompanyCode='{0}'
                      AND BranchCode='{1}'
                      AND partno='{3}'", CompanyCode, BranchCode, CurrentUser.UserId, partno, costPrice.ToString().Replace(",", "."), oldcostPrice.ToString().Replace(",", "."), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                result = ctx.Database.ExecuteSqlCommand(code);

                if (result >= 0)
                {
                    result = 0;
                    decimal RetailPrice = 0;
                    decimal RetailPriceInclTax = 0;
                    decimal PurchasePrice = 0;
                    decimal OldRetailPrice = 0;
                    decimal OldPurchasePrice = 0;
                    decimal Discount = 0;
                    decimal OldDiscount = 0;
                    if (drHst == null || drHst.Count == 0)
                    {
                        RetailPrice = 0;
                        RetailPriceInclTax = Convert.ToDecimal(0);
                        PurchasePrice = Convert.ToDecimal(0);
                        OldRetailPrice = Convert.ToDecimal(0);
                        OldPurchasePrice = Convert.ToDecimal(0);
                        Discount = Convert.ToDecimal(0);
                        OldDiscount = Convert.ToDecimal(0);
                    }
                    else
                    {
                        RetailPrice = Convert.ToDecimal(drHst[0].RetailPrice);
                        RetailPriceInclTax = Convert.ToDecimal(drHst[0].RetailPriceInclTax);
                        PurchasePrice = Convert.ToDecimal(drHst[0].PurchasePrice);
                        OldRetailPrice = Convert.ToDecimal(drHst[0].OldRetailPrice);
                        OldPurchasePrice = Convert.ToDecimal(drHst[0].OldPurchasePrice);
                        Discount = Convert.ToDecimal(drHst[0].Discount);
                        OldDiscount = Convert.ToDecimal(drHst[0].OldDiscount);

                    }

                    var cd = string.Format(@"
                      INSERT INTO spHstItemPrice
                          (
                           CompanyCode,BranchCode,PartNo,UpdateDate,
                           RetailPrice, RetailPriceInclTax, PurchasePrice,
                           OldRetailPrice, OldPurchasePrice,
                           Discount, OldDiscount,
                           CostPrice,OldCostPirce,CreatedBy,CreatedDate
                           )
                      VALUES (
                          '{0}','{1}','{2}','{3}',
                          {4}, {5}, {6},
                          {7}, {8}, 
                          {9}, {10},
                          '{11}','{12}','{13}','{14}'
                          )", CompanyCode, BranchCode, partno, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), RetailPrice.ToString().Replace(",", "."), RetailPriceInclTax.ToString().Replace(",", "."),
                            PurchasePrice.ToString().Replace(",", "."), OldRetailPrice.ToString().Replace(",", "."), OldPurchasePrice.ToString().Replace(",", "."), Discount.ToString().Replace(",", "."), OldDiscount.ToString().Replace(",", "."), costPrice.ToString().Replace(",", "."), oldcostPrice.ToString().Replace(",", "."),
                            CurrentUser.UserId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    try
                    {
                        result = ctx.Database.ExecuteSqlCommand(cd);
                    }
                    catch
                    {
                        result = -1;
                    }
                }
            }
            return result;
        }

        public JsonResult loadData(string WRSNo, string SupplierCode, string TransType, string BinningNo)
        {
            var query = "";
            if (WRSNo == "")
            {
                query = string.Format(@"
            select isnull(sum(dtl.ReceivedQty), 0) as ReceivedQty 
            from spTrnPBinnHdr hdr with(nolock, nowait)
            left join spTrnPBinnDtl dtl with(nolock, nowait) on dtl.CompanyCode = hdr.CompanyCode 
	            and dtl.BranchCode = hdr.BranchCode 
	            and dtl.BinningNo = hdr.BinningNo 
            where hdr.BinningNo = '{2}'
	            and hdr.CompanyCode = '{0}'
	            and hdr.BranchCode = '{1}'", CompanyCode, BranchCode, BinningNo);
            }
            else
            {
                query = string.Format(@"
            select isnull(sum(dtl.ReceivedQty), 0) as ReceivedQty 
            from spTrnPRcvHdr hdr with(nolock, nowait)
            left join spTrnPRcvDtl dtl with(nolock, nowait) on dtl.CompanyCode = hdr.CompanyCode 
	            and dtl.BranchCode = hdr.BranchCode 
	            and dtl.WRSNo = hdr.WRSNo 
            where hdr.WRSNo = '{2}'
	            and hdr.CompanyCode = '{0}' 
	            and hdr.BranchCode = '{1}'", CompanyCode, BranchCode, WRSNo);
            }


            var str = string.Format(@"
            select LookUpValueName from GnMstLookUpDtl 
            WHERE CompanyCode = '{0}' 
            AND LookUpValue = '{1}' AND CodeID='TTWR'", CompanyCode, TransType);

            var record = ctx.gnMstSupplierViews.Find(CompanyCode, SupplierCode);
            var Tot = ctx.Database.SqlQuery<TotalQty>(query).FirstOrDefault();
            var LoadTabel = getDetail(ctx, CompanyCode, BranchCode, WRSNo);
                //ctx.GridEntryWRSs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.WRSNo == WRSNo);
            var DetailVal = ctx.Database.SqlQuery<TotalQty>(str).FirstOrDefault();
            return Json(new
            {
                Supp = record,
                QtyTot = Tot,
                DatGrid = LoadTabel,
                detVal = DetailVal
            });
        }

        public static IQueryable<EntryWRSModel> getDetail(DataContext ctx, string CompanyCode, string BranchCode, string WRSNo)
        {

            var data = ctx.SpTrnPRcvDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.WRSNo == WRSNo)
                .Join(ctx.SpMstItemInfos, w => new { w.CompanyCode, w.PartNo }, i => new { i.CompanyCode, i.PartNo }, (w, i) => new { w, i })
                .Select(d => new EntryWRSModel
                {
                    CompanyCode = d.w.CompanyCode,
                    BranchCode = d.w.BranchCode,
                    DocNo = d.w.DocNo,
                    PartNo = d.w.PartNo,
                    PurchasePrice = d.w.PurchasePrice,
                    WRSNo = d.w.WRSNo,
                    DiscPct = d.w.DiscPct,
                    ReceivedQty = d.w.ReceivedQty,
                    BoxNo = d.w.BoxNo,
                    NmPart = d.i.PartName
                });

            return data;
        }

    }
}
