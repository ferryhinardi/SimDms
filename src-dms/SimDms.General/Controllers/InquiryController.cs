using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.General.Controllers
{
    public class InquiryController : BaseController
    {

        public string Customers()
        {
            return HtmlRender("inquiry/customers.js");
        }

    }
}
