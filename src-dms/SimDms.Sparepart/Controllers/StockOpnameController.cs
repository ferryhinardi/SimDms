using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers
{
    public class StockOpnameController : BaseController
    {
        // Proses Inventory Form/Tag
        public string lnk6001()
        {
            return HtmlRender("stockopname/ProsesInventoryFormTag.js");
        }

        // Proses Blank Inventory Form/Tag
        public string lnk6002()
        {
            return HtmlRender("stockopname/ProsesBlankInventoryFormTag.js");
        }


        // Entry Inventory Form/Tag
        public string lnk6003()
        {
            return HtmlRender("stockopname/EntryInventoryFormTag.js");
        }


        // Pembatalan Blank Inventory Form/Tag
        public string lnk6004()
        {
            return HtmlRender("stockopname/PembatalanBlankInventoryFormTag.js");
        }


        // Cetak Analisa Stock Taking
        public string lnk6005()
        {
            return HtmlRender("stockopname/CetakAnalisaStockTaking.js");
        }
        
        // Posting Data Stock Taking to Master
        public string lnk6006()
        {
            return HtmlRender("stockopname/PostingDataStockTakingtoMaster.js");
        }        

    }

}
