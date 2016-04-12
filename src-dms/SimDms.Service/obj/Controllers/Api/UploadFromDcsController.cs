using SimDms.Service.Models;
using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common.DcsWs;
using System.Data;
using SimDms.Common.Models;
using System.Data.SqlClient;
using Newtonsoft.Json;


namespace SimDms.Service.Controllers.Api
{
    public class UploadFromDcsController : BaseController
    {
        private UploadTypeDCS uploadType;
        private int mxLenght = 0;
        private string msg = "";
        public JsonResult Default()
        {
            var dateFrom = Helpers.StartOfMonth();
            var dateTo = Helpers.EndOfMonth();

            Dictionary<string, string> dicDataID = new Dictionary<string, string>();
            dicDataID.Add("Campaign", "WCAMP");
            dicDataID.Add("Flat Rate", "WFRAT");
            dicDataID.Add("Judgement Code", "WJUDG");
            dicDataID.Add("PDI & FSC Amount", "WPDFS");
            dicDataID.Add("Section Code", "WSECT");
            dicDataID.Add("Trouble Code", "WTROB");
            dicDataID.Add("Warranty", "WWRNT");

            var dataID = dicDataID.Select(p => new
            {
                text = p.Key,
                value = p.Value
            });

            return Json(new { DateFrom = dateFrom, DateTo = dateTo, dsDataID = dataID, IsOnline = DcsValidated() });
        }

        private class ComboData
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        public JsonResult RetrieveData(string DataID, DateTime DateFrom, DateTime DateTo, bool AllStatus)
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

