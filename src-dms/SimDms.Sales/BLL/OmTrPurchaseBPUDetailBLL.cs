using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrPurchaseBPUDetailBLL : BaseBLL
    {
         #region -- Initiate --
        public OmTrPurchaseBPUDetailBLL(DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --

        public OmTrPurchaseBPUDetailView GetRecordView(string pONo, string bPUNo, int bPUSeq)
        {
            var record = ctx.OmTrPurchaseBPUDetailViews.Find(CompanyCode, BranchCode, pONo, bPUNo, bPUSeq);
            return record;
        }

        public omTrPurchaseBPUDetail GetRecord(string pONo, string bPUNo, int bPUSeq)
        {
            var record = ctx.omTrPurchaseBPUDetail.Find(CompanyCode, BranchCode, pONo, bPUNo, bPUSeq);
            return record;
        }

        public IQueryable<omTrPurchaseBPUDetail> Select(string pONo, string bPUNo)
        {
            return ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                        && p.PONo == pONo && p.BPUNo == bPUNo);
        }

        public int GetRecordCount(string pONo, string bPUNo)
        {
            var intRecord = ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                        && p.PONo == pONo && p.BPUNo == bPUNo).Count();

            return intRecord;
        }

        public IQueryable<OmTrPurchaseBPUDetailView> Select4Table(string bpuNo)
        {
            var records = ctx.OmTrPurchaseBPUDetailViews.Where(
                p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode
                && p.BPUNo == bpuNo
            );

            return records;
        }

        public IQueryable<OmUtlSSJALDtl3> SelectFromUtl(string sjNo, string chassisCode, decimal chassisNo)
        {
            var queryable = from a in ctx.OmUtlSSJALDtl3s
                            join b in ctx.OmUtlSSJALHdrs on new { a.CompanyCode, a.BranchCode, a.BatchNo }
                                equals new { b.CompanyCode, b.BranchCode, b.BatchNo }
                            where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SJNo == sjNo
                                && a.ChassisCode == chassisCode && a.ChassisNo == chassisNo
                            select a;

            return queryable;
        }

        public bool CheckChassisNo(string chassisCode, decimal chassisNo)
        {
            var nochassis = (from a in ctx.omTrPurchaseBPUDetail
                             where a.CompanyCode == CompanyCode && a.ChassisCode == chassisCode
                             && a.ChassisNo == chassisNo && a.isReturn == false
                             select new { ChassisNo = a.ChassisNo }).Union(from b in ctx.OmMstVehicles
                                                                           where b.CompanyCode == CompanyCode && b.ChassisCode == chassisCode
                                                                           && b.ChassisNo == chassisNo
                                                                           select new { ChassisNo = b.ChassisNo }).FirstOrDefault();

            if (nochassis != null)
            {
                if (nochassis.ChassisNo != 0) { return true; }
                else { return false; }
            }
            else { return false; }

        }

        public bool CheckEngineNo(string engineCode, decimal engineNo)
        {
            var noengine = (from a in ctx.omTrPurchaseBPUDetail
                            where a.CompanyCode == CompanyCode && a.EngineCode == engineCode
                            && a.EngineNo == engineNo && a.isReturn == false
                            select a.EngineNo).Union(from b in ctx.OmMstVehicles
                                                     where b.CompanyCode == CompanyCode && b.EngineCode == engineCode
                                                      && b.EngineNo == engineNo
                                                     select b.EngineNo).FirstOrDefault();
            if (noengine != null)
                return true;
            else return false;
        }

        public int GetNextSeqNo(string bpuNo)
        {
            var bpuDetail = ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode == CompanyCode
                            && p.BranchCode == BranchCode && p.BPUNo == bpuNo).OrderByDescending(p => p.BPUSeq);

            return (bpuDetail.Count() > 0) ? bpuDetail.Max(p => p.BPUSeq) + 1 : 1;
        }

        public int Insert(omTrPurchaseBPUDetail record)
        {
            ctx.omTrPurchaseBPUDetail.Add(record);
            
            return ctx.SaveChanges();
        }

        public int Update()
        {
            return ctx.SaveChanges();
        }

        public bool Save(omTrPurchaseBPUDetail record, bool isNew, bool isDORelease)
        {
            bool result = false;
            try
            {
                if (isDORelease)
                {
                    record.DORelNo = GetNewDONo(Convert.ToDateTime(record.DORelDate));
                    if (record.DORelNo.EndsWith("X")) throw new ApplicationException(string.Format(GetMessage(SysMessages.MSG_5046), "DO Release"));
                    record.DORelDate = DateTime.Now;
                }

                Helpers.ReplaceNullable(record);
                result = (isNew) ? Insert(record) > 0 : Update() >= 0;
                if (result == true)
                {
                    bool isNewModel = false;
                    var omTrPurchaseBPUDetailModelBLL = new OmTrPurchaseBPUDetailModelBLL(ctx, CurrentUser.UserId);
                    OmTrPurchaseBPUDetailModel recModel = omTrPurchaseBPUDetailModelBLL.GetRecord(record.PONo,
                        record.BPUNo, record.SalesModelCode, Convert.ToDecimal(record.SalesModelYear));
                    if (recModel == null)
                    {
                        isNewModel = true;
                        recModel = new OmTrPurchaseBPUDetailModel();
                        recModel.CompanyCode = record.CompanyCode;
                        recModel.BranchCode = record.BranchCode;
                        recModel.PONo = record.PONo;
                        recModel.BPUNo = record.BPUNo;
                        recModel.SalesModelCode = record.SalesModelCode;
                        recModel.SalesModelYear = Convert.ToDecimal(record.SalesModelYear);
                        recModel.QuantityBPU = 0;
                        recModel.QuantityHPP = 0;
                        recModel.CreatedBy = CurrentUser.UserId;
                        recModel.CreatedDate = DateTime.Now;
                    }
                    recModel.QuantityBPU = (recModel.QuantityBPU == null) ? 0 : recModel.QuantityBPU;
                    if (isNew) recModel.QuantityBPU = recModel.QuantityBPU + 1;
                    recModel.LastUpdateBy = CurrentUser.UserId;
                    recModel.LastUpdateDate = DateTime.Now;
                    try
                    {
                        Helpers.ReplaceNullable(recModel);
                        result = (isNewModel) ? omTrPurchaseBPUDetailModelBLL.Insert(recModel) > 0 : omTrPurchaseBPUDetailModelBLL.Update() >= 0;
                        if (result == true)
                        {
                            decimal poSalesModelYear = Convert.ToDecimal(record.SalesModelYear);

                            string bpuType = "";
                            omTrPurchaseBPU oOmTrPurchaseBPU = ctx.omTrPurchaseBPU.Find(record.CompanyCode, record.BranchCode, record.PONo, record.BPUNo);
                            if (oOmTrPurchaseBPU != null)
                                bpuType = oOmTrPurchaseBPU.BPUType;

                            // Jika user 2W abaikan SalesModelYear
                            if (ProductType.Equals("2W") && bpuType.Equals("2"))
                            {
                                var datPOModel = ctx.OmTrPurchasePOModels.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                                    && p.PONo == record.PONo && p.SalesModelCode == record.SalesModelCode);
                                if (datPOModel.Count() > 0)
                                    poSalesModelYear = datPOModel.FirstOrDefault().SalesModelYear; ;
                            }

                            OmTrPurchasePOModel recPO = ctx.OmTrPurchasePOModels.Find(record.CompanyCode
                                    , record.BranchCode, record.PONo, record.SalesModelCode, poSalesModelYear);
                            if (recPO != null)
                            {
                                if (isNew)
                                {
                                    recPO.QuantityBPU = recPO.QuantityBPU ?? 0;
                                    recPO.QuantityBPU = recPO.QuantityBPU + 1;
                                    try
                                    {
                                        Helpers.ReplaceNullable(recPO);
                                        result = ctx.SaveChanges() >= 0;
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(ex.Message);
                                    }
                                }
                            }
                        }
                        omTrPurchaseBPUDetailModelBLL = null;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public int DeleteRecord(string poNo, string bpuNo, int bpuSeq, string salesModelCode, decimal salesModelYear)
        {
            int result = -1;
            try
            {
                var listString = "0,1".Split(',');
                var objDtl = ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                    && p.PONo == poNo && p.BPUNo == bpuNo && p.BPUSeq == bpuSeq && p.StatusHPP == "0" && p.isReturn == false
                    && listString.Contains(p.StatusSJRel)).FirstOrDefault();
                ctx.omTrPurchaseBPUDetail.Remove(objDtl);
                result = ctx.SaveChanges();

                if (result > -1)
                {
                    var objDtlModel = ctx.OmTrPurchaseBPUDetailModels.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                        && p.PONo == poNo && p.BPUNo == bpuNo && p.SalesModelCode == salesModelCode && p.SalesModelYear == salesModelYear)
                        .FirstOrDefault();
                    objDtlModel.QuantityBPU = objDtlModel.QuantityBPU - 1;

                    Helpers.ReplaceNullable(objDtlModel);
                    result = ctx.SaveChanges();

                    if (result > -1)
                    {
                        string bpuType = "";
                        var oOmTrPurchaseBPU = ctx.omTrPurchaseBPU.Find(CompanyCode, BranchCode, poNo, bpuNo);
                        if (oOmTrPurchaseBPU != null)
                            bpuType = oOmTrPurchaseBPU.BPUType;

                        // Jika user 2W abaikan SalesModelYear
                        if (ProductType.Equals("2W") && bpuType.Equals("2"))
                        {
                            var rec = ctx.OmTrPurchasePOModels.Where(p => p.CompanyCode == CompanyCode
                                && p.BranchCode == BranchCode && p.PONo == poNo && p.SalesModelCode == salesModelCode);

                            if (rec.Count() > 0)
                                salesModelYear = rec.FirstOrDefault().SalesModelYear;
                        }

                        var objPOModel = ctx.OmTrPurchasePOModels.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                            && p.PONo == poNo && p.SalesModelCode == salesModelCode && p.SalesModelYear == salesModelYear).FirstOrDefault();
                        objPOModel.QuantityBPU = objPOModel.QuantityBPU - 1;
                        
                        Helpers.ReplaceNullable(objPOModel);
                        result = ctx.SaveChanges();

                        if (result < 0)
                            throw new Exception("Gagal update PO Model");
                    }
                    else
                        throw new Exception("Gagal update BPU Detail Model");
                }
                else
                    throw new Exception("Gagal delete BPU Detail");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public string GetNewDONo(DateTime transDate)
        {
            return GetNewDocumentNo(GnMstDocumentConstant.DOR, transDate, "OM");
        }

        public List<omTrPurchaseBPUDetail> CheckVehicleBuyBack(string bpuNo)
        {
            var listString = "2,6".Split(',');
            var listDtl = (from a in ctx.omTrPurchaseBPUDetail
                           join b in ctx.OmMstVehicles
                           on new { a.CompanyCode, a.ChassisCode, ChassisNo = a.ChassisNo }
                           equals new { b.CompanyCode, b.ChassisCode, b.ChassisNo }
                           where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
                           && a.BPUNo == bpuNo && listString.Contains(b.Status)
                           select new omTrPurchaseBPUDetail()
                           {
                               ChassisCode = a.ChassisCode,
                               ChassisNo = a.ChassisNo
                           }).Distinct().ToList();

            return listDtl;
        }

        public int GetSeqNoVehicleHist(string chassisCode, int chassiNo)
        {
            var rec = ctx.OmMstVehicleHistorys.Where(
                p => p.CompanyCode == CompanyCode
                    && p.ChassisCode == chassisCode
                    && p.ChassisNo == chassiNo
                ).ToList();

            return (rec != null) ? (int)rec.Max(p => p.SeqNo) + 1 : 1;
        }

        public bool CreateVehicle(string poNo, string bpuNo, bool isBuyBack)
        {
            bool result = false;
            try
            {
                var BPUs = from dtl in ctx.omTrPurchaseBPUDetail
                              join hdr in ctx.omTrPurchaseBPU on new { dtl.CompanyCode, dtl.BranchCode, dtl.PONo, dtl.BPUNo }
                              equals new { hdr.CompanyCode, hdr.BranchCode, hdr.PONo, hdr.BPUNo } into _hdr
                              from hdr in _hdr.DefaultIfEmpty()
                              where dtl.CompanyCode == CompanyCode
                                  && dtl.BranchCode == BranchCode && dtl.PONo == poNo && dtl.BPUNo == bpuNo
                              select new
                              {
                                  dtl.ChassisCode,
                                  dtl.ChassisNo,
                                  dtl.EngineCode,
                                  dtl.EngineNo,
                                  dtl.SalesModelCode,
                                  dtl.SalesModelYear,
                                  dtl.ColourCode,
                                  dtl.ServiceBookNo,
                                  dtl.KeyNo,
                                  dtl.PONo,
                                  dtl.BPUNo,
                                  hdr.WarehouseCode,
                                  hdr.BPUDate,
                                  hdr.RefferenceDONo,
                                  dtl.BPUSeq,
                                  hdr.BPUSJDate,
                                  hdr.RefferenceDODate,
                                  hdr.RefferenceSJNo,
                                  hdr.RefferenceSJDate
                              };

                var listBPUs = BPUs.ToList();
                BPUs = null;
                if (listBPUs != null && listBPUs.Count() > 0)
                {
                    foreach (var recBPU in listBPUs)
                    {
                        OmTrInventQtyVehicleBLL omTrInventQtyVehicleBLL = new OmTrInventQtyVehicleBLL(ctx, CurrentUser.UserId);
                        OmMstVehicle recVehicle = null;

                        // for buy back vehicle that already sold
                        if (isBuyBack)
                        {
                            recVehicle = ctx.OmMstVehicles.Find(CompanyCode, recBPU.ChassisCode, Convert.ToDecimal(recBPU.ChassisNo));
                            if (recVehicle != null)
                            {
                                OmMstVehicleHistory oOmMstVehicleHistory = new OmMstVehicleHistory();
                                oOmMstVehicleHistory.CompanyCode = recVehicle.CompanyCode;
                                oOmMstVehicleHistory.ChassisCode = recVehicle.ChassisCode;
                                oOmMstVehicleHistory.ChassisNo = recVehicle.ChassisNo;
                                oOmMstVehicleHistory.SeqNo = GetSeqNoVehicleHist(recBPU.ChassisCode, Convert.ToInt32(recBPU.ChassisNo));
                                oOmMstVehicleHistory.EngineCode = recVehicle.EngineCode;
                                oOmMstVehicleHistory.EngineNo = recVehicle.EngineNo ?? 0;
                                oOmMstVehicleHistory.SalesModelCode = recVehicle.SalesModelCode;
                                oOmMstVehicleHistory.SalesModelYear = recVehicle.SalesModelYear ?? 0;
                                oOmMstVehicleHistory.ColourCode = recVehicle.ColourCode;
                                oOmMstVehicleHistory.ServiceBookNo = recVehicle.ServiceBookNo;
                                oOmMstVehicleHistory.KeyNo = recVehicle.KeyNo;
                                oOmMstVehicleHistory.COGSUnit = recVehicle.COGSUnit ?? 0;
                                oOmMstVehicleHistory.COGSOthers = recVehicle.COGSOthers ?? 0;
                                oOmMstVehicleHistory.COGSKaroseri = recVehicle.COGSKaroseri ?? 0;
                                oOmMstVehicleHistory.PpnBmBuyPaid = recVehicle.PpnBmBuyPaid ?? 0;
                                oOmMstVehicleHistory.PpnBmBuy = recVehicle.PpnBmBuy ?? 0;
                                oOmMstVehicleHistory.SalesNetAmt = recVehicle.SalesNetAmt ?? 0;
                                oOmMstVehicleHistory.PpnBmSellPaid = recVehicle.PpnBmSellPaid ?? 0;
                                oOmMstVehicleHistory.PpnBmSell = recVehicle.PpnBmSell ?? 0;
                                oOmMstVehicleHistory.PONo = recVehicle.PONo;
                                oOmMstVehicleHistory.POReturnNo = recVehicle.POReturnNo;
                                oOmMstVehicleHistory.BPUNo = recVehicle.BPUNo;
                                oOmMstVehicleHistory.HPPNo = recVehicle.HPPNo;
                                oOmMstVehicleHistory.KaroseriSPKNo = recVehicle.KaroseriSPKNo;
                                oOmMstVehicleHistory.SONo = recVehicle.SONo;
                                oOmMstVehicleHistory.SOReturnNo = recVehicle.SOReturnNo;
                                oOmMstVehicleHistory.DONo = recVehicle.DONo;
                                oOmMstVehicleHistory.BPKNo = recVehicle.BPKNo;
                                oOmMstVehicleHistory.InvoiceNo = recVehicle.InvoiceNo;
                                oOmMstVehicleHistory.ReqOutNo = recVehicle.ReqOutNo;
                                oOmMstVehicleHistory.TransferOutNo = recVehicle.TransferOutNo;
                                oOmMstVehicleHistory.TransferInNo = recVehicle.TransferInNo;
                                oOmMstVehicleHistory.WarehouseCode = recVehicle.WarehouseCode;
                                oOmMstVehicleHistory.Remark = "PURCHASE";
                                oOmMstVehicleHistory.Status = recVehicle.Status;
                                oOmMstVehicleHistory.IsAlreadyPDI = recVehicle.IsAlreadyPDI;
                                oOmMstVehicleHistory.BPUDate = Convert.ToDateTime(recVehicle.BPUDate);
                                oOmMstVehicleHistory.FakturPolisiNo = recVehicle.FakturPolisiNo;
                                oOmMstVehicleHistory.FakturPolisiDate = Convert.ToDateTime(recVehicle.FakturPolisiDate);
                                oOmMstVehicleHistory.PoliceRegistrationNo = recVehicle.PoliceRegistrationNo;
                                oOmMstVehicleHistory.PoliceRegistrationDate = Convert.ToDateTime(recVehicle.PoliceRegistrationDate);
                                oOmMstVehicleHistory.IsProfitCenterSales = recVehicle.IsProfitCenterSales;
                                oOmMstVehicleHistory.IsProfitCenterService = recVehicle.IsProfitCenterService;
                                oOmMstVehicleHistory.IsActive = recVehicle.IsActive;
                                oOmMstVehicleHistory.CreatedBy = CurrentUser.UserId;
                                oOmMstVehicleHistory.CreatedDate = DateTime.Now;
                                oOmMstVehicleHistory.LastUpdateBy = CurrentUser.UserId;
                                oOmMstVehicleHistory.LastUpdateDate = DateTime.Now;
                                oOmMstVehicleHistory.IsLocked = recVehicle.IsLocked;
                                oOmMstVehicleHistory.LockedBy = recVehicle.LockedBy;
                                oOmMstVehicleHistory.LockedDate = Convert.ToDateTime(recVehicle.LockedDate);
                                oOmMstVehicleHistory.IsNonRegister = recVehicle.IsNonRegister;
                                oOmMstVehicleHistory.BPUDate = Convert.ToDateTime(recVehicle.BPUDate);
                                oOmMstVehicleHistory.SuzukiDONo = recVehicle.SuzukiDONo;
                                oOmMstVehicleHistory.SuzukiDODate = recVehicle.SuzukiDODate;
                                oOmMstVehicleHistory.SuzukiSJNo = recVehicle.SuzukiSJNo;
                                oOmMstVehicleHistory.SuzukiSJDate = recVehicle.SuzukiSJDate;
                                ctx.OmMstVehicleHistorys.Add(oOmMstVehicleHistory);

                                Helpers.ReplaceNullable(oOmMstVehicleHistory);
                                if (ctx.SaveChanges() > 0)
                                {
                                    ctx.OmMstVehicles.Remove(recVehicle);
                                    result = (ctx.SaveChanges() > 0) ? true : false;
                                    if (!result)
                                        throw new Exception("Gagal delete Master Kendaraaan");
                                }
                                else
                                    throw new Exception("Gagal insert History Master Kendaraan");
                            }
                        }

                        bool isNew = false;
                        recVehicle = null;
                        var decChassisNo = Convert.ToDecimal(recBPU.ChassisNo);
                        recVehicle = ctx.OmMstVehicles.Where(p => p.CompanyCode == CompanyCode && p.ChassisCode == recBPU.ChassisCode
                        && p.ChassisNo == decChassisNo).FirstOrDefault();

                        if (recVehicle == null)
                        {
                            isNew = true;
                            recVehicle = new OmMstVehicle();
                            recVehicle.CompanyCode = CompanyCode;
                            recVehicle.ChassisCode = recBPU.ChassisCode;
                            recVehicle.ChassisNo = Convert.ToDecimal(recBPU.ChassisNo);
                            recVehicle.IsActive = true;
                            recVehicle.Status = "0";
                            recVehicle.IsProfitCenterSales = true;
                            recVehicle.CreatedBy = CurrentUser.UserId;
                            recVehicle.CreatedDate = DateTime.Now;
                        }
                        recVehicle.EngineCode = recBPU.EngineCode;
                        recVehicle.EngineNo = Convert.ToDecimal(recBPU.EngineNo);
                        recVehicle.SalesModelCode = recBPU.SalesModelCode;
                        recVehicle.SalesModelYear = Convert.ToDecimal(recBPU.SalesModelYear);
                        recVehicle.ColourCode = recBPU.ColourCode;
                        recVehicle.ServiceBookNo = recBPU.ServiceBookNo;
                        recVehicle.KeyNo = recBPU.KeyNo;
                        recVehicle.PONo = recBPU.PONo;
                        recVehicle.BPUNo = recBPU.BPUNo;
                        recVehicle.BPUDate = Convert.ToDateTime(recBPU.BPUDate);
                        recVehicle.WarehouseCode = recBPU.WarehouseCode;
                        recVehicle.SuzukiDONo = recBPU.RefferenceDONo;
                        recVehicle.SuzukiDODate = Convert.ToDateTime(recBPU.RefferenceDODate);
                        recVehicle.SuzukiSJNo = recBPU.RefferenceSJNo;
                        recVehicle.SuzukiSJDate = Convert.ToDateTime(recBPU.RefferenceSJDate);
                        recVehicle.TransferInMultiBranchNo = "";
                        recVehicle.TransferOutMultiBranchNo = "";
                        
                        //try
                        //{
                        Helpers.ReplaceNullable(recVehicle);
                        if (isNew)
                        {
                            ctx.OmMstVehicles.Add(recVehicle);
                            result = ctx.SaveChanges() > 0;
                        }
                        else
                        {
                            result = ctx.SaveChanges() >= 0;
                        }

                        if (result == true)
                        {
                            bool isNewInvent = false;
                            DateTime bpuDate = Convert.ToDateTime(recBPU.BPUDate).Date;
                            OmMstVehicleTemp recRemp = ctx.OmMstVehicleTemps.Find(CompanyCode, recBPU.ChassisCode, string.Format("{0}-{1}", recBPU.RefferenceDONo, recBPU.BPUSeq));
                            if (recRemp == null)
                            {
                                recRemp = ctx.OmMstVehicleTemps.Find(CompanyCode, recBPU.ChassisCode, recBPU.RefferenceDONo);
                                if (recRemp == null)
                                {
                                    OmTrInventQtyVehicle recInvent = omTrInventQtyVehicleBLL.GetRecord(Convert.ToDecimal(bpuDate.Year),
                                        Convert.ToDecimal(bpuDate.Month), recBPU.SalesModelCode, Convert.ToDecimal(recBPU.SalesModelYear),
                                        recBPU.ColourCode, recBPU.WarehouseCode);
                                    if (recInvent == null)
                                    {
                                        isNewInvent = true;
                                        recInvent = new OmTrInventQtyVehicle();
                                        recInvent.CompanyCode = CompanyCode;
                                        recInvent.BranchCode = BranchCode;
                                        recInvent.Year = Convert.ToDecimal(bpuDate.Year);
                                        recInvent.Month = Convert.ToDecimal(bpuDate.Month);
                                        recInvent.SalesModelCode = recBPU.SalesModelCode;
                                        recInvent.SalesModelYear = Convert.ToDecimal(recBPU.SalesModelYear);
                                        recInvent.ColourCode = recBPU.ColourCode;
                                        recInvent.WarehouseCode = recBPU.WarehouseCode;
                                        recInvent.Alocation = 0;
                                        recInvent.QtyOut = 0;
                                        
                                        recInvent.BeginningOH = recInvent.BeginningOH ?? 0;
                                        recInvent.BeginningAV = recInvent.BeginningAV ?? 0;

                                        OmTrInventQtyVehicle recBegin = omTrInventQtyVehicleBLL.BeginningQtyVehicle(Convert.ToDecimal(bpuDate.Year),
                                            Convert.ToDecimal(bpuDate.Month), recBPU.SalesModelCode, Convert.ToDecimal(recBPU.SalesModelYear),
                                            recBPU.ColourCode, recBPU.WarehouseCode);
                                        if (recBegin != null)
                                        {
                                            recBegin.EndingOH = recBegin.EndingOH ?? 0;
                                            recBegin.EndingAV = recBegin.EndingAV ?? 0;

                                            recInvent.BeginningOH = Convert.ToDecimal(recBegin.EndingOH);
                                            recInvent.BeginningAV = Convert.ToDecimal(recBegin.EndingAV);
                                        }
                                        else
                                        {
                                            recInvent.BeginningOH = recInvent.BeginningOH ?? 0;
                                            recInvent.BeginningAV = recInvent.BeginningAV ?? 0;
                                            
                                            recInvent.BeginningOH = 0;
                                            recInvent.BeginningAV = 0;
                                        }
                                        recInvent.Status = "0";
                                        recInvent.CreatedBy = CurrentUser.UserId;
                                        recInvent.CreatedDate = DateTime.Now;
                                    }

                                    recInvent.QtyIn = recInvent.QtyIn ?? 0;
                                    recInvent.BeginningOH = recInvent.BeginningOH ?? 0;
                                    recInvent.BeginningAV = recInvent.BeginningAV ?? 0;
                                    recInvent.EndingOH = recInvent.EndingOH ?? 0;
                                    recInvent.EndingAV = recInvent.EndingAV ?? 0;
                                    recInvent.QtyIn = recInvent.QtyIn ?? 0;
                                    recInvent.QtyOut = recInvent.QtyOut ?? 0;
                                    recInvent.Alocation = recInvent.Alocation ?? 0;

                                    recInvent.QtyIn = recInvent.QtyIn + 1;
                                    recInvent.EndingOH = (recInvent.BeginningOH + recInvent.QtyIn) - recInvent.QtyOut;
                                    recInvent.EndingAV = (recInvent.BeginningAV + recInvent.QtyIn) - recInvent.Alocation - recInvent.QtyOut;

                                    Helpers.ReplaceNullable(recInvent);
                                    if (isNewInvent)
                                    {
                                        ctx.OmTrInventQtyVehicles.Add(recInvent);
                                        result = ctx.SaveChanges() > 0;
                                    }
                                    else
                                    {
                                        result = ctx.SaveChanges() >= 0;
                                    }
                                    if (!result)
                                        throw new Exception("Gagal update qty kendaraan holding (1)");
                                }
                            }
                            if (recRemp != null)
                            {
                                OmMstVehicle recVehicle1 = ctx.OmMstVehicles.Find(CompanyCode, recBPU.ChassisCode, Convert.ToDecimal(recBPU.ChassisNo));
                                if (recVehicle1 != null)
                                {
                                    recVehicle1.COGSUnit = recVehicle1.COGSUnit ?? 0;
                                    recVehicle1.COGSOthers = recVehicle1.COGSOthers ?? 0;
                                    recVehicle1.PpnBmBuyPaid = recVehicle1.PpnBmBuyPaid ?? 0;
                                    recVehicle1.PpnBmBuy = recVehicle1.PpnBmBuy ?? 0;

                                    recRemp.COGSUnit = recRemp.COGSUnit ?? 0;
                                    recRemp.COGSOthers = recRemp.COGSOthers ?? 0;
                                    recRemp.PpnBmBuyPaid = recRemp.PpnBmBuyPaid ?? 0;
                                    recRemp.PpnBmBuy = recRemp.PpnBmBuy ?? 0;

                                    recVehicle1.COGSUnit = recRemp.COGSUnit;
                                    recVehicle1.COGSOthers = recRemp.COGSOthers;
                                    recVehicle1.PpnBmBuyPaid = recRemp.PpnBmBuyPaid;
                                    recVehicle1.PpnBmBuy = recRemp.PpnBmBuy;

                                    Helpers.ReplaceNullable(recVehicle1);
                                    ctx.SaveChanges();
                                }
                                recRemp.IsActive = false;
                                recRemp.LastUpdateBy = CurrentUser.UserId;
                                recRemp.LastUpdateDate = DateTime.Now;

                                Helpers.ReplaceNullable(recRemp);
                                if (ctx.SaveChanges() > 0)
                                {

                                    if (recBPU.WarehouseCode == recRemp.WarehouseCode && Convert.ToDateTime(recBPU.BPUSJDate).Date == Convert.ToDateTime(recRemp.BPUDate).Date)
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        OmTrInventQtyVehicle recInvent = omTrInventQtyVehicleBLL.GetRecord(Convert.ToDecimal(Convert.ToDateTime(recBPU.BPUSJDate).Date.Year),
                                            Convert.ToDecimal(Convert.ToDateTime(recBPU.BPUSJDate).Date.Month), recBPU.SalesModelCode
                                            , Convert.ToDecimal(recBPU.SalesModelYear), recBPU.ColourCode, recBPU.WarehouseCode);
                                        if (recInvent == null)
                                        {
                                            isNewInvent = true;
                                            recInvent = new OmTrInventQtyVehicle();
                                            recInvent.CompanyCode = CompanyCode;
                                            recInvent.BranchCode = BranchCode;
                                            recInvent.Year = Convert.ToDecimal(Convert.ToDateTime(recBPU.BPUSJDate).Date.Year);
                                            recInvent.Month = Convert.ToDecimal(Convert.ToDateTime(recBPU.BPUSJDate).Date.Month);
                                            recInvent.SalesModelCode = recBPU.SalesModelCode;
                                            recInvent.SalesModelYear = Convert.ToDecimal(recBPU.SalesModelYear);
                                            recInvent.ColourCode = recBPU.ColourCode;
                                            recInvent.WarehouseCode = recBPU.WarehouseCode;
                                            recInvent.Alocation = 0;
                                            recInvent.QtyOut = 0;
                                            OmTrInventQtyVehicle rowBegin = omTrInventQtyVehicleBLL.BeginningQtyVehicle(Convert.ToDecimal(Convert.ToDateTime(recBPU.BPUSJDate).Date.Year),
                                                Convert.ToDecimal(Convert.ToDateTime(recBPU.BPUSJDate).Date.Month), recBPU.SalesModelCode
                                                , Convert.ToDecimal(recBPU.SalesModelYear), recBPU.ColourCode, recBPU.WarehouseCode);
                                            if (rowBegin != null)
                                            {
                                                recInvent.BeginningOH = recInvent.BeginningOH ?? 0;
                                                recInvent.BeginningAV = recInvent.BeginningAV ?? 0;
                                                rowBegin.EndingOH = rowBegin.EndingOH ?? 0;
                                                rowBegin.EndingAV = rowBegin.EndingAV ?? 0;

                                                recInvent.BeginningOH = Convert.ToDecimal(rowBegin.EndingOH);
                                                recInvent.BeginningAV = Convert.ToDecimal(rowBegin.EndingAV);
                                            }
                                            else
                                            {
                                                recInvent.BeginningOH = 0;
                                                recInvent.BeginningAV = 0;
                                            }
                                            recInvent.Status = "0";
                                            recInvent.CreatedBy = CurrentUser.UserId;
                                            recInvent.CreatedDate = DateTime.Now;
                                        }
                                        recInvent.QtyIn = recInvent.QtyIn ?? 0;
                                        recInvent.QtyOut = recInvent.QtyOut ?? 0;
                                        recInvent.EndingOH = recInvent.EndingOH ?? 0;
                                        recInvent.EndingAV = recInvent.EndingAV ?? 0;
                                        recInvent.BeginningOH = recInvent.BeginningOH ?? 0;
                                        recInvent.BeginningAV = recInvent.BeginningAV ?? 0;
                                        recInvent.Alocation = recInvent.Alocation ?? 0;

                                        recInvent.QtyIn = recInvent.QtyIn + 1;
                                        recInvent.EndingOH = (recInvent.BeginningOH + recInvent.QtyIn) - recInvent.QtyOut;
                                        recInvent.EndingAV = (recInvent.BeginningAV + recInvent.QtyIn) - recInvent.Alocation - recInvent.QtyOut;

                                        Helpers.ReplaceNullable(recInvent);
                                        if (isNewInvent)
                                        {
                                            ctx.OmTrInventQtyVehicles.Add(recInvent);
                                            result = ctx.SaveChanges() > 0;
                                        }
                                        else
                                        {
                                            result = ctx.SaveChanges() >= 0;
                                        }

                                        if (result)
                                        {
                                            OmTrInventQtyVehicle recInventOld = omTrInventQtyVehicleBLL.GetRecord(Convert.ToDecimal(Convert.ToDateTime(recBPU.BPUSJDate).Date.Year),
                                                Convert.ToDecimal(Convert.ToDateTime(recBPU.BPUSJDate).Date.Month), recBPU.SalesModelCode
                                                , Convert.ToDecimal(recBPU.SalesModelYear), recRemp.ColourCode.ToString(), recRemp.WarehouseCode);

                                            if (recInventOld != null)
                                            {
                                                recInventOld.QtyOut = recInventOld.QtyOut ?? 0;
                                                recInventOld.QtyIn = recInventOld.QtyIn ?? 0;
                                                recInventOld.EndingOH = recInventOld.EndingOH ?? 0;
                                                recInventOld.EndingAV = recInventOld.EndingAV ?? 0;
                                                recInventOld.BeginningOH = recInventOld.BeginningOH ?? 0;
                                                recInventOld.BeginningAV = recInventOld.BeginningAV ?? 0;
                                                recInventOld.Alocation = recInventOld.Alocation ?? 0;

                                                recInventOld.QtyOut = recInventOld.QtyOut + 1;
                                                recInventOld.EndingOH = (recInventOld.BeginningOH + recInventOld.QtyIn) - recInventOld.QtyOut;
                                                recInventOld.EndingAV = (recInventOld.BeginningAV + recInventOld.QtyIn) - recInventOld.Alocation - recInventOld.QtyOut;
                                                recInventOld.LastUpdateBy = CurrentUser.UserId;
                                                recInventOld.LastUpdateDate = DateTime.Now;

                                                Helpers.ReplaceNullable(recInventOld);
                                                if (ctx.SaveChanges() >= 0)
                                                    result = true;
                                                else
                                                {
                                                    //result = false;
                                                    throw new Exception("Gagal update qty kendaraan temporary");
                                                }
                                            }
                                        }
                                        else
                                            throw new Exception("Gagal update qty kendaraan holding (2)");
                                    }
                                }
                                else
                                    throw new Exception("Gagal update COGS Master Kendaraan");
                            }
                        }
                        else
                            throw new Exception("Gagal insert or update Master Kendaraan");

                        omTrInventQtyVehicleBLL = null;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public bool CreateVehicleTemp(string poNo, string bpuNo)
        {
            bool result = false;
            try
            {
                var BPUs = (from dtl in ctx.omTrPurchaseBPUDetail
                            join hdr in ctx.omTrPurchaseBPU on new { dtl.CompanyCode, dtl.BranchCode, dtl.PONo, dtl.BPUNo }
                            equals new { hdr.CompanyCode, hdr.BranchCode, hdr.PONo, hdr.BPUNo } into _hdr
                            from hdr in _hdr.DefaultIfEmpty()
                            where dtl.CompanyCode == CompanyCode && dtl.BranchCode == BranchCode
                            && dtl.PONo == poNo && dtl.BPUNo == bpuNo
                            select new OmMstVehicleTempLookUp()
                            {
                                ChassisCode = dtl.ChassisCode,
                                RefDONo = hdr.RefferenceDONo,
                                ChassisNo = dtl.ChassisNo,
                                EngineCode = dtl.EngineCode,
                                EngineNo = dtl.EngineNo,
                                SalesModelCode = dtl.SalesModelCode,
                                SalesModelYear = dtl.SalesModelYear,
                                ColourCode = dtl.ColourCode,
                                ServiceBookNo = dtl.ServiceBookNo,
                                KeyNo = dtl.KeyNo,
                                PONo = dtl.PONo,
                                BPUNo = dtl.BPUNo,
                                WarehouseCode = hdr.WarehouseCode,
                                BPUDate = hdr.BPUDate,
                                BPUSeq = dtl.BPUSeq
                            });

                var listBPU = BPUs.ToList();
                BPUs = null;

                if (listBPU != null && listBPU.Count > 0)
                {
                    foreach (OmMstVehicleTempLookUp rec in listBPU)
                    {
                        bool isNew = false;
                        var refDONo = rec.RefDONo + "-" + rec.BPUSeq.ToString();
                        OmMstVehicleTemp recVehicle = ctx.OmMstVehicleTemps.Find(CompanyCode, rec.ChassisCode, refDONo);
                        if (recVehicle == null)
                        {
                            isNew = true;
                            recVehicle = new OmMstVehicleTemp();
                            recVehicle.CompanyCode = CompanyCode;
                            recVehicle.ChassisCode = rec.ChassisCode;
                            recVehicle.RefDONo = refDONo;
                            recVehicle.IsActive = true;
                            recVehicle.Status = "0";
                            recVehicle.IsProfitCenterSales = true;
                            recVehicle.CreatedBy = CurrentUser.UserId;
                            recVehicle.CreatedDate = DateTime.Now;
                        }
                        recVehicle.ChassisNo = rec.ChassisNo;
                        recVehicle.EngineCode = rec.EngineCode;
                        recVehicle.EngineNo = rec.EngineNo;
                        recVehicle.SalesModelCode = rec.SalesModelCode;
                        recVehicle.SalesModelYear = rec.SalesModelYear;
                        recVehicle.ColourCode = rec.ColourCode;
                        recVehicle.ServiceBookNo = rec.ServiceBookNo;
                        recVehicle.KeyNo = rec.KeyNo;
                        recVehicle.PONo = rec.PONo;
                        recVehicle.BPUNo = rec.BPUNo;
                        recVehicle.BPUDate = rec.BPUDate;
                        recVehicle.WarehouseCode = rec.WarehouseCode;

                        Helpers.ReplaceNullable(recVehicle);
                        if (isNew)
                        {
                            ctx.OmMstVehicleTemps.Add(recVehicle);
                            result = ctx.SaveChanges() > 0;
                        }
                        else
                        {
                            result = ctx.SaveChanges() >= 0;
                        }

                        if (result == true)
                        {
                            bool isNewInvent = false;
                            DateTime bpuDate = Convert.ToDateTime(rec.BPUDate).Date;
                            OmTrInventQtyVehicleBLL omTrInventQtyVehicleBLL = new OmTrInventQtyVehicleBLL(ctx, CurrentUser.UserId);
                            OmTrInventQtyVehicle recInvent = omTrInventQtyVehicleBLL.GetRecord(Convert.ToDecimal(bpuDate.Year), Convert.ToDecimal(bpuDate.Month),
                                rec.SalesModelCode, Convert.ToDecimal(rec.SalesModelYear), rec.ColourCode, rec.WarehouseCode);
                            if (recInvent == null)
                            {
                                isNewInvent = true;
                                recInvent = new OmTrInventQtyVehicle();
                                recInvent.CompanyCode = CompanyCode;
                                recInvent.BranchCode = BranchCode;
                                recInvent.Year = Convert.ToDecimal(bpuDate.Year);
                                recInvent.Month = Convert.ToDecimal(bpuDate.Month);
                                recInvent.SalesModelCode = rec.SalesModelCode;
                                recInvent.SalesModelYear = Convert.ToDecimal(rec.SalesModelYear);
                                recInvent.ColourCode = rec.ColourCode;
                                recInvent.WarehouseCode = rec.WarehouseCode;
                                recInvent.Alocation = 0;
                                recInvent.QtyOut = 0;
                                OmTrInventQtyVehicle rowBegin = omTrInventQtyVehicleBLL.BeginningQtyVehicle(Convert.ToDecimal(bpuDate.Year), Convert.ToDecimal(bpuDate.Month),
                                    rec.SalesModelCode, Convert.ToDecimal(rec.SalesModelYear), rec.ColourCode, rec.WarehouseCode);
                                if (rowBegin != null)
                                {
                                    recInvent.BeginningOH = recInvent.BeginningOH ?? 0;
                                    recInvent.BeginningAV = recInvent.BeginningAV ?? 0;
                                    rowBegin.EndingOH = rowBegin.EndingOH ?? 0;
                                    rowBegin.EndingAV = rowBegin.EndingAV ?? 0; 

                                    recInvent.BeginningOH = Convert.ToDecimal(rowBegin.EndingOH);
                                    recInvent.BeginningAV = Convert.ToDecimal(rowBegin.EndingAV);
                                }
                                else
                                {
                                    recInvent.BeginningOH = 0;
                                    recInvent.BeginningAV = 0;
                                }
                                recInvent.Status = "0";
                                recInvent.CreatedBy = CurrentUser.UserId;
                                recInvent.CreatedDate = DateTime.Now;
                            }
                            recInvent.QtyIn = recInvent.QtyIn ?? 0;
                            recInvent.QtyOut = recInvent.QtyOut ?? 0;
                            recInvent.BeginningOH = recInvent.BeginningOH ?? 0;
                            recInvent.BeginningAV = recInvent.BeginningAV ?? 0;
                            recInvent.EndingOH = recInvent.EndingOH ?? 0;
                            recInvent.EndingAV = recInvent.EndingAV ?? 0;
                            recInvent.Alocation = recInvent.Alocation ?? 0;

                            recInvent.QtyIn = recInvent.QtyIn + 1;
                            recInvent.EndingOH = (recInvent.BeginningOH + recInvent.QtyIn) - recInvent.QtyOut;
                            recInvent.EndingAV = (recInvent.BeginningAV + recInvent.QtyIn) - recInvent.Alocation - recInvent.QtyOut;

                            Helpers.ReplaceNullable(recInvent);
                            if (isNewInvent)
                            {
                                ctx.OmTrInventQtyVehicles.Add(recInvent);
                                result = ctx.SaveChanges() > 0;
                            }
                            else
                            {
                                result = ctx.SaveChanges() >= 0;
                            }

                            omTrInventQtyVehicleBLL = null;
                            if (!result)
                                throw new Exception("Gagal qty inventory kendaraan temporary");
                        }
                        else
                            throw new Exception("Gagal update or insert Master Kendaraan Temporary");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public bool UpdateFromSJ(string bpuNo)
        {
            bool result = false;
            try
            {
                var BPUDtls = ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode == CompanyCode 
                    && p.BranchCode == BranchCode && p.BPUNo == bpuNo && p.isReturn == false);

                var recBPUDtls = BPUDtls.ToList();
                BPUDtls = null;

                if (recBPUDtls != null && recBPUDtls.Count > 0)
                {
                    foreach (var recBPUDtl in recBPUDtls)
                    {
                        try
                        {
                            var recHPPSubDtl = ctx.omTrPurchaseHPPSubDetail.Where(
                                    p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                                    && p.BPUNo == bpuNo && p.HPPSeq == recBPUDtl.BPUSeq
                                ).FirstOrDefault();

                            if (recHPPSubDtl != null)
                            {
                                var recHPPMdl = ctx.omTrPurchaseHPPDetailModel.Where(p => p.CompanyCode == CompanyCode
                                    && p.BranchCode == BranchCode && p.BPUNo == bpuNo && p.SalesModelCode == recHPPSubDtl.SalesModelCode
                                    && p.SalesModelYear == recHPPSubDtl.SalesModelYear).FirstOrDefault();

                                if (recHPPMdl != null)
                                {
                                    OmMstVehicle recVehicle = null;
                                    recVehicle = ctx.OmMstVehicles.Find(CompanyCode, recBPUDtl.ChassisCode, Convert.ToDecimal(recBPUDtl.ChassisNo));
                                    if (recVehicle != null)
                                    {
                                        recVehicle.COGSUnit = recVehicle.COGSUnit ?? 0;
                                        recVehicle.COGSOthers = recVehicle.COGSOthers ?? 0;
                                        recVehicle.PpnBmBuyPaid = recVehicle.PpnBmBuyPaid ?? 0;
                                        recVehicle.PpnBmBuy = recVehicle.PpnBmBuy ?? 0;
                                        recHPPMdl.AfterDiscDPP = recHPPMdl.AfterDiscDPP ?? 0;
                                        recHPPMdl.OthersDPP = recHPPMdl.OthersDPP ?? 0;
                                        recHPPMdl.PPnBMPaid = recHPPMdl.PPnBMPaid ?? 0;
                                        recHPPMdl.AfterDiscPPnBM = recHPPMdl.AfterDiscPPnBM ?? 0;

                                        recVehicle.HPPNo = recHPPMdl.HPPNo;
                                        recVehicle.COGSUnit = Convert.ToDecimal(recHPPMdl.AfterDiscDPP);
                                        recVehicle.COGSOthers = Convert.ToDecimal(recHPPMdl.OthersDPP);
                                        recVehicle.PpnBmBuyPaid = Convert.ToDecimal(recHPPMdl.PPnBMPaid);
                                        recVehicle.PpnBmBuy = Convert.ToDecimal(recHPPMdl.AfterDiscPPnBM);
                                        try
                                        {
                                            Helpers.ReplaceNullable(recVehicle);
                                            result = ctx.SaveChanges() >= 0;
                                            if (result == true)
                                            {
                                                var recordHPPSubDtl = ctx.omTrPurchaseHPPSubDetail.Find(CompanyCode, BranchCode, 
                                                    recHPPMdl.HPPNo, bpuNo, Convert.ToDecimal(recBPUDtl.BPUSeq));

                                                if (recordHPPSubDtl != null)
                                                {
                                                    recordHPPSubDtl.ChassisNo = Convert.ToDecimal(recBPUDtl.ChassisNo);
                                                    recordHPPSubDtl.EngineNo = Convert.ToDecimal(recBPUDtl.EngineNo);
                                                    recordHPPSubDtl.LastUpdateBy = CurrentUser.UserId;
                                                    recordHPPSubDtl.LastUpdateDate = DateTime.Now;
                                                    try
                                                    {
                                                        Helpers.ReplaceNullable(recordHPPSubDtl);
                                                        result = ctx.SaveChanges() >= 0;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        throw new Exception(ex.Message);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(ex.Message);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        #endregion
    }
}