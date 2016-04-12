using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Transactions;
using GeLang;
using SimDms.Sparepart.Models;
using System.Text;
using SimDms.Common;
using SimDms.Common.DcsWs;
using SimDms.Common.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class EntryRequestSparepartController : BaseController
    {
        public bool Status(string REQNo)
        {
            var oHdr = ctx.spTrnPREQHdr.Find(CompanyCode, BranchCode, REQNo);
            if (int.Parse(oHdr.Status) == 2)
                return true;
            return false;
        }

        public JsonResult Save(spTrnPREQHdr model)
        {
            string msg = "";
            string ReqNo = "";
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {

                    if (string.IsNullOrEmpty(model.REQNo))
                        ReqNo = GetNewDocumentNo("REQ", DateTime.Now);
                    else
                        ReqNo = model.REQNo;


                    var record = ctx.spTrnPREQHdr.Find(CompanyCode, BranchCode, ReqNo);


                    if (record == null)
                    {
                        record = new spTrnPREQHdr
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            REQNo = ReqNo,
                            Status = "0",
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now
                        };

                        ctx.spTrnPREQHdr.Add(record);
                        msg = "New Trasnactiopn REQ added...";
                    }
                    else
                    {
                        ctx.spTrnPREQHdr.Attach(record);
                        msg = "Transaction Updated";
                    }


                    record.REQDate = model.REQDate;
                    record.SupplierCode = model.SupplierCode;
                    record.Remark = model.Remark;
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
                    record.PrintSeq = 0;
                    record.ProcessFlag = "0";
                    record.isDeleted = false;

                    ctx.SaveChanges();
                    trans.Commit();

                    return Json(new { success = true, message = msg, data = record });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

            }
        }

        public JsonResult spTrnPREQHdrBrowse()
        {
            string sql = string.Format(@"EXEC uspfn_spTrnPREQHdr_Web '{0}', '{1}'",
                CompanyCode, BranchCode, 500);

            var records = ctx.Database.SqlQuery<spTrnPREQHdrView>(sql).AsQueryable();
            return Json(records.toKG());
        }

        public JsonResult GetREQDetail(string REQNo)
        {
            var header = ctx.spTrnPREQHdr.Find(CompanyCode, BranchCode, REQNo);
            var supplierName = ctx.GnMstSuppliers.Find(CompanyCode, header.SupplierCode).SupplierName;

            string sql = string.Format("exec uspfn_GetREQDetails '{0}','{1}','{2}'", CompanyCode, BranchCode, REQNo);
            var ListSODtl = ctx.Database.SqlQuery<SpTrnPREQDtlView>(sql).ToList();
            decimal orderqty = 0;
            foreach (var SO in ListSODtl)
            {
                orderqty += SO.OrderQty.Value;
            }
            return Json(new { dataHeader = header, data = ListSODtl, totalorder = orderqty, supplierName = supplierName });
        }

        public JsonResult CheckStatus(string REQNo)
        {
            string status = "", statusCode = "";
            var sql = string.Format("select Status from spTrnPREQHdr where CompanyCode='{0}' and BranchCode = '{1}' and REQNo = '{2}'", CompanyCode, BranchCode, REQNo);
            statusCode = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
            if (!string.IsNullOrEmpty(statusCode))
            {
                if (statusCode == "0")
                    status = "DRAFT";
                if (statusCode == "1")
                    status = "PRINTED";
                if (statusCode == "2")
                    status = "APPROVED";
            }
            return Json(new { success = true, statusPrint = status, statusCode = statusCode });
        }

        public JsonResult SaveDetails(spTrnPREQDtl model, string REQNo)
        {
            string msg = "";

            var record = ctx.spTrnPREQDtl.Find(CompanyCode, BranchCode, REQNo, model.PartNo);
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (record == null)
                    {
                        record = new spTrnPREQDtl
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            REQNo = REQNo,
                            PartNo = model.PartNo,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                        };
                        ctx.spTrnPREQDtl.Add(record);
                        msg = "New Transaction Request Sparepart Details added...";
                    }

                    var rcdHdr = ctx.spTrnPREQHdr.Find(CompanyCode, BranchCode, record.REQNo);
                    if (rcdHdr.Status == "1")
                    {
                        rcdHdr.Status = "0";
                        rcdHdr.LastUpdateBy = CurrentUser.UserId;
                        rcdHdr.LastUpdateDate = DateTime.Now;
                    }

                    record = prepareSpTrnPREQDtl(record, model);
                    ctx.SaveChanges();
                    trans.Commit();

                    return Json(new { success = true, message = "", data = rcdHdr });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        private spTrnPREQDtl prepareSpTrnPREQDtl(spTrnPREQDtl record, spTrnPREQDtl model)
        {
            record.OrderQty = model.OrderQty;
            spMstItem oItem = ctx.spMstItems.Find(CompanyCode, BranchCode, model.PartNo);
            record.ABCClass = (oItem == null) ? "" : oItem.ABCClass;
            record.MovingCode = (oItem == null) ? "" : oItem.MovingCode;
            record.PartCategory = (oItem == null) ? "" : oItem.PartCategory;
            record.TypeOfGoods = (oItem == null) ? "" : oItem.TypeOfGoods;
            record.ProductType = CurrentUser.CoProfile.ProductType;
            record.Note = model.Note;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            return record;
        }

        public JsonResult PrintREQ(string REQNo)
        {
            object returnObj = null;
            try
            {
                var recordHdr = ctx.spTrnPREQHdr.Find(CompanyCode, BranchCode, REQNo);
                if (recordHdr != null)
                {
                    recordHdr.Status = "1";
                    recordHdr.PrintSeq += 1;
                    recordHdr.LastUpdateBy = CurrentUser.UserId;
                    recordHdr.LastUpdateDate = DateTime.Now;
                }
                ctx.SaveChanges();
                returnObj = new { success = true, message = "" };
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = ex.Message.ToString() };
            }
            return Json(returnObj);
        }

        public JsonResult Delete(spTrnPREQHdr model)
        {
            object returnObj = null;
            try
            {
                var record = ctx.spTrnPREQHdr.Find(CompanyCode, BranchCode, model.REQNo);
                if (record != null)
                {
                    record.Status = "3";
                    record.isDeleted = true;
                    record.LastUpdateDate = DateTime.Now;
                    record.LastUpdateBy = CurrentUser.UserId;
                }
                else { 
                    throw new Exception(string.Format("Data {0} tidak di temukan", model.REQNo)); 
                }
                ctx.SaveChanges();
                returnObj = new { success = true, message = "" };
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = ex.Message.ToString() };
            }
            return Json(returnObj);
        }

        public JsonResult DeleteDetail(spTrnPREQDtl model, string REQNo)
        {
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                object returnObj = null;
                try
                {
                    if (Status(REQNo))
                    {
                        throw new ArgumentException("Nomor dokumen ini sudah ter-posting !!");
                    }
                    var POHdr = ctx.spTrnPREQHdr.Find(CompanyCode, BranchCode, REQNo);

                    if (POHdr != null)
                    {
                        POHdr.Status = "0";
                        POHdr.LastUpdateBy = CurrentUser.UserId;
                        POHdr.LastUpdateDate = DateTime.Now;
                    }
                    var REQDtlTmp = ctx.spTrnPREQDtl.Find(CompanyCode, BranchCode, REQNo, model.PartNo);
                    if (REQDtlTmp != null)
                    {
                        ctx.spTrnPREQDtl.Remove(REQDtlTmp);
                    }
                    else
                    {
                        throw new Exception("Data REQ details tidak ditemukan");
                    }

                    ctx.SaveChanges();
                    trans.Commit();

                    returnObj = new { success = true, message = "", data = POHdr };
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnObj = new { success = false, message = "Error pada saar Delete REQ Details, Message=" + ex.Message.ToString() };
                }

                return Json(returnObj);
            }
        }

        public JsonResult CreateREQ(spTrnPREQHdr model)
        {
            object returnObj = null;
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (Status(model.REQNo))
                    {
                        throw new Exception("Nomor dokumen ini sudah ter-posting !!");
                    }

                    var recordTmpHdr = ctx.spTrnPREQHdr.Find(CompanyCode, BranchCode, model.REQNo);
                    if (recordTmpHdr.Status.Equals("2") || recordTmpHdr.Status.Equals("3") || recordTmpHdr.Status.Equals("0"))
                    {
                        throw new ArgumentException(string.Format(ctx.SysMsgs.Find("5045").MessageCaption, recordTmpHdr.REQNo, "Create REQ"));
                    }


                    string errorMsg = "";
                    //errorMsg = DateTransValidation(model.REQDate.Value);
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        throw new ArgumentException(errorMsg);
                    }
                    ProcessCreateREQ(model);
                    returnObj = new { success = true, message = "" };

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnObj = new { success = false, message = "Error pada saat Process CreatePOS, Message=" + ex.Message.ToString() };
                }
            }
            return Json(returnObj);
        }

        private void ProcessCreateREQ(spTrnPREQHdr TrnPREQHdr)
        {
            try
            {
                var tmpPREQHdr = ctx.spTrnPREQHdr.Find(CompanyCode, BranchCode, TrnPREQHdr.REQNo);
                if (tmpPREQHdr != null)
                {
                    if (tmpPREQHdr.Status.Equals("1"))
                    {
                        tmpPREQHdr.Status = "2";
                        tmpPREQHdr.LastUpdateBy = CurrentUser.UserId;
                        tmpPREQHdr.LastUpdateDate = DateTime.Now;
                        ctx.SaveChanges();

                    }
                    else { throw new ArgumentException("REQ belum di Cetak, silahkan dicetak terlebih dahulu"); }
                }
                else { throw new ArgumentException("Data tidak ditemukan, Tolong Dipastikan kalau data sudah benar"); }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message.ToString());
            }
        }

        public JsonResult CancelREQ(spTrnPREQHdr model)
        {
            object returnObj = null;
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (ctx.SpTrnSORDHdrs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.OrderNo == model.REQNo).FirstOrDefault() != null)
                    {
                        return Json(returnObj = new { success = false, message = "Document tidak dapat di cancel,Document sudah di proses"});
                    }

                    var oRcrd = ctx.spTrnPREQHdr.Find(CompanyCode, BranchCode, model.REQNo);
                    if (oRcrd.Status.Equals("3"))
                    {
                        throw new Exception(string.Format(ctx.SysMsgs.Find("5045").MessageCaption, oRcrd.REQNo, "Cancel REQ"));
                    }
                   
                    oRcrd.Status = "0";
                    oRcrd.LastUpdateBy = CurrentUser.UserId;
                    oRcrd.LastUpdateDate = DateTime.Now;

                    ctx.SaveChanges();
                    trans.Commit();

                    returnObj = new { success = true, message = "" };
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnObj = new { success = false, message = "Error pada saat Proses Cancel REQ" + ex.Message.ToString() };
                }
            }
            return Json(returnObj);
        }
    }

}
