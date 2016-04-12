using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers
{
    public class UtilityController:BaseController
    {
        //  Upload File
        public string lnk9001()
        {
            return HtmlRender("utility/UploadFile.js");
        }
        //  Switch Type Part User
        public string lnk9002()
        {
            return HtmlRender("utility/SwitchTypePartUser.js");
        }

        //  Send File
        public string lnk9003()
        {
            return HtmlRender("utility/SendFile.js");
        }

        //  Reset Stock Opname
        public string lnk9004()
        {
            return HtmlRender("utility/ResetStockOpname.js");
        }

        //  Upload From DCS
        public string lnk9005()
        {
            return HtmlRender("utility/UploadFromDCS.js");
        }
        //  Maintain Harga Pokok
        public string lnk9006()
        {
            return HtmlRender("utility/MaintainHargaPokok.js");
        }
        //  Maintain Type Part
        public string lnk9007()
        {
            return HtmlRender("utility/MaintainTypePart.js");
        }
        //  Upload Data Delear (Sparepart)
        public string lnk9008()
        {
            return HtmlRender("utility/UploadDataDelearSparepart.js");
        }

        //  Generate AOS Item Sparepart List
        public string lnk9009()
        {
            return HtmlRender("utility/aositemsplist.js");
        }

        // Setup Master Part
        public string lnk9010()
        {
            return HtmlRender("utility/SetupMasterPart.js");
        }

    }

}
