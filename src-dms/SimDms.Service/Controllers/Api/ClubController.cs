using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class ClubController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                
                ClubCodeStr = "KLB/XX/YYYYYY",
                CreatedDate = DateTime.Now,
                LaborDiscPct = 0,
                PartDiscPct = 0,
                MaterialDiscPct = 0,
                ClubDateStart = DateTime.Now,
                ClubDateFinish = DateTime.Now,
                ClubSince = DateTime.Now,   
                
            });
        }

        public string getData() {
            var transdate = ctx.CoProfileServices.Find(CompanyCode,BranchCode).TransDate;
            return GetNewDocumentNo("KLB", transdate.Value);
        }

        public JsonResult Save(svMstClub model)
        {

            var record = ctx.svMstClubs.Find(CompanyCode, model.ClubCode);
            string code = getData();
            if (record == null)
            {
                record = new svMstClub
                {
                    CompanyCode = CompanyCode,
                    ClubCode = code,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                };
                ctx.svMstClubs.Add(record);
            }

            record.Description = model.Description;
            record.LaborDiscPct = model.LaborDiscPct;
            record.PartDiscPct = model.PartDiscPct;
            record.MaterialDiscPct = model.MaterialDiscPct;
            record.IsActive = model.IsActive;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, record.ClubCode, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SaveKedua(SvClubView model, CustomerVehicle DetModel, bool IsActiveP)
        {

            var record = ctx.CustomerVehicles.Find(CompanyCode, DetModel.ChassisCode, DetModel.ChassisNo);
            if (record == null)
            {
                record = new CustomerVehicle
                {
                    CompanyCode = CompanyCode,
                    ChassisCode = DetModel.ChassisCode,
                    ChassisNo = DetModel.ChassisNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
            
                    
                };
                ctx.CustomerVehicles.Add(record);
            }

            
            record.ClubCode = model.ClubCodeStr;
            record.ClubNo = DetModel.ClubNo;
            record.ClubDateStart = DetModel.ClubDateStart;
            record.ClubDateFinish = DetModel.ClubDateFinish;
            record.ClubSince = DetModel.ClubSince;
            record.IsActive = IsActiveP;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, record.ClubCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult deleteData(svMstClub model, string ClubCodeStr)
        {

            var record = ctx.svMstClubs.FirstOrDefault(a=>a.CompanyCode==CompanyCode && a.ClubCode == ClubCodeStr);
            if (record != null)
            {
                ctx.svMstClubs.Remove(record);
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

        public JsonResult deleteDataKedua(svMstClub model, CustomerVehicle DetModel)
        {

            var record = ctx.CustomerVehicles.Find(CompanyCode, DetModel.ChassisCode, DetModel.ChassisNo);
            if (record == null)
            {
                record = new CustomerVehicle
                {
                    CompanyCode = CompanyCode,
                    ChassisCode = DetModel.ChassisCode,
                    ChassisNo = DetModel.ChassisNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                };
                ctx.CustomerVehicles.Add(record);
            }

            record.ClubCode = "";
            record.ClubNo = "";
            record.ClubDateStart = Convert.ToDateTime("1900-01-01 00:00:00");
            record.ClubDateFinish = Convert.ToDateTime("1900-01-01 00:00:00");
            record.ClubSince = Convert.ToDateTime("1900-01-01 00:00:00");
            record.IsActive = false;
            //record.ProductionYear = model.ProductionYear;
            
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            //record.ContactName = model.ContactName;
            // record.ContactAddress = model.ContactAddress;
            //record.ContactPhone = model.ContactPhone;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, record.ClubCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult getDataTable(SvClubTable model, string ClubCodeStr)
        {

            var record = ctx.SvClubTables.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.ClubCode == ClubCodeStr).Select(x => new { 
                PoliceRegNo = x.PoliceRegNo,
                CustomerCode = x.CustomerCode,
                ClubNo = x.ClubNo,
                ClubDateStart = x.ClubDateStart,
                ClubDateFinish = x.ClubDateFinish,
                ClubSince = x.ClubSince,
                IsActiveP = x.IsActiveP,
                ChassisCode = x.ChassisCode,
                ChassisNo = x.ChassisNo,
                IsActiveDesc = x.IsActiveDesc,
                CustomerName = x.CustomerName,
                ServiceBookNo = x.ServiceBookNo
            });

//            int lengthTemp = record.Count();
            return Json(record);


        }

    }
}
