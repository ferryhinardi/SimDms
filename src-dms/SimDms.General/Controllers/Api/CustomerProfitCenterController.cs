using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common;

namespace SimDms.General.Controllers.Api
{
    public class CustomerProfitCenterController : BaseController
    {
        public JsonResult ProfitCenterCode(CustomerProfitCenterModel model)
        {
            var record = ctx.LookUpDtls.Find(CompanyCode, "PFCN", model.ProfitCenterCode);
                           
            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult KelompokAR(CustomerProfitCenterModel model)
        {
            var record = ctx.LookUpDtls.Find(CompanyCode, "GPAR", model.KelAR);

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult CustomerClasses(CustomerProfitCenterModel model)
        {
            var record = ctx.GnMstCustomerClasses.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.CustomerClass == model.CustomerClass && p.ProfitCenterCode == model.ProfitCenterCode).FirstOrDefault();
              
             if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult CustomerProfitCenter(string CustomerCode, string pc)
        {
            var me = ctx.ProfitCenters.Find(CompanyCode, BranchCode, CustomerCode, pc);
            if (me != null)
            {
                var query = string.Format(@"
                select  a.*,b.LookUpValueName ProfitCenterName
                from GnMstCustomerProfitCenter a 
                inner join gnMstLookUpDtl b 
                    on a.ProfitCenterCode = b.LookUpValue 
                WHERE   a.CompanyCode = '{0}'
                        and a.BranchCode = '{1}'
                        and a.CustomerCode= '{2}'
                        and a.ProfitCenterCode = '{3}'
                       ", CompanyCode, BranchCode, CustomerCode, pc);
                var queryable = ctx.Database.SqlQuery<GnMstCustomerProfitCenterView>(query).FirstOrDefault();
                return Json(queryable);
            }
            else {
                return Json(new{data = false});
            }
        }
        public JsonResult Save(CustomerProfitCenterModel model)
        {
            //Validasi belum ada
            var record = ctx.GnMstCustomers.Find(CompanyCode, model.CustomerCode);
            ResultModel result = new ResultModel();
            string userId = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            if (model.DiscPct < 0 || model.DiscPct > 100)
            {
                result.message = "Diskon harus bernilai antara 1 - 100.";
                return Json(result);
            }
            if (model.LaborDiscPct < 0 || model.LaborDiscPct > 100)
            {
                result.message = "Diskon Jasa harus bernilai antara 1 - 100.";
                return Json(result);
            }
            if (model.MaterialDiscPct < 0 || model.MaterialDiscPct > 100)
            {
                result.message = "Diskon Material harus bernilai antara 1 - 100.";
                return Json(result);
            }
            if (model.PartDiscPct < 0 || model.PartDiscPct > 100)
            {
                result.message = "Diskon Part harus bernilai antara 1 - 100.";
                return Json(result);
            }

            var recordProfitCenter = ctx.ProfitCenters.Find(CompanyCode, BranchCode, model.CustomerCode, model.ProfitCenterCode);
            if (recordProfitCenter == null)
            {
                recordProfitCenter = new ProfitCenter();
                recordProfitCenter.CompanyCode = CompanyCode;
                recordProfitCenter.BranchCode = BranchCode;
                recordProfitCenter.CustomerCode = model.CustomerCode;
                recordProfitCenter.ProfitCenterCode = model.ProfitCenterCode;
                recordProfitCenter.CreatedBy = userId;
                recordProfitCenter.CreatedDate = currentTime;

                ctx.ProfitCenters.Add(recordProfitCenter);
            }

            recordProfitCenter.GroupPriceCode = model.GroupPrice;
            recordProfitCenter.SalesCode = model.KelAR;
            recordProfitCenter.Salesman = model.Salesman;
            recordProfitCenter.CustomerClass = model.CustomerClass;
            recordProfitCenter.ContactPerson = model.ContactPerson;
            recordProfitCenter.isOverDueAllowed = model.isOverDueAllowed;
            recordProfitCenter.isBlackList = model.isBlackList;
            recordProfitCenter.DiscPct = model.DiscPct;
            recordProfitCenter.PartDiscPct = model.PartDiscPct;
            recordProfitCenter.MaterialDiscPct = model.MaterialDiscPct;
            recordProfitCenter.LaborDiscPct = model.LaborDiscPct;
            recordProfitCenter.TOPCode = model.TOPCode;
            recordProfitCenter.TaxCode = model.TaxCode;
            recordProfitCenter.TaxTransCode = model.TaxTransCode;
            recordProfitCenter.CollectorCode = model.CollectorCode;
            recordProfitCenter.CreditLimit = model.CreditLimit;
            recordProfitCenter.CustomerGrade = model.CustomerGrade;
            recordProfitCenter.PaymentCode = model.PaymentCode;
            recordProfitCenter.GroupPriceCode = model.GroupPriceCode;
            recordProfitCenter.CustomerGrade = model.CustomerGrade;
            //IsLinkToSales belum dicek
            recordProfitCenter.SalesType = model.SalesType;
            recordProfitCenter.LastUpdateBy = userId;
            recordProfitCenter.LastUpdateDate = currentTime;

            var recordDealerProfitCenter = ctx.GnMstCustomerDealerProfitCenters.Find(CompanyCode, BranchCode, model.CustomerCode, model.ProfitCenterCode);
            if (recordDealerProfitCenter == null)
            {
                recordDealerProfitCenter = new GnMstCustomerDealerProfitCenter();
                recordDealerProfitCenter.CompanyCode = CompanyCode;
                recordDealerProfitCenter.BranchCode = BranchCode;
                recordDealerProfitCenter.CustomerCode = model.CustomerCode;
                recordDealerProfitCenter.ProfitCenterCode = model.ProfitCenterCode;
                recordDealerProfitCenter.CreatedBy = userId;
                recordDealerProfitCenter.CreatedDate = currentTime;

                ctx.GnMstCustomerDealerProfitCenters.Add(recordDealerProfitCenter);
            }

            recordDealerProfitCenter.CustomerClass = model.CustomerClass;
            recordDealerProfitCenter.CustomerClassName = model.CustomerClassName;
            recordDealerProfitCenter.TaxCode = model.TaxCode;
            recordDealerProfitCenter.TaxTransCode = model.TaxTransCode;
            recordDealerProfitCenter.DiscPct = model.DiscPct;
            recordDealerProfitCenter.LaborDiscPct = model.LaborDiscPct;
            recordDealerProfitCenter.MaterialDiscPct = model.MaterialDiscPct;
            recordDealerProfitCenter.PartDiscPct = model.PartDiscPct;
            recordDealerProfitCenter.TOPCode = model.TOPCode;
            recordDealerProfitCenter.TOPDesc = model.TOPDesc;
            recordDealerProfitCenter.PaymentCode = model.PaymentDesc;
            recordDealerProfitCenter.PaymentDesc = model.PaymentDesc;
            recordDealerProfitCenter.CreditLimit = model.CreditLimit;
            recordDealerProfitCenter.CustomerGrade = model.CustomerGrade;
            recordDealerProfitCenter.ContactPerson = model.ContactPerson;
            recordDealerProfitCenter.CollectorCode = model.CollectorCode;
            recordDealerProfitCenter.SalesType = model.SalesType;
            recordDealerProfitCenter.SalesCode = model.KelAR;
            recordDealerProfitCenter.isOverDueAllowed = model.isOverDueAllowed;
            recordDealerProfitCenter.isBlackList = model.isBlackList;
            recordDealerProfitCenter.Salesman = model.Salesman;
            recordDealerProfitCenter.DCSFlag = "0";
            recordDealerProfitCenter.DCSDate = new DateTime(1900, 1, 1);
            recordDealerProfitCenter.LastUpdateBy = userId;
            recordDealerProfitCenter.LastUpdateDate = currentTime;
            recordDealerProfitCenter.GroupPriceCode = model.GroupPriceCode;
            try
            {
                Helpers.ReplaceNullable(recordProfitCenter);
                Helpers.ReplaceNullable(recordDealerProfitCenter);
                ctx.SaveChanges();
                result.data = _GnMstCustomerProfitCenters(model.CustomerCode, record.CustomerGovName);
                result.status = true;
                result.message = "Data profit center berhasil disimpan.";
            }
            catch (Exception)
            {
                result.message = "Data profit center tidak dapat disimpan.";
            }

            return Json(result);
        }

        public JsonResult Delete(CustomerProfitCenterModel model)
        {
            var record = ctx.GnMstCustomers.Find(CompanyCode, model.CustomerCode);
            ResultModel result = InitializeResultModel();

            var customerProfitCenter = ctx.ProfitCenters.Find(CompanyCode, BranchCode, model.CustomerCode, model.ProfitCenterCode);
            var dealerProfitCenter = ctx.GnMstCustomerDealerProfitCenters.Find(CompanyCode, BranchCode, model.CustomerCode, model.ProfitCenterCode);

            if (customerProfitCenter == null)
            {
                result.message = "Tidak dapat menemukan data profit center customer yang akan dihapus.";
                return Json(result);
            }

            if (dealerProfitCenter == null)
            {
                result.message = "Tidak dapat menemukan data profit center dealer yang akan dihapus.";
                return Json(result);
            }

            ctx.ProfitCenters.Remove(customerProfitCenter);
            ctx.GnMstCustomerDealerProfitCenters.Remove(dealerProfitCenter);

            try
            {
                ctx.SaveChanges();
                result.data = _GnMstCustomerProfitCenters(model.CustomerCode, record.CustomerGovName);
                result.message = "Data profit center berhasil dihapus.";
                result.status = true;

            }
            catch (Exception)
            {
                result.message = "Gagal menhapus data profit center.";
            }

            return Json(result);
        }

        [HttpGet]
        public IQueryable<GnMstCustomerProfitCenterView> _GnMstCustomerProfitCenters(string CustomerCode, string CustomerGovName)
        {

            var queryable = ctx.Database.SqlQuery<GnMstCustomerProfitCenterView>("select  a.*,b.LookUpValueName ProfitCenterName,'" + CustomerGovName + "' as CustomerGovName from GnMstCustomerProfitCenter a inner join gnMstLookUpDtl b on a.ProfitCenterCode=b.LookUpValue where a.CompanyCode='" + CompanyCode + "' and a.BranchCode='" + BranchCode + "' and a.CustomerCode='" + CustomerCode + "' and  b.CodeID='PFCN'").AsQueryable();
            return (queryable);

        }

    }
}
