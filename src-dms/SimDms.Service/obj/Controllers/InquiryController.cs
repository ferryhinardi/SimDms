using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers
{
    public class InquiryController : BaseController
    {
        public string JobMonitoring()
        {
            return HtmlRender("inquiry/jobmonitoring.js");
        }

        public string JobMonitoringScreen()
        {
            return HtmlRender("inquiry/jobmonitoringscreen.js");
        }
       
        public string WorkOrder()
        {
            return HtmlRender("inquiry/workorder.js");
        }

        public string PartMaterial()
        {
            return HtmlRender("inquiry/partmaterial.js");
        }

        public string UnitInfo()
        { 
            return HtmlRender("inquiry/unitinfo.js");
        }

        public string InqMSI()
        {
            return HtmlRender("inquiry/inqmsi.js");
        }

        public string HistServiceVehicle()
        {
            return HtmlRender("inquiry/histservicevehicle.js");
        }

        public string DcsData()
        {
            return HtmlRender("inquiry/DcsData.js");
        }

        public string HistServiceVehicleWSDS()
        {
            return HtmlRender("inquiry/histservicevehicle_wsds.js");
        }

        public string paketservice()
        {
            return HtmlRender("inquiry/paketservice.js");
        }

        public string pvttrans()
        {
            return HtmlRender("inquiry/pvttrans.js");
        }
        public string pvtinv()
        {
            return HtmlRender("inquiry/pvtinv.js");
        }
        public string pvtjournal()
        {
            return HtmlRender("inquiry/pvtjournal.js");
        }
        public string pvtcustomer()
        {
            return HtmlRender("inquiry/pvtcustomer.js");
        }
        public string pvtmsi()
        {
            return HtmlRender("inquiry/pvtmsi.js");
        }
    }
}
