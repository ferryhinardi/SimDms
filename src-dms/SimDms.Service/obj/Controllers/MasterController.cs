using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers
{
    public class MasterController : BaseController
    {
        //referensi service
        public string Ref()
        {
            return HtmlRender("master/referensi_service.js");
        
        }
        public string reffsrvprint()
        {
            return HtmlRender("master/reffsrvprint.js");
        }
        //tipe pembayaran
        public string tipemb()
        {
            return HtmlRender("master/tipe_pembayaran.js");
        }
        //stall
        public string stall()
        {
            return HtmlRender("master/stall.js");
        }
        //waktu kerja
        public string wkt()
        {
            return HtmlRender("master/waktu.js");
        }
        //kehadiran mekanik
        public string absen()
        {
            return HtmlRender("master/kehadiran_mekanik.js");
        }
        //garansi
        public string garansi()
        {
            return HtmlRender("master/garansi.js");
        }
        public string warrantyprint()
        {
            return HtmlRender("master/warrantyprint.js");
        }
        //campaign
        public string campaign()
        {
            return HtmlRender("master/campaign.js");
        }
        //campaign
        public string campaignprint()
        {
            return HtmlRender("master/campaignprint.js");
        }
        //Tarif Kupon Service Gratis
        public string ksg()
        {
            return HtmlRender("master/Ksg.js");
        }
        //tarif jasa
        public string tafja()
        {
            return HtmlRender("master/tarif_jasa.js");
        }
        //pekerjaan
        public string kerja()
        {
            return HtmlRender("master/pekerjaan.js");
        }
        //print pekerjaan
        public string jobprint()
        {
            return HtmlRender("master/jobprint.js");
        }
        //kendaraan dan pelanggan
        public string kdanp()
        {
            return HtmlRender("master/customer_vehicle.js");
        }
        //kontrak service
        public string konserv()
        {
            return HtmlRender("master/kontrak_service.js");
        }
        //klub
        public string klub()
        {
            return HtmlRender("master/klub.js");
        }
        //event
        public string evn()
        {
            return HtmlRender("master/event.js");
        }
        //paket service
        public string pakserv()
        {
            return HtmlRender("master/paket_service.js");
        }
        //pendaftaran paket service
        public string pps()
        {
            return HtmlRender("master/pend_paket_service.js");
        }
        //target
        public string target()
        {
            return HtmlRender("master/target.js");
        }
        //account
        public string acc()
        {
            return HtmlRender("master/account.js");
        }
        //pendaftaran campaign 
        public string pcampaign()
        {
            return HtmlRender("master/pendaftaran_campaign.js");
        }
        //history Vehicle 
        public string historyVehicle()
        {
            return HtmlRender("master/historyVehicle.js");
        }
        //LookUp Service Book 
        public string LookUpServiceBook()
        {
            return HtmlRender("master/LookUpServiceBook.js");
        }
        //Print Customer Vehicle
        public string PrintCustomerVehicle()
        {
            return HtmlRender("master/print_customer_vehicle.js");
        }
        //Print ksg
        public string PrintKsg()
        {
            return HtmlRender("master/print_ksg.js");
        }
        //Print Kontrak Service
        public string PrintKontrakService()
        {
            return HtmlRender("master/print_kontrak_service.js");
        }
        //Print Event
        public string PrintEvent()
        {
            return HtmlRender("master/print_event.js");
        }
        //Print Club
        public string PrintKlub()
        {
            return HtmlRender("master/print_klub.js");
        }
        
    }
}
