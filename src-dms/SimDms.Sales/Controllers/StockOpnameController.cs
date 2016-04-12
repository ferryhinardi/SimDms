using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers
{
    public class StockOpnameController : BaseController
    {
        public string ProsesStockTaking()
        {
            return HtmlRender("stockopname/ProsesStockTaking.js");
        }

        public string EntryInvTag()
        {
            return HtmlRender("stockopname/EntryInvTag.js");
        }

        public string PostingStockTaking()
        {
            return HtmlRender("stockopname/PostingStockTaking.js");
        }

        public string InvTag()
        {
            return HtmlRender("stockopname/InvTag.js");
        }

        public string StockOpnameVeh()
        {
            return HtmlRender("stockopname/StockOpnameVeh.js");
        }

        public string RincianVehHilang()
        {
            return HtmlRender("stockopname/RincianVehHilang.js");
        }

    }
}
