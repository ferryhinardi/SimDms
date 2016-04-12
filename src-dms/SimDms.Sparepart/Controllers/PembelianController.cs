using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers
{
    public class PembelianController : BaseController
    {
 

        //Process Suggestion Order
        public string pso()
        {
            return HtmlRender("pembelian/pso.js");
        }

        //entry order Sparepart
        public string entryorder()
        {
            return HtmlRender("pembelian/entryorder.js");
        }
        //Maintenance On Order Subtitusi
        public string maintenancesubtitusi()
        { 
            return HtmlRender("pembelian/maintenancesubtitusi.js");
        }
        //Maintenance On Order MaintenancePo
        public string maintenancepo()
 
        {
            return HtmlRender("pembelian/maintenancepo.js");
        }

        public string entryrequest()
        {
            return HtmlRender("pembelian/entryrequest.js");
        }

        public string entryorderaos()
        {
            return HtmlRender("pembelian/entryorderaos.js");
        }  
    }
}
