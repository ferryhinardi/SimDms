using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class SlController : BaseController
    {
        public string InqReportSalesNStock()
        {
            return HtmlRender("sl/InqReportSalesNStock.js");
        }

        public string Mssi()
        {
            return HtmlRender("sl/mssi.js");
        }

        public string ModelMapping()
        {
            return HtmlRender("sl/modelmapping.js");
        }

        public string LiveStock()
        {
            return HtmlRender("sl/liveStock.js");
        }

        public string liveStockMaintenance()
        {
            return HtmlRender("sl/liveStockMaintenance.js");
        }

        public string MSSIMaster()
        {
            return HtmlRender("sl/mssi-master.js");
        }

        public string InqSales()
        {                
            return HtmlRender("sl/InqSales.js");        
        }

		public string LiveStockDealer()
		{
			return HtmlRender("sl/liveStockDealer.js");
		}

		public string liveStockDealerMaintenance()
		{
			return HtmlRender("sl/liveStockDealerMaintenance.js");
		}

        public string InqFakturPolisi()
        {
            return HtmlRender("sl/InqFakturPolisi.js");
        }

        public string GenerateDSR()
        {
            return HtmlRender("its/GenerateDSR.js");
        }
    }
}
