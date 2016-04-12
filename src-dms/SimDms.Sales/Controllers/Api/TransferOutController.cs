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
using SimDms.Sales.Models.Result;

namespace SimDms.Sales.Controllers.Api
{
    public class TransferOutController : BaseController
    {
        private string errMsg = "";

        public JsonResult TransferOutDetailLoad(string TransferOutNo)
        {
            //            var query = string.Format(@"
            //                 select Convert(varchar, a.TransferOutSeq) as TransferOutSeq, a.SalesModelCode, Convert(varchar,a.SalesModelYear) as SalesModelYear, 
            //            a.ChassisCode, Convert(varchar,a.ChassisNo) as ChassisNo , a.EngineCode, Convert(varchar,a.EngineNo ) as EngineNo, c.SalesModelDesc, 
            //            e.RefferenceDesc1 as ColourName, a.ColourCode, a.Remark
            //            from omTrInventTransferOutDetail a
            //            LEFT JOIN omMstModel c
            //            ON a.CompanyCode = c.CompanyCode
            //            AND a.SalesModelCode = c.SalesModelCode
            //            left join omMstRefference e
            //			ON a.CompanyCode = e.CompanyCode
            //            AND a.ColourCode = e.RefferenceCode
            //            where a.CompanyCode = '{0}'
            //            and a.BranchCode = '{1}'
            //            and a.TransferOutNo = '{2}'
            //                       ", CompanyCode, BranchCode, TransferOutNo);
            return Json(GetGrid(TransferOutNo));
        }

