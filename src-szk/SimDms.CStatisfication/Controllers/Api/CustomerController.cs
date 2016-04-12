using SimDms.CStatisfication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class CustomerController : BaseController
    {
        public JsonResult GetTDayCall(string CompanyCode, string CustomerCode, string Chassis)
        {
            bool isNew = false;
            var data = ctx.TDayCalls.Where(p =>
                p.CompanyCode == CompanyCode &&
                p.CustomerCode == CustomerCode &&
                p.Chassis == Chassis
                ).FirstOrDefault();
            if (data == null)
            {
                data = new TDayCall();
                var cust = ctx.Customers.Find(CompanyCode, CustomerCode, Chassis);
                if (cust != null)
                {
                    data.CompanyCode = cust.CompanyCode;
                    data.CustomerCode = cust.CompanyCode;
                    data.Chassis = cust.Chassis;
                    data.Engine = cust.Engine;
                    data.PoliceRegNo = cust.PoliceRegNo;
                    data.SalesmanCode = cust.SalesmanCode;
                    data.SalesmanName = cust.SalesmanName;
                }
                isNew = true;
            }


            return Json(new { success = true, data = data, isNew = isNew }); ;
        }

        public JsonResult SaveTDayCall(TDayCall model)
        {
            var record = ctx.TDayCalls.Find(model.CompanyCode, model.CustomerCode, model.Chassis);
            if (record == null)
            {
                record = model;
                record.BPKDate = DateTime.Now;
                record.STNKDate = DateTime.Now;
                ctx.TDayCalls.Add(record);
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
