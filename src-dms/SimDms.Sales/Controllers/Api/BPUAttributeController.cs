using SimDms.Sales.BLL;
using SimDms.Sales.Models;
//using SimDms.Sales.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using System.Transactions;

namespace SimDms.Sales.Controllers.Api
{
    public class BPUAttributeController : BaseController
    {
        public JsonResult BPUView(string BPUNo)  
        {
            var titleName = ctx.omTrPurchaseBPU.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.BPUNo == BPUNo).FirstOrDefault();
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    TitleName = titleName.RefferenceDONo
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = true,
                    TitleName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Warehouse(string WarehouseCode) 
        {
            var titleName = ctx.LookUpDtls.Where(a => a.CompanyCode == CompanyCode && a.CodeID == "MPWH" && a.LookUpValue == WarehouseCode).FirstOrDefault();
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    TitleName = titleName.LookUpValueName
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = true,
                    TitleName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(OmTrPurchaseBPUAttribute model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.omTrPurchaseBPUAttributes.Find(companyCode, BranchCode, model.BPUNo, model.DONo);

            if (me == null)
            {
                me = new OmTrPurchaseBPUAttribute();
                me.CreateDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreateBy = userID;
                ctx.omTrPurchaseBPUAttributes.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.BPUNo = model.BPUNo;
            me.DONo = model.DONo;
            me.Status = model.Status;
            me.FakturPolisiNo = model.FakturPolisiNo;
            me.FakturPolisiDate = model.FakturPolisiDate;
            me.WareHouseCode = model.WareHouseCode;
            me.Subsidi1 = model.Subsidi1;
            me.Subsidi2 = model.Subsidi2;
            me.Subsidi3 = model.Subsidi3;
            me.Subsidi4 = model.Subsidi4;
            me.PotSKP = model.PotSKP;
            me.Kompensasi = model.Kompensasi;
            me.HargaAccessories = model.HargaAccessories;
            me.AttributeChar1 = model.AttributeChar1;
            me.AttributeNum1 = model.AttributeNum1;
            me.AttributeDate1 = model.AttributeDate1;
            me.AttributeChar2 = model.AttributeChar2;
            me.AttributeNum2 = model.AttributeNum2;
            me.AttributeDate2 = model.AttributeDate2;
            me.AttributeChar3 = model.AttributeChar3;
            me.AttributeNum3 = model.AttributeNum3;
            me.AttributeDate3 = model.AttributeDate3;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = me });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete(OmTrPurchaseBPUAttribute model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.omTrPurchaseBPUAttributes.Find(companyCode, BranchCode, model.BPUNo, model.DONo);
                    if (me != null)
                    {
                        ctx.omTrPurchaseBPUAttributes.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data BPU Attribute berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete BPU Attribute, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete BPU Attribute, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
    }
}
