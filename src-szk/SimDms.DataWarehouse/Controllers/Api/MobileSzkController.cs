using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class MobileSzkController : BaseController
    {
        public ContentResult DealerOutstanding(string callback)
        {
            var json = Exec(new { query = "uspfn_SysDealerOutstanding" });

            return Content(String.Format("{0}({1});",
                callback,
                new JavaScriptSerializer().Serialize(json.Data)),
                "application/javascript");
        }

        public ContentResult DealerExtracted(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_SysDealerExtracted",
                param = new List<dynamic>
                {
                    new { key = "TableName", value = Request.Params["TableName"] },
                    new { key = "DealerCode", value = Request.Params["DealerCode"] },
                },
            });

            return Content(String.Format("{0}({1});",
                callback,
                new JavaScriptSerializer().Serialize(json.Data)),
                "application/javascript");
        }

        public ContentResult DealerTables(string callback)
        {
            var json = Exec(new { query = "uspfn_SysDealerTable" });

            return Content(String.Format("{0}({1});",
                callback,
                new JavaScriptSerializer().Serialize(json.Data)),
                "application/javascript");
        }
    }
}
