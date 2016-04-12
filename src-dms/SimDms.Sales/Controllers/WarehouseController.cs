using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers
{
    public class WarehouseController : BaseController
    {
        public string ReceivingUnit()
        {
            return HtmlRender("warehouse/ReceivingUnit.js");
        }
    }
}