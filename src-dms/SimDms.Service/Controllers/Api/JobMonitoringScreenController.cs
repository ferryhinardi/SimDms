using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class JobMonitoringScreenController : BaseController
    {
        public ActionResult Default()
        {
            var sqlHdr = "exec uspfn_SvGetMonitoringHdr";
            var hdr = ctx.Database.SqlQuery<MonitorHdr>(sqlHdr);


            return Json(new
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
            });
        }

        public class MonitorHdr
        {
            public string Field01 { get; set; }
            public string Field02 { get; set; }
            public int? Field03 { get; set; }
            public string Field04 { get; set; }
            public int? Field05 { get; set; }
        }
    }
}
