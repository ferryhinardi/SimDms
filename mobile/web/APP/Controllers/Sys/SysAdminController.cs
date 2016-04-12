using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eXpressAPI;
using eXpressAPI.Controllers;
using eXpressAPI.Models;
//using MvcCodeRouting.Web.Mvc;

namespace eXpressAPP.Controllers.Sys
{
    [Authorize]
    public class SysAdminController : DefaultController
    {
        public ActionResult Index()
        {
            ViewBag.UserName = UserName;
            return View(GenerateNavigationMenu());
        }
    }
}