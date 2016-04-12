using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;

namespace SimDms.Service.Controllers.Api
{
    public class CampaignController : BaseController
    {
        //
        // GET: /Campaign/

        public JsonResult Default()
        {
            return Json(new
            {

                CompanyCode = CompanyCode,
                CloseDate = DateTime.Now,
                ProductType = ProductType,
            });
        }

        public JsonResult Save(SvMstCampaign model)
        {
            var record = ctx.SvMstCampaigns.Find(CompanyCode, ProductType, model.ComplainCode, model.DefectCode, model.ChassisCode, model.ChassisStartNo, model.ChassisEndNo);

            if (record == null)
            {
                record = new SvMstCampaign
                {
                    CompanyCode = CompanyCode,
                    ProductType = ProductType,
                    ComplainCode = model.ComplainCode,
                    DefectCode = model.DefectCode,
                    ChassisCode = model.ChassisCode.ToUpper(),
                    ChassisStartNo = model.ChassisStartNo,
                    ChassisEndNo = model.ChassisEndNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                };
                ctx.SvMstCampaigns.Add(record);
            }
            
            record.OperationNo = model.OperationNo;
            record.CloseDate = model.CloseDate;
            record.Description = model.Description.ToUpper();
            record.IsActive = model.IsActive;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;

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

        public JsonResult deleteData(SvMstCampaign model)
        {
            var record = ctx.SvMstCampaigns.Find(CompanyCode, ProductType, model.ComplainCode, model.DefectCode, model.ChassisCode, model.ChassisStartNo, model.ChassisEndNo);
            if (record != null)
            {
                ctx.SvMstCampaigns.Remove(record);
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

        public JsonResult ReffService(string reffType)
        {
            var record = ctx.svMstRefferenceServices.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType && p.RefferenceType == reffType)
                .Select(p => new
                {
                    p.RefferenceType,
                    p.RefferenceCode, 
                    p.Description, 
                    p.DescriptionEng,
                    IsActive = p.IsActive ? "Aktif" : "Tidak Aktif"
                }).OrderBy(p => p.RefferenceCode);

            return Json(record.AsQueryable().toKG());
        }

        public JsonResult GetComplainCode()
        {
            var record = ctx.SvMstCampaigns.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType)
                .Select(p => new
                {
                    p.ComplainCode,
                    DescriptionComplain = (ctx.svMstRefferenceServices.Where(p1 => p1.CompanyCode == CompanyCode && p1.ProductType == ProductType && p1.RefferenceType == "COMPLNCD")).FirstOrDefault().Description,
                    DescriptionEngComplain = (ctx.svMstRefferenceServices.Where(p1 => p1.CompanyCode == CompanyCode && p1.ProductType == ProductType && p1.RefferenceType == "COMPLNCD")).FirstOrDefault().DescriptionEng,
                }).Distinct();

            return Json(record.AsQueryable().toKG());
        }

        public JsonResult ComplainCode()
        {
            var record = ctx.SvMstCampaigns.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType)
                .Select(p => new
                {
                    p.ComplainCode,
                    DescriptionComplain = (ctx.svMstRefferenceServices.Where(p1 => p1.CompanyCode == CompanyCode && p1.ProductType == ProductType && p1.RefferenceType == "COMPLNCD")).FirstOrDefault().Description,
                    DescriptionEngComplain = (ctx.svMstRefferenceServices.Where(p1 => p1.CompanyCode == CompanyCode && p1.ProductType == ProductType && p1.RefferenceType == "COMPLNCD")).FirstOrDefault().DescriptionEng,
                }).Distinct();

            return Json(record.AsQueryable().toKG());
        }

        public JsonResult DefectCode()
        {
            var record = ctx.SvMstCampaigns.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType)
                .Select(p => new
                {
                    p.DefectCode,
                    DescriptionComplain = (ctx.svMstRefferenceServices.Where(p1 => p1.CompanyCode == CompanyCode && p1.ProductType == ProductType && p1.RefferenceType == "DEFECTCD")).FirstOrDefault().Description,
                    DescriptionEngComplain = (ctx.svMstRefferenceServices.Where(p1 => p1.CompanyCode == CompanyCode && p1.ProductType == ProductType && p1.RefferenceType == "DEFECTCD")).FirstOrDefault().DescriptionEng,
                }).Distinct();

