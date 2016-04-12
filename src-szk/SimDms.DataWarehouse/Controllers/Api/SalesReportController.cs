using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class SalesReportController : BaseController
    {
        CultureInfo ciID = new CultureInfo("id-ID");
        public JsonResult Default()
        {
            return Json(new
            {
                success = true,
                data = new
                {
                    Date = DateTime.Now
                }
            });
        }
    }
}
