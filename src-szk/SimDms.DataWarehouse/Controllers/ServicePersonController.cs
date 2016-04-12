using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class ServicePersonController : BaseController
    {
        public string PersList()
        {
            return HtmlRender("sv/perslist.js");
        }

        public string PrsAchieve()
        {
            return HtmlRender("sv/persachieve.js");
        }

        public string PrsMuta()
        {
            return HtmlRender("sv/persmuta.js");
        }

        public string PersInvalid()
        {
            return HtmlRender("sv/persinvalid.js");
        }
    }
}
