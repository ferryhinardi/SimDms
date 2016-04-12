using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using SimDms.Sparepart.Models;
using SimDms.Common.Models;
using System.Data.SqlClient;
using System.Data;
using TracerX;


namespace SimDms.Sparepart.Controllers.Api
{
    public class PengeluaranController : BaseController
    {
        private const string ProfitCenter = "300";
        private const string ProfitCenterName = "SPARE PART";

        [HttpPost]
        public JsonResult CustomerData(string PickingSlipNo, string CustomerCode)
        {
            var pickingSlipNo = HttpContext.Request["PickingSlipNo"];
            var customerCode = HttpContext.Request["CustomerCode"];

            var sql = string.Format("exec uspfn_getCustSPTrans '{0}','{1}'", pickingSlipNo, customerCode);
            var data = ctx.Database.SqlQuery<SpTrnSInvoiceDtl>(sql).FirstOrDefault();
            return Json(data);
        }

        public JsonResult TrnInvoiceDetails(string InvoiceNo)
        {
            InvoiceNo = HttpContext.Request.QueryString["InvoiceNo"];
            var sql = string.Format("exec uspfn_SPGetSpTranDetails '{0}','{1}','{2}'", CompanyCode, BranchCode, InvoiceNo);
            var queryable = ctx.Database.SqlQuery<CustomerTransDtl>(sql).AsQueryable();
            return Json(queryable);
        }

        public JsonResult GetSpTrnSFPJDtl(string FPJNo)
        {
            var listDtl = GetSpTrnsFPJDtlList(FPJNo);
            return Json(listDtl);
        }

        public List<SpTrnSFPJDtlView> GetSpTrnsFPJDtlList(string FPJNo)
        {
            var sql = string.Format("exec uspfn_spGetTrnSFPJDtl '{0}','{1}','{2}'", CompanyCode, BranchCode, FPJNo);
            var listDtl = ctx.Database.SqlQuery<SpTrnSFPJDtlView>(sql).ToList();
            return listDtl;
        }

        public JsonResult GetTrnSInvoiceDtl(string InvoiceNo)
        {
            string sql = string.Format("exec uspfn_spGetTrnSInvoiceDtl '{0}','{1}','{2}'", CompanyCode, BranchCode, InvoiceNo);
            var listInvoiceDtl = ctx.Database.SqlQuery<SpTrnSFPJDtlView>(sql).ToList();
            return Json(listInvoiceDtl);
        }


        [HttpPost]
        public JsonResult CheckStatus(string WhereValue, string Table, string ColumnName)
        {
            string status = "", statusCode = "";
            //var recordCheck = ctx.SpTrnSFPJHdrs.Find(CompanyCode, BranchCode, NoFakturPajak);
            var sql = string.Format("select Status from {0} where CompanyCode='{1}' and BranchCode = '{2}' and {3} = '{4}'", Table, CompanyCode, BranchCode, ColumnName, WhereValue);
            statusCode = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
            if (!string.IsNullOrEmpty(statusCode))
            {
                var oLookup = ctx.LookUpDtls.Where(m => m.CompanyCode == CompanyCode && m.CodeID == "STAT" && m.LookUpValue == statusCode).FirstOrDefault();
                if (oLookup != null)
                    status = oLookup.LookUpValueName;
            }
            return Json(new { success = true, statusPrint = status, statusCode = statusCode });
        }


        [HttpPost]
        public JsonResult UpdateStatusFakturPajak(string NoFakturPajak)
        {
            bool successRtr = false;
            var recordCheck = ctx.SpTrnSFPJHdrs.Find(CompanyCode, BranchCode, NoFakturPajak);
            if (recordCheck != null)
            {
                if (recordCheck.Status.ToString().Equals("0"))
                {
                    recordCheck.Status = "1";
                    recordCheck.LastUpdateBy = CurrentUser.UserId;
                    recordCheck.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                }
                successRtr = true;
            }
            return Json(new { success = successRtr });
        }

        [HttpPost]
        public JsonResult Save(PembFakturPenjualan model)
        {
            SpTrnSFPJHdr oSpTrnSFPJHdr = null;
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    //ctx.Database.CommandTimeout = (int)TimeSpan.FromHours(1).TotalSeconds;
                    bool isNew = false;
                    SpTrnSInvoiceHdr recordInvHdr = null;
                    SpTrnSFPJInfo spTrnSFPJInfo = null;
                    oSpTrnSFPJHdr = ctx.SpTrnSFPJHdrs.Find(CompanyCode, BranchCode, model.FPJNo);
                    if (oSpTrnSFPJHdr == null)
                    {
                        recordInvHdr = ctx.SpTrnSInvoiceHdrs.Find(CompanyCode, BranchCode, model.InvoiceNo);
                        if (recordInvHdr == null)
                        {
                            throw new Exception("Data Invoice tidak ditemukan.");
                        }

                        SpTrnSPickingHdr recordPLR = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, model.PickingSlipNo);

                        if (recordPLR == null)
                        {
                            throw new Exception("Data Picking tidak ditemukan.");
                        }


                        if (recordInvHdr.Status.Equals("2"))
                        {
                            string message = ctx.SysMsgs.Find("5045").MessageCaption;
                            var msg = string.Format(message, model.InvoiceNo, "Generated");//string.Format(ctx.SysMsgs.Find("1101"),model.InvoiceNo, "Generated");
                            //throw new Exception("Error,"+msg);
                            return Json(new { success = false, message = msg });
                            //throw error Message, Invoice sudah pernah di generate
                        }

                        isNew = true;
                        //new record
                        oSpTrnSFPJHdr = new SpTrnSFPJHdr();
                        oSpTrnSFPJHdr.CompanyCode = CompanyCode;
                        oSpTrnSFPJHdr.BranchCode = BranchCode;
                        oSpTrnSFPJHdr.FPJDate = DateTime.Now;
                        oSpTrnSFPJHdr.InvoiceNo = model.InvoiceNo;
                        oSpTrnSFPJHdr.InvoiceDate = recordInvHdr.InvoiceDate; //model.InvoiceDate;
                        oSpTrnSFPJHdr.PickingSlipNo = model.PickingSlipNo;
                        oSpTrnSFPJHdr.PickingSlipDate = recordInvHdr.PickingSlipDate; //model.PickingSlipDate;
                        oSpTrnSFPJHdr.TransType = recordInvHdr.TransType;  //model.TransType;
                        oSpTrnSFPJHdr.CustomerCode = recordInvHdr.CustomerCode;  //model.CustomerCode;
                        oSpTrnSFPJHdr.CustomerCodeBill = recordInvHdr.CustomerCodeBill;  //model.CustomerCodeBill;
                        oSpTrnSFPJHdr.CustomerCodeShip = recordInvHdr.CustomerCodeShip;  //model.CustomerCodeShip;
                        oSpTrnSFPJHdr.FPJCentralDate = Convert.ToDateTime("1900-01-01");
                        oSpTrnSFPJHdr.LockingDate = Convert.ToDateTime("1900-01-01");
                        oSpTrnSFPJHdr.FPJCentralNo = "";

                        var CustProfitCenter = ctx.MstCustomerProfitCenters.Find(CompanyCode, BranchCode, model.CustomerCode, "300");
                        if (CustProfitCenter != null)
                        {
                            oSpTrnSFPJHdr.TOPCode = CustProfitCenter.TOPCode;
                            var TopDays = ctx.LookUpDtls.Find(CompanyCode, "TOPC", CustProfitCenter.TOPCode).ParaValue;
                            oSpTrnSFPJHdr.TOPDays = Convert.ToDecimal(TopDays);
                        }
                        else
                        {
                            trans.Rollback();

                            var msg = "Terdapat kesalahan pada TOP pelanggan, periksa kembali data pelanggan anda !";
                            return Json(new { success = false, message = msg });
                        }

                        oSpTrnSFPJHdr.DueDate = oSpTrnSFPJHdr.FPJDate.AddDays((int)oSpTrnSFPJHdr.TOPDays);
                        //oSpTrnSFPJHdr.TotSalesAmt = model.TotSalesAmt;
                        //oSpTrnSFPJHdr.TotSalesQty = model.TotSalesQty;
                        //oSpTrnSFPJHdr.TotDiscAmt = model.TotDiscAmt;
                        //oSpTrnSFPJHdr.TotDPPAmt = model.TotDPPAmt;
                        //oSpTrnSFPJHdr.TotPPNAmt = model.TotPPNAmt;
                        //oSpTrnSFPJHdr.TotFinalSalesAmt = model.TotFinalSalesAmt;

                        oSpTrnSFPJHdr.TotSalesQty = recordInvHdr.TotSalesQty;
                        oSpTrnSFPJHdr.TotSalesAmt = recordInvHdr.TotSalesAmt;
                        oSpTrnSFPJHdr.TotDiscAmt = recordInvHdr.TotDiscAmt;
                        oSpTrnSFPJHdr.TotDPPAmt = recordInvHdr.TotDPPAmt;
                        oSpTrnSFPJHdr.TotPPNAmt = recordInvHdr.TotPPNAmt;
                        oSpTrnSFPJHdr.TotFinalSalesAmt = recordInvHdr.TotFinalSalesAmt;


                        var CustData = ctx.GnMstCustomers.Find(CompanyCode, model.CustomerCode);
                        if ((CustData.isPKP == null ? false : CustData.isPKP.Value))
                        {
                            //if(string.IsNullOrEmpty(model.Cu
                            if (string.IsNullOrEmpty(model.CustomerNameTagih))
                            {
                                trans.Rollback();

                                var msg = "Harap setting Nama Instansi (Pajak) untuk melanjutkan proses !";
                                return Json(new { success = false, message = msg });
                            }
                        }

                        oSpTrnSFPJHdr.IsPKP = (CustData.isPKP == null ? false : CustData.isPKP.Value);
                        oSpTrnSFPJHdr.TPTrans = (CustData.isPKP == null ? false : CustData.isPKP.Value) ? "P" : "S";
                        oSpTrnSFPJHdr.Status = "0";
                        oSpTrnSFPJHdr.PrintSeq = 0;
                        oSpTrnSFPJHdr.TypeOfGoods = recordPLR.TypeOfGoods;
                        oSpTrnSFPJHdr.CreatedBy = CurrentUser.UserId;
                        oSpTrnSFPJHdr.CreatedDate = DateTime.Now;
                        oSpTrnSFPJHdr.LastUpdateBy = CurrentUser.UserId;
                        oSpTrnSFPJHdr.LastUpdateDate = DateTime.Now;

                        //prepare data FPJInfo
                        spTrnSFPJInfo = new SpTrnSFPJInfo();
                        //ctx.SpTrnSFPJInfos.Add(spTrnSFPJInfo);
                        spTrnSFPJInfo.CompanyCode = CompanyCode;
                        spTrnSFPJInfo.BranchCode = BranchCode;
                        var custTagih = ctx.GnMstCustomers.Find(CompanyCode, oSpTrnSFPJHdr.CustomerCodeBill);
                        if (custTagih != null)
                        {
                            spTrnSFPJInfo.Address1 = custTagih.Address1; //model.Address1Tagih;
                            spTrnSFPJInfo.Address2 = custTagih.Address2; //model.Address2Tagih;
                            spTrnSFPJInfo.Address3 = custTagih.Address3; //model.Address3Tagih;
                            spTrnSFPJInfo.Address4 = custTagih.Address4; //model.Address4Tagih;
                            spTrnSFPJInfo.CustomerName = custTagih.CustomerGovName; //model.CustomerNameTagih;
                        }
                        else
                        {
                            trans.Rollback();

                            var msg = "Terdapat kesalahan pada data pelanggan, periksa kembali data pelanggan anda !";
                            return Json(new { success = false, message = msg });
                        }

                        spTrnSFPJInfo.IsPKP = (CustData.isPKP == null ? false : CustData.isPKP.Value);
                        spTrnSFPJInfo.NPWPNo = CustData.NPWPNo;
                        spTrnSFPJInfo.NPWPDate = CustData.NPWPDate == null ? new DateTime(1900, 1, 1) : CustData.NPWPDate.Value;
                        spTrnSFPJInfo.SKPNo = CustData.SKPNo;
                        spTrnSFPJInfo.SKPDate = CustData.SKPDate == null ? new DateTime(1900, 1, 1) : CustData.SKPDate.Value;
                        spTrnSFPJInfo.CreatedBy = User.Identity.Name;
                        spTrnSFPJInfo.CreatedDate = DateTime.Now;
                        spTrnSFPJInfo.LastUpdateBy = User.Identity.Name;
                        spTrnSFPJInfo.LastUpdateDate = DateTime.Now;

                        oSpTrnSFPJHdr.FPJSignature = GetSignDate(oSpTrnSFPJHdr.FPJDate, oSpTrnSFPJHdr.DueDate);

                        //string errMsg = string.Empty;
                        string errMsg = DateTransValidation(oSpTrnSFPJHdr.FPJDate.Date);
                        if (!string.IsNullOrEmpty(errMsg))
                        {
                            trans.Rollback();

                            return Json(new { success = false, message = errMsg });
                        }
                    }
                    else
                    {
                        //update existing record
                        spTrnSFPJInfo = ctx.SpTrnSFPJInfos.Find(CompanyCode, BranchCode, model.FPJNo);
                        if (spTrnSFPJInfo != null)
                        {
                            if (oSpTrnSFPJHdr.TOPDays == 0)
                            {
                                //set value from Cust Model Tagihan
                                spTrnSFPJInfo.CustomerName = model.CustomerNameTagih;
                                spTrnSFPJInfo.Address1 = model.Address1Tagih;
                                spTrnSFPJInfo.Address2 = model.Address2Tagih;
                                spTrnSFPJInfo.Address3 = model.Address3Tagih;
                                spTrnSFPJInfo.Address4 = model.Address4Tagih;
                            }
                            spTrnSFPJInfo.LastUpdateBy = User.Identity.Name;
                            spTrnSFPJInfo.LastUpdateDate = DateTime.Now;
                            oSpTrnSFPJHdr.FPJSignature = DateTime.Now;
                        }
                    }

                    if (GenFPJ(oSpTrnSFPJHdr, spTrnSFPJInfo, isNew))
                    {
                        ctx.SaveChanges();

                        var invDtlCount = (from a in ctx.SpTrnSInvoiceHdrs 
                                          join b in ctx.SpTrnSInvoiceDtls on new {a.CompanyCode, a.BranchCode, a.InvoiceNo}
                                          equals new {b.CompanyCode, b.BranchCode, b.InvoiceNo}
                                          where a.CompanyCode == CompanyCode 
                                            && a.BranchCode == BranchCode 
                                            && a.InvoiceNo == model.InvoiceNo
                                        select b.InvoiceNo).ToList().Count();

                        var fpjDtlCount = (from a in ctx.SpTrnSFPJHdrs
                                      join b in ctx.SpTrnSFPJDtls on new { a.CompanyCode, a.BranchCode, a.FPJNo }
                                     equals new { b.CompanyCode, b.BranchCode, b.FPJNo }
                                      where a.CompanyCode == CompanyCode
                                        && a.BranchCode == BranchCode
                                        && a.FPJNo == oSpTrnSFPJHdr.FPJNo
                                           select b).ToList().Count();

                        if (invDtlCount == fpjDtlCount)
                        {
                            trans.Commit();
                        }
                        else
                        {
                            trans.Rollback();

                            return Json(new { success = false, message = "Gagal Simpan Faktur Penjualan. Data Detail Invoice tidak tersimpan semua." });
                        }

                        return Json(new { success = true, message = "Faktur Penjualan berhasil disimpan.", FPJNo = oSpTrnSFPJHdr.FPJNo });
                    }
                    else
                    {
                        trans.Rollback();

                        return Json(new { success = false, message = "Gagal simpan Faktur Penjulan" });
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        public JsonResult GeneratePINVS(PembFakturPenjualan model)
        {
            object returnObj = null;
            try
            {
                //StreamWriter sw = new StreamWriter();
                StringBuilder sb = new StringBuilder();
                string PINVS = "";
                sb.Append(getCharacter("H", 1, false));
                // Data ID.
                sb.Append(getCharacter("PINVD", 5, false));
                // Dealer Code
                sb.Append(getCharacter(BranchCode, 10, false));

                //FPJHdr
                SpTrnSFPJHdr recordFPJHdr = ctx.SpTrnSFPJHdrs.Find(CompanyCode, BranchCode, model.FPJNo);

                //FPJDtl
                var listFPJDtl = GetSpTrnsFPJDtlList(recordFPJHdr.FPJNo);

                // Receiving Dealer Code
                GnMstCustomer recCustomer = ctx.GnMstCustomers.Find(CompanyCode, recordFPJHdr.CustomerCode);
                sb.Append(getCharacter(recCustomer.StandardCode.Trim(), 10, false));

                CoProfile recordProfile = ctx.CoProfiles.Find(CompanyCode, BranchCode);
                // Dealer Name
                sb.Append(getCharacter(recordProfile.CompanyName, 50, false));
                // Total Number of Item
                sb.Append(getCharacter(listFPJDtl.Count.ToString(), 6, true));
                // Invoice Number
                sb.Append(getCharacter(recordFPJHdr.InvoiceNo, 15, false));
                // Delivery Number
                sb.Append(getCharacter(recordFPJHdr.InvoiceNo.Substring(7, 6), 6, false));
                // Delivery Date                
                sb.Append(getCharacter(recordFPJHdr.InvoiceDate.ToString("yyyyMMdd"), 8, false));

                foreach (var FPJDtl in listFPJDtl)
                {
                    sb.AppendLine();
                    // Record ID
                    sb.Append(getCharacter("1", 1, false));
                    // Order Number
                    sb.Append(getCharacter(FPJDtl.OrderNo, 15, false));
                    // Sales Number
                    sb.Append(getCharacter(FPJDtl.DocNo.ToString().Substring(7), 6, false));
                    // Currency
                    sb.Append(getCharacter("IDR", 3, false));
                    // Part Number Order
                    sb.Append(getCharacter(FPJDtl.PartNoOriginal.ToString(), 15, false));
                    // Part Number to be shipped
                    sb.Append(getCharacter(FPJDtl.PartNo.ToString(), 15, false));
                    // Shipped Quantity
                    sb.Append(getCharacter(FPJDtl.QtyBill.ToString(), 9, true));
                    // Sales Unit Quantity
                    SpMstItemInfo recordItemInfo = ctx.SpMstItemInfos.Find(CompanyCode, FPJDtl.PartNo);
                    sb.Append(getCharacter(Convert.ToInt32(recordItemInfo.SalesUnit).ToString(), 3, true));
                    // Price
                    SpTrnSFPJDtl recordDtl = ctx.SpTrnSFPJDtls.Find(CompanyCode, BranchCode, recordFPJHdr.FPJNo,
                        "00", FPJDtl.PartNo, FPJDtl.PartNoOriginal, FPJDtl.DocNo);
                    sb.Append(getCharacter(recordDtl.RetailPrice.ToString(), 10, true));
                    // Discount
                    sb.Append(getCharacter(recordDtl.DiscAmt.ToString(), 10, true));
                    // Net Amount
                    sb.Append(getCharacter(recordDtl.NetSalesAmt.ToString(), 15, true));
                    // Process Date
                    sb.Append(getCharacter(recordFPJHdr.InvoiceDate.ToString("yyyyMMdd"), 8, false));
                    // Blank Filler
                    sb.Append(getCharacter(" ", 1, false));
                }
                returnObj = new { success = true, message = "", pinvs = sb.ToString() };
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = "Error pada saat Generate PINVS File, message=" + ex.Message.ToString() };
            }
            return Json(returnObj);
        }

