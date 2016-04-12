using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers
{
    public class EmplController : BaseController
    {
        public string Persinfo()
        {
            return HtmlRender("empl/persinfo.js");
        }
        public string PersinfoDisabled()
        {
            return HtmlRender("empl/persinfo-disabled.js");
        }

        public string Salesinfo()
        {
            return HtmlRender("empl/salesinfo.js");
        }

        public string Workexp()
        {
            return HtmlRender("empl/workexp.js");
        }

        public string Mutation()
        {
            return HtmlRender("empl/mutation.js");
        }

        public string Education()
        {
            return HtmlRender("empl/education.js");
        }

        public string ServiceInfo()
        {
            return HtmlRender("empl/serviceinfo.js");
        }
    }
}
