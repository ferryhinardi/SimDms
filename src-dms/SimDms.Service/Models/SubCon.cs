using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svTrnPOSubCon")]
    public class SubCon
    {
        [Key]
        [Column(Order = 0)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 1)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string POStatus { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime JobOrderDate { get; set; }
        public string JobType { get; set; }
        public string BasicModel { get; set; }
        public string RecNo { get; set; }
        public DateTime? RecDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public DateTime DueDate { get; set; }
        public string SupplierCode { get; set; }
        public decimal PODisc { get; set; }
        public decimal? PPhPct { get; set; }
        public decimal? PPnPct { get; set; }
        public decimal? GrossAmt { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? DppAmt { get; set; }
        public decimal? PphAmt { get; set; }
        public decimal? PpnAmt { get; set; }
        public decimal? ServiceAmt { get; set; }
        public string Remarks { get; set; }
        public decimal? PrintSeq { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }

    [Table("svTrnPOSubConView")]
    public class SubConView
    {
        [Key]
        [Column(Order = 0)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 1)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime JobOrderDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public decimal PODisc { get; set; }
        public decimal? ServiceAmt { get; set; }
        public string RefferenceCode { get; set; }
        public string POStatus { get; set; }
        public string Description { get; set; }
    }

    [Table("SvTrnPOSubConRcvView")]
    public class SubConRcvView
    {
        [Key]
        [Column(Order = 0)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 1)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime JobOrderDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public decimal PODisc { get; set; }
        public decimal? ServiceAmt { get; set; }
        public string POStatus { get; set; }
        public string Description { get; set; }
        public string RecNo { get; set; }
        public DateTime? RecDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string FPJGovNo { get; set; }
    }
}