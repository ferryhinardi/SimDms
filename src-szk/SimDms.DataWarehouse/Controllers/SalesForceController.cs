using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class SalesForceController : BaseController
    {
        public string PersList()
        {
            return HtmlRender("sf/perslist.js");
        }

        public string PrsAchieve()
        {
            return HtmlRender("sf/persachieve.js");
        }

        public string PrsMuta()
        {
            return HtmlRender("sf/persmuta.js");
        }

        public string PersInvalid()
        {
            return HtmlRender("sf/persinvalid.js");
        }

        public string PersInfo()
        {
            return HtmlRender("sf/persinfo.js");
        }

        public string PersInfo2()
        {
            return HtmlRender("sf/persinfo2.js");
        }

        public string PersInfoCutOff()
        {
            return HtmlRender("sf/persinfocutoff.js");
        }

        public string Mutation()
        {
            return HtmlRender("sf/mutation.js");
        }

        public string Trend()
        {
            return HtmlRender("sf/trend.js");
        }

        public string OutstandingTraining()
        {
            return HtmlRender("sf/outstanding-training.js");
        }

        public string ReviewSFM()
        {
            return HtmlRender("sf/review-sfm.js");
        }

        public string SalesTeam()
        {
            return HtmlRender("sf/sales-team.js");
        }

        public string SalesmanTraining()
        {
            return HtmlRender("sf/salesman-training.js");
        }

        public string SCSHBMTraining()
        {
            return HtmlRender("sf/sc-sh-bm-training.js");
        }

        public string TurnOver()
        {
            return HtmlRender("sf/turn-over.js");
        }

        public string TurnOverRatio()
        {
            return HtmlRender("sf/turnoverratio.js");
        }

        public string InvalidEmployee()
        {
            return HtmlRender("sf/invalidemployee.js");
        }

        public string MpDashboard()
        {
            return HtmlRender("sf/mpdashboard.js");
        }

        public string TrainingDetail()
        {
            return HtmlRender("sf/trainingdetail.js");
        }

        public string TrainingDashboard()
        {
            return HtmlRender("sf/trainingdashboard.js");
        }

        public string InOutData()
        {
            return HtmlRender("sf/inoutdata.js");
        }
    }
}
