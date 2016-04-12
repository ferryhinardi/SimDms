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
using System.Text;
using System.Transactions;

namespace SimDms.Sparepart.Controllers.Api
{
    public class PSOController : BaseController
    {
        //private spTrnPSUGGORHdr record;
        private bool result = false;
        private string msg = "";
        private const int
            SUGGOR = 10,
            REORDER = 20,
            POS = 30;
        private int flagProses;
        DataTable dtTemp;

        public JsonResult Index()
        {
            var partType = ctx.LookUpDtls.Find(CompanyCode, "TPGO", TypeOfGoods).LookUpValueName;

            return Json(new { PartType = partType });
        }

        public JsonResult GetStatus(string suggorNo)
        {
            var Hdr = ctx.spTrnPSUGGORHdrs.Find(CompanyCode, BranchCode, suggorNo);

            if (Hdr != null)
            {
                var lblStatus = SetStatusLabel(Hdr.Status);
                return Json(new { lblstatus = lblStatus});
            }
            return Json(null);
        }

        public JsonResult getTablePSO(spTrnPSUGGORHdr record)
        {
            var table = ctx.spTrnPSUGGORDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SuggorNo == record.SuggorNo);
            return Json(table);
        }

        public JsonResult saveDetail(spTrnPSUGGORDtl model)
        {
            var Hdr = ctx.spTrnPSUGGORHdrs.Find(CompanyCode, BranchCode, model.SuggorNo);
            var oldStat = Hdr.Status;
            var Dtl = ctx.spTrnPSUGGORDtls.Find(CompanyCode, BranchCode, model.SuggorNo, model.PartNo);
            if (Dtl != null)
            {
                Dtl.SuggorCorrecQty = model.SuggorCorrecQty;
                Dtl.LastUpdateBy = CurrentUser.UserId;
                Dtl.LastUpdateDate = DateTime.Now;

                if (flagProses == REORDER)
                {
                    Hdr.Status = "0";
                    Hdr.LastUpdateBy = CurrentUser.UserId;
                    Hdr.LastUpdateDate = DateTime.Now;
                }

                try
                {
                    ctx.SaveChanges();
                    var table = ctx.spTrnPSUGGORDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SuggorNo == model.SuggorNo);
                    return Json(new { success = true, table = table, lblstatus = SetStatusLabel(flagProses == REORDER ? Hdr.Status : oldStat) });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json("");
        }
       
        public JsonResult DoWork(spTrnPSUGGORHdr model)
        {
            DataSet dsSuggor = ProcessSuggor(model);
            int counterCheck = 0;
            dtTemp = new DataTable();

            DataTable dtSuggor = dsSuggor.Tables[1];
            dtSuggor.DefaultView.RowFilter = "IsValid=1 AND SuggorQty > 0";
            if (dtSuggor.DefaultView.ToTable().Rows.Count == 0)
            {
                msg = "Tidak Ada Suggor Yang Di Proses";
                return Json(new { success = false, message = msg });
            }

            dtSuggor.DefaultView.Sort = "[" + dtSuggor.Columns[1].ColumnName + "] asc";

            for (int i = 0; i < dtSuggor.DefaultView.ToTable().Rows.Count; i++)
            {
                dtSuggor.DefaultView[i]["SeqNo"] = i + 1;
                counterCheck = i + 1;
            }

            flagProses = SUGGOR;
            dtTemp = dtSuggor.DefaultView.ToTable();
            return Json(dtTemp);
        }

        public JsonResult ProcessSuggorSave(spTrnPSUGGORHdr header, List<spTrnPSUGGORDtl> dtDetail)
        {
            decimal counterRows = 0;
            int counterIndex = 0;
            bool result = false;
            var suggorNo = "";
            try
            {
                var record = new spTrnPSUGGORHdr();
                record.CompanyCode = CompanyCode;
                record.BranchCode = BranchCode;
                record.SuggorNo = GetNewDocumentNo("SGR", DateTime.Now);
                if (record.SuggorNo.EndsWith("X")) return Json(new { success = false, message = "Dokumen No SUGGOR belum ada di Master Dokumen" });
                record.SuggorDate = DateTime.Now;
                record.TypeOfGoods = TypeOfGoods;
                record.POSNo = header.POSNo;
                record.POSDate = DateTime.Now;
                record.SupplierCode = header.SupplierCode;
                record.ProductType = ProductType;
                record.MovingCode = header.MovingCode;
                record.OrderType = header.OrderType;
                record.Status = "0";
                record.PrintSeq = 0;
                record.IsVoid = false;

                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;

                ctx.spTrnPSUGGORHdrs.Add(record);

                result = ctx.SaveChanges() > 0;
                suggorNo = record.SuggorNo;
                if (result)
                {
                    counterRows = dtDetail.Count();
                    foreach (var row in dtDetail.ToList())
                    {
                        counterIndex++;

                        spTrnPSUGGORDtl suggorDtl = new spTrnPSUGGORDtl();
                        suggorDtl.CompanyCode = record.CompanyCode;
                        suggorDtl.BranchCode = record.BranchCode;
                        suggorDtl.SuggorNo = record.SuggorNo;
                        suggorDtl.PartNo = row.PartNo;
                        suggorDtl.SeqNo = row.SeqNo;
                        suggorDtl.OnHand = row.OnHand;
                        suggorDtl.OnOrder = row.OnOrder;
                        suggorDtl.InTransit = row.InTransit;
                        suggorDtl.AllocationSL = row.AllocationSL;
                        suggorDtl.AllocationSP = row.AllocationSP;
                        suggorDtl.AllocationSR = row.AllocationSR;
                        suggorDtl.BackOrderSL = row.BackOrderSL;
                        suggorDtl.BackOrderSP = row.BackOrderSP;
                        suggorDtl.BackOrderSR = row.BackOrderSR;
                        suggorDtl.ReservedSL = row.ReservedSL;
                        suggorDtl.ReservedSP = row.ReservedSP;
                        suggorDtl.ReservedSR = row.ReservedSR;
                        suggorDtl.DemandAvg = row.DemandAvg;
                        suggorDtl.OrderPoint = row.OrderPoint;
                        suggorDtl.SafetyStock = row.SafetyStock;
                        suggorDtl.isExistInItems = row.isExistInItems;

                        suggorDtl.AvailableQty = row.AvailableQty;
                        suggorDtl.SuggorQty = row.SuggorQty;
                        suggorDtl.SuggorCorrecQty = row.SuggorCorrecQty;
                        suggorDtl.ProductType = record.ProductType;
                        suggorDtl.PartCategory = row.PartCategory;
                        suggorDtl.CreatedBy = CurrentUser.UserId;
                        suggorDtl.CreatedDate = DateTime.Now;
                        suggorDtl.LastUpdateBy = CurrentUser.UserId;
                        suggorDtl.LastUpdateDate = DateTime.Now;

                        spMstItemPrice oItemPrice = ctx.spMstItemPrices.Find(suggorDtl.CompanyCode, suggorDtl.BranchCode, suggorDtl.PartNo);
                        if (oItemPrice != null)
                        {
                            suggorDtl.PurchasePrice = oItemPrice.PurchasePrice;
                            suggorDtl.CostPrice = oItemPrice.CostPrice;
                        }

                        // start save subs data 
                        // get history data 
                        int intRowCount = 0;
                        double totDemand = 0;
                        string query = "";

                        List<SpMstItemModNew> list = fc_SelectAllModifikasi(suggorDtl.PartNo);
                        foreach (SpMstItemModNew itemMod in list)
                        {
                            if (record.SuggorDate.Value.AddMonths(-6).Year < record.SuggorDate.Value.Year)
                            {
                                query = @"
                                   SELECT PARTNO, SUM(I) AS I,SUM(II) AS II,SUM(III) AS III,SUM(IV) AS IV,SUM(V) AS V,SUM(VI) AS VI
                                   FROM
                                    (
                                        SELECT @p2 AS PartNo, 0 AS I, 
                                            0 AS II, 0 AS III, 0 AS IV,
                                            0 AS V, 0 AS VI
                                        UNION ALL
                                        SELECT PartNo
                                            , sum(case [Month] when month(dateadd(month,-6,@p3)) then isnull(DemandQty,0) else 0 end) I
                                            , sum(case [Month] when month(dateadd(month,-5,@p3)) then isnull(DemandQty,0) else 0 end) II
                                            , sum(case [Month] when month(dateadd(month,-4,@p3)) then isnull(DemandQty,0) else 0 end) III
                                            , sum(case [Month] when month(dateadd(month,-3,@p3)) then isnull(DemandQty,0) else 0 end) IV
                                            , sum(case [Month] when month(dateadd(month,-2,@p3)) then isnull(DemandQty,0) else 0 end) V
                                            , sum(case [Month] when month(dateadd(month,-1,@p3)) then isnull(DemandQty,0) else 0 end) VI
                                        FROM spHstDemandItem hstDemand with(nolock, nowait) 
                                        WHERE hstDemand.CompanyCode = @p0
                                              AND hstDemand.BranchCode = @p1
                                              AND hstDemand.PartNo = @p2
                                              AND 
		                                        (
		                                          hstDemand.[Year] = year(dateadd(month,-6,@p3)) 
                                                  AND hstDemand.[Month] >= month(dateadd(month,-6,@p3)) 
                                                  OR hstDemand.[Year] = year(@p3)
                                                  AND hstDemand.[Month] <= month(@p3)
		                                        )
                                        GROUP BY PartNo
                                    ) A
                                    GROUP BY A.PARTNO
                                ";
                            }
                            else
                            {
                                query = @"
                                   SELECT PARTNO, SUM(I) AS I,SUM(II) AS II,SUM(III) AS III,SUM(IV) AS IV,SUM(V) AS V,SUM(VI) AS VI
                                   FROM
                                    (
                                        SELECT @p2 AS PartNo, 0 AS I, 
                                            0 AS II, 0 AS III, 0 AS IV,
                                            0 AS V, 0 AS VI
                                        UNION ALL
                                        SELECT	PartNo
                                            , sum(case [Month] when month(dateadd(month,-6,@p3)) then isnull(DemandQty,0) else 0 end) I
                                            , sum(case [Month] when month(dateadd(month,-5,@p3)) then isnull(DemandQty,0) else 0 end) II
                                            , sum(case [Month] when month(dateadd(month,-4,@p3)) then isnull(DemandQty,0) else 0 end) III
                                            , sum(case [Month] when month(dateadd(month,-3,@p3)) then isnull(DemandQty,0) else 0 end) IV
                                            , sum(case [Month] when month(dateadd(month,-2,@p3)) then isnull(DemandQty,0) else 0 end) V
                                            , sum(case [Month] when month(dateadd(month,-1,@p3)) then isnull(DemandQty,0) else 0 end) VI
                                        FROM spHstDemandItem hstDemand with(nolock, nowait)
                                        WHERE hstDemand.CompanyCode = @p0
                                          AND hstDemand.BranchCode = @p1
                                          AND hstDemand.PartNo = @p2
                                          AND (hstDemand.[Year] = year(@p3)
                                               AND hstDemand.[Month] BETWEEN month(dateadd(month,-6,@p3))
                                               AND month(@p3))
                                        GROUP BY PartNo
                                    ) A
                                    GROUP BY A.PARTNO
                                ";
                            }

                            object[] parameters = { CompanyCode, BranchCode, itemMod.PartNo, record.SuggorDate };

                            var rows = ctx.Database.SqlQuery<PreSaveSuggor>(query, parameters).FirstOrDefault();

                            spTrnPSUGGORSubDtl oSpTrnPSUGGORSubDtl = null;

                            if (rows != null)
                            {

                                try
                                {
                                    oSpTrnPSUGGORSubDtl = ctx.spTrnPSUGGORSubDtls.Find(record.CompanyCode
                                                            , record.BranchCode, record.SuggorNo
                                                            , suggorDtl.PartNo.ToString()
                                                            , rows.PartNo);

                                    if (oSpTrnPSUGGORSubDtl == null)
                                    {
                                        oSpTrnPSUGGORSubDtl = new spTrnPSUGGORSubDtl();
                                        oSpTrnPSUGGORSubDtl.CompanyCode = record.CompanyCode;
                                        oSpTrnPSUGGORSubDtl.BranchCode = record.BranchCode;
                                        oSpTrnPSUGGORSubDtl.SuggorNo = record.SuggorNo;
                                        ctx.spTrnPSUGGORSubDtls.Add(oSpTrnPSUGGORSubDtl);
                                    }

                                    intRowCount = intRowCount + 1;

                                    oSpTrnPSUGGORSubDtl.PartNoSuggor = suggorDtl.PartNo.ToString();
                                    oSpTrnPSUGGORSubDtl.PartNo = itemMod.PartNo.ToString();
                                    oSpTrnPSUGGORSubDtl.SeqNo = intRowCount;
                                    oSpTrnPSUGGORSubDtl.I = Convert.ToDecimal(rows.I);
                                    oSpTrnPSUGGORSubDtl.II = Convert.ToDecimal(rows.II);
                                    oSpTrnPSUGGORSubDtl.III = Convert.ToDecimal(rows.III);
                                    oSpTrnPSUGGORSubDtl.IV = Convert.ToDecimal(rows.IV);
                                    oSpTrnPSUGGORSubDtl.V = Convert.ToDecimal(rows.V);
                                    oSpTrnPSUGGORSubDtl.VI = Convert.ToDecimal(rows.VI);
                                    oSpTrnPSUGGORSubDtl.CreatedBy = CurrentUser.UserId;
                                    oSpTrnPSUGGORSubDtl.CreatedDate = DateTime.Now;

                                    // save data substitusi 
                                    result = ctx.SaveChanges() > 0;

                                    if (!result)
                                        return Json(new { success = false });
                                }
                                catch
                                {
                                    msg = "Proses Penyimpanan gagal Ada kesalahan data SUGGOR";
                                    return Json(new { success = false, message = msg });
                                }
                            }
                            else
                            {
                                msg = "Tidak ada data Demand Histories, Proses Penyimpanan Gagal";
                                return Json(new { success = false, message = msg });
                            }
                            totDemand += (Convert.ToDouble(rows.I) + Convert.ToDouble(rows.II) + Convert.ToDouble(rows.III) +
                                          Convert.ToDouble(rows.IV) + Convert.ToDouble(rows.V) + Convert.ToDouble(rows.VI));
                        }

                        // Insert Data Suggor Detail
                        suggorDtl.DemandAvg = Convert.ToDecimal(totDemand / 180);
                        ctx.spTrnPSUGGORDtls.Add(suggorDtl);
                        result = ctx.SaveChanges() > 0;

                        spMstItem oSpMstItems = ctx.spMstItems.Find(CompanyCode, BranchCode, row.PartNo);
                        if (oSpMstItems != null && result)
                        {
                            oSpMstItems.DemandAverage = Convert.ToDecimal(totDemand / 180);
                            oSpMstItems.OrderPointQty = row.OrderPoint == null ? 0 : row.OrderPoint;
                            oSpMstItems.SafetyStockQty = row.SafetyStock == null ? 0 : row.SafetyStock;

                            ctx.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            var display = ctx.spTrnPSUGGORHdrs.Find(CompanyCode, BranchCode, suggorNo);
            return Json(new { success = true, lblstatus = SetStatusLabel("0"), display = display });
        }

        public JsonResult ReOrderPrint(string suggorNo)
        {
            var Hdr = ctx.spTrnPSUGGORHdrs.Find(CompanyCode, BranchCode, suggorNo);
            if (Hdr != null)
            {
                Hdr.Status = "1";
                Hdr.PrintSeq += 1;
                Hdr.LastUpdateBy = CurrentUser.UserId;
                Hdr.LastUpdateDate = DateTime.Now;

                bool result = ctx.SaveChanges() > 0;
                if (!result)
                {
                    return Json(new { success = false, message = "Gagal print data" });
                }
                flagProses = REORDER;
            }
            return Json(new { success = true, lblstatus = SetStatusLabel(Hdr.Status) });
        }

        public JsonResult ProcessToPO(string suggorNo, List<spTrnPSUGGORDtl> dtSuggor)
        {
            decimal counterRows = 0;
            int counterIndex = 0;

            bool result = false;

            var Hdr = ctx.spTrnPSUGGORHdrs.Find(CompanyCode, BranchCode, suggorNo);

            try
            {
                if (Hdr != null)
                {
                    var dtv = DateTransValidation(Hdr.SuggorDate.Value);
                    if (dtv != "") return Json(new { success = false, message = dtv });

                    var query = @"
                            UPDATE spTrnPSUGGORDtl 
                            SET isExistInItems = f.flag, 
                                LastUpdateBy = @p3, 
                                LastUpdateDate = @p4
                            FROM spTrnPSUGGORDtl e,
                            (
	                            select x.companycode, x.branchcode, x.suggorno,
                                x.partno, b.partno as al_partno, 
	                            case when b.partno is NULL
		                            then 0 
		                            else 1 
		                            end as flag 
	                            from
	                            (
	                            select a.companycode,a.branchcode,a.suggorno,a.partno 
	                            from spTrnPSUGGORDtl a
	                            where companycode = @p0
	                            and branchcode = @p1
                                and a.suggorno = @p2
	                            ) x left join spMstItems b 
                                on x.companyCode = b.CompanyCode
	                            and x.Branchcode = b.branchcode
	                            and x.partno = b.partno
                            ) f
                            WHERE e.companycode = f.companycode
                            AND e.branchcode = f.branchcode
                            AND e.suggorno = f.suggorno
                            AND e.partno = f.partno
                    ";

                    object[] parameters = { CompanyCode, BranchCode, suggorNo, CurrentUser.UserId, DateTime.Now };

                    if (ctx.Database.ExecuteSqlCommand(query, parameters) == 0)
                        result = false;
                    else
                        result = true;

                    if (!result) return Json(new { success = false, message = "Proses update status item registrasi gagal" });

                    if (result)
                    {
                        Hdr.POSNo = GetNewDocumentNo("POS", DateTime.Now);
                        if (Hdr.POSNo.EndsWith("X")) return Json(new { success = false, message = "Dokumen No POS belum ada di Master Dokumen" });

                        Hdr.POSDate = DateTime.Now;
                        Hdr.Status = "2";
                        Hdr.LastUpdateBy = CurrentUser.UserId;
                        Hdr.LastUpdateDate = DateTime.Now;

                        spTrnPPOSHdr recordPO = new spTrnPPOSHdr();
                        recordPO.CompanyCode = CompanyCode;
                        recordPO.BranchCode = BranchCode;
                        recordPO.POSNo = Hdr.POSNo;
                        recordPO.POSDate = Hdr.POSDate;
                        recordPO.SupplierCode = Hdr.SupplierCode;
                        recordPO.OrderType = Hdr.OrderType;
                        recordPO.isBO = true;
                        recordPO.isSubstution = true;
                        recordPO.isSuggorProcess = true;
                        recordPO.Remark = "";
                        recordPO.ProductType = Hdr.ProductType;
                        recordPO.PrintSeq = 0;
                        recordPO.Status = "0";
                        recordPO.TypeOfGoods = Hdr.TypeOfGoods;
                        recordPO.isGenPORDD = false;
                        recordPO.CreatedBy = CurrentUser.UserId;
                        recordPO.CreatedDate = DateTime.Now;
                        recordPO.LastUpdateBy = CurrentUser.UserId;
                        recordPO.LastUpdateDate = DateTime.Now;

                        ctx.spTrnPPOSHdrs.Add(recordPO);
                        ctx.SaveChanges();

                        counterRows = dtSuggor.Count();
                        foreach (var row in dtSuggor)
                        {
                            counterIndex++;
                            if (row.SuggorCorrecQty > 0)
                            {
                                spTrnPPOSDtl recordPODtl = new spTrnPPOSDtl();
                                recordPODtl.CompanyCode = CompanyCode;
                                recordPODtl.BranchCode = BranchCode;
                                recordPODtl.POSNo = recordPO.POSNo;
                                recordPODtl.PartNo = row.PartNo;
                                recordPODtl.SeqNo = row.SeqNo.Value;
                                recordPODtl.OrderQty = row.SuggorCorrecQty;
                                recordPODtl.SuggorQty = row.SuggorQty;

                                spMstItemPrice oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, recordPODtl.PartNo);
                                if (oItemPrice == null)
                                {
                                    msg = "Harga Beli [" + recordPODtl.PartNo.ToString() + "] tidak ditemukan.";
                                    return Json(new { success = false, message = msg });
                                }

                                recordPODtl.PurchasePrice = (oItemPrice == null) ? 0 : oItemPrice.PurchasePrice;
                                recordPODtl.DiscPct = GetDiscountPct(Hdr.SupplierCode, recordPODtl.PartNo, Hdr.OrderType, DateTime.Now);
                                recordPODtl.PurchasePriceNett = Math.Round(recordPODtl.PurchasePrice.Value - (recordPODtl.PurchasePrice.Value * (recordPODtl.DiscPct.Value / 100)));
                                recordPODtl.CostPrice = oItemPrice.CostPrice;

                                decimal purchaseAmount = recordPODtl.OrderQty.Value * recordPODtl.PurchasePrice.Value;
                                decimal discountAmount = Math.Round(purchaseAmount * (recordPODtl.DiscPct.Value / 100), 0, MidpointRounding.AwayFromZero);
                                recordPODtl.TotalAmount = purchaseAmount - discountAmount;

                                if (recordPODtl.PurchasePrice <= 0)
                                {
                                    msg = "Harga Beli [" + recordPODtl.PartNo.ToString() + "] tidak boleh kurang atau sama dengan nol";
                                    return Json(new { success = false, message = msg });
                                }

                                spMstItem item = ctx.spMstItems.Find(CompanyCode, BranchCode, recordPODtl.PartNo);
                                recordPODtl.ABCClass = (item == null) ? "" : item.ABCClass;
                                recordPODtl.MovingCode = (item == null) ? "" : item.MovingCode;
                                recordPODtl.ProductType = Hdr.ProductType;
                                recordPODtl.PartCategory = (item == null) ? "" : item.PartCategory;
                                recordPODtl.CreatedBy = CurrentUser.UserId;
                                recordPODtl.CreatedDate = DateTime.Now;

                                ctx.spTrnPPOSDtls.Add(recordPODtl);
                                ctx.SaveChanges();
                            }
                        }
                    }
                    var dtSuggorDtl = ctx.spTrnPSUGGORDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SuggorNo == suggorNo && !a.isExistInItems);

                    if (dtSuggorDtl.Count() > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var row in dtSuggorDtl)
                            sb.AppendFormat(", {0}", row.PartNo);

                        msg = string.Format("No. part berikut ini belum terdaftar di Master Item : \r\n{0}", sb.ToString().Substring(2));
                        return Json(new { success = false, message = msg });
                    }
                }
                result = true;
                var display = ctx.spTrnPSUGGORHdrs.Find(CompanyCode, BranchCode, suggorNo);
                return Json(new { success = result, display = display, lblstatus = SetStatusLabel(Hdr.Status) });
            }
            catch (Exception ex)
            {
                result = false;
                return Json(new { success = result });
            }
        }

        public JsonResult delete(string suggorNo)
        {
            var msg = "";
            var record = ctx.spTrnPSUGGORHdrs.Find(CompanyCode, BranchCode, suggorNo);

            if (record != null)
            {
                record.Status = "3";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
            }

            try
            {
                ctx.SaveChanges();
                var lblStatus = SetStatusLabel(record.Status);
                return Json(new { success = true, lblstatus = lblStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private DataSet ProcessSuggor(spTrnPSUGGORHdr record)
        {
            DataSet dsSuggor = new DataSet();
            //decimal counter = 1;
            decimal counterRows = 0;
            int counterIndex = 0;

            var query = "exec uspfn_spProcessSuggor @p0,@p1,@p2,@p3,@p4";
            object[] parameters = { CompanyCode, BranchCode, record.MovingCode, record.SupplierCode, TypeOfGoods };
            //object[] parameters = { CompanyCode, BranchCode, "0","00000100", "0" };
            var dtDetail = ctx.Database.SqlQuery<ProcessSuggor>(query, parameters).ToList();

            /// Create Temporary DataTable to Store Suggor Parameter
            DataTable dtSuggorDraft = new DataTable();
            dtSuggorDraft.Columns.Add("SeqNo", typeof(int));
            dtSuggorDraft.Columns.Add("PartNo", typeof(string));
            dtSuggorDraft.Columns.Add("AvailableQty", typeof(decimal));
            dtSuggorDraft.Columns.Add("SuggorQty", typeof(decimal));
            dtSuggorDraft.Columns.Add("SuggorCorrecQty", typeof(decimal));
            dtSuggorDraft.Columns.Add("PartCategory", typeof(string));
            dtSuggorDraft.Columns.Add("SupplierCode", typeof(string));
            dtSuggorDraft.Columns.Add("IsValid", typeof(bool));

            dtSuggorDraft.Columns.Add("DemandQty", typeof(decimal));
            dtSuggorDraft.Columns.Add("DemandAverage", typeof(decimal));
            dtSuggorDraft.Columns.Add("LeadTime", typeof(decimal));
            dtSuggorDraft.Columns.Add("OrderCycle", typeof(decimal));
            dtSuggorDraft.Columns.Add("SafetyStock", typeof(decimal));

            dtSuggorDraft.Columns.Add("OrderPoint", typeof(decimal));
            dtSuggorDraft.Columns.Add("OnHand", typeof(decimal));
            dtSuggorDraft.Columns.Add("OnOrder", typeof(decimal));
            dtSuggorDraft.Columns.Add("InTransit", typeof(decimal));
            dtSuggorDraft.Columns.Add("AllocationSP", typeof(decimal));
            dtSuggorDraft.Columns.Add("AllocationSR", typeof(decimal));
            dtSuggorDraft.Columns.Add("AllocationSL", typeof(decimal));
            dtSuggorDraft.Columns.Add("BackOrderSP", typeof(decimal));
            dtSuggorDraft.Columns.Add("BackOrderSR", typeof(decimal));
            dtSuggorDraft.Columns.Add("BackOrderSL", typeof(decimal));
            dtSuggorDraft.Columns.Add("ReservedSP", typeof(decimal));
            dtSuggorDraft.Columns.Add("ReservedSR", typeof(decimal));
            dtSuggorDraft.Columns.Add("ReservedSL", typeof(decimal));
            dtSuggorDraft.Columns.Add("isExistInItems", typeof(bool));

            /// Create dtSuggor as a clone form dtSuggorDraft
            DataTable dtSuggor = dtSuggorDraft.Clone();

            /// Create Hash table 
            Hashtable hash = new Hashtable();

            int no = 1;
            try
            {
                counterRows = dtDetail.Count();
                foreach (var row in dtDetail)
                {
                    counterIndex++;
                    row.No = no++;

                    /// add new row and insert in dtSuggorDraft
                    DataRow newRow = dtSuggorDraft.NewRow();
                    newRow["SeqNo"] = dtSuggorDraft.Rows.Count + 1;
                    newRow["PartNo"] = row.PartNo;  //oSpMstItemMod.PartNo;
                    newRow["AvailableQty"] = row.AvailableQty;
                    newRow["SuggorQty"] = row.AvailableQty;
                    newRow["SuggorCorrecQty"] = row.AvailableQty;
                    newRow["PartCategory"] = row.PartCategory;

                    newRow["DemandQty"] = 0;  //row.stItemMod.D6MonthQty;
                    newRow["DemandAverage"] = 0;
                    newRow["LeadTime"] = row.LeadTime;
                    newRow["OrderCycle"] = row.OrderCycle;
                    newRow["SafetyStock"] = row.SafetyStock;

                    newRow["OrderPoint"] = row.OrderPoint;
                    newRow["OnHand"] = row.OnHand;
                    newRow["OnOrder"] = row.OnOrder;
                    newRow["InTransit"] = row.InTransit;
                    newRow["AllocationSP"] = row.AllocationSP;
                    newRow["AllocationSR"] = row.AllocationSR;
                    newRow["AllocationSL"] = row.AllocationSL;
                    newRow["BackOrderSP"] = row.BackOrderSP;
                    newRow["BackOrderSR"] = row.BackOrderSR;
                    newRow["BackOrderSL"] = row.BackOrderSL;
                    newRow["ReservedSP"] = row.ReservedSP;
                    newRow["ReservedSR"] = row.ReservedSR;
                    newRow["ReservedSL"] = row.ReservedSL;

                    /// Insert data newRow into dtSuggorDraft
                    dtSuggorDraft.Rows.Add(newRow);

                    /// Count Demand Qty each family and Get Last Family Part
                    SpMstItemModNew oSpMstItemMod = fc_SelectEndPartMod((string)row.PartNo);
                    spMstItem oSpMstItems = ctx.spMstItems.Find(CompanyCode, BranchCode, oSpMstItemMod.PartNo);
                    SpMstItemInfo oSpMstItemInfo = ctx.SpMstItemInfos.Find(CompanyCode, oSpMstItemMod.PartNo);

                    if (oSpMstItemInfo != null && oSpMstItemInfo.SupplierCode == record.SupplierCode)
                    {
                        if (hash[oSpMstItemMod.PartNo] == null)
                        {
                            /// copy all records newRow into dtSuggor
                            dtSuggor.Rows.Add(newRow.ItemArray);

                            /// Create newSuggorRow base on the latest-1 dtSuggor data
                            DataRow newSuggorRow = dtSuggor.Rows[dtSuggor.Rows.Count - 1];

                            /// copy all records dtSuggor rows into Hashtable
                            hash[oSpMstItemMod.PartNo] = newSuggorRow;

                            /// insert into dtSuggor for latest Modification Part
                            newSuggorRow["SeqNo"] = dtSuggor.Rows.Count;
                            newSuggorRow["PartNo"] = oSpMstItemMod.PartNo;
                            newSuggorRow["DemandQty"] = 0;

                            if (oSpMstItems == null)
                                newSuggorRow["isExistInItems"] = false;
                            else
                                newSuggorRow["isExistInItems"] = true;
                        }
                        else
                        {
                            /// Copy all data from Hash table to oldRow
                            DataRow oldRow = (DataRow)hash[oSpMstItemMod.PartNo];

                            /// Accumulate all data for modification parts to new Parts Modification
                            oldRow["AvailableQty"] = Convert.ToDecimal(oldRow["AvailableQty"]) + Convert.ToDecimal(row.AvailableQty);

                            oldRow["LeadTime"] = row.LeadTime;
                            oldRow["OrderCycle"] = row.OrderCycle;
                            oldRow["SafetyStock"] = row.SafetyStock;

                            /// Accumulate all data for modification parts to new Parts Modification
                            oldRow["OrderPoint"] = Convert.ToDecimal(oldRow["OrderPoint"]) + Convert.ToDecimal(row.OrderPoint);
                            oldRow["OnHand"] = Convert.ToDecimal(oldRow["OnHand"]) + Convert.ToDecimal(row.OnHand);
                            oldRow["OnOrder"] = Convert.ToDecimal(oldRow["OnOrder"]) + Convert.ToDecimal(row.OnOrder);
                            oldRow["InTransit"] = Convert.ToDecimal(oldRow["InTransit"]) + Convert.ToDecimal(row.InTransit);
                            oldRow["AllocationSP"] = Convert.ToDecimal(oldRow["AllocationSP"]) + Convert.ToDecimal(row.AllocationSP);
                            oldRow["AllocationSR"] = Convert.ToDecimal(oldRow["AllocationSR"]) + Convert.ToDecimal(row.AllocationSR);
                            oldRow["AllocationSL"] = Convert.ToDecimal(oldRow["AllocationSL"]) + Convert.ToDecimal(row.AllocationSL);
                            oldRow["BackOrderSP"] = Convert.ToDecimal(oldRow["BackOrderSP"]) + Convert.ToDecimal(row.BackOrderSP);
                            oldRow["BackOrderSR"] = Convert.ToDecimal(oldRow["BackOrderSR"]) + Convert.ToDecimal(row.BackOrderSR);
                            oldRow["BackOrderSL"] = Convert.ToDecimal(oldRow["BackOrderSL"]) + Convert.ToDecimal(row.BackOrderSL);
                            oldRow["ReservedSP"] = Convert.ToDecimal(oldRow["ReservedSP"]) + Convert.ToDecimal(row.ReservedSP);
                            oldRow["ReservedSR"] = Convert.ToDecimal(oldRow["ReservedSR"]) + Convert.ToDecimal(row.ReservedSR);
                            oldRow["ReservedSL"] = Convert.ToDecimal(oldRow["ReservedSL"]) + Convert.ToDecimal(row.ReservedSL);
                        }
                    }
                }
                counterIndex = 0;
                counterRows = dtSuggor.Rows.Count;
                /// Looping to calculate Suggor Qty
                foreach (DataRow row in dtSuggor.Rows)
                {
                    counterIndex++;
                    decimal dQty = Convert.ToDecimal(row["DemandQty"]);
                    decimal ltp = Convert.ToDecimal(row["LeadTime"]);
                    decimal ocp = Convert.ToDecimal(row["OrderCycle"]);
                    decimal ssp = Convert.ToDecimal(row["SafetyStock"]);

                    decimal onHand = Convert.ToDecimal(row["OnHand"]);
                    decimal onOrder = Convert.ToDecimal(row["OnOrder"]);
                    decimal inTransit = Convert.ToDecimal(row["InTransit"]);
                    decimal spAlloc = Convert.ToDecimal(row["AllocationSP"]);
                    decimal srAlloc = Convert.ToDecimal(row["AllocationSR"]);
                    decimal slAlloc = Convert.ToDecimal(row["AllocationSL"]);
                    decimal spBO = Convert.ToDecimal(row["BackOrderSP"]);
                    decimal srBO = Convert.ToDecimal(row["BackOrderSR"]);
                    decimal slBO = Convert.ToDecimal(row["BackOrderSL"]);
                    decimal spReserv = Convert.ToDecimal(row["ReservedSP"]);
                    decimal srReserv = Convert.ToDecimal(row["ReservedSR"]);
                    decimal slReserv = Convert.ToDecimal(row["ReservedSL"]);

                    spMstItem oSpMstItems = ctx.spMstItems.Find(CompanyCode, BranchCode, (string)row["PartNo"]);

                    decimal dAvg = oSpMstItems == null ? 0 : oSpMstItems.DemandAverage.Value;
                    decimal ssQty = (dAvg * ssp);
                    decimal orderPoint = (dAvg * (ltp + ocp)) + ssQty;
                    decimal totalStock = (onHand + onOrder + inTransit -
                                         (spAlloc + srAlloc + slAlloc + spBO + srBO + slBO +
                                          spReserv + srReserv + slReserv)) > 0 ?
                                         (onHand + onOrder + inTransit -
                                         (spAlloc + srAlloc + slAlloc + spBO + srBO + slBO +
                                          spReserv + srReserv + slReserv)) : 0;

                    decimal suggorQty = orderPoint - totalStock;

                    row["DemandAverage"] = dAvg;
                    row["SafetyStock"] = ssQty;
                    row["OrderPoint"] = orderPoint;

                    row["SuggorQty"] = Convert.ToInt32(suggorQty);
                    row["SuggorCorrecQty"] = row["SuggorQty"];

                    /// Get Suggor - Supplier Code and Compare with Supplier Code Parameter
                    SpMstItemInfo oSpMstItemInfo = ctx.SpMstItemInfos.Find(CompanyCode, (string)row["PartNo"]);
                    if (oSpMstItemInfo != null)
                    {
                        row["SupplierCode"] = oSpMstItemInfo.SupplierCode;
                        row["IsValid"] = (oSpMstItemInfo.SupplierCode.Equals(record.SupplierCode));
                    }
                }

                DataTable dtInfo = new DataTable();
                dtInfo.Columns.Add("id");
                dtInfo.Columns.Add("desc");
                dtInfo.Rows.Add("dtSuggor", "Original Suggor");
                dtInfo.Rows.Add("dtSuggorDraft", "Original Suggor Draft");

                dsSuggor.Tables.Add(dtInfo);
                dsSuggor.Tables.Add(dtSuggor);
                dsSuggor.Tables.Add(dtSuggorDraft);

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return dsSuggor;
        }

        private SpMstItemModNew fc_SelectEndPartMod(string partNo)
        {
            List<SpMstItemModNew> list = fc_SelectAllModifikasi(partNo);
            return list[list.Count - 1];
        }

        private List<SpMstItemModNew> fc_SelectAllModifikasi(string partNo)
        {
            List<SpMstItemModNew> list = new List<SpMstItemModNew>();

            var data = ctx.Database.SqlQuery<GetModification>("Select * from GetSUGGORModifikasi(@p0)", partNo).ToList();
            if (data != null && data.Count() > 0)
            {
                foreach (var row in data)
                {
                    SpMstItemModNew record = new SpMstItemModNew();
                    record.PartNo = row.ID;
                    record.InterChangeCode = row.InterChangeCode;
                    list.Add(record);
                }
            }
            return list;
        }

        public decimal GetDiscountPct(string supplierCode, string partNo, string ordertype, DateTime paramDate)
        {
            decimal discount = 0;

            spMstItem item = ctx.spMstItems.Find(CompanyCode, BranchCode, partNo);
            var decPurcDiscPct = item.PurcDiscPct ?? 0;
            if (item != null && decPurcDiscPct > 0)
            {
                discount = decPurcDiscPct;
            }
            else
            {
                //SpMstItemsBLL
                // discount from master supplier
                var obj1 = ctx.SupplierProfitCenters.Find(CompanyCode, BranchCode, supplierCode, ProfitCenter);
                discount = (obj1 == null) ? discount : discount + obj1.DiscPct.Value;

                // discount from master ordertype
                var obj2 = ctx.LookUpDtls.Find(CompanyCode, "ORTP", ordertype);
                discount = (obj2 == null) ? discount : discount + Convert.ToDecimal(obj2.ParaValue);

                // discount from master campaign
                var obj3 = ctx.spMstPurchCampaigns.Find(CompanyCode, BranchCode, supplierCode, partNo, paramDate);
                if (obj3 != null && obj3.BegDate <= paramDate && obj3.EndDate >= paramDate)
                {
                    discount = discount + obj3.DiscPct.Value;
                }
            }
            return (discount < 0 ? 0 : discount);
        }

        public JsonResult PembelianBrowse()
        {
            string sql = string.Format(@"EXEC uspfn_SpPembelianBrowse_Web '{0}', '{1}', '{2}', '{3}', '{4}'",
                CompanyCode, BranchCode, TypeOfGoods, Helpers.GetDynamicFilter(Request), 500);

            var records = ctx.Database.SqlQuery<spPembelianView>(sql).AsQueryable();
            return Json(records.toKG(ApplyFilterKendoGrid.False));
        }
    }
}