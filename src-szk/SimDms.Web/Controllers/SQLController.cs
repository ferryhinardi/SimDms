using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Web.Controllers
{
    [Authorize]
    public class SQLController : BaseController
    {
        public ActionResult Index()
        {
            string Url = Request.Url.ToString().ToLower();
            ViewBag.BaseAddress = Url.Contains("dms.suzuki.co.id") ? "http://dms.suzuki.co.id:9091" : "http://tbsdmsap01:9091";
            ViewBag.SocketUrl = ViewBag.BaseAddress + "/socket.io/socket.io.js";
            return View();
        }

        public string CheckPwd(string id)
        {
            string ret = "0";
            if (id == "script4dealer")
            {
                ret = "1";
            }
            return ret;
        }

    }

    public class CallController : BaseController
    {
        public string Index()
        {
            return DateTime.Now.ToString();
        }

    }
}
