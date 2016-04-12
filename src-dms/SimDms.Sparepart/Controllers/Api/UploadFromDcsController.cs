using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Globalization;
using System.Transactions;
using SimDms.Common;
using SimDms.Sparepart.Models;
using SimDms.Common.Models;
using SimDms.Common.DcsWs;
using System.ComponentModel;
using Newtonsoft.Json;

namespace SimDms.Sparepart.Controllers.Api
{
    public class UploadFromDcsController : BaseController
    {
        private UploadTypeDCS uploadType;
        private DcsWsSoapClient ws = new DcsWsSoapClient();
        private string msg = "";
        private int mxLenght = 0;

        private string tranStart = @"DECLARE @intErrorCode INT
BEGIN TRAN" + Environment.NewLine;
        private string tranEnd = @"SELECT @intErrorCode = @@ERROR
                                        IF (@intErrorCode <> 0) GOTO PROBLEM
                                        COMMIT TRAN

                                        PROBLEM:
                                        IF (@intErrorCode <> 0) BEGIN
                                            PRINT @intErrorCode
                                            ROLLBACK TRAN
                                        END";


        #region PPRCD

        private string headerPPRCD = "";
        private StringBuilder sbPPRCDFinal = new StringBuilder();
        private string[] lines = null;
        private bool success = false;
        private bool allBranchPPRCD = false;
        private int progressValue = 0;
        private bool allBranch = false;
        private long CodeId;
        #endregion
        public JsonResult Default()
        {
            var dateFrom = Helpers.StartOfMonth();
            var dateTo = Helpers.EndOfMonth();

            return Json(new { DateFrom = dateFrom, DateTo = dateTo});
        }

        public JsonResult RetrieveDataFromDcs(string PeriodFrom, string PeriodTo, string DataId, bool AllStatus)
        {
            object returnObj = null;
            try
            {
                var listOfData = RetrieveUploadDataV2(PeriodFrom, PeriodTo, DataId, AllStatus);
                return Json(new { success = true, data = listOfData.ToList() });
            }
            catch (Exception ex)
            {

            }
            return Json("");
        }

        public JsonResult WsStatus()
        {
            return Json(new { status = ws.IsValid() ? "Online" : "Offline" });
        }

        public List<GnDcsUploadFile> RetrieveUploadDataV2(string PeriodFrom, string PeriodTo, string DataId, bool AllStatus)
        {

            if (ws.IsValid())
            {
                ///Web Service Online
                var lastId = ctx.Database.SqlQuery<decimal>("select isnull(max(ID), 0) from gnDcsUploadFile where DataID ='" + DataId + "'").FirstOrDefault();
                var CustCode = ctx.Database.SqlQuery<string>(string.Format("exec uspfn_gnGetDcsDealerCode '{0}','{1}','{2}'", CompanyCode, BranchCode, DataId)).FirstOrDefault();
                long LastId = long.Parse(lastId.ToString());

                var WsResponse = ws.RetrieveUploadDataV2(CustCode, DataId, LastId);
                foreach (var data in WsResponse)
                {
                    //insert data to gnDcsUploadFile
                    string[] lines = data.Split('\n');
                    string[] headers = lines[0].Split('|');

                    StringBuilder sbContent = new StringBuilder();

                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (lines[i].Length > 10)
                        {
                            //content += ((i == 1) ? "" : "\n") + lines[i];
                            sbContent.Append(((i == 1) ? "" : "\n") + lines[i]);
                        }
                    }

                    var entity = ctx.GnDcsUploadFiles.Find(decimal.Parse(headers[0]));
                    if (entity == null)
                    {
                        var gnDcsUploadFile = new GnDcsUploadFile();
                        gnDcsUploadFile.ID = decimal.Parse(headers[0]);
                        gnDcsUploadFile.DataID = headers[1];
                        gnDcsUploadFile.CustomerCode = headers[2];
                        gnDcsUploadFile.ProductType = headers[3];
                        gnDcsUploadFile.Contents = sbContent.ToString();
                        gnDcsUploadFile.Status = headers[4];
                        gnDcsUploadFile.CreatedDate = DateTime.ParseExact(headers[5], "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
                        gnDcsUploadFile.Header = lines[1];
                        ctx.GnDcsUploadFiles.Add(gnDcsUploadFile);
                    }
                }
                ctx.SaveChanges();
            }
            string query = "";
            if (AllStatus)
            {
                query = "exec uspfn_spGetDcsUploadFileAllStatus '{0}','{1}','{2}','{3}','{4}'";
            }
            else
            {
                query = "exec uspfn_spGetDcsUploadFile '{0}','{1}','{2}','{3}','{4}'";
            }
            var productType = CurrentUser.CoProfile.ProductType == "2W" ? "2" : "4";
            var productType1 = "";
            var UP = ctx.SysUserProfitCenters.Find(CurrentUser.UserId);
            if (UP.ProfitCenter == "300")
                productType1 = CurrentUser.CoProfile.ProductType == "2W" ? "2" : "4";
            else
                productType1 = CurrentUser.CoProfile.ProductType == "2W" ? "A" : "B";
            var listUpload = ctx.Database.SqlQuery<GnDcsUploadFile>(string.Format(query, productType, productType1, DataId, PeriodFrom, PeriodTo)).ToList();

            return listUpload;
        }

        public JsonResult CheckData(string Id)
        {
            var UploadItem = ctx.GnDcsUploadFiles.Find(decimal.Parse(Id));
            object returnObj = null;
            var lines = UploadItem.Contents.Split('\r');
            if (lines != null)
            {
                if (UploadItem != null)
                {
                    if (UploadItem.Status.Equals("BERHASIL"))
                    {
                        throw new ArgumentException("Data sudah pernah di upload....!");
                    }
                    else
                    {
                        throw new ArgumentException("Data baru");
                    }
                    uploadType = UploadTypeDCS.PINVS;
                }
            }
            return Json(returnObj);
        }

