using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers
{
    public class TransController : BaseController
    {
        public string Spk()
        {
            return HtmlRender("trans/spk.js");
        }

        public string Spk2()
        {
            return HtmlRender("trans/spk2.js");
        }

        public string SpkAdmin()
        {
            return HtmlRender("trans/spkadmin.js");
        }

        public string SpkPrint()
        {
            return HtmlRender("trans/spkprint.js");
        }

        public string SpkPrintEstimasi()
        {
            return HtmlRender("trans/spkprintestimasi.js");
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

        public string SubConPrint()
        {
            return HtmlRender("trans/subconprint.js");
        }

        public string SubConRcv()
        {
            return HtmlRender("trans/subconrcv.js");
        }

        public string SubConRcvPrint()
        {
            return HtmlRender("trans/subconrcvprint.js");
        }

        public string GenFPJ()
        {
            return HtmlRender("trans/genfpj.js");
        }

        public string GenFPJHQ()
        {
            return HtmlRender("trans/genfpjhq.js");
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

        public string GenKsg()
        {
            return HtmlRender("trans/genksg.js");
        }

        public string GenKsgSpk()
        {
            return HtmlRender("trans/genksgspk.js");
        }

        public string MainKSG()
        {
            return HtmlRender("trans/mainksg.js");
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

        public string InputInfoClaim()
        {
            return HtmlRender("trans/inputinfoclaimspk.js");
        }

        public string InputInfoClaimInv()
        {
            return HtmlRender("trans/inputinfoclaiminv.js");
        }

        public string UploadClaim()
        {
            return HtmlRender("trans/uploadclaim.js");
        }
            
        public string GenerateClaim()
        {
            return HtmlRender("trans/generatewarantyclaim.js");
        }

        public string ReceiveClaim()
        {
            return HtmlRender("trans/receiveclaim.js");
        }

        public string MaintainWarrantyClaim()
        {
            return HtmlRender("trans/maintain_claim.js");
        }

        public string CancelInvoice()
        {
            return HtmlRender("trans/cancelinvoice.js");
        }

        public string inputvor()
        {
            return HtmlRender("trans/inputvor.js");
        }

        public string InputApproval()
        {
            return HtmlRender("trans/inputapprovalpdifsc.js");
        }
    }
}
