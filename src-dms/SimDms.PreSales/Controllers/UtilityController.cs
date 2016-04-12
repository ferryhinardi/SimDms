using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.PreSales.Controllers
{
    public class UtilityController : BaseController
    {
        public string GenerateCoupon()
        {
            return HtmlRender("utility/generatecoupon.js");
        }

        public string GenerateKDP()
        {
            return HtmlRender("utility/generatekdp.js");
        }

        public string UploadFile()
        {
            return HtmlRender("utility/uploadfile.js");
        }

        public string UtilityKDP()
        {
            return HtmlRender("utility/utilitykdp.js");
        }

        public string TransferKDP()
        {
            return HtmlRender("utility/transferkdp.js");
        }
    }
}