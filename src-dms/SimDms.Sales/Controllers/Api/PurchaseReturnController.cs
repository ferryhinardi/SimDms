using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.Models;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Data;

namespace SimDms.Sales.Controllers.Api
{
    public class PurchaseReturnController : BaseController
    {
        private const string RTP = "RTP"; // Purchase Order (Order Management/Unit)

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult DetailBPU(string ReturnNo)
        {
            var gridDetail = ctx.omTrPurchaseReturnDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ReturnNo == ReturnNo);
            return Json(new { success = true, grid = gridDetail });
        }

        public JsonResult DetailModel(string ReturnNo, string BPUNo)
        {
            var gridDetail = ctx.omTrPurchaseReturnSubDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ReturnNo == ReturnNo && a.BPUNo == BPUNo);
            return Json(new { success = true, grid = gridDetail });
        }

        public JsonResult Approve(omTrPurchaseReturn model)
        {
            using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var sqlstr = ctx.Database.ExecuteSqlCommand("uspfn_OmApprovePurchaseReturn '" + CompanyCode + "','" + BranchCode + "','" + model.ReturnNo + "','" + CurrentUser.UserId + "'");
                    tran.Commit();

                    return Json(new { success = true, result = sqlstr, Status = "Approved" });
                }
                catch(Exception ex){
                    tran.Rollback();
                    return Json(new { success = false, message = "Proses Approve gagal. Exception message: " + ex.Message });
                }
            }
        }

        public JsonResult prePrint(omTrPurchaseReturn model)
        {
            var msg = "";
            var Hdr = ctx.omTrPurchaseReturn.Find(CompanyCode, BranchCode, model.ReturnNo);
            var Dtl1 = ctx.omTrPurchaseReturnDetail.Where(a=>a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ReturnNo == model.ReturnNo);
            var Dtl2 = ctx.omTrPurchaseReturnSubDetail.Where(a=>a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ReturnNo == model.ReturnNo);
            if (Dtl1.Count() < 1  || Dtl2.Count() < 1)
            {
                msg = "Dokumen tidak dapat dicetak karena tidak memiliki data detail";
                return Json(new { success = false, message = msg });
            }

            if (Hdr.Status == "0" || Hdr.Status.Trim() == "")
            {
                Hdr.Status = "1";
            }

            Hdr.LastUpdateBy = CurrentUser.UserId;
            Hdr.LastUpdateDate = DateTime.Now;

            var result = ctx.SaveChanges() >= 0 ? true : false;

            return Json(new { success = result, Status = "Printed", stat = Hdr.Status });
        }

        public JsonResult Save(omTrPurchaseReturn model)
        {
            using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    string errMsg = string.Empty;
                    if (!DateTransValidation(model.ReturnDate.Value.Date, ref errMsg))
                    {
                        throw new Exception(errMsg);
                    }

                    var record = ctx.omTrPurchaseReturn.Find(CompanyCode, BranchCode, model.ReturnNo);
                    if (record == null)
                    {
                        record = new omTrPurchaseReturn
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            ReturnNo = GetNewDocumentNo(RTP, model.ReturnDate.Value),
                            ReturnDate = model.ReturnDate,
                            RefferenceNo = model.RefferenceNo,
                            RefferenceDate = model.RefferenceDate,
                            HPPNo = model.HPPNo,
                            RefferenceFakturPajakNo = model.RefferenceFakturPajakNo,
                            Remark = model.Remark == null ? "" : model.Remark,
                            Status = "0",
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                            LastUpdateBy = CurrentUser.UserId,
                            LastUpdateDate = ctx.CurrentTime,
                            isLocked = false,
                            LockingBy = "",
                            LockingDate = Convert.ToDateTime("1900-01-01")
                        };

                        ctx.omTrPurchaseReturn.Add(record);
                    }

                    record.Remark = model.Remark == null ? "" : model.Remark;
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
                    record.isLocked = false;
                    record.LockingBy = "";
                    record.LockingDate = Convert.ToDateTime("1900-01-01");

                    Helpers.ReplaceNullable(record);
                    ctx.SaveChanges();

                    tran.Commit();                    
                    
                    return Json(new { success = true, message = errMsg, data = record });
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        public JsonResult SaveBPU(omTrPurchaseReturn model, omTrPurchaseReturnDetail bpuModel)
        {
            var pr = ctx.omTrPurchaseReturn.Find(CompanyCode, BranchCode, model.ReturnNo);
            if (pr != null)
            {
                if (pr.Status != "0" && pr.Status != "1")
                {
                    return Json(new { success = false, message = "Data sudah tidak bisa ditambah.", data = pr });
                }
            }

            string msg = "";
            var record = ctx.omTrPurchaseReturnDetail.Find(CompanyCode, BranchCode, model.ReturnNo, bpuModel.BPUNo);

            if (record == null)
            {
                record = new omTrPurchaseReturnDetail
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ReturnNo = model.ReturnNo,
                    BPUNo = bpuModel.BPUNo,
                    Remark = bpuModel.Remark == null ? "" : bpuModel.Remark,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                };

                ctx.omTrPurchaseReturnDetail.Add(record);
            }
            
            record.Remark = bpuModel.Remark == null ? "" : bpuModel.Remark;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            Helpers.ReplaceNullable(record);
            try
            {
                ctx.SaveChanges();
                var isChangeStatus = ChangeStatusPrint(model.ReturnNo);
                var records = ctx.omTrPurchaseReturnDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ReturnNo == model.ReturnNo);
                return Json(new { success = true, message = msg, data = record, result = records, IsChangeStatus = isChangeStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SaveModel(omTrPurchaseReturn model, omTrPurchaseReturnDetail bpuModel, omTrPurchaseReturnSubDetail detailModel)
        {
            try
            {
                var record = ctx.omTrPurchaseReturn.Find(CompanyCode, BranchCode, model.ReturnNo);
                if (record != null)
                {
                    if (record.Status != "0" && record.Status != "1")
                    {
                        return Json(new { success = false, message = "Tidak bisa tambah atau update data Sales Model." });
                    }
                }

                if (Select4LookupModelTextChange(model.HPPNo, bpuModel.BPUNo, detailModel.SalesModelCode) == 0)
                {
                    throw new Exception("Data Sales Model Code tidak ditemukan.");
                }
                if (Select4LookupModelYearTextChange(model.HPPNo, bpuModel.BPUNo, detailModel.SalesModelCode, detailModel.SalesModelYear.Value) == 0)
                {
                    throw new Exception("Data Sales Model Year tidak ditemukan.");
                }

                var reBPU = SelectDistinctBPUs(bpuModel.BPUNo).FirstOrDefault();
                using(var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        if (reBPU.BPUType == "0")
                        {
                            if (Select4LookupChassisBPUType0(OmMstRefferenceConstant.CLCD, model.HPPNo, bpuModel.BPUNo, detailModel.SalesModelCode, detailModel.SalesModelYear.Value) == 0)
                                throw new Exception("No. Rangka tidak ditemukan");

                            var recHPPSubDetBPUType0 = ctx.omTrPurchaseHPPSubDetail.Where(x =>
                                    x.CompanyCode == CompanyCode
                                    && x.BranchCode == BranchCode
                                    && x.HPPNo == model.HPPNo
                                    && x.BPUNo == bpuModel.BPUNo
                                    && x.SalesModelCode == detailModel.SalesModelCode
                                    && x.SalesModelYear == detailModel.SalesModelYear
                                ).ToList();

                            foreach (var rec in recHPPSubDetBPUType0)
                            {
                                bool isNewSubDet = false;
                                var recordSubDet = ctx.omTrPurchaseReturnSubDetail.Find(CompanyCode, BranchCode, model.ReturnNo, bpuModel.BPUNo, rec.HPPSeq);
                                if (recordSubDet == null)
                                {
                                    isNewSubDet = true;
                                    recordSubDet = new omTrPurchaseReturnSubDetail();
                                    recordSubDet.CompanyCode = CompanyCode;
                                    recordSubDet.BranchCode = BranchCode;
                                    recordSubDet.ReturnNo = model.ReturnNo;
                                    recordSubDet.BPUNo = bpuModel.BPUNo;
                                    recordSubDet.ReturnSeq = rec.HPPSeq;
                                    recordSubDet.CreatedBy = CurrentUser.UserId;
                                    recordSubDet.CreatedDate = DateTime.Now;
                                    ctx.omTrPurchaseReturnSubDetail.Add(recordSubDet);
                                }
                                recordSubDet.SalesModelCode = detailModel.SalesModelCode;
                                recordSubDet.SalesModelYear = detailModel.SalesModelYear ?? 0;
                                recordSubDet.ChassisNo = rec.ChassisNo;
                                recordSubDet.ChassisCode = rec.ChassisCode;
                                recordSubDet.EngineCode = rec.EngineCode;
                                recordSubDet.EngineNo = rec.EngineNo ?? 0;
                                recordSubDet.ColourCode = rec.ColourCode;
                                recordSubDet.Remark = detailModel.Remark;
                                recordSubDet.LastUpdateBy = CurrentUser.UserId;
                                recordSubDet.LastUpdateDate = DateTime.Now;

                                Helpers.ReplaceNullable(recordSubDet);

                                ctx.SaveChanges();

                                SaveModelNextProcess(recordSubDet, rec, model.ReturnNo, isNewSubDet);
                            }
                        }
                        else
                        {
                            if (!WarehouseIsHolding(detailModel.ChassisCode, Convert.ToInt32(detailModel.ChassisNo)))
                            {
                                throw new Exception("Kendaraan ini sudah berpindah gudang, silahkan transfer ke holding dahulu.");
                            }

                            var recHPPSubDetBPUTypeNot0 = ctx.omTrPurchaseHPPSubDetail.Where(x => 
                                    x.CompanyCode == CompanyCode
                                    && x.BranchCode == BranchCode
                                    && x.HPPNo == model.HPPNo
                                    && x.BPUNo == bpuModel.BPUNo
                                    && x.ChassisCode == detailModel.ChassisCode
                                    && x.ChassisNo == detailModel.ChassisNo.Value
                                ).ToList();
                        
                            foreach(var rec in recHPPSubDetBPUTypeNot0){
                                bool isNewSubDet = false;
                                var recordSubDet = ctx.omTrPurchaseReturnSubDetail.Find(CompanyCode, BranchCode, model.ReturnNo, bpuModel.BPUNo, rec.HPPSeq);
                                if (recordSubDet == null)
                                {
                                    isNewSubDet = true;
                                    recordSubDet = new omTrPurchaseReturnSubDetail();
                                    recordSubDet.CompanyCode = CompanyCode;
                                    recordSubDet.BranchCode = BranchCode;
                                    recordSubDet.ReturnNo = model.ReturnNo;
                                    recordSubDet.BPUNo = bpuModel.BPUNo;
                                    recordSubDet.ReturnSeq = rec.HPPSeq;
                                    recordSubDet.CreatedBy = CurrentUser.UserId;
                                    recordSubDet.CreatedDate = DateTime.Now;

                                    ctx.omTrPurchaseReturnSubDetail.Add(recordSubDet);
                                }
                                recordSubDet.SalesModelCode = detailModel.SalesModelCode;
                                recordSubDet.SalesModelYear = detailModel.SalesModelYear ?? 0;
                                recordSubDet.ChassisNo = detailModel.ChassisNo ?? 0;
                                recordSubDet.ChassisCode = rec.ChassisCode;
                                recordSubDet.EngineCode = rec.EngineCode;
                                recordSubDet.EngineNo = rec.EngineNo ?? 0;
                                recordSubDet.ColourCode = rec.ColourCode;
                                recordSubDet.Remark = detailModel.Remark;
                                recordSubDet.LastUpdateBy = CurrentUser.UserId;
                                recordSubDet.LastUpdateDate = DateTime.Now;

                                Helpers.ReplaceNullable(recordSubDet);

                                ctx.SaveChanges();

                                SaveModelNextProcess(recordSubDet, rec, model.ReturnNo, isNewSubDet);
                            }
                        }

                        tran.Commit();

                        var isChangeStatus = ChangeStatusPrint(model.ReturnNo);
                        var bpuType = "";
                        var recBPU = ctx.omTrPurchaseBPU.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.BPUNo == bpuModel.BPUNo);
                        if(recBPU.Count() > 0)
                            bpuType = recBPU.FirstOrDefault().BPUType;

                        var records = ctx.omTrPurchaseReturnSubDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
                            && a.ReturnNo == bpuModel.ReturnNo && a.BPUNo == bpuModel.BPUNo);
                        
                        return Json(new { success = true, message = "", data = records, BPUType = bpuType, IsChangeStatus = isChangeStatus });
                    }   
                    catch(Exception ex) {
                        tran.Rollback();
                        return Json(new {success = false, message = "Proses simpan data gagal. Exception message: " + ex.Message });
                    }   
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Proses simpan data gagal. Exception message: " + ex.Message });
            }
        }

        public JsonResult SaveModel_OLD(omTrPurchaseReturn model, omTrPurchaseReturnDetail bpuModel, omTrPurchaseReturnSubDetail detailModel)
        {
            string msg = "";
            var data = ctx.Database.SqlQuery<omTrPurchaseHPPSubDetail>("SELECT *" +
            "FROM OmTrPurchaseHPPSubDetail "+
            "WHERE CompanyCode = @p0 "+
            "AND BranchCode = @p1 "+
            "AND HPPNo = @p2 "+
            "AND BPUNo = @p3 "+
            "AND SalesModelCode = @p4 " +
            "AND SalesModelYear = @p5", CompanyCode, BranchCode, model.HPPNo, bpuModel.BPUNo, detailModel.SalesModelCode, detailModel.SalesModelYear).ToList();

            foreach (var row in data)
            {
                var record = ctx.omTrPurchaseReturnSubDetail.Find(CompanyCode, BranchCode, model.ReturnNo, bpuModel.BPUNo, detailModel.ReturnSeq);

                if (record == null)
                {
                    record = new omTrPurchaseReturnSubDetail
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        ReturnNo = model.ReturnNo,
                        BPUNo = bpuModel.BPUNo,
                        ReturnSeq = detailModel.ReturnSeq,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now,
                    };

                    ctx.omTrPurchaseReturnSubDetail.Add(record);
                }
                
                record.SalesModelCode = detailModel.SalesModelCode;
                record.SalesModelYear = detailModel.SalesModelYear;
                record.ChassisCode = row.ChassisCode;
                record.ChassisNo = row.ChassisNo;
                record.EngineCode = row.EngineCode;
                record.EngineNo = row.EngineNo;
                record.ColourCode = row.ColourCode;
                record.Remark = detailModel.Remark == null ? "" : detailModel.Remark;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctx.CurrentTime;     
                record.Remark = detailModel.Remark == null ? "" : detailModel.Remark;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;

                Helpers.ReplaceNullable(record);

                var HPPSubDetail = ctx.omTrPurchaseHPPSubDetail.Find(CompanyCode, BranchCode, model.HPPNo, bpuModel.BPUNo, row.HPPSeq);

                HPPSubDetail.isReturn = true;
                HPPSubDetail.LastUpdateBy = CurrentUser.UserId;
                HPPSubDetail.LastUpdateDate = DateTime.Now;

                var pPONo = ctx.omTrPurchaseBPU.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BPUNo == bpuModel.BPUNo)
                                .FirstOrDefault().PONo;

                var BPUDetail = ctx.omTrPurchaseBPUDetail.Find(CompanyCode, BranchCode, pPONo, record.BPUNo, row.HPPSeq);

                if (BPUDetail != null)
                {
                    BPUDetail.isReturn = true;
                    BPUDetail.LastUpdateBy = CurrentUser.UserId;
                    BPUDetail.LastUpdateDate = DateTime.Now;

                    var ReturnDetailModel = ctx.omTrPurchaseReturnDetailModel.Find(CompanyCode, BranchCode, model.ReturnNo, record.BPUNo, record.SalesModelCode, record.SalesModelYear);
                    var PurchaseHPPDetailModel = ctx.omTrPurchaseHPPDetailModel.Find(CompanyCode, BranchCode, row.HPPNo, record.BPUNo, record.SalesModelCode, record.SalesModelYear);

                    if (ReturnDetailModel == null)
                    {
                        ReturnDetailModel = new omTrPurchaseReturnDetailModel
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            ReturnNo = model.ReturnNo,
                            BPUNo = record.BPUNo,
                            SalesModelCode = record.SalesModelCode,
                            SalesModelYear =Convert.ToDecimal(record.SalesModelYear),
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                            //Quantity = ReturnDetailModel.Quantity + 1
                        };
                        ctx.omTrPurchaseReturnDetailModel.Add(ReturnDetailModel);
                    }

                    ReturnDetailModel.Quantity = row.ChassisCode != detailModel.ChassisCode ? 
                        (ReturnDetailModel.Quantity ?? 0) + 1 : 1;
                    
                    ReturnDetailModel.BeforeDiscDPP = PurchaseHPPDetailModel.BeforeDiscDPP;
                    ReturnDetailModel.DiscExcludePPn = PurchaseHPPDetailModel.DiscExcludePPn;
                    ReturnDetailModel.AfterDiscDPP = PurchaseHPPDetailModel.AfterDiscDPP;
                    ReturnDetailModel.AfterDiscPPn = PurchaseHPPDetailModel.AfterDiscPPn;
                    ReturnDetailModel.AfterDiscPPnBM = PurchaseHPPDetailModel.AfterDiscPPnBM;
                    ReturnDetailModel.AfterDiscTotal = PurchaseHPPDetailModel.AfterDiscTotal;
                    ReturnDetailModel.OthersDPP = PurchaseHPPDetailModel.OthersDPP;
                    ReturnDetailModel.OthersPPn = PurchaseHPPDetailModel.OthersPPn;
                    ReturnDetailModel.LastUpdateBy = CurrentUser.UserId;
                    ReturnDetailModel.LastUpdateDate = DateTime.Now;

                    Helpers.ReplaceNullable(ReturnDetailModel);
                }
             
            }

            using (var transScope = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ctx.SaveChanges();
                    transScope.Commit();

                    var records = ctx.omTrPurchaseReturnSubDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ReturnNo == bpuModel.ReturnNo && a.BPUNo == bpuModel.BPUNo);

                    return Json(new { success = true, message = msg, data = data.AsQueryable(), result = records });
                }
                catch (Exception ex)
                {
                    transScope.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        public JsonResult Delete(omTrPurchaseReturn model)
        {
            var recordPurRet = ctx.omTrPurchaseReturn.Find(CompanyCode, BranchCode, model.ReturnNo);
            if (recordPurRet != null)
            {
                if (recordPurRet.Status != "0" && recordPurRet.Status != "1")
                {
                    return Json(new { success = false, message = "Data sudah tidak bisa hapus." });
                }
            }

            var record = ctx.omTrPurchaseReturn.Find(CompanyCode, BranchCode, model.ReturnNo);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        DeleteReturn(model.ReturnNo, model.HPPNo, "", "", 0, "", 0, 1);
                        tran.Commit();

                        return Json(new { success = true, data = record, Status = "Canceled" });
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();

                        return Json(new { success = false, message = ex.Message });
                    }
                }

