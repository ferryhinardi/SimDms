using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class ItsController : BaseController
    {
        public string Inqkdpcoupon()
        {
            return HtmlRender("its/Inqkdpcoupon.js");
        }

        public string InqProd()
        {
            return HtmlRender("its/inqprod.js");
        }

        public string InqWithSts()
        {
            return HtmlRender("its/inqwithsts.js");
        }

        public string SummaryExecutive()
        {
            return HtmlRender("its/SummaryExecutive.js");
        }

        public string SummaryExecutive2()
        {
            return HtmlRender("its/SummaryExecutive2.js");
        }

        public string SummaryExecutive3()
        {
            return HtmlRender("its/SummaryExecutive3.js");
        }

        public string SummaryExecutive4()
        {
            return HtmlRender("its/SummaryExecutive4.js");
        }

        public string SummaryExecutive5()
        {
            return HtmlRender("its/SummaryExecutive5.js");
        }

        public string ExecutiveChart1()
        {
            return HtmlRender("its/ExecutiveChart1.js");
        }

        public string ExecutiveChart2()
        {
            return HtmlRender("its/ExecutiveChart2.js");
        }

        public string ExecutiveChart3()
        {
            return HtmlRender("its/ExecutiveChart3.js");
        }

        public string SummaryDataIts()
        {
            return HtmlRender("its/inqexecits.js");
        }

        public string GenerateITS()
        {
            return HtmlRender("its/GenerateITS.js");
        }

        public string MonitoringProductivity()
        {
            return HtmlRender("its/MonitoringProductivity.js");
        }

        public string GenerateITSWithStatusAndTestDrive()
        {
            return HtmlRender("its/inqItsWithStatusAndTestDrive.js");
        }

        #region Slide, Presenter

        public string SlideChart()
        {
            return HtmlRender("its/SlideChart.js");
        }

       
        #endregion

        public string InqITSIndent()
        {
            return HtmlRender("its/inqItsIndent.js");
        }

        public string InqItsByTestDrive()
        {
            return HtmlRender("its/InqItsByTestDrive.js");
        }

        public string InqItsByLeadTime()
        {
            return HtmlRender("its/InqItsByLeadTime.js");
        }

        public string InqItsByLostCase()
        {
            return HtmlRender("its/InqItsByLostCase.js");

        }

        public string InqItsByPerolehanData()
        {
            return HtmlRender("its/InqItsByPerolehanData.js");
        }

        public string InqItsReportInqHarianDealer()
        {
            return HtmlRender("its/inqItsReportFreshInqHarianDealer.js");
        }

        public string inqItsReportAccumByPeriode()
        {
            return HtmlRender("its/inqItsReportAccumByPeriode.js");
        }

        public string RekapInqSpkFak()
        {
            return HtmlRender("its/RekapInqSpkFak.js");
        }

        public string InqSalesProspect()
        {
            return HtmlRender("its/inqSalesProspect.js");
        }

        public string InqGenerateSalesPersonContribution()
        {
            return HtmlRender("its/InqGenerateSalesPersonContribution.js");
        }

        public string inqItsReportSPKWorkDay()
        {
            return HtmlRender("its/inqItsReportSPKWorkDay.js");
        }

        public string GenerateITSToExcel()
        {
            return HtmlRender("its/GenerateITSToExcel.js");
        }
    }
}
