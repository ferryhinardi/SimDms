using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers
{
    public class UtilityController : BaseController
    {
        public string MaintainChassis()
        {
            return HtmlRender("Utility/maintainchassis.js");
        }

        public string MaintainVehicle()
        {
            return HtmlRender("Utility/maintainvehicle.js");
        }

        public string ClosingMonth()
        {
            return HtmlRender("Utility/closingmonth.js");
        }

        public string PostingLaporan()
        {
            return HtmlRender("Utility/postinglaporan.js");
        }

        public string UploadFile()
        {
            return HtmlRender("Utility/uploadfile.js");
        }

        public string MaintainServiceBookNo()
        {
            return HtmlRender("Utility/maintainservicebookno.js");
        }

        public string InvoiceCancel()
        {
            return HtmlRender("Utility/invoicecancel.js");
        }

        public string SendSMR()
        {
            return HtmlRender("Utility/sendsmr.js");
        }

        public string SendCustomerData()
        {
            return HtmlRender("Utility/sendcustomerdata.js");
        }

        public string UploadFromDCS()
        {
            return HtmlRender("Utility/uploadfromdcs.js");
        }
    }
}
