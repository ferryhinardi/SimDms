using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers
{
    public class MasterController : BaseController
    {
        public string Holiday()
        {
            return HtmlRender("master/holiday.js");
        }
    }
}
