using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Web.Controllers
{
    public class DashboardController : BaseController
    {
        public ActionResult ExecutiveSummary()
        {
            return View();
        }
    }
}
