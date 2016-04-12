using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.PreSales.Controllers
{
    public class TransController : BaseController
    {
        public string InputKdp()
        {
            return HtmlRender("trans/inkdp.js");
        }

        public string inputkdpcoupon()
        {
            return HtmlRender("trans/inkdpcoupon.js");
        }

        public string InputKdpAdmin()
        {
            return HtmlRender("trans/inkdpadmin.js");
        }

        public string ClnUpKdp()
        {
            return HtmlRender("trans/clnupkdp.js");
        }

        public string SalesOrder()
        {
            return HtmlRender("trans/sales-order.js");
        }

        public string SalesOrderPrint()
        {
            return HtmlRender("trans/sales-order-print.js");
        }
    }
}
