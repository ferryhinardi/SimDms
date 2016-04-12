using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Tax.Controllers
{ 
    public class UtilityController : BaseController 
    {
        #region Utility
        public string SetFakTax()
        {
            return HtmlRender("Utility/SetFakTax.js");
        }
        public string PerbaikanNoPajak()
        {
            return HtmlRender("Utility/perbaikannopajak.js");
        }
        #endregion
    }
}
