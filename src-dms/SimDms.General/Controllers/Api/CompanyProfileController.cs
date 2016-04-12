using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.General.Models.Others;
using SimDms.Common;
using SimDms.Common.Models;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;
using SimDms.Sparepart.Models;


namespace SimDms.General.Controllers.Api
{
    public class CompanyProfileController : BaseController
    {
        public JsonResult Header(CoProfile model)
        {
            var me = ctx.CoProfiles.Find(CompanyCode, model.BranchCode);
            return Json(new { success = true, data = me });
        }

        public JsonResult DetailSpare(CoProfile model)
        {
            var me = ctx.CoProfileSpare.Find(CompanyCode, model.BranchCode);
            return Json(new { success = true, data = me });
        }

        public JsonResult DetailService(CoProfile model)
        {
            var me = ctx.CoProfileService.Find(CompanyCode, model.BranchCode);
            return Json(new { success = true, data = me });
        }

        public JsonResult DetailUnit(CoProfile model)
        {
            var me = ctx.CoProfileSales.Find(CompanyCode, model.BranchCode);
            return Json(new { success = true, data = me });
        }

        public JsonResult DetailFinance(CoProfile model)
        {
            var me = ctx.CoProfileFinance.Find(CompanyCode, model.BranchCode);
            return Json(new { success = true, data = me });
        }

