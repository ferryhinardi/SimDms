using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Tax.Models
{
    [Table("GnTaxPPN")]
    public class GnTaxPPN
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal PeriodYear { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal PeriodMonth { get; set; }
        [Key]
        [Column(Order = 6)]
        public string ProfitCenter { get; set; }
        [Key]
        [Column(Order = 7)]
        public string TaxType { get; set; }
        public decimal? DPPStd { get; set; }
        public decimal? DPPSdh { get; set; }
        public decimal? PPNStd { get; set; }
        public decimal? PPNSdh { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }
}