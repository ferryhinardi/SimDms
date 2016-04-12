using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omTrPurchaseReturn")]
    public class omTrPurchaseReturn
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReturnNo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string HPPNo { get; set; }
        public string RefferenceFakturPajakNo { get; set; }
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

    [Table("omTrPurchaseReturnDetail")]
    public class omTrPurchaseReturnDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReturnNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BPUNo { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("omTrPurchaseReturnDetailModel")]
    public class omTrPurchaseReturnDetailModel
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReturnNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BPUNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SalesModelYear { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscPPnBM { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("omTrPurchaseReturnSubDetail")]
    public class omTrPurchaseReturnSubDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReturnNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BPUNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ReturnSeq { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class inquiryTrPurchaseReturnView
    {
        public string ReturnNo { get; set; }
        public string ReturnDate { get; set; }
        public string RefferenceNo { get; set; }
        public string RefferenceDate { get; set; }
        public string HPPNo { get; set; }
        public string RefferenceFakturPajakNo { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class ReturnDetailModel
    {
        public string ReturnNo { get; set; }
        public string BPUNo { get; set; }
        public string SalesModelCode { get; set; }
    }

    public class ReturnView
    {
        public string ReturnNo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string HPPNo { get; set; }
        public DateTime? HPPDate { get; set; }
        public string RefferenceFakturPajakNo { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Stat { get; set; }
    }

    public class ReturnSubDetail
    {
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string HPPSeq { get; set; }
    }

}