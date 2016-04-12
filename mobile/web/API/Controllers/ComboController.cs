using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using eXpressAPI.Models;
using System.Data;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("appdata/list")]
    [Route("{action=index}")]
    public class ComboController : DefaultController
    {
        public string Index()
        {
            return "Welcome";
        }

        public JsonResult DbConfig()
        {
            return Json(new { Source = MyGlobalVar.DataString(0), Db1 = MyGlobalVar.DataString(1), Db2 = MyGlobalVar.DataString(2) }, JsonRequestBehavior.AllowGet);
        }

        public string CurrentUser()
        {
            return UserName;
        }
        
        public JsonResult Menus()
        {
            string callback = Request["callback"];
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.Menus.AsQueryable();
            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => x.Name.Contains(sfilter));
            }

            return Json(data.Select(x => new { value = x.MenuId, text = x.MenuId + " - " + x.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Roles()
        {
            return Json(ListRoles(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Companies()
        {
            return Json(ListCompany(), JsonRequestBehavior.AllowGet);
        }
        
    }
}