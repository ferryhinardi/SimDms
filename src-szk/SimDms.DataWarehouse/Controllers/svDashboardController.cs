using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SimDms.DataWarehouse.Controllers
{
    public class SvDashboardController : BaseController
    {
        public string UnitRevenueDashboard()
        {
            return HtmlRender("sv/dashboard/UnitRevenueDashboard.js");
        }

        public string CSIScoreDashboard()
        {
            return HtmlRender("sv/dashboard/CSIScoreDashboard.js");
        }

        public string ProductivityDashboard()
        {
            return HtmlRender("sv/dashboard/ProductivityDashboard.js");
        }
    }
}
