using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.BLL;
using SimDms.Sales.Models;
using SimDms.Common;
using System.Transactions;
using TracerX;

namespace SimDms.Sales.Controllers.Api
{
    public class InvoiceController : BaseController
    {
        public JsonResult Save(OmTrSalesInvoice mdl)
        {
            try
            {
                string serr = "";
                if (!DateTransValidation((DateTime)mdl.InvoiceDate, ref serr))
                {
                    return Json(new { success = false, message = serr });
                }

                if (mdl.DueDate < mdl.InvoiceDate)
                {
                    return Json(new { success = false, message = "Tanggal jatuh tempo tidak boleh lebih kecil daripada tanggal invoice" });
                }

                if (ctx.Database.SqlQuery<omSlsInvBrowse>(string.Format("exec uspfn_omSlsInvLkpSO {0},{1}",
                          CompanyCode, BranchCode))
                    .Where(x => x.SONo == mdl.SONo).FirstOrDefault() == null)
                {
                    return Json(new { success = false, message = "Invalid SONo" });

                }

                var hdr = ctx.OmTrSalesInvoices.Find(CompanyCode, BranchCode, mdl.InvoiceNo);
                if (hdr == null)
                {
                    hdr = new OmTrSalesInvoice();
                    hdr.CompanyCode = CompanyCode;
                    hdr.BranchCode = BranchCode;
                    hdr.InvoiceNo = GetNewDocumentNo("IVU", (DateTime)mdl.InvoiceDate);
                    hdr.LockingBy = MyLogger.GetCRC32(hdr.CompanyCode + hdr.BranchCode + hdr.InvoiceNo);
                    hdr.CreatedBy = CurrentUser.UserId;
                    hdr.CreatedDate = DateTime.Now;
                    hdr.isLocked = false;
                    ctx.OmTrSalesInvoices.Add(hdr);
                }

                hdr.InvoiceDate = mdl.InvoiceDate;
                hdr.SONo = mdl.SONo;
                hdr.CustomerCode = mdl.CustomerCode;
                hdr.BillTo = mdl.BillTo;
                hdr.DueDate = mdl.DueDate;

                var cust = ctx.GnMstCustomer.Find(CompanyCode, mdl.CustomerCode);
                if (cust != null)
                {
                    hdr.isStandard = cust.isPKP;
                }
                else
                {
                    //XMessageBox.ShowWarning("Tidak menemukan master pelanggan", "ERROR!"); return null;
                }
                hdr.Remark = mdl.Remark;
                hdr.Status = "0";
                hdr.LastUpdateBy = CurrentUser.UserId;
                hdr.LastUpdateDate = ctx.CurrentTime;
                Helpers.ReplaceNullable(hdr);
                ctx.SaveChanges();
                return Json(new { success = true, InvoiceNo = hdr.InvoiceNo, InvoiceDate = hdr.InvoiceDate, Status = hdr.Status, StatusDsc = getStringStatus(hdr.Status) });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(OmTrSalesInvoice mdl)
        {
            try
            {
                var record = ctx.OmTrSalesInvoices.Find(CompanyCode, BranchCode, mdl.InvoiceNo);
                if (record != null)
                {
                    if (record.Status != "0" && record.Status != "1")
                    {
                        return Json(new { success = false, message = "Invalid Status" });
                    }
                }
                if (record == null)
                {
                    return Json(new { success = false, message = "Invalid Status" });
                }

                OmTrSalesBLL.Instance(CurrentUser.UserId).DeleteInv(ctx, record);

                return Json(new { success = true, InvoiceNo = record.InvoiceNo, Status = record.Status, StatusDsc = getStringStatus(record.Status) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult Approve(OmTrSalesInvoice mdl)
        {
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))

                {
                    bool result = false;
                    var record = ctx.OmTrSalesInvoices.Find(CompanyCode, BranchCode, mdl.InvoiceNo);
                    if (record == null)
                    {
                        return Json(new { success = false, message = "Invalid Status" });
                    }

                    // Insert Invoice AccsSeq
                    #region SaveInvoiceAccsSeq

                    result = ctx.Database.ExecuteSqlCommand("exec uspfn_omSaveInvoiceAccSeq '" + CompanyCode + "','" + BranchCode + "','" + mdl.InvoiceNo + "','" + mdl.SONo + "','" + CurrentUser.UserId + "'") > -1;
                    #endregion

                    var dtPart = ctx.Database.SqlQuery<omSlsInvSlctFrTblInvAccSeq>(string.Format("exec uspfn_omSlsInvSlctFrTblInvAccSeq {0},{1},'{2}'", CompanyCode, BranchCode, mdl.InvoiceNo));

                    //if (dtPart.Count() > 0)
                    //{
                    //    XDataGridView.SetDataSource(lvPart, dtPart);
                    //}

                    //Cek sudah jalankan pengeluaran stok atau belum

                    var accRec = ctx.OmTrSalesSOAccsSeqs.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SONo == mdl.SONo).FirstOrDefault();
                    if (accRec != null)
                    {
                        if (!OmTrSalesBLL.Instance(CurrentUser.UserId).CekItemOut(ctx, record.SONo))
                        {
                            return Json(new { success = false, message = "Silahkan jalankan proses pengeluaran part accesories terlebih dahulu!" });
                        }
                    }
                    //todo :: cek so apakah so punya sparepart, jika tidak, lewati method ini
                    //jika punya sparepart, cek apakah di inv
                                        
                    if (!OmTrSalesBLL.Instance(CurrentUser.UserId).AllPartIsGet(ctx, record))
                    {
                        return Json(new { success = false, message = "Terdapat part yang belum di ambil" }); ;
                    }

                    // Cek Accessories
                    if (!OmTrSalesBLL.Instance(CurrentUser.UserId).CekStatusAccessories(ctx, record.SONo))
                        return Json(new { success = false, message = "Invalid Status Accessories" }); ;

                    string msg = string.Empty;
                    result = OmTrSalesBLL.Instance(CurrentUser.UserId).ApproveInv(ctx, record, ref msg);
                    
                    if (result)
                    {
                        tranScope.Complete();
                        return Json(new { success = true, Status = record.Status, StatusDsc = getStringStatus(record.Status) });
                    }
                    else
                    {
                        return Json(new { success = false, message = msg });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult Print(OmTrSalesInvoice mdl)
        {
            try
            {
                //check whether detail model data doesn't exist                
                var dtModel = ctx.omTrSalesInvoiceModel
                                      .Where(x => x.CompanyCode == CompanyCode &&
                                          x.BranchCode == BranchCode &&
                                          x.InvoiceNo == mdl.InvoiceNo)
                                          .ToList();

                if (dtModel.Count() < 1)
                {
                    return Json(new { success = false, message = "Dokumen tidak dapat dicetak karena tidak memiliki data detail" });
                }

                var record = ctx.OmTrSalesInvoices
                            .Find(CompanyCode, BranchCode, mdl.InvoiceNo);

                if (record == null)
                    return Json(new { success = false, message = "Invalid Invoice" });

                string flag = ctx.LookUpDtls
                               .Where(x => x.CompanyCode == CompanyCode &&
                                            x.CodeID == "INV_FRM" &&
                                            x.LookUpValue == "FRM_SLS")
                                .Select(x => x.ParaValue)
                                .FirstOrDefault();
                string infoDebet = ctx.LookUpDtls
                    .Where(x => x.CompanyCode == CompanyCode &&
                               x.CodeID == "DBT_INFO")
                    .FirstOrDefault()
                    .ParaValue;

                if (record.Status == "0" || record.Status.Trim() == "")
                {
                    record.Status = "1";
                }
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctx.CurrentTime;
                ctx.SaveChanges();

                return Json(new { success = true, flag = flag, infoDebet = infoDebet, Status = record.Status, StatusDsc = getStringStatus(record.Status), message = "" });

                //return Json(new { success = true, flag = "UMC", infoDebet = infoDebet, Status = record.Status, StatusDsc = getStringStatus(record.Status), message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult addDtlBpk(omTrSalesInvoiceBPK mdl, bool isAll)
        {
            try
            {
                
                //using (var tranScope = new TransactionScope(TransactionScopeOption.Suppress))
                using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    var record = ctx.OmTrSalesInvoices.Find(CompanyCode, BranchCode, mdl.InvoiceNo);
                    if (record != null)
                    {
                        if (record.Status != "0" && record.Status != "1")
                            return Json(new { success = false, message = "Invalid Status" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid Invoice" });
                    }


                    if (!OmTrSalesBLL.Instance(CurrentUser.UserId)
                        .isBPKDateOK(ctx, new omTrSalesInvoiceBPK() { BPKNo = mdl.BPKNo, InvoiceNo = mdl.InvoiceNo }))
                    {
                        return Json(new { success = false, message = "Tanggal BPK tidak sama dengan tanggal BPK yang sudah ada" });
                    }

                    bool isNew = false;
                    string strUID = "";
                    //recordInvBPK = prepareRecordInvBPK(ref isNew);
                    #region  prepareRecordInvBPK
                    var recordInvBPK = ctx.omTrSalesInvoiceBPK.Find(CompanyCode, BranchCode, mdl.InvoiceNo, mdl.BPKNo);
                    if (recordInvBPK == null)
                    {
                        isNew = true;
                        recordInvBPK = new omTrSalesInvoiceBPK();
                        recordInvBPK.CompanyCode = CompanyCode;
                        recordInvBPK.BranchCode = BranchCode;
                        recordInvBPK.InvoiceNo = mdl.InvoiceNo;
                        recordInvBPK.BPKNo = mdl.BPKNo;
                        strUID = MyLogger.GetCRC32(CompanyCode + BranchCode + mdl.InvoiceNo + mdl.BPKNo) + CurrentUser.UserId;
                        if (strUID.Length > 15)
                        {
                            recordInvBPK.CreatedBy = strUID.Substring(0, 15);
                        }
                        else
                        {
                            recordInvBPK.CreatedBy = strUID;
                        }
                        
                        recordInvBPK.CreatedDate = ctx.CurrentTime;
                        ctx.omTrSalesInvoiceBPK.Add(recordInvBPK);
                    }
                    recordInvBPK.Remark = mdl.Remark == null ? "" : mdl.Remark;

                    recordInvBPK.LastUpdateBy = CurrentUser.UserId;
                    recordInvBPK.LastUpdateDate = ctx.CurrentTime;


                    #endregion
                    record.Status = "0";
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = ctx.CurrentTime;
                    Helpers.ReplaceNullable(recordInvBPK);
                    bool result = ctx.SaveChanges() >= 0;


                    if (isAll)
                    {
                        if (!OmTrSalesBLL.Instance(CurrentUser.UserId).InvInsertAllBPK(ctx, recordInvBPK, record))
                        {
                            return Json(new { success = false, message = "Data tidak valid atau COGS Unit tersebut masih 0" });
                        }
                    }
                    tranScope.Complete();
                    return Json(new { success = true, InvoiceNo = record.InvoiceNo, Status = record.Status, StatusDsc = getStringStatus(record.Status) });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult delDtlBpk(omTrSalesInvoiceBPK mdl)
        {
            try
            {
                var record = ctx.OmTrSalesInvoices.Find(CompanyCode, BranchCode, mdl.InvoiceNo);
                if (record != null)
                {
                    if (record.Status != "0" && record.Status != "1")
                    {
                        return Json(new { success = false, message = "Invoice Not Found" });
                    }
                }

                if (mdl.BPKNo == "" || mdl.BPKNo == null)
                    return Json(new { success = false, message = "Invoice Not Found" });

                var recordInvBPK = ctx.omTrSalesInvoiceBPK.Find(CompanyCode, BranchCode, mdl.InvoiceNo, mdl.BPKNo);
                if (recordInvBPK == null)
                {
                    return Json(new { success = false, message = "Invoice BPK Not Found" });
                }
                OmTrSalesBLL.Instance(CurrentUser.UserId).DeleteInvBPK(ctx, recordInvBPK, record);
                return Json(new { success = true, hdrStatus = record.Status, hdrStatusDsc = getStringStatus(record.Status) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult cekDataInvoiceVin(string invNo)
        {
            try
            {
                var qry="";
                int iRec = 0;
                bool hasil = false;
                ctx.omTrSalesInvoiceModel.Where(x => x.CompanyCode == CompanyCode &&
                       x.BranchCode == BranchCode &&
                       x.InvoiceNo == invNo).ToList().ForEach( mdl => 
                {
                    qry = "SELECT Count(*) FROM omTrSalesInvoiceVin WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "' AND BPKNo='" + mdl.BPKNo + "' AND SalesModelCode='" + mdl.SalesModelCode + "' AND SalesModelYear='" + mdl.SalesModelYear + "'";
                    iRec = ctx.Database.SqlQuery<int>(qry).FirstOrDefault();
                    if (iRec == mdl.Quantity)
                    {
                        hasil = true;
                        //return Json(new { success = true, message = "" });
                        //var record = ctx.omTrSalesInvoiceVin.Find(CompanyCode, BranchCode, mdl.BPKNo, row.SalesModelCode, row.SalesModelYear);
                    }
                    else
                    {
                        hasil = false;
                    }
                });

                //if (mdl != null)
                //{
                //    var qry = "SELECT Count(*) FROM omTrSalesInvoiceVin WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "' AND BPKNo='" + mdl.BPKNo + "' AND SalesModelCode='" + mdl.SalesModelCode + "' AND SalesModelYear='" + mdl.SalesModelYear + "'";
                //    int iRec = ctx.Database.SqlQuery<int>(qry).FirstOrDefault();
                //    if (iRec == mdl.Quantity)
                //    {
                //        return Json(new { success = true, message = "" });
                //        //var record = ctx.omTrSalesInvoiceVin.Find(CompanyCode, BranchCode, mdl.BPKNo, row.SalesModelCode, row.SalesModelYear);
                //    }
                //}

                if (hasil)
                {
                    return Json(new { success = true, message = "" });
                }
                else
                {
                    return Json(new { success = false, message = "Invoice tidak ada di Sales Invoice Vin, silahkan cek Price List Branch untuk unit ini!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult addDtlSlsMdl(omTrSalesInvoiceModel mdl)
        {
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout =  TimeSpan.FromMinutes(30) }))
                {
                    var record = ctx.OmTrSalesInvoices.Find(CompanyCode, BranchCode, mdl.InvoiceNo);

                    if (record == null)
                    {
                        return Json(new { success = false, message = "Invoice Not Found" });
                    }

                    if (record.Status != "0" && record.Status != "1")
                    {
                        return Json(new { success = false, message = "Invalid Invoice" });
                    }


                    var row = ctx.Database.SqlQuery<omSlsInvSlctMdlVldt>
                        (string.Format("exec uspfn_omSlsInvSlctModelVldt {0},{1},'{2}','{3}'", CompanyCode, BranchCode, mdl.BPKNo, mdl.InvoiceNo))
                        .AsQueryable()
                        .Where(x => x.SalesModelCode == mdl.SalesModelCode)
                        .FirstOrDefault();

                    if (row == null)
                    {
                        return Json(new { success = false, message = "Invalid Model" });
                    }

                    var row2 =
                        ctx.Database.SqlQuery<omSlsInvSlctMdlYrVldt>
                        (string.Format("exec uspfn_omSlsInvSlctMdlYrVldt {0},{1},'{2}','{3}','{4}'", CompanyCode, BranchCode, mdl.BPKNo, mdl.InvoiceNo, mdl.SalesModelCode))
                        .AsQueryable()
                        .Where(x => x.SalesModelYear == mdl.SalesModelYear)
                        .FirstOrDefault();

                    if (row2 == null)
                    {
                        return Json(new { success = false, message = "Invalid Model" });
                    }

                    bool isNew = false;
                    string strUID = "";
                    var recordInvModel = ctx.omTrSalesInvoiceModel
                                        .Find(CompanyCode, BranchCode, mdl.InvoiceNo, mdl.BPKNo, mdl.SalesModelCode, mdl.SalesModelYear);

                    #region prepareRecordInvModel
                    if (recordInvModel == null)
                    {
                        isNew = true;
                        recordInvModel = new omTrSalesInvoiceModel();
                        recordInvModel.CompanyCode = CompanyCode;
                        recordInvModel.BranchCode = BranchCode;
                        recordInvModel.InvoiceNo = mdl.InvoiceNo;
                        recordInvModel.BPKNo = mdl.BPKNo;
                        recordInvModel.SalesModelCode = mdl.SalesModelCode;
                        recordInvModel.SalesModelYear = mdl.SalesModelYear;
                        strUID = MyLogger.GetCRC32(CompanyCode + BranchCode + mdl.InvoiceNo + mdl.BPKNo + mdl.SalesModelCode + Convert.ToString(mdl.SalesModelYear)) + CurrentUser.UserId;
                        if (strUID.Length > 15)
                        {
                            recordInvModel.CreatedBy = strUID.Substring(0, 15);
                        }
                        else
                        {
                            recordInvModel.CreatedBy = strUID;
                        }
                        recordInvModel.CreatedDate = ctx.CurrentTime;
                    }

                    recordInvModel.Quantity = mdl.Quantity;

                    var so = ctx.OmTrSalesSOModels
                                .Find(CompanyCode, BranchCode, record.SONo, mdl.SalesModelCode, mdl.SalesModelYear);
                    if (so != null)
                    {
                        recordInvModel.AfterDiscDPP = so.AfterDiscDPP;
                        recordInvModel.AfterDiscPPn = so.AfterDiscPPn;
                        recordInvModel.AfterDiscPPnBM = so.AfterDiscPPnBM;
                        recordInvModel.AfterDiscTotal = so.AfterDiscTotal;
                        recordInvModel.BeforeDiscDPP = so.BeforeDiscDPP;
                        recordInvModel.DiscExcludePPn = so.DiscExcludePPn;
                        recordInvModel.DiscIncludePPn = so.DiscIncludePPn;
                        recordInvModel.OthersDPP = so.OthersDPP;
                        recordInvModel.OthersPPn = so.OthersPPn;
                        recordInvModel.ShipAmt = so.ShipAmt;
                        recordInvModel.DepositAmt = so.DepositAmt;
                        recordInvModel.OthersAmt = so.OthersAmt;

                    }

                    recordInvModel.QuantityReturn = 0;
                    recordInvModel.Remark = mdl.Remark == null ? "" : mdl.Remark;

                    recordInvModel.LastUpdateBy = CurrentUser.UserId;
                    recordInvModel.LastUpdateDate = ctx.CurrentTime;
                    Helpers.ReplaceNullable(recordInvModel);
                    #endregion

                    if(!OmTrSalesBLL.Instance(CurrentUser.UserId).SaveInvModel(ctx, recordInvModel, record, isNew))
                    {
                        return Json(new { success = false, message="Proses Save Invoice Model gagal atau COGS unit ini 0!" });
                    }
                    tranScope.Complete();
                    return Json(new { success = true, hdrStatus = record.Status, hdrStatusDsc = getStringStatus(record.Status) });

                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult delDtlSlsMdl(omTrSalesInvoiceModel mdl)
        {
            try
            {
                var record = ctx.OmTrSalesInvoices.Find(CompanyCode, BranchCode, mdl.InvoiceNo);
                if (record != null)
                {
                    if (record.Status != "0" && record.Status != "1")
                    {
                        return Json(new { success = false, message = "Invoice Not Found" });
                    }
                }

                if (mdl.SalesModelCode == null || mdl.SalesModelYear == null)
                {
                    return Json(new { success = false, message = "Sales Model Code amd Sales Model Year Must be Filled" });
                }

                var recordInvModel = ctx.omTrSalesInvoiceModel
                                .Find(CompanyCode, BranchCode, mdl.InvoiceNo, mdl.BPKNo, mdl.SalesModelCode, mdl.SalesModelYear);

                OmTrSalesBLL.Instance(CurrentUser.UserId).DeleteInvModel(ctx, recordInvModel, record);

                return Json(new { success = true, message = "", Status = record.Status, StatusDsc = getStringStatus(record.Status) }); ;

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }); ;
            }
        }

        public JsonResult vldtSlsMdlYr(omTrSalesInvoiceModel mdl)
        {
            return Json(ctx.Database
                        .SqlQuery<MstModelYearDesc>("exec uspfn_omSlsInvSlsMdlYearVldt {0},{1},{2},{3},{4}",
                                                    CompanyCode, BranchCode, mdl.InvoiceNo, mdl.BPKNo, mdl.SalesModelCode)                        
                        .FirstOrDefault());
        }

        public JsonResult modeldetil(omTrSalesInvoiceModel mdl,string SONo)
        {
            try 
	        {	        
                   if ( mdl.SalesModelYear<=0)
                   {
                       return Json(new { success = false, message = "" });
                   }
                       
                var dt =  ctx.Database
                        .SqlQuery<MstModelYearDesc>("exec uspfn_omSlsInvSlsMdlYearVldt {0},{1},{2},{3},{4}",
                                                    CompanyCode, BranchCode, mdl.InvoiceNo, mdl.BPKNo, mdl.SalesModelCode);
                

                    

                var row =dt.Where(x=>x.SalesModelYear==mdl.SalesModelYear).FirstOrDefault();
                if (row != null)
                {
                    //txtSalesModelYear.Text = row[0]["SalesModelYear"].ToString() ?? "";
                    //txtSalesModelDesc.Text = (string)row[0]["SalesModelDesc"] ?? "";
                    //first, check whether data exists in omtrsalesinvoiceModel, if it does, populate model
                    //from omtrsalesinvoiceModel, otherwise, populate value from omtrsalessoModel
                    var recordInvModel = ctx.omTrSalesInvoiceModel.Find(CompanyCode, BranchCode, mdl.InvoiceNo, mdl.BPKNo, mdl.SalesModelCode, mdl.SalesModelYear);

                    if (recordInvModel != null)
                        return Json(new { success = true, isinvmodel = true, data = recordInvModel, message = "" });
                    else
                    {
                        var so = ctx.OmTrSalesSOModels.Find(CompanyCode, BranchCode, SONo, mdl.SalesModelCode, mdl.SalesModelYear);
                        if (so != null)
                        {
                            //SetSalesModelValue(so);
                            var bpkModel = ctx.OmTrSalesBPKModels.Find(CompanyCode, BranchCode,
                                mdl.BPKNo, mdl.SalesModelCode, mdl.SalesModelYear);
                            //if (bpkModel != null) txtQuantity.Text = bpkModel.QuantityBPK.ToString();

                            return Json(new { success = true, isinvmodel = false, somdl = so, bpkmdl = bpkModel });
                        }
                    }
                }
                return Json(new { success = false,message="record not found" });
		
	            }
	            catch (Exception ex)
	            {

		             return Json(new { success = false,message=ex.Message}); 
	            }
         
        }


        public JsonResult validatebpk(omTrSalesInvoiceBPK mdl)
        {
            try
            {

                var dt = ctx.omTrSalesInvoiceBPK.Where(x =>

                            x.CompanyCode == CompanyCode &&
                             x.BranchCode == BranchCode &&
                                 x.BPKNo == mdl.BPKNo)
                            .FirstOrDefault();
                             
                if (dt==null  )
                {
                    return Json(new { success = true, message =""});
                }
                else
                {                    
                    return Json(new { success = false, message = "No BPK : " + dt.BPKNo+ ", sudah digunakan di No Invoice : " + dt.InvoiceNo });                    
                }
            }
            catch (Exception ex)
            {

                return Json(new {success=false,message=ex.Message });
            }
        }

      
    }
}
