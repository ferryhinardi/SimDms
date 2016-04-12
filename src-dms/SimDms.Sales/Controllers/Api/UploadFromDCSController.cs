using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.DcsWs;
using System.Data;
using SimDms.Common.Models;
using SimDms.Sales.BLL;
using SimDms.Sales.Models;
using System.Data.SqlClient;
using System.Text;
using System.IO;

namespace SimDms.Sales.Controllers.Api
{
    public class UploadFromDCSController : BaseController
    {
        private UploadTypeDCS uploadType;
        private string msg = "";
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
        private static List<string> MessageDcs = new List<string>();

        public JsonResult Default()
        {
            var dateFrom = Helpers.StartOfMonth();
            var dateTo = Helpers.EndOfMonth();

            Dictionary<string, string> dicDataID = new Dictionary<string, string>();
            dicDataID.Add("SPORD", "SPORD");
            dicDataID.Add("SDORD", "SDORD");
            //dicDataID.Add("SPRIC", "SPRIC");
            dicDataID.Add("SSJAL", "SSJAL");
            dicDataID.Add("SHPOK", "SHPOK");
            dicDataID.Add("SACCS", "SACCS");
            dicDataID.Add("SFPO1", "SFPO1");
            dicDataID.Add("SFPO2", "SFPO2");
            dicDataID.Add("SFPLB", "SFPLB");
            dicDataID.Add("SUADE", "SUADE");
            dicDataID.Add("SFPDA", "SFPDA");

            var dataID = dicDataID.Select(p => new
            {
                text = p.Key,
                value = p.Value
            });

            return Json(new { DateFrom = dateFrom, DateTo = dateTo, dsDataID = dataID, IsOnline = DcsValidated() });
        }

        public JsonResult RetriveData(string DataID, DateTime DateFrom, DateTime DateTo, bool AllStatus)
        {
            try
            {
                DcsHelper dcsHelper = new DcsHelper(CurrentUser.UserId);
                var records = dcsHelper.RetrieveUploadDataV2(DataID, DateFrom, DateTo, DcsValidated(), AllStatus);
                dcsHelper = null;

                return Json(new { success = true, data = records });
            }
            catch (Exception ex)
            {
                return JsonException(ex.Message);
            }
        }

        public JsonResult CheckData(string DataID, string Contents)
        {
            try
            {
                DcsDataMap dcsDataMap = new DcsDataMap();
                List<DataTable> listDt = new List<DataTable>();
                IEnumerable<SysFlatFileHdr> recHdr;

                string check = Contents.Replace("\r\n", "\n");
                var lines = check.Split('\n');

                DataTable dtHeader;
                dtHeader = dcsDataMap.MappingDataHeader(DataID, lines);

                var countDtl = dcsDataMap.GetCountDetail(DataID);
                if (countDtl == 1)
                    dcsDataMap.MappingData(DataID, Contents, out recHdr, ref listDt);
                else
                    dcsDataMap.MappingData(DataID, Contents, countDtl, out recHdr, ref listDt);

                dcsDataMap = null;

                //return Json(new { success = true, header = recHdr, detail = listDt });
                return Json(new { success = true, header = dtHeader, detail = listDt });
            }
            catch (Exception ex)
            {
                return JsonException(ex.Message);
            }
        }

