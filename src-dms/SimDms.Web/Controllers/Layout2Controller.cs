using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SimDms.Web.Controllers
{
    public class Layout2Controller : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.BranchCode = BranchCode;
            ViewBag.BranchName = BranchName;
            return View();
        }
    }
}
