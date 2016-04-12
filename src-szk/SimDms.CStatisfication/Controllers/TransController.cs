using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers
{
    public class TransController : BaseController
    {
        public string TDayCall()
        {
            return HtmlRender("trans/tdcall.js");
        }

        public string CusBday()
        {
            return HtmlRender("trans/cusbday.js");
        }

        public string StnkExt()
        {
            return HtmlRender("trans/stnkext.js");
        }

        public string Bpkb()
        {
            return HtmlRender("trans/bpkb.js");
        }

        public string CHoliday()
        {
            return HtmlRender("trans/choliday.js");
        }
    }
}
