using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Controllers
{
    public class SRSController : BaseController
    {
        public string servicereminder()
        {
            return HtmlRender("srs/servicereminder.js");
        }
        public string dailyserviceretention()
        {
            return HtmlRender("srs/dailyserviceretention.js");
        }
    }
}