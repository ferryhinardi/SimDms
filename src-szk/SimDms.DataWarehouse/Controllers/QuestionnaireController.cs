using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class QuestionnaireController : BaseController
    {
        public string Questionnaire()
        {
            return HtmlRender("qa/questionnaire.js");
        }

        public string QaRowData()
        {
            return HtmlRender("qa/qarowdata.js");
        }

        public string QaReportByType()
        {
            return HtmlRender("qa/rekapbytype.js");
        }

        public string QaReportByTypeDealer()
        {
            return HtmlRender("qa/rekapbytypedealer.js");
        }

        public string QaReportSummary()
        {
            return HtmlRender("qa/qarekapsummary.js");
        }

        public string QaRowDataDealer()
        {
            return HtmlRender("qa/rowdatadealer.js");
        }

        public string QaMonitoringCust()
        {
            return HtmlRender("qa/monitoringpercustomer.js");
        }

        public string QaMonitoringOutlet()
        {
            return HtmlRender("qa/monitoringperoutlet.js");
        }

        public string Questionnaire2()
        {
            return HtmlRender("qa/questionnaire2.js");
        }

        public string QaRowData2()
        {
            return HtmlRender("qa/qarowdata2.js");
        }

        public string QaRowDataDealer2()
        {
            return HtmlRender("qa/rowdatadealer2.js");
        }

        public string QaReportSummary2()
        {
            return HtmlRender("qa/qa2rekapsummary.js");
        }

        public string QaMonitoringCust2()
        {
            return HtmlRender("qa/monitoringpercustomer2.js");
        }

        public string QaMonitoringOutlet2()
        {
            return HtmlRender("qa/monitoringperoutlet2.js");
        }

        public string QaReportSummary2Dealer()
        {
            return HtmlRender("qa/qa2rekapsummarydealer.js");
        }

        public string QaMonitoringOutletv2()
        {
            return HtmlRender("qa/monitoringoutletv2.js");
        }

        public string QaMonitoringOutletv2Dealer()
        {
            return HtmlRender("qa/monitoringoutletv2dealer.js");
        }

        public string QaSummaryOccupation()
        {
            return HtmlRender("qa/summaryperoccupation.js");
        }

        public string MstQaException()
        {
            return HtmlRender("qa/MstQaException.js");
        }
    }
}
