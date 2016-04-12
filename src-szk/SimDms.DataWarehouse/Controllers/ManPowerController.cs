using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class ManPowerController : BaseController
    {
        public string PersList()
        {
            return HtmlRender("mp/perslist.js");
        }

        public string PrsAchieve()
        {
            return HtmlRender("mp/persachieve.js");
        }

        public string PrsMuta()
        {
            return HtmlRender("mp/persmuta.js");
        }

        public string PersInvalid()
        {
            return HtmlRender("mp/persinvalid.js");
        }

        public string Consolidation()
        {
            return HtmlRender("mp/consolidation.js");
        }

        public string InqSendData()
        {
            return HtmlRender("mp/inqSendData.js");
        }

        public string MpPersInfo()
        {
            return HtmlRender("mp/MpPersInfo.js");
        }

        public string MpDashboard()
        {
            return HtmlRender("mp/MpDashboard.js");
        }

        public string MpTrainingSummary()
        {
            return HtmlRender("mp/MpTrainingSummary.js");
        }

        public string MpDataTrend()
        {
            return HtmlRender("mp/MpDataTrend.js");
        }

        public string MpRotation()
        {
            return HtmlRender("mp/MpRotation.js");
        }

        public string MpTrainingDetails()
        {
            return HtmlRender("mp/MpTrainingDetails.js");
        }

        public string MpSalesTeamStructures()
        {
            return HtmlRender("mp/MpSalesTeamStructures.js");
        }

        public string DemographicCondition()
        {
            return HtmlRender("mp/DemographicCondition.js");
        }

        public string Promotion()
        {
            return HtmlRender("mp/Promotion.js");
        }

        public string Demotion()
        {
            return HtmlRender("mp/Demotion.js");
        }
    }
}
