using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class SalesSystemController : BaseController
    {
        //
        // GET: /SalesSystem/

        public string DailySPKReportPerDealer()
        {
            return HtmlRender("sl/DailySPKReportPerDealer.js");
        }
    }
}
