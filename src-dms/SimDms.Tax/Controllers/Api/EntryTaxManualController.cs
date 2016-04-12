using SimDms.Common;
using SimDms.Tax.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Tax.Controllers.Api
{
    public class EntryTaxManualController : BaseController
    {
        public JsonResult Default()
        {
            var TaxInCode = ctx.LookUpDtls.Where(a => a.CompanyCode == CompanyCode && a.CodeID.Equals(GnMstLookUpHdr.TaxCodeIN))
                .OrderBy(a => a.SeqNo).Select(a => new ConsolidationLookup { LookUpValue = a.LookUpValue, LookUpValueName = a.LookUpValueName });

            var ProductDesc = ctx.LookUpDtls.Find(CompanyCode,GnMstLookUpHdr.ProductType, ProductType).LookUpValueName;

            return Json(new
            {
                CompanyCode = CompanyCode,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                TaxCode = TaxInCode.FirstOrDefault().LookUpValue,
                TaxDesc = TaxInCode.FirstOrDefault().LookUpValueName,
                ProductType = ProductType,
                ProductDesc = ProductDesc
            });
        }

        public JsonResult ValidateSave(GetTaxIn model)
        {
            var IsExistSuppAndTaxNo = ctx.gnTaxIn.Where(a => a.CompanyCode == model.CompanyCode && a.BranchCode == model.BranchCode && a.ProductType == model.ProductType 
                && a.SupplierCode == model.SupplierCode && a.TaxNo == model.TaxNo) != null;

            var msgCon = "Sudah ada data dengan Supplier " + model.SupplierCode + " dan No Seri Pajak " + model.TaxNo + " Apakah anda ingin melanjutkan?";

            var IsExistRecord = ctx.gnTaxIn.Where(a => a.CompanyCode == model.CompanyCode && a.BranchCode == model.BranchCode && a.ProductType == model.ProductType
                && a.PeriodYear == model.PeriodYear && a.PeriodMonth == model.PeriodMonth && a.SupplierCode == model.SupplierCode && a.TaxNo == model.TaxNo) != null;

            var msgExist = "Data sudah tersimpan di database";

            return Json(new
            {
                success = IsExistSuppAndTaxNo,
                msgCon = msgCon,
                isExsistRecord = IsExistRecord,
                msgExist = msgExist
            });
        }

        public JsonResult Save(GetTaxIn model)
        {
            var result = false;
            var msg = "";

            var seqNoTaxIn = ctx.gnTaxIn.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
                     && a.ProductType == ProductType && a.PeriodYear == model.PeriodYear && a.PeriodMonth == model.PeriodMonth).Max(a => a.SeqNo) ?? 0;

            var seqNoTaxInHst = ctx.GnTaxInHistories.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
                 && a.ProductType == ProductType && a.PeriodYear == model.PeriodYear && a.PeriodMonth == model.PeriodMonth && a.IsDeleted == true).Max(a => a.SeqNo) ?? 0;

            var lastSeqNo = (seqNoTaxIn > seqNoTaxInHst) ? seqNoTaxIn : seqNoTaxInHst;

            var record = new gnTaxIn()
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                ProductType = ProductType,
                PeriodYear = model.PeriodYear,
                PeriodMonth = model.PeriodMonth,
                SeqNo = lastSeqNo + 1,
                ProfitCenter = model.ProfitCenter,
                TypeOfGoods = model.TypeOfGoods,
                TaxCode = model.TaxCode,
                TransactionCode = model.TransactionCode,
                StatusCode = model.StatusCode,
                DocumentCode = model.DocumentCode,
                DocumentType = model.DocumentType,
                SupplierCode = model.SupplierCode,
                SupplierName = model.SupplierName,
                IsPKP = model.IsPKP,
                NPWP = model.NPWP,
                FPJNo = model.FPJNo,
                FPJDate = model.FPJDate,
                ReferenceNo = model.ReferenceNo,
                ReferenceDate = model.ReferenceDate,
                TaxNo = model.TaxNo,
                TaxDate = model.TaxDate,
                SubmissionDate = model.SubmissionDate,
                DPPAmt = model.DPPAmt,
                PPNAmt = model.PPNAmt,
                PPNBmAmt = model.PPNBmAmt,
                Description = model.Description,
                Quantity = model.Quantity,
                CreatedBy = CurrentUser.UserId,
                CreatedDate = DateTime.Now,
                LastupdateBy = CurrentUser.UserId,
                LastupdateDate = DateTime.Now
            };

            ctx.gnTaxIn.Add(record);

            result = ctx.SaveChanges() > 0;


            msg = result ? string.Empty : "Gagal simpan data Pajak Masukan";

            if (result)
            {
                DateTime date = new DateTime(Convert.ToInt32(model.PeriodYear), Convert.ToInt32(model.PeriodMonth), 1);
                string taxType = (record.DocumentType == "F") ? "3" : "4";

                var cek = ctx.GnTaxPPns.Find(CompanyCode, BranchCode, ProductType, model.PeriodYear, model.PeriodMonth, model.ProfitCenter, taxType) != null;
                if(cek)
                {
                    result = RecalculatePPN(model.CompanyCode, model.BranchCode, model.ProductType, (int)model.PeriodYear, (int)model.PeriodMonth, model.ProfitCenter, model.DocumentType);
                }
                else
                {
                    object[] PPnParams = { CompanyCode, BranchCode, ProductType, CurrentUser.UserId, date };
                    var PPnQuery = IsFPJCentral() ? "exec usprpt_GnInsertTaxPPNWoBranch @p0,@p1,@p2,@p3,@p4" : "exec usprpt_GnInsertTaxPPN @p0,@p1,@p2,@p3,@p4";
                    try
                    {
                        result = ctx.Database.ExecuteSqlCommand(PPnQuery, PPnParams) >= 0 ? true : false;
                    }
                    catch
                    {
                        result = false;
                    }

                }

                if (result)
                {
                    result = UpdateTotal(model.CompanyCode, (IsFPJCentral() ? "" : model.BranchCode), model.ProductType, (int)model.PeriodYear, (int)model.PeriodMonth);
                    if (!result) msg = "Perhitungan GrandTotal gagal disimpan";
                }
                else
                    msg = "Perhitungan PPN gagal disimpan";
            }     

            return Json(new { success = result, message = msg});
        }

        private bool IsFPJCentral()
        {
            return ctx.CoProfiles.Where(c => c.CompanyCode == CompanyCode)
                .Join(ctx.OrganizationDtls.Where(d => d.IsBranch == false), x => new { x.CompanyCode, x.BranchCode }, y => new { y.CompanyCode, y.BranchCode }, (x, y) => new { x, y }).Select(a => a.x.IsFPJCentralized.Value).FirstOrDefault();
        }

        private bool RecalculatePPN(string companyCode, string branchCode, string productType, int year, int month, string profitCenter, string docType)
        {
            var result = false;
            object[] recPPnParams = { companyCode, branchCode, productType, year, month, profitCenter, docType };
            var recPPnQuery = "exec uspfn_TaxRecalculatePPN @p0,@p1,@p2,@p3,@p4,@p5,@p6";
            result = ctx.Database.ExecuteSqlCommand(recPPnQuery, recPPnParams) >= 0 ? true : false;

            return result;
        }

        private bool UpdateTotal(string companyCode, string branchCode, string productType, int year, int month)
        {
            var result = false;
            DateTime date = new DateTime(year, month, 1);
            object[] updateParams = { companyCode, (IsFPJCentral() ? "" : branchCode), productType, date };
            var updateQuery = "exec uspfn_TaxUpdateTotal @p0,@p1,@p2,@p3";
            result = ctx.Database.ExecuteSqlCommand(updateQuery, updateParams) >= 0 ? true : false;
            return result;
        }
    }
}