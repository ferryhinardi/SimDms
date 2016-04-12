using SimDms.CStatisfication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class TicketController : BaseController
    {
        public JsonResult CurrentOutlet()
        {
            return Json(new
            {
                success = true,
                data = new Outlet
                {
                    CompanyCode = "6006400",
                    CompanyName = "PT. BUANA INDOMOBIL TRADA",
                    DealerCode = "6006406",
                    DealerName = "PT. BUANA INDOMOBIL TRADA - DEWI SARTIKA"
                }
            });
        }
    }
}
