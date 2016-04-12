using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;


namespace SimDms.Sales.Controllers
{
    public class MasterController : BaseController
    {
        
        public string listpricebranches()
        {
            return HtmlRender("master/listpricebranches.js");
        }

        public string refference()
        {
            return HtmlRender("master/refference.js");
        }

        public string model()
        {
            return HtmlRender("master/model.js");
        }

        public string modelcolor()
        {
            return HtmlRender("master/modelcolor.js");
        }

        public string salestarget()
        {
            return HtmlRender("master/salestarget.js");
        }

        public string othersinventory()
        {
            return HtmlRender("master/othersinventory.js");
        }

        public string companyaccount()
        {
            return HtmlRender("master/companyaccount.js");
        }

        public string modelaccount()
        {
            return HtmlRender("master/modelaccount.js");
        }

        public string bbnkir()
        {
            return HtmlRender("master/bbnkir.js");
        }

        public string pricelistjual()
        {
            return HtmlRender("master/pricelistjual.js");
        }

        public string pricelistbeli()
        {
            return HtmlRender("master/pricelistbeli.js");
        }

        public string karoseri()
        {
            return HtmlRender("master/karoseri.js");
        }

        public string modelyear()
        {
            return HtmlRender("master/modelyear.js");
        }

        public string perlengkapan()
        {
            return HtmlRender("master/perlengkapan.js");
        }

        public string modelperlengkapan()
        {
            return HtmlRender("master/modelperlengkapan.js");
        }

        public string PrintSalesTarget()
        {
            return HtmlRender("master/print_sales_target.js");
        }

        public string LiveStokDealer()
        {
            return HtmlRender("master/livestokdealer.js");
        }
    }
}
