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
using OfficeOpenXml;
using EP = SimDms.Common.EPPlusHelper;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;

namespace SimDms.Sparepart.Controllers.Api
{
    public class PembelianEntryOrderSparepartController : BaseController
    {
        /// <summary>
        ///   The <c>PembelianEntryOrderSparepartController</c> type 
        ///   Fungsi save pada Entry Pesanan Sparepart.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Pembelian Entry Order Sparepart Controller<br></br>
        ///     Execute Model <t></t>: <br></br>
        ///     Primary Key<t></t>: (CompanyCode, BranchCode, model.POSNo) <br></br>
        ///     Tabel<t></t>: spTrnPPOSHdr <br></br>
        ///     Link Tabel<t></t>: spTrnPPOSdtl <br></br>
        ///     Update Link ref<t></t>: <br></br>
        ///   </para>
        /// </remarks>
        /// 
        private const string dataID = "PORDS";
        private string msg = "";
        private DcsWsSoapClient ws = new DcsWsSoapClient();

        public JsonResult Default()
        {
            var curDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm");

            return Json(new { currentDate = curDate });
        }

        protected string GetNewDocumentNo(string doctype, DateTime transdate)
        {
            var sql = "uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            var result = ctx.Database.SqlQuery<string>(sql, CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate.ToString("yyyy-MM-dd")).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Get List Sparepart Order Details
        /// </summary>
        /// <param name="POSNo"></param>
        /// <returns></returns>
        public JsonResult GetSODetail(string POSNo)
        {
            var header = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, POSNo);
            var supplierName = ctx.GnMstSuppliers.Find(CompanyCode, header.SupplierCode).SupplierName;

            string sql = string.Format("exec uspfn_GetSODetails '{0}','{1}','{2}'", CompanyCode, BranchCode, POSNo);
            var ListSODtl = ctx.Database.SqlQuery<SpTrnPPOSDtlView>(sql).ToList();
            decimal orderqty = 0, totalAmt = 0;
            foreach (var SO in ListSODtl)
            {
                orderqty += SO.OrderQty.Value;
                totalAmt += ((SO.TotalAmount == null) ? 0 : SO.TotalAmount.Value);
            }
            return Json(new { dataHeader = header, data = ListSODtl, totalorder = orderqty, totalamt = totalAmt, supplierName = supplierName });
        }


        public JsonResult GetSODetailAOS(string POSNo, string Branch)
        {
            var header = ctx.spTrnPPOSHdrs.Find(CompanyCode, Branch, POSNo);
            var supplierName = ctx.GnMstSuppliers.Find(CompanyCode, header.SupplierCode).SupplierName;

            string sql = string.Format("exec uspfn_GetSODetails '{0}','{1}','{2}'", CompanyCode, Branch, POSNo);
            var ListSODtl = ctx.Database.SqlQuery<SpTrnPPOSDtlView>(sql).ToList();
            decimal orderqty = 0, totalAmt = 0;
            foreach (var SO in ListSODtl)
            {
                orderqty += SO.OrderQty.Value;
                totalAmt += ((SO.TotalAmount == null) ? 0 : SO.TotalAmount.Value);
            }
            return Json(new { dataHeader = header, data = ListSODtl, totalorder = orderqty, totalamt = totalAmt, supplierName = supplierName });
        }


        /// <summary>
        /// Save Pembelian Order Sparepart
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Save(spTrnPPOSHdr model)
        {
            string msg = "";
            string PosNo = "";
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (string.IsNullOrEmpty(model.POSNo))
                        PosNo = GetNewDocumentNo("POS", DateTime.Now);
                    else
                        PosNo = model.POSNo;

                    var record = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, PosNo);

                    if (record == null)
                    {
                        record = new spTrnPPOSHdr
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            POSNo = PosNo,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now
                        };

                        ctx.spTrnPPOSHdrs.Add(record);
                        msg = "New Transaction POS added...";
                    }
                    else
                    {
                        ctx.spTrnPPOSHdrs.Attach(record);
                        msg = "Transaction Updated";
                    }

                    model.isSubstution = false;
                    model.isSuggorProcess = false;

                    record.POSDate = model.POSDate;
                    record.SupplierCode = model.SupplierCode;
                    record.OrderType = model.OrderType;
                    record.isBO = model.isBO;
                    record.isSubstution = model.isSubstution;
                    record.isSuggorProcess = model.isSuggorProcess;
                    record.Remark = model.Remark;
                    record.ProductType = model.ProductType;
                    record.ExPickingSlipNo = model.ExPickingSlipNo;
                    record.ExPickingSlipDate = model.ExPickingSlipDate;
                    record.Transportation = model.Transportation;
                    record.TypeOfGoods = model.TypeOfGoods;
                    record.isGenPORDD = false;
                    record.isDeleted = false;
                    record.Status = "0";
                    record.PrintSeq = 0;
                    record.ProductType = CurrentUser.CoProfile.ProductType;
                    record.OrderType = model.OrderType;
                    record.TypeOfGoods = CurrentUser.TypeOfGoods;

                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;

                    Helpers.ReplaceNullable(record);
                    ctx.SaveChanges();
                    trans.Commit();

