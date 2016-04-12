using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers
{
    public class MasterController : BaseController
    {
        public string Lookup()
        {
            return HtmlRender("master/lookup.js");
        }

        public string Holiday()
        {
            return HtmlRender("master/holiday.js");
        }

        public string Shift()
        {
            return HtmlRender("master/shift.js");
        }

        public string Dept()
        {
            return HtmlRender("master/dept.js");
        }

        public string Position()
        {
            return HtmlRender("master/position.js");
        }
    }
}