        #region *** Process upload ***
        public JsonResult UploadData(string Id, string contents)
        {
            DcsHelper dcsHelper = new DcsHelper(CurrentUser.UserId);
            var UploadItem = ctx.GnDcsUploadFiles.Find(decimal.Parse(Id));
            bool result = false;
            string check = contents.Replace('\r', '\n');
            var lines = check.Split('\n');

            try
            {
                #region SERVICE FLAT FILE
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.WCAMP))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.WCAMP;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.WFRAT))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.WFRAT;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.WJUDG))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.WJUDG;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.WPDFS))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.WPDFS;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.WSECT))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.WSECT;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.WTROB))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.WTROB;
                    }
                }
                if (lines != null && dcsHelper.Validate(lines, UploadTypeDCS.WWRNT))
                {
                    if (UploadItem != null)
                    {
                        if (UploadItem.Status.Equals("P"))
                        {
                            throw new ArgumentException("Data sudah pernah di upload....!");
                        }
                        uploadType = UploadTypeDCS.WWRNT;
                    }
                }
                #endregion

                result = ProccessUpload(uploadType, lines, Id);
            }
            catch (Exception ex)
            {
                result = false;
                return Json(new { success = result, message = ex.Message });
            }

            return Json(new { success = result, message = msg });

        }

        private bool ProccessUpload(UploadTypeDCS uploadType, string[] lines, string ID)
        {
            DcsHelper dcsHelper = new DcsHelper(CurrentUser.UserId);
            //var UploadItem = ctx.GnDcsUploadFiles.Find(decimal.Parse(DataId));

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
                        int hasilUpdateStat = UpdateUploadDataStatusCtx(long.Parse(ID.ToString()), success);
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
                if (lines != null && dcsHelper.Validate(lines, uploadType))
                {
                    //success = ProcessUploadDataTuning(lines, uploadType, long.Parse(DataId.ToString()));

                    if (success)
                    {
                        int hasilUpdateStat = UpdateUploadDataStatusCtx(long.Parse(ID.ToString()), success);
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
                // REGION : Service

                case UploadTypeDCS.WCAMP:
                    return UploadDataWCAMPLocal(lines);
                case UploadTypeDCS.WJUDG:
                    return UploadDataWJUDGLocal(lines);
                case UploadTypeDCS.WPDFS:
                    return UploadDataWPDFSLocal(lines);
                case UploadTypeDCS.WSECT:
                    return UploadDataWSECTLocal(lines);
                case UploadTypeDCS.WWRNT:
                    return UploadDataWWRNTLocal(lines);
                case UploadTypeDCS.WTROB:
                    return UploadDataWTROBLocal(lines);
                case UploadTypeDCS.WFRAT:
                    return UploadDataWFRATLocal(lines);
                //case UploadTypeDCS.WSMRR:
                //    return UploadDataWSMRRLocal(ctx, lines);

                default:
                    return false;
            }
        }

        private bool UploadDataWCAMPLocal(string[] lines)
        {
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);

            if (maxLinesLast == 0)
            {
                maxLine = maxLine - 1;
            }

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return false;
            }

            string headerText = lines[0];
            string query = "";

            try
            {
                string ccode = user.CompanyCode;
                string bcode = user.BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = user.UserId;

                int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

                CoProfile oGnMstCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oGnMstCoProfile.CompanyCode != headerText.Substring(16, 10).Trim()
                    && headerText.Substring(16, 10).Trim() != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }
                LookUpDtl oGnMstLookUpDtl = ctx.LookUpDtls.Find(ccode, GnMstLookUpHdr.ProductType,
                    oGnMstCoProfile.ProductType);
                if (oGnMstLookUpDtl.ParaValue != headerText.Substring(82, 1).Trim())
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }

                query += string.Format("INSERT INTO SvUtlCampaign (CompanyCode, ProcessDate, DealerCode, ReceivedDealerCode," +
                                        " DealerName, TotalNoOfItem, ProductType, CreatedBy, CreatedDate)" +
                                        " VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}')",
                                        ccode, cdate, headerText.Substring(6, 10).Trim(), headerText.Substring(16, 10).Trim()
                                        , headerText.Substring(26, 50).Trim(), Convert.ToDecimal(headerText.Substring(76, 6).Trim())
                                        , headerText.Substring(82, 1).Trim(), uid, cdate);

                if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                {
                    result = false;
                    msg = "Process upload gagal";
                    return result;
                }

                query = string.Empty;
                for (int i = 1; i < maxLine; i++)
                {
                    string detailText = lines[i];

                    query += string.Format("INSERT INTO SvUtlCampaignRange (CompanyCode, ProcessDate, SeqNo, ComplainCode, " +
                                            " DefectCode, ChassisCode, ChassisStartNo, ChassisEndNo, CloseDate, TaskCode, Description)" +
                                            " VALUES ('{0}','{1}',{2},'{3}','{4}','{5}',{6},{7},'{8}','{9}','{10}') ",
                                            ccode, cdate, i, detailText.Substring(1, 2).Trim(), detailText.Substring(3, 2).Trim()
                                            , detailText.Substring(5, 11).Trim(), Convert.ToDecimal(detailText.Substring(16, 6).Trim())
                                            , Convert.ToDecimal(detailText.Substring(22, 6).Trim()), Convert.ToDecimal(detailText.Substring(28, 8).Trim())
                                            , detailText.Substring(36, 6).Trim(), detailText.Substring(42, 100).Trim());

                    if (!(GetRecordCampaign(user.CompanyCode, user.CoProfile.ProductType, detailText.Substring(1, 2).Trim(), detailText.Substring(3, 2).Trim()
                        , detailText.Substring(5, 11).Trim(), Convert.ToDecimal(detailText.Substring(16, 6).Trim()), Convert.ToDecimal(detailText.Substring(22, 6).Trim()))))
                    {
                        query += string.Format("INSERT INTO SvMstCampaign (CompanyCode, ProductType, ComplainCode, DefectCode, ChassisCode, ChassisStartNo, " +
                                               " ChassisEndNo, IsActive, isLocked, OperationNo, CloseDate, Description ,CreatedBy, CreatedDate, LastUpdateBy," +
                                               " LastUpdateDate) VALUES ('{0}','{1}','{2}','{3}','{4}',{5},{6},'{7}','{8}','{9}','{10}','{11}','{12}','{13}', '{14}', '{15}') ",
                                               ccode, user.CoProfile.ProductType, detailText.Substring(1, 2).Trim(), detailText.Substring(3, 2).Trim(),
                                               detailText.Substring(5, 11).Trim(), Convert.ToDecimal(detailText.Substring(16, 6).Trim()), Convert.ToDecimal(detailText.Substring(22, 6).Trim())
                                               , "1", "0", detailText.Substring(36, 6).Trim(), Convert.ToDecimal(detailText.Substring(28, 8).Trim()), detailText.Substring(42, 100).Trim(), uid, cdate, uid, cdate);
                    }
                    else
                    {
                        query += string.Format("UPDATE SvMstCampaign SET OperationNo = '{7}', CloseDate = '{8}', Description = '{9}', LastUpdateBy = '{10}'," +
                                                " LastUpdateDate = '{11}' WHERE CompanyCode = '{0}' AND ProductType = '{1}' AND ComplainCode = '{2}' AND " +
                                                " DefectCode = '{3}' AND ChassisCode = '{4}' AND ChassisStartNo = {5} AND ChassisEndNo = {6} ",
                                                ccode, user.CoProfile.ProductType, detailText.Substring(1, 2).Trim(), detailText.Substring(3, 2).Trim(),
                                                detailText.Substring(5, 11).Trim(), Convert.ToDecimal(detailText.Substring(16, 6).Trim()), Convert.ToDecimal(detailText.Substring(22, 6).Trim()),
                                                detailText.Substring(36, 6).Trim(), Convert.ToDecimal(detailText.Substring(28, 8).Trim()), detailText.Substring(42, 100).Trim(), uid, cdate);
                    }

                    if ((i % 50) == 0)
                    {
                        if (ctx.Database.ExecuteSqlCommand(query) > 0)
                            query = string.Empty;
                        else
                        {
                            result = false;
                            return result;
                        }
                    }
                    result = true;
                }
                if (!(string.IsNullOrEmpty(query)))
                {
                    if (ctx.Database.ExecuteSqlCommand(query) > 0)
                        query = string.Empty;
                    else
                    {
                        result = false;
                        msg = "Process upload gagal";
                        return result;
                    }
                }

                if (result)
                {
                    SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                    cmd.CommandText = "SELECT COUNT (*) FROM SvUtlCampaignRange";
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    decimal decCount = Convert.ToDecimal(dt.Rows[0][0]);
                    if (decCount != Convert.ToDecimal(headerText.Substring(76, 6).Trim()))
                    {
                        msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                        result = false;
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {

                msg = ex.Message;
                result = false;
            }


            return result;
        }

        private bool UploadDataWJUDGLocal(string[] lines)
        {
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            string headerText = lines[0];


            if (maxLinesLast == 0)
            {
                maxLine = maxLine - 1;
            }

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return false;
            }

            try
            {
                string ccode = user.CompanyCode;
                string productType = user.CoProfile.ProductType;
                string bcode = user.BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = user.UserId;
                string query = string.Empty;

                query += string.Format("DELETE SvUtlJudgementDescription");
                int flag = ctx.Database.ExecuteSqlCommand(query);
                query = string.Empty;

                int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

                CoProfile oGnMstCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oGnMstCoProfile.CompanyCode != headerText.Substring(16, 10).Trim()
                    && headerText.Substring(16, 10).Trim() != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result = false;
                }

                query += string.Format("INSERT INTO SvUtlJudgementCode (Companycode, ProcessDate, Dealercode, ReceivedDealerCode," +
                                       " DealerName, TotalNoOfItem, ProductType, CreatedBy, CreatedDate)" +
                                       " VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}') ",
                                       ccode, cdate, headerText.Substring(6, 10).Trim(), headerText.Substring(16, 10).Trim()
                                       , headerText.Substring(26, 50).Trim(), Convert.ToDecimal(headerText.Substring(76, 6).Trim())
                                       , headerText.Substring(82, 1).Trim(), uid, cdate);

                if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                {
                    result = false;
                    return result;
                }

                query = string.Empty;
                for (int i = 1; i < maxLine; i++)
                {
                    string detailText = lines[i];
                    if (string.IsNullOrEmpty(detailText))
                    {
                        break;
                    }

                    query += string.Format("INSERT INTO SvUtlJudgementDescription (CompanyCode, ProcessDate, SeqNo, JudgementCode, Description, " +
                                            " DescriptionEng) VALUES ('{0}','{1}',{2},'{3}','{4}','{5}') ",
                                            ccode, cdate, i, detailText.Substring(1, 4).Trim(), detailText.Substring(5, 100).Trim()
                                           , detailText.Substring(105, 100).Trim());

                    if (GetRecordRefference(ccode, productType, "JUDGECOD", detailText.Substring(1, 4).Trim()))
                    {
                        query += string.Format("UPDATE SvMstRefferenceService SET Description = '{4}', DescriptionEng = '{5}', LastUpdateBy = '{6}', LastUpdateDate = '{7}'" +
                                               " WHERE CompanyCode = '{0}' AND ProductType = '{1}' AND RefferenceType = '{2}' AND RefferenceCode = '{3}' ",
                                               ccode, productType, "JUDGECOD", detailText.Substring(1, 4).Trim(), detailText.Substring(1, 4).Trim(),
                                               detailText.Substring(5, 100).Trim(), uid, cdate);
                    }
                    else
                    {
                        query += string.Format("INSERT INTO SvMstRefferenceService (CompanyCode, ProductType, RefferenceType, RefferenceCode, Description," +
                                               " DescriptionEng, Createdby, Createddate, LastUpdateBy, LastUpdateDate, IsLocked, IsActive)" +
                                               " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}', '{11}') ",
                                               ccode, productType, "JUDGECOD", detailText.Substring(1, 4).Trim(), detailText.Substring(5, 100).Trim()
                                               , detailText.Substring(105, 100).Trim(), uid, cdate, uid, cdate, "0", "1");
                    }

                    if ((i % 10) == 0)
                    {

                        if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                        {
                            result = false;
                            return result;
                        }

                        query = string.Empty;
                    }
                    result = true;
                }

                if (result)
                {
                    SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                    cmd.CommandText = "SELECT COUNT (*) FROM SvUtlJudgementDescription";
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    decimal decCount = Convert.ToDecimal(dt.Rows[0][0]);
                    if (decCount != Convert.ToDecimal(headerText.Substring(76, 6).Trim()))
                    {
                        msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                        result = false;
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }

            return result;
        }

        private bool UploadDataWPDFSLocal(string[] lines)
        {
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            string headerText = lines[0];

            if (maxLinesLast == 0)
            {
                maxLine = maxLine - 1;
            }

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return false;
            }

            try
            {
                string ccode = user.CompanyCode;
                string bcode = user.BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = user.UserId;
                string query = string.Empty;
                int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

                CoProfile oGnMstCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oGnMstCoProfile.CompanyCode != headerText.Substring(16, 10).Trim()
                   && headerText.Substring(16, 10).Trim() != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }
                LookUpDtl oGnMstLookUpDtl = ctx.LookUpDtls.Find(ccode, GnMstLookUpHdr.ProductType,
                    oGnMstCoProfile.ProductType);
                if (oGnMstLookUpDtl.ParaValue != headerText.Substring(82, 1).Trim())
                {
                    msg = "Product Type tidak sesuai dengan Company Profile";
                    return result;
                }
                var dealerArea = ctx.LookUpDtls.Find(ccode, GnMstLookUpHdr.DealerArea, GnMstLookUpHdr.DealerArea);
                if (oGnMstLookUpDtl.ParaValue != headerText.Substring(82, 1).Trim())
                {
                    msg = "Product Type tidak sesuai dengan Company Profile";
                    return result;
                }

                query += string.Format("INSERT INTO SvUtlPdiFsc (CompanyCode, ProcessDate, DealerCode, ReceivedDealerCode, DealerName," +
                                       " TotalNoOfItem, ProductType, CreatedBy, CreatedDate)" +
                                       " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}') ",
                                       ccode, cdate, headerText.Substring(6, 10).Trim(), headerText.Substring(16, 10).Trim(),
                                       headerText.Substring(26, 50).Trim(), Convert.ToDecimal(headerText.Substring(76, 6).Trim()),
                                       headerText.Substring(82, 1).Trim(), uid, cdate);

                if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                {
                    result = false;
                    return result;
                }
                query = string.Empty;

                for (int i = 1; i < maxLine; i++)
                {
                    string detailText = lines[i];

                    if (Convert.ToString(dealerArea) == detailText.Substring(126, 5).Trim())
                    {
                        query += string.Format("INSERT INTO SvUtlPdiFscAmount (CompanyCode, ProcessDate, SeqNo, BasicModel, TransmissionType, PdiFsc, RegularLaborAmount," +
                                               " RegularMaterialAmount,RegularTotalAmount, CampaignLaborAmount, CampaignMaterialAmount, CampaignTotalAmount, EffectiveDate, Description)" +
                                               " VALUES ('{0}','{1}',{2},'{3}','{4}',{5},{6},{7},{8},{9},{10},{11},'{12}','{13}') ",
                                               ccode, cdate, i, detailText.Substring(1, 15).Trim(), detailText.Substring(16, 2).Trim(), detailText.Substring(18, 2).Trim(),
                                               Convert.ToDecimal(detailText.Substring(20, 8).Trim()), Convert.ToDecimal(detailText.Substring(28, 8).Trim()), Convert.ToDecimal(detailText.Substring(36, 8).Trim())
                                               , Convert.ToDecimal(detailText.Substring(44, 8).Trim()), Convert.ToDecimal(detailText.Substring(52, 8).Trim()), Convert.ToDecimal(detailText.Substring(60, 8).Trim()),
                                               Convert.ToDateTime(detailText.Substring(68, 8).Trim()), detailText.Substring(76, 50).Trim());

                        #region === Insert/Update PdiFsc Campaign ===

                        if (GetRecordPdiFsc(ccode, user.CoProfile.ProductType, detailText.Substring(1, 15).Trim(), "1", detailText.Substring(16, 2).Trim()
                               , Convert.ToDecimal(detailText.Substring(18, 2).Trim()), Convert.ToDateTime(detailText.Substring(68, 8).Trim())))
                        {
                            query += string.Format("UPDATE SvMstPdiFscRate SET Description = '{7}' , RegularLaborAmount = {8} , RegularMaterialAmount = {9} , " +
                                                   " RegularTotalAmount = {10} , LastUpdateBy = '{11}'  , LastUpdateDate = '{12}' WHERE CompanyCode = '{0}' AND " +
                                                   " ProductType = '{1}' AND BasicModel = '{2}' AND isCampaign = '{3}' AND TransmissionType = '{4}' AND PdiFscSeq = {5} AND EffectiveDate = '{6}' ",
                                                   ccode, user.CoProfile.ProductType, detailText.Substring(1, 15).Trim(), "1", detailText.Substring(16, 2).Trim()
                                                   , Convert.ToDecimal(detailText.Substring(18, 2).Trim()), Convert.ToDateTime(detailText.Substring(68, 8).Trim()),
                                                   detailText.Substring(76, 50).Trim(), Convert.ToDecimal(detailText.Substring(44, 8).Trim()), Convert.ToDecimal(detailText.Substring(52, 8).Trim())
                                                   , Convert.ToDecimal(detailText.Substring(60, 8).Trim()), uid, cdate);
                        }
                        else
                        {
                            query += string.Format("INSERT INTO SvMstPdiFscRate (CompanyCode, ProductType, BasicModel, IsCampaign, TransmissionType, PdiFscSeq," +
                                                   " EffectiveDate,Description, RegularLaborAmount, RegularMaterialAmount, RegularTotalAmount, IsActive, IsLocked, CreatedBy, CreatedDate)" +
                                                   " VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}',{8},{9},{10},'{11}','{12}','{13}', '{14}') ",
                                                   ccode, user.CoProfile.ProductType, detailText.Substring(1, 15).Trim(), "1", detailText.Substring(16, 2).Trim(),
                                                   Convert.ToDecimal(detailText.Substring(18, 2).Trim()), Convert.ToDateTime(detailText.Substring(68, 8).Trim()),
                                                   detailText.Substring(76, 50).Trim(), Convert.ToDecimal(detailText.Substring(44, 8).Trim()),
                                                   Convert.ToDecimal(detailText.Substring(52, 8).Trim()), Convert.ToDecimal(detailText.Substring(60, 8).Trim()), "1", "0", uid, cdate);
                        }
                        #endregion

                        #region === Insert/Update PdiFsc Regular ===
                        if (GetRecordPdiFsc(ccode, user.CoProfile.ProductType, detailText.Substring(1, 15).Trim(), "0", detailText.Substring(16, 2).Trim()
                             , Convert.ToDecimal(detailText.Substring(18, 2).Trim()), Convert.ToDateTime(detailText.Substring(68, 8).Trim())))
                        {
                            query += string.Format("UPDATE SvMstPdiFscRate SET Description = '{7}' , RegularLaborAmount = {8} , RegularMaterialAmount = {9} , RegularTotalAmount = {10} , LastUpdateBy = '{11}' , LastUpdateDate = '{12}' " +
                                                   " WHERE CompanyCode = '{0}' AND ProductType = '{1}' AND BasicModel = '{2}' AND isCampaign = '{3}' AND TransmissionType = '{4}' AND PdiFscSeq = {5} AND EffectiveDate = '{6}' ",
                                                   ccode, user.CoProfile.ProductType, detailText.Substring(1, 15).Trim(), "1", detailText.Substring(16, 2).Trim(), Convert.ToDecimal(detailText.Substring(18, 2).Trim()),
                                                   Convert.ToDateTime(detailText.Substring(68, 8).Trim()), detailText.Substring(76, 50).Trim(), Convert.ToDecimal(detailText.Substring(20, 8).Trim()),
                                                   Convert.ToDecimal(detailText.Substring(28, 8).Trim()), Convert.ToDecimal(detailText.Substring(36, 8).Trim()), uid, cdate);
                        }
                        else
                        {
                            query += string.Format("INSERT INTO SvMstPdiFscRate (CompanyCode, ProductType, BasicModel, IsCampaign, TransmissionType, PdiFscSeq, EffectiveDate,Description, RegularLaborAmount, RegularMaterialAmount," +
                                                   " RegularTotalAmount, IsActive, IsLocked, CreatedBy, CreatedDate) VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}',{8},{9},'{10}','{11}','{12}','{13}', '{14}') ",
                                                   ccode, user.CoProfile.ProductType, detailText.Substring(1, 15).Trim(), "0", detailText.Substring(16, 2).Trim(), Convert.ToDecimal(detailText.Substring(18, 2).Trim())
                                                   , Convert.ToDateTime(detailText.Substring(68, 8).Trim()), detailText.Substring(76, 50).Trim(), Convert.ToDecimal(detailText.Substring(20, 8).Trim()),
                                                   Convert.ToDecimal(detailText.Substring(28, 8).Trim()), Convert.ToDecimal(detailText.Substring(36, 8).Trim()), "1", "0", uid, cdate);
                        }
                        #endregion
                    }

                    if ((i % 20) == 0)
                    {
                        if (query != string.Empty)
                        {
                            if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                            {
                                result = false;
                                return result;
                            }
                        }
                        query = string.Empty;
                    }
                }
                if (!(string.IsNullOrEmpty(query)))
                {
                    if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                    {
                        result = false;
                        return result;
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }

            return result;
        }

        private bool UploadDataWSECTLocal(string[] lines)
        {
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            string query = string.Empty;
            string headerText = lines[0];

            if (maxLinesLast == 0)
            {
                maxLine = maxLine - 1;
            }

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return false;
            }

            try
            {
                string ccode = user.CompanyCode;
                string productType = user.CoProfile.ProductType;
                string bcode = user.BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = user.UserId;
                int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

                CoProfile oGnMstCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oGnMstCoProfile.CompanyCode != headerText.Substring(16, 10).Trim()
                   && headerText.Substring(16, 10).Trim() != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }
                query += string.Format("INSERT INTO svUtlSectionCode (CompanyCode, ProcessDate, DealerCode, ReceivedDealerCode, DealerName, " +
                                       " TotalNoOfItem, ProductType, CreatedBy, CreatedDate) VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}', '{8}') ",
                                       ccode, cdate, headerText.Substring(6, 10).Trim(), headerText.Substring(16, 10).Trim(), headerText.Substring(26, 50).Trim(),
                                       Convert.ToDecimal(headerText.Substring(76, 6).Trim()), user.CoProfile.ProductType, uid, cdate);

                if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                {
                    result = false;
                    return result;
                }
                query = string.Empty;
                for (int i = 1; i < maxLine; i++)
                {
                    string detailText = lines[i];

                    query += string.Format("INSERT INTO SvUtlSectionDescription (CompanyCode, ProcessDate, SeqNo, SectionCode, Description)" +
                                           " VALUES ('{0}','{1}',{2},'{3}','{4}') ",
                                           ccode, cdate, i, detailText.Substring(1, 2).Trim(), detailText.Substring(3, 80).Trim());

                    if (GetRecordRefference(ccode, user.CoProfile.ProductType, "SECTIOCD", detailText.Substring(1, 2).Trim()))
                    {
                        query += string.Format("UPDATE SvMstRefferenceService SET Description = '{4}', DescriptionEng = '{4}'," +
                                               " LastUpdateBy = '{5}' , LastUpdateDate = '{6}' WHERE CompanyCode = '{0}' AND ProductType = '{1}' AND RefferenceType = '{2}' AND RefferenceCode = '{3}' ",
                                               ccode, user.CoProfile.ProductType, "SECTIOCD", detailText.Substring(1, 2).Trim(), detailText.Substring(3, 80).Trim(), uid, cdate);
                    }
                    else
                    {
                        query += string.Format("INSERT INTO SvMstRefferenceService (CompanyCode, ProductType, RefferenceType, RefferenceCode, Description," +
                                               " DescriptionEng, IsActive, IsLocked, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)" +
                                               " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}') ",
                                               ccode, user.CoProfile.ProductType, "SECTIOCD", detailText.Substring(1, 2).Trim()
                                               , detailText.Substring(3, 80).Trim(), detailText.Substring(3, 80).Trim(), "1", "0", uid, cdate, uid, cdate);
                    }

                    if ((i % 50) == 0)
                    {
                        if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                        {
                            result = false;
                            return result;
                        }

                        query = string.Empty;
                    }
                }
                if (!(string.IsNullOrEmpty(query)))
                {
                    if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                    {
                        result = false;
                        return result;
                    }
                }
                if (result)
                {
                    SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                    cmd.CommandText = "SELECT COUNT (*) FROM SvUtlSectionDescription";
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    decimal decCount = Convert.ToDecimal(dt.Rows[0][0]);
                    if (decCount != Convert.ToDecimal(headerText.Substring(76, 6).Trim()))
                    {
                        msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                        result = false;
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }

            return result;
        }

        private bool UploadDataWWRNTLocal(string[] lines)
        {
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            string query = string.Empty;
            string headerText = lines[0];

            if (maxLinesLast == 0)
            {
                maxLine = maxLine - 1;
            }

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return false;
            }

            try
            {
                string ccode = user.CompanyCode;
                string bcode = user.BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = user.UserId;

                int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;
                CoProfile oGnMstCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oGnMstCoProfile.CompanyCode != headerText.Substring(16, 10).Trim()
                    && headerText.Substring(16, 10).Trim() != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }

                LookUpDtl oGnMstLookUpDtl = ctx.LookUpDtls.Find(ccode, GnMstLookUpHdr.ProductType,
                   oGnMstCoProfile.ProductType);
                if (oGnMstLookUpDtl.ParaValue != headerText.Substring(82, 1).Trim())
                {
                    msg = "Product Type tidak sesuai dengan Company Profile";
                    return result;
                }

                query += string.Format("INSERT INTO SvUtlWarranty (CompanyCode, ProcessDate, DealerCode, ReceivedDealerCode, DealerName," +
                                       " TotalNoOfItem, ProductType, CreatedBy, CreatedDate) VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}') ",
                                       ccode, cdate, headerText.Substring(6, 10).Trim(), headerText.Substring(16, 10).Trim(), headerText.Substring(26, 50).Trim(),
                                       Convert.ToDecimal(headerText.Substring(76, 6).Trim()), headerText.Substring(82, 1).Trim(), uid, cdate);

                if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                {
                    result = false;
                    return result;
                }
                query = string.Empty;

                for (int i = 1; i < maxLine; i++)
                {
                    string detailText = lines[i];

                    query += string.Format("INSERT INTO svUtlWarrantyTime (CompanyCode, ProcessDate, SeqNo, BasicModel, TaskCode, Odometer, TimePeriod, TimeDim," +
                                           " EffectiveSalesDate, Description) VALUES ('{0}','{1}',{2},'{3}','{4}',{5},{6},'{7}','{8}','{9}') ",
                                           ccode, cdate, i, detailText.Substring(1, 15).Trim(), detailText.Substring(16, 6).Trim(),
                                           Convert.ToDecimal(detailText.Substring(22, 9).Trim()), Convert.ToDecimal(detailText.Substring(31, 2).Trim()),
                                           detailText.Substring(33, 1).Trim(), Convert.ToDateTime(detailText.Substring(38, 8).Trim()), detailText.Substring(46, 80).Trim());

                    if (GetRecordWarranty(ccode, user.CoProfile.ProductType, detailText.Substring(1, 15).Trim(), detailText.Substring(16, 6).Trim()))
                    {
                        query += string.Format("UPDATE SvMstWarranty SET Description = '{4}' , OdoMeter = {5}, TimePeriod = {6}, TimeDim = '{7}', LastUpdateBy = '{8}', LastUpdateDate = '{9}'" +
                                               " WHERE CompanyCode = '{0}' AND ProductType = '{1}' AND BasicModel = '{2}' AND TaskCode = '{3}' ",
                                               ccode, detailText.Substring(1, 15).Trim(), detailText.Substring(16, 6).Trim(), detailText.Substring(46, 80).Trim(),
                                               Convert.ToDecimal(detailText.Substring(22, 9).Trim()), Convert.ToDecimal(detailText.Substring(31, 2).Trim()), detailText.Substring(33, 1).Trim(), uid, cdate);
                    }
                    else
                    {
                        query += string.Format("INSERT INTO SvMstWarranty (CompanyCode, ProductType, BasicModel, OperationNo, EffectiveDate, Description, Odometer, TimePeriod, TimeDim," +
                                               " IsActive, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, IsLocked)" +
                                               " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}') ",
                                               ccode, user.CoProfile.ProductType, detailText.Substring(1, 15).Trim(), detailText.Substring(16, 6).Trim(),
                                               Convert.ToDateTime(detailText.Substring(38, 8).Trim()), detailText.Substring(46, 80).Trim(), Convert.ToDecimal(detailText.Substring(22, 9).Trim())
                                               , Convert.ToDecimal(detailText.Substring(31, 2).Trim()), detailText.Substring(33, 1).Trim(), "1", uid, cdate, uid, cdate, "0");
                    }

                    if ((i % 20) == 0)
                    {
                        if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                        {
                            result = false;
                            return result;
                        }

                        query = string.Empty;
                    }
                }

                if (!(string.IsNullOrEmpty(query)))
                {
                    //ctx.CommandText = query;
                    //result = DaoBase.ExecuteNonQuery(ctx) > 0;
                    //if (!result) return result;
                    if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                    {
                        result = false;
                        return result;
                    }
                }

                if (result)
                {
                    SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                    cmd.CommandText = "SELECT COUNT (*) FROM SvUtlWarrantyTime";
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    decimal decCount = Convert.ToDecimal(dt.Rows[0][0]);
                    if (decCount != Convert.ToDecimal(headerText.Substring(76, 6).Trim()))
                    {
                        msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                        result = false;
                    }
                }


            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }

            return result;
        }

        private bool UploadDataWTROBLocal(string[] lines)
        {
            bool result = false;

            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            string query = string.Empty;
            string headerText = lines[0];

            if (maxLinesLast == 0)
            {
                maxLine = maxLine - 1;
            }

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                return false;
            }

            try
            {
                string ccode = user.CompanyCode;
                string productType = user.CoProfile.ProductType;
                string bcode = user.BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = user.UserId;

                int num_fin = (string.IsNullOrEmpty(lines[lines.Length - 1])) ? 2 : 1;

                CoProfile oGnMstCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oGnMstCoProfile.CompanyCode != headerText.Substring(16, 10).Trim()
                    && headerText.Substring(16, 10).Trim() != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }

                query += string.Format("INSERT INTO SvUtlTroubleCode (CompanyCode, ProcessDate, DealerCode, ReceivedDealerCode, DealerName, " +
                                       " TotalNoOfItem, ProductType, CreatedBy, CreatedDate) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}') ",
                                       ccode, cdate, headerText.Substring(6, 10).Trim(), headerText.Substring(16, 10).Trim(), headerText.Substring(26, 50).Trim(),
                                       Convert.ToDecimal(headerText.Substring(76, 6).Trim()), headerText.Substring(82, 1).Trim(), uid, cdate);

                if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                {
                    result = false;
                    return result;
                }
                query = string.Empty;

                for (int i = 1; i < maxLine; i++)
                {
                    string detailText = lines[i];

                    string troubleCodeFlag = "";
                    if (detailText.Substring(1, 2).Trim() == "CC")
                        troubleCodeFlag = "COMPLNCD";
                    else if (detailText.Substring(1, 2).Trim() == "DC")
                        troubleCodeFlag = "DEFECTCD";

                    query += string.Format("INSERT INTO svUtlTroubleDescription (CompanyCode, ProcessDate, SeqNo, TroubleCodeFlag, ComplainDefectCode, Description)" +
                                           " VALUES ('{0}','{1}',{2},'{3}','{4}','{5}') ",
                                           ccode, cdate, i, detailText.Substring(1, 2).Trim(), detailText.Substring(3, 2).Trim(), detailText.Substring(5, 80).Trim());

                    if (GetRecordRefference(ccode, productType, troubleCodeFlag, detailText.Substring(3, 2).Trim()))
                    {
                        query += string.Format("UPDATE SvMstRefferenceService SET Description = '{4}' , DescriptionEng = '{4}' , LastUpdateBy = '{5}', LastUpdateDate = '{6}' " +
                                               " WHERE CompanyCode = '{0}' AND ProductType = '{1}' AND RefferenceType = '{2}' AND RefferenceCode = '{3}' ",
                                               ccode, productType, troubleCodeFlag, detailText.Substring(3, 2).Trim(), detailText.Substring(5, 80).Trim(), uid, cdate);
                    }
                    else
                    {
                        query += string.Format("INSERT INTO SvMstRefferenceService (CompanyCode, ProductType, RefferenceType, RefferenceCode, Description, DescriptionEng, CreatedBy," +
                                               " CreatedDate,LastUpdateBy, LastUpdateDate, IsActive,IsLocked) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}') ",
                                               ccode, productType, troubleCodeFlag, detailText.Substring(3, 2).Trim(), detailText.Substring(5, 80).Trim(), detailText.Substring(5, 80).Trim(),
                                               uid, cdate, uid, cdate, "1", "0");
                    }
                    if ((i % 20) == 0)
                    {
                        if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                        {
                            result = false;
                            return result;
                        }

                        query = string.Empty;
                    }
                }

                if (!(string.IsNullOrEmpty(query)))
                {
                    if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                    {
                        result = false;
                        return result;
                    }
                }

                if (result)
                {
                    SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                    cmd.CommandText = "SELECT COUNT (*) FROM SvUtlTroubleDescription";
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    decimal decCount = Convert.ToDecimal(dt.Rows[0][0]);
                    if (decCount != Convert.ToDecimal(headerText.Substring(76, 6).Trim()))
                    {
                        msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                        result = false;
                    }
                }

                result = true;

            }
            catch (Exception ex)
            {

                throw;
            }
            return result;
        }

        private bool UploadDataWFRATLocal(string[] lines)
        {
            bool result = false;
            int maxLine = lines.Length;
            int maxLinesLast = lines[maxLine - 1].Length;
            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            string query = string.Empty;
            if (maxLinesLast == 0)
            {
                maxLine = maxLine - 1;
            }

            string ccode = user.CompanyCode;
            string bcode = user.BranchCode;
            DateTime cdate = DateTime.Now;
            string uid = user.UserId;

            for (int i = 1; i < maxLine; i++)
            {
                string detailText = lines[i];
                query += string.Format("insert into SvUtlFlatRateTime values ('{0}',getdate(),'{1}','{2}','{3}','{4}','{5}') "
                           , ccode, i, detailText.Substring(1, 4).Trim(), detailText.Substring(5, 11).Trim()
                           , Convert.ToDecimal(detailText.Substring(16, 4).Trim()) * new decimal(0.1), detailText.Substring(20, 100).Trim());
            }
            try
            {
                int val = ctx.Database.ExecuteSqlCommand(query);
                if (val > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                    msg = "Proses upload gagal";
                }
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }

            return result;
        }

        #region *** Support WCAMP ***
        private bool GetRecordCampaign(string companyCode, string productType, string complainCode, string defectCode, string chassisCode
            , decimal chassisStartNo, decimal chassisEndNo)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT * FROM SvMstCampaign WHERE CompanyCode = '{0}' AND ProductType = '{1}' AND ComplainCode = '{2}'" +
                                            " AND DefectCode = '{3}' AND ChassisCode = '{4}' AND ChassisStartNo = {5} AND ChassisEndNo = {6}",
                                            companyCode, productType, complainCode, defectCode, chassisCode, chassisStartNo, chassisEndNo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 1)
                return true;
            else return false;
        }
        #endregion

        #region *** Support WJUDG ***
        private bool GetRecordRefference(string companyCode, string productType, string refferenceType, string refferenceCode)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format(@"SELECT * FROM svMstRefferenceService WHERE CompanyCode = '{0}' AND ProductType = '{1}' AND RefferenceType = '{2}' AND RefferenceCode = '{3}'",
                companyCode, productType, refferenceType, refferenceCode);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count == 1) return true;
            else return false;
        }
        #endregion

        #region *** Support WPDFS ***
        private bool GetRecordPdiFsc(string companyCode, string productType, string basicModel, string isCampaign, string transmissionType
            , decimal pdiFscSeq, DateTime effectiveDate)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT * FROM SvMstPdiFscRate WHERE CompanyCode = '{0}' AND ProductType = '{1}' AND BasicModel = '{2}'" +
                                            " AND isCampaign = '{3}' AND TransmissionType = '{4}' AND PdiFscSeq = {5} AND EffectiveDate = '{6}'",
                                            companyCode, productType, basicModel, isCampaign, transmissionType, pdiFscSeq, effectiveDate);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count == 1) return true;
            else return false;
        }
        #endregion

        #region *** Support WWRNT ***
        private bool GetRecordWarranty(string companyCode, string productType, string basicModel, string taskCode)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("SELECT * FROM SvMstWarranty WHERE CompanyCode = '{0}' AND ProductType = '{1}' " +
                                            " AND BasicModel = '{2}' AND OperationNo = '{3}'", companyCode, productType, basicModel, taskCode);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count == 1) return true;
            else return false;
        }
        #endregion

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

        #endregion

        #region --- check data ---

        public JsonResult CheckData(string DataID, string Contents, string Id)
        {
            //try
            //{
            //    DcsDataMap dcsDataMap = new DcsDataMap();
            //    List<DataTable> listDt = new List<DataTable>();
            //    IEnumerable<SysFlatFileHdr> recHdr;

            //    var countDtl = dcsDataMap.GetCountDetail(DataID);
            //    if (countDtl == 1)
            //        dcsDataMap.MappingData(DataID, Contents, out recHdr, ref listDt);
            //    else
            //        dcsDataMap.MappingData(DataID, Contents, countDtl, out recHdr, ref listDt);

            //    dcsDataMap = null;

            //    string output1 = JsonConvert.SerializeObject(recHdr, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.Indented });
            //    string output2 = JsonConvert.SerializeObject(listDt, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.Indented });

            //    //return Json(new { success = true, message = "", header = recHdr, detail = listDt });
            //    return Json(new { success = true, data = output1, detail = output2 });

            //}
            //catch (Exception ex)
            //{
            //    //return JsonException(ex.Message);
            //    return Json(new { success = false, message = ex.Message });
            //}

            DcsDataMap dcsDataMap = new DcsDataMap();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            List<DataTable> listDt = new List<DataTable>();

            var lines = Contents.Split('\n');
            string data = lines[0].ToString();
            cmd.CommandText = string.Format("select sum(FieldLength) from sysFlatFileHdr where CodeID = '{0}'", DataID);

            SqlDataAdapter sda1 = new SqlDataAdapter(cmd);
            DataTable dt1 = new DataTable();
            sda1.Fill(dt1);
            mxLenght = Convert.ToInt32(dt1.Rows[0][0].ToString());

            DataTable dtHeader = new DataTable();
            //dtHeader = MappingDataHeader(DataID, data, lines, cmd);
            dtHeader = dcsDataMap.MappingDataHeader(DataID, lines);

            string output1 = JsonConvert.SerializeObject(dtHeader, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.Indented });

            try
            {
                if (GetCountDetail(DataID, cmd) == 1)
                {
                    listDt = MappingDataDetail(DataID, lines, cmd);
                    //return Json(new { success = true, header = dtHeader, detail = listDt });  
                    string output2 = JsonConvert.SerializeObject(listDt, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.Indented });

                    return Json(new { success = true, header = output1, detail = output2 });
                }
                else
                {
                    listDt = MappingDataDetail(DataID, lines, cmd, GetCountDetail(DataID, cmd));
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #region *** move to dcs data map ***
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

            cmd.CommandText = string.Format("select * from sysFlatFileDtl where CodeID = '{0}' order by SeqNo", CodeId);
            SqlDataAdapter sda2 = new SqlDataAdapter(cmd);
            DataTable dt2 = new DataTable("Data Detail");
            sda2.Fill(dt2);

            int maxLine = lines.Length;
            int maxLinelast = lines[maxLine - 1].Length;
            if (maxLinelast == 0 || maxLine == null)
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

            cmd.CommandText = string.Format("select * from sysFlatFileDtl where CodeID = '{0}' order by SeqNo", CodeId);
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
        #endregion


        protected JsonResult JsonException(string message)
        {
            return Json(new { success = false, message = "Terjadi Kesalahan, Hubungi SDMS Support", error_log = message });
        }

        #region -- Private Method --
        private bool DcsValidated()
        {
            var DcsHelper = new DcsWsSoapClient();
            return DcsHelper.IsValid();
        }
        #endregion
    }
}
