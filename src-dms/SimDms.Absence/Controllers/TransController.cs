using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers
{
    public class TransController : BaseController
    {
        public string Upload()
        {
            return HtmlRender("trans/upload.js");
        }

        public string ShiftMap()
        {
            return HtmlRender("trans/shiftmap.js");
        }

        public string Overtime()
        {
            return HtmlRender("trans/abovertime.js");
        }
    }
}
