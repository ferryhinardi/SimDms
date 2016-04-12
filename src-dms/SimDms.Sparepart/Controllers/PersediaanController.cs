using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers
{
    public class PersediaanController:BaseController
    {
        // Inventory Adjustment
        public string lnk5001()
        {
            return HtmlRender("persediaan/InventoryAdjustment.js");
        }


        // Warehouse Transfer
        public string lnk5002()
        {
            return HtmlRender("persediaan/WarehouseTransfer.js");
        }


        // Reserved Sparepart
        public string lnk5003()
        {
            return HtmlRender("persediaan/ReservedSparepart.js");
        }


    }

}