        [HttpPost]
        public JsonResult Save(CoProfile model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.CoProfiles.Find(companyCode, model.BranchCode);

            if (me == null)
            {
                me = new CoProfile();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.CoProfiles.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = model.BranchCode;
            me.CompanyName = model.CompanyName;
            me.CompanyGovName = model.CompanyGovName;
            me.Address1 = model.Address1;
            me.Address2 = model.Address2;
            me.Address3 = model.Address3;
            me.Address4 = model.Address4;
            me.ZipCode = model.ZipCode;
            me.IsPKP = model.IsPKP;
            me.SKPNo = model.SKPNo;
            me.SKPDate = model.SKPDate;
            me.NPWPNo = model.NPWPNo;
            me.NPWPDate = model.NPWPDate;
            me.CityCode = model.CityCode;
            me.AreaCode = model.AreaCode;
            me.PhoneNo = model.PhoneNo;
            me.FaxNo = model.FaxNo;
            me.OwnershipName = model.OwnershipName;
            me.TaxTransCode = model.TaxTransCode;
            me.TaxCabCode = model.TaxCabCode;
            me.IsFPJCentralized = model.IsFPJCentralized;
            me.ProductType = model.ProductType;
            me.IsLinkToService = model.IsLinkToService;
            me.IsLinkToSpare = model.IsLinkToSpare;
            me.IsLinkToSales = model.IsLinkToSales;
            me.IsLinkToFinance = model.IsLinkToFinance;
            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Company Profile berhasil disimpan.";
                result.data = new
                {
                    CompanyName = me.CompanyName,
                    CompanyGovName = me.CompanyGovName
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Company Profile tidak bisa disimpan.";
                MyLogger.Info("Error on Company Profile saving: " + Ex.Message);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Save2(GnMstCoProfileSpare model) 
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.CoProfileSpare.Find(CompanyCode, model.BranchCode);

            if (me == null)
            {
                me = new GnMstCoProfileSpare();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.CoProfileSpare.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = model.BranchCode;
            me.BOPeriod = model.BOPeriod;
            me.ABCClassAPct = model.ABCClassAPct;
            me.ABCClassBPct = model.ABCClassBPct;
            me.ABCClassCPct = model.ABCClassCPct;
            me.FiscalYear = model.FiscalYear;
            me.FiscalMonth = model.FiscalMonth;
            me.FiscalPeriod = model.FiscalPeriod;
            me.PeriodBeg = model.PeriodBeg;
            me.PeriodEnd = model.PeriodEnd;
            me.ContactPersonName = model.ContactPersonName;
            me.FaxNo = model.FaxNo;
            me.PhoneNo = model.PhoneNo;
            me.HandphoneNo = model.HandphoneNo;
            me.EmailAddr = model.EmailAddr;
            me.isPurchasePriceIncPPN = model.isPurchasePriceIncPPN;
            me.isRetailPriceIncPPN = model.isRetailPriceIncPPN;
            me.IsLinkWRS = model.IsLinkWRS;
            me.TransDate = model.TransDate;
            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Company Profile Spare Part berhasil disimpan.";
                result.data = new
                {
                    CompanyCode = me.CompanyCode,
                    BranchCode = me.BranchCode
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Company Profile Spare Part tidak bisa disimpan.";
                MyLogger.Info("Error on Company Profile Spare Part saving: " + Ex.Message);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Save3(GnMstCoProfileService model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.CoProfileService.Find(CompanyCode, model.BranchCode);

            if (me == null)
            {
                me = new GnMstCoProfileService();
                me.CreatedDate = currentTime;
                me.LastupdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.CoProfileService.Add(me);
            }
            else
            {
                me.LastupdateDate = currentTime;
                me.LastupdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = model.BranchCode;
            me.ContactPersonName = model.ContactPersonName;
            me.PhoneNo = model.PhoneNo;
            me.FaxNo = model.FaxNo;
            me.HandPhoneNo = model.HandPhoneNo;
            me.EmailAddr = model.EmailAddr;
            me.MOUNo = model.MOUNo;
            me.MOUDate = model.MOUDate;
            me.BuildingOwnership = model.BuildingOwnership;
            me.LandOwnership = model.LandOwnership;
            me.FiscalYear = model.FiscalYear;
            me.FiscalMonth = model.FiscalMonth;
            me.FiscalPeriod = model.FiscalPeriod;
            me.PeriodBeg = model.PeriodBeg;
            me.PeriodEnd = model.PeriodEnd;
            me.EstimateTimeFlag = model.EstimateTimeFlag;
            me.TransDate = model.TransDate;
            me.DealerCodeWSMR = model.DealerCodeWSMR;
            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Company Profile Service berhasil disimpan.";
                result.data = new
                {
                    CompanyCode = me.CompanyCode,
                    BranchCode = me.BranchCode
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Company Profile Service tidak bisa disimpan.";
                MyLogger.Info("Error on Company Profile Service saving: " + Ex.Message);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Save4(GnMstCoProfileSales model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.CoProfileSales.Find(CompanyCode, model.BranchCode);

            if (me == null)
            {
                me = new GnMstCoProfileSales();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.CoProfileSales.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = model.BranchCode;
            me.FiscalYear = model.FiscalYear;
            me.FiscalMonth = model.FiscalMonth;
            me.FiscalPeriod = model.FiscalPeriod;
            me.PeriodBeg = model.PeriodBeg;
            me.PeriodEnd = model.PeriodEnd;
            me.ContactPersonName = model.ContactPersonName;
            me.FaxNo = model.FaxNo;
            me.PhoneNo = model.PhoneNo;
            me.HandPhoneNo = model.HandPhoneNo;
            me.EmailAddr = model.EmailAddr;
            me.TransDate = model.TransDate;
            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Company Profile Sales berhasil disimpan.";
                result.data = new
                {
                    CompanyCode = me.CompanyCode,
                    BranchCode = me.BranchCode
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Company Profile Sales tidak bisa disimpan.";
                MyLogger.Info("Error on Company Profile Sales saving: " + Ex.Message);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Save5(CoProfileFinance model) 
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.CoProfileFinance.Find(CompanyCode, model.BranchCode);

            if (me == null)
            {
                me = new CoProfileFinance();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.CoProfileFinance.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = model.BranchCode;
            me.FiscalYear = model.FiscalYear;
            me.FiscalMonth = model.FiscalMonth;
            me.FiscalPeriod = model.FiscalPeriod;
            me.PeriodBeg = model.PeriodBeg;
            me.PeriodEnd = model.PeriodEnd;
            me.FiscalYearAR = model.FiscalYearAR;
            me.FiscalMonthAR = model.FiscalMonthAR;
            me.FiscalPeriodAR = model.FiscalPeriodAR;
            me.PeriodBegAR = model.PeriodBegAR;
            me.PeriodEndAR = model.PeriodEndAR;
            me.FiscalYearGL = model.FiscalYearGL;
            me.FiscalMonthGL = model.FiscalMonthGL;
            me.FiscalPeriodGL = model.FiscalPeriodGL;
            me.PeriodBegGL = model.PeriodBegGL;
            me.PeriodEndGL = model.PeriodEndGL;
            me.TransDateAP = model.TransDateAP;
            me.TransDateAR = model.TransDateAR;
            me.TransDateGL = model.TransDateGL;
            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Company Profile Sales berhasil disimpan.";
                result.data = new
                {
                    CompanyCode = me.CompanyCode,
                    BranchCode = me.BranchCode
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Company Profile Finance tidak bisa disimpan.";
                MyLogger.Info("Error on Company Profile Finance saving: " + Ex.Message);
            }

            return Json(result);
        }

        public JsonResult Delete(CoProfile model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = model.BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.CoProfiles.Find(companyCode, branchCode);
                    if (me != null)
                    {
                        ctx.CoProfiles.Remove(me);
                        var me2 = ctx.CoProfileSpare.Find(companyCode, branchCode);
                        var me3 = ctx.CoProfileService.Find(companyCode, branchCode);
                        var me4 = ctx.CoProfileSales.Find(companyCode, branchCode);
                        var me5 = ctx.CoProfileFinance.Find(companyCode, branchCode);
                        if (me2 != null)
                        {
                            ctx.CoProfileSpare.Remove(me2);
                            //ctx.SaveChanges();
                        }
                        
                        if (me3 != null)
                        {
                            ctx.CoProfileService.Remove(me3);
                            //ctx.SaveChanges();
                        }

                        if (me4 != null)
                        {
                            ctx.CoProfileSales.Remove(me4);
                            //ctx.SaveChanges();
                        }

                        if (me5 != null)
                        {
                            ctx.CoProfileFinance.Remove(me5);
                            //ctx.SaveChanges();
                        }
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Company Profile berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Company Profile , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Company Profile , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        public JsonResult Delete2(GnMstCoProfileSpare model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.CoProfileSpare.Find(companyCode, branchCode);
                    if (me != null)
                    {
                        ctx.CoProfileSpare.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Company Profile Spare berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Company Profile Spare, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Company Profile Spare , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
    }
}
