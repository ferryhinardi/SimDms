using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers
{
    public class InquiryController : BaseController
    {
        public string Perslist()
        {
            return HtmlRender("inquiry/perslist.js");
        }

        public string PersMuta()
        {
            return HtmlRender("inquiry/persmuta.js");
        }

        public string PersAchieve()
        {
            return HtmlRender("inquiry/persachieve.js");
        }

        public string PersInvalid()
        {
            return HtmlRender("inquiry/persinvalid.js");
        }

        public string SfmPersInfo()
        {
            return HtmlRender("inquiry/sfmpersinfo.js");
        }

        public string SfmMutation()
        {
            return HtmlRender("inquiry/sfmmutation.js");
        }

        public string SfmTrend()
        {
            return HtmlRender("inquiry/sfmtrend.js");
        }

        public string SfmTurnOver()
        {
            return HtmlRender("inquiry/sfmturnover.js");
        }

        public string SfmSummary()
        {
            return HtmlRender("inquiry/sfmsummary.js");
        }

        public string AttendanceDaily()
        {
            return HtmlRender("inquiry/attendance.js");
        }

        public string AttendanceResume()
        {
            return HtmlRender("inquiry/attendance_resume.js");
        }

        public string SfmTurnOverRatio()
        {
            return HtmlRender("inquiry/sfmturnoverratio.js");
        }

        public string SfmInvalidEmployee()
        {
            return HtmlRender("inquiry/sfminvalidemployee.js");
        }

        public string SfmMpDashboard()
        {
            return HtmlRender("inquiry/info/MpDashboard.js");
        }

        public string SfmDemographicCondition()
        {
            return HtmlRender("inquiry/info/DemographicCondition.js");
        }

        public string SfmPromotion()
        {
            return HtmlRender("inquiry/turnover/promotion.js");
        }

        public string SfmDemotion()
        {
            return HtmlRender("inquiry/turnover/demotion.js");
        }

        public string SfmTrainingDashboard()
        {
            return HtmlRender("inquiry/sfmtrainingdashboard.js");
        }

        public string SfmInOutData()
        {
            return HtmlRender("inquiry/sfminoutdata.js");
        }


        public string SfmTrainingDetail()
        {
            return HtmlRender("inquiry/sfmtrainingdetail.js");
        }
    }
}
