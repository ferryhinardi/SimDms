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
    public class StockTakingPostController : BaseController
    {
        
        public JsonResult FisrtLoad()
        {
            
            var query = string.Format(@"
            SELECT TOP 1 * FROM spTrnStockTakingHdr 
            WHERE CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND (Status = 1 OR Status = 0)", CompanyCode, BranchCode);

            var recordHeader = ctx.Database.SqlQuery<SpTrnStockTakingHdr>(query).FirstOrDefault();
            if (recordHeader != null)
            {
                var record = ctx.LookUpDtls.Find(CompanyCode, "WRCD", recordHeader.WarehouseCode);
                return Json(new
                {
                    success = true,
                    data = record
                });
            }
            else {
                return Json(new
                {
                   success = false
                });
            }
            
        }

        public void Proses() 
        {
            //try
           // {
                Proccess_Master();
                //return Json(new { success = true });
           // }
           // catch (Exception e) {
           //     throw new Exception("Save Fail");
               // return Json(new { success = false, message = e.Message });
         //   }
        }

        public JsonResult Proccess_Master()
        {
            var MSG_5039 = "{0} gagal {1}";
           // var pesan = "";
            try {
                    string errMsg = string.Empty;
                    if (!DateTransValidation(DateTime.Now, ref errMsg))
                    {
                        return Json(new {success = false, message = errMsg });
                    }
                    var dtStockTakingHdr = ctx.SpTrnStockTakingHdrs.Where(a=>a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && (a.Status == "0" || a.Status == "1")).ToList();
                    if (dtStockTakingHdr.Count == 1)
                    {
                        var recHdr = ctx.SpTrnStockTakingHdrs.Find(CompanyCode, BranchCode, dtStockTakingHdr[0].STHdrNo.ToString());
                        if (recHdr == null)
                        {
                            var pesan = string.Format((MSG_5039), "Proses Stock Taking Master", "Terdapat kesalahan data");
                            throw new Exception(pesan);
                            //return Json(new{ success = false, message = pesan});
                        }

                        if (!CheckPartStockEntry(dtStockTakingHdr[0].STHdrNo.ToString()))
                        {
                            var pesan = string.Format((MSG_5039), "Proses Stock Taking Master", "Terdapat data part yang belum di-entry");
                            throw new Exception(pesan);
                            //return Json(new{ success = false, message = pesan});
                        }

                        var dtPartStat = GetPartStatDtl(dtStockTakingHdr[0].STHdrNo.ToString());
                        if (dtPartStat.Count > 0)
                        {
                            var pesan = string.Format((MSG_5039), "Proses Stock Taking Master", "Lakukan Proses Cetak Analisa terlebih dahulu");
                            throw new Exception(pesan);
                            //return Json(new{ success = false, message = pesan});
                        }

                        if (IsBlankExist(dtStockTakingHdr[0].STHdrNo.ToString()))
                        {
                            var pesan = "Lakukan proses Pembatalan Inv. Form/Tag terlebih dahulu";
                            throw new Exception(pesan);
                            //return Json(new{ success = false, message = pesan});
                        }


                        return Json(new { success = true, data = recHdr });
                      //  if (XMessageBox.PostingConfirmationStandard() == DialogResult.Yes)
                      //  {
                            //pbSO.Visible = true;
                            //lblProgSO.Visible = true;
                            //this.Enabled = false;

                         //   PostingProgress pb = new PostingProgress();
                          //  pb.MassageChanged += new PostingProgress.MassageChangeEvent(pb_MassageChanged);
                           // pb.ProgressChanged += new PostingProgress.ProgressChangeEvent(pb_ProgressChanged);
                          //  isSuccess = StockTackingBLL.ProcToMasterTuning(user, recHdr, txtWarehouseCode.Text, pb);
                       // }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Save Fail" });
                    }
            }catch(Exception e){
                return Json(new { success = false, message = e.Message });
            }
        }

        public JsonResult ProcToMasterTuning(string WarehouseCode, SpTrnStockTakingHdr recHdr)
        {
            using (ctx)
            {
                using (var trx = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        bool result = false;
                        var dtStockMaster = SelectSumStockTakingDataTable(WarehouseCode);
                        if (dtStockMaster.Count > 0)
                        {
                            result = UpdateLocationItemLocForStockTaking(WarehouseCode, dtStockMaster, recHdr);
                            if (result)
                            {
                                result = AdjustmentForStockTakingTuning(dtStockMaster, recHdr, WarehouseCode);
                                if (result)
                                {
                                    result = WHTransferForStockTakingTuning(dtStockMaster, recHdr, WarehouseCode);
                                    if (result)
                                    {
                                        result = SpTrnStockTakingDtlUpdate(recHdr) && SpTrnStockTakingHdrUpdate(recHdr.STHdrNo);
                                    }
                                }
                            }
                        }

                        if (result)
                        {
                            //Transaction Commit
                            trx.Commit();
                            return Json(new { success = true });
                        }
                        else
                        {
                            //Transaction Rollback
                            trx.Rollback();
                            return Json(new { success = false });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction
                        trx.Rollback();
                        return Json(new { success = false, error_log = ex.Message });
                    }
                }
            }
        }

        public JsonResult setInformationStock()
        {
            string bodyAdjustment = "";
            string bodyWarehouse = "";
            //lblInformation.Visible = true;

            var dtStockInfo = GetAdjustmentTransferSA("A");
            var dtStockInfoT = GetAdjustmentTransferSA("B");
            if (dtStockInfo.Count > 0)
            {
                if (dtStockInfo.Count == 1)
                    bodyAdjustment = "1. AdjustmentNo   : " + dtStockInfo[0].AdjustmentNo.ToString();
                else
                    bodyAdjustment = "1. AdjustmentNo   : " + dtStockInfo[0].AdjustmentNo.ToString() + " S/D " + dtStockInfo[dtStockInfo.Count - 1].AdjustmentNo.ToString();
            }
            else
                bodyAdjustment = "1. AdjustmentNo   : <tidak ada>";

            if (dtStockInfoT.Count > 0)
            {
                if (dtStockInfoT.Count == 1)
                    bodyWarehouse = "2. WarehouseTrfNo : " + dtStockInfoT[0].WHTrfNo.ToString();
                else
                    bodyWarehouse = "2. WarehouseTrfNo : " + dtStockInfoT[0].WHTrfNo.ToString() + " S/D " + dtStockInfoT[dtStockInfoT.Count - 1].WHTrfNo.ToString();
            }
            else
                bodyWarehouse = "2. WarehouseTrfNo : <tidak ada>";

            try
            {
                return Json(new { success = true, data = bodyAdjustment + "\n\n"+ bodyWarehouse });
            }catch{
                return Json(new { success = false, message = "Error, terjadi kesalahan!" });
            }

        }

        private List<GetAdjustmentTransferSA> GetAdjustmentTransferSA(string opt)
        {
            if (opt == "A")
            {
                var query = string.Format(@"
                SELECT 
                a.AdjustmentNo,
                (SELECT COUNT(PartNo) FROM SpTrnIAdjustDtl WHERE AdjustmentNo IN (SELECT AdjustmentNo FROM SpTrnIAdjustHdr WHERE ReferenceNo = c.STHdrNo)) AS PartAdjust,
                (SELECT COUNT(PartNo) FROM SpTrnIWhTrfDtl WHERE WHTrfNo IN (SELECT WHTrfNo FROM SpTrnIWhTrfHdr WHERE ReferenceNo = c.STHdrNo)) AS PartRusak
                FROM
                SpTrnIAdjustHdr a
                LEFT JOIN (SELECT TOP 1 STHdrNo, CompanyCode, BranchCode from spTrnStockTakingHdr
                WHERE CreatedDate IN (SELECT MAX(CreatedDate) FROM spTrnStockTakingHdr WHERE BranchCode = '{1}')
                ORDER BY STHDrNo DESC) c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
                WHERE 
                a.ReferenceNo = c.STHdrNo	 AND
                a.CompanyCode = '{0}' AND
                a.BranchCode = '{1}'
            ", CompanyCode, BranchCode);
                return ctx.Database.SqlQuery<GetAdjustmentTransferSA>(query).ToList();
            }
            else {
                var query = string.Format(@"
                SELECT 
                a.WHTrfNo,
                (SELECT COUNT(PartNo) FROM SpTrnIAdjustDtl WHERE AdjustmentNo IN (SELECT AdjustmentNo FROM SpTrnIAdjustHdr WHERE ReferenceNo = c.STHdrNo)) AS PartAdjust,
                (SELECT COUNT(PartNo) FROM SpTrnIWhTrfDtl WHERE WHTrfNo IN (SELECT WHTrfNo FROM SpTrnIWhTrfHdr WHERE ReferenceNo = c.STHdrNo)) AS PartRusak
                FROM SpTrnIWHTrfHdr a 
                LEFT JOIN (SELECT TOP 1 STHdrNo, CompanyCode, BranchCode from spTrnStockTakingHdr
                WHERE CreatedDate IN (SELECT MAX(CreatedDate) FROM spTrnStockTakingHdr WHERE BranchCode = '{1}')
                ORDER BY STHDrNo DESC) c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
                WHERE 
                a.ReferenceNo = c.STHdrNo	 AND
                a.CompanyCode = '{0}' AND
                a.BranchCode = '{1}'
            ", CompanyCode, BranchCode);
                return ctx.Database.SqlQuery<GetAdjustmentTransferSA>(query).ToList();
            }
            

        }

        private bool SpTrnStockTakingHdrUpdate(string stHdrNo)
        {
            bool result = true;
            var recSTHeader = ctx.SpTrnStockTakingHdrs.Find(CompanyCode, BranchCode, stHdrNo);
            if (recSTHeader != null)
            {
                // update record header"
                recSTHeader.Status = "2";
                recSTHeader.LastUpdatedBy = CurrentUser.UserId;
                recSTHeader.LastUpdatedDate = DateTime.Now;
                try
                {
                    ctx.SaveChanges();
                    result = true;
                }
                catch {
                    result = false;
                }
            }
            return result;
        }

        private bool SpTrnStockTakingDtlUpdate(SpTrnStockTakingHdr recHeader)
        {
            bool result = true;
            var query = string.Format(@"
                UPDATE
                spTrnStockTakingDtl
            SET
                Status = 2,
                LastUpdatedBy = '{4}',
                LastUpdatedDate = '{3}'
            WHERE
                CompanyCode = '{0}' AND
                BranchCode = '{1}'  AND
                STHdrNo = '{2}'      
            ", CompanyCode, BranchCode, recHeader.STHdrNo, DateTime.Now, CurrentUser.UserId);

            try
            {
                ctx.Database.ExecuteSqlCommand(query);
                result = true;
            }
            catch {
                result = false;
            }
            return result;
        }

        private bool WHTransferForStockTakingTuning(List<SumStockTaking> dtPart, SpTrnStockTakingHdr recHdr, string warehouseCode)
        {
            string filterAdjust = "";

            var dt = ctx.LookUpDtls.Where(a => a.CompanyCode == CompanyCode && a.CodeID == TypeOfGoods).ToList();
            List<SumStockTaking> rowTransfer = new List<SumStockTaking>();

            foreach (var row in dt)
            {
                filterAdjust = string.Format("STDmgQty > 0 AND TypeOfGoods = '{0}'", row.LookUpValue);
                rowTransfer = dtPart.Where(a=>a.STDmgQty > 0 && a.TypeOfGoods == row.LookUpValue).ToList();
                if (rowTransfer.Count > 0)
                    if (!WHTransferForStockTakingNew(rowTransfer, recHdr, recHdr.WarehouseCode))
                        return false;
            }
            return true;
        }

        private bool WHTransferForStockTakingNew(List<SumStockTaking> dtSTDetail, SpTrnStockTakingHdr recHdr, string warehouseCode)
        {
            bool result = true;
            SpMstItemLoc oItemLocDao = new SpMstItemLoc();
            spMstItem oItemDao = new spMstItem();
            spMstItemPrice oItemPriceDao = new spMstItemPrice();

            SpMstItemLoc recItemLoc, recItemLocTemp = null;
            spMstItemPrice recItemPrice = null;
            spMstItem recItem = null;

            var recWHHeader = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, string.Empty);
            var recWHDetail = ctx.spTrnIWHTrfDtls.Find(CompanyCode, BranchCode,"", "", "", "");

            bool loop = false;
            int counterIndex = 0;
            decimal counterRows = dtSTDetail.Count;

            for (int x = 0; x < dtSTDetail.Count; x++)
            {
                //var toWHCode = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "WRCD" && p.SeqNo == 4).FirstOrDefault().LookUpValueName;
                //var recWHDetail = ctx.spTrnIWHTrfDtls.Find(CompanyCode, BranchCode, recWHHeader.WHTrfNo, dtSTDetail[x].PartNo.ToString(), warehouseCode, toWHCode);
                counterIndex++;
                // get OnHand quantity from spMstItemLoc table
                recItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, dtSTDetail[x].PartNo.ToString(), warehouseCode);
                recItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, dtSTDetail[x].PartNo.ToString());
                recItem = ctx.spMstItems.Find(CompanyCode, BranchCode, dtSTDetail[x].PartNo.ToString());

                if (Convert.ToDecimal(dtSTDetail[x].STDmgQty.ToString()) > 0)
                {
                    while (!loop)
                    {
                        // do warehouse transfer
                        if (recWHHeader == null) {
                            recWHHeader = new spTrnIWHTrfHdr() { 
                                CompanyCode = CompanyCode,
                                BranchCode = BranchCode,
                                WHTrfNo = string.Empty,
                                CreatedBy = CurrentUser.UserId,
                                CreatedDate = DateTime.Now
                            };
                        }
                        
                        recWHHeader.WHTrfDate = DateTime.Now;
                        recWHHeader.ReferenceNo = recHdr.STHdrNo;
                        recWHHeader.ReferenceDate = recHdr.CreatedDate;
                        //recWHHeader.Status = "2";
                        recWHHeader.Status = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "STAT" && p.SeqNo == 3).FirstOrDefault().LookUpValue;
                        recWHHeader.LastUpdateBy = CurrentUser.UserId;
                        recWHHeader.LastUpdateDate = DateTime.Now;
                        try
                        {
                            ctx.SaveChanges();
                            loop = true;
                        }
                        catch {
                            loop = false;
                        }
                    }

                    //if (recWHDetail == null)
                    //{
                        recWHDetail = new spTrnIWHTrfDtl() { 
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            WHTrfNo = recWHHeader.WHTrfNo,
                            PartNo = dtSTDetail[x].PartNo.ToString(),
                            Qty = Convert.ToDecimal(dtSTDetail[x].STDmgQty.ToString()),
                            RetailPrice = recItemPrice.RetailPrice,
                            RetailPriceInclTax = recItemPrice.RetailPriceInclTax,
                            CostPrice = recItemPrice.CostPrice,
                            FromWarehouseCode = warehouseCode,
                            FromLocationCode = recItemLoc.LocationCode,
                            ToWarehouseCode = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "WRCD" && p.LookUpValue == "99").FirstOrDefault().ParaValue,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now
                        };
                        ctx.spTrnIWHTrfDtls.Add(recWHDetail);
                    //}
                   
                    //recItemLocTemp = SpMstItemLocBLL.GetRecord(user.CompanyCode, user.BranchCode, recSTDetail.PartNo, recWHDetail.ToWarehouseCode);
                    recItemLocTemp = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, dtSTDetail[x].PartNo.ToString(), recWHDetail.ToWarehouseCode);
                    if (recItemLocTemp != null)
                    {
                        recWHDetail.ToLocationCode = recItemLocTemp.LocationCode;

                    }
                    else
                    {
                        recWHDetail.ToLocationCode = "-";
                    }
                    recWHDetail.ReasonCode = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "RSWT" && p.SeqNo == 2).FirstOrDefault().ParaValue;// correction stock for reason warehouse transfer 
                    recWHDetail.MovingCode = recItem.MovingCode;
                    recWHDetail.LastUpdateBy = CurrentUser.UserId;
                    recWHDetail.LastUpdateDate = DateTime.Now;
                    try
                    {
                        ctx.SaveChanges();
                        loop = true;
                    }
                    catch{
                        loop = false;
                    }
                    
                    var dt = ctx.spTrnIWHTrfDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.WHTrfNo == recWHDetail.WHTrfNo && a.PartNo == recWHDetail.PartNo).ToList();
                    string epesan = "";
                    result = updateQuantity(dt, ref epesan);
                }

                
            }// end-of-Foreach
            return result;
        }

        public bool updateQuantity(List<spTrnIWHTrfDtl> dtDetail, ref string ePesan)
        {
            bool stat = false;
            int caseStock = 0;
            bool result = false;
            //decimal QtyTujuan = 0;
            decimal QtyReal = 0;
            string[] warehouseRest = { "97", "98", "99", "X1", "X2", "X3" };

            if (dtDetail.Count > 0)
            {
                for (int x = 0; x < dtDetail.Count; x++)
                {
                    if (x == dtDetail.Count - 1)
                        stat = true;

                    // NOTES : This line will insert new item locations based on the the "events" value
                    var recordLocTo = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, dtDetail[x].PartNo.ToString(),
                                  dtDetail[x].ToWarehouseCode.ToString());

                    var recordLocFrom = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode,
                                    dtDetail[x].PartNo.ToString(),
                                    dtDetail[x].FromWarehouseCode.ToString());
                    if (recordLocFrom.OnHand == null) {
                        recordLocFrom.OnHand = 0;
                    }
                    if (recordLocFrom.AllocationSP == null) {
                        recordLocFrom.AllocationSP = 0;
                    }
                    // TO DO : This line of code below will prepare the data
                    var recordItem = ctx.spMstItems.Find(CompanyCode, BranchCode,
                                 dtDetail[x].PartNo.ToString());

                    var recordDetail = ctx.spTrnIWHTrfDtls.Find(CompanyCode, BranchCode, dtDetail[x].WHTrfNo.ToString(),
                                   dtDetail[x].PartNo.ToString(), dtDetail[x].FromWarehouseCode.ToString(), dtDetail[x].ToWarehouseCode.ToString());

                    var recordHeader = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, dtDetail[x].WHTrfNo.ToString());

                    if (recordLocTo == null)
                    {
                        recordLocTo = new SpMstItemLoc() { 
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            PartNo = dtDetail[x].PartNo.ToString(),
                            WarehouseCode = dtDetail[x].ToWarehouseCode.ToString(),
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                            LocationCode = "-"
                        };
                        ctx.SpMstItemLocs.Add(recordLocTo);
                    }

                    // TO DO : This line of code will update the items
                    if (recordDetail != null)
                    {
                        string qty = dtDetail[x].Qty.ToString();

                        // TO DO : This line of code will check again the Qty
                        decimal avaQty = Convert.ToDecimal(recordLocFrom.OnHand - (recordLocFrom.AllocationSP
                            + recordLocFrom.AllocationSR + recordLocFrom.AllocationSL
                            + recordLocFrom.ReservedSP + recordLocFrom.ReservedSL
                            + recordLocFrom.ReservedSR));

                        QtyReal = decimal.Parse(qty);

                        if (avaQty < QtyReal)
                        {
                            // TO DO : This line of code will handle the error that caused not by exception
                            ePesan = "Qty yang tersedia tidak mencukupi utk Claim";
                            throw new Exception(ePesan);
                        }

                        // TO DO : This line of code below will update the spMstItems
                        // ==========================================================

                        caseStock = SetCaseUpdateStock(dtDetail[x].FromWarehouseCode.ToString(),
                                    dtDetail[x].ToWarehouseCode.ToString(), warehouseRest);

                        switch (caseStock)
                        {   /*  Claim Part [Shortage, Damage, Wrong Part]*/
                            case 1:
                                result = new SpMstItemLocBLL().UpdateStock(ctx, dtDetail[x].PartNo.ToString(),
                                         dtDetail[x].FromWarehouseCode.ToString(), (QtyReal * -1), 0, 0, 0, string.Empty)
                                         &&
                                         UpdateStockWarehouse(dtDetail[x].PartNo.ToString(),
                                         dtDetail[x].ToWarehouseCode.ToString(), QtyReal, 0, 0, 0);
                                break;
                            /*  Receiving Claim Part [Shortage, Damage, Wrong Part]*/
                            case 2:
                                result = new SpMstItemLocBLL().UpdateStock(ctx, dtDetail[x].PartNo.ToString(),
                                         dtDetail[x].ToWarehouseCode.ToString(), QtyReal, 0, 0, 0, string.Empty)
                                         &&
                                         UpdateStockWarehouse(dtDetail[x].PartNo.ToString(),
                                         dtDetail[x].FromWarehouseCode.ToString(), (QtyReal * -1), 0, 0, 0);
                                break;
                            /*  Warehouse Transfer */
                            case 3:
                                result = UpdateStockWarehouse(dtDetail[x].PartNo.ToString(),
                                         dtDetail[x].FromWarehouseCode.ToString(), (QtyReal * -1), 0, 0, 0)
                                         &&
                                         UpdateStockWarehouse(dtDetail[x].PartNo.ToString(),
                                         dtDetail[x].ToWarehouseCode.ToString(), QtyReal, 0, 0, 0);
                                break;
                        }

                        
                        if (!result)
                        {
                            ePesan = "Proses Update Persediaan Gagal";
                            result = false;
                            throw new Exception(ePesan);
                        }

                        if (result)
                            result = MovementLog(recordHeader.WHTrfNo, recordHeader.WHTrfDate, dtDetail[x].PartNo.ToString(), dtDetail[x].ToWarehouseCode.ToString(),
                                dtDetail[x].FromWarehouseCode.ToString(), false, "", Convert.ToDecimal(dtDetail[x].Qty.ToString()));

                        if (!result)
                        {
                            ePesan = "Update Movement Log Transaksi Gagal";
                            result = false;
                            throw new Exception(ePesan);
                        }

                        if (stat)
                        {
                            recordHeader.Status = "2";
                            recordHeader.LastUpdateBy = CurrentUser.UserId;
                            recordHeader.LastUpdateDate = DateTime.Now;
                            try{
                                ctx.SaveChanges();
                                result = true;
                            }catch{
                                result = false;
                            }

                            if (!result)
                            {
                                ePesan = "Update Status Warehouse TRF Header Gagal";
                                result = false;
                                throw new Exception(ePesan);
                            }
                        }

                    }
                    else
                    {
                        ePesan = "Detail Data transaksi tidak ada";
                        result = false;
                        throw new Exception(ePesan);
                    }
                }
            }
            return result;
        }

        private bool MovementLog(string docno, DateTime? docdate, string partno, string whcodeTo, string whcodeFrom, bool caseAdj, string opAdjust, decimal qty)
        {
            if (docdate == null)
            {
                docdate = DateTime.Now;
            }
            string signCode = "OUT";
            string subSignCode = "";
            bool result = false;
            SpTrnIMovement oSpTrnIMovementDao = new SpTrnIMovement();
            var oItems = ctx.spMstItems.Find(CompanyCode, BranchCode, partno);
            var oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, partno);
            var oItemLocFrom = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcodeFrom);
            var oItemLocTo = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcodeTo);
            var oIMovement = ctx.SpTrnIMovements.Find(CompanyCode, BranchCode, docno, docdate, DateTime.Now);

            //TO DO : Nilai dari subsigncode dan signcode perlu dimasukkan dalam commonBLL
            if (caseAdj)
            {
                subSignCode = "ADJ";
                if (opAdjust == "+")
                    signCode = "IN";
            }
            else
                subSignCode = "WTR";

            // TO DO : Insert to Item Movement record
            if (whcodeTo != "")
            {
                
                
                if (oItemLocTo != null)
                {
                    oIMovement = new SpTrnIMovement() { 
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        DocNo = docno,
                        DocDate = (DateTime)docdate,
                        CreatedDate = DateTime.Now,
                        CreatedBy = CurrentUser.UserId

                    };
                    ctx.SpTrnIMovements.Add(oIMovement);
                }
                    oIMovement.WarehouseCode = oItemLocTo.WarehouseCode;
                    oIMovement.LocationCode = oItemLocTo.LocationCode;
                    oIMovement.PartNo = oItemLocTo.PartNo;
                    oIMovement.SignCode = "IN";
                    oIMovement.SubSignCode = subSignCode;
                    oIMovement.Qty = qty;
                    oIMovement.Price = oItemPrice.RetailPrice;
                    oIMovement.CostPrice = oItemPrice.CostPrice;
                    oIMovement.ABCClass = oItems.ABCClass;
                    oIMovement.MovingCode = oItems.MovingCode;
                    oIMovement.ProductType = oItems.ProductType;
                    oIMovement.PartCategory = oItems.PartCategory;

                    try
                    {
                        ctx.SaveChanges();
                        result = true;
                    }
                    catch {
                        result = false;
                    }
            }

            if (oItemLocFrom != null && oItemPrice != null && oItems != null)
            {
                if (oIMovement == null) {
                    oIMovement = new SpTrnIMovement() {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        DocNo = docno,
                        DocDate = (DateTime)docdate,
                        CreatedDate = DateTime.Now,
                        CreatedBy = CurrentUser.UserId
                    };
                }
                
                oIMovement.WarehouseCode = oItemLocFrom.WarehouseCode;
                oIMovement.LocationCode = oItemLocFrom.LocationCode;
                oIMovement.PartNo = oItemLocFrom.PartNo;
                oIMovement.SignCode = signCode;
                oIMovement.SubSignCode = subSignCode;
                oIMovement.Qty = qty;
                oIMovement.Price = oItemPrice.RetailPrice;
                oIMovement.CostPrice = oItemPrice.CostPrice;
                oIMovement.ABCClass = oItems.ABCClass;
                oIMovement.MovingCode = oItems.MovingCode;
                oIMovement.ProductType = oItems.ProductType;
                oIMovement.PartCategory = oItems.PartCategory;

                try
                {
                    ctx.SaveChanges();
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }

        public bool UpdateStockWarehouse(string partno, string whcode, decimal onhand, int allocation, int backorder, int reserved)
        {
            bool result = false;
            var oItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, partno, whcode);

            if (oItemLoc != null)
            {
                //TODO : Tambahkan check result untuk yang hasilnya negatif
                if (Math.Abs(onhand) > 0)
                {
                    oItemLoc.OnHand += onhand;

                    // Tambahkan check result untuk yang ItemLoc negatif
                    if (oItemLoc.OnHand < 0)
                    {
                        throw new Exception(string.Format("OnHand untuk Part = {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.OnHand));
                    }
                }

                if (Math.Abs(allocation) > 0)
                {
                    oItemLoc.AllocationSP += allocation;

                    // Tambahkan check result untuk yang ItemLoc negatif
                    if (oItemLoc.AllocationSP < 0)
                    {
                        throw new Exception(string.Format("AllocationSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSP));
                    }
                }

                if (Math.Abs(backorder) > 0)
                {
                    oItemLoc.BackOrderSP += backorder;

                    // Tambahkan check result untuk yang ItemLoc negatif
                    if (oItemLoc.BackOrderSP < 0)
                    {
                        throw new Exception(string.Format("BackOrderSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSP));
                    }
                }

                if (Math.Abs(reserved) > 0)
                {
                    oItemLoc.ReservedSP += reserved;

                    // Tambahkan check result untuk yang ItemLoc negatif
                    if (oItemLoc.ReservedSP < 0)
                    {
                        throw new Exception(string.Format("ReservedSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.ReservedSP));
                    }
                }
                oItemLoc.LastUpdateDate = DateTime.Now;
                oItemLoc.LastUpdateBy = CurrentUser.UserId;
                try
                {
                    ctx.SaveChanges();
                    result = true;
                }
                catch {
                    result = false;
                }

            }
            return result;
        }

        public int SetCaseUpdateStock(string fromWH,string toWH,string[] whValid)
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

        private bool AdjustmentForStockTakingTuning(List<SumStockTaking> dtPart, SpTrnStockTakingHdr recHdr, string WarehouseCode)
        {
            string filterAdjust = "";
            var dt = ctx.LookUpDtls.Where(a => a.CompanyCode == CompanyCode && a.CodeID == "TPGO").ToList();
            //DataRow[] rowAdjust;
            foreach (var row in dt)
            {
                filterAdjust = string.Format("OnHandQty <> STQty AND TypeOfGoods = '{0}'", row.LookUpValue);
                var rowAdjust = dtPart.Where(a=>a.OnHandQty == a.STQty && a.TypeOfGoods == row.LookUpValue).ToList();
                if (rowAdjust.Count > 0)
                    if (!AdjustmentForStockTakingNew(rowAdjust, recHdr, recHdr.WarehouseCode))
                        return false;
            }
            return true;
        }

        public bool AdjustmentForStockTakingNew(List<SumStockTaking> dtDetail, SpTrnStockTakingHdr recHdr, string warehouseCode)
        {
            SpMstItemLoc oItemLocDao = new SpMstItemLoc();
            spMstItem oItemDao = new spMstItem();
            spMstItemPrice oItemPriceDao = new spMstItemPrice();
            SpTrnIAdjustHdr oAdjHeaderDao = new SpTrnIAdjustHdr();
            SpTrnIAdjustHdr oSpTrnIAdjustHdrDao = new SpTrnIAdjustHdr();

            SpMstItemLoc recItemLoc = null;
            spMstItemPrice recItemPrice = null;
            spMstItem recItem = null;
           // SpTrnIAdjustHdr recAdjHeader = null;
            var recAdjHeader = ctx.SpTrnIAdjustHdrs.Find(CompanyCode, BranchCode, string.Empty);
            var recAdjDetil = ctx.SpTrnIAdjustDtls.Find(CompanyCode, BranchCode, "", "", "");
            bool loop = false;
            int counterIndex = 0;
            decimal counterRows = dtDetail.Count;

            for (int x = 0; x < dtDetail.Count; x++)
            {
                //var recAdjDetil = ctx.SpTrnIAdjustDtls.Find(CompanyCode, BranchCode, recAdjHeader.AdjustmentNo, warehouseCode, dtDetail[x].PartNo.ToString());
            
                if (dtDetail[x].STQty == null) {
                    dtDetail[x].STQty = 0;
                }
                counterIndex++;
                // get OnHand quantity from spMstItemLoc table
                recItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, dtDetail[x].PartNo.ToString(), warehouseCode);
                recItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, dtDetail[x].PartNo.ToString());
                recItem = ctx.spMstItems.Find(CompanyCode, BranchCode, dtDetail[x].PartNo.ToString());
                if (recItemLoc.OnHand == null) {
                    recItemLoc.OnHand = 0;
                }
                // prepare quantity adjustment and adjustment code 
                decimal qtyAdjust = 0;
                string strCodeAdjust = string.Empty;
                if (Convert.ToDecimal(dtDetail[x].STQty.ToString()) > Convert.ToDecimal(dtDetail[x].OnHandQty.ToString()))
                {
                    qtyAdjust = (decimal)dtDetail[x].STQty - (decimal)recItemLoc.OnHand;
                    strCodeAdjust = "+";
                }
                else
                {
                    qtyAdjust = (decimal)recItemLoc.OnHand - (decimal)dtDetail[x].STQty;
                    strCodeAdjust = "-";
                }

                //// save adjustment header
                while (!loop)
                {
                    //if (recAdjHeader == null) {
                        recAdjHeader = new SpTrnIAdjustHdr()
                        { 
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            AdjustmentNo = GetNewDocumentNo("ADJ", DateTime.Now),
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now
                        };
                        ctx.SpTrnIAdjustHdrs.Add(recAdjHeader);
                    //}

                    recAdjHeader.TypeOfGoods = dtDetail[x].TypeOfGoods.ToString();
                    recAdjHeader.AdjustmentDate = DateTime.Now;
                    recAdjHeader.ReferenceNo = recHdr.STHdrNo;
                    recAdjHeader.ReferenceDate = recHdr.CreatedDate;
                    recAdjHeader.Status = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "STAT" && p.SeqNo == 3).FirstOrDefault().LookUpValue;
                    recAdjHeader.LastUpdateBy = CurrentUser.UserId;
                    recAdjHeader.LastUpdateDate = DateTime.Now;

                    try
                    {
                        ctx.SaveChanges();
                        loop = true;
                    }
                    catch {
                        loop = false;
                    }

                    
                }

                // save adjustment detail
                //if (recAdjDetil == null) {
                    recAdjDetil = new SpTrnIAdjustDtl()
                    { 
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        AdjustmentNo = recAdjHeader.AdjustmentNo,
                        WarehouseCode = warehouseCode,
                        PartNo  = dtDetail[x].PartNo.ToString()
                    };
                    ctx.SpTrnIAdjustDtls.Add(recAdjDetil);
                //}
                
                recAdjDetil.LocationCode = recItemLoc.LocationCode;
                recAdjDetil.AdjustmentCode = strCodeAdjust;
                recAdjDetil.QtyAdjustment = qtyAdjust;
                recAdjDetil.RetailPrice = recItemPrice.RetailPrice;
                recAdjDetil.RetailPriceInclTax = recItemPrice.RetailPriceInclTax;
                recAdjDetil.CostPrice = recItemPrice.CostPrice;
                recAdjDetil.ReasonCode = ctx.LookUpDtls.Find(CompanyCode, "RSAD", "KORK").ParaValue;// correction stock for reason warehouse transfer ; 
                recAdjDetil.MovingCode = recItem.MovingCode;
                try
                {
                    ctx.SaveChanges();
                    loop = true;
                }
                catch{
                    loop = false;
                }
                
            }
            return loop;
        }

        public bool UpdateLocationItemLocForStockTaking(string WarehouseCode, List<SumStockTaking> dtDetail, SpTrnStockTakingHdr recHdr)
        {
            if (dtDetail.Count == 0)
                throw new Exception("Stock Taking to master is failed");

            //string sort = "STNo, PartNo, SeqNo ASC";
            string queryUpdateLocation = "";
            int counter = 0;
            int updResult = 0;
            decimal counterRows = dtDetail.Count;
            var locStockDtl = SelectStockTaking(recHdr.STHdrNo);

            foreach (var row in dtDetail)
            {
                counter++;
                string queryLoc = "";
                int inc = 0;
                string filter = "PartNo = '" + row.PartNo.ToString() + "'";
                string[] location = { "", "", "", "", "", "", "" };

                var rowDtl = locStockDtl.Where(a => a.PartNo == row.PartNo.ToString()).OrderBy(x=>x.STNo).ThenBy(x=>x.PartNo).ThenBy(x=>x.SEqNo).ToList();
                
                for (int x = 0; x < rowDtl.Count; x++)
                {
                    
                        location[inc] = rowDtl[x].LocationCode.ToString();
                    inc++;
                }
                queryLoc = "UPDATE spMstItemLoc SET LocationCode = '" + location[0] + "', LocationSub1 = '" + location[1] + "', " +
                    "LocationSub2 = '" + location[2] + "', LocationSub3 = '" + location[3] + "', LocationSub4 = '" + location[4] + "',LocationSub5 = '" + location[5] + "', " +
                    "LocationSub6 = '" + location[6] + "', LastUpdateBy = '" + CurrentUser.UserId + "', LastUpdateDate = '" + DateTime.Now.ToString("MM/dd/yyyy") + "' WHERE  CompanyCode = '" + CompanyCode + "' AND " +
                    "BranchCode = '" + BranchCode + "' AND PartNo = '" + row.PartNo.ToString() + "' AND WarehouseCode = '" + WarehouseCode + "' ";
                queryUpdateLocation += queryLoc;
                if ((counter % 50) == 0)
                {
                    //ctx.Database.ExecuteSqlCommand(queryUpdateLocation);
                    updResult = ctx.Database.ExecuteSqlCommand(queryUpdateLocation);
                    if (!(updResult > 0)) return false;
                    else
                        queryUpdateLocation = "";
                }

            }

            if (queryUpdateLocation != "")
            {
                var query = @"" + queryUpdateLocation + "";
                updResult = ctx.Database.ExecuteSqlCommand(query);
                if (!(updResult > 0)) return false;
            }

            return true;
        }

        private List<IsValidSTAnalyze> SelectStockTaking(string STHdrNo)
        {
            var query = string.Format(@"
                SELECT STNo, SeqNo, PartNo, LocationCode FROM SpTrnStockTakingDtl WHERE
                CompanyCode = '{0}' AND
                BranchCode = '{1}' AND
                STHdrNo = '{2}'               
                ORDER BY STNo, SeqNo ASC
            ", CompanyCode, BranchCode, STHdrNo);
            return ctx.Database.SqlQuery<IsValidSTAnalyze>(query).ToList();
        }

        public List<SumStockTaking> SelectSumStockTakingDataTable(string WarehouseCode)
        {
            var query = string.Format(@"
                SELECT a.CompanyCode, a.BranchCode, a.STHdrNo, a.PartNo, c.TypeOfGoods,
                  SUM(a.OnHandQty) as OnHandQty, SUM(a.STQty) as STQty, 
                  SUM(a.StDmgQty) as StDmgQty, c.TypeOfGoods
                FROM SpTrnStockTakingDtl a with(nolock, nowait), SpTrnStockTakingHdr b with(nolock, nowait),SpMstItems c with(nolock, nowait) 
                WHERE a.CompanyCode = '{0}'
	                AND a.BranchCode = '{1}'
                    AND a.CompanyCode = b.CompanyCode
                    AND a.BranchCode = b.BranchCode                
	                AND b.WarehouseCode = '{2}'
	                AND b.STHdrNo = a.STHdrNo
                    AND c.CompanyCode = b.CompanyCOde 	                
                    AND c.BranchCode = b.BranchCode
	                AND c.PartNo = a.PartNo
	                AND b.STHdrNo = a.STHdrNo
                    AND a.status = 1
                    AND b.status = 1
                GROUP BY a.CompanyCode, a.BranchCode, a.STHdrNo, a.PartNo, c.TypeOfGoods
            ", CompanyCode, BranchCode, WarehouseCode);
            return ctx.Database.SqlQuery<SumStockTaking>(query).ToList();
            
        }

        private bool IsBlankExist(string STHdrNo)
        {
 	        var query = string.Format(@"
            SELECT
	        TOP 1 *
            FROM
	            spTrnStockTakingTemp
            WHERE
	        CompanyCode = '{0}'
	        AND BranchCode = '{1}'
	        AND STHdrNo = '{2}'
	        AND PartNo = ''
	        AND Status < 2
            ", CompanyCode, BranchCode, STHdrNo);
            var row = ctx.Database.SqlQuery<SpTrnStockTakingTemp>(query).ToList();
            return row.Count > 0 ? true : false;
        }

        private List<partno> GetPartStatDtl(string STHdrNo)
        {
 	        var query = string.Format(@"
                SELECT PartNo
            FROM SpTrnStockTakingDtl
            WHERE
            CompanyCode = '{0}' AND
            BranchCode = '{1}'   AND
            STHdrNo = '{2}'         AND
            Status NOT IN (1,2)
            ", CompanyCode, BranchCode, STHdrNo);
            return ctx.Database.SqlQuery<partno>(query).ToList();
        }

        public bool CheckPartStockEntry(string SthdrNo)
        {
 	        bool check = true;
            var dtPartTemp = GetPartNoTemp(SthdrNo);
            var dtPartDetail = ctx.SpTrnStockTakingDtls.Where(a=>a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.STHdrNo == SthdrNo).ToList();
            if (dtPartTemp.Count <= dtPartDetail.Count)
                {
                    foreach (var rowTemp in dtPartTemp)
                    {
                        if (check)
                        {
                            foreach (var rowDetail in dtPartDetail)
                            {
                                check = false;
                                if (rowTemp.PartNo.ToString().ToUpper() == rowDetail.PartNo.ToString().ToUpper())
                                {
                                    check = true;
                                    break;
                                }
                            }
                        }
                        else
                            break;
                    }
                }
                else
                    check = false;

                return check;
        }

        public List<partno> GetPartNoTemp(string SthdrNo)
        {
 	        var query = string.Format(@"
            SELECT
                        DISTINCT(PartNo)
                    FROM SpTrnStockTakingTemp
                    WHERE 
                        CompanyCode = '{0}' AND
                        BranchCode = '{1}'   AND
                        STHdrNo = '{2}'  AND
                        PartNo <> ''
            ", CompanyCode, BranchCode, SthdrNo);
            return ctx.Database.SqlQuery<partno>(query).ToList();
        }
        public class partno
        {
                public string PartNo {get; set;}
        }

        private bool DateTransValidation(DateTime date, ref string msg)
        {
            var MSG_5006 = "{0} tidak sesuai dengan {1}";
            if (CurrentUser == null)
            {
                msg = "Invalid user";
                return false;
            }

            string pcentre = ProfitCenter;
            var currDate = DateTime.Now.Date;
            string errMsg1 = string.Format((MSG_5006), "Tanggal transaksi", "periode transaksi");
            string errMsg2 = string.Format((MSG_5006), "Tanggal Transaksi", "Tanggal Server");
            string errMsg3 = string.Format("Periode sedang di locked");
            string errMsg4 = string.Format("Tanggal Transaksi lebih kecil dari tanggal [TransDate]");

            date = date.Date;

            // 100 : Check for Unit (Sales) 
            if (pcentre.Equals("100"))
            {
                GnMstCoProfileSales oSales = ctx.GnMstCoProfileSalesmant.Find(CompanyCode, BranchCode);
                if (oSales != null)
                {
                    if (oSales.TransDate.Equals(DBNull.Value) || oSales.TransDate < new DateTime(1900, 1, 2)) oSales.TransDate = currDate;
                    if (date >= oSales.PeriodBeg && date <= oSales.PeriodEnd)
                    {
                        if (date <= currDate)
                        {
                            if (date >= oSales.TransDate)
                            {
                                if (oSales.isLocked == true)
                                {
                                    msg = errMsg3;
                                    return false;
                                }
                            }
                            else
                            {
                                errMsg4 = errMsg4.Replace("[TransDate]", oSales.TransDate.ToString());
                                msg = errMsg4;
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            msg = errMsg2;
                            return false;
                        }
                    }
                    else
                    {
                        msg = errMsg1;
                        return false;
                    }
                }
            }

            // 200 : Check for Service 
            if (pcentre.Equals("200"))
            {
                GnMstCoProfileService oService = ctx.GnMstCoProfileServices.Find(CompanyCode, BranchCode);
                if (oService != null)
                {
                    if (oService.TransDate.Equals(DBNull.Value) || oService.TransDate < new DateTime(1900, 1, 2)) oService.TransDate = currDate;
                    if (date >= oService.PeriodBeg.Date && date <= oService.PeriodEnd.Date)
                    {
                        if (date <= currDate)
                        {
                            if (date >= oService.TransDate)
                            {
                                if (oService.isLocked == true)
                                {
                                    msg = errMsg3;
                                    return false;
                                }
                            }
                            else
                            {
                                errMsg4 = errMsg4.Replace("[TransDate]", oService.TransDate.ToString());
                                msg = errMsg4;
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            msg = errMsg2;
                            return false;
                        }
                    }
                    else
                    {
                        msg = errMsg1;
                        return false;
                    }
                }
            }

            // 300 : Check for Sparepart 
            if (pcentre.Equals("300"))
            {
                GnMstCoProfileSpare oSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                if (oSpare != null)
                {
                    if (oSpare.TransDate.Equals(DBNull.Value) || oSpare.TransDate < new DateTime(1900, 1, 2)) oSpare.TransDate = currDate;
                    if (date >= oSpare.PeriodBeg.Date && date <= oSpare.PeriodEnd.Date)
                    {
                        if (date <= currDate)
                        {
                            if (date >= oSpare.TransDate.Date)
                            {
                                if (oSpare.isLocked == true)
                                {
                                    msg = errMsg3;
                                    return false;
                                }
                            }
                            else
                            {
                                errMsg4 = errMsg4.Replace("[TransDate]", oSpare.TransDate.Date.ToString("dd-MMM-yyyy"));
                                msg = errMsg4;
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            msg = errMsg2;
                            return false;
                        }
                    }
                    else
                    {
                        msg = errMsg1;
                        return false;
                    }
                }
            }


            msg = "Profit Center not valid";
            return false;
        }
    }
}
