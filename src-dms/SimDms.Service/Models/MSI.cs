using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class MSI
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public decimal? PeriodYear { get; set; }
        public int? SeqNo { get; set; }
        public string MsiGroup { get; set; }
        public string MsiDesc { get; set; }
        public string Unit { get; set; }
        public decimal? Average { get; set; }
        public decimal? Total { get; set; }
        public decimal? Jan { get; set; }
        public decimal? Feb { get; set; }
        public decimal? Mar { get; set; }
        public decimal? Apr { get; set; }
        public decimal? May { get; set; }
        public decimal? Jun { get; set; }
        public decimal? Jul { get; set; }
        public decimal? Aug { get; set; }
        public decimal? Sep { get; set; }
        public decimal? Oct { get; set; }
        public decimal? Nov { get; set; }
        public decimal? Dec { get; set; }
    }
}