using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class SPKExhibitionController : BaseController
    {
        public string SPKExhibition()
        {
            return HtmlRender("spke/trans/SPKExhibition.js");
        }

        public string RawdataSPKExhibition()
        {
            return HtmlRender("spke/RawdataSPKExhibition.js");
        }

        public string RekapSPKExhibitionCustomer()
        {
            return HtmlRender("spke/RekapSPKExhibitionCustomer.js");
        }

        public string SummarySPKExhibitionByDate()
        {
            return HtmlRender("spke/SummarySPKExhibitionByDate.js");
        }

        public string DailySPKReportPerDealer()
        {
            return HtmlRender("spke/DailySPKReportPerDealer.js");
        }

        public string SummarySPKReportByDatePerShift()
        {
            return HtmlRender("spke/SummarySPKReportByDatePerShift.js");
        }

        public string spkerpexsum()
        {
            return HtmlRender("spke/spkerpexsum.js");
        }
    }
}
