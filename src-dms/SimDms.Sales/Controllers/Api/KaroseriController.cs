using SimDms.Sales.BLL;
using SimDms.Sales.Models;
//using SimDms.Sales.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using System.Transactions;

namespace SimDms.Sales.Controllers.Api
{
    public class KaroseriController : BaseController
    {
        public JsonResult GetPurchaseKaroseri(string KaroseriSPKNo)
        {
            var record = ctx.omTrPurchaseKaroseri.Find(CompanyCode, BranchCode, KaroseriSPKNo);

            return Json(new { success = true, data = record });
        }

        public JsonResult KaroseriDetailLoad(string KaroseriSPKNo)
        {
            var query = string.Format(@"
                SELECT a.KaroseriSPKNo,a.ChassisCode, Convert(varchar,a.ChassisNo) AS ChassisNo,
            a.EngineCode,Convert(varchar,a.EngineNo) AS EngineNo,ColourCodeOld,b.RefferenceDesc1 as ColourOld,
            a.ColourCodeNew,c.RefferenceDesc1 as ColourNew,a.Remark
            FROM omTrPurchaseKaroseriDetail a
            LEFT JOIN omMstRefference b
            ON a.CompanyCode = b.CompanyCode
            AND a.ColourCodeOld =  b.RefferenceCode
            AND b.RefferenceType = 'COLO'
            LEFT JOIN omMstRefference c
            ON a.CompanyCode = c.CompanyCode
            AND a.ColourCodeNew =  c.RefferenceCode
            AND c.RefferenceType = 'COLO'
            WHERE a.CompanyCode = '{0}'
            AND a.BranchCode = '{1}'
            AND a.KaroseriSPKNo = '{2}'
            ORDER BY a.ChassisNo ASC
                       ", CompanyCode, BranchCode, KaroseriSPKNo);
            return Json(ctx.Database.SqlQuery<KaroseriDetailView>(query).AsQueryable());
        }

        public JsonResult printKaroseri(omTrPurchaseKaroseri model)
        {
            var me = ctx.omTrPurchaseKaroseri.Find(CompanyCode, BranchCode, model.KaroseriSPKNo);
            var data = new omTrPurchaseKaroseri();
            if (me != null)
            {
                var meBPU = ctx.omTrPurchaseKaroseriDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.KaroseriSPKNo == model.KaroseriSPKNo).FirstOrDefault();

                var meDSM = ctx.omTrPurchaseKaroseriDetail
                    .Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.KaroseriSPKNo == model.KaroseriSPKNo && p.ChassisCode == meBPU.ChassisCode)
                    .FirstOrDefault();
                if (meDSM != null)
                {
                    me.Status = "1";
                    ctx.SaveChanges();
                    data = ctx.omTrPurchaseKaroseri.Find(CompanyCode, BranchCode, model.KaroseriSPKNo);
                    return Json(new { success = true, data = data });
                }
                else
                {
                    return Json(new { success = false, message = meBPU.KaroseriSPKNo + " : do not have table detail model in HPP!" });
                }
            }
            else
            {
                return Json(new { success = false, data = data });
            }

        }

