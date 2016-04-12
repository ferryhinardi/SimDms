using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;

namespace SimDms.General.Controllers.Api
{
    public class CustomerDiscountController : BaseController
    {
        [HttpPost]
        public JsonResult Save(CustomerDiscountModel model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            if (string.IsNullOrEmpty(model.CustomerCode))
            {
                result.message = "Anda belum memilih customer untuk ditambahkan data diskon";
                return Json(result);
            }

            if (string.IsNullOrEmpty(model.ProfitCenterCode))
            {
                result.message = "Anda belum mengisi kode profit center.";
                return Json(result);
            }

            if (string.IsNullOrEmpty(model.TypeOfGoods))
            {
                result.message = "Anda belum mengisi tipe part.";
                return Json(result);
            }

            var data = ctx.GnMstCustomerDiscs.Find(companyCode, branchCode, model.CustomerCode, model.ProfitCenterCode, model.TypeOfGoods);
            if (data == null)
            {
                data = new GnMstCustomerDisc();
                data.CompanyCode = companyCode;
                data.BranchCode = branchCode;
                data.CustomerCode = model.CustomerCode;
                data.ProfitCenterCode = model.ProfitCenterCode;
                data.TypeOfGoods = model.TypeOfGoods;

                data.CreatedBy = userID;
                data.CreatedDate = currentTime;

                ctx.GnMstCustomerDiscs.Add(data);
            }

            data.DiscPct = (model.DiscPct==null?0:model.DiscPct.Value);

            try
            {
                ctx.SaveChanges();
                result.data = _GnMstCustomerDiscount(model.CustomerCode);
                result.message = "Data diskon berhasil disimpan.";
                result.status = true;
            }
            catch (Exception)
            {
                result.message = "Data diskon tidak dapat disimpan.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(CustomerDiscountModel model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var data = ctx.GnMstCustomerDiscs.Find(companyCode, branchCode, model.CustomerCode, model.ProfitCenterCode, model.TypeOfGoods);
            if (data == null)
            {
                result.message = "Data diskon customer tidak dapat ditemukan.";
                return Json(result);
            }

            ctx.GnMstCustomerDiscs.Remove(data);

            try
            {
                ctx.SaveChanges();
                result.data = _GnMstCustomerDiscount(model.CustomerCode);
                result.message = "Data diskon berhasil dihapus.";
                result.status = true;
            }
            catch (Exception)
            {
                result.message = "Data diskon tidak dapat dihapus.";
            }

            return Json(result);
        }

        [HttpGet]
        public IQueryable<GnMstCustomerDiscView> _GnMstCustomerDiscount(string CustomerCode) 
        {
            var query = string.Format(@"
                        select  a.*,b.LookUpValueName ProfitCenterName, c.LookUpValueName TypeOfGoodsName
                        from gnMstCustomerDisc a 
                        inner join gnMstLookUpDtl b 
                            on a.ProfitCenterCode=b.LookUpValue
                        inner join gnMstLookUpDtl c 
                            on a.TypeOfGoods=c.LookUpValue 
                        where a.CompanyCode='{0}' 
                        and a.BranchCode='{1}' 
                        and a.CustomerCode='{2}' 
                        and  b.CodeID='PFCN'
                        and  c.CodeID='TPGO'", CompanyCode, BranchCode, CustomerCode);
           var queryable = ctx.Database.SqlQuery<GnMstCustomerDiscView>(query).AsQueryable();
           return (queryable);
        }
    }
}
