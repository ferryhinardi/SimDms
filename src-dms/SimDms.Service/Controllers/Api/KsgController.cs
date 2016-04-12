using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;  
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class KsgController : BaseController
    {
        //
        // GET: /Ksg/

        public JsonResult Default()
        {
            return Json(new
            {

                CompanyCode = CompanyCode,
                EffectiveDate = DateTime.Now,
                ProductType = ProductType,
                PdiFscSeq = 0,
                LaborRate = 0,
                RegularLaborAmount = 0,
                RegularMaterialAmount = 0,
                RegularTotalAmount = 0,
            });
        }

        public JsonResult BasicModel(SvBasicKsgView model)
        {
            var record = ctx.SvBasicKsgViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.BasicModel == model.BasicModel).FirstOrDefault();

            return Json(new { success = record != null ? true : false, data = record });
        }

        public JsonResult Save(PdiFscRate model)
        {
            var record = ctx.PdiFscRates.Find(CompanyCode, ProductType, model.BasicModel, model.IsCampaign, model.TransmissionType, model.PdiFscSeq, model.EffectiveDate);

            if (record == null)
            {
                record = new PdiFscRate
                {
                    CompanyCode = CompanyCode,
                    ProductType = ProductType,
                    BasicModel = model.BasicModel,
                    IsCampaign = model.IsCampaign,
                    TransmissionType = model.TransmissionType,
                    PdiFscSeq = model.PdiFscSeq,
                    EffectiveDate = model.EffectiveDate,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                };
                ctx.PdiFscRates.Add(record);
            }

            record.Description = model.Description;
            record.RegularLaborAmount = model.RegularLaborAmount;
            record.RegularMaterialAmount = model.RegularMaterialAmount;
            record.RegularTotalAmount = model.RegularTotalAmount;
            record.IsActive = model.IsActive;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;
            record.LaborRate = model.LaborRate;

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

        public JsonResult deleteData(PdiFscRate model)
        {

            var record = ctx.PdiFscRates.Find(CompanyCode, ProductType, model.BasicModel, model.IsCampaign, model.TransmissionType, model.PdiFscSeq, model.EffectiveDate);
            if (record != null)
            {
                ctx.PdiFscRates.Remove(record);
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

    }
}