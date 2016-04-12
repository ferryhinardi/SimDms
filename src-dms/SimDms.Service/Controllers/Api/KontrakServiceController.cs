using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class KontrakServiceController : BaseController
    {
       public JsonResult Default()
        {
            return Json(new
            {
                
                ContractNo = "KTK/XX/XXXXXX",
                ContractDate = DateTime.Now,
                LaborDiscPct = 0,
                PartDiscPct = 0,
                MaterialDiscPct = 0,
                BeginPeriod = DateTime.Now,
                EndPeriod = DateTime.Now,
                RefferenceDate = DateTime.Now,
                FakturPolisiDate = DateTime.Now,
                
            });
        }

        public string getData() 
        {
            var transdate = ctx.CoProfileServices.Find(CompanyCode,BranchCode).TransDate;
            return GetNewDocumentNo("KTK", (DateTime)transdate);
        }

        public JsonResult Save(svMstContract model)
        {
            string BeginPeriod = Convert.ToString(model.BeginPeriod);
            string EndPeriod = Convert.ToString(model.EndPeriod);
            int comparison = string.Compare(BeginPeriod, EndPeriod, false);

            if (comparison > 0)
            {
                var msg = "Period awal tidak boleh lebih besar sama dengan period akhir";
                return Json(new { success = false, message = msg });
            }

            var record = ctx.svMstContracts.Find(CompanyCode, model.ContractNo);
            //string code = getData();
            if (record == null)
            {
                record = new svMstContract
                {
                    CompanyCode = CompanyCode,
                    ContractNo = GetNewDocumentNo("RTP", model.ContractDate.Value),
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
            
                };
                ctx.svMstContracts.Add(record);
            }
            record.ContractDate = model.ContractDate;
            record.Description = model.Description;
            record.CustomerCode = model.CustomerCode;
            record.RefferenceNo = model.RefferenceNo;
            record.RefferenceDate = model.RefferenceDate;
            record.BeginPeriod = model.BeginPeriod;
            record.EndPeriod = model.EndPeriod;
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
                return Json(new { success = true, record.ContractNo });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SaveKedua(svMstContract model, CustomerVehicle DetModel)
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
            record.ContractNo = model.ContractNo;
            record.ClubCode = model.ContractNo;
            record.ClubNo = DetModel.ClubNo;
            record.ClubDateStart = DetModel.ClubDateStart;
            record.ClubDateFinish = DetModel.ClubDateFinish;
            record.ClubSince = DetModel.ClubSince;
            record.IsActive = DetModel.IsActive;
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

        public JsonResult deleteData(svMstContract model)
        {

            var record = ctx.svMstContracts.Find(CompanyCode, model.ContractNo);
            if (record != null)
            {
                ctx.svMstContracts.Remove(record);
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

        public JsonResult deleteDataKedua(svMstContract model, CustomerVehicle DetModel)
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
            record.ContractNo = "";
            record.ClubNo = DetModel.ClubNo;
            record.ClubDateStart = DetModel.ClubDateStart;
            record.ClubDateFinish = DetModel.ClubDateFinish;
            record.ClubSince = DetModel.ClubSince;
            record.IsActive = DetModel.IsActive;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            
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

        public JsonResult getDataTable(SvVehicleDetailView model, string CustomerCode, string ContractNo)
        {

            var record = ctx.SvVehicleDetailViews.Where(x => x.CompanyCode == CompanyCode && x.CustomerCode == CustomerCode && x.ContractNoStr == ContractNo).Select(x => new
            { 
                PoliceRegNo = x.PoliceRegNo,
                EngineNo = x.EngineNo,
                EngineCode = x.EngineCode,
                ChassisCode = x.ChassisCode,
                ChassisNo = x.ChassisNo,
                ServiceBookNo = x.ServiceBookNo,
                FakturPolisiDate = x.FakturPolisiDate,
            });

            return Json(record);


        }

        public JsonResult getPelangganDetail(SvCustomerDetailView model, string CustomerCode) 
        {
            var record = ctx.SvCustomerDetailViews.Where(a=> a.CustomerCode == CustomerCode && a.CompanyCode == CompanyCode && a.ProfitCenterCode == ProfitCenter).FirstOrDefault() ;
            return Json(record);
        }

    }

    }
