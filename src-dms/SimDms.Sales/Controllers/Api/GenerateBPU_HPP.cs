using SimDms.Sales.BLL;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.Models;
using System.Data.SqlClient;
using System.Transactions;
using System.Data.Entity.Validation;

namespace SimDms.Sales.Controllers.Api
{
    public class GenerateBPU_HPPController : BaseController
    {

        //public JsonResult Default()
        //{
        //    bool checkChassis = false;
        //    string check = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "BPUF" && p.LookUpValue == "STATUS").FirstOrDefault().ParaValue;
        //    if (!string.IsNullOrEmpty(check))
        //        checkChassis = (check.Equals("1")) ? true : false;

        //    return Json(new { isCheckChassis = checkChassis });
        //}

        public JsonResult LookupInvoice()
        {
            try
            {
                //var invoice = ctxMD.omTrSalesInvoices.Where(x => x.CustomerCode == BranchCode && x.isLocked == false);
                var invoice = from a in ctxMD.omTrSalesInvoices
                              join b in ctxMD.gnMstCustomers
                              on new { a.CompanyCode, a.BranchCode, a.CustomerCode } equals new { CompanyCode = CompanyMD, BranchCode = BranchMD, b.CustomerCode }
                              join c in ctxMD.omTrSalesSOs
                              on new { a.CompanyCode, a.BranchCode, a.SONo } equals new { CompanyCode = CompanyMD, BranchCode = BranchMD, c.SONo }
                              where a.CustomerCode == BranchCode && a.isLocked == false && a.Status=="2"
                              select new InvoiceFields
                              {
                                  InvoiceNo = a.InvoiceNo,
                                  InvoiceDate = a.InvoiceDate,
                                  PONo = c.RefferenceNo,
                                  PODate = c.RefferenceDate,
                                  CustomerCode = a.CustomerCode,
                                  CustomerName = b.CustomerName
                              };
                             
                return Json(invoice.AsQueryable().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult getDetail(string InvoiceNo)
        {
            string qry = String.Format(@"SELECT a.SalesModelCode, a.SalesModelYear, b.ColourCode, c.RefferenceDesc1 AS ColourDesc, b.ChassisCode, b.ChassisNo, b.EngineCode, b.EngineNo, d.ServiceBookNo, d.KeyNo 
                                        FROM omTrSalesInvoiceModel a
                                        INNER JOIN omTrSalesInvoiceVin b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.InvoiceNo = a.InvoiceNo
                                        INNER JOIN omMstRefference c ON c.CompanyCode = a.CompanyCode AND c.RefferenceCode = b.ColourCode AND c.RefferenceType='COLO'
                                        INNER JOIN omMstVehicle d ON d.CompanyCode = a.CompanyCode AND d.SalesModelCode = a.SalesModelCode AND d.SalesModelYear = a.SalesModelYear
										AND d.ColourCode = b.ColourCode AND d.ChassisCode = b.ChassisCode AND d.ChassisNo = b.ChassisNo AND d.EngineCode = b.EngineCode AND d.EngineNo = b.EngineNo
                                        WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}' AND a.InvoiceNo='{2}'", CompanyMD, BranchMD, InvoiceNo);
            var dtl = ctxMD.Database.SqlQuery<DetailFields>(qry).AsQueryable();

            if (dtl != null)
            {
                return Json (new { success = true, DataGrid = dtl });
            }
            return Json(new { success = false, message = "Data Detail tidak ada!" });
        }

        public JsonResult doProcess(string InvoiceNo, string PONo, string WHouse)
        {
            string ShipTo = "";
            string ReffDO = "";
            DateTime? ReffDODate = null;
            string ReffSJ = "";
            DateTime? ReffSJDate = null;
            string WH = "";
            string Expedition = "";
            bool result = false;
            Int16 BpuSeq = 1;
            bool isBuyBack = false;
            string dbMD = ctx.Database.SqlQuery<string>("SELECT TOP 1 DbMD from gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'").FirstOrDefault();

            if (dbMD == "") { throw new Exception(); }

            var InvRec = ctxMD.omTrSalesInvoices.Where(x => x.CompanyCode == CompanyMD && x.BranchCode == BranchMD && x.InvoiceNo == InvoiceNo).FirstOrDefault();
            if (InvRec != null)
            {
                var DoRec = ctxMD.omTrSalesDOs.Where(x => x.CompanyCode == CompanyMD && x.BranchCode == BranchMD && x.SONo == InvRec.SONo).FirstOrDefault();
                if (DoRec != null)
                {
                    var SJRec = ctxMD.omTrSalesBPKs.Where(x => x.CompanyCode == CompanyMD && x.BranchCode == BranchMD && x.DONo == DoRec.DONo).FirstOrDefault();
                    if (SJRec != null)
                    {
                        ShipTo = SJRec.ShipTo;
                        ReffDO = DoRec.DONo;
                        ReffDODate = DoRec.DODate;
                        ReffSJ = SJRec.BPKNo;
                        ReffSJDate = SJRec.BPKDate;
                        Expedition = SJRec.Expedition;
                    }else{
                        return Json(new { success = false, message = "Proses generate gagal!, SJRec" });
                    }
                }else{
                    return Json(new { success = false, message = "Proses generate gagal!, DoRec" });
                }
            }else{
                 return Json(new { success = false, message = "Proses generate gagal!, InvRec" });
            }
           
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                        new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    //Process BPU
                    string BPUNo = GetNewDocumentNo(GnMstDocumentConstant.BPU, Convert.ToDateTime(DateTime.Now));
                    var BpuHdr = ctx.omTrPurchaseBPU.Find(CompanyCode, BranchCode, PONo, BPUNo);
                    if (BpuHdr == null)
                    {
                        BpuHdr = new omTrPurchaseBPU();
                        BpuHdr.CompanyCode = CompanyCode;
                        BpuHdr.BranchCode = BranchCode;
                        BpuHdr.PONo = PONo;
                        BpuHdr.BPUNo = BPUNo;
                        BpuHdr.BPUDate = DateTime.Now;
                        BpuHdr.SupplierCode = UnitBranchMD;
                        BpuHdr.ShipTo = ShipTo;
                        BpuHdr.RefferenceDONo = ReffDO;
                        BpuHdr.RefferenceDODate = ReffDODate;
                        BpuHdr.RefferenceSJNo = ReffSJ;
                        BpuHdr.RefferenceSJDate = ReffSJDate;
                        BpuHdr.WarehouseCode = WHouse;
                        BpuHdr.Expedition = Expedition;
                        BpuHdr.BPUType = "2";
                        BpuHdr.Status = "2";
                        BpuHdr.CreatedBy = CurrentUser.UserId;
                        BpuHdr.CreatedDate = DateTime.Now;

                        ctx.omTrPurchaseBPU.Add(BpuHdr);
                        Helpers.ReplaceNullable(BpuHdr);
                        result = ctx.SaveChanges() >= 0;

                    }
                    if (result)
                    {
                        string qry = String.Format(@"SELECT a.SalesModelCode, a.SalesModelYear, b.ColourCode, c.RefferenceDesc1 AS ColourDesc, b.ChassisCode, b.ChassisNo, b.EngineCode, b.EngineNo, d.ServiceBookNo, d.KeyNo 
                                        FROM {3}..omTrSalesInvoiceModel a
                                        INNER JOIN {3}..omTrSalesInvoiceVin b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.InvoiceNo = a.InvoiceNo
                                        INNER JOIN {3}..omMstRefference c ON c.CompanyCode = a.CompanyCode AND c.RefferenceCode = b.ColourCode AND c.RefferenceType='COLO'
                                        INNER JOIN {3}..omMstVehicle d ON d.CompanyCode = a.CompanyCode AND d.SalesModelCode = a.SalesModelCode AND d.SalesModelYear = a.SalesModelYear
										AND d.ColourCode = b.ColourCode AND d.ChassisCode = b.ChassisCode AND d.ChassisNo = b.ChassisNo AND d.EngineCode = b.EngineCode AND d.EngineNo = b.EngineNo
                                        WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}' AND a.InvoiceNo='{2}'", CompanyMD, BranchMD, InvoiceNo, dbMD);

                        var det = ctx.Database.SqlQuery<DetailFields>(qry).ToList();
                        if (det != null)
                        {
                            det.ForEach(y => {
                                var BpuDetail = ctx.omTrPurchaseBPUDetail.Find(CompanyCode, BranchCode, PONo, BPUNo, BpuSeq);
                                BpuDetail = new omTrPurchaseBPUDetail();
                                BpuDetail.CompanyCode = CompanyCode;
                                BpuDetail.BranchCode = BranchCode;
                                BpuDetail.BPUNo = BPUNo;
                                BpuDetail.PONo = PONo;
                                BpuDetail.BPUSeq = BpuSeq;
                                BpuDetail.SalesModelCode =y.SalesModelCode;
                                BpuDetail.SalesModelYear = y.SalesModelYear;
                                BpuDetail.ColourCode = y.ColourCode;
                                BpuDetail.ChassisCode = y.ChassisCode;
                                BpuDetail.ChassisNo = y.ChassisNo;
                                BpuDetail.EngineCode = y.EngineCode;
                                BpuDetail.EngineNo = y.EngineNo;
                                BpuDetail.ServiceBookNo = y.ServiceBookNo;
                                BpuDetail.KeyNo = y.KeyNo;
                                BpuDetail.Remark = BpuDetail.SJRelNo = BpuDetail.DORelNo = "";
                                BpuDetail.SJRelDate = BpuDetail.DORelDate = new DateTime(1900, 1, 1);
                                BpuDetail.StatusHPP = "1";
                                BpuDetail.StatusSJRel = BpuDetail.StatusDORel = "0";
                                BpuDetail.isReturn = false;
                                BpuDetail.CreatedBy = BpuDetail.LastUpdateBy = CurrentUser.UserId;
                                BpuDetail.CreatedDate = BpuDetail.LastUpdateDate = DateTime.Now;
 
                                ctx.omTrPurchaseBPUDetail.Add(BpuDetail);

                                result = ctx.SaveChanges() >= 0;

                                if (!result) { throw new Exception(); }
                  
                                OmMstVehicle vehicle = ctx.OmMstVehicles.Find(CompanyCode, y.ChassisCode, y.ChassisNo);
                                //var vMD = ctxMD.OmMstVehicles.Find(CompanyMD, y.ChassisCode, y.ChassisNo);
                                if (vehicle != null)
                                {
                                    //vehicle.SuzukiDONo = vMD.SuzukiDONo;
                                    //vehicle.SuzukiDODate = Convert.ToDateTime(vMD.SuzukiDODate);
                                    //vehicle.SuzukiSJNo = vMD.SuzukiSJNo;
                                    //vehicle.SuzukiSJDate = Convert.ToDateTime(vMD.SuzukiSJDate);

                                    //result = ctx.SaveChanges() >= 0;

                                    isBuyBack = true;
                                    OmMstVehicleHistory oOmMstVehicleHistory = new OmMstVehicleHistory();
                                    oOmMstVehicleHistory.CompanyCode = CompanyCode;
                                    oOmMstVehicleHistory.ChassisCode = vehicle.ChassisCode;
                                    oOmMstVehicleHistory.ChassisNo = vehicle.ChassisNo;
                                    oOmMstVehicleHistory.SeqNo = GetSeqNoVehicleHist(vehicle.ChassisCode, Convert.ToInt32(vehicle.ChassisNo));
                                    oOmMstVehicleHistory.EngineCode = vehicle.EngineCode;
                                    oOmMstVehicleHistory.EngineNo = (Decimal)vehicle.EngineNo;
                                    oOmMstVehicleHistory.SalesModelCode = vehicle.SalesModelCode;
                                    oOmMstVehicleHistory.SalesModelYear = (Decimal)vehicle.SalesModelYear;
                                    oOmMstVehicleHistory.ColourCode = vehicle.ColourCode;
                                    oOmMstVehicleHistory.ServiceBookNo = vehicle.ServiceBookNo;
                                    oOmMstVehicleHistory.KeyNo = vehicle.KeyNo;
                                    oOmMstVehicleHistory.COGSUnit = vehicle.COGSUnit ?? 0;
                                    oOmMstVehicleHistory.COGSOthers = vehicle.COGSOthers ?? 0;
                                    oOmMstVehicleHistory.COGSKaroseri = vehicle.COGSKaroseri ?? 0;
                                    oOmMstVehicleHistory.PpnBmBuyPaid = vehicle.PpnBmBuyPaid ?? 0;
                                    oOmMstVehicleHistory.PpnBmBuy = vehicle.PpnBmBuy ?? 0;
                                    oOmMstVehicleHistory.SalesNetAmt = vehicle.SalesNetAmt ?? 0;
                                    oOmMstVehicleHistory.PpnBmSellPaid = vehicle.PpnBmSellPaid ?? 0;
                                    oOmMstVehicleHistory.PpnBmSell = vehicle.PpnBmSell ?? 0;
                                    oOmMstVehicleHistory.PONo = vehicle.PONo;
                                    oOmMstVehicleHistory.POReturnNo = vehicle.POReturnNo;
                                    oOmMstVehicleHistory.BPUNo = vehicle.BPUNo;
                                    oOmMstVehicleHistory.HPPNo = vehicle.HPPNo;
                                    oOmMstVehicleHistory.KaroseriSPKNo = vehicle.KaroseriSPKNo;
                                    oOmMstVehicleHistory.SONo = vehicle.SONo;
                                    oOmMstVehicleHistory.SOReturnNo = vehicle.SOReturnNo;
                                    oOmMstVehicleHistory.DONo = vehicle.DONo;
                                    oOmMstVehicleHistory.BPKNo = vehicle.BPKNo;
                                    oOmMstVehicleHistory.InvoiceNo = vehicle.InvoiceNo;
                                    oOmMstVehicleHistory.ReqOutNo = vehicle.ReqOutNo;
                                    oOmMstVehicleHistory.TransferOutNo = vehicle.TransferOutNo;
                                    oOmMstVehicleHistory.TransferInNo = vehicle.TransferInNo;
                                    oOmMstVehicleHistory.WarehouseCode = vehicle.WarehouseCode;
                                    oOmMstVehicleHistory.Remark = "PURCHASE";
                                    oOmMstVehicleHistory.Status = vehicle.Status;
                                    oOmMstVehicleHistory.IsAlreadyPDI = vehicle.IsAlreadyPDI;
                                    oOmMstVehicleHistory.BPUDate = Convert.ToDateTime(vehicle.BPUDate);
                                    oOmMstVehicleHistory.FakturPolisiNo = vehicle.FakturPolisiNo;
                                    oOmMstVehicleHistory.FakturPolisiDate = Convert.ToDateTime(vehicle.FakturPolisiDate);
                                    oOmMstVehicleHistory.PoliceRegistrationNo = vehicle.PoliceRegistrationNo;
                                    oOmMstVehicleHistory.PoliceRegistrationDate = Convert.ToDateTime(vehicle.PoliceRegistrationDate);
                                    oOmMstVehicleHistory.IsProfitCenterSales = vehicle.IsProfitCenterSales;
                                    oOmMstVehicleHistory.IsProfitCenterService = vehicle.IsProfitCenterService;
                                    oOmMstVehicleHistory.IsActive = vehicle.IsActive;
                                    oOmMstVehicleHistory.CreatedBy = CurrentUser.UserId;
                                    oOmMstVehicleHistory.CreatedDate = DateTime.Now;
                                    oOmMstVehicleHistory.LastUpdateBy = CurrentUser.UserId;
                                    oOmMstVehicleHistory.LastUpdateDate = DateTime.Now;
                                    oOmMstVehicleHistory.IsLocked = vehicle.IsLocked;
                                    oOmMstVehicleHistory.LockedBy = vehicle.LockedBy;
                                    oOmMstVehicleHistory.LockedDate = Convert.ToDateTime(vehicle.LockedDate);
                                    oOmMstVehicleHistory.IsNonRegister = vehicle.IsNonRegister;
                                    oOmMstVehicleHistory.BPUDate = Convert.ToDateTime(vehicle.BPUDate);
                                    oOmMstVehicleHistory.SuzukiDONo = vehicle.SuzukiDONo;
                                    oOmMstVehicleHistory.SuzukiDODate = vehicle.SuzukiDODate;
                                    oOmMstVehicleHistory.SuzukiSJNo = vehicle.SuzukiSJNo;
                                    oOmMstVehicleHistory.SuzukiSJDate = vehicle.SuzukiSJDate;
                                    
                                    ctx.OmMstVehicleHistorys.Add(oOmMstVehicleHistory);
                                    Helpers.ReplaceNullable(oOmMstVehicleHistory);
                                    result = ctx.SaveChanges() >= 0;

                                    if (!result) { throw new Exception(); }
                                }
                                else
                                {
                                    //var vMD = ctxMD.OmMstVehicles.Find(CompanyMD, y.ChassisCode, y.ChassisNo);
                                    var vMD = ctx.Database.SqlQuery<OmMstVehicle>("SELECT * FROM " + dbMD + "..OmMstVehicle WHERE CompanyCode='" + CompanyMD + "' AND ChassisCode='" + y.ChassisCode + "' AND ChassisNo='" + y.ChassisNo + "'").FirstOrDefault();
                                    if (vMD != null)
                                    {
                                        OmMstVehicle newMstVehicle = new OmMstVehicle();
                                        newMstVehicle.CompanyCode = CompanyCode;
                                        newMstVehicle.ChassisCode = vMD.ChassisCode;
                                        newMstVehicle.ChassisNo = Convert.ToDecimal(vMD.ChassisNo);
                                        newMstVehicle.IsActive = true;
                                        newMstVehicle.Status = "0";
                                        newMstVehicle.IsProfitCenterSales = true;
                                        newMstVehicle.CreatedBy = CurrentUser.UserId;
                                        newMstVehicle.CreatedDate = DateTime.Now;
                                        newMstVehicle.EngineCode = vMD.EngineCode;
                                        newMstVehicle.EngineNo = Convert.ToDecimal(vMD.EngineNo);
                                        newMstVehicle.SalesModelCode = vMD.SalesModelCode;
                                        newMstVehicle.SalesModelYear = Convert.ToDecimal(vMD.SalesModelYear);
                                        newMstVehicle.ColourCode = vMD.ColourCode;
                                        newMstVehicle.ServiceBookNo = vMD.ServiceBookNo;
                                        newMstVehicle.KeyNo = vMD.KeyNo;
                                        newMstVehicle.COGSUnit = vMD.COGSUnit;
                                        newMstVehicle.COGSOthers = vMD.COGSOthers;
                                        newMstVehicle.COGSKaroseri = vMD.COGSKaroseri;
                                        newMstVehicle.PpnBmBuyPaid = vMD.PpnBmBuyPaid;
                                        newMstVehicle.PpnBmBuy = vMD.PpnBmBuy;
                                        newMstVehicle.PpnBmSellPaid = vMD.PpnBmSellPaid;
                                        newMstVehicle.PpnBmSell = vMD.PpnBmSell;
                                        newMstVehicle.PONo = vMD.PONo;
                                        newMstVehicle.BPUNo = vMD.BPUNo;
                                        newMstVehicle.BPUDate = Convert.ToDateTime(vMD.BPUDate);
                                        newMstVehicle.WarehouseCode = WHouse;
                                        newMstVehicle.SuzukiDONo = vMD.SuzukiDONo;
                                        newMstVehicle.SuzukiDODate = Convert.ToDateTime(vMD.SuzukiDODate);
                                        newMstVehicle.SuzukiSJNo = vMD.SuzukiSJNo;
                                        newMstVehicle.SuzukiSJDate = Convert.ToDateTime(vMD.SuzukiSJDate);
                                        newMstVehicle.TransferInMultiBranchNo = "";
                                        newMstVehicle.TransferOutMultiBranchNo = "";

                                        ctx.OmMstVehicles.Add(newMstVehicle);
                                        Helpers.ReplaceNullable(newMstVehicle);

                                        if (!result) { throw new Exception(); }
                                    }

                                    var recInvent = ctx.OmTrInventQtyVehicles.Find(CompanyCode, BranchCode, DateTime.Now.Year, DateTime.Now.Month, y.SalesModelCode, y.SalesModelYear, y.ColourCode, WHouse);
                                    if (recInvent == null)
                                    {
                                        recInvent = new OmTrInventQtyVehicle();
                                        recInvent.CompanyCode = CompanyCode;
                                        recInvent.BranchCode = BranchCode;
                                        recInvent.Year = Convert.ToDecimal(DateTime.Now.Year);
                                        recInvent.Month = Convert.ToDecimal(DateTime.Now.Month);
                                        recInvent.SalesModelCode = y.SalesModelCode;
                                        recInvent.SalesModelYear = Convert.ToDecimal(y.SalesModelYear);
                                        recInvent.ColourCode = y.ColourCode;
                                        recInvent.WarehouseCode = WHouse;
                                        recInvent.Alocation = 0;
                                        recInvent.QtyOut = 0;
                                        recInvent.QtyIn = recInvent.QtyIn + 1;

                                        var recBegin = ctx.OmTrInventQtyVehicles.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                                            && (p.Year == DateTime.Now.Year && p.Month < DateTime.Now.Month || p.Year < DateTime.Now.Year) 
                                            && p.SalesModelCode == y.SalesModelCode && p.SalesModelYear == y.SalesModelYear && p.ColourCode == y.ColourCode && p.WarehouseCode == WHouse)
                                            .OrderByDescending(p => p.Year).OrderByDescending(p => p.Month).FirstOrDefault();

                                        if (recBegin != null)
                                        {
                                            recBegin.EndingOH = recBegin.EndingOH ?? 0;
                                            recBegin.EndingAV = recBegin.EndingAV ?? 0;

                                            recInvent.BeginningOH = Convert.ToDecimal(recBegin.EndingOH);
                                            recInvent.BeginningAV = Convert.ToDecimal(recBegin.EndingAV);
                                        }
                                        recInvent.EndingOH = (recInvent.BeginningOH + recInvent.QtyIn) - recInvent.QtyOut;
                                        recInvent.EndingAV = (recInvent.BeginningAV + recInvent.QtyIn) - recInvent.Alocation - recInvent.QtyOut;
                                        recInvent.Status = "0";
                                        recInvent.CreatedBy = CurrentUser.UserId;
                                        recInvent.CreatedDate = DateTime.Now;

                                        Helpers.ReplaceNullable(recInvent);
                                        ctx.OmTrInventQtyVehicles.Add(recInvent);
                                        result = ctx.SaveChanges() > 0;

                                        if (!result) { throw new Exception(); }
                                    }
                                    else
                                    {
                                        recInvent.QtyIn = recInvent.QtyIn + 1;
                                        recInvent.EndingOH = (recInvent.BeginningOH + recInvent.QtyIn) - recInvent.QtyOut;
                                        recInvent.EndingAV = (recInvent.BeginningAV + recInvent.QtyIn) - recInvent.Alocation - recInvent.QtyOut;
                                        recInvent.LastUpdateBy = CurrentUser.UserId;
                                        recInvent.LastUpdateDate = DateTime.Now;

                                        result = ctx.SaveChanges() > 0;
                                        if (!result) { throw new Exception(); }
                                    }
                                }
                                BpuSeq += 1;
                            });
                            result = ctx.SaveChanges() >= 0;
                        }

                        if (result)
                        {
                            //ctxMD.omTrSalesInvoiceModels.Where(x => x.CompanyCode == CompanyMD && x.BranchCode == BranchMD && x.InvoiceNo == InvoiceNo).ToList().ForEach(y => {
                            ctx.Database.SqlQuery<omTrSalesInvoiceModel>(String.Format(@"SELECT * FROM {3}..omTrSalesInvoiceModel WHERE CompanyCode='{0}' AND BranchCode='{1}' AND InvoiceNo='{2}'", CompanyMD, BranchMD, InvoiceNo, dbMD)).ToList().ForEach(y => {  
                                var qr = String.Format(@"SELECT Count(*) FROM {5}..omTrSalesInvoiceModel WHERE CompanyCode = '{0}' AND BranchCode = '{1}' AND InvoiceNo='{2}'
                                                        AND SalesModelCode='{3}' AND SalesModelYear='{4}'", CompanyMD, BranchMD, InvoiceNo, y.SalesModelCode, y.SalesModelYear, dbMD);
                                //var BpuModel = ctx.OmTrPurchaseBPUDetailModels.Find(CompanyCode, BranchCode, PONo, BPUNo, y.SalesModelCode, y.SalesModelYear);
                                OmTrPurchaseBPUDetailModel BpuModel = new OmTrPurchaseBPUDetailModel();
                                BpuModel.CompanyCode = CompanyCode;
                                BpuModel.BranchCode = BranchCode;
                                BpuModel.PONo = PONo;
                                BpuModel.BPUNo = BPUNo;
                                BpuModel.SalesModelCode = y.SalesModelCode;
                                BpuModel.SalesModelYear = y.SalesModelYear;
                                Int32 qty = ctx.Database.SqlQuery<Int32>(qr).FirstOrDefault();
                                BpuModel.QuantityBPU = qty;
                                BpuModel.QuantityHPP = qty;
                                BpuModel.CreatedBy = CurrentUser.UserId;
                                BpuModel.CreatedDate = DateTime.Now;
                                BpuModel.LastUpdateBy = CurrentUser.UserId;
                                BpuModel.LastUpdateDate = DateTime.Now;

                                ctx.OmTrPurchaseBPUDetailModels.Add(BpuModel);
                            });
                            result = ctx.SaveChanges() >= 0;
                        }
                        else
                        {
                            throw new Exception(); 
                        }
                    }
                    else
                    {
                        throw new Exception(); 
                    }
                    //End BPU
                    //Process HPP
                    string HPPNo = GetNewDocumentNo("HPU", DateTime.Now);
                    omTrPurchaseHPP HppHdr = new omTrPurchaseHPP();
                    HppHdr.CompanyCode = CompanyCode;
                    HppHdr.BranchCode = BranchCode;
                    HppHdr.HPPNo = HPPNo;
                    HppHdr.HPPDate = DateTime.Now;
                    HppHdr.PONo = PONo;
                    HppHdr.SupplierCode = UnitBranchMD;
                    HppHdr.RefferenceInvoiceNo = InvoiceNo;
                    HppHdr.RefferenceInvoiceDate = InvRec.InvoiceDate;
                    HppHdr.RefferenceFakturPajakNo = HppHdr.Remark = HppHdr.LockingBy = "";
                    HppHdr.RefferenceFakturPajakDate = HppHdr.DueDate = HppHdr.LockingDate = new DateTime(1900,1,1);
                    HppHdr.Status = "2";
                    HppHdr.isLocked = false;
                    HppHdr.CreatedBy = CurrentUser.UserId;
                    HppHdr.CreatedDate = DateTime.Now;

                    ctx.omTrPurchaseHPP.Add(HppHdr);
                    result = ctx.SaveChanges() >= 0;
                    if (!result) { throw new Exception(); }

                    omTrPurchaseHPPDetail HppDtl = new omTrPurchaseHPPDetail();
                    HppDtl.CompanyCode = CompanyCode;
                    HppDtl.BranchCode = BranchCode;
                    HppDtl.HPPNo = HPPNo;
                    HppDtl.BPUNo = BPUNo;
                    HppDtl.Remark = "";
                    HppDtl.CreatedBy = HppDtl.LastUpdateBy = CurrentUser.UserId;
                    HppDtl.CreatedDate = HppDtl.LastUpdateDate = DateTime.Now;

                    ctx.omTrPurchaseHPPDetail.Add(HppDtl);
                    result = ctx.SaveChanges() >= 0;
                    if (!result) { throw new Exception(); }

                    //ctxMD.omTrSalesInvoiceModels.Where(x => x.CompanyCode == CompanyMD && x.BranchCode == BranchMD && x.InvoiceNo == InvoiceNo).ToList()
                    ctx.Database.SqlQuery<omTrSalesInvoiceModel>(String.Format(@"SELECT * FROM {3}..omTrSalesInvoiceModel WHERE CompanyCode='{0}' AND BranchCode='{1}' AND InvoiceNo='{2}'", CompanyMD, BranchMD, InvoiceNo, dbMD)).ToList()
                    .ForEach(z => {
                        omTrPurchaseHPPDetailModel HppMdl = new omTrPurchaseHPPDetailModel();
                        HppMdl.CompanyCode = CompanyCode;
                        HppMdl.BranchCode = BranchCode;
                        HppMdl.HPPNo = HPPNo;
                        HppMdl.BPUNo = BPUNo;
                        HppMdl.SalesModelCode = z.SalesModelCode;
                        HppMdl.SalesModelYear = z.SalesModelYear;
                        HppMdl.Quantity = z.Quantity;
                        HppMdl.BeforeDiscDPP = z.BeforeDiscDPP;
                        HppMdl.DiscExcludePPn = z.DiscExcludePPn;
                        HppMdl.AfterDiscDPP = z.AfterDiscDPP;
                        HppMdl.AfterDiscPPn = z.AfterDiscPPn;
                        HppMdl.AfterDiscPPnBM = z.AfterDiscPPnBM;
                        HppMdl.AfterDiscTotal = z.AfterDiscTotal;
                        HppMdl.PPnBMPaid = z.PPnBMPaid;
                        HppMdl.OthersDPP = z.OthersDPP != null ? z.OthersDPP : 0;
                        HppMdl.OthersPPn = z.OthersPPn != null ? z.OthersPPn : 0;
                        HppMdl.Remark = "";
                        HppMdl.CreatedBy = HppMdl.LastUpdateBy = CurrentUser.UserId;
                        HppMdl.CreatedDate = HppMdl.LastUpdateDate = DateTime.Now;

                        ctx.omTrPurchaseHPPDetailModel.Add(HppMdl);
                        result = ctx.SaveChanges() >= 0;
                        if (!result) { throw new Exception(); }

                    });

                    Int16 iSeq = 0;
                    //ctxMD.omTrSalesInvoiceVins.Where(x => x.CompanyCode == CompanyMD && x.BranchCode == BranchMD && x.InvoiceNo == InvoiceNo).ToList()
                    ctx.Database.SqlQuery<omTrSalesInvoiceVin>(String.Format(@"SELECT * FROM {3}..omTrSalesInvoiceVin WHERE CompanyCode='{0}' AND BranchCode='{1}' AND InvoiceNo='{2}'", CompanyMD, BranchMD, InvoiceNo, dbMD)).ToList()    
                        .ForEach(z =>{
                            iSeq += 1;
                            omTrPurchaseHPPSubDetail HppSubDet = new omTrPurchaseHPPSubDetail();
                            HppSubDet.CompanyCode = CompanyCode;
                            HppSubDet.BranchCode = BranchCode;
                            HppSubDet.HPPNo = HPPNo;
                            HppSubDet.BPUNo = BPUNo;
                            HppSubDet.HPPSeq = iSeq;
                            HppSubDet.SalesModelCode = z.SalesModelCode;
                            HppSubDet.SalesModelYear = z.SalesModelYear;
                            HppSubDet.ColourCode = z.ColourCode;
                            HppSubDet.ChassisCode = z.ChassisCode;
                            HppSubDet.ChassisNo = (Decimal)z.ChassisNo;
                            HppSubDet.EngineCode = z.EngineCode;
                            HppSubDet.EngineNo = z.EngineNo;
                            HppSubDet.Remark = "";
                            HppSubDet.isReturn = false;
                            HppSubDet.CreatedBy = CurrentUser.UserId;
                            HppSubDet.CreatedDate = DateTime.Now;

                            ctx.omTrPurchaseHPPSubDetail.Add(HppSubDet);
                            result = ctx.SaveChanges() >= 0;
                            if (!result) { throw new Exception(); }

                            //Update omMstvehicle
                            //ctxMD.omTrSalesInvoiceModels.Where(x => x.CompanyCode == CompanyMD && x.BranchCode == BranchMD && x.InvoiceNo == InvoiceNo).ToList()

                            var veh = ctx.OmMstVehicles.Find(CompanyCode, z.ChassisCode, z.ChassisNo);
                            if (veh != null)
                            {
                                var mdl = ctx.omTrPurchaseHPPDetailModel.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.SalesModelCode == z.SalesModelCode && m.SalesModelYear == z.SalesModelYear && m.HPPNo == HPPNo && m.BPUNo == BPUNo).FirstOrDefault();
                                string qr = String.Format(@"SELECT ISNULL(SUM(c.OthersDPP),0) FROM OmTrPurchaseHPPDetailModelOthers c WHERE c.CompanyCode = '{0}'
												            AND c.BPUNo = '{1}' AND c.SalesModelCode = '{2}' AND c.SalesModelYear = '{3}'
												            AND c.OthersCode in (SELECT OthersNonInventory FROM omMstOthersNonInventory)", CompanyCode, BPUNo, z.SalesModelCode, z.SalesModelYear);
                                decimal vOther = ctx.Database.SqlQuery<decimal>(qr).FirstOrDefault();
                                if (mdl != null)
                                {
                                    veh.COGSUnit = mdl.AfterDiscDPP;
                                    veh.COGSOthers = mdl.OthersDPP - vOther;
                                    veh.PpnBmBuyPaid = mdl.PPnBMPaid;
                                    veh.PpnBmBuy = mdl.AfterDiscPPnBM;
                                    veh.HPPNo = mdl.HPPNo;
                                    veh.LastUpdateBy = CurrentUser.UserId;
                                    veh.LastUpdateDate = DateTime.Now;

                                    result = ctx.SaveChanges() >= 0;
                                    if (!result) { throw new Exception(); }
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                            else
                            {
                                throw new Exception();
                            }
                        });
                    //End HPP
                    //Update Invoice
                    ctx.Database.ExecuteSqlCommand("UPDATE " + dbMD + "..omTrSalesInvoice SET IsLocked=1 WHERE CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' AND InvoiceNo='" + InvoiceNo + "'");

                    if (result)
                    {
                        tranScope.Complete();
                        return Json(new { success = true, message="Proses generate berhasil!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Proses generate gagal!" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Proses generate gagal!", error_log = ex.Message });
            }
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

    }

    public class InvoiceFields
    {
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }

    }

    public class DetailFields
    {
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ColourDesc { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal EngineNo { get; set; }
        public string ServiceBookNo {get; set; }
        public string KeyNo {get; set; }

    }
   
}
