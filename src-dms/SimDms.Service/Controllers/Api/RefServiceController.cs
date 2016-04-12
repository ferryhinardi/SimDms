using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;

namespace SimDms.Service.Controllers.Api
{
    public class RefServiceController : BaseController
    {
        //
        // GET: /RefService/

        public JsonResult Default()
        {
            return Json(new
            {
               
                CompanyCode = CompanyCode,
            });
        }

        public JsonResult Save(svMstRefferenceService model)
        {
            var record = ctx.svMstRefferenceServices.Find(CompanyCode, ProductType, model.RefferenceType, model.RefferenceCode);

            if (record == null)
            {
                record = new svMstRefferenceService
                {
                    CompanyCode = CompanyCode,
                    ProductType = ProductType,
                    RefferenceType = model.RefferenceType,
                    RefferenceCode = model.RefferenceCode,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                };
                ctx.svMstRefferenceServices.Add(record);
            }

            record.Description = model.Description;
            record.DescriptionEng = model.DescriptionEng;
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

        public JsonResult deleteData(svMstRefferenceService model)
        {

            var record = ctx.svMstRefferenceServices.Find(CompanyCode, ProductType, model.RefferenceType, model.RefferenceCode);
            if (record != null)
            {
                ctx.svMstRefferenceServices.Remove(record);
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

        public JsonResult Get(svMstRefferenceService model)
        {
            var record = ctx.svMstRefferenceServices.Find(CompanyCode, ProductType, model.RefferenceType, model.RefferenceCode);
            if (record != null)
                return Json(new { success = true, data = record });
            else return Json(new { success = false });
        }

        public string RefferenceCode { get; set; }

        public object RefferenceType { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public object DescriptionEng { get; set; }
    }
}
