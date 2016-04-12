using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Tax.Controllers
{
    public class TaxController : BaseController
    {
        public string GenTax()
        {
            return HtmlRender("tax/gentax.js");
        }

        public string GenTaxOnline()
        {
            return HtmlRender("tax/gentaxonline.js");
        }
        public string KonsolidasiTaxIn()
        {
            return HtmlRender("tax/consolidationtaxin.js");
        }
        public string KonsolidasiTaxOut() 
        {
            return HtmlRender("tax/consolidationtaxout.js");
        }
        public string EntryTaxManual() 
        {
            return HtmlRender("tax/entrytaxmanual.js");
        }
        public string MaintenanceFPS()
        {
            return HtmlRender("tax/maintenancefps.js");
        }
    }
}
