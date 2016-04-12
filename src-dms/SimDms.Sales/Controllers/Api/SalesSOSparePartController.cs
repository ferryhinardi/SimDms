using SimDms.Sales.Models;
using SimDms.Sales.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers.Api
{
    public class SalesSOSparePartController : BaseController
    {
        public JsonResult Save(SparePartForm model)
        {
            ResultModel result = InitializeResult();
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string userId = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            if (string.IsNullOrWhiteSpace(model.PartNo))
            {
                result.message = "Part No. tidak boleh kosong.";
                return Json(result);
            }

            if (string.IsNullOrWhiteSpace(model.SONumber))
            {
                result.message = "No.SO tidak boleh kosong.";
                return Json(result);
            }

            if (model.SparePartQtyPart == null || model.SparePartQtyPart <= 0)
            {
                result.message = "Jumlah part tidak boleh 0.";
                return Json(result);
            }

            if (model.SparePartQtyUnit == null || model.SparePartQtyUnit <= 0)
            {
                result.message = "Jumlah part per-unit tidak boleh 0.";
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

            //OmTrSalesSOAccsSeq omTrSalesSOAccSeq = ctx.OmTrSalesSOAccsSeqs.Find(companyCode, branchCode, model.SONumber, model.PartNo);
            OmTrSalesSOAccsSeq omTrSalesSOAccSeq = ctx.OmTrSalesSOAccsSeqs.Where(x =>
                    x.CompanyCode == companyCode
                    &&
                    x.BranchCode == branchCode
                    &&
                    x.SONo == model.SONumber
                    &&
                    x.PartNo == model.PartNo
                ).FirstOrDefault();
            if (omTrSalesSOAccSeq == null)
            {
                omTrSalesSOAccSeq = new OmTrSalesSOAccsSeq();
                omTrSalesSOAccSeq.CompanyCode = companyCode;
                omTrSalesSOAccSeq.BranchCode = branchCode;
                omTrSalesSOAccSeq.SONo = model.SONumber;
                omTrSalesSOAccSeq.PartNo = model.PartNo;
                omTrSalesSOAccSeq.PartSeq = GetPartSeq(companyCode, branchCode, model.SONumber);

                omTrSalesSOAccSeq.Qty = model.SparePartQtyPart;
                omTrSalesSOAccSeq.DemandQty = model.SparePartQtyPart * model.SparePartQtyUnit;
                omTrSalesSOAccSeq.ReturnQty = 0;
                omTrSalesSOAccSeq.SupplyQty = 0;

                SpMstItemPrice spMstItemPrice = ctx.SpMstItemPrices.Find(companyCode, branchCode, model.PartNo);
                if (spMstItemPrice != null)
                {
                    spMstItemPrice.CostPrice = spMstItemPrice.CostPrice;
                    spMstItemPrice.RetailPrice = spMstItemPrice.RetailPrice;
                }
                else
                {
                    spMstItemPrice.CostPrice = 0;
                    spMstItemPrice.RetailPrice = 0;
                }

                decimal? aftDiscTotal = 0;
                decimal? befDiscTotal = 0;
                decimal? discExcPPn = 0;
                decimal? aftDiscDPP = 0;
                decimal? aftDiscPPn = 0;
                decimal? ppnPct = ctx.Database.SqlQuery<decimal?>("exec uspfn_SalesSOTax @CompanyCode=@p0, @CustomerCode=@p1", companyCode, model.CustomerCode).FirstOrDefault();
                if (ppnPct == null || ppnPct == 0)
                {
                    ppnPct = 0;
                }

                aftDiscTotal = model.SparePartTotalAfterDisc;
                befDiscTotal = model.SparePartTotalBeforeDisc;
                aftDiscDPP = Math.Round(Convert.ToDecimal(aftDiscTotal / ((100 + ppnPct) / 100)), MidpointRounding.AwayFromZero);
                discExcPPn = befDiscTotal - aftDiscDPP;
                if (discExcPPn < 0)
                {
                    discExcPPn = 0;
                    befDiscTotal = aftDiscDPP;
                }
                aftDiscPPn = aftDiscTotal - aftDiscDPP;

                omTrSalesSOAccSeq.RetailPrice = model.SparePartTotalBeforeDisc;
                omTrSalesSOAccSeq.DiscExcludePPn = discExcPPn;
                omTrSalesSOAccSeq.AfterDiscDPP = aftDiscDPP;
                omTrSalesSOAccSeq.AfterDiscPPn = aftDiscPPn;
                omTrSalesSOAccSeq.AfterDiscTotal = model.SparePartTotalAfterDisc;

                SpMstItem spMstItem = ctx.SpMstItems.Find(companyCode, branchCode, model.PartNo);
                if (spMstItem != null)
                {
                    omTrSalesSOAccSeq.TypeOfGoods = spMstItem.TypeOfGoods;
                }

                omTrSalesSOAccSeq.isSubstitution = false;

                ctx.OmTrSalesSOAccsSeqs.Add(omTrSalesSOAccSeq);
            }
            else
            {
                result.message = "Data spare part sudah ada, silakan hapus terlebih dahulu, lalu tambahkan lagi data baru untuk mengubah data.";
                return Json(result);
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "";
                result.data = omTrSalesSOAccSeq;
            }
            catch (Exception)
            {
                result.message = "Data sparepart tidak bisa disimpan";
            }

            return Json(result);
        }

        public JsonResult Delete(OmTrSalesSOAccsSeqList model)
        {
            ResultModel result = InitializeResult();
            var data = ctx.OmTrSalesSOAccsSeqs.Find(CompanyCode, BranchCode, model.SONumber, model.PartNo, model.PartSeq);

            if (data != null)
            {
                ctx.OmTrSalesSOAccsSeqs.Remove(data);
            }
            else
            {
                result.message = "Tidak dapat menemukan data sparepart yang akan dihapus.";
                return Json(result);
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Data spareparts berhasil dihapus.";
            }
            catch (Exception)
            {
                result.message = "";
            }

            return Json(result);
        }

        private decimal GetPartSeq(string companyCode, string branchCode, string soNumber)
        {
            decimal seqNo = 0;

            var data = ctx.OmTrSalesSOAccsSeqs.Where(x => 
                    x.CompanyCode == companyCode
                    &&
                    x.BranchCode == branchCode
                    &&
                    x.SONo == soNumber
                ).OrderByDescending(x => x.PartSeq).FirstOrDefault();

            if (data == null)
            {
                seqNo = 1;
            }
            else
            {
                seqNo = data.PartSeq + 1;
            }

            return seqNo;
        }
    }
}