            return Json(record.AsQueryable().toKG());
        }

        public JsonResult GetComplCode(SvMstCampaign model)
        {
            var record = ctx.SvMstCampaigns.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType)
                .Select(p => new
                {
                    p.ComplainCode,
                    DescriptionComplain = (ctx.svMstRefferenceServices.Where(p1 => p1.CompanyCode == CompanyCode && p1.ProductType == ProductType && p1.RefferenceType == "COMPLNCD")).FirstOrDefault().Description,
                    DescriptionEngComplain = (ctx.svMstRefferenceServices.Where(p1 => p1.CompanyCode == CompanyCode && p1.ProductType == ProductType && p1.RefferenceType == "COMPLNCD")).FirstOrDefault().DescriptionEng,
                }).Distinct();

            var record1 = record.Where(p => p.ComplainCode == model.ComplainCode).FirstOrDefault();

            if (record1 != null)
                return Json(new { success = true, data = record1 });
            else return Json(new { success = false });
        }

        public JsonResult GetDefectCode(SvMstCampaign model)
        {
            var record = ctx.SvMstCampaigns.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType)
                .Select(p => new
                {
                    p.DefectCode,
                    DescriptionComplain = (ctx.svMstRefferenceServices.Where(p1 => p1.CompanyCode == CompanyCode && p1.ProductType == ProductType && p1.RefferenceType == "DEFECTCD")).FirstOrDefault().Description,
                    DescriptionEngComplain = (ctx.svMstRefferenceServices.Where(p1 => p1.CompanyCode == CompanyCode && p1.ProductType == ProductType && p1.RefferenceType == "DEFECTCD")).FirstOrDefault().DescriptionEng,
                }).Distinct();

            var record1 = record.Where(p => p.DefectCode == model.DefectCode).FirstOrDefault();

            if (record1 != null)
                return Json(new { success = true, data = record1 });
            else return Json(new { success = false });
        }
         
        public JsonResult OperationNo(SvMstCampaign model)
        {
            var record = from p in ctx.SvMstCampaigns
                         join p1 in ctx.svMstTasks on new { p.CompanyCode, p.ProductType, p.OperationNo } equals new { p1.CompanyCode, p1.ProductType, p1.OperationNo }
                         where p.CompanyCode == CompanyCode && p.ProductType == ProductType && p.OperationNo != ""
                         select new
                         {
                             p.OperationNo,
                             p1.Description,
                             IsActive = p.IsActive ? "Aktif" : "Tidak Aktif"
                         };

            return Json(record.Distinct().AsQueryable().toKG());
        }

        public JsonResult GetComplain(SvMstCampaign model)
        {
            var record = ctx.svMstRefferenceServices.Find(CompanyCode, ProductType, "COMPLNCD", model.ComplainCode);

            if (record != null) 
                return Json(new { success = true, data = record });
            else return Json(new { success = false } );
        }

        public JsonResult GetDefect(SvMstCampaign model)
        {
            var record = ctx.svMstRefferenceServices.Find(CompanyCode, ProductType, "DEFECTCD", model.DefectCode);

            if (record != null)
                return Json(new { success = true, data = record });
            else return Json(new { success = false });
        }

        public JsonResult GetOperationNo(SvMstCampaign model)
        {
            var record = ctx.svMstTasks.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.IsActive == true && x.OperationNo == model.OperationNo).FirstOrDefault();
            if (record != null)
                return Json(new { success = true, data = record });
            else return Json(new { success = false });
        }

        public JsonResult Get(SvMstCampaign model)
        {
            var record = ctx.SvMstCampaigns.Find(CompanyCode, ProductType, model.ComplainCode, model.DefectCode, model.ChassisCode, model.ChassisStartNo, model.ChassisEndNo);
            if (record != null)
                return Json(new { success = true, data = record });
            else return Json(new { success = false });
        }

        public JsonResult GetChassisCode(SvMstCampaign model)
        {
            var record = ctx.SvMstCampaigns.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType && p.ChassisCode == model.ChassisCode).Distinct().FirstOrDefault();
            if (record != null)
                return Json(new { success = true, data = record });
            else return Json(new { success = false });
        }

        public JsonResult ChassisCode()
        {
            var record = ctx.SvMstCampaigns.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType).Distinct();

            return Json(record.AsQueryable().toKG());
        }
    }
}
