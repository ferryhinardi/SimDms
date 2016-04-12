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

namespace SimDms.Sparepart.Controllers.Api
{
    public class ReceivingClaimVendorController : BaseController
    {
        public string getData()
        {
            return GetNewDocumentNoHpp("CLR", DateTime.Now.ToString("yyyyMMdd"));
        }

        public JsonResult Save(SpTrnPRcvClaimHdr model)
        {
            var record = ctx.SpTrnPRcvClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimReceivedNo, model.ClaimNo);
            var Recv = ctx.spTrnPClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimNo);
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (record == null)
                    {
                        record = new SpTrnPRcvClaimHdr
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            ClaimReceivedNo = getData(),
                            ClaimNo = model.ClaimNo,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                            TypeOfGoods = TypeOfGoods

                        };
                        ctx.SpTrnPRcvClaimHdrs.Add(record);
                    }
                    var clmdate = Convert.ToDateTime(Recv.ClaimDate);
                    record.ClaimReceivedDate = model.ClaimReceivedDate;
                    record.ReferenceNo = Recv.ReferenceNo;
                    record.ReferenceDate = Recv.ReferenceDate;
                    record.SupplierCode = Recv.SupplierCode;
                    record.ClaimDate = clmdate;
                    record.Status = "0";
                    record.PrintSeq = 0;
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
                    
                    ctx.SaveChanges();
                    trans.Commit();

                    return Json(new { success = true, clm = record.ClaimReceivedNo });
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        public JsonResult SaveDetail(SpTrnPRcvClaimDtl model)
        {
            var record = ctx.SpTrnPRcvClaimDtls.Find(CompanyCode, BranchCode, model.ClaimReceivedNo, model.ClaimNo, model.PartNo, model.DocNo);
            var recordSpTrnPClaimDtl = ctx.SpTrnPClaimDtls.Find(CompanyCode, BranchCode, model.ClaimNo, model.PartNo, model.DocNo);
            if (record == null)
            {
                record = new SpTrnPRcvClaimDtl
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ClaimReceivedNo = model.ClaimReceivedNo,
                    ClaimNo = model.ClaimNo,
                    PartNo = model.PartNo,
                    DocNo = model.DocNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now

                };
                ctx.SpTrnPRcvClaimDtls.Add(record);
            }
            if (model.PartNoWrong == null) {
                model.PartNoWrong = "";
            }
            record.PartNoWrong = model.PartNoWrong;
            record.TotRcvClaimQty = (model.RcvDamageQty + model.RcvOvertageQty + model.RcvShortageQty + model.RcvWrongQty);
            record.RcvDamageQty = model.RcvDamageQty;
            record.RcvOvertageQty = model.RcvOvertageQty;
            record.RcvShortageQty = model.RcvShortageQty;
            record.RcvWrongQty = model.RcvWrongQty;
            record.ClaimType = model.ClaimType;
            record.DocDate = recordSpTrnPClaimDtl.DocDate;
            record.ABCClass = recordSpTrnPClaimDtl.ABCClass;
            record.CostPrice = recordSpTrnPClaimDtl.CostPrice;
            record.MovingCode = recordSpTrnPClaimDtl.MovingCode;
            record.PartCategory = recordSpTrnPClaimDtl.PartCategory;
            record.ProductType = recordSpTrnPClaimDtl.ProductType;
            record.PurchasePrice = recordSpTrnPClaimDtl.PurchasePrice;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.Status = "0";

            try
            {
                ctx.SaveChanges();
                var datadetail = ctx.SpTrnPRcvClaimDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimReceivedNo == model.ClaimReceivedNo);
                return Json(new { success = true, data = datadetail });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult deleteData(SpTrnPRcvClaimHdr model)
        {
            var record = ctx.SpTrnPRcvClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimReceivedNo, model.ClaimNo);
            if (record != null) {
                record.Status = "3";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
            }
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult delete(SpTrnPRcvClaimDtl model)
        {
            var record = ctx.SpTrnPRcvClaimDtls.Find(CompanyCode, BranchCode, model.ClaimReceivedNo, model.ClaimNo, model.PartNo, model.DocNo);
            if (record != null)
            {
                ctx.SpTrnPRcvClaimDtls.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                var datadetail = ctx.SpTrnPRcvClaimDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimReceivedNo == model.ClaimReceivedNo);
                return Json(new { success = true, data = datadetail });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Print(SpTrnPRcvClaimHdr model)
        {
            var record = ctx.SpTrnPRcvClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimReceivedNo, model.ClaimNo);
            if (record != null)
            {
                record.Status = "1";
                record.PrintSeq += 1;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
            }
            try
            {
                ctx.SaveChanges();
                var detail = ctx.SpTrnPRcvClaimDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimReceivedNo == model.ClaimReceivedNo);
                return Json(new { success = true, isi = record, datdet = detail});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }


        }

        public JsonResult CloseClaim(SpTrnPRcvClaimHdr model, SpTrnPRcvClaimDtl model2)
        {
            var dtDetails = ctx.SpTrnPRcvClaimDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimReceivedNo == model.ClaimReceivedNo);
            var recordHdr = ctx.SpTrnPRcvClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimReceivedNo, model.ClaimNo);

            var recordItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, model2.PartNo, "00");
            var recordItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, model2.PartNo);
            var recordItem = ctx.spMstItems.Find(CompanyCode, BranchCode, model2.PartNo);
            var grid = dataGridDetail(model.ClaimReceivedNo);
            var psn = "";
            if (grid == null) {
                psn = "Tidak ada data detail";
                return Json(new { success = false, pesan = psn });
            }
            else if (recordHdr == null || recordHdr.Status == "2")
            {
                psn = "Record sudah pernah di-posting";
                return Json(new { success = false, pesan = psn });
            }
            else {
                psn = "Record berhasil di close";
            }

            if (model2.RcvDamageQty > 0 || model2.RcvShortageQty > 0 || model2.RcvWrongQty > 0)
            {
                if (recordHdr != null)
                {
                    var recWHH = ctx.spTrnIWHTrfHdrs.Find(CompanyCode, BranchCode, "");
                    if (recWHH == null)
                    {
                        recWHH = new spTrnIWHTrfHdr
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            WHTrfNo = string.Empty,

                        };
                        ctx.spTrnIWHTrfHdrs.Add(recWHH);
                        
                    }
                    recWHH.WHTrfDate = DateTime.Now;
                    recWHH.ReferenceNo = recordHdr.ClaimReceivedNo;
                    recWHH.ReferenceDate = recordHdr.ClaimReceivedDate;
                    recWHH.TypeOfGoods = TypeOfGoods;
                    recWHH.Status = "2";
                    recWHH.CreatedBy = CurrentUser.UserId;
                    recWHH.CreatedDate = DateTime.Now;
                    recWHH.LastUpdateBy = CurrentUser.UserId;
                    recWHH.LastUpdateDate = DateTime.Now;
                    
                }
            }

            recordHdr.Status = "2";
            recordHdr.LastUpdateBy = CurrentUser.UserId;
            recordHdr.LastUpdateDate = DateTime.Now;
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, pesan = psn });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }


        }

        public JsonResult CancelClaim(SpTrnPRcvClaimHdr model, SpTrnPRcvClaimDtl model2) 
        {
            var dtDetails = ctx.SpTrnPRcvClaimDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimReceivedNo == model.ClaimReceivedNo);
            var recordHdr = ctx.SpTrnPRcvClaimHdrs.Find(CompanyCode, BranchCode, model.ClaimReceivedNo, model.ClaimNo);
            bool result = true;
            var recordItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, model2.PartNo);
            var psn = "";
            if (recordHdr == null || recordHdr.Status == "2") {
                psn = "Record sudah pernah di-posting";
                
                return Json(new { success = false, pesan = psn });
                result = false;
            }
            else if (recordHdr == null || recordHdr.Status == "8")
            {
                psn = "Record sudah pernah di-cancel";

                return Json(new { success = false, pesan = psn });
                result = false;
            }
            else {
                psn = "Record berhasil di cancel";
            }

            if (result) {
                //decimal totAmaountClaim = 0;
                if (dtDetails == null) {
                    return Json(false);
                }
                if (recordItemPrice != null) {
                 //    totAmaountClaim += dtDetails.PurchasePrice * (dtDetails.RcvDamageQty + dtDetails.RcvOvertageQty + dtDetails.RcvShortageQty + dtDetails.RcvWrongQty);
                }

                model2.CostPrice = (recordItemPrice != null) ? recordItemPrice.CostPrice : 0;
                model2.LastUpdateBy = CurrentUser.UserId;
                model2.LastUpdateDate = DateTime.Now;
                
                //ctx.SpTrnPRcvClaimDtls.Add(dtDetails);
            }

            recordHdr.LastUpdateBy = CurrentUser.UserId;
            recordHdr.LastUpdateDate = DateTime.Now;
            recordHdr.Status = "8";
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, pesan = psn });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult dataGridDetail(string ClaimReceivedNo)
        {
            var record = ctx.SpTrnPRcvClaimDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimReceivedNo == ClaimReceivedNo);
            return Json(record);
        }

        public JsonResult PartOrderDetail(string ClaimNo, string PartNo)
        {
            var record = ctx.SpTrnPClaimDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ClaimNo == ClaimNo && a.PartNo == PartNo).FirstOrDefault();
            return Json(record);
        }

        
        

    }
}
