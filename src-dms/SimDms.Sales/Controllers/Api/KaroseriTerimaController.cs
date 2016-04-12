using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.Models;
using System.Data;

namespace SimDms.Sales.Controllers.Api
{
    public class KaroseriTerimaController : BaseController
    {
        private const string KTR = "KRT"; // Karoseri Terima

        public ActionResult Index()
        {
            return View();
        }

        private string getStringStatus(string status)
        {
            var Status = status == "0" ? "OPEN" 
                           : status == "1" ? "PRINTED"
                           : status == "2" ? "APPROVED"
                           : status == "3" ? "CANCELED"
                           : status == "9" ? "FINISHED" : "";
            return Status;
        }

        public JsonResult DetailKaroseriterima(string KaroseriTerimaNo)
        {
            var gridDetail = ctx.omTrPurchaseKaroseriTerimaDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.KaroseriTerimaNo == KaroseriTerimaNo).ToList()
                .Select(m => new KaroseriTerimaDetailView
                {
                    ChassisNo = m.ChassisNo,
                    ChassisCode = m.ChassisCode,
                    EngineNo = m.EngineNo,
                    EngineCode = m.EngineCode,
                    ColourCodeOld = m.ColourCodeOld,
                    ColourCodeNew = m.ColourCodeNew,
                    ColourNameOld = ctx.MstRefferences.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.RefferenceType == "COLO" && e.RefferenceCode == m.ColourCodeOld).RefferenceDesc1,
                    ColourNameNew = ctx.MstRefferences.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.RefferenceType == "COLO" && e.RefferenceCode == m.ColourCodeNew).RefferenceDesc1,
                    Remark = m.Remark
                });
            return Json(new { success = true, grid = gridDetail });
        }

        public JsonResult Approve(omTrPurchaseKaroseriTerima model)
        {
            using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var msg = "";
//                    var omTrPurchasekaroseriTerima = ctx.omTrPurchaseKaroseriTerima.Find(CompanyCode, BranchCode, model.KaroseriTerimaNo);
//                    string sql = string.Format(
//                        @"update a
//                            set a.Status = '0',
//                            a.KaroseriTerimaNo = '{0}',
//                            a.SalesModelCode = '{1}',
//                            a.ColourCode = b.ColourCodeNew,
//                            a.COGSKaroseri = {2}
//                            from omMstVehicle as a
//                            inner join OmTrPurchaseKaroseriTerimaDetail b
//                            on a.CompanyCode = b.CompanyCode
//                            and a.ChassisCode = b.ChassisCode
//                            and a.ChassisNo = b.ChassisNo
//                            where b.CompanyCode = '{3}'
//                            and b.BranchCode = '{4}'
//                            and b.KaroseriTerimaNo = '{0}'", model.KaroseriTerimaNo, omTrPurchasekaroseriTerima.SalesModelCodeNew, omTrPurchasekaroseriTerima.DPPMaterial.Value + omTrPurchasekaroseriTerima.DPPOthers.Value + omTrPurchasekaroseriTerima.DPPFee.Value, CompanyCode, BranchCode);
//                    ctx.Database.ExecuteSqlCommand(sql);
//                    ctx.SaveChanges();

                    List<omTrPurchaseKaroseriTerimaDetail> recordDetail = ctx.omTrPurchaseKaroseriTerimaDetail.Where(
                        p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.KaroseriTerimaNo == model.KaroseriTerimaNo).ToList();
                    foreach (var items in recordDetail)
                    {
                        OmMstVehicle recVehicle = ctx.OmMstVehicles.Find(CompanyCode, items.ChassisCode, items.ChassisNo);

                        if (recVehicle != null)
                        {
                            recVehicle.Status = "0";
                            recVehicle.KaroseriTerimaNo = model.KaroseriTerimaNo;
                            recVehicle.SalesModelCode = model.SalesModelCodeNew;
                            recVehicle.ColourCode = items.ColourCodeNew;
                            recVehicle.COGSKaroseri = model.DPPMaterial.Value + model.DPPOthers.Value + model.DPPFee.Value;
                            
                            ctx.SaveChanges();
                        }

                        OmTrInventQtyVehicle recInvent = ctx.OmTrInventQtyVehicles.Find(
                            CompanyCode, BranchCode, model.KaroseriTerimaDate.Value.Year, model.KaroseriTerimaDate.Value.Month
                            , model.SalesModelCodeNew, model.SalesModelYear, items.ColourCodeNew, recVehicle.WarehouseCode);
                        if (recInvent == null)
                        {
                            recInvent = new OmTrInventQtyVehicle();
                            recInvent.CompanyCode = CompanyCode;
                            recInvent.BranchCode = BranchCode;
                            recInvent.Year = model.KaroseriTerimaDate.Value.Year;
                            recInvent.Month = model.KaroseriTerimaDate.Value.Month;
                            recInvent.SalesModelCode = model.SalesModelCodeNew;
                            recInvent.SalesModelYear = model.SalesModelYear.Value;
                            recInvent.ColourCode = items.ColourCodeNew;
                            recInvent.WarehouseCode = recVehicle.WarehouseCode.ToString();
                            recInvent.Alocation = 0;
                            recInvent.QtyOut = 0;
                            recInvent.BeginningOH = 0;
                            recInvent.BeginningAV = 0;
                            recInvent.Status = "0";
                            recInvent.CreatedBy = CurrentUser.UserId;
                            recInvent.CreatedDate = DateTime.Now;

                            ctx.OmTrInventQtyVehicles.Add(recInvent);
                        }

                        recInvent.QtyIn = recInvent.QtyIn.Value + 1;
                        recInvent.EndingOH = (recInvent.BeginningOH + recInvent.QtyIn) - recInvent.QtyOut;
                        recInvent.EndingAV = (recInvent.BeginningAV + recInvent.QtyIn) - recInvent.Alocation - recInvent.QtyOut;

                        Helpers.ReplaceNullable(recInvent);

                        ctx.SaveChanges();

                        OmTrInventQtyVehicle recInventOld = ctx.OmTrInventQtyVehicles.Find(CompanyCode, BranchCode, model.KaroseriTerimaDate.Value.Year
                                              , model.KaroseriTerimaDate.Value.Month, model.SalesModelCodeOld, model.SalesModelYear
                                              , items.ColourCodeOld, recVehicle.WarehouseCode.ToString());
                        if (recInventOld != null)
                        {
                            recInventOld.QtyOut = recInventOld.QtyOut.Value + 1;
                            recInventOld.EndingOH = (recInventOld.BeginningOH + recInventOld.QtyIn) - recInventOld.QtyOut;
                            recInventOld.EndingAV = (recInventOld.BeginningAV + recInventOld.QtyIn) - recInventOld.Alocation - recInventOld.QtyOut;

                            Helpers.ReplaceNullable(recInventOld);
                            ctx.SaveChanges();
                        }
                    }

                    var record = ctx.omTrPurchaseKaroseriTerima.Find(CompanyCode, BranchCode, model.KaroseriTerimaNo);

                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
                    record.Status = "2";

                    bool success = ctx.SaveChanges() >= 0;
                    if (success)
                    {
                        tran.Commit();
                        msg = string.Format(ctx.SysMsgs.Find("5038").MessageCaption, "Approve Karoseri Terima", "");
                        return Json(new { success = success, message = msg, status = getStringStatus(record.Status), Result = record.Status });
                    }
                    else
                    {
                        tran.Rollback();
                        msg = string.Format(ctx.SysMsgs.Find("5039").MessageCaption, "Approve Karoseri Terima", "");
                        return Json(new { success = success, message = msg });
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return Json(new { success = false, message = "Proses Approve Karoseri Terima tidak berhasil. Exception message: " +ex.Message});
                }
            }
        }

        public JsonResult prePrint(omTrPurchaseKaroseriTerima model)
        {
            var msg = "";
            var Hdr = ctx.omTrPurchaseKaroseriTerima.Find(CompanyCode, BranchCode, model.KaroseriTerimaNo);
            var Dtl1 = ctx.omTrPurchaseKaroseriTerimaDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.KaroseriTerimaNo == model.KaroseriTerimaNo);
            if (Dtl1.Count() < 1)
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

            var result = ctx.SaveChanges() >= 0;

            return Json(new { success = result, Status = getStringStatus(Hdr.Status), stat = Hdr.Status });
        }

        public JsonResult Save(omTrPurchaseKaroseriTerima model)
        {
            string msg = "";
            var record = ctx.omTrPurchaseKaroseriTerima.Find(CompanyCode, BranchCode, model.KaroseriTerimaNo);

            if (record == null)
            {
                record = new omTrPurchaseKaroseriTerima
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    KaroseriTerimaNo = GetNewDocumentNo(KTR, model.KaroseriTerimaDate.Value),
                    KaroseriTerimaDate = model.KaroseriTerimaDate,
                    KaroseriSPKNo = model.KaroseriSPKNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,

                    
                };

                ctx.omTrPurchaseKaroseriTerima.Add(record);
            }

            record.SupplierCode = model.SupplierCode;
            record.RefferenceInvoiceNo = model.RefferenceInvoiceNo;
            record.RefferenceInvoiceDate = model.RefferenceInvoiceDate;
            record.RefferenceFakturPajakNo = model.RefferenceFakturPajakNo == null ? ".-." : model.RefferenceFakturPajakNo;
            record.RefferenceFakturPajakDate = model.RefferenceFakturPajakDate;
            record.DueDate = model.DueDate;
            record.SalesModelCodeOld = model.SalesModelCodeOld;
            record.SalesModelYear = model.SalesModelYear;
            record.SalesModelCodeNew = model.SalesModelCodeNew;
            record.ChassisCode = model.ChassisCode;
            record.Quantity = model.Quantity == null ? 0 : model.Quantity;
            record.DPPMaterial = model.DPPMaterial == null ? 0 : model.DPPMaterial;
            record.DPPFee = model.DPPFee == null ? 0 : model.DPPFee;
            record.DPPOthers = model.DPPOthers == null ? 0 : model.DPPOthers;
            record.PPn = model.PPn == null ? 0 : model.PPn;
            record.Total = model.Total == null ? 0 : model.Total;
            record.PPh = model.PPh == null ? 0 : model.PPh;
            record.Remark = model.Remark == null ? "" : model.Remark;
            record.Status = "0";
                   
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.isLocked = false;
            record.LockingBy = "";
            record.LockingDate = model.DueDate.Value;

            try
            {
                Helpers.ReplaceNullable(record);
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record, status = getStringStatus(record.Status) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SaveDetail(omTrPurchaseKaroseriTerima model, omTrPurchaseKaroseriTerimaDetail detailModel)
        {
            var record = ctx.omTrPurchaseKaroseriTerimaDetail.Find(CompanyCode, BranchCode, model.KaroseriTerimaNo, detailModel.ChassisCode, detailModel.ChassisNo);

            if (record == null)
            {
                record = new omTrPurchaseKaroseriTerimaDetail
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    KaroseriTerimaNo = model.KaroseriTerimaNo,   
                    ChassisCode = detailModel.ChassisCode,
                    ChassisNo = detailModel.ChassisNo,
                    EngineCode = detailModel.EngineCode,
                    EngineNo = detailModel.EngineNo,
                    ColourCodeOld = detailModel.ColourCodeOld,
                    ColourCodeNew = detailModel.ColourCodeNew,   
                    Remark = detailModel.Remark == null ? "" : detailModel.Remark,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                };

                ctx.omTrPurchaseKaroseriTerimaDetail.Add(record);
            }
            else
            {
                record.Remark = detailModel.Remark == null ? "" : detailModel.Remark;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
                
                ctx.omTrPurchaseKaroseriTerimaDetail.Attach(record);
            }

            var recordHdr = ctx.omTrPurchaseKaroseriTerima.Find(CompanyCode, BranchCode, model.KaroseriTerimaNo);
            if (recordHdr != null)
            {
                recordHdr.Quantity = recordHdr.Quantity + 1;
                ctx.SaveChanges();
            }

            ctx.Database.ExecuteSqlCommand(@"UPDATE omTrPurchaseKaroseriDetail SET isKaroseriTerima = 1
                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
                                                "' and KaroseriSPKNo='" + model.KaroseriSPKNo + "' and ChassisCode='" + detailModel.ChassisCode +
                                                "' and ChassisNo='" + detailModel.ChassisNo + "'");

            try
            {
                ctx.SaveChanges();
                var gridDetail = ctx.omTrPurchaseKaroseriTerimaDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.KaroseriTerimaNo == model.KaroseriTerimaNo).ToList()
               .Select(m => new KaroseriTerimaDetailView
               {
                   ChassisNo = m.ChassisNo,
                   ChassisCode = m.ChassisCode,
                   EngineNo = m.EngineNo,
                   EngineCode = m.EngineCode,
                   ColourCodeOld = m.ColourCodeOld,
                   ColourCodeNew = m.ColourCodeNew,
                   ColourNameOld = ctx.MstRefferences.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.RefferenceType == "COLO" && e.RefferenceCode == m.ColourCodeOld).RefferenceDesc1,
                   ColourNameNew = ctx.MstRefferences.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.RefferenceType == "COLO" && e.RefferenceCode == m.ColourCodeNew).RefferenceDesc1,
                   Remark = m.Remark
               });
                return Json(new { success = true, grid = gridDetail });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(omTrPurchaseKaroseriTerima model)
        {
            var record = ctx.omTrPurchaseKaroseriTerima.Find(CompanyCode, BranchCode, model.KaroseriTerimaNo);
            var Dtl1 = ctx.omTrPurchaseKaroseriTerimaDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.KaroseriTerimaNo == model.KaroseriTerimaNo);

            if (record == null || Dtl1.Count() > 0)
            {
                return Json(new { success = false, message = "Delete gagal, Masih ada detail" });
            }
            else
            {
                ctx.Database.ExecuteSqlCommand(@"UPDATE omTrPurchaseKaroseriTerima SET Status = 3
                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
                                                "' and KaroseriTerimaNo='" + model.KaroseriTerimaNo + "'");

                ctx.SaveChanges();
            }
            try
            {
                var data = (from e in ctx.omTrPurchaseKaroseriTerima
                           where e.CompanyCode == CompanyCode &&
                           e.BranchCode == BranchCode
                           select new KaroseriTerimaView
                           {
                               KaroseriTerimaNo = e.KaroseriTerimaNo,
                               KaroseriTerimaDate = e.KaroseriTerimaDate,
                               KaroseriSPKNo = e.KaroseriSPKNo,
                               SupplierCode = e.SupplierCode,
                               RefferenceInvoiceNo = e.RefferenceInvoiceNo,
                               RefferenceInvoiceDate = e.RefferenceInvoiceDate,
                               RefferenceFakturPajakNo = e.RefferenceFakturPajakNo,
                               RefferenceFakturPajakDate = e.RefferenceFakturPajakDate,
                               DueDate = e.DueDate,
                               SalesModelCodeOld = e.SalesModelCodeOld,
                               SalesModelYear = e.SalesModelYear,
                               SalesModelCodeNew = e.SalesModelCodeNew,
                               ChassisCode = e.ChassisCode,
                               Quantity = e.Quantity,
                               DPPMaterial = e.DPPMaterial,
                               DPPFee = e.DPPFee,
                               DPPOthers = e.DPPOthers,
                               PPn = e.PPn,
                               Total = e.Total,
                               PPh = e.PPh,
                               Status = e.Status == "0" ? "OPEN" : e.Status == "1" ? "PRINTED"
                               : e.Status == "2" ? "APPROVED"
                               : e.Status == "3" ? "CANCELED"
                               : e.Status == "9" ? "FINISHED" : "",
                               Stat = e.Status
                           }).FirstOrDefault();

                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
        public JsonResult DeleteDetail(omTrPurchaseKaroseriTerima model, omTrPurchaseKaroseriTerimaDetail detailModel)
        {
            using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var record = ctx.omTrPurchaseKaroseriTerimaDetail.Find(CompanyCode, BranchCode, model.KaroseriTerimaNo, detailModel.ChassisCode, detailModel.ChassisNo);

                    if (record == null)
                    {
                        return Json(new { success = false, message = "Record not found or has been deleted" });
                    }
                    else
                    {
                        ctx.Database.ExecuteSqlCommand(@"UPDATE omTrPurchaseKaroseriDetail SET isKaroseriTerima = 0
                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
                                                        "' and KaroseriSPKNo='" + model.KaroseriSPKNo + "' and ChassisCode='" + detailModel.ChassisCode +
                                                        "' and ChassisNo='" + detailModel.ChassisNo + "'");

                        ctx.omTrPurchaseKaroseriTerimaDetail.Remove(record);

                        ctx.SaveChanges();

                        tran.Commit();
                    }

                    var gridDetail = ctx.omTrPurchaseKaroseriTerimaDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.KaroseriTerimaNo == model.KaroseriTerimaNo).ToList()
                   .Select(m => new KaroseriTerimaDetailView
                   {
                       ChassisNo = m.ChassisNo,
                       ChassisCode = m.ChassisCode,
                       EngineNo = m.EngineNo,
                       EngineCode = m.EngineCode,
                       ColourCodeOld = m.ColourCodeOld,
                       ColourCodeNew = m.ColourCodeNew,
                       ColourNameOld = ctx.MstRefferences.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.RefferenceType == "COLO" && e.RefferenceCode == m.ColourCodeOld).RefferenceDesc1,
                       ColourNameNew = ctx.MstRefferences.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.RefferenceType == "COLO" && e.RefferenceCode == m.ColourCodeNew).RefferenceDesc1,
                       Remark = m.Remark
                   });

                    return Json(new { success = true, grid = gridDetail });
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    return Json(new { success = false, message = "Hapus Data Detil tidak berhasil. Exception message: " + ex.Message });
                }
            }
        }
        public JsonResult ReCalculateDPPnPPN(omTrPurchaseKaroseriTerima model)
        {
            var oGnMstSupplierProfitCenter = ctx.SupplierProfitCenter.Find(CompanyCode, BranchCode, model.SupplierCode, "100");
            string ptPPn = (oGnMstSupplierProfitCenter == null) ? "NON" : oGnMstSupplierProfitCenter.TaxCode;
            var oGnMstTax = ctx.Taxs.Find(CompanyCode, ptPPn);
            decimal pctPPn = (oGnMstTax == null) ? 0 : oGnMstTax.TaxPct.Value;
            model.DPPMaterial = Math.Round(model.Total.Value / ((100 + pctPPn) / 100), MidpointRounding.AwayFromZero);
            decimal tPPn = Math.Floor(model.DPPMaterial.Value * (pctPPn / 100));
            model.PPn = model.Total.Value - model.DPPMaterial.Value;
            model.Remark = model.Remark == null ? "" : model.Remark;
            return Json(new { success = true, data = model });
        }
    }
}
