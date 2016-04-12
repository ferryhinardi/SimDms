using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class SparepartController : BaseController
    {
        public string GeneratePartSales()
        {
            return HtmlRender("sp/GeneratePartSales.js");
        }

        public string GeneratePartSales2()
        {
            return HtmlRender("sp/GeneratePartSales2.js");
        }

        public string LiveStockPortal()
        {
            return HtmlRender("sp/LiveStockPortal.js");
        }

        public string LiveStockPortal2()
        {
            return HtmlRender("sp/LiveStockPortal2.js");
        }

        public string LiveStockPortalInq()
        {
            return HtmlRender("sp/LiveStockPortalInq.js");
        }

        public string LiveStockPortalTransMode()
        {
            return HtmlRender("sp/LiveStockPortalTransMode.js");
        }

        public string LiveStockPortalInq2()
        {
            return HtmlRender("sp/LiveStockPortalInq2.js");
        }

        public string LiveStockPortalTransMode2()
        {
            return HtmlRender("sp/LiveStockPortalTransMode2.js");
        }

        public string SpAnalysisRpt()
        {
            return HtmlRender("sp/SpAnalysisRpt.js");
        }

        public string SpAnalysisWeeklyRpt()
        {
            return HtmlRender("sp/SpAnalysisWeeklyRpt.js");
        }

        public string GenerateLogReport() 
        {
            return HtmlRender("sp/generatelogreport.js");
        }

        public string SpAnalysisMonthlyRpt()
        {
            return HtmlRender("sp/SpAnalysisMonthlyRpt.js");
        }

        public string SpAosLog()
        {
            return HtmlRender("sp/spAosLog.js");
        }

        public string SpAosListReport()
        {
            return HtmlRender("sp/AOSListReport.js");
        }

        public string MstAOSWarrantyParts()
        {
            return HtmlRender("sp/master/MstAOSWarrantyParts.js");
        }
    }
}
