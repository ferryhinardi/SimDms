using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers
{
    public class InventoryController : BaseController
    {   
        public string transferout()
        {
            return HtmlRender("inventory/transferout.js");
        }

        public string transferoutprint()
        {
            return HtmlRender("inventory/transferoutprint.js");
        }

      public string transferin()
        {
            return HtmlRender("inventory/transferin.js");
        }

      public string gantiwarna()
        {
            return HtmlRender("inventory/gantiwarna.js");
        }

      public string transferoutmulti()
      {
          return HtmlRender("inventory/transferoutmulti.js");
      }

      public string transferinmulti() 
      {
          return HtmlRender("inventory/transferinmulti.js");
      }
    }

}
