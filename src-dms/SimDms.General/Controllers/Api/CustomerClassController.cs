using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;


namespace SimDms.General.Controllers.Api
{
    public class CustomerClassController : BaseController 
    {          
        [HttpPost]
        public JsonResult Save(GnMstCustomerClass model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var customerclass = ctx.GnMstCustomerClasses.Find(companyCode, branchCode, model.CustomerClass, model.TypeOfGoods);
 
            if (customerclass == null)
            {
                customerclass = new GnMstCustomerClass();
                customerclass.CreatedDate = currentTime;
                customerclass.LastUpdateDate = currentTime;
                customerclass.CreatedBy = userID;
                ctx.GnMstCustomerClasses.Add(customerclass);
            }
                else{
                    customerclass.LastUpdateDate = currentTime;
                    customerclass.LastUpdateBy = userID;
            }               
                customerclass.CustomerClass = model.CustomerClass;
                customerclass.CustomerClassName = model.CustomerClassName;
                customerclass.CompanyCode = companyCode;
                customerclass.BranchCode = branchCode;
                customerclass.TypeOfGoods = model.TypeOfGoods;
                customerclass.ProfitCenterCode = model.ProfitCenterCode;
                customerclass.ReceivableAccNo = model.ReceivableAccNo;
                customerclass.DownPaymentAccNo = model.DownPaymentAccNo;
                customerclass.InterestAccNo = model.InterestAccNo;
                customerclass.TaxOutAccNo = model.TaxOutAccNo;
                customerclass.LuxuryTaxAccNo = model.LuxuryTaxAccNo;

                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data Customer Class berhasil disimpan.";
                    result.data = new
                    {
                        CustomerClass = customerclass.CustomerClass,
                        CustomerClassName = customerclass.CustomerClass
                    };
                }
                catch (Exception Ex)
                {
                    result.message = "Data customer tidak bisa disimpan.";
                    MyLogger.Info("Error on customer saving: " + Ex.Message);
                }
            
            return Json(result);
        }

        public JsonResult Delete(GnMstCustomerClass model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var customerclass = ctx.GnMstCustomerClasses.Find(companyCode, branchCode, model.CustomerClass, model.TypeOfGoods);
                    if (customerclass != null)
                    {
                        ctx.GnMstCustomerClasses.Remove(customerclass);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data customer class berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete customer class , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete customer class , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

    }
}
