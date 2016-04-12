using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers
{
    public class PenerimaanController : BaseController
    {
 

        // Entry Draft Penerimaan (Binning)
        public string edp()
        {
            return HtmlRender("penerimaan/edp.js");
        }
        // Entry Penerimaan Persediaan (WRS)
        public string wrs()
        {
            return HtmlRender("penerimaan/wrs.js");
        }
        // Entry HPP
        public string hpp()
        {
            return HtmlRender("penerimaan/hpp.js");
        }
        //Entry Claim Supplier
        public string entryclaimsupplier()
        {
            return HtmlRender("penerimaan/entryclaimsupplier.js");
        }
        //Receiving Claim Vendor
        public string claimvendor()
        {
            return HtmlRender("penerimaan/claimvendor.js");
        }


    }
}
