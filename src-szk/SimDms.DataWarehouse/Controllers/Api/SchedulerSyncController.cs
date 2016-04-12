using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class SchedulerSyncController : BaseController
    {
        public ContentResult DealerOutstanding(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_SysDealerHistGet2",
                param = new List<dynamic>
                {
                    new { key = "length", value = Request.Params["length"] ?? "1000" },
                }
            });

            return Content(String.Format("{0}({1});",
                callback,
                new JavaScriptSerializer().Serialize(json.Data)),
                "application/javascript");
        }

        public JsonResult EntrySQL(SysSQLGateway data)
        {
            var ret = GenerateSQL(data);

            return Json(new
            {
                success = ret,
                status = ret,
                TaskNo = data.TaskNo
            }, JsonRequestBehavior.AllowGet);
        }


    }
}
