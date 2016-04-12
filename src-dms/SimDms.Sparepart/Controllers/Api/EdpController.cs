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
    public class EdpController : BaseController
    {
        private string pesan_error = "";
        private string errPartNo = "";
        private string errException = "";

        public JsonResult Default()
        {
            var lookUpBINS = ctx.LookUpDtls.Find(CompanyCode, "BINS", "CTAX");
            bool PPNVisible = false;
            if (lookUpBINS != null)
            {
                if (lookUpBINS.ParaValue == "1") PPNVisible = true;
            }

            var lookUpTREX = ctx.LookUpDtls.Find(CompanyCode, "TREX", "STATUS");
            bool bExt = false;
            if (lookUpTREX != null)
            {
                if (lookUpTREX.ParaValue == "1") bExt = true;
            }

            var curDate = DateTime.Now.Date.ToString("yyyy-MM-dd");

            return Json(new { PPNVisible = PPNVisible, bExt = bExt, currentDate = curDate });
        }

        // Ditambahkan: left join on a.CompanyCode = b.CompanyCode 
        // dan WHERE a.CompanyCode = @CompanyCode
        public JsonResult LookupSupplierInternal(string noDN)
        {
            var sql = string.Format(@"SELECT a.BranchCode SupplierCode, 
	            (
		            SELECT SupplierName 
		            FROM GnMstSupplier 
		            WHERE CompanyCode = a.CompanyCode 
			            AND SupplierCode = a.BranchCode
	            ) SupplierName,
	            a.LmpNo 
            FROM spTrnSLmpHdr a
            left join 
            (
	            SELECT SupplierCode, DNSupplierNo LmpNo, TypeOfGoods
	            FROM SpTrnPBinnHdr
	            WHERE CompanyCode = '{0}'
		            AND BranchCode = '{1}'
		            AND ReceivingType = 1
		            AND TransType = 3
		            AND Status <> 3
            ) b ON a.BranchCode = b.SupplierCode AND a.LmpNo = b.LmpNo AND a.TypeOfGoods = b.TypeOfGoods
            WHERE a.CustomerCode = '{1}'
	            AND a.TransType = '14'
	            AND b.LmpNo IS NULL 
                AND a.TypeOfGoods = '{2}'
                AND a.LmpNo =  '{2}'", CompanyCode, BranchCode, TypeOfGoods, noDN);

            var rec = ctx.Database.SqlQuery<EdpSupplierInternal>(sql).AsQueryable().toKG();

            return Json(rec);
        }

        public JsonResult LookupSupplier()
        {
            string sql = string.Format(@"
                SELECT 
                    a.CompanyCode, b.BranchCode,
                    a.SupplierCode, a.SupplierName, (a.address1+' '+a.address2+' '+a.address3+' '+a.address4) as Alamat,
                    b.DiscPct as Diskon, (Case a.Status when 0 then 'Tidak Aktif' else 'Aktif' end) as [Status],
                    (SELECT Lookupvaluename FROM gnmstlookupdtl WHERE codeid='PFCN' 
                        AND lookupvalue = b.ProfitCentercode) as ProfitCenterCodeStr
                FROM 
                    gnMstSupplier a
                JOIN gnmstSupplierProfitCenter b ON a.CompanyCode= b.CompanyCode
	                AND a.SupplierCode = b.SupplierCode
                WHERE 
                    a.CompanyCode='{0}'
                    AND b.BranchCode='{1}'
                    AND b.isBlackList=0
                    AND a.status = 1 
                    AND b.ProfitCenterCode='{2}'
                ORDER BY a.SupplierCode", CompanyCode,BranchCode, ProfitCenter);

            var rec = ctx.Database.SqlQuery<SpEdpSupplier>(sql).AsQueryable().toKG();

            return Json(rec);
        }



        public JsonResult GetPINVData(spTrnPBinnHdr model, string isPPN)
        {
            try
            {
                //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

                //cmd.CommandText = "uspfn_GetPINVDData";
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Clear();
                //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                //cmd.Parameters.AddWithValue("@DNNo", model.DNSupplierNo);
                //cmd.Parameters.AddWithValue("@SupplierCode", model.SupplierCode);
                //cmd.Parameters.AddWithValue("@TypeOfGoods", TypeOfGoods);
                //cmd.Parameters.AddWithValue("@PPN", isPPN);
                //SqlDataAdapter daDtl = new SqlDataAdapter(cmd);
                //DataTable dtDtl = new DataTable();
                //daDtl.Fill(dtDtl);

                //var detail = GetJson(dtDtl);
                bool bPPN = false;
                if (isPPN == "1") bPPN = true;

                var detail = PINVDData(model, bPPN);

                decimal decTotalQty = 0;
                decimal decTotalAmt = 0;

                #region ** Error Message **
                string errSub = "Part bermasalah pada setting subtitusi :";
                string errPurchase = "\nPart bermasalah pada setting price :";
                string errMessage = "";
                int counterPrice = 1;
                int counterSub = 1;
                bool subError = false;
                bool priError = false;
                #endregion

                if (detail.Count > 0)
                {
                    foreach (var row in detail)
                    {
                        decTotalQty++;
                        decTotalAmt = decTotalAmt + (
                                         (row.PurchasePrice.Value * row.ReceivedQty.Value)
                                         * (1 - (1 * (row.DiscPct.Value / 100))));
                        string PosNo = row.DocNo;
                        string PartNo = row.PartNo;
                        PORderBalance dtOrdBal = Select4NoPart(CompanyCode, BranchCode, PosNo, PartNo).FirstOrDefault();
                        if (dtOrdBal == null)
                        {
                            //dgvBinningList.Rows[i].DefaultCellStyle.BackColor = Color.IndianRed;
                            //dgvBinningList.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                            errSub += "\n" + counterSub.ToString() + ". " + PartNo;
                            counterSub++;
                            subError = true;
                        }
                        else
                        {
                            //dgvBinningList.Rows[i].DefaultCellStyle.BackColor = Color.White;
                            //dgvBinningList.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                        }

                        spMstItemPrice recPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, PartNo);
                        if (row.PurchasePrice != recPrice.PurchasePrice || recPrice.PurchasePrice == 0 || recPrice == null)
                        {
                            //dgvBinningList.Rows[i].Cells[3].Style.BackColor = Color.Gold;
                            //dgvBinningList.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            errPurchase += "\n" + counterPrice.ToString() + ". " + PartNo;
                            counterPrice++;
                            priError = true;
                        }
                        else
                        {
                            //dgvBinningList.Rows[i].Cells[3].Style.BackColor = Color.White;
                            //dgvBinningList.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        }
                    }
                }
                else
                    return Json(new { success = false, message = "Tidak ada part outstanding untuk nomor dokumen yang terpilih dengan tipe part user anda !" });

                if (!priError)
                    errPurchase += "\n- Tidak ada masalah";

                if (!subError)
                    errSub += "\n- Tidak ada masalah";

                errMessage = errSub + "\n" + errPurchase;

                if (subError || priError)
                {
                    return Json(new { success = false, data = detail, dataDetail = detail.FirstOrDefault(), message = errMessage, priError = "1" });
                }
                else
                    return Json(new { success = true, data = detail, dataDetail = detail.FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult UpdateMasterItemPrice(spTrnPBinnHdr model, string isPPN)
        {
            bool bPPN = false;
            if (isPPN == "1") bPPN = true;

            var detail = PINVDData(model, bPPN);

            foreach (var row in detail)
            {
                var recPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, row.PartNo);
                if (row.PurchasePrice != recPrice.PurchasePrice || recPrice.PurchasePrice == 0 || recPrice == null)
                {
                    UpdateMasterItem drHstPrice = GetLatestRecordByPartNo(CompanyCode, BranchCode, row.PartNo);

                    decimal oldPurchasePrice = (drHstPrice == null) ? 0 : drHstPrice.PurchasePrice;
                    decimal oldCostPrice = (drHstPrice == null) ? 0 : drHstPrice.CostPrice;
                    decimal oldRetailPriceInclTax = (drHstPrice == null) ? 0 : drHstPrice.RetailPriceInclTax;
                    decimal oldRetailPrice = (drHstPrice == null) ? 0 : drHstPrice.RetailPrice;
                    decimal discount = (drHstPrice == null) ? 0 : drHstPrice.Discount;
                    decimal oldDiscount = (drHstPrice == null) ? 0 : drHstPrice.OldDiscount;

                    // Insert for tabel history;
                    spHstItemPrice historyItemPrice = new spHstItemPrice();
                    historyItemPrice.CompanyCode = CompanyCode;
                    historyItemPrice.BranchCode = BranchCode;
                    historyItemPrice.PartNo = row.PartNo;
                    historyItemPrice.UpdateDate = DateTime.Now;
                    historyItemPrice.CreatedBy = CurrentUser.UserId;
                    historyItemPrice.CreatedDate = DateTime.Now;
                    historyItemPrice.CostPrice = (oldCostPrice == 0) ? row.PurchasePrice : oldCostPrice;
                    historyItemPrice.LastPurchaseUpdate = (row.PurchasePrice != oldPurchasePrice) ? DateTime.Now : drHstPrice.LastPurchaseUpdate;
                    historyItemPrice.LastRetailPriceUpdate = (recPrice.RetailPrice != oldRetailPrice) ?
                        DateTime.Now : drHstPrice.LastRetailPriceUpdate;

                    historyItemPrice.LastUpdateBy = CurrentUser.UserId;
                    historyItemPrice.LastUpdateDate = DateTime.Now;
                    historyItemPrice.OldCostPirce = oldCostPrice;
                    historyItemPrice.OldPurchasePrice = oldPurchasePrice;
                    historyItemPrice.OldRetailPrice = oldRetailPrice;
                    historyItemPrice.PurchasePrice = row.PurchasePrice;
                    historyItemPrice.RetailPrice = recPrice.RetailPrice;
                    historyItemPrice.RetailPriceInclTax = recPrice.RetailPriceInclTax;
                    historyItemPrice.Discount = discount;
                    historyItemPrice.OldDiscount = oldDiscount;

                    recPrice.PurchasePrice = row.PurchasePrice;
                    recPrice.LastUpdateBy = CurrentUser.UserId;
                    recPrice.LastUpdateDate = DateTime.Now;

                   ctx.spHstItemPrices.Add(historyItemPrice);
                   ctx.SaveChanges();
                }
            }

            return Json(new { success = true });
        }

        private UpdateMasterItem GetLatestRecordByPartNo(string companyCode, string branchCode, string partNo)
        {
            string cmdText = string.Format(@"
SELECT
	[CompanyCode]
      ,[BranchCode]
      ,[PartNo]
      ,[UpdateDate]
      ,ISNULL([RetailPrice], 0) RetailPrice
      ,ISNULL([RetailPriceInclTax],0) RetailPriceInclTax
      ,ISNULL([PurchasePrice],0) PurchasePrice
      ,ISNULL([CostPrice],0) CostPrice
      ,ISNULL([Discount],0) Discount
      ,ISNULL([OldRetailPrice], 0) OldRetailPrice
      ,ISNULL([OldPurchasePrice],0) OldPurchasePrice
      ,ISNULL([OldCostPirce],0) OldCostPrice
      ,ISNULL([OldDiscount],0) OldDiscount
      ,isnull([LastPurchaseUpdate],getdate()) LastPurchaseUpdate
      ,isnull([LastRetailPriceUpdate],getdate()) LastRetailPriceUpdate
      ,[CreatedBy]
      ,[CreatedDate]
      ,[LastUpdateBy]
      ,[LastUpdateDate]
FROM
	spHstItemPrice
WHERE
	CompanyCode = '{0}'
	AND BranchCode = '{1}'
	AND PartNo = '{2}'
	AND UpdateDate = (SELECT MAX(UpdateDate) FROM spHstItemPrice WHERE CompanyCode = '{0}' AND BranchCode = '{1}' AND PartNo = '{2}')
", companyCode, branchCode, partNo);

            var data = ctx.Database.SqlQuery<UpdateMasterItem>(cmdText).FirstOrDefault();
            
            return data;
        }


        private List<PORderBalance> Select4NoPart(string companyCode, string branchCode, string posNo, string partNo)
        {
            string cmdText = string.Format(@"
                SELECT 
                    a.POSNo, a.PartNo, b.PartName, a.OrderQty, 
                    a.OnOrder, a.Intransit, a.Received,a.DiscPct, a.PurchasePrice, 
                    a.SeqNo, a.SupplierCode, a.OnOrder, a.PartNoOriginal, 
                    a.TypeOfGoods 
                FROM 
                    spTrnPOrderBalance a 
                INNER JOIN spMstItemInfo b
                   ON b.PartNo      = a.PartNo
                  AND b.CompanyCode = a.CompanyCode
                WHERE a.CompanyCode = '{0}'
                  AND a.BranchCode  = '{1}'
                  AND a.PosNo    like '{2}'
                  AND a.PartNo      = '{3}'
                  AND a.TypeOfGoods = '{4}'
                ORDER BY a.POSNo DESC, a.SeqNo", companyCode, branchCode, posNo, partNo, TypeOfGoods);

            var data = ctx.Database.SqlQuery<PORderBalance>(cmdText).ToList();

            return data;
        }

        private List<PINVDData> PINVDData(spTrnPBinnHdr model, bool isPPN)
        {
            var cmdText = "exec uspfn_GetPINVDData @p0,@p1,@p2,@p3,@p4,@p5";
            object[] paramPINVD = { CompanyCode, BranchCode, model.DNSupplierNo, model.SupplierCode, TypeOfGoods, isPPN };

            var data = ctx.Database.SqlQuery<PINVDData>(cmdText, paramPINVD).ToList();
            return data;
        }

        public string getData()
        {
            try
            {
                return GetNewDocumentNoHpp("BNL", DateTime.Now.ToString("yyyyMMdd"));
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                return e.Message;
            }
        }

        public JsonResult Save(spTrnPBinnHdr model, string pil, string CustomerCode, bool isPPN, bool IsExt)
        {
            string msg = "";

            if (IsClosedDocNo(model.BinningNo, ref msg)){
                var dat = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
                return Json(new { success = false, message = msg, isi = dat });
            }

            if (pil == "2" || pil == "3")
            {
                if (pil == "2")
                {
                    if (PINVDData(model, isPPN).Count() < 1)
                    {
                        msg = "Tidak ada detail PINV yang dapat diproses";
                        return Json(new { success = false, message = msg });
                    }

                    if (CheckqtyBinning(model))
                    {
                       msg = "Gagal Simpan Binning. Terdapat part yang sudah pernah di-upload/diterima manual (qty melebihi batas maksimal) !";
                        return Json(new { success = false, message = msg });
                    }
                }
            }

            msg = DateTransValidation(model.BinningDate.Value.Date);
            if (!string.IsNullOrEmpty(msg))
            {
                return Json(new { success = false, message = msg });
            }

            // Cek Part For Transfer Stock External
            if (pil == "3" && IsExt)
            {
                var data = SelectBinningFromTransferStock(CompanyCode, BranchCode
                                    , model.SupplierCode, model.DNSupplierNo);

                string partNo = "";
                var dtPart = new List<BinnFromTransferStock>();
                if (data.Count > 0)
                {
                    partNo = data.FirstOrDefault().PartNo;
                    // Cek MstItems
                    var queryItem = string.Format(@"select * from {0}..spMstItems where CompanyCode = '{1}' and BranchCode = '{2}' and PartNo = '{3}'", GetDbMD(), CompanyMD, BranchMD, partNo);
                    var item = ctx.Database.SqlQuery<spMstItem>(queryItem).FirstOrDefault();

                    if (item == null)
                    {
                        var binn = new BinnFromTransferStock();
                        binn.PartNo = partNo;
                        dtPart.Add(binn);
                        binn = null;
                    }
                    else
                    {
                        // Cek Item Price
                        //spMstItemPrice oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, partNo);
                        var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                            GetDbMD(), CompanyMD, BranchMD, partNo);
                        var itemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();
                        
                        if (itemPrice == null){
                            var binn = new BinnFromTransferStock();
                            binn.PartNo = partNo;
                            dtPart.Add(binn);
                            binn = null;
                        }
                        else
                        {
                            // Cek Item Location
                            var queryItemLoc = string.Format(@"select * from {0}..SpMstItemLoc where CompanyCode ='{1}' and BranchCode ='{2}' and PartNo = '{3}' and WarehouseCode = '{4}'", GetDbMD(), CompanyMD, BranchMD, partNo, WarehouseMD);
                            var itemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(queryItemLoc).FirstOrDefault();

                            if (itemLoc == null){
                                var binn = new BinnFromTransferStock();
                                binn.PartNo = partNo;
                                dtPart.Add(binn);
                                binn = null;
                            }
                        }
                    }
                }

                if (dtPart.Count() > 0)
                {
                    return Json(new {success = false, message= "Ada Master Item, Master Item Price, Master Item Location yang belum terdaftar ", partExist = false, partData = dtPart });
                }
            }

            using (ctx)
            {
                using (var trx = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        if (string.IsNullOrEmpty(model.ReferenceNo))
                        {
                            model.ReferenceNo = "";
                        }
                        var TotItem = "";
                        var TotBinningAmt = "";
                        if (!model.TotItem.HasValue || model.TotItem.ToString() == "")
                        {
                            TotItem = "0";
                        }
                        else
                        {
                            TotItem = model.TotItem.ToString();
                        }

                        if (!model.TotBinningAmt.HasValue || model.TotBinningAmt.ToString() == "")
                        {
                            TotBinningAmt = "0";
                        }
                        else
                        {
                            TotBinningAmt = model.TotBinningAmt.ToString();
                        }

                        var record = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
                        if (record == null)
                        {
                            if (string.IsNullOrEmpty(model.BinningNo))
                                model.BinningNo = GetNewDocumentNo("BNL", DateTime.Now.Date);
                            else
                                model.BinningNo = model.BinningNo;

                            if (model.BinningNo.Length > 20)
                            {
                                return Json(new { success = false, message = model.BinningNo });
                            }

                            record = new spTrnPBinnHdr
                            {
                                CompanyCode = CompanyCode,
                                BranchCode = BranchCode,
                                BinningNo = model.BinningNo,
                                CreatedBy = CurrentUser.UserId,
                                CreatedDate = DateTime.Now,
                                TypeOfGoods = TypeOfGoods

                            };

                            ctx.spTrnPBinnHdrs.Add(record);
                        }
                        //insert to header
                        ctx.SaveChanges();

                        if (pil == "3")
                        {
                            if (IsExt)
                                model.isLocked = true;
                        }
                        record.BinningDate = model.BinningDate;
                        record.ReceivingType = pil;
                        record.TransType = model.TransType;
                        record.DNSupplierNo = model.DNSupplierNo;
                        record.DNSupplierDate = model.DNSupplierDate;

                        if (pil == "1" || pil == "2")
                        {
                            var  oGnMstLookUpDtl = ctx.LookUpDtls.Find(CompanyCode, "BINS", "CTAX");
                            if (oGnMstLookUpDtl == null)
                                model.isLocked = false;
                            else
                            {
                                if (oGnMstLookUpDtl.ParaValue == "0")
                                    model.isLocked = false;
                                else if (oGnMstLookUpDtl.ParaValue == "1")
                                    model.isLocked = true;
                            }
                        }

                        if (model.TransType == "2")
                        {
                            record.SupplierCode = CustomerCode;
                        }
                        else
                        {
                            record.SupplierCode = model.SupplierCode;
                        }
                        record.ReferenceNo = model.ReferenceNo;
                        record.ReferenceDate = model.ReferenceDate;
                        if (pil == "1")
                        {
                            record.TotItem = Convert.ToDecimal(TotItem);
                            record.TotBinningAmt = Convert.ToDecimal(TotBinningAmt);
                        }
                        else if (pil == "2")
                        {
                            var data = PINVDData(model, isPPN);

                            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                            //cmd.CommandText = "uspfn_GetPINVDData";
                            //cmd.CommandType = CommandType.StoredProcedure;
                            //cmd.Parameters.Clear();
                            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                            //cmd.Parameters.AddWithValue("@DNNo", model.DNSupplierNo);
                            //cmd.Parameters.AddWithValue("@SupplierCode", model.SupplierCode);
                            //cmd.Parameters.AddWithValue("@TypeOfGoods", TypeOfGoods);
                            //cmd.Parameters.AddWithValue("@PPN", isPPN);
                            //SqlDataAdapter da = new SqlDataAdapter(cmd);
                            //DataTable dt = new DataTable();
                            //da.Fill(dt);

                            //for (int i = 0; i < data.Count(); i++)
                            foreach(var dt in data)
                            {
                                //var loc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, dt.Rows[i]["PartNo"].ToString(), "00");
                                //var oItem = ctx.spMstItems.Find(CompanyCode, BranchCode, dt.Rows[i]["PartNo"].ToString());

                                var partno = dt.PartNo;//dt.Rows[i]["PartNo"].ToString();

                                var queryItemLoc = string.Format(@"select * from {0}..SpMstItemLoc where CompanyCode ='{1}' and BranchCode ='{2}' and PartNo = '{3}' and WarehouseCode = '{4}'", GetDbMD(), CompanyMD, BranchMD, partno, WarehouseMD);
                                var loc = ctx.Database.SqlQuery<SpMstItemLoc>(queryItemLoc).FirstOrDefault();

                                var queryItem = string.Format(@"select * from {0}..spMstItems where CompanyCode = '{1}' and BranchCode = '{2}' and PartNo = '{3}'", GetDbMD(), CompanyMD, BranchMD, partno);
                                var oItem = ctx.Database.SqlQuery<spMstItem>(queryItem).FirstOrDefault();

                                var binnDtls = new SpTrnPBinnDtl
                                {
                                    CompanyCode = CompanyCode,
                                    BranchCode = BranchCode,
                                    BinningNo = model.BinningNo,
                                    DocNo = dt.DocNo,
                                    CostPrice = dt.CostPrice,
                                    DiscPct = dt.DiscPct,
                                    PartNo = dt.PartNo,
                                    ProductType = dt.ProductType,
                                    PurchasePrice =dt.PurchasePrice,
                                    ReceivedQty = dt.ReceivedQty,
                                    WarehouseCode = "00",
                                    LocationCode = (loc == null) ? "" : loc.LocationCode,
                                    ABCClass = (oItem == null) ? "" : oItem.ABCClass,
                                    MovingCode = (oItem == null) ? "" : oItem.MovingCode,
                                    PartCategory = (oItem == null) ? "" : oItem.PartCategory,
                                    BoxNo = dt.BoxNo,
                                    DocDate = new DateTime(1900, 1, 1),
                                    CreatedBy = CurrentUser.UserId,
                                    CreatedDate = DateTime.Now,
                                    LastUpdateBy = CurrentUser.UserId,
                                    LastUpdateDate = DateTime.Now
                                };
                                ctx.SpTrnPBinnDtls.Add(binnDtls);

                                //ADDED by Benedict 01-Mar-15
                                var query = @"
                        UPDATE SpUtlPINVDDtl SET status='2'
                            WHERE CompanyCode=@p0 AND
                            BranchCode=@p1 AND
                            DeliveryNo=@p2 AND
                            OrderNo=@p3 AND
                            PartNoShip=@p4
                        ";
                                try
                                {
                                    ctx.Database.ExecuteSqlCommand(query, CompanyCode, BranchCode,
                                        record.DNSupplierNo, binnDtls.DocNo, binnDtls.PartNo);
                                }
                                catch (Exception ex)
                                {
                                    // Rollback Transaction
                                    trx.Rollback();

                                    return Json(new { success = false, message = "Gagal menyimpan data", error = ex.Message });
                                }
                                // ** -- **
                            }

                        }
                        else if (pil == "3")
                        {
                            decimal totbin = 0;
                            var totitembin = 0;
                            var partN = "";
                            ctx.SpLoadDetail_TranStocks.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.LampiranNo == model.DNSupplierNo && a.DealerCode == model.SupplierCode).ToList().ForEach(v =>
                            {
                                if (v.PartNo != partN)
                                {
                                    totitembin = totitembin + 1;
                                }
                                decimal tot = Convert.ToDecimal((v.ReceivedQty * v.PurchasePrice) - ((v.ReceivedQty * v.PurchasePrice) * v.DiscPct / 100));
                                totbin = Decimal.Round(totbin + tot);
                                partN = v.PartNo;

                            });
                            record.TotItem = totitembin;
                            record.TotBinningAmt = totbin;
                        }

                        record.Status = "0";
                        record.PrintSeq = 0;
                        record.LastUpdateBy = CurrentUser.UserId;
                        record.LastUpdateDate = DateTime.Now;
                        record.isLocked = model.isLocked;

                        Helpers.ReplaceNullable(record);

                        ctx.SaveChanges();
                        if (pil == "2")
                        {
                            string sql = string.Format(@"SELECT 
                                COUNT(ReceivedQty) TotItem 
                                , SUM((PurchasePrice * ReceivedQty) - ROUND((PurchasePrice * ReceivedQty * DiscPct / 100), 0)) TotBinningAmt
                        FROM  
                            SpTrnPBinnDtl WITH(NOLOCK, NOWAIT)
                        WHERE
                            CompanyCode = '{0}'
                                AND BranchCode = '{1}'
                                AND BinningNo = '{2}'
                        GROUP BY 
                                BinningNo", CompanyCode, BranchCode, record.BinningNo);
                            var totalItem = ctx.Database.SqlQuery<TotalItem>(sql).FirstOrDefault();

                            record.TotItem = totalItem.TotItem;
                            record.TotBinningAmt = totalItem.TotBinningAmt;
                            ctx.SaveChanges();
                        }
                        if (pil == "3")
                        {
                            TranStock_Detail(model.BinningNo, model.DNSupplierNo, model.SupplierCode);
                        }

                        // Commit transaction
                        trx.Commit();
                        var dat = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
                        return Json(new { success = true, isi = dat });
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction
                        trx.Rollback();
                        return Json(new { success = false, message = "Gagal menyimpan data", error = ex.Message });
                    }
                }
            }


            ////////////////

        }

        public JsonResult Save_OLD(spTrnPBinnHdr model, string pil, string CustomerCode, bool isPPN)
        {
            using (ctx)
            {
                using (var trx = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        if (string.IsNullOrEmpty(model.ReferenceNo))
                        {
                            model.ReferenceNo = "";
                        }
                        var TotItem = "";
                        var TotBinningAmt = "";
                        if (!model.TotItem.HasValue || model.TotItem.ToString() == "")
                        {
                            TotItem = "0";
                        }
                        else
                        {
                            TotItem = model.TotItem.ToString();
                        }

                        if (!model.TotBinningAmt.HasValue || model.TotBinningAmt.ToString() == "")
                        {
                            TotBinningAmt = "0";
                        }
                        else
                        {
                            TotBinningAmt = model.TotBinningAmt.ToString();
                        }
                        var record = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
                        if (record == null)
                        {
                            if (string.IsNullOrEmpty(model.BinningNo))
                                model.BinningNo = GetNewDocumentNo("BNL", DateTime.Now);
                            else
                                model.BinningNo = model.BinningNo;

                            if (model.BinningNo.Length > 20)
                            {
                                return Json(new { success = false, message = model.BinningNo });
                            }

                            record = new spTrnPBinnHdr
                            {
                                CompanyCode = CompanyCode,
                                BranchCode = BranchCode,
                                BinningNo = model.BinningNo,
                                CreatedBy = CurrentUser.UserId,
                                CreatedDate = DateTime.Now,
                                TypeOfGoods = TypeOfGoods

                            };
                            ctx.spTrnPBinnHdrs.Add(record);
                        }
                        //insert to header
                        ctx.SaveChanges();

                        record.BinningDate = model.BinningDate;
                        record.ReceivingType = pil;
                        record.TransType = model.TransType;
                        record.DNSupplierNo = model.DNSupplierNo;
                        record.DNSupplierDate = model.DNSupplierDate;
                        if (model.TransType == "2")
                        {
                            record.SupplierCode = CustomerCode;
                        }
                        else
                        {
                            record.SupplierCode = model.SupplierCode;
                        }
                        record.ReferenceNo = model.ReferenceNo;
                        record.ReferenceDate = model.ReferenceDate;
                        if (pil == "1")
                        {
                            record.TotItem = Convert.ToDecimal(TotItem);
                            record.TotBinningAmt = Convert.ToDecimal(TotBinningAmt);
                        }
                        else if (pil == "2")
                        {
                            var data = PINVDData(model, isPPN);

                            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                            //cmd.CommandText = "uspfn_GetPINVDData";
                            //cmd.CommandType = CommandType.StoredProcedure;
                            //cmd.Parameters.Clear();
                            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                            //cmd.Parameters.AddWithValue("@DNNo", model.DNSupplierNo);
                            //cmd.Parameters.AddWithValue("@SupplierCode", model.SupplierCode);
                            //cmd.Parameters.AddWithValue("@TypeOfGoods", TypeOfGoods);
                            //cmd.Parameters.AddWithValue("@PPN", isPPN);
                            //SqlDataAdapter da = new SqlDataAdapter(cmd);
                            //DataTable dt = new DataTable();
                            //da.Fill(dt);

                            //for (int i = 0; i < data.Count(); i++)
                            foreach(var dt in data)
                            {
                                //var loc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, dt.Rows[i]["PartNo"].ToString(), "00");
                                //var oItem = ctx.spMstItems.Find(CompanyCode, BranchCode, dt.Rows[i]["PartNo"].ToString());

                                var partno = dt.PartNo;//dt.Rows[i]["PartNo"].ToString();

                                var queryItemLoc = string.Format(@"select * from {0}..SpMstItemLoc where CompanyCode ='{1}' and BranchCode ='{2}' and PartNo = '{3}' and WarehouseCode = '{4}'", GetDbMD(), CompanyMD, BranchMD, partno, WarehouseMD);
                                var loc = ctx.Database.SqlQuery<SpMstItemLoc>(queryItemLoc).FirstOrDefault();

                                var queryItem = string.Format(@"select * from {0}..spMstItems where CompanyCode = '{1}' and BranchCode = '{2}' and PartNo = '{3}'", GetDbMD(), CompanyMD, BranchMD, partno);
                                var oItem = ctx.Database.SqlQuery<spMstItem>(queryItem).FirstOrDefault();

                                var binnDtls = new SpTrnPBinnDtl
                                {
                                    CompanyCode = CompanyCode,
                                    BranchCode = BranchCode,
                                    BinningNo = model.BinningNo,
                                    DocNo = dt.DocNo,
                                    CostPrice = dt.CostPrice,
                                    DiscPct = dt.DiscPct,
                                    PartNo = dt.PartNo,
                                    ProductType = dt.ProductType,
                                    PurchasePrice =dt.PurchasePrice,
                                    ReceivedQty = dt.ReceivedQty,
                                    WarehouseCode = "00",
                                    LocationCode = (loc == null) ? "" : loc.LocationCode,
                                    ABCClass = (oItem == null) ? "" : oItem.ABCClass,
                                    MovingCode = (oItem == null) ? "" : oItem.MovingCode,
                                    PartCategory = (oItem == null) ? "" : oItem.PartCategory,
                                    BoxNo = dt.BoxNo,
                                    DocDate = new DateTime(1900, 1, 1),
                                    CreatedBy = CurrentUser.UserId,
                                    CreatedDate = DateTime.Now,
                                    LastUpdateBy = CurrentUser.UserId,
                                    LastUpdateDate = DateTime.Now
                                };
                                ctx.SpTrnPBinnDtls.Add(binnDtls);

                                //ADDED by Benedict 01-Mar-15
                                var query = @"
                        UPDATE SpUtlPINVDDtl SET status='2'
                            WHERE CompanyCode=@p0 AND
                            BranchCode=@p1 AND
                            DeliveryNo=@p2 AND
                            OrderNo=@p3 AND
                            PartNoShip=@p4
                        ";
                                try
                                {
                                    ctx.Database.ExecuteSqlCommand(query, CompanyCode, BranchCode,
                                        record.DNSupplierNo, binnDtls.DocNo, binnDtls.PartNo);
                                }
                                catch (Exception ex)
                                {
                                    // Rollback Transaction
                                    trx.Rollback();

                                    return Json(new { success = false, message = "Gagal menyimpan data", error = ex.Message });
                                }
                                // ** -- **
                            }

                        }
                        else if (pil == "3")
                        {
                            decimal totbin = 0;
                            var totitembin = 0;
                            var partN = "";
                            ctx.SpLoadDetail_TranStocks.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.LampiranNo == model.DNSupplierNo && a.DealerCode == model.SupplierCode).ToList().ForEach(v =>
                            {
                                if (v.PartNo != partN)
                                {
                                    totitembin = totitembin + 1;
                                }
                                decimal tot = Convert.ToDecimal((v.ReceivedQty * v.PurchasePrice) - ((v.ReceivedQty * v.PurchasePrice) * v.DiscPct / 100));
                                totbin = Decimal.Round(totbin + tot);
                                partN = v.PartNo;

                            });
                            record.TotItem = totitembin;
                            record.TotBinningAmt = totbin;
                        }

                        record.Status = "0";
                        record.PrintSeq = 0;
                        record.LastUpdateBy = CurrentUser.UserId;
                        record.LastUpdateDate = DateTime.Now;
                        record.isLocked = false;

                        ctx.SaveChanges();
                        if (pil == "2")
                        {
                            string sql = string.Format(@"SELECT 
                                COUNT(ReceivedQty) TotItem 
                                , SUM((PurchasePrice * ReceivedQty) - ROUND((PurchasePrice * ReceivedQty * DiscPct / 100), 0)) TotBinningAmt
                        FROM  
                            SpTrnPBinnDtl WITH(NOLOCK, NOWAIT)
                        WHERE
                            CompanyCode = '{0}'
                                AND BranchCode = '{1}'
                                AND BinningNo = '{2}'
                        GROUP BY 
                                BinningNo", CompanyCode, BranchCode, record.BinningNo);
                            var totalItem = ctx.Database.SqlQuery<TotalItem>(sql).FirstOrDefault();

                            record.TotItem = totalItem.TotItem;
                            record.TotBinningAmt = totalItem.TotBinningAmt;
                            ctx.SaveChanges();
                        }
                        if (pil == "3")
                        {
                            TranStock_Detail(model.BinningNo, model.DNSupplierNo, model.SupplierCode);
                        }

                        // Commit transaction
                        trx.Commit();
                        var dat = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
                        return Json(new { success = true, isi = dat });
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction
                        trx.Rollback();
                        return Json(new { success = false, message = "Gagal menyimpan data", error = ex.Message });
                    }
                }
            }
        }

        public JsonResult TranStock_Detail(string BinnNo, string DNSupplierNo, string SupplierCode)
        {
            var record = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, BinnNo);
            ctx.SpLoadDetail_TranStocks.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.LampiranNo == DNSupplierNo && a.DealerCode == SupplierCode).ToList().ForEach(x =>
            {
                var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                              GetDbMD(), CompanyMD, BranchMD, x.PartNo);

                var sqlItemLoc = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                                 GetDbMD(), CompanyMD, BranchMD, x.PartNo, WarehouseMD);

                var oItem = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();
                var loc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();
                var spTrnPPOSHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, x.DocNo);
                var binnDtls = ctx.SpTrnPBinnDtls.Find(CompanyCode, BranchCode, BinnNo, x.PartNo, x.DocNo, x.BoxNo);
                if (binnDtls == null)
                {
                    decimal cost = 0;
                    if (record.TransType == "5" && record.ReceivingType == "1")
                        cost = Convert.ToDecimal(x.PurchasePrice);
                    else
                    {
                        var oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, x.PartNo);
                        cost = (oItemPrice == null) ? 0 : Convert.ToDecimal(oItemPrice.CostPrice);
                    }
                    binnDtls = new SpTrnPBinnDtl
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        BinningNo = BinnNo,
                        PartNo = x.PartNo,
                        DocNo = x.DocNo,
                        WarehouseCode = "00",
                        LocationCode = (loc == null) ? "" : loc.LocationCode,
                        ABCClass = (oItem == null) ? "" : oItem.ABCClass,
                        MovingCode = (oItem == null) ? "" : oItem.MovingCode,
                        ProductType = (oItem == null) ? "" : oItem.ProductType,
                        PartCategory = (oItem == null) ? "" : oItem.PartCategory,
                        BoxNo = x.BoxNo,
                        ReceivedQty = Convert.ToDecimal(x.ReceivedQty),
                        PurchasePrice = Convert.ToDecimal(x.PurchasePrice),
                        CostPrice = cost,
                        DiscPct = Convert.ToDecimal(x.DiscPct),

                        DocDate = (spTrnPPOSHdr == null) ? DateTime.Now : spTrnPPOSHdr.POSDate,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now,
                        LastUpdateBy = CurrentUser.UserId,
                        LastUpdateDate = DateTime.Now

                    };
                    ctx.SpTrnPBinnDtls.Add(binnDtls);
                    ctx.SaveChanges();
                }
            });

            var UtlStock = ctx.SpUtlStockTrfHdrs.Find(CompanyCode, BranchCode, SupplierCode, DNSupplierNo);
            if (UtlStock != null)
            {
                UtlStock.Status = "2";
                UtlStock.BinningNo = record.BinningNo;
                UtlStock.BinningDate = record.BinningDate;
                UtlStock.LastUpdateBy = CurrentUser.UserId;
                UtlStock.LastUpdateDate = DateTime.Now;
            }

            ctx.SaveChanges();
            return Json(new { success = true });

        }

        public JsonResult SaveRecordDetail(SaveSpTrnPBinnDtl model, string pil)
        {
            using (var tran = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    //  decimal.Round(123, 032);
                    decimal? q = 0;

                    if (model.BoxNo == null)
                    {
                        model.BoxNo = "";
                    }
                    decimal? totqty = 0;

                    var updateHdr = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
                    if (updateHdr == null)
                    {
                        throw new Exception("<b> No. BN tidak ditemukan.</b> Simpan Entry Draft Penerimaan (Binning) terlebih dahulu.");
                    }
                    
                    var checkpartno = ctx.SpTrnPBinnDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == model.BinningNo && a.PartNo == model.PartNo && a.DocNo == model.DocNo).SingleOrDefault();

                    spTrnPBinnHdr record = new spTrnPBinnHdr();
                    var binnDtls = ctx.SpTrnPBinnDtls.Find(CompanyCode, BranchCode, model.BinningNo, model.PartNo, model.DocNo, model.BoxNo);
                    if (binnDtls == null)
                    {
                        binnDtls = new SpTrnPBinnDtl
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            BinningNo = model.BinningNo,
                            PartNo = model.PartNo,
                            DocNo = model.DocNo,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,

                        };
                        binnDtls.WarehouseCode = WarehouseMD;
                        ctx.SpTrnPBinnDtls.Add(binnDtls);

                        ctx.spTrnPBinnHdrs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.Status == "1" || a.Status == "0").ToList().ForEach(x =>
                        {
                            ctx.SpTrnPBinnDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == x.BinningNo && a.DocNo == model.DocNo && a.PartNo == model.PartNo).ToList().ForEach(y =>
                            {
                                q += y.ReceivedQty;
                            });
                        });


                        var onorderqty = new spTrnPOrderBalance();

                        try
                        {
                            onorderqty = ctx.spTrnPOrderBalances.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.POSNo == model.DocNo && a.PartNo == model.PartNo).SingleOrDefault();
                        }
                        catch
                        {
                            throw new Exception("Terjadi kesalahan saat Pengecekan Qty Order");
                        }

                        if (onorderqty.OnOrder < model.ReceivedQty)
                        {
                            return Json(new { success = false, message = "Quantity penerimaan tidak boleh lebih besar dari maksimum on order" });
                        }


                        if (checkpartno != null)
                        {
                            updateHdr.TotItem = model.TotItem;
                        }
                        else
                        {
                            updateHdr.TotItem = updateHdr.TotItem + 1;
                        }

                        decimal total = Convert.ToDecimal((model.ReceivedQty * model.PurchasePrice) - ((model.ReceivedQty * model.PurchasePrice) * model.DiscPct / 100));
                        updateHdr.TotBinningAmt = updateHdr.TotBinningAmt + decimal.Round(total);
                    }
                    else
                    {

                        ctx.spTrnPBinnHdrs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.Status == "1" || a.Status == "0").ToList().ForEach(x =>
                        {
                            ctx.SpTrnPBinnDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == x.BinningNo && a.DocNo == model.DocNo && a.PartNo != model.PartNo).ToList().ForEach(y =>
                            {
                                q += 0;
                            });
                        });


                        var onorderqty = new spTrnPOrderBalance();
                        try
                        {
                            onorderqty = ctx.spTrnPOrderBalances.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.POSNo == model.DocNo && a.PartNo == model.PartNo).SingleOrDefault();
                        }
                        catch { }

                        if (onorderqty.OnOrder < model.ReceivedQty)
                        {
                            return Json(new { success = false, message = "Quantity penerimaan tidak boleh lebih besar dari maksimum on order" });
                        }

                        updateHdr.TotItem = model.TotItem;
                        //      updateHdr.TotBinningAmt = model.TotBinningAmt;

                        decimal? jum = 0;
                        ctx.SpTrnPBinnDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == model.BinningNo).ToList().ForEach(v =>
                        {
                            if (v.DocNo != model.DocNo || v.PartNo != model.PartNo)
                            {
                                decimal tot = Convert.ToDecimal((v.ReceivedQty * v.PurchasePrice) - ((v.ReceivedQty * v.PurchasePrice) * v.DiscPct / 100));
                                jum += decimal.Round(tot);
                            }

                        });

                        decimal totalamt = Convert.ToDecimal((model.ReceivedQty * model.PurchasePrice) - ((model.ReceivedQty * model.PurchasePrice) * model.DiscPct / 100));
                        updateHdr.TotBinningAmt = jum + decimal.Round(totalamt);
                    }


                    if (pil == "4")
                    {
                        binnDtls.DocDate = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, model.DocNo).POSDate;
                    }
                    else if (pil == "2")
                    {
                        binnDtls.DocDate = ctx.SpTrnSBPSFHdrs.Find(CompanyCode, BranchCode, model.DocNo).BPSFDate;
                    }
                    else
                    {
                        binnDtls.DocDate = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, model.DocNo).POSDate;
                        //binnDtls.DocDate = model.DocDate;
                    }

                    var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                                  GetDbMD(), CompanyMD, BranchMD, model.PartNo);

                    var sqlItemLoc = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                                     GetDbMD(), CompanyMD, BranchMD, model.PartNo, WarehouseMD);

                    var oItem = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();
                    var loc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLoc).FirstOrDefault();

                    binnDtls.LocationCode = (loc == null) ? "" : loc.LocationCode;
                    binnDtls.ABCClass = (oItem == null) ? "" : oItem.ABCClass;
                    binnDtls.MovingCode = (oItem == null) ? "" : oItem.MovingCode;
                    binnDtls.ProductType = (oItem == null) ? "" : oItem.ProductType;
                    binnDtls.PartCategory = (oItem == null) ? "" : oItem.PartCategory;
                    binnDtls.BoxNo = model.BoxNo;
                    binnDtls.ReceivedQty = Convert.ToDecimal(model.ReceivedQty);
                    binnDtls.PurchasePrice = Convert.ToDecimal(model.PurchasePrice);
                    //binnDtls.CostPrice = binnDtls.CostPrice;
                    //binnDtls.DiscPct = binnDtls.DiscPct;
                    if (record.TransType == "5" && record.ReceivingType == "1")
                        binnDtls.CostPrice = Convert.ToDecimal(model.PurchasePrice);
                    else
                    {
                        var oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, model.PartNo);
                        binnDtls.CostPrice = (oItemPrice == null) ? 0 : oItemPrice.CostPrice;
                    }
                    binnDtls.DiscPct = Convert.ToDecimal(model.DiscPct);

                    binnDtls.LastUpdateBy = CurrentUser.UserId;
                    binnDtls.LastUpdateDate = DateTime.Now;

                    Helpers.ReplaceNullable(binnDtls);

                    ctx.SaveChanges();
                    tran.Commit();

                    ctx.SpEdpDetails.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == model.BinningNo).ToList().ForEach(x =>
                    {
                        totqty += x.ReceivedQty;
                    });
                    var detail = ctx.SpEdpDetails.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == model.BinningNo);
                    return Json(new
                    {
                        success = true,
                        dat = detail,
                        bintot = updateHdr.TotBinningAmt,
                        totitem = updateHdr.TotItem,
                        tqty = totqty
                    });
                }
                catch (Exception)
                {
                    tran.Rollback();
                    return Json(new { success = false, message = "Gagal save detail !!!" });
                }
            }
        }


        public JsonResult delete(spTrnPBinnHdr model, string pil)
        {
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var query = "";
                    var record = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
                    if (record == null)
                    {
                        record = new spTrnPBinnHdr
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            BinningNo = model.BinningNo,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                            TypeOfGoods = TypeOfGoods

                        };
                        ctx.spTrnPBinnHdrs.Add(record);
                    }
                    record.Status = "3";
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;

                    if (pil == "3")
                    {

                        query = string.Format(@"
                            UPDATE SpUtlStockTrfHdr 
                            SET Status = '0' 
                                , BinningNo = ''
                                , BinningDate = '1900-01-01 00:00:00.000'
                                , LastUpdateBy = '{4}'
                                , LastUpdateDate = GetDate()
                            WHERE CompanyCode = '{0}' AND BranchCode = '{1}' AND DealerCode = '{2}'
                            AND LampiranNo = '{3}' AND RcvDealerCode = '{1}'", CompanyCode, BranchCode, model.SupplierCode, model.DNSupplierNo, CurrentUser.UserId);
                            
                        ctx.Database.ExecuteSqlCommand(query);
                    }
                    else
                    {
                        var result = ctx.LookUpDtls.Find(CompanyCode, "PORDS", "1");
                        if (result == null)
                        {
                            query = string.Format(@"
                            UPDATE SpUtlPINVDDtl SET Status = '0' 
                            WHERE CompanyCode = '{0}' 
                            AND BranchCode = '{1}' 
                            AND DealerCode = '{2}' 
                            AND DeliveryNo = '{3}' 
                            AND TypeOfGoods = '{4}'", CompanyCode, BranchCode, model.SupplierCode, model.DNSupplierNo, TypeOfGoods);
                        }
                        else
                        {
                            query = string.Format(@"
                                UPDATE SpUtlPINVDDtl SET Status = '0' 
                                WHERE CompanyCode = '{0}' 
                                AND BranchCode = '{1}' 
                                AND DealerCode = '{2}' 
                                AND DeliveryNo = '{3}' 
                                AND TypeOfGoods = '{4}'", CompanyCode, BranchCode, (result.ParaValue == "1") ? ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode).LockingBy : CompanyCode, model.DNSupplierNo, TypeOfGoods);
                        }

                        ctx.Database.ExecuteSqlCommand(query);
                    }

                    ctx.SaveChanges();
                    trans.Commit();

                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    trans.Rollback();

                    return Json(new { success = false, message = "Gagal hapus data!!!", error_log = e.Message });
                }
            }
        }

        public JsonResult deleteDetail(SpTrnPBinnDtl model)
        {
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (model.BoxNo == null)
                    {
                        model.BoxNo = "";
                    }
                    var recordHdr = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
                    if (recordHdr != null)
                    {
                        recordHdr.TotItem = recordHdr.TotItem - 1;
                        decimal total = Convert.ToDecimal((model.ReceivedQty * model.PurchasePrice) - ((model.ReceivedQty * model.PurchasePrice) * model.DiscPct / 100));
                        recordHdr.TotBinningAmt = recordHdr.TotBinningAmt - decimal.Round(total);
                    }
                    var binnDtl = ctx.SpTrnPBinnDtls.Find(CompanyCode, BranchCode, model.BinningNo, model.PartNo, model.DocNo, model.BoxNo);
                    if (binnDtl != null)
                    {
                        ctx.SpTrnPBinnDtls.Remove(binnDtl);
                    }
                    //recordHdr.TotItem = recordHdr.TotItem;
                    //recordHdr.TotBinningAmt = recordHdr.TotBinningAmt;

                    recordHdr.Status = "0";
                    recordHdr.LastUpdateBy = CurrentUser.UserId;
                    recordHdr.LastUpdateDate = DateTime.Now;
                    
                    ctx.SaveChanges();
                    trans.Commit();
                    
                    return Json(new { success = true, bintot = recordHdr.TotBinningAmt, totitem = recordHdr.TotItem });
                }
                catch (Exception)
                {
                    trans.Rollback();

                    return Json(new { success = false, message = "Gagal hapus detail!!!" });
                }
            }
        }

        public JsonResult VerifyBinning()
        {
            var pesan = "";
            var genWRS = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode).IsLinkWRS;
            if (genWRS)
            {
                pesan = "Proses ini akan membuat binning dan WRS secara bersamaan\nLanjutkan proses ?";
                return Json(new { success = true, message = pesan });
            }
            else
            {
                pesan = "Apakah anda yakin ?";
                return Json(new { success = true, message = pesan });
            }
        }

        public JsonResult CloseBinning(spTrnPBinnHdr model)
        {
             using (ctx)
            {
                using (var trx = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        //int receivingType;
                        var genWRS = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode).IsLinkWRS;
                        //if (genWRS)
                        //{
                        //    pesan_error = PostingBinning(genWRS, model);
                        //    if (pesan_error != string.Empty)
                        //    {
                        //        return Json(new { success = false, message = pesan_error });
                        //    }
                        //}
                        //else
                        //{
                        pesan_error = PostingBinning(genWRS, model);
                        //    if (pesan_error != string.Empty)
                        //    {
                        //        return Json(new { success = false, message = pesan_error });
                        //    }
                        //}

                        if (pesan_error == string.Empty)
                        {
                            // Commit Transaction
                            trx.Commit();

                            var x = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo).Status;
                            return Json(new { success = true, message = "Binning " + model.BinningNo + " Berhasil di Close", status = x });
                        }
                        else
                        {
                            // Rollbcak Transaction
                            trx.Rollback();

                            return Json(new { success = false, message = pesan_error });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction
                        trx.Rollback();
                        return Json(new { success = false, message = pesan_error, err_log = ex.Message });
                    }
                }
            }
        }

        public string PostingBinning(bool genWRS, spTrnPBinnHdr model)
        {
            string wrsNo = "";
            var MSG_5045 = " sudah di ";
            var MSG_5020 = "Transaction can't be process because no detail data found";
            var MSG_5039 = " gagal";

            DateTime wrsTgl = DateTime.Now;
            var record = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
            var detail = ctx.SpEdpDetails.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == model.BinningNo).ToList();
            if (detail.Count == 0)
            {
                return MSG_5020;
            }
            if (record != null)
            {
                pesan_error = DateTransValidation(model.BinningDate.Value.Date);
                if (!string.IsNullOrEmpty(pesan_error))
                {
                    return pesan_error;
                }

                bool result = false;
                var recordTemp = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, record.BinningNo);
                if (recordTemp.Status.Equals("2"))
                {
                    pesan_error = "DocNo" + record.BinningNo + MSG_5045 + "CLOSE";
                    return pesan_error;
                }
                else if (recordTemp.Status.Equals("3"))
                {
                    pesan_error = "DocNo" + record.BinningNo + MSG_5045 + "Delete";
                    return pesan_error;
                }
                else if (recordTemp.Status.Equals("4"))
                {
                    pesan_error = "DocNo" + record.BinningNo + MSG_5045 + "WRS";
                    return pesan_error;
                }

                if (record.Status == "0")
                {
                    pesan_error = "Binning List harus diprint terlebih dahulu.!!";
                    return pesan_error;
                }

                if (record.TransType == "4")
                {
                    if (CheckqtyBinning(model))
                    {
                        pesan_error = "Closing Binning" + MSG_5039 + " Terdapat qty yang melebihi batas maksimal";
                        return pesan_error;
                    }
                }
                if (record.TransType == "2")
                {
                    int num = 0;
                    bool flagError = false;
                    var dtCheck = ctx.SpSelectByNoBinnings.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == model.BinningNo).ToList();
                    foreach (var rowBinning in dtCheck)
                    {
                        if (!CheckMaxQtyBinningClosing(decimal.Parse(rowBinning.ReceivedQty.ToString()), model.BinningNo, rowBinning.DocNo.ToString(), rowBinning.PartNo.ToString(), model.SupplierCode))
                        {
                            flagError = true;
                        }
                        num++;
                    }
                    if (flagError)
                    {
                        pesan_error = "Closing Binning" + MSG_5039 + " Terdapat qty yang melebihi batas maksimal";
                        return pesan_error;
                    }
                }

                bool pinv = false;
                if (model.ReceivingType == "2") pinv = true;
                result = CloseBinningList(record, genWRS, ref wrsNo, ref wrsTgl, pinv);
                if (!result)
                {
                    pesan_error = "Proses Closing Binning List Gagal : " + pesan_error;
                    //+" " + errPartNo + " " + errException;
                    return pesan_error;
                }
            }
            return pesan_error;
        }

        private bool CloseBinningList(spTrnPBinnHdr recordHdr, bool genWrs, ref string wrsNo, ref DateTime wrsTgl, bool pinvs)
        {
            SpMstItemLocBLL spMstItemLocBLL = SpMstItemLocBLL.Instance(CurrentUser.UserId);
            bool result = false;
            string binningNo = recordHdr.BinningNo;
            spTrnPBinnHdr oSpTrnPBinnHdrDao = new spTrnPBinnHdr();
            spTrnPPOSHdr oSpTrnPPOSHdrDao = new spTrnPPOSHdr();
            SpTrnPRcvHdr oSpTrnPRcvHdrDao = new SpTrnPRcvHdr();
            Supplier oGnMstSupplierDao = new Supplier();
            try
            {
                recordHdr = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, binningNo);
                if (Convert.ToInt32(recordHdr.Status) > 1)
                    return result;
                if (recordHdr.TransType == "4")
                {
                    var dtTable = ctx.SpTrnPBinnDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == recordHdr.BinningNo).FirstOrDefault();

                    if (dtTable != null)
                    {
                        var binnHdr = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, recordHdr.BinningNo);
                        if (genWrs)
                            binnHdr.Status = "4";
                        else
                            binnHdr.Status = "2";
                        binnHdr.LastUpdateBy = CurrentUser.UserId;
                        binnHdr.LastUpdateDate = DateTime.Now;
                        try
                        {
                            ctx.SaveChanges();
                            result = true;
                        }
                        catch
                        {
                            result = false;
                        }


                        if (result)
                        {
                            string tempPosNo = "";

                            ctx.SpTrnPBinnDtls.Where(b => b.CompanyCode == CompanyCode && b.BranchCode == BranchCode && b.BinningNo == recordHdr.BinningNo).ToList().ForEach(x =>
                            {
                                var dt = GetMaxQtyBinning(x.BinningNo.ToString(), x.DocNo.ToString(), x.PartNo.ToString(), recordHdr.SupplierCode);
                                if (dt.Count() > 0)
                                {

                                    var maxQty = dt.FirstOrDefault().OrderQty;
                                    //decimal maxQty = decimal.Parse(dt.Rows[0][0].ToString());
                                    decimal qty = Convert.ToDecimal(x.ReceivedQty.ToString());
                                    var lastQty = dt.FirstOrDefault().ReceiveQty;
                                    //decimal lastQty = decimal.Parse(dt.Rows[0][1].ToString());
                                    if ((lastQty + qty) > maxQty)
                                    {
                                        pesan_error = "Qty Received melebihi Qty yang di Pesan";
                                        result = false;
                                    }
                                }
                            });

                            if (recordHdr.TransType == "4")
                            {
                                SpTrnPBinnDtl recordDtl = new SpTrnPBinnDtl();

                                var rdtl = ctx.SpTrnPBinnDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == binningNo).FirstOrDefault();
                                if (!(UpdateOrderBalance(recordDtl, recordHdr.SupplierCode, binningNo)))
                                {
                                    result = false;
                                    return result;
                                }

                                ctx.SpTrnPBinnDtls.Where(b => b.CompanyCode == CompanyCode && b.BranchCode == BranchCode && b.BinningNo == recordHdr.BinningNo).ToList().ForEach(x =>
                                {
                                    if (tempPosNo != x.DocNo.ToString())
                                    {
                                        var posHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, x.DocNo.ToString());
                                        posHdr.Status = "4";
                                        posHdr.LastUpdateBy = CurrentUser.UserId;
                                        posHdr.LastUpdateDate = DateTime.Now;
                                        try
                                        {
                                            ctx.SaveChanges();
                                            result = true;
                                        }
                                        catch
                                        {
                                            result = false;
                                        }

                                        if (!result)
                                        {
                                            pesan_error = "Update Status POS Header Gagal";
                                            result = false;
                                        }
                                    }
                                    tempPosNo = x.DocNo.ToString();
                                });
                            }
                        }
                        else
                            pesan_error = "Data sedang di Locking, Tunggu beberapa saat lagi";

                        if (pinvs)
                        {
                            var recSupplier = ctx.GnMstSuppliers.Find(CompanyCode, recordHdr.SupplierCode);
                            var qr = string.Format(@"
                            UPDATE spUtlPINVDHdr SET Status = 2, BinningNo = '{5}', BinningDate = '{6}' , 
                            LastUpdateBy = '{7}' , LastUpdateDate = GetDate() WHERE CompanyCode = '{0}' 
                            AND BranchCode = '{1}' AND DealerCode = '{2}' AND DeliveryNo = '{3}' AND TypeOfGoods = '{4}'",
                            recordHdr.CompanyCode, recordHdr.BranchCode, recSupplier.StandardCode, recordHdr.DNSupplierNo, recordHdr.TypeOfGoods, recordHdr.BinningNo, recordHdr.BinningDate, CurrentUser.UserId);
                            try
                            {
                                ctx.Database.ExecuteSqlCommand(qr);
                                result = true;
                            }
                            catch
                            {
                                result = false;
                            }
                            if (!result)
                            {
                                pesan_error = "Update Status PINVD Header Gagal";
                                return result;
                            }
                        }
                    }
                }
                else
                {
                    
                        var binnHdr = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, recordHdr.BinningNo);

                        if (genWrs)
                            binnHdr.Status = "4";
                        else
                            binnHdr.Status = "2";

                        binnHdr.LastUpdateBy = CurrentUser.UserId;
                        binnHdr.LastUpdateDate = DateTime.Now;
                        try
                        {
                            ctx.SaveChanges();
                            result = true;
                        }
                        catch
                        {
                            result = false;
                        }

                        if (!result)
                        {
                            pesan_error = "Data sedang di Locking, Tunggu beberapa saat lagi";
                            return result;
                        }
                }

                if (genWrs)
                {
                    SpTrnPRcvHdr recordWRSHdr = new SpTrnPRcvHdr();
                    recordWRSHdr.BinningDate = recordHdr.BinningDate;
                    recordWRSHdr.BinningNo = recordHdr.BinningNo;
                    recordWRSHdr.CompanyCode = CompanyCode;
                    recordWRSHdr.BranchCode = BranchCode;
                    recordWRSHdr.CreatedBy = CurrentUser.UserId;
                    recordWRSHdr.CreatedDate = DateTime.Now;
                    recordWRSHdr.DNSupplierDate = recordHdr.DNSupplierDate;
                    recordWRSHdr.DNSupplierNo = recordHdr.DNSupplierNo;
                    recordWRSHdr.LastUpdateBy = CurrentUser.UserId;
                    recordWRSHdr.LastUpdateDate = DateTime.Now;
                    recordWRSHdr.PrintSeq = 1;
                    recordWRSHdr.ReceivingType = recordHdr.ReceivingType;
                    recordWRSHdr.ReferenceDate = recordHdr.ReferenceDate;
                    recordWRSHdr.ReferenceNo = recordHdr.ReferenceNo;
                    recordWRSHdr.Status = "2";
                    recordWRSHdr.SupplierCode = recordHdr.SupplierCode;
                    recordWRSHdr.TotItem = recordHdr.TotItem;
                    recordWRSHdr.TotWRSAmt = recordHdr.TotBinningAmt;
                    recordWRSHdr.TransType = recordHdr.TransType;
                    recordWRSHdr.TypeOfGoods = recordHdr.TypeOfGoods;
                    var recSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                    recordWRSHdr.WRSDate = recSpare != null ? recSpare.TransDate : DateTime.Now;


                    if (recordWRSHdr.TransType == "4")
                        recordWRSHdr.WRSNo = GetNewGLNo("WRL");
                    else
                        recordWRSHdr.WRSNo = GetNewGLNo("WRN");
                    try
                    {
                        ctx.SpTrnPRcvHdrs.Add(recordWRSHdr);
                        ctx.SaveChanges();
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                    if (result)
                        result = SaveWRSDetailByBinning(recordHdr.BinningNo, recordWRSHdr.WRSNo);

                    wrsNo = recordWRSHdr.WRSNo;
                    wrsTgl = (DateTime)recordWRSHdr.WRSDate;

                    spTrnPPOSHdr posHdrDao = new spTrnPPOSHdr();
                    spMstItemPrice itemPriceDao = new spMstItemPrice();
                    SpTrnPRcvDtl receiveDtlDao = new SpTrnPRcvDtl();

                    bool isUpdateOnHandOnly = false;
                    bool isUpdateOnHandAndInTransit = false;
                    bool isUpdateOnHandAndBorrow = false;
                    bool isUpdateOnHandAndBorrowed = false;
                    decimal avgCost = 0;
                    decimal pPrice = 0;
                    decimal taxPct = 0;

                    var receiveDtls = ctx.SpSelectByNoWRSes.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.WRSNo == recordWRSHdr.WRSNo).ToList();
                    foreach (var receiveDtl in receiveDtls)
                    {
                        // check for transaction type 4
                        if (recordWRSHdr.TransType.Equals("4"))   // pembelian
                        {
                            // get POS header
                            var posHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, receiveDtl.DocNo);
                            if (posHdr == null)
                                return result = false;

                            // update status POS header
                            posHdr.Status = "5";    // Receiving Generated
                            posHdr.LastUpdateBy = CurrentUser.UserId;
                            posHdr.LastUpdateDate = DateTime.Now;
                            try
                            {
                                ctx.SaveChanges();
                                result = true;
                            }
                            catch
                            {
                                result = false;
                            }
                            if (!result)
                                return result;
                        }

                        // get initial
                        isUpdateOnHandOnly = (recordWRSHdr.ReceivingType.Equals("1") && recordWRSHdr.TransType.Equals("0")) ||  // BPS & transfer stock
                                             (recordWRSHdr.ReceivingType.Equals("1") && recordWRSHdr.TransType.Equals("3")) ||  // BPS & lain-lain
                                             (recordWRSHdr.ReceivingType.Equals("3") && recordWRSHdr.TransType.Equals("0"));    // transfer stock & transfer stock

                        isUpdateOnHandAndInTransit = (recordWRSHdr.ReceivingType.Equals("1") && recordWRSHdr.TransType.Equals("4")) || // BPS & pembelian
                                                     (recordWRSHdr.ReceivingType.Equals("2") && recordWRSHdr.TransType.Equals("4"));   // PINV & pembelian

                        isUpdateOnHandAndBorrow = (recordWRSHdr.ReceivingType.Equals("1") && recordWRSHdr.TransType.Equals("1"));  // BPS & peminjaman

                        isUpdateOnHandAndBorrowed = (recordWRSHdr.ReceivingType.Equals("1") && recordWRSHdr.TransType.Equals("2"));  // BPS & pengembalian


                        // update items table
                        if (isUpdateOnHandOnly)
                        {
                            // update onhand in SpMstItems & SpMstItemLoc table
                            result = spMstItemLocBLL.UpdateStock(ctx, receiveDtl.PartNo, receiveDtl.WarehouseCode, receiveDtl.ReceivedQty, 0, 0, 0, string.Empty);
                            if (!result)
                                return result;   // false
                        }// end-of-if-isUpdateOnHandOnly

                        if (isUpdateOnHandAndBorrow)
                        {
                            // update onhand and borrow in SpMstItems table
                            // update onhand in SpMstItems and SpMstItemLoc
                            result = spMstItemLocBLL.UpdateStock(ctx, receiveDtl.PartNo, receiveDtl.WarehouseCode, receiveDtl.ReceivedQty, 0, 0, 0, string.Empty);
                            if (!result)
                                return result;   // false

                            // update borrow in SpMstItems
                            result = spMstItemLocBLL.UpdateStock(ctx, receiveDtl.PartNo, 0, receiveDtl.ReceivedQty);
                            if (!result)
                                return result;

                        }// end-of-if-isUpdateOnHandAndBorrow

                        if (isUpdateOnHandAndBorrowed)
                        {
                            // update onhand and borrow in SpMstItems table
                            // update onhand in SpMstItems and SpMstItemLoc
                            result = spMstItemLocBLL.UpdateStock(ctx, receiveDtl.PartNo, receiveDtl.WarehouseCode, receiveDtl.ReceivedQty, 0, 0, 0, string.Empty);
                            if (!result)
                                return result;   // false

                            // update borrow in SpMstItems
                            result = spMstItemLocBLL.UpdateStock(ctx, receiveDtl.PartNo, 0, receiveDtl.ReceivedQty * -1);
                            if (!result)
                                return result;

                        }// end-of-if-isUpdateOnHandAndBorrowed


                        // get item price
                        var itemPrice = ctx.spMstItemPrices.Find(recordWRSHdr.CompanyCode, recordWRSHdr.BranchCode, receiveDtl.PartNo);
                        if (itemPrice == null)
                            return result = false;

                        // update wrs detail cost price 
                        receiveDtlDao.CostPrice = itemPrice.CostPrice;
                        receiveDtlDao.LastUpdateBy = CurrentUser.UserId;
                        receiveDtlDao.LastUpdateDate = DateTime.Now;
                        try
                        {
                            ctx.SpTrnPRcvDtls.Add(receiveDtlDao);
                            ctx.SaveChanges();
                            result = true;
                        }
                        catch
                        {
                            result = false;
                        }

                        if (!result)
                            return result;   // false

                        if (recordWRSHdr.TransType.Equals("4") || recordWRSHdr.TransType.Equals("0")) // 4->pembelian ; 0->transfer stock
                        {
                            // get cost price from SpMstItems
                            var rowItem = SelectCostPrice(receiveDtl.PartNo);
                            if (rowItem == null)
                                return result = false;

                            // get is Purchase Include Tax
                            string TaxCod = ctx.MstSupplierProfitCenters.Find(CompanyCode, BranchCode, recordWRSHdr.SupplierCode, ProfitCenter).TaxCode;
                            if (string.IsNullOrEmpty(TaxCod))
                                taxPct = 0;
                            else
                                taxPct = Convert.ToDecimal(ctx.Taxes.Find(CompanyCode, TaxCod).TaxPct);

                            pPrice = (decimal)receiveDtl.PurchasePrice;

                            // get average cost
                            avgCost = AverageCost((decimal)rowItem[0].OnHand, (decimal)rowItem[0].CostPrice, receiveDtl.ReceivedQty, pPrice, receiveDtl.DiscPct);

                            // update average cost in item price table
                            result = new EntryWRSController().UpdateItemAvgCost(receiveDtl.PartNo, avgCost, (decimal)rowItem[0].CostPrice) >= 0;
                            if (!result)
                                return result;   // false
                        }

                        if (isUpdateOnHandAndInTransit)
                        {
                            // update orderbalance & SpMstItems
                            result = new EntryWRSController().UpdateIntransitOrderBalance(receiveDtl.DocNo, recordWRSHdr.SupplierCode, receiveDtl.PartNo, receiveDtl.ReceivedQty, receiveDtl.WRSNo, recordWRSHdr.WRSDate) >= 0;
                            if (!result)
                                return result;

                            result = new EntryWRSController().UpdateItemStockLocation(receiveDtl.PartNo, receiveDtl.ReceivedQty, 1, "00") >= 0;
                            if (!result)
                                return result;
                        }

                        if (result)
                            result = new EntryWRSController().MovementLog(recordWRSHdr.WRSNo, recordWRSHdr.WRSDate, receiveDtl.PartNo, receiveDtl.WarehouseCode, "IN", "WRS", receiveDtl.ReceivedQty);
                        if (!result)
                            return result;
                    }
                }
                ctx.SaveChanges();
            }
            catch (Exception e)
            {
                errException = e.Message;
                result = false;
            }
            return result;
        }


        private decimal AverageCost(decimal invStock, decimal costPrice, decimal? receivedQty, decimal purchasePrice, decimal? discPct)
        {
            return Convert.ToDecimal(((invStock * costPrice) + ((receivedQty * purchasePrice) * (1 - (discPct / 100)))) / (invStock + receivedQty));
        }

        private List<QueryCloseWrs1> SelectCostPrice(string partNo)
        {
            var query = string.Format(@"
        SELECT A.CompanyCode, A.BranchCode, A.PartNo,
        C.SupplierCode, A.OnHand, B.CostPrice
        FROM {0}..spMstItems A
        INNER JOIN spMstItemPrice B 
        ON (A.CompanyCode = B.CompanyCode) AND (A.BranchCode = B.BranchCode) AND (A.PartNo = B.PartNo)
        INNER JOIN spMstItemInfo C
        ON C.CompanyCode = A.CompanyCode AND C.PartNo = A.PartNo
        WHERE A.CompanyCode = '{1}'
        AND A.BranchCode = '{2}'
        AND A.TypeOfGoods = '{3}'
        AND A.ProductType = '{4}'
        AND A.PartNo = '{5}'
        ORDER BY
        A.CompanyCode ASC,
        A.BranchCode ASC,
        A.PartNo ASC", GetDbMD(), CompanyCode, BranchCode, TypeOfGoods, ProductType, partNo);
            var dt = ctx.Database.SqlQuery<QueryCloseWrs1>(query).ToList();
            return dt;
        }

        private bool SaveWRSDetailByBinning(string binningNo, string wrsNo)
        {
            bool result = true;
            var query = string.Format(@"
            INSERT INTO spTrnPRcvDtl SELECT 
            CompanyCode,BranchCode, '{2}' WRSNo,  PartNo, 
            DocNo,DocDate,WarehouseCode,LocationCode,BoxNo,
            ReceivedQty,PurchasePrice,CostPrice,DiscPct,ABCClass,
            MovingCode,ProductType,PartCategory,CreatedBy,CreatedDate,
            LastUpdateBy,LastUpdateDate FROM spTrnPBinnDtl WHERE CompanyCode = '{0}' 
            AND BranchCode = '{1}' AND BinningNo = '{3}'", CompanyCode, BranchCode, wrsNo, binningNo);
            try
            {
                ctx.Database.ExecuteSqlCommand(query);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private string GetNewGLNo(string TIPE)
        {
            return GetNewDocumentNoHpp(TIPE, DateTime.Now.ToString("yyyyMMdd"));
        }

        public bool UpdateOrderBalance(SpTrnPBinnDtl oSpTrnPBinnDtl, string suppcode, string BinnNo)
        {
            bool result = false;
            spTrnPOrderBalance oSpTrnPOrderBalanceDao = new spTrnPOrderBalance();

            ctx.SpTrnPBinnDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == BinnNo).ToList().ForEach(rec =>
            {
                errPartNo = rec.PartNo;
                var qr = ctx.spTrnPOrderBalances.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.POSNo == rec.DocNo && a.SupplierCode == suppcode && a.PartNo == rec.PartNo).FirstOrDefault();
                decimal? qty = rec.ReceivedQty;
                //ct.SpTrnPBinnDtls.Find(CompanyCode, BranchCode, rec.BinningNo, rec.PartNo, rec.DocNo, rec.BoxNo).ReceivedQty;
                if (qr == null)
                {
                    pesan_error = string.Format("\nPart {0}, Tidak ditemukan di Part No yang di Pesan", oSpTrnPBinnDtl.PartNo);
                    result = false;
                }
                else
                {
                    if (qty == decimal.Zero)
                    {
                        pesan_error = string.Format("\nRecived Qty harus lebih besar dari 0 (No Part : {0})", oSpTrnPBinnDtl.PartNo);
                        result = false;
                    }

                    if (qty < qr.OnOrder)
                    {
                        qr.InTransit = qr.InTransit + qty;
                        qr.OnOrder = qr.OnOrder - qty;
                        qr.LastUpdateBy = CurrentUser.UserId;
                        qr.LastUpdateDate = DateTime.Now;
                        result = true;
                        if (!result)
                        {
                            pesan_error = "Update On Order Stock di Order Balance Gagal";
                            result = false;
                        }
                        else
                        {
                            if (!(UpdateOnOrderStock(qr.PartNo, 0, qty, 0)))
                            {
                                pesan_error = "Update On Order Stock di Order Balance Gagal";
                                result = false;
                            }
                            result = true;
                        }
                    }
                    else
                    {
                        decimal qtyOnOrder = (decimal)qr.OnOrder;
                        if (qtyOnOrder > 0)
                        {
                            qr.InTransit = qr.InTransit + qr.OnOrder;
                            qr.OnOrder = 0;
                            qr.LastUpdateBy = CurrentUser.UserId;
                            qr.LastUpdateDate = DateTime.Now;
                            try
                            {
                                //      ct.SaveChanges();
                                result = true;
                            }
                            catch
                            {
                                result = false;
                            }
                            if (!result)
                            {
                                pesan_error = "Update On Order Stock di Order Balance Gagal";
                                result = false;
                            }
                            else
                            {
                                if (!(UpdateOnOrderStock(qr.PartNo, 0, qtyOnOrder, 0)))
                                {
                                    pesan_error = "Update On Order Stock di Order Balance Gagal";
                                    result = false;
                                }
                                qty = qty - qtyOnOrder;
                            }
                        }
                    }
                }
            });

            return ctx.SaveChanges() > 0;
        }

        private bool UpdateOnOrderStock(string partno, int onorder, decimal? intransit, int received)
        {
            if (intransit == null)
            {
                intransit = 0;
            }
            bool result = false;

            //spMstItem oItemDao = new spMstItem();
            var query = string.Format(@"select * from {0}..spMstItems where CompanyCode = '{1}' and BranchCode = '{2}' and PartNo = '{3}'", GetDbMD(), CompanyMD, BranchMD, partno);
            var oItem = ctx.Database.SqlQuery<spMstItem>(query).FirstOrDefault();

                //DealerCode() == "MD" ? ctx.spMstItems.Find(CompanyCode, BranchCode, partno) : ctxMD.spMstItems.Find(CompanyCode, BranchCode, partno);
            if (oItem != null)
            {
                //if (Math.Abs(onorder) > 0)
                //{
                //    oItem.OnOrder += onorder;
                //    setUpdate = "set OnOrder = " + oItem.OnOrder;
                //}

                if (Math.Abs((decimal)intransit) > 0)
                {
                    oItem.OnOrder -= intransit;
                    oItem.InTransit += intransit;
                }

                //if (received > 0)
                //{
                //    oItem.InTransit -= received;
                //    oItem.OnHand += received;
                //    setUpdate = "set InTransit = " + oItem.InTransit +", OnHand = " + oItem.OnHand;
                //}

                if (oItem.OnOrder < 0 || oItem.InTransit < 0)
                    return false;

                oItem.LastUpdateDate = DateTime.Now;
                oItem.LastUpdateBy = CurrentUser.UserId;

                var setUpdate = string.Format(@"UPDATE {0}..spMstItems 
                SET OnOrder = {1}, InTransit = {2}, LastUpdateDate = '{3}', LastUpdateBy = '{4}'
                WHERE CompanyCode = '{5}' and BranchCode = '{6}' and PartNo = '{7}'"
                , GetDbMD(), oItem.OnOrder, oItem.InTransit, DateTime.Now, CurrentUser.UserId, CompanyMD, BranchMD, partno);
               
                try
                {
                    ctx.Database.ExecuteSqlCommand(setUpdate);
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }

        private bool CheckMaxQtyBinningClosing(decimal qty, string BinningNo, string DocNo, string PartNo, string supplierCode)
        {
            bool result = true;
            var dt = GetMaxQtyBinning(BinningNo, DocNo, PartNo, supplierCode).FirstOrDefault();
            if (dt != null)
            {
                //decimal maxQty = decimal.Parse(dt.Rows[0][0].ToString());
                //decimal lastQty = decimal.Parse(dt.Rows[0][1].ToString());
                var maxQty = dt.OrderQty;
                var lastQty = dt.ReceiveQty;
                if ((lastQty + qty) > maxQty)
                    result = false;
            }
            return result;
        }

        private List<MaxQty> GetMaxQtyBinning(string BinningNo, string DocNo, string PartNo, string supplierCode)
        {
            var opt = "";
            if (DocNo.StartsWith("P"))
            {
                opt = "P";
            }
            //DataSet ds = new DataSet();

            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //SqlDataAdapter da = new SqlDataAdapter(cmd);

            //cmd.CommandText = "sp_GetMaxQtyBinning";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@DocNo", DocNo);
            //cmd.Parameters.AddWithValue("@SupplierCode", supplierCode);
            //cmd.Parameters.AddWithValue("@PartNo", PartNo);
            //cmd.Parameters.AddWithValue("@BinningNo", BinningNo);
            //cmd.Parameters.AddWithValue("@Opt", opt);

            object[] paramBin = { CompanyCode, BranchCode, DocNo, supplierCode, PartNo, BinningNo, opt };
            string query = "sp_GetMaxQtyBinning @p0,@p1,@p2,@p3,@p4,@p5,@p6";

            var data = ctx.Database.SqlQuery<MaxQty>(query, paramBin).ToList();

            //da.Fill(ds);

            //return ds.Tables[0];
            return data;
        }

        private class MaxQty
        {
            public decimal? OrderQty { get; set; }
            public decimal? ReceiveQty { get; set; }
        }

        private bool CheckqtyBinning(spTrnPBinnHdr model)
        {
            bool flagError = false;
            var dtCheck = ctx.SpEdpDetails.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == model.BinningNo).ToList();
            for (int x = 0; x < dtCheck.Count; x++)
            {
                if ((GetMaxOnOrder(dtCheck[x].DocNo.ToString(), dtCheck[x].PartNo.ToString()) - (decimal)dtCheck[x].ReceivedQty) < 0)
                {
                    //dtCheck[x].DefaultCellStyle.BackColor = Color.IndianRed;
                    flagError = true;
                }
            }
            return flagError;
        }

        private decimal GetMaxOnOrder(string PosNo, string partNo)
        {
            var query = string.Format(@"
            SELECT ISNULL(SUM(OnOrder),0) AS OnOrder FROM SpTrnPOrderBalance
            WHERE
            CompanyCode = '{0}' AND BranchCode = '{1}' AND
            POSNo = '{2}' AND PartNo = '{3}'
            ", CompanyCode, BranchCode, PosNo, partNo);
            var dt = ctx.Database.SqlQuery<COGSLev>(query).FirstOrDefault();
            if (dt != null)
            {
                return (string.IsNullOrEmpty(dt.OnOrder.ToString())) ? 0 : (decimal)dt.OnOrder;
            }
            else
            {
                return 0;
            }
        }

        public JsonResult Print(spTrnPBinnHdr model, string type, string LampiranNo, string DealerCode)
        {
            var record = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, model.BinningNo);
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
                if (type == "3") {
                    var detail = ctx.SpLoadDetail_TranStocks.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.LampiranNo == LampiranNo && a.DealerCode == DealerCode).ToList();
                    return Json(new { success = true, status = record.Status, datdet = detail });
                }
                else
                {
                    var detail = ctx.SpEdpDetails.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == model.BinningNo);
                    return Json(new { success = true, status = record.Status, datdet = detail });

                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Gagal print data!!!" });
            }


        }

        public JsonResult loadData(string BinningNo, string SupplierCode)
        {
            var query = "";

            query = string.Format(@"
            select isnull(sum(dtl.ReceivedQty), 0) as ReceivedQty 
            from spTrnPBinnHdr hdr with(nolock, nowait)
            left join spTrnPBinnDtl dtl with(nolock, nowait) on dtl.CompanyCode = hdr.CompanyCode 
            and dtl.BranchCode = hdr.BranchCode 
            and dtl.BinningNo = hdr.BinningNo 
            where hdr.BinningNo = '{2}'
            and hdr.CompanyCode = '{0}'
            and hdr.BranchCode = '{1}'", CompanyCode, BranchCode, BinningNo);

            var detail = ctx.SpEdpDetails.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BinningNo == BinningNo).ToList();
            var record = ctx.gnMstSupplierViews.Find(CompanyCode, SupplierCode);
            var Tot = ctx.Database.SqlQuery<TotalQty>(query).FirstOrDefault();
            //  var LoadTabel = ctx.GridEntryWRSs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.WRSNo == WRSNo);

            var islinkwrs = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode).IsLinkWRS;
            return Json(new
            {
                Supp = record,
                QtyTot = Tot,
                DatGrid = detail,
                status = islinkwrs
            });
        }

        public JsonResult LoadPelanggan(string CustomerCode)
        {
            var query = string.Format(@"
            SELECT * FROM GnMstSupplierProfitCenter WHERE CompanyCode = '{0}' and BranchCode = '{1}'  
            and SupplierCode = '{2}' AND ProfitCenterCode = '{3}'", CompanyCode, BranchCode, CustomerCode, ProfitCenter);
            var record = ctx.Database.SqlQuery<SupplierProfitCenter>(query).FirstOrDefault();

            return Json(record);
        }

        public JsonResult Detail_TransferStock(string LampiranNo, string DealerCode)
        {
            var record = ctx.SpLoadDetail_TranStocks.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.LampiranNo == LampiranNo && a.DealerCode == DealerCode).ToList();

            try
            {
                return Json(new { success = true, data = record });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Gagal mengambil data!!!" });
            }
        }

        public JsonResult SelectDtlUtlPINVDtl(spTrnPBinnHdr model)
        {
            var record = ctx.spUtlPINVDtlViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.DeliveryNo == model.DNSupplierNo).FirstOrDefault();

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult GetBinnDtlBpsByPartNo(string BinningNo, string PartNo, string DocumentNo)
        {
            try{
                var binnDtls =  ctx.SpTrnPBinnDtls.Where(
                    x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode 
                    && x.BinningNo == BinningNo && x.PartNo == PartNo && x.DocNo == DocumentNo);

                if (binnDtls.Count() > 0)
                {
                    return Json(new {success = true, data = binnDtls.FirstOrDefault()});
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch(Exception e){
                return Json(new { success = false, error_log = e.Message });
            }
        }


        #region == Private Method ==
        private bool IsClosedDocNo(string binningNo, ref string msg)
        {
            var record = ctx.spTrnPBinnHdrs.Find(CompanyCode, BranchCode, binningNo);
            if (record == null) return false;
            if (int.Parse(record.Status) > 1)
            {
                msg = "No. dokumen ini telah diubah oleh user lain";
                return true;
            }
            return false;
        }

        private List<BinnFromTransferStock> SelectBinningFromTransferStock(string companyCode, string branchCode, string supplierCode, string lampiranNo)
        {
            var sql = string.Format(@"
                SELECT 
                 row_number () OVER (ORDER BY a.CreatedDate) AS NoUrut
                , a.OrderNo as DocNo
                , a.PartNo
                , a.PurchasePrice
                , '0.00' as DiscPct
                , a.QtyShipped ReceivedQty
                , '' as BoxNo
                , ISNULL(b.PartName, '') as NmPart
                FROM spUtlStockTrfDtl a
                LEFT JOIN SpMstItemInfo b ON b.CompanyCode=a.CompanyCode
                    AND b.PartNo=a.PartNo
                WHERE a.CompanyCode = '{0}'
                  AND a.BranchCode = '{1}'
                  AND a.DealerCode = '{2}'
                  AND a.LampiranNo = '{3}'", 
                companyCode, BranchCode, supplierCode, lampiranNo);
            
            var records = ctx.Database.SqlQuery<BinnFromTransferStock>(sql);

            return records.ToList();
        }
        
        #endregion
    }
}
