using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("OmUtlSHPOKHdr")]
    public class OmUtlSHPOKHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string RcvDealerCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string BatchNo { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("OmUtlSHPOKDtl1")]
    public class OmUtlSHPOKDtl1
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BatchNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string SKPNo { get; set; }
        public string FakturPajakNo { get; set; }
        public DateTime? FakturPajakDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

    [Table("OmUtlSHPOKDtl2")]
    public class OmUtlSHPOKDtl2
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BatchNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string InvoiceNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string DocumentNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string DocumentType { get; set; }
    }

    [Table("OmUtlSHPOKDtl3")]
    public class OmUtlSHPOKDtl3
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BatchNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string InvoiceNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string DocumentNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 7)]
        public decimal SalesModelYear { get; set; }
        public decimal? BeforeDiscountDPP { get; set; }
        public decimal? DiscountExcludePPN { get; set; }
        public decimal? AfterDiscountDPP { get; set; }
        public decimal? AfterDiscountPPN { get; set; }
        public decimal? AfterDiscountPPNBM { get; set; }
        public decimal? AfterDiscountTotal { get; set; }
        public decimal? PPNBMPaid { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPN { get; set; }
        public decimal? Quantity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("OmUtlSHPOKDtl4")]
    public class OmUtlSHPOKDtl4
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BatchNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string InvoiceNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string DocumentNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 7)]
        public decimal SalesModelYear { get; set; }
        [Key]
        [Column(Order = 8)]
        public string ColourCode { get; set; }
        [Key]
        [Column(Order = 9)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 10)]
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("OmUtlSHPOKDtlO")]
    public class OmUtlSHPOKDtlO
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BatchNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string InvoiceNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string DocumentNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 7)]
        public decimal SalesModelYear { get; set; }
        [Key]
        [Column(Order = 8)]
        public string OthersCode { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPN { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}