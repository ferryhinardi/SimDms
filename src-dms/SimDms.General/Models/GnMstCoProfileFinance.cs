using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.General.Models
{
    [Table("gnMstCoProfileFinance")]
    public class GnMstCoProfileFinance
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public decimal? FiscalYear { get; set; }
        public decimal? FiscalMonth { get; set; }
        public decimal? FiscalPeriod { get; set; }
        public DateTime? PeriodBeg { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public decimal? FiscalYearAR { get; set; }
        public decimal? FiscalMonthAR { get; set; }
        public decimal? FiscalPeriodAR { get; set; }
        public DateTime? PeriodBegAR { get; set; }
        public DateTime? PeriodEndAR { get; set; }
        public decimal? FiscalYearGL { get; set; }
        public decimal? FiscalMonthGL { get; set; }
        public decimal? FiscalPeriodGL { get; set; }
        public DateTime? PeriodBegGL { get; set; }
        public DateTime? PeriodEndGL { get; set; }
        public DateTime? TransDateAP { get; set; }
        public DateTime? TransDateAR { get; set; }
        public DateTime? TransDateGL { get; set; }
    }
}