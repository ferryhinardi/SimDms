using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class GridController : BaseController
    {
        public JsonResult Customers()
        {
            var queryable = ctx.Customers.Where(p => p.CustomerName != "");  // retrieve customer from database using EF
            return Json(GeLang.DataTables<SimDms.CStatisfication.Models.Customer>.Parse(queryable, Request));
        }
    }
}
