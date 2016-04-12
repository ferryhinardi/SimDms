using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.Controllers
{
    public class MasterController : BaseController
    {
        public string TeamMember()
        {
            return HtmlRender("master/teammember.js");
        }

        public string Outlets()
        {
            return HtmlRender("master/outlets.js");
        }

        public string Position()
        {
            return HtmlRender("master/position.js");
        }

        public string SalesmanGrade()
        {
            return HtmlRender("master/salesmangrade.js");
        }

        public string GroupTypeSeq()
        {
            return HtmlRender("master/grouptypeseq.js");
        }

        public string Organization()
        {
            return HtmlRender("master/organization.js");
        }
    }
}