using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers
{
    public class DashboardController : BaseController
    {
        public string EmplDist()
        {
            return HtmlRender("dashboard/empldist.js");
        }

        public string EmplDist2()
        {
            return HtmlRender("dashboard/empldist2.js");
        }

        public string EmplByDept()
        {
            return HtmlRender("dashboard/emplbydept.js");
        }
    }
}
