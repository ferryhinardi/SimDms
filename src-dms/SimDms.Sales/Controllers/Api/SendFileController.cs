using SimDms.Sales.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.Models;
using System.Data;
using System.Text;
using SimDms.Sales.Models;
using SimDms.Common.DcsWs;
using System.IO;

namespace SimDms.Sales.Controllers.Api
{
    public class SendFileController : BaseController
    {
        private const string DataId = "SFREQ";
        private DcsWsSoapClient ws = new DcsWsSoapClient();
        public JsonResult Select4Send(DateTime dateFrom, DateTime dateTo, bool isCBU, bool isFPOL)
        {
            if (isFPOL)
            {
                var omTrSalesReqBLL = new OmTrSalesReqBLL(ctx, CurrentUser.UserId);
                var records = omTrSalesReqBLL.Select4Send(dateFrom, dateTo, isCBU);
                omTrSalesReqBLL = null;

                return Json(records.toKG());
            }
            else
            {
                var omTrSalesReqBLL = new OmTrSalesReqBLL(ctx, CurrentUser.UserId);
                var records = omTrSalesReqBLL.Select4Send(dateFrom, dateTo);
                omTrSalesReqBLL = null;

                return Json(records.toKG());
            }
        }

        public bool DcsValidated()
        {
            DcsWsSoapClient DcsHelper = new DcsWsSoapClient();
            bool IsDcsValid = DcsHelper.IsValid();
            DcsHelper = null;
            if (IsDcsValid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult DataForDownload()
        {
            ActionResult result = View();
            DateTime DateFrom = Convert.ToDateTime(Request["DateFrom"].ToString());
            DateTime DateTo = Convert.ToDateTime(Request["DateTo"]);
            string RequestFrom = string.IsNullOrEmpty(Request["RequestFrom"]) ? "" : Request["RequestFrom"].ToString();
            string RequestTo = string.IsNullOrEmpty(Request["RequestTo"]) ? "" : Request["RequestTo"].ToString();
            bool IsFPOL = string.IsNullOrEmpty(Request["IsFPOL"]) ? false : Convert.ToBoolean(Request["IsFPOL"]);
            bool IsStock = string.IsNullOrEmpty(Request["IsStock"]) ? false : Convert.ToBoolean(Request["IsStock"].ToString());
            bool IsRFPOL = string.IsNullOrEmpty(Request["IsRFPOL"]) ? false : Convert.ToBoolean(Request["IsRFPOL"]);
            bool IsRequest = string.IsNullOrEmpty(Request["IsRequest"]) ? false : Convert.ToBoolean(Request["IsRequest"].ToString());
            bool IsCBU = string.IsNullOrEmpty(Request["IsCBU"]) ? false : Convert.ToBoolean(Request["IsCBU"].ToString());

            StringBuilder sb = new StringBuilder();
            var sendBLL = new SendBLL(ctx, CurrentUser.UserId);
            if (IsFPOL)
            {
                if (IsRequest)
                {
                    int comparison = string.Compare(RequestFrom, RequestTo, false);
                    if (comparison > 0)
                    {
                        return ThrowException(string.Format(GetMessage(SysMessages.MSG_6001), "No.Request Awal", "No.Request Akhir"));
                    }
                }

                string pCheck; if (RequestFrom != "") pCheck = "1"; else pCheck = "0";
                var recFakturPolisi = sendBLL.GetFakturPolisiDataTableNoReq(DateFrom, DateTo, pCheck, RequestFrom, RequestTo, IsCBU);
                var recCount = recFakturPolisi.Count();
                if (recCount < 1)
                {
                    return ThrowException("Data tidak ditemukan");
                }
                else
                {
                    CoProfile co = ctx.CoProfiles.Find(CompanyCode, BranchCode);
                    string companyName = "";
                    if (co != null)
                    {
                        companyName = (co.CompanyName.ToString() == "") ? "" : co.CompanyName.ToString();
                    }
                    string batchNo = sendBLL.GetFakturPolisiBatchNo();

                    string msg = string.Empty;
                    string flatFile = p_GenerateFakturPolisiFile(recFakturPolisi, recCount, batchNo, ref sb, ref msg);

                    //string lines = File.ReadAllText(flatFile);
                    //call p_GeneretaFile method from client
                    result = p_GeneretaFile("SFREQ", flatFile);

                    //DownloadFile(flatFile);
                }
            }
            else if (IsStock)
            {
                var recHoldings = ctx.OrganizationDtls.Where(p => p.CompanyCode == CompanyCode && p.IsBranch == false).ToList();
                if (recHoldings != null)
                {
                    var recHolding = recHoldings.FirstOrDefault();

                    if (recHolding.BranchCode != BranchCode)
                    {
                        return ThrowException("Maaf cabang anda bukan cabang holding, hanya holding yang di-ijinkan mengirim-kan file stock info !");
                    }

                    var recStock = sendBLL.GetStockDataTable(DateFrom, DateTo);
                    var recCount = recStock.Count(); 
                    if (recCount < 1)
                    {
                        return ThrowException("Data tidak ditemukan");
                    }
                    else
                    {
                        var co = ctx.CoProfiles.Find(CompanyCode, BranchCode);
                        string companyName = "";
                        if (co != null)
                        {
                            companyName = (co.CompanyName.ToString() == "") ? "" : co.CompanyName.ToString();
                        }
                        string batchNo = sendBLL.GetStockBatchNo();

                        string msg = string.Empty;
                        string flatFile = p_GenerateStockFile(recStock, recCount, batchNo, ref msg);
                        //DownloadFile(flatFile);
                        //string lines = File.ReadAllText(flatFile);

                        int counter = 1;
                        sb = new StringBuilder();
                        string[] lines = flatFile.Split('\n');
                        foreach (string var in lines)
                        {
                            if (var.Trim().Length > 0)
                            {
                                if (counter == lines.Length)
                                    sb.Append(var);
                                else
                                    sb.AppendLine(var);
                            }
                            counter++;
                        }

                        result = p_GeneretaFile("SDSTK", sb.ToString());
                    }
                }
            }
            else if (IsRFPOL)
            {
                if (IsRequest)
                {
                    int comparison = string.Compare(RequestFrom, RequestTo, false);
                    if (comparison > 0)
                    {
                        return ThrowException(string.Format(GetMessage(SysMessages.MSG_6001), "No.Revisi Awal", "No.Revisi Akhir"));
                    }
                }

                string pCheck; if (IsRequest) pCheck = "1"; else pCheck = "0";
                var recFakturPolisi = sendBLL.GetRevFakturPolisiDataTableNoRev(DateFrom, DateTo, pCheck, RequestFrom, RequestTo);
                var recCount = recFakturPolisi.Count();
                if (recCount < 1)
                {
                    return ThrowException("Data tidak ditemukan");
                }
                else
                {
                    CoProfile co = ctx.CoProfiles.Find(CompanyCode, BranchCode);
                    string companyName = "";
                    if (co != null)
                    {
                        companyName = (co.CompanyName.ToString() == "") ? "" : co.CompanyName.ToString();
                    }
                    //string batchNo = sendBLL.GetRevFakturPolisiBatchNo();

                    string msg = string.Empty;
                    string flatFile = p_GenerateRevFakturPolisiFile(recFakturPolisi, recCount, ref sb, ref msg);

                    //string lines = File.ReadAllText(flatFile);
                    //call p_GeneretaFile method from client
                    result = p_GeneretaFile("SFREV", flatFile);

                    //DownloadFile(flatFile);
                }
            }
            sendBLL = null;

            return result;
        }

        public JsonResult DataForDownload_Validated(DateTime DateFrom, DateTime DateTo, string RequestFrom = "", string RequestTo = "",
            bool IsRFPOL = false, bool IsFPOL = false, bool IsStock = false, bool IsRequest = false, bool IsCBU = false)
        {
            JsonResult result = Json(new { });
            StringBuilder sb = new StringBuilder();
            var sendBLL = new SendBLL(ctx, CurrentUser.UserId);
            if (IsFPOL)
            {
                if (IsRequest)
                {
                    int comparison = string.Compare(RequestFrom, RequestTo, false);
                    if (comparison > 0)
                    {
                        return ThrowException(string.Format(GetMessage(SysMessages.MSG_6001), "No.Request Awal", "No.Request Akhir"));
                    }
                }

                string pCheck; if (IsRequest) pCheck = "1"; else pCheck = "0";
                var recFakturPolisi = sendBLL.GetFakturPolisiDataTableNoReq(DateFrom, DateTo, pCheck, RequestFrom, RequestTo, IsCBU);
                var recCount = recFakturPolisi != null ? recFakturPolisi.Count() : 0;
                if (recCount < 1)
                {
                    return ThrowException("Data tidak ditemukan");
                }
            }
            else if (IsStock)
            {
                var recHoldings = ctx.OrganizationDtls.Where(p => p.CompanyCode == CompanyCode && p.IsBranch == false).ToList();
                if (recHoldings != null)
                {
                    var recHolding = recHoldings.FirstOrDefault();

                    if (recHolding.BranchCode != BranchCode)
                    {
                        result = ThrowException("Maaf cabang anda bukan cabang holding, hanya holding yang di-ijinkan mengirim-kan file stock info !");
                    }

                    var recStock = sendBLL.GetStockDataTable(DateFrom, DateTo);
                    var recCount = recStock!= null? recStock.Count() : 0;
                    if (recCount < 1)
                    {
                        result = ThrowException("Data tidak ditemukan");
                    }
                }
            }
            else if (IsRFPOL)
            {
                if (IsRequest)
                {
                    int comparison = string.Compare(RequestFrom, RequestTo, false);
                    if (comparison > 0)
                    {
                        return ThrowException(string.Format(GetMessage(SysMessages.MSG_6001), "No.Revisi Awal", "No.Revisi Akhir"));
                    }
                }

                string pCheck; if (IsRequest) pCheck = "1"; else pCheck = "0";
                var recFakturPolisi = sendBLL.GetRevFakturPolisiDataTableNoRev(DateFrom, DateTo, pCheck, RequestFrom, RequestTo);
                var recCount = recFakturPolisi != null ? recFakturPolisi.Count() : 0;
                if (recCount < 1)
                {
                    return ThrowException("Data tidak ditemukan");
                }
            }
            sendBLL = null;

            return result;
        }

        public JsonResult DataForDisplay(DateTime DateFrom, DateTime DateTo, string RequestFrom = "", string RequestTo = "",
            bool IsRFPOL = false, bool IsFPOL = false, bool IsStock = false, bool IsRequest = false, bool IsCBU = false)
        {
            try
            {
                var result = Json(new { });
                StringBuilder sb = new StringBuilder();
                var sendBLL = new SendBLL(ctx, CurrentUser.UserId);
                if (IsFPOL)
                {
                    if (IsRequest)
                    {
                        int comparison = string.Compare(RequestFrom, RequestTo, false);
                        if (comparison > 0)
                        {
                            return ThrowException(string.Format(GetMessage(SysMessages.MSG_6001), "No.Request Awal", "No.Request Akhir"));
                        }
                    }

                    string pCheck; if (IsRequest) pCheck = "1"; else pCheck = "0";
                    var recFakturPolisi = sendBLL.GetFakturPolisiDataTableNoReq(DateFrom, DateTo, pCheck, RequestFrom, RequestTo, IsCBU);
                    var recCount = recFakturPolisi != null ? recFakturPolisi.Count() : 0;
                    if (recCount < 1)
                    {
                        return ThrowException("Data tidak ditemukan");
                    }
                    else
                    {
                        CoProfile co = ctx.CoProfiles.Find(CompanyCode, BranchCode);
                        string companyName = "";
                        if (co != null)
                        {
                            companyName = (co.CompanyName.ToString() == "") ? "" : co.CompanyName.ToString();
                        }
                        string batchNo = sendBLL.GetFakturPolisiBatchNo();

                        string msg = string.Empty;
                        string flatFile = p_GenerateFakturPolisiFile(recFakturPolisi, recCount, batchNo, ref sb, ref msg);

                        //string lines = File.ReadAllText(flatFile);
                        result = Json(new { success = true, code = "SFREQ", data = flatFile, message = msg });
                    }
                }
                else if (IsStock)
                {
                    var recHoldings = ctx.OrganizationDtls.Where(p => p.CompanyCode == CompanyCode && p.IsBranch == false).ToList();
                    if (recHoldings != null)
                    {
                        var recHolding = recHoldings.FirstOrDefault();

                        if (recHolding.BranchCode != BranchCode)
                        {
                            return ThrowException("Maaf cabang anda bukan cabang holding, hanya holding yang di-ijinkan mengirim-kan file stock info !");
                        }

                        var recStock = sendBLL.GetStockDataTable(DateFrom, DateTo);
                        var recCount = recStock != null ? recStock.Count() : 0;
                        if (recCount < 1)
                        {
                            return ThrowException("Data tidak ditemukan");
                        }
                        else
                        {
                            var co = ctx.CoProfiles.Find(CompanyCode, BranchCode);
                            string companyName = "";
                            if (co != null)
                            {
                                companyName = (co.CompanyName.ToString() == "") ? "" : co.CompanyName.ToString();
                            }
                            string batchNo = sendBLL.GetStockBatchNo();

                            string msg = string.Empty;
                            string flatFile = p_GenerateStockFile(recStock, recCount, batchNo, ref msg);
                            //DownloadFile(flatFile);
                            //string lines = File.ReadAllText(flatFile);
                            result = Json(new { success = true, code = "SDSTK", data = flatFile, message = msg });
                        }
                    }
                }
                else
                    if (IsRFPOL)
                    {
                        if (IsRequest)
                        {
                            int comparison = string.Compare(RequestFrom, RequestTo, false);
                            if (comparison > 0)
                            {
                                return ThrowException(string.Format(GetMessage(SysMessages.MSG_6001), "No.Revision Awal", "No.Revision Akhir"));
                            }
                        }

                        string pCheck; if (IsRequest) pCheck = "1"; else pCheck = "0";
                        var recFakturPolisi = sendBLL.GetRevFakturPolisiDataTableNoRev(DateFrom, DateTo, pCheck, RequestFrom, RequestTo);
                        var recCount = recFakturPolisi != null ? recFakturPolisi.Count() : 0;
                        if (recCount < 1)
                        {
                            return ThrowException("Data tidak ditemukan");
                        }
                        else
                        {
                            CoProfile co = ctx.CoProfiles.Find(CompanyCode, BranchCode);
                            string companyName = "";
                            if (co != null)
                            {
                                companyName = (co.CompanyName.ToString() == "") ? "" : co.CompanyName.ToString();
                            }
                            //string batchNo = sendBLL.GetRevFakturPolisiBatchNo();

                            string msg = string.Empty;
                            string flatFile = p_GenerateRevFakturPolisiFile(recFakturPolisi, recCount, ref sb, ref msg);

                            //string lines = File.ReadAllText(flatFile);
                            result = Json(new { success = true, code = "SFREV", data = flatFile, message = msg });
                        }
                    }

                sendBLL = null;

                return result;
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Terjadi Kesalahan, Hubungi SDMS Support", error_log = ex.Message });
            }
        }

        public JsonResult SendToDcs(string Contents)
        {
            var msg = "";
            bool rsl = false;
            string header = Contents.Split('\n')[0];

            var checkOnline = ws.IsValid() ? "online" : "offline";

            if (checkOnline == "online")
            {
                try
                {
                    string result = ws.SendToDcs(DataId, CompanyCode, Contents, ProductType);
                    if (result.StartsWith("FAIL")) return Json(new { success = false, message = result.Substring(5) });

                    msg = string.Format("{0} berhasil di upload (dcs :" + checkOnline + ")", DataId);
                    rsl = true;
                }
                catch
                {
                    msg = string.Format("{0} gagal digenerate (dcs :" + checkOnline + ")", DataId);
                    rsl = false;
                }
            }
            else
            {
                rsl = false;
                msg = string.Format("{0} gagal digenerate (dcs :" + checkOnline + ") ", DataId);
            }

            return Json(new { success = rsl, message = msg });

        }


        #region -- Private Method --
        private string p_GenerateString(string message = "", int length = 0, bool isInteger = false)
        {
            string s = "";
            if (isInteger)
            {
                s = message.PadLeft(length, '0').Substring(0, length);
            }
            else
            {
                s = message.PadRight(length, ' ').Substring(0, length);
            }
            return s;
        }

        //private void p_DownloadFile(string flatFile)
        //{
        //    if (flatFile == string.Empty)
        //    {
        //        XMessageBox.ShowWarning("Direktori tidak ada");
        //        txtNmFile.Focus(); txtNmFile.SelectAll();
        //        return;
        //    }
        //    else
        //    {
        //        string[] lines = File.ReadAllLines(flatFile);
        //        if (lines.Length > 0)
        //            XReportViewer.ShowTextReport(lines);
        //    }
        //}

        private string p_GenerateFakturPolisiFile(IEnumerable<OmSelectFakturPolisi> recFakturPolisi, decimal totItem, string batchNo, ref StringBuilder sb, ref string msg)
        {
            string sDateFile;
            sDateFile = DateTime.Now.ToString("yyyyMMddHHmmss");
            //string path = txtNmFile.Text + "\\SFREQ" + "-" + user.CompanyCode + "-" + sDateFile + ".txt";
            //string productType = "";

            var recLookUpDtl = ctx.LookUpDtls.Find(CompanyCode, GnMstLookUpHdr.ProductType, ProductType);

            string prodType = "";
            if (recLookUpDtl != null)
            {
                prodType = recLookUpDtl.ParaValue.ToString();
            }


            //if (!Directory.Exists(txtNmFile.Text))
            //{
            //    return "";
            //}

            /* Application.DoEvents();
             //StreamWriter sw = new StreamWriter(path);
             pbUpload.Minimum = 0;
             pbUpload.Maximum = dt.Rows.Count;
             pbUpload.Value = 0;
             btnUpload.Enabled = false;
             StringBuilder sb = new StringBuilder();
             */
            string dtlRecord1 = string.Empty;
            string dtlRecord2 = string.Empty;
            string dtlRecord3 = string.Empty;
            string dtlRecord4 = string.Empty;

            string companyName_1 = string.Empty;
            string customerCode = string.Empty;

            LookUpDtl recDtl = ctx.LookUpDtls.Find(CompanyCode, GnMstLookUpHdr.FlagFRQ, "1");
            if (recDtl != null && recDtl.ParaValue == "1")
            {
                string cust = string.Empty;
                var recCPSales = ctx.GnMstCoProfileSaleses.Find(CompanyCode, BranchCode);
                if (recCPSales != null)
                {
                    cust = recCPSales.LockingBy.ToString();
                }
                if (string.IsNullOrEmpty(cust))
                    customerCode = BranchCode;
                else
                    customerCode = cust;
            }
            else
            {
                customerCode = CompanyCode;
            }

            var recOrgHdr = ctx.OrganizationHdrs.Find(CompanyCode);
            if (recOrgHdr != null)
            {
                companyName_1 = recOrgHdr.CompanyName.ToString();
            }

            string header = p_GenerateString("H", 1, false);
            header += p_GenerateString(GnMstLookUpHdr.FlagFRQ, 5, false);
            header += p_GenerateString(customerCode, 7, false);
            header += p_GenerateString("1000000", 7, false);
            header += p_GenerateString(companyName_1, 50, false);
            header += p_GenerateString(totItem.ToString(), 6, true);
            header += p_GenerateString(prodType.ToString(), 1, false);
            header += p_GenerateString(batchNo, 6, false);
            header += p_GenerateString("", 657, false);
            sb.AppendLine(header);
            ///*pbUpload.Value = 0;
            int nCounter = 1;
            int len = Convert.ToInt32(totItem == null ? 0 : totItem);

            if (ProductType == "2W")
            {
                // *** For 2 Wheels ***
                //---------------------//
                foreach (OmSelectFakturPolisi rec in recFakturPolisi)
                {
                    ///*Application.DoEvents();
                    dtlRecord1 = p_GenerateString("1", 1, false);
                    dtlRecord1 += p_GenerateString(rec.ReqNo, 15, false);
                    dtlRecord1 += p_GenerateString((rec.reqDate != null) ? rec.reqDate.ToString("yyyyMMdd") : "", 8, false);
                    dtlRecord1 += p_GenerateString(rec.StatusFaktur, 1, false);
                    dtlRecord1 += p_GenerateString(rec.StandardCode, 15, false);
                    dtlRecord1 += p_GenerateString(rec.StandardCodeDesc, 50, false);
                    dtlRecord1 += p_GenerateString("", 650, false);
                    sb.AppendLine(dtlRecord1);

                    dtlRecord2 = p_GenerateString("2", 1, false);
                    dtlRecord2 += p_GenerateString(rec.ChassisCode, 15, false);
                    dtlRecord2 += p_GenerateString(rec.ChassisNo.ToString(), 10, true);
                    dtlRecord2 += p_GenerateString(rec.DealerCategory, 15, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKNo, 15, false);
                    dtlRecord2 += p_GenerateString(rec.SalesmanName, 100, false);
                    string newline = "\xA";
                    string[] SKPKNameSplit = rec.SKPKName.Split(new char[] { '\n' });
                    if (SKPKNameSplit.Length > 1)
                    {
                        dtlRecord2 += p_GenerateString(SKPKNameSplit[0], 40, false) + "||" + p_GenerateString(SKPKNameSplit[1], 40, false) + p_GenerateString("", 18, false);
                    }
                    else
                    {
                        dtlRecord2 += p_GenerateString(SKPKNameSplit[0], 40, false) + p_GenerateString("", 60, false);
                    }
                    dtlRecord2 += p_GenerateString(rec.SKPKAddress1, 40, false) + p_GenerateString("", 60, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKAddress2, 40, false) + p_GenerateString("", 60, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKAddress3, 34, false) + p_GenerateString("", 66, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKCity, 15, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKTelp1, 15, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKTelp2, 15, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKHP, 15, false);
                    dtlRecord2 += p_GenerateString((rec.SKPKBirthday != null) ? rec.SKPKBirthday.ToString("yyyyMMdd") : "", 8, false);
                    dtlRecord2 += p_GenerateString(rec.ReasonCode, 15, false);
                    dtlRecord2 += p_GenerateString(rec.ReasonNonFaktur, 100, false);
                    dtlRecord2 += p_GenerateString("", 1, false);
                    sb.AppendLine(dtlRecord2);

                    dtlRecord3 = p_GenerateString("3", 1, false);
                    string[] FPNameSplit = rec.FakturPolisiName.Split(new char[] { '\n' });

                    if (FPNameSplit.Length > 1)
                    {
                        dtlRecord3 += p_GenerateString(FPNameSplit[0], 40, false) + "||" + p_GenerateString(FPNameSplit[1], 40, false) + p_GenerateString("", 18, false);
                    }
                    else
                    {
                        dtlRecord3 += p_GenerateString(FPNameSplit[0], 40, false) + p_GenerateString("", 60, false);
                    }
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiAddress1, 40, false) + p_GenerateString("", 60, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiAddress2, 40, false) + p_GenerateString("", 60, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiAddress3, 34, false) + p_GenerateString("", 66, false);
                    dtlRecord3 += p_GenerateString(rec.PostalCode, 15, false);
                    dtlRecord3 += p_GenerateString(rec.PostalCodeDesc, 100, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiCity, 15, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiTelp1, 15, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiTelp2, 15, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiHP, 15, false);
                    dtlRecord3 += p_GenerateString((rec.fakturPolisiBirthday != null) ? rec.fakturPolisiBirthday.ToString("yyyyMMdd") : "", 8, false);
                    dtlRecord3 += p_GenerateString(rec.IsCityTransport, 1, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiNo, 15, false);
                    dtlRecord3 += p_GenerateString((rec.fakturPolisiDate != null) ? rec.fakturPolisiDate.ToString("yyyyMMdd") : "", 8, false);
                    dtlRecord3 += p_GenerateString(rec.IDNo, 40, false);
                    dtlRecord3 += p_GenerateString((rec.isProject != null) ? rec.isProject.ToString() : "false", 1, false);
                    dtlRecord3 += p_GenerateString("", 91, false);
                    sb.AppendLine(dtlRecord3);

                    dtlRecord4 = p_GenerateString("4", 1, false);
                    dtlRecord4 += p_GenerateString(rec.JenisKelamin, 5, false);
                    dtlRecord4 += p_GenerateString(rec.TempatPembelian, 5, false);
                    dtlRecord4 += p_GenerateString(rec.TempatPembelianOther, 30, false);
                    dtlRecord4 += p_GenerateString(rec.KendaraanYgPernahDimiliki, 5, false);
                    dtlRecord4 += p_GenerateString(rec.SumberPembelian, 5, false);
                    dtlRecord4 += p_GenerateString(rec.SumberPembelianOther, 30, false);
                    dtlRecord4 += p_GenerateString(rec.AsalPembelian, 5, false);
                    dtlRecord4 += p_GenerateString(rec.AsalPembelianOther, 30, false);
                    dtlRecord4 += p_GenerateString(rec.InfoSuzukiDari, 5, false);
                    dtlRecord4 += p_GenerateString(rec.InfoSuzukiDariOther, 30, false);
                    dtlRecord4 += p_GenerateString(rec.FaktorPentingMemilihMotor, 5, false);
                    dtlRecord4 += p_GenerateString(rec.PendidikanTerakhir, 5, false);
                    dtlRecord4 += p_GenerateString(rec.PendidikanTerakhirOther, 30, false);
                    dtlRecord4 += p_GenerateString(rec.PenghasilanPerBulan, 5, false);
                    dtlRecord4 += p_GenerateString(rec.Pekerjaan, 5, false);
                    dtlRecord4 += p_GenerateString(rec.PekerjaanOther, 30, false);
                    dtlRecord4 += p_GenerateString(rec.PenggunaanMotor, 5, false);
                    dtlRecord4 += p_GenerateString(rec.PenggunaanMotorOther, 30, false);
                    dtlRecord4 += p_GenerateString(rec.CaraPembelian, 5, false);
                    dtlRecord4 += p_GenerateString(rec.Leasing, 5, false);
                    dtlRecord4 += p_GenerateString(rec.LeasingOther, 30, false);
                    dtlRecord4 += p_GenerateString(rec.JangkaWaktuKredit, 5, false);
                    dtlRecord4 += p_GenerateString(rec.JangkaWaktuKreditOther, 30, false);
                    dtlRecord4 += p_GenerateString(rec.ModelYgPernahDimiliki, 5, false);
                    dtlRecord4 += p_GenerateString("", 394, false);

                    if (nCounter == len)
                        sb.Append(dtlRecord4);
                    else
                        sb.AppendLine(dtlRecord4);

                    msg = "Berhasil generate data " + nCounter + " dari " + len + " record(s)";
                    ///*pbUpload.Value = nCounter;
                    nCounter++;
                    ///*Thread.Sleep(10);
                }
            }
            else
            {
                // *** For 4 Wheels ***
                //---------------------//
                foreach (OmSelectFakturPolisi rec in recFakturPolisi)
                {
                    ///*Application.DoEvents();
                    dtlRecord1 = p_GenerateString("1", 1, false);
                    dtlRecord1 += p_GenerateString(rec.ReqNo, 15, false);
                    dtlRecord1 += p_GenerateString((rec.reqDate != null) ? rec.reqDate.ToString("yyyyMMdd") : "", 8, false);
                    dtlRecord1 += p_GenerateString(rec.StatusFaktur, 1, false);
                    dtlRecord1 += p_GenerateString(rec.StandardCode, 15, false);
                    dtlRecord1 += p_GenerateString(rec.StandardCodeDesc, 50, false);
                    dtlRecord1 += p_GenerateString("", 650, false);
                    sb.AppendLine(dtlRecord1);

                    dtlRecord2 = p_GenerateString("2", 1, false);
                    dtlRecord2 += p_GenerateString(rec.ChassisCode, 15, false);
                    dtlRecord2 += p_GenerateString(rec.ChassisNo.ToString(), 10, true);
                    dtlRecord2 += p_GenerateString(rec.DealerCategory, 15, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKNo, 15, false);
                    dtlRecord2 += p_GenerateString(rec.SalesmanName, 100, false);
                    string newline = "\xA";
                    string[] SKPKNameSplit = rec.SKPKName.Split(new char[] { '\n' });
                    if (SKPKNameSplit.Length > 1)
                    {
                        dtlRecord2 += p_GenerateString(SKPKNameSplit[0], 40, false) + "||" + p_GenerateString(SKPKNameSplit[1], 40, false) + p_GenerateString("", 18, false);
                    }
                    else
                    {
                        dtlRecord2 += p_GenerateString(SKPKNameSplit[0], 40, false) + p_GenerateString("", 60, false);
                    }
                    dtlRecord2 += p_GenerateString(rec.SKPKAddress1, 40, false) + p_GenerateString("", 60, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKAddress2, 40, false) + p_GenerateString("", 60, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKAddress3, 34, false) + p_GenerateString("", 66, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKCity, 15, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKTelp1, 15, false);
                    dtlRecord2 += p_GenerateString((rec.SKPKTelp2 != null) ? rec.SKPKTelp2 : "", 15, false);
                    dtlRecord2 += p_GenerateString(rec.SKPKHP, 15, false);
                    dtlRecord2 += p_GenerateString((rec.SKPKBirthday != null) ? rec.SKPKBirthday.ToString("yyyyMMdd") : "", 8, false);
                    dtlRecord2 += p_GenerateString(rec.ReasonCode, 15, false);
                    dtlRecord2 += p_GenerateString((rec.ReasonNonFaktur != null) ? rec.ReasonNonFaktur : "", 100, false);
                    dtlRecord2 += p_GenerateString("", 1, false);
                    sb.AppendLine(dtlRecord2);

                    dtlRecord3 = p_GenerateString("3", 1, false);
                    string[] FPNameSplit = rec.FakturPolisiName.Split(new char[] { '\n' });

                    if (FPNameSplit.Length > 1)
                    {
                        dtlRecord3 += p_GenerateString(FPNameSplit[0], 40, false) + "||" + p_GenerateString(FPNameSplit[1], 40, false) + p_GenerateString("", 18, false);
                    }
                    else
                    {
                        dtlRecord3 += p_GenerateString(FPNameSplit[0], 40, false) + p_GenerateString("", 60, false);
                    }
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiAddress1, 40, false) + p_GenerateString("", 60, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiAddress2, 40, false) + p_GenerateString("", 60, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiAddress3, 34, false) + p_GenerateString("", 66, false);
                    dtlRecord3 += p_GenerateString(rec.PostalCode, 15, false);
                    dtlRecord3 += p_GenerateString(rec.PostalCodeDesc, 100, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiCity, 15, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiTelp1, 15, false);
                    dtlRecord3 += p_GenerateString((rec.FakturPolisiTelp2 != null) ? rec.FakturPolisiTelp2 : "", 15, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiHP, 15, false);
                    dtlRecord3 += p_GenerateString((rec.fakturPolisiBirthday != null) ? rec.fakturPolisiBirthday.ToString("yyyyMMdd") : "", 8, false);
                    dtlRecord3 += p_GenerateString(rec.IsCityTransport, 1, false);
                    dtlRecord3 += p_GenerateString(rec.FakturPolisiNo, 15, false);
                    dtlRecord3 += p_GenerateString((rec.fakturPolisiDate != null) ? rec.fakturPolisiDate.ToString("yyyyMMdd") : "", 8, false);
                    dtlRecord3 += p_GenerateString(rec.IDNo, 40, false);
                    dtlRecord3 += p_GenerateString((rec.isProject != null) ? rec.isProject.ToString() : "false", 1, false);
                    dtlRecord3 += p_GenerateString("", 91, false);

                    if (nCounter == len)
                        sb.Append(dtlRecord3);
                    else
                        sb.AppendLine(dtlRecord3);

                    msg = "Berhasil generate data " + nCounter + " dari " + len + " record(s)";
                    ///*pbUpload.Value = nCounter;
                    nCounter++;
                    ///*Thread.Sleep(10);
                }
            }
            //File.WriteAllText(path, sb.ToString());
            ///*btnUpload.Enabled = true;
            return sb.ToString();
        }

