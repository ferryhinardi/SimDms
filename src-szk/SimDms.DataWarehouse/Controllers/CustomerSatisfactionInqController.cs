using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class CustomerSatisfactionInqController : BaseController
    {
        public string Rekap3DaysCall()
        {
            return HtmlRender("cs/tdaycall.js");
        }

        public string StnlExtension()
        {
            return HtmlRender("cs/stnkext.js");
        }
        public string BpkbReminder()
        {
            return HtmlRender("cs/bpkbreminder.js");
        }
        public string CustomerBirthday()
        {
            return HtmlRender("cs/custbday.js");
        }
        public string CustomerFeedback()
        {
            return HtmlRender("cs/feedback.js");
        }

        public string CustStatus()
        {
            return HtmlRender("cs/CustStat.js");
        }

        public string DashboardOutstanding()
        {
            return HtmlRender("cs/dashboard.js");
        }

        public string DashboardOutstanding2()
        {
            return HtmlRender("cs/dashboard2.js");
        }

        public string DashboardNotOutstanding()
        {
            return HtmlRender("cs/dashboard-negasi.js");
        }

        public string Review()
        {
            return HtmlRender("cs/review/review.js");
        }
    }
}
