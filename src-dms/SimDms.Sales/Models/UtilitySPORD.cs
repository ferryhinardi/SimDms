using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("OmUtlSPORDHdr")]
    public class OmUtlSPORDHdr
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

    [Table("OmUtlSPORDDtl1")]
    public class OmUtlSPORDDtl1
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
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Status { get; set; }
    }

    [Table("OmUtlSPORDDtl2")]
    public class OmUtlSPORDDtl2
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
        public string SKPNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SalesModelYear { get; set; }
        public decimal? BeforeDiscountDPP { get; set; }
        public decimal? BeforeDiscountPPN { get; set; }
        public decimal? BeforeDiscountPPNBM { get; set; }
        public decimal? BeforeDiscountTotal { get; set; }
        public decimal? DiscountExcludePPN { get; set; }
        public decimal? DiscountIncludePPN { get; set; }
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

    [Table("OmUtlSPORDDtl3")]
    public class OmUtlSPORDDtl3
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
        public string SKPNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SalesModelYear { get; set; }
        [Key]
        [Column(Order = 7)]
        public string ColourCode { get; set; }
        public decimal? Quantity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class OutstandingAccount
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string CompanyName { get; set; }
        public string SalesModelCode { get; set; }
        public int? IsSalesAccNo { get; set; }
        public int? IsDiscountAccNo { get; set; }
        public int? IsReturnAccNo { get; set; }
        public int? IsCOGSAccNo { get; set; }
        public int? IsInventoryAccNo { get; set; }
        public int? IsSalesAccNoAks { get; set; }
        public int? IsReturnAccNoAks { get; set; }
        public int? IsBBNAccNo { get; set; }
        public int? IsKIRAccNo { get; set; }
        public int? IsHReturnAccNo { get; set; }
        public int? IsPReturnAccNo { get; set; }
        public int? IsDiscountAccNoAks { get; set; }
        public int? IsShipAccNo { get; set; }
        public int? IsDepositAccNo { get; set; }
        public int? IsOthersAccNo { get; set; }
        public int? IsInTransitAccNo { get; set; }
        public string SalesModelDesc { get; set; }
    }
}