using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omTrPurchasePerlengkapanIn")]
    public class omTrPurchasePerlengkapanIn
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PerlengkapanNo { get; set; }
        public DateTime? PerlengkapanDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string PerlengkapanType { get; set; }
        public string SourceDoc { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("omTrPurchasePerlengkapanInDetail")]
    public class omTrPurchasePerlengkapanInDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PerlengkapanNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PerlengkapanCode { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class inquiryTrPerlengkapanInView
    {
        public string PerlengkapanNo { get; set; }
        public string PerlengkapanDate { get; set; }
        public string RefferenceNo { get; set; }
        public string RefferenceDate { get; set; }
        public string PerlengkapanType { get; set; }
        public string SourceDoc { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class PerlengkapanInDetailView
    {
        public string PerlengkapanCode { get; set; }
        public string PerlengkapanName { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
    }

    public class PerlengkapanInView
    {
        public string PerlengkapanNo { get; set; }
        public DateTime? PerlengkapanDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string PerlengkapanType { get; set; }
        public string PerlengkapanTypeName { get; set; }
        public string SourceDoc { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Stat { get; set; }
    }

    public class SourceDocBrowse
    {
        public string BPUNo { get; set; }
        public DateTime? BPUDate { get; set; }
        public string PONo { get; set; }
        public string ReffereneSJNo { get; set; }

        public string TransferInNo { get; set; }
        public DateTime? TransferInDate { get; set; }

        public string ReturnNo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string RefferenceNo { get; set; }
    }

    public class ReffEqInNoBrowse
    {
        public string BPPNo { get; set; }
        public string BatchNo { get; set; }
        public DateTime? BPPDate { get; set; }
        public string BPUNo { get; set; }
        public string RefferenceSJNo { get; set; }
    }
}