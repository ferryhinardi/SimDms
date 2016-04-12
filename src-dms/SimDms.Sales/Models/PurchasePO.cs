using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omTrPurchasePO")]
    public class OmTrPurchasePO
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string SupplierCode { get; set; }
        public string BillTo { get; set; }
        public string ShipTo { get; set; }
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

    [Table("omTrPurchasePOModel")]
    public class OmTrPurchasePOModel
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal SalesModelYear { get; set; }
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
        public decimal? PPnBMPaid { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPn { get; set; }
        public decimal? QuantityPO { get; set; }
        public decimal? QuantityBPU { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("omMstPricelistBuy")]
    public class OmMstPricelistBuy
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SupplierCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal SalesModelYear { get; set; }
        public decimal? PPnBMPaid { get; set; }
        public decimal? DPP { get; set; }
        public decimal? PPn { get; set; }
        public decimal? PPnBM { get; set; }
        public decimal? Total { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
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

    [Table("omTrPurchasePOModelColour")]
    public class OmTrPurchasePOModelColour
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal SalesModelYear { get; set; }
        [Key]
        [Column(Order = 6)]
        public string ColourCode { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class POBrowse
    {
        public string PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Status { get; set; }
        public string Stat { get; set; }

        public string BillTo { get; set; }
        public string ShipTo { get; set; }
        public string Remark { get; set; }
    }

    public class PODetailModel
    {
        public string PONo { get; set; }
        public string SupplierCode { get; set; }
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
    }

    public class PODetailView
    {
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
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
        public decimal? PPnBMPaid { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPn { get; set; }
        public decimal? QuantityPO { get; set; }
        public decimal? QuantityBPU { get; set; }
        public string Remark { get; set; }
    }

    public class POColourView
    {
        
        public string ColourCode { get; set; }
        public string ColourDesc { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
    }

    public class ReffNoBrowse
    {
        public string BatchNo { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string DealerCode { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
    }

    public class OmTrPurchasePOView
    {       
        public string PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string SupplierCode { get; set; }
        public string BillTo { get; set; }
        public string ShipTo { get; set; }
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

    public class InquiryTrPurchasePOView
    {
        public string PONo { get; set; }
        public string PODate { get; set; }
        public string RefferenceNo { get; set; }
        public string RefferenceDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string BillTo { get; set; }
        public string ShipTo { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class OmTrPurchasePOSelect4BPUView
    {
        public string PONo { get; set; }
        public DateTime PODate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime RefferenceDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string BillTo { get; set; }
        public string ShipTo { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class SalesModelCodeBrowse
    {
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
    }

    public class ColourCodeBrowse
    {
        public string ColourCode { get; set; }
        public string ColourDesc { get; set; }
        public string Remark { get; set; }
    }
}