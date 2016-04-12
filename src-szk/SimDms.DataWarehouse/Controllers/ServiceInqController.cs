using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class ServiceInqController : BaseController
    {
        public string InqData()
        {
            return HtmlRender("sv/inqdata.js");
        }

        public string InqMsi()
        {
            return HtmlRender("sv/inqmsi.js");
        }

        public string InqMsiV2()
        {
            return HtmlRender("sv/inqmsi2.js");
        }

        public string InqMsiR2V2()
        {
            return HtmlRender("sv/inqmsiRduaV2.js");
        }

        public string gnDataMRSR()
        {
            return HtmlRender("sv/gnDataMRSR.js");
        }

        public string InqInv()
        {
            return HtmlRender("sv/inqinv.js");
        }

        public string InqActiveVin()
        {
            return HtmlRender("sv/inqacvtivevin.js");
        }

        public string GenerateUnitIntake()
        {
            return HtmlRender("sv/generate-unit-intake.js");
        }

        public string GenerateUnitIntakeR2()
        {
            return HtmlRender("sv/generate-unit-intakeR2.js");
        }

        public string GenerateDataSubmission()
        {
            return HtmlRender("sv/generate-data-submission.js");
        }


        public string GenerateDataSubmissionR2()
        {
            return HtmlRender("sv/generate-data-submission-R2.js");
        }


        public string GenerateSumUnitIntake()
        {
            return HtmlRender("sv/generate-sum-unit-intake.js");
        }

        public string GenerateSumUnitIntakeR2()
        {
            return HtmlRender("sv/generate-sum-unit-intakeR2.js");
        }

        public string RegisterSpk()
        {
            return HtmlRender("sv/RegisterSpk.js");
        }
        public string InqMsiRdua()
        {
            return HtmlRender("sv/inqmsirdua.js");
        }

        public string ReportVOR()
        {
            return HtmlRender("sv/generate-data/vorReport.js");
        }

        public string ConsistencyReportVOR()
        {
            return HtmlRender("sv/generate-data/ConsistencyReportVOR.js");
        }

        public string HistJobDelayVOR()
        {
            return HtmlRender("sv/generate-data/HistJobDelayVOR.js");
        }

        public string PvtMsi()
        {
            return HtmlRender("sv/pvtmsi.js");
        }

        //Detail Revenue Register SPK
        public string DtlRevenueRegSpk()
        {
            return HtmlRender("sv/generate-data/detailrevenueregspk.js");
        }

        public string GenerateDRH()
        {
            return HtmlRender("sv/generate-data/GenerateDRH.js");
        }

        public string CampaignClaim()
        {
            return HtmlRender("sv/generate-data/CampaignClaim.js");
        }
    }
}
