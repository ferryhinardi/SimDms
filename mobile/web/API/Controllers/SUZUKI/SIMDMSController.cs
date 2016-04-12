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
    [RoutePrefix("suzuki/simdms")]
    [Route("{action=index}")]
    public class SIMDMSController : DefaultController
    {
        public string Index()
        {
            return "Welcome to SIMDMS";
        }

        public JsonResult ListEvents()
        {
            SaveResult ret = new SaveResult(0, false);

            if (HaveRole("Exhibition"))
            {
                ret = MyGlobalVar.SqlQueryDB("SELECT Id, ExhibitionName TEXT FROM SysExhibitionInfo ORDER BY StartPeriod desc");
                if (ret.Count > 0)
                {
                    ret.data = ret.Table();
                }
            }
            else
            {
                ret.success = false;
                ret.message = "Access Denied !!!";
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExhibitionResult(int id)
        {
            SaveResult ret = new SaveResult(0, false);
            if (!HaveRole("Exhibition"))
            {
                ret.message = "Access Denied !!!";
            }
            else
            {
                ret = MyGlobalVar.SqlQueryDB("EXEC uspfn_SysExhibitionQuery " + id.ToString());
            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        
    }
}