//                ctx.Database.ExecuteSqlCommand(@"UPDATE omTrPurchaseReturn SET Status = 3
//                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
//                                                "' and ReturnNo='" + model.ReturnNo + "'");
//                ctx.Database.ExecuteSqlCommand(@"DELETE omTrPurchaseReturnDetail 
//                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
//                                                "' and ReturnNo='" + model.ReturnNo + "'");
//                ctx.Database.ExecuteSqlCommand(@"DELETE omTrPurchaseReturnSubDetail 
//                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
//                                                "' and ReturnNo='" + model.ReturnNo + "'");
            }
        }

        public JsonResult DeleteBPU(omTrPurchaseReturn model, omTrPurchaseReturnDetail bpuModel, List<omTrPurchaseReturnSubDetail> listDetailModel)
        {
            var pr = ctx.omTrPurchaseReturn.Find(CompanyCode, BranchCode, model.ReturnNo);
            if (pr != null)
            {
                if (pr.Status != "0" && pr.Status != "1") {
                    return Json(new { success = false, message = "Data sudah tidak bisa dihapus.", data = pr });
                }
            }

            var record = ctx.omTrPurchaseReturnDetail.Find(CompanyCode, BranchCode, model.ReturnNo, bpuModel.BPUNo);
            if (record != null)
            {
                using (var tran = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        if (listDetailModel == null)
                        {
                            ctx.omTrPurchaseReturnDetail.Remove(record);
                        }
                        else
                        {
                            var pCompanyCode = new SqlParameter("@CompanyCode", SqlDbType.VarChar);
                            pCompanyCode.Value = CompanyCode;
                            var pBranchCode = new SqlParameter("@BranchCode", SqlDbType.VarChar);
                            pBranchCode.Value = BranchCode;
                            var pReturnNo = new SqlParameter("@ReturnNo", SqlDbType.VarChar);
                            pReturnNo.Value = model.ReturnNo;
                            var pHPPNo = new SqlParameter("@HPPNo", SqlDbType.VarChar);
                            pHPPNo.Value = model.HPPNo;
                            var pBPUNo = new SqlParameter("@BPUNo", SqlDbType.VarChar);
                            pBPUNo.Value = bpuModel.BPUNo;
                            var pSalesModelCode = new SqlParameter("@SalesModelCode", SqlDbType.VarChar);
                            pSalesModelCode.Value = "";
                            var pSalesModelYear = new SqlParameter("@SalesModelYear", SqlDbType.Int);
                            pSalesModelYear.Value = 0;
                            var pChassisCode = new SqlParameter("@ChassisCode", SqlDbType.VarChar);
                            pChassisCode.Value = "";
                            var pChassisNo = new SqlParameter("@ChassisNo", SqlDbType.Int);
                            pChassisNo.Value = 0;
                            var pUserID = new SqlParameter("@UserID", SqlDbType.VarChar);
                            pUserID.Value = CurrentUser.UserId;
                            var pDeleteMode = new SqlParameter("@DeleteMode", SqlDbType.Int);
                            pDeleteMode.Value = 2;

                            object[] parameters = { pCompanyCode, pBranchCode, pReturnNo, pHPPNo, pBPUNo, 
                                pSalesModelCode, pSalesModelYear, pChassisCode, pChassisNo, pUserID, pDeleteMode};
                            ctx.Database.ExecuteSqlCommand(string.Format(@"exec uspfn_OmDeletePurchaseReturn @CompanyCode, @BranchCode, @ReturnNo, @HPPNo,
                                @BPUNo, @SalesModelCode, @SalesModelYear, @ChassisCode, @ChassisNo, @UserID, @DeleteMode"), parameters);

                        }
                        
                        ctx.SaveChanges();
                        tran.Commit();

                        var records = ctx.omTrPurchaseReturnSubDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ReturnNo == bpuModel.ReturnNo && a.BPUNo == bpuModel.BPUNo);
                        return Json(new { success = true, data = record, result = records });

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();

                        return Json(new { success = false, message = ex.Message });
                    }
                }
            }
            else
            {
                return Json(new { success = false, message = "Data tidak ditemukan atau sudah dihapus." });
            }
        }

        public JsonResult DeleteModel(omTrPurchaseReturn model, omTrPurchaseReturnDetail bpuModel, omTrPurchaseReturnSubDetail detailModel)
        {
            var recordPurRet = ctx.omTrPurchaseReturn.Find(CompanyCode, BranchCode, model.ReturnNo);
            if (recordPurRet != null)
            {
                if (recordPurRet.Status != "0" && recordPurRet.Status != "1")
                {
                    return Json(new { success = false, message = "Data Sales Model tidak bisa hapus." });
                }
            }

            var record = ctx.omTrPurchaseReturnSubDetail.Find(CompanyCode, BranchCode, model.ReturnNo, bpuModel.BPUNo, detailModel.ReturnSeq);

            if (record == null)
            {
                return Json(new { success = false, message = "Data Sales Model tidak ditemukan." });
            }
            else
            {
                using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var pCompanyCode = new SqlParameter("@CompanyCode", SqlDbType.VarChar);
                        pCompanyCode.Value = CompanyCode;
                        var pBranchCode = new SqlParameter("@BranchCode", SqlDbType.VarChar);
                        pBranchCode.Value = BranchCode;
                        var pReturnNo = new SqlParameter("@ReturnNo", SqlDbType.VarChar);
                        pReturnNo.Value = model.ReturnNo;
                        var pHPPNo = new SqlParameter("@HPPNo", SqlDbType.VarChar);
                        pHPPNo.Value = model.HPPNo;
                        var pBPUNo = new SqlParameter("@BPUNo", SqlDbType.VarChar);
                        pBPUNo.Value = bpuModel.BPUNo;
                        var pSalesModelCode = new SqlParameter("@SalesModelCode", SqlDbType.VarChar);
                        pSalesModelCode.Value = detailModel.SalesModelCode;
                        var pSalesModelYear = new SqlParameter("@SalesModelYear", SqlDbType.Int);
                        pSalesModelYear.Value = detailModel.SalesModelYear;
                        var pChassisCode = new SqlParameter("@ChassisCode", SqlDbType.VarChar);
                        pChassisCode.Value = detailModel.ChassisCode;
                        var pChassisNo = new SqlParameter("@ChassisNo", SqlDbType.Int);
                        pChassisNo.Value = detailModel.ChassisNo;
                        var pUserID = new SqlParameter("@UserID", SqlDbType.VarChar);
                        pUserID.Value = CurrentUser.UserId;
                        var pDeleteMode = new SqlParameter("@DeleteMode", SqlDbType.Int);
                        pDeleteMode.Value = 3;

                        object[] parameters = { pCompanyCode, pBranchCode, pReturnNo, pHPPNo, pBPUNo, 
                                pSalesModelCode, pSalesModelYear, pChassisCode, pChassisNo, pUserID, pDeleteMode};
                        ctx.Database.ExecuteSqlCommand(string.Format(@"exec uspfn_OmDeletePurchaseReturn @CompanyCode, @BranchCode, @ReturnNo, @HPPNo,
                                @BPUNo, @SalesModelCode, @SalesModelYear, @ChassisCode, @ChassisNo, @UserID, @DeleteMode"), parameters);

                        ctx.SaveChanges();
                        tran.Commit();

                        var isChangeStatus = ChangeStatusPrint(model.ReturnNo);
                        var bpuType = "";
                        var recBPU = ctx.omTrPurchaseBPU.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.BPUNo == bpuModel.BPUNo);
                        if (recBPU.Count() > 0)
                            bpuType = recBPU.FirstOrDefault().BPUType;

                        var records = ctx.omTrPurchaseReturnSubDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode 
                            && a.ReturnNo == bpuModel.ReturnNo && a.BPUNo == bpuModel.BPUNo);

                        return Json(new { success = true, data = record, result = records, BPUType = bpuType, IsChangeStatus = isChangeStatus });
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();

                        return Json(new { success = false, message = "Proses delete data gagal. Exception Message: " + ex.Message });
                    }
                }
            }
        }

        private IEnumerable<dynamic> PopulateRecord()
        {
            var data = from e in ctx.omTrPurchaseReturn
                       where e.CompanyCode == CompanyCode &&
                       e.BranchCode == BranchCode
                       select new ReturnView
                       {
                           ReturnNo = e.ReturnNo,
                           ReturnDate = e.ReturnDate,
                           RefferenceNo = e.RefferenceNo,
                           RefferenceDate = e.RefferenceDate,
                           HPPNo = e.HPPNo,
                           RefferenceFakturPajakNo = e.RefferenceFakturPajakNo,
                           Remark = e.Remark,
                           Status = e.Status == "0" ? "Open" : e.Status == "1" ? "Printed"
                           : e.Status == "2" ? "Approved"
                           : e.Status == "3" ? "Canceled"
                           : e.Status == "9" ? "Finished" : "",
                           Stat = e.Status
                       };

            return data;
        }

        private int Select4LookupModelTextChange(string hPPNo, string bPUNo, string salesModelCode)
        {
            var count = 0;
            var sql = @"       
            SELECT a.SalesModelCode,a.SalesModelDesc
            FROM omMstModel a
            WHERE a.SalesModelCode in (select distinct SalesModelCode from omTrPurchaseHPPDetailModel
						               where CompanyCode = @p0 and BranchCode = @p1
						               and HPPNo = @p2 and BPUNo = @p3 and SalesModelCode = @p4)
            AND a.CompanyCode = @p0
            ORDER BY a.SalesModelCode ASC";

            object[] parameters = { CompanyCode, BranchCode, hPPNo, bPUNo, salesModelCode};
            count = ctx.Database.SqlQuery<dynamic>(sql, parameters).Count();

            return count;
        }

        private int Select4LookupModelYearTextChange(string hPPNo, string bPUNo, string salesModelCode, decimal salesModelYear)
        {
            var count = 0;
            var sql = @"       
            SELECT Convert(varchar,a.SalesModelYear) AS SalesModelYear,a.SalesModelDesc
            FROM omMstModelYear a
            WHERE a.SalesModelYear in (select distinct SalesModelYear from omTrPurchaseHPPDetailModel
                                       where CompanyCode = @p0 and BranchCode = @p1
                                       and HPPNo = @p2 and BPUNo = @p3 and SalesModelCode = @p4 and SalesModelYear = @p5)
            AND a.CompanyCode = @p0
            AND a.SalesModelCode = @p4
            ORDER BY SalesModelYear ASC";

            object[] parameters = { CompanyCode, BranchCode, hPPNo, bPUNo, salesModelCode, salesModelYear };
            count = ctx.Database.SqlQuery<dynamic>(sql, parameters).Count();

            return count;
        }

        private int Select4LookupChassisBPUType0(string reffType, string hPPNo, string bPUNo, string salesModelCode, decimal salesModelYear)
        {
            var count = 0;
            var sql = @"       
            SELECT Convert(varchar,a.ChassisNo) AS ChassisNo,a.EngineCode,a.ChassisCode,
            Convert(varchar,a.EngineNo) AS EngineNo,
            a.ColourCode + ' - ' + b.RefferenceDesc1 AS Colour, Convert(varchar,a.HPPSeq) as HPPSeq
            FROM omTrPurchaseHPPSubDetail a
            LEFT JOIN omMstRefference b
            ON a.CompanyCode = b.CompanyCode
            AND b.RefferenceType = @p2
            AND b.RefferenceCode = a.ColourCode
            WHERE a.CompanyCode = @p0
            AND a.BranchCode = @p1
            AND a.HPPNo = @p3
            AND a.BPUNo = @p4
            AND a.SalesModelCode = @p5
            AND a.SalesModelYear = @p6
            AND a.isReturn = '0'
            ORDER BY a.ChassisNo ASC";

            object[] parameters = { CompanyCode, BranchCode, reffType, hPPNo, bPUNo, salesModelCode, salesModelYear };
            count = ctx.Database.SqlQuery<dynamic>(sql, parameters).Count();

            return count;
        }

        private int Select4LookupModelYear(string hPPNo, string bPUNo, string salesModelCode)
        {
            var count = 0;
            var sql = @"       
            SELECT Convert(varchar,a.SalesModelYear) AS SalesModelYear,a.SalesModelDesc
            FROM omMstModelYear a
            WHERE a.SalesModelYear in (select distinct SalesModelYear from omTrPurchaseHPPDetailModel
                                       where CompanyCode = @p0 and BranchCode = @p1
                                       and HPPNo = @p2 and BPUNo = @p3 and SalesModelCode = @p4)
            AND a.CompanyCode = @p0
            AND a.SalesModelCode = @p4
            ORDER BY SalesModelYear ASC";
            object[] parameters = { CompanyCode, BranchCode, hPPNo, bPUNo, salesModelCode };
            count = ctx.Database.SqlQuery<dynamic>(sql, parameters).Count();

            return count;
        }

        private IEnumerable<dynamic> SelectDistinctBPUs(string bpuNo){
            var recBPUs = ctx.omTrPurchaseBPU.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.BPUNo == bpuNo)
                    .Select(x => new {
                        PONo = x.PONo,
                        BPUNo = x.BPUNo,
                        BPUType = x.BPUType
                    }).Distinct();

            if(recBPUs.Count() == 0)
                throw new Exception("Data BPU tidak ditemukan.");


            return recBPUs;
        }

        private bool WarehouseIsHolding(string chassisCode, int chassisNo) 
        {
            var sql = @"
                select isnull(count(a.WarehouseCode),0) Jml 
                from omMstVehicle a
                where a.CompanyCode=@p0 and a.ChassisCode=@p2 and a.ChassisNo=@p3
	                and exists 
	                (
		                select 1 
		                from gnMstLookUpDtl 
		                where CompanyCode= a.CompanyCode and CodeID='MPWH' 
			                and ParaValue= @p1 and LookUpValue= a.WarehouseCode
	                )";

            object[] parameter = {CompanyCode, BranchCode, chassisCode, chassisNo};
            var o = ctx.Database.SqlQuery<int>(sql, parameter).FirstOrDefault();
            
            return (Convert.ToInt32(o) > 0) ? true : false;
        }

        private bool ChangeStatusPrint(string returnNo)
        {
            bool isChange = false;
            var recPurRtn = ctx.omTrPurchaseReturn.Find(CompanyCode, BranchCode, returnNo);
            if (recPurRtn != null)
            {
                if (recPurRtn.Status == "1")
                {
                    recPurRtn.Status = "0";

                    if (ctx.SaveChanges() > 0)
                    {
                        isChange= true;
                    }
                }
            }

            return isChange;
        }

        private void SaveModelNextProcess(omTrPurchaseReturnSubDetail recPurRetSdtl, omTrPurchaseHPPSubDetail recHppSdtl, string pReturn, bool isNew){
            var HPPSubDetail = ctx.omTrPurchaseHPPSubDetail.Find(CompanyCode, BranchCode, recHppSdtl.HPPNo, recPurRetSdtl.BPUNo, recHppSdtl.HPPSeq);
            if(HPPSubDetail != null){
                HPPSubDetail.isReturn = true;
                HPPSubDetail.LastUpdateBy = CurrentUser.UserId;
                HPPSubDetail.LastUpdateDate = DateTime.Now;

                ctx.SaveChanges();
            }

            string pPONo = SelectDistinctBPUs(recPurRetSdtl.BPUNo).FirstOrDefault().PONo;
            var BPUDetail = ctx.omTrPurchaseBPUDetail.Find(CompanyCode, BranchCode, pPONo, recPurRetSdtl.BPUNo, recHppSdtl.HPPSeq);
            if (BPUDetail != null)
            {
                BPUDetail.isReturn = true;
                BPUDetail.LastUpdateBy = CurrentUser.UserId;
                BPUDetail.LastUpdateDate = DateTime.Now;

                ctx.SaveChanges();

                var ReturnDetailModel = ctx.omTrPurchaseReturnDetailModel.Find(CompanyCode, BranchCode, pReturn, recPurRetSdtl.BPUNo, recPurRetSdtl.SalesModelCode, recPurRetSdtl.SalesModelYear);
                if (ReturnDetailModel == null)
                {
                    ReturnDetailModel = new omTrPurchaseReturnDetailModel();
                    ReturnDetailModel.CompanyCode = CompanyCode;
                    ReturnDetailModel.BranchCode = BranchCode;
                    ReturnDetailModel.ReturnNo = pReturn;
                    ReturnDetailModel.BPUNo = recPurRetSdtl.BPUNo;
                    ReturnDetailModel.SalesModelCode = recPurRetSdtl.SalesModelCode;
                    ReturnDetailModel.SalesModelYear = recPurRetSdtl.SalesModelYear.Value;
                    ReturnDetailModel.CreatedBy = CurrentUser.UserId;
                    ReturnDetailModel.CreatedDate = DateTime.Now;

                    ctx.omTrPurchaseReturnDetailModel.Add(ReturnDetailModel);
                }
                if (isNew) ReturnDetailModel.Quantity = (ReturnDetailModel.Quantity == null ? 0 : ReturnDetailModel.Quantity) + 1;
                var purHppDtl = ctx.omTrPurchaseHPPDetailModel.Find(CompanyCode, BranchCode, recHppSdtl.HPPNo, recPurRetSdtl.BPUNo, recPurRetSdtl.SalesModelCode, recPurRetSdtl.SalesModelYear);
                if(purHppDtl != null){
                    ReturnDetailModel.BeforeDiscDPP = purHppDtl.BeforeDiscDPP ?? 0;
                    ReturnDetailModel.DiscExcludePPn = purHppDtl.DiscExcludePPn ?? 0;
                    ReturnDetailModel.AfterDiscDPP = purHppDtl.AfterDiscDPP ?? 0;
                    ReturnDetailModel.AfterDiscPPn = purHppDtl.AfterDiscPPn ?? 0;
                    ReturnDetailModel.AfterDiscPPnBM = purHppDtl.AfterDiscPPnBM ?? 0;
                    ReturnDetailModel.AfterDiscTotal = purHppDtl.AfterDiscTotal ?? 0;
                    ReturnDetailModel.OthersDPP = purHppDtl.OthersDPP ?? 0;
                    ReturnDetailModel.OthersPPn = purHppDtl.OthersPPn ?? 0;
                }
                ReturnDetailModel.LastUpdateBy = CurrentUser.UserId;
                ReturnDetailModel.LastUpdateDate = DateTime.Now;
    
                Helpers.ReplaceNullable(ReturnDetailModel);

                ctx.SaveChanges();
            }
        }

        private void DeleteReturn(string retunNo, string hppNo, string bpuNo, string salesModelCode
            , int salesModelYear, string chassisCode, int chassisNo, int deleteMode)
        {
            var sql = "exec uspfn_OmDeletePurchaseReturn @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10";
            object[] parameters = { CompanyCode, BranchCode, retunNo, hppNo, bpuNo, salesModelCode, salesModelYear,
                                      chassisCode, chassisNo, CurrentUser.UserId, deleteMode };
            ctx.Database.ExecuteSqlCommand(sql, parameters);
        }
    }
}