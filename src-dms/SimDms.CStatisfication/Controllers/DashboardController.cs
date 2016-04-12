using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers
{
    public class DashboardController : BaseController
    {
        public string Summary()
        {
            return HtmlRender("dash/summary.js");
        }
    }
}
