using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class InsController : BaseController 
    {
        public string pmIndent()
        {
            return HtmlRender("ins/trans/pmIndent.js");
        }

        public string pmquota()
        {
            return HtmlRender("ins/mst/pmquota.js");
        }

        public string inqIndent()
        {
            return HtmlRender("ins/inq/inqindent.js");
        }
    }
}
