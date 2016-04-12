using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.Models;
using SimDms.Sales.Models.Result;

namespace SimDms.Sales.Controllers.Api
{
    public class ColourModelController : BaseController
    {
        public JsonResult Save(ColourModelForm model)
        {
            ResultModel result = InitializeResult();
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string userID = CurrentUser.UserId;
            DateTime currentDate = DateTime.Now;

            if (string.IsNullOrWhiteSpace(model.ColourCode))
            {
                result.message = "Kode warna harus diisi.";
                return Json(result);
            }

            if (model.Quantity == null || model.Quantity == 0)
            {
                result.message = "Jumlah tidak boleh kosong atau 0.";
                return Json(result);
            }

            var data = ctx.OmTrSalesSOModelColours.Where(x =>
                            x.CompanyCode == companyCode
                         && x.BranchCode == branchCode
                         && x.SONo == model.SONumber
                         && x.SalesModelCode == model.SalesModelCode
                         && x.SalesModelYear == model.SalesModelYear
                         && x.ColourCode == model.ColourCode
                       ).FirstOrDefault();

            if (data == null)
            {
                data = new OmTrSalesSOModelColour();
                data.CompanyCode = companyCode;
                data.BranchCode = branchCode;
                data.SONo = model.SONumber;
                data.SalesModelCode = model.SalesModelCode;
                data.SalesModelYear = model.SalesModelYear;
                data.ColourCode = model.ColourCode;
                data.CreatedBy = userID;
                data.CreatedDate = currentDate;

                ctx.OmTrSalesSOModelColours.Add(data);
            }
            else
            {
                result.message = "Data colour model sudah ada di dalam database.";
                return Json(result);
            }

            data.Remark = model.RemarkColour;
            data.LastUpdateBy = userID;
            data.LastUpdateDate = currentDate;
            data.Quantity = model.Quantity;

            try
            {
                ctx.SaveChanges();
                ctx.Database.ExecuteSqlCommand("exec uspfn_UpdateTotalUnit @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2, @SalesModelCode=@p3, @SalesModelYear=@p4", companyCode, branchCode, model.SONumber, model.SalesModelCode, model.SalesModelYear);

                result.status = true;
                result.message = "Data model colour berhasil disimpan.";
            }
            catch (Exception)
            {
                result.message = "Data model colour gagal dismpan.";
            }

            return Json(result);
        }

        public JsonResult Delete(OmTrSalesSOModelColour model)
        {
            ResultModel result = InitializeResult();
            string companyCode = CompanyCode;
            string branchCode = BranchCode;

            var data = ctx.OmTrSalesSOModelColours.Where(x =>
                        x.CompanyCode == model.CompanyCode
                     && x.BranchCode == model.BranchCode
                     && x.SONo == model.SONo
                     && x.SalesModelCode == model.SalesModelCode
                     && x.SalesModelYear == model.SalesModelYear
                ).FirstOrDefault();

            if (data != null)
            {
                ctx.OmTrSalesSOModelColours.Remove(data);

                try
                {
                    ctx.SaveChanges();
                    ctx.Database.ExecuteSqlCommand("exec uspfn_UpdateTotalUnit @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2, @SalesModelCode=@p3, @SalesModelYear=@p4", companyCode, branchCode, model.SONo, model.SalesModelCode, model.SalesModelYear);

                    result.status = true;
                    result.message = "Data colour model berhasil dihapus.";
                }
                catch (Exception)
                {
                    result.message = "Data colour model gagal dihapus. Mungkin disebabkan masih mempunyai details nomor rangka.";
                }
            }
            else
            {
                result.message = "Tidak bisa menemukan data colour model yang bisa dihapus.";
            }

            return Json(result);
        }

    }
}
