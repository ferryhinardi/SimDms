using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers
{
    public class ReportController : BaseController
    {
        public string Holiday()
        {
            return HtmlRender("report/holiday.js");
        }

        public string Shift()
        {
            return HtmlRender("report/shift.js");
        }

        public string Position()
        {
            return HtmlRender("report/position.js");
        }
    }
}