        public IEnumerable<TransferInDetailView> GetGrid(string TransferOutNo)
        {
            var query = string.Format(@"
                 select Convert(varchar, a.TransferOutSeq) as TransferOutSeq, a.SalesModelCode, Convert(varchar,a.SalesModelYear) as SalesModelYear, 
            a.ChassisCode, Convert(varchar,a.ChassisNo) as ChassisNo , a.EngineCode, Convert(varchar,a.EngineNo ) as EngineNo, c.SalesModelDesc, 
            e.RefferenceDesc1 as ColourName, a.ColourCode, a.Remark
            from omTrInventTransferOutDetail a
            LEFT JOIN omMstModel c
            ON a.CompanyCode = c.CompanyCode
            AND a.SalesModelCode = c.SalesModelCode
            left join omMstRefference e
			ON a.CompanyCode = e.CompanyCode
            AND a.ColourCode = e.RefferenceCode
            where a.CompanyCode = '{0}'
            and a.BranchCode = '{1}'
            and a.TransferOutNo = '{2}'
                       ", CompanyCode, BranchCode, TransferOutNo);
            var data = ctx.Database.SqlQuery<TransferInDetailView>(query);

            return data;
        }

        public JsonResult TransferOutDefault()
        {
            var me = ctx.CoProfiles.Where(m => m.BranchCode == CurrentUser.BranchCode).FirstOrDefault();
            var my = ctx.LookUpDtls.Where(m => m.ParaValue == CurrentUser.BranchCode && m.CodeID == "MPWH").OrderBy(x=>x.SeqNo).FirstOrDefault();
            var data = new InquiryTrTransferOutView
            {
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
            return Json(new { data = ctx.Database.SqlQuery<WHview>(query).AsQueryable().FirstOrDefault() });
        }

        public JsonResult updateHdr(OmTrInventTransferOut model)
        {
            var me = ctx.OmTrInventTransferOut.Find(CompanyCode, BranchCode, model.TransferOutNo);
            var data = new OmTrInventTransferOut();
            if (me != null)
            {
                var meBPU = ctx.omTrInventTransferOutDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.TransferOutNo == model.TransferOutNo).FirstOrDefault();

                if (meBPU != null && me.Status == "0")
                {
                    var meDSM = ctx.omTrInventTransferOutDetail
                        .Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.TransferOutNo == model.TransferOutNo)
                        .FirstOrDefault();
                    if (meDSM != null)
                    {
                        me.Status = "1";
                        ctx.SaveChanges();
                        data = ctx.OmTrInventTransferOut.Find(CompanyCode, BranchCode, model.TransferOutNo);
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
                    me.LastUpdateBy = CurrentUser.UserId;
                    me.LastUpdateDate = DateTime.Now;

                    var result = Approve(me);
                    if (result)
                    {
                        data = ctx.OmTrInventTransferOut.Find(CompanyCode, BranchCode, model.TransferOutNo);
                        return Json(new { success = result, data = data });
                    }
                    else
                    {
                        return Json(new { success = result, message = errMsg });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Data detail belum di-isi" });
                }

            }
            else
            {
                return Json(new { success = false, data = data });
            }

        }
        public JsonResult Print(OmTrInventTransferOut model)
        {
            var hdr = ctx.OmTrInventTransferOut.Find(CompanyCode, BranchCode, model.TransferOutNo);
            if (hdr != null)
            {
                if (hdr.Status == "0")
                {
                    hdr.Status = "1";
                    ctx.SaveChanges();
                    return Json(new { success = true, data = hdr });
                }
                else
                {
                    return Json(new { success = true, data = hdr });
                }
            }
            else
            {
                return Json(new { success = false, message = "Tidak ada Data untuk diprint." });
            }
        }

        [HttpPost]
        public JsonResult Save(OmTrInventTransferOut model)
        {
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.OmTrInventTransferOut.Find(companyCode, BranchCode, model.TransferOutNo);
            try
            {
                if (me == null)
                {
                    me = new OmTrInventTransferOut();
                    me.CreatedDate = currentTime;
                    me.LastUpdateDate = currentTime;
                    me.CreatedBy = userID;
                    var TransferOutNo = GetNewDocumentNo("VTO", model.TransferOutDate.Value);
                    me.TransferOutNo = TransferOutNo;
                    me.Status = "0";
                    ctx.OmTrInventTransferOut.Add(me);
                }
                else
                {
                    me.LastUpdateDate = currentTime;
                    me.LastUpdateBy = userID;
                }
                me.CompanyCode = CompanyCode;
                me.BranchCode = BranchCode;
                me.TransferOutDate = model.TransferOutDate;
                me.ReferenceNo = model.ReferenceNo;
                me.ReferenceDate = model.ReferenceDate != null ? model.ReferenceDate : Convert.ToDateTime("1900/01/01");
                me.BranchCodeFrom = model.BranchCodeFrom;
                me.WareHouseCodeFrom = model.WareHouseCodeFrom;
                me.BranchCodeTo = model.BranchCodeTo;
                me.WareHouseCodeTo = model.WareHouseCodeTo;
                me.ReturnDate = model.ReturnDate != null ? model.ReturnDate : Convert.ToDateTime("1900/01/01");
                me.Remark = model.Remark;

                Helpers.ReplaceNullable(me);

                ctx.SaveChanges();
                return Json(new { success = true, data = me });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete(OmTrInventTransferOut model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var me = ctx.OmTrInventTransferOut.Find(companyCode, BranchCode, model.TransferOutNo);
                    var meDtl = ctx.omTrInventTransferOutDetail.Where(m => m.CompanyCode == companyCode && m.BranchCode == BranchCode && m.TransferOutNo == model.TransferOutNo).ToArray();
                    if (me != null)
                    {
                        var x = meDtl.Length;
                        for (var i = 0; i < x; i++)
                        {
                            ctx.omTrInventTransferOutDetail.Remove(meDtl[i]);
                            ctx.SaveChanges();
                        }
                        ctx.OmTrInventTransferOut.Remove(me);
                        ctx.SaveChanges();
                        trans.Commit();

                        returnObj = new { success = true, message = "Data Transfer Out berhasil di delete." };
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Data Transfer Out yang akan di Hapus tidak ditemukan." };
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnObj = new { success = false, message = "Error ketika mendelete  Transfer Out, Message Exception : " + ex.ToString() };
                }
            }
            return Json(returnObj);
        }

        [HttpPost]
        public JsonResult Save2(omTrInventTransferOutDetail model)
        {
            ResultModel result = InitializeResult();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.omTrInventTransferOutDetail.Find(CompanyCode, BranchCode, model.TransferOutNo, model.TransferOutSeq);

            if (me == null)
            {
                me = new omTrInventTransferOutDetail();
                decimal seqNo = Select4MaxSeq(CompanyCode, BranchCode, model.TransferOutNo);
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                me.TransferOutSeq = seqNo + 1;
                me.StatusTransferIn = "0";
                ctx.omTrInventTransferOutDetail.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.TransferOutNo = model.TransferOutNo;
            me.SalesModelCode = model.SalesModelCode;
            me.SalesModelYear = model.SalesModelYear;
            me.ChassisCode = model.ChassisCode;
            me.ChassisNo = model.ChassisNo;
            me.EngineCode = model.EngineCode;
            me.EngineNo = model.EngineNo;
            me.ColourCode = model.ColourCode;
            me.Remark = model.Remark;

            try
            {
                Helpers.ReplaceNullable(me);

                ctx.SaveChanges();
                var my = ctx.OmMstModels.Where(p => p.SalesModelCode == model.SalesModelCode).FirstOrDefault();
                //var mi = ctx.MstRefferences.Where(p => p.RefferenceCode == model.ColourCode).FirstOrDefault();
                result.status = true;
                result.message = "Data Kode Pos berhasil disimpan.";
                //result.data = new
                //{
                //    SalesModelCode = me.SalesModelCode,
                //    SalesModelYear = me.SalesModelYear,
                //    SalesModelDesc = my.SalesModelDesc,
                //    ChassisCode = me.ChassisCode,
                //    ChassisNo = me.ChassisNo,
                //    EngineCode = me.EngineCode,
                //    EngineNo = me.EngineNo,
                //    ColourCode = me.ColourCode,
                //    ColourName = mi.RefferenceDesc1,
                //    Remark = me.Remark
                //};
                result.data = GetGrid(model.TransferOutNo);
                return Json(result);
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete2(omTrInventTransferOutDetail model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var me = ctx.omTrInventTransferOutDetail.Find(CompanyCode, BranchCode, model.TransferOutNo, model.TransferOutSeq);
                    if (me != null)
                    {
                        ctx.omTrInventTransferOutDetail.Remove(me);
                        ctx.SaveChanges();
                        trans.Commit();
                        
                        returnObj = new { success = true, message = "Data Transfer Out Detail berhasil di delete." };
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Data Transfer Out yang akan dihapus tidak ditemukan." };
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
            var query = String.Format(@"
            SELECT isnull(max(TransferOutSeq),0) FROM OmTrInventTransferOutDetail
            WHERE CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND TransferOutNo = '{2}'", CompanyCode, BranchCode, pNo);
            var queryable = ctx.Database.SqlQuery<decimal>(query).FirstOrDefault();
            return (queryable);
        }

        public bool Approve(OmTrInventTransferOut record)
        {
            bool result = false;
            string companyCode = record.CompanyCode;
            string branchCode = record.BranchCode;
            string transferOutNo = record.TransferOutNo;
            string warehouseCode = record.WareHouseCodeFrom;
            DateTime transferOutDate = record.TransferOutDate.Value;

            using (var TransApprove = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var dtTransferOutVehicle = from a in ctx.omTrInventTransferOutDetail
                                               join b in ctx.OmTrInventTransferOut
                                               on new { a.CompanyCode, a.BranchCode, a.TransferOutNo }
                                               equals new { b.CompanyCode, b.BranchCode, b.TransferOutNo }
                                               where a.CompanyCode == companyCode
                                               && a.BranchCode == branchCode
                                               && a.TransferOutNo == transferOutNo
                                               select new
                                               {
                                                   a.ChassisCode,
                                                   a.ChassisNo,
                                                   a.SalesModelCode,
                                                   a.SalesModelYear,
                                                   a.ColourCode
                                               };


                    if (dtTransferOutVehicle.Count() > 0)
                    {
                        foreach (var row in dtTransferOutVehicle.ToList())
                        {
                            OmMstVehicle oOmMstVehicle = ctx.OmMstVehicles.Find(CompanyCode, row.ChassisCode, row.ChassisNo);

                            if (oOmMstVehicle == null)
                            {
                                errMsg = "\nUntuk kendaraan dengan ChassisCode= " + oOmMstVehicle.ChassisCode + " dan ChassisNo= " + oOmMstVehicle.ChassisNo.ToString()
                                    + " belum menjadi stock";
                                return result = false;
                            }

                            decimal cogs = oOmMstVehicle.COGSUnit.Value + oOmMstVehicle.COGSKaroseri.Value + oOmMstVehicle.COGSOthers.Value;
                            if (cogs <= 0)
                            {
                                errMsg = "\nUntuk kendaraan dengan ChassisCode= " + oOmMstVehicle.ChassisCode + " dan ChassisNo= " + oOmMstVehicle.ChassisNo.ToString()
                                    + " nilai HPP masih 0";
                                return result = false;
                            }

                            if (oOmMstVehicle.Status != "0" || oOmMstVehicle.SONo != "" || oOmMstVehicle.DONo != ""
                                && oOmMstVehicle.BPKNo != "" || oOmMstVehicle.InvoiceNo != "")
                            {
                                if (oOmMstVehicle.SOReturnNo == "")
                                {
                                    errMsg = "\nUntuk kendaraan dengan ChassisCode= " + oOmMstVehicle.ChassisCode + " dan ChassisNo= " + oOmMstVehicle.ChassisNo.ToString()
                                        + " tidak dalam status Ready";
                                    return result = false;
                                }
                            }

                            oOmMstVehicle.Status = "7";
                            oOmMstVehicle.TransferOutNo = record.TransferOutNo;
                            oOmMstVehicle.LastUpdateDate = DateTime.Now;
                            oOmMstVehicle.LastUpdateBy = CurrentUser.UserId;

                            result = ctx.SaveChanges() >= 0;

                            if (result == true)
                            {
                                var qtyVehicle = ctx.OmTrInventQtyVehicles.Find(companyCode, branchCode, Convert.ToDecimal(transferOutDate.Year),
                                    Convert.ToDecimal(transferOutDate.Month), row.SalesModelCode, row.SalesModelYear,
                                    row.ColourCode, warehouseCode);

                                if (qtyVehicle != null)
                                {
                                    qtyVehicle.QtyOut = qtyVehicle.QtyOut + 1;
                                    qtyVehicle.EndingOH = (qtyVehicle.BeginningOH + qtyVehicle.QtyIn) - qtyVehicle.QtyOut;
                                    qtyVehicle.EndingAV = (qtyVehicle.BeginningAV + qtyVehicle.QtyIn) - qtyVehicle.Alocation - qtyVehicle.QtyOut;

                                    qtyVehicle.LastUpdateBy = CurrentUser.UserId;
                                    qtyVehicle.LastUpdateDate = DateTime.Now;

                                    if (ctx.SaveChanges() < 0)
                                    { result = false; break; }
                                    else
                                        result = true;
                                }
                            }
                            else
                                return result;
                        }
                    }
                    if (result)
                    {
                            TransApprove.Commit();
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    TransApprove.Rollback();
                    errMsg = ex.Message;
                }
               
                return result;
            }
        }
    }
}
