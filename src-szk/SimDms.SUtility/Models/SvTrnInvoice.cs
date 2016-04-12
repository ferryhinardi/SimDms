using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.SUtility.Models
{
    [Table("SvTrnInvoice")]
    public class SvTrnInvoice
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
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceStatus { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string JobType { get; set; }
        public string ServiceRequestDesc { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public decimal? Odometer { get; set; }
        public bool? IsPKP { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? SignedDate { get; set; }
        public decimal? LaborDiscPct { get; set; }
        public decimal? PartsDiscPct { get; set; }
        public decimal? MaterialDiscPct { get; set; }
        public decimal? PphPct { get; set; }
        public decimal? PpnPct { get; set; }
        public decimal? LaborGrossAmt { get; set; }
        public decimal? PartsGrossAmt { get; set; }
        public decimal? MaterialGrossAmt { get; set; }
        public decimal? LaborDiscAmt { get; set; }
        public decimal? PartsDiscAmt { get; set; }
        public decimal? MaterialDiscAmt { get; set; }
        public decimal? LaborDppAmt { get; set; }
        public decimal? PartsDppAmt { get; set; }
        public decimal? MaterialDppAmt { get; set; }
        public decimal? TotalDppAmt { get; set; }
        public decimal? TotalPphAmt { get; set; }
        public decimal? TotalPpnAmt { get; set; }
        public decimal? TotalSrvAmt { get; set; }
        public string Remarks { get; set; }
        public decimal? PrintSeq { get; set; }
        public char PostingFlag { get; set; }
        public DateTime? PostingDate { get; set; }
        public bool? IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }
}