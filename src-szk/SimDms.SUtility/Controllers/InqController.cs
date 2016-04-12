using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.SUtility.Controllers
{
    public class InqController : BaseController
    {
        public string GeneratePartSales()
        {
            return HtmlRender("inq/GeneratePartSales.js");
        }

    }
}
