using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.CStatisfication.Models
{
    [Table("CsCustBpkb")]
    public class CsCustBpkb
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Chassis { get; set; }
        public DateTime? BpkbReadyDate { get; set; }
        public DateTime? BpkbPickUp { get; set; }
        public bool ReqInfoLeasing { get; set; }
        public bool ReqInfoCust { get; set; }
        public bool ReqKtp { get; set; }
        public bool ReqStnk { get; set; }
        public bool ReqSuratKuasa { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public DateTime? FinishDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }

    [Table("CsLkuBpkbReminderView")]
    public class CsLkuBpkbReminderView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
 
        public string Chassis { get; set; }
        public string Engine { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public DateTime? BpkbReadyDate { get; set; }
        public DateTime? BpkbPickUp { get; set; }
        public DateTime? BpkbDate { get; set; }
        public DateTime? StnkDate { get; set; }
        public DateTime? BPKDate { get; set; }
        public int? Status { get; set; }
        public bool? IsLeasing { get; set; }
        public string Category { get; set; }
        public string LeasingCo { get; set; }
        public string LeasingName { get; set; }
        public decimal? Installment { get; set; }
        public string PoliceRegNo { get; set; }
        public string Outstanding { get; set; }
        public DateTime? DelayedRetrievalDate { get; set; }
        public string DelayedRetrievalNote { get; set; }
    }

    [Table("CsBpkbRetrievalInformation")]
    public class CsBpkbRetrievalInformation
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public DateTime? RetrievalEstimationDate { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}