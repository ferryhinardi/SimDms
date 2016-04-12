using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
//using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;
using SimDms.Sparepart.Models;


namespace SimDms.General.Controllers.Api
{
    public class SupplierClassController : BaseController
    {          
        [HttpPost]
        public JsonResult Save(GnMstSupplierClass model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var Supplierclass = ctx.GnMstSupplierClasses.Find(companyCode, branchCode, model.SupplierClass, model.TypeOfGoods);
 
            if (Supplierclass == null)
            {
                Supplierclass = new GnMstSupplierClass();
                Supplierclass.CreatedDate = currentTime;
                Supplierclass.LastUpdateDate = currentTime;
                Supplierclass.CreatedBy = userID;
                ctx.GnMstSupplierClasses.Add(Supplierclass);
            }
                else{
                    Supplierclass.LastUpdateDate = currentTime;
                    Supplierclass.LastUpdateBy = userID;
            }               
                Supplierclass.SupplierClass = model.SupplierClass;
                Supplierclass.SupplierClassName = model.SupplierClassName;
                Supplierclass.CompanyCode = companyCode;
                Supplierclass.BranchCode = branchCode;
                Supplierclass.TypeOfGoods = model.TypeOfGoods;
                Supplierclass.ProfitCenterCode = model.ProfitCenterCode;
                Supplierclass.PayableAccNo = model.PayableAccNo;
                Supplierclass.OtherAccNo = model.OtherAccNo;
                Supplierclass.DownPaymentAccNo = model.DownPaymentAccNo;
                Supplierclass.InterestAccNo = model.InterestAccNo;
                Supplierclass.TaxInAccNo = model.TaxInAccNo;
                Supplierclass.WitholdingTaxAccNo = model.WitholdingTaxAccNo;
                
                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data Supplier Class berhasil disimpan.";
                    result.data = new
                    {
                        SupplierClass = Supplierclass.SupplierClass,
                        SupplierClassName = Supplierclass.SupplierClass
                    };
                }
                catch (Exception Ex)
                {
                    result.message = "Data Supplier tidak bisa disimpan.";
                    MyLogger.Info("Error on Supplier saving: " + Ex.Message);
                }
            
            return Json(result);
        }

        public JsonResult Delete(GnMstSupplierClass model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var Supplierclass = ctx.GnMstSupplierClasses.Find(companyCode, branchCode, model.SupplierClass, model.TypeOfGoods);
                    if (Supplierclass != null)
                    {
                        ctx.GnMstSupplierClasses.Remove(Supplierclass);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Supplier class berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Supplier class , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Supplier class , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
    }
}
