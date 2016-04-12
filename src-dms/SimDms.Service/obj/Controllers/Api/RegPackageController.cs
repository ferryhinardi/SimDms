using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class RegPackageController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {

                BeginDate = DateTime.Now,
                EndDate = DateTime.Now,
                

            });
        }

        public JsonResult Save(svMstPackageContract model, string AccountNo)
        {

            var record = ctx.svMstPackageContracts.Find(CompanyCode, model.PackageCode, model.ChassisCode, model.ChassisNo);
                if (record == null)
                {
                    record = new svMstPackageContract
                    {
                        CompanyCode = CompanyCode,
                        PackageCode = model.PackageCode,
                        ChassisCode = model.ChassisCode,
                        ChassisNo = model.ChassisNo,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now,

                    };
                    ctx.svMstPackageContracts.Add(record);
                }
                record.PoliceRegNo = model.PoliceRegNo;
                record.CustomerCode = model.CustomerCode;
                record.BeginDate = model.BeginDate;
                record.EndDate = model.EndDate;
                record.VirtualAccount = AccountNo;
                record.IsActive = true;
                record.LastUpdatedBy = CurrentUser.UserId;
                record.LastUpdatedDate = DateTime.Now;
            
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

        public JsonResult deleteData(svMstPackageContract model)
        {

            var record = ctx.svMstPackageContracts.Find(CompanyCode, model.PackageCode, model.ChassisCode, model.ChassisNo);
            if (record != null)
            {
                ctx.svMstPackageContracts.Remove(record);
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

        public JsonResult BrowseAction(SvKendaraanPel model, string PoliceRegNo)
        {
            var record = ctx.SvKendaraanPels.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.PoliceRegNo == PoliceRegNo);
            return Json(record);
        }

        public JsonResult OpenAction(svMstPackageContract model, string PoliceRegNo)
        {
            var record = ctx.svMstPackageContracts.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.PoliceRegNo == PoliceRegNo);
            return Json(record);
        }

        public JsonResult getPackageName(svMstPackage model, string PackageCode)
        {
            var record = ctx.svMstPackages.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.PackageCode == PackageCode);
            return Json(record);
        }

        public JsonResult getNoAccDesc(SvNomorAccView model, string AccountNo)
        {
            var record = ctx.SvNomorAccViews.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.AccountNo == AccountNo);
            return Json(record);
        }

    }
}
