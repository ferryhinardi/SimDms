using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers
{
    public class SalesController : BaseController
    {
        public string DraftSalesOrder()
        {
            return HtmlRender("sales/draftsalesorder.js");
        }


        public string listpricebranches()
        {
            return HtmlRender("sales/listpricebranches.js");
        }

        public string SalesOrder()
        {
            return HtmlRender("sales/sales-order.js");
        }

        public string SalesOrderSFM()
        {
            return HtmlRender("sales/salesordersfm.js");
        }
        
        public string SalesOrderPO()
        {
            return HtmlRender("sales/salesorderPO.js");
        }

        public string delor()
        {
            return HtmlRender("sales/delor.js");
        }

        public string bpk()
        {
            return HtmlRender("sales/bpk.js");
        }

        public string perlengkapanout()
        {
            return HtmlRender("sales/perlengkapanout.js");
        }

        public string invoice()
        {
            return HtmlRender("sales/invoice.js");
        }

        public string retur()
        {
            return HtmlRender("sales/retur.js");
        }

        public string permohonanfktrpls()
        {
            return HtmlRender("sales/permohonanfktrpls.js");
        }

        public string fakturpolisi()
        {
            return HtmlRender("sales/fakturpolisi.js");
        }

        public string mntnfktrpls()
        {
            return HtmlRender("sales/mntnfktrpls.js");
        }

        public string spktrkbbn()
        {
            return HtmlRender("sales/spktrkbbn.js");
        }

        public string tandaterimabpkb()
        {
            return HtmlRender("sales/tandaterimabpkb.js");
        }

        public string tandaterimastnk()
        {
            return HtmlRender("sales/tandaterimastnk.js");
        }

        public string kwitansiunit()
        {
            return HtmlRender("sales/kwitansiunit.js");
        }

        public string permohonanfktrplsDCS()
        {
            return HtmlRender("sales/permohonanfktrplsDCS.js");
        }

        public string OwnershipVehicle() 
        {
            return HtmlRender("sales/ownershipvehicle.js");
        }

        public string Permohonanfpsd()
        {
            return HtmlRender("sales/permohonanfpsd.js");
        }

        public string InformasiPengiriman()
        {
            return HtmlRender("sales/InformasiPengirimanKendaraan.js");
        }

        public string fktrplsRev()
        {
            return HtmlRender("sales/fktrplsRev.js");
        }

        public string Regrevfakpol()
        {
            return HtmlRender("sales/regrevfakpol.js");
        }
    }
}
