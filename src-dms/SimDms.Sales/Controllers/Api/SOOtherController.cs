using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.Models;
using SimDms.Sales.Models.Result;

namespace SimDms.Sales.Controllers.Api
{
    public class SOOtherController : BaseController
    {
        public JsonResult Save(SOOtherFormModel model)
        {
            ResultModel result = InitializeResult();
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var lookupBNN = ctx.LookUpDtls.Where(x => x.CompanyCode == companyCode && x.CodeID == "BBNFL" && x.LookUpValue == "STATUS").FirstOrDefault();
            if (lookupBNN != null && lookupBNN.ParaValue == "1" && model.BBN == null)
            {
                result.message = "BBN harus diisi.";
                return Json(result);
            }

            var lookupDNUFL = ctx.LookUpDtls.Where(x => x.CompanyCode == companyCode && x.CodeID == "BBNFL" && x.LookUpValue == "STATUS").FirstOrDefault();
            if (lookupDNUFL != null && lookupDNUFL.ParaValue == "1")
            {
                if ((model.BBN + model.KIR) > 0 && string.IsNullOrWhiteSpace(model.SupplierCode))
                {
                    result.message = "Supplier BBN harus diisi terlebih dahulu.";
                    return Json(result);
                }
            }

            // PERLU DITANYAKAN
            /*
                record = OmTrSalesSOBLL.GetRecord(user.CompanyCode, user.BranchCode, txtSONo.Text);
                if (record != null)
                {
                    if (record.Status != "0" && record.Status != "1") { PopulateRecord(record); return; }
                }
             */

            if (string.IsNullOrWhiteSpace(model.STNKName))
            {
                result.message = "End user STNK harus diisi.";
                return Json(result);
            }

            var countVin = ctx.omTrSalesSOVins.Where(x =>
                       x.CompanyCode == companyCode
                    && x.BranchCode == branchCode
                    && x.SONo == model.SONumber
                    && x.SalesModelCode == model.SalesModelCode
                    && x.SalesModelYear == model.SalesModelYear
                    && x.ColourCode == model.ColourCode
                ).Count();
            var sumQuantity = ctx.OmTrSalesSOModelColours.Where(x =>
                       x.CompanyCode == companyCode
                    && x.BranchCode == branchCode
                    && x.SONo == model.SONumber
                    && x.SalesModelCode == model.SalesModelCode
                    && x.SalesModelYear == model.SalesModelYear
                    && x.ColourCode == model.ColourCode
                ).Select(x => x.Quantity).Sum();

            if (countVin >= sumQuantity)
            {
                result.message = "Jumlah Detail Lain-Lain tidak boleh lebih dari jumlah Detil Warna.";
                return Json(result);
            }

            var omTrSalesSo = ctx.OmTRSalesSOs.Where(x =>
                       x.CompanyCode == companyCode
                    && x.BranchCode == branchCode
                    && x.SONo == model.SONumber
                ).FirstOrDefault();
            if (omTrSalesSo != null)
            {
                omTrSalesSo.Status = "0";
                omTrSalesSo.CreatedBy = userID;
                omTrSalesSo.CreatedDate = currentTime;
            }
            else
            {
                result.message = "Tidak bisa menemukan data SO.";
                return Json(result);
            }

            var omTrSalesSOVin = ctx.omTrSalesSOVins.Where(x =>
                       x.CompanyCode == companyCode
                    && x.BranchCode == branchCode
                    && x.SONo == model.SONumber
                    && x.SalesModelCode == model.SalesModelCode
                    && x.SalesModelYear == model.SalesModelYear
                    && x.ColourCode == model.ColourCode
                    && x.ChassisCode == model.ChassisCode
                    && x.ChassisNo == model.ChassisNo
                ).FirstOrDefault();

            if (omTrSalesSOVin == null)
            {
                omTrSalesSOVin = new omTrSalesSOVin();
                omTrSalesSOVin.CompanyCode = companyCode;
                omTrSalesSOVin.BranchCode = branchCode;
                omTrSalesSOVin.SONo = model.SONumber;
                omTrSalesSOVin.SalesModelCode = model.SalesModelCode;
                omTrSalesSOVin.SalesModelYear = model.SalesModelYear;
                omTrSalesSOVin.ColourCode = model.ColourCode;
                omTrSalesSOVin.ChassisCode = model.ChassisCode;
                omTrSalesSOVin.ChassisNo = model.ChassisNo;
                omTrSalesSOVin.CreatedBy = userID;
                omTrSalesSOVin.CreatedDate = currentTime;
                omTrSalesSOVin.SOSeq = ctx.omTrSalesSOVins.Where(x =>
                           x.CompanyCode == companyCode
                        && x.BranchCode == branchCode
                        && x.SONo == model.SONumber
                        && x.SalesModelCode == model.SalesModelCode
                        && x.SalesModelYear == model.SalesModelYear
                        && x.ColourCode == model.ColourCode
                ).Count() + 1;

                ctx.omTrSalesSOVins.Add(omTrSalesSOVin);
            }

            omTrSalesSOVin.LastUpdateBy = userID;
            omTrSalesSOVin.LastUpdateDate = currentTime;
            omTrSalesSOVin.Remark = model.RemarkOther;
            omTrSalesSOVin.BBN = model.BBN;
            omTrSalesSOVin.KIR = model.KIR;
            omTrSalesSOVin.CityCode = model.CityCode;
            omTrSalesSOVin.StatusReq = '0';
            omTrSalesSOVin.SupplierBBN = model.SupplierCode;
            omTrSalesSOVin.EndUserName = model.STNKName;
            omTrSalesSOVin.EndUserAddress1 = model.STNKAddress1;
            omTrSalesSOVin.EndUserAddress2 = model.STNKAddress2;
            omTrSalesSOVin.EndUserAddress3 = model.STNKAddress3;


            var omMstVehicle = ctx.OmMstVehicles.Find(companyCode, model.ChassisCode, model.ChassisNo);
            if (omMstVehicle != null)
            {
                omTrSalesSOVin.EngineCode = omMstVehicle.EngineCode;
                omTrSalesSOVin.EngineNo = omMstVehicle.EngineNo;
                omTrSalesSOVin.KeyNo = omMstVehicle.KeyNo;
                omTrSalesSOVin.ServiceBookNo = omMstVehicle.ServiceBookNo;
            }
            else
            {
                result.message = "Tidak bisa menemukan data others untuk ditambahkan.";
                return Json(result);
            }


            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Data Other berhasil disimpan.";
            }
            catch (Exception)
            {
                result.message = "Data other gagal disimpan.";
            }

