using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace SimDms.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
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

        public void RedirectToDoc()
        {
            var session = ctx.SysSessions.Where(m=>m.SessionUser == CurrentUser.UserId && m.IsLogout == false).FirstOrDefault();
            var DocumentURL = ConfigurationManager.AppSettings["DOC_URL_VALIDATE"].ToString();
            Response.Redirect(DocumentURL + session.SessionId);
        }
    }
}
