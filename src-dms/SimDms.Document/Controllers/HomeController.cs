using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Document.Models;
using System.Web.Security;

namespace SimDms.Document.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        public ActionResult ValidateUser(string session)
        {
            Session["Session"] = session;
            DMSContext dtx = new DMSContext();
            var user = dtx.Database.SqlQuery<string>("select SessionUser from SysSession where sessionID = {0} AND IsLogout = '0'", session).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(user))
                FormsAuthentication.SetAuthCookie(user, false);
            else
            {
                FormsAuthentication.SignOut();
                Session.Abandon();

                // clear authentication cookie
                HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                cookie1.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie1);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Index(string session)
        {
            return View();
        }

        public ActionResult GetContent(int MenuID)
        {
            var item = ctx.SysHelps.Where(m => m.MenuID == MenuID).FirstOrDefault();
            return Json(new { content = item.Content });
        }

    }
}
