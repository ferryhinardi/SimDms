using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers
{
    public class InquiryController : BaseController
    {
        public string TDayCall()
        {
            return HtmlRender("inquiry/tdaycall.js");
        }

        public string StnkExt()
        {
            return HtmlRender("inquiry/stnkext.js");
        }

        public string BpkbReminder()
        {
            return HtmlRender("inquiry/bpkbreminder.js");
        }

        public string CustHoliday()
        {
            return HtmlRender("inquiry/custholiday.js");
        }

        public string CustBirthday()
        {
            return HtmlRender("inquiry/custbday.js");
        }

        public string CustFeedback()
        {
            return HtmlRender("inquiry/feedback.js");
        }

        public string JsTest()
        {
            return HtmlRender("inquiry/jstest.js");
        }

        public string CsSettings()
        {
            return HtmlRender("master/settings.js");
        }

        public string CustStatus()
        {
            return HtmlRender("inquiry/InqCustStat.js");
        }

        public string outstandingDelivery()
        {
            return HtmlRender("inquiry/outstandingDelivery.js");
        }
    }
}