        public JsonResult updateHdr(omTrPurchaseKaroseri model)
        {
            var me = ctx.omTrPurchaseKaroseri.Find(CompanyCode, BranchCode, model.KaroseriSPKNo);
            var data = new omTrPurchaseKaroseri();
            if (me != null)
            {
                var meBPU = ctx.omTrPurchaseKaroseriDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.KaroseriSPKNo == model.KaroseriSPKNo).FirstOrDefault();
                

                if (meBPU != null && me.Status == "0")
                {
                    var meDSM = ctx.omTrPurchaseKaroseriDetail
                        .Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.KaroseriSPKNo == model.KaroseriSPKNo && p.ChassisCode == meBPU.ChassisCode)
                        .FirstOrDefault();
                    if (meDSM != null)
                    {
                        me.Status = "1";
                        
                        ctx.SaveChanges();
                        data = ctx.omTrPurchaseKaroseri.Find(CompanyCode, BranchCode, model.KaroseriSPKNo);
                        return Json(new { success = true, data = data }); 
                    }
                    else
                    {
                        return Json(new { success = false, message = meBPU.KaroseriSPKNo + " : do not have table detail model in HPP!" });
                    }
                }
                else if (meBPU != null && me.Status == "1")
                {
                    me.Status = "2";
                    ctx.SaveChanges();
                    data = ctx.omTrPurchaseKaroseri.Find(CompanyCode, BranchCode, model.KaroseriSPKNo);

                    var omMstVehicle = ctx.OmMstVehicles.Find(CompanyCode, meBPU.ChassisCode, meBPU.ChassisNo);
                    omMstVehicle.Status = "1";
                    omMstVehicle.KaroseriSPKNo = model.KaroseriSPKNo;
                    ctx.SaveChanges();

                    return Json(new { success = true, data = data }); 
                }
                else
                {
                    return Json(new { success = false, message = "You must fill table detail!" });
                }
                
            }
            else { 
                return Json(new { success = false, data = data }); 
            }

        }

        [HttpPost]
        public JsonResult Save(omTrPurchaseKaroseri model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.omTrPurchaseKaroseri.Find(companyCode, BranchCode, model.KaroseriSPKNo);

            if (me == null)
            {
                me = new omTrPurchaseKaroseri();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                var KaroseriSPKNo = GetNewDocumentNo("KRI", model.KaroseriSPKDate.Value);
                me.KaroseriSPKNo = KaroseriSPKNo;
                ctx.omTrPurchaseKaroseri.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }

            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.KaroseriSPKDate = model.KaroseriSPKDate;
            me.RefferenceNo = model.RefferenceNo;
            me.RefferenceDate = model.RefferenceDate;
            me.SupplierCode = model.SupplierCode;
            me.SalesModelCodeOld = model.SalesModelCodeOld;
            me.SalesModelYear = model.SalesModelYear;
            me.SalesModelCodeNew = model.SalesModelCodeNew;
            me.ChassisCode = model.ChassisCode;
            me.Quantity = model.Quantity;
            me.DPPMaterial = model.DPPMaterial;
            me.DPPFee = model.DPPFee;
            me.DPPOthers = model.DPPOthers;
            me.PPn = model.PPn;
            me.Total = model.Total;
            me.DurationDays = model.DurationDays;
            me.Remark = model.Remark;
            me.Quantity = 0;
            me.Status = "0";
            me.Remark = "";
            me.isLocked = false;
            me.LockingBy = "";
            me.LockingDate = new DateTime(1900, 1, 1);
            me.WarehouseCode = model.WarehouseCode;

            try
            {
                Helpers.ReplaceNullable(me);
                ctx.SaveChanges();
                return Json(new { success = true, data = me });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete(omTrPurchaseKaroseri model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.omTrPurchaseKaroseri.Find(companyCode, BranchCode, model.KaroseriSPKNo);
                    if (me != null)
                    {
                        ctx.omTrPurchaseKaroseri.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Karoseri berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Karoseri, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete HPP, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        [HttpPost]
        public JsonResult Save2(omTrPurchaseKaroseriDetail model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;
            try
            {
                var me = ctx.omTrPurchaseKaroseriDetail.Find(CompanyCode, BranchCode, model.KaroseriSPKNo, model.ChassisCode, model.ChassisNo);

                if (me == null)
                {
                    me = new omTrPurchaseKaroseriDetail();
                    me.CreatedDate = currentTime;
                    me.LastUpdateDate = currentTime;
                    me.CreatedBy = userID;
                    ctx.omTrPurchaseKaroseriDetail.Add(me);
                }
                else
                {
                    me.LastUpdateDate = currentTime;
                    me.LastUpdateBy = userID;
                }
                me.CompanyCode= CompanyCode;                me.BranchCode= BranchCode;                me.KaroseriSPKNo= model.KaroseriSPKNo;                me.ChassisCode= model.ChassisCode;                me.ChassisNo= model.ChassisNo;                me.EngineCode= model.EngineCode;                me.EngineNo= model.EngineNo;                me.ColourCodeOld= model.ColourCodeOld;                me.ColourCodeNew= model.ColourCodeNew;                me.Remark= model.Remark;                me.isKaroseriTerima= model.isKaroseriTerima;

                Helpers.ReplaceNullable(me);
                ctx.SaveChanges();

                var record = ctx. omTrPurchaseKaroseri.Find(CompanyCode, BranchCode, model.KaroseriSPKNo);
                if (record != null)
                {
                    record.Status = "0";
                    int intDetail = ctx.omTrPurchaseKaroseriDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.KaroseriSPKNo == model.KaroseriSPKNo).Count();

                    record.Quantity = Convert.ToDecimal(intDetail);

                    ctx.SaveChanges();
                }

                var varColorNew = string.Format(@"select RefferenceDesc1 from omMstRefference where RefferenceType = 'COLO' and RefferenceCode = '{0}'", me.ColourCodeNew);
                var colorNew = ctx.Database.SqlQuery<string>(varColorNew);
               
                return Json(new { success = true, data = me, colorNew = colorNew });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete2(omTrPurchaseKaroseriDetail model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                   var me = ctx.omTrPurchaseKaroseriDetail.Find(CompanyCode, BranchCode, model.KaroseriSPKNo, model.ChassisCode, model.ChassisNo);
                    if (me != null)
                    {
                        ctx.omTrPurchaseKaroseriDetail.Remove(me);
                        ctx.SaveChanges();

                        var record = ctx.omTrPurchaseKaroseri.Find(CompanyCode, BranchCode, model.KaroseriSPKNo);
                        if (record != null)
                        {
                            record.Status = "0";

                            record.Quantity = record.Quantity - 1;

                            ctx.SaveChanges();
                        }

                        returnObj = new { success = true, message = "Data Karoseri Detail berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Karoseri Detail, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Karoseri Detail, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        public JsonResult ReCalculateDPPnPPN(omTrPurchaseKaroseri model)
        {
            var oGnMstSupplierProfitCenter = ctx.SupplierProfitCenter.Find(CompanyCode, BranchCode, model.SupplierCode, "100");
            string ptPPn = (oGnMstSupplierProfitCenter == null) ? "NON" : oGnMstSupplierProfitCenter.TaxCode;
            var oGnMstTax = ctx.Taxs.Find(CompanyCode, ptPPn);
            decimal pctPPn = (oGnMstTax == null) ? 0 : oGnMstTax.TaxPct.Value;
            model.DPPMaterial = Math.Round(model.Total.Value / ((100 + pctPPn) / 100), MidpointRounding.AwayFromZero);
            decimal tPPn = Math.Floor(model.DPPMaterial.Value * (pctPPn / 100));
            model.PPn = model.Total.Value - model.DPPMaterial.Value;
            return Json(new { success = true, data = model }); 
        }

        public JsonResult CheckValidasiHolding()
        {
            var dataHolding = ctx.OrganizationDtls.Where(p => p.CompanyCode == CompanyCode && p.IsBranch == false);
            
            if (dataHolding.Count() == 1)
            {
                if (dataHolding.FirstOrDefault().BranchCode != BranchCode)
                {
                    return Json(new { success = false, message = "User bukan termasuk user Holding!" });
                }
            }

            return Json(new { success = true, isUserHolding = false }); 
        }
    }
}
