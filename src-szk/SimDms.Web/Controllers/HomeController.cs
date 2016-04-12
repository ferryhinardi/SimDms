using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        //public ActionResult Index(string id)
        //{

        //    var report = ctx.ReportSessions.Find(id);

        //    if (report != null)
        //    {
        //        ViewBag.reportid = report.ReportId;
        //        ViewBag.Parameters = report.Parameters;
        //    }

        //    return View();
        //}

        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult Secure()
        {
            return View();
        }

        public string SetState()
        {
            return HtmlRender("home/setstate.js");
        }

        public string GetState()
        {
            return HtmlRender("home/getstate.js");
        }

        public JsonResult SaveState(string name, string value)
        {
            Session[name] = value;
            return Json(new { success = true, name = name, value = value });
        }

        public JsonResult ReadState(string name)
        {
            var value = Session[name];
            return Json(new { success = true, name = name, value = value });
        }
    }
}
