using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers
{
    public class ReportController : BaseController
    {
        //
        // GET: /Report/

        //Register Booking / Estimasi
        public string RegBooking()
        {
            return HtmlRender("report/RegBooking.js");
        }

        //Register faktur Service
        public string RegFakturService()
        {
            return HtmlRender("report/RegFakturService.js");
        }

        //Register faktur Service hq
        public string RegFakturServiceHQ()
        {
            return HtmlRender("report/RegFakturServiceHQ.js");
        }

        //Register Pekerjaan Luar
        public string RegPekerjaanLuar()
        {
            return HtmlRender("report/RegPekerjaanLuar.js");
        }

        //Register Return Part (Cancel Faktur Service)
        public string RegReturnPart()
        {
            return HtmlRender("report/RegReturnPart.js");
        }

        //Register Return Service
        public string RegReturnServ()
        {
            return HtmlRender("report/RegReturnServ.js");
        }

        //Regiter Surat Perintah Kerja
        public string RegSpk()
        {
            return HtmlRender("report/RegSpkRpt.js");
        }
        
        //Invoice Cancel
        public string CancelInvoice()
        {
            return HtmlRender("report/RptCancelInvoice.js");
        }

        public string RptPDIFSCClaim()
        {
            return HtmlRender("report/RptPdiFscClaim.js");
        }

        public string RptPenjSvcTahunan()
        {
            return HtmlRender("report/RptPenjServTahunan.js");
        }
        
        public string RptPenjSvc()
        {
            return HtmlRender("report/RptPenjualanService.js");
        }

        // Summary Surat Perintah Kerja
        public string SummarySPK()
        {
            return HtmlRender("report/SummarySPK.js");
        }

        //Report Penjualan Service
        public string PenjualanService()
        {
            return HtmlRender("report/RptPenjualanService.js");
        }

        //report Pernjualan Service Perkelompok Pekerjaan
        public string PenjSvcGroupJob()
        {
            return HtmlRender("report/RptPenjSvcGroupJob.js");
        }

        //report penjualan Service Permodel & Pelanggan
        public string PenjSvcModelCust()
        {
            return HtmlRender("report/PenjSvcModelCust.js");
        }

        //report Penjualan service per periode dan Jenis Pekerjaan
        public string PenjSvcPeriodJobType()
        {
            return HtmlRender("report/PenjSvcJobPeriod.js");
        }

        //Report Penjualan Service Tahunan
        public string PenjualanServiceTahunan()
        {
            return HtmlRender("report/RptPenjServTahunan.js");
        }

        //report service tahunan perkelompok pekerjaan
        public string PenjualanGroupJob()
        {
            return HtmlRender("report/PenjualanGroupJob.js");
        }
        
        //report perbandingan penjualan service tahunan
        public string DiffYearlySales()
        {
            return HtmlRender("report/DiffYearlySales.js");
        }

        //report Perincian Intensif
        public string DtlInsentif()
        {
            return HtmlRender("report/dtlinsentif.js");
        }

        //rekapitulasi perincian intensif
        public string RekapDtlIntensif()
        {
            return HtmlRender("report/RekapDtlIntensif.js");
        }

        //laporan service per-mekanik
        public string SvcMecanic()
        {
            return HtmlRender("report/SvcMecanic.js");
        }

        public string HistKendaraan()
        {
            return HtmlRender("report/HistKendaraan.js");
        }

        public string CustomerStatus()
        {
            return HtmlRender("report/CustomerStatus.js");
        }

        public string PendingDocument()
        {
            return HtmlRender("report/PendingDocument.js");
        }

        public string SvRpTrn013()
        {
            return HtmlRender("report/svrptrn013.js");
        }
        public string SvRpTrn014()
        {
            return HtmlRender("report/svrptrn014.js");
        }

        #region Report Service

        public string performaservice()
        {
            return HtmlRender("report/service/performaservice.js");
        }

        public string serviceadvisor()
        {
            return HtmlRender("report/service/serviceadvisor.js");
        }

        public string pdifscdealerclaim()
        {
            return HtmlRender("report/service/pdifscdealerclaim.js");
        }

        public string serviceactivity()
        {
            return HtmlRender("report/service/serviceactivity.js");
        }

        public string fsccamplist()
        {
            return HtmlRender("report/service/fsccamplist.js");
        }

        public string claimdetaillist()
        {
            return HtmlRender("report/claimdetaillist.js");
        }

        #endregion

        public string fktrpenjpreprinted()
        {
            return HtmlRender("report/fktrpenjpreprinted.js");
        }

        public string printdialogretention()
        {
            return HtmlRender("report/printdialogretention.js");
        }

        public string RptWorkshopBodyPaint()
        {
            return HtmlRender("report/RptWorkshopBodyPaint.js");
        }

        public string RegSublet()
        {
            return HtmlRender("report/RegSublet.js");
        }

    }
}
