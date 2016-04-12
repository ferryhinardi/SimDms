using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers
{
    public class InquiryController : BaseController
    {
        #region Purchase

        public string po()
        {
            return HtmlRender("inq/purchase/po.js");
        }



        public string bpu()
        {
            return HtmlRender("inq/purchase/bpu.js");
        }


        public string hpp()
        {
            return HtmlRender("inq/purchase/hpp.js");
        }


        public string purchasereturn()
        {
            return HtmlRender("inq/purchase/purchasereturn.js");
        }


        public string perlengkapanin()
        {
            return HtmlRender("inq/purchase/perlengkapanin.js");
        }


        public string perlengkapanadjustment()
        {
            return HtmlRender("inq/purchase/perlengkapanadjustment.js");
        }


        public string karoseri()
        {
            return HtmlRender("inq/purchase/karoseri.js");
        }


        public string karoseriterima()
        {
            return HtmlRender("inq/purchase/karoseriterima.js");
        }
        #endregion

        #region sales

        public string so()
        {
            return HtmlRender("inq/sales/so.js");
        }

        public string delivorder()
        {
            return HtmlRender("inq/sales/delivorder.js");
        }

        public string bpk()
        {
            return HtmlRender("inq/sales/bpk.js");
        }

        public string invoice()
        {
            return HtmlRender("inq/sales/invoice.js");
        }

        public string retur()
        {
            return HtmlRender("inq/sales/retur.js");
        }

        public string perlengkapanout()
        {
            return HtmlRender("inq/sales/perlengkapanout.js");
        }

        public string permohonanfakturpolisi()
        {
            return HtmlRender("inq/sales/permohonanfakturpolisi.js");
        }

        public string spktrackingbbn()
        {
            return HtmlRender("inq/sales/spktrackingbbn.js");
        }
        #endregion

        #region Inventory

        public string transferout()
        {
            return HtmlRender("inq/inventory/transferout.js");
        }

        public string transferin()
        {
            return HtmlRender("inq/inventory/transferin.js");
        }

        public string stokkendaraan()
        {
            return HtmlRender("inq/inventory/stokkendaraan.js");
        }

        public string stokperlengkapan()
        {
            return HtmlRender("inq/inventory/stokperlengkapan.js");
        }
        #endregion
        

        public string datakendaraan()
        {
            return HtmlRender("inq/datakendaraan.js");
        }

        public string datakendaraanoutcogs()
        {
            return HtmlRender("inq/datakendaraanoutcogs.js");
        }

        public string inqdatadcs()
        {
            return HtmlRender("inq/inqdatadcs.js");
        }

        public string sales()
        {
            return HtmlRender("inq/sales.js");
        }

        public string customer()
        {
            return HtmlRender("inq/customer.js");
        }

        public string prodsales()
        {
            return HtmlRender("inq/prodsales.js");
        }

        public string ITS()
        {
            return HtmlRender("inq/ITS.js");
        }

        public string ITSStatus()
        {
            return HtmlRender("inq/ITSStatus.js");
        }

        public string LiveStock()
        {
            return HtmlRender("inq/LiveStock.js");
        }
    }
}
