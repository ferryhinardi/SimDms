using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.PreSales.Controllers.Api
{
    public class CityController : BaseController
    {
        public JsonResult GetName(string id = "")
        {
            var city = ctx.LookUpDtls.Find(CompanyCode, "CITY", id);
            if (city != null)
            {
                return Json(new { success = true, data = city.LookUpValueName ?? "" });
            }
            else
            {
                return Json(new { success = false, data = "" });
            }
        }
    }
}
