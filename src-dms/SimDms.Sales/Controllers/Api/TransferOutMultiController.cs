using SimDms.Sales.BLL;
using SimDms.Sales.Models;
//using SimDms.Sales.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using System.Data;
using SimDms.Sales.Models.Result;

namespace SimDms.Sales.Controllers.Api
{
    public class TransferOutMultiController : BaseController  
    {
        public JsonResult DetailLoad(string TransferOutNo)
        {
            var query = string.Format(@"
                 SELECT 
                Convert(varchar,a.TransferOutSeq) as TransferOutSeq,a.SalesModelCode,
                Convert(varchar,a.SalesModelYear) as SalesModelYear,b.SalesModelDesc,
                a.ChassisCode,Convert(varchar,a.ChassisNo) as ChassisNo,a.EngineCode,
                Convert(varchar,a.EngineNo) as EngineNo,a.ColourCode,c.RefferenceDesc1 as ColourName,
                a.Remark
            FROM OmTrInventTransferOutDetailMultiBranch a
            LEFT JOIN omMstModel b ON a.CompanyCode = b.CompanyCode
                AND a.SalesModelCode = b.SalesModelCode
            LEFT JOIN omMstRefference c ON a.CompanyCode = c.CompanyCode
                AND a.ColourCode = c.RefferenceCode
            WHERE c.RefferenceType = 'COLO'
                AND a.CompanyCode = '{0}'
                AND a.BranchCode = '{1}'
                AND a.TransferOutNo = '{2}'
            ORDER BY a.TransferOutSeq, a.SalesModelCode,a.SalesModelYear,a.ChassisNo ASC
                       ", CompanyCode, BranchCode, TransferOutNo);
            return Json(ctx.Database.SqlQuery<TransferInDetailView>(query).AsQueryable());
        }

        public JsonResult Default()
        {
            var me = ctx.CoProfiles.Where(m => m.BranchCode == CurrentUser.BranchCode).FirstOrDefault();
            var my = ctx.LookUpDtls.Where(m => m.ParaValue == CurrentUser.BranchCode && m.CodeID == "MPWH").FirstOrDefault();
            var data = new InquiryTrTransferOutView
            {
                CompanyCode = me.CompanyCode,
                BranchCodeFrom = CurrentUser.BranchCode,
                BranchNameFrom = me.CompanyName,
                WareHouseCodeFrom = my.LookUpValue,
                WareHouseNameFrom = my.LookUpValueName
            };
            return Json(new { success = true, data = data });
        }

        public JsonResult Select4WH(string branchCode) 
        {
            var query = String.Format(@" 
            SELECT LookUpValue , LookUpValueName , SeqNo
                FROM gnMstLookUpDtl
                WHERE CompanyCode = '{0}'
                    AND CodeID = 'MPWH'
                    AND ParaValue= '{1}'
                ORDER BY SeqNo", CompanyCode, branchCode);
            return Json(new{data = ctx.Database.SqlQuery<WHview>(query).AsQueryable().FirstOrDefault()});
        }

        public JsonResult updateHdr(OmTrInventTransferOutMultiBranch model)
        {
            var me = ctx.OmTrInventTransferOutMultiBranch.Find(CompanyCode, BranchCode, model.TransferOutNo);
            var data = new OmTrInventTransferOutMultiBranch();
            if (me != null)
            {
                var meBPU = ctx.omTrInventTransferOutDetailMultiBranch.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.TransferOutNo == model.TransferOutNo).FirstOrDefault();
                

                if (meBPU != null && me.Status == "0")
                {
                    var meDSM = ctx.omTrInventTransferOutDetailMultiBranch
                        .Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.TransferOutNo == model.TransferOutNo)
                        .FirstOrDefault();
                    if (meDSM != null)
                    {
                        me.Status = "1";
                        ctx.SaveChanges();
                        data = ctx.OmTrInventTransferOutMultiBranch.Find(CompanyCode, BranchCode, model.TransferOutNo);
                        return Json(new { success = true, data = data }); 
                    }
                    else
                    {
                        return Json(new { success = false, message = meBPU.TransferOutNo + " : do not have table detail model in HPP!" });
                    }
                }
                else if (meBPU != null && me.Status == "1")
                {
                    me.Status = "2";
                    ctx.SaveChanges();
                    data = ctx.OmTrInventTransferOutMultiBranch.Find(CompanyCode, BranchCode, model.TransferOutNo);
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


        public JsonResult Approve(OmTrInventTransferOutMultiBranch model)
        {
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            DateTime currentTime = DateTime.Now;
            string errMsg ="";
            
            try 
	        {	        
                if(string.IsNullOrEmpty( model.TransferOutNo))
                return Json(new{success=false,message="Nomor Transfer Out tidak boleh kosong"});

                var me = ctx.OmTrInventTransferOutMultiBranch.Find(companyCode, BranchCode, model.TransferOutNo);
            
                if(me==null)
            	return Json(new{success=false,message="Invalid Nomor Transfer Out"});

                if(me.Status!="1")
                    return Json(new{success=false,message="Invalid Status"});

                var dt=ctx.omTrInventTransferOutDetailMultiBranch
                        .Where (x=>x.CompanyCode==CompanyCode &&
                                    x.BranchCode==BranchCode &&
                                    x.TransferOutNo==model.TransferOutNo)
                        .ToList();

                if(dt.Count>0)
                {
                    using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            foreach (var x in dt)
                            {
                                var oOmMstVehicle = ctx.OmMstVehicles
                                                    .Where(v =>
                                                        v.CompanyCode == CompanyCode &&
                                                        v.ChassisCode == x.ChassisCode &&
                                                        v.ChassisNo == x.ChassisNo
                                                     ).FirstOrDefault();

                                if (oOmMstVehicle == null)
                                {
                                    trans.Rollback();
                                    errMsg = "\nUntuk kendaraan dengan ChassisCode= " + oOmMstVehicle.ChassisCode + " dan ChassisNo= " + oOmMstVehicle.ChassisNo.ToString()
                                        + " belum menjadi stock";

                                    return Json(new { success = false, message = errMsg });
                                }

                                decimal cogs = oOmMstVehicle.COGSUnit + oOmMstVehicle.COGSKaroseri
                                                    + oOmMstVehicle.COGSOthers.Value ?? 0;
                                if (cogs <= 0)
                                {
                                    trans.Rollback();
                                    errMsg = "\nUntuk kendaraan dengan Chassis Code= " + oOmMstVehicle.ChassisCode + " dan Chassis No= " + oOmMstVehicle.ChassisNo.ToString()
                                        + " nilai HPP masih 0";
                                    
                                    return Json(new { success = false, message = errMsg });
                                }

                                if (oOmMstVehicle.Status != "0" || oOmMstVehicle.SONo != "" || oOmMstVehicle.DONo != ""
                                    && oOmMstVehicle.BPKNo != "" || oOmMstVehicle.InvoiceNo != "")
                                {
                                    if (oOmMstVehicle.SOReturnNo == "")
                                    {
                                        trans.Rollback();
                                        errMsg = "\nUntuk kendaraan dengan Chassis Code= " + oOmMstVehicle.ChassisCode + " dan Chassis No= " + oOmMstVehicle.ChassisNo.ToString()
                                            + " tidak dalam status Ready";
                                        
                                        return Json(new { success = false, message = errMsg });
                                    }
                                }

                                oOmMstVehicle.Status = "7";
                                oOmMstVehicle.TransferOutMultiBranchNo = x.TransferOutNo.ToString();
                                oOmMstVehicle.LastUpdateDate = ctx.CurrentTime;
                                oOmMstVehicle.LastUpdateBy = CurrentUser.UserId;

                                //result = oOmMstVehicleDao.Update(oOmMstVehicle) >= 0;                     
                                var qtyVehicle = ctx.OmTrInventQtyVehicles
                                    .Where(v => v.CompanyCode == CompanyCode &&
                                             v.BranchCode == BranchCode &&
                                             v.Year == me.TransferOutDate.Value.Year &&
                                             v.Month == me.TransferOutDate.Value.Month &&
                                             v.SalesModelCode == x.SalesModelCode &&
                                             v.SalesModelYear == x.SalesModelYear &&
                                             v.ColourCode == x.ColourCode &&
                                             v.WarehouseCode == me.WareHouseCodeFrom
                                     ).FirstOrDefault();

                                if (qtyVehicle != null)
                                {
                                    qtyVehicle.QtyOut = qtyVehicle.QtyOut + 1;
                                    qtyVehicle.EndingOH = (qtyVehicle.BeginningOH + qtyVehicle.QtyIn) - qtyVehicle.QtyOut;
                                    qtyVehicle.EndingAV = (qtyVehicle.BeginningAV + qtyVehicle.QtyIn) - qtyVehicle.Alocation - qtyVehicle.QtyOut;

                                    qtyVehicle.LastUpdateBy = CurrentUser.UserId;
                                    qtyVehicle.LastUpdateDate = ctx.CurrentTime;
                                }
                            };

                            me.Status = "2";
                            me.LastUpdateBy = CurrentUser.UserId;
                            me.LastUpdateDate = ctx.CurrentTime;

                            ctx.SaveChanges();
                            trans.Commit();
                            
                            return Json(new { success = true, message = "Approved", data = me });
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            return Json(new { success = false, message = ex.Message });
                        }
                    }
                }
                else
                {
                    return Json(new{success=false,message="Transfer Out Tidak Memiliki Detail"});
                }
	        }
	        catch (Exception ex)
	        {
                return Json(new { success = false, message = ex.Message });
	        }
        }

        [HttpPost]
        public JsonResult Save(OmTrInventTransferOutMultiBranch model)
        {
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.OmTrInventTransferOutMultiBranch.Find(companyCode, BranchCode, model.TransferOutNo);
            if (me == null)
            {
                me = new OmTrInventTransferOutMultiBranch();
                me.CompanyCode = companyCode;
                me.BranchCode = branchCode;
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                var TransferOutNo = GetNewDocumentNo("VTO", model.TransferOutDate.Value);
                me.TransferOutNo = TransferOutNo;
                me.Status = "0";
                ctx.OmTrInventTransferOutMultiBranch.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            //me.TransferOutNo = model.TransferOutNo;
            me.TransferOutDate = model.TransferOutDate;
            me.ReferenceNo = model.ReferenceNo;
            me.ReferenceDate = model.ReferenceDate;
            me.CompanyCodeFrom = model.CompanyCodeFrom ?? "";
            me.BranchCodeFrom = model.BranchCodeFrom ?? "";
            me.WareHouseCodeFrom = model.WareHouseCodeFrom ?? "";
            me.CompanyCodeTo = model.CompanyCodeTo ?? "";
            me.BranchCodeTo = model.BranchCodeTo ?? "";
            me.WareHouseCodeTo = model.WareHouseCodeTo ?? "";
            me.ReturnDate = model.ReturnDate;
            me.Remark = model.Remark;
            //me.Status = model.Status;
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

        public JsonResult Delete(OmTrInventTransferOutMultiBranch model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var me = ctx.OmTrInventTransferOutMultiBranch.Find(companyCode, BranchCode, model.TransferOutNo);
                    var meDtl = ctx.omTrInventTransferOutDetailMultiBranch.Where(m => m.CompanyCode == companyCode && m.BranchCode == BranchCode && m.TransferOutNo == model.TransferOutNo).ToArray();
                    if (me != null)
                    {
                        var x = meDtl.Length;
                        for (var i = 0; i < x; i++)
                        {
                            ctx.omTrInventTransferOutDetailMultiBranch.Remove(meDtl[i]);
                            ctx.SaveChanges();
                        }
                        ctx.OmTrInventTransferOutMultiBranch.Remove(me);
                        ctx.SaveChanges();
                        trans.Commit();
                        returnObj = new { success = true, message = "Data Transfer Out berhasil di delete." };
                        trans.Commit();
                    }
                    else
                    {
                        trans.Rollback();
                        returnObj = new { success = false, message = "Error ketika mendelete  Transfer Out, Message=Data tidak ditemukan" };
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnObj = new { success = false, message = "Error ketika mendelete  Transfer Out, Message=" + ex.ToString() };
                }
            }
            
            return Json(returnObj);
        }

        [HttpPost]
        public JsonResult Save2(omTrInventTransferOutDetailMultiBranch model, string WarehouseCode)
        {
            ResultModel result = InitializeResult();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var record = ctx.OmTrInventTransferOutMultiBranch.Find(CompanyCode, BranchCode, model.TransferOutNo);
            if (record != null)
            {
                if (record.Status != "0" && record.Status != "1") {
                    var query = string.Format(@"
                     SELECT 
                        Convert(varchar,a.TransferOutSeq) as TransferOutSeq,a.SalesModelCode,
                        Convert(varchar,a.SalesModelYear) as SalesModelYear,b.SalesModelDesc,
                        a.ChassisCode,Convert(varchar,a.ChassisNo) as ChassisNo,a.EngineCode,
                        Convert(varchar,a.EngineNo) as EngineNo,a.ColourCode,c.RefferenceDesc1 as ColourName,
                        a.Remark
                    FROM OmTrInventTransferOutDetailMultiBranch a
                    LEFT JOIN omMstModel b ON a.CompanyCode = b.CompanyCode
                        AND a.SalesModelCode = b.SalesModelCode
                    LEFT JOIN omMstRefference c ON a.CompanyCode = c.CompanyCode
                        AND a.ColourCode = c.RefferenceCode
                    WHERE c.RefferenceType = 'COLO'
                        AND a.CompanyCode = '{0}'
                        AND a.BranchCode = '{1}'
                        AND a.TransferOutNo = '{2}'
                    ORDER BY a.TransferOutSeq, a.SalesModelCode,a.SalesModelYear,a.ChassisNo ASC
                               ", CompanyCode, BranchCode, model.TransferOutNo);

                    return Json(ctx.Database.SqlQuery<TransferInDetailView>(query).AsQueryable());
                }
            }
            
            var vehicle = ctx.OmTrInventQtyVehicles.Find(CompanyCode, BranchCode
            , Convert.ToDecimal(record.TransferOutDate.Value.Year), Convert.ToDecimal(record.TransferOutDate.Value.Month),
            model.SalesModelCode, Convert.ToDecimal(model.SalesModelYear), model.ColourCode, WarehouseCode);

            if (vehicle == null)
            {
                return Json(new { success = false, message = "Model kendaraan tidak ada di Table InventQty untuk periode ini !" });
            }

            
            var mstVhc = ctx.OmMstVehicles.Find(CompanyCode, model.ChassisCode, model.ChassisNo);
            if (mstVhc == null)
            {
                return Json(new { success = false, message = "Tipe Kendaraan tidak ada di data Master !!!" });
            }

            var me = ctx.omTrInventTransferOutDetailMultiBranch.Find(CompanyCode, BranchCode, model.TransferOutNo, model.TransferOutSeq);
            if (me == null)
            {
                me = new omTrInventTransferOutDetailMultiBranch();
                decimal seqNo = Select4MaxSeq(CompanyCode, BranchCode, model.TransferOutNo);
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                me.TransferOutSeq = seqNo + 1;
                me.StatusTransferIn = "0";
                ctx.omTrInventTransferOutDetailMultiBranch.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.TransferOutNo = model.TransferOutNo;
            //me.TransferOutSeq = model.TransferOutSeq;
            me.SalesModelCode = model.SalesModelCode;
            me.SalesModelYear = model.SalesModelYear;
            me.ChassisCode = model.ChassisCode;
            me.ChassisNo = model.ChassisNo;
            me.EngineCode = model.EngineCode;
            me.EngineNo = model.EngineNo;
            me.ColourCode = model.ColourCode;
            me.COGSUnit = mstVhc.COGSUnit;
            me.COGSOthers = mstVhc.COGSOthers;
            me.COGSKaroseri = mstVhc.COGSKaroseri;
            me.Remark = model.Remark;
            //me.StatusTransferIn = model.StatusTransferIn;
            try
            {
                Helpers.ReplaceNullable(me);
                ctx.SaveChanges();
                var my = ctx.OmMstModels.Where(p => p.SalesModelCode == model.SalesModelCode).FirstOrDefault();
                var mi = ctx.MstRefferences.Where(p => p.RefferenceCode == model.ColourCode).FirstOrDefault();
                result.status = true; 
                result.message = "Data Kode Pos berhasil disimpan.";
                result.data = new
                {
                    SalesModelCode = me.SalesModelCode,
                    SalesModelYear = me.SalesModelYear,
                    SalesModelDesc = my.SalesModelDesc,
                    ChassisCode = me.ChassisCode,
                    ChassisNo = me.ChassisNo,
                    EngineCode = me.EngineCode,
                    EngineNo = me.EngineNo,
                    ColourCode = me.ColourCode,
                    ColourName = mi.RefferenceDesc1,
                    Remark = me.Remark
                };
                return Json(result);
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete2(omTrInventTransferOutDetailMultiBranch model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var me = ctx.omTrInventTransferOutDetailMultiBranch.Find(CompanyCode, BranchCode, model.TransferOutNo, model.TransferOutSeq);
                    if (me != null)
                    {
                        ctx.omTrInventTransferOutDetailMultiBranch.Remove(me);
                        ctx.SaveChanges();
                        trans.Commit();
                        returnObj = new { success = true, message = "Data Transfer Out Detail berhasil di delete." };
                    }
                    else
                    {
                        trans.Rollback();
                        returnObj = new { success = false, message = "Error ketika mendelete Transfer Out Detail, Message=Data tidak ditemukan" };
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnObj = new { success = false, message = "Error ketika mendelete Transfer Out Detail, Message=" + ex.ToString() };
                }
            }

            return Json(returnObj);
        }

        protected decimal Select4MaxSeq(string CompanyCode, string BranchCode, string pNo)
        {
            var query =  String.Format(@"
            SELECT isnull(max(TransferOutSeq),0) FROM omTrInventTransferOutDetailMultiBranch
            WHERE CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND TransferOutNo = '{2}'", CompanyCode, BranchCode, pNo);
            var queryable = ctx.Database.SqlQuery<decimal>(query).FirstOrDefault();
            return (queryable);
        }
    }
}
