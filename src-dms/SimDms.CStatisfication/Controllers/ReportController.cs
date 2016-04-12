using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers
{
    public class ReportController : BaseController
    {
        public string TDayCall()
        {
            return HtmlRender("report/tdaycall.js");
        }
        public string StnkExt()
        {
            return HtmlRender("report/stnkext.js");
        }
        public string CusBday()
        {
            return HtmlRender("report/cusbday.js");
        }
        public string Bpkb()
        {
            return HtmlRender("report/bpkb.js");
        }
    }
}
