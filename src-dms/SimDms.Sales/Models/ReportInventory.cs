using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    public class ReportInvetoryLookUp
    {
        public string Code { get; set; }
        public string Desc { get; set; }
    }

    public class GetPeriod
    {
        public string Code { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public DateTime PeriodBeg { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    public class PeriodePostingJurnal
    {
        public string Periode { get; set; }
        public string FiscalYear { get; set; }
        public Decimal PeriodeNum { get; set; }
        public Decimal FiscalMonth { get; set; }
        public string PeriodeName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StatusSP { get; set; }
        public string StatusSL { get; set; }
        public string StatusSV { get; set; }
        public string StatusAP { get; set; }
        public string StatusAR { get; set; }
        public string StatusGL { get; set; }
        public string StatusFiscal { get; set; }
    }
}