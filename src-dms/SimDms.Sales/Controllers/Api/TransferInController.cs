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
    public class TransferInController : BaseController  
    {

        public JsonResult TransferInDetailLoad(string TransferInNo)
        {
            return Json( GetGrid(TransferInNo));
        }

        public IEnumerable<TransferOutDetailView> GetGrid(string TransferInNo)
        {
            var query = string.Format(@"
                SELECT Convert(varchar,a.TransferInSeq) as 
            TransferInSeq,a.SalesModelCode,Convert(varchar,a.SalesModelYear) as SalesModelYear,b.SalesModelDesc,
            a.ChassisCode,Convert(varchar,a.ChassisNo) as ChassisNo,a.EngineCode,Convert(varchar,a.EngineNo) as EngineNo,a.ColourCode,
            c.RefferenceDesc1 as ColourName,a.Remark
            FROM OmTrInventTransferInDetail a
            LEFT JOIN omMstModel b
            ON a.CompanyCode = b.CompanyCode
            AND a.SalesModelCode = b.SalesModelCode
            LEFT JOIN omMstRefference c
            ON a.CompanyCode = c.CompanyCode
            AND a.ColourCode = c.RefferenceCode
            WHERE c.RefferenceType = 'COLO'
            AND a.CompanyCode = '{0}'
            AND a.BranchCode = '{1}'
            AND a.TransferInNo = '{2}'
            ORDER BY a.SalesModelCode,a.SalesModelYear,a.ChassisNo ASC
                       ", CompanyCode, BranchCode, TransferInNo);

            var data = ctx.Database.SqlQuery<TransferOutDetailView>(query);
            return data;
        }

        public JsonResult Approve(omTrInventTransferIn model)
        {
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    object[] parameters = { CompanyCode, BranchCode, model.TransferInNo, CurrentUser.UserId };
                    ctx.Database.ExecuteSqlCommand("uspfn_OmApproveTransferIn @p0, @p1, @p2, @p3", parameters);
                    trans.Commit();

                    var me = ctx.omTrInventTransferIn.Find(CompanyCode, BranchCode, model.TransferInNo);
                    return Json(new { success = true, transferInNo = me.TransferInNo, data = me });
                }
                catch (Exception Ex)
                {
                    trans.Rollback();
                    return Json(new { success = false, message =  "Aprrove Transfer In Gagal. Exception Message : " + Ex.Message });
                }
            }
        }
        public JsonResult updateHdr(omTrInventTransferIn model)
        {
            var me = ctx.omTrInventTransferIn.Find(CompanyCode, BranchCode, model.TransferInNo);
            var data = new omTrInventTransferIn();
            if (me != null)
            {
                var meBPU = ctx.omTrInventTransferInDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.TransferInNo == model.TransferInNo).FirstOrDefault();
                

                if (meBPU != null && me.Status == "0")
                {
                    var meDSM = ctx.omTrInventTransferInDetail
                        .Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.TransferInNo == model.TransferInNo && p.ChassisCode == meBPU.ChassisCode)
                        .FirstOrDefault();
                    if (meDSM != null)
                    {
                        me.Status = "1";
                        ctx.SaveChanges();
                        data = ctx.omTrInventTransferIn.Find(CompanyCode, BranchCode, model.TransferInNo);
                        return Json(new { success = true, data = data }); 
                    }
                    else
                    {
                        return Json(new { success = false, message = meBPU.TransferInNo + " : do not have table detail model in Transfer In!" });
                    }
                }
                else if (meBPU != null && me.Status == "1")
                {
                    me.Status = "2";
                    ctx.SaveChanges();
                    data = ctx.omTrInventTransferIn.Find(CompanyCode, BranchCode, model.TransferInNo);
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
        public JsonResult Print(omTrInventTransferIn model)
        {
            var hdr = ctx.omTrInventTransferIn.Find(CompanyCode, BranchCode, model.TransferInNo);
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
                return Json(new { success = false, message = "Tidak ada Data untuk diprint."});
            }
        }

        [HttpPost]
        public JsonResult Save(omTrInventTransferIn model, bool isAll)
        {
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            DateTime currentTime = DateTime.Now;
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var me = ctx.omTrInventTransferIn.Find(companyCode, BranchCode, model.TransferInNo);

                    if (me == null)
                    {
                        me = new omTrInventTransferIn();
                        me.CreatedDate = currentTime;
                        me.LastUpdateDate = currentTime;
                        me.CreatedBy = userID;
                        var TransferInNo = GetNewDocumentNo("VTI", model.TransferInDate.Value);
                        me.TransferInNo = TransferInNo;
                        me.Status = "0";
                        ctx.omTrInventTransferIn.Add(me);
                    }
                    else
                    {
                        me.LastUpdateDate = currentTime;
                        me.LastUpdateBy = userID;
                    }
                    me.CompanyCode = CompanyCode;
                    me.BranchCode = BranchCode;
                    me.TransferInDate = model.TransferInDate;
                    me.TransferOutNo = model.TransferOutNo;
                    me.ReferenceNo = model.ReferenceNo;
                    me.ReferenceDate = model.ReferenceDate;
                    me.BranchCodeFrom = model.BranchCodeFrom;
                    me.WareHouseCodeFrom = model.WareHouseCodeFrom;
                    me.BranchCodeTo = model.BranchCodeTo;
                    me.WareHouseCodeTo = model.WareHouseCodeTo;
                    me.ReturnDate = model.ReturnDate;
                    me.Remark = model.Remark;

                    Helpers.ReplaceNullable(me);

                    var result = ctx.SaveChanges() > 0;
                    if (isAll)
                    {
                        result = InsertAllDetail(me);
                    }

                    if (result)
                    {
                        trans.Commit();

                        return Json(new { success = true, transferInNo = me.TransferInNo });
                    }
                    else
                    {
                        trans.Commit();
                        throw new Exception();
                    }
                }
                catch (Exception Ex)
                {
                    trans.Commit();
                    return Json(new { success = false, message = Ex.Message });
                }
            }
        }
        private bool InsertAllDetail(omTrInventTransferIn record)
        {
            bool result = true;

            var outDetail = ctx.omTrInventTransferOutDetail.Where(a => a.CompanyCode == record.CompanyCode && a.BranchCode == record.BranchCodeFrom && a.TransferOutNo == record.TransferOutNo && a.StatusTransferIn == "0").ToList();

            if (outDetail.Count() > 0)
            {
                foreach (var row in outDetail)
                {
                    var recordDtl = new omTrInventTransferInDetail();
                    recordDtl.CompanyCode = CompanyCode;
                    recordDtl.BranchCode = BranchCode;
                    recordDtl.TransferInNo = record.TransferInNo;
                    recordDtl.TransferInSeq = GetTransferInSeq(record.TransferInNo);
                    recordDtl.CreatedBy = CurrentUser.UserId;
                    recordDtl.CreatedDate = DateTime.Now;

                    recordDtl.SalesModelCode = row.SalesModelCode;
                    recordDtl.SalesModelYear = row.SalesModelYear;
                    recordDtl.ChassisCode = row.ChassisCode;
                    recordDtl.ChassisNo = row.ChassisNo;
                    recordDtl.EngineCode = row.EngineCode;
                    recordDtl.EngineNo = row.EngineNo;
                    recordDtl.ColourCode = row.ColourCode;
                    recordDtl.Remark = row.Remark;
                    recordDtl.LastUpdateBy = CurrentUser.UserId;
                    recordDtl.LastUpdateDate = DateTime.Now;
                    ctx.omTrInventTransferInDetail.Add(recordDtl);

                    Helpers.ReplaceNullable(recordDtl);
                    if (!Insert(row.CompanyCode, row.BranchCode, recordDtl, true, record.TransferOutNo, recordDtl.ChassisCode, int.Parse(recordDtl.ChassisNo.ToString()), record))
                    {
                        result = false; 
                        break;
                    }
                }
            }

            return result;
        }

        private bool Insert(string companyCode, string branchCode, omTrInventTransferInDetail record, bool isNew, string TransferOutNo,
                            string ChassisCode, int ChassisNo, omTrInventTransferIn recordHdr)
        {
            bool result = false;

            if (isNew)
            {
                var CheckTransferInDetailWithTrfOut = from a in ctx.omTrInventTransferInDetail
                                                      join b in ctx.omTrInventTransferIn
                                                      on new { a.CompanyCode, a.BranchCode, a.TransferInNo }
                                                      equals new { b.CompanyCode, b.BranchCode, b.TransferInNo }
                                                      where b.CompanyCode == companyCode && b.BranchCode == branchCode
                                                      && b.TransferOutNo == TransferOutNo && a.ChassisCode == ChassisCode && a.ChassisNo == ChassisNo
                                                      select new
                                                      {
                                                          a.BranchCode,
                                                          a.TransferInNo,
                                                          a.ChassisCode,
                                                          a.ChassisNo
                                                      };

                if (CheckTransferInDetailWithTrfOut.Count() > 0)
                    throw new Exception("Data ini sudah ada di BranchCode= " + CheckTransferInDetailWithTrfOut.First().BranchCode + " TrasferInNo= " + CheckTransferInDetailWithTrfOut.First().TransferInNo);

                string[] status = {"0","1"};

                var CheckTransferInDetail = from a in ctx.omTrInventTransferInDetail
                                            join b in ctx.omTrInventTransferIn
                                            on new {a.CompanyCode,a.BranchCode,a.TransferInNo}
                                            equals new {b.CompanyCode,b.BranchCode,b.TransferInNo}
                                            where a.ChassisCode == ChassisCode && a.ChassisNo == ChassisNo && status.Contains(b.Status)
                                            select new
                                            {
                                                a.BranchCode,
                                                a.TransferInNo,
                                                a.ChassisCode,
                                                a.ChassisNo
                                            };

                if (CheckTransferInDetail.Count() > 0)
                    throw new Exception("Data ini sudah ada di BranchCode= " + CheckTransferInDetail.First().BranchCode + " TrasferInNo= " + CheckTransferInDetail.First().TransferInNo);
            }

            // Insert table OmTrInventTransferInDetail
            result = ctx.SaveChanges() >= 0;

            // Update table omTrInventTransferOutDetail
            if (result)
            {
                var dataDetail = ctx.omTrInventTransferOutDetail.FirstOrDefault(a=>a.CompanyCode == companyCode && a.BranchCode == branchCode && a.TransferOutNo == TransferOutNo
                    && a.SalesModelCode == record.SalesModelCode && a.SalesModelYear == record.SalesModelYear && a.ChassisCode == record.ChassisCode && a.ChassisNo == record.ChassisNo);

                if (dataDetail != null)
                {
                    dataDetail.StatusTransferIn = "1";
                    dataDetail.LastUpdateBy = CurrentUser.UserId;
                    dataDetail.LastUpdateDate = DateTime.Now;
                    result = ctx.SaveChanges() >= 0;
                }
            }

            // change status header
            if (result)
            {
                if (!recordHdr.Status.Equals("0"))
                {
                    recordHdr.Status = "0";
                    recordHdr.LastUpdateDate = DateTime.Now;
                    recordHdr.LastUpdateBy = CurrentUser.UserId;
                    result = ctx.SaveChanges() >= 0;
                }
            }

            return result;
        }

        private int GetTransferInSeq(string TransferInNo)
        {
            var transIn = ctx.omTrInventTransferInDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.TransferInNo == TransferInNo).ToList();
            var No = (transIn.Count() == 0) ? 0 : transIn.Max(a => a.TransferInSeq);
            return Convert.ToInt32(No) + 1;
        }

        public JsonResult Delete(omTrInventTransferIn model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var me = ctx.omTrInventTransferIn.Find(CompanyCode, BranchCode, model.TransferInNo);
                    var meDtl = ctx.omTrInventTransferInDetail.Where(m => m.CompanyCode == companyCode && m.BranchCode == BranchCode && m.TransferInNo == model.TransferInNo).ToArray();
                    if (me != null)
                    {
                        var x = meDtl.Length;
                        for (var i = 0; i < x; i++ )
                        {
                            ctx.omTrInventTransferInDetail.Remove(meDtl[i]);
                            ctx.SaveChanges();
                        }
                        ctx.omTrInventTransferIn.Remove(me);
                        ctx.SaveChanges();
                        trans.Commit();
                        
                        returnObj = new { success = true, message = "Data Transfer In berhasil di delete." };
                    }
                    else
                    {
                        trans.Rollback();
                        returnObj = new { success = false, message = "Error ketika mendelete Transfer In, Message=Data tidak ditemukan" };
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnObj = new { success = false, message = "Error ketika mendelete Transfer In, Message=" + ex.ToString() };
                }
            }

            return Json(returnObj);
        }

        [HttpPost]
        public JsonResult Save2(omTrInventTransferInDetail model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var me = ctx.omTrInventTransferInDetail.Find(CompanyCode, BranchCode, model.TransferInNo, model.TransferInSeq);
                    var hdr = ctx.omTrInventTransferIn.Find(CompanyCode, BranchCode, model.TransferInNo);
                    if (hdr == null)
                    {
                        return Json(new { success = false, message = "Invalid Transferin No" });
                    }
                    if (me == null)
                    {
                        me = new omTrInventTransferInDetail();
                        decimal seqNo = Select4MaxSeq(CompanyCode, BranchCode, model.TransferInNo);
                        me.CreatedDate = currentTime;
                        me.LastUpdateDate = currentTime;
                        me.CreatedBy = userID;
                        me.TransferInSeq = seqNo + 1;
                        me.StatusTransferOut = "0";
                        ctx.omTrInventTransferInDetail.Add(me);
                    }
                    else
                    {
                        me.LastUpdateDate = currentTime;
                        me.LastUpdateBy = userID;
                    }
                    me.CompanyCode = CompanyCode;
                    me.BranchCode = BranchCode;
                    me.TransferInNo = model.TransferInNo;
                    me.SalesModelCode = model.SalesModelCode;
                    me.SalesModelYear = model.SalesModelYear;
                    me.ChassisCode = model.ChassisCode;
                    me.ChassisNo = model.ChassisNo;
                    me.EngineCode = model.EngineCode;
                    me.EngineNo = model.EngineNo;
                    me.ColourCode = model.ColourCode;
                    me.Remark = model.Remark;

                    Helpers.ReplaceNullable(me);
                    ctx.SaveChanges();

                    var outDetail = ctx.omTrInventTransferOutDetail
                        .Where(a => a.CompanyCode == me.CompanyCode
                                    && a.TransferOutNo == hdr.TransferOutNo
                                    && a.SalesModelCode == me.SalesModelCode
                                    && a.SalesModelYear == me.SalesModelYear
                                    && a.ChassisCode == me.ChassisCode
                                    && a.ChassisNo == me.ChassisNo
                                    && a.EngineCode == me.EngineCode
                                    && a.EngineNo == me.EngineNo
                                    && a.ColourCode == me.ColourCode
                           )
                           .FirstOrDefault();

                    if (outDetail != null)
                    {
                        outDetail.StatusTransferIn = "1";
                    }

                    ctx.SaveChanges();
                    trans.Commit();

                    var data = GetGrid(model.TransferInNo);
                    return Json(new { success = true, data = data });
                }
                catch (Exception Ex)
                {
                    trans.Rollback();
                    return Json(new { success = false, message = Ex.Message });
                }
            }
        }
        public JsonResult Delete2(string trOutNo, string branchCodeFrom, omTrInventTransferInDetail model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var me = ctx.omTrInventTransferInDetail.Find(CompanyCode, BranchCode, model.TransferInNo, model.TransferInSeq);
                    if (me != null)
                    {
                        var dtl = ctx.omTrInventTransferOutDetail.Where(x => x.CompanyCode == me.CompanyCode && x.BranchCode == branchCodeFrom && x.TransferOutNo == trOutNo
                                                                        && x.ChassisCode == me.ChassisCode && x.ChassisNo == me.ChassisNo).FirstOrDefault();
                        if (dtl != null)
                        {
                            ctx.omTrInventTransferInDetail.Remove(me);
                            dtl.StatusTransferIn = "0";
                            ctx.SaveChanges();
                            trans.Commit();

                            returnObj = new { success = true, message = "Data Transfer In Detail berhasil di delete." };
                        }
                        else
                        {
                            returnObj = new { success = false, message = "Data Transfer Out Detail tidak ditemukan!." };
                        }
                    }
                    else
                    {
                        trans.Rollback();
                        returnObj = new { success = false, message = "Error ketika mendelete Transfer In Detail, Message=Data tidak ditemukan" };
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnObj = new { success = false, message = "Error ketika mendelete Transfer In Detail, Message=" + ex.ToString() };
                }
            }

            return Json(returnObj);
        }
        protected decimal Select4MaxSeq(string CompanyCode, string BranchCode, string pNo)
        {
            var query = String.Format(@"
            SELECT isnull(max(TransferInSeq),0) FROM OmTrInventTransferInDetail
            WHERE CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND TransferInNo = '{2}'", CompanyCode, BranchCode, pNo);
            var queryable = ctx.Database.SqlQuery<decimal>(query).FirstOrDefault();
            return (queryable);
        }
    }
}
