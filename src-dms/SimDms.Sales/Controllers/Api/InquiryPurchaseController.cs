using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers.Api
{
    public class InquiryPurchaseController : BaseController
    {
        //Inquiry PO

        public JsonResult searchPO(string Status, DateTime PODate, DateTime PODateTo, string NoReff, string NoReffTo, string NoPO, string NoPOTo, string SupplierCode, string SupplierCodeTo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrPurchasePOView>("sp_InquiryPurchaseOrder '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + PODate + "','" + PODateTo + "','" + NoReff + "','" + NoReffTo + "','" + NoPO + "','" + NoPOTo + "','" + SupplierCode + "','" + SupplierCodeTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetPO(string PONo)
        {
            var gridDetail = ctx.OmTrPurchasePOModels.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PONo == PONo);
            return Json(new { success = true, grid = gridDetail });
        }

        public JsonResult GetDetailPOColour(PODetailModel model)
        {
            var gridColour = ctx.OmTrPurchasePOModelColours.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PONo == model.PONo && a.SalesModelCode == model.SalesModelCode);
            return Json(new { success = true, grid = gridColour });
        }

        //Inquiry BPU

        public JsonResult searchBPU(string Status, string BPUType, DateTime BPUDate, DateTime BPUDateTo, string NoPO, string NoPOTo, string NoRefDO, string NoRefDOTo, string NoRefSJ, string NoRefSJTo, string NoBPU, string NoBPUTo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrPurchaseBPUView>
                ("sp_InquiryBPU '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + BPUType + "','" + BPUDate + "','" + BPUDateTo + "','" + NoPO + "','" + NoPOTo + "','" + NoRefDO + "','" + NoRefDOTo + "','" + NoRefSJ + "','" + NoRefSJTo + "','" + NoBPU + "','" + NoBPUTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetDetailBPU(BPUDetailModel model)
        {
            var detail = ctx.omTrPurchaseBPUDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BPUNo == model.BPUNo).ToList()
                .Select(m => new InquiryTrPurchaseBPUDetailView
                {
                    SalesModelCode = m.SalesModelCode,
                    SalesModelYear = m.SalesModelYear,
                    SalesModelDesc = ctx.MstModels.FirstOrDefault(f => f.CompanyCode == CompanyCode && f.SalesModelCode == m.SalesModelCode).SalesModelDesc,
                    ColourCode = m.ColourCode,
                    ColourName = ctx.MstRefferences.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.RefferenceType == "COLO" && e.RefferenceCode == m.ColourCode && e.Status == "1").RefferenceDesc1,
                    ChassisCode = m.ChassisCode,
                    ChassisNo = m.ChassisNo,
                    EngineCode = m.EngineCode,
                    EngineNo = m.EngineNo,
                    ServiceBookNo = m.ServiceBookNo,
                    KeyNo = m.KeyNo,
                    Remark = m.Remark
                });

            return Json(new { success = true, detail = detail });
        }

        //Inquiry HPP

        public JsonResult searchHPP(string Status, DateTime HPPDate, DateTime HPPDateTo, string NoReff, string NoReffTo, string NoHPP, string NoHPPTo, string NoPO, string NoPOTo, string SupplierCode, string SupplierCodeTo)
        {
            var sqlstr = ctx.Database.SqlQuery<InquiryTrPurchaseHPPView>
                ("sp_InquiryHPP '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + HPPDate + "','" + HPPDateTo + "','" + NoReff + "','" + NoReffTo + "','" + NoHPP + "','" + NoHPPTo + "','" + NoPO + "','" + NoPOTo + "','" + SupplierCode + "','" + SupplierCodeTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetDetailHPP(HPPDetailModel model)
        {
            var gridDetail = ctx.omTrPurchaseHPPDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.HPPNo == model.HPPNo).ToList()
                .Select(m => new HPPDetailModel
                {
                    BPUNo = m.BPUNo,
                    RefferenceDONo = ctx.omTrPurchaseBPU.FirstOrDefault(f => f.CompanyCode == CompanyCode && f.BPUNo == m.BPUNo).RefferenceDONo,
                    RefferenceSJNo = ctx.omTrPurchaseBPU.FirstOrDefault(f => f.CompanyCode == CompanyCode && f.BPUNo == m.BPUNo).RefferenceSJNo,
                    Remark = m.Remark
                });
            return Json(new { success = true, detail = gridDetail });
        }

        public JsonResult GetModel(HPPDetailModel model)
        {
            var gridDetail = ctx.omTrPurchaseHPPDetailModel.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BPUNo == model.BPUNo);
            return Json(new { success = true, detail = gridDetail });
        }

        public JsonResult GetDetailModelHPP(HPPDetailModel model)
        {
            var gridDetail = ctx.omTrPurchaseHPPSubDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BPUNo == model.BPUNo && a.SalesModelCode == model.SalesModelCode);
            return Json(new { success = true, detail = gridDetail });
        }

        //Inquiry PurchaseReturn

        public JsonResult searchPurchaseReturn(string Status, DateTime ReturnDate, DateTime ReturnDateTo, string NoHPP, string NoHPPTo, string NoReturn, string NoReturnTo)
        {
            var sqlstr = ctx.Database.SqlQuery<inquiryTrPurchaseReturnView>
                ("sp_InquiryPurchaseReturn '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + ReturnDate + "','" + ReturnDateTo + "','" + NoHPP + "','" + NoHPPTo + "','" + NoReturn + "','" + NoReturnTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetDetailReturn(ReturnDetailModel model)
        {
            var gridDetail = ctx.omTrPurchaseReturnDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ReturnNo == model.ReturnNo);
            return Json(new { success = true, detail = gridDetail });
        }

        public JsonResult GetDetailModel(ReturnDetailModel model)
        {
            var gridDetail = ctx.omTrPurchaseReturnSubDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ReturnNo == model.ReturnNo && a.BPUNo == model.BPUNo);
            return Json(new { success = true, detail = gridDetail });
        }

        //Inquiry PerlengkapanIn

        public JsonResult searchPerlengkapanIn(string Status, string PerlengkapanType, DateTime PerlengkapanDate, DateTime PerlengkapanDateTo, string NoReff, string NoReffTo, string NoPerlengkapan, string NoPerlengkapanTo)
        {
            var sqlstr = ctx.Database.SqlQuery<inquiryTrPerlengkapanInView>
                ("sp_InquiryPerlengkapanIn '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + PerlengkapanType + "','" + PerlengkapanDate + "','" + PerlengkapanDateTo + "','" + NoReff + "','" + NoReffTo + "','" + NoPerlengkapan + "','" + NoPerlengkapanTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetDetailPerlengkapanIn(inquiryTrPerlengkapanInView model)
        {
            var queryable = from p in ctx.omTrPurchasePerlengkapanInDetail
                            where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.PerlengkapanNo == model.PerlengkapanNo
                            select new PerlengkapanInDetailView()
                            {
                                PerlengkapanName = ctx.MstPerlengkapan.FirstOrDefault(f => f.CompanyCode == CompanyCode && f.PerlengkapanCode == p.PerlengkapanCode).PerlengkapanName,
                                Quantity = p.Quantity,
                                Remark = p.Remark
                            };
            //var gridDetail = ctx.omTrPurchasePerlengkapanInDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PerlengkapanNo == model.PerlengkapanNo);
            return Json(new { success = true, detail = queryable });
        }

        //Inquiry PerlengkapanAdjustment

        public JsonResult searchPerlengkapanAdjustment(string Status, DateTime AdjustmentDate, DateTime AdjustmentDateTo, string NoReff, string NoReffTo, string NoAdjustment, string NoAdjustmentTo)
        {
            var sqlstr = ctx.Database.SqlQuery<inquiryTrPerlengkapanAdjustmentView>
                ("sp_InquiryPerlengkapanAdjustent '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + AdjustmentDate + "','" + AdjustmentDateTo + "','" + NoReff + "','" + NoReffTo + "','" + NoAdjustment + "','" + NoAdjustmentTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetDetailPerlengkapanAdjustment(inquiryTrPerlengkapanAdjustmentView model)
        {
            var queryable = from p in ctx.omTrPurchasePerlengkapanAdjustmentDetail
                            where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.AdjustmentNo == model.AdjustmentNo
                            select new PerlengkapanAdjustmentDetailView()
                            {
                                PerlengkapanName = ctx.MstPerlengkapan.FirstOrDefault(f => f.CompanyCode == CompanyCode && f.PerlengkapanCode == p.PerlengkapanCode).PerlengkapanName,
                                Quantity = p.Quantity,
                                Remark = p.Remark
                            };
            //var gridDetail = ctx.omTrPurchasePerlengkapanInDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PerlengkapanNo == model.PerlengkapanNo);
            return Json(new { success = true, detail = queryable });
        }

        //Inquiry Karoseri

        public JsonResult searchKaroseri(string Status, DateTime KaroseriSPKtDate, DateTime KaroseriSPKtDateTo, string SalesModelCodeOld, string SalesModelCodeOldTo, string SalesModelYear, string SalesModelYearTo, string NoSPKKaroseri, string NoSPKKaroseriTo, string SupplierCode, string SupplierCodeTo)
        {
            var sqlstr = ctx.Database.SqlQuery<inquiryTrPurchaseKaroseriView>
                ("sp_InquiryKaroseri '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + KaroseriSPKtDate + "','" + KaroseriSPKtDateTo + "','" + SalesModelCodeOld + "','" + SalesModelCodeOldTo + "','" + SalesModelYear + "','" + SalesModelYearTo + "','" + NoSPKKaroseri + "','" + NoSPKKaroseriTo + "','" + SupplierCode + "','" + SupplierCodeTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetKaroseriDetail(KaroseriDetailModel model)
        {
            var gridDetail = ctx.omTrPurchaseKaroseriDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.KaroseriSPKNo == model.KaroseriSPKNo);
            return Json(new { success = true, detail = gridDetail });
        }

        //Inquiry KaroseriTerima

        public JsonResult searchKaroseriTerima(string Status, DateTime KaroseriTerimaDate, DateTime KaroseriTerimaDateTo, string SalesModelCodeOld, string SalesModelCodeOldTo, string SalesModelYear, string SalesModelYearTo, string NoSPKKaroseri, string NoSPKKaroseriTo, string KaroseriTerimaNo, string KaroseriTerimaNoTo)
        {
            var sqlstr = ctx.Database.SqlQuery<inquiryTrPurchaseKaroseriTerimaView>
                ("sp_InquiryKaroseriTerima '" + CompanyCode + "','" + BranchCode + "','" + Status + "','" + KaroseriTerimaDate + "','" + KaroseriTerimaDateTo + "','" + SalesModelCodeOld + "','" + SalesModelCodeOldTo + "','" + SalesModelYear + "','" + SalesModelYearTo + "','" + NoSPKKaroseri + "','" + NoSPKKaroseriTo + "','" + KaroseriTerimaNo + "','" + KaroseriTerimaNoTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetKaroseriTerimaDetail(KaroseriTerimaDetailModel model)
        {
            var gridDetail = ctx.omTrPurchaseKaroseriTerimaDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.KaroseriTerimaNo == model.KaroseriTerimaNo);
            return Json(new { success = true, detail = gridDetail });
        }
    }
}
