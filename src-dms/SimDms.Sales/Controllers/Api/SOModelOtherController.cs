using SimDms.Sales.Models;
using SimDms.Sales.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers.Api
{
    public class SOModelOtherController : BaseController
    {
        public JsonResult Save(SalesSOModelOthersForm model)
        {
            ResultModel result = InitializeResult();
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string userId = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            if (string.IsNullOrWhiteSpace(model.RefferenceCode))
            {
                result.message = "Kode Acc Others harus diisi.";
                return Json(result);
            }

            if (string.IsNullOrWhiteSpace(model.CustomerCode))
            {
                result.message = "Anda belum memilih customer.";
                return Json(result);
            }

            if (string.IsNullOrWhiteSpace(model.SONumber))
            {
                result.message = "Anda belum memilih SO Number.";
                return Json(result);
            }

            OmTRSalesSO omTrSalesSO = ctx.OmTRSalesSOs.Find(companyCode, branchCode, model.SONumber);
            if (omTrSalesSO == null)
            {
                result.message = "SO tidak ditemukan.";
                return Json(result);
            }
            else
            {
                omTrSalesSO.Status = "0";
                omTrSalesSO.LastUpdateBy = userId;
                omTrSalesSO.LastUpdateDate = currentTime;
            }

            if (model.SalesModelYear == 0)
            {
                result.message = "Anda belum memiliki data SO Model / anda tidak mengidentifikasi Sales Model Year.";
            }

            OmTrSalesSoModelOther omTrSalesSOmodelOthers = ctx.OmTrSalesSoModelOthers.Find(companyCode, branchCode, model.SONumber, model.SalesModelCode, model.SalesModelYear, model.RefferenceCode);
            if (omTrSalesSOmodelOthers != null)
            {
                result.message = "Data Accesories sudah ada, silakan hapus dulu lalu tambahkan yang baru untuk mengubah data.";
                return Json(result);
            }

            omTrSalesSOmodelOthers = new OmTrSalesSoModelOther();
            omTrSalesSOmodelOthers.CompanyCode = companyCode;
            omTrSalesSOmodelOthers.BranchCode = branchCode;
            omTrSalesSOmodelOthers.CreatedBy = userId;
            omTrSalesSOmodelOthers.CreatedDate = DateTime.Now;
            omTrSalesSOmodelOthers.LastUpdateBy = userId;
            omTrSalesSOmodelOthers.LastUpdateDate = DateTime.Now;
            omTrSalesSOmodelOthers.OtherCode = model.RefferenceCode;
            omTrSalesSOmodelOthers.SalesModelCode = model.SalesModelCode;
            omTrSalesSOmodelOthers.SalesModelYear = model.SalesModelYear;
            omTrSalesSOmodelOthers.SONo = model.SONumber;
            omTrSalesSOmodelOthers.Remark = model.AccOthersRemark;

            decimal? ppnPct = ctx.Database.SqlQuery<decimal?>("exec uspfn_SalesSOTax @CompanyCode=@p0, @CustomerCode=@p1", companyCode, model.CustomerCode).FirstOrDefault();
            if (ppnPct == null || ppnPct == 0)
            {
                ppnPct = 0;
            }

            if (model.AccOthersTotalBeforeDisc < model.AccOthersTotalAfterDisc)
            {
                model.AccOthersTotalBeforeDisc = model.AccOthersTotalAfterDisc;
            }

            omTrSalesSOmodelOthers.BeforeDiscTotal = model.AccOthersTotalBeforeDisc;
            omTrSalesSOmodelOthers.AfterDiscTotal = model.AccOthersTotalAfterDisc;

            omTrSalesSOmodelOthers.BeforeDiscDPP = model.AccOthersTotalBeforeDisc / ((100 + ppnPct) / 100);
            omTrSalesSOmodelOthers.BeforeDiscPPn = model.AccOthersTotalBeforeDisc - omTrSalesSOmodelOthers.BeforeDiscDPP;

            omTrSalesSOmodelOthers.AfterDiscDPP = model.AccOthersTotalAfterDisc / ((100 + ppnPct) / 100);
            omTrSalesSOmodelOthers.AfterDiscPPn = model.AccOthersTotalAfterDisc - omTrSalesSOmodelOthers.AfterDiscDPP;

            omTrSalesSOmodelOthers.DiscIncludePPn = model.AccOthersTotalBeforeDisc - model.AccOthersTotalAfterDisc;
            omTrSalesSOmodelOthers.DiscExcludePPn = omTrSalesSOmodelOthers.BeforeDiscDPP - omTrSalesSOmodelOthers.AfterDiscDPP;

            omTrSalesSOmodelOthers.Total = omTrSalesSOmodelOthers.AfterDiscTotal;
            omTrSalesSOmodelOthers.PPn = omTrSalesSOmodelOthers.AfterDiscPPn;
            omTrSalesSOmodelOthers.DPP = omTrSalesSOmodelOthers.AfterDiscDPP;

            ctx.OmTrSalesSoModelOthers.Add(omTrSalesSOmodelOthers);

            try
            {
                ctx.SaveChanges();

                result.message = "Data Acc Others berhasil disimpan.";
                result.status = true;
            }
            catch (Exception)
            {
                result.message = "";
            }

            return Json(result);
        }

        public JsonResult Delete(OmTrSalesSOModelOtherList model)
        {
            ResultModel result = InitializeResult();

            var data = ctx.OmTrSalesSoModelOthers.Find(model.CompanyCode, model.BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear, model.OtherCode);
            if (data == null)
            {
                result.message = "Tidak dapat menemukan data Acc Others untuk dihapus.";
                return Json(result);
            }

            ctx.OmTrSalesSoModelOthers.Remove(data);

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Data Acc Others berhasil dihapus.";
            }
            catch (Exception)
            {
                result.message = "Tidak dapat menghapus data Acc Others.";
            }

            return Json(result);
        }
    }
}
