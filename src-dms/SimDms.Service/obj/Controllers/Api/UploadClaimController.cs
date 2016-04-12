using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.BLL;
using SimDms.Service.Models;
using System.Collections;
using System.Data;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Service.Controllers.Api
{
    public class UploadClaimController : BaseController
    {
        #region Public JsonResult
        public JsonResult Default()
        {
            return Json(new
            {
                GenerateDate = DateTime.Now,
                RefferenceDate = DateTime.Now,
                LotNo = string.Empty
            });
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (!file.FileName.ToLower().EndsWith("txt"))
                    throw new Exception("File harus berupa text document (.txt)");
                var textFile = new StreamReader(file.InputStream);
                var content = textFile.ReadToEnd();

                return Json(new { success = true, Content = content, FileName = file.FileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Upload(string Content, WarrantyClaimUploadHdr model)
        {
            try
            {
                string[] lines = Content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                p_ValidateFile(lines, model);
                
                var oWClaimDtlFiles = p_WClaimDtlFiles(lines);
                int j = 1;
                var listDetail = new List<WarrantyClaimUploadDtls>();
                var listPart = new List<WarrantyClaimUploadPart>();
                var listCost = new List<WarrantyClaimUploadCost>();
                Hashtable hash = new Hashtable();

                var cust = ctx.Customers.Where(p => p.CompanyCode == CompanyCode && p.CustomerCode == model.SenderDealerCode).FirstOrDefault();
                for (int i = 0; i < oWClaimDtlFiles.Count; i++)
                {
                    listDetail.Add(p_WClaimDtlFile(oWClaimDtlFiles, i, cust));
                    
                    int k = 1;
                    foreach (WarrantyClaimPartFile partfile in oWClaimDtlFiles[i].ListPartFiles)
                    {
                        foreach (WarrantyClaimPartFile.WarrantyClaimPart claimpart in partfile.WarrantyClaimParts)
                        {
                            var oMstPart = ctx.ItemInfos.Where(p => p.CompanyCode == CompanyCode && p.PartNo == claimpart.PartNo).FirstOrDefault();
                            var oPart = new WarrantyClaimUploadPart()
                            {
                                GenerateSeq = i + 1,
                                PartSeq = k,
                                IsCausal = claimpart.CausalPartCode.Equals("X"),
                                PartNo = claimpart.PartNo,
                                Quantity = claimpart.Qty,
                                PartName = (oMstPart == null) ? "" : oMstPart.PartName ?? ""
                            };
                            listPart.Add(oPart);
                            k++;
                        }
                    }

                    if (hash[oWClaimDtlFiles[i].BasicModel] == null)
                    {
                        var oCost = new WarrantyClaimUploadCost()
                        {
                            RecNo = j,
                            BasicModel = oWClaimDtlFiles[i].BasicModel,
                            TotalClaimAmt = 0
                        };
                        hash[oWClaimDtlFiles[i].BasicModel] = oCost;
                        listCost.Add(oCost);
                        j++;
                    }
                    else
                    {
                        var oCost = (WarrantyClaimUploadCost)hash[oWClaimDtlFiles[i].BasicModel];
                        oCost.TotalClaimAmt = Convert.ToDecimal(oCost.TotalClaimAmt) + 0;
                    }
                }

                return Json(new { success = true, CostInfo = listCost, PartInfo = listPart, DetailInfo = listDetail });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult CheckSave(string Content, WarrantyClaimUploadHdr model)
        {
            string msg = string.Empty;
            bool isExist = false;
            bool isSuccess = false;
            var GenerateDate = Convert.ToDateTime(model.GenerateDate);
            DateTransValidation(GenerateDate);
            try
            {
                string[] lines = Content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                var oWarrantyClaimBLL = WarrantyClaimBLL.Instance(CurrentUser.UserId);
                Claim oClaim = oWarrantyClaimBLL.GetRecordClaim(p_ValidateFile(lines, model));
                if (oClaim != null)
                {
                    if (Convert.ToInt32(oClaim.PostingFlag) >= 2)
                    {
                        throw new Exception(string.Format("Dokumen No {0} sudah di {1}", oClaim.GenerateNo, "Posting"));
                    }
                    else
                    {
                        msg = "Data sudah pernah diupload. Apakah Anda ingin update ?";
                        isSuccess = true;
                        isExist = true;
                    }
                }
                else {
                    isSuccess = true;
                }

                return Json(new { success = isSuccess, message = msg, exist = isExist });
            }
            catch (Exception ex)
            {
                string innerEx = (ex.InnerException == null) ? ex.Message :
                (ex.InnerException.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message;
                {
                    return Json(new
                    {
                        success = false,
                        message = (ex.InnerException == null) ? ex.Message : innerEx
                    });
                }
            }
        }

        public JsonResult Save(string Content, WarrantyClaimUploadHdr model)
        {
            string msg = string.Empty;
            bool isSuccess = false;
            try
            {
                string generateNo;
                string FPJNo = string.Empty;
                var generateDate = Convert.ToDateTime(model.GenerateDate);
                var datHeader = new WarrantyClaimUploadHdr();
                var listDetail = new List<WarrantyClaimUploadDtls>();
                var listPart = new List<WarrantyClaimUploadPart>();
                var listCost = new List<WarrantyClaimUploadCost>();

                string[] lines = Content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                var oWClaimHdrFile = p_ValidateFile(lines, model);
                var oWClaimDtlFiles = p_WClaimDtlFiles(lines);
                GnMstCustomer cust = ctx.Customers.Where(p => p.CompanyCode == CompanyCode 
                    && p.CustomerCode == model.SenderDealerCode).FirstOrDefault();
                for (int i = 0; i < oWClaimDtlFiles.Count; i++)
                {
                    listDetail.Add(p_WClaimDtlFile(oWClaimDtlFiles, i, cust));
                }

                WarrantyClaimBLL oWarrantyClaimBLL = WarrantyClaimBLL.Instance(CurrentUser.UserId);
                isSuccess = oWarrantyClaimBLL.Save(oWClaimHdrFile, oWClaimDtlFiles, out generateNo, generateDate, FPJNo);
                if (isSuccess)
                {
                    p_PopulateData(generateNo, oWarrantyClaimBLL, datHeader, listDetail, listPart, listCost);
                }

                return Json(new { success = isSuccess, CostInfo = listCost, PartInfo = listPart, DetailInfo = listDetail, HeaderInfo = datHeader });
            }
            catch (Exception ex)
            {
                string innerEx = (ex.InnerException == null) ? ex.Message :
                (ex.InnerException.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message;
                {
                    return Json(new
                    {
                        success = false,
                        message = (ex.InnerException == null) ? ex.Message : innerEx
                    });
                }
            }
        }

        public JsonResult PopulateData(string generateNo)
        {
            try
            {
                var datHeader = new WarrantyClaimUploadHdr();
                var listDetail = new List<WarrantyClaimUploadDtls>();
                var listPart = new List<WarrantyClaimUploadPart>();
                var listCost = new List<WarrantyClaimUploadCost>();

                WarrantyClaimBLL oWarrantyClaimBLL = WarrantyClaimBLL.Instance(CurrentUser.UserId);
                p_PopulateData(generateNo, oWarrantyClaimBLL, datHeader, listDetail, listPart, listCost);
                
                return Json(new { success = true, CostInfo = listCost, PartInfo = listPart, DetailInfo = listDetail, HeaderInfo = datHeader });
            }
            catch (Exception ex)
            {
                string innerEx = (ex.InnerException == null) ? ex.Message :
                (ex.InnerException.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message;
                {
                    return Json(new
                    {
                        success = false,
                        message = (ex.InnerException == null) ? ex.Message : innerEx
                    });
                }
            }
        }
        #endregion

        #region Private Method
        private WarrantyClaimHdrFile p_ValidateFile(string[] lines, WarrantyClaimUploadHdr model )
        {
            var oWClaimHdrFile = new WarrantyClaimHdrFile(lines[0]);
            if (oWClaimHdrFile.DataID != "WCLAM")
                throw new Exception("Data Header tidak sesuai dengan Warranty Claim Format!");

            //  Dealer Code harus sama dengan Kode Pengirim               
            if (oWClaimHdrFile.DealerCode != model.SenderDealerCode)
                throw new Exception("`Kode Dealer pada File tidak sesuai dengan yang Anda masukkan !");

            //  Received Dealer Code harus sama dengan Company Code sesuai dengan user login
            if (oWClaimHdrFile.RcvDealerCode != CompanyCode)
                throw new Exception("Kode Dealer Penerima pada File tidak sesuai dengan Kode Dealer Anda !");

            //  Receipt No = No Kwitansi yang di-input
            if (oWClaimHdrFile.ReceiptNo != model.RefferenceNo)
                throw new Exception("No Kwitansi pada File tidak sesuai dengan No Kwitansi yang Anda Masukkan !");

            //  Receipt Date = Tgl Kwitansi yang di-input
            if (oWClaimHdrFile.ReceiptDate.Date != model.RefferenceDate)
                throw new Exception("Tgl Kwitansi pada File tidak sesuai dengan tgl Kwitansi yang Anda Masukkan !");

            //  Product Type harus sama dengan Product Type sesuai dengan user login
            if (oWClaimHdrFile.ProductType != (
                    ProductType == "2W" ? "A" :
                    ProductType == "4W" ? "B" :
                    ProductType == "OB" ? "C" : ProductType))
                throw new Exception("Product Type Pada File Tidak Sesuai Dengan Product Type Dealer Anda !");

            //  No. Lot harus sama dengan yang diinput
            if (oWClaimHdrFile.LotNo != Convert.ToInt32(model.LotNo))
                throw new Exception("No. Lot Pada File Tidak Sesuai Dengan No. Lot Anda Masukkan!");

            //  Total Number of Item harus sama dengan jumlah record di detail
            if (oWClaimHdrFile.TotalItem != p_GetTotalItem(lines))
                throw new Exception("Jumlah Record di Detail Tidak Valid dengan Informasi di Header!");

            return oWClaimHdrFile;
        }

        private WarrantyClaimUploadDtls p_WClaimDtlFile(List<WarrantyClaimDtlFile> oWClaimDtlFiles, int i, GnMstCustomer cust)
        {
            var oDtl = new WarrantyClaimUploadDtls()
            {
                GenerateSeq = i + 1,
                CategoryCode = oWClaimDtlFiles[i].ClaimCategoryCode,
                IssueNo = oWClaimDtlFiles[i].IssueNo += (cust != null ? "-" + cust.CustomerAbbrName : ""),
                IssueDate = oWClaimDtlFiles[i].IssueDate,
                InvoiceNo = "",
                ServiceBookNo = oWClaimDtlFiles[i].ServiceBookNo,
                ChassisCode = oWClaimDtlFiles[i].ChassisCode,
                ChassisNo = oWClaimDtlFiles[i].ChassisNo,
                EngineCode = oWClaimDtlFiles[i].EngineCode,
                EngineNo = oWClaimDtlFiles[i].EngineNo,
                BasicModel = oWClaimDtlFiles[i].BasicModel,
                RegisteredDate = oWClaimDtlFiles[i].RegisteredDate,
                RepairedDate = oWClaimDtlFiles[i].RepairedDate,
                Odometer = oWClaimDtlFiles[i].Odometer,
                ComplainCode = oWClaimDtlFiles[i].TroubleCode.Substring(0, 2),
                DefectCode = oWClaimDtlFiles[i].TroubleCode.Substring(2, 2),
                SubletHour = 0,
                BasicCode = oWClaimDtlFiles[i].OperationNumber.Substring(0, 7),
                VarCom = oWClaimDtlFiles[i].OperationNumber.Substring(7, 1),
                OperationHour = Convert.ToDecimal(oWClaimDtlFiles[i].ActualLaborTime) / 10,
                ClaimAmt = 0,
                TroubleDescription = oWClaimDtlFiles[i].RepairDescription,
                ProblemExplanation = ""
            };

            return oDtl;
        }

        private List<WarrantyClaimDtlFile> p_WClaimDtlFiles(string[] lines)
        {
            var oWClaimDtlFiles = new List<WarrantyClaimDtlFile>();
            WarrantyClaimDtlFile oWClaimDtlFile = null;
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("1"))
                {
                    oWClaimDtlFile = new WarrantyClaimDtlFile(lines[i]);
                    oWClaimDtlFiles.Add(oWClaimDtlFile);
                }
                if (lines[i].StartsWith("2"))
                {
                    if (oWClaimDtlFile != null)
                    {
                        WarrantyClaimPartFile partfile = new WarrantyClaimPartFile(lines[i]);
                        oWClaimDtlFile.AddPartFile(partfile);
                    }
                }
                if (lines[i].StartsWith("3"))
                {
                    oWClaimDtlFile.RepairDescription = lines[i].Substring(1).ToString();
                }
            }

            return oWClaimDtlFiles;
        }

        private void p_PopulateData(string generateNo, WarrantyClaimBLL oWarrantyClaimBLL, WarrantyClaimUploadHdr datHeader, List<WarrantyClaimUploadDtls> listDetail,
            List<WarrantyClaimUploadPart> listPart, List<WarrantyClaimUploadCost> listCost)
        {
            Claim oSvTrnClaim = oWarrantyClaimBLL.GetRecordClaim(generateNo, "2");
            if (oSvTrnClaim != null)
            {
                datHeader.GenerateNo = oSvTrnClaim.GenerateNo;
                datHeader.GenerateDate = oSvTrnClaim.GenerateDate;//.ToString("dd MMM yyy  HH:mm");
                datHeader.SenderDealerCode = oSvTrnClaim.SenderDealerCode;
                datHeader.SenderDealerName = oSvTrnClaim.SenderDealerName;
                datHeader.RefferenceNo = oSvTrnClaim.RefferenceNo;
                datHeader.RefferenceDate = oSvTrnClaim.RefferenceDate;
                datHeader.LotNo = oSvTrnClaim.LotNo; //.ToString();

                DataSet ds = oWarrantyClaimBLL.GetDataClaim(oSvTrnClaim.GenerateNo);
                if (ds.Tables.Count > 2)
                {
                    listDetail.Clear();
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var oWCUploadDtls = new WarrantyClaimUploadDtls()
                        {
                            GenerateSeq = Convert.ToInt32(row["GenerateSeq"]),
                            CategoryCode = Convert.ToString(row["CategoryCode"]),
                            IssueNo = Convert.ToString(row["IssueNo"]),
                            IssueDate = Convert.ToDateTime(row["IssueDate"]),
                            InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                            ServiceBookNo = Convert.ToString(row["ServiceBookNo"]),
                            ChassisCode = Convert.ToString(row["ChassisCode"]),
                            ChassisNo = Convert.ToString(row["ChassisNo"]),
                            EngineCode = Convert.ToString(row["EngineCode"]),
                            EngineNo = Convert.ToString(row["EngineNo"]),
                            BasicModel = Convert.ToString(row["BasicModel"]),
                            RegisteredDate = Convert.ToDateTime(row["RegisteredDate"]),
                            RepairedDate = Convert.ToDateTime(row["RepairedDate"]),
                            Odometer = Convert.ToDecimal(row["Odometer"]),
                            ComplainCode = Convert.ToString(row["ComplainCode"]),
                            DefectCode = Convert.ToString(row["DefectCode"]),
                            SubletHour = Convert.ToDecimal(row["SubletHour"]),
                            BasicCode = Convert.ToString(row["BasicCode"]),
                            VarCom = Convert.ToString(row["VarCom"]),
                            OperationHour = Convert.ToDecimal(row["OperationHour"]),
                            ClaimAmt = Convert.ToDecimal(row["ClaimAmt"]),
                            TroubleDescription = Convert.ToString(row["TroubleDescription"]),
                            ProblemExplanation = Convert.ToString(row["ProblemExplanation"])
                        };
                        listDetail.Add(oWCUploadDtls);
                    }

                    listCost.Clear();
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        var oWCUploadCost = new WarrantyClaimUploadCost()
                        {
                            RecNo = Convert.ToInt32(row["RecNo"]),
                            BasicModel = Convert.ToString(row["BasicModel"]),
                            TotalClaimAmt = Convert.ToDecimal(row["TotalClaimAmt"])
                        };
                        listCost.Add(oWCUploadCost);
                    }

                    listPart.Clear();
                    foreach (DataRow row in ds.Tables[2].Rows)
                    {
                        var oWCUploadPart = new WarrantyClaimUploadPart()
                        {
                            GenerateSeq = Convert.ToInt32(row["GenerateSeq"]),
                            PartSeq = Convert.ToInt32(row["PartSeq"]),
                            IsCausal = Convert.ToBoolean(row["IsCausal"]),
                            PartNo = Convert.ToString(row["PartNo"]),
                            Quantity = Convert.ToDecimal(row["Quantity"]),
                            PartName = Convert.ToString(row["PartName"])
                        };
                        listPart.Add(oWCUploadPart);
                    }
                }
            }
        }

        private int p_GetTotalItem(string[] lines)
        {
            int i = 0;
            foreach (string line in lines)
            {
                if (line.StartsWith("1")) i++;
            }
            return i;
        }
        #endregion
    }
}