        #region *** Process upload ***
        public JsonResult UploadData(string Id, string contents)
        {
            bool result = false;
            DcsHelper dcsHelper = new DcsHelper(CurrentUser.UserId);
            var UploadItem = ctx.GnDcsUploadFiles.Find(decimal.Parse(Id));
            string check = contents.Replace("\r\n", "\n");
            var lines = check.Split('\n');

            try
            {
                #region SALES FLAT FILE
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.SPORD))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            msg = "Data sudah pernah di upload....!";
                            result = false;
                        }
                        uploadType = UploadTypeDCS.SPORD;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.SDORD))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            msg = "Data sudah pernah di upload....!";
                            result = false;
                        }
                        uploadType = UploadTypeDCS.SDORD;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.SPRIC))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            msg = "Data sudah pernah di upload....!";
                            result = false;
                        }
                        uploadType = UploadTypeDCS.SPRIC;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.SSJAL))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            msg = "Data sudah pernah di upload....!";
                            result = false;
                        }
                        uploadType = UploadTypeDCS.SSJAL;
                    }
                }

                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.SHPOK))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            msg = "Data sudah pernah di upload....!";
                            result = false;
                        }
                        uploadType = UploadTypeDCS.SHPOK;
                    }
                }

                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.SACCS))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            msg = "Data sudah pernah di upload....!";
                            result = false;
                        }
                        uploadType = UploadTypeDCS.SACCS;
                    }
                }

                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.SFPO1))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            msg = "Data sudah pernah di upload....!";
                            result = false;
                        }
                        uploadType = UploadTypeDCS.SFPO1;
                    }
                }

                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.SFPO2))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            msg = "Data sudah pernah di upload....!";
                            result = false;
                        }
                        uploadType = UploadTypeDCS.SFPO2;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.SFPLB))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            msg = "Data sudah pernah di upload....!";
                            result = false;
                        }
                        uploadType = UploadTypeDCS.SFPLB;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.SUADE))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            msg = "Data sudah pernah di upload....!";
                            result = false;
                        }
                        uploadType = UploadTypeDCS.SUADE;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.SFPDA))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            msg = "Data sudah pernah di upload....!";
                            result = false;
                        }
                        uploadType = UploadTypeDCS.SFPDA;
                    }
                }


                #endregion

                result = ProccessUpload(uploadType, lines, Id);
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }


            return Json(new { success = result, message = msg });
        }

        private bool ProccessUpload(UploadTypeDCS uploadType, string[] lines, string ID)
        {
            DcsHelper dcsHelper = new DcsHelper(CurrentUser.UserId);
            bool success = false;
            bool result = false;

            if (uploadType != UploadTypeDCS.PPRCD && uploadType != UploadTypeDCS.WFRAT)
            {
                try
                {
                    if (lines != null && dcsHelper.Validate(lines, uploadType))
                    {
                        success = ProcessUploadDataTuningCtx(lines, uploadType);
                    }
                    if (success)
                    {
                        int hasilUpdateStat = dcsHelper.UpdateUploadDataStatusCtx(long.Parse(ID.ToString()), success);
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
                        result = false;
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    result = false;
                }
            }
            return result;
        }

        [MTAThread]
        private bool ProcessUploadDataTuningCtx(string[] lines, UploadTypeDCS uploadType)
        {
            switch (uploadType)
            {
                // REGION : SALES

                case UploadTypeDCS.SPORD:
                    return UploadDataSPORDLocal(lines);
                case UploadTypeDCS.SDORD:
                    return UploadDataSDORDLocal(lines);
                //case UploadTypeDCS.SPRIC:
                //    return UploadDataSPRICLocal(lines);
                case UploadTypeDCS.SSJAL:
                    return UploadDataSSJALLocal(lines);
                case UploadTypeDCS.SHPOK:
                    return UploadDataSHPOKLocal(lines);
                case UploadTypeDCS.SACCS:
                    return UploadDataSACCSLocal(lines);
                case UploadTypeDCS.SFPO1:
                    return UploadDataSFPOLLocal(lines);
                case UploadTypeDCS.SFPO2:
                    return UploadDataSFPOLLocal(lines);
                case UploadTypeDCS.SFPLB:
                    return UploadDataSFPLBLocal(lines);
                case UploadTypeDCS.SUADE:
                    return UploadDataSUADELocal(lines);
                case UploadTypeDCS.SFPDA:
                    return UploadDataSFPDALocal(lines);

                default:
                    return false;
            }
        }

        private bool UploadDataSPORDLocal(string[] lines)
        {
            var dcsHelper = new DcsHelper(CurrentUser.UserId);
            var maxLine = lines.Length;
            var maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            var linesLength = 260;
            MessageDcs.Clear();
            var query = tranStart;

            if (lines.Length <= 0)
            {
                msg = "Tidak ada data pada flat file";
                return false;
            }

            if (lines[0].Length != linesLength)
            {
                msg = "Jumlah karakter pada Header tidak sesuai";
                return false;
            }

            if (lines[0].Substring(0, 1) != "H")
            {
                msg = "Header tidak ditemukan pada flat file";
                return false;
            }

            var num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;
            var oSPORDHdrFile = new UploadBLL.SPORDHdrFile(lines[0]);
            if (oSPORDHdrFile == null || oSPORDHdrFile.DataID != "SPORD")
            {
                msg = "File ini bukan dokumen SPORD yang valid";
                return false;
            }

            var oOmUtlSPORDHdr = ctx.OmUtlSPORDHdrs.Find(user.CompanyCode, user.BranchCode, oSPORDHdrFile.DealerCode,
                user.CompanyCode, oSPORDHdrFile.BatchNo);

            if (oOmUtlSPORDHdr != null)
            {
                msg = "Header sudah ada di database";
                return false;
            }

            query += string.Format(@"INSERT INTO OmUtlSPORDHdr (CompanyCode, BranchCode, DealerCode, RcvDealerCode, BatchNo, Status, 
                                    CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                    user.CompanyCode, user.BranchCode, oSPORDHdrFile.DealerCode, user.CompanyCode,
                                    oSPORDHdrFile.BatchNo, "0", user.UserId, DateTime.Now, user.UserId, DateTime.Now);
            query += Environment.NewLine;

            var skpNo = ""; var salesModelCode = ""; var salesModelYear = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (lines[i].Length != linesLength)
                {
                    msg = "Jumlah karakter pada baris " + i.ToString() + "tidak sesuai";
                    return false;
                }

                if (lines[i].Substring(0, 1) == "1")
                {
                    var oSPORDDtl1File = new UploadBLL.SPORDDtl1File(lines[i]);
                    if (oSPORDDtl1File != null && oSPORDDtl1File.SKPNo != "")
                    {
                        var oOmUtlSPORDDtl1 = ctx.OmUtlSPORDDtl1s.Find(user.CompanyCode, user.BranchCode
                                                    , oSPORDHdrFile.BatchNo, oSPORDDtl1File.SKPNo);

                        if (oOmUtlSPORDDtl1 != null)
                        {
                            msg = "Detail 1 sudah ada di database";
                            return false;
                        }

                        var x = ctx.OmUtlSPORDDtl1s.Find(user.CompanyCode, user.BranchCode, oSPORDHdrFile.BatchNo, oSPORDDtl1File.SKPNo);
                        if (x != null)
                        {
                            msg = "No SKP: " + oSPORDDtl1File.SKPNo + " sudah diupload";
                            return false;
                        }

                        query += string.Format(@"INSERT INTO OmUtlSPORDDtl1 (CompanyCode, BranchCode, BatchNo, SKPNo, SKPDate, CreatedBy, 
                                                CreatedDate, LastUpdateBy, LastUpdateDate, Status)
                                                VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ",
                                                user.CompanyCode, user.BranchCode, oSPORDHdrFile.BatchNo, oSPORDDtl1File.SKPNo,
                                                oSPORDDtl1File.SKPDate, user.UserId, DateTime.Now, user.UserId, DateTime.Now, "0");
                        query += Environment.NewLine;
                        skpNo = oSPORDDtl1File.SKPNo;
                    }
                }
                else if (lines[i].Substring(0, 1) == "2")
                {
                    var oSPORDDtl2File = new UploadBLL.SPORDDtl2File(lines[i]);
                    if (oSPORDDtl2File != null && oSPORDDtl2File.SalesModelCode != "")
                    {
                        var oOmUtlSPORDDtl2 = ctx.OmUtlSPORDDtl2s.Find(user.CompanyCode, user.BranchCode
                                                    , oSPORDHdrFile.BatchNo, skpNo, oSPORDDtl2File.SalesModelCode
                                                    , oSPORDDtl2File.SalesModelYear);

                        if (oOmUtlSPORDDtl2 != null)
                        {
                            msg = "Detail 2 sudah ada di database";
                            return false;
                        }

                        query += string.Format(@"INSERT INTO OmUtlSPORDDtl2 (CompanyCode, BranchCode, BatchNo, SKPNo, SalesModelCode, SalesModelYear, 
                                                BeforeDiscountDPP, BeforeDiscountPPN, BeforeDiscountPPNBM, BeforeDiscountTotal, DiscountExcludePPN, 
                                                DiscountIncludePPN, AfterDiscountDPP, AfterDiscountPPN, AfterDiscountPPNBM, AfterDiscountTotal, PPNBMPaid, 
                                                OthersDPP, OthersPPN, Quantity, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate) 
                                                VALUES ('{0}', '{1}','{2}','{3}','{4}', '{5}', {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, 
                                                        {16}, {17}, {18}, {19}, '{20}', '{21}', '{22}', '{23}') ",
                                                user.CompanyCode, user.BranchCode, oSPORDHdrFile.BatchNo, skpNo, oSPORDDtl2File.SalesModelCode,
                                                oSPORDDtl2File.SalesModelYear, oSPORDDtl2File.BeforeDiscDPP, oSPORDDtl2File.BeforeDiscPPN,
                                                oSPORDDtl2File.BeforeDiscPPNBM, oSPORDDtl2File.BeforeDiscTotal, oSPORDDtl2File.DiscountExcludePPN,
                                                oSPORDDtl2File.DiscountIncludePPN, oSPORDDtl2File.AfterDiscDPP, oSPORDDtl2File.AfterDiscPPN,
                                                oSPORDDtl2File.AfterDiscPPNBM, oSPORDDtl2File.AfterDiscTotal, oSPORDDtl2File.PPNBMPaid, oSPORDDtl2File.OthersDPP,
                                                oSPORDDtl2File.OthersPPN, oSPORDDtl2File.Quantity, user.UserId, DateTime.Now, user.UserId, DateTime.Now);
                        query += Environment.NewLine;
                        salesModelCode = oSPORDDtl2File.SalesModelCode;
                        salesModelYear = oSPORDDtl2File.SalesModelYear;
                    }
                }
                else if (lines[i].Substring(0, 1) == "3")
                {
                    var oSPORDDtl3File = new UploadBLL.SPORDDtl3File(lines[i]);
                    if (oSPORDDtl3File != null && oSPORDDtl3File.ColourCode != "")
                    {
                        var oOmUtlSPORDDtl3 = ctx.OmUtlSPORDDtl3s.Find(user.CompanyCode, user.BranchCode
                                                    , oSPORDHdrFile.BatchNo, skpNo, salesModelCode
                                                    , salesModelYear, oSPORDDtl3File.ColourCode);
                        if (oOmUtlSPORDDtl3 != null)
                        {
                            msg = "Detail 3 sudah ada di database";
                            return false;
                        }

                        query += string.Format(@"INSERT INTO OmUtlSPORDDtl3 (CompanyCode, BranchCode, BatchNo, SKPNo, SalesModelCode, 
                                                    SalesModelYear, ColourCode, Quantity, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}','{9}','{10}','{11}')",
                                                    user.CompanyCode, user.BranchCode, oSPORDHdrFile.BatchNo, skpNo, salesModelCode, salesModelYear,
                                                    oSPORDDtl3File.ColourCode, oSPORDDtl3File.Quantity, user.UserId, DateTime.Now, user.UserId, DateTime.Now);
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
            return ctx.Database.ExecuteSqlCommand(query) > 0;
        }

        private bool UploadDataSDORDLocal(string[] lines)
        {
            var dcsHelper = new DcsHelper(CurrentUser.UserId);
            var maxLine = lines.Length;
            var maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            var linesLength = 96;
            MessageDcs.Clear();
            var query = tranStart;

            if (lines.Length <= 0)
            {
                msg = "Tidak ada data pada flat file";
                return false;
            }

            if (lines[0].Length != linesLength)
            {
                msg = "Jumlah karakter pada Header tidak sesuai";
                return false;
            }


            if (lines[0].Substring(0, 1) != "H")
            {
                msg = "Header tidak ditemukan pada flat file";
                return false;
            }

            var oSDORDHdrFile = new UploadBLL.SDORDHdrFile(lines[0]);
            if (oSDORDHdrFile == null || oSDORDHdrFile.DataID != "SDORD")
            {
                msg = "File ini bukan dokumen SPORD yang valid";
                return false;
            }

            var num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

            var oOmUtlSDORDHdr = ctx.OmUtlSDORDHdrs.Find(user.CompanyCode, user.BranchCode
            , oSDORDHdrFile.DealerCode, user.CompanyCode, oSDORDHdrFile.BatchNo);

            if (oOmUtlSDORDHdr != null)
            {
                msg = "Header sudah ada di database";
                return false;
            }

            query += string.Format(@"INSERT INTO OmUtlSDORDHdr (CompanyCode, BranchCode, DealerCode, RcvDealerCode, 
                                    BatchNo, Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                    user.CompanyCode, user.BranchCode, oSDORDHdrFile.DealerCode, user.CompanyCode,
                                    oSDORDHdrFile.BatchNo, "0", user.UserId, DateTime.Now, user.UserId, DateTime.Now);
            query += Environment.NewLine;

            var doNo = ""; var salesModelCode = ""; var salesModelYear = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length != linesLength)
                {
                    msg = "Jumlah karakter pada baris " + i.ToString() + "tidak sesuai";
                    return false;
                }
                if (lines[i].Substring(0, 1) == "1")
                {
                    var oSDORDDtl1File = new UploadBLL.SDORDDtl1File(lines[i]);
                    if (oSDORDDtl1File != null && oSDORDDtl1File.DONo != "")
                    {
                        var oOmUtlSDORDDtl1 = ctx.OmUtlSDORDDtl1s.Find(user.CompanyCode, user.BranchCode
                                                    , oSDORDHdrFile.BatchNo, oSDORDDtl1File.DONo);
                        if (oOmUtlSDORDDtl1 != null)
                        {
                            msg = "Detail 1 sudah ada di database";
                            return false;
                        }

                        var x = ctx.OmUtlSDORDDtl1s.Find(user.CompanyCode, user.BranchCode,
                                oSDORDHdrFile.BatchNo, oSDORDDtl1File.DONo);
                        if (x != null)
                        {
                            msg = "No SKP: " + oSDORDDtl1File.DONo + " sudah diupload";
                            return false;
                        }

                        query += string.Format(@"INSERT INTO OmUtlSDORDDtl1 (CompanyCode, BranchCode, BatchNo, DONo, DODate, 
                                                    SKPNo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, Status)
                                                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}') ",
                                                    user.CompanyCode, user.BranchCode, oSDORDHdrFile.BatchNo, oSDORDDtl1File.DONo,
                                                    oSDORDDtl1File.DODate, oSDORDDtl1File.SKPNo, user.UserId, DateTime.Now, user.UserId, DateTime.Now, "0");
                        query += Environment.NewLine;
                        doNo = oSDORDDtl1File.DONo;
                    }
                }
                if (lines[i].Substring(0, 1) == "2")
                {
                    var oSDORDDtl2File = new UploadBLL.SDORDDtl2File(lines[i]);
                    if (oSDORDDtl2File != null && oSDORDDtl2File.SalesModelCode != "")
                    {
                        var oOmUtlSDORDDtl2 = ctx.OmUtlSDORDDtl2s.Find(user.CompanyCode, user.BranchCode
                                                    , oSDORDHdrFile.BatchNo, doNo, oSDORDDtl2File.SalesModelCode
                                                    , oSDORDDtl2File.SalesModelYear);
                        if (oOmUtlSDORDDtl2 != null)
                        {
                            msg = "Detail 2 sudah ada di database";
                            return false;
                        }
                        query += string.Format(@"INSERT INTO OmUtlSDORDDtl2 (CompanyCode, BranchCode, BatchNo, DONo, SalesModelCode, 
                                                    SalesModelYear, Quantity, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}','{8}','{9}','{10}') ",
                                                    user.CompanyCode, user.BranchCode, oSDORDHdrFile.BatchNo, doNo, oSDORDDtl2File.SalesModelCode,
                                                    oSDORDDtl2File.SalesModelYear, oSDORDDtl2File.Quantity, user.UserId, DateTime.Now, user.UserId, DateTime.Now);
                        query += Environment.NewLine;
                        salesModelCode = oSDORDDtl2File.SalesModelCode;
                        salesModelYear = oSDORDDtl2File.SalesModelYear;
                    }
                }
                if (lines[i].Substring(0, 1) == "3")
                {
                    var oSDORDDtl3File = new UploadBLL.SDORDDtl3File(lines[i]);
                    if (oSDORDDtl3File != null && oSDORDDtl3File.ColourCode != "")
                    {
                        var oOmUtlSDORDDtl3 = ctx.OmUtlSDORDDtl3s.Find(user.CompanyCode, user.BranchCode
                                                    , oSDORDHdrFile.BatchNo, doNo, salesModelCode
                                                    , salesModelYear, oSDORDDtl3File.ColourCode, oSDORDDtl3File.ChassisCode, oSDORDDtl3File.ChassisNo);
                        if (oOmUtlSDORDDtl3 != null)
                        {
                            msg = "Detail 3 sudah ada di database";
                            return false;
                        }
                        query += string.Format(@"INSERT INTO OmUtlSDORDDtl3 (CompanyCode, BranchCode, BatchNo, DONo, SalesModelCode, SalesModelYear, ColourCode, 
                                                ChassisCode, ChassisNo, EngineCode, EngineNo, ServiceBookNo, KeyNo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}') ",
                                                user.CompanyCode, user.BranchCode, oSDORDHdrFile.BatchNo, doNo, salesModelCode, salesModelYear, oSDORDDtl3File.ColourCode,
                                                oSDORDDtl3File.ChassisCode, oSDORDDtl3File.ChassisNo, oSDORDDtl3File.EngineCode, oSDORDDtl3File.EngineNo,
                                                oSDORDDtl3File.ServiceBookNo, oSDORDDtl3File.KeyNo, user.UserId, DateTime.Now, user.UserId, DateTime.Now);
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
            return ctx.Database.ExecuteSqlCommand(query) > 0;
        }

        private bool UploadDataSSJALLocal(string[] lines)
        {
            DcsHelper dcsHelper = new DcsHelper(CurrentUser.UserId);
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            var cProfile = ctx.CoProfiles.Find(user.CompanyCode, user.BranchCode);
            int linesLength = 96;
            MessageDcs.Clear();
            bool revisi = false;
            DataTable dtHeader;
            string bpuNo = "", poNo = "", chassisCode = "", chassisNo = "";
            int qtyBPU = 0;
            var query = tranStart;

            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return false;
            }

            if (lines[0].Length != linesLength)
            {
                msg = "Jumlah karakter pada Header tidak sesuai";
                return false;
            }

            if (lines[0].Substring(0, 1) == "H")
            {
                //string query = string.Empty;
                UploadBLL.SSJALHdrFile oSSJALHdrFile = new UploadBLL.SSJALHdrFile(lines[0]);
                if (oSSJALHdrFile != null && oSSJALHdrFile.DataID == "SSJAL")
                {
                    int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

                    OmUtlSSJALHdr oOmUtlSSJALHdr = ctx.OmUtlSSJALHdrs.Find(user.CompanyCode, user.BranchCode
                                                , oSSJALHdrFile.DealerCode, user.CompanyCode, oSSJALHdrFile.BatchNo);

                    if (oOmUtlSSJALHdr != null)
                    {
                        msg = "Proses upload file gagal";
                        result = false; return result;
                    }
                    else
                    {
                        //untuk test di remark dulu
                        query += string.Format(@"INSERT INTO OmUtlSSJALHdr (CompanyCode, BranchCode, DealerCode, RcvDealerCode, BatchNo, 
                                                Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                                user.CompanyCode, user.BranchCode, oSSJALHdrFile.DealerCode, user.CompanyCode,
                                                oSSJALHdrFile.BatchNo, "0", user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                        query += Environment.NewLine;
                        //if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                        //{
                        //    result = false;
                        //    msg = "Process upload gagal";
                        //    return result;
                        //}
                        //else
                        //{
                        //    result = true;
                        //}
                        //////untuk test flow result=true;
                        ////result = true;
                        //query = string.Empty;

                        //if (result == true)
                        //{
                        string sjNo = ""; string salesModelCode = ""; int salesModelYear = 0;
                        for (int i = 1; i < lines.Length; i++)
                        {
                            //query = string.Empty;
                            if (lines[i].Length == linesLength)
                            {
                                if (lines[i].Substring(0, 1) == "1")
                                {
                                    UploadBLL.SSJALDtl1File oSSJALDtl1File = new UploadBLL.SSJALDtl1File(lines[i]);
                                    if (oSSJALDtl1File != null && oSSJALDtl1File.SJNo != "")
                                    {
                                        OmUtlSSJALDtl1 oOmUtlSSJALDtl1 = ctx.OmUtlSSJALDtl1s.Find(user.CompanyCode, user.BranchCode
                                                                    , oSSJALHdrFile.BatchNo, oSSJALDtl1File.SJNo);

                                        if (oOmUtlSSJALDtl1 != null)
                                        {
                                            msg = "Proses upload file gagal";
                                            result = false; return result;
                                        }
                                        else
                                        {
                                            var x = ctx.OmUtlSSJALDtl1s.Find(user.CompanyCode, user.BranchCode, oSSJALHdrFile.BatchNo, oSSJALDtl1File.SJNo);
                                            if (x != null)
                                            {
                                                msg = "flat file data No SJ: " + oSSJALDtl1File.SJNo + " sudah diupload";
                                                result = false; return result;
                                            }
                                            else
                                            {
                                                //untuk test di remark dulu
                                                query += string.Format(@"INSERT INTO OmUtlSSJALDtl1 (CompanyCode, BranchCode, BatchNo, SJNo, SJDate, SKPNo, DONo, 
                                                                            DODate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, IsBlokir, Status) 
                                                                            VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')",
                                                                        user.CompanyCode, user.BranchCode, oSSJALHdrFile.BatchNo, oSSJALDtl1File.SJNo, oSSJALDtl1File.SJDate,
                                                                        oSSJALDtl1File.SKPNo, oSSJALDtl1File.DONo, oSSJALDtl1File.DODate, user.UserId, DateTime.Now, user.UserId,
                                                                        DateTime.Now, oSSJALDtl1File.IsBlokir, "0");

                                                sjNo = oSSJALDtl1File.SJNo;
                                                query += Environment.NewLine;

                                                if (user.CoProfile.ProductType == "4W")
                                                {
                                                    DataRow row = GetBPUNobyReffFJ(user.CompanyCode, user.BranchCode, oSSJALDtl1File.SJNo);

                                                    if (row != null)
                                                    {
                                                        bpuNo = row["BPUNo"].ToString();
                                                        poNo = row["PONo"].ToString();
                                                        sjNo = oSSJALDtl1File.SJNo;
                                                        chassisCode = row["ChassisCode"].ToString();
                                                        chassisNo = row["ChassisNo"].ToString();
                                                        revisi = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //if (!(string.IsNullOrEmpty(query)))
                                    //{
                                    //    result = ctx.Database.ExecuteSqlCommand(query) > 0;
                                    //    query = "";
                                    //}
                                }
                                if (lines[i].Substring(0, 1) == "2")
                                {
                                    UploadBLL.SSJALDtl2File oSSJALDtl2File = new UploadBLL.SSJALDtl2File(lines[i]);
                                    if (oSSJALDtl2File != null && oSSJALDtl2File.SalesModelCode != "")
                                    {
                                        OmUtlSSJALDtl2 oOmUtlSSJALDtl2 = ctx.OmUtlSSJALDtl2s.Find(user.CompanyCode, user.BranchCode
                                                                        , oSSJALHdrFile.BatchNo, sjNo, oSSJALDtl2File.SalesModelCode
                                                                        , oSSJALDtl2File.SalesModelYear);

                                        if (oOmUtlSSJALDtl2 != null)
                                        {
                                            msg = "Proses upload file gagal";
                                            result = false; return result;
                                        }
                                        else
                                        {
                                            //untuk test di remark dulu
                                            query += string.Format(@"INSERT INTO OmUtlSSJALDtl2 (CompanyCode, BranchCode, BatchNo, SJNo, 
                                                                        SalesModelCode, SalesModelYear, Quantity, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                                        VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}','{8}','{9}','{10}') ",
                                                                        user.CompanyCode, user.BranchCode, oSSJALHdrFile.BatchNo, sjNo, oSSJALDtl2File.SalesModelCode,
                                                                        oSSJALDtl2File.SalesModelYear, oSSJALDtl2File.Quantity, user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                                            salesModelCode = oSSJALDtl2File.SalesModelCode;
                                            salesModelYear = oSSJALDtl2File.SalesModelYear;

                                            query += Environment.NewLine;
                                        }
                                    }
                                    //if (!(string.IsNullOrEmpty(query)))
                                    //{
                                    //    result = ctx.Database.ExecuteSqlCommand(query) > 0;
                                    //    query = "";
                                    //}
                                }
                                if (lines[i].Substring(0, 1) == "3")
                                {
                                    UploadBLL.SSJALDtl3File oSSJALDtl3File = new UploadBLL.SSJALDtl3File(lines[i]);
                                    if (oSSJALDtl3File != null && oSSJALDtl3File.ColourCode != "")
                                    {
                                        OmUtlSSJALDtl3 oOmUtlSSJALDtl3 = ctx.OmUtlSSJALDtl3s.Find(user.CompanyCode, user.BranchCode
                                                                    , oSSJALHdrFile.BatchNo, sjNo, salesModelCode
                                                                    , salesModelYear, oSSJALDtl3File.ColourCode, oSSJALDtl3File.ChassisCode, oSSJALDtl3File.ChassisNo);

                                        if (oOmUtlSSJALDtl3 != null)
                                        {
                                            if (!revisi)
                                            {
                                                msg = "Proses upload file gagal";
                                                result = false; return result;
                                            }
                                        }
                                        else
                                        {
                                            if (revisi)
                                            {
                                                if (bpuNo != "")
                                                {
                                                    //untuk test di remark dulu
                                                    query += string.Format(@"INSERT INTO OmUtlSSJALDtl3 (CompanyCode, BranchCode, BatchNo, SJNo, SalesModelCode, SalesModelYear, ColourCode, 
                                                                                ChassisCode, ChassisNo, EngineCode, EngineNo, ServiceBookNo, KeyNo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                                                VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}') ",
                                                                            user.CompanyCode, user.BranchCode, oSSJALHdrFile.BatchNo, sjNo, salesModelCode, salesModelYear, oSSJALDtl3File.ColourCode,
                                                                            oSSJALDtl3File.ChassisCode, oSSJALDtl3File.ChassisNo, oSSJALDtl3File.EngineCode, oSSJALDtl3File.EngineNo,
                                                                            oSSJALDtl3File.ServiceBookNo, oSSJALDtl3File.KeyNo, user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                                                    query += string.Format(@" UPDATE [omTrPurchaseBPUDetail] 
                                                                                    SET [ChassisNo] = '{5}'
                                                                                    ,[EngineNo] = '{6}'
                                                                                    ,[ServiceBookNo] = '{7}'
                                                                                    ,[KeyNo] = '{8}'
                                                                                    WHERE [CompanyCode] = '{0}'
                                                                                    and [BranchCode] = '{1}' and [BPUNo] = '{2}'and [SalesModelCode] = '{3}' and [SalesModelYear] = '{4}' 

                                                                                    UPDATE [OmMstVehicle]
                                                                                    SET [ChassisNo] = '{5}'
                                                                                    ,[EngineNo] = '{6}'
                                                                                    ,[ServiceBookNo] = '{7}'
                                                                                    ,[KeyNo] = '{8}'
                                                                                    WHERE [CompanyCode] = '{0}'
                                                                                    and [ChassisCode] = '{10}' and [ChassisNo] = {11}
                                        
                                                                                    UPDATE [OmTrpurchaseHppSubDetail]
                                                                                    SET [ChassisNo] = '{5}'
                                                                                    ,[EngineNo] = '{6}'                                              
                                                                                    WHERE [CompanyCode] = '{0}'
                                                                                    and [BranchCode] = '{1}' and [ChassisCode] = '{10}' and [ChassisNo] = {11} ",
                                                                                user.CompanyCode, user.BranchCode, bpuNo, salesModelCode, salesModelYear, oSSJALDtl3File.ChassisNo,
                                                                                oSSJALDtl3File.EngineNo, oSSJALDtl3File.ServiceBookNo, oSSJALDtl3File.KeyNo, sjNo, chassisCode, chassisNo);

                                                    query += Environment.NewLine;
                                                }
                                            }
                                            else
                                            {
                                                query += string.Format(@"INSERT INTO OmUtlSSJALDtl3 (CompanyCode, BranchCode, BatchNo, SJNo, SalesModelCode, SalesModelYear, ColourCode, ChassisCode, 
                                                                            ChassisNo, EngineCode, EngineNo, ServiceBookNo, KeyNo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                                            VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}') ",
                                                                            user.CompanyCode, user.BranchCode, oSSJALHdrFile.BatchNo, sjNo, salesModelCode, salesModelYear, oSSJALDtl3File.ColourCode,
                                                                            oSSJALDtl3File.ChassisCode, oSSJALDtl3File.ChassisNo, oSSJALDtl3File.EngineCode, oSSJALDtl3File.EngineNo,
                                                                            oSSJALDtl3File.ServiceBookNo, oSSJALDtl3File.KeyNo, user.UserId, DateTime.Now, user.UserId, DateTime.Now);
                                                query += Environment.NewLine;
                                            }
                                        }
                                    }
                                    //if (!(string.IsNullOrEmpty(query)))
                                    //{
                                    //    result = ctx.Database.ExecuteSqlCommand(query) > 0;
                                    //    query = "";
                                    //}
                                    bpuNo = "";
                                    revisi = false;
                                }
                            }
                            else
                            {
                                msg = "Proses Upload Gagal";
                            }
                        }
                        //}
                        //else
                        //{
                        //    msg = "Proses Upload Gagal";
                        //}
                    }

                    query += tranEnd;
                    if (string.IsNullOrEmpty(query))
                    {
                        msg = "Tidak ada data yang akan diupload";
                        return false;
                    }
                    result = ctx.Database.ExecuteSqlCommand(query) > 0;
                }
                else
                {
                    msg = "flat file tidak valid";
                    result = false;
                    return result;
                }
            }

            return result;
        }

        private bool UploadDataSHPOKLocal(string[] lines)
        {
            DcsHelper dcsHelper = new DcsHelper(CurrentUser.UserId);
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            int linesLength = 188;
            MessageDcs.Clear();
            var query = tranStart;

            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return false;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    //string query = string.Empty;
                    UploadBLL.SHPOKHdrFile oSHPOKHdrFile = new UploadBLL.SHPOKHdrFile(lines[0]);
                    if (oSHPOKHdrFile != null && oSHPOKHdrFile.DataID == "SHPOK")
                    {
                        int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

                        OmUtlSHPOKHdr oOmUtlSHPOKHdr = ctx.OmUtlSHPOKHdrs.Find(user.CompanyCode, user.BranchCode
                                                    , oSHPOKHdrFile.DealerCode, user.CompanyCode, oSHPOKHdrFile.BatchNo);

                        if (oOmUtlSHPOKHdr != null)
                        {
                            msg = "Proses upload file gagal";
                            return false;
                        }
                        else
                        {
                            //untuk test di remark dulu
                            query += string.Format(@"INSERT INTO OmUtlSHPOKHdr (CompanyCode, BranchCode, DealerCode, RcvDealerCode, 
                                                    BatchNo, Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                                    user.CompanyCode, user.BranchCode, oSHPOKHdrFile.DealerCode, user.CompanyCode,
                                                    oSHPOKHdrFile.BatchNo, "0", user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                            //if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                            //{
                            //    result = false;
                            //    msg = "Process upload gagal";
                            //    return result;
                            //}
                            //else
                            //{
                            //    result = true;
                            //}
                            ////untuk test result=true;
                            //result = true;

                            query += Environment.NewLine;

                            //query = string.Empty;

                            //if (result)
                            //{
                            string invoiceNo = ""; string documentNo = ""; string salesModelCode = ""; int salesModelYear = 0;
                            UploadBLL.SHPOKDtl1File oSHPOKDtl1File = new UploadBLL.SHPOKDtl1File();
                            UploadBLL.SHPOKDtl2File oSHPOKDtl2File = new UploadBLL.SHPOKDtl2File();
                            UploadBLL.SHPOKDtl3File oSHPOKDtl3File = new UploadBLL.SHPOKDtl3File();
                            for (int i = 1; i < lines.Length; i++)
                            {
                                //query = string.Empty;
                                if (lines[i].Length == linesLength)
                                {
                                    if (lines[i].Substring(0, 1) == "1")
                                    {
                                        oSHPOKDtl1File = new UploadBLL.SHPOKDtl1File(lines[i]);
                                        if (oSHPOKDtl1File != null && oSHPOKDtl1File.InvoiceNo != "")
                                        {
                                            OmUtlSHPOKDtl1 oOmUtlSHPOKDtl1 = ctx.OmUtlSHPOKDtl1s.Find(user.CompanyCode, user.BranchCode
                                                                            , oSHPOKHdrFile.BatchNo, oSHPOKDtl1File.InvoiceNo);

                                            if (oOmUtlSHPOKDtl1 != null)
                                            {
                                                msg = "Proses upload file gagal";
                                                return false;
                                            }
                                            else
                                            {
                                                //untuk test di remark dulu
                                                query += string.Format(@"INSERT INTO OmUtlSHPOKDtl1 (CompanyCode, BranchCode, BatchNo, InvoiceNo, InvoiceDate, SKPNo, FakturPajakNo, 
                                                                        FakturPajakDate, DueDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, Status, Remark)
                                                                        VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}') ",
                                                                        user.CompanyCode, user.BranchCode, oSHPOKHdrFile.BatchNo, oSHPOKDtl1File.InvoiceNo, oSHPOKDtl1File.InvoiceDate,
                                                                        oSHPOKDtl1File.SKPNo, oSHPOKDtl1File.FakturPajakNo, oSHPOKDtl1File.FakturPajakDate, oSHPOKDtl1File.DueDate,
                                                                        user.UserId, DateTime.Now, user.UserId, DateTime.Now, "0", oSHPOKDtl1File.Remark);
                                                invoiceNo = oSHPOKDtl1File.InvoiceNo;

                                                query += Environment.NewLine;
                                            }
                                        }
                                    }
                                    if (lines[i].Substring(0, 1) == "2")
                                    {
                                        oSHPOKDtl2File = new UploadBLL.SHPOKDtl2File(lines[i]);
                                        if (oSHPOKDtl2File != null && oSHPOKDtl2File.DocNo != "")
                                        {
                                            OmUtlSHPOKDtl2 oOmUtlSHPOKDtl2 = ctx.OmUtlSHPOKDtl2s.Find(user.CompanyCode, user.BranchCode
                                                                        , oSHPOKHdrFile.BatchNo, invoiceNo, oSHPOKDtl2File.DocNo);

                                            if (oOmUtlSHPOKDtl2 != null)
                                            {
                                                msg = "Proses upload file gagal";
                                                return false;
                                            }
                                            else
                                            {
                                                //untuk test di remark dulu
                                                query += string.Format(@"INSERT INTO OmUtlSHPOKDtl2 (CompanyCode, BranchCode, BatchNo, InvoiceNo, 
                                                                            DocumentNo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, DocumentType)
                                                                            VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ",
                                                                        user.CompanyCode, user.BranchCode, oSHPOKHdrFile.BatchNo, invoiceNo, oSHPOKDtl2File.DocNo,
                                                                        user.UserId, DateTime.Now, user.UserId, DateTime.Now, oSHPOKDtl2File.DocType);

                                                documentNo = oSHPOKDtl2File.DocNo;

                                                query += Environment.NewLine;
                                            }
                                        }
                                    }
                                    if (lines[i].Substring(0, 1) == "3")
                                    {
                                        oSHPOKDtl3File = new UploadBLL.SHPOKDtl3File(lines[i]);
                                        if (oSHPOKDtl3File != null && oSHPOKDtl3File.SalesModelCode != "")
                                        {
                                            OmUtlSHPOKDtl3 oOmUtlSHPOKDtl3 = ctx.OmUtlSHPOKDtl3s.Find(user.CompanyCode, user.BranchCode
                                                                        , oSHPOKHdrFile.BatchNo, invoiceNo, documentNo, oSHPOKDtl3File.SalesModelCode
                                                                        , Convert.ToDecimal(oSHPOKDtl3File.SalesModelYear.ToString()));

                                            if (oOmUtlSHPOKDtl3 != null)
                                            {
                                                msg = "Proses upload file gagal";
                                                return false;
                                            }
                                            else
                                            {
                                                //untuk test di remark dulu
                                                query += string.Format(@"INSERT INTO OmUtlSHPOKDtl3 (CompanyCode, BranchCode, BatchNo, InvoiceNo, DocumentNo, 
                                                                            SalesModelCode, SalesModelYear, BeforeDiscountDPP, DiscountExcludePPN, AfterDiscountDPP, 
                                                                            AfterDiscountPPN, AfterDiscountPPNBM, AfterDiscountTotal, PPNBMPaid, OthersDPP, OthersPPN, 
                                                                            Quantity, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                                            VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},'{17}','{18}','{19}','{20}') ",
                                                                        user.CompanyCode, user.BranchCode, oSHPOKHdrFile.BatchNo, invoiceNo, documentNo, oSHPOKDtl3File.SalesModelCode,
                                                                        oSHPOKDtl3File.SalesModelYear, oSHPOKDtl3File.BeforeDiscDPP, oSHPOKDtl3File.DiscountExcludePPN, oSHPOKDtl3File.AfterDiscDPP,
                                                                        oSHPOKDtl3File.AfterDiscPPN, oSHPOKDtl3File.AfterDiscPPNBM, oSHPOKDtl3File.AfterDiscTotal, oSHPOKDtl3File.PPNBMPaid,
                                                                        oSHPOKDtl3File.OthersDPP, oSHPOKDtl3File.OthersPPN, oSHPOKDtl3File.Quantity, user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                                                salesModelCode = oSHPOKDtl3File.SalesModelCode;
                                                salesModelYear = oSHPOKDtl3File.SalesModelYear;

                                                query += Environment.NewLine;
                                            }
                                        }
                                    }
                                    if (lines[i].Substring(0, 1) == "O")
                                    {
                                        UploadBLL.SHPOKDtlOFile oSHPOKDtlOFile = new UploadBLL.SHPOKDtlOFile(lines[i]);
                                        if (oSHPOKDtlOFile != null && oSHPOKDtlOFile.OthersCode != "")
                                        {
                                            OmUtlSHPOKDtlO oDtOmUtlSHPOKDtlO = ctx.OmUtlSHPOKDtlOs.Find(user.CompanyCode, user.BranchCode, oSHPOKHdrFile.BatchNo, oSHPOKDtl1File.InvoiceNo, oSHPOKDtl2File.DocNo, oSHPOKDtl3File.SalesModelCode, oSHPOKDtl3File.SalesModelYear, oSHPOKDtlOFile.OthersCode);
                                            if (oDtOmUtlSHPOKDtlO != null)
                                            {
                                                msg = "Proses upload file gagal";
                                                return false;
                                            }
                                            else
                                            {
                                                OmUtlSHPOKDtlO oOmUtlSHPOKDtlO = new OmUtlSHPOKDtlO();
                                                oOmUtlSHPOKDtlO.BatchNo = oSHPOKHdrFile.BatchNo;
                                                oOmUtlSHPOKDtlO.BranchCode = user.BranchCode;
                                                oOmUtlSHPOKDtlO.CompanyCode = user.CompanyCode;
                                                oOmUtlSHPOKDtlO.CreatedBy = user.UserId;
                                                oOmUtlSHPOKDtlO.CreatedDate = DateTime.Now;
                                                oOmUtlSHPOKDtlO.DocumentNo = oSHPOKDtl2File.DocNo;
                                                oOmUtlSHPOKDtlO.InvoiceNo = oSHPOKDtl1File.InvoiceNo;
                                                oOmUtlSHPOKDtlO.OthersCode = oSHPOKDtlOFile.OthersCode;
                                                oOmUtlSHPOKDtlO.OthersDPP = oSHPOKDtlOFile.OthersDPP;
                                                oOmUtlSHPOKDtlO.OthersPPN = oSHPOKDtlOFile.OthersPPN;
                                                oOmUtlSHPOKDtlO.SalesModelCode = oSHPOKDtl3File.SalesModelCode;
                                                oOmUtlSHPOKDtlO.SalesModelYear = oSHPOKDtl3File.SalesModelYear;

                                                //untuk test di remark dulu
                                                query += string.Format(@"INSERT INTO [omUtlSHPOKDtlO]([CompanyCode],[BranchCode],[BatchNo],[InvoiceNo],[DocumentNo],
                                                                            [SalesModelCode],[SalesModelYear],[OthersCode],[OthersDPP],[OthersPPN],[CreatedBy],[CreatedDate],[LastUpdateBy],[LastUpdateDate])
                                                                            VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')",
                                                                        oOmUtlSHPOKDtlO.CompanyCode, oOmUtlSHPOKDtlO.BranchCode, oOmUtlSHPOKDtlO.BatchNo, oOmUtlSHPOKDtlO.InvoiceNo,
                                                                        oOmUtlSHPOKDtlO.DocumentNo, oOmUtlSHPOKDtlO.SalesModelCode, oOmUtlSHPOKDtlO.SalesModelYear, oOmUtlSHPOKDtlO.OthersCode,
                                                                        oOmUtlSHPOKDtlO.OthersDPP, oOmUtlSHPOKDtlO.OthersPPN, user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                                                query += Environment.NewLine;
                                            }
                                        }
                                    }
                                    if (lines[i].Substring(0, 1) == "4")
                                    {
                                        UploadBLL.SHPOKDtl4File oSHPOKDtl4File = new UploadBLL.SHPOKDtl4File(lines[i]);
                                        if (oSHPOKDtl4File != null && oSHPOKDtl4File.ChassisCode != "")
                                        {
                                            OmUtlSHPOKDtl4 oOmUtlSHPOKDtl4 = ctx.OmUtlSHPOKDtl4s.Find(user.CompanyCode, user.BranchCode
                                                                        , oSHPOKHdrFile.BatchNo, invoiceNo, documentNo, salesModelCode
                                                                        , Convert.ToDecimal(salesModelYear.ToString()), oSHPOKDtl4File.ColourCode, oSHPOKDtl4File.ChassisCode
                                                                        , Convert.ToDecimal(oSHPOKDtl4File.ChassisNo.ToString()));

                                            if (oOmUtlSHPOKDtl4 != null)
                                            {
                                                msg = "Proses upload file gagal";
                                                return false;
                                            }
                                            else
                                            {
                                                //untuk test di remark dulu
                                                query += string.Format(@"INSERT INTO OmUtlSHPOKDtl4 (CompanyCode, BranchCode, BatchNo, InvoiceNo, DocumentNo, 
                                                                            SalesModelCode, SalesModelYear, ColourCode, ChassisCode, ChassisNo, EngineCode, EngineNo, 
                                                                            CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                                            VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')",
                                                                        user.CompanyCode, user.BranchCode, oSHPOKHdrFile.BatchNo, invoiceNo, documentNo, salesModelCode,
                                                                        salesModelYear, oSHPOKDtl4File.ColourCode, oSHPOKDtl4File.ChassisCode, oSHPOKDtl4File.ChassisNo,
                                                                        oSHPOKDtl4File.EngineCode, oSHPOKDtl4File.EngineNo, user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                                                query += Environment.NewLine;
                                            }
                                        }
                                    }
                                    //if (!(string.IsNullOrEmpty(query)))
                                    //{
                                    //    result = ctx.Database.ExecuteSqlCommand(query) > 0;
                                    //    query = "";
                                    //}
                                }
                                else
                                {
                                    msg = "Proses Upload Gagal";
                                    result = false;
                                    return result;
                                }
                            }
                            //}
                            //else
                            //{
                            //    msg = "Proses Upload Gagal";
                            //    result = false;
                            //    return result;
                            //}
                        }

                        query += tranEnd;
                        if (string.IsNullOrEmpty(query))
                        {
                            msg = "Tidak ada data yang akan diupload";
                            return false;
                        }
                        result = ctx.Database.ExecuteSqlCommand(query) > 0;
                    }
                    else
                    {
                        msg = "flat file tidak valid";
                        return result = false;
                    }
                }
            }
            else
            {
                msg = "flat file tidak valid";
                return result = false;
            }

            return result;
        }

        private bool UploadDataSACCSLocal(string[] lines)
        {
            DcsHelper dcsHelper = new DcsHelper(CurrentUser.UserId);
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            int linesLength = 82;
            MessageDcs.Clear();
            var query = tranStart;

            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return false;
            }

            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    //string query = string.Empty;

                    UploadBLL.SACCSHdrFile oSACCSHdrFile = new UploadBLL.SACCSHdrFile(lines[0]);
                    if (oSACCSHdrFile != null && oSACCSHdrFile.DataID == "SACCS")
                    {
                        int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;
                        OmUtlSACCSHdr oOmUtlSACCSHdr = ctx.OmUtlSACCSHdrs.Find(user.CompanyCode, user.BranchCode
                                                    , oSACCSHdrFile.DealerCode, user.CompanyCode, oSACCSHdrFile.BatchNo);

                        if (oOmUtlSACCSHdr != null)
                        {
                            msg = "Proses upload file gagal";
                            return false;
                        }
                        else
                        {
                            //untuk test di remark dulu
                            query += string.Format(@"INSERT INTO OmUtlSACCSHdr (CompanyCode, BranchCode, DealerCode, RcvDealerCode, 
                                                    BatchNo, Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                                    user.CompanyCode, user.BranchCode, oSACCSHdrFile.DealerCode, user.CompanyCode,
                                                    oSACCSHdrFile.BatchNo, "0", user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                            query += Environment.NewLine;

                            //if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                            //{
                            //    result = false;
                            //    msg = "Process upload gagal";
                            //    return result;
                            //}
                            //else
                            //{
                            //    result = true;
                            //}
                            ////untuk test result=true;
                            //result = true;
                            //query = string.Empty;

                            //if (result)
                            //{
                            string bppNo = "";
                            for (int i = 1; i < lines.Length; i++)
                            {
                                //query = string.Empty;
                                if (lines[i].Length == linesLength)
                                {
                                    if (lines[i].Substring(0, 1) == "1")
                                    {
                                        UploadBLL.SACCSDtl1File oSACCSDtl1File = new UploadBLL.SACCSDtl1File(lines[i]);
                                        if (oSACCSDtl1File != null && oSACCSDtl1File.BPPNo != "")
                                        {
                                            OmUtlSACCSDtl1 oOmUtlSACCSDtl1 = ctx.OmUtlSACCSDtl1s.Find(user.CompanyCode, user.BranchCode
                                                                        , oSACCSHdrFile.BatchNo, oSACCSDtl1File.BPPNo);
                                            if (oOmUtlSACCSDtl1 != null)
                                            {
                                                msg = "Proses upload file gagal";
                                                return false;
                                            }
                                            else
                                            {
                                                //untuk test di remark dulu
                                                query += string.Format(@"INSERT INTO OmUtlSACCSDtl1 (CompanyCode, BranchCode, BatchNo, BPPNo, 
                                                                            BPPDate, SJNo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, Status)
                                                                            VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}') ",
                                                                        user.CompanyCode, user.BranchCode, oSACCSHdrFile.BatchNo, oSACCSDtl1File.BPPNo,
                                                                        oSACCSDtl1File.BPPDate, oSACCSDtl1File.SJNo, user.UserId, DateTime.Now, user.UserId, DateTime.Now, "0");

                                                bppNo = oSACCSDtl1File.BPPNo;

                                                query += Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            msg = "Proses upload file gagal";
                                            return false;
                                        }
                                    }
                                    if (lines[i].Substring(0, 1) == "2")
                                    {
                                        UploadBLL.SACCSDtl2File oSACCSDtl2File = new UploadBLL.SACCSDtl2File(lines[i]);
                                        if (oSACCSDtl2File != null && oSACCSDtl2File.PerlengkapanCode != "")
                                        {
                                            OmUtlSACCSDtl2 oOmUtlSACCSDtl2 = ctx.OmUtlSACCSDtl2s.Find(user.CompanyCode, user.BranchCode
                                                                        , oSACCSHdrFile.BatchNo, bppNo, oSACCSDtl2File.PerlengkapanCode);

                                            if (oOmUtlSACCSDtl2 != null)
                                            {
                                                msg = "Proses upload file gagal";
                                                return false;
                                            }
                                            else
                                            {
                                                //untuk test di remark dulu
                                                query += string.Format(@"INSERT INTO OmUtlSACCSDtl2 (CompanyCode, BranchCode, BatchNo, BPPNo, 
                                                                            PerlengkapanCode, Quantity, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                                            VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}','{9}')",
                                                                        user.CompanyCode, user.BranchCode, oSACCSHdrFile.BatchNo, bppNo, oSACCSDtl2File.PerlengkapanCode,
                                                                        oSACCSDtl2File.Quantity, user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                                                query += Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            msg = "Proses upload file gagal";
                                            return false;
                                        }
                                    }
                                    //if (!(string.IsNullOrEmpty(query)))
                                    //{
                                    //    result = ctx.Database.ExecuteSqlCommand(query) > 0;
                                    //}
                                }
                                else
                                {
                                    msg = "Proses upload file gagal";
                                    return false;
                                }
                            }


                            //}
                            //else
                            //{
                            //    msg = "Proses upload file gagal";
                            //    return false;
                            //}
                        }

                        query += tranEnd;
                        if (string.IsNullOrEmpty(query))
                        {
                            msg = "Tidak ada data yang akan diupload";
                            return false;
                        }
                        return ctx.Database.ExecuteSqlCommand(query) > 0;
                    }
                    else
                    {
                        msg = "flat file tidak valid";
                        return false;
                    }
                }
            }
            else
            {
                msg = "flat file tidak valid";
                return false;
            }

            return result;
        }

        private bool UploadDataSFPOLLocal(string[] lines)
        {
            DcsHelper dcsHelper = new DcsHelper(CurrentUser.UserId);
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            int linesLength = 162;
            MessageDcs.Clear();

            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return false;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    string query = string.Empty;
                    UploadBLL.SFPOLHdrFile oSFPOLHdrFile = new UploadBLL.SFPOLHdrFile(lines[0]);
                    if ((oSFPOLHdrFile != null && oSFPOLHdrFile.DataID == "SFPO1") ||
                            (oSFPOLHdrFile != null && oSFPOLHdrFile.DataID == "SFPO2"))
                    {
                        OmUtlSFPOLHdr oOmUtlSFPOLHdr = ctx.OmUtlSFPOLHdrs.Find(user.CompanyCode, user.BranchCode
                                                    , oSFPOLHdrFile.DealerCode, user.CompanyCode, oSFPOLHdrFile.BatchNo);

                        if (oOmUtlSFPOLHdr != null)
                        {
                            for (int i = 1; i < lines.Length; i++)
                            {
                                query = string.Empty;
                                if (lines[i].Length == linesLength)
                                {
                                    if (lines[i].Substring(0, 1) == "1")
                                    {
                                        UploadBLL.SFPOLDtl1File oSFPOLDtl1File = new UploadBLL.SFPOLDtl1File(lines[i]);

                                        OmUtlSFPOLDtl1 oOmUtlSFPOLDtl1 = ctx.OmUtlSFPOLDtl1s.Find(user.CompanyCode, user.BranchCode
                                                                    , oSFPOLHdrFile.BatchNo, oSFPOLDtl1File.FakturPolisiNo, oSFPOLDtl1File.SalesModelCode
                                                                    , oSFPOLDtl1File.SalesModelYear, oSFPOLDtl1File.ColourCode, oSFPOLDtl1File.ChassisCode
                                                                    , oSFPOLDtl1File.ChassisNo);

                                        if (oOmUtlSFPOLDtl1 != null)
                                        {
                                            OmTrSalesFakturPolisi oOmTrSalesFakturPolisi = ctx.OmTrSalesFakturPolisi.Find(user.CompanyCode,
                                                       user.BranchCode, oSFPOLDtl1File.FakturPolisiNo);
                                            if (oOmTrSalesFakturPolisi == null)
                                            {
                                                DataRow dtRow = SelectDetailVehicle(user.CompanyCode, oOmUtlSFPOLDtl1.ChassisCode, oOmUtlSFPOLDtl1.ChassisNo);
                                               
                                                oOmUtlSFPOLDtl1.ReqNo = (dtRow != null) ? dtRow["ReqNo"].ToString() : oOmUtlSFPOLDtl1.ReqNo;

                                                //untuk test di remark dulu
                                                query = string.Format(@"INSERT INTO OmTrSalesFakturPolisi 
                                                                        (CompanyCode, BranchCode, FakturPolisiNo, SalesModelCode, SalesModelYear, 
                                                                        ColourCode, ChassisCode, ChassisNo, EngineCode, EngineNo, IsBlanko, FakturPolisiDate, 
                                                                        FakturPolisiProcess, SJImniNo, DOImniNo, ReqNo, CreatedBy, CreatedDate, LastUpdateBy, 
                                                                        LastUpdateDate, Status, IsManual)
                                                                       VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',
                                                                       '{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}') ",
                                                                       user.CompanyCode, user.BranchCode, oSFPOLDtl1File.FakturPolisiNo, oSFPOLDtl1File.SalesModelCode,
                                                                       oSFPOLDtl1File.SalesModelYear, oSFPOLDtl1File.ColourCode, oSFPOLDtl1File.ChassisCode,
                                                                       oSFPOLDtl1File.ChassisNo, oSFPOLDtl1File.EngineCode, oSFPOLDtl1File.EngineNo,
                                                                       (oSFPOLDtl1File.IsBlanko.ToString()) == "Y" ? "1" : "0", oSFPOLDtl1File.FakturPolisiDate,
                                                                       oSFPOLDtl1File.FakturPolisiProcess, oSFPOLDtl1File.SJNo, oSFPOLDtl1File.DONo, oSFPOLDtl1File.ReqNo,
                                                                       user.UserId, DateTime.Now, user.UserId, DateTime.Now, "0", "2");

                                                oOmTrSalesFakturPolisi = new OmTrSalesFakturPolisi();
                                                oOmTrSalesFakturPolisi.FakturPolisiNo = oSFPOLDtl1File.FakturPolisiNo;
                                                oOmTrSalesFakturPolisi.FakturPolisiDate = oSFPOLDtl1File.FakturPolisiDate;

                                                if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                                                {
                                                    result = false;
                                                    msg = "Process upload gagal";
                                                    return result;
                                                }
                                                else
                                                {
                                                    result = true;
                                                }
                                                ////untuk test result=true
                                                //result = true;
                                                query = string.Empty;
                                            }
                                            else
                                            {
                                                result = true;
                                            }
                                            if (result)
                                            {
                                                var oOmTrSalesReqDetail = GetReqDetailRecord(user.CompanyCode, user.BranchCode, oSFPOLDtl1File.ChassisCode, oSFPOLDtl1File.ChassisNo);
                                                if (oOmTrSalesReqDetail != null)
                                                {
                                                    //untuk test di remark dulu
                                                    query = string.Format(@"UPDATE OmTrSalesReqDetail
                                                                            SET FakturPolisiNo = '{0}' , FakturPolisiDate = '{1}' , LastUpdateBy = '{2}', LastUpdateDate = GetDate() 
                                                                            WHERE CompanyCode = '{3}' AND BranchCode = '{4}' AND ChassisCode = '{5}' AND ChassisNo = '{6}'",
                                                                            oOmTrSalesFakturPolisi.FakturPolisiNo, oOmTrSalesFakturPolisi.FakturPolisiDate,
                                                                            user.UserId, user.CompanyCode, user.BranchCode, oOmUtlSFPOLDtl1.ChassisCode,
                                                                            oOmUtlSFPOLDtl1.ChassisNo);

                                                    if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                                                    {
                                                        result = false;
                                                        msg = "Process upload gagal";
                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        result = true;
                                                    }
                                                    query = string.Empty;
                                                }
                                                else
                                                {
                                                    result = true;
                                                }

                                            }
                                            if (result)
                                            {
                                                OmMstVehicle oOmMstVehicle = ctx.OmMstVehicles.Find(user.CompanyCode,
                                                    oSFPOLDtl1File.ChassisCode, oSFPOLDtl1File.ChassisNo);

                                                if (oOmMstVehicle != null)
                                                {
                                                    //untuk test di remark dulu
                                                    query = string.Format(@"UPDATE OmMstVehicle
                                                                                SET FakturPolisiNo = '{0}' , FakturPolisiDate = '{1}' , LastUpdateBy = '{2}', LastUpdateDate = GetDate() 
                                                                                WHERE 
                                                                                CompanyCode = '{3}' AND ChassisCode = '{4}' AND ChassisNo = '{5}'",
                                                                            oOmTrSalesFakturPolisi.FakturPolisiNo, oOmTrSalesFakturPolisi.FakturPolisiDate,
                                                                            user.UserId, user.CompanyCode, oOmUtlSFPOLDtl1.ChassisCode, oOmUtlSFPOLDtl1.ChassisNo);

                                                    if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                                                    {
                                                        result = false;
                                                        msg = "Process upload gagal";
                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        result = true;
                                                    }
                                                    query = string.Empty;
                                                }
                                                else
                                                {
                                                    result = true;
                                                }

                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    msg = "flat file tidak ada data";
                                    return false;
                                }
                            }

                        }
                        else
                        {
                            //untuk test di remark dulu
                            query = string.Format(@"INSERT INTO OmUtlSFPOLHdr 
                                                    (CompanyCode, BranchCode, DealerCode, RcvDealerCode, BatchNo, Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                                    user.CompanyCode, user.BranchCode, oSFPOLHdrFile.DealerCode, user.CompanyCode,
                                                    oSFPOLHdrFile.BatchNo, "0", user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                            if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                            {
                                result = false;
                                msg = "Process upload gagal";
                                return result;
                            }
                            else
                            {
                                result = true;
                            }
                            ////untuk test result=true;
                            //result = true;
                            query = string.Empty;

                            if (result == true)
                            {
                                for (int i = 1; i < lines.Length; i++)
                                {
                                    query = string.Empty;
                                    if (lines[i].Length == linesLength)
                                    {
                                        if (lines[i].Substring(0, 1) == "1")
                                        {
                                            UploadBLL.SFPOLDtl1File oSFPOLDtl1File = new UploadBLL.SFPOLDtl1File(lines[i]);
                                            if (oSFPOLDtl1File != null && oSFPOLDtl1File.FakturPolisiNo != "")
                                            {
                                                OmUtlSFPOLDtl1 oOmUtlSFPOLDtl1 = ctx.OmUtlSFPOLDtl1s.Find(user.CompanyCode, user.BranchCode
                                                                            , oSFPOLHdrFile.BatchNo, oSFPOLDtl1File.FakturPolisiNo, oSFPOLDtl1File.SalesModelCode
                                                                            , oSFPOLDtl1File.SalesModelYear, oSFPOLDtl1File.ColourCode, oSFPOLDtl1File.ChassisCode
                                                                            , oSFPOLDtl1File.ChassisNo);

                                                if (oOmUtlSFPOLDtl1 == null)
                                                {
                                                    //untuk test di remark dulu
                                                    query += string.Format(@"INSERT INTO OmUtlSFPOLDtl1 (CompanyCode, BranchCode, BatchNo, FakturPolisiNo, 
                                                                            SalesModelCode, SalesModelYear, ColourCode, ChassisCode, ChassisNo, EngineCode, 
                                                                            EngineNo, IsBlanko, FakturPolisiDate, FakturPolisiProcess, DONo, SJNo, ReqNo, 
                                                                            CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                                            VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',
                                                                            '{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}') ",
                                                                             user.CompanyCode, user.BranchCode, oSFPOLHdrFile.BatchNo, oSFPOLDtl1File.FakturPolisiNo,
                                                                             oSFPOLDtl1File.SalesModelCode, oSFPOLDtl1File.SalesModelYear, oSFPOLDtl1File.ColourCode,
                                                                             oSFPOLDtl1File.ChassisCode, oSFPOLDtl1File.ChassisNo, oSFPOLDtl1File.EngineCode,
                                                                             oSFPOLDtl1File.EngineNo, (oSFPOLDtl1File.IsBlanko.ToString()) == "Y" ? "1" : "0",
                                                                             oSFPOLDtl1File.FakturPolisiDate, oSFPOLDtl1File.FakturPolisiProcess, oSFPOLDtl1File.DONo,
                                                                             oSFPOLDtl1File.SJNo, oSFPOLDtl1File.ReqNo, user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                                                    if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                                                    {
                                                        result = false;
                                                        msg = "Process upload gagal";
                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        result = true;
                                                    }
                                                    query = string.Empty;

                                                    if (result)
                                                    {
                                                        OmTrSalesFakturPolisi oOmTrSalesFakturPolisi = ctx.OmTrSalesFakturPolisi.Find(user.CompanyCode,
                                                                user.BranchCode, oSFPOLDtl1File.FakturPolisiNo);
                                                        if (oOmTrSalesFakturPolisi == null)
                                                        {
                                                            oOmUtlSFPOLDtl1 = ctx.OmUtlSFPOLDtl1s.Find(user.CompanyCode, user.BranchCode
                                                                           , oSFPOLHdrFile.BatchNo, oSFPOLDtl1File.FakturPolisiNo, oSFPOLDtl1File.SalesModelCode
                                                                           , oSFPOLDtl1File.SalesModelYear, oSFPOLDtl1File.ColourCode, oSFPOLDtl1File.ChassisCode
                                                                           , oSFPOLDtl1File.ChassisNo);

                                                            DataRow dtRow = SelectDetailVehicle(user.CompanyCode, oOmUtlSFPOLDtl1.ChassisCode, oOmUtlSFPOLDtl1.ChassisNo);
                                                           
                                                            oOmUtlSFPOLDtl1.ReqNo = (dtRow != null) ? dtRow["ReqNo"].ToString() : oOmUtlSFPOLDtl1.ReqNo;

                                                            //untuk test di remark dulu
                                                            query = string.Format(@"INSERT INTO OmTrSalesFakturPolisi (CompanyCode, BranchCode, FakturPolisiNo, SalesModelCode, 
                                                                                    SalesModelYear, ColourCode, ChassisCode, ChassisNo, EngineCode, EngineNo, IsBlanko, FakturPolisiDate, 
                                                                                    FakturPolisiProcess, SJImniNo, DOImniNo, ReqNo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, Status, IsManual)
                                                                                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}',
                                                                                    '{17}','{18}','{19}','{20}','{21}') ",
                                                                                    user.CompanyCode, user.BranchCode, oSFPOLDtl1File.FakturPolisiNo, oSFPOLDtl1File.SalesModelCode,
                                                                                    oSFPOLDtl1File.SalesModelYear, oSFPOLDtl1File.ColourCode, oSFPOLDtl1File.ChassisCode, oSFPOLDtl1File.ChassisNo,
                                                                                    oSFPOLDtl1File.EngineCode, oSFPOLDtl1File.EngineNo, (oSFPOLDtl1File.IsBlanko.ToString()) == "Y" ? "1" : "0",
                                                                                    oSFPOLDtl1File.FakturPolisiDate, oSFPOLDtl1File.FakturPolisiProcess, oSFPOLDtl1File.SJNo, oSFPOLDtl1File.DONo,
                                                                                    oSFPOLDtl1File.ReqNo, user.UserId, DateTime.Now, user.UserId, DateTime.Now, "0", "2");

                                                            oOmTrSalesFakturPolisi = new OmTrSalesFakturPolisi();
                                                            oOmTrSalesFakturPolisi.FakturPolisiNo = oSFPOLDtl1File.FakturPolisiNo;
                                                            oOmTrSalesFakturPolisi.FakturPolisiDate = oSFPOLDtl1File.FakturPolisiDate;

                                                            if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                                                            {
                                                                result = false;
                                                                msg = "Process upload gagal";
                                                                return result;
                                                            }
                                                            else
                                                            {
                                                                result = true;
                                                            }
                                                            query = string.Empty;

                                                        }
                                                        if (result)
                                                        {
                                                            var oOmTrSalesReqDetail = GetReqDetailRecord(user.CompanyCode, user.BranchCode, oSFPOLDtl1File.ChassisCode,
                                                                    oSFPOLDtl1File.ChassisNo);
                                                            if (oOmTrSalesReqDetail != null)
                                                            {
                                                                //untuk test di remark dulu
                                                                query = string.Format(@"UPDATE OmTrSalesReqDetail
                                                                                        SET FakturPolisiNo = '{0}' , FakturPolisiDate = '{1}' , LastUpdateBy = '{2}', LastUpdateDate = GetDate() 
                                                                                        WHERE 
                                                                                        CompanyCode = '{3}' AND BranchCode = '{4}' AND ChassisCode = '{5}' AND ChassisNo = '{6}'",
                                                                                        oOmTrSalesFakturPolisi.FakturPolisiNo, oOmTrSalesFakturPolisi.FakturPolisiDate,
                                                                                        user.UserId, user.CompanyCode, user.BranchCode, oOmUtlSFPOLDtl1.ChassisCode, oOmUtlSFPOLDtl1.ChassisNo);

                                                                if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                                                                {
                                                                    result = false;
                                                                    msg = "Process upload gagal";
                                                                    return result;
                                                                }
                                                                else
                                                                {
                                                                    result = true;
                                                                }
                                                                query = string.Empty;
                                                            }
                                                            else
                                                            {
                                                                result = true;
                                                            }
                                                        }
                                                        if (result)
                                                        {
                                                            OmMstVehicle oOmMstVehicle = ctx.OmMstVehicles.Find(user.CompanyCode,
                                                                    oSFPOLDtl1File.ChassisCode, oSFPOLDtl1File.ChassisNo);
                                                            if (oOmMstVehicle != null)
                                                            {
                                                                //untuk test di remark dulu
                                                                query = string.Format(@"UPDATE OmMstVehicle
                                                                                        SET FakturPolisiNo = '{0}' , FakturPolisiDate = '{1}' , LastUpdateBy = '{2}', LastUpdateDate = GetDate() 
                                                                                        WHERE CompanyCode = '{3}' AND ChassisCode = '{4}' AND ChassisNo = '{5}'",
                                                                                        oOmTrSalesFakturPolisi.FakturPolisiNo, oOmTrSalesFakturPolisi.FakturPolisiDate, user.UserId,
                                                                                        user.CompanyCode, oOmUtlSFPOLDtl1.ChassisCode, oOmUtlSFPOLDtl1.ChassisNo);

                                                                if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                                                                {
                                                                    result = false;
                                                                    msg = "Process upload gagal";
                                                                    return result;
                                                                }
                                                                else
                                                                {
                                                                    result = true;
                                                                }
                                                                query = string.Empty;

                                                            }
                                                            else
                                                            {
                                                                result = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        msg = "Flat file tidak valid, mohon di-cek";
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                msg = "Gagal insert table OmUtlSFPOLHdr";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        msg = "flat file tidak valid";
                        return false;
                    }
                }
            }
            else
            {
                msg = "flat file tidak valid";
                return false;
            }

            return result;
        }

        private bool UploadDataSFPLBLocal(string[] lines)
        {
            DcsHelper dcsHelper = new DcsHelper(CurrentUser.UserId);
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);

            bool clear = true;
            string chassisCode = string.Empty;
            int chassisNo = 0;
            string batchNo = "";
            string flagFPol = dcsHelper.GetParaValueDCS(user.CompanyCode, GnMstDocumentConstant.DLRSTK, GnMstDocumentConstant.FPOL);
            string flagSJal = dcsHelper.GetParaValueDCS(user.CompanyCode, GnMstDocumentConstant.DLRSTK, GnMstDocumentConstant.SJAL);

            int linesLength = 450;
            MessageDcs.Clear();

            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return result;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

                    string query = string.Empty;
                    StringBuilder sb = new StringBuilder();

                    UploadBLL.SFPLBHdrFile oSFPLBHdrFile = new UploadBLL.SFPLBHdrFile(lines[0]);
                    GnMstCustomer dtCustomer = ctx.GnMstCustomer.Find(user.CompanyCode, oSFPLBHdrFile.DealerCode);

                    if (!user.CompanyCode.Equals(oSFPLBHdrFile.RcvDealerCode))
                    {
                        msg = "Invalid flat file, kode perusahaan tidak sama";
                        result = false; return result;
                    }

                    if (dtCustomer != null && dtCustomer.CategoryCode != "01")
                    {
                        msg = "Invalid flat file, kode perusahaan penerima bukan Sub-Dealer";
                        result = false; return result;
                    }

                    if (oSFPLBHdrFile != null && (oSFPLBHdrFile.DataID == "SFPLB" || oSFPLBHdrFile.DataID == "SFPLA" || oSFPLBHdrFile.DataID == "SFPLR"))
                    {
                        omUtlFpolReq oOmUtlFPolReq = ctx.omUtlFpolReqs.Find(user.CompanyCode, user.BranchCode, oSFPLBHdrFile.DealerCode, oSFPLBHdrFile.BatchNo);
                        if (oOmUtlFPolReq != null)
                        {
                            msg = "Proses upload file gagal";
                            return result;
                        }
                        else
                        {
                            //untuk test di remark dulu
                            query = string.Format(@"INSERT INTO omUtlFPolReq (CompanyCode, BranchCode, DealerCode, BatchNo, Status, 
                                                    CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                                                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                                    user.CompanyCode, user.BranchCode, oSFPLBHdrFile.DealerCode, oSFPLBHdrFile.BatchNo, "0",
                                                    user.UserId, DateTime.Now, user.UserId, DateTime.Now);

                            if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                            {
                                result = false;
                                msg = "Process upload gagal";
                                return result;
                            }
                            else
                            {
                                result = true;
                            }
                            ////untuk test result=true;
                            //result = true;
                            query = string.Empty;

                            batchNo = oSFPLBHdrFile.BatchNo;
                            oOmUtlFPolReq = ctx.omUtlFpolReqs.Find(user.CompanyCode, user.BranchCode, oSFPLBHdrFile.DealerCode, oSFPLBHdrFile.BatchNo);

                            if (result == true)
                            {
                                for (int i = 1; i < lines.Length; i++)
                                {
                                    if (i < 50)
                                    {
                                        if (clear == true)
                                        {
                                            //untuk test di remark dulu
                                            query = @"INSERT INTO [omUtlFPolReqDetail]([CompanyCode],[BranchCode],[BatchNo],[ChassisCode],[ChassisNo],[EngineCode],
                                                        [EngineNo],[SalesModelCode],[SalesModelYear],[SalesModelDescription],[ModelLine],[ColourCode],[ColourDescription],
                                                        [ServiceBookNo],[FakturPolisiNo],[FakturPolisiDate],[FpolisiModelDescription],[SISDeliveryOrderNo],
                                                        [SISDeliveryOrderDate],[SISDeliveryOrderAtasNama],[SISSuratJalanNo],[SISSuratJalanDate],[SISSuratJalanAtasNama],
                                                        [OldDealerCode],[DealerClass],[DealerName],[SKPKNo],[SuratPermohonanNo],[SalesmanName],[SKPKName],[SKPKName2],
                                                        [SKPKAddr1],[SKPKAddr2],[SKPKAddr3],[SKPKCityCode],[SKPKPhoneNo1],[SKPKPhoneNo2],[SKPKHPNo],[SKPKBirthday],
                                                        [FPolName],[FPolName2],[FPolAddr1],[FPolAddr2],[FPolAddr3],[FPolPostCode],[FPolPostName],[FPolCityCode],
                                                        [FPolKecamatanCode],[FPolPhoneNo1],[FPolPhoneNo2],[FPolHPNo],[FPolBirthday],[IdentificationNo],[IsProject],
                                                        [ReasonCode],[ReasonDescription],[ProcessDate],[IsCityTransport],[CreatedBy],[CreatedDate],[LastUpdateBy],
                                                        [LastUpdateDate],[Status])
                                                      VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}',
                                                        '{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}',
                                                        '{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}','{52}','{53}','{54}','{55}',
                                                        '{56}','{57}','{58}','{59}','{60}','{61}','{62}')";

                                            query.Replace("{0}", user.CompanyCode);
                                            query.Replace("{1}", user.BranchCode);
                                            query.Replace("{2}", oSFPLBHdrFile.BatchNo);
                                            query.Replace("{58}", user.UserId);
                                            query.Replace("{59}", DateTime.Now.ToString());
                                            query.Replace("{60}", user.UserId);
                                            query.Replace("{61}", DateTime.Now.ToString());
                                            query.Replace("{62}", "0");

                                            clear = false;
                                        }
                                        string line = lines[i];
                                        if (lines[i].Length == linesLength)
                                        {
                                            #region ** Line 1 **
                                            if (lines[i].Substring(0, 1) == "1")
                                            {
                                                UploadBLL.SFPLBDtl1File oSFPLBDtl1File = new UploadBLL.SFPLBDtl1File(lines[i]);
                                                if (oSFPLBDtl1File != null && oSFPLBDtl1File.DeliveryOrder != "")
                                                {
                                                    query.Replace("{17}", oSFPLBDtl1File.DeliveryOrder);
                                                    query.Replace("{18}", oSFPLBDtl1File.DeliveryOrderDate.ToShortDateString());
                                                    query.Replace("{19}", oSFPLBDtl1File.DeliveryOrderAtasNama);
                                                }
                                            }
                                            #endregion
                                            #region ** Line 2 **
                                            if (lines[i].Substring(0, 1) == "2")
                                            {
                                                UploadBLL.SFPLBDtl2File oSFPLBDtl2File = new UploadBLL.SFPLBDtl2File(lines[i]);
                                                if (oSFPLBDtl2File != null && oSFPLBDtl2File.SuratJalan != "")
                                                {
                                                    query.Replace("{20}", oSFPLBDtl2File.SuratJalan);
                                                    query.Replace("{21}", oSFPLBDtl2File.SuratJalanDate.ToShortDateString());
                                                    query.Replace("{22}", oSFPLBDtl2File.SuratJalanAtasNama);
                                                    query.Replace("{7}", oSFPLBDtl2File.ModelCode);
                                                    query.Replace("{9}", oSFPLBDtl2File.SalesModelDescription);
                                                    query.Replace("{16}", oSFPLBDtl2File.FpolisiModelDescription);
                                                    query.Replace("{10}", oSFPLBDtl2File.ModelLine);
                                                    query.Replace("{23}", oSFPLBDtl2File.OldDealerCode);
                                                }
                                            }
                                            #endregion
                                            #region ** Line 3 **
                                            if (lines[i].Substring(0, 1) == "3")
                                            {
                                                UploadBLL.SFPLBDtl3File oSFPLBDtl3File = new UploadBLL.SFPLBDtl3File(lines[i]);
                                                if (oSFPLBDtl3File != null && oSFPLBDtl3File.DealerClass != "")
                                                {
                                                    query.Replace("{24}", oSFPLBDtl3File.DealerClass);
                                                    query.Replace("{25}", oSFPLBDtl3File.DealerName);
                                                    query.Replace("{26}", oSFPLBDtl3File.NoSKPK);
                                                    query.Replace("{27}", oSFPLBDtl3File.NoSuratPermohonan);
                                                    query.Replace("{28}", oSFPLBDtl3File.SalesmanName);
                                                    query.Replace("{29}", oSFPLBDtl3File.NamaSKPK);
                                                    query.Replace("{30}", oSFPLBDtl3File.NamaSKPK2);
                                                    query.Replace("{31}", oSFPLBDtl3File.Alamat1SKPK);
                                                    query.Replace("{32}", oSFPLBDtl3File.Alamat2SKPK);
                                                    query.Replace("{33}", oSFPLBDtl3File.Alamat3SKPK);
                                                    query.Replace("{34}", oSFPLBDtl3File.CityCode);
                                                    query.Replace("{35}", oSFPLBDtl3File.TeleponNo1);
                                                    query.Replace("{36}", oSFPLBDtl3File.TeleponNo2);
                                                    query.Replace("{37}", oSFPLBDtl3File.HandPhoneNo);
                                                    query.Replace("{38}", oSFPLBDtl3File.BirthdaySKPK);
                                                }
                                            }
                                            #endregion
                                            #region ** Line 4 **
                                            if (lines[i].Substring(0, 1) == "4")
                                            {
                                                UploadBLL.SFPLBDtl4File oSFPLBDtl4File = new UploadBLL.SFPLBDtl4File(lines[i]);
                                                if (oSFPLBDtl4File != null && oSFPLBDtl4File.Nama != "")
                                                {
                                                    query.Replace("{39}", oSFPLBDtl4File.Nama);
                                                    query.Replace("{40}", oSFPLBDtl4File.Nama2);
                                                    query.Replace("{41}", oSFPLBDtl4File.Alamat1);
                                                    query.Replace("{42}", oSFPLBDtl4File.Alamat2);
                                                    query.Replace("{43}", oSFPLBDtl4File.Alamat3);
                                                    query.Replace("{44}", oSFPLBDtl4File.PostCode);
                                                    query.Replace("{45}", oSFPLBDtl4File.PostName);
                                                    query.Replace("{46}", oSFPLBDtl4File.CityCode);
                                                    query.Replace("{47}", oSFPLBDtl4File.KodeKecamatan);
                                                    query.Replace("{48}", oSFPLBDtl4File.Telepon1);
                                                    query.Replace("{49}", oSFPLBDtl4File.Telepon2);
                                                    query.Replace("{50}", oSFPLBDtl4File.HandPhone);
                                                    query.Replace("{51}", oSFPLBDtl4File.BirthdayFpol.ToShortDateString());
                                                    query.Replace("{52}", oSFPLBDtl4File.IDNO);
                                                    query.Replace("{53}", oSFPLBDtl4File.IsProject);
                                                }
                                            }
                                            #endregion
                                            #region ** Line 5 **
                                            if (lines[i].Substring(0, 1) == "5")
                                            {
                                                UploadBLL.SFPLBDtl5File oSFPLBDtl5File = new UploadBLL.SFPLBDtl5File(lines[i]);
                                                if (oSFPLBDtl5File != null && oSFPLBDtl5File.FakturPolisiNo != "")
                                                {
                                                    query.Replace("{54}", oSFPLBDtl5File.ReasonCode);
                                                    query.Replace("{55}", oSFPLBDtl5File.ReasonDescription);
                                                    query.Replace("{56}", oSFPLBDtl5File.ProcessDate);
                                                    query.Replace("{57}", oSFPLBDtl5File.IsCityTransport);
                                                    query.Replace("{14}", oSFPLBDtl5File.FakturPolisiNo);
                                                    query.Replace("{15}", oSFPLBDtl5File.FakturPolisiDate.ToShortDateString());
                                                    query.Replace("{3}", oSFPLBDtl5File.KodeRangka);
                                                    query.Replace("{4}", oSFPLBDtl5File.NoRangka.ToString());
                                                    query.Replace("{5}", oSFPLBDtl5File.KodeMesin);
                                                    query.Replace("{6}", oSFPLBDtl5File.NoMesin.ToString());
                                                    query.Replace("{8}", oSFPLBDtl5File.Year.ToString());
                                                    query.Replace("{11}", oSFPLBDtl5File.ColorCode);
                                                    query.Replace("{12}", oSFPLBDtl5File.ColorDescription);
                                                    query.Replace("{13}", oSFPLBDtl5File.ServiceBookNo);

                                                    chassisCode = oSFPLBDtl5File.KodeRangka;
                                                    chassisNo = oSFPLBDtl5File.NoRangka;

                                                }
                                            }
                                            #endregion
                                            #region ** Line 6 **
                                            if (lines[i].Substring(0, 1) == "6")
                                            {
                                                UploadBLL.SFPLBDtl6File oSFPLBDtl6File = new UploadBLL.SFPLBDtl6File(lines[i]);
                                                if (oSFPLBDtl6File != null && oSFPLBDtl6File.JenisKelamin != "")
                                                {
                                                    if (oSFPLBDtl6File.JenisKelamin != "1")
                                                    {
                                                        //untuk test di remark dulu
                                                        query += string.Format(@" INSERT INTO [omUtlFPolReqDetailAdditional]([CompanyCode],[BranchCode],[BatchNo],[ChassisCode],
                                                                                    [ChassisNo],[Gender],[TempatPembelian],[TempatPembelianOthers],[KendaraanYgPernahDimiliki],
                                                                                    [KendaraanYgPernahDimilikiModel],[SumberPembelian],[SumberPembelianOthers],[AsalPembelian],
                                                                                    [AsalPembelianOthers],[InfoSuzukiDari],[InfoSuzukiDariOthers],[FaktorPentingKendaraan],
                                                                                    [PendidikanTerakhir],[PendidikanTerakhirOthers],[PenghasilanKeluarga],[Pekerjaan],[PekerjaanOthers],
                                                                                    [PenggunaanKendaraan],[PenggunaanKendaraanOthers],[CaraPembelian],[Leasing],[LeasingOthers],
                                                                                    [JangkaWaktuKredit],[JangkaWaktuKreditOthers],[CreatedBy],[CreatedDate],[LastUpdateBy],[LastUpdateDate])
                                                                                  VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',
                                                                                    '{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}',
                                                                                    '{29}','{30}','{31}','{32}')",
                                                                                  user.CompanyCode, user.BranchCode, oSFPLBHdrFile.BatchNo, chassisCode, chassisNo.ToString(),
                                                                                  oSFPLBDtl6File.JenisKelamin, oSFPLBDtl6File.TempatPembelianMotor, oSFPLBDtl6File.TPSOther,
                                                                                  oSFPLBDtl6File.MotorYgPernahDipakai, oSFPLBDtl6File.TypeKendaraan, oSFPLBDtl6File.SumberPembelian,
                                                                                  oSFPLBDtl6File.SUPOthers, oSFPLBDtl6File.AsalPembelian, oSFPLBDtl6File.ASPOthers,
                                                                                  oSFPLBDtl6File.InformasiSepedaMotor, oSFPLBDtl6File.SRIOthers, oSFPLBDtl6File.FaktorPentingSpdMotor,
                                                                                  oSFPLBDtl6File.PendidikanTerakhir, oSFPLBDtl6File.PDKOthers, oSFPLBDtl6File.PenghasilanKeluarga,
                                                                                  oSFPLBDtl6File.Pekerjaan, oSFPLBDtl6File.PEKOthers, oSFPLBDtl6File.MotorCycleFunction, oSFPLBDtl6File.USEOthers,
                                                                                  oSFPLBDtl6File.CaraPembelian, oSFPLBDtl6File.LeasingYgDipakai, oSFPLBDtl6File.LSGOthers, oSFPLBDtl6File.JangkaWaktuKredit,
                                                                                  oSFPLBDtl6File.JWKOthers, user.UserId, DateTime.Now, user.UserId, DateTime.Now);
                                                    }

                                                    if (flagFPol == "1")
                                                    {
                                                        var oOmUtlFPolReqDetail = GetDatabyChassisCodeAndChassisNo(user.CompanyCode, chassisCode, Convert.ToDecimal(chassisNo));
                                                        if (oOmUtlFPolReqDetail != null)
                                                        {
                                                            msg = "Kendaraan dengan Chassis Code : " + chassisCode + " dan no Rangka : " + chassisNo.ToString() + " sudah tersedia di database Faktur Polisi";
                                                            result = false;
                                                            query = string.Empty;
                                                        }

                                                    }
                                                    if (flagSJal == "1")
                                                    {
                                                        int oOmTrSalesInvoice = CountSalesInvoicebyChassisCodeAndNo(user.CompanyCode, user.BranchCode, chassisCode, Convert.ToDecimal(chassisNo));

                                                        if (oOmTrSalesInvoice == 0)
                                                        {
                                                            msg = "Kendaraan dengan Chassis Code : " + chassisCode + " dan no Rangka : " + chassisNo.ToString() + " tidak tersedia dalam database VIN invoice";
                                                            result = false;
                                                            query = string.Empty;
                                                        }
                                                    }

                                                    sb.AppendLine(query);
                                                    clear = true;
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            msg = "Proses upload gagal";
                                            return result;
                                        }
                                    }
                                    else
                                    {
                                        query = sb.ToString();
                                        if (query != "")
                                        {
                                            result = ctx.Database.ExecuteSqlCommand(query) > 0;
                                            query = "";
                                        }
                                        else result = true;
                                        query = "";

                                        sb = new StringBuilder();
                                        clear = true;

                                        if (clear)
                                        {
                                            //untuk test di remark dulu
                                            query = @"INSERT INTO [omUtlFPolReqDetail]([CompanyCode],[BranchCode],[BatchNo],[ChassisCode],[ChassisNo],
                                                        [EngineCode],[EngineNo],[SalesModelCode],[SalesModelYear],[SalesModelDescription],[ModelLine],
                                                        [ColourCode],[ColourDescription],[ServiceBookNo],[FakturPolisiNo],[FakturPolisiDate],
                                                        [FpolisiModelDescription],[SISDeliveryOrderNo],[SISDeliveryOrderDate],[SISDeliveryOrderAtasNama],
                                                        [SISSuratJalanNo],[SISSuratJalanDate],[SISSuratJalanAtasNama],[OldDealerCode],[DealerClass],
                                                        [DealerName],[SKPKNo],[SuratPermohonanNo],[SalesmanName],[SKPKName],[SKPKName2],[SKPKAddr1],
                                                        [SKPKAddr2],[SKPKAddr3],[SKPKCityCode],[SKPKPhoneNo1],[SKPKPhoneNo2],[SKPKHPNo],[SKPKBirthday],
                                                        [FPolName],[FPolName2],[FPolAddr1],[FPolAddr2],[FPolAddr3],[FPolPostCode],[FPolPostName],
                                                        [FPolCityCode],[FPolKecamatanCode],[FPolPhoneNo1],[FPolPhoneNo2],[FPolHPNo],[FPolBirthday],
                                                        [IdentificationNo],[IsProject],[ReasonCode],[ReasonDescription],[ProcessDate],[IsCityTransport],
                                                        [CreatedBy],[CreatedDate],[LastUpdateBy],[LastUpdateDate],[Status])
                                                      VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}',
                                                        '{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}',
                                                        '{28}','{29}','{30}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}',
                                                        '{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}','{60}','{61}','{62}')";

                                            query.Replace("{0}", user.CompanyCode);
                                            query.Replace("{1}", user.BranchCode);
                                            query.Replace("{2}", oSFPLBHdrFile.BatchNo);
                                            query.Replace("{58}", user.UserId);
                                            query.Replace("{59}", DateTime.Now.ToString());
                                            query.Replace("{60}", user.UserId);
                                            query.Replace("{61}", DateTime.Now.ToString());
                                            query.Replace("{62}", "0");

                                            clear = false;
                                        }
                                        string line = lines[i];
                                        if (lines[i].Length == linesLength)
                                        {
                                            #region ** Line 1 **
                                            if (lines[i].Substring(0, 1) == "1")
                                            {
                                                UploadBLL.SFPLBDtl1File oSFPLBDtl1File = new UploadBLL.SFPLBDtl1File(lines[i]);
                                                if (oSFPLBDtl1File != null && oSFPLBDtl1File.DeliveryOrder != "")
                                                {
                                                    query.Replace("{17}", oSFPLBDtl1File.DeliveryOrder);
                                                    query.Replace("{18}", oSFPLBDtl1File.DeliveryOrderDate.ToShortDateString());
                                                    query.Replace("{19}", oSFPLBDtl1File.DeliveryOrderAtasNama);
                                                }
                                            }
                                            #endregion
                                            #region ** Line 2 **
                                            if (lines[i].Substring(0, 1) == "2")
                                            {
                                                UploadBLL.SFPLBDtl2File oSFPLBDtl2File = new UploadBLL.SFPLBDtl2File(lines[i]);
                                                if (oSFPLBDtl2File != null && oSFPLBDtl2File.SuratJalan != "")
                                                {
                                                    query.Replace("{20}", oSFPLBDtl2File.SuratJalan);
                                                    query.Replace("{21}", oSFPLBDtl2File.SuratJalanDate.ToShortDateString());
                                                    query.Replace("{22}", oSFPLBDtl2File.SuratJalanAtasNama);
                                                    query.Replace("{7}", oSFPLBDtl2File.ModelCode);
                                                    query.Replace("{9}", oSFPLBDtl2File.SalesModelDescription);
                                                    query.Replace("{16}", oSFPLBDtl2File.FpolisiModelDescription);
                                                    query.Replace("{10}", oSFPLBDtl2File.ModelLine);
                                                    query.Replace("{23}", oSFPLBDtl2File.OldDealerCode);
                                                }
                                            }
                                            #endregion
                                            #region ** Line 3 **
                                            if (lines[i].Substring(0, 1) == "3")
                                            {
                                                UploadBLL.SFPLBDtl3File oSFPLBDtl3File = new UploadBLL.SFPLBDtl3File(lines[i]);
                                                if (oSFPLBDtl3File != null && oSFPLBDtl3File.DealerClass != "")
                                                {
                                                    query.Replace("{24}", oSFPLBDtl3File.DealerClass);
                                                    query.Replace("{25}", oSFPLBDtl3File.DealerName);
                                                    query.Replace("{26}", oSFPLBDtl3File.NoSKPK);
                                                    query.Replace("{27}", oSFPLBDtl3File.NoSuratPermohonan);
                                                    query.Replace("{28}", oSFPLBDtl3File.SalesmanName);
                                                    query.Replace("{29}", oSFPLBDtl3File.NamaSKPK);
                                                    query.Replace("{30}", oSFPLBDtl3File.NamaSKPK2);
                                                    query.Replace("{31}", oSFPLBDtl3File.Alamat1SKPK);
                                                    query.Replace("{32}", oSFPLBDtl3File.Alamat2SKPK);
                                                    query.Replace("{33}", oSFPLBDtl3File.Alamat3SKPK);
                                                    query.Replace("{34}", oSFPLBDtl3File.CityCode);
                                                    query.Replace("{35}", oSFPLBDtl3File.TeleponNo1);
                                                    query.Replace("{36}", oSFPLBDtl3File.TeleponNo2);
                                                    query.Replace("{37}", oSFPLBDtl3File.HandPhoneNo);
                                                    query.Replace("{38}", oSFPLBDtl3File.BirthdaySKPK);
                                                }
                                            }
                                            #endregion
                                            #region ** Line 4 **
                                            if (lines[i].Substring(0, 1) == "4")
                                            {
                                                UploadBLL.SFPLBDtl4File oSFPLBDtl4File = new UploadBLL.SFPLBDtl4File(lines[i]);
                                                if (oSFPLBDtl4File != null && oSFPLBDtl4File.Nama != "")
                                                {
                                                    query.Replace("{39}", oSFPLBDtl4File.Nama);
                                                    query.Replace("{40}", oSFPLBDtl4File.Nama2);
                                                    query.Replace("{41}", oSFPLBDtl4File.Alamat1);
                                                    query.Replace("{42}", oSFPLBDtl4File.Alamat2);
                                                    query.Replace("{43}", oSFPLBDtl4File.Alamat3);
                                                    query.Replace("{44}", oSFPLBDtl4File.PostCode);
                                                    query.Replace("{45}", oSFPLBDtl4File.PostName);
                                                    query.Replace("{46}", oSFPLBDtl4File.CityCode);
                                                    query.Replace("{47}", oSFPLBDtl4File.KodeKecamatan);
                                                    query.Replace("{48}", oSFPLBDtl4File.Telepon1);
                                                    query.Replace("{49}", oSFPLBDtl4File.Telepon2);
                                                    query.Replace("{50}", oSFPLBDtl4File.HandPhone);
                                                    query.Replace("{51}", oSFPLBDtl4File.BirthdayFpol.ToShortDateString());
                                                    query.Replace("{52}", oSFPLBDtl4File.IDNO);
                                                    query.Replace("{53}", oSFPLBDtl4File.IsProject);
                                                }
                                            }
                                            #endregion
                                            #region ** Line 5 **
                                            if (lines[i].Substring(0, 1) == "5")
                                            {
                                                UploadBLL.SFPLBDtl5File oSFPLBDtl5File = new UploadBLL.SFPLBDtl5File(lines[i]);
                                                if (oSFPLBDtl5File != null && oSFPLBDtl5File.FakturPolisiNo != "")
                                                {
                                                    query.Replace("{54}", oSFPLBDtl5File.ReasonCode);
                                                    query.Replace("{55}", oSFPLBDtl5File.ReasonDescription);
                                                    query.Replace("{56}", oSFPLBDtl5File.ProcessDate);
                                                    query.Replace("{57}", oSFPLBDtl5File.IsCityTransport);
                                                    query.Replace("{14}", oSFPLBDtl5File.FakturPolisiNo);
                                                    query.Replace("{15}", oSFPLBDtl5File.FakturPolisiDate.ToShortDateString());
                                                    query.Replace("{3}", oSFPLBDtl5File.KodeRangka);
                                                    query.Replace("{4}", oSFPLBDtl5File.NoRangka.ToString());
                                                    query.Replace("{5}", oSFPLBDtl5File.KodeMesin);
                                                    query.Replace("{6}", oSFPLBDtl5File.NoMesin.ToString());
                                                    query.Replace("{8}", oSFPLBDtl5File.Year.ToString());
                                                    query.Replace("{11}", oSFPLBDtl5File.ColorCode);
                                                    query.Replace("{12}", oSFPLBDtl5File.ColorDescription);
                                                    query.Replace("{13}", oSFPLBDtl5File.ServiceBookNo);

                                                    chassisCode = oSFPLBDtl5File.KodeRangka;
                                                    chassisNo = oSFPLBDtl5File.NoRangka;

                                                }
                                            }
                                            #endregion
                                            #region ** Line 6 **
                                            if (lines[i].Substring(0, 1) == "6")
                                            {
                                                UploadBLL.SFPLBDtl6File oSFPLBDtl6File = new UploadBLL.SFPLBDtl6File(lines[i]);
                                                if (oSFPLBDtl6File != null && oSFPLBDtl6File.JenisKelamin != "")
                                                {
                                                    if (oSFPLBDtl6File.JenisKelamin != "1")
                                                    {
                                                        //untuk test di remark dulu
                                                        query += string.Format(@" INSERT INTO [omUtlFPolReqDetailAdditional]([CompanyCode],[BranchCode],[BatchNo],[ChassisCode],
                                                                                    [ChassisNo],[Gender],[TempatPembelian],[TempatPembelianOthers],[KendaraanYgPernahDimiliki],
                                                                                    [KendaraanYgPernahDimilikiModel],[SumberPembelian],[SumberPembelianOthers],[AsalPembelian],
                                                                                    [AsalPembelianOthers],[InfoSuzukiDari],[InfoSuzukiDariOthers],[FaktorPentingKendaraan],
                                                                                    [PendidikanTerakhir],[PendidikanTerakhirOthers],[PenghasilanKeluarga],[Pekerjaan],
                                                                                    [PekerjaanOthers],[PenggunaanKendaraan],[PenggunaanKendaraanOthers],[CaraPembelian],[Leasing],
                                                                                    [LeasingOthers],[JangkaWaktuKredit],[JangkaWaktuKreditOthers],[CreatedBy],[CreatedDate],[LastUpdateBy],[LastUpdateDate])
                                                                                  VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}',
                                                                                    '{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                                                                                  user.CompanyCode, user.BranchCode, oSFPLBHdrFile.BatchNo, chassisCode, chassisNo.ToString(),
                                                                                  oSFPLBDtl6File.JenisKelamin, oSFPLBDtl6File.TempatPembelianMotor, oSFPLBDtl6File.TPSOther,
                                                                                  oSFPLBDtl6File.MotorYgPernahDipakai, oSFPLBDtl6File.TypeKendaraan, oSFPLBDtl6File.SumberPembelian,
                                                                                  oSFPLBDtl6File.SUPOthers, oSFPLBDtl6File.AsalPembelian, oSFPLBDtl6File.ASPOthers,
                                                                                  oSFPLBDtl6File.InformasiSepedaMotor, oSFPLBDtl6File.SRIOthers, oSFPLBDtl6File.FaktorPentingSpdMotor,
                                                                                  oSFPLBDtl6File.PendidikanTerakhir, oSFPLBDtl6File.PDKOthers, oSFPLBDtl6File.PenghasilanKeluarga,
                                                                                  oSFPLBDtl6File.Pekerjaan, oSFPLBDtl6File.PEKOthers, oSFPLBDtl6File.MotorCycleFunction,
                                                                                  oSFPLBDtl6File.USEOthers, oSFPLBDtl6File.CaraPembelian, oSFPLBDtl6File.LeasingYgDipakai,
                                                                                  oSFPLBDtl6File.LSGOthers, oSFPLBDtl6File.JangkaWaktuKredit, oSFPLBDtl6File.JWKOthers,
                                                                                  user.UserId, DateTime.Now, user.UserId, DateTime.Now);
                                                    }
                                                    if (flagFPol == "1")
                                                    {
                                                        var oOmUtlFPolReqDetail = GetDatabyChassisCodeAndChassisNo(user.CompanyCode, chassisCode, Convert.ToDecimal(chassisNo));
                                                        if (oOmUtlFPolReqDetail != null)
                                                        {
                                                            msg = "Kendaraan dengan Chassis Code : " + chassisCode + " dan no Rangka : " + chassisNo.ToString() + " sudah tersedia di database Faktur Polisi";
                                                            result = false;
                                                            query = string.Empty;
                                                        }
                                                    }
                                                    if (flagSJal == "1")
                                                    {
                                                        int oOmTrSalesInvoice = CountSalesInvoicebyChassisCodeAndNo(user.CompanyCode, user.BranchCode, chassisCode, Convert.ToDecimal(chassisNo));
                                                        if (oOmTrSalesInvoice == 0)
                                                        {
                                                            msg = "Kendaraan dengan Chassis Code : " + chassisCode + " dan no Rangka : " + chassisNo.ToString() + " tidak tersedia dalam database VIN invoice";
                                                            result = false;
                                                            query = string.Empty;
                                                        }

                                                    }

                                                    sb.AppendLine(query);
                                                    clear = true;
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            msg = "Proses upload gagal";
                                            return result;
                                        }
                                    }
                                }
                                if (sb != null)
                                {
                                    query = sb.ToString();
                                    if (query != "")
                                    {
                                        result = ctx.Database.ExecuteSqlCommand(query) > 0;
                                        query = "";
                                    }
                                    else
                                        result = true;
                                }
                            }
                            else
                            {
                                msg = "Proses upload gagal";
                                return result;
                            }
                        }
                    }

                    else
                    {
                        msg = "flat file tidak valid";
                        return false;
                    }
                }
            }
            else
            {
                msg = "flat file tidak valid";
                return false;
            }

            return result;
        }

        private bool UploadDataSUADELocal(string[] lines)
        {
            DcsHelper dcsHelper = new DcsHelper(CurrentUser.UserId);
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);

            int linesLength = 110;
            MessageDcs.Clear();
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return result;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

                    string query = string.Empty;
                    string storedQuery = string.Empty;
                    string execQuery = string.Empty;

                    StringBuilder sb = new StringBuilder();
                    UploadBLL.SUADEHdrFile oSUADEHdrFile = new UploadBLL.SUADEHdrFile(lines[0]);

                    GnMstCustomer dtCustomer = ctx.GnMstCustomer.Find(user.CompanyCode, oSUADEHdrFile.DealerCode);
                    GnMstCoProfileSales GnMstCoProfileSales = ctx.GnMstCoProfileSaleses.Find(user.CompanyCode, user.BranchCode);
                    string lockingBy = string.Empty;
                    if (GnMstCoProfileSales != null) lockingBy = GnMstCoProfileSales.LockingBy;

                    if (!user.CompanyCode.Equals(oSUADEHdrFile.RcvDealerCode) && !lockingBy.Equals(oSUADEHdrFile.RcvDealerCode))
                    {
                        msg = "Invalid flat file, kode perusahaan tidak sama";
                        result = false; return result;
                    }

                    if (dtCustomer != null && dtCustomer.CategoryCode != "01")
                    {
                        msg = "Invalid flat file, kode perusahaan penerima bukan Sub-Dealer";
                        result = false; return result;
                    }
                    if (oSUADEHdrFile != null && oSUADEHdrFile.DataID == "SUADE")
                    {
                        int count = 0;
                        int totalQuota = 0;
                        List<string> stringQuery = new List<string>();
                        List<string> listStoredQuery = new List<string>();
                        List<int> listTotal = new List<int>();
                        bool bStart = true;
                        bool bDoubleLine = false;
                        int countDouble = 0, loopCountDouble = 0;

                        for (int i = 1; i < lines.Length; i++)
                        {
                            count++;
                            if (count > 50 && !query.Contains("{"))
                            {
                                query = string.Format(@"Declare @IndentID int Declare @TotalIndent int " + execQuery);
                                result = ctx.Database.ExecuteSqlCommand(query) > 0;
                                query = @"Declare @IndentID int ";
                                count = 0;
                            }

                            if (!query.Contains("{") || bDoubleLine)
                            {
                                if (lines[i].Substring(0, 1) == "2")
                                {
                                    if (bDoubleLine)
                                    {
                                        stringQuery[loopCountDouble] += listStoredQuery[loopCountDouble];
                                        if (loopCountDouble == countDouble - 1)
                                            loopCountDouble = 0;
                                        else
                                            loopCountDouble += 1;
                                    }
                                    else
                                        query += storedQuery;
                                }
                                else
                                {
                                    totalQuota = 0;
                                    query += " if(select COUNT(*) from omMstIndent " +
                                                  "   where TypeCode = '{0}' and MarketModelCode = '{1}' and Variant = '{2}' and ModelYear = '{3}' and Year = '{4}' and Month = '{5}' and ColourCode = '{6}') > 0 " +
                                                  "  begin " +
                                                  "      UPDATE [omMstIndent] " +
                                                  "          SET [QuotaUnits] = '{8}', [UnitStatus] = '{10}', [ColourStatus] = '{11}' ,[isNeedReposting] = '1',[LastUpdateBy] = '{15}',[LastUpdateDate] = '{16}' " +
                                                  "      WHERE [TypeCode] = '{0}' and [MarketModelCode] = '{1}' and [Variant] = '{2}' and [ModelYear] = '{3}' and [Year] = '{4}' and [Month] = '{5}' and [ColourCode] = '{6}' " +
                                                  "      set @IndentID = (select TOP 1 IndentID from omMstIndent where TypeCode = '{0}' and MarketModelCode = '{1}' and Variant = '{2}' and ModelYear = '{3}' and Year = '{4}' and Month = '{5}' and ColourCode = '{6}') " +
                                                  "      set @TotalIndent = (select COUNT(*) from omHstIndent where IndentID = @IndentID) " +
                                        //"      INSERT INTO [omHstIndent]([IndentID],[LogSeq],[UnitStatus],[ColourStatus],[QuotaUnits],[LastUpdateBy],[LastUpdateDate],[CustomerID]) "+
                                        //"      VALUES(@IndentID, @TotalIndent,'{10}','{11}','{8}','{15}','{16}','') "+
                                                  "      INSERT INTO [omHstIndent]([IndentID],[LogSeq],[UnitStatus],[ColourStatus],[QuotaUnits],[LastUpdateBy],[LastUpdateDate]) " +
                                                  "      VALUES(@IndentID, @TotalIndent,'{10}','{11}','{8}','{15}','{16}') " +
                                                  "  end " +
                                              " else " +
                                                  "  begin" +
                                                       " INSERT INTO [omMstIndent]([TypeCode],[MarketModelCode],[Variant],[ModelYear],[Year],[Month],[ColourCode],[OriginalQuotaUnits],[QuotaUnits] " +
                                                       " ,[AllocationUnits],[UnitStatus],[ColourStatus],[isNeedReposting],[CreatedBy],[CreatedDate],[LastUpdateBy],[LastUpdateDate]) " +
                                                       " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','0','{13}','{14}','{15}','{16}') " +
                                                   " end ";
                                }
                            }
                            if (lines[i].Length == linesLength)
                            {
                                if (lines[i].Substring(0, 1) == "1")
                                {
                                    UploadBLL.SUADEDtl1File oSUADEDtl1File = new UploadBLL.SUADEDtl1File(lines[i]);
                                    if (oSUADEDtl1File != null)
                                    {
                                        query = query.Replace("{0}", oSUADEDtl1File.TypeCode);
                                        query = query.Replace("{1}", oSUADEDtl1File.MarketModelCode);
                                        query = query.Replace("{2}", oSUADEDtl1File.Variant);
                                        query = query.Replace("{3}", oSUADEDtl1File.ModelYear.ToString());
                                        query = query.Replace("{4}", oSUADEDtl1File.Year.ToString());
                                        query = query.Replace("{5}", oSUADEDtl1File.Month.ToString());
                                        query = query.Replace("{9}", "0");
                                        storedQuery = query;
                                        totalQuota = oSUADEDtl1File.TotalQuotaUnits;

                                        if (bStart)
                                        {
                                            stringQuery.Add(query);
                                            listStoredQuery.Add(query);
                                            listTotal.Add(totalQuota);
                                            bStart = bDoubleLine = false;
                                        }
                                        if (lines[i - 1].Substring(0, 1) == "1" && !bStart)
                                        {
                                            totalQuota = 0;
                                            string tempQuery = " if(select COUNT(*) from omMstIndent where TypeCode = '{0}' and MarketModelCode = '{1}' and Variant = '{2}' and ModelYear = '{3}' and Year = '{4}' and Month = '{5}' and ColourCode = '{6}' ) > 0 " +
                                                               "     begin " +
                                                               "         UPDATE [omMstIndent] " +
                                                               "         SET [QuotaUnits] = '{8}', [UnitStatus] = '{10}', [ColourStatus] = '{11}' ,[isNeedReposting] = '1',[LastUpdateBy] = '{15}',[LastUpdateDate] = '{16}' " +
                                                               "         WHERE [TypeCode] = '{0}' and [MarketModelCode] = '{1}' and [Variant] = '{2}' and [ModelYear] = '{3}' and [Year] = '{4}' and [Month] = '{5}' and [ColourCode] = '{6}' " +
                                                               "         set @IndentID = (select TOP 1 IndentID from omMstIndent where TypeCode = '{0}' and MarketModelCode = '{1}' and Variant = '{2}' and ModelYear = '{3}' and Year = '{4}' and Month = '{5}' and ColourCode = '{6}') " +
                                                               "         set @TotalIndent = (select COUNT(*) from omHstIndent where IndentID = @IndentID) " +
                                                //"         INSERT INTO [omHstIndent]([IndentID],[LogSeq],[UnitStatus],[ColourStatus],[QuotaUnits],[LastUpdateBy],[LastUpdateDate],[CustomerID]) "+
                                                //"         VALUES(@IndentID, @TotalIndent,'{10}','{11}','{8}','{15}','{16}','') "+
                                                               "         INSERT INTO [omHstIndent]([IndentID],[LogSeq],[UnitStatus],[ColourStatus],[QuotaUnits],[LastUpdateBy],[LastUpdateDate]) " +
                                                               "         VALUES(@IndentID, @TotalIndent,'{10}','{11}','{8}','{15}','{16}') " +
                                                               "     end " +
                                                               " else " +
                                                               "     begin " +
                                                               "     INSERT INTO [omMstIndent]([TypeCode],[MarketModelCode],[Variant],[ModelYear],[Year],[Month],[ColourCode],[OriginalQuotaUnits],[QuotaUnits] " +
                                                               "     ,[AllocationUnits],[UnitStatus],[ColourStatus],[isNeedReposting],[CreatedBy],[CreatedDate],[LastUpdateBy],[LastUpdateDate]) " +
                                                               "     VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','0','{13}','{14}','{15}','{16}') " +
                                                               " end ";

                                            tempQuery = tempQuery.Replace("{0}", oSUADEDtl1File.TypeCode);
                                            tempQuery = tempQuery.Replace("{1}", oSUADEDtl1File.MarketModelCode);
                                            tempQuery = tempQuery.Replace("{2}", oSUADEDtl1File.Variant);
                                            tempQuery = tempQuery.Replace("{3}", oSUADEDtl1File.ModelYear.ToString());
                                            tempQuery = tempQuery.Replace("{4}", oSUADEDtl1File.Year.ToString());
                                            tempQuery = tempQuery.Replace("{5}", oSUADEDtl1File.Month.ToString());
                                            tempQuery = tempQuery.Replace("{9}", "0");

                                            query += tempQuery;

                                            stringQuery.Add(tempQuery);
                                            listStoredQuery.Add(tempQuery);
                                            listTotal.Add(totalQuota);
                                            bDoubleLine = true;
                                            countDouble = stringQuery.Count;
                                        }
                                        if (bDoubleLine) query = string.Empty;
                                    }
                                }
                                else if (lines[i].Substring(0, 1) == "2")
                                {
                                    UploadBLL.SUADEDtl2File oSUADEDtl2File = new UploadBLL.SUADEDtl2File(lines[i]);
                                    if (oSUADEDtl2File != null)
                                    {
                                        if (bDoubleLine && loopCountDouble < countDouble)
                                        {
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{6}", oSUADEDtl2File.ColourCode);
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{7}", oSUADEDtl2File.ColourStatus == "F" ? listTotal[loopCountDouble].ToString() : oSUADEDtl2File.QuotaUnits.ToString());
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{8}", oSUADEDtl2File.ColourStatus == "F" ? listTotal[loopCountDouble].ToString() : oSUADEDtl2File.QuotaUnits.ToString());
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{10}", oSUADEDtl2File.UnitStatus);
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{11}", oSUADEDtl2File.ColourStatus.ToString());
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{13}", user.UserId);
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{14}", DateTime.Now.ToString("yyyyMMdd"));
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{15}", user.UserId);
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{16}", DateTime.Now.ToString("yyyyMMdd"));
                                            execQuery += stringQuery[loopCountDouble];
                                            stringQuery[loopCountDouble] = string.Empty;
                                            bStart = true;
                                        }
                                        else
                                        {
                                            query = query.Replace("{6}", oSUADEDtl2File.ColourCode);
                                            query = query.Replace("{7}", oSUADEDtl2File.ColourStatus == "F" ? totalQuota.ToString() : oSUADEDtl2File.QuotaUnits.ToString());
                                            query = query.Replace("{8}", oSUADEDtl2File.ColourStatus == "F" ? totalQuota.ToString() : oSUADEDtl2File.QuotaUnits.ToString());
                                            query = query.Replace("{10}", oSUADEDtl2File.UnitStatus);
                                            query = query.Replace("{11}", oSUADEDtl2File.ColourStatus.ToString());
                                            query = query.Replace("{13}", user.UserId);
                                            query = query.Replace("{14}", DateTime.Now.ToString("yyyyMMdd"));
                                            query = query.Replace("{15}", user.UserId);
                                            query = query.Replace("{16}", DateTime.Now.ToString("yyyyMMdd"));
                                            execQuery += query;
                                            query = string.Empty;
                                            bStart = true;
                                            stringQuery.Clear();
                                            listTotal.Clear();
                                            listStoredQuery.Clear();
                                        }
                                    }
                                }
                            }
                        }

                        query = string.Format(@"Declare @IndentID int Declare @TotalIndent int " + execQuery);
                        //untuk test di remark dulu
                        result = ctx.Database.ExecuteSqlCommand(query) > 0;
                        sb = new StringBuilder();
                        query = string.Empty;
                        count = 0;
                    }
                    else
                    {
                        msg = "flat file tidak valid";
                        return false;
                    }

                }
            }
            else
            {
                msg = "flat file tidak valid";
                return false;
            }

            return result;
        }

        private bool UploadDataSFPDALocal(string[] lines)
        {
            var dcsHelper = new DcsHelper(CurrentUser.UserId);
            var linesCount = 209;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            MessageDcs.Clear();

            if (lines.Count() <= 0) return false;
            if (lines[0].Count() != linesCount) return false;
            if (lines[0].Substring(0, 1) != "H") return false;         //Record ID
            if (lines[0].Substring(1, 5) != "SFPDA") return false;     //Data ID
            if (int.Parse(lines[0].Substring(16, 10)) != 2000000) return false; //Received Dealer Code
            if (lines[0].Substring(98, 1) != "B") return false;        //Product Type
            if (lines[1].Count() != linesCount) return false;
            if (lines[2].Count() != linesCount) return false;
            if (lines[3].Count() != linesCount) return false;
            if (lines[4].Count() != linesCount) return false;

            var noFaktur = lines[1].Substring(1, 15);
            var record = ctx.omFakturPoldas.FirstOrDefault(x => x.NoFaktur == noFaktur);
            if (record != null) ctx.omFakturPoldas.Remove(record);
            var newRecord = new omFakturPolda
            {
                DealerCode = lines[0].Substring(6, 10),
                DealerName = lines[0].Substring(26, 50),
                NoFaktur = noFaktur,
                TglFaktur = new DateTime(int.Parse(lines[1].Substring(16, 4)),
                    int.Parse(lines[1].Substring(20, 2)), int.Parse(lines[1].Substring(22, 2))),
                Merk = lines[1].Substring(24, 20),
                Tipe = lines[1].Substring(44, 50),
                ThnPembuatan = decimal.Parse(lines[1].Substring(94, 4)),
                ThnPerakitan = decimal.Parse(lines[1].Substring(99, 4)),
                Silinder = lines[1].Substring(102, 15),
                Warna = lines[1].Substring(117, 30),
                NoRangka = lines[1].Substring(147, 20),
                NoMesin = lines[1].Substring(167, 20),
                BahanBakar = lines[1].Substring(187, 10),
                Pemilik = lines[2].Substring(1, 50),
                Pemilik2 = lines[2].Substring(51, 50),
                Alamat = lines[2].Substring(101, 100),
                Alamat2 = lines[3].Substring(1, 100),
                Alamat3 = lines[3].Substring(101, 100),
                JenisKendaraan = lines[4].Substring(1, 20),
                NoFormA = lines[4].Substring(21, 25),
                TglFormA = new DateTime(int.Parse(lines[4].Substring(46, 4)),
                    int.Parse(lines[4].Substring(50, 2)), int.Parse(lines[4].Substring(52, 2))),
                NoKTP = lines[4].Substring(54, 20),
                NoTelp = lines[4].Substring(74, 15),
                NoHP = lines[4].Substring(89, 15),
                NoPIB = lines[4].Substring(104, 15),
                NoSUT = lines[4].Substring(119, 30),
                NoTPT = lines[4].Substring(149, 30),
                NoSRUT = lines[4].Substring(179, 30),
                IsActive = true,
                CreatedBy = CurrentUser.UserId,
                LastUpdateBy = CurrentUser.UserId,
                CreatedDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            };
            ctx.omFakturPoldas.Add(newRecord);
            ctx.SaveChanges();
            return true;
        }

        #endregion

        #region *** support ssjal ***
        public DataRow GetBPUNobyReffFJ(string companyCode, string branchCode, string reffSJNo)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format(@" select TOP 1 * from dbo.omTrPurchaseBPU a
                                            left join omTrPurchaseBPUDetail b on a.CompanyCode = b.CompanyCode
                                            and a.BranchCode = b.BranchCode
                                            and a.PONO = b.PONo
                                            and a.BPUNo = b.BPUNo
                                            where a.CompanyCode = '{0}'
                                            and a.BranchCode ='{1}'
                                            and a.RefferenceSJNo ='{2}'
                                            and a.Status <> '3'
                                            and b.BPUSeq = 1",
                                            companyCode, branchCode, reffSJNo);

            SqlDataAdapter sa = new SqlDataAdapter(cmd);
            sa.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                dr = dt.Rows[0];
                return dr;
            }


            //var dtModel = from o in(
            //                    from p in ctx.omTrPurchaseBPU
            //                    join q in ctx.omTrPurchaseBPUDetail
            //                        on new { p.CompanyCode, p.BranchCode, p.PONo, p.BPUNo } equals new { q.CompanyCode, q.BranchCode, q.PONo, q.BPUNo }
            //                    where
            //                    p.CompanyCode == companyCode &&
            //                    p.BranchCode == branchCode &&
            //                    p.RefferenceSJNo == reffSJNo &&
            //                    p.Status != "3" &&
            //                    q.BPUSeq == 1
            //                    group new {p,q} by p.RefferenceSJNo into view
            //                    select view.FirstOrDefault()
            //              )
            //              select new OmTrPurchaseBPUDetailView
            //              {
            //                  BPUNo=o.p.BPUNo,
            //                  PONo=o.p.PONo,
            //                  ChassisCode=o.q.ChassisCode,
            //                  ChassisNo=o.q.ChassisNo.ToString()
            //              };

        }
        #endregion

        #region *** support sfpo1 & sfpo2 ***
        private DataRow SelectDetailVehicle(string CompanyCode, string ChassisCode, decimal ChassisNo)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = string.Format(@"select 
                                                a.chassisCode, a.ChassisNo, a.SalesModelCode, a.SalesModelYear, a.ColourCode, 
                                                a.EngineCode, a.EngineNo,c.RefferenceDONo, c.RefferenceSJNo, isnull(e.ReqNo,'') ReqNo
                                              from omMstVehicle a 
                                              inner join omTrPurchaseBPUDetail b on  a.companyCode = b.companyCode and a.chassisCode = b.chassisCode and a. chassisNo = b.chassisNo
	                                          inner join omTrPurchaseBPU c on b.companyCode = c.companyCode and b.branchCode = c.branchCode and b.BPUNo = c.BPUNo
	                                          left join omTrSalesReqDetail e on b.companyCode = e.companyCode and b.chassisCode = e.chassisCode and b.chassisNo = e.chassisNo
                                              where a.companyCode = '{0}' and a.chassisCode = '{1}' and a.ChassisNo = '{2}'",
                                              CompanyCode, ChassisCode, ChassisNo);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                dr = null;
            }
            else
            {
                dr = dt.Rows[0];
            }

            return dr;
        }

        private IQueryable<SalesReqDetailView> GetReqDetailRecord(string CompanyCode, string BranchCode, string ChassisCode, decimal ChassisNo)
        {
            var query = from p in ctx.omTrSalesReqDetail
                        where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ChassisCode == ChassisCode && p.ChassisNo == ChassisNo
                        select new SalesReqDetailView
                        {
                            ReqNo = p.ReqNo,
                            ChassisCode = p.ChassisCode,
                            ChassisNo = p.ChassisNo,
                            SONo = p.SONo,
                            BPKNo = p.BPKNo
                        };
            int q = query.ToList().Count;

            if (q==0)
            {
                query = null;
            }
            return query;

        }

        #endregion

        #region *** support sfplb ***

        private IQueryable<omUtlFpolReqView> GetDatabyChassisCodeAndChassisNo(string companyCode, string chassisCode, decimal chassisNo)
        {
            var query = from p in ctx.OmUtlFPolReqDetails
                        where p.CompanyCode == companyCode && p.ChassisCode == chassisCode && p.ChassisNo == chassisNo
                        select new omUtlFpolReqView
                        {
                            BatchNo = p.BatchNo
                        };

            return query;
        }

        private int CountSalesInvoicebyChassisCodeAndNo(string companyCode, string branchCode, string chassisCode, decimal chassisNo)
        {
            int y = 0;
            var dtModel = ctx.omTrSalesInvoiceVin
                            .Where(p => p.CompanyCode == companyCode && p.BranchCode == branchCode
                                && p.ChassisCode == chassisCode
                                && p.ChassisNo == chassisNo).ToList();
            if (dtModel.Count >= 0)
            {
                y = 1;
            }
            else y = 0;

            return y;
        }

        #endregion

        #region -- Private Method --
        private bool DcsValidated()
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
        #endregion

        //public JsonResult Test(string DataID, DateTime DateFrom, DateTime DateTo, bool AllStatus)
        //{
        //    var rows = ctx.GnDcsUploadFiles.Where(x => x.DataID == DataID);


        //    return Json(new { data = rows });
        //}

        public JsonResult CheckSFPDA(string Id, string contents)
        {
            try
            {
                var dcsHelper = new DcsHelper(CurrentUser.UserId);
                var UploadItem = ctx.GnDcsUploadFiles.Find(decimal.Parse(Id));
                string check = contents.Replace("\r\n", "\n");
                var lines = check.Split('\n');
                var noFaktur = lines[1].Substring(1, 15);
                var existed = ctx.omFakturPoldas.FirstOrDefault(x => x.NoFaktur == noFaktur);
                return Json(new { message = "", isExist = existed != null });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }

        }
    }
}