                    return Json(new { success = true, message = msg, data = record });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Json(new { success = false, message = "Proses Create PORDS file gagal", err = ex.Message });
                }
            }
        }

        /// <summary>
        /// Print Pembelian Entry order sparepart
        /// </summary>
        /// <returns></returns>
        public JsonResult PrintSO(string POSNo)
        {
            object returnObj = null;
            try
            {
                var recordHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, POSNo);
                if (recordHdr != null)
                {
                    recordHdr.Status = recordHdr.Status == "0" ? "1" : recordHdr.Status;
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

        /// <summary>
        /// Delete Pembelian Order Sparepart
        /// </summary>
        /// <param name="model">spTrnPPOSHdr model</param>
        /// <returns></returns>
        public JsonResult Delete(spTrnPPOSHdr model)
        {
            object returnObj = null;
            try
            {
                var record = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, model.POSNo);
                if (record != null)
                {
                    record.Status = "3";
                    record.LastUpdateDate = DateTime.Now;
                    record.LastUpdateBy = CurrentUser.UserId;
                }
                else { throw new Exception(string.Format("Data {0} tidak di temukan", model.POSNo)); }
                ctx.SaveChanges();
                returnObj = new { success = true, message = "" };
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = ex.Message.ToString() };
            }
            return Json(returnObj);
        }

        /// <summary>
        /// Delete Detaiol Pembelian Entry Order Sparepart
        /// </summary>
        /// <param name="PODtl">SpTrnSORDDtl Object</param>
        /// <returns></returns>
        public JsonResult DeleteDetailPO(SpTrnSORDDtl PODtl, string POSNo)
        {
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                object returnObj = null;
                try
                {
                    if (CheckStatus(POSNo))
                    {
                        throw new Exception("Nomor dokumen ini sudah ter-posting !!");
                    }
                    var POHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, POSNo);

                    if (POHdr != null)
                    {
                        POHdr.Status = "0";
                        POHdr.LastUpdateBy = CurrentUser.UserId;
                        POHdr.LastUpdateDate = DateTime.Now;
                    }
                    var PODtlTmp = ctx.spTrnPPOSDtls.Find(CompanyCode, BranchCode, POSNo, PODtl.PartNo);
                    if (PODtlTmp != null)
                    {
                        ctx.spTrnPPOSDtls.Remove(PODtlTmp);
                    }
                    else
                    {
                        throw new Exception("Data SO details tidak ditemukan");
                    }

                    ctx.SaveChanges();
                    trans.Commit();

                    returnObj = new { success = true, message = "", data = POHdr };
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnObj = new { success = false, message = "Error pada saar Delete SO Details, Message=" + ex.Message.ToString() };
                }

                return Json(returnObj);
            }
        }


        /// <summary>
        /// Save Pembelian Order Sparepart Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveDetails(spTrnPPOSDtl model, string POSNo)
        {
            string msg = "";

            var record = ctx.spTrnPPOSDtls.Find(CompanyCode, BranchCode, POSNo, model.PartNo);
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (record == null)
                    {
                        record = new spTrnPPOSDtl
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            POSNo = POSNo,
                            PartNo = model.PartNo,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                            SuggorQty = model.OrderQty
                        };
                        ctx.spTrnPPOSDtls.Add(record);
                        msg = "New Transaction Order Sparepart Details added...";
                    }

                    var rcdHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, record.POSNo);
                    if (rcdHdr.Status == "1")
                    {
                        rcdHdr.Status = "0";
                        rcdHdr.LastUpdateBy = CurrentUser.UserId;
                        rcdHdr.LastUpdateDate = DateTime.Now;
                    }

                    record = prepareSpTrnPPOSDtl(record, model);

                    Helpers.ReplaceNullable(record);
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

        /// <summary>
        /// Create Part Order Sales
        /// </summary>
        /// <param name="model">SpTrnsPPOSHdr Model</param>
        /// <returns></returns>
        public JsonResult CreatePOS(spTrnPPOSHdr model)
        {
            object returnObj = null;
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (CheckStatus(model.POSNo))
                    {
                        throw new Exception("Nomor dokumen ini sudah ter-posting !!");
                    }

                    var recordTmpHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, model.POSNo);
                    if (recordTmpHdr.Status.Equals("2") || recordTmpHdr.Status.Equals("3") || recordTmpHdr.Status.Equals("0"))
                    {
                        throw new Exception(string.Format(ctx.SysMsgs.Find("5045").MessageCaption, recordTmpHdr.POSNo, "Create POS"));
                    }

                    //check total amount in client side
                    //check date trans valid
                    string errorMsg = "";
                    errorMsg = DateTransValidation(model.POSDate.Value);
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        throw new Exception(errorMsg);
                    }

                    ProcessCreatePOS(model);
                    trans.Commit();
                    returnObj = new { success = true, message = "" };
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnObj = new { success = false, message = "Error pada saat Process CreatePOS, Message=" + ex.Message.ToString() };
                }
            }

            return Json(returnObj);
        }

        public FileContentResult DownloadFileAOS(string SupplierCode, string POSNo, string Branch)
        {
            var supplier = ctx.GnMstSuppliers.Find(CompanyCode, SupplierCode);
            var PPOSHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, Branch, POSNo);
            List<SpTrnPPOSDtlView> listPOSDtl = ctx.Database.SqlQuery<SpTrnPPOSDtlView>(string.Format("exec uspfn_spPPOSDtl4Table '{0}','{1}','{2}'", CompanyCode, Branch, POSNo)).ToList();
            string PORDS = GenPORDSAOS(PPOSHdr, listPOSDtl, Branch).Replace("<br/>", string.Empty);
            byte[] content = new byte[PORDS.Length * sizeof(char)];
            System.Buffer.BlockCopy(PORDS.ToCharArray(), 0, content, 0, content.Length);
            string contentType = "application/text";
            Response.Clear();
            MemoryStream ms = new MemoryStream(content);
            Response.ContentType = "application/text";
            Response.AddHeader("content-disposition", "attachment;filename=PORDS.txt");
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
            //Parameters to file are
            //1. The File Path on the File Server
            //2. The content type MIME type
            //3. The parameter for the file save by the browser

            PPOSHdr.isGenPORDD = true;
            PPOSHdr.LastUpdateBy = CurrentUser.UserId;
            PPOSHdr.LastUpdateDate = DateTime.Now;
            ctx.SaveChanges();

            LogHeaderFile(dataID, CompanyCode, PORDS, ProductType);

            return File(content, contentType, "PORDS.txt");
        }

        public FileContentResult DownloadFile(string SupplierCode, string POSNo)
        {
            var supplier = ctx.GnMstSuppliers.Find(CompanyCode, SupplierCode);
            var PPOSHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, POSNo);
            List<SpTrnPPOSDtlView> listPOSDtl = ctx.Database.SqlQuery<SpTrnPPOSDtlView>(string.Format("exec uspfn_spPPOSDtl4Table '{0}','{1}','{2}'", CompanyCode, BranchCode, POSNo)).ToList();
            string PORDS = GenPORDS(PPOSHdr, listPOSDtl).Replace("<br/>", string.Empty);
            byte[] content = new byte[PORDS.Length * sizeof(char)];
            System.Buffer.BlockCopy(PORDS.ToCharArray(), 0, content, 0, content.Length);
            string contentType = "application/text";
            Response.Clear();
            MemoryStream ms = new MemoryStream(content);
            Response.ContentType = "application/text";
            Response.AddHeader("content-disposition", "attachment;filename=PORDS.txt");
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
            //Parameters to file are
            //1. The File Path on the File Server
            //2. The content type MIME type
            //3. The parameter for the file save by the browser
            PPOSHdr.isGenPORDD = true;
            PPOSHdr.LastUpdateBy = CurrentUser.UserId;
            PPOSHdr.LastUpdateDate = DateTime.Now;
            ctx.SaveChanges();

            LogHeaderFile(dataID, CompanyCode, PORDS, ProductType);
            //msg = string.Format("{0} berhasil di upload", dataID);


            return File(content, contentType, "PORDS.txt");
        }

        /// <summary>
        /// Process Create Pembelian Order Sparepart
        /// </summary>
        /// <param name="TrnPPOSHdr">spTrnPPOSHdr  Object</param>
        private void ProcessCreatePOS(spTrnPPOSHdr TrnPPOSHdr)
        {
            //try
            //{
                var tmpPPOSHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, TrnPPOSHdr.POSNo);
                if (tmpPPOSHdr != null)
                {
                    if (tmpPPOSHdr.Status.Equals("1"))
                    {
                        tmpPPOSHdr.Status = "2";
                        tmpPPOSHdr.LastUpdateBy = CurrentUser.UserId;
                        tmpPPOSHdr.LastUpdateDate = DateTime.Now;
                        ctx.SaveChanges();

                        //get so details
                        var listSODtl = ctx.spTrnPPOSDtls.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.POSNo == TrnPPOSHdr.POSNo).ToList();
                        foreach (var SODtl in listSODtl)
                        {
                            spTrnPOrderBalance recordOrdBal = ctx.spTrnPOrderBalances.Find(CompanyCode, BranchCode, tmpPPOSHdr.POSNo, tmpPPOSHdr.SupplierCode,
                                SODtl.PartNo, SODtl.SeqNo);
                            if (recordOrdBal == null)
                            {
                                recordOrdBal = new spTrnPOrderBalance();
                                recordOrdBal.CompanyCode = CompanyCode;
                                recordOrdBal.BranchCode = BranchCode;
                                recordOrdBal.POSNo = TrnPPOSHdr.POSNo;
                                recordOrdBal.SupplierCode = TrnPPOSHdr.SupplierCode;
                                recordOrdBal.PartNo = SODtl.PartNo;
                                recordOrdBal.CreatedBy = CurrentUser.UserId;
                                recordOrdBal.CreatedDate = DateTime.Now;
                                recordOrdBal.SeqNo = GetNewSeqNo(tmpPPOSHdr.POSNo);
                                ctx.spTrnPOrderBalances.Add(recordOrdBal);
                            }

                            recordOrdBal.PartNoOriginal = recordOrdBal.PartNo;
                            recordOrdBal.POSDate = tmpPPOSHdr.POSDate.Value;
                            recordOrdBal.OrderQty = Convert.ToDecimal(SODtl.OrderQty == null ? 0 : SODtl.OrderQty.Value);
                            recordOrdBal.OnOrder = recordOrdBal.OrderQty;
                            recordOrdBal.InTransit = 0;
                            recordOrdBal.Received = 0;
                            recordOrdBal.Located = 0;
                            recordOrdBal.DiscPct = (SODtl.DiscPct == null ? 0 : SODtl.DiscPct.Value);
                            recordOrdBal.TypeOfGoods = tmpPPOSHdr.TypeOfGoods;

                            recordOrdBal.PurchasePrice = Convert.ToDecimal(SODtl.PurchasePrice == null ? 0 : SODtl.PurchasePrice.Value);
                            recordOrdBal.CostPrice = Convert.ToDecimal(SODtl.CostPrice == null ? 0 : SODtl.CostPrice);

                            recordOrdBal.ABCClass = string.IsNullOrEmpty(SODtl.ABCClass) ? "C" : SODtl.ABCClass;
                            recordOrdBal.MovingCode = string.IsNullOrEmpty(SODtl.MovingCode) ? "0" : SODtl.MovingCode;

                            recordOrdBal.LastUpdateBy = CurrentUser.UserId;
                            recordOrdBal.LastUpdateDate = DateTime.Now;
                            
                            Helpers.ReplaceNullable(recordOrdBal);
                            ctx.SaveChanges();

                            //update Order Stock
                            if (!UpdateOnOrderStock(SODtl.PartNo, (int)recordOrdBal.OnOrder.Value, 0, 0))
                            {
                                throw new Exception("Update On Order Persediaan Gagal");
                            }
                            //UpdateLastItemDate(recordOrdBal.POSNo, "LPD");
                            UpdateLastItemDate(recordOrdBal.PartNoOriginal, "LPD");
                        }
                    }
                    else { throw new Exception("POS belum di Cetak, silahkan dicetak terlebih dahulu"); }
                }
                else { throw new Exception("Data tidak ditemukan, Tolong Dipastikan kalau data sudah benar"); }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message.ToString());
            //}
        }

        /// <summary>
        /// Get New spTrnPOrderBalance SeqNo
        /// </summary>
        /// <param name="PosNo"></param>
        /// <returns></returns>
        public decimal GetNewSeqNo(string PosNo)
        {
            string sql = string.Format(@"
            SELECT TOP 1 SeqNo
            FROM SpTrnPOrderBalance
            WHERE CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND POSNo = '{2}'
            ORDER BY SeqNo DESC
            ", CompanyCode, BranchCode, PosNo);
            decimal val = ctx.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            return val + 1;
        }

        /// <summary>
        /// Check Status spTrnPPOSHdr
        /// </summary>
        /// <param name="PosNo"></param>
        /// <returns></returns>
        public bool CheckStatus(string PosNo)
        {
            var oHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, PosNo);
            if (int.Parse(oHdr.Status) >= 2)
                return true;
            return false;
        }

        /// <summary>
        /// Cancel Process Order Sparepart
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult CancelPOS(spTrnPPOSHdr model)
        {
            object returnObj = null;
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var oRcrd = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, model.POSNo);
                    if (oRcrd.Status.Equals("3"))
                    {
                        throw new Exception(string.Format(ctx.SysMsgs.Find("5045").MessageCaption, oRcrd.POSNo, "Cancel POS"));
                    }

                    //
                    var listNoPosUsed = GetNoPosUsed(model.POSNo);
                    if (listNoPosUsed.Count > 0)
                    {
                        string BinnNo = "";
                        int hit = 0;
                        foreach (var NoPosUsed in listNoPosUsed)
                        {
                            hit++;
                            if (hit < listNoPosUsed.Count)
                                BinnNo += "[ " + NoPosUsed.ToString() + "; ";
                            else if (hit == listNoPosUsed.Count)
                                BinnNo += NoPosUsed.ToString() + " ]";
                        }
                        throw new Exception("Record Sudah Pernah Digunakan Dalam Transaction" + " " + BinnNo);
                    }

                    List<orderPartview> orderbalanceLst = Select4NoPartOrderBalance(model.POSNo);
                    bool result;
                    if (orderbalanceLst.Count() > 0)
                    {
                        foreach (var Order in orderbalanceLst)
                        {
                            if (Order.OrderQty != Order.OnOrder)
                            {
                                throw new Exception("Record Sudah Pernah Digunakan Dalam Transaction");
                            }
                            result = UpdateOnOrderStock(Order.PartNo, (int)(-1 * Order.OnOrder), 0, 0);
                            if (!result)
                            {
                                throw new Exception("Update OnOrder Stock di Master Item tidak berhasil");
                            }
                            //delete SpTrnPOrderBalance
                            var POrderBlnc = ctx.spTrnPOrderBalances.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.POSNo == Order.POSNo).FirstOrDefault();
                            ctx.spTrnPOrderBalances.Remove(POrderBlnc);
                        }
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
                    returnObj = new { success = false, message = "Error pada saal Proses Cancel POS" + ex.Message.ToString() };
                }
            }
            return Json(returnObj);
        }

        /// <summary>
        /// Create PPORDS File
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult CreatePORDS(spTrnPPOSHdr model)
        {
            object returnObj = null;
            try
            {
                if (string.IsNullOrEmpty(model.SupplierCode))
                {
                    throw new Exception("Kode Standard Supplier belum disetup untuk Kode Supplier ini");
                }
                var supplier = ctx.GnMstSuppliers.Find(model.CompanyCode, model.SupplierCode);
                List<SpTrnPPOSDtlView> listPOSDtl = ctx.Database.SqlQuery<SpTrnPPOSDtlView>(string.Format("exec uspfn_spPPOSDtl4Table '{0}','{1}','{2}'", CompanyCode, BranchCode, model.POSNo)).ToList();
                string PORDS = GenPORDS(model, listPOSDtl);

                returnObj = new { success = true, message = "", data = PORDS.Replace(" ", "&nbsp;") };
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = ex.Message };
            }
            return Json(returnObj);
        }

        public string GenPORDS(spTrnPPOSHdr PPOSHdr, List<SpTrnPPOSDtlView> ListPPOSDtl)
        {
            PPOSHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, PPOSHdr.POSNo);
            StringBuilder sb = new StringBuilder();
            // Record ID
            sb.Append("H");
            // Data ID 
            sb.Append("PORDS");
            //Dealer Code
            if (ListPPOSDtl[0].ParaValue == "" || ListPPOSDtl[0].ParaValue == "0")
                sb.Append(CompanyCode.PadRight(10, ' ').Substring(0, 10));
            else
                sb.Append(ListPPOSDtl[0].Dealer.PadRight(10, ' ').Substring(0, 10));

            var supplier = ctx.GnMstSuppliers.Find(CompanyCode, PPOSHdr.SupplierCode);
            sb.Append(supplier.StandardCode.PadRight(10, ' ').Substring(0, 10));
            //Ship to dealer code
            sb.Append(ListPPOSDtl[0].ShipTo.ToString().PadRight(10, ' ').Substring(0, 10));
            // Total Number of Item
            sb.Append(ListPPOSDtl.Count.ToString().PadLeft(6, '0').Substring(0, 6));
            // Order Number
            sb.Append(PPOSHdr.POSNo.PadRight(15, ' ').Substring(0, 15));
            // Order Date
            sb.Append(PPOSHdr.POSDate.Value.ToString("yyyMMdd").PadRight(8, ' ').Substring(0, 8));
            // Order Type
            sb.Append(PPOSHdr.OrderType.PadRight(1, ' ').Substring(0, 1));
            // Product Type
            sb.Append(PPOSHdr.ProductType.PadRight(1, ' ').Substring(0, 1));
            // Back Order Status
            sb.Append(PPOSHdr.isBO.Value ? "Y" : "N");
            int i = 0;
            foreach (var PPOSDtl in ListPPOSDtl)
            {
                sb.AppendLine("<br/>");
                sb.Append("1");
                // Line
                sb.Append(i.ToString().PadLeft(5, '0').Substring(0, 5));
                // Part No                                
                sb.Append(PPOSDtl.PartNo.ToString().PadRight(15, ' ').Substring(0, 15));
                // Quantity
                decimal orderQty = 0; string sdate = DateTime.Today.ToString("yyyMMdd");
                var lookup = ctx.LookUpDtls.Find(CompanyCode, "POCON", PPOSDtl.PartNo);
                if (lookup != null)
                {
                    if (lookup.ParaValue == "1")
                    {
                        var itemInfo = ctx.SpMstItemInfos.Find(CompanyCode, PPOSDtl.PartNo.ToString());
                        if (itemInfo != null)
                        {
                            orderQty = Convert.ToDecimal(PPOSDtl.OrderQty) / itemInfo.OrderUnit.Value;
                        }
                        else
                        {
                            orderQty = PPOSDtl.OrderQty.Value;
                        }
                    }
                    else
                    {
                        orderQty = PPOSDtl.OrderQty.Value;
                    }
                }
                else
                {
                    orderQty = PPOSDtl.OrderQty.Value;
                }
                sb.Append(Convert.ToInt32(orderQty).ToString().PadLeft(6, '0').Substring(0, 6));
                // Process Date
                sb.Append(sdate);
                // Blank Filler
                if (PPOSHdr.Remark != "")
                    sb.Append(PPOSHdr.Remark.PadLeft(33, ' '));
                else
                    sb.Append(((string)" ").PadRight(33, ' '));
            }
            return sb.ToString();
        }


        public JsonResult CreatePORDSAOS(spTrnPPOSHdr model, string Branch)
        {
            object returnObj = null;
            try
            {
                if (string.IsNullOrEmpty(model.SupplierCode))
                {
                    throw new ArgumentException("Kode Standard Supplier belum disetup untuk Kode Supplier ini");
                }
                var supplier = ctx.GnMstSuppliers.Find(model.CompanyCode, model.SupplierCode);
                List<SpTrnPPOSDtlView> listPOSDtl = ctx.Database.SqlQuery<SpTrnPPOSDtlView>(string.Format("exec uspfn_spPPOSDtl4Table '{0}','{1}','{2}'", CompanyCode, Branch, model.POSNo)).ToList();
                string PORDS = GenPORDSAOS(model, listPOSDtl, Branch);

                returnObj = new { success = true, message = "", data = PORDS.Replace(" ", "&nbsp;") };
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = ex.Message };
            }
            return Json(returnObj);
        }

        public string GenPORDSAOS(spTrnPPOSHdr PPOSHdr, List<SpTrnPPOSDtlView> ListPPOSDtl, string Branch)
        {
            PPOSHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, Branch, PPOSHdr.POSNo);
            StringBuilder sb = new StringBuilder();
            // Record ID
            sb.Append("H");
            // Data ID 
            sb.Append("PORDS");
            //Dealer Code
            if (ListPPOSDtl[0].ParaValue == "" || ListPPOSDtl[0].ParaValue == "0")
                sb.Append(CompanyCode.PadRight(10, ' ').Substring(0, 10));
            else
                sb.Append(ListPPOSDtl[0].Dealer.PadRight(10, ' ').Substring(0, 10));

            var supplier = ctx.GnMstSuppliers.Find(CompanyCode, PPOSHdr.SupplierCode);
            sb.Append(supplier.StandardCode.PadRight(10, ' ').Substring(0, 10));
            //Ship to dealer code
            sb.Append(ListPPOSDtl[0].ShipTo.ToString().PadRight(10, ' ').Substring(0, 10));
            // Total Number of Item
            sb.Append(ListPPOSDtl.Count.ToString().PadLeft(6, '0').Substring(0, 6));
            // Order Number
            sb.Append(PPOSHdr.POSNo.PadRight(15, ' ').Substring(0, 15));
            // Order Date
            sb.Append(PPOSHdr.POSDate.Value.ToString("yyyMMdd").PadRight(8, ' ').Substring(0, 8));
            // Order Type
            sb.Append(PPOSHdr.OrderType.PadRight(1, ' ').Substring(0, 1));
            // Product Type
            sb.Append(PPOSHdr.ProductType.PadRight(1, ' ').Substring(0, 1));
            // Back Order Status
            sb.Append(PPOSHdr.isBO.Value ? "Y" : "N");
            int i = 0;
            foreach (var PPOSDtl in ListPPOSDtl)
            {
                sb.AppendLine("<br/>");
                sb.Append("1");
                // Line
                sb.Append(i.ToString().PadLeft(5, '0').Substring(0, 5));
                // Part No                                
                sb.Append(PPOSDtl.PartNo.ToString().PadRight(15, ' ').Substring(0, 15));
                // Quantity
                decimal orderQty = 0; string sdate = DateTime.Today.ToString("yyyMMdd");
                var lookup = ctx.LookUpDtls.Find(CompanyCode, "POCON", PPOSDtl.PartNo);
                if (lookup != null)
                {
                    if (lookup.ParaValue == "1")
                    {
                        var itemInfo = ctx.SpMstItemInfos.Find(CompanyCode, PPOSDtl.PartNo.ToString());
                        if (itemInfo != null)
                        {
                            orderQty = Convert.ToDecimal(PPOSDtl.OrderQty) / itemInfo.OrderUnit.Value;
                        }
                        else
                        {
                            orderQty = PPOSDtl.OrderQty.Value;
                        }
                    }
                    else
                    {
                        orderQty = PPOSDtl.OrderQty.Value;
                    }
                }
                else
                {
                    orderQty = PPOSDtl.OrderQty.Value;
                }
                sb.Append(Convert.ToInt32(orderQty).ToString().PadLeft(6, '0').Substring(0, 6));
                // Process Date
                sb.Append(sdate);
                // Blank Filler
                if (PPOSHdr.Remark != "")
                    sb.Append(PPOSHdr.Remark.PadLeft(33, ' '));
                else
                    sb.Append(((string)" ").PadRight(33, ' '));
            }
            return sb.ToString();
        }


        public JsonResult SendFileAOS(spTrnPPOSHdr model, string Branch)
        {
            var posHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, Branch, model.POSNo);

            var supplier = ctx.GnMstSuppliers.Find(model.CompanyCode, model.SupplierCode);
            List<SpTrnPPOSDtlView> listPOSDtl = ctx.Database.SqlQuery<SpTrnPPOSDtlView>(string.Format("exec uspfn_spPPOSDtl4Table '{0}','{1}','{2}'", CompanyCode, Branch, model.POSNo)).ToList();
            string data1 = GenPORDSAOS(model, listPOSDtl, Branch).Replace("<br/>", string.Empty);
            string data = data1.Replace("\r\n", "\n");
            string header = data.Split('\n')[0];
            try
            {

                string result = ws.SendToDcs(dataID, CompanyCode, data, ProductType);
                if (result.StartsWith("FAIL")) return Json(new { success = false, message = result.Substring(5) });

                LogHeaderFile(dataID, CompanyCode, header, ProductType);
                msg = string.Format("{0} berhasil di upload", dataID);

                posHdr.isGenPORDD = true;
                posHdr.LastUpdateBy = CurrentUser.UserId;
                posHdr.LastUpdateDate = DateTime.Now;
                ctx.SaveChanges();
                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                msg = string.Format("{0} gagal digenerate : {1}", dataID, ex.Message.ToString());
                return Json(new { success = false, message = msg });
            }
        }

        public JsonResult SendFile(spTrnPPOSHdr model)
        {
            var posHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, model.POSNo);

            var supplier = ctx.GnMstSuppliers.Find(model.CompanyCode, model.SupplierCode);
            List<SpTrnPPOSDtlView> listPOSDtl = ctx.Database.SqlQuery<SpTrnPPOSDtlView>(string.Format("exec uspfn_spPPOSDtl4Table '{0}','{1}','{2}'", CompanyCode, BranchCode, model.POSNo)).ToList();
            string data1 = GenPORDS(model, listPOSDtl).Replace("<br/>", string.Empty);
            string data = data1.Replace("\r\n", "\n");
            string header = data.Split('\n')[0];
            try
            {

                string result = ws.SendToDcs(dataID, CompanyCode, data, ProductType);
                if (result.StartsWith("FAIL")) return Json(new { success = false, message = result.Substring(5) });

                LogHeaderFile(dataID, CompanyCode, header, ProductType);
                msg = string.Format("{0} berhasil di upload", dataID);

                posHdr.isGenPORDD = true;
                posHdr.LastUpdateBy = CurrentUser.UserId;
                posHdr.LastUpdateDate = DateTime.Now;
                ctx.SaveChanges();
                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                msg = string.Format("{0} gagal digenerate : {1}", dataID, ex.Message.ToString());
                return Json(new { success = false, message = msg });
            }
        }

        public JsonResult ValidateHeaderFileAOS(spTrnPPOSHdr model, string Branch)
        {
            var result = true;

            var supplier = ctx.GnMstSuppliers.Find(model.CompanyCode, model.SupplierCode);
            List<SpTrnPPOSDtlView> listPOSDtl = ctx.Database.SqlQuery<SpTrnPPOSDtlView>(string.Format("exec uspfn_spPPOSDtl4Table '{0}','{1}','{2}'", CompanyCode, Branch, model.POSNo)).ToList();
            string PORDS = GenPORDSAOS(model, listPOSDtl, Branch);

            PORDS = PORDS.Split('\n')[0].Replace("<br/>", string.Empty);
            string qry = string.Format("select * from gnDcsUploadFile where DataID = '{0}' and Header = '{1}'", dataID, PORDS);
            var dt = ctx.Database.SqlQuery<GnDcsUploadFile>(qry);
            if (dt.Count() > 0)
            {
                result = false;
                msg = string.Format("Data {0} sudah pernah dikirim pada {1}, apakah akan dikirim ulang?", dataID, dt.FirstOrDefault().CreatedDate);
            }

            return Json(new { success = result, message = msg });
        }

        public JsonResult ValidateHeaderFile(spTrnPPOSHdr model)
        {
            var result = true;

            var supplier = ctx.GnMstSuppliers.Find(model.CompanyCode, model.SupplierCode);
            List<SpTrnPPOSDtlView> listPOSDtl = ctx.Database.SqlQuery<SpTrnPPOSDtlView>(string.Format("exec uspfn_spPPOSDtl4Table '{0}','{1}','{2}'", CompanyCode, BranchCode, model.POSNo)).ToList();
            string PORDS = GenPORDS(model, listPOSDtl);

            PORDS = PORDS.Split('\n')[0].Replace("<br/>", string.Empty);
            string qry = string.Format("select * from gnDcsUploadFile where DataID = '{0}' and Header = '{1}'", dataID, PORDS);
            var dt = ctx.Database.SqlQuery<GnDcsUploadFile>(qry);
            if (dt.Count() > 0)
            {
                result = false;
                msg = string.Format("Data {0} sudah pernah dikirim pada {1}, apakah akan dikirim ulang?", dataID, dt.FirstOrDefault().CreatedDate);
            }

            return Json(new { success = result, message = msg });
        }

        private void LogHeaderFile(string dataID, string custCode, string header, string prodType)
        {
            string query = "exec uspfn_spLogHeader @p0,@p1,@p2,@p3,@p4,@p5";
            object[] Parameters = { dataID, custCode, prodType, "SEND", DateTime.Now, header };
            ctx.Database.ExecuteSqlCommand(query, Parameters);
        }


        /// <summary>
        /// UpdateOnOrderStock
        /// </summary>
        /// <param name="partno"></param>
        /// <param name="onorder"></param>
        /// <param name="intransit"></param>
        /// <param name="received"></param>
        /// <returns></returns>
        private bool UpdateOnOrderStock(string partno, int onorder, decimal? intransit, int received)
        {
            bool md = DealerCode() == "MD";
            if (intransit == null)
            {
                intransit = 0;
            }
            bool result = false;
            spMstItem oItemDao = new spMstItem();
            //spMstItem oItem = md ? ctx.spMstItems.Find(CompanyCode, BranchCode, partno) : ctxMD.spMstItems.Find(CompanyMD, BranchMD, partno);
            var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                GetDbMD(), CompanyMD, BranchMD, partno);
            spMstItem oItem = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();

            decimal? tempOnOrder = oItem.OnOrder;
            decimal? tempInTransit = oItem.InTransit;
            decimal? tempOnHand = oItem.OnHand;

            if (oItem != null)
            {
                if (Math.Abs(onorder) > 0)
                {
                    oItem.OnOrder += onorder;

                    tempOnOrder = oItem.OnOrder;
                }

                if (Math.Abs((decimal)intransit) > 0)
                {
                    oItem.OnOrder -= intransit;
                    oItem.InTransit += intransit;

                    tempOnOrder = oItem.OnOrder;
                    tempInTransit = oItem.InTransit;
                }

                if (received > 0)
                {
                    oItem.InTransit -= received;
                    oItem.OnHand += received;

                    tempInTransit = oItem.InTransit;
                    tempOnHand = oItem.OnHand;
                }

                if (oItem.OnOrder < 0 || oItem.InTransit < 0)
                    return false;

                oItem.LastUpdateDate = DateTime.Now;
                oItem.LastUpdateBy = CurrentUser.UserId;

                string query = string.Format("update {0}..spMstItems set OnOrder='{1}',InTransit='{2}',OnHand='{3}'," +
                                               "LastUpdateDate='{4}',LastUpdateBy='{5}' " +
                                              " WHERE CompanyCode='{6}' AND BranchCode ='{7}' AND PartNo ='{8}'",
                                                GetDbMD(), tempOnOrder, tempInTransit, tempOnHand, DateTime.Now,
                                                CurrentUser.UserId, CompanyMD, BranchMD, partno);

                result = ctx.Database.ExecuteSqlCommand(query) > 0;
                if (result)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
                //try
                //{
                //    ctx.SaveChanges();
                //    result = true;
                //}
                //catch
                //{
                //    result = false;
                //}
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PosNo"></param>
        /// <returns></returns>
        private List<orderPartview> Select4NoPartOrderBalance(string PosNo)
        {
            var sql = string.Format("exec uspfn_Select4NoPartOrderBalance '{0}','{1}','{2}'", CompanyCode, BranchCode, PosNo);
            var ListOrderBalance = ctx.Database.SqlQuery<orderPartview>(sql).ToList();
            return ListOrderBalance;
        }


        /// <summary>
        /// Get No POS Used
        /// </summary>
        /// <param name="PosNo"></param>
        /// <returns></returns>
        private List<string> GetNoPosUsed(string PosNo)
        {
            string sql = @"SELECT
                Distinct(a.BinningNo) 
            FROM
                spTrnPBinnDtl a
            INNER JOIN spTrnPBinnHdr b ON
                a.CompanyCode = b.CompanyCode AND
                a.BranchCode = b.BranchCode   AND
                a.BinningNo = b.BinningNo
            WHERE
                a.CompanyCode = '{0}'    AND
                a.BranchCode = '{1}'      AND
                a.DocNo = '{2}'                AND
                b.Status NOT IN ('3')";
            sql = string.Format(sql, CompanyCode, BranchCode, PosNo);
            List<string> listPos = ctx.Database.SqlQuery<string>(sql).ToList();
            return listPos;
        }


        /// <summary>
        /// Prepare spTrnPPOSDtl befor insert info database
        /// </summary>
        /// <param name="record"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private spTrnPPOSDtl prepareSpTrnPPOSDtl(spTrnPPOSDtl record, spTrnPPOSDtl model)
        {

            record.OrderQty = model.OrderQty;
            record.PurchasePrice = model.PurchasePrice;
            record.DiscPct = model.DiscPct;
            record.PurchasePriceNett = Math.Round((decimal)(model.PurchasePrice - (model.PurchasePrice * (((model.DiscPct == null) ? 0 : model.DiscPct) / 100))));

            spMstItemPrice oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, model.PartNo);

            record.CostPrice = (oItemPrice == null) ? 0 : oItemPrice.CostPrice;

            decimal purchaseAmount = model.OrderQty.Value * model.PurchasePrice.Value;
            decimal discountAmount = Math.Round(purchaseAmount * (model.DiscPct.Value / 100), 0, MidpointRounding.AwayFromZero);
            record.TotalAmount = purchaseAmount - discountAmount;

            spMstItem oItem = ctx.spMstItems.Find(CompanyCode, BranchCode, model.PartNo);
            record.ABCClass = (oItem == null) ? "" : oItem.ABCClass;
            record.MovingCode = (oItem == null) ? "" : oItem.MovingCode;
            record.PartCategory = (oItem == null) ? "" : oItem.PartCategory;
            record.ProductType = CurrentUser.CoProfile.ProductType;
            record.Note = model.Note;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            return record;
        }

        public JsonResult CheckOTValue(string lookupValue)
        {
            var paraValue = ctx.LookUpDtls.FirstOrDefault(a => a.CodeID == "ORTP" && a.LookUpValue == lookupValue).ParaValue;
            return Json(new { paravalue = paraValue });
        }


        public JsonResult spTrnPPOSHdrBrowse()
        {
            string sql = string.Format(@"EXEC uspfn_spTrnPPOSHdr_Web '{0}', '{1}', '{2}', '{3}', '{4}', '{5}'",
                CompanyCode, BranchCode, TypeOfGoods, "2", Helpers.GetDynamicFilter(Request), 500);

            var records = ctx.Database.SqlQuery<spTrnPPOSHdrView>(sql).AsQueryable();
            return Json(records.toKG(ApplyFilterKendoGrid.False));            
        }

        public JsonResult GetLookupEOS_FLAG()
        {
            string bEnabled = "0";
            var record = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "DISC_EOS_FLAG" && p.LookUpValue == BranchCode).FirstOrDefault();
            if (record != null)
                bEnabled = record.ParaValue;

            return Json (new { success = true, enabled = (bEnabled == "0") ? true : false });
        }

        public JsonResult doExport(string Branch, string PMonth, string PYear)
        {
            var result = ctx.Database.SqlQuery<AOSModel>("exec uspfn_GetAOSByPeriode '" + Branch + "','" + PMonth + "','" + PYear + "'").ToList();
            var message = "";

            var MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(Convert.ToInt16(PMonth));

            try
            {
                var package = new ExcelPackage();
                package = GenerateExcel(package, MonthName, PYear, result);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { success = true, message = message, value = guid });
            }
            catch (Exception e)
            {
                message = e.Message;
                return Json(new { success = false, message = message });
            }
        }

        private static ExcelPackage GenerateExcel(ExcelPackage package, string pMonth, string pYear, List <AOSModel> result)
        {
            var sheet = package.Workbook.Worksheets.Add("AOSLog");
            var z = sheet.Cells[1, 1];
            //var data = result.ResultSetFor<AOSModel>().ToList();

            var data = result;

            #region -- Constants --
            const int
                rTitle = 1,
                rPMonth = 2,
                rPYear = 3,
                rHeader1 = 4,
                rData = 5,

                cStart = 1,
                cNo = 1,
                cDealerAbb = 2,
                cOutletAbb = 3,
                cPOSDate = 4,
                cPOSNo = 5,
                cPartNo = 6,
                cPartName = 7,
                cQtySuggor = 8,
                cStatus = 9,
                cEnd = 9;

            double
                wNo = EP.GetTrueColWidth(5),
                wAbb = EP.GetTrueColWidth(25),
                wPOS = EP.GetTrueColWidth(18),
                wPartNo = EP.GetTrueColWidth(18),
                wPartName = EP.GetTrueColWidth(65),
                wStatus = EP.GetTrueColWidth(12);

            const string
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";
            #endregion

            sheet.Column(cNo).Width = wNo;
            sheet.Column(cDealerAbb).Width = wStatus;
            sheet.Column(cOutletAbb).Width = wAbb;
            sheet.Column(cPOSDate).Width = wStatus;
            sheet.Column(cPOSNo).Width = wPOS;
            sheet.Column(cPartNo).Width = wPartNo;
            sheet.Column(cPartName).Width = wPartName;
            sheet.Column(cQtySuggor).Width = wStatus;
            sheet.Column(cStatus).Width = wStatus;
           
            #region -- Title --
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cEnd);
            z.Value = "AOS LOG REPORT";
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetRange(rPMonth, cNo, rPMonth, cDealerAbb);
            z.Value = "Periode Bulan";
            z.Merge = true;
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rPMonth, cOutletAbb);
            z.Value = ": " + pMonth;
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rPYear, cNo, rPYear, cDealerAbb);
            z.Value = "Periode Tahun";
            z.Merge = true;
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rPYear, cOutletAbb);
            z.Value = ": " + pYear;
            z.Style.Font.Bold = true;
            #endregion

            #region -- Header --
            z.Address = EP.GetRange(rHeader1, cStart, rHeader1, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
            z.Style.Font.Bold = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z.Address = EP.GetRange(rHeader1, cNo, rHeader1, cNo);
            z.Value = "No";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cDealerAbb);
            z.Value = "Dealer";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cOutletAbb);
            z.Value = "Outlet";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cPOSDate);
            z.Value = "Create Date";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cPOSNo);
            z.Value = "POS No";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cPartNo);
            z.Value = "Part No";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cPartName);
            z.Value = "Part Name";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cQtySuggor);
            z.Value = "Suggor Qty";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader1, cStatus);
            z.Value = "Status";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            #endregion

            #region -- Data --
            if (data.Count == 0) return package;
            for (int i = 0; i < data.Count; i++)
            {
                var row = rData + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cNo, Value = i + 1 },
                    new ExcelCellItem { Column = cDealerAbb, Value = data[i].DealerAbb },
                    new ExcelCellItem { Column = cOutletAbb, Value = data[i].OutletAbb },
                    new ExcelCellItem { Column = cPOSDate, Value = data[i].POSDate },
                    new ExcelCellItem { Column = cPOSNo, Value = data[i].POSNo },
                    new ExcelCellItem { Column = cPartNo, Value = data[i].PartNo },
                    new ExcelCellItem { Column = cPartName, Value = data[i].PartName },
                    new ExcelCellItem { Column = cQtySuggor, Value = data[i].SuggorQty },
                    new ExcelCellItem { Column = cStatus, Value = data[i].SStatus },
                };

                foreach (var item in items)
                {
                    z.Address = EP.GetCell(row, item.Column);
                    z.Value = item.Value;
                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                }
            }
            #endregion

            //#region -- Total --
            //var rTotal = data.Count + rData;
            //z.Address = EP.GetRange(rTotal, cStart, rTotal, cEnd);
            //z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 241, 221));
            //z.Style.Font.Bold = true;
            ////z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ////z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //z.Address = EP.GetRange(rTotal, cPeriode, rTotal, cOutlet);
            //z.Value = "TOTAL";
            //z.Merge = true;
            //z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            //var sums = new List<ExcelCellItem>
            //{
            //    //new ExcelCellItem { Column = cPeriode, Value = "TOTAL" },
            //    //new ExcelCellItem { Column = cOutlet, Value = "" },
            //    new ExcelCellItem { Column = cDoData, Value = total[0].TotDoData },
            //    new ExcelCellItem { Column = cDelivery, Value = total[0].TotDeliveryDate },
            //    new ExcelCellItem { Column = cTdCallByDO, Value = total[0].TotTDaysCallData },
            //    new ExcelCellItem { Column = cTdCallByInput, Value = total[0].TotTDaysCallByInput },
            //};

            //foreach (var sum in sums)
            //{
            //    z.Address = EP.GetCell(rTotal, sum.Column);
            //    z.Value = sum.Value;
            //    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            //    z.Style.Numberformat.Format = sum.Format != null ? sum.Format : fCustom;
            //}

            //sheet.Row(rTotal).Height = hTotal;

            //#endregion

            return package;
        }

        public FileContentResult DownloadExcelFile(string key, string fileName)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=" + fileName + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        private class AOSModel
        {
            public string DealerAbb { get; set; }
            public string OutletAbb { get; set; }
            public string POSDate { get; set; }
            public string POSNo { get; set; }
            public string PartNo { get; set; }
            public string PartName { get; set; }
            public Decimal SuggorQty { get; set; }
            public string SStatus { get; set; }
        }
    }
}
