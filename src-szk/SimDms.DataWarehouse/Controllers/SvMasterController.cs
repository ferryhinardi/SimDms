using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class SvMasterController : BaseController
    {
        public string CSIScore()
        {
            return HtmlRender("sv/master/CSIScore.js");
        }

        public string UnitRevenueServiceTarget()
        {
            return HtmlRender("sv/master/UnitRST.js");
        }

        public string Campaign()
        {
            return HtmlRender("sv/master/Campaign.js");
        }

        public string SvMstMRSR() 
        {
            return HtmlRender("sv/master/SvMstMRSR.js");
        }

        public string TargetVIN()
        {
            return HtmlRender("sv/master/TargetVIN.js");
        }

        public string MstDealerMapping()
        {
            return HtmlRender("sv/master/input-dealer-mapping.js");
        }
    }
}