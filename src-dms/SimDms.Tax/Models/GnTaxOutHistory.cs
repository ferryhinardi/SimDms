using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Tax.Models
{
    [Table("GnTaxOutHistory")]
    public class GnTaxOutHistory
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
        public decimal? SeqNo { get; set; }
        public string ProfitCenter { get; set; }
        public string TypeOfGoods { get; set; }
        public string TaxCode { get; set; }
        public string TransactionCode { get; set; }
        public string StatusCode { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentType { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public bool? IsPKP { get; set; }
        public string NPWP { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string TaxNo { get; set; }
        public DateTime? TaxDate { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public decimal? DPPAmt { get; set; }
        public decimal? PPNAmt { get; set; }
        public decimal? PPNBmAmt { get; set; }
        public string Description { get; set; }
        public decimal? Quantity { get; set; }
        public bool? IsDeleted { get; set; }
        public string IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public string SKPNo { get; set; }
    }
}