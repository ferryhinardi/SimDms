using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TracerX;

namespace SimDms.Sparepart.Controllers
{
    public class MasterController : BaseController
    {
        public MasterController()
        {
            Thread.CurrentThread.Name = "SimDms.Sparepart.MasterController";
        }

        //Master Item
        public string masteritem()
        {
            MyLogger.Log.Info("Sparepart/Master/MasterItem load");
            return HtmlRender("master/masteritem.js");
        }

        //GL Account Mapping
        public string glaccmapping()
        {
            MyLogger.Log.Info("Sparepart/Master/GLMAPPPING load");
            return HtmlRender("master/glaccmapping.js");
        }

        //Item Konversi
        public string itemkonversi()
        {
            return HtmlRender("master/itemkonversi.js");
        }
        //Item Lokasi
        public string itemlokasi()
        {
            return HtmlRender("master/itemlokasi.js");
        }
        //Item Modification (Subtitution Part)
        public string itemmodification()
        {
            return HtmlRender("master/itemmodification.js");
        }
        //Item Price
        public string itemprice()
        {
            return HtmlRender("master/itemprice.js");
        }
        //Master Company Account
        public string companyacc()
        {
            return HtmlRender("master/companyacc.js");
        }
        //Moving Code
        public string movingcode()
        {
            return HtmlRender("master/movingcode.js");
        }
        //Parameter Order
        public string parameterorder()
        {
            return HtmlRender("master/parameterorder.js");
        }
        //Part Campaign utk Pembelian
        public string campaignpembelian()
        {
            return HtmlRender("master/campaignpembelian.js");
        }
        //Part Campaign utk Penjualan
        public string campaignpenjualan()
        {
            return HtmlRender("master/campaignpenjualan.js");
        }
        //Target Penjualan
        public string targetpenjualan()
        {
            return HtmlRender("master/targetpenjualan.js");
        }

        public string tmpl3()
        {
            return HtmlRender("master/tmpl3.js");
        }

        public string testexport()
        {
            return HtmlRender("master/testexport.js");
        }

        public string test1()
        {
            return HtmlRender("master/test1.js");
        }

        public string aosalert()
        {
            return HtmlRender("master/aosalert.js");
        }

        public JsonResult cors(string viewid)
        {
            return  Json ( 
                new { success=true,
                      file = string.Format("{0}{1}.js", ("http://localhost:9116/assets/js/app/sp/"), viewid)
            });
        }
    }
}
