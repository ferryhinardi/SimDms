using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Tax.Models
{
    [Table("gnGenerateTax")]
    public class gnGenerateTax
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public int PeriodTaxYear { get; set; }
        [Key]
        [Column(Order = 4)]
        public int PeriodTaxMonth { get; set; }
        [Key]
        [Column(Order = 5)]
        public string ProfitCenterCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public string FPJGovNo { get; set; }
        public DateTime? FPJGovDate { get; set; }
        [Key]
        [Column(Order = 8)]
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string RefNo { get; set; }
        public DateTime? RefDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    [Table("TxFpjConfig")]
    public class TxFpjConfig
    {
        [Key]
        [Column(Order = 1)]
        public string FpjID { get; set; }
        public string FpjValue { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }

    public class GenerateTaxView 
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int PeriodTaxYear { get; set; }
        public int PeriodTaxMonth { get; set; }
        public string ProfitCenterCode { get; set; }
        public string FPJGovNo { get; set; }
        public DateTime? FPJGovDate { get; set; }
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string RefNo { get; set; }
        public DateTime? RefDate { get; set; }
        public string CustomerName { get; set; } 
        public decimal DPPAmt { get; set; }
        public decimal PPNAmt { get; set; }
        public decimal Total { get; set; }
        public string Address { get; set; }
        public string FpjNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public DateTime? DueDate { get; set; }
        public Int64? FpjSeqNo { get; set; }
    }
}