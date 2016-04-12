using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class SvChartController : BaseController
    {
        public string MonitoringByPeriode()
        {
            return HtmlRender("sv/chart/MonitoringByPeriode.js");
        }

        public string UnitIntakeSummary()
        {
            return HtmlRender("sv/chart/UnitIntakeSummary.js");
        }

        public string RegisterSpk1()
        {
            return HtmlRender("sv/chart/RegisterSpk1.js");
        }

        public string RegisterSpk2()
        {
            return HtmlRender("sv/chart/RegisterSpk2.js");
        }
    }
}
