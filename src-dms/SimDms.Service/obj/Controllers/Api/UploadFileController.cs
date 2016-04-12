using SimDms.Common;
using SimDms.Common.Models;
using SimDms.Service.BLL;
using SimDms.Service.Models;
using SimDms.Service.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class UploadFileController : BaseController
    {
        private UploadBLL.UploadType uploadType;
        private static string msg = "";

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file, string uploadType)
        {
            ResultModel result = new ResultModel();
            string userId = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            if (file != null)
            {
                byte[] rawData = new byte[file.ContentLength];
                file.InputStream.Read(rawData, 0, file.ContentLength);
                result.message = file.FileName;
                result.details = Encoding.UTF8.GetString(rawData);
            }

            return Json(result);
        }

        private UploadBLL.UploadType Type(string UploadType)
        {
            if (UploadType.Equals("WJUDG")) { uploadType = UploadBLL.UploadType.WJUDG; }
            if (UploadType.Equals("WTROB")) { uploadType = UploadBLL.UploadType.WTROB; }
            if (UploadType.Equals("WSECT")) { uploadType = UploadBLL.UploadType.WSECT; }
            if (UploadType.Equals("WFRAT")) { uploadType = UploadBLL.UploadType.WFRAT; }
            if (UploadType.Equals("WWRNT")) { uploadType = UploadBLL.UploadType.WWRNT; }
            if (UploadType.Equals("WCAMP")) { uploadType = UploadBLL.UploadType.WCAMP; }
            if (UploadType.Equals("WPDFS")) { uploadType = UploadBLL.UploadType.WPDFS; }
            if (UploadType.Equals("WFREE")) { uploadType = UploadBLL.UploadType.WFREE; }
            if (UploadType.Equals("WFRMB")) { uploadType = UploadBLL.UploadType.WFRMB; }
            if (UploadType.Equals("WCLAM")) { uploadType = UploadBLL.UploadType.WCLAM; }
            if (UploadType.Equals("WCMRB")) { uploadType = UploadBLL.UploadType.WCMRB; }

            return uploadType;
        }

        public bool ProcessUploadData(string[] lines, UploadBLL.UploadType uploadType)
        {
            ClearTableUtility(uploadType);
            switch (uploadType)
            {
                case UploadBLL.UploadType.WJUDG:
                    return UploadDataWJUDG(lines);
                case UploadBLL.UploadType.WTROB:
                    return UploadDataWTROB(lines);
                case UploadBLL.UploadType.WFRAT:
                    return UploadDataWFRAT(lines);
                case UploadBLL.UploadType.WPDFS:
                    return UploadDataWPDFS(lines);
                case UploadBLL.UploadType.WSECT:
                    return UploadDataWSECT(lines);
                case UploadBLL.UploadType.WCAMP:
                    return UploadDataWCAMP(lines);
                case UploadBLL.UploadType.WWRNT:
                    return UploadDataWWRNT(lines);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Upload Data Judgement Code
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool UploadDataWJUDG(string[] lines)
        {
            bool result = false;

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }
            string headerText = lines[0];

            try
            {
                
                string ccode = CompanyCode;
                string productType = ProductType;
                string bcode = BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;

                SvUtlJudgementCode oSvUtlJudgementCode = new SvUtlJudgementCode();
                oSvUtlJudgementCode.CompanyCode = ccode;
                oSvUtlJudgementCode.ProcessDate = cdate;
                oSvUtlJudgementCode.DealerCode = headerText.Substring(6, 10).Trim();
                oSvUtlJudgementCode.ReceivedDealerCode = headerText.Substring(16, 10).Trim();
                CoProfile oCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oCoProfile.CompanyCode != oSvUtlJudgementCode.ReceivedDealerCode
                    && oSvUtlJudgementCode.ReceivedDealerCode != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlJudgementCode.DealerName = headerText.Substring(26, 50).Trim();
                oSvUtlJudgementCode.TotalNoOfItem = Convert.ToDecimal(headerText.Substring(76, 6).Trim());
                oSvUtlJudgementCode.ProductType = headerText.Substring(82, 1).Trim();
                LookUpDtl oLookUpDtl = ctx.LookUpDtls.Find(ccode, GnMstLookUpHdr.ProductType, oCoProfile.ProductType);
                if (oLookUpDtl.ParaValue != oSvUtlJudgementCode.ProductType)
                {
                    msg = "Product Type tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlJudgementCode.CreatedBy = uid;
                oSvUtlJudgementCode.CreatedDate = cdate;
                ctx.SvUtlJudgementCodes.Add(oSvUtlJudgementCode);
                result = ctx.SaveChanges() > 0;
                if (!result) return result;

                for (int i = 1; i < lines.Length; i++)
                {
                    string detailText = lines[i];
                    SvUtlJudgementDescription oSvUtlJudgementDescription = new SvUtlJudgementDescription();
                    oSvUtlJudgementDescription.CompanyCode = ccode;
                    oSvUtlJudgementDescription.ProcessDate = cdate;
                    oSvUtlJudgementDescription.SeqNo = i;
                    oSvUtlJudgementDescription.JudgementCode = detailText.Substring(1, 4).Trim();
                    oSvUtlJudgementDescription.Description = detailText.Substring(5, 100).Trim();
                    oSvUtlJudgementDescription.DescriptionEng = detailText.Substring(105, 100).Trim();
                    ctx.SvUtlJudgementDescriptions.Add(oSvUtlJudgementDescription);
                    result = ctx.SaveChanges() > 0;
                    if (!result) break;

                    svMstRefferenceService osvMstRefferenceService = ctx.svMstRefferenceServices.Find(ccode,
                        productType, "JUDGECOD", oSvUtlJudgementDescription.JudgementCode);
                    if (osvMstRefferenceService == null)
                    {
                        osvMstRefferenceService = new svMstRefferenceService();
                        osvMstRefferenceService.CompanyCode = ccode;
                        osvMstRefferenceService.ProductType = productType;
                        osvMstRefferenceService.RefferenceType = "JUDGECOD";
                        osvMstRefferenceService.RefferenceCode = oSvUtlJudgementDescription.JudgementCode;
                        osvMstRefferenceService.IsLocked = false;
                        osvMstRefferenceService.CreatedBy = uid;
                        osvMstRefferenceService.CreatedDate = cdate;
                        ctx.svMstRefferenceServices.Add(osvMstRefferenceService);
                    }
                   
                    osvMstRefferenceService.LastupdateBy = uid;
                    osvMstRefferenceService.LastupdateDate = cdate;
                    osvMstRefferenceService.Description = oSvUtlJudgementDescription.Description;
                    osvMstRefferenceService.DescriptionEng = oSvUtlJudgementDescription.DescriptionEng;
                    osvMstRefferenceService.IsActive = true;

                    result = ctx.SaveChanges() > 0;

                    if (!result) break;
                }
                if (result)
                {
                    decimal decCount = ctx.SvUtlJudgementDescriptions.Count();
                        
                    if (decCount != oSvUtlJudgementCode.TotalNoOfItem)
                    {
                        msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return result;
        }

        /// <summary>
        /// Upload Data Trouble Code
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private  bool UploadDataWTROB(string[] lines)
        {
            bool result = false;
           
            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }
            string headerText = lines[0];
            
            try
            {
                
                string ccode = CompanyCode;
                string productType = ProductType;
                string bcode = BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;

                SvUtlTroubleCode oSvUtlTroubleCode = new SvUtlTroubleCode();
                oSvUtlTroubleCode.CompanyCode = ccode;
                oSvUtlTroubleCode.ProcessDate = cdate;
                oSvUtlTroubleCode.DealerCode = headerText.Substring(6, 10).Trim();
                oSvUtlTroubleCode.ReceivedDealerCode = headerText.Substring(16, 10).Trim();
                CoProfile oCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oCoProfile.CompanyCode != oSvUtlTroubleCode.ReceivedDealerCode
                    && oSvUtlTroubleCode.ReceivedDealerCode != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlTroubleCode.DealerName = headerText.Substring(26, 50).Trim();
                oSvUtlTroubleCode.TotalNoOfItem = Convert.ToDecimal(headerText.Substring(76, 6).Trim());
                oSvUtlTroubleCode.ProductType = headerText.Substring(82, 1).Trim();
                LookUpDtl oLookUpDtl = ctx.LookUpDtls.Find(ccode, GnMstLookUpHdr.ProductType,
                   oCoProfile.ProductType);
                if (oLookUpDtl.ParaValue != oSvUtlTroubleCode.ProductType)
                {
                    msg = "Product Type tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlTroubleCode.CreatedBy = uid;
                oSvUtlTroubleCode.CreatedDate = cdate;
                ctx.SvUtlTroubleCodes.Add(oSvUtlTroubleCode);
                result =ctx.SaveChanges() > 0;
                if (!result) return result;

                for (int i = 1; i < lines.Length; i++)
                {
                    string detailText = lines[i];
                    SvUtlTroubleDescription oSvUtlTroubleDescription = new SvUtlTroubleDescription();
                    oSvUtlTroubleDescription.CompanyCode = ccode;
                    oSvUtlTroubleDescription.ProcessDate = cdate;
                    oSvUtlTroubleDescription.SeqNo = i;
                    oSvUtlTroubleDescription.TroubleCodeFlag = detailText.Substring(1, 2).Trim();
                    oSvUtlTroubleDescription.ComplainDefectCode = detailText.Substring(3, 2).Trim();
                    oSvUtlTroubleDescription.Description = detailText.Substring(5, 80).Trim();
                    ctx.SvUtlTroubleDescriptions.Add(oSvUtlTroubleDescription);
                    result = ctx.SaveChanges() > 0;
                    if (!result) break;

                    string troubleCodeFlag = "";
                    if (oSvUtlTroubleDescription.TroubleCodeFlag == "CC")
                        troubleCodeFlag = "COMPLNCD";
                    else if (oSvUtlTroubleDescription.TroubleCodeFlag == "DC")
                        troubleCodeFlag = "DEFECTCD";

                    svMstRefferenceService osvMstRefferenceService = ctx.svMstRefferenceServices.Find(ccode,
                        productType, troubleCodeFlag, oSvUtlTroubleDescription.ComplainDefectCode);
                    if (osvMstRefferenceService == null)
                    {
                        osvMstRefferenceService = new svMstRefferenceService();
                        osvMstRefferenceService.CompanyCode = ccode;
                        osvMstRefferenceService.ProductType = productType;
                        osvMstRefferenceService.RefferenceType = troubleCodeFlag;
                        osvMstRefferenceService.RefferenceCode = oSvUtlTroubleDescription.ComplainDefectCode;
                        osvMstRefferenceService.IsLocked = false;
                        osvMstRefferenceService.CreatedBy = uid;
                        osvMstRefferenceService.CreatedDate = cdate;
                        ctx.svMstRefferenceServices.Add(osvMstRefferenceService);
                    }
                    
                    osvMstRefferenceService.LastupdateBy = uid;
                    osvMstRefferenceService.LastupdateDate = cdate;
                    osvMstRefferenceService.Description = oSvUtlTroubleDescription.Description;
                    osvMstRefferenceService.DescriptionEng = oSvUtlTroubleDescription.Description;
                    osvMstRefferenceService.IsActive = true;

                    result = ctx.SaveChanges() > 0;

                    if (!result) break;
                }
                if (result)
                {
                    decimal decCount = ctx.SvUtlTroubleDescriptions.Count();
                    if (decCount != oSvUtlTroubleCode.TotalNoOfItem)
                    {
                        msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return result;
        }

        /// <summary>
        /// Upload Data Section Code
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private  bool UploadDataWSECT(string[] lines)
        {
            bool result = false;

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }
            string headerText = lines[0];
            
            try
            {
                string ccode = CompanyCode;
                string productType = ProductType;
                string bcode = BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;

                SvUtlSectionCode oSvUtlSectionCode = new SvUtlSectionCode();
                oSvUtlSectionCode.CompanyCode = ccode;
                oSvUtlSectionCode.ProcessDate = cdate;
                oSvUtlSectionCode.DealerCode = headerText.Substring(6, 10).Trim();
                oSvUtlSectionCode.ReceivedDealerCode = headerText.Substring(16, 10).Trim();
                CoProfile oCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oCoProfile.CompanyCode != oSvUtlSectionCode.ReceivedDealerCode
                    && oSvUtlSectionCode.ReceivedDealerCode != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlSectionCode.DealerName = headerText.Substring(26, 50).Trim();
                oSvUtlSectionCode.TotalNoOfItem = Convert.ToDecimal(headerText.Substring(76, 6).Trim());
                oSvUtlSectionCode.ProductType = headerText.Substring(82, 1).Trim();
                LookUpDtl oLookUpDtl = ctx.LookUpDtls.Find(ccode, GnMstLookUpHdr.ProductType,
                   oCoProfile.ProductType);
                if (oLookUpDtl.ParaValue != oSvUtlSectionCode.ProductType)
                {
                    msg = "Product Type tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlSectionCode.CreatedBy = uid;
                oSvUtlSectionCode.CreatedDate = cdate;
                ctx.SvUtlSectionCodes.Add(oSvUtlSectionCode);
                result = ctx.SaveChanges() > 0;
                if (!result) return result;

                for (int i = 1; i < lines.Length; i++)
                {
                    string detailText = lines[i];
                    SvUtlSectionDescription oSvUtlSectionDescription = new SvUtlSectionDescription();
                    oSvUtlSectionDescription.CompanyCode = ccode;
                    oSvUtlSectionDescription.ProcessDate = cdate;
                    oSvUtlSectionDescription.SeqNo = i;
                    oSvUtlSectionDescription.SectionCode = detailText.Substring(1, 2).Trim();
                    oSvUtlSectionDescription.Description = detailText.Substring(3, 80).Trim();
                    ctx.SvUtlSectionDescriptions.Add(oSvUtlSectionDescription);
                    result =  ctx.SaveChanges() > 0;
                    if (!result) break;

                    svMstRefferenceService osvMstRefferenceService = ctx.svMstRefferenceServices.Find(ccode,
                        productType, "SECTIOCD", oSvUtlSectionDescription.SectionCode);
                    if (osvMstRefferenceService == null)
                    {
                        osvMstRefferenceService = new svMstRefferenceService();
                        osvMstRefferenceService.CompanyCode = ccode;
                        osvMstRefferenceService.ProductType = productType;
                        osvMstRefferenceService.RefferenceType = "SECTIOCD";
                        osvMstRefferenceService.RefferenceCode = oSvUtlSectionDescription.SectionCode;
                        osvMstRefferenceService.IsActive = true;
                        osvMstRefferenceService.IsLocked = false;
                        osvMstRefferenceService.CreatedBy = uid;
                        osvMstRefferenceService.CreatedDate = cdate;
                        ctx.svMstRefferenceServices.Add(osvMstRefferenceService);
                    }
                   
                    osvMstRefferenceService.LastupdateBy = uid;
                    osvMstRefferenceService.LastupdateDate = cdate;
                    osvMstRefferenceService.Description = oSvUtlSectionDescription.Description;
                    osvMstRefferenceService.DescriptionEng = oSvUtlSectionDescription.Description;

                    result = ctx.SaveChanges() > 0;

                    if (!result) break;
                }
                if (result)
                {
                    decimal decCount =ctx.SvUtlSectionDescriptions.Count();
                    if (decCount != oSvUtlSectionCode.TotalNoOfItem)
                    {
                        msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
          
            }

            return result;
        }

        /// <summary>
        /// Upload Data Flat Rate
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private  bool UploadDataWFRAT(string[] lines)
        {
            bool result = false;

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }
            string headerText = lines[0];
            
            try
            {
                
                string ccode = CompanyCode;
                string bcode = BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;

                SvUtlFlatRate oSvUtlFlatRate = new SvUtlFlatRate();
                oSvUtlFlatRate.CompanyCode = ccode;
                oSvUtlFlatRate.ProcessDate = cdate;
                oSvUtlFlatRate.DealerCode = headerText.Substring(6, 10).Trim();
                oSvUtlFlatRate.ReceivedDealerCode = headerText.Substring(16, 10).Trim();
                CoProfile oCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oCoProfile.CompanyCode != oSvUtlFlatRate.ReceivedDealerCode && oSvUtlFlatRate.ReceivedDealerCode != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlFlatRate.DealerName = headerText.Substring(26, 50).Trim();
                oSvUtlFlatRate.TotalNoOfItem = Convert.ToDecimal(headerText.Substring(76, 6).Trim());
                if (oSvUtlFlatRate.TotalNoOfItem != lines.Length - 1)
                {
                    msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                    return result;
                }
                oSvUtlFlatRate.ProductType = headerText.Substring(82, 1).Trim();
                LookUpDtl oLookUpDtl = ctx.LookUpDtls.Find(ccode, GnMstLookUpHdr.ProductType, oCoProfile.ProductType);
                if (oLookUpDtl.ParaValue != oSvUtlFlatRate.ProductType)
                {
                    msg = "Product Type tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlFlatRate.CreatedBy = uid;
                oSvUtlFlatRate.CreatedDate = cdate;
                ctx.SvUtlFlatRates.Add(oSvUtlFlatRate);
                result = ctx.SaveChanges() > 0;
                if (!result) return result;

                
                string sql = "";
                
                for (int i = 1; i < lines.Length; i++)
                {
                    string detailText = lines[i];
                    sql += string.Format("insert into SvUtlFlatRateTime values ('{0}',getdate(),'{1}','{2}','{3}','{4}','{5}')\n"
                           , ccode, i, detailText.Substring(1, 4).Trim(), detailText.Substring(5, 11).Trim()
                           , Convert.ToDecimal(detailText.Substring(16, 4).Trim()) * new decimal(0.1), detailText.Substring(20, 100).Trim());

                    if ((i % 100) == 0 || i == lines.Length - 1)
                    {
                        result = ctx.Database.ExecuteSqlCommand(sql) > 0;
                        sql = string.Empty;
                    }
                }

                sql = string.Format("exec uspfn_SvUtlUpdateFlatRate {0},{1},'{2}','{3}'", CompanyCode, BranchCode, ProductType, CurrentUser.UserId);

                ctx.Database.ExecuteSqlCommand(sql);

            }
            catch (Exception ex)
            {
                result = false;
             
            }

            return result;
        }

        /// <summary>
        /// Upload Data Warranty
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private  bool UploadDataWWRNT(string[] lines)
        {
            bool result = false;

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }
            string headerText = lines[0];
            
            try
            {
                string ccode = CompanyCode;
                string bcode = BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;

                SvUtlWarranty oSvUtlWarranty = new SvUtlWarranty();
                oSvUtlWarranty.CompanyCode = ccode;
                oSvUtlWarranty.ProcessDate = cdate;
                oSvUtlWarranty.DealerCode = headerText.Substring(6, 10).Trim();
                oSvUtlWarranty.ReceivedDealerCode = headerText.Substring(16, 10).Trim();
                CoProfile oCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oCoProfile.CompanyCode != oSvUtlWarranty.ReceivedDealerCode
                    && oSvUtlWarranty.ReceivedDealerCode != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlWarranty.DealerName = headerText.Substring(26, 50).Trim();
                oSvUtlWarranty.TotalNoOfItem = Convert.ToDecimal(headerText.Substring(76, 6).Trim());
                oSvUtlWarranty.ProductType = headerText.Substring(82, 1).Trim();
                LookUpDtl oLookUpDtl = ctx.LookUpDtls.Find(ccode, GnMstLookUpHdr.ProductType,
                    oCoProfile.ProductType);
                if (oLookUpDtl.ParaValue != oSvUtlWarranty.ProductType)
                {
                    msg = "Product Type tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlWarranty.CreatedBy = uid;
                oSvUtlWarranty.CreatedDate = cdate;
                ctx.SvUtlWarrantys.Add(oSvUtlWarranty);
                result = ctx.SaveChanges() > 0;

                
                for (int i = 1; i < lines.Length; i++)
                {
                    string detailText = lines[i];
                    SvUtlWarrantyTime oSvUtlWarrantyTime = new SvUtlWarrantyTime();
                    oSvUtlWarrantyTime.CompanyCode = ccode;
                    oSvUtlWarrantyTime.ProcessDate = cdate;
                    oSvUtlWarrantyTime.SeqNo = i;
                    oSvUtlWarrantyTime.BasicModel = detailText.Substring(1, 15).Trim();
                    oSvUtlWarrantyTime.TaskCode = detailText.Substring(16, 6).Trim();
                    oSvUtlWarrantyTime.Odometer = Convert.ToDecimal(detailText.Substring(22, 9).Trim());
                    oSvUtlWarrantyTime.TimePeriod = Convert.ToDecimal(detailText.Substring(31, 2).Trim());
                    oSvUtlWarrantyTime.TimeDim = detailText.Substring(33, 1).Trim();
                    oSvUtlWarrantyTime.EffectiveSalesDate = Convert.ToDateTime(detailText.Substring(38, 8).Trim());
                    oSvUtlWarrantyTime.Description = detailText.Substring(46, 80).Trim();
                    ctx.SvUtlWarrantyTimes.Add(oSvUtlWarrantyTime);
                    result = ctx.SaveChanges() > 0;
                    if (!result) break;

                    SvMstWarranty oSvMstWarranty = ctx.SvMstWarranties.Find(CompanyCode, ProductType,
                        oSvUtlWarrantyTime.BasicModel, oSvUtlWarrantyTime.TaskCode);
                    if (oSvMstWarranty == null)
                    {
                        oSvMstWarranty = new SvMstWarranty();
                        oSvMstWarranty.CompanyCode = ccode;
                        oSvMstWarranty.ProductType = ProductType;
                        oSvMstWarranty.BasicModel = oSvUtlWarrantyTime.BasicModel;
                        oSvMstWarranty.OperationNo = oSvUtlWarrantyTime.TaskCode;
                        oSvMstWarranty.EffectiveDate = oSvUtlWarrantyTime.EffectiveSalesDate.Value;
                        oSvMstWarranty.IsActive = true;
                        oSvMstWarranty.CreatedBy = uid;
                        oSvMstWarranty.CreatedDate = cdate;
                        ctx.SvMstWarranties.Add(oSvMstWarranty);
                    }
                    
                    oSvMstWarranty.Description = oSvUtlWarrantyTime.Description;
                    oSvMstWarranty.Odometer = oSvUtlWarrantyTime.Odometer.Value;
                    oSvMstWarranty.TimePeriod = oSvUtlWarrantyTime.TimePeriod;
                    oSvMstWarranty.TimeDim = oSvUtlWarrantyTime.TimeDim;
                    oSvMstWarranty.LastupdateBy = uid;
                    oSvMstWarranty.LastupdateDate = cdate;

                    result = ctx.SaveChanges() > 0;

                    if (!result) break;
                }

                if (result)
                {
                    decimal decCount = ctx.SvUtlWarrantyTimes.Count();
                    if (decCount != oSvUtlWarranty.TotalNoOfItem)
                    {
                        msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                        result = false;
                    }
                }
             
            }
            catch (Exception ex)
            {
              
            }

            return result;
        }

        /// <summary>
        /// Upload Data Campaign
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool UploadDataWCAMP(string[] lines)
        {
           var result = false;
            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }

            string headerText = lines[0];
            
            try
            {
                string ccode = CompanyCode;
                string bcode = BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;

                SvUtlCampaign oSvUtlCampaign = new SvUtlCampaign();
                oSvUtlCampaign.CompanyCode = ccode;
                oSvUtlCampaign.ProcessDate = cdate;
                oSvUtlCampaign.DealerCode = headerText.Substring(6, 10).Trim();
                oSvUtlCampaign.ReceivedDealerCode = headerText.Substring(16, 10).Trim();
                CoProfile oCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oCoProfile.CompanyCode != oSvUtlCampaign.ReceivedDealerCode
                    && oSvUtlCampaign.ReceivedDealerCode != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }

                oSvUtlCampaign.DealerName = headerText.Substring(26, 50).Trim();
                oSvUtlCampaign.TotalNoOfItem = Convert.ToDecimal(headerText.Substring(76, 6).Trim());
                oSvUtlCampaign.ProductType = headerText.Substring(82, 1).Trim();
                LookUpDtl oLookUpDtl = ctx.LookUpDtls.Find(ccode, GnMstLookUpHdr.ProductType,
                    oCoProfile.ProductType);
                if (oLookUpDtl.ParaValue != oSvUtlCampaign.ProductType)
                {
                    msg = "Product Type tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlCampaign.CreatedBy = uid;
                oSvUtlCampaign.CreatedDate = cdate;
                ctx.SvUtlCampaigns.Add(oSvUtlCampaign);
                result = ctx.SaveChanges() > 0;
                if (!result) return result;

                
                for (int i = 1; i < lines.Length; i++)
                {
                    string detailText = lines[i];
                    SvUtlCampaignRange oSvUtlCampaignRange = new SvUtlCampaignRange();
                    oSvUtlCampaignRange.CompanyCode = ccode;
                    oSvUtlCampaignRange.ProcessDate = cdate;
                    oSvUtlCampaignRange.SeqNo = i;
                    oSvUtlCampaignRange.ComplainCode = detailText.Substring(1, 2).Trim();
                    oSvUtlCampaignRange.DefectCode = detailText.Substring(3, 2).Trim();
                    oSvUtlCampaignRange.ChassisCode = detailText.Substring(5, 11).Trim();
                    oSvUtlCampaignRange.ChassisStartNo = Convert.ToDecimal(detailText.Substring(16, 6).Trim());
                    oSvUtlCampaignRange.ChassisEndNo = Convert.ToDecimal(detailText.Substring(22, 6).Trim());
                    oSvUtlCampaignRange.CloseDate = Convert.ToDateTime(detailText.Substring(28, 8).Trim());
                    oSvUtlCampaignRange.TaskCode = detailText.Substring(36, 6).Trim();
                    oSvUtlCampaignRange.Description = detailText.Substring(42, 100).Trim();
                    ctx.SvUtlCampaignRanges.Add(oSvUtlCampaignRange);
                    result = ctx.SaveChanges() > 0;
                    if (!result) break;

                    SvMstCampaign oSvMstCampaign = ctx.SvMstCampaigns.Find(ccode, ProductType,
                        oSvUtlCampaignRange.ComplainCode, oSvUtlCampaignRange.DefectCode,
                        oSvUtlCampaignRange.ChassisCode, oSvUtlCampaignRange.ChassisStartNo,
                        oSvUtlCampaignRange.ChassisEndNo);
                    if (oSvMstCampaign == null)
                    {
                        oSvMstCampaign = new SvMstCampaign();
                        oSvMstCampaign.CompanyCode = ccode;
                        oSvMstCampaign.ProductType = ProductType;
                        oSvMstCampaign.ComplainCode = oSvUtlCampaignRange.ComplainCode;
                        oSvMstCampaign.DefectCode = oSvUtlCampaignRange.DefectCode;
                        oSvMstCampaign.ChassisCode = oSvUtlCampaignRange.ChassisCode;
                        oSvMstCampaign.ChassisStartNo = oSvUtlCampaignRange.ChassisStartNo.Value;
                        oSvMstCampaign.ChassisEndNo = oSvUtlCampaignRange.ChassisEndNo.Value;
                        oSvMstCampaign.IsActive = true;
                        oSvMstCampaign.IsLocked = false;

                        oSvMstCampaign.CreatedBy = uid;
                        oSvMstCampaign.CreatedDate = cdate;
                        ctx.SvMstCampaigns.Add(oSvMstCampaign);
                    }
                    
                    oSvMstCampaign.OperationNo = oSvUtlCampaignRange.TaskCode;
                    oSvMstCampaign.CloseDate = oSvUtlCampaignRange.CloseDate.Value;
                    oSvMstCampaign.Description = oSvUtlCampaignRange.Description;

                    oSvMstCampaign.LastupdateBy = uid;
                    oSvMstCampaign.LastupdateDate = cdate;

                    result = ctx.SaveChanges() > 0;

                    if (!result) break;
                }

                if (result)
                {
                    
                    decimal decCount = ctx.SvUtlCampaignRanges.Count();
                    if (decCount != oSvUtlCampaign.TotalNoOfItem)
                    {
                        msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                        result = false;
                    }
                }
               
            }
            catch (Exception ex)
            {
            }

            return result;
        }

        /// <summary>
        /// Upload Data PDI & FSC Amount
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool UploadDataWPDFS(string[] lines)
        {
            bool result = false;

            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }
            string headerText = lines[0];
           
            try
            {
                string ccode = CompanyCode;
                string bcode = BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;

                SvUtlPdiFsc oSvUtlPdiFsc = new SvUtlPdiFsc();
                oSvUtlPdiFsc.CompanyCode = ccode;
                oSvUtlPdiFsc.ProcessDate = cdate;
                oSvUtlPdiFsc.DealerCode = headerText.Substring(6, 10).Trim();
                oSvUtlPdiFsc.ReceivedDealerCode = headerText.Substring(16, 10).Trim();
                CoProfile oCoProfile = ctx.CoProfiles.Find(ccode, bcode);
                if (oCoProfile.CompanyCode != oSvUtlPdiFsc.ReceivedDealerCode
                    && oSvUtlPdiFsc.ReceivedDealerCode != "9999999")
                {
                    msg = "Received Dealer Code tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlPdiFsc.DealerName = headerText.Substring(26, 50).Trim();
                oSvUtlPdiFsc.TotalNoOfItem = Convert.ToDecimal(headerText.Substring(76, 6).Trim());
                oSvUtlPdiFsc.ProductType = headerText.Substring(82, 1).Trim();
                LookUpDtl oLookUpDtl = ctx.LookUpDtls.Find(ccode, GnMstLookUpHdr.ProductType,
                   oCoProfile.ProductType);
                if (oLookUpDtl.ParaValue != oSvUtlPdiFsc.ProductType)
                {
                    msg = "Product Type tidak sesuai dengan Company Profile";
                    return result;
                }
                oSvUtlPdiFsc.CreatedBy = uid;
                oSvUtlPdiFsc.CreatedDate = cdate;
                ctx.SvUtlPdiFscs.Add(oSvUtlPdiFsc);
                result = ctx.SaveChanges() > 0;
                if (!result) return result;

                for (int i = 1; i < lines.Length; i++)
                {
                    string detailText = lines[i];
                    SvUtlPdiFscAmount oSvUtlPdiFscAmount = new SvUtlPdiFscAmount();
                    oSvUtlPdiFscAmount.CompanyCode = ccode;
                    oSvUtlPdiFscAmount.ProcessDate = cdate;
                    oSvUtlPdiFscAmount.SeqNo = i;
                    oSvUtlPdiFscAmount.BasicModel = detailText.Substring(1, 15).Trim();
                    oSvUtlPdiFscAmount.TransmissionType = detailText.Substring(16, 2).Trim();
                    oSvUtlPdiFscAmount.PdiFsc = detailText.Substring(18, 2).Trim();
                    oSvUtlPdiFscAmount.RegularLaborAmount = Convert.ToDecimal(detailText.Substring(20, 8).Trim());
                    oSvUtlPdiFscAmount.RegularMaterialAmount = Convert.ToDecimal(detailText.Substring(28, 8).Trim());
                    oSvUtlPdiFscAmount.RegularTotalAmount = Convert.ToDecimal(detailText.Substring(36, 8).Trim());
                    oSvUtlPdiFscAmount.CampaignLaborAmount = Convert.ToDecimal(detailText.Substring(44, 8).Trim());
                    oSvUtlPdiFscAmount.CampaignMaterialAmount = Convert.ToDecimal(detailText.Substring(52, 8).Trim());
                    oSvUtlPdiFscAmount.CampaignTotalAmount = Convert.ToDecimal(detailText.Substring(60, 8).Trim());
                    oSvUtlPdiFscAmount.EffectiveDate = Convert.ToDateTime(detailText.Substring(68, 8).Trim());
                    oSvUtlPdiFscAmount.Description = detailText.Substring(76, 50).Trim();
                    ctx.SvUtlPdiFscAmounts.Add(oSvUtlPdiFscAmount);
                    result = ctx.SaveChanges() > 0;
                    if (!result) break;

                    PdiFscRate oPdiFscRate = ctx.PdiFscRates.Find(ccode, ProductType,
                        oSvUtlPdiFscAmount.BasicModel, true, oSvUtlPdiFscAmount.TransmissionType,
                        Convert.ToDecimal(oSvUtlPdiFscAmount.PdiFsc), oSvUtlPdiFscAmount.EffectiveDate);
                    if (oPdiFscRate == null)
                    {
                        oPdiFscRate = new PdiFscRate();
                        oPdiFscRate.CompanyCode = ccode;
                        oPdiFscRate.ProductType = ProductType;
                        oPdiFscRate.BasicModel = oSvUtlPdiFscAmount.BasicModel;
                        oPdiFscRate.IsCampaign = true;
                        oPdiFscRate.TransmissionType = oSvUtlPdiFscAmount.TransmissionType;
                        oPdiFscRate.PdiFscSeq = Convert.ToDecimal(oSvUtlPdiFscAmount.PdiFsc);
                        oPdiFscRate.EffectiveDate = oSvUtlPdiFscAmount.EffectiveDate.Value;
                        oPdiFscRate.IsActive = true;
                        oPdiFscRate.IsLocked = false;

                        oPdiFscRate.CreatedBy = uid;
                        oPdiFscRate.CreatedDate = cdate;
                        ctx.PdiFscRates.Add(oPdiFscRate);
                    }
                   
                    oPdiFscRate.Description = oSvUtlPdiFscAmount.Description;
                    oPdiFscRate.RegularLaborAmount = oSvUtlPdiFscAmount.CampaignLaborAmount.Value;
                    oPdiFscRate.RegularMaterialAmount = oSvUtlPdiFscAmount.CampaignMaterialAmount.Value;
                    oPdiFscRate.RegularTotalAmount = oSvUtlPdiFscAmount.CampaignTotalAmount.Value;

                    oPdiFscRate.LastupdateBy = uid;
                    oPdiFscRate.LastupdateDate = cdate;

                    result = ctx.SaveChanges() > 0;
                    if (!result) break;

                    oPdiFscRate = ctx.PdiFscRates.Find(ccode, ProductType,
                        oSvUtlPdiFscAmount.BasicModel, false, oSvUtlPdiFscAmount.TransmissionType,
                        Convert.ToDecimal(oSvUtlPdiFscAmount.PdiFsc), oSvUtlPdiFscAmount.EffectiveDate);
                    if (oPdiFscRate == null)
                    {
                        oPdiFscRate = new PdiFscRate();
                        oPdiFscRate.CompanyCode = ccode;
                        oPdiFscRate.ProductType = ProductType;
                        oPdiFscRate.BasicModel = oSvUtlPdiFscAmount.BasicModel;
                        oPdiFscRate.IsCampaign = false;
                        oPdiFscRate.TransmissionType = oSvUtlPdiFscAmount.TransmissionType;
                        oPdiFscRate.PdiFscSeq = Convert.ToDecimal(oSvUtlPdiFscAmount.PdiFsc);
                        oPdiFscRate.EffectiveDate = oSvUtlPdiFscAmount.EffectiveDate.Value;
                        oPdiFscRate.IsActive = true;
                        oPdiFscRate.IsLocked = false;

                        oPdiFscRate.CreatedBy = uid;
                        oPdiFscRate.CreatedDate = cdate;
                        ctx.PdiFscRates.Add(oPdiFscRate);
                    }
                    
                    oPdiFscRate.Description = oSvUtlPdiFscAmount.Description;
                    oPdiFscRate.RegularLaborAmount = oSvUtlPdiFscAmount.RegularLaborAmount.Value;
                    oPdiFscRate.RegularMaterialAmount = oSvUtlPdiFscAmount.RegularMaterialAmount.Value;
                    oPdiFscRate.RegularTotalAmount = oSvUtlPdiFscAmount.RegularTotalAmount.Value;

                    oPdiFscRate.LastupdateBy = uid;
                    oPdiFscRate.LastupdateDate = cdate;

                    result = ctx.SaveChanges() > 0;

                    if (!result) break;
                }
                if (result)
                {
                    
                    decimal decCount = ctx.SvUtlPdiFscAmounts.Count();
                    if (decCount != oSvUtlPdiFsc.TotalNoOfItem)
                    {
                        msg = "Jumlah total header tidak sesuai dengan jumlah item yang diupload";
                        result = false;
                    }
                }
                
            }
            catch (Exception ex)
            {
            
            }
            return result;
        }

        private void ClearTableUtility(UploadBLL.UploadType uploadType)
        {
            var query = "";
            switch (uploadType)
            {
                case UploadBLL.UploadType.WCAMP:
                    query = "DELETE FROM svUtlCampaign; DELETE FROM svUtlCampaignRange";
                    break;
                case UploadBLL.UploadType.WFRAT:
                    query = "DELETE FROM svUtlFlatRate; DELETE FROM svUtlFlatRateTime";
                    break;
                case UploadBLL.UploadType.WJUDG:
                    query = "DELETE FROM svUtlJudgementCode; DELETE FROM svUtlJudgementDescription";
                    break;
                case UploadBLL.UploadType.WPDFS:
                    query = "DELETE FROM svUtlPdiFsc; DELETE FROM svUtlPdiFscAmount";
                    break;
                case UploadBLL.UploadType.WSECT:
                    query = "DELETE FROM svUtlSectionCode; DELETE FROM svUtlSectionDescription";
                    break;
                case UploadBLL.UploadType.WTROB:
                    query = "DELETE FROM svUtlTroubleCode; DELETE FROM svUtlTroubleDescription";
                    break;
                case UploadBLL.UploadType.WWRNT:
                    query = "DELETE FROM svUtlWarranty; DELETE FROM svUtlWarrantyTime";
                    break;
            }
            ctx.Database.ExecuteSqlCommand(query);
        }
    }
}