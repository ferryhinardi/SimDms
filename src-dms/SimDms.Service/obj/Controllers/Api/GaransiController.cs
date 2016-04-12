using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;

namespace SimDms.Service.Controllers.Api
{
    public class GaransiController : BaseController
    {
        //
        // GET: /Garansi/

        public JsonResult Default()
        {
            return Json(new
            {

                CompanyCode = CompanyCode,
                EffectiveDate = DateTime.Now,
                Odometer = "0.00",
                TimePeriod = "0.00",
                ProductType = ProductType,
                //CompanyName = CompanyName,
                //BranchCode = BranchCode,
                //BranchName = BranchName,
                //ServiceType = 2,
                //JobOrderDate = DateTime.Now,
                //StartService = DateTime.Now,
                //FinishService = DateTime.Now
            });
        }

        public JsonResult Save(SvMstWarranty model)
        {
            var record = ctx.SvMstWarranties.Find(CompanyCode, ProductType, model.BasicModel, model.OperationNo);

            if (record == null)
            {
                record = new SvMstWarranty
                {
                    CompanyCode = CompanyCode,
                    ProductType = ProductType,
                    BasicModel = model.BasicModel,
                    OperationNo = model.OperationNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                };
                ctx.SvMstWarranties.Add(record);
            }

            record.Description = model.Description;
            record.Odometer = model.Odometer;
            record.TimePeriod = model.TimePeriod.Value;
            record.TimeDim = model.TimeDim;
            record.EffectiveDate = model.EffectiveDate;
            record.IsActive = model.IsActive;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = "";
            record.LockingDate = new DateTime(1900,1,1);

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult deleteData(SvMstWarranty model)
        {

            var record = ctx.SvMstWarranties.Find(CompanyCode, ProductType, model.BasicModel, model.OperationNo);
            if (record != null)
            {
                ctx.SvMstWarranties.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }


        }

        public JsonResult BasicModelBrowse()
        {
            var queryable = ctx.svMstRefferenceServices.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.RefferenceType == "BASMODEL")
                .Select(p => new
                {
                    BasicModel = p.RefferenceCode, 
                    TechnicalModelCode = p.DescriptionEng,
                    ModelDescription = p.Description, 
                    Status = p.IsActive ? "Aktif" : "Tidak Aktif"
                }).OrderBy(p => p.BasicModel);

            return Json(queryable.AsQueryable().toKG());
        }

        public JsonResult GetBasicModel(SvMstWarranty model)
        {
            var record = ctx.svMstRefferenceServices.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType && p.RefferenceType == "BASMODEL" && p.RefferenceCode == model.BasicModel).FirstOrDefault();

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult OperationNoBrowse(string basicModel)
        {
            var queryable = ctx.svMstTasks.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.BasicModel == basicModel)
                .Select(p => new
                {
                    p.OperationNo,
                    p.Description,
                    p.TechnicalModelCode,
                    IsSubCon = p.IsSubCon.Value ? "Ya" : "Tidak",
                    IsCampaign = p.IsCampaign ? "Ya" : "Tidak",
                    Status = p.IsActive ? "Aktif" : "Tidak Aktif",
                    p.OperationHour,
                    p.ClaimHour
                }).OrderBy(p => p.OperationNo);

            return Json(queryable.AsQueryable().toKG());
        }

        public JsonResult GetOperationNo(SvMstWarranty model)
        {
            var record = ctx.svMstTasks.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType && p.BasicModel == model.BasicModel && p.OperationNo == model.OperationNo).FirstOrDefault();

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult Get(SvMstWarranty model)
        {
            var record = ctx.SvMstWarranties.Find(CompanyCode, ProductType, model.BasicModel, model.OperationNo);

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }
    }
}
