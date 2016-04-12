using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Tax.Controllers
{
    public class ReportController : BaseController
    {
        #region Reports
        public string LapTaxIn()
        {
            return HtmlRender("reports/laptaxin.js");
        }
        public string LapTaxOut()
        {
            return HtmlRender("reports/laptaxout.js");
        }
        public string AkumulasiPPN()
        {
            return HtmlRender("reports/akumulasippn.js");
        }
        public string PPN()
        {
            return HtmlRender("reports/ppn.js");
        }
        public string TaxIneSPT()
        {
            return HtmlRender("reports/taxinespt.js");
        }
        public string TaxOuteSPT()
        {
            return HtmlRender("reports/taxoutespt.js");
        }
        public string SkemaTaxOut()
        { 
            return HtmlRender("reports/skemataxout.js");
        }
        public string SkemaTaxIn()
        {
            return HtmlRender("reports/skemataxin.js");
        }
        #endregion
    }
}
