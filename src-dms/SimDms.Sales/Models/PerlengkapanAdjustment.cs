using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omTrPurchasePerlengkapanAdjustment")]
    public class omTrPurchasePerlengkapanAdjustment
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string AdjustmentNo { get; set; }
        public DateTime? AdjustmentDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string PerlengkapanType { get; set; }
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

    [Table("omTrPurchasePerlengkapanAdjustmentDetail")]
    public class omTrPurchasePerlengkapanAdjustmentDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string AdjustmentNo { get; set; }
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

    public class inquiryTrPerlengkapanAdjustmentView
    {
        public string AdjustmentNo { get; set; }
        public string AdjustmentDate { get; set; }
        public string RefferenceNo { get; set; }
        public string RefferenceDate { get; set; }
        public string PerlengkapanType { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class PerlengkapanAdjustmentDetailView
    {
        public string PerlengkapanCode { get; set; }
        public string PerlengkapanName { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
    }

    public class PerlengkapanAdjustmentView
    {
        public string AdjustmentNo { get; set; }
        public DateTime? AdjustmentDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string PerlengkapanCode { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Stat { get; set; }
    }
}