        public JsonResult UploadData(string Id)
        {
            bool result = false;
            var UploadItem = ctx.GnDcsUploadFiles.Find(decimal.Parse(Id));
            string contents = UploadItem.Contents;
            string check = contents.Replace("\r\n", "\n");
            var lines = check.Split('\n');
            //allBranch = rst;
            try
            {

                #region SPAREPART FLAT FILE
                if (lines != null && Validate(lines, UploadTypeDCS.PINVS))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.PINVS;
                    }
                }
                if (lines != null && Validate(lines, UploadTypeDCS.PPRCD))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.PPRCD;
                    }
                }
                if (lines != null && Validate(lines, UploadTypeDCS.PMODP))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.PMODP;
                    }
                }
                if (lines != null && Validate(lines, UploadTypeDCS.PMDLM))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.PMDLM;
                    }
                }
                if (lines != null && Validate(lines, UploadTypeDCS.MSMDL))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.MSMDL;
                    }
                }
                #endregion

                result = ProccessUpload(uploadType, Id, lines);
                return Json(new { success = result, data = lines, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool ProccessUpload(UploadTypeDCS uploadType, string DataId, string[] lines)
        {
            var UploadItem = ctx.GnDcsUploadFiles.Find(decimal.Parse(DataId));
            //var lines = UploadItem.Contents.Split('\r');

            bool success = false;
            bool result = false;
            if (uploadType != UploadTypeDCS.PPRCD && uploadType != UploadTypeDCS.WFRAT)
            {
                try
                {
                    if (lines != null && Validate(lines, uploadType))
                    {
                        success = ProcessUploadDataTuningCtx(lines, uploadType);
                    }

                    if (success)
                    {
                        int hasilUpdateStat = UpdateUploadDataStatusCtx(long.Parse(DataId.ToString()), success);
                        if (hasilUpdateStat != 0)
                        {
                            msg = "Proses Upload Berhasil";
                        }
                        else
                        {
                            msg = "Ada kesalahan saat update Status Upload File";
                        }
                        result = true;
                    }
                    else
                    {
                        msg = msg;
                        result = false;
                    }

                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    result = false;
                }

            }
            else
            {
                if (lines != null && Validate(lines, uploadType))
                {
                    success = ProcessUploadDataTuning(lines, uploadType, long.Parse(DataId.ToString()));

                    if (success)
                    {
                        int hasilUpdateStat = UpdateUploadDataStatusCtx(long.Parse(DataId.ToString()), success);
                        if (hasilUpdateStat != 0)
                        {
                            msg = "Proses Upload Berhasil";
                        }
                        else
                        {
                            msg = "Ada kesalahan saat update Status Upload File";
                        }
                        result = true;
                    }
                    else
                    {
                        msg = msg;
                        result = false;
                    }
                }
            }

            return result;
        }

        [MTAThread]
        private bool ProcessUploadDataTuningCtx(string[] lines, UploadTypeDCS uploadType)
        {
            switch (uploadType)
            {
                // REGION : SPAREPART
                case UploadTypeDCS.PINVS:
                    return UploadDataPINVSLocal(lines);
                //case UploadTypeDCS.PPRCD:
                //    return UploadDataPPRCDTuningLocal(lines);
                case UploadTypeDCS.PMODP:
                    return UploadDataPMODPTuningLocal(lines);
                case UploadTypeDCS.PMDLM:
                    return UploadDataPMDLMLocal(lines);
                case UploadTypeDCS.MSMDL:
                    return UploadDataMSMDLLocal(lines);
                default:
                    return false;
            }
        }

        [MTAThread]
        private bool ProcessUploadDataTuning(string[] lines, UploadTypeDCS uploadType, long DataId)
        {
            switch (uploadType)
            {
                // REGION : SPAREPART
                //case UploadTypeDCS.PINVS:
                //    return UploadDataPINVSLocal(lines);
                case UploadTypeDCS.PPRCD:
                    return UploadDataPPRCDTuningLocal(lines, DataId);
                //case UploadTypeDCS.PMODP:
                //    return UploadDataPMODPTuningLocal(lines);
                //case UploadTypeDCS.PMDLM:
                //    return UploadDataPMDLMLocal(lines);
                //case UploadTypeDCS.MSMDL:
                //    return UploadDataMSMDLLocal(lines);
                //case UploadTypeDCS.SHIST:
                //    return UploadDataSHISTLocal(lines);


                default:
                    return false;
            }
        }

        private static bool Validate(string[] lines, UploadTypeDCS uploadType)
        {
            switch (uploadType)
            {
                // REGION : SPAREPART                       
                case UploadTypeDCS.PINVS:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PINVS"));
                case UploadTypeDCS.PPRCD:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PPRCD"));
                case UploadTypeDCS.PMODP:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PMODP"));
                case UploadTypeDCS.PMDLM:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PMDLM"));
                case UploadTypeDCS.MSMDL:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("MSMDL"));

                default:
                    return false;
            }
        }

        private bool UploadDataPINVSLocal(string[] lines)
        {
            var query = tranStart;
            bool result = false;
            //int rslt = 0;
            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine-1].Length;

            if (maxLinesLast==0)
            {
                maxLine = maxLine - 1;
            }           

            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return false;
            }
            string headerText = lines[0];
            if (headerText.Length < 110)
            {
                msg = "flat file text header < 110 karakter";
                return false;
            }
            // Jika text detail < 110 karakter, return false
            for (int i = 1; i < maxLine; i++)
            {
                //if (lines[i].Length < 126 && lines[i].Length > 0)
                if (lines[i].Length < 126)
                {
                    msg = "flat file text detail ke- " + i + " [ panjang karakter : " + lines[i].Length + "], kurang dari 110 karakter";
                    return false;
                }
            }

            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            int counter = 0;
            decimal num = 0;
            try
            {
                string ccode = user.CompanyCode;
                string bcode = user.BranchCode;
                string typeOfGoods = user.TypeOfGoods;
                DateTime cdate = DateTime.Now;
                string uid = user.UserId;
                //string query = "";
                int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

                //insert into spUtlPINVDHdr - header
                query += string.Format(" INSERT INTO spUtlPINVDHdr (CompanyCode,BranchCode,DealerCode,RcvDealerCode,ShipToDealerCode,InvoiceNo," +
                                        " DeliveryNo,InvoiceDate,DeliveryDate,BinningNo,BinningDate,Status,CreatedBy,CreatedDate,LastUpdateBy," +
                                        " LastUpdateDate, TypeOfGoods) " +
                                        " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}', '0')",
                                        ccode, bcode, headerText.Substring(6, 10).Trim(), headerText.Substring(16, 10).Trim(), headerText.Substring(26, 10).Trim(),
                                        headerText.Substring(42, 15).Trim(), headerText.Substring(65, 6).Trim(), DateTime.Now,
                                        DateTime.Now, string.Empty, "1900-01-01 00:00:00.000", "0", uid, cdate.ToString(), uid, cdate.ToString());

                query += Environment.NewLine;

                num = lines.Length - 1;
                //query = "";
                LookUpDtl oGnMstLookUpDtl = null;
                SpMstItemInfo SpMstItemInfo = null;
                decimal orderUnit = 0;
                decimal decQtyShipped = 0, decPurchasePrice = 0, decDiscAmt = 0;
                for (int i = 1; i < maxLine; i++)
                {
                    counter++;
                    string detailText = lines[i];

                    spMstItemPrice oMstItemPrice = ctx.spMstItemPrices.Find(ccode, bcode, detailText.Substring(45, 15).Trim());
                    spTrnPPOSHdr dtPosHdr = ctx.spTrnPPOSHdrs.Find(user.CompanyCode, user.BranchCode, detailText.Substring(1, 13).Trim());
                    spMstItem oMstItems = ctx.spMstItems.Find(ccode, bcode, detailText.Substring(45, 15).Trim());

                    var x1 = detailText.Substring(45, 15).Trim();
                    var x2 = detailText.Substring(1, 13).Trim();
                    var x3 = detailText.Substring(45, 15).Trim();

                    if (oMstItemPrice == null)
                    {
                        // process gagal remove header
                        //var oUtlPINVDHdr = ctx.SpUtlPINVDHdrs.Find(ccode, BranchCode, headerText.Substring(6, 10).Trim(), headerText.Substring(65, 6), typeOfGoods);
                        //if (oUtlPINVDHdr != null)
                        //{
                        //    ctx.SpUtlPINVDHdrs.Remove(oUtlPINVDHdr);
                        //    ctx.SaveChanges();
                        //}

                        ////remove detail
                        //var sql = string.Format("Delete from spUtlPINVDDtl where CompanyCode = '{0}' and BranchCode = '{1}' and DealerCode ='{2}' and DeliveryNo='{3}'", ccode, BranchCode, headerText.Substring(6, 10).Trim(), headerText.Substring(65, 6));
                        //ctx.Database.ExecuteSqlCommand(sql);

                        msg = "Ada kesalahan di data detail Upload File";
                        result = false;
                        return result;
                    }

                    string tpGo = "";
                    string partCat = "";

                    if (oMstItems == null)
                    {
                        tpGo = "0";
                        partCat = "SPR";
                    }
                    else
                    {
                        tpGo = oMstItems.TypeOfGoods.ToString();
                        partCat = oMstItems.PartCategory.ToString();
                    }

                    oGnMstLookUpDtl = ctx.LookUpDtls.Find(user.CompanyCode, "POCON", detailText.Substring(45, 15).Trim());
                    if (oGnMstLookUpDtl != null)
                    {
                        if (oGnMstLookUpDtl.ParaValue == "1")
                        {
                            SpMstItemInfo = ctx.SpMstItemInfos.Find(user.CompanyCode, detailText.Substring(45, 15).Trim());
                            orderUnit = Convert.ToDecimal(Convert.ToString(SpMstItemInfo.OrderUnit));
                            if (SpMstItemInfo != null)
                            {
                                decQtyShipped = Convert.ToDecimal(detailText.Substring(75, 9).Trim()) * orderUnit;
                                decPurchasePrice = Convert.ToDecimal(detailText.Substring(84, 10).Trim()) / orderUnit;
                                decDiscAmt = Convert.ToDecimal(detailText.Substring(100, 10).Trim()) / orderUnit;
                            }
                            else
                            {
                                decQtyShipped = Convert.ToDecimal(detailText.Substring(75, 9).Trim());
                                decPurchasePrice = Convert.ToDecimal(detailText.Substring(84, 10).Trim());
                                decDiscAmt = Convert.ToDecimal(detailText.Substring(100, 10).Trim());
                            }
                        }
                        else
                        {
                            decQtyShipped = Convert.ToDecimal(detailText.Substring(75, 9).Trim());
                            decPurchasePrice = Convert.ToDecimal(detailText.Substring(84, 10).Trim());
                            decDiscAmt = Convert.ToDecimal(detailText.Substring(100, 10).Trim());
                        }
                    }
                    else
                    {
                        decQtyShipped = Convert.ToDecimal(detailText.Substring(75, 9).Trim());
                        decPurchasePrice = Convert.ToDecimal(detailText.Substring(84, 10).Trim());
                        decDiscAmt = Convert.ToDecimal(detailText.Substring(100, 10).Trim());
                    }

                    //insert into SpUtlPINVDDtl - Detail
                    query += string.Format("INSERT INTO spUtlPINVDDtl (CompanyCode,BranchCode,DealerCode,OrderNo,SalesNo,SalesDate,CaseNumber" +
                                           " ,PartNo,DeliveryNo,PartNoShip,QtyShipped,PurchasePrice,DiscPct,DiscAmt,SalesUnit,CostPrice,TotInvoiceAmt,ProcessDate,ProductType,PartCategory" +
                                            " ,Status,SupplierCode,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate, TypeOfGoods) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'" +
                                            " ,'{9}','{10}',{11},{12},{13},{14},{15},{16},'{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}')",
                                            ccode, bcode, headerText.Substring(6, 10).Trim(), detailText.Substring(1, 13).Trim(), detailText.Substring(16, 6).Trim()
                                            , Convert.ToDecimal(detailText.Substring(22, 8).Trim()), detailText.Substring(30, 15).Trim(), detailText.Substring(45, 15).Trim()
                                            , headerText.Substring(65, 6).Trim(), detailText.Substring(60, 15).Trim(), decQtyShipped, decPurchasePrice
                                            , Convert.ToDecimal(detailText.Substring(94, 6).Trim()) / 100, decDiscAmt, Convert.ToDecimal(detailText.Substring(75, 9).Trim())
                                            , oMstItemPrice.CostPrice, Convert.ToDecimal(detailText.Substring(110, 15)), DateTime.Now, user.CoProfile.ProductType, partCat, "0"
                                            , (dtPosHdr != null) ? dtPosHdr.SupplierCode : string.Empty, uid, cdate, uid, cdate, tpGo);

                    query += Environment.NewLine;
                    counter = 0;

                }

                query += tranEnd;
                if (string.IsNullOrEmpty(query))
                {
                    msg = "Tidak ada data yang akan diupload";
                    return false;
                }
                result = ctx.Database.ExecuteSqlCommand(query) > 0;

                GeneratePINVS(user.CompanyCode, user.BranchCode, headerText.Substring(6, 10).Trim(), headerText.Substring(42, 15).Trim());
                //result = true;
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }
            return result;
        }

        private bool UploadDataPMODPTuningLocal(string[] lines)
        {
            var query = tranStart;
            bool result = false;
            int maxLine = lines.Length;

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ditemukan";
                result = false;
            }

            string headerText = lines[0];
            if (headerText.Length < 80)
            {
                msg = "flat file text header: {'" + headerText.Length + "'}, kurang dari 80 karakter";
                return false;
            }

            for (int i = 1; i < maxLine; i++)
            {
                //if (lines[i].Length < 80 && lines[i].Length > 0) 
                if (lines[i].Length < 80)
                {
                    msg = "flat file text detail ke- " + i + " [ panjang karakter : " + lines[i].Length + "], kurang dari 80 karakter";
                    return false;
                }
            }

            int counter = 0;
            decimal num = 0;

            try
            {
                int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;
                // Create Temporary DataTable to Store Upload Price 
                DataTable dtUploadPrice = new DataTable();
                dtUploadPrice.Columns.Add("OldPartNo", typeof(string));
                dtUploadPrice.Columns.Add("EndMark", typeof(string));
                dtUploadPrice.Columns.Add("NewPartNo", typeof(string));
                dtUploadPrice.Columns.Add("UnitConvertion", typeof(decimal));
                dtUploadPrice.Columns.Add("ReasonCode", typeof(string));
                dtUploadPrice.Columns.Add("InterChangeCode", typeof(string));

                // Looping data yang ada di flat file, dimulai line ke-2
                num = lines.Length - 1;
                for (int i = 0; i < maxLine; i++)
                {
                    if (string.IsNullOrEmpty(lines[i].ToString()))
                        break;

                    counter++;
                    DataRow row = dtUploadPrice.NewRow();
                    row["OldPartNo"] = lines[i].Substring(1, 15).Trim();
                    row["InterChangeCode"] = lines[i].Substring(16, 2).Trim();
                    row["NewPartNo"] = lines[i].Substring(19, 15).Trim();
                    row["EndMark"] = lines[i].Substring(34, 50).Trim();
                    row["ReasonCode"] = lines[i].Substring(84, 9).Trim();
                    dtUploadPrice.Rows.Add(row);
                }

                var user = ctx.SysUsers.Find(CurrentUser.UserId);

                string companyCode = user.CompanyCode;
                string userId = user.UserId;
                string productType = user.CoProfile.ProductType;
                DateTime time = DateTime.Now;
                string sql = string.Empty;

                DataTable dtPartMod = SelectPartNoMod(companyCode, productType);
                DataTable dtPartCategory = SelectPartCategory(companyCode);

                dtPartMod.PrimaryKey = new DataColumn[] { dtPartMod.Columns["PartNo"] };
                dtPartCategory.PrimaryKey = new DataColumn[] { dtPartCategory.Columns["PartNo"] };

                counter = 0;
                num = dtUploadPrice.Rows.Count;

                foreach (DataRow row in dtUploadPrice.Rows)
                {
                    DataRow drPartMod = dtPartMod.Rows.Find(row["OldPartNo"]);
                    if (drPartMod != null)
                    {
                        if ((string)row["OldPartNo"] == (string)drPartMod["PartNo"])
                        {
                            counter++;
                            query += string.Format(" UPDATE spMstItemMod SET NewPartNo = '{0}', InterChangeCode = '{1}', " +
                                                " EndMark = '{2}', LastUpdateBy = '{3}', LastUpdateDate = '{4}' " +
                                                " WHERE CompanyCode = '{5}' AND PartNo = '{6}'",
                                                (string)row["NewPartNo"], (string)row["InterChangeCode"], (string)row["EndMark"],
                                                userId, time, companyCode, (string)row["OldPartNo"]);

                            query += Environment.NewLine;
                        }
                    }
                    else
                    {
                        DataRow drPartCategory = dtPartCategory.Rows.Find(row["OldPartNo"]);
                        if (drPartCategory != null)
                        {
                            counter++;
                            string partCategory = (string)drPartCategory["PartCategory"];
                            query += string.Format("INSERT INTO spMstItemMod VALUES('{0}', '{1}', '{2}', '{3}', '{4}', " +
                                                 " '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}')",
                                                 companyCode, (string)row["OldPartNo"], (string)row["NewPartNo"], 0,
                                                 (string)row["InterChangeCode"], productType, partCategory, (string)row["EndMark"],
                                                 userId, time, userId, time, false, null, null);

                            query += Environment.NewLine;
                        }
                    }

                }

                query += tranEnd;
                if (string.IsNullOrEmpty(query))
                {
                    msg = "Tidak ada data yang akan diupload";
                    return false;
                }
                result= ctx.Database.ExecuteSqlCommand(query) > 0;

            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }

            return result;
        }

        private bool UploadDataPMDLMLocal(string[] lines)
        {
            var query = tranStart;
            bool result = false;
            int maxLine = lines.Length;

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ditemukan";
                return false;
            }

            string headerText = lines[0];
            // Jika text header < 134 karakter, return false
            if (headerText.Length < 133)
            {
                msg = "flat file text header: {'" + headerText.Length + "'}, kurang dari 134 karakter";
                return false;
            }

            // Jika text detail < 134 karakter, return false
            for (int i = 1; i < maxLine; i++)
            {
                if (lines[i].Length < 93)
                {
                    msg = "flat file text detail ke- " + i + " [ panjang karakter : " + lines[i].Length + "], kurang dari 93 karakter";
                    return false;
                }
            }

            // Check jumlah detail berdasarkan informasi dari header
            int counter = 0;
            decimal num = Convert.ToInt32(headerText.Substring(6, 6));
            int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

            if (num != lines.Length - num_fin)
            {
                msg = "informasi jumlah detail di header: {'" + num + "'}, jumlah detail aktual: {'" + (lines.Length - 1) + "'}";
                return false;
            }

            var user = ctx.SysUsers.Find(CurrentUser.UserId);

            try
            {
                DateTime cdate = DateTime.Now;
                string uid = user.UserId;
                string ccode = user.CompanyCode;
                string bcode = user.BranchCode;
                //string queryNew = "";

                for (int i = 0; i < maxLine; i++)
                {
                    if (string.IsNullOrEmpty(lines[i].ToString()))
                        break;

                    counter++;
                    string partno = lines[i].Substring(1, 15);
                    string modelno = lines[i].Substring(16, 30);

                    bool isnew = false;
                    DataRow oMstItemInfo = GetRecordInfo(ccode, partno);
                    DataRow oMstItemModel = GetRecordModel(ccode, partno, modelno);

                    if (oMstItemInfo != null)
                    {
                        if (oMstItemModel == null)
                            isnew = true;
                        if (isnew)
                        {
                            query += string.Format("INSERT SpMstItemModel (CompanyCode, PartNo, ModelCode, PartCategory, " +
                                " CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)" +
                                " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                                ccode, partno, modelno, oMstItemInfo["PartCategory"].ToString(), uid, cdate, uid, cdate);
                        }
                        else
                        {
                            query += string.Format("UPDATE SpMstItemModel SET PartCategory = '{0}', LastUpdateBy = '{1}' ," +
                                " LastUpdateDate = GetDate() WHERE CompanyCode = '{2}' AND PartNo = '{3}' AND ModelCode = '{4}'",
                                oMstItemInfo["PartCategory"].ToString(), user.UserId, user.CompanyCode, partno, modelno);
                        }

                        query += Environment.NewLine;
                    }
                    else
                    {
                        msg = "Invalid Part No, Tidak terdapat informasinya di spMstItemInfo";
                        result = false;
                    }

                }

                query += tranEnd;
                if (string.IsNullOrEmpty(query))
                {
                    msg = "Tidak ada data yang akan diupload";
                    return false;
                }
                result = ctx.Database.ExecuteSqlCommand(query) > 0;

            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }

            return result;
        }

        private bool UploadDataMSMDLLocal(string[] lines)
        {
            var query = tranStart;
            bool result = false;
            int maxLine = lines.Length;

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ditemukan";
                result = false;
            }
            string headerText = lines[0];
            // Jika text header < 117 karakter, return false
            if (headerText.Length < 116)
            {
                msg = "flat file text header: {'" + headerText.Length + "'}, kurang dari 117 karakter";
                result = false;
            }

            // Jika text detail < 117 karakter, return false
            for (int i = 1; i < maxLine; i++)
            {
                //if (lines[i].Length < 116 && lines[i].Length > 0)
                if (lines[i].Length < 116)
                {
                    msg = "flat file text detail ke- " + i + " [ panjang karakter : " + lines[i].Length + "], kurang dari 116 karakter";
                    result = false;
                }
            }

            // Check jumlah detail berdasarkan informasi dari header
            int counter = 0;
            decimal num = Convert.ToInt32(headerText.Substring(7, 6));
            int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;
            if (num != lines.Length - num_fin)
            {
                msg = "informasi jumlah detail di header: {'" + num + "'}, jumlah detail aktual: {'" + (lines.Length - 1) + "'}";
                result = false;
            }

            var user = ctx.SysUsers.Find(CurrentUser.UserId);

            try
            {
                DateTime cdate = DateTime.Now;
                string uid = user.UserId;
                string ccode = user.CompanyCode;
                string codeId = GnMstLookUpHdr.ModelVehicle;

                DataTable dtSeq = GetRecordMstLookUpDtl(ccode, codeId);
                int seq = dtSeq.Rows.Count;
                //string query = "";

                for (int i = 0; i < maxLine; i++)
                {
                    if (string.IsNullOrEmpty(lines[i].ToString()))
                        break;

                    counter++;
                    string linetemp = lines[i].Replace("'", "''");
                    string lookupValue = lines[i].Substring(1, 15);
                    DataRow oGnMstLookUpHdr = GetRecordLookUpHdr(ccode, codeId);
                    DataRow oGnMstLookUpDtl = GetRecordLookUpDtl(ccode, codeId, lookupValue);

                    if (oGnMstLookUpHdr != null)
                    {
                        if (oGnMstLookUpDtl == null)
                        {
                            query += string.Format("INSERT INTO GnMstLookUpDtl (CompanyCode, CodeId, SeqNo, LookupValue, ParaValue," +
                                                    " LookupValueName, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate) " +
                                                    " VALUES('{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                                    ccode, codeId, (seq + i), linetemp.Substring(1, 15), linetemp.Substring(1, 15),
                                                    linetemp.Substring(16, 100), user.UserId, cdate, user.UserId, cdate);
                        }
                        else
                        {
                            query += string.Format("UPDATE GnMstLookUpDtl SET ParaValue = '{0}' , LookupValueName = '{1}' , " +
                                                    " LastUpdateBy = '{2}' , LastUpdateDate = '{3}' WHERE CompanyCode = '{4}' " +
                                                    " AND CodeID = '{5}' AND LookUpValue = '{6}'",
                                                    linetemp.Substring(1, 15), linetemp.Substring(16, 100), user.UserId,
                                                    cdate, ccode, codeId, lookupValue);
                        }

                        query += Environment.NewLine;
                    }
                    else
                    {
                        msg = "Invalid Code ID, Tidak terdapat informasinya di gnMstLookUpHdr";
                        result = false;
                    }
                    //if (ctx.Database.ExecuteSqlCommand(query) < 0)
                    //{
                    //    result = false;
                    //}
                    //query = "";
                    counter = 0;
                }

                query += tranEnd;
                if (string.IsNullOrEmpty(query))
                {
                    msg = "Tidak ada data yang akan diupload";
                    return false;
                }
                result = ctx.Database.ExecuteSqlCommand(query) > 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                result = false; ;
            }

            return result;
        }

        [MTAThread]
        private bool UploadDataPPRCDTuningLocal(string[] lines, long DataId)
        {
            var query = tranStart;
            bool result = false;
            int maxLine = lines.Length;

            int counter = 0;
            decimal retailPriceInclTax = 0;
            decimal retailPrice = 0;
            decimal realpurchasePriceInclTax = 0;
            decimal realpurchasePrice = 0;
            decimal OrigrealpurchasePrice = 0;

            headerPPRCD = lines[0];

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }

            string headerText = lines[0];
            if (headerText.Length < 165)
            {
                msg = "flat file text header : {'" + headerText.Length + "'}, kurang dari 165 karakter";
                result = false;
            }

            // Jika text detail < 165 karakter, return false
            for (int i = 1; i < maxLine; i++)
            {
                //if (lines[i].Length < 165 && lines[i].Length > 0)
                if (lines[i].Length < 165)
                {
                    msg = "flat file text detail ke- " + i + " [ panjang karakter : " + lines[i].Length + "], kurang dari 165 karakter";
                    result = false;
                }
            }

            // Check jumlah detail berdasarkan informasi dari header
            decimal num = Convert.ToInt32(headerText.Substring(13, 6));
            int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;
            if (num != lines.Length - num_fin)
            {
                msg = "informasi jumlah detail di header: {'" + (num.ToString("n2")) + "'}, jumlah detail aktual: {'" + (lines.Length - 1) + "'}";
                result = false;
            }

            var user = ctx.SysUsers.Find(CurrentUser.UserId);

            try
            {
                string companyCode = user.CompanyCode;
                string branchCode = user.BranchCode;
                string productType = user.CoProfile.ProductType;
                var profitCenter = ctx.SysUserProfitCenters.Find(user.UserId);

                string SupplierCD = headerPPRCD.Substring(6, 7);
                DataRow oGnMstSupplier = SelectByStandardCode(companyCode, SupplierCD);
                string SupplierCode = oGnMstSupplier == null ? string.Empty : oGnMstSupplier["SupplierCode"].ToString();

                SupplierProfitCenter oGnMstSupp = ctx.SupplierProfitCenters.Find(companyCode, branchCode, SupplierCode, profitCenter.ProfitCenter);

                string TaxCode = oGnMstSupp.TaxCode;
                string userId = user.UserId;
                DateTime time = DateTime.Now;
                decimal? taxPct = ctx.Taxes.Find(companyCode, TaxCode).TaxPct;
                //string sql = string.Empty;

                bool isRetailPriceIncPPN = ctxMD.GnMstCoProfileSpares.Find(companyCode, branchCode).isRetailPriceIncPPN;
                bool isPurchasePriceIncPPN = ctxMD.GnMstCoProfileSpares.Find(companyCode, branchCode).isPurchasePriceIncPPN;

                decimal discount = 0;
                decimal costPrice = 0;
                decimal oldRetailPrice = 0;
                decimal oldPurchasePrice = 0;
                decimal oldCostPrice = 0;

                DataTable dtItemInfo = SelectPartCategory(companyCode);
                DataTable dtItems = SelectPartNoOnlyWoBranch(companyCode);
                DataTable dtItemPrice = SelectPartNoOnlyWoBranchPrice(companyCode);
                DataTable dtHstPrice = GetPartNoBasedOnMaxUpdateDateWoBranchCode(companyCode);

                dtItemInfo.PrimaryKey = new DataColumn[] { dtItemInfo.Columns["PartNo"] };
                dtItems.PrimaryKey = new DataColumn[] { dtItems.Columns["PartNo"], dtItems.Columns["BranchCode"] };
                dtItemPrice.PrimaryKey = new DataColumn[] { dtItemPrice.Columns["PartNo"], dtItemPrice.Columns["BranchCode"] };
                dtHstPrice.PrimaryKey = new DataColumn[] { dtHstPrice.Columns["PartNo"] };

                for (int i = 1; i < maxLine; i++)
                {
                    if (string.IsNullOrEmpty(lines[i].ToString()))
                    {
                        break;
                    }

                    string partNo = lines[i].Substring(1, 15);
                    string partName = lines[i].Substring(16, 50).Replace("¿", " ").Replace("�", " ").Replace("'", "’");
                    string partCategory = lines[i].Substring(69, 3);
                    decimal salesUnit = Convert.ToDecimal(lines[i].Substring(72, 5));
                    decimal orderUnit = Convert.ToDecimal(lines[i].Substring(77, 5));

                    if (isPurchasePriceIncPPN)
                    {
                        OrigrealpurchasePrice = Convert.ToDecimal(lines[i].Substring(82, 9));
                        realpurchasePriceInclTax = Convert.ToDecimal(lines[i].Substring(82, 9));
                        realpurchasePrice = Math.Truncate(realpurchasePriceInclTax / Convert.ToDecimal((1 + (taxPct / 100))));
                    }
                    else
                    {
                        OrigrealpurchasePrice = Convert.ToDecimal(lines[i].Substring(82, 9));
                        realpurchasePrice = Convert.ToDecimal(lines[i].Substring(82, 9));
                        realpurchasePriceInclTax = Math.Truncate(realpurchasePrice * Convert.ToDecimal((1 + (taxPct / 100))));
                    }

                    if (isRetailPriceIncPPN)
                    {
                        retailPriceInclTax = Convert.ToDecimal(lines[i].Substring(82, 9));
                        retailPrice = Math.Truncate(retailPriceInclTax / Convert.ToDecimal((1 + (taxPct / 100))));
                    }
                    else
                    {
                        retailPrice = Convert.ToDecimal(lines[i].Substring(82, 9));
                        retailPriceInclTax = Math.Truncate(retailPrice * Convert.ToDecimal((1 + (taxPct / 100))));
                    }

                    // Insert/Update spMstItemInfo
                    DataRow drInfo = dtItemInfo.Rows.Find(partNo);
                    counter++;

                    if (drInfo != null)
                    {
                        query += string.Format("UPDATE spMstItemInfo SET SalesUnit = '{0}', OrderUnit = '{1}', " +
                                            " PurchasePrice = '{2}', LastUpdateBy = '{3}', LastUpdateDate = '{4}', " +
                                            " PartName = '{7}' WHERE CompanyCode = '{5}' AND PartNo = '{6}'",
                                            salesUnit, orderUnit, OrigrealpurchasePrice, userId, time, companyCode, partNo, partName);

                        query += Environment.NewLine;
                    }
                    else
                    {
                        query += string.Format("INSERT INTO spMstItemInfo VALUES('{0}', '{1}', '{2}', '{3}', '{4}'," +
                                             " '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', " +
                                             " '{14}', '{15}', '{16}', '{17}', '{18}', '{19}')",
                                            companyCode, partNo, SupplierCode, partName, true, 0, salesUnit, orderUnit,
                                            OrigrealpurchasePrice, "PCS", "1", productType, partCategory, userId, time,
                                            userId, time, false, null, null);
                        query += Environment.NewLine;
                    }

                    //if (ctx.Database.ExecuteSqlCommand(sql) == 0)
                    //{
                    //    msg = "Proses upload part MstItemPrice gagal";
                    //    result = false;
                    //}
                    //sql = string.Empty;

                    DataRow[] drItems = dtItems.Select("BranchCode = '" + branchCode + "' AND PartNo = '" + partNo + "'");
                    if (drItems != null)
                    {
                        // Insert/Update spMstItemPrice
                        DataRow[] drPrice = dtItemPrice.Select("BranchCode = '" + branchCode + "' AND PartNo = '" + partNo + "'");

                        if (drPrice.Length != 0)
                        {
                            query += string.Format("UPDATE spMstItemPrice SET RetailPrice = '{0}'," +
                                                 " RetailPriceInclTax = '{1}', PurchasePrice = '{2}', OldRetailPrice = '{3}'," +
                                                 " OldPurchasePrice = '{4}', LastUpdateBy = '{5}', LastUpdateDate = '{6}' " +
                                                 " WHERE CompanyCode = '{7}'AND BranchCode = '{8}' AND PartNo = '{9}'",
                                                    retailPrice, retailPriceInclTax, realpurchasePrice, (decimal)drPrice[0]["RetailPrice"],
                                                    (decimal)drPrice[0]["PurchasePrice"], userId, time, companyCode, branchCode, partNo);
                            query += Environment.NewLine;
                        }

                        //if (ctx.Database.ExecuteSqlCommand(sql) == 0)
                        //{
                        //    msg = "Proses upload part MstItemPrice gagal";
                        //    result = false;
                        //}
                        //sql = string.Empty;

                        //Insert spHstItemPrice
                        //DataRow[] drHst = dtHstPrice.Select("BranchCode = '" + branchCode + "' AND PartNo = '" + partNo + "'");
                        DataRow drHst = dtHstPrice.Rows.Find(partNo);

                        if (drHst != null)
                        {
                            discount = (decimal)drHst["Discount"];
                            oldRetailPrice = (decimal)drHst["RetailPrice"];
                            oldPurchasePrice = (decimal)drHst["PurchasePrice"];
                            oldCostPrice = (decimal)drHst["CostPrice"];
                            costPrice = oldCostPrice;

                            DateTime lastPurchasePriceUpdate = drHst["LastPurchaseUpdate"].ToString() == string.Empty ?
                                                                   time : Convert.ToDateTime(drHst["LastPurchaseUpdate"].ToString());
                            DateTime lastRetailPriceUpdate = drHst["LastRetailPriceUpdate"].ToString() == string.Empty ?
                                                               time : Convert.ToDateTime(drHst["LastRetailPriceUpdate"].ToString());

                            query += string.Format("INSERT INTO spHstItemPrice VALUES('{0}', '{1}', '{2}'," +
                                                 " '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', " +
                                                 " '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '0')",
                                                  companyCode, branchCode, partNo, DateTime.Now, retailPrice, retailPriceInclTax,
                                                  realpurchasePrice, costPrice, discount.ToString().Replace(',', '.'),
                                                  oldRetailPrice.ToString(), oldPurchasePrice.ToString(), oldCostPrice,
                                                  discount, lastPurchasePriceUpdate, lastRetailPriceUpdate, userId, time, userId, time);
                            query += Environment.NewLine;
                        }
                        else
                        {
                            discount = 0;
                            oldRetailPrice = 0;
                            oldPurchasePrice = 0;
                            oldCostPrice = 0;
                            costPrice = realpurchasePrice;

                            query += string.Format("INSERT INTO spHstItemPrice VALUES('{0}', '{1}', '{2}'," +
                                                 " '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}'," +
                                                 " '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}','0')",
                                                 companyCode, branchCode, partNo, DateTime.Now, retailPrice, retailPriceInclTax,
                                                 realpurchasePrice, costPrice, discount.ToString().Replace(',', '.'),
                                                 oldRetailPrice, oldPurchasePrice, oldCostPrice, discount.ToString().Replace(',', '.'),
                                                 null, null, userId, time, userId, time);
                            query += Environment.NewLine;
                        }
                    }

                    //if (counter != 0 && (counter % 50 == 0) && !string.IsNullOrEmpty(sql))
                    //{
                    //    if (ctx.Database.ExecuteSqlCommand(sql) == 0)
                    //    {
                    //        msg = "Proses upload part price gagal";
                    //        result = false;
                    //    }
                    //    sql = string.Empty;
                    //}

                }
                //result = true;
                query += tranEnd;
                if (string.IsNullOrEmpty(query))
                {
                    msg = "Tidak ada data yang akan diupload";
                    return false;
                }
                result = ctx.Database.ExecuteSqlCommand(query) > 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                result = false;
            }

            return result;
        }

        #region *** Support PINVS ***
        public int UpdateUploadDataStatusCtx(long id, bool success)
        {
            int result = 0;
            string status = (success) ? "P" : "X";

            //update status gnDcsUploadFile
            var sql = string.Format("update gnDcsUploadFile set Status = '{0}', UpdatedDate = getdate() where ID ='{1}'", status, id);
            result = ctx.Database.ExecuteSqlCommand(sql);

            //update status ws untuk test local di command
            //ws.UpdateUploadDataStatus(id, status);

            return result;
        }

        public void GeneratePINVS(string companyCode, string branchCode, string dealerCode, string deliveryNo)
        {
            var sql = string.Format("exec uspfn_GeneratePINVS '{0}','{1}','{2}','{3}'", companyCode, branchCode, dealerCode, deliveryNo);
            ctx.Database.ExecuteSqlCommand(sql);

        }
        #endregion

        #region *** Support PMODP ***
        public DataTable SelectPartNoMod(string CompanyCode, string ProductType)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT distinct a.PartNo, a.PartCategory FROM spMstItemInfo a " +
                                    " INNER JOIN spMstItemMod b ON a.CompanyCode = b.CompanyCode AND a.PartNo = b.PartNo " +
                                    " WHERE a.CompanyCode = '{0}' AND a.ProductType = '{1}'", CompanyCode, ProductType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        public DataTable SelectPartCategory(string CompanyCode)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT PartNo, PartCategory FROM spMstItemInfo" +
                                            " WHERE CompanyCode = '{0}'", CompanyCode);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }
        #endregion

        #region *** Support PMDLM ***
        public DataRow GetRecordInfo(string CompanyCode, string PartNo)
        {
            DataRow dr;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT TOP 1 * FROM SpMstItemInfo WHERE CompanyCode = '{0}' AND PartNo = '{1}'", CompanyCode, PartNo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                dr = dt.Rows[0];
                return dr;
            }
            else return null;

        }

        public DataRow GetRecordModel(string CompanyCode, string PartNo, string ModelNo)
        {
            DataRow dr;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT TOP 1 * FROM SpMstItemModel WHERE CompanyCode = '{0}' AND PartNo = '{1}' and ModelCode = '{2}''", CompanyCode, PartNo, ModelNo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                dr = dt.Rows[0];
                return dr;
            }
            else return null;
        }
        #endregion

        #region *** Support MSMDL ***
        public DataTable GetRecordMstLookUpDtl(string CompanyCode, string CodeID)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT * FROM gnMstLookUpDtl" +
                                            " WHERE CompanyCode = '{0}' AND CodeID='{0}'", CompanyCode, CodeID);

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }

        public DataRow GetRecordLookUpHdr(string CompanyCode, string CodeID)
        {
            DataRow dr;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT TOP 1 * FROM GnMstLookUpHdr WHERE CompanyCode = '{0}' AND CodeID = '{1}'", CompanyCode, CodeID);

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                dr = dt.Rows[0];
                return dr;
            }
            else return null;
        }

        public DataRow GetRecordLookUpDtl(string CompanyCode, string CodeID, string LookupValue)
        {
            DataRow dr;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT TOP 1 * FROM GnMstLookUpDtl WHERE CompanyCode = '{0}' AND CodeID = '{1}' AND LookUpValue = '{2}'", CompanyCode, CodeID, LookupValue);

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                dr = dt.Rows[0];
                return dr;
            }
            else return null;
        }

        #endregion

        #region *** Support PPRCD ***

        private DataTable SelectOrgBranch()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("select * from gnMstOrganizationDtl");

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }

        private DataRow SelectByStandardCode(string CompanyCode, string SupplierCD)
        {
            DataRow dr;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT SupplierCode FROM gnMstSupplier WHERE CompanyCode = '{0}' AND StandardCode = '{1}'", CompanyCode, SupplierCD);

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                dr = dt.Rows[0];
                return dr;
            }
            else
                return null;
        }

        private DataTable SelectPartNoOnlyWoBranch(string CompanyCode)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT BranchCode, PartNo FROM spMstItems WITH(NOLOCK, NOWAIT) WHERE CompanyCode = '{0}'", CompanyCode);

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }

        private DataTable SelectPartNoOnlyWoBranchPrice(string CompanyCode)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT BranchCode,PartNo, ISNULL(RetailPrice, 0) RetailPrice, ISNULL(PurchasePrice, 0) PurchasePrice " +
                                            " FROM spMstItemPrice  WITH(NOLOCK, NOWAIT) WHERE CompanyCode = '{0}'", CompanyCode);

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }

        private DataTable GetPartNoBasedOnMaxUpdateDateWoBranchCode(string CompanyCode)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT a.BranchCode, a.PartNo, a.UpdateDate, ISNULL(RetailPrice,0) AS RetailPrice, " +
                                            " ISNULL(RetailPriceInclTax,0) AS RetailPriceInclTax,  ISNULL(PurchasePrice,0) AS PurchasePrice," +
                                            " ISNULL(CostPrice,0) AS CostPrice, ISNULL(Discount,0) AS Discount, ISNULL(OldRetailPrice,0) AS OldRetailPrice, " +
                                            " ISNULL(OldPurchasePrice,0) AS OldPurchasePrice, ISNULL(OldCostPirce,0) AS OldCostPirce," +
                                            " ISNULL(OldDiscount,0) AS OldDiscount, LastPurchaseUpdate, LastRetailPriceUpdate " +
                                            " FROM spHstItemPrice a " +
                                            " INNER JOIN (SELECT  PartNo, MAX(UpdateDate) UpdateDate FROM spHstItemPrice b " +
                                            " WHERE b.CompanyCode = '{0}' GROUP BY PartNo) LastUpdate ON LastUpdate.PartNo = a.PartNo " +
                                            " AND LastUpdate.UpdateDate = a.UpdateDate WHERE a.CompanyCode = '{0}'", CompanyCode);

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }

        private int UpdateUploadDataStatusCtxWoDCS(long id, bool success)
        {
            int result = 0;
            string status = (success) ? "P" : "X";

            var sql = string.Format("update gnDcsUploadFile set Status = '{0}', UpdatedDate = getdate() where ID ='{1}'", status, id);
            result = ctx.Database.ExecuteSqlCommand(sql);

            ws.UpdateUploadDataStatus(id, status);

            return result;

        }

        #endregion

        public JsonResult MappingData(string Id)
        {
            DcsDataMap dcsDataMap = new DcsDataMap();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            List<DataTable> listDt = new List<DataTable>();

            var UploadItem = ctx.GnDcsUploadFiles.Find(decimal.Parse(Id));
            string codeID = UploadItem.DataID;
            string contents = UploadItem.Contents;
            string check = contents.Replace('\r', '\n');
            var lines = check.Split('\n');
            string data = lines[0].ToString();

            cmd.CommandText = string.Format("select sum(FieldLength) from sysFlatFileHdr where CodeID = '{0}'", codeID);

            SqlDataAdapter sda1 = new SqlDataAdapter(cmd);
            DataTable dt1 = new DataTable();
            sda1.Fill(dt1);
            mxLenght = Convert.ToInt32(dt1.Rows[0][0].ToString());

            DataTable dtHeader;
            //dtHeader = MappingDataHeader(codeID, data, lines, cmd);
            dtHeader = dcsDataMap.MappingDataHeader(codeID, lines);
            if (GetCountDetail(codeID, cmd) == 1)
            {
                listDt = MappingDataDetail(Id, lines, cmd);
                return Json(new { success = true, data = dtHeader, detail = listDt });
            }
            else
            {
                listDt = MappingDataDetail(Id, lines, cmd, GetCountDetail(codeID, cmd));
                return Json(new { success = false });
            }
        }

        #region *** move to dcsdatamap ***
        //private DataTable MappingDataHeader(string CodeId, string data, string[] lines, SqlCommand cmd)
        //{
        //    string line = "";

        //    cmd.CommandText = string.Format("select *, convert(varchar(200), '') FieldValue from sysFlatFileHdr where CodeID = '{0}' order by SeqNo", CodeId);
        //    SqlDataAdapter sda2 = new SqlDataAdapter(cmd);
        //    DataTable dt2 = new DataTable();
            
        //    sda2.Fill(dt2);

        //    line = lines[0].PadRight(mxLenght, ' ');

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("FieldDesc", typeof(string));
        //    dt.Columns.Add("FieldValue", typeof(string));

        //    for (int i = 0; i < dt2.Rows.Count; i++)
        //    {
        //        int pos = Convert.ToInt32(dt2.Rows[i][2].ToString()) - 1;
        //        int len = Convert.ToInt32(dt2.Rows[i][3].ToString());
        //        string s = line.Substring(pos, len);
        //        string t = dt2.Rows[i][5].ToString();

        //        DataRow row = dt.NewRow();
        //        row["FieldDesc"] = t;
        //        row["FieldValue"] = s;
        //        dt.Rows.Add(row);
        //    }
        //    int test = dt.Rows.Count;
        //    return dt;
        //}
        #endregion

        private List<DataTable> MappingDataDetail(string CodeId, string[] lines, SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            List<DataTable> listDt = new List<DataTable>();
            string line = "";
            var UploadItem = ctx.GnDcsUploadFiles.Find(decimal.Parse(CodeId));
            string codeID = UploadItem.DataID;

            cmd.CommandText = string.Format("select * from sysFlatFileDtl where CodeID = '{0}' order by SeqNo", codeID);
            SqlDataAdapter sda2 = new SqlDataAdapter(cmd);
            DataTable dt2 = new DataTable("Data Detail");
            sda2.Fill(dt2);

            int maxLine = lines.Length;
            int maxLinelast = lines[maxLine-1].Length;
            if (maxLinelast==0 || maxLine==null)
            {
                maxLine = maxLine - 1;
            }
           

            if (lines.Length < 51)
            {
                dt.Columns.Add("FieldDesc", typeof(string));
                for (int x = 0; x < dt2.Rows.Count; x++)
                {
                    DataRow row = dt.NewRow();
                    //for (int i = 1; i < lines.Length; i++)
                    for (int i = 1; i < maxLine; i++)
                    {
                        if (x == 0)
                        {
                            dt.Columns.Add("FieldValue" + i, typeof(string));
                        }

                        line = lines[i].PadRight(mxLenght, ' ');
                        int pos = Convert.ToInt32(dt2.Rows[x][3].ToString()) - 1;
                        int len = Convert.ToInt32(dt2.Rows[x][4].ToString());
                        string s = line.Substring(pos, len);
                        string t = dt2.Rows[x][6].ToString();

                        if (i == 1)
                        {
                            row["FieldDesc"] = t;
                        }

                        row["FieldValue" + i] = s;
                    }
                    dt.Rows.Add(row);
                }
                listDt.Add(dt);

                int test = dt.Rows.Count;
            }
            else
            {
                for (int i = 1; i < lines.Length; i++)
                {
                    DataRow row = dt.NewRow();
                    line = lines[i].PadRight(mxLenght, ' ');
                    for (int x = 0; x < dt2.Rows.Count; x++)
                    {
                        int pos = Convert.ToInt32(dt2.Rows[x][3].ToString()) - 1;
                        int len = Convert.ToInt32(dt2.Rows[x][4].ToString());
                        string s = line.Substring(pos, len);
                        string t = dt2.Rows[x][5].ToString();

                        if (i == 1)
                        {
                            dt.Columns.Add(t, typeof(string));
                        }
                        row[t] = s;
                    }
                    dt.Rows.Add(row);
                }
                listDt.Add(dt);
            }

            return listDt;
        }

        private List<DataTable> MappingDataDetail(string CodeId, string[] lines, SqlCommand cmd, int countDtl)
        {
            DataTable dt = new DataTable();
            List<DataTable> listDt = new List<DataTable>();
            string line = "";
            var UploadItem = ctx.GnDcsUploadFiles.Find(decimal.Parse(CodeId));
            string codeID = UploadItem.DataID;

            cmd.CommandText = string.Format("select * from sysFlatFileDtl where CodeID = '{0}' order by SeqNo", codeID);
            SqlDataAdapter sda2 = new SqlDataAdapter(cmd);
            DataTable dt2 = new DataTable("Data Detail");
            sda2.Fill(dt2);

            for (int i = 0; i < countDtl; i++)
            {
                if (i > 0)
                {
                    for (int y = 1; y < lines.Length; y++)
                    {
                        DataRow row = dt.NewRow();
                        line = lines[i].PadRight(mxLenght, ' ');
                        for (int x = 0; x < dt2.Rows.Count; x++)
                        {
                            int pos = Convert.ToInt32(dt2.Rows[x][3].ToString()) - 1;
                            int len = Convert.ToInt32(dt2.Rows[x][4].ToString());
                            string s = line.Substring(pos, len);
                            string t = dt2.Rows[x][5].ToString();

                            if (y == 1)
                            {
                                dt.Columns.Add(t, typeof(string));
                            }
                            row[t] = s;
                        }
                        dt.Rows.Add(row);
                    }
                }
                else
                {
                    for (int y = 1; y < lines.Length; y++)
                    {
                        DataRow row = dt.NewRow();
                        line = lines[i].PadRight(mxLenght, ' ');
                        for (int x = 0; x < dt2.Rows.Count; x++)
                        {
                            int pos = Convert.ToInt32(dt2.Rows[x][3].ToString()) - 1;
                            int len = Convert.ToInt32(dt2.Rows[x][4].ToString());
                            string s = line.Substring(pos, len);
                            string t = dt2.Rows[x][5].ToString();

                            if (y == 1)
                            {
                                dt.Columns.Add(t, typeof(string));
                            }
                            row[t] = s;
                        }
                        dt.Rows.Add(row);
                    }
                }

                listDt.Add(dt);
            }

            return listDt;
        }

        private int GetCountDetail(string codeID, SqlCommand cmd)
        {
            cmd.CommandText = string.Format("select detailID from sysflatfiledtl where codeID = '{0}' group by detailID", codeID);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("Data Detail");
            sda.Fill(dt);
            return dt.Rows.Count;
        }

    }
}
