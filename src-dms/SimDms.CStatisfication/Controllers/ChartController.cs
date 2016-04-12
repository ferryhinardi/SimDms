using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers
{
    public class ChartController : BaseController
    {
        public string Monitoring1()
        {
            return HtmlRender("chart/monitoring1.js");
        }

        public string Monitoring2()
        {
            return HtmlRender("chart/monitoring2.js");
        }

        public string Monitoring3()
        {
            return HtmlRender("chart/monitoring3.js");
        }

        public string Monitoring4()
        {
            return HtmlRender("chart/monitoring4.js");
        }

        public string Monitoring5()
        {
            return HtmlRender("chart/monitoring5.js");
        }

        public string Monitoring6()
        {
            return HtmlRender("chart/monitoring6.js");
        }

        public string Monitoring7()
        {
            return HtmlRender("chart/monitoring7.js");
        }

        public string Monitoring8()
        {
            return HtmlRender("chart/monitoring8.js");
        }
    }
}