using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class SRSServiceReminderController : BaseController
    {
        public JsonResult GetSrvReminderGrid(string PoliceRegNo, DateTime FirstPeriod, DateTime EndPeriod, bool Active, bool NonActive)
        {
            //string value = PoliceRegNo.Trim().Replace("*", "%").Replace(" ", "%").Replace("'", "");
            //if (!(value.StartsWith("%") || value.EndsWith("_")))
            //{
            //    value = value + "%";
            //}

            var SQL = @"
                    select
                     a.BasicModel
                    ,a.PoliceRegNo
                    ,a.CustomerCode
                    ,b.CustomerName
                    ,(b.Address1+' '+b.Address2+' '+b.Address3+' '+b.Address4) as Address
                    ,b.PhoneNo
                    ,b.HPNo
                    ,case year(a.LastServiceDate) when 1900 then null else a.LastServiceDate end LastServiceDate
                    ,a.LastServiceOdometer
                    ,a.LastJobType
                    ,a.ChassisCode
                    ,a.ChassisNo
                    ,a.EngineCode
                    ,a.EngineNo
                    ,a.ServiceBookNo
                    ,c.RefferenceDesc1 Color
                    ,case year(a.FakturPolisiDate) when 1900 then null else a.FakturPolisiDate end FakturPolisiDate
                    ,b.Email
                    ,case year(b.BirthDate) when 1900 then null else b.BirthDate end BirthDate
                    ,a.ContactName
                    ,a.ContactAddress
                    ,a.ContactPhone
                    from svMstCustomerVehicle a
                    left join gnMstCustomer b
                      on b.CompanyCode = a.CompanyCode
                     and b.CustomerCode = a.CustomerCode
                    left join omMstRefference c
                      on c.CompanyCode = a.CompanyCode
                     and c.RefferenceCode = a.ColourCode
                     and c.RefferenceType = 'COLO'
                    where 1 = 1
                     and a.CompanyCode = {0}
                     and a.PoliceRegNo like '{1}'
                     and convert(varchar,a.LastServiceDate,112) between {2} and {3}
                    ";
            if (Active)
                SQL += " and a.isActive = 1";
            else if (Active && NonActive)
                SQL += " and a.isActive IN (0,1)";
            else if (!Active)
                SQL += " and a.isActive = 0";
            else
                SQL += " and a.isActive = 9";

            var query = string.Format(SQL, CompanyCode, PoliceRegNo, FirstPeriod.ToString("yyyyMMdd"), EndPeriod.ToString("yyyyMMdd"));

            var data = ctx.Database.SqlQuery<MstCustomerVehicleView>(query);

            return Json(data);
        }
    }
}