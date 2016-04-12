using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omTrPurchaseHPP")]
    public class omTrPurchaseHPP
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string HPPNo { get; set; }
        public DateTime? HPPDate { get; set; }
        public string PONo { get; set; }
        public string SupplierCode { get; set; }
        public string BillTo { get; set; }
        public string RefferenceInvoiceNo { get; set; }
        public DateTime? RefferenceInvoiceDate { get; set; }
        public string RefferenceFakturPajakNo { get; set; }
        public DateTime? RefferenceFakturPajakDate { get; set; }
        public DateTime? DueDate { get; set; }
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

    [Table("omTrPurchaseHPPDetail")]
    public class omTrPurchaseHPPDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string HPPNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BPUNo { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("omTrPurchaseHPPDetailModel")]
    public class omTrPurchaseHPPDetailModel
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string HPPNo { get; set; }
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
        public decimal? PPnBMPaid { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPn { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("omTrPurchaseHPPDetailModelOthers")]
    public class omTrPurchaseHPPDetailModelOthers
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string HPPNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BPUNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SalesModelYear { get; set; }
        [Key]
        [Column(Order = 7)]
        public string OthersCode { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPN { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("omTrPurchaseHPPSubDetail")]
    public class omTrPurchaseHPPSubDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string HPPNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BPUNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal HPPSeq { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string Remark { get; set; }
        public bool? isReturn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class HPPView
    {
        public string HPPNo { get; set; }
        public DateTime? HPPDate { get; set; }
        public string PONo { get; set; }
        public string SupplierCode { get; set; }
        public string BillTo { get; set; }
        public string RefferenceInvoiceNo { get; set; }
        public DateTime? RefferenceInvoiceDate { get; set; }
        public string RefferenceFakturPajakNo { get; set; }
        public DateTime? RefferenceFakturPajakDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string SupplierName { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    public class InquiryTrPurchaseHPPView
    {
        public string HPPNo { get; set; }
        public string HPPDate { get; set; }
        public string PONo { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string BillTo { get; set; }
        public string RefferenceInvoiceNo { get; set; }
        public string RefferenceInvoiceDate { get; set; }
        public string RefferenceFakturPajakNo { get; set; }
        public string RefferenceFakturPajakDate { get; set; }
        public string DueDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class HPPDetailModel
    {
        public string PONo { get; set; }
        public string HPPNo { get; set; }
        public string BPUNo { get; set; }
        public string RefferenceDONo { get; set; }
        public string RefferenceSJNo { get; set; }
        public string Remark { get; set; }
        public string SalesModelCode { get; set; }
    }

    public class ReffInvView
    {
        public string BatchNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string FakturPajakNo { get; set; }
        public DateTime? FakturPajakDate { get; set; }
        public string PONo { get; set; }
        public string SupplierCode { get; set; }
        public string BillTo { get; set; }
        public DateTime? DueDate { get; set; }
        public string SupplierName { get; set; }
        public string RefferenceNo { get; set; }
        public string Remark { get; set; } 
    }

    public class SalesModelView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string HPPNo { get; set; }
        public string BPUNo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscPPnBM { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? PPnBMPaid { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPn { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
    public class OmSalesModelView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SONo { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; } 
        public decimal SalesModelYear { get; set; }
        public string ChassisCode { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public decimal? BeforeDiscPPn { get; set; }
        public decimal? BeforeDiscPPnBM { get; set; }
        public decimal? BeforeDiscTotal { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? DiscIncludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscPPnBM { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPn { get; set; }
        public decimal? QuantitySO { get; set; }
        public decimal? QuantityDO { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public decimal? ShipAmt { get; set; }
        public decimal? DepositAmt { get; set; }
        public decimal? OthersAmt { get; set; }
    }
    public class SubSalesModelView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string HPPNo { get; set; }
        public string BPUNo { get; set; }
        public decimal HPPSeq { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string Remark { get; set; }
        public bool? isReturn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public decimal BPUSeq { get; set; } 
    }
}