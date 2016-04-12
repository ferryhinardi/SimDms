using SimDms.Common.Models;
using SimDms.Sales.Models.Result;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Text;
using SimDms.Sales.BLL;
using System.Text.RegularExpressions;
using SimDms.Common;
using SimDms.Sales.Models;

namespace SimDms.Sales.Controllers.Api
{
    public class UploadFileController : BaseController
    {
        private UploadBLL.UploadType uploadType;
        private static string msg = "";

        public JsonResult Default()
        {
            return Json(new
            {
                IsBranch
            });
        }
        
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file, string uploadType)
        {
            ResultModel result = new ResultModel();
            string userId = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            if(file != null)
            {
                byte[] rawData = new byte[file.ContentLength];
                file.InputStream.Read(rawData, 0, file.ContentLength);
                result.message = file.FileName;
                result.details = Encoding.UTF8.GetString(rawData);
            }

            return Json(result);
        }

        public JsonResult Process(string UploadType, string Contents)
        {
            string[] lines = null;

            lines = Regex.Split(Contents, "\r\n");

            if (UploadBLL.Validate(lines, Type(UploadType)))
            {
                bool isRecDCS = false;
                string o = ctx.LookUpDtls.Find(CompanyCode, "OMRECDCS", "STATUS").ParaValue;
                if (!string.IsNullOrEmpty(o))
                    if (o.Equals("1")) isRecDCS = true;

                string kodePerusahaan = CompanyCode;
                if (isRecDCS)
                    kodePerusahaan = ctx.GnMstCoProfileSaleses.Where(a => a.CompanyCode == CompanyCode).Select(a => a.LockingBy).Distinct().ToString();

                if (string.IsNullOrEmpty(kodePerusahaan))
                {
                    if (isRecDCS)
                        return Json(new { success = false, message = "Mohon Setup LockingBy di CoProfile Sales" });
                    else
                        return Json(new { success = false, message = "CompanyCode belum tersetting untuk UserID: " + CurrentUser.UserId });
                }

                if (uploadType != UploadBLL.UploadType.SFPLA && uploadType != UploadBLL.UploadType.SFPLB && uploadType != UploadBLL.UploadType.SFPLR)
                {

                    var result = ProcessUploadData(lines, uploadType, isRecDCS, kodePerusahaan);
                    if (result)
                    {
                        return Json(new { success = result });
                    }
                    else
                    {
                        return Json(new { success = result, message = msg });
                    }
                }
                else
                {
                    if (UploadDataSFPLB(lines, isRecDCS, kodePerusahaan))
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = msg });
                    }
                }
            }
            else
            {
                return Json(new { success = false, message = "Invalid Format Upload File" });
            }
        }

        private UploadBLL.UploadType Type(string UploadType)
        {
            if (UploadType.Equals("SPORD")) { uploadType = UploadBLL.UploadType.SPORD; }
            if (UploadType.Equals("SDORD")) { uploadType = UploadBLL.UploadType.SDORD; }
            if (UploadType.Equals("SSJAL")) { uploadType = UploadBLL.UploadType.SSJAL; }
            if (UploadType.Equals("SHPOK")) { uploadType = UploadBLL.UploadType.SHPOK; }
            if (UploadType.Equals("SACCS")) { uploadType = UploadBLL.UploadType.SACCS; }
            if (UploadType.Equals("SFPO1")) { uploadType = UploadBLL.UploadType.SFPO1; }
            if (UploadType.Equals("SFPO2")) { uploadType = UploadBLL.UploadType.SFPO2; }
            if (UploadType.Equals("SPRIC")) { uploadType = UploadBLL.UploadType.SPRIC; }
            if (UploadType.Equals("SFPLR")) { uploadType = UploadBLL.UploadType.SFPLR; }
            if (UploadType.Equals("SFPLA")) { uploadType = UploadBLL.UploadType.SFPLA; }
            if (UploadType.Equals("SUADE")) { uploadType = UploadBLL.UploadType.SUADE; }

            return uploadType;
        }

        private bool ProcessUploadData(string[] lines, UploadBLL.UploadType uploadType, bool isRecDCS, string kodePerusahaan)
        {
            switch (uploadType)
            {
                case UploadBLL.UploadType.SPORD:
                    return UploadDataSPORD(lines, isRecDCS, kodePerusahaan);
                case UploadBLL.UploadType.SDORD:
                    return UploadDataSDORD(lines, isRecDCS, kodePerusahaan);
                case UploadBLL.UploadType.SSJAL:
                    return UploadDataSSJAL(lines, isRecDCS, kodePerusahaan);
                case UploadBLL.UploadType.SHPOK:
                    return UploadDataSHPOK(lines, isRecDCS, kodePerusahaan);
                case UploadBLL.UploadType.SACCS:
                    return UploadDataSACCS(lines, isRecDCS, kodePerusahaan);
                case UploadBLL.UploadType.SFPO1:
                    return UploadDataSFPOL(lines, isRecDCS, kodePerusahaan);
                case UploadBLL.UploadType.SFPO2:
                    return UploadDataSFPOL(lines, isRecDCS, kodePerusahaan);
                //case UploadBLL.UploadType.SPRIC:
                //    return UploadDataSPRIC(lines, isRecDCS, kodePerusahaan);
                case UploadBLL.UploadType.SFPLB:
                    return UploadDataSFPLB(lines, isRecDCS, kodePerusahaan);
                case UploadBLL.UploadType.SFPLA:
                    return UploadDataSFPLB(lines, isRecDCS, kodePerusahaan);
                case UploadBLL.UploadType.SFPLR:
                    return UploadDataSFPLB(lines, isRecDCS, kodePerusahaan);
                case UploadBLL.UploadType.FAPIO:
                    return UploadDataFAPIO(lines, isRecDCS, kodePerusahaan);
                case UploadBLL.UploadType.SUADE:
                    return UploadDataSUADE(lines, isRecDCS, kodePerusahaan);
                default:
                    return false;
            }
        }

        private bool UploadDataSPORD(string[] lines, bool isRecDCS, string kodePerusahaan)
        {
            bool result = false;
            int linesLength = 260;
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false; 
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    try
                    {
                        UploadBLL.SPORDHdrFile oSPORDHdrFile = new UploadBLL.SPORDHdrFile(lines[0]);
                        string RcvDealerCode = (isRecDCS) ? CompanyCode : oSPORDHdrFile.RcvDealerCode;

                        if (!kodePerusahaan.Equals(oSPORDHdrFile.RcvDealerCode))
                        {
                            msg = "Invalid flat file, kode perusahaan tidak sama";
                            if (isRecDCS) msg += "\nHarap cek setup LockingBy di CoProfile Sales";
                            result = false; 
                        }

                        if (oSPORDHdrFile != null && oSPORDHdrFile.DataID == "SPORD")
                        {
                            OmUtlSPORDHdr oOmUtlSPORDHdr = ctx.OmUtlSPORDHdrs.Find(CompanyCode, BranchCode
                                                    , oSPORDHdrFile.DealerCode, RcvDealerCode, oSPORDHdrFile.BatchNo);
                            if (oOmUtlSPORDHdr != null)
                            {
                                msg = "No Batch: " + oSPORDHdrFile.BatchNo + " sudah ada";
                                result = false; 
                            }
                            else
                            {
                                oOmUtlSPORDHdr = new OmUtlSPORDHdr();
                                oOmUtlSPORDHdr.CompanyCode = CompanyCode;
                                oOmUtlSPORDHdr.BranchCode = BranchCode;
                                oOmUtlSPORDHdr.DealerCode = oSPORDHdrFile.DealerCode;
                                oOmUtlSPORDHdr.RcvDealerCode = RcvDealerCode;
                                oOmUtlSPORDHdr.BatchNo = oSPORDHdrFile.BatchNo;
                                oOmUtlSPORDHdr.Status = "0";
                                oOmUtlSPORDHdr.CreatedBy = CurrentUser.UserId;
                                oOmUtlSPORDHdr.CreatedDate = DateTime.Now;
                                oOmUtlSPORDHdr.LastUpdateBy = CurrentUser.UserId;
                                oOmUtlSPORDHdr.LastUpdateDate = DateTime.Now;
                                ctx.OmUtlSPORDHdrs.Add(oOmUtlSPORDHdr);
                                result = ctx.SaveChanges() > 0;
                                if (result == true)
                                {
                                    string skpNo = ""; string salesModelCode = ""; int salesModelYear = 0;
                                    for (int i = 1; i < lines.Length; i++)
                                    {
                                        if (lines[i].Length == linesLength)
                                        {
                                            if (lines[i].Substring(0, 1) == "1")
                                            {
                                                UploadBLL.SPORDDtl1File oSPORDDtl1File = new UploadBLL.SPORDDtl1File(lines[i]);
                                                if (oSPORDDtl1File != null && oSPORDDtl1File.SKPNo != "")
                                                {
                                                    OmUtlSPORDDtl1 oOmUtlSPORDDtl1 = ctx.OmUtlSPORDDtl1s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSPORDHdr.BatchNo, oSPORDDtl1File.SKPNo);
                                                    if (oOmUtlSPORDDtl1 != null)
                                                    {
                                                        msg = "No SKP : " + oSPORDDtl1File.SKPNo + " sudah ada";
                                                        result = false; 
                                                    }
                                                    else
                                                    {
                                                        //added by Akhmad Nuryanto 3 Sept 2009
                                                        if (ctx.OmUtlSPORDDtl1s.Find(CompanyCode, BranchCode, oOmUtlSPORDHdr.BatchNo, oSPORDDtl1File.SKPNo) != null)
                                                        {
                                                            msg ="flat file data No SKP: " + oSPORDDtl1File.SKPNo + " sudah diupload";
                                                            result = false; 
                                                        }
                                                        else
                                                        {
                                                            oOmUtlSPORDDtl1 = new OmUtlSPORDDtl1();
                                                            oOmUtlSPORDDtl1.CompanyCode = CompanyCode;
                                                            oOmUtlSPORDDtl1.BranchCode = BranchCode;
                                                            oOmUtlSPORDDtl1.BatchNo = oOmUtlSPORDHdr.BatchNo;
                                                            oOmUtlSPORDDtl1.SKPNo = oSPORDDtl1File.SKPNo;
                                                            skpNo = oOmUtlSPORDDtl1.SKPNo;
                                                            oOmUtlSPORDDtl1.SKPDate = oSPORDDtl1File.SKPDate;
                                                            oOmUtlSPORDDtl1.CreatedBy = CurrentUser.UserId;
                                                            oOmUtlSPORDDtl1.CreatedDate = DateTime.Now;
                                                            oOmUtlSPORDDtl1.LastUpdateBy = CurrentUser.UserId;
                                                            oOmUtlSPORDDtl1.LastUpdateDate = DateTime.Now;
                                                            oOmUtlSPORDDtl1.Status = "0";
                                                            ctx.OmUtlSPORDDtl1s.Add(oOmUtlSPORDDtl1);
                                                            result = ctx.SaveChanges() > 0;
                                                            if (!result)
                                                            {
                                                                msg = "Gagal input SPORD Detail 1";
                                                                return result;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "2")
                                            {
                                                UploadBLL.SPORDDtl2File oSPORDDtl2File = new UploadBLL.SPORDDtl2File(lines[i]);
                                                if (oSPORDDtl2File != null && oSPORDDtl2File.SalesModelCode != "")
                                                {
                                                    OmUtlSPORDDtl2 oOmUtlSPORDDtl2 = ctx.OmUtlSPORDDtl2s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSPORDHdr.BatchNo, skpNo, oSPORDDtl2File.SalesModelCode
                                                                            , oSPORDDtl2File.SalesModelYear);
                                                    if (oOmUtlSPORDDtl2 != null)
                                                    {
                                                        msg ="Kendaraan dengan SKPNo :" + skpNo + " ,SalesModelCode :" + oSPORDDtl2File.SalesModelCode +
                                                            " dan SalesModelYear :" + oSPORDDtl2File.SalesModelYear.ToString() + " sudah ada";
                                                        result = false; 
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSPORDDtl2 = new OmUtlSPORDDtl2();
                                                        oOmUtlSPORDDtl2.CompanyCode = CompanyCode;
                                                        oOmUtlSPORDDtl2.BranchCode = BranchCode;
                                                        oOmUtlSPORDDtl2.BatchNo = oOmUtlSPORDHdr.BatchNo;
                                                        oOmUtlSPORDDtl2.SKPNo = skpNo;
                                                        oOmUtlSPORDDtl2.SalesModelCode = oSPORDDtl2File.SalesModelCode;
                                                        salesModelCode = oOmUtlSPORDDtl2.SalesModelCode;
                                                        oOmUtlSPORDDtl2.SalesModelYear = oSPORDDtl2File.SalesModelYear;
                                                        salesModelYear = oSPORDDtl2File.SalesModelYear;
                                                        oOmUtlSPORDDtl2.BeforeDiscountDPP = oSPORDDtl2File.BeforeDiscDPP;
                                                        oOmUtlSPORDDtl2.BeforeDiscountPPN = oSPORDDtl2File.BeforeDiscPPN;
                                                        oOmUtlSPORDDtl2.BeforeDiscountPPNBM = oSPORDDtl2File.BeforeDiscPPNBM;
                                                        oOmUtlSPORDDtl2.BeforeDiscountTotal = oSPORDDtl2File.BeforeDiscTotal;
                                                        oOmUtlSPORDDtl2.DiscountExcludePPN = oSPORDDtl2File.DiscountExcludePPN;
                                                        oOmUtlSPORDDtl2.DiscountIncludePPN = oSPORDDtl2File.DiscountIncludePPN;
                                                        oOmUtlSPORDDtl2.AfterDiscountDPP = oSPORDDtl2File.AfterDiscDPP;
                                                        oOmUtlSPORDDtl2.AfterDiscountPPN = oSPORDDtl2File.AfterDiscPPN;
                                                        oOmUtlSPORDDtl2.AfterDiscountPPNBM = oSPORDDtl2File.AfterDiscPPNBM;
                                                        oOmUtlSPORDDtl2.AfterDiscountTotal = oSPORDDtl2File.AfterDiscTotal;
                                                        oOmUtlSPORDDtl2.PPNBMPaid = oSPORDDtl2File.PPNBMPaid;
                                                        oOmUtlSPORDDtl2.OthersDPP = oSPORDDtl2File.OthersDPP;
                                                        oOmUtlSPORDDtl2.OthersPPN = oSPORDDtl2File.OthersPPN;
                                                        oOmUtlSPORDDtl2.Quantity = oSPORDDtl2File.Quantity;
                                                        oOmUtlSPORDDtl2.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSPORDDtl2.CreatedDate = DateTime.Now;
                                                        oOmUtlSPORDDtl2.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSPORDDtl2.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSPORDDtl2s.Add(oOmUtlSPORDDtl2);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal input SPORD Detail 2";
                                                            return result;
                                                        }
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "3")
                                            {
                                                UploadBLL.SPORDDtl3File oSPORDDtl3File = new UploadBLL.SPORDDtl3File(lines[i]);
                                                if (oSPORDDtl3File != null && oSPORDDtl3File.ColourCode != "")
                                                {
                                                    OmUtlSPORDDtl3 oOmUtlSPORDDtl3 = ctx.OmUtlSPORDDtl3s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSPORDHdr.BatchNo, skpNo, salesModelCode
                                                                            , salesModelYear, oSPORDDtl3File.ColourCode);
                                                    if (oOmUtlSPORDDtl3 != null)
                                                    {
                                                        msg ="Kendaraan dengan SKPNo :" + skpNo + " ,SalesModelCode :" + salesModelCode +
                                                            " ,SalesModelYear :" + salesModelYear.ToString() + "dan Warna: " + oSPORDDtl3File.ColourCode + " sudah ada";
                                                        result = false; 
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSPORDDtl3 = new OmUtlSPORDDtl3();
                                                        oOmUtlSPORDDtl3.CompanyCode = CompanyCode;
                                                        oOmUtlSPORDDtl3.BranchCode = BranchCode;
                                                        oOmUtlSPORDDtl3.BatchNo = oOmUtlSPORDHdr.BatchNo;
                                                        oOmUtlSPORDDtl3.SKPNo = skpNo;
                                                        oOmUtlSPORDDtl3.SalesModelCode = salesModelCode;
                                                        oOmUtlSPORDDtl3.SalesModelYear = salesModelYear;
                                                        oOmUtlSPORDDtl3.ColourCode = oSPORDDtl3File.ColourCode;
                                                        oOmUtlSPORDDtl3.Quantity = oSPORDDtl3File.Quantity;
                                                        oOmUtlSPORDDtl3.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSPORDDtl3.CreatedDate = DateTime.Now;
                                                        oOmUtlSPORDDtl3.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSPORDDtl3.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSPORDDtl3s.Add(oOmUtlSPORDDtl3);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal input SPORD Detail 3";
                                                            return result;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            msg ="flat file tidak valid";
                            result = false; 
                        }
                    }
                    catch (Exception ex)
                    {
                        msg = ex.Message;
                    }
                }
            }
            else
            {
                msg ="flat file tidak valid";
                result = false; 
            }

            return result;
        }

        private bool UploadDataSDORD(string[] lines, bool isRecDCS, string kodePerusahaan)
        {
            bool result = false;
            int linesLength = 96;

            if (lines.Length <= 0)
            {
                msg ="flat file tidak ada data";
                result = false;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    try
                    {
                        UploadBLL.SDORDHdrFile oSDORDHdrFile = new UploadBLL.SDORDHdrFile(lines[0]);
                        string RcvDealerCode = (isRecDCS) ? CompanyCode : oSDORDHdrFile.RcvDealerCode;

                        if (!kodePerusahaan.Equals(oSDORDHdrFile.RcvDealerCode))
                        {
                            string msg = "Invalid flat file, kode perusahaan tidak sama";
                            if (isRecDCS) msg += "\nHarap cek setup LockingBy di CoProfile Sales";
                            result = false;
                        }

                        if (oSDORDHdrFile != null && oSDORDHdrFile.DataID == "SDORD")
                        {
                            OmUtlSDORDHdr oOmUtlSDORDHdr = ctx.OmUtlSDORDHdrs.Find(CompanyCode, BranchCode
                                                    , oSDORDHdrFile.DealerCode, RcvDealerCode, oSDORDHdrFile.BatchNo);
                            if (oOmUtlSDORDHdr != null)
                            {
                                msg ="No Batch: " + oSDORDHdrFile.BatchNo + " sudah ada";
                                result = false;
                            }
                            else
                            {
                                oOmUtlSDORDHdr = new OmUtlSDORDHdr();
                                oOmUtlSDORDHdr.CompanyCode = CompanyCode;
                                oOmUtlSDORDHdr.BranchCode = BranchCode;
                                oOmUtlSDORDHdr.DealerCode = oSDORDHdrFile.DealerCode;
                                oOmUtlSDORDHdr.RcvDealerCode = RcvDealerCode;
                                oOmUtlSDORDHdr.BatchNo = oSDORDHdrFile.BatchNo;
                                oOmUtlSDORDHdr.Status = "0";
                                oOmUtlSDORDHdr.CreatedBy = CurrentUser.UserId;
                                oOmUtlSDORDHdr.CreatedDate = DateTime.Now;
                                oOmUtlSDORDHdr.LastUpdateBy = CurrentUser.UserId;
                                oOmUtlSDORDHdr.LastUpdateDate = DateTime.Now;
                                ctx.OmUtlSDORDHdrs.Add(oOmUtlSDORDHdr);
                                result = ctx.SaveChanges() > 0;
                                if (result == true)
                                {
                                    string doNo = ""; string salesModelCode = ""; int salesModelYear = 0;
                                    for (int i = 1; i < lines.Length; i++)
                                    {
                                        if (lines[i].Length == linesLength)
                                        {
                                            if (lines[i].Substring(0, 1) == "1")
                                            {
                                                UploadBLL.SDORDDtl1File oSDORDDtl1File = new UploadBLL.SDORDDtl1File(lines[i]);
                                                if (oSDORDDtl1File != null && oSDORDDtl1File.DONo != "")
                                                {
                                                    OmUtlSDORDDtl1 oOmUtlSDORDDtl1 = ctx.OmUtlSDORDDtl1s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSDORDHdr.BatchNo, oSDORDDtl1File.DONo);
                                                    if (oOmUtlSDORDDtl1 != null)
                                                    {
                                                        msg ="No DO: " + oSDORDDtl1File.DONo + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        //added by Akhmad Nuryanto 3 Sept 2009
                                                        if (ctx.OmUtlSDORDDtl1s.Find(CompanyCode, BranchCode, oOmUtlSDORDHdr.BatchNo, oSDORDDtl1File.DONo) != null)
                                                        {
                                                            msg ="flat file data no do: " + oSDORDDtl1File.DONo + " sudah diupload";
                                                            result = false;
                                                        }
                                                        else
                                                        {
                                                            oOmUtlSDORDDtl1 = new OmUtlSDORDDtl1();
                                                            oOmUtlSDORDDtl1.CompanyCode = CompanyCode;
                                                            oOmUtlSDORDDtl1.BranchCode = BranchCode;
                                                            oOmUtlSDORDDtl1.BatchNo = oOmUtlSDORDHdr.BatchNo;
                                                            oOmUtlSDORDDtl1.DONo = oSDORDDtl1File.DONo;
                                                            doNo = oOmUtlSDORDDtl1.DONo;
                                                            oOmUtlSDORDDtl1.DODate = oSDORDDtl1File.DODate;
                                                            oOmUtlSDORDDtl1.SKPNo = oSDORDDtl1File.SKPNo;
                                                            oOmUtlSDORDDtl1.CreatedBy = CurrentUser.UserId;
                                                            oOmUtlSDORDDtl1.CreatedDate = DateTime.Now;
                                                            oOmUtlSDORDDtl1.LastUpdateBy = CurrentUser.UserId;
                                                            oOmUtlSDORDDtl1.LastUpdateDate = DateTime.Now;
                                                            oOmUtlSDORDDtl1.Status = "0";
                                                            ctx.OmUtlSDORDDtl1s.Add(oOmUtlSDORDDtl1);
                                                            result = ctx.SaveChanges() > 0;
                                                            if (!result)
                                                            {
                                                                msg ="Gagal insert SDORD detail 1";
                                                                return result;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "2")
                                            {
                                                UploadBLL.SDORDDtl2File oSDORDDtl2File = new UploadBLL.SDORDDtl2File(lines[i]);
                                                if (oSDORDDtl2File != null && oSDORDDtl2File.SalesModelCode != "")
                                                {
                                                    OmUtlSDORDDtl2 oOmUtlSDORDDtl2 = ctx.OmUtlSDORDDtl2s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSDORDHdr.BatchNo, doNo, oSDORDDtl2File.SalesModelCode
                                                                            , oSDORDDtl2File.SalesModelYear);
                                                    if (oOmUtlSDORDDtl2 != null)
                                                    {
                                                        msg ="Kendaraan dengan DO: " + doNo + " ,SalesModelCode: " + oSDORDDtl2File.SalesModelCode
                                                            + " dan SalesModelYear: " + oSDORDDtl2File.SalesModelYear.ToString() + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSDORDDtl2 = new OmUtlSDORDDtl2();
                                                        oOmUtlSDORDDtl2.CompanyCode = CompanyCode;
                                                        oOmUtlSDORDDtl2.BranchCode = BranchCode;
                                                        oOmUtlSDORDDtl2.BatchNo = oOmUtlSDORDHdr.BatchNo;
                                                        oOmUtlSDORDDtl2.DONo = doNo;
                                                        oOmUtlSDORDDtl2.SalesModelCode = oSDORDDtl2File.SalesModelCode;
                                                        salesModelCode = oOmUtlSDORDDtl2.SalesModelCode;
                                                        oOmUtlSDORDDtl2.SalesModelYear = oSDORDDtl2File.SalesModelYear;
                                                        salesModelYear = oSDORDDtl2File.SalesModelYear;
                                                        oOmUtlSDORDDtl2.Quantity = oSDORDDtl2File.Quantity;
                                                        oOmUtlSDORDDtl2.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSDORDDtl2.CreatedDate = DateTime.Now;
                                                        oOmUtlSDORDDtl2.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSDORDDtl2.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSDORDDtl2s.Add(oOmUtlSDORDDtl2);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal insert SDORD detail 2";
                                                            return result;
                                                        }
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "3")
                                            {
                                                UploadBLL.SDORDDtl3File oSDORDDtl3File = new UploadBLL.SDORDDtl3File(lines[i]);
                                                if (oSDORDDtl3File != null && oSDORDDtl3File.ColourCode != "")
                                                {
                                                    OmUtlSDORDDtl3 oOmUtlSDORDDtl3 = ctx.OmUtlSDORDDtl3s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSDORDHdr.BatchNo, doNo, salesModelCode
                                                                            , salesModelYear, oSDORDDtl3File.ColourCode, oSDORDDtl3File.ChassisCode, oSDORDDtl3File.ChassisNo);
                                                    if (oOmUtlSDORDDtl3 != null)
                                                    {
                                                        msg ="Kendaraan dengan DO: " + doNo + " ,SalesModelCode: " + salesModelCode
                                                            + " ,SalesModelYear: " + salesModelYear.ToString() + ", dan Warna: " + oSDORDDtl3File.ColourCode + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSDORDDtl3 = new OmUtlSDORDDtl3();
                                                        oOmUtlSDORDDtl3.CompanyCode = CompanyCode;
                                                        oOmUtlSDORDDtl3.BranchCode = BranchCode;
                                                        oOmUtlSDORDDtl3.BatchNo = oOmUtlSDORDHdr.BatchNo;
                                                        oOmUtlSDORDDtl3.DONo = doNo;
                                                        oOmUtlSDORDDtl3.SalesModelCode = salesModelCode;
                                                        oOmUtlSDORDDtl3.SalesModelYear = salesModelYear;
                                                        oOmUtlSDORDDtl3.ColourCode = oSDORDDtl3File.ColourCode;
                                                        oOmUtlSDORDDtl3.ChassisCode = oSDORDDtl3File.ChassisCode;
                                                        oOmUtlSDORDDtl3.ChassisNo = oSDORDDtl3File.ChassisNo;
                                                        oOmUtlSDORDDtl3.EngineCode = oSDORDDtl3File.EngineCode;
                                                        oOmUtlSDORDDtl3.EngineNo = oSDORDDtl3File.EngineNo;
                                                        oOmUtlSDORDDtl3.ServiceBookNo = oSDORDDtl3File.ServiceBookNo;
                                                        oOmUtlSDORDDtl3.KeyNo = oSDORDDtl3File.KeyNo;
                                                        oOmUtlSDORDDtl3.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSDORDDtl3.CreatedDate = DateTime.Now;
                                                        oOmUtlSDORDDtl3.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSDORDDtl3.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSDORDDtl3s.Add(oOmUtlSDORDDtl3);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal insert SDORD detail 3";
                                                            return result;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            msg ="flat file tidak valid";
                            result = false;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
            {
                msg ="flat file tidak valid";
                result = false;
            }

            return result;
        }

        private bool UploadDataSSJAL(string[] lines, bool isRecDCS, string kodePerusahaan)
        {
            bool result = false;
            int linesLength = 96;
            bool revisi = false;
            string bpuNo = "", poNo = "";

            if (lines.Length <= 0)
            {
                msg ="flat file tidak ada data";
                return false;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    try
                    {
                        UploadBLL.SSJALHdrFile oSSJALHdrFile = new UploadBLL.SSJALHdrFile(lines[0]);
                        string RcvDealerCode = (isRecDCS) ? CompanyCode : oSSJALHdrFile.RcvDealerCode;

                        if (!kodePerusahaan.Equals(oSSJALHdrFile.RcvDealerCode))
                        {
                            string msg = "Invalid flat file, kode perusahaan tidak sama";
                            if (isRecDCS) msg += "\nHarap cek setup LockingBy di CoProfile Sales";
                            result = false;
                        }

                        if (oSSJALHdrFile != null && oSSJALHdrFile.DataID == "SSJAL")
                        {
                            OmUtlSSJALHdr oOmUtlSSJALHdr = ctx.OmUtlSSJALHdrs.Find(CompanyCode, BranchCode
                                                    , oSSJALHdrFile.DealerCode, RcvDealerCode, oSSJALHdrFile.BatchNo);
                            if (oOmUtlSSJALHdr != null)
                            {
                                msg ="No Batch: " + oSSJALHdrFile.BatchNo + " sudah ada";
                                result = false;
                            }
                            else
                            {
                                oOmUtlSSJALHdr = new OmUtlSSJALHdr();
                                oOmUtlSSJALHdr.CompanyCode = CompanyCode;
                                oOmUtlSSJALHdr.BranchCode = BranchCode;
                                oOmUtlSSJALHdr.DealerCode = oSSJALHdrFile.DealerCode;
                                oOmUtlSSJALHdr.RcvDealerCode = RcvDealerCode;
                                oOmUtlSSJALHdr.BatchNo = oSSJALHdrFile.BatchNo;
                                oOmUtlSSJALHdr.Status = "0";
                                oOmUtlSSJALHdr.CreatedBy = CurrentUser.UserId;
                                oOmUtlSSJALHdr.CreatedDate = DateTime.Now;
                                oOmUtlSSJALHdr.LastUpdateBy = CurrentUser.UserId;
                                oOmUtlSSJALHdr.LastUpdateDate = DateTime.Now;
                                ctx.OmUtlSSJALHdrs.Add(oOmUtlSSJALHdr);
                                result = ctx.SaveChanges() > 0;
                                if (result == true)
                                {
                                    string sjNo = ""; string salesModelCode = ""; int salesModelYear = 0;
                                    for (int i = 1; i < lines.Length; i++)
                                    {
                                        if (lines[i].Length == linesLength)
                                        {
                                            if (lines[i].Substring(0, 1) == "1")
                                            {
                                                UploadBLL.SSJALDtl1File oSSJALDtl1File = new UploadBLL.SSJALDtl1File(lines[i]);
                                                if (oSSJALDtl1File != null && oSSJALDtl1File.SJNo != "")
                                                {
                                                    OmUtlSSJALDtl1 oOmUtlSSJALDtl1 = ctx.OmUtlSSJALDtl1s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSSJALHdr.BatchNo, oSSJALDtl1File.SJNo);
                                                    if (oOmUtlSSJALDtl1 != null)
                                                    {
                                                        msg ="No SJ: " + oSSJALDtl1File.SJNo + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        if (ctx.OmUtlSSJALDtl1s.Find(CompanyCode, BranchCode, oOmUtlSSJALHdr.BatchNo, oSSJALDtl1File.SJNo)!= null)
                                                        {
                                                            msg ="flat file data No SJ: " + oSSJALDtl1File.SJNo + " sudah diupload";
                                                            result = false;
                                                        }
                                                        else
                                                        {
                                                            oOmUtlSSJALDtl1 = new OmUtlSSJALDtl1();
                                                            oOmUtlSSJALDtl1.CompanyCode = CompanyCode;
                                                            oOmUtlSSJALDtl1.BranchCode = BranchCode;
                                                            oOmUtlSSJALDtl1.BatchNo = oOmUtlSSJALHdr.BatchNo;
                                                            oOmUtlSSJALDtl1.SJNo = oSSJALDtl1File.SJNo;
                                                            sjNo = oOmUtlSSJALDtl1.SJNo;
                                                            oOmUtlSSJALDtl1.SJDate = oSSJALDtl1File.SJDate;
                                                            oOmUtlSSJALDtl1.SKPNo = oSSJALDtl1File.SKPNo;
                                                            oOmUtlSSJALDtl1.DONo = oSSJALDtl1File.DONo;
                                                            oOmUtlSSJALDtl1.DODate = oSSJALDtl1File.DODate;
                                                            oOmUtlSSJALDtl1.CreatedBy = CurrentUser.UserId;
                                                            oOmUtlSSJALDtl1.CreatedDate = DateTime.Now;
                                                            oOmUtlSSJALDtl1.LastUpdateBy = CurrentUser.UserId;
                                                            oOmUtlSSJALDtl1.LastUpdateDate = DateTime.Now;
                                                            oOmUtlSSJALDtl1.IsBlokir = oSSJALDtl1File.IsBlokir;

                                                            if (oSSJALDtl1File.FlagRevisi == "R")
                                                            {
                                                                if (ProductType == "4W")
                                                                {
//                                                                    
                                                                    var row = GetBPUNobyReffFJ(oSSJALDtl1File.SJNo);
                                                                    if (row != null)
                                                                    {
                                                                        bpuNo = row.BPUNo;
                                                                        poNo = row.PONo;
                                                                        oOmUtlSSJALDtl1.Status = "1";
                                                                        revisi = true;
                                                                    }
                                                                    else
                                                                        oOmUtlSSJALDtl1.Status = "0";
                                                                }
                                                                else
                                                                {
                                                                    result = false;
                                                                    return false;
                                                                }
                                                            }
                                                            else
                                                                oOmUtlSSJALDtl1.Status = "0";
                                                            ctx.OmUtlSSJALDtl1s.Add(oOmUtlSSJALDtl1);
                                                            result = ctx.SaveChanges() > 0;
                                                            if (!result)
                                                            {
                                                                msg ="Gagal insert SSJAL Detail 1";
                                                                return result;
                                                            }
                                                        }

                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "2")
                                            {
                                                UploadBLL.SSJALDtl2File oSSJALDtl2File = new UploadBLL.SSJALDtl2File(lines[i]);
                                                if (oSSJALDtl2File != null && oSSJALDtl2File.SalesModelCode != "")
                                                {
                                                    OmUtlSSJALDtl2 oOmUtlSSJALDtl2 = ctx.OmUtlSSJALDtl2s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSSJALHdr.BatchNo, sjNo, oSSJALDtl2File.SalesModelCode
                                                                            , oSSJALDtl2File.SalesModelYear);
                                                    if (oOmUtlSSJALDtl2 != null)
                                                    {
                                                        msg ="Kendaraan dengan SJ: " + sjNo + " ,SalesModelCode: " + oSSJALDtl2File.SalesModelCode
                                                            + " dan SalesModelYear: " + oSSJALDtl2File.SalesModelYear.ToString() + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSSJALDtl2 = new OmUtlSSJALDtl2();
                                                        oOmUtlSSJALDtl2.CompanyCode = CompanyCode;
                                                        oOmUtlSSJALDtl2.BranchCode = BranchCode;
                                                        oOmUtlSSJALDtl2.BatchNo = oOmUtlSSJALHdr.BatchNo;
                                                        oOmUtlSSJALDtl2.SJNo = sjNo;
                                                        oOmUtlSSJALDtl2.SalesModelCode = oSSJALDtl2File.SalesModelCode;
                                                        salesModelCode = oOmUtlSSJALDtl2.SalesModelCode;
                                                        oOmUtlSSJALDtl2.SalesModelYear = oSSJALDtl2File.SalesModelYear;
                                                        salesModelYear = oSSJALDtl2File.SalesModelYear;
                                                        oOmUtlSSJALDtl2.Quantity = oSSJALDtl2File.Quantity;
                                                        oOmUtlSSJALDtl2.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSSJALDtl2.CreatedDate = DateTime.Now;
                                                        oOmUtlSSJALDtl2.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSSJALDtl2.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSSJALDtl2s.Add(oOmUtlSSJALDtl2);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal insert SSJAL Detail 2";
                                                            return result;
                                                        }
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "3")
                                            {
                                                UploadBLL.SSJALDtl3File oSSJALDtl3File = new UploadBLL.SSJALDtl3File(lines[i]);
                                                if (oSSJALDtl3File != null && oSSJALDtl3File.ColourCode != "")
                                                {
                                                    OmUtlSSJALDtl3 oOmUtlSSJALDtl3 = ctx.OmUtlSSJALDtl3s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSSJALHdr.BatchNo, sjNo, salesModelCode
                                                                            , salesModelYear, oSSJALDtl3File.ColourCode, oSSJALDtl3File.ChassisCode, oSSJALDtl3File.ChassisNo);
                                                    if (oOmUtlSSJALDtl3 != null)
                                                    {
                                                        msg ="Kendaraan dengan ChassisCode: " + oSSJALDtl3File.ChassisCode + " dan ChassisNo: " + oSSJALDtl3File.ChassisNo.ToString() + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSSJALDtl3 = new OmUtlSSJALDtl3();
                                                        oOmUtlSSJALDtl3.CompanyCode = CompanyCode;
                                                        oOmUtlSSJALDtl3.BranchCode = BranchCode;
                                                        oOmUtlSSJALDtl3.BatchNo = oOmUtlSSJALHdr.BatchNo;
                                                        oOmUtlSSJALDtl3.SJNo = sjNo;
                                                        oOmUtlSSJALDtl3.SalesModelCode = salesModelCode;
                                                        oOmUtlSSJALDtl3.SalesModelYear = salesModelYear;
                                                        oOmUtlSSJALDtl3.ColourCode = oSSJALDtl3File.ColourCode;
                                                        oOmUtlSSJALDtl3.ChassisCode = oSSJALDtl3File.ChassisCode;
                                                        oOmUtlSSJALDtl3.ChassisNo = oSSJALDtl3File.ChassisNo;
                                                        oOmUtlSSJALDtl3.EngineCode = oSSJALDtl3File.EngineCode;
                                                        oOmUtlSSJALDtl3.EngineNo = oSSJALDtl3File.EngineNo;
                                                        oOmUtlSSJALDtl3.ServiceBookNo = oSSJALDtl3File.ServiceBookNo;
                                                        oOmUtlSSJALDtl3.KeyNo = oSSJALDtl3File.KeyNo;
                                                        oOmUtlSSJALDtl3.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSSJALDtl3.CreatedDate = DateTime.Now;
                                                        oOmUtlSSJALDtl3.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSSJALDtl3.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSSJALDtl3s.Add(oOmUtlSSJALDtl3);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal insert SSJAL Detail 3";
                                                            return result;
                                                        }
                                                    }

                                                    if (bpuNo != "")
                                                    {
                                                        omTrPurchaseBPUDetail oOmTrPurchaseBPUDetail = ctx.omTrPurchaseBPUDetail.Find(CompanyCode, BranchCode, poNo, bpuNo, 1);
                                                        if (oOmTrPurchaseBPUDetail != null)
                                                        {
                                                            oOmTrPurchaseBPUDetail.ChassisNo = oOmUtlSSJALDtl3.ChassisNo;
                                                            oOmTrPurchaseBPUDetail.EngineNo = oOmUtlSSJALDtl3.EngineNo;
                                                            oOmTrPurchaseBPUDetail.KeyNo = oOmUtlSSJALDtl3.KeyNo;
                                                            oOmTrPurchaseBPUDetail.ServiceBookNo = oOmUtlSSJALDtl3.ServiceBookNo;
                                                            oOmTrPurchaseBPUDetail.LastUpdateBy = CurrentUser.UserId;
                                                            oOmTrPurchaseBPUDetail.LastUpdateDate = DateTime.Now;

                                                            result = ctx.SaveChanges() > 0;
                                                            if (!result)
                                                            {
                                                                msg ="Gagal Update SSJAL Detail 3";
                                                                return result;
                                                            }
                                                        }
                                                        bpuNo = "";
                                                        poNo = "";
                                                        revisi = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            msg ="flat file tidak valid";
                            result = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
            else
            {
                msg ="flat file tidak valid";
                result = false;
            }

            return result;
        }

        private bool UploadDataSSJALBackUp(string[] lines, bool isRecDCS, string kodePerusahaan)
        {
            bool result = false;
            int linesLength = 96;
            bool revisi = false;
            string bpuNo = "", poNo = "";

            if (lines.Length <= 0)
            {
                msg ="flat file tidak ada data";
                return false;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    try
                    {
                        UploadBLL.SSJALHdrFile oSSJALHdrFile = new UploadBLL.SSJALHdrFile(lines[0]);
                        string RcvDealerCode = (isRecDCS) ? CompanyCode : oSSJALHdrFile.RcvDealerCode;

                        if (!kodePerusahaan.Equals(oSSJALHdrFile.RcvDealerCode))
                        {
                            string msg = "Invalid flat file, kode perusahaan tidak sama";
                            if (isRecDCS) msg += "\nHarap cek setup LockingBy di CoProfile Sales";
                            result = false;
                        }

                        if (oSSJALHdrFile != null && oSSJALHdrFile.DataID == "SSJAL")
                        {
                            OmUtlSSJALHdr oOmUtlSSJALHdr = ctx.OmUtlSSJALHdrs.Find(CompanyCode, BranchCode
                                                    , oSSJALHdrFile.DealerCode, RcvDealerCode, oSSJALHdrFile.BatchNo);
                            if (oOmUtlSSJALHdr != null)
                            {
                                msg ="No Batch: " + oSSJALHdrFile.BatchNo + " sudah ada";
                                result = false;
                            }
                            else
                            {
                                oOmUtlSSJALHdr = new OmUtlSSJALHdr();
                                oOmUtlSSJALHdr.CompanyCode = CompanyCode;
                                oOmUtlSSJALHdr.BranchCode = BranchCode;
                                oOmUtlSSJALHdr.DealerCode = oSSJALHdrFile.DealerCode;
                                oOmUtlSSJALHdr.RcvDealerCode = RcvDealerCode;
                                oOmUtlSSJALHdr.BatchNo = oSSJALHdrFile.BatchNo;
                                oOmUtlSSJALHdr.Status = "0";
                                oOmUtlSSJALHdr.CreatedBy = CurrentUser.UserId;
                                oOmUtlSSJALHdr.CreatedDate = DateTime.Now;
                                oOmUtlSSJALHdr.LastUpdateBy = CurrentUser.UserId;
                                oOmUtlSSJALHdr.LastUpdateDate = DateTime.Now;
                                ctx.OmUtlSSJALHdrs.Add(oOmUtlSSJALHdr);
                                result = ctx.SaveChanges() > 0;
                                if (result == true)
                                {
                                    string sjNo = ""; string salesModelCode = ""; int salesModelYear = 0;
                                    for (int i = 1; i < lines.Length; i++)
                                    {
                                        if (lines[i].Length == linesLength)
                                        {
                                            if (lines[i].Substring(0, 1) == "1")
                                            {
                                                UploadBLL.SSJALDtl1File oSSJALDtl1File = new UploadBLL.SSJALDtl1File(lines[i]);
                                                if (oSSJALDtl1File != null && oSSJALDtl1File.SJNo != "")
                                                {
                                                    OmUtlSSJALDtl1 oOmUtlSSJALDtl1 = ctx.OmUtlSSJALDtl1s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSSJALHdr.BatchNo, oSSJALDtl1File.SJNo);
                                                    sjNo = oSSJALDtl1File.SJNo;

                                                    if (oOmUtlSSJALDtl1 != null)
                                                    {
                                                        msg ="BatchNo : " + oOmUtlSSJALHdr.BatchNo + " dan No SJ: " + oSSJALDtl1File.SJNo + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSSJALDtl1 = new OmUtlSSJALDtl1();
                                                        oOmUtlSSJALDtl1.CompanyCode = CompanyCode;
                                                        oOmUtlSSJALDtl1.BranchCode = BranchCode;
                                                        oOmUtlSSJALDtl1.BatchNo = oOmUtlSSJALHdr.BatchNo;
                                                        oOmUtlSSJALDtl1.SJNo = oSSJALDtl1File.SJNo;
                                                        oOmUtlSSJALDtl1.SJDate = oSSJALDtl1File.SJDate;
                                                        oOmUtlSSJALDtl1.SKPNo = oSSJALDtl1File.SKPNo;
                                                        oOmUtlSSJALDtl1.DONo = oSSJALDtl1File.DONo;
                                                        oOmUtlSSJALDtl1.DODate = oSSJALDtl1File.DODate;
                                                        oOmUtlSSJALDtl1.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSSJALDtl1.CreatedDate = DateTime.Now;
                                                        oOmUtlSSJALDtl1.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSSJALDtl1.LastUpdateDate = DateTime.Now;
                                                        oOmUtlSSJALDtl1.IsBlokir = oSSJALDtl1File.IsBlokir;
                                                        oOmUtlSSJALDtl1.Status = "0";
                                                        ctx.OmUtlSSJALDtl1s.Add(oOmUtlSSJALDtl1);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal insert SSJAL Detail 1";
                                                            return result;
                                                        }

                                                        if (oSSJALDtl1File.FlagRevisi == "R")
                                                        {
                                                            if (ProductType == "4W")
                                                            {
                                                                var row = GetBPUNobyReffFJ(oSSJALDtl1File.SJNo);
                                                                if (row != null)
                                                                {
                                                                    bpuNo = row.BPUNo;
                                                                    poNo = row.PONo;
                                                                    revisi = true;
                                                                }

                                                            }
                                                            else
                                                            {
                                                                result = false;
                                                                return false;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "2")
                                            {
                                                UploadBLL.SSJALDtl2File oSSJALDtl2File = new UploadBLL.SSJALDtl2File(lines[i]);
                                                if (oSSJALDtl2File != null && oSSJALDtl2File.SalesModelCode != "")
                                                {
                                                    OmUtlSSJALDtl2 oOmUtlSSJALDtl2 = ctx.OmUtlSSJALDtl2s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSSJALHdr.BatchNo, sjNo, oSSJALDtl2File.SalesModelCode
                                                                            , oSSJALDtl2File.SalesModelYear);
                                                    if (oOmUtlSSJALDtl2 != null)
                                                    {
                                                        if (!revisi)
                                                        {
                                                            msg ="Kendaraan dengan SJ: " + sjNo + " ,SalesModelCode: " + oSSJALDtl2File.SalesModelCode
                                                                + " dan SalesModelYear: " + oSSJALDtl2File.SalesModelYear.ToString() + " sudah ada";
                                                            result = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                       var existsSSJALDtl2 = ctx.OmUtlSSJALDtl2s.FirstOrDefault(a=>a.CompanyCode == CompanyCode && a.BranchCode == BranchCode 
                                                           && a.SJNo == sjNo && a.SalesModelCode ==oSSJALDtl2File.SalesModelCode && a.SalesModelYear ==  oSSJALDtl2File.SalesModelYear) !=null;

                                                        if (!existsSSJALDtl2)
                                                        {
                                                            oOmUtlSSJALDtl2 = new OmUtlSSJALDtl2();
                                                            oOmUtlSSJALDtl2.CompanyCode = CompanyCode;
                                                            oOmUtlSSJALDtl2.BranchCode = BranchCode;
                                                            oOmUtlSSJALDtl2.BatchNo = oOmUtlSSJALHdr.BatchNo;
                                                            oOmUtlSSJALDtl2.SJNo = sjNo;
                                                            oOmUtlSSJALDtl2.SalesModelCode = oSSJALDtl2File.SalesModelCode;
                                                            salesModelCode = oOmUtlSSJALDtl2.SalesModelCode;
                                                            oOmUtlSSJALDtl2.SalesModelYear = oSSJALDtl2File.SalesModelYear;
                                                            salesModelYear = oSSJALDtl2File.SalesModelYear;
                                                            oOmUtlSSJALDtl2.Quantity = oSSJALDtl2File.Quantity;
                                                            oOmUtlSSJALDtl2.CreatedBy = CurrentUser.UserId;
                                                            oOmUtlSSJALDtl2.CreatedDate = DateTime.Now;
                                                            oOmUtlSSJALDtl2.LastUpdateBy = CurrentUser.UserId;
                                                            oOmUtlSSJALDtl2.LastUpdateDate = DateTime.Now;
                                                            ctx.OmUtlSSJALDtl2s.Add(oOmUtlSSJALDtl2);
                                                            result = ctx.SaveChanges() > 0;
                                                            if (!result)
                                                            {
                                                                msg ="Gagal insert SSJAL Detail 2";
                                                                return result;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "3")
                                            {
                                                UploadBLL.SSJALDtl3File oSSJALDtl3File = new UploadBLL.SSJALDtl3File(lines[i]);
                                                if (oSSJALDtl3File != null && oSSJALDtl3File.ColourCode != "")
                                                {
                                                    OmUtlSSJALDtl3 oOmUtlSSJALDtl3 = ctx.OmUtlSSJALDtl3s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSSJALHdr.BatchNo, sjNo, salesModelCode
                                                                            , salesModelYear, oSSJALDtl3File.ColourCode, oSSJALDtl3File.ChassisCode, oSSJALDtl3File.ChassisNo);
                                                    if (oOmUtlSSJALDtl3 != null)
                                                    {
                                                        if (!revisi)
                                                        {
                                                            msg ="Kendaraan dengan ChassisCode: " + oSSJALDtl3File.ChassisCode + " dan ChassisNo: " + oSSJALDtl3File.ChassisNo.ToString() + " sudah ada";
                                                            result = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var existSSJLDtl = ctx.OmUtlSSJALDtl3s.FirstOrDefault(a=>a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.SJNo == sjNo 
                                                            && a.SalesModelCode == salesModelCode && a.SalesModelYear == salesModelYear && a.ColourCode ==oSSJALDtl3File.ColourCode && a.ChassisNo == oSSJALDtl3File.ChassisNo) != null;

                                                        if (!existSSJLDtl)
                                                        {
                                                            oOmUtlSSJALDtl3 = new OmUtlSSJALDtl3();
                                                            oOmUtlSSJALDtl3.CompanyCode = CompanyCode;
                                                            oOmUtlSSJALDtl3.BranchCode = BranchCode;
                                                            oOmUtlSSJALDtl3.BatchNo = oOmUtlSSJALHdr.BatchNo;
                                                            oOmUtlSSJALDtl3.SJNo = sjNo;
                                                            oOmUtlSSJALDtl3.SalesModelCode = salesModelCode;
                                                            oOmUtlSSJALDtl3.SalesModelYear = salesModelYear;
                                                            oOmUtlSSJALDtl3.ColourCode = oSSJALDtl3File.ColourCode;
                                                            oOmUtlSSJALDtl3.ChassisCode = oSSJALDtl3File.ChassisCode;
                                                            oOmUtlSSJALDtl3.ChassisNo = oSSJALDtl3File.ChassisNo;
                                                            oOmUtlSSJALDtl3.EngineCode = oSSJALDtl3File.EngineCode;
                                                            oOmUtlSSJALDtl3.EngineNo = oSSJALDtl3File.EngineNo;
                                                            oOmUtlSSJALDtl3.ServiceBookNo = oSSJALDtl3File.ServiceBookNo;
                                                            oOmUtlSSJALDtl3.KeyNo = oSSJALDtl3File.KeyNo;
                                                            oOmUtlSSJALDtl3.CreatedBy = CurrentUser.UserId;
                                                            oOmUtlSSJALDtl3.CreatedDate = DateTime.Now;
                                                            oOmUtlSSJALDtl3.LastUpdateBy = CurrentUser.UserId;
                                                            oOmUtlSSJALDtl3.LastUpdateDate = DateTime.Now;
                                                            ctx.OmUtlSSJALDtl3s.Add(oOmUtlSSJALDtl3);
                                                            result = ctx.SaveChanges() > 0;
                                                            if (!result)
                                                            {
                                                                msg ="Gagal insert SSJAL Detail 3";
                                                                return result;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            oOmUtlSSJALDtl3 = ctx.OmUtlSSJALDtl3s.Find(CompanyCode, BranchCode, sjNo, salesModelCode, salesModelYear.ToString(), oSSJALDtl3File.ColourCode, oSSJALDtl3File.ChassisCode);
                                                            oOmUtlSSJALDtl3.ChassisNo = oSSJALDtl3File.ChassisNo;
                                                            oOmUtlSSJALDtl3.EngineNo = oSSJALDtl3File.EngineNo;
                                                            oOmUtlSSJALDtl3.KeyNo = oSSJALDtl3File.KeyNo;
                                                            oOmUtlSSJALDtl3.ServiceBookNo = oSSJALDtl3File.ServiceBookNo;
                                                            oOmUtlSSJALDtl3.LastUpdateBy = CurrentUser.UserId;
                                                            oOmUtlSSJALDtl3.LastUpdateDate = DateTime.Now;

                                                            result = ctx.SaveChanges() > 0;
                                                            if (!result)
                                                            {
                                                                msg ="Gagal Update SSJAL Detail 3";
                                                                return result;
                                                            }
                                                            else
                                                            {
                                                                if (bpuNo != "")
                                                                {
                                                                    omTrPurchaseBPUDetail oOmTrPurchaseBPUDetail = ctx.omTrPurchaseBPUDetail.Find(CompanyCode, BranchCode, poNo, bpuNo, 1);
                                                                    if (oOmTrPurchaseBPUDetail != null)
                                                                    {
                                                                        oOmTrPurchaseBPUDetail.ChassisNo = oOmUtlSSJALDtl3.ChassisNo;
                                                                        oOmTrPurchaseBPUDetail.EngineNo = oOmUtlSSJALDtl3.EngineNo;
                                                                        oOmTrPurchaseBPUDetail.KeyNo = oOmUtlSSJALDtl3.KeyNo;
                                                                        oOmTrPurchaseBPUDetail.ServiceBookNo = oOmUtlSSJALDtl3.ServiceBookNo;
                                                                        oOmTrPurchaseBPUDetail.LastUpdateBy = CurrentUser.UserId;
                                                                        oOmTrPurchaseBPUDetail.LastUpdateDate = DateTime.Now;

                                                                        result = ctx.SaveChanges() > 0;
                                                                        if (!result)
                                                                        {
                                                                            msg ="Gagal Update SSJAL Detail 3";
                                                                            return result;
                                                                        }
                                                                    }
                                                                }

                                                            }
                                                        }
                                                        bpuNo = "";
                                                        poNo = "";
                                                        revisi = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            msg ="flat file tidak valid";
                            result = false;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
            {
                msg ="flat file tidak valid";
                result = false;
            }

            return result;
        }

        private bool UploadDataSHPOK(string[] lines, bool isRecDCS, string kodePerusahaan)
        {
            bool result = false;
            int linesLength = 188;

            if (lines.Length <= 0)
            {
                msg ="flat file tidak ada data";
                return false;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    try
                    {
                        UploadBLL.SHPOKHdrFile oSHPOKHdrFile = new UploadBLL.SHPOKHdrFile(lines[0]);
                        string RcvDealerCode = (isRecDCS) ? CompanyCode : oSHPOKHdrFile.RcvDealerCode;

                        if (!kodePerusahaan.Equals(oSHPOKHdrFile.RcvDealerCode))
                        {
                            string msg = "Invalid flat file, kode perusahaan tidak sama";
                            if (isRecDCS) msg += "\nHarap cek setup LockingBy di CoProfile Sales";
                            result = false;
                        }

                        if (oSHPOKHdrFile != null && oSHPOKHdrFile.DataID == "SHPOK")
                        {
                            OmUtlSHPOKHdr oOmUtlSHPOKHdr = ctx.OmUtlSHPOKHdrs.Find(CompanyCode, BranchCode
                                                    , oSHPOKHdrFile.DealerCode, RcvDealerCode, oSHPOKHdrFile.BatchNo);
                            if (oOmUtlSHPOKHdr != null)
                            {
                                msg ="No. Batch: " + oSHPOKHdrFile.BatchNo + " sudah ada";
                                result = false;
                            }
                            else
                            {
                                oOmUtlSHPOKHdr = new OmUtlSHPOKHdr();
                                oOmUtlSHPOKHdr.CompanyCode = CompanyCode;
                                oOmUtlSHPOKHdr.BranchCode = BranchCode;
                                oOmUtlSHPOKHdr.DealerCode = oSHPOKHdrFile.DealerCode;
                                oOmUtlSHPOKHdr.RcvDealerCode = RcvDealerCode;
                                oOmUtlSHPOKHdr.BatchNo = oSHPOKHdrFile.BatchNo;
                                oOmUtlSHPOKHdr.Status = "0";
                                oOmUtlSHPOKHdr.CreatedBy = CurrentUser.UserId;
                                oOmUtlSHPOKHdr.CreatedDate = DateTime.Now;
                                oOmUtlSHPOKHdr.LastUpdateBy = CurrentUser.UserId;
                                oOmUtlSHPOKHdr.LastUpdateDate = DateTime.Now;
                                ctx.OmUtlSHPOKHdrs.Add(oOmUtlSHPOKHdr);
                                result = ctx.SaveChanges() > 0;
                                if (result == true)
                                {
                                    string invoiceNo = ""; string documentNo = ""; string salesModelCode = ""; int salesModelYear = 0;
                                    for (int i = 1; i < lines.Length; i++)
                                    {
                                        if (lines[i].Length == linesLength)
                                        {
                                            if (lines[i].Substring(0, 1) == "1")
                                            {
                                                UploadBLL.SHPOKDtl1File oSHPOKDtl1File = new UploadBLL.SHPOKDtl1File(lines[i]);
                                                if (oSHPOKDtl1File != null && oSHPOKDtl1File.InvoiceNo != "")
                                                {
                                                    OmUtlSHPOKDtl1 oOmUtlSHPOKDtl1 = ctx.OmUtlSHPOKDtl1s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSHPOKHdr.BatchNo, oSHPOKDtl1File.InvoiceNo);
                                                    if (oOmUtlSHPOKDtl1 != null)
                                                    {
                                                        msg ="No. Invoice: " + oSHPOKDtl1File.InvoiceNo + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSHPOKDtl1 = new OmUtlSHPOKDtl1();
                                                        oOmUtlSHPOKDtl1.CompanyCode = CompanyCode;
                                                        oOmUtlSHPOKDtl1.BranchCode = BranchCode;
                                                        oOmUtlSHPOKDtl1.BatchNo = oOmUtlSHPOKHdr.BatchNo;
                                                        oOmUtlSHPOKDtl1.InvoiceNo = oSHPOKDtl1File.InvoiceNo;
                                                        invoiceNo = oOmUtlSHPOKDtl1.InvoiceNo;
                                                        oOmUtlSHPOKDtl1.InvoiceDate = oSHPOKDtl1File.InvoiceDate;
                                                        oOmUtlSHPOKDtl1.SKPNo = oSHPOKDtl1File.SKPNo;
                                                        oOmUtlSHPOKDtl1.FakturPajakNo = oSHPOKDtl1File.FakturPajakNo;
                                                        oOmUtlSHPOKDtl1.FakturPajakDate = oSHPOKDtl1File.FakturPajakDate;
                                                        oOmUtlSHPOKDtl1.Remark = oSHPOKDtl1File.Remark;
                                                        oOmUtlSHPOKDtl1.DueDate = oSHPOKDtl1File.DueDate;
                                                        oOmUtlSHPOKDtl1.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSHPOKDtl1.CreatedDate = DateTime.Now;
                                                        oOmUtlSHPOKDtl1.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSHPOKDtl1.LastUpdateDate = DateTime.Now;
                                                        oOmUtlSHPOKDtl1.Status = "0";
                                                        ctx.OmUtlSHPOKDtl1s.Add(oOmUtlSHPOKDtl1);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal insert SHPOK detail 1";
                                                            return result;
                                                        }
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "2")
                                            {
                                                UploadBLL.SHPOKDtl2File oSHPOKDtl2File = new UploadBLL.SHPOKDtl2File(lines[i]);
                                                if (oSHPOKDtl2File != null && oSHPOKDtl2File.DocNo != "")
                                                {
                                                    OmUtlSHPOKDtl2 oOmUtlSHPOKDtl2 = ctx.OmUtlSHPOKDtl2s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSHPOKHdr.BatchNo, invoiceNo, oSHPOKDtl2File.DocNo
                                                                            );
                                                    if (oOmUtlSHPOKDtl2 != null)
                                                    {
                                                        msg ="No. Invoice: " + invoiceNo + " dan DocNo: " + oSHPOKDtl2File.DocNo + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSHPOKDtl2 = new OmUtlSHPOKDtl2();
                                                        oOmUtlSHPOKDtl2.CompanyCode = CompanyCode;
                                                        oOmUtlSHPOKDtl2.BranchCode = BranchCode;
                                                        oOmUtlSHPOKDtl2.BatchNo = oOmUtlSHPOKHdr.BatchNo;
                                                        oOmUtlSHPOKDtl2.InvoiceNo = invoiceNo;
                                                        oOmUtlSHPOKDtl2.DocumentNo = oSHPOKDtl2File.DocNo;
                                                        documentNo = oSHPOKDtl2File.DocNo;
                                                        oOmUtlSHPOKDtl2.DocumentType = oSHPOKDtl2File.DocType;
                                                        oOmUtlSHPOKDtl2.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSHPOKDtl2.CreatedDate = DateTime.Now;
                                                        oOmUtlSHPOKDtl2.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSHPOKDtl2.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSHPOKDtl2s.Add(oOmUtlSHPOKDtl2);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal insert SHPOK detail 2";
                                                            return result;
                                                        }
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "3")
                                            {
                                                UploadBLL.SHPOKDtl3File oSHPOKDtl3File = new UploadBLL.SHPOKDtl3File(lines[i]);
                                                if (oSHPOKDtl3File != null && oSHPOKDtl3File.SalesModelCode != "")
                                                {
                                                    OmUtlSHPOKDtl3 oOmUtlSHPOKDtl3 = ctx.OmUtlSHPOKDtl3s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSHPOKHdr.BatchNo, invoiceNo, documentNo, oSHPOKDtl3File.SalesModelCode
                                                                            , oSHPOKDtl3File.SalesModelYear.ToString());
                                                    if (oOmUtlSHPOKDtl3 != null)
                                                    {
                                                        msg ="Kendaraan dengan Invoice: " + invoiceNo + " ,DocNo: " + documentNo
                                                            + " ,SalesModelCode: " + oSHPOKDtl3File.SalesModelCode + " dan SalesModelYear: " + oSHPOKDtl3File.SalesModelYear.ToString() + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSHPOKDtl3 = new OmUtlSHPOKDtl3();
                                                        oOmUtlSHPOKDtl3.CompanyCode = CompanyCode;
                                                        oOmUtlSHPOKDtl3.BranchCode = BranchCode;
                                                        oOmUtlSHPOKDtl3.BatchNo = oOmUtlSHPOKHdr.BatchNo;
                                                        oOmUtlSHPOKDtl3.InvoiceNo = invoiceNo;
                                                        oOmUtlSHPOKDtl3.DocumentNo = documentNo;
                                                        oOmUtlSHPOKDtl3.SalesModelCode = oSHPOKDtl3File.SalesModelCode;
                                                        salesModelCode = oOmUtlSHPOKDtl3.SalesModelCode;
                                                        oOmUtlSHPOKDtl3.SalesModelYear = oSHPOKDtl3File.SalesModelYear;
                                                        salesModelYear = oSHPOKDtl3File.SalesModelYear;
                                                        oOmUtlSHPOKDtl3.Quantity = oSHPOKDtl3File.Quantity;
                                                        oOmUtlSHPOKDtl3.BeforeDiscountDPP = oSHPOKDtl3File.BeforeDiscDPP;
                                                        oOmUtlSHPOKDtl3.DiscountExcludePPN = oSHPOKDtl3File.DiscountExcludePPN;
                                                        oOmUtlSHPOKDtl3.AfterDiscountDPP = oSHPOKDtl3File.AfterDiscDPP;
                                                        oOmUtlSHPOKDtl3.AfterDiscountPPN = oSHPOKDtl3File.AfterDiscPPN;
                                                        oOmUtlSHPOKDtl3.AfterDiscountPPNBM = oSHPOKDtl3File.AfterDiscPPNBM;
                                                        oOmUtlSHPOKDtl3.AfterDiscountTotal = oSHPOKDtl3File.AfterDiscTotal;
                                                        oOmUtlSHPOKDtl3.PPNBMPaid = oSHPOKDtl3File.PPNBMPaid;
                                                        oOmUtlSHPOKDtl3.OthersDPP = oSHPOKDtl3File.OthersDPP;
                                                        oOmUtlSHPOKDtl3.OthersPPN = oSHPOKDtl3File.OthersPPN;
                                                        oOmUtlSHPOKDtl3.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSHPOKDtl3.CreatedDate = DateTime.Now;
                                                        oOmUtlSHPOKDtl3.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSHPOKDtl3.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSHPOKDtl3s.Add(oOmUtlSHPOKDtl3);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal insert SHPOK detail 3";
                                                            return result;
                                                        }
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "O")
                                            {
                                                UploadBLL.SHPOKDtlOFile oSHPOKDtlOFile = new UploadBLL. SHPOKDtlOFile(lines[i]);
                                                if (oSHPOKDtlOFile != null)
                                                {
                                                    OmUtlSHPOKDtlO oOmUtlSHPOKDtlO = ctx.OmUtlSHPOKDtlOs.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSHPOKHdr.BatchNo, invoiceNo, documentNo, salesModelCode
                                                                            , salesModelYear.ToString(), oSHPOKDtlOFile.OthersCode);
                                                    if (oOmUtlSHPOKDtlO != null)
                                                    {
                                                        msg ="Kendaraan dengan InvoiceNo : " + invoiceNo.ToString() + " dan documentNo: " + documentNo.ToString() + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSHPOKDtlO = new OmUtlSHPOKDtlO();
                                                        oOmUtlSHPOKDtlO.CompanyCode = CompanyCode;
                                                        oOmUtlSHPOKDtlO.BranchCode = BranchCode;
                                                        oOmUtlSHPOKDtlO.BatchNo = oOmUtlSHPOKHdr.BatchNo;
                                                        oOmUtlSHPOKDtlO.InvoiceNo = invoiceNo;
                                                        oOmUtlSHPOKDtlO.DocumentNo = documentNo;
                                                        oOmUtlSHPOKDtlO.SalesModelCode = salesModelCode;
                                                        oOmUtlSHPOKDtlO.SalesModelYear = salesModelYear;
                                                        oOmUtlSHPOKDtlO.OthersCode = oSHPOKDtlOFile.OthersCode;
                                                        oOmUtlSHPOKDtlO.OthersDPP = oSHPOKDtlOFile.OthersDPP;
                                                        oOmUtlSHPOKDtlO.OthersPPN = oSHPOKDtlOFile.OthersPPN;
                                                        oOmUtlSHPOKDtlO.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSHPOKDtlO.CreatedDate = DateTime.Now;
                                                        oOmUtlSHPOKDtlO.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSHPOKDtlO.LastUpdateDate = DateTime.Now;

                                                        ctx.OmUtlSHPOKDtlOs.Add(oOmUtlSHPOKDtlO);
                                                        ctx.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "4")
                                            {
                                                UploadBLL.SHPOKDtl4File oSHPOKDtl4File = new UploadBLL.SHPOKDtl4File(lines[i]);
                                                if (oSHPOKDtl4File != null && oSHPOKDtl4File.ChassisCode != "")
                                                {
                                                    OmUtlSHPOKDtl4 oOmUtlSHPOKDtl4 = ctx.OmUtlSHPOKDtl4s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSHPOKHdr.BatchNo, invoiceNo, documentNo, salesModelCode
                                                                            , salesModelYear.ToString(), oSHPOKDtl4File.ColourCode, oSHPOKDtl4File.ChassisCode
                                                                            , oSHPOKDtl4File.ChassisNo.ToString());
                                                    if (oOmUtlSHPOKDtl4 != null)
                                                    {
                                                        msg ="Kendaraan dengan ChassisCode: " + oSHPOKDtl4File.ChassisCode + " dan ChassisNo: " + oSHPOKDtl4File.ChassisNo.ToString() + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSHPOKDtl4 = new OmUtlSHPOKDtl4();
                                                        oOmUtlSHPOKDtl4.CompanyCode = CompanyCode;
                                                        oOmUtlSHPOKDtl4.BranchCode = BranchCode;
                                                        oOmUtlSHPOKDtl4.BatchNo = oOmUtlSHPOKHdr.BatchNo;
                                                        oOmUtlSHPOKDtl4.InvoiceNo = invoiceNo;
                                                        oOmUtlSHPOKDtl4.DocumentNo = documentNo;
                                                        oOmUtlSHPOKDtl4.SalesModelCode = salesModelCode;
                                                        oOmUtlSHPOKDtl4.SalesModelYear = salesModelYear;
                                                        oOmUtlSHPOKDtl4.ColourCode = oSHPOKDtl4File.ColourCode;
                                                        oOmUtlSHPOKDtl4.ChassisCode = oSHPOKDtl4File.ChassisCode;
                                                        oOmUtlSHPOKDtl4.ChassisNo = oSHPOKDtl4File.ChassisNo;
                                                        oOmUtlSHPOKDtl4.EngineCode = oSHPOKDtl4File.EngineCode;
                                                        oOmUtlSHPOKDtl4.EngineNo = oSHPOKDtl4File.EngineNo;
                                                        oOmUtlSHPOKDtl4.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSHPOKDtl4.CreatedDate = DateTime.Now;
                                                        oOmUtlSHPOKDtl4.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSHPOKDtl4.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSHPOKDtl4s.Add(oOmUtlSHPOKDtl4);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal insert SHPOK detail 4";
                                                            return result;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            msg ="flat file tidak valid";
                            result = false;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
            {
                msg ="flat file tidak valid";
                result = false;
            }

            return result;
        }

        private bool UploadDataSACCS(string[] lines, bool isRecDCS, string kodePerusahaan)
        {
            bool result = false;
            int linesLength = 82;

            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    try
                    {
                        UploadBLL.SACCSHdrFile oSACCSHdrFile = new UploadBLL.SACCSHdrFile(lines[0]);
                        string RcvDealerCode = (isRecDCS) ? CompanyCode : oSACCSHdrFile.RcvDealerCode;

                        if (!kodePerusahaan.Equals(oSACCSHdrFile.RcvDealerCode))
                        {
                            string msg = "Invalid flat file, kode perusahaan tidak sama";
                            if (isRecDCS) msg += "\nHarap cek setup LockingBy di CoProfile Sales";
                            result = false;
                        }

                        if (oSACCSHdrFile != null && oSACCSHdrFile.DataID == "SACCS")
                        {
                            OmUtlSACCSHdr oOmUtlSACCSHdr = ctx.OmUtlSACCSHdrs.Find(CompanyCode, BranchCode
                                                    , oSACCSHdrFile.DealerCode, RcvDealerCode, oSACCSHdrFile.BatchNo);
                            if (oOmUtlSACCSHdr != null)
                            {
                                msg ="No Batch: " + oSACCSHdrFile.BatchNo + " sudah ada";
                                result = false;
                            }
                            else
                            {
                                oOmUtlSACCSHdr = new OmUtlSACCSHdr();
                                oOmUtlSACCSHdr.CompanyCode = CompanyCode;
                                oOmUtlSACCSHdr.BranchCode = BranchCode;
                                oOmUtlSACCSHdr.DealerCode = oSACCSHdrFile.DealerCode;
                                oOmUtlSACCSHdr.RcvDealerCode = RcvDealerCode;
                                oOmUtlSACCSHdr.BatchNo = oSACCSHdrFile.BatchNo;
                                oOmUtlSACCSHdr.Status = "0";
                                oOmUtlSACCSHdr.CreatedBy = CurrentUser.UserId;
                                oOmUtlSACCSHdr.CreatedDate = DateTime.Now;
                                oOmUtlSACCSHdr.LastUpdateBy = CurrentUser.UserId;
                                oOmUtlSACCSHdr.LastUpdateDate = DateTime.Now;
                                ctx.OmUtlSACCSHdrs.Add(oOmUtlSACCSHdr);
                                result = ctx.SaveChanges() > 0;
                                if (result == true)
                                {
                                    string bppNo = "";
                                    for (int i = 1; i < lines.Length; i++)
                                    {
                                        if (lines[i].Length == linesLength)
                                        {
                                            if (lines[i].Substring(0, 1) == "1")
                                            {
                                                UploadBLL.SACCSDtl1File oSACCSDtl1File = new UploadBLL.SACCSDtl1File(lines[i]);
                                                if (oSACCSDtl1File != null && oSACCSDtl1File.BPPNo != "")
                                                {
                                                    OmUtlSACCSDtl1 oOmUtlSACCSDtl1 = ctx.OmUtlSACCSDtl1s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSACCSHdr.BatchNo, oSACCSDtl1File.BPPNo);
                                                    if (oOmUtlSACCSDtl1 != null)
                                                    {
                                                        msg ="No. BPPNo: " + oSACCSDtl1File.BPPNo + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSACCSDtl1 = new OmUtlSACCSDtl1();
                                                        oOmUtlSACCSDtl1.CompanyCode = CompanyCode;
                                                        oOmUtlSACCSDtl1.BranchCode = BranchCode;
                                                        oOmUtlSACCSDtl1.BatchNo = oOmUtlSACCSHdr.BatchNo;
                                                        oOmUtlSACCSDtl1.BPPNo = oSACCSDtl1File.BPPNo;
                                                        bppNo = oOmUtlSACCSDtl1.BPPNo;
                                                        oOmUtlSACCSDtl1.BPPDate = oSACCSDtl1File.BPPDate;
                                                        oOmUtlSACCSDtl1.SJNo = oSACCSDtl1File.SJNo;
                                                        oOmUtlSACCSDtl1.Status = "0";
                                                        oOmUtlSACCSDtl1.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSACCSDtl1.CreatedDate = DateTime.Now;
                                                        oOmUtlSACCSDtl1.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSACCSDtl1.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSACCSDtl1s.Add(oOmUtlSACCSDtl1);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal insert SACCS detail 1";
                                                            return result;
                                                        }
                                                    }
                                                }
                                            }

                                            if (lines[i].Substring(0, 1) == "2")
                                            {
                                                UploadBLL.SACCSDtl2File oSACCSDtl2File = new UploadBLL.SACCSDtl2File(lines[i]);
                                                if (oSACCSDtl2File != null && oSACCSDtl2File.PerlengkapanCode != "")
                                                {
                                                    OmUtlSACCSDtl2 oOmUtlSACCSDtl2 = ctx.OmUtlSACCSDtl2s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSACCSHdr.BatchNo, bppNo, oSACCSDtl2File.PerlengkapanCode
                                                                            );
                                                    if (oOmUtlSACCSDtl2 != null)
                                                    {
                                                        msg ="No. BPPNo: " + bppNo + " dan Perlengkapan: " + oSACCSDtl2File.PerlengkapanCode + " sudah ada";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSACCSDtl2 = new OmUtlSACCSDtl2();
                                                        oOmUtlSACCSDtl2.CompanyCode = CompanyCode;
                                                        oOmUtlSACCSDtl2.BranchCode = BranchCode;
                                                        oOmUtlSACCSDtl2.BatchNo = oOmUtlSACCSHdr.BatchNo;
                                                        oOmUtlSACCSDtl2.BPPNo = bppNo;
                                                        oOmUtlSACCSDtl2.PerlengkapanCode = oSACCSDtl2File.PerlengkapanCode;
                                                        oOmUtlSACCSDtl2.Quantity = oSACCSDtl2File.Quantity;
                                                        oOmUtlSACCSDtl2.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSACCSDtl2.CreatedDate = DateTime.Now;
                                                        oOmUtlSACCSDtl2.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSACCSDtl2.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSACCSDtl2s.Add(oOmUtlSACCSDtl2);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (!result)
                                                        {
                                                            msg ="Gagal insert SACCS detail 2";
                                                            return result;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            msg ="flat file tidak valid";
                            result = false;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
            {
                msg ="flat file tidak valid";
                result = false;
            }

            return result;
        }

        private bool UploadDataSFPOL(string[] lines, bool isRecDCS, string kodePerusahaan)
        {
            bool result = false;
            int linesLength = 162;
            

            if (lines.Length <= 0)
            {
                msg ="flat file tidak ada data";
                result = false;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    try
                    {
                        UploadBLL.SFPOLHdrFile oSFPOLHdrFile = new UploadBLL.SFPOLHdrFile(lines[0]);
                        string RcvDealerCode = (isRecDCS) ? CompanyCode : oSFPOLHdrFile.RcvDealerCode;

                        if (!kodePerusahaan.Equals(oSFPOLHdrFile.RcvDealerCode))
                        {
                            string msg = "Invalid flat file, kode perusahaan tidak sama";
                            if (isRecDCS) msg += "\nHarap cek setup LockingBy di CoProfile Sales";
                            result = false;
                        }

                        if ((oSFPOLHdrFile != null && oSFPOLHdrFile.DataID == "SFPO1") ||
                            (oSFPOLHdrFile != null && oSFPOLHdrFile.DataID == "SFPO2")
                            )
                        {
                            OmUtlSFPOLHdr oOmUtlSFPOLHdr = ctx.OmUtlSFPOLHdrs.Find(CompanyCode, BranchCode
                                                    , oSFPOLHdrFile.DealerCode, RcvDealerCode, oSFPOLHdrFile.BatchNo);
                            if (oOmUtlSFPOLHdr != null)
                            {
                                msg ="No. Batch: " + oSFPOLHdrFile.BatchNo + " sudah ada";
                                result = false;
                            }
                            else
                            {
                                oOmUtlSFPOLHdr = new OmUtlSFPOLHdr();
                                oOmUtlSFPOLHdr.CompanyCode = CompanyCode;
                                oOmUtlSFPOLHdr.BranchCode = BranchCode;
                                oOmUtlSFPOLHdr.DealerCode = oSFPOLHdrFile.DealerCode;
                                oOmUtlSFPOLHdr.RcvDealerCode = RcvDealerCode;
                                oOmUtlSFPOLHdr.BatchNo = oSFPOLHdrFile.BatchNo;
                                oOmUtlSFPOLHdr.Status = "0";
                                oOmUtlSFPOLHdr.CreatedBy = CurrentUser.UserId;
                                oOmUtlSFPOLHdr.CreatedDate = DateTime.Now;
                                oOmUtlSFPOLHdr.LastUpdateBy = CurrentUser.UserId;
                                oOmUtlSFPOLHdr.LastUpdateDate = DateTime.Now;
                                ctx.OmUtlSFPOLHdrs.Add(oOmUtlSFPOLHdr);
                                result = ctx.SaveChanges() > 0;
                                if (result == true)
                                {
                                    for (int i = 1; i < lines.Length; i++)
                                    {
                                        if (lines[i].Length == linesLength)
                                        {
                                            if (lines[i].Substring(0, 1) == "1")
                                            {
                                                UploadBLL.SFPOLDtl1File oSFPOLDtl1File = new UploadBLL.SFPOLDtl1File(lines[i]);
                                                if (oSFPOLDtl1File != null && oSFPOLDtl1File.FakturPolisiNo != "")
                                                {
                                                    OmUtlSFPOLDtl1 oOmUtlSFPOLDtl1 = ctx.OmUtlSFPOLDtl1s.Find(CompanyCode, BranchCode
                                                                            , oOmUtlSFPOLHdr.BatchNo, oSFPOLDtl1File.FakturPolisiNo, oSFPOLDtl1File.SalesModelCode
                                                                            , oSFPOLDtl1File.SalesModelYear.ToString(), oSFPOLDtl1File.ColourCode, oSFPOLDtl1File.ChassisCode
                                                                            , oSFPOLDtl1File.ChassisNo.ToString());
                                                    if (oOmUtlSFPOLDtl1 != null)
                                                    {
                                                        msg ="Kendaraan dengan ChassisCode: " + oSFPOLDtl1File.ChassisCode + " dan ChassisNo: " + oSFPOLDtl1File.ChassisNo.ToString() + " sudah ada di Utility";
                                                        result = false;
                                                    }
                                                    else
                                                    {
                                                        oOmUtlSFPOLDtl1 = new OmUtlSFPOLDtl1();
                                                        oOmUtlSFPOLDtl1.CompanyCode = CompanyCode;
                                                        oOmUtlSFPOLDtl1.BranchCode = BranchCode;
                                                        oOmUtlSFPOLDtl1.BatchNo = oOmUtlSFPOLHdr.BatchNo;
                                                        oOmUtlSFPOLDtl1.FakturPolisiNo = oSFPOLDtl1File.FakturPolisiNo;
                                                        oOmUtlSFPOLDtl1.SalesModelCode = oSFPOLDtl1File.SalesModelCode;
                                                        oOmUtlSFPOLDtl1.SalesModelYear = oSFPOLDtl1File.SalesModelYear;
                                                        oOmUtlSFPOLDtl1.ColourCode = oSFPOLDtl1File.ColourCode;
                                                        oOmUtlSFPOLDtl1.ChassisCode = oSFPOLDtl1File.ChassisCode;
                                                        oOmUtlSFPOLDtl1.ChassisNo = oSFPOLDtl1File.ChassisNo;
                                                        oOmUtlSFPOLDtl1.EngineCode = oSFPOLDtl1File.EngineCode;
                                                        oOmUtlSFPOLDtl1.EngineNo = oSFPOLDtl1File.EngineNo;
                                                        oOmUtlSFPOLDtl1.IsBlanko = oSFPOLDtl1File.IsBlanko == "Y";
                                                        oOmUtlSFPOLDtl1.FakturPolisiDate = oSFPOLDtl1File.FakturPolisiDate;
                                                        oOmUtlSFPOLDtl1.FakturPolisiProcess = oSFPOLDtl1File.FakturPolisiProcess;
                                                        oOmUtlSFPOLDtl1.DONo = oSFPOLDtl1File.DONo;
                                                        oOmUtlSFPOLDtl1.SJNo = oSFPOLDtl1File.SJNo;
                                                        oOmUtlSFPOLDtl1.ReqNo = oSFPOLDtl1File.ReqNo;
                                                        oOmUtlSFPOLDtl1.CreatedBy = CurrentUser.UserId;
                                                        oOmUtlSFPOLDtl1.CreatedDate = DateTime.Now;
                                                        oOmUtlSFPOLDtl1.LastUpdateBy = CurrentUser.UserId;
                                                        oOmUtlSFPOLDtl1.LastUpdateDate = DateTime.Now;
                                                        ctx.OmUtlSFPOLDtl1s.Add(oOmUtlSFPOLDtl1);
                                                        result = ctx.SaveChanges() > 0;
                                                        if (result)
                                                        {
                                                            OmTrSalesFakturPolisi oOmTrSalesFakturPolisi = ctx.OmTrSalesFakturPolisi.Find(CompanyCode,
                                                                BranchCode, oOmUtlSFPOLDtl1.FakturPolisiNo);
                                                            if (oOmTrSalesFakturPolisi != null)
                                                            {
                                                                msg ="No Faktur Polisi: " + oOmUtlSFPOLDtl1.FakturPolisiNo + " sudah ada";
                                                                result = false;
                                                            }
                                                            else
                                                            {
                                                                var dtlVehicle = SelectDetailVehicle(oOmUtlSFPOLDtl1.ChassisCode, oOmUtlSFPOLDtl1.ChassisNo);

                                                                oOmTrSalesFakturPolisi = new OmTrSalesFakturPolisi();
                                                                oOmTrSalesFakturPolisi.CompanyCode = CompanyCode;
                                                                oOmTrSalesFakturPolisi.BranchCode = BranchCode;
                                                                oOmTrSalesFakturPolisi.FakturPolisiNo = oOmUtlSFPOLDtl1.FakturPolisiNo;
                                                                oOmTrSalesFakturPolisi.SalesModelCode = oOmUtlSFPOLDtl1.SalesModelCode;
                                                                oOmTrSalesFakturPolisi.SalesModelYear = oOmUtlSFPOLDtl1.SalesModelYear;
                                                                oOmTrSalesFakturPolisi.ColourCode = oOmUtlSFPOLDtl1.ColourCode;
                                                                oOmTrSalesFakturPolisi.ChassisCode = oOmUtlSFPOLDtl1.ChassisCode;
                                                                oOmTrSalesFakturPolisi.ChassisNo = oOmUtlSFPOLDtl1.ChassisNo;
                                                                oOmTrSalesFakturPolisi.EngineCode = oOmUtlSFPOLDtl1.EngineCode;
                                                                oOmTrSalesFakturPolisi.EngineNo = oOmUtlSFPOLDtl1.EngineNo;
                                                                oOmTrSalesFakturPolisi.IsBlanko = oOmUtlSFPOLDtl1.IsBlanko.Value;
                                                                oOmTrSalesFakturPolisi.FakturPolisiDate = oOmUtlSFPOLDtl1.FakturPolisiDate;
                                                                oOmTrSalesFakturPolisi.FakturPolisiProcess = oOmUtlSFPOLDtl1.FakturPolisiProcess;
                                                                oOmTrSalesFakturPolisi.SJImniNo = oOmUtlSFPOLDtl1.SJNo;
                                                                oOmTrSalesFakturPolisi.DOImniNo = oOmUtlSFPOLDtl1.DONo;
                                                                oOmTrSalesFakturPolisi.ReqNo = (dtlVehicle != null) ? dtlVehicle.ReqNo : oOmUtlSFPOLDtl1.ReqNo;
                                                                oOmTrSalesFakturPolisi.CreatedBy = CurrentUser.UserId;
                                                                oOmTrSalesFakturPolisi.CreatedDate = DateTime.Now;
                                                                oOmTrSalesFakturPolisi.IsManual = false;
                                                                oOmTrSalesFakturPolisi.Status = "2";
                                                                ctx.OmTrSalesFakturPolisi.Add(oOmTrSalesFakturPolisi);
                                                                result = ctx.SaveChanges() > 0;
                                                                if (result)
                                                                {

                                                                    omTrSalesReqDetail oOmTrSalesReqDetail = ctx.omTrSalesReqDetail.FirstOrDefault(a=>a.CompanyCode == CompanyCode && a.ChassisCode ==oOmTrSalesFakturPolisi.ChassisCode && a.ChassisNo == oOmTrSalesFakturPolisi.ChassisNo);
                                                                    oOmTrSalesReqDetail.FakturPolisiNo = oOmTrSalesFakturPolisi.FakturPolisiNo;
                                                                    oOmTrSalesReqDetail.FakturPolisiDate = oOmTrSalesFakturPolisi.FakturPolisiDate;
                                                                    oOmTrSalesReqDetail.LastUpdateBy = CurrentUser.UserId;
                                                                    oOmTrSalesReqDetail.LastUpdateDate = DateTime.Now;

                                                                    if (ctx.SaveChanges() < 0)
                                                                    {
                                                                        msg ="Update Request Detail ChassisCode: " + oOmUtlSFPOLDtl1.ChassisCode + " dan ChassisNo"
                                                                                + oOmUtlSFPOLDtl1.ChassisNo.ToString() + " gagal";
                                                                        result = false;
                                                                    }
                                                                }

                                                                if (result)
                                                                {
                                                                    OmMstVehicle oOmMstVehicle = ctx.OmMstVehicles.Find(CompanyCode,
                                                                        oOmUtlSFPOLDtl1.ChassisCode, oOmUtlSFPOLDtl1.ChassisNo);
                                                                    if (oOmMstVehicle != null)
                                                                    {
                                                                        oOmMstVehicle.FakturPolisiNo = oOmTrSalesFakturPolisi.FakturPolisiNo;
                                                                        oOmMstVehicle.FakturPolisiDate = oOmTrSalesFakturPolisi.FakturPolisiDate;
                                                                        oOmMstVehicle.LastUpdateBy = CurrentUser.UserId;
                                                                        oOmMstVehicle.LastUpdateDate = DateTime.Now;
                                                                        if (ctx.SaveChanges() < 1)
                                                                        {
                                                                            msg ="Update Master Kendaraan ChassisCode: " + oOmUtlSFPOLDtl1.ChassisCode + " dan ChassisNo"
                                                                                + oOmUtlSFPOLDtl1.ChassisNo.ToString() + " gagal";
                                                                            result = false;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            msg ="flat file tidak valid";
                            result = false;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
            {
                msg ="flat file tidak valid";
                return false;
            }

            return result;
        }

        //private bool UploadDataSPRIC(string[] lines, bool isRecDCS, string kodePerusahaan)
        //{
        //    bool result = false;
        //    int linesLength = 100;

        //    if (lines.Length <= 0)
        //    {
        //        msg = "flat file tidak ada data";
        //        result = false;
        //    }
        //    if (lines[0].Length == linesLength)
        //    {
        //        if (lines[0].Substring(0, 1) == "H")
        //        {
        //            try
        //            {
        //                UploadBLL.SPRICHdrFile oSPRICHdrFile = new UploadBLL.SPRICHdrFile(lines[0]);
        //                string RcvDealerCode = (isRecDCS) ? CompanyCode : oSPRICHdrFile.RcvDealerCode;

        //                if (!kodePerusahaan.Equals(oSPRICHdrFile.RcvDealerCode))
        //                {
        //                    string msg = "Invalid flat file, kode perusahaan tidak sama";
        //                    if (isRecDCS) msg += "\nHarap cek setup LockingBy di CoProfile Sales";
        //                    result = false;
        //                }

        //                if ((oSPRICHdrFile != null && oSPRICHdrFile.DataID == "SPRIC"))
        //                {
        //                    OmUtlSPRICHdr oOmUtlSPRICHdr = OmUtlSPRICHdrBLL.GetRecord(ctx, CompanyCode, BranchCode
        //                                            , oSPRICHdrFile.DealerCode, RcvDealerCode, oSPRICHdrFile.BatchNo);
        //                    if (oOmUtlSPRICHdr != null)
        //                    {
        //                        msg = "No: " + oSPRICHdrFile.BatchNo + " sudah ada";
        //                        result = false;
        //                    }
        //                    else
        //                    {
        //                        oOmUtlSPRICHdr = new OmUtlSPRICHdr();
        //                        oOmUtlSPRICHdr.CompanyCode = CompanyCode;
        //                        oOmUtlSPRICHdr.BranchCode = BranchCode;
        //                        oOmUtlSPRICHdr.DealerCode = oSPRICHdrFile.DealerCode;
        //                        oOmUtlSPRICHdr.RcvDealerCode = RcvDealerCode;
        //                        oOmUtlSPRICHdr.BatchNo = oSPRICHdrFile.BatchNo;
        //                        oOmUtlSPRICHdr.Status = "0";
        //                        oOmUtlSPRICHdr.CreatedBy = CurrentUser.UserId;
        //                        oOmUtlSPRICHdr.CreatedDate = DateTime.Now;
        //                        oOmUtlSPRICHdr.LastUpdateBy = CurrentUser.UserId;
        //                        oOmUtlSPRICHdr.LastUpdateDate = DateTime.Now;

        //                        result = oOmUtlSPRICHdrDao.Insert(oOmUtlSPRICHdr) > 0;
        //                        if (result == true)
        //                        {
        //                            for (int i = 1; i < lines.Length; i++)
        //                            {
        //                                if (lines[i].Length == linesLength)
        //                                {
        //                                    if (lines[i].Substring(0, 1) == "1")
        //                                    {
        //                                        UploadBLL.SPRICDtlFile oSPRICDtlFile = new UploadBLL.SPRICDtlFile(lines[i]);
        //                                        if (oSPRICDtlFile != null && oSPRICDtlFile.SalesModelCode != "")
        //                                        {
        //                                            OmUtlSPRICDtl oOmUtlSPRICDtl = OmUtlSPRICDtlBLL.GetRecord(ctx, CompanyCode, BranchCode
        //                                                                    , oOmUtlSPRICHdr.BatchNo, oSPRICDtlFile.SalesModelCode, oSPRICDtlFile.SalesModelYear.ToString());
        //                                            if (oOmUtlSPRICDtl != null)
        //                                            {
        //                                                msg = "Kendaraan dengan SalesModelCode: " + oSPRICDtlFile.SalesModelCode + " dan SalesModelYear: " + oSPRICDtlFile.SalesModelYear.ToString() + " sudah ada";
        //                                                result = false;
        //                                            }
        //                                            else
        //                                            {
        //                                                oOmUtlSPRICDtl = new OmUtlSPRICDtl();
        //                                                oOmUtlSPRICDtl.CompanyCode = CompanyCode;
        //                                                oOmUtlSPRICDtl.BranchCode = BranchCode;
        //                                                oOmUtlSPRICDtl.BatchNo = oOmUtlSPRICHdr.BatchNo;
        //                                                oOmUtlSPRICDtl.SalesModelCode = oSPRICDtlFile.SalesModelCode;
        //                                                oOmUtlSPRICDtl.SalesModelYear = oSPRICDtlFile.SalesModelYear;
        //                                                oOmUtlSPRICDtl.PPnBMPaid = oSPRICDtlFile.PPnBMPaid;
        //                                                oOmUtlSPRICDtl.DPP = oSPRICDtlFile.ModelPriceDPP;
        //                                                oOmUtlSPRICDtl.PPn = oSPRICDtlFile.ModelPricePPn;
        //                                                oOmUtlSPRICDtl.Total = oSPRICDtlFile.ModelPriceTotal;
        //                                                oOmUtlSPRICDtl.FromDate = oSPRICDtlFile.PriceStartDate;
        //                                                oOmUtlSPRICDtl.Status = "0";
        //                                                oOmUtlSPRICDtl.CreatedBy = CurrentUser.UserId;
        //                                                oOmUtlSPRICDtl.CreatedDate = DateTime.Now;
        //                                                oOmUtlSPRICDtl.LastUpdateBy = CurrentUser.UserId;
        //                                                oOmUtlSPRICDtl.LastUpdateDate = DateTime.Now;
        //                                                if (oOmUtlSPRICDtlDao.Insert(oOmUtlSPRICDtl) < 1)
        //                                                {
        //                                                    msg = "Gagal insert SPRIC detail";
        //                                                    result = false;
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    msg = "flat file tidak valid";
        //                    result = false;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        }
        //    }
        //    else
        //    {
        //        msg = "flat file tidak valid";
        //        result = false;
        //    }

        //    return result;
        //}

        private bool UploadDataSFPLB(string[] lines, bool isRecDCS, string kodePerusahaan)
        {
            bool result = false;
            bool clear = true;
            string chassisCode = string.Empty;
            int chassisNo = 0;

            int linesLength = 450;
            

            if (lines.Length <= 0)
            {
                msg ="flat file tidak ada data";
                return result;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    try
                    {
                        string query = string.Empty;
                        StringBuilder sb = new StringBuilder();

                        UploadBLL.SFPLBHdrFile oSFPLBHdrFile = new UploadBLL.SFPLBHdrFile(lines[0]);
                        string RcvDealerCode = (isRecDCS) ? CompanyCode : oSFPLBHdrFile.RcvDealerCode;

                        var dtCustomer = ctx.GnMstCustomer.FirstOrDefault(a=>a.CompanyCode == CompanyCode && (a.CustomerCode == CompanyCode || a.StandardCode == oSFPLBHdrFile.DealerCode));

                        if (!kodePerusahaan.Equals(oSFPLBHdrFile.RcvDealerCode))
                        {
                            string msg = "Invalid flat file, kode perusahaan tidak sama";
                            if (isRecDCS) msg += "\nHarap cek setup LockingBy di CoProfile Sales";
                            result = false;
                        }
                        if (dtCustomer != null)
                        {
                            if (dtCustomer.CategoryCode != "01")
                            {
                                string msg = "Invalid flat file, kode perusahaan penerima bukan Sub-Dealer";
                                result = false;
                            }
                        }
                        else
                        {
                            string msg = "Invalid flat file, Data DealerCode Tidak tersedia";
                            result = false;
                        }

                        if (oSFPLBHdrFile != null && (oSFPLBHdrFile.DataID == "SFPLB" || oSFPLBHdrFile.DataID == "SFPLA" || oSFPLBHdrFile.DataID == "SFPLR"))
                        {
                            omUtlFpolReq oOmUtlFPolReq = ctx.omUtlFpolReqs.Find(CompanyCode, BranchCode, dtCustomer.CustomerCode, oSFPLBHdrFile.BatchNo);
                            
                            if (oOmUtlFPolReq != null)
                            {
                                msg ="Proses upload file gagal";
                                return result;
                            }
                            else
                            {
                                var oOmUtlFPolReqRecord = new omUtlFpolReq()
                                {
                                    CompanyCode = CompanyCode,
                                    BranchCode = BranchCode,
                                    DealerCode = dtCustomer.CustomerCode,
                                    BatchNo = oSFPLBHdrFile.BatchNo,
                                    Status = "0",
                                    CreatedBy = CurrentUser.UserId,
                                    CreatedDate = DateTime.Now,
                                    LastUpdateBy = CurrentUser.UserId,
                                    LastUpdateDate = DateTime.Now
                                };

                                ctx.omUtlFpolReqs.Add(oOmUtlFPolReqRecord);

                                oOmUtlFPolReq = ctx.omUtlFpolReqs.Find(CompanyCode, BranchCode, dtCustomer.CustomerCode, oSFPLBHdrFile.BatchNo);

                                if (result == true)
                                {
                                    for (int i = 1; i < lines.Length; i++)
                                    {
                                        if (i < 50)
                                        {
                                            if (clear == true)
                                            {
                                                query = @"INSERT INTO [omUtlFPolReqDetail]([CompanyCode],[BranchCode],[BatchNo],[ChassisCode],[ChassisNo],[EngineCode],[EngineNo],[SalesModelCode],[SalesModelYear],[SalesModelDescription],[ModelLine],[ColourCode],[ColourDescription],[ServiceBookNo],[FakturPolisiNo],[FakturPolisiDate],[FpolisiModelDescription],[SISDeliveryOrderNo],[SISDeliveryOrderDate],[SISDeliveryOrderAtasNama],[SISSuratJalanNo],[SISSuratJalanDate],[SISSuratJalanAtasNama],[OldDealerCode],[DealerClass],[DealerName],[SKPKNo],[SuratPermohonanNo],[SalesmanName],[SKPKName],[SKPKName2],[SKPKAddr1],[SKPKAddr2],[SKPKAddr3],[SKPKCityCode],[SKPKPhoneNo1],[SKPKPhoneNo2],[SKPKHPNo],[SKPKBirthday],[FPolName],[FPolName2],[FPolAddr1],[FPolAddr2],[FPolAddr3],[FPolPostCode],[FPolPostName],[FPolCityCode],[FPolKecamatanCode],[FPolPhoneNo1],[FPolPhoneNo2],[FPolHPNo],[FPolBirthday],[IdentificationNo],[IsProject],[ReasonCode],[ReasonDescription],[ProcessDate],[IsCityTransport],[CreatedBy],[CreatedDate],[LastUpdateBy],[LastUpdateDate],[Status])
VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}','{60}','{61}','{62}')";
                                                query = query.Replace("{0}", CompanyCode);
                                                query = query.Replace("{1}", BranchCode);
                                                query = query.Replace("{2}", oSFPLBHdrFile.BatchNo);
                                                query = query.Replace("{58}", CurrentUser.UserId);
                                                query = query.Replace("{59}", DateTime.Now.ToShortDateString());
                                                query = query.Replace("{60}", CurrentUser.UserId);
                                                query = query.Replace("{61}", DateTime.Now.ToShortDateString());
                                                query = query.Replace("{62}", "0");

                                                clear = false;
                                            }

                                            string line = lines[i];

                                            if (lines[i].Length == linesLength)
                                            {
                                                #region ** Line 1
                                                if (lines[i].Substring(0, 1) == "1")
                                                {
                                                    UploadBLL.SFPLBDtl1File oSFPLBDtl1File = new UploadBLL.SFPLBDtl1File(lines[i]);
                                                    if (oSFPLBDtl1File != null)
                                                    {
                                                        query = query.Replace("{17}", oSFPLBDtl1File.DeliveryOrder);
                                                        query = query.Replace("{18}", oSFPLBDtl1File.DeliveryOrderDate.ToShortDateString());
                                                        query = query.Replace("{19}", oSFPLBDtl1File.DeliveryOrderAtasNama);
                                                    }
                                                }
                                                #endregion
                                                #region ** Line 2
                                                if (lines[i].Substring(0, 1) == "2")
                                                {
                                                    UploadBLL.SFPLBDtl2File oSFPLBDtl2File = new UploadBLL.SFPLBDtl2File(lines[i]);
                                                    if (oSFPLBDtl2File != null)
                                                    {
                                                        query = query.Replace("{20}", oSFPLBDtl2File.SuratJalan);
                                                        query = query.Replace("{21}", oSFPLBDtl2File.SuratJalanDate.ToShortDateString());
                                                        query = query.Replace("{22}", oSFPLBDtl2File.SuratJalanAtasNama);
                                                        query = query.Replace("{7}", oSFPLBDtl2File.ModelCode);
                                                        query = query.Replace("{9}", oSFPLBDtl2File.SalesModelDescription);
                                                        query = query.Replace("{16}", oSFPLBDtl2File.FpolisiModelDescription);
                                                        query = query.Replace("{10}", oSFPLBDtl2File.ModelLine);
                                                        query = query.Replace("{23}", oSFPLBDtl2File.OldDealerCode);
                                                    }
                                                }
                                                #endregion
                                                #region ** Line 3
                                                if (lines[i].Substring(0, 1) == "3")
                                                {
                                                    UploadBLL.SFPLBDtl3File oSFPLBDtl3File = new UploadBLL.SFPLBDtl3File(lines[i]);
                                                    if (oSFPLBDtl3File != null)
                                                    {
                                                        query = query.Replace("{24}", oSFPLBDtl3File.DealerClass);
                                                        query = query.Replace("{25}", oSFPLBDtl3File.DealerName);
                                                        query = query.Replace("{26}", oSFPLBDtl3File.NoSKPK);
                                                        query = query.Replace("{27}", oSFPLBDtl3File.NoSuratPermohonan);
                                                        query = query.Replace("{28}", oSFPLBDtl3File.SalesmanName);
                                                        query = query.Replace("{29}", oSFPLBDtl3File.NamaSKPK);
                                                        query = query.Replace("{30}", oSFPLBDtl3File.NamaSKPK2);
                                                        query = query.Replace("{31}", oSFPLBDtl3File.Alamat1SKPK);
                                                        query = query.Replace("{32}", oSFPLBDtl3File.Alamat2SKPK);
                                                        query = query.Replace("{33}", oSFPLBDtl3File.Alamat3SKPK);
                                                        query = query.Replace("{34}", oSFPLBDtl3File.CityCode);
                                                        query = query.Replace("{35}", oSFPLBDtl3File.TeleponNo1);
                                                        query = query.Replace("{36}", oSFPLBDtl3File.TeleponNo2);
                                                        query = query.Replace("{37}", oSFPLBDtl3File.HandPhoneNo);
                                                        query = query.Replace("{38}", oSFPLBDtl3File.BirthdaySKPK);
                                                    }
                                                }
                                                #endregion
                                                #region ** Line 4
                                                if (lines[i].Substring(0, 1) == "4")
                                                {
                                                    UploadBLL.SFPLBDtl4File oSFPLBDtl4File = new UploadBLL.SFPLBDtl4File(lines[i]);
                                                    if (oSFPLBDtl4File != null)
                                                    {
                                                        query = query.Replace("{39}", oSFPLBDtl4File.Nama);
                                                        query = query.Replace("{40}", oSFPLBDtl4File.Nama2);
                                                        query = query.Replace("{41}", oSFPLBDtl4File.Alamat1);
                                                        query = query.Replace("{42}", oSFPLBDtl4File.Alamat2);
                                                        query = query.Replace("{43}", oSFPLBDtl4File.Alamat3);
                                                        query = query.Replace("{44}", oSFPLBDtl4File.PostCode);
                                                        query = query.Replace("{45}", oSFPLBDtl4File.PostName);
                                                        query = query.Replace("{46}", oSFPLBDtl4File.CityCode);
                                                        query = query.Replace("{47}", oSFPLBDtl4File.KodeKecamatan);
                                                        query = query.Replace("{48}", oSFPLBDtl4File.Telepon1);
                                                        query = query.Replace("{49}", oSFPLBDtl4File.Telepon2);
                                                        query = query.Replace("{50}", oSFPLBDtl4File.HandPhone);
                                                        query = query.Replace("{51}", oSFPLBDtl4File.BirthdayFpol.ToShortDateString());
                                                        query = query.Replace("{52}", oSFPLBDtl4File.IDNO);
                                                        query = query.Replace("{53}", oSFPLBDtl4File.IsProject);
                                                    }
                                                }
                                                #endregion
                                                #region ** Line 5
                                                if (lines[i].Substring(0, 1) == "5")
                                                {
                                                    UploadBLL.SFPLBDtl5File oSFPLBDtl5File = new UploadBLL.SFPLBDtl5File(lines[i]);
                                                    if (oSFPLBDtl5File != null)
                                                    {
                                                        query = query.Replace("{54}", oSFPLBDtl5File.ReasonCode);
                                                        query = query.Replace("{55}", oSFPLBDtl5File.ReasonDescription);
                                                        query = query.Replace("{56}", oSFPLBDtl5File.ProcessDate);
                                                        query = query.Replace("{57}", oSFPLBDtl5File.IsCityTransport);
                                                        query = query.Replace("{14}", oSFPLBDtl5File.FakturPolisiNo);
                                                        query = query.Replace("{15}", oSFPLBDtl5File.FakturPolisiDate.ToShortDateString());
                                                        query = query.Replace("{3}", oSFPLBDtl5File.KodeRangka);
                                                        query = query.Replace("{4}", oSFPLBDtl5File.NoRangka.ToString());
                                                        query = query.Replace("{5}", oSFPLBDtl5File.KodeMesin);
                                                        query = query.Replace("{6}", oSFPLBDtl5File.NoMesin.ToString());
                                                        query = query.Replace("{8}", oSFPLBDtl5File.Year.ToString());
                                                        query = query.Replace("{11}", oSFPLBDtl5File.ColorCode);
                                                        query = query.Replace("{12}", oSFPLBDtl5File.ColorDescription);
                                                        query = query.Replace("{13}", oSFPLBDtl5File.ServiceBookNo);

                                                        chassisCode = oSFPLBDtl5File.KodeRangka;
                                                        chassisNo = oSFPLBDtl5File.NoRangka;

                                                    }
                                                }
                                                #endregion
                                                #region ** Line 6
                                                if (lines[i].Substring(0, 1) == "6")
                                                {
                                                    UploadBLL.SFPLBDtl6File oSFPLBDtl6File = new UploadBLL.SFPLBDtl6File(lines[i]);
                                                    if (oSFPLBDtl6File != null)
                                                    {
                                                        if (oSFPLBDtl6File.JenisKelamin != "1")
                                                        {
                                                            query += string.Format(@" INSERT INTO [omUtlFPolReqDetailAdditional]([CompanyCode],[BranchCode],[BatchNo],[ChassisCode],[ChassisNo],[Gender],[TempatPembelian],[TempatPembelianOthers],[KendaraanYgPernahDimiliki],[KendaraanYgPernahDimilikiModel],[SumberPembelian],[SumberPembelianOthers],[AsalPembelian],[AsalPembelianOthers],[InfoSuzukiDari],[InfoSuzukiDariOthers],[FaktorPentingKendaraan],[PendidikanTerakhir],[PendidikanTerakhirOthers],[PenghasilanKeluarga],[Pekerjaan],[PekerjaanOthers],[PenggunaanKendaraan],[PenggunaanKendaraanOthers],[CaraPembelian],[Leasing],[LeasingOthers],[JangkaWaktuKredit],[JangkaWaktuKreditOthers],[CreatedBy],[CreatedDate],[LastUpdateBy],[LastUpdateDate])
     VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')", CompanyCode, BranchCode, oSFPLBHdrFile.BatchNo, chassisCode, chassisNo.ToString(), oSFPLBDtl6File.JenisKelamin, oSFPLBDtl6File.TempatPembelianMotor, oSFPLBDtl6File.TPSOther, oSFPLBDtl6File.MotorYgPernahDipakai, oSFPLBDtl6File.TypeKendaraan, oSFPLBDtl6File.SumberPembelian, oSFPLBDtl6File.SUPOthers, oSFPLBDtl6File.AsalPembelian, oSFPLBDtl6File.ASPOthers, oSFPLBDtl6File.InformasiSepedaMotor, oSFPLBDtl6File.SRIOthers, oSFPLBDtl6File.FaktorPentingSpdMotor, oSFPLBDtl6File.PendidikanTerakhir, oSFPLBDtl6File.PDKOthers, oSFPLBDtl6File.PenghasilanKeluarga, oSFPLBDtl6File.Pekerjaan, oSFPLBDtl6File.PEKOthers, oSFPLBDtl6File.MotorCycleFunction, oSFPLBDtl6File.USEOthers, oSFPLBDtl6File.CaraPembelian, oSFPLBDtl6File.LeasingYgDipakai, oSFPLBDtl6File.LSGOthers, oSFPLBDtl6File.JangkaWaktuKredit, oSFPLBDtl6File.JWKOthers, CurrentUser.UserId, DateTime.Now, CurrentUser.UserId, DateTime.Now);
                                                        }

                                                        sb.AppendLine(query);
                                                        clear = true;
                                                    }
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                throw new Exception("Proses Upload Gagal");
                                            }
                                        }
                                        else
                                        {
                                            result = ctx.Database.ExecuteSqlCommand(sb.ToString()) > 0;
                                            query = "";
                                            i = 0;
                                        }
                                    }
                                    result = ctx.Database.ExecuteSqlCommand(sb.ToString()) > 0;
                                    query = "";
                                }
                                else
                                {
                                    throw new Exception("Proses Upload Gagal");
                                }
                            }
                        }
                        else
                        {
                            msg ="flat file tidak valid";
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
            {
                msg ="flat file tidak valid";
                return false;
            }

            return result;
        }

        private bool UploadDataFAPIO(string[] lines, bool isRecDCS, string kodePerusahaan)
        {
            bool result = false;
            bool clear = true;
            string chassisCode = string.Empty;
            int chassisNo = 0;

            int linesLength = 450;

            if (lines.Length <= 0)
            {
                msg ="flat file tidak ada data";
                return result;
            }

            if (lines[0].Substring(0, 1) == "H")
            {
                try
                {
                    string query = string.Empty;
                    StringBuilder sb = new StringBuilder();

                    UploadBLL.FAPIOHdrFile oFAPIOHdrFile = new UploadBLL.FAPIOHdrFile(lines[0]);
                    string RcvDealerCode = (isRecDCS) ? CompanyCode : oFAPIOHdrFile.RcvDealerCode;

                    var dtCustomer = ctx.GnMstCustomer.FirstOrDefault(a=>a.CompanyCode == CompanyCode && (a.CustomerCode == CompanyCode || a.StandardCode == oFAPIOHdrFile.SupplierCode));

                    if (!kodePerusahaan.Equals(oFAPIOHdrFile.RcvDealerCode))
                    {
                        string msg = "Invalid flat file, kode perusahaan tidak sama";
                        if (isRecDCS) msg += "\nHarap cek setup LockingBy di CoProfile Sales";
                        result = false;
                    }

                    int maxID = Convert.ToInt32(ctx.ApUtlInvoiceOthersHdrs.Max(a=>a.IDNo).ToString()) + 1;
                    int seqID = 1;
                    int count = 0;

                    if (oFAPIOHdrFile != null && oFAPIOHdrFile.DataID == "FAPIO")
                    {
                        UploadBLL.FAPIODtl1File oFAPIODtl1File = new UploadBLL.FAPIODtl1File();
                        for (int i = 1; i < lines.Length; i++)
                        {
                            count++;
                            if (count < 50)
                            {
                                if (lines[i].Substring(0, 1) == "1")
                                {
                                    oFAPIODtl1File = new UploadBLL.FAPIODtl1File(lines[i]);
                                    if (oFAPIODtl1File != null)
                                    {
                                        query = "";

                                        maxID++;
                                        seqID = 0;
                                        query = @"INSERT INTO [apUtlInvoiceOthersHdr]([IDNo],[CompanyCode],[BranchCode],[ProductType],[TransactionType],[FakturNo],[FakturDate],[TaxNo],[TaxDate],[TotalDPP],[TotalPPN],[TotalInvoice],[TermOfPayment],[Description],[TotalNoInquiry],[SupplierCode],[IsExists],[BalanceDPP],[BalancePPN],[BalanceInvoice])
     VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}','{15}', 0, '{9}', '{10}', '{11}')";
                                        query = query.Replace("{0}", maxID.ToString());
                                        query = query.Replace("{1}", CompanyCode);
                                        query = query.Replace("{2}", BranchCode);
                                        query = query.Replace("{3}", ProductType);
                                        query = query.Replace("{4}", oFAPIODtl1File.TransactionType);
                                        query = query.Replace("{5}", oFAPIODtl1File.FakturNo);
                                        query = query.Replace("{6}", oFAPIODtl1File.FakturDate.ToShortDateString());
                                        if (oFAPIODtl1File.TaxNo != "")
                                            query = query.Replace("{7}", oFAPIODtl1File.TaxNo);
                                        else
                                        {
                                            string msg = "Invalid flat file, No Seri Pajak kosong / blank." + "\n" + "Dengan No Faktur : " + oFAPIODtl1File.FakturNo + "," + "\n" + "nilai Invoice: " + oFAPIODtl1File.TotalInvoice.ToString("n0");
                                            result = false;
                                        }
                                        query = query.Replace("{8}", oFAPIODtl1File.TaxDate.ToShortDateString());
                                        query = query.Replace("{9}", oFAPIODtl1File.TotalDPP.ToString());
                                        query = query.Replace("{10}", oFAPIODtl1File.TotalPPN.ToString());
                                        query = query.Replace("{11}", oFAPIODtl1File.TotalInvoice.ToString());
                                        query = query.Replace("{12}", oFAPIODtl1File.TermOfPayment);
                                        query = query.Replace("{13}", oFAPIODtl1File.Description);
                                        query = query.Replace("{14}", oFAPIODtl1File.TotalNumberOfInquiry.ToString());
                                        query = query.Replace("{15}", oFAPIOHdrFile.SupplierCode.ToString());
                                    }
                                }

                                if (lines[i].Substring(0, 1) == "2")
                                {
                                    UploadBLL.FAPIODtl2File oFAPIODtl2File = new UploadBLL.FAPIODtl2File(lines[i]);
                                    query += @" 
INSERT INTO [apUtlInvoiceOthersDtl]([IDNo],[Seq],[MemoLineDescription],[DueDate],[NilaiDPP],[NilaiPPN],[NilaiInvoice],[BalanceDPP],[BalancePPn],[BalanceInvoice],[Description])
     VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{4}', '{5}', '{6}','{7}')";
                                    query = query.Replace("{0}", maxID.ToString());
                                    query = query.Replace("{1}", seqID.ToString());
                                    query = query.Replace("{2}", oFAPIODtl2File.MemoLineDescription);
                                    query = query.Replace("{3}", oFAPIODtl2File.DueDate.ToShortDateString());
                                    query = query.Replace("{4}", oFAPIODtl2File.NilaiDPP.ToString());
                                    query = query.Replace("{5}", oFAPIODtl2File.NilaiPPN.ToString());
                                    query = query.Replace("{6}", oFAPIODtl2File.NilaiInvoice.ToString());
                                    query = query.Replace("{7}", oFAPIODtl2File.Description.ToString());
                                    seqID++;
                                }
                                sb.AppendLine(query);
                                query = "";
                            }
                            else
                            {
                                result = ctx.Database.ExecuteSqlCommand(sb.ToString()) > 0;
                                sb = new StringBuilder();
                                query = "";
                                count = 0;
                            }
                        }
                        result = ctx.Database.ExecuteSqlCommand(sb.ToString()) > 0;
                        sb = new StringBuilder();
                        query = "";
                    }
                    else
                    {
                        throw new Exception("Proses Upload Gagal");
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                }
            }

            return result;
        }

        private bool UploadDataSUADE(string[] lines, bool isRecDCS, string kodePerusahaan)
        {
            bool result = false;
            int linesLength = 110;

            if (lines.Length <= 0)
            {
                msg ="flat file tidak ada data";
                return result;
            }
            if (lines[0].Length == linesLength)
            {
                if (lines[0].Substring(0, 1) == "H")
                {
                    string query = string.Empty;
                    string storedQuery = string.Empty;
                    string execQuery = string.Empty;

                    StringBuilder sb = new StringBuilder();

                    UploadBLL.SUADEHdrFile oSUADEHdrFile = new UploadBLL.SUADEHdrFile(lines[0]);
                    GnMstCustomer dtCustomer = ctx.GnMstCustomer.Find(CompanyCode, oSUADEHdrFile.DealerCode);

                    if (!CompanyCode.Equals(oSUADEHdrFile.RcvDealerCode))
                    {
                        string msg = "Invalid flat file, kode perusahaan tidak sama";
                        result = false;
                    }

                    if (dtCustomer != null && dtCustomer.CategoryCode != "01")
                    {
                        string msg = "Invalid flat file, kode perusahaan penerima bukan Sub-Dealer";
                        result = false;
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
                                result = ctx.Database.ExecuteSqlCommand(@"Declare @IndentID int 
                                Declare @TotalIndent int
                                " + execQuery) > 0;
                                sb = new StringBuilder();
                                query = string.Empty;
                                execQuery = string.Empty;
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
                                    query += @"
if(select COUNT(*) from omMstIndent where TypeCode = '{0}' and MarketModelCode = '{1}' and Variant = '{2}' and ModelYear = '{3}' and Year = '{4}' and Month = '{5}' and ColourCode = '{6}') > 0
begin
	UPDATE [omMstIndent]
	   SET [QuotaUnits] = '{8}', [UnitStatus] = '{10}', [ColourStatus] = '{11}' ,[isNeedReposting] = '1',[LastUpdateBy] = '{15}',[LastUpdateDate] = '{16}'
	 WHERE [TypeCode] = '{0}' and [MarketModelCode] = '{1}' and [Variant] = '{2}' and [ModelYear] = '{3}' and [Year] = '{4}' and [Month] = '{5}' and [ColourCode] = '{6}'
	
	set @IndentID = (select TOP 1 IndentID from omMstIndent where TypeCode = '{0}' and MarketModelCode = '{1}' and Variant = '{2}' and ModelYear = '{3}' and Year = '{4}' and Month = '{5}' and ColourCode = '{6}')
	set @TotalIndent = (select COUNT(*) from omHstIndent where IndentID = @IndentID)
	INSERT INTO [omHstIndent]([IndentID],[LogSeq],[UnitStatus],[ColourStatus],[QuotaUnits],[LastUpdateBy],[LastUpdateDate])
     VALUES(@IndentID, @TotalIndent,'{10}','{11}','{8}','{15}','{16}')
    
    exec uspfn_omRepostingIndent
end
else
begin
	INSERT INTO [omMstIndent]([TypeCode],[MarketModelCode],[Variant],[ModelYear],[Year],[Month],[ColourCode],[OriginalQuotaUnits],[QuotaUnits]
			   ,[AllocationUnits],[UnitStatus],[ColourStatus],[isNeedReposting],[CreatedBy],[CreatedDate],[LastUpdateBy],[LastUpdateDate])
		 VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','0','{13}','{14}','{15}','{16}')
end ";
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
                                            string tempQuery = @"if(select COUNT(*) from omMstIndent where TypeCode = '{0}' and MarketModelCode = '{1}' and Variant = '{2}' and ModelYear = '{3}' and Year = '{4}' and Month = '{5}' and ColourCode = '{6}' ) > 0
                                                    begin
	                                                    UPDATE [omMstIndent]
	                                                       SET [QuotaUnits] = '{8}', [UnitStatus] = '{10}', [ColourStatus] = '{11}' ,[isNeedReposting] = '1',[LastUpdateBy] = '{15}',[LastUpdateDate] = '{16}'
	                                                     WHERE [TypeCode] = '{0}' and [MarketModelCode] = '{1}' and [Variant] = '{2}' and [ModelYear] = '{3}' and [Year] = '{4}' and [Month] = '{5}' and [ColourCode] = '{6}'
                                                    	
	                                                    set @IndentID = (select TOP 1 IndentID from omMstIndent where TypeCode = '{0}' and MarketModelCode = '{1}' and Variant = '{2}' and ModelYear = '{3}' and Year = '{4}' and Month = '{5}' and ColourCode = '{6}')
	                                                    set @TotalIndent = (select COUNT(*) from omHstIndent where IndentID = @IndentID)
	                                                    INSERT INTO [omHstIndent]([IndentID],[LogSeq],[UnitStatus],[ColourStatus],[QuotaUnits],[LastUpdateBy],[LastUpdateDate])
                                                         VALUES(@IndentID, @TotalIndent,'{10}','{11}','{8}','{15}','{16}')
                                                    end
                                                    else
                                                    begin
	                                                    INSERT INTO [omMstIndent]([TypeCode],[MarketModelCode],[Variant],[ModelYear],[Year],[Month],[ColourCode],[OriginalQuotaUnits],[QuotaUnits]
			                                                       ,[AllocationUnits],[UnitStatus],[ColourStatus],[isNeedReposting],[CreatedBy],[CreatedDate],[LastUpdateBy],[LastUpdateDate])
		                                                     VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','0','{13}','{14}','{15}','{16}')
                                                    end ";

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
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{13}", CurrentUser.UserId);
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{14}", DateTime.Now.ToString("yyyyMMdd"));
                                            stringQuery[loopCountDouble] = stringQuery[loopCountDouble].Replace("{15}", CurrentUser.UserId);
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
                                            query = query.Replace("{13}", CurrentUser.UserId);
                                            query = query.Replace("{14}", DateTime.Now.ToString("yyyyMMdd"));
                                            query = query.Replace("{15}", CurrentUser.UserId);
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
                        result = ctx.Database.ExecuteSqlCommand(@"Declare @IndentID int 
                        Declare @TotalIndent int
                        " + execQuery) > 0;
                        sb = new StringBuilder();
                        query = string.Empty;
                        count = 0;
                    }
                    else
                    {
                        msg ="flat file tidak valid";
                        return false;
                    }
                }
            }
            else
            {
                msg ="flat file tidak valid";
                return false;
            }

            return result;
        } 
        
        private UploadBLL.BPUNobyReffFJ GetBPUNobyReffFJ(string SJNo)
        {
            var data = from a in ctx.omTrPurchaseBPU
                       join b in ctx.omTrPurchaseBPUDetail
                       on new {a.CompanyCode,a.BranchCode,a.PONo,a.BPUNo}
                       equals new {b.CompanyCode,b.BranchCode,b.PONo,b.BPUNo} into _b
                       from b in _b.DefaultIfEmpty()
                       where a.CompanyCode == CompanyCode
                       && a.BranchCode == BranchCode
                       && a.RefferenceSJNo == SJNo
                       && a.Status != "3"
                       && b.BPUSeq == 1
                       select new UploadBLL.BPUNobyReffFJ
                       {
                           PONo = a.PONo,
                           BPUNo = a.BPUNo
                       };

            return data.FirstOrDefault();
        }

        private DetailVehicle SelectDetailVehicle(string chassisCode, decimal chassisNo)
        {
            var data = from a in ctx.OmMstVehicles
                       join b in ctx.omTrPurchaseBPUDetail
                       on new { a.CompanyCode, a.ChassisCode, a.ChassisNo }
                       equals new { b.CompanyCode, b.ChassisCode, b.ChassisNo }
                       join c in ctx.omTrPurchaseBPU
                       on new { b.CompanyCode, b.BranchCode, b.BPUNo }
                       equals new { c.CompanyCode, c.BranchCode, c.BPUNo }
                       join d in ctx.omTrSalesReqDetail
                       on new { b.CompanyCode, b.ChassisCode, b.ChassisNo }
                       equals new { d.CompanyCode, d.ChassisCode, d.ChassisNo } into _d
                       from d in _d.DefaultIfEmpty()
                       where a.CompanyCode == CompanyCode && a.ChassisCode == chassisCode && a.ChassisNo == chassisNo
                       select new DetailVehicle
                       {
                           ChassisCode = a.ChassisCode,
                           ChassisNo = a.ChassisNo,
                           SalesModelCode = a.SalesModelCode,
                           SalesModelYear = a.SalesModelYear,
                           ColourCode = a.ColourCode,
                           EngineCode = a.EngineCode,
                           EngineNo = a.EngineNo,
                           RefferenceDONo = c.RefferenceDONo,
                           RefferenceSJNo = c.RefferenceSJNo,
                           ReqNo = d.ReqNo ?? ""
                       };

            return data.FirstOrDefault();
        }

        public class DetailVehicle
        {
            public string ChassisCode { get; set; }
            public decimal ChassisNo { get; set; }
            public string SalesModelCode { get; set; }
            public decimal? SalesModelYear { get; set; }
            public string ColourCode { get; set; }
            public string EngineCode { get; set; }
            public decimal? EngineNo { get; set; }
            public string RefferenceDONo { get; set; }
            public string RefferenceSJNo { get; set; }
            public string ReqNo { get; set; }
        }

    }
}