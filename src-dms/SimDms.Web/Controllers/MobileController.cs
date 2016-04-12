using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Web.Controllers
{
    public class MobileController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}
