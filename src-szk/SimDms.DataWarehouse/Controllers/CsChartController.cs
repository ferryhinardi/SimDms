using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class CsChartController : BaseController
    {
        public string Monitoring()
        {
            return HtmlRender("cs/chart/Monitoring.js");
        }

        public string Monitoring1()
        {
            return HtmlRender("cs/chart/Monitoring1.js");
        }

        public string Monitoring2()
        {
            return HtmlRender("cs/chart/Monitoring2.js");
        }

        public string Monitoring3()
        {
            return HtmlRender("cs/chart/Monitoring3.js");
        }

        public string Monitoring4()
        {
            return HtmlRender("cs/chart/Monitoring4.js");
        }

        public string Monitoring5()
        {
            return HtmlRender("cs/chart/Monitoring5.js");
        }

        public string stnkExtensionReport()
        {
            return HtmlRender("cs/chart/stnkExtensionReport.js");
        }

        public string Monitoring7()
        {
            return HtmlRender("cs/chart/Monitoring7.js");
        }

        public string customerBirthdayReport()
        {
            return HtmlRender("cs/chart/customerBirthdayReport.js");
        }
    }
}
