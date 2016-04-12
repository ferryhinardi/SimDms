using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class RegCampaignController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {

                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                
            });
        }

        public JsonResult btnPoliceRegNo(MstCustomerVehicleForMstRegCompaign model)
        {
            var query = string.Format(@"
                select	vehicle.PoliceRegNo 
		                , vehicle.ChassisCode , vehicle.ChassisNo , vehicle.EngineCode , vehicle.EngineNo , vehicle.ServiceBookNo , vehicle.CustomerCode
		                , cust.CustomerName , cust.Address1 , cust.Address2 , cust.Address3 , cust.CityCode , isnull(cust.IbuKota,'') CityName		                
                from	svMstCustomerVehicle vehicle
		                inner join gnMstCustomer cust on vehicle.CompanyCode = cust.CompanyCode
			                and vehicle.CustomerCode = cust.CustomerCode		                                                          
                where vehicle.CompanyCode = '{0}' and PoliceRegNo = '{1}'
                order by  PoliceRegNo, CustomerName
            ", CompanyCode, model.PoliceRegNo);

            var record = ctx.Database.SqlQuery<MstCustomerVehicleForMstRegCompaign>(query).FirstOrDefault();

            return Json(new { success = record != null ? true : false, data = record });
        }

        public JsonResult Save(svMstFscCampaign model, string LookupValueName)
        {
           var kat = LookupValueName;
           var CF = "";
           if (kat == "ACCESORIES")
           {
               CF = "1";
           }
           else {
               CF = "2";
           }

            var record = ctx.svMstFscCampaigns.FirstOrDefault(a=>a.CompanyCode == CompanyCode && a.ChassisCode == model.ChassisCode && a.ChassisNo ==  model.ChassisNo && a.CampaignFlag == CF);
            if (record == null)
            {
                record = new svMstFscCampaign
                {
                    CompanyCode = CompanyCode,
                    ChassisCode = model.ChassisCode,
                    ChassisNo = model.ChassisNo,
                    CampaignFlag = CF,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,

                };
                ctx.svMstFscCampaigns.Add(record);
            }
            record.SalesModel = model.SalesModel;
            record.FacturNo = "";
            record.FacturDate = DateTime.Now;
            record.StartDate = model.StartDate;
            record.EndDate = model.EndDate;
            record.UpdatedBy = CurrentUser.UserId;
            record.UpdatedDate = DateTime.Now;
            
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

        public JsonResult deleteData(svMstFscCampaign model, string LookupValueName)
        {

            var kat = LookupValueName;
            var CF = "";
            if (kat == "ACCESORIES")
            {
                CF = "1";
            }
            else
            {
                CF = "2";
            }

            var record = ctx.svMstFscCampaigns.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.ChassisCode == model.ChassisCode && a.ChassisNo == model.ChassisNo && a.CampaignFlag == CF);
            if (record != null)
            {
                ctx.svMstFscCampaigns.Remove(record);
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