        private string p_GenerateStockFile(IEnumerable<OmGetStockDataTable> recStock, decimal totItem, string batchNo, ref string msg)
        {
            string sDateFile;
            sDateFile = DateTime.Now.ToString("yyyyMMddHHmmss");
            //string path = txtNmFile.Text + "\\SDSTK" + "-" + user.CompanyCode + "-" + sDateFile + ".txt";

           //StreamWriter sw = new StreamWriter(@"F:\Testing.txt");
            string data = "";

            string companyName_1 = string.Empty;
            string customerCode = string.Empty;

            LookUpDtl recDtl = ctx.LookUpDtls.Find(CompanyCode, GnMstLookUpHdr.FlagSTK, "1");
            if (recDtl != null && recDtl.ParaValue == "1")
            {

                string cust = string.Empty;
                var recCPSales = ctx.GnMstCoProfileSaleses.Find(CompanyCode, BranchCode);
                if (recCPSales != null)
                {
                    cust = recCPSales.LockingBy.ToString();
                }
                if (string.IsNullOrEmpty(cust))
                    customerCode = BranchCode;
                else
                    customerCode = cust;

                var recOrgDtl = ctx.OrganizationDtls.Find(CompanyCode, customerCode);
                if (recOrgDtl != null)
                {
                    companyName_1 = recOrgDtl.BranchName.ToString();
                }
            }
            else
            {
                customerCode = CompanyCode;
                var recOrgHdr = ctx.OrganizationHdrs.Find(CompanyCode);
                if (recOrgHdr != null)
                {
                    companyName_1 = recOrgHdr.CompanyName.ToString();
                }
            }

            string dtlRecord = string.Empty;
            string header = p_GenerateString("H", 1, false);
            header += p_GenerateString(GnMstLookUpHdr.FlagSTK, 5, false);
            header += p_GenerateString(customerCode, 10, false);
            header += p_GenerateString("2000000", 10, false); //1000000
            header += p_GenerateString(companyName_1, 50, false);
            header += p_GenerateString(totItem.ToString(), 6, true);
            header += p_GenerateString(batchNo, 6, false);
            header += p_GenerateString("", 112, false);
            data += header + "\n";
            ///*pbUpload.Value = 0;
            int nCounter = 1;
            int len = Convert.ToInt32(totItem == null ? 0 : totItem);
            foreach (OmGetStockDataTable rec in recStock)
            {
                ///*Application.DoEvents();
                dtlRecord = p_GenerateString(rec.RecordID, 1, false);
                dtlRecord += p_GenerateString(rec.Supplier_CustomerCode, 10, false);
                dtlRecord += p_GenerateString((rec.transactionDate != null) ? rec.transactionDate.ToString("yyyyMMdd") : "", 8, false);
                if (rec.ChassisNo == 0)
                {
                    dtlRecord += p_GenerateString("", 15, false);
                    dtlRecord += p_GenerateString("", 10, false);
                }
                else
                {
                    dtlRecord += p_GenerateString(rec.ChassisCode, 15, false);
                    dtlRecord += p_GenerateString(rec.ChassisNo.ToString(), 10, true);
                }
                dtlRecord += p_GenerateString(rec.DoNo, 15, false);
                dtlRecord += p_GenerateString(rec.transactionType, 2, false);
                dtlRecord += p_GenerateString(rec.ReasonCode, 1, false);

                //dtlRecord += GenerateString(row["EngineCode, 15, false);
                //dtlRecord += GenerateString(row["EngineNo, 10, true);       
                //dtlRecord += GenerateString(row["Supplier/CustomerName, 100, false);

                dtlRecord += p_GenerateString("", 138, false);
                if (!(nCounter == len))
                    data += dtlRecord + "\n";
                else
                    data += dtlRecord;
                msg = "Berhasil generate data " + nCounter + " dari " + len + " record(s)";
                ///*pbUpload.Value = nCounter;
                nCounter++;
                ///*Thread.Sleep(10);
            }

            ///*btnUpload.Enabled = true;
            return data;
        }