        private string getCharacter(string szCharacter, int iLength, bool bWriteInteger)
        {
            if (szCharacter.Length < iLength)
            {
                if (bWriteInteger)
                    szCharacter = szCharacter.PadLeft(iLength, '0');
                else
                    szCharacter = szCharacter.PadLeft(iLength, ' ');
            }
            else
                szCharacter = szCharacter.Substring(0, iLength);
            return szCharacter;
        }



        private bool GenFPJ(SpTrnSFPJHdr recordHeader, SpTrnSFPJInfo recordFPJInfo, bool isNew)
        {
            bool result = false;
            try
            {
                string errorMsg;
                string msgCode = errorMsg = "";

                if (isNew)
                {
                    recordHeader.FPJNo = recordHeader.IsPKP ? GetNewFPJStd(recordHeader.FPJDate) : GetNewFPJStd(recordHeader.FPJDate);
                    //var myCRC32 = MyLogger.GetCRC32(CompanyCode + BranchCode + recordHeader.FPJNo);
                    //recordHeader.LockingBy = myCRC32;

                    if (recordHeader.FPJNo.EndsWith("X"))
                    {
                        var msg = string.Format(ctx.SysMsgs.Find("5046").MessageCaption, "Faktur Pajak");
                        throw new Exception(msg);
                    }
                    recordHeader.DeliveryNo = recordHeader.FPJNo.Substring(7);

                    #region "Goverment Tax"
                    if ((recordHeader.FPJNo.ToString().Contains("SDH")))
                        recordHeader.FPJGovNo = GetNoSeriFakturPajak(recordHeader.FPJNo);
                    else recordHeader.FPJGovNo = "";
                    #endregion

                    #region "Ambil Inisial No Faktur Penjualan"
                    SpTrnSInvoiceHdr oInvoiceHdr = ctx.SpTrnSInvoiceHdrs.Find(CompanyCode, BranchCode, recordHeader.InvoiceNo);
                    if (oInvoiceHdr.Status == "2")
                    {
                        result = false;
                        errorMsg += "Picking List ini sudah dibuatkan faktur pajak" + "\n";
                        throw new Exception(errorMsg);
                        //return result;
                    }
                    else if (oInvoiceHdr.Status == "0")
                    {
                        result = (UpdateStatus(recordHeader.InvoiceNo, "2", recordHeader.FPJNo, recordHeader.FPJDate) > 0 ? true : false);
                        if (result)
                        {
                            if (!isNew)
                            {
                                if (string.IsNullOrEmpty(recordHeader.TOPCode))
                                {
                                    errorMsg += "Proses Update Faktur Pajak Header Gagal, Term Of Payment belum ter-set !" + "\n";
                                    result = false;
                                    throw new Exception(errorMsg);
                                }

                                int pos = 0;
                                try
                                {
                                    pos = 1;
                                    var oSpTrnSFPJHdr = ctx.SpTrnSFPJHdrs.Find(CompanyCode, BranchCode, recordHeader.FPJNo);
                                    if (oSpTrnSFPJHdr == null)
                                    {
                                        ctx.SpTrnSFPJHdrs.Add(oSpTrnSFPJHdr);
                                    }
                                    else
                                    {
                                        oSpTrnSFPJHdr = recordHeader;
                                    }
                                    ctx.SaveChanges();

                                    pos = 2;
                                    var oSpTrnSFPJInfo = ctx.SpTrnSFPJInfos.Find(CompanyCode, BranchCode, recordHeader.FPJNo);
                                    if (oSpTrnSFPJInfo == null)
                                    {
                                        ctx.SpTrnSFPJInfos.Add(oSpTrnSFPJInfo);
                                    }
                                    else
                                    {
                                        oSpTrnSFPJInfo = recordFPJInfo;
                                    }
                                    ctx.SaveChanges();

                                    result = true;
                                }
                                catch (Exception ex)
                                {
                                    //dbTrans.Dispose();
                                    result = false;
                                    if (pos == 1)
                                        errorMsg += "Penyimpanan Faktur Pajak Header Gagal " + "\n";
                                    if (pos == 2)
                                        errorMsg += "Penyimpanan Faktur Pajak Info Gagal " + "\n";

                                    // Raise Error Exception
                                    throw new Exception(errorMsg + ", Exception Message : " + ex.Message);
                                }

                                /* Insert ke table FPJDTL dan update stock OnHand dan Alokasi di table 
                                * Items & Item Lokasi 
                                */
                            }
                            else
                            {
                                #region ** dipindahkan kedalam transaction scope **

                                //#region Simpan data ke table SpTRNSFPJHdr
                                //if (string.IsNullOrEmpty(recordHeader.TOPCode))
                                //{
                                //    errorMsg += "Proses Insert Faktur Pajak Header Gagal, Term Of Payment belum ter-set !" + "\n";
                                //    result = false;
                                //}

                                //try
                                //{
                                //    ctx.SpTrnSFPJHdrs.Add(recordHeader);
                                //    ctx.SaveChanges();
                                //}
                                //catch (Exception ex)
                                //{
                                //    throw new Exception("Penyimpanan Faktur Pajak Header Gagal" + "\n");
                                //}
                                //#endregion

                                //#region Simpan data ke table spTRNSFPJInfo
                                //recordFPJInfo.FPJNo = recordHeader.FPJNo;
                                //try
                                //{
                                //    ctx.SpTrnSFPJInfos.Add(recordFPJInfo);
                                //}
                                //catch (Exception ex)
                                //{
                                //    errorMsg += "Penyimpanan Faktur Pajak Info Gagal.";
                                //}
                                //#endregion

                                //#region Simpan data ke table SpTrnSFPJDtl
                                //try
                                //{
                                //    SaveFPJDtl(recordHeader.FPJNo, recordHeader.InvoiceNo);
                                //}
                                //catch (Exception)
                                //{
                                //    errorMsg += "Peyimpan Faktur Pajak Detail Gagal.";
                                //}
                                //#endregion

                                //#region Simpan data ke table ArInterface
                                //try
                                //{
                                //    GenerateAR(recordHeader);

                                //}
                                //catch (Exception ex)
                                //{
                                //    throw new Exception("Generate Piutang Pelanggan untuk Penjualan Gagal, Message=" + ex.Message.ToString(), ex.InnerException);
                                //}
                                //#endregion

                                //#region Simpan data ke table GlInterface
                                //try
                                //{
                                //    GenerateGL(recordHeader);

                                //}
                                //catch (Exception ex)
                                //{
                                //    throw new Exception("Generate Journal untuk Penjualan Gagal, Message=" + ex.Message.ToString(), ex.InnerException);
                                //}
                                //#endregion

                                #endregion

                                try
                                {
                                    #region Simpan data ke table SpTRNSFPJHdr

                                    if (string.IsNullOrEmpty(recordHeader.TOPCode))
                                    {
                                        errorMsg += "Proses Insert Faktur Pajak Header Gagal, Term Of Payment belum ter-set !" + "\n";
                                        result = false;

                                        // Raise Error Exception
                                        throw new Exception(errorMsg);
                                    }
                                    ctx.SpTrnSFPJHdrs.Add(recordHeader);
                                    ctx.SaveChanges();

                                    #endregion

                                    #region Simpan data ke table spTRNSFPJInfo

                                    recordFPJInfo.FPJNo = recordHeader.FPJNo;
                                    ctx.SpTrnSFPJInfos.Add(recordFPJInfo);

                                    #endregion

                                    #region Simpan data ke table SpTrnSFPJDtl

                                    result = SaveFPJDtl(recordHeader.FPJNo, recordHeader.InvoiceNo);

                                    #endregion

                                    #region Simpan data ke table ArInterface

                                    //var CustProfitCenter = ctx.MstCustomerProfitCenters.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode, recordHeader.CustomerCode, ProfitCenter);
                                    //var CustClass = ctx.GnMstCustomerClasses.Where(e => e.CompanyCode == "6115204001"
                                    //                && e.BranchCode == "6115204422"
                                    //                && e.CustomerClass == "PART-UMM"
                                    //                && e.ProfitCenterCode == "300"
                                    //                && e.TypeOfGoods == "0").FirstOrDefault();

                                    GenerateAR(recordHeader);

                                    #endregion

                                    #region Simpan data ke table GlInterface

                                    GenerateGL(recordHeader);

                                    #endregion

                                    result = true;
                                }
                                catch (Exception ex)
                                {
                                    result = false;
                                    throw new Exception("Error Pada Function GenFPJ, Exception Message : " + ex.Message.ToString());
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception("Error Pada Function GenFPJ, Exception Message : " + ex.Message.ToString());
            }

            return result;
        }

        public void GenerateAR(SpTrnSFPJHdr spTrnSFPJHdr)
        {
            try
            {
                var oCustProfitCenter = ctx.MstCustomerProfitCenters.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode, spTrnSFPJHdr.CustomerCode, ProfitCenter);
                if (oCustProfitCenter == null){
                    throw new Exception("Customer Profit Center tidak di temukan");
                }

                var oCustomerClass = ctx.GnMstCustomerClasses.Where(e => e.CompanyCode == oCustProfitCenter.CompanyCode
                    && e.BranchCode == oCustProfitCenter.BranchCode
                    && e.CustomerClass == oCustProfitCenter.CustomerClass
                    && e.ProfitCenterCode == oCustProfitCenter.ProfitCenterCode
                    && e.TypeOfGoods == spTrnSFPJHdr.TypeOfGoods).FirstOrDefault();

                //var ReceivableAccNo = ctx.Database.SqlQuery<string>("SELECT ReceivableAccNo FROM gnMstCustomerClass WHERE CompanyCode='" + oCustProfitCenter.CompanyCode + "'"
                //                                                        + "AND BranchCode='" + oCustProfitCenter.BranchCode + "' AND CustomerClass='" + oCustProfitCenter.CustomerClass + "'"
                //                                                        + "AND ProfitCenterCode='" + oCustProfitCenter.ProfitCenterCode + "' AND TypeOfGoods='" + spTrnSFPJHdr.TypeOfGoods + "'");
                
                if (oCustomerClass == null)
                { 
                    throw new Exception("Customer Class tidak di temukan"); 
                }

                var oLookupDtl = ctx.LookUpDtls.Find(CurrentUser.CompanyCode, "TOPC", oCustProfitCenter.TOPCode);
                if (oLookupDtl != null)
                {
                    ArInterface oArInterface = new ArInterface();
                    oArInterface.CompanyCode = CurrentUser.CompanyCode;
                    oArInterface.BranchCode = CurrentUser.BranchCode;
                    oArInterface.DocNo = spTrnSFPJHdr.FPJNo;
                    oArInterface.DocDate = spTrnSFPJHdr.FPJDate;
                    oArInterface.ProfitCenterCode = ProfitCenter;
                    oArInterface.NettAmt = spTrnSFPJHdr.TotFinalSalesAmt;
                    oArInterface.ReceiveAmt = 0;
                    oArInterface.CustomerCode = spTrnSFPJHdr.CustomerCode;
                    oArInterface.TOPCode = oCustProfitCenter.TOPCode;
                    oArInterface.SalesCode = oCustProfitCenter.SalesCode;
                    oArInterface.DueDate = spTrnSFPJHdr.FPJDate.AddDays(Convert.ToDouble(oLookupDtl.ParaValue));
                    oArInterface.StatusFlag = "0";
                    oArInterface.TypeTrans = "INVOICE";
                    oArInterface.CreateBy = CurrentUser.UserId;
                    oArInterface.CreateDate = DateTime.Now;
                    oArInterface.AccountNo = oCustomerClass.ReceivableAccNo;
                    oArInterface.FakturPajakNo = spTrnSFPJHdr.FPJGovNo;
                    oArInterface.FakturPajakDate = spTrnSFPJHdr.FPJSignature;
                    oArInterface.BlockAmt = oArInterface.DebetAmt = oArInterface.CreditAmt = 0;
                    oArInterface.LeasingCode = "";
                    ctx.ArInterfaces.Add(oArInterface);
                    ctx.SaveChanges();
                }
                else
                {
                    throw new Exception(string.Format("Lookup details value untuk TOPC {0} tidak ditemukan", oCustProfitCenter.TOPCode));
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function GenerateAR, Exception Message=" + ex.Message.ToString());
            }
        }

        public void GenerateGL(SpTrnSFPJHdr spTrnSFPJHdr)
        {
            decimal COGS = 0;
            try
            {
                var queryCOGS = string.Format(@"SELECT SUM (QtyBill * CostPrice) 
                            FROM spTrnSFPJDtl with (nolock, nowait) WHERE
                            CompanyCode = '{0}' AND
                            BranchCode = '{1}' AND
                            FPJNo = '{2}'", CompanyCode, BranchCode, spTrnSFPJHdr.FPJNo);
                COGS = ctx.Database.SqlQuery<decimal>(queryCOGS).FirstOrDefault();

                var oCustProfitCenter = ctx.MstCustomerProfitCenters.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode, spTrnSFPJHdr.CustomerCode, ProfitCenter);

                if (oCustProfitCenter == null)
                {
                    throw new Exception("Customer Profit Center tidak di temukan");
                }


                var oCustomerClass = ctx.GnMstCustomerClasses.Where(e => e.CompanyCode == oCustProfitCenter.CompanyCode
                    && e.BranchCode == oCustProfitCenter.BranchCode
                    && e.CustomerClass == oCustProfitCenter.CustomerClass
                    && e.ProfitCenterCode == oCustProfitCenter.ProfitCenterCode
                    && e.TypeOfGoods == spTrnSFPJHdr.TypeOfGoods).FirstOrDefault();

                var oAccount = ctx.spMstAccounts.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode, spTrnSFPJHdr.TypeOfGoods);

                if (oAccount == null)
                {
                    throw new Exception("Master Account tidak di temukan"); 
                }

                if (oCustomerClass == null)
                { 
                    throw new Exception("Customer Class tidak di temukan"); 
                }

                var seq = ctx.GLInterfaces.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == spTrnSFPJHdr.FPJNo);
                decimal seqno = 0;

                // Insert AR Journal
                if ((spTrnSFPJHdr.TotSalesAmt + spTrnSFPJHdr.TotPPNAmt - spTrnSFPJHdr.TotDiscAmt) > 0)
                {
                    if (seq.FirstOrDefault() == null) { seqno = 1; } else { seqno = seq.Select(a => a.SeqNo).Max() + 1; }
                    GLInterface oJurnal = new GLInterface();

                    // Initialize General Data
                    oJurnal.DocNo = spTrnSFPJHdr.FPJNo;
                    oJurnal.DocDate = spTrnSFPJHdr.FPJDate;
                    oJurnal.JournalCode = "SPAREPART";
                    oJurnal.TypeJournal = "INVOICE";
                    oJurnal.ApplyTo = spTrnSFPJHdr.FPJNo;
                    oJurnal.StatusFlag = "0";
                    oJurnal.CreateDate = DateTime.Now;
                    oJurnal.SeqNo = seqno;
                    oJurnal.AccountNo = oCustomerClass.ReceivableAccNo;
                    oJurnal.AmountDb = spTrnSFPJHdr.TotSalesAmt + spTrnSFPJHdr.TotPPNAmt - spTrnSFPJHdr.TotDiscAmt;
                    oJurnal.AmountCr = 0;
                    oJurnal.TypeTrans = "AR";
                    oJurnal.CompanyCode = CompanyCode;
                    oJurnal.BranchCode = BranchCode;
                    oJurnal.ProfitCenterCode = ProfitCenter;
                    oJurnal.AccDate = oJurnal.CreateDate;
                    oJurnal.ApplyTo = oJurnal.DocNo != oJurnal.ApplyTo ? oJurnal.ApplyTo : oJurnal.DocNo;
                    oJurnal.BatchNo = string.Empty;
                    oJurnal.BatchDate = Convert.ToDateTime("1900-01-01");
                    oJurnal.StatusFlag = oJurnal.StatusFlag;
                    oJurnal.CreateBy = CurrentUser.UserId;
                    oJurnal.LastUpdateBy = CurrentUser.UserId;
                    oJurnal.LastUpdateDate = oJurnal.CreateDate;

                    ctx.GLInterfaces.Add(oJurnal);
                    ctx.SaveChanges();
                }

                // Insert Sales Journal
                if (spTrnSFPJHdr.TotSalesAmt > 0)
                {
                    if (seq.FirstOrDefault() == null) { seqno = 1; } else { seqno = seq.Select(a => a.SeqNo).Max() + 1; }
                    GLInterface oJurnal = new GLInterface();

                    // Initialize General Data
                    oJurnal.DocNo = spTrnSFPJHdr.FPJNo;
                    oJurnal.DocDate = spTrnSFPJHdr.FPJDate;
                    oJurnal.JournalCode = "SPAREPART";
                    oJurnal.TypeJournal = "INVOICE";
                    oJurnal.ApplyTo = spTrnSFPJHdr.FPJNo;
                    oJurnal.StatusFlag = "0";
                    oJurnal.CreateDate = DateTime.Now;
                    oJurnal.SeqNo = seqno;
                    oJurnal.AccountNo = oAccount.SalesAccNo;
                    oJurnal.AmountDb = decimal.Zero;
                    oJurnal.AmountCr = spTrnSFPJHdr.TotSalesAmt;
                    oJurnal.TypeTrans = "SALES";
                    oJurnal.CompanyCode = CompanyCode;
                    oJurnal.BranchCode = BranchCode;
                    oJurnal.ProfitCenterCode = ProfitCenter;
                    oJurnal.AccDate = oJurnal.CreateDate;
                    oJurnal.ApplyTo = oJurnal.DocNo != oJurnal.ApplyTo ? oJurnal.ApplyTo : oJurnal.DocNo;
                    oJurnal.BatchNo = string.Empty;
                    oJurnal.BatchDate = Convert.ToDateTime("1900-01-01");
                    oJurnal.StatusFlag = oJurnal.StatusFlag;
                    oJurnal.CreateBy = CurrentUser.UserId;
                    oJurnal.LastUpdateBy = CurrentUser.UserId;
                    oJurnal.LastUpdateDate = oJurnal.CreateDate;

                    ctx.GLInterfaces.Add(oJurnal);
                    ctx.SaveChanges();
                }

                // Insert Sales Journal
                if (spTrnSFPJHdr.TotPPNAmt > 0)
                {
                    if (seq.FirstOrDefault() == null) { seqno = 1; } else { seqno = seq.Select(a => a.SeqNo).Max() + 1; }
                    GLInterface oJurnal = new GLInterface();

                    // Initialize General Data
                    oJurnal.DocNo = spTrnSFPJHdr.FPJNo;
                    oJurnal.DocDate = spTrnSFPJHdr.FPJDate;
                    oJurnal.JournalCode = "SPAREPART";
                    oJurnal.TypeJournal = "INVOICE";
                    oJurnal.ApplyTo = spTrnSFPJHdr.FPJNo;
                    oJurnal.StatusFlag = "0";
                    oJurnal.CreateDate = DateTime.Now;
                    oJurnal.SeqNo = seqno;
                    oJurnal.AccountNo = oCustomerClass.TaxOutAccNo;
                    oJurnal.AmountDb = decimal.Zero;
                    oJurnal.AmountCr = spTrnSFPJHdr.TotPPNAmt;
                    oJurnal.TypeTrans = "TAXOUT";
                    oJurnal.CompanyCode = CompanyCode;
                    oJurnal.BranchCode = BranchCode;
                    oJurnal.ProfitCenterCode = ProfitCenter;
                    oJurnal.AccDate = oJurnal.CreateDate;
                    oJurnal.ApplyTo = oJurnal.DocNo != oJurnal.ApplyTo ? oJurnal.ApplyTo : oJurnal.DocNo;
                    oJurnal.BatchNo = string.Empty;
                    oJurnal.BatchDate = Convert.ToDateTime("1900-01-01");
                    oJurnal.StatusFlag = oJurnal.StatusFlag;
                    oJurnal.CreateBy = CurrentUser.UserId;
                    oJurnal.LastUpdateBy = CurrentUser.UserId;
                    oJurnal.LastUpdateDate = oJurnal.CreateDate;

                    ctx.GLInterfaces.Add(oJurnal);
                    ctx.SaveChanges();
                }

                // Insert Discount Journal
                if (spTrnSFPJHdr.TotDiscAmt > 0)
                {
                    if (seq.FirstOrDefault() == null) { seqno = 1; } else { seqno = seq.Select(a => a.SeqNo).Max() + 1; }
                    GLInterface oJurnal = new GLInterface();

                    // Initialize General Data
                    oJurnal.DocNo = spTrnSFPJHdr.FPJNo;
                    oJurnal.DocDate = spTrnSFPJHdr.FPJDate;
                    oJurnal.JournalCode = "SPAREPART";
                    oJurnal.TypeJournal = "INVOICE";
                    oJurnal.ApplyTo = spTrnSFPJHdr.FPJNo;
                    oJurnal.StatusFlag = "0";
                    oJurnal.CreateDate = DateTime.Now;
                    oJurnal.SeqNo = seqno;
                    oJurnal.AccountNo = oAccount.DiscAccNo;
                    oJurnal.AmountDb = spTrnSFPJHdr.TotDiscAmt;
                    oJurnal.AmountCr = decimal.Zero;
                    oJurnal.TypeTrans = "DISC1";
                    oJurnal.CompanyCode = CompanyCode;
                    oJurnal.BranchCode = BranchCode;
                    oJurnal.ProfitCenterCode = ProfitCenter;
                    oJurnal.AccDate = oJurnal.CreateDate;
                    oJurnal.ApplyTo = oJurnal.DocNo != oJurnal.ApplyTo ? oJurnal.ApplyTo : oJurnal.DocNo;
                    oJurnal.BatchNo = string.Empty;
                    oJurnal.BatchDate = Convert.ToDateTime("1900-01-01");
                    oJurnal.StatusFlag = oJurnal.StatusFlag;
                    oJurnal.CreateBy = CurrentUser.UserId;
                    oJurnal.LastUpdateBy = CurrentUser.UserId;
                    oJurnal.LastUpdateDate = oJurnal.CreateDate;

                    ctx.GLInterfaces.Add(oJurnal);
                    ctx.SaveChanges();
                }

                if (COGS > 0)
                {
                    // Insert COGS Journal
                    if (seq.FirstOrDefault() == null) { seqno = 1; } else { seqno = seq.Select(a => a.SeqNo).Max() + 1; }
                    GLInterface oJurnal = new GLInterface();

                    // Initialize General Data
                    oJurnal.DocNo = spTrnSFPJHdr.FPJNo;
                    oJurnal.DocDate = spTrnSFPJHdr.FPJDate;
                    oJurnal.JournalCode = "SPAREPART";
                    oJurnal.TypeJournal = "INVOICE";
                    oJurnal.ApplyTo = spTrnSFPJHdr.FPJNo;
                    oJurnal.StatusFlag = "0";
                    oJurnal.CreateDate = DateTime.Now;
                    oJurnal.SeqNo = seqno;
                    oJurnal.AccountNo = oAccount.COGSAccNo;
                    oJurnal.AmountDb = COGS;
                    oJurnal.AmountCr = decimal.Zero;
                    oJurnal.TypeTrans = "COGS";
                    oJurnal.CompanyCode = CompanyCode;
                    oJurnal.BranchCode = BranchCode;
                    oJurnal.ProfitCenterCode = ProfitCenter;
                    oJurnal.AccDate = oJurnal.CreateDate;
                    oJurnal.ApplyTo = oJurnal.DocNo != oJurnal.ApplyTo ? oJurnal.ApplyTo : oJurnal.DocNo;
                    oJurnal.BatchNo = string.Empty;
                    oJurnal.BatchDate = Convert.ToDateTime("1900-01-01");
                    oJurnal.StatusFlag = oJurnal.StatusFlag;
                    oJurnal.CreateBy = CurrentUser.UserId;
                    oJurnal.LastUpdateBy = CurrentUser.UserId;
                    oJurnal.LastUpdateDate = oJurnal.CreateDate;

                    ctx.GLInterfaces.Add(oJurnal);
                    ctx.SaveChanges();

                    // Insert Inventory Journal

                    if (seq.FirstOrDefault() == null) { seqno = 1; } else { seqno = seq.Select(a => a.SeqNo).Max() + 1; }
                    GLInterface oInvJurnal = new GLInterface();

                    // Initialize General Data
                    oInvJurnal.DocNo = spTrnSFPJHdr.FPJNo;
                    oInvJurnal.DocDate = spTrnSFPJHdr.FPJDate;
                    oInvJurnal.JournalCode = "SPAREPART";
                    oInvJurnal.TypeJournal = "INVOICE";
                    oInvJurnal.ApplyTo = spTrnSFPJHdr.FPJNo;
                    oInvJurnal.StatusFlag = "0";
                    oInvJurnal.CreateDate = DateTime.Now;
                    oInvJurnal.SeqNo = seqno;
                    oInvJurnal.AccountNo = oAccount.InventoryAccNo;
                    oInvJurnal.AmountDb = decimal.Zero;
                    oInvJurnal.AmountCr = COGS;
                    oInvJurnal.TypeTrans = "INVENTORY";
                    oInvJurnal.CompanyCode = CompanyCode;
                    oInvJurnal.BranchCode = BranchCode;
                    oInvJurnal.ProfitCenterCode = ProfitCenter;
                    oInvJurnal.AccDate = oInvJurnal.CreateDate;
                    oInvJurnal.ApplyTo = oInvJurnal.DocNo != oInvJurnal.ApplyTo ? oInvJurnal.ApplyTo : oInvJurnal.DocNo;
                    oInvJurnal.BatchNo = string.Empty;
                    oInvJurnal.BatchDate = Convert.ToDateTime("1900-01-01");
                    oInvJurnal.StatusFlag = oInvJurnal.StatusFlag;
                    oInvJurnal.CreateBy = CurrentUser.UserId;
                    oInvJurnal.LastUpdateBy = CurrentUser.UserId;
                    oInvJurnal.LastUpdateDate = oInvJurnal.CreateDate;

                    ctx.GLInterfaces.Add(oInvJurnal);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function GenerateAR, Message=" + ex.Message.ToString());
            }
        }

        private bool SaveFPJDtl(string FPJNo, string InvoiceNo)
        {
            bool returnVal = false;
            string errorMsg = "";
            try
            {
                SpTrnSFPJDtl spTrnSFPJDtl = null;
                SpTrnSFPJHdr recordHdr = ctx.SpTrnSFPJHdrs.Find(CompanyCode, BranchCode, FPJNo);
                SpTrnSInvoiceHdr recordInvHdr = ctx.SpTrnSInvoiceHdrs.Find(CompanyCode, BranchCode, InvoiceNo);
                var listSpTrnSInvoiceDtl = ctx.SpTrnSInvoiceDtls.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.InvoiceNo == InvoiceNo).ToList();
                var oGnmStcoProfileSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                var fiscalMonth = oGnmStcoProfileSpare.FiscalMonth;
                var fiscalYear = oGnmStcoProfileSpare.FiscalYear;
                int iSeq = 1;
                foreach (var SpTrnSInvoiceDtl in listSpTrnSInvoiceDtl)
                {
                    // remark, no need to check, just insert new.
                    //spTrnSFPJDtl = ctx.SpTrnSFPJDtls.Find(CompanyCode, BranchCode, FPJNo, SpTrnSInvoiceDtl.WarehouseCode, SpTrnSInvoiceDtl.PartNo, SpTrnSInvoiceDtl.PartNoOriginal, SpTrnSInvoiceDtl.DocNo);
                    //if (spTrnSFPJDtl == null)
                    //{
                        spTrnSFPJDtl = new SpTrnSFPJDtl();
                        spTrnSFPJDtl.CompanyCode = CompanyCode;
                        spTrnSFPJDtl.BranchCode = BranchCode;
                        spTrnSFPJDtl.FPJNo = FPJNo;
                        spTrnSFPJDtl.Warehousecode = SpTrnSInvoiceDtl.WarehouseCode;
                        spTrnSFPJDtl.PartNo = SpTrnSInvoiceDtl.PartNo;
                        spTrnSFPJDtl.PartNoOriginal = SpTrnSInvoiceDtl.PartNoOriginal;
                        spTrnSFPJDtl.DocNo = SpTrnSInvoiceDtl.DocNo;
                        spTrnSFPJDtl.DocDate = SpTrnSInvoiceDtl.DocDate;
                        spTrnSFPJDtl.ReferenceNo = SpTrnSInvoiceDtl.ReferenceNo;
                        spTrnSFPJDtl.ProductType = SpTrnSInvoiceDtl.ProductType;
                        spTrnSFPJDtl.CreatedBy = User.Identity.Name;
                        spTrnSFPJDtl.CreatedDate = DateTime.Now;
                        ctx.SpTrnSFPJDtls.Add(spTrnSFPJDtl);
                    //}

                    spTrnSFPJDtl.LastUpdateBy = User.Identity.Name;
                    spTrnSFPJDtl.LastUpdateDate = DateTime.Now;

                    spTrnSFPJDtl.ABCCLass = SpTrnSInvoiceDtl.ABCClass;
                    spTrnSFPJDtl.LocationCode = SpTrnSInvoiceDtl.LocationCode;
                    spTrnSFPJDtl.MovingCode = SpTrnSInvoiceDtl.MovingCode;
                    spTrnSFPJDtl.PartCategory = SpTrnSInvoiceDtl.PartCategory;
                   
                    //#region --> Get new COGS price from Item Price
                    //var oSpMstItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, SpTrnSInvoiceDtl.PartNo);
                    spTrnSFPJDtl.CostPrice = SpTrnSInvoiceDtl.CostPrice; //(oSpMstItemPrice == null ? 0 : oSpMstItemPrice.CostPrice);
                    //#endregion

                    spTrnSFPJDtl.DiscPct = SpTrnSInvoiceDtl.DiscPct;
                    spTrnSFPJDtl.RetailPrice = SpTrnSInvoiceDtl.RetailPrice;
                    spTrnSFPJDtl.RetailPriceInclTax = SpTrnSInvoiceDtl.RetailPriceInclTax;
                    spTrnSFPJDtl.DocDate = SpTrnSInvoiceDtl.DocDate;
                    spTrnSFPJDtl.ReferenceDate = SpTrnSInvoiceDtl.ReferenceDate;
                    spTrnSFPJDtl.QtyBill = SpTrnSInvoiceDtl.QtyBill;
                    spTrnSFPJDtl.NetSalesAmt = SpTrnSInvoiceDtl.NetSalesAmt;
                    spTrnSFPJDtl.PPNAmt = SpTrnSInvoiceDtl.PPNAmt;
                    spTrnSFPJDtl.SalesAmt = SpTrnSInvoiceDtl.SalesAmt;
                    spTrnSFPJDtl.TotSalesAmt = SpTrnSInvoiceDtl.TotSalesAmt;
                    spTrnSFPJDtl.DiscAmt = SpTrnSInvoiceDtl.DiscAmt;

                    try
                    {
                        //ctx.SpTrnSFPJDtls.Add(spTrnSFPJDtl);
                        ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        returnVal = false;
                        errorMsg += "Proses Insert data Faktur Pajak Detail Gagal";
                        throw new Exception("Error occured in function SaveFPJDtl, Message=" + errorMsg + ", Exception Message : " + ex.ToString(), ex.InnerException);
                    }

                    UpdateStock(spTrnSFPJDtl.PartNo,
                        spTrnSFPJDtl.Warehousecode,
                        Convert.ToDecimal(spTrnSFPJDtl.QtyBill) * -1,
                        Convert.ToDecimal(spTrnSFPJDtl.QtyBill) * -1, 0, 0, recordInvHdr.SalesType
                    );

                    UpdateLastItemDate(spTrnSFPJDtl.PartNo, "LSD");

                   returnVal = MovementLog(recordHdr.FPJNo, recordHdr.FPJDate, SpTrnSInvoiceDtl.PartNo, SpTrnSInvoiceDtl.WarehouseCode,
                        "OUT", "FAKTUR", SpTrnSInvoiceDtl.QtyBill);


                   //Insert svSDMovement
                   if (IsMD == false)
                   {
                    var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                                GetDbMD(), CompanyMD, BranchMD, SpTrnSInvoiceDtl.PartNo);
                    spMstItemPrice oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

                        var qrDb = "SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'";
                        string DBMD = ctx.Database.SqlQuery<string>(qrDb).FirstOrDefault();

                        var iQry = @"insert into " + DBMD + @"..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice,   
	                        TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, 
	                        Status, ProcessStatus, ProcessDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate) 
                            VALUES('" + CompanyCode + "','" + BranchCode + "','" + recordHdr.FPJNo + "','" + ctx.CurrentTime + "','" + SpTrnSInvoiceDtl.PartNo +
                        "','" + (iSeq) + "','" + SpTrnSInvoiceDtl.WarehouseCode + "','" + SpTrnSInvoiceDtl.QtyBill + "','" + SpTrnSInvoiceDtl.QtyBill + "','" + SpTrnSInvoiceDtl.DiscPct + "','" + SpTrnSInvoiceDtl.CostPrice +
                        "','" + SpTrnSInvoiceDtl.RetailPrice + "','" + CurrentUser.TypeOfGoods + "','" + CompanyMD + "','" + BranchMD + "','" + SpTrnSInvoiceDtl.WarehouseCode + "','" + SpTrnSInvoiceDtl.RetailPriceInclTax + "','" + SpTrnSInvoiceDtl.RetailPrice +
                        "','" + oItemPrice.CostPrice + "','x','" + SpTrnSInvoiceDtl.ProductType + "','300','0','0','" + ctx.CurrentTime + "','" + CurrentUser.UserId + "','" + ctx.CurrentTime + "','" + CurrentUser.UserId + "','" + ctx.CurrentTime + "')";

                        var sqlCount = string.Format("select count(*) as count from " + DBMD + @"..svHstSDMovement
                                    where CompanyCode = '{0}' 
                                    and BranchCode = '{1}'
                                    and DocNo = '{2}'
                                    and PartNo = '{3}'", CompanyCode, BranchCode, recordHdr.FPJNo, SpTrnSInvoiceDtl.PartNo);
                        var count = (int)ctx.Database.SqlQuery<int>(sqlCount).FirstOrDefault();

                        if (count > 0)
                        {
                            var qty = SpTrnSInvoiceDtl.QtyBill;
                            iQry = string.Format("update " + DBMD + @"..svSDMovement 
                                    set QtyOrder += {0}, Qty += {1}
                                    where CompanyCode = '{2}' 
                                    and BranchCode = '{3}'
                                    and DocNo = '{4}'
                                    and PartNo = '{5}'",
                                qty, qty, CompanyCode, BranchCode, recordHdr.FPJNo, SpTrnSInvoiceDtl.PartNo);
                        }
                        else
                        {
                            iSeq = iSeq + 1;
                        }

                        ctx.Database.ExecuteSqlCommand(iQry);
                    }

                    #region Update SPHstDemandCust
                    //SpMstDe
                    var recordDmsCust = ctx.SpHstDemandCusts.Find(CompanyCode, BranchCode, fiscalYear, fiscalMonth, recordInvHdr.CustomerCode, SpTrnSInvoiceDtl.PartNo);
                    if (recordDmsCust == null)
                    {
                        recordDmsCust = new SpHstDemandCust();
                        recordDmsCust.CompanyCode = CompanyCode;
                        recordDmsCust.BranchCode = BranchCode;
                        recordDmsCust.Year = Convert.ToInt32(fiscalYear);
                        recordDmsCust.Month = Convert.ToInt32(fiscalMonth);
                        recordDmsCust.CustomerCode = recordInvHdr.CustomerCode;
                        recordDmsCust.PartNo = SpTrnSInvoiceDtl.PartNo;
                        recordDmsCust.SalesFreq = 1;
                        recordDmsCust.SalesQty = SpTrnSInvoiceDtl.QtyBill;

                        var recordItems = ctx.spMstItems.Find(CompanyCode, BranchCode, SpTrnSInvoiceDtl.PartNo);
                        recordDmsCust.MovingCode = recordItems.MovingCode;
                        recordDmsCust.ProductType = recordItems.ProductType;
                        recordDmsCust.partCategory = recordItems.PartCategory;
                        recordDmsCust.ABCClass = recordItems.ABCClass;

                        ctx.SpHstDemandCusts.Add(recordDmsCust);
                    }
                    recordDmsCust.SalesQty += SpTrnSInvoiceDtl.QtyBill;
                    recordDmsCust.SalesFreq += 1;
                    recordDmsCust.LastUpdateBy = CurrentUser.UserId;
                    recordDmsCust.LastUpdateDate = DateTime.Now;

                    #endregion
                }
                ctx.SaveChanges();

                if (IsMD == false)
                {
                    var sql = string.Format("exec uspfn_UpdateSvSDMovement '{0}','{1}','{2}'", CompanyCode, BranchCode, FPJNo);
                    ctx.Database.ExecuteSqlCommand(sql);
                }

                returnVal = true;
            }
            catch (Exception ex)
            {
                returnVal = false;

                throw new Exception(ex.Message);
            }

            return returnVal;
        }

        private bool MovementLog(string docno, DateTime docdate, string partno, string whcode, string signcode, string subsigncode, decimal qty)
        {
            var md = DealerCode() == "MD";
            //SpTrnMovement
            //var oItem = ctxMD.spMstItems.Find(CompanyMD, BranchMD, partno);
            var Item = @"select * from " + GetDbMD() + @"..spMstItems where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + partno + "'";
            spMstItem oItem = ctx.Database.SqlQuery<spMstItem>(Item).FirstOrDefault();
            
            //var oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, partno);
            var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                        GetDbMD(), CompanyMD, BranchMD, partno);
            var oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();
            
            
            //var oItemLoc = ctxMD.SpMstItemLocs.Find(CompanyMD, BranchMD, partno, whcode);
            var ItemLoc = @"select * from " + GetDbMD() + @"..SpMstItemLoc where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + partno + "' and WarehouseCode ='" + whcode + "'";
            SpMstItemLoc oItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(ItemLoc).FirstOrDefault();

            var oIMovement = new SpTrnIMovement();
            if (oItemLoc != null && oItemPrice != null && oItem != null)
            {
                oIMovement.CompanyCode = CompanyCode;
                oIMovement.BranchCode = BranchCode;
                oIMovement.DocNo = docno;
                oIMovement.DocDate = docdate;
                oIMovement.CreatedDate = DateTime.Now;
                oIMovement.WarehouseCode = oItemLoc.WarehouseCode;
                oIMovement.LocationCode = oItemLoc.LocationCode;
                oIMovement.PartNo = oItemLoc.PartNo;
                oIMovement.SignCode = signcode;
                oIMovement.SubSignCode = subsigncode;
                oIMovement.Qty = qty;
                oIMovement.Price = GetRetailPrice(partno, oItemPrice.RetailPrice.Value); //oItemPrice.RetailPrice; 
                oIMovement.CostPrice = GetCostPrice(partno); //oItemPrice.CostPrice;
                oIMovement.ABCClass = oItem.ABCClass;
                oIMovement.MovingCode = oItem.MovingCode;
                oIMovement.ProductType = oItem.ProductType;
                oIMovement.PartCategory = oItem.PartCategory;
                oIMovement.CreatedBy = CurrentUser.UserId;

                ctx.SpTrnIMovements.Add(oIMovement);
            }
            var result = ctx.SaveChanges() > 0 ;

            return result;
        }

        private void UpdateLastItemDate(string partNo, string isUpdateWhat)
        {
            spMstItem mstItem = ctx.spMstItems.Find(CompanyCode, BranchCode, partNo);

            if (mstItem != null)
            {
                if (isUpdateWhat.ToUpper() == "LLD")
                {
                    mstItem.LastDemandDate = DateTime.Now;
                }
                else if (isUpdateWhat.ToUpper() == "LPD")
                {
                    mstItem.LastPurchaseDate = DateTime.Now;
                }
                else if (isUpdateWhat.ToUpper() == "LSD")
                {
                    mstItem.LastSalesDate = DateTime.Now;
                }

                mstItem.LastUpdateDate = DateTime.Now;
                mstItem.LastUpdateBy = CurrentUser.UserId;
                ctx.SaveChanges();
            }
        }

        #region "Move to BaseController"
        //public void UpdateStock(string partno, string whcode, decimal onhand, decimal alloaction, decimal backorder, decimal reserved, string salesType)
        //{
        //    try
        //    {
        //        var oItem = ctx.spMstItems.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode, partno);
        //        if (oItem != null)
        //        {
        //            var oItemLoc = ctx.SpMstItemLocs.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode, partno, whcode);

        //            if (oItemLoc != null)
        //            {
        //                if (Math.Abs(onhand) > 0)
        //                {
        //                    oItemLoc.OnHand += onhand;
        //                    oItem.OnHand += onhand;

        //                    if (oItemLoc.OnHand < 0)
        //                    {
        //                        throw new Exception(string.Format("OnHand untuk Part = {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.OnHand));
        //                    }

        //                    if (oItem.OnHand < 0)
        //                    {
        //                        throw new Exception(string.Format("OnHand untuk Part = {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.OnHand));
        //                    }
        //                }

        //                if (Math.Abs(alloaction) > 0)
        //                {
        //                    if (!string.IsNullOrEmpty(salesType) && (salesType == "0" || salesType == "1"))
        //                    {
        //                        oItemLoc.AllocationSP += alloaction;
        //                        oItem.AllocationSP += alloaction;

        //                        if (oItemLoc.AllocationSP < 0)
        //                        {
        //                            throw new Exception(string.Format("AllocationSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSP));
        //                        }
        //                        if (oItem.AllocationSP < 0)
        //                        {
        //                            throw new Exception(string.Format("AllocationSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSP));
        //                        }
        //                    }

        //                    if (!string.IsNullOrEmpty(salesType) && salesType == "2")
        //                    {
        //                        oItemLoc.AllocationSR += alloaction;
        //                        oItem.AllocationSR += alloaction;

        //                        if (oItemLoc.AllocationSR < 0)
        //                        {
        //                            throw new Exception(string.Format("AllocationSR untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSR));
        //                        }

        //                        if (oItem.AllocationSR < 0)
        //                        {
        //                            throw new Exception(string.Format("AllocationSR untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSR));
        //                        }
        //                    }

        //                    if (!string.IsNullOrEmpty(salesType) && salesType == "3")
        //                    {
        //                        oItemLoc.AllocationSL += alloaction;
        //                        oItem.AllocationSL += alloaction;

        //                        if (oItemLoc.AllocationSL < 0)
        //                        {
        //                            throw new Exception(string.Format("AllocationSL untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSR));
        //                        }
        //                        if (oItem.AllocationSL < 0)
        //                        {
        //                            throw new Exception(string.Format("AllocationSL untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.AllocationSR));
        //                        }
        //                    }
        //                }

        //                if (Math.Abs(backorder) > 0)
        //                {
        //                    if (!string.IsNullOrEmpty(salesType) && (salesType == "0" || salesType == "1"))
        //                    {
        //                        oItemLoc.BackOrderSP += backorder;
        //                        oItem.BackOrderSP += backorder;

        //                        if (oItemLoc.BackOrderSP < 0)
        //                        {
        //                            throw new Exception(string.Format("BackOrderSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSP));
        //                        }
        //                        if (oItem.BackOrderSP < 0)
        //                        {
        //                            throw new Exception(string.Format("BackOrderSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSP));
        //                        }
        //                    }

        //                    if (!string.IsNullOrEmpty(salesType) && (salesType == "2"))
        //                    {
        //                        oItemLoc.BackOrderSR += backorder;
        //                        oItem.BackOrderSR += backorder;

        //                        if (oItemLoc.BackOrderSR < 0)
        //                        {
        //                            throw new Exception(string.Format("BackOrderSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSP));
        //                        }

        //                        if (oItem.BackOrderSP < 0)
        //                        {
        //                            throw new Exception(string.Format("BackOrderSR untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSR));
        //                        }
        //                    }

        //                    if (!string.IsNullOrEmpty(salesType) && (salesType == "3"))
        //                    {
        //                        oItemLoc.BackOrderSL += backorder;
        //                        oItem.BackOrderSL += backorder;

        //                        if (oItemLoc.BackOrderSL < 0)
        //                        {
        //                            throw new Exception(string.Format("BackOrderSL untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSL));
        //                        }

        //                        if (oItem.BackOrderSL < 0)
        //                        {
        //                            throw new Exception(string.Format("BackOrderSL untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSL));
        //                        }
        //                    }

        //                    if (Math.Abs(reserved) > 0)
        //                    {
        //                        oItemLoc.ReservedSP += reserved;
        //                        oItem.ReservedSP += reserved;
        //                        if (oItemLoc.ReservedSP < 0)
        //                        {
        //                            throw new Exception(string.Format("ReservedSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.ReservedSP));
        //                        }
        //                        if (oItem.ReservedSP < 0)
        //                        {
        //                            throw new Exception(string.Format("ReservedSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.ReservedSP));
        //                        }
        //                    }

        //                    oItemLoc.LastUpdateDate = DateTime.Now;
        //                    oItemLoc.LastUpdateBy = CurrentUser.UserId;
        //                    oItem.LastUpdateDate = DateTime.Now;
        //                    oItem.LastUpdateBy = CurrentUser.UserId;
        //                    ctx.SaveChanges();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error pada function UpdateStock, Message=" + ex.Message.ToString());
        //    }
        //}
        #endregion

        public DateTime GetSignDate(DateTime transDate, DateTime DueDate)
        {
            DateTime signDate = transDate.Date;
            DateTime returnDate = new DateTime();
            var FPJSignDate = ctx.GnMstFPJSignDates.Find(CompanyCode, BranchCode, ProfitCenter);
            if (FPJSignDate == null)
            {
                throw new Exception("Message=Master Sign Date Faktur Pajak belum ada.");
            }
            else
            {
                try
                {
                    if (FPJSignDate.FPJOption == "1")
                    {
                        if (DueDate.Month == transDate.Month)
                        {
                            signDate = transDate.Date;
                        }
                        else
                        {
                            signDate = new DateTime(transDate.Date.Year, transDate.Date.Month + 1, 1, 0, 0, 0);
                        }
                    }
                    else
                    {
                        signDate = transDate.Date;
                    }
                }
                catch (Exception ex)
                {
                    signDate = DateTime.Parse("1900/01/01");
                }
            }

            return signDate;
        }

        private string GetNoSeriFakturPajak(string FPJNo)
        {
            string NoSeriFPJ = "";
            try
            {
                var coProfile = ctx.CoProfiles.Find(CompanyCode, BranchCode);
                if (coProfile != null && FPJNo.Contains("FPJ") && FPJNo.Trim().Length == 13)
                {
                    string codetrans = coProfile.TaxTransCode.Substring(0, 2).PadRight(3, '0');
                    string codecabang = coProfile.TaxCabCode.Substring(0, 3);
                    string tahun = FPJNo.Substring(7).PadLeft(8, '0');
                    var seq = FPJNo.Substring(7).PadLeft(8, '0');
                    NoSeriFPJ = string.Format("{0}.{1}-{2}.{3}", codetrans, codecabang, tahun, seq);
                }
                else if (coProfile != null && FPJNo.Contains("SDH") && FPJNo.Trim().Length == 13)
                {
                    NoSeriFPJ = FPJNo;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function GetNoSeriFakturPajak, Message=" + ex.Message.ToString());
            }
            return NoSeriFPJ;
        }

        private string GetNewFPJStd(DateTime TglFpjStd)
        {
            return GetNewDocumentNo("FPJ", TglFpjStd);
        }

        private string GetNewFPJSdh(DateTime TglFPJSdh)
        {
            return GetNewDocumentNo("SDH", TglFPJSdh);
        }

        private int UpdateStatus(string docNo, string status, string FPJNo, DateTime FPJDate)
        {
            int result = -1;
            try
            {
                string query = string.Format("UPDATE spTrnSInvoiceHdr With(ReadPast)" +
            " SET Status = '{6}', " +
            " FPJNo = '{2}', " +
            " FPJDate = '{3}', " +
            " LastUpdateBy = '{4}', " +
            " LastUpdateDate = '{5}' " +
            " WHERE CompanyCode = '{0}' AND " +
            " BranchCode = '{1}' AND InvoiceNo = '{7}'", CompanyCode, BranchCode, FPJNo, FPJDate, User.Identity.Name, DateTime.Now, status, docNo);
                ctx.Database.ExecuteSqlCommand(query);
                result = 1;
            }
            catch
            {
                result = -1;
            }

            return result;
        }

        public JsonResult GetDataOutstandingBO(string CompanyCode, string BranchCode, string CustomerCode, string TrsType, string TypeOfGoods, string SlsType, string ProductType)
        {
            var msg = "";
            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "EXEC uspfn_spGetDataOutstandingBO '" + CompanyCode + "','" + BranchCode + "','" + CustomerCode + "','" + TrsType + "','" + TypeOfGoods + "','" + SlsType + "','" + ProductType + "'";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count == 0)
                {
                    msg = "Tidak ada data yang ditampilkan";
                    return Json(new { success = false, message = msg });
                }
                else
                {
                    return Json(new { success = true, data = dt.Tables[0] }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult GenTextPINVS(string fpj, string flName)
        {
            StringBuilder sb = new StringBuilder();
            sb = GenPINVS(fpj);
            string fileName = flName;

            string text = Convert.ToString(sb);
            var buffer = Encoding.UTF8.GetBytes(text);
            var stream = new System.IO.MemoryStream(buffer, 0, buffer.Length);
            stream.Position = 0;
            return File(stream, "text/plain", flName + ".txt");

            //MemoryStream 

        }

        private string formatDateTime(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }

        private StringBuilder GenPINVS(string fpj)
        {
            var user = ctx.SysUsers.Find(CurrentUser.UserId);

            try
            {
                StringBuilder sb = new StringBuilder();

                // Add some text to the file.
                // Record ID.
                sb.Append(getCharacter("H", 1, false));
                // Data ID.
                sb.Append(getCharacter("PINVS", 5, false));
                // Dealer Code
                sb.Append(getCharacter(user.BranchCode, 10, false));
                //FPJHdr
                SpTrnSFPJHdr recordFPJHdr = ctx.SpTrnSFPJHdrs.Find(user.CompanyCode, user.BranchCode, fpj);
                //FPJDtl
                var listFPJDtl = GetSpTrnsFPJDtlList(recordFPJHdr.FPJNo);
                // Receiving Dealer Code
                GnMstCustomer recCustomer = ctx.GnMstCustomers.Find(user.CompanyCode, recordFPJHdr.CustomerCode);
                sb.Append(getCharacter(recCustomer.StandardCode.Trim(), 10, false));

                CoProfile recordProfile = ctx.CoProfiles.Find(user.CompanyCode, user.BranchCode);
                // Ship To Dealer Code
                sb.Append(getCharacter(recCustomer.StandardCode.Trim(), 10, false));
                // Total Number of Item
                sb.Append(getCharacter(listFPJDtl.Count.ToString(), 6, true));
                // Invoice Number
                sb.Append(getCharacter(recordFPJHdr.InvoiceNo, 15, false));
                // Invoice Date
                sb.Append(getCharacter(formatDateTime(recordFPJHdr.InvoiceDate), 8, false));
                // Delivery Number
                sb.Append(getCharacter(recordFPJHdr.InvoiceNo.Substring(7, 6), 6, false));
                // Delivery Date                
                sb.Append(getCharacter(formatDateTime(recordFPJHdr.InvoiceDate), 8, false));
                // Blank Filler
                sb.Append(getCharacter(" ", 54, false));

                foreach (var FPJDtl in listFPJDtl)
                {
                    sb.AppendLine();
                    // Record ID
                    sb.Append(getCharacter("1", 1, false));
                    // Order Number
                    sb.Append(getCharacter(FPJDtl.OrderNo, 15, false));
                    // Sales Number
                    sb.Append(getCharacter(FPJDtl.DocNo.ToString().Substring(7), 6, false));
                    // Sales Number Date
                    sb.Append(getCharacter(formatDateTime(Convert.ToDateTime(FPJDtl.DocDate)), 8, false));
                    // Case Number 
                    sb.Append(getCharacter(" ", 15, false));
                    // Part Number Order
                    sb.Append(getCharacter(FPJDtl.PartNoOriginal.ToString(), 15, false));
                    // Part Number to be shipped
                    sb.Append(getCharacter(FPJDtl.PartNo.ToString(), 15, false));
                    // Shipped Quantity
                    sb.Append(getCharacter(FPJDtl.QtyBill.ToString(), 9, true));
                    // Price
                    SpTrnSFPJDtl recordDtl = ctx.SpTrnSFPJDtls.Find(user.CompanyCode, user.BranchCode, recordFPJHdr.FPJNo,
                        "00", FPJDtl.PartNo, FPJDtl.PartNoOriginal, FPJDtl.DocNo);
                    sb.Append(getCharacter(recordDtl.RetailPrice.ToString(), 10, true));
                    // Discount Pct
                    sb.Append(getCharacter(recordDtl.DiscPct.ToString(), 6, true));
                    // Discount 
                    sb.Append(getCharacter(recordDtl.DiscAmt.ToString(), 10, true));
                    // Net Amount
                    sb.Append(getCharacter(recordDtl.NetSalesAmt.ToString(), 15, true));
                    // Process Date
                    sb.Append(getCharacter(recordFPJHdr.InvoiceDate.ToString("yyyyMMdd"), 8, false));
                }

                return sb;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult GetDataByPickingListNo(string PickingListNo)
        {
            var Uid = CurrentUser;
            string sql = string.Format("exec uspfn_spGetPickingListByNo '{0}', '{1}', '{2}', '{3}', '{4}', '{5}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, ProductType, ProfitCenter, PickingListNo);
            var data = ctx.Database.SqlQuery<PickingList>(sql).FirstOrDefault();
            if (data != null)
            {
                return Json(new { success = true, data, message = "" });
            }
            else
            {
                return Json(new { success = false, message = "data not found" });
            }
        }

        public JsonResult GetDataByNoFPJ(string FPJNo)
        {
            var Uid = CurrentUser;
            string sql = string.Format("exec uspfn_spGetFPJLookUpByNo '{0}', '{1}', '{2}', '{3}', '{4}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, "0", FPJNo);
            var data = ctx.Database.SqlQuery<FPJLookup>(sql).FirstOrDefault();
            if (data != null)
            {
                return Json(new { success = true, data, message = "" });
            }
            else
            {
                return Json(new { success = false, message = "data not found" });
            }
        }
    }
}
