using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace SimDms.Sales.Controllers.Api
{
    public class POController : BaseController
    {
        private const string PUR = "PUR"; // Purchase Order (Order Management/Unit)
        private string msg = "";
        private string poNoUpload = "";
        //string batchNo;

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPO(string PONo)
        {
            var record = ctx.OmTrPurchasePOs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PONo == PONo).ToList()
                .Select(c => new POBrowse
                {
                    PONo = c.PONo,
                    PODate = c.PODate,
                    RefferenceNo = c.RefferenceNo,
                    RefferenceDate = c.RefferenceDate,
                    SupplierCode = c.SupplierCode,
                    SupplierName = ctx.Supplier.Find(CompanyCode,c.SupplierCode).SupplierName,
                    BillTo = c.BillTo,
                    ShipTo = c.ShipTo,
                    Remark = c.Remark,
                    Status = c.Status
                });
            var gridDetail = ctx.OmTrPurchasePOModels.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PONo == PONo);
            return Json(new { success = true, record = record, grid = gridDetail });
        }

        public JsonResult GetDetailPO(PODetailModel model)
        {
            decimal? beforeDiscTotal = 0;
            var dtot = ctx.OmMstPricelistBuys.Find(CompanyCode, BranchCode, model.SupplierCode, model.SalesModelCode, model.SalesModelYear);
            if (dtot != null)
            {
                beforeDiscTotal = dtot.Total;
            }

            var detail = ctx.OmTrPurchasePOModels.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PONo == model.PONo && a.SalesModelCode == model.SalesModelCode && a.SalesModelYear == model.SalesModelYear).ToList()
                .Select(e => new PODetailView
                {
                    SalesModelCode = e.SalesModelCode,
                    SalesModelYear = e.SalesModelYear,
                    SalesModelDesc = ctx.OmMstModels.Find(CompanyCode, model.SalesModelCode).SalesModelDesc,
                    BeforeDiscDPP = e.BeforeDiscDPP,
                    BeforeDiscPPn = e.BeforeDiscPPn,
                    BeforeDiscPPnBM = e.BeforeDiscPPnBM,
                    BeforeDiscTotal = beforeDiscTotal,
                    DiscExcludePPn = e.DiscExcludePPn,
                    DiscIncludePPn = beforeDiscTotal == 0 ? 0 : beforeDiscTotal - e.AfterDiscTotal,
                    AfterDiscDPP = e.AfterDiscDPP,
                    AfterDiscPPn = e.AfterDiscPPn,
                    AfterDiscPPnBM = e.AfterDiscPPnBM,
                    AfterDiscTotal = e.AfterDiscTotal,
                    PPnBMPaid = e.PPnBMPaid,
                    OthersDPP = e.OthersDPP,
                    OthersPPn = e.OthersPPn,
                    QuantityPO = e.QuantityPO,
                    QuantityBPU = e.QuantityBPU,
                    Remark = e.Remark
                });

            //var colour = ctx.OmTrPurchasePOModelColours.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
            //    && a.PONo == model.PONo && a.SalesModelCode == model.SalesModelCode && a.SalesModelYear == model.SalesModelYear).ToList()
            //    .Select(m => new POColourView
            //    {
            //        ColourCode = m.ColourCode,
            //        ColourDesc = "",
            //        //ctx.MstRefferences.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.RefferenceType == "COLO" && e.RefferenceCode == m.ColourCode && e.Status == "1").RefferenceDesc1,
            //        Quantity = m.Quantity,
            //        Remark = m.Remark
            //    });

            var colour = from a in ctx.OmTrPurchasePOModelColours
                            join b in ctx.MstRefferences
                            on new { a.CompanyCode, a.ColourCode, RefferenceType = "COLO", Status = "1" } equals new { b.CompanyCode, ColourCode = b.RefferenceCode, b.RefferenceType, b.Status }
                            where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PONo == model.PONo && a.SalesModelCode == model.SalesModelCode && a.SalesModelYear == model.SalesModelYear
                            select new POColourView
                             {
                                 ColourCode = a.ColourCode,
                                 ColourDesc = b.RefferenceDesc1,
                                 Quantity = a.Quantity,
                                 Remark = a.Remark
                             };


            return Json(new { success = true, detail = detail, colour = colour });
        }

        public JsonResult ValidateSave(OmTrPurchasePO model, string salesModeCode, int options, string batchNo)
        {
            var record = ctx.OmTrPurchasePOs.Find(CompanyCode, BranchCode, model.PONo);

            if (!CekStatusDocument(record))
            {
                msg = "Status dokumen sudah Approve atau Cancel !";
                return Json(new { success = false, message = msg });
            }

            var valDate = DateTransValidation(model.PODate.Value, ref msg);
            if (!valDate) return Json(new { success = valDate, message = msg });

            //var isExistReffNo = ctx.OmTrPurchasePOs.Where(a => a.CompanyCode == CompanyCode
            //    && a.BranchCode == BranchCode && a.RefferenceNo == model.RefferenceNo && a.Status != "3");
            //if (isExistReffNo.Count() > 0)
            //{
            //    msg = "No. Reff sudah ada di PO";
            //    return Json(new { success = false, message = msg });
            //}

            bool isNew = false;
            if (record == null)
            {
                isNew = true;
                record = new OmTrPurchasePO();
                record.CompanyCode = CompanyCode;
                record.BranchCode = BranchCode;
                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
            }
            record.PODate = model.PODate;
            record.RefferenceNo = model.RefferenceNo;
            record.RefferenceDate = model.RefferenceNo != "" ? model.RefferenceDate : Convert.ToDateTime("1900/01/01");
            record.SupplierCode = model.SupplierCode;
            record.BillTo = model.BillTo;
            record.ShipTo = model.ShipTo;
            record.Remark = model.Remark;
            record.Status = "0";

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            bool result = false;
            bool outstanding = false;
            var PONo = "";
            if (options == 1 && isNew)
            {
                string standardCode = "";
                var supplier = ctx.Supplier.Find(CompanyCode, model.SupplierCode);
                if (supplier != null)
                    standardCode = supplier.StandardCode;
                OmUtlSPORDHdr utlPord = ctx.OmUtlSPORDHdrs.Find(CompanyCode, BranchCode,
                    standardCode, CompanyCode, batchNo);
                if (utlPord == null)
                {
                    msg = "Data upload tidak ditemukan";
                    return Json(new { success = false, message = msg });
                }
                else
                {
                    object[] parameters = { CompanyCode, BranchCode, model.RefferenceNo, batchNo, "", 1};
                    var query = "exec usprpt_OmRpPurTrn011 @p0,@p1,@p2,@p3,@p4,@p5";

                    var dtAccount = ctx.Database.SqlQuery<OutstandingAccount>(query, parameters);

                    if (dtAccount.Count() > 0)
                    {
                        msg = "Ada account yang belum disetting, apakah anda ingin melihat listnya?";
                        outstanding = true;
                    }

                    var msg2 = "Data Berhasil disimpan.";
                    result = SavePOUpload(record, batchNo, ref msg2);

                    return Json(new { success = result, message = outstanding ? msg : "", message2 = msg2, outstanding = outstanding, PONo = poNoUpload });
                }
            }
            else
            {
                PONo = SavePO(record, isNew);
                result = PONo != "";
                msg = "Data Save";
            }

            return Json(new { success = result, message = msg, PONo = PONo });
        }

        private string SavePO(OmTrPurchasePO record, bool isNew)
        {
            if (isNew)
            {
                record.PONo = GetNewDocumentNo(PUR, record.PODate.Value);
                ctx.OmTrPurchasePOs.Add(record);
            }
            Helpers.ReplaceNullable(record);
            var result = ctx.SaveChanges() > 0 ;

            return result ? record.PONo : "";
        }

        private bool SavePOUpload(OmTrPurchasePO record, string batchNo, ref string msg2)
        {
            using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    bool result = false;
                    record.PONo = GetNewDocumentNo(PUR, record.PODate.Value);
                    ctx.OmTrPurchasePOs.Add(record);
                    Helpers.ReplaceNullable(record);
                    result = ctx.SaveChanges() > 0;
                    if (result)
                    {
                        var SKPNo = record.RefferenceNo;
                        var PONo = record.PONo;
                        var utilDtl = ctx.OmUtlSPORDDtl1s.Find(CompanyCode, BranchCode, batchNo, SKPNo);
                        if (utilDtl != null)
                        {
                            utilDtl.Status = "1";
                            utilDtl.LastUpdateBy = CurrentUser.UserId;
                            utilDtl.LastUpdateDate = DateTime.Now;

                            if (ctx.SaveChanges() > 0)
                            {
                                var dtModelPORD = ctx.OmUtlSPORDDtl2s.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SKPNo == SKPNo && a.BatchNo == batchNo).ToList();
                                if (dtModelPORD.Count() > 0)
                                {
                                    foreach (var dtl in dtModelPORD)
                                    {
                                        OmTrPurchasePOModel model = new OmTrPurchasePOModel
                                        {
                                            CompanyCode = CompanyCode,
                                            BranchCode = BranchCode,
                                            PONo = PONo,
                                            SalesModelCode = dtl.SalesModelCode,
                                            SalesModelYear = dtl.SalesModelYear,
                                            BeforeDiscDPP = dtl.BeforeDiscountDPP,
                                            BeforeDiscPPn = dtl.BeforeDiscountPPN,
                                            BeforeDiscPPnBM = dtl.BeforeDiscountPPNBM,
                                            BeforeDiscTotal = dtl.BeforeDiscountTotal,
                                            DiscExcludePPn = dtl.DiscountExcludePPN,
                                            DiscIncludePPn = dtl.DiscountIncludePPN,
                                            AfterDiscDPP = dtl.AfterDiscountDPP,
                                            AfterDiscPPn = dtl.AfterDiscountPPN,
                                            AfterDiscPPnBM = dtl.AfterDiscountPPNBM,
                                            AfterDiscTotal = dtl.AfterDiscountTotal,
                                            PPnBMPaid = dtl.PPNBMPaid,
                                            OthersDPP = dtl.OthersDPP,
                                            OthersPPn = dtl.OthersPPN,
                                            QuantityPO = dtl.Quantity,
                                            QuantityBPU = 0,
                                            Remark = "",
                                            CreatedBy = CurrentUser.UserId,
                                            CreatedDate = DateTime.Now
                                        };
                                        ctx.OmTrPurchasePOModels.Add(model);
                                        Helpers.ReplaceNullable(model);
                                        if (ctx.SaveChanges() < 0)
                                        {
                                            throw new Exception("Gagal save di PO Model");
                                        }
                                        else
                                        {
                                            var dtlModelColour = ctx.OmUtlSPORDDtl3s.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SKPNo == SKPNo && a.SalesModelCode == model.SalesModelCode && a.SalesModelYear == model.SalesModelYear && a.BatchNo == batchNo).ToList();

                                            if (dtlModelColour.Count() > 0)
                                            {
                                                foreach (var colour in dtlModelColour)
                                                {
                                                    var masterModelColour = ctx.OmMstModelColours.Find(CompanyCode, model.SalesModelCode, colour.ColourCode);
                                                    if (masterModelColour == null)
                                                    {
                                                        throw new Exception("Untuk Model " + model.SalesModelCode + " Warna " + colour.ColourCode + " belum ada");
                                                    }

                                                    OmTrPurchasePOModelColour modelColour = new OmTrPurchasePOModelColour
                                                    {
                                                        CompanyCode = CompanyCode,
                                                        BranchCode = BranchCode,
                                                        PONo = PONo,
                                                        SalesModelCode = model.SalesModelCode,
                                                        SalesModelYear = model.SalesModelYear,
                                                        ColourCode = colour.ColourCode,
                                                        Quantity = colour.Quantity,
                                                        Remark = "",
                                                        CreatedBy = CurrentUser.UserId,
                                                        CreatedDate = DateTime.Now
                                                    };
                                                    Helpers.ReplaceNullable(modelColour);
                                                    ctx.OmTrPurchasePOModelColours.Add(modelColour);
                                                    if (ctx.SaveChanges() < 0)
                                                    {
                                                        throw new Exception("Gagal save di PO Model Colour");
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }

                            }
                        }
                        poNoUpload = PONo;
                    }
                    else throw new Exception("Gagal update table temporary 1");

                    if (result)
                    {
                        tran.Commit();
                    }
                    else
                    {
                        tran.Rollback();
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    msg2 = ex.Message;

                    return false;
                }
            }
        }

        public JsonResult SaveDetail(OmTrPurchasePO model, OmTrPurchasePOModel poModel)
        {
            var result = false;
            var outstanding = false;
            var record = ctx.OmTrPurchasePOs.Find(CompanyCode, BranchCode, model.PONo);
            var detailRecord = ctx.OmTrPurchasePOModels.Find(CompanyCode, BranchCode, model.PONo, poModel.SalesModelCode, poModel.SalesModelYear);
            if (detailRecord == null)
            {
                detailRecord = new OmTrPurchasePOModel()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    PONo = model.PONo,
                    SalesModelCode = poModel.SalesModelCode,
                    SalesModelYear = poModel.SalesModelYear,
                    QuantityPO = 0,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };
                ctx.OmTrPurchasePOModels.Add(detailRecord);
            }

            var priceListBuy = ctx.OmMstPricelistBuys.Find(CompanyCode, BranchCode, model.SupplierCode, poModel.SalesModelCode, poModel.SalesModelYear);

            detailRecord.BeforeDiscDPP = priceListBuy != null ? priceListBuy.DPP : 0;
            detailRecord.BeforeDiscPPn = priceListBuy != null ?priceListBuy.PPn : 0;
            detailRecord.BeforeDiscPPnBM = priceListBuy != null ?priceListBuy.PPnBM : 0;
            detailRecord.BeforeDiscTotal = priceListBuy != null ?priceListBuy.Total : 0;
            detailRecord.DiscExcludePPn = priceListBuy != null ? (priceListBuy.DPP - poModel.AfterDiscDPP) : 0 - poModel.AfterDiscDPP;
            detailRecord.DiscIncludePPn = poModel.DiscIncludePPn ?? 0;
            detailRecord.AfterDiscDPP = poModel.AfterDiscDPP ?? 0;
            detailRecord.AfterDiscPPn = poModel.AfterDiscPPn ?? 0;
            detailRecord.AfterDiscPPnBM = poModel.AfterDiscPPnBM ?? 0;
            detailRecord.AfterDiscTotal = poModel.AfterDiscTotal ?? 0;
            detailRecord.PPnBMPaid = poModel.PPnBMPaid ?? 0;
            detailRecord.OthersDPP = poModel.OthersDPP ?? 0;
            detailRecord.OthersPPn = poModel.OthersPPn ?? 0;
            detailRecord.QuantityBPU = 0;
            detailRecord.Remark = poModel.Remark ?? "";
            detailRecord.LastUpdateBy = CurrentUser.UserId;
            detailRecord.LastUpdateDate = DateTime.Now;

            Helpers.ReplaceNullable(detailRecord);
            try
            {
                result = ctx.SaveChanges() > 0;
                if (result)
                {

                    object[] parameters = { CompanyCode, BranchCode, model.RefferenceNo, "", poModel.SalesModelCode, 0 };
                    var query = "exec usprpt_OmRpPurTrn011 @p0,@p1,@p2,@p3,@p4,@p5";
                    var dtAccount = ctx.Database.SqlQuery<OutstandingAccount>(query, parameters);

                    if (dtAccount.Count() > 0)
                    {
                        msg = "Ada account yang belum disetting, apakah anda ingin melihat listnya?";
                        outstanding = true;
                    }

                    resetStatusPO(record.PONo);
                }

                return Json(new { success = result, message = outstanding ? msg : "", outstanding = outstanding });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

        public JsonResult DeleteDetail(OmTrPurchasePO model, OmTrPurchasePOModel poMOdel)
        {
            var result = false;

            var query = @"DELETE FROM dbo.omTrPurchasePOModelColour
                                WHERE companyCode = @p0
                                AND branchCode = @p1
                                AND PONo = @p2 
                                AND salesModelCode = @p3
                                AND salesModelYear = @p4";

            object[] parameters = { CompanyCode, BranchCode, model.PONo, poMOdel.SalesModelCode, poMOdel.SalesModelYear };

            result = ctx.Database.ExecuteSqlCommand(query, parameters) > -1;

            var recordModel = ctx.OmTrPurchasePOModels.Find(CompanyCode, BranchCode, model.PONo, poMOdel.SalesModelCode, poMOdel.SalesModelYear);

            if (result)
            {
                ctx.OmTrPurchasePOModels.Remove(recordModel);
                result = ctx.SaveChanges() >= 0;
            }

            if (result)
            {
                resetStatusPO(model.PONo);
            }

            return Json(new { success = result });
        }

        public JsonResult SaveColour(OmTrPurchasePO model, OmTrPurchasePOModel poModel, OmTrPurchasePOModelColour colourModel)
        {
            var result = false;
            var record = ctx.OmTrPurchasePOs.Find(CompanyCode, BranchCode, model.PONo);
            var modelRecord = ctx.OmTrPurchasePOModels.Find(CompanyCode, BranchCode, model.PONo, poModel.SalesModelCode, poModel.SalesModelYear);
            var colourrecord = ctx.OmTrPurchasePOModelColours.Find(CompanyCode, BranchCode, model.PONo, poModel.SalesModelCode, poModel.SalesModelYear, colourModel.ColourCode);

            if (colourrecord == null)
            {
                colourrecord = new OmTrPurchasePOModelColour()
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    PONo = model.PONo,
                    SalesModelCode = poModel.SalesModelCode,
                    SalesModelYear = poModel.SalesModelYear,
                    ColourCode = colourModel.ColourCode,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };
                ctx.OmTrPurchasePOModelColours.Add(colourrecord);
            }
            colourrecord.Quantity = colourModel.Quantity;
            colourrecord.Remark = colourModel.Remark ?? "";

            colourrecord.LastUpdateBy = CurrentUser.UserId;
            colourrecord.LastUpdateDate = DateTime.Now;

            try
            {
                result = ctx.SaveChanges() >= 0;

                if (result)
                {
                    decimal totQty = getQtyDetail(colourrecord);

                    modelRecord.QuantityPO = totQty;
                    result = ctx.SaveChanges() >= 0;
                }

                if (result)
                {
                    resetStatusPO(model.PONo);
                }

                return Json(new { success = result });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

        public JsonResult DeleteColour(OmTrPurchasePO model, OmTrPurchasePOModel poModel, OmTrPurchasePOModelColour colourModel)
        {
            var result = false;
            var recordModel = ctx.OmTrPurchasePOModels.Find(CompanyCode, BranchCode, model.PONo, poModel.SalesModelCode, poModel.SalesModelYear);
            var recordColour = ctx.OmTrPurchasePOModelColours.Find(CompanyCode, BranchCode, model.PONo, poModel.SalesModelCode, poModel.SalesModelYear, colourModel.ColourCode);

            ctx.OmTrPurchasePOModelColours.Remove(recordColour);

            result = ctx.SaveChanges() > 0;
            if (result)
            {
                decimal totQty = getQtyDetail(recordColour);
                if (recordModel != null)
                {
                    recordModel.QuantityPO = totQty;
                    result = ctx.SaveChanges() >= 0;
                }
            }

            if (result)
            {
                resetStatusPO(model.PONo);
            }

            return Json(new { success = result});
        }

        public JsonResult prePrint(OmTrPurchasePO model)
        {
            var Hdr = ctx.OmTrPurchasePOs.Find(CompanyCode, BranchCode, model.PONo);
            var Dtls = ctx.OmTrPurchasePOModels.Where(a=>a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.PONo == model.PONo);
            if (Dtls.Count() < 1)
            {
                msg = ctx.SysMsgs.Find("5047").MessageCaption;
                return Json(new { success = false, message = msg });
            }
            else
            {
                foreach (var dtl in Dtls)
                {
                    if (dtl.QuantityPO.ToString() == "0")
                    {
                        msg = "Dokumen tidak dapat dicetak karena satu atau lebih detail PO memiliki Quantity = 0";
                        return Json(new { success = false, message = msg });
                    }
                }
            }

            if (Hdr.Status == "0" || Hdr.Status.Trim() == "")
            {
                Hdr.Status = "1";
            }

            Hdr.LastUpdateBy = CurrentUser.UserId;
            Hdr.LastUpdateDate = DateTime.Now;

            var result = ctx.SaveChanges() >= 0;

            var Status = getStringStatus(Hdr.Status);

            return Json(new { success = result, Status = Status, stat = Hdr.Status });
        }

        public JsonResult ApprovePO(OmTrPurchasePO model)
        {
            var record = ctx.OmTrPurchasePOs.Find(CompanyCode, BranchCode, model.PONo);

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.Status = "2";

            bool success = ctx.SaveChanges() >= 0;
            if (success)
            {
                msg = string.Format(ctx.SysMsgs.Find("5038").MessageCaption, "Approve PO", "");
                return Json(new { success = success, message = msg, status = getStringStatus(record.Status) });
            }
            else
            {
                msg = string.Format(ctx.SysMsgs.Find("5039").MessageCaption, "Approve PO", "");
                return Json(new { success = success, message = msg });
            }
        }

        public JsonResult CancelPO(OmTrPurchasePO model)
        {
            var record = ctx.OmTrPurchasePOs.Find(CompanyCode, BranchCode, model.PONo);

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.Status = "3";

            bool success = ctx.SaveChanges() > 0;

            if (success)
            {
                var utlSPORDDtl = ctx.OmUtlSPORDDtl1s.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SKPNo == record.RefferenceNo);

                if (utlSPORDDtl.Where(m=>m.SKPNo != "").Count() > 0)
                {
                    utlSPORDDtl.FirstOrDefault().Status = "0";
                    success = ctx.SaveChanges() > 0;
                }
                else
                {
                    success = true;
                }
            }

            if (success)
            {
                msg = string.Format(ctx.SysMsgs.Find("5038").MessageCaption, "Cancel PO", "");
                return Json(new { success = success, message = msg, status = getStringStatus(record.Status) });
            }
            else
            {
                msg = string.Format(ctx.SysMsgs.Find("5039").MessageCaption, "Cancel PO", "");
                return Json(new { success = success, message = msg });
            }
        }
    
        private bool CekStatusDocument(OmTrPurchasePO record)
        {
            bool result = true;

            if (record != null)
            {
                if (record.Status != "0" && record.Status != "1")
                {
                    result = false;
                }
            }
            return result;
        }

        private void resetStatusPO(string PONo)
        {
            var record = ctx.OmTrPurchasePOs.Find(CompanyCode, BranchCode, PONo);

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.Status = "0";

            ctx.SaveChanges();
        }

        private decimal getQtyDetail(OmTrPurchasePOModelColour model)
        {
            try
            {
                var data = ctx.OmTrPurchasePOModelColours.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SalesModelCode == model.SalesModelCode && a.SalesModelYear == model.SalesModelYear && a.PONo == model.PONo)
                    .GroupBy(a => new { a.PONo, a.SalesModelCode, a.SalesModelYear })
                    .Select(m => new { totQty = m.Sum(e => e.Quantity) }).FirstOrDefault();
                
                return data.totQty.Value;
            }
            catch
            {
                return 0;
            }
        }

        public JsonResult calculatePrice(bool val, string supplierCode, OmTrPurchasePOModel poModel)
        {
            var ppnPct = (from a in ctx.Taxs
                          join d in ctx.SupplierProfitCenter
                          on new { a.CompanyCode, a.TaxCode }
                          equals new { d.CompanyCode, d.TaxCode }
                          where a.CompanyCode == CompanyCode
                          && d.SupplierCode == supplierCode
                          && d.ProfitCenterCode == "100"
                          select a.TaxPct).FirstOrDefault() ?? 10;

            var ppnBmPctBuy = ctx.OmMstModels.Find(CompanyCode, poModel.SalesModelCode);

            var ppnBmPct = ppnBmPctBuy != null ? ppnBmPctBuy.PpnBmPctBuy.Value : 10;

            var totalPriceBeforeDisc = poModel.BeforeDiscTotal ?? 0;

            if (val == true)
            {
                var totalPrice = poModel.AfterDiscTotal ?? 0;

                decimal dpp = Math.Round(totalPrice / ((100 + ppnPct + ppnBmPct) / 100), MidpointRounding.AwayFromZero);
                decimal ppnBm = Math.Floor(dpp * (ppnBmPct / 100));
                decimal ppn = totalPrice - dpp - ppnBm;
                decimal disc = totalPriceBeforeDisc - totalPrice;

                return Json(new
                {
                    totalPrice = totalPrice,
                    dpp = dpp,
                    ppn = ppn,
                    ppnBm = ppnBm,
                    disc = disc
                });
            }
            else
            {
                var dpp = poModel.AfterDiscDPP ?? 0;

                decimal ppn = dpp * (ppnPct / 100);
                decimal ppnBm = Math.Floor(dpp * (ppnBmPct / 100));
                decimal totalPrice = dpp + ppn + ppnBm;
                decimal disc = totalPriceBeforeDisc - totalPrice;

                return Json(new
                {
                    dpp = dpp,
                    ppn = ppn,
                    ppnBm = ppnBm,
                    totalPrice = totalPrice,
                    disc = disc
                });
            }
        }

        public JsonResult getDetailColour(string PONo, string salesModelcode, decimal salesModelYear)
        {
            var colour = ctx.OmTrPurchasePOModelColours.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
                && a.PONo == PONo && a.SalesModelCode == salesModelcode && a.SalesModelYear == salesModelYear).ToList()
                .Select(m => new POColourView { 
                ColourCode = m.ColourCode,
                ColourDesc = ctx.MstRefferences.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.RefferenceType == "COLO" && e.RefferenceCode == m.ColourCode && e.Status == "1").RefferenceDesc1,
                Quantity = m.Quantity,
                Remark = m.Remark
                });

            return Json(colour);
        }

        
    }
}