        private string p_GenerateRevFakturPolisiFile(IEnumerable<OmSelectRevFakturPolisi> recFakturPolisi, decimal totItem, ref StringBuilder sb, ref string msg)
        {
            List<OmSelectRevFakturPolisi> recFakturPolisi2 = recFakturPolisi.ToList();
            string sDateFile;
            sDateFile = DateTime.Now.ToString("yyyyMMddHHmmss");
            //string path = txtNmFile.Text + "\\SFREQ" + "-" + user.CompanyCode + "-" + sDateFile + ".txt";
            //string productType = "";

            var recLookUpDtl = ctx.LookUpDtls.Find(CompanyCode, GnMstLookUpHdr.ProductType, ProductType);

            string prodType = "";
            if (recLookUpDtl != null)
            {
                prodType = recLookUpDtl.ParaValue.ToString();
            }


            //if (!Directory.Exists(txtNmFile.Text))
            //{
            //    return "";
            //}

            /* Application.DoEvents();
             //StreamWriter sw = new StreamWriter(path);
             pbUpload.Minimum = 0;
             pbUpload.Maximum = dt.Rows.Count;
             pbUpload.Value = 0;
             btnUpload.Enabled = false;
             StringBuilder sb = new StringBuilder();
             */
            string dtlRecord1 = string.Empty;
            string dtlRecord2 = string.Empty;
            string dtlRecord3 = string.Empty;
            string dtlRecord4 = string.Empty;

            string companyName_1 = string.Empty;
            string RevDesc_1 = string.Empty;
            string customerCode = string.Empty;

            LookUpDtl recDtl = ctx.LookUpDtls.Find(CompanyCode, GnMstLookUpHdr.FlagFRV, "1");
            if (recDtl != null && recDtl.ParaValue == "1")
            {
                string cust = string.Empty;
                var recCPSales = ctx.GnMstCoProfileSaleses.Find(CompanyCode, BranchCode);
                if (recCPSales != null)
                {
                    cust = recCPSales.LockingBy.ToString();
                }
                if (string.IsNullOrEmpty(cust))
                    customerCode = BranchCode;
                else
                    customerCode = cust;
            }
            else
            {
                customerCode = CompanyCode;
            }

            var recOrgHdr = ctx.OrganizationHdrs.Find(CompanyCode);
            if (recOrgHdr != null)
            {
                companyName_1 = recOrgHdr.CompanyName.ToString();
            }

            string header = p_GenerateString("H", 1, false);
            header += p_GenerateString(GnMstLookUpHdr.FlagFRV, 5, false);
            header += p_GenerateString(CompanyCode, 7, false);
            header += p_GenerateString("2000000", 7, false);
            header += p_GenerateString(companyName_1, 50, false);
            header += p_GenerateString(totItem.ToString(), 6, true);
            header += p_GenerateString(prodType, 1, false);
            header += p_GenerateString(recFakturPolisi.FirstOrDefault().RevisionNo + " - " + recFakturPolisi.LastOrDefault().RevisionNo, 100, false);
            header += p_GenerateString("", 104, false);
            sb.AppendLine(header);
            ///*pbUpload.Value = 0;
            int nCounter = 1;
            int len = Convert.ToInt32(totItem);

            foreach (OmSelectRevFakturPolisi rec in recFakturPolisi2)
            {
                var recRevDesc = ctx.LookUpDtls.Where(x => x.CompanyCode == CompanyCode && x.CodeID == GnMstLookUpHdr.FPOLRevision && x.LookUpValue == rec.RevisionCode).ToList();
                if (recRevDesc.Count != 0)
                {
                    RevDesc_1 = recRevDesc.FirstOrDefault().LookUpValueName.ToString();
                }
                ///*Application.DoEvents();
                dtlRecord1 = p_GenerateString("1", 1, false);
                dtlRecord1 += p_GenerateString(rec.RevisionNo, 15, false);
                dtlRecord1 += p_GenerateString((rec.RevisionDate != null) ? rec.RevisionDate.ToString("yyyyMMdd") : "", 8, false);
                dtlRecord1 += p_GenerateString(rec.SendCounter.ToString(), 2, true);
                dtlRecord1 += p_GenerateString(rec.RevisionSeq.ToString(), 2, true);
                dtlRecord1 += p_GenerateString(rec.RevisionCode, 5, false);
                dtlRecord1 += p_GenerateString(RevDesc_1, 50, false);
                dtlRecord1 += p_GenerateString(rec.FakturPolisiNo, 15, false);
                dtlRecord1 += p_GenerateString((rec.FakturPolisiDate != null) ? rec.FakturPolisiDate.ToString("yyyyMMdd") : "", 8, false);
                dtlRecord1 += p_GenerateString(rec.ChassisCode, 15, false);
                dtlRecord1 += p_GenerateString(rec.ChassisNo.ToString(), 10, false);
                dtlRecord1 += p_GenerateString(rec.IDNo, 50, false);
                dtlRecord1 += p_GenerateString(rec.FakturPolisiTelp1, 15, false);
                dtlRecord1 += p_GenerateString(rec.FakturPolisiTelp2, 15, false);
                dtlRecord1 += p_GenerateString(rec.FakturPolisiHP, 15, false);
                dtlRecord1 += p_GenerateString((rec.FakturPolisiBirthday != null) ? rec.FakturPolisiBirthday.ToString("yyyyMMdd") : "", 8, false);
                dtlRecord1 += p_GenerateString("", 47, false);
                sb.AppendLine(dtlRecord1);

                dtlRecord2 = p_GenerateString("2", 1, false);
                dtlRecord2 += p_GenerateString(rec.FakturPolisiName, 80, false);
                dtlRecord2 += p_GenerateString(rec.FakturPolisiAddress1, 40, false);
                dtlRecord2 += p_GenerateString(rec.FakturPolisiAddress2, 40, false);
                dtlRecord2 += p_GenerateString(rec.FakturPolisiAddress3, 40, false);
                dtlRecord2 += p_GenerateString(rec.PostalCode, 15, false);
                dtlRecord2 += p_GenerateString(rec.PostalCodeDesc, 50, false);
                dtlRecord2 += p_GenerateString(rec.FakturPolisiCity, 15, false);

                if (nCounter == len)
                    sb.Append(dtlRecord2);
                else
                    sb.AppendLine(dtlRecord2);

                msg = "Berhasil generate data " + nCounter + " dari " + len + " record(s)";
                ///*pbUpload.Value = nCounter;
                nCounter++;
                ///*Thread.Sleep(10);
            }
            //File.WriteAllText(path, sb.ToString());
            ///*btnUpload.Enabled = true;
            return sb.ToString();
        }

        private ActionResult p_GeneretaFile(string code, string flatFile)
        {
            var bytesFile = Encoding.UTF8.GetBytes(flatFile);
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.AddHeader("content-disposition", "attachment;filename=" + code + ".txt");
            using (MemoryStream MyMemoryStream = new MemoryStream(bytesFile))
            {
                MyMemoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
                Response.Close();

                return File(Response.OutputStream, Response.ContentType);
            }
        }

        #endregion
    }
}