            return Json(result);
        }

        public JsonResult Delete(omTrSalesSOVin model)
        {
            ResultModel result = InitializeResult();
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var omTrSalesSO = ctx.OmTRSalesSOs.Where(x =>
                       x.CompanyCode == model.CompanyCode
                    && x.BranchCode == model.BranchCode
                    && x.SONo == model.SONo
                ).FirstOrDefault();

            if (omTrSalesSO != null)
            {
                omTrSalesSO.Status = "0";
                omTrSalesSO.CreatedBy = userID;
                omTrSalesSO.CreatedDate = DateTime.Now;
            }
            else
            {
                result.message = "Tidak bisa menemukan data SO.";
                return Json(result);
            }

            var omTrSalesSOVin = ctx.omTrSalesSOVins.Where(x =>
                       x.CompanyCode == companyCode
                    && x.BranchCode == branchCode
                    && x.SONo == model.SONo
                    && x.SalesModelCode == model.SalesModelCode
                    && x.SalesModelYear == model.SalesModelYear
                    && x.SOSeq == model.SOSeq
                    && x.ColourCode == model.ColourCode
                ).FirstOrDefault();

            if (omTrSalesSOVin != null)
            {
                ctx.omTrSalesSOVins.Remove(omTrSalesSOVin);
            }
            else
            {
                result.message = "Tidak bisa menemukan data Others untuk dihapus.";
                return Json(result);
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Data others berhasil dihapus.";
            }
            catch (Exception)
            {
                result.message = "Data others tidak dapat dihapus.";
            }

            return Json(result);
        }
    }
}
