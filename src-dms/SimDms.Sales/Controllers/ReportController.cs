using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers
{
    public class ReportController : BaseController
    {

        #region master report

        public string RptRefference()
        {
            return HtmlRender("reports/master/RptRefference.js");
        }

        public string RptModel()
        {
            return HtmlRender("reports/master/ReportModel.js");
        }

        public string RptModelColour()
        {
            return HtmlRender("reports/master/RptModelColour.js");
        }

        public string RptModelYear()
        {
            return HtmlRender("reports/master/RptModelYear.js");
        }

        public string RptPerlengkapan()
        {
            return HtmlRender("reports/master/RptPerlengkapan.js");
        }

        public string RptModelPerlengkapan()
        {
            return HtmlRender("reports/master/RptModelPerlengkapan.js");
        }

        public string RptKaroseri()
        {
            return HtmlRender("reports/master/RptKaroseri.js");
        }

        public string RptPriceListBeli()
        {
            return HtmlRender("reports/master/RptPriceListBeli.js");
        }

        public string RptPriceListJual()
        {
            return HtmlRender("reports/master/RptPriceListJual.js");
        }

        public string RptBBNdanKIR()
        {
            return HtmlRender("reports/master/RptBBNdanKIR.js");
        }

        public string RptModelAccount()
        {
            return HtmlRender("reports/master/RptModelAccount.js");
        }

        #endregion

        #region purchase

        public string po()
        {
            return HtmlRender("reports/purchase/po.js");
        }

        public string bpu()
        {
            return HtmlRender("reports/purchase/bpu.js");
        }

        public string hpp()
        {
            return HtmlRender("reports/purchase/hpp.js");
        }

        public string perlengkapanin()
        {
            return HtmlRender("reports/purchase/perlengkapanin.js");
        }

        public string perlengkapanadjusment()
        {
            return HtmlRender("reports/purchase/perlengkapanadjusment.js");
        }

        public string registerpesanankendaraan()
        {
            return HtmlRender("reports/purchase/registerpesanankendaraan.js");
        }

        public string registerbuktipenerimaanunit()
        {
            return HtmlRender("reports/purchase/registerbuktipenerimaanunit.js");
        }

        public string sdmsinvdoattributes()
        {
            return HtmlRender("reports/purchase/sdmsinvdoattributes.js");
        }

        public string registerpengembalianunit()
        {
            return HtmlRender("reports/purchase/registerpengembalianunit.js");
        }

        public string laporanstatistikpembelian()
        {
            return HtmlRender("reports/purchase/laporanstatistikpembelian.js");
        }

        public string registerspkkaroseri()
        {
            return HtmlRender("reports/purchase/registerspkkaroseri.js");
        }

        public string registerpenerimaankaroseri()
        {
            return HtmlRender("reports/purchase/registerpenerimaankaroseri.js");
        }
        #endregion

        #region sales

        public string so()
        {
            return HtmlRender("reports/sales/so.js");
        }

        public string delor()
        {
            return HtmlRender("reports/sales/delor.js");
        }

        public string bpk()
        {
            return HtmlRender("reports/sales/bpk.js");
        }

        public string invoice()
        {
            return HtmlRender("reports/sales/invoice.js");
        }

        public string salesachievementrecord()
        {
            return HtmlRender("reports/sales/salesachievementrecord.js");
        }

        public string registerso()
        {
            return HtmlRender("reports/sales/registerso.js");
        }

        public string registerdo()
        {
            return HtmlRender("reports/sales/registerdo.js");
        }

        public string rgstrbuktikrmkndddrn()
        {
            return HtmlRender("reports/sales/rgstrbuktikrmkndddrn.js");
        }

        public string rgstrfktrpenjunit()
        {
            return HtmlRender("reports/sales/rgstrfktrpenjunit.js");
        }

        public string rgstrreturinvc()
        {
            return HtmlRender("reports/sales/rgstrreturinvc.js");
        }

        public string rgstrbatalso()
        {
            return HtmlRender("reports/sales/rgstrbatalso.js");
        }

        public string rgstrnotadebet()
        {
            return HtmlRender("reports/sales/rgstrnotadebet.js");
        }

        public string rgstrplgnperlsg()
        {
            return HtmlRender("reports/sales/rgstrplgnperlsg.js");
        }

        public string lstrinciankndr()
        {
            return HtmlRender("reports/sales/lstrinciankndr.js");
        }

        public string penjterbaik()
        {
            return HtmlRender("reports/sales/penjterbaik.js");
        }

        public string fktrreqygsdhdgnrt()
        {
            return HtmlRender("reports/sales/fktrreqygsdhdgnrt.js");
        }

        public string rekapharian()
        {
            return HtmlRender("reports/sales/rekapharian.js");
        }

        public string summpermhnfp()
        {
            return HtmlRender("reports/sales/summpermhnfp.js");
        }

        public string rgstrfktrpls()
        {
            return HtmlRender("reports/sales/rgstrfktrpls.js");
        }

        public string rgstrhrnpnrbtfktrpls()
        {
            return HtmlRender("reports/sales/rgstrhrnpnrbtfktrpls.js");
        }

        public string outstandingblnkfp()
        {
            return HtmlRender("reports/sales/outstandingblnkfp.js");
        }

        public string rgstrspk()
        {
            return HtmlRender("reports/sales/rgstrspk.js");
        }

        public string lapfaktur()
        {
            return HtmlRender("reports/sales/lapfaktur.js");
        }

        public string lappenggblnk()
        {
            return HtmlRender("reports/sales/lappenggblnk.js");
        }

        public string custlistslsunit()
        {
            return HtmlRender("reports/sales/custlistslsunit.js");
        }

        public string listbpkb()
        {
            return HtmlRender("reports/sales/listbpkb.js");
        }

        public string fktrpnjpreprinted()
        {
            return HtmlRender("reports/sales/fktrpnjpreprinted.js");
        }

        public string ttdterimabpkb() 
        {
            return HtmlRender("reports/sales/ttdterimabpkb.js");
        }

        public string daftarbpkbperlks() 
        {
            return HtmlRender("reports/sales/daftarbpkbperlks.js");
        }
        public string outstandingspk()
        {
            return HtmlRender("reports/sales/outstandingspk.js");
        }
        public string tdtrmfktrpngjnpngrsnbnn()
        {
            return HtmlRender("reports/sales/tdtrmfktrpngjnpngrsnbnn.js");
        }
        public string dailyslsallbranch()
        {
            return HtmlRender("reports/sales/dailyslsallbranch.js");
        }

        public string dailyslsallbranchv2()
        {
            return HtmlRender("reports/sales/dailyslsallbranchv2.js");
        }

        public string datacustomerprofile()
        {
            return HtmlRender("reports/sales/datacustomerprofile.js");
        }

        #endregion

        #region inventory

        public string perincianstok()
        {
            return HtmlRender("reports/inventory/perincianstok.js");
        }

        public string stokinventory()
        {
            return HtmlRender("reports/inventory/stokinventory.js");
        }

        public string laporanstokkendaraan()
        {
            return HtmlRender("reports/inventory/laporanstokkendaraan.js");
        }

        public string transferkendaraanin()
        {
            return HtmlRender("reports/inventory/transferkendaraanin.js");
        }

        public string transferkendaraanout()
        {
            return HtmlRender("reports/inventory/transferkendaraanout.js");
        }

        public string gantiwarna()
        {
            return HtmlRender("reports/inventory/gantiwarna.js");
        }

        public string dealerstock()
        {
            return HtmlRender("reports/inventory/dealerstock.js");
        }

        public string stokinventoryv2()
        {
            return HtmlRender("reports/inventory/stokinventoryv2.js");
        }

        public string transferkendaraaninmltbranch()
        {
            return HtmlRender("reports/inventory/transferkendaraaninmltbranch.js");
        }

        public string transferkendaraanoutmltbranch()
        {
            return HtmlRender("reports/inventory/transferkendaraanoutmltbranch.js");
        }

        #endregion
        
        public string dokumenpending()
        {
            return HtmlRender("reports/dokumenpending.js");
        }

        public string statistiksalesman()
        {
            return HtmlRender("reports/statistiksalesman.js");
        }

        public string laporanlabarugi()
        {
            return HtmlRender("reports/laporanlabarugi.js");
        }

         public string ReportSalesNStock()
        {
            return HtmlRender("reports/ReportSalesNStock.js");
        }

         public string laporanlabarugiv2()
         {
             return HtmlRender("reports/laporanlabarugiv2.js");
         }

         public string VehicleOwnershipInfo()
         {
             return HtmlRender("reports/vehicleownershipinfo.js");
         }

    }
}
