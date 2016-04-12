using SimDms.Sales.Models;
using SimDms.Sales.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers.Api
{
    public class SalesModelController : BaseController
    {
        public JsonResult Save(SalesModelForm model)
        {
            ResultModel result = InitializeResult();
            string userID = CurrentUser.UserId;
            DateTime currentDate = DateTime.Now;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;

            if (string.IsNullOrWhiteSpace(model.SalesModelCode))
            {
                result.message = "Sales Model Code harus diisi.";
                return Json(result);
            }

            if (model.AfterDiscTotal == null || model.AfterDiscTotal <= 0)
            {
                result.message = "Total harga harus diisi.";
                return Json(result);
            }

            if (model.DPP == null || model.DPP == 0)
            {
                result.message = "DPP harus diisi.";
                return Json(result);
            }

            decimal? minStaff = 0;
            var omMstPriceListSell = ctx.OmMstPricelistSells.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.GroupPriceCode == model.GroupPriceCode && x.SalesModelCode == model.SalesModelCode && x.SalesModelYear == model.SalesModelYear).FirstOrDefault();
            if (omMstPriceListSell != null)
            {
                minStaff = omMstPriceListSell.TotalMinStaff;
            }
            else
            {
                result.message = "Pricelist jual belum ada.";
                return Json(result);
            }

            var omTrSalesSO = ctx.OmTRSalesSOs.Find(companyCode, branchCode, model.SONumber);
            if (omTrSalesSO != null)
            {
                omTrSalesSO.Status = "0";
                omTrSalesSO.LastUpdateBy = userID;
                omTrSalesSO.LastUpdateDate = currentDate;
            }
            else
            {
                result.message = "No. SO tidak ditemukan.";
                return Json(result);
            }

            if (!string.IsNullOrWhiteSpace(omTrSalesSO.ProspectNo))
            {
                var lockITS = ctx.LookUpDtls.Where(x => x.CompanyCode == companyCode && x.CodeID == "LockSOITS" && x.SeqNo == 0 && x.LookUpValue.Trim().ToLower() == "Status" && x.ParaValue == "1");
                if (lockITS != null)
                {
                    var itsNumber = Convert.ToInt32(model.ITSNumber);
                    var pmKDP = ctx.PmKdps.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.InquiryNumber == itsNumber).FirstOrDefault();
                    if (pmKDP != null)
                    {
                        var omMstModel = ctx.OmMstModels.Find(companyCode, model.SalesModelCode);
                        if (omMstModel != null)
                        {
                            if (pmKDP.TipeKendaraan != omMstModel.GroupCode)
                            {
                                result.message = "Kendaraan yang dipilih harus sama dengan kendaraan yang diisi pada ITS.";
                                return Json(result);
                            }
                        }
                        else
                        {
                            result.message = "Tipe kendaraan tidak terdaftar di master model.";
                            return Json(result);
                        }
                    }
                    else
                    {
                        result.message = "Kendaraan tidak terdaftar di ITS.";
                    }
                }
            }

            var omTrSalesSOModel = ctx.OmTrSalesSOModels.Find(companyCode, branchCode, model.SONumber, model.SalesModelCode, model.SalesModelYear);
            if (omTrSalesSOModel == null)
            {
                omTrSalesSOModel = new OmTrSalesSOModel();
                omTrSalesSOModel.CompanyCode = companyCode;
                omTrSalesSOModel.BranchCode = branchCode;
                omTrSalesSOModel.CreatedBy = userID;
                omTrSalesSOModel.CreatedDate = DateTime.Now;
                omTrSalesSOModel.SONo = model.SONumber;
                omTrSalesSOModel.SalesModelCode = model.SalesModelCode;
                omTrSalesSOModel.SalesModelYear = model.SalesModelYear;
                omTrSalesSOModel.QuantitySO = 0;
                omTrSalesSOModel.QuantityDO = 0;

                ctx.OmTrSalesSOModels.Add(omTrSalesSOModel);
            }
            else
            {
                result.message = "Data Sales Model sudah ada di dalam database. Jika anda ingin mengubah data, hapus dulu data Sales Model ini.";
                return Json(result);
            }

            omTrSalesSOModel.ChassisCode = model.ChassisCode;
            omTrSalesSOModel.BeforeDiscDPP = model.BeforeDiscDPP;
            omTrSalesSOModel.BeforeDiscPPn = model.BeforeDiscPPn;
            omTrSalesSOModel.BeforeDiscPPnBM = model.BeforeDiscPPnBM;
            omTrSalesSOModel.BeforeDiscTotal = model.BeforeDiscTotal;
            omTrSalesSOModel.DiscExcludePPn = (model.BeforeDiscDPP - model.DPP);
            omTrSalesSOModel.DiscIncludePPn = (model.BeforeDiscTotal - model.AfterDiscTotal);
            omTrSalesSOModel.BeforeDiscTotal = model.BeforeDiscTotal;
            omTrSalesSOModel.AfterDiscDPP = model.DPP;
            omTrSalesSOModel.AfterDiscPPn = model.PPn;
            omTrSalesSOModel.AfterDiscPPnBM = model.PPnBM;
            omTrSalesSOModel.AfterDiscTotal = model.AfterDiscTotal;
            omTrSalesSOModel.Remark = model.Remark;
            omTrSalesSOModel.OthersAmt = omTrSalesSOModel.OthersAmt ?? 0;
            omTrSalesSOModel.OthersDPP = omTrSalesSOModel.OthersDPP ?? 0;
            omTrSalesSOModel.OthersPPn = omTrSalesSOModel.OthersPPn ?? 0;
            omTrSalesSOModel.QuantityDO = omTrSalesSOModel.QuantityDO ?? 0;
            omTrSalesSOModel.QuantitySO = omTrSalesSOModel.QuantitySO ?? 0;
            omTrSalesSOModel.ShipAmt = model.ShipAmt ?? 0;
            omTrSalesSOModel.DepositAmt = model.DepositAmt ?? 0;
            omTrSalesSOModel.OthersAmt = model.OthersAmt ?? 0;
            
            try
            {
                ctx.SaveChanges();

                ctx.Database.ExecuteSqlCommand("exec uspfn_UpdateTotalUnit @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2, @SalesModelCode=@p3, @SalesModelYear=@p4", companyCode, branchCode, model.SONumber, model.SalesModelCode, model.SalesModelYear);
                decimal totalUnit = ctx.Database.SqlQuery<decimal>("exec uspfn_GetTotalUnit @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", companyCode, branchCode, model.SONumber).FirstOrDefault();
                
                result.status = true;
                result.message = "Data Sales Model berhasil disimpan.";
            }
            catch (Exception)
            {
                result.message = "Data Sales Model gagal disimpan.";
            } 

            return Json(result);
        }

        public JsonResult LoadPriceList()
        {
            ResultModel result = InitializeResult();
            string salesModelCode = Request["SalesModelCode"] ?? "";
            decimal? salesModelYear = Convert.ToDecimal(Request["SalesModelYear"] ?? "0");
            string groupPriceCode = Request["GroupPriceCode"] ?? "";

            var data = ctx.OmMstPricelistSells.Where(x =>
                x.CompanyCode == CompanyCode
                &&
                x.BranchCode == BranchCode
                &&
                x.SalesModelCode == salesModelCode
                &&
                x.SalesModelYear == salesModelYear
                &&
                x.GroupPriceCode == groupPriceCode
            ).FirstOrDefault();

            if (data != null)
            {
                result.status = true;
                result.data = data;
            }

            return Json(result);
        }

        public JsonResult Delete(OmTrSalesSOModel model)
        {
            ResultModel result = InitializeResult();

            string companyCode = CompanyCode;
            string branchCode = BranchCode;

            var colourModels = ctx.OmTrSalesSOModelColours.Where(x =>
                       x.CompanyCode == companyCode
                    && x.BranchCode == branchCode
                    && x.SONo == model.SONo
                    && x.SalesModelCode == model.SalesModelCode
                    && x.SalesModelYear == model.SalesModelYear
                );

            if (colourModels.Count() > 0)
            {
                result.message = "Data Sales Model tidak dapat dihapus, karena masih memiliki detail Colour Model";
                return Json(result);
            }

            var data = ctx.OmTrSalesSOModels.Find(model.CompanyCode, model.BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear);
            if (data != null)
            {
                ctx.OmTrSalesSOModels.Remove(data);

                try
                {
                    ctx.SaveChanges();

                    result.status = true;
                    result.message = "Data Sales Model berhasil dihapus.";
                }
                catch (Exception)
                {
                    result.message = "Data Sales Model gagal dihapus.";
                }
            }
            else
            {
                result.message = "Tidak ada data Sales Model yang bisa dihapus.";
            }

            return Json(result);
        }
    }
}
