using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.PreSales.Controllers;

namespace SimDms.PreSales.Controllers
{
    public class ReportController : BaseController
    {
        public string InqProd()
        {
            return HtmlRender("report/inqprod.js");
        }

        public string InqFollowup()
        {
            return HtmlRender("report/inqfollowup.js");
        }
    }
}
