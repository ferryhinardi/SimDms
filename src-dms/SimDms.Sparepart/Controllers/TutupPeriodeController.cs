using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers
{
    public class TutupPeriodexController:BaseController
    {
        // Proses Tutup Bulan
        public string lnk7001()
        {
            return HtmlRender("tutupperiode/ProsesTutupBulan.js");
        }

 


    }

}
