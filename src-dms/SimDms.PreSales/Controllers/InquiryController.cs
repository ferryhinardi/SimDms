using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.PreSales.Controllers
{
    public class InquiryController : BaseController
    {
        public string Followup()
        {
            return HtmlRender("inquiry/followup.js");
        }

        public string LostCase()
        {
            return HtmlRender("inquiry/lostcase.js");
        }

        public string Summary()
        {
            return HtmlRender("inquiry/summary.js");
        }

        public string Achieve()
        {
            return HtmlRender("inquiry/achieve.js");
        }

        public string Outstanding()
        {
            return HtmlRender("inquiry/outstanding.js");
        }

        public string Periode()
        {
            return HtmlRender("inquiry/byperiode.js");
        }

        public string InqIts()
        {
            return HtmlRender("inquiry/InqIts.js");
        }

        public string salesachievement()
        {
            return HtmlRender("inquiry/salesachievement.js");
        }

        public string OutstandingProspek()
        {
            return HtmlRender("inquiry/outstandingprospek.js");
        }

        public string SISHistory() 
        {
            return HtmlRender("inquiry/sishistory.js");
        }
        //inqitsmkt
        public string InqItsMkt()
        {
            return HtmlRender("inquiry/inqItsMkt.js");
        }

        //inqitsstatus
        public string InqItsStatus()
        {
            return HtmlRender("inquiry/inqitsstatus.js");
        }

        public string GenerateITSWithStatusAndTestDrive()
        {
            return HtmlRender("inquiry/inqItsWithStatusAndTestDrive.js");
        }

        public string Monitoring()
        {
            return HtmlRender("inquiry/Monitoring.js");
        }

        public string MonitoringByWorkingDay()
        {
            return HtmlRender("inquiry/MonitoringByWorkingDay.js");
        }
    }
}
