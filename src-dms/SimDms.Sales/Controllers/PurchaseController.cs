using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers
{
    public class PurchaseController : BaseController
    {
        public string purchaseorder()
        {
            return HtmlRender("purchase/purchaseorder.js");
        }

        public string bpu()
        {
            return HtmlRender("purchase/bpu.js");
        }

        public string bpuattribute()
        {
            return HtmlRender("purchase/bpuattribute.js");
        }

        public string hpp()
        {
            return HtmlRender("purchase/hpp.js");
        }

        public string retur()
        {
            return HtmlRender("purchase/retur.js");
        }

        public string perlengkapanin()
        {
            return HtmlRender("purchase/perlengkapanin.js");
        }

        public string perlengkapanadjst()
        {
            return HtmlRender("purchase/perlengkapanadjst.js");
        }

        public string karoseri()
        {
            return HtmlRender("purchase/karoseri.js");
        }

        public string karoseriterima()
        {
            return HtmlRender("purchase/karoseriterima.js");
        }
        
        public string generateBPU_HPP()
        {
            return HtmlRender("purchase/GenerateBPU_HPP.js");
        }
    }
}
