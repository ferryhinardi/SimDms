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
    public class BPUController : BaseController
    {

        public JsonResult Default()
        {
            bool checkChassis = false;
            string check = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "BPUF" && p.LookUpValue == "STATUS").FirstOrDefault().ParaValue;
            if (!string.IsNullOrEmpty(check))
                checkChassis = (check.Equals("1")) ? true : false;

            return Json(new { isCheckChassis = checkChassis });
        }

        public JsonResult GetWarehouse(string BPUType)
        {
            var ParaValue = BPUType.Contains("SJ") ? "SJ" : BPUType;

            var rec = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "DWHC" && x.ParaValue == ParaValue);
            if (rec != null)
            {
                return Json(new
                {
                    WarehouseCode = rec.LookUpValue,
                    WarehouseName = rec.LookUpValueName
                });
            }
            else
            {
                return Json(new
                {
                    WarehouseCode = "",
                    WarehouseName = ""
                });
            }
        }   

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

        public JsonResult Select4LookUp()
        {
            var oTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
            var data = oTrPurchaseBPUBLL.Select4LookUp();
            oTrPurchaseBPUBLL = null;

            return Json(data.toKG());
        }

        public JsonResult GetBPU(string pONo, string bPUNo)
        {
            try
            {
                var oTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                var record = oTrPurchaseBPUBLL.GetRecordView(pONo, bPUNo);
                oTrPurchaseBPUBLL = null;

                var oTrPurchaseBPUDetailBLL = new OmTrPurchaseBPUDetailBLL(ctx, CurrentUser.UserId);
                var records = oTrPurchaseBPUDetailBLL.Select4Table(bPUNo);
                oTrPurchaseBPUDetailBLL = null;

                return Json(new { success = true, data = record, dataDtl = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GetRecord(string PONo, string DONo)
        {
            try
            {
                var oTrPurchasePBUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                var recordPBU = oTrPurchasePBUBLL.select4BPU(PONo, DONo);
                string bpuNo = string.Empty;
                if (recordPBU != null)
                {
                    bpuNo = recordPBU.FirstOrDefault().BPUNo;
                }

                var record = oTrPurchasePBUBLL.GetRecordView(PONo, bpuNo);
                recordPBU = null;
                oTrPurchasePBUBLL = null;

                var oTrPurchaseBPUDetailBLL = new OmTrPurchaseBPUDetailBLL(ctx, CurrentUser.UserId);
                var records = oTrPurchaseBPUDetailBLL.Select4Table(bpuNo);
                oTrPurchaseBPUDetailBLL = null;

                return Json(new { success = true, data = record, dataDtl = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GetBPUDetail(omTrPurchaseBPUDetail model, string PONo)
        {
            try
            {
                var oTrPurchaseBPUDetailBLL = new OmTrPurchaseBPUDetailBLL(ctx, CurrentUser.UserId);
                var record = oTrPurchaseBPUDetailBLL.GetRecordView(PONo, model.BPUNo, model.BPUSeq);
                oTrPurchaseBPUDetailBLL = null;

                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupDO()
        {
            try
            {
                var oTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                var records = oTrPurchaseBPUBLL.SelectReffNo();
                oTrPurchaseBPUBLL = null;

                return Json(records.AsQueryable().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookUpDO_DOSJorSJBOOKING(string tipe)
        {
            try
            {
                var isDOSJ = (tipe == "DO_SJ") ? true : false;
                var oTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);

                if (isDOSJ)
                {
                    var records = oTrPurchaseBPUBLL.SelectReffSJ(false);
                    oTrPurchaseBPUBLL = null;
                    
                    return Json(records.AsQueryable().toKG());
                }
                else
                {
                    var records = oTrPurchaseBPUBLL.SelectReffSJBooking();
                    oTrPurchaseBPUBLL = null;
                    
                    return Json(records.AsQueryable().toKG());
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupDO_SJandUpload()
        {
            try
            {
                var oTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                var records = oTrPurchaseBPUBLL.SelectReffSJ(true);
                oTrPurchaseBPUBLL = null;

                return Json(records.AsQueryable().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupSJ_True()
        {
            try
            {
                var oTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                var records = oTrPurchaseBPUBLL.SelectReffSJ(true);
                oTrPurchaseBPUBLL = null;

                return Json(records.AsQueryable().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupSJ_False()
        {
            try
            {
                var oTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                var records = oTrPurchaseBPUBLL.SelectReffSJ(false);
                oTrPurchaseBPUBLL = null;

                return Json(records.AsQueryable().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupSJ_BOOKING()
        {
            try
            {
                var oTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                var records = oTrPurchaseBPUBLL.SelectReffSJBooking();
                oTrPurchaseBPUBLL = null;

                return Json(records.AsQueryable().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupPONo()
        {
            try
            {
                var oTrPurchasePOBLL = new OmTrPurchasePOBLL(ctx, CurrentUser.UserId);
                var records = oTrPurchasePOBLL.Select4BPU("2").Select(
                    p => new
                    {
                        p.PONo,
                        p.SupplierCode,
                        p.SupplierName,
                        p.ShipTo
                    }
                );
                oTrPurchasePOBLL = null;

                return Json(records.AsQueryable().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupReffNo()
        {
            try
            {
                var oTrPurchasePOBLL = new OmTrPurchasePOBLL(ctx, CurrentUser.UserId);
                var records = oTrPurchasePOBLL.Select4BPU("2").Select(
                    p => new
                    {
                        ReffNo = p.RefferenceNo,
                        p.PONo,
                        p.SupplierCode,
                        p.SupplierName,
                        p.ShipTo
                    }
                );
                oTrPurchasePOBLL = null;

                return Json(records.AsQueryable().toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupExpedition()
        {
            try
            {
                var records = from a in ctx.Supplier
                              join b in ctx.SupplierProfitCenter
                              on new { a.CompanyCode, a.SupplierCode } equals new { b.CompanyCode, b.SupplierCode }
                              where a.CompanyCode == CompanyCode && b.BranchCode == BranchCode
                              && b.isBlackList == false && a.Status == "1"
                              && b.ProfitCenterCode == ProfitCenter
                              select new
                              {
                                  Expedition = a.SupplierCode,
                                  ExpeditionName = a.SupplierName,
                                  Alamat = a.Address1 + " " + a.Address2 + " " + a.Address3 + " " + a.Address4,
                                  Diskon = b.DiscPct,
                                  Status = (a.Status) == "0" ? "Tidak Aktif" : "Aktif",
                                  Profit = ctx.LookUpDtls.Where(p => p.CodeID == "PFCN" && p.LookUpValue == b.ProfitCenterCode).FirstOrDefault().LookUpValueName
                              };

                return Json(records.toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupWarehouse(string Tipe)
        {
            try
            {
                string sql = string.Format(@"
                    select a.CompanyCode,
                    a.CodeID,
                    a.LookUpValue,
                    a.SeqNo,
                    a.LookUpValueName,
                    a.CreatedBy,
                    a.CreatedDate,
                    a.LastUpdateBy,
                    a.LastUpdateDate
                    , b.ParaValue
                    from (
		                select * from gnMstLookUpDtl 
		                where CompanyCode = '{0}' and CodeID = '{1}' and ParaValue= '{2}'
	                ) a
                    left join (
		                select * from gnMstLookUpDtl 
		                where CompanyCode = '{3}' and CodeID = 'DWHC'
	                ) b on a.CompanyCode=b.CompanyCode and a.LookUpValue=b.LookUpValue
                    where isnull(b.ParaValue,'')= (case when isnull(b.ParaValue,'')='' or '{4}'='' then isnull(b.ParaValue,'') else '{5}' end)
                    order by SeqNo
                    ", CompanyCode, GnMstLookUpHdr.MappingWarehouse, BranchCode, CompanyCode, Tipe, Tipe);

                var records = new DataContext().Database.SqlQuery<LookUpDtl>(sql.Trim()).Select(
                    p => new
                    {
                        WarehouseCode = p.LookUpValue,
                        WarehouseName = p.LookUpValueName,
                        RefferenceDesc2 = ""
                    }).AsQueryable();

                return Json(records.toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupModel(string PONo)
        {
            try
            {
                var records = (from dtl in ctx.OmTrPurchasePOModels
                               join hdr in ctx.OmTrPurchasePOs on new { dtl.CompanyCode, dtl.BranchCode, dtl.PONo }
                                    equals new { hdr.CompanyCode, hdr.BranchCode, hdr.PONo } into _hdr
                               from hdr in _hdr.DefaultIfEmpty()
                               join mdl in ctx.MstModels on new { dtl.CompanyCode, dtl.SalesModelCode, Status = "1" }
                                    equals new { mdl.CompanyCode, mdl.SalesModelCode, mdl.Status } into _mdl
                               from mdl in _mdl.DefaultIfEmpty()
                               where dtl.CompanyCode == CompanyCode && dtl.BranchCode == BranchCode
                               && dtl.PONo == PONo && dtl.QuantityBPU < dtl.QuantityPO && hdr.Status == "2"
                               select new
                               {
                                   dtl.SalesModelCode,
                                   mdl.EngineCode
                               }).Distinct();

                return Json(records.toKG());

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupModelYear(string PONo, string SalesModelCode)
        {
            try
            {
                var records = (from dtl in ctx.OmTrPurchasePOModels
                               join hdr in ctx.OmTrPurchasePOs on new { dtl.CompanyCode, dtl.BranchCode, dtl.PONo }
                                   equals new { hdr.CompanyCode, hdr.BranchCode, hdr.PONo } into _hdr
                               from hdr in _hdr.DefaultIfEmpty()
                               join mdl in ctx.MstModelYear on new { dtl.CompanyCode, dtl.SalesModelCode, Status = "1" }
                                   equals new { mdl.CompanyCode, mdl.SalesModelCode, mdl.Status } into _mdl
                               from mdl in _mdl.DefaultIfEmpty()
                               where dtl.CompanyCode == CompanyCode && dtl.BranchCode == BranchCode
                                   && dtl.PONo == PONo && dtl.SalesModelCode == SalesModelCode
                                   && dtl.QuantityBPU < dtl.QuantityPO && hdr.Status == "2"
                                   && mdl.SalesModelYear >= dtl.SalesModelYear - 1 && mdl.SalesModelYear <= dtl.SalesModelYear + 1
                                   && mdl.SalesModelYear == ((ProductType == "2W") ? mdl.SalesModelYear : dtl.SalesModelYear)
                               select new
                               {
                                   mdl.SalesModelYear,
                                   mdl.SalesModelDesc,
                                   mdl.ChassisCode
                               }).ToList().AsQueryable();

                return Json(records.toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LookupColour(string SalesModelCode)
        {
            try
            {
                var isMaster = false;
                var records = from a in ctx.OmMstModelColours
                              where a.CompanyCode == CompanyCode && a.SalesModelCode == SalesModelCode
                              select new
                              {
                                  a.ColourCode,
                                  ColourName = (from b in ctx.MstRefferences
                                                where
                                                b.RefferenceCode == a.ColourCode
                                                && b.CompanyCode == CompanyCode
                                                && b.RefferenceType == OmMstRefferenceConstant.CLCD
                                                select b.RefferenceDesc1).FirstOrDefault(),
                                  a.Status
                              };

                if (!isMaster)
                {
                    records = from c in records
                              where c.Status == "1"
                              select c;
                }

                return Json(records.toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult SupplierName(string PONo)
        {
            try
            {
                var supplierName = string.Empty;
                var oTrPurchasePOBLL = new OmTrPurchasePOBLL(ctx, CurrentUser.UserId);
                var record = oTrPurchasePOBLL.Select4BPU("2").Where(p => p.PONo == PONo).FirstOrDefault();
                oTrPurchasePOBLL = null;
                if (record != null)
                {
                    supplierName = record.SupplierName;
                }

                return Json(new { success = true, SupplierName = supplierName});
            }
            catch (Exception ex)
            {
                return JsonException(ex.Message);
            }
        }

        public JsonResult Save(OmTrPurchaseBPULookupView model, string optionsTrans,
            bool dtpBPUEnable, bool dtpBPUSJEnable, bool chkReffDODate, bool chkReffSJDate)
        {
            try
            {
                model.Remark = (model.Remark != null) ? model.Remark.Trim() : "";
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    string BPU_FORMAT = string.IsNullOrEmpty(model.BPUNo) ? model.BPUNo = "BPU/YY/XXXXXX" : "BPU/YY/XXXXXX";
                    OmTrPurchaseBPUBLL oTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                    string errMsg = string.Empty;
                    var BPUDate = (DateTime)model.BPUDate;
                    var BPUSJDate = model.BPUSJDate != null ? (DateTime)model.BPUSJDate : DateTime.Now;

                    if (dtpBPUEnable && !DateTransValidation(BPUDate, ref errMsg))
                    {
                        return ThrowException(errMsg + " (Tgl BPU)");
                    }

                    if (model.Tipe == "SJ" && dtpBPUSJEnable && !DateTransValidation(BPUSJDate, ref errMsg))
                    {
                        return ThrowException(errMsg + " (Tgl BPU SJ)");
                    }

                    if (optionsTrans == "Upload")
                    {
                        //**** CHECK UPLOAD
                        if (model.Tipe == "DO")
                        {
                            if (string.IsNullOrEmpty(model.RefferenceDONo))
                            {
                                return ThrowException("Isikan dahulu No Refference DO !");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(model.RefferenceDONo) && string.IsNullOrEmpty(model.RefferenceSJNo))
                            {
                                return ThrowException("Isikan dahulu No Refference DO dan No Refference SJ !");
                            }
                        }

                        if (model.Tipe == "DO")
                        {
                            if (model.BPUNo.Equals(BPU_FORMAT))
                            {
                                if (oTrPurchaseBPUBLL.IsExistDONo(model.RefferenceDONo.Trim()))
                                {
                                    return ThrowException("No. Ref. DO sudah ada di BPU");
                                }
                            }
                        }
                        else
                        {
                            if (model.BPUNo.Equals(BPU_FORMAT))
                            {
                                if (oTrPurchaseBPUBLL.IsExistSJNo(model.RefferenceSJNo.Trim()))
                                {
                                    return ThrowException("No. Ref. SJ sudah ada di BPU");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (model.Tipe == "DO")
                        {
                            if (string.IsNullOrEmpty(model.RefferenceDONo))
                            {
                                return ThrowException("No.Reff.DO harus diisi");
                            }
                            if (chkReffDODate == false)
                            {
                                return ThrowException("Tanggal Reff.DO harus diisi");
                            }
                            if (model.BPUNo.Equals(BPU_FORMAT))
                            {
                                if (oTrPurchaseBPUBLL.IsExistDONo(model.RefferenceDONo.Trim()))
                                {
                                    return ThrowException("No. Ref. DO sudah ada di BPU");
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(model.RefferenceDONo))
                            {
                                return ThrowException("No.Reff.DO harus diisi");
                            }
                            if (chkReffDODate == false)
                            {
                                return ThrowException("Tanggal Reff.DO harus diisi");
                            }
                            if (string.IsNullOrEmpty(model.RefferenceSJNo))
                            {
                                return ThrowException("No.Reff.SJ harus diisi");
                            }
                            if (chkReffSJDate == false)
                            {
                                return ThrowException("Tanggal Reff.SJ harus diisi");
                            }
                            if (model.BPUNo.Equals(BPU_FORMAT))
                            {
                                if (oTrPurchaseBPUBLL.IsExistSJNo(model.RefferenceSJNo.Trim()))
                                {
                                    return ThrowException("No. Ref. SJ sudah ada di BPU");
                                }
                            }
                        }
                    }

                    bool isNew = false;
                    var record = oTrPurchaseBPUBLL.GetRecord(model.PONo, model.BPUNo);
                    if (record == null)
                    {
                        isNew = true;
                        record = new omTrPurchaseBPU();
                        record.CompanyCode = CompanyCode;
                        record.BranchCode = BranchCode;
                        record.PONo = model.PONo;
                        record.Status = ((int)OmTrPurchaseBPUBLL.status.OPEN).ToString();
                        record.CreatedBy = CurrentUser.UserId;
                        record.CreatedDate = DateTime.Now;
                    }
                    if (model.Tipe == "DO") record.BPUType = ((int)OmTrPurchaseBPUBLL.bpuTipe.DO).ToString();
                    else if (model.Tipe == "SJ")
                    {
                        record.BPUType = ((int)OmTrPurchaseBPUBLL.bpuTipe.SJ).ToString(); record.Status = ((int)OmTrPurchaseBPUBLL.status.OPEN).ToString();
                        record.BPUSJDate = model.BPUSJDate;
                    }
                    else if (model.Tipe == "DO_SJ") record.BPUType = ((int)OmTrPurchaseBPUBLL.bpuTipe.DO_SJ).ToString();
                    else if (model.Tipe == "SJ_BOOKING") record.BPUType = ((int)OmTrPurchaseBPUBLL.bpuTipe.SJ_BOOKING).ToString();

                    record.BPUDate = model.BPUDate;
                    record.SupplierCode = model.SupplierCode;
                    record.ShipTo = model.ShipTo;
                    record.RefferenceDONo = model.RefferenceDONo;
                    if (chkReffDODate)
                        record.RefferenceDODate = model.RefferenceDODate;
                    else record.RefferenceDODate = DateTime.Parse("1900/01/01");
                    record.RefferenceSJNo = model.RefferenceSJNo;
                    if (chkReffSJDate)
                        record.RefferenceSJDate = model.RefferenceSJDate;
                    else record.RefferenceSJDate = DateTime.Parse("1900/01/01");
                    record.WarehouseCode = model.WarehouseCode;
                    record.Expedition = model.Expedition;
                    record.Remark = model.Remark;

                    bool result = false;
                    var supplier = ctx.Supplier.Find(CompanyCode, model.DealerCode);
                    if (optionsTrans == "Upload" && model.Tipe == "DO" && isNew)
                    {
                        string standardCode = "";
                        if (supplier != null)
                            standardCode = supplier.StandardCode.ToString();
                        supplier = null;
                        var DORDHdr = ctx.OmUtlSDORDHdrs.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                            && p.DealerCode == standardCode && p.RcvDealerCode == CompanyCode && p.BatchNo == model.BatchNo
                            ).FirstOrDefault();
                        if (DORDHdr == null)
                        { return ThrowException("Data upload tidak ditemukan"); }
                        else
                        {
                            DORDHdr = null;
                            result = oTrPurchaseBPUBLL.SaveUpload(record, model.BatchNo);
                        }
                    }
                    else if (optionsTrans == "Upload" && model.Tipe == "SJ")
                    {
                        string standardCode = ""; string errMsg1 = "";
                        if (supplier != null)
                            standardCode = supplier.StandardCode;
                        OmUtlSSJALHdr SSJALHdr = ctx.OmUtlSSJALHdrs.Find(CompanyCode, BranchCode, standardCode, CompanyCode, model.BatchNo);
                        if (SSJALHdr == null)
                        { return ThrowException("Data upload tidak ditemukan"); }
                        else
                        {
                            SSJALHdr = null;
                            result = oTrPurchaseBPUBLL.SaveSJUpload(record, model.BatchNo, ref errMsg1);
                        }
                        if (errMsg1 != string.Empty)
                        {
                            return ThrowException(errMsg1);
                        }
                    }
                    else if ((optionsTrans == "Upload" && model.Tipe == "DO_SJ" && isNew) ||
                        (optionsTrans == "Upload" && model.Tipe == "SJ_BOOKING" && isNew))
                    {
                        string standardCode = "";
                        if (supplier != null)
                            standardCode = supplier.StandardCode;
                        OmUtlSSJALHdr SSJALHdr = ctx.OmUtlSSJALHdrs.Find(CompanyCode, BranchCode, standardCode, CompanyCode, model.BatchNo);
                        if (SSJALHdr == null)
                        { return ThrowException("Data upload tidak ditemukan"); }
                        else
                        {
                            SSJALHdr = null;
                            result = oTrPurchaseBPUBLL.SaveDOSJUpload(record, model.BatchNo, (model.Tipe == "SJ_BOOKING") ? true : false);
                        }
                    }
                    else
                        result = oTrPurchaseBPUBLL.Save(record, isNew);

                    if (result == true)
                    {
                        //if (gvDetail.RowCount > 0)
                        //{
                        //    EnableDetail(false);
                        //    ResetDetail();
                        //}
                        //else
                        //{
                        //    SetToolBar(XToolBar.StdNew);
                        //    EnableDetail(true);
                        //    txtSalesModelCode.Focus(); txtSalesModelCode.SelectAll();
                        //}
                        //PopulateRecord();
                        p_UpdateStatus(oTrPurchaseBPUBLL, model.PONo, model.BPUNo);
                    }

                    tranScope.Complete();
                    oTrPurchaseBPUBLL = null;
                    
                    return Json(new { success = true, message = "Simpan BPU berhasil", BPUNo = record.BPUNo, BPUStatus = record.Status });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Simpan BPU tidak berhasil. Periksa kembali inputan anda", error_log = ex.Message });
            }
        }

        public JsonResult Delete(string PONo, string BPUNo)
        {
            string msg = "Hapus data BPU tidak berhasil.";
            bool deleted = false;
            try
            {
                using (var transScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var omTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                    var record = omTrPurchaseBPUBLL.GetRecord(PONo, BPUNo);
                    if (record != null)
                    {
                        int result = omTrPurchaseBPUBLL.DeleteRecord(PONo, BPUNo);
                        if (result == 1)
                        {
                            transScope.Complete();
                            msg = "Berhasil hapus data BPU.";
                            deleted = true;
                        }
                    }

                    return Json(new { success = deleted, message = msg });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = msg, error_log = ex.Message });
            }
        }

        public JsonResult SaveDetailValidate(OmTrPurchaseBPULookupView model, OmTrPurchaseBPUDetailView modelDtl)
        {
            var dat = Json(new { });
            try
            {
                var omTrPurchaseBPUDetailBLL = new OmTrPurchaseBPUDetailBLL(ctx, CurrentUser.UserId);
                var oPOModelColour = ctx.OmTrPurchasePOModelColours.Find(CompanyCode, BranchCode, model.PONo,
                    modelDtl.SalesModelCode, modelDtl.SalesModelYear, modelDtl.ColourCode);
                var productType = this.ProductType;
                
                if (model.Tipe != "DO")
                {
                    var mstVehicle = ctx.OmMstVehicles.Find(CompanyCode, modelDtl.ChassisCode, Convert.ToDecimal(modelDtl.ChassisNo));
                    var checkChassisNo = omTrPurchaseBPUDetailBLL.CheckChassisNo(modelDtl.ChassisCode, Convert.ToDecimal(modelDtl.ChassisNo));
                    var checkEngineNo = omTrPurchaseBPUDetailBLL.CheckEngineNo(modelDtl.EngineCode, Convert.ToDecimal(modelDtl.EngineNo));
                    var omUTLSSJALDtl3 = omTrPurchaseBPUDetailBLL.SelectFromUtl(model.RefferenceSJNo, modelDtl.ChassisCode,
                        decimal.Parse(modelDtl.ChassisNo)).FirstOrDefault();

                    //var POModelColour = oPOModelColour;
                    //var ProductType = productType;
                    //var VehicleStatus = (mstVehicle != null) ? mstVehicle.Status : "X";
                    //var isExistsChassisNo = checkChassisNo;
                    //var isExistsEngineNo = checkEngineNo;
                    //var EngineNo = (omUTLSSJALDtl3 != null) ? omUTLSSJALDtl3.EngineNo.ToString() : "";

                    dat = Json(new
                    {
                        success = true,
                        data = new
                        {
                            POModelColour = oPOModelColour,
                            ProductType = productType,
                            VehicleStatus = (mstVehicle != null) ? mstVehicle.Status : "X",
                            isExistsChassisNo = checkChassisNo,
                            isExistsEngineNo = checkEngineNo,
                            EngineNo = (omUTLSSJALDtl3 != null) ? omUTLSSJALDtl3.EngineNo.ToString() : ""
                        }
                    });
                }

                if (model.Tipe == "DO")
                {
                    var datDO = omTrPurchaseBPUDetailBLL.Select4Table(model.BPUNo);
                    var oStandardCode = ctx.Supplier.Find(CompanyCode, model.SupplierCode);
                    dat = Json(new
                    {
                        success = true,
                        data = new
                        {
                            POModelColour = oPOModelColour,
                            ProductType = productType,
                            StandardCode = (oStandardCode != null) ? oStandardCode.StandardCode : null,
                            DOCount = datDO.Count()
                        }
                    });
                }
                omTrPurchaseBPUDetailBLL = null;

                return dat;
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Simpan BPU Detail tidak berhasil. Periksa kembali inputan anda!", error_log = ex.Message });
            }
        }

        public JsonResult SaveDetail(OmTrPurchaseBPULookupView model, OmTrPurchaseBPUDetailView modelDtl)
        {
            try
            {
                var omTrPurchaseBPUDetailBLL = new OmTrPurchaseBPUDetailBLL(ctx, CurrentUser.UserId);
                bool bolSave = false;
                if (modelDtl.BPUSeq == null) modelDtl.BPUSeq = 0;
                var recordDetail = omTrPurchaseBPUDetailBLL.GetRecord(model.PONo, model.BPUNo, modelDtl.BPUSeq);
                using (var transScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    bool isNew = false;
                    if (recordDetail == null)
                    {
                        isNew = true;
                        recordDetail = new omTrPurchaseBPUDetail();
                        recordDetail.CompanyCode = CompanyCode;
                        recordDetail.BranchCode = BranchCode;
                        recordDetail.BPUNo = model.BPUNo;
                        recordDetail.PONo = model.PONo;
                        recordDetail.BPUSeq = omTrPurchaseBPUDetailBLL.GetNextSeqNo(model.BPUNo);
                        recordDetail.StatusHPP = "0";
                        recordDetail.isReturn = false;
                        recordDetail.CreatedBy = CurrentUser.UserId;
                        recordDetail.CreatedDate = DateTime.Now;
                    }
                    recordDetail.SalesModelCode = modelDtl.SalesModelCode;
                    recordDetail.SalesModelYear = modelDtl.SalesModelYear;
                    recordDetail.ColourCode = modelDtl.ColourCode;
                    recordDetail.ChassisCode = modelDtl.ChassisCode;
                    recordDetail.EngineCode = modelDtl.EngineCode;
                    recordDetail.Remark = modelDtl.Remark;
                    if (model.Tipe != "DO")
                    {
                        recordDetail.ChassisNo = Convert.ToDecimal(modelDtl.ChassisNo);
                        recordDetail.EngineNo = Convert.ToDecimal(modelDtl.EngineNo);
                        recordDetail.ServiceBookNo = modelDtl.ServiceBookNo;
                        recordDetail.KeyNo = modelDtl.KeyNo;
                    }
                    recordDetail.SJRelNo = "";
                    bool isDORelease = false;
                    if (model.Tipe == "DO")
                    {
                        recordDetail.StatusDORel = "1";
                        recordDetail.StatusSJRel = "0";
                    }
                    else if (model.Tipe != "SJ")
                    {
                        recordDetail.StatusDORel = "9";
                        if (recordDetail.DORelNo == string.Empty)
                            isDORelease = true;
                        recordDetail.StatusSJRel = "0";
                    }
                    else if (model.Tipe != "DO_SJ")
                    {
                        recordDetail.StatusDORel = "0";
                        recordDetail.StatusSJRel = "0";
                    }
                    else if (model.Tipe != "SJ_BOOKING")
                    {
                        recordDetail.StatusDORel = "0";
                        recordDetail.StatusSJRel = "1";
                    }
                    recordDetail.LastUpdateBy = CurrentUser.UserId;
                    recordDetail.LastUpdateDate = DateTime.Now;
                    bool result = omTrPurchaseBPUDetailBLL.Save(recordDetail, isNew, isDORelease);
                    if (result == true)
                    {
                        // Update Master Vehicle
                        try
                        {
                            OmMstVehicle vehicle = ctx.OmMstVehicles.Find(CompanyCode, modelDtl.ChassisCode
                                , Convert.ToDecimal(modelDtl.ChassisNo));
                            if (vehicle != null)
                            {
                                vehicle.SuzukiDONo = model.RefferenceDONo;
                                vehicle.SuzukiDODate = Convert.ToDateTime(model.RefferenceDODate);
                                vehicle.SuzukiSJNo = model.RefferenceSJNo;
                                vehicle.SuzukiSJDate = Convert.ToDateTime(model.RefferenceSJDate);

                                result = ctx.SaveChanges() >= 0;
                            }
                        }
                        catch
                        {
                            result = true;
                        }

                        var omTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                        if (model.Tipe == "SJ")
                        {
                            var record = omTrPurchaseBPUBLL.GetRecord(model.PONo, model.BPUNo);
                            if (record != null)
                            {
                                if (record.BPUType != ((int)OmTrPurchaseBPUBLL.bpuTipe.SJ).ToString())
                                {
                                    record.BPUType = ((int)OmTrPurchaseBPUBLL.bpuTipe.SJ).ToString();
                                    record.Status = ((int)OmTrPurchaseBPUBLL.status.OPEN).ToString();
                                    record.RefferenceSJNo = model.RefferenceSJNo;
                                    record.RefferenceSJDate = Convert.ToDateTime(model.RefferenceSJDate);
                                    record.LastUpdateBy = CurrentUser.UserId;
                                    record.LastUpdateDate = DateTime.Now;
                                    omTrPurchaseBPUBLL.Update();
                                }
                            }
                        }
                        else p_UpdateStatus(omTrPurchaseBPUBLL, model.PONo, model.BPUNo);
                        transScope.Complete();
                        bolSave = true;
                        omTrPurchaseBPUBLL = null;
                    }
                    var datDetail = omTrPurchaseBPUDetailBLL.Select4Table(model.BPUNo);
                    omTrPurchaseBPUDetailBLL = null;

                    return Json(new { success = bolSave, message = "Simpan BPU Detail berhasil", dataDtl = datDetail });
                }
            }
            catch (DbEntityValidationException ex)
            {
                return Json(new { success = false, message = "Simpan BPU Detail tidak berhasil. Periksa kembali inputan anda!", error_log = ex.Message });
            }
        }

        public JsonResult DeleteDetail(OmTrPurchaseBPUDetailView modelDtl, bool rbSJ)
        {
            try
            {
                var omTrPurchaseBPUDetailBLL = new OmTrPurchaseBPUDetailBLL(ctx, CurrentUser.UserId);
                var recordDetail = omTrPurchaseBPUDetailBLL.GetRecord(modelDtl.PONo, modelDtl.BPUNo, modelDtl.BPUSeq);
                using (var transScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    int print = 0;
                    bool deleted = false;
                    string msg = string.Empty;
                    if (recordDetail != null)
                    {
                        int result = omTrPurchaseBPUDetailBLL.DeleteRecord(modelDtl.PONo, modelDtl.BPUNo, 
                            modelDtl.BPUSeq, modelDtl.SalesModelCode, (decimal)modelDtl.SalesModelYear);
                        if (result == 1)
                        {
                            print = ChangeStatusPrint(modelDtl.PONo, modelDtl.BPUNo);
                            if (!rbSJ){
                                transScope.Complete();
                                msg = "Hapus BPU Detail berhasil.";
                                deleted = true;
                            }
                        }
                        else
                        {
                            msg = "Hapus BPU Detail tidak berhasil.";
                        }
                    }               
                    omTrPurchaseBPUDetailBLL = null;

                    return Json(new { success = deleted, message = msg });
                }
            }
            catch (DbEntityValidationException ex)
            {
                return Json(new { success = false, message = "Hapus BPU Detail tidak berhasil.", error_log = ex.Message });
            }
        }

        public JsonResult Print(string PONo, string BPUNo)
        {
            try
            {
                string msg = "";
                bool printed = false;
                var omTrPurchaseBPUDetailBLL = new OmTrPurchaseBPUDetailBLL(ctx, CurrentUser.UserId);
                var dtlCount = omTrPurchaseBPUDetailBLL.GetRecordCount(PONo, BPUNo);
                if (dtlCount > 0)
                {
                    var omTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                    var record = omTrPurchaseBPUBLL.GetRecord(PONo, BPUNo);
                    if (record == null) return ThrowException("Data BPU tidak ditemukan.");
                    if (record.Status == ((int)OmTrPurchaseBPUBLL.status.OPEN).ToString() || record.Status.Trim() == "")
                    {
                        record.Status = ((int)OmTrPurchaseBPUBLL.status.PRINTED).ToString();
                    }
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
                    if (omTrPurchaseBPUBLL.Update() >= 0)
                    {
                        printed = true;
                    }
                    omTrPurchaseBPUBLL = null;
                }
                else
                {
                    msg = "Dokumen tidak dapat dicetak karena tidak memiliki data detail";
                }
                omTrPurchaseBPUDetailBLL = null;
                
                return Json(new { success = printed, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Dokumen tidak dapat dicetak", error_log = ex.Message });
            }
        }

        public JsonResult ApproveRecord(string PONo, string BPUNo, bool rbDO, bool rbSJ, bool isBuyBack, bool isConfirmBuyBack)
        {
            string msg = "Proses Approved BPU tidak berhasil.";
            bool result = false;
            try
            {
                using (var transScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var status = string.Empty;
                    var omTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
                    var omTrPurchaseBPUDetailBLL = new OmTrPurchaseBPUDetailBLL(ctx, CurrentUser.UserId);
                    var dtlCount = omTrPurchaseBPUDetailBLL.GetRecordCount(PONo, BPUNo);
                    if (dtlCount == 0)
                    {
                        return ThrowException("BPU belum lengkap, detil tidak ada");
                    }
                    if (rbDO == false)
                    {
                        var recDetails = ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode == CompanyCode
                            && p.BranchCode == BranchCode && p.PONo == PONo && p.BPUNo == BPUNo
                            && (((p.ChassisNo == null) ? 0 : p.ChassisNo) == 0 || ((p.EngineNo == null) ? 0 : p.EngineNo) == 0
                            || p.ServiceBookNo == "") && p.isReturn == false); 
                        
                        var listDetails = recDetails.ToList();
                        if (listDetails.Count > 0)
                        {
                            return ThrowException("BPU belum lengkap, detil tidak lengkap");
                        }
                        else
                        {
                            if (isConfirmBuyBack == false)
                            {
                                if (rbSJ)
                                {
                                    foreach (omTrPurchaseBPUDetail recDetail in listDetails)
                                    {
                                        OmMstVehicle oOmMstVehicle = ctx.OmMstVehicles.Find(CompanyCode, recDetail.ChassisCode, (decimal)recDetail.ChassisNo);
                                        if (oOmMstVehicle != null)
                                        {
                                            var recVehicles = omTrPurchaseBPUDetailBLL.CheckVehicleBuyBack(BPUNo);
                                            if (recVehicles.Count > 0)
                                            {
                                                msg = "Ada kendaraan yang pernah dibeli dan dijual:";
                                                foreach (omTrPurchaseBPUDetail recVehicle in recVehicles)
                                                    msg += "\n- ChassisCode: " + recVehicle.ChassisCode + " ChassisNo: " + recVehicle.ChassisNo;
                                                msg += "\nApakah anda ingin membeli kembali?";
                                                
                                                return Json(new { isConfirmBuyBack = true, message = msg });
                                                //if (XMessageBox.ShowConfirmation(msg) == DialogResult.OK)
                                                //    isBuyBack = true;
                                                //else
                                                //{
                                                //    isBuyBack = false;
                                                //    return;
                                                //}
                                            }
                                            else
                                            {
                                                return ThrowException("Kendaraan belum pernah di purchase return dan dijual");
                                            }
                                        }
                                        oOmMstVehicle = null;
                                    }
                                }
                            }
                            recDetails = null;

                            var record = omTrPurchaseBPUBLL.GetRecord(PONo, BPUNo);
                            if (record != null)
                            {
                                record.Status = ((int)OmTrPurchaseBPUBLL.status.CLOSED).ToString();
                                record.LastUpdateBy = CurrentUser.UserId;
                                record.LastUpdateDate = DateTime.Now;
                                //string bpuType = "";
                                //if (rbSJ)
                                //{
                                //    bpuType = "1";
                                //    result = true;
                                //    result = isHPPAlreadyApprove();
                                //}
                                //else 
                                result = true;
                                if (result == true)
                                {
                                    result = omTrPurchaseBPUDetailBLL.CreateVehicle(PONo, BPUNo, isBuyBack);
                                    if (result == true)
                                    {
                                        result = omTrPurchaseBPUBLL.Update() >= 0;
                                        status = ((int)OmTrPurchaseBPUBLL.status.CLOSED).ToString();
                                        if (rbSJ)
                                            UpdateHPPSubDetail(omTrPurchaseBPUDetailBLL, BPUNo);
                                        //result = 
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var record = omTrPurchaseBPUBLL.GetRecord(PONo, BPUNo);
                        if (record != null)
                        {
                            status = ((int)OmTrPurchaseBPUBLL.status.CLOSED).ToString();
                            record.Status = status;
                            record.LastUpdateBy = CurrentUser.UserId;
                            record.LastUpdateDate = DateTime.Now;
                            result = omTrPurchaseBPUBLL.Update() >= 0;
                            record = null;
                            if (result == true)
                            {
                                result = omTrPurchaseBPUDetailBLL.CreateVehicleTemp(PONo, BPUNo);
                            }
                        }
                    }
                    if (result == true)
                    {
                        transScope.Complete();
                        msg = "Proses Approved BPU berhasil";
                    }
                    omTrPurchaseBPUBLL = null;
                    omTrPurchaseBPUDetailBLL = null;

                    return Json(new { success = result, message = msg, BPUStatus = status });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = msg, error_log = ex.Message });
            }
        }

        #region -- Private Method --
        private void p_UpdateStatus(OmTrPurchaseBPUBLL omTrPurchaseBPUBLL, string PONo, string BPUNo)
        {
            var record = omTrPurchaseBPUBLL.GetRecord(PONo, BPUNo);
            if (record != null)
            {
                if (record.Status == ((int)OmTrPurchaseBPUBLL.status.PRINTED).ToString())
                {
                    record.Status = ((int)OmTrPurchaseBPUBLL.status.OPEN).ToString();
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
                    omTrPurchaseBPUBLL.Update();
                }
            }
        }

        private int ChangeStatusPrint(string PONo, string BPUNo)
        {
            var omTrPurchaseBPUBLL = new OmTrPurchaseBPUBLL(ctx, CurrentUser.UserId);
            var record = omTrPurchaseBPUBLL.GetRecord(PONo, BPUNo);
            record.Status = "0";
            var result = omTrPurchaseBPUBLL.Update();
            omTrPurchaseBPUBLL = null;

            return result;
        }

        private bool isHPPAlreadyApprove(string BPUNo)
        {
            List<String> msg;
            msg = new List<string>();
            var recHPPs = (from dtl in ctx.omTrPurchaseHPPSubDetail
                           join hdr in ctx.omTrPurchaseHPP on new { dtl.CompanyCode, dtl.BranchCode, dtl.HPPNo }
                           equals new { hdr.CompanyCode, hdr.BranchCode, hdr.HPPNo }
                           where dtl.BPUNo == BPUNo && dtl.isReturn == false
                           select new
                           {
                               dtl.HPPNo,
                               hdr.Status
                           }).ToList();

            if (recHPPs != null || recHPPs.Count > 0)
            {
                foreach (var recHPP in recHPPs)
                {
                    if (recHPP.Status != "2" && recHPP.Status != "5")
                    {
                        msg.Add(recHPP.HPPNo);
                    }
                }
            }
            
            if (msg.Count > 0)
            {
                string message = "";
                for (int i = 0; i < msg.Count; i++)
                {
                    message += msg[i] + "\r\n";
                }
                throw new Exception("Transaksi HPP belum di approve untuk No. \r\n" + message);
            }
            else return true;
        }

        private bool UpdateHPPSubDetail(OmTrPurchaseBPUDetailBLL omTrPurchaseBPUDetailBLL, string BPUNo)
        {
            bool result = omTrPurchaseBPUDetailBLL.UpdateFromSJ(BPUNo);
            return result;
        }
        #endregion
    }
}
