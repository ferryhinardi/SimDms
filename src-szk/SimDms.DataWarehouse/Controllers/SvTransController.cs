using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class SvTransController : BaseController
    {
        public string Ftir()
        {
            return HtmlRender("sv/trans/ftir.js");
        }
        public string FtirInq()
        {
            return HtmlRender("sv/trans/ftir-inq.js");
        }

        public string TrnMRSR() 
        {
            return HtmlRender("sv/trans/TrnMRSR.js");
        }
    }
}
