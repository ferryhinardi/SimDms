using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers
{
    public class TransController : BaseController
    {
        public string Spk()
        {
            return HtmlRender("trans/spk.js");
        }

        public string Mekanik()
        {
            return HtmlRender("trans/mekanik.js");
        }

        public string Invoice()
        {
            return HtmlRender("trans/invoice.js");
        }

        public string SubCon()
        {
            return HtmlRender("trans/subcon.js");
        }

        public string SubConRcv()
        {
            return HtmlRender("trans/subconrcv.js");
        }

        public string GenFPJ()
        {
            return HtmlRender("trans/genfpj.js");
        }

        public string MaintainFPJ()
        {
            return HtmlRender("trans/maintainfpj.js");
        }

        public string GenFPJStd()
        {
            return HtmlRender("trans/genfpjstd.js");
        }

        public string MaintainInv()
        {
            return HtmlRender("trans/maintaininv.js");
        }

        public string Retur()
        {
            return HtmlRender("trans/return.js");
        }

        public string MaintainSPK() 
        {
            return HtmlRender("trans/maintainspk.js");
        }

        public string ViewJobOrder()
        {
            return HtmlRender("trans/viewjoborder.js");
        }

        public string InvoiceBatch()
        {
            return HtmlRender("trans/invoicebatch.js");
        }

        public string InputKSG()
        {
            return HtmlRender("trans/inputksg.js");
        }

        public string KsgSpk()
        {
            return HtmlRender("trans/ksgspk.js");
        }

        public string KsgInv()
        {
            return HtmlRender("trans/ksginv.js");
        }

        public string UploadKsg()
        {
            return HtmlRender("trans/uploadksg.js");
        }

        public string GetWarrantyClaim()
        {
            return HtmlRender("trans/getwarrantyclaim.js");
        }

        public string GetPackage()
        {
            return HtmlRender("trans/getpackage.js");
        }

        public string InputWarrantyClaim()
        {
            return HtmlRender("trans/inputwarrantyclaim.js");
        }

        public string GenPackage()
        {
            return HtmlRender("trans/genpackage.js");
        }
    }
}
