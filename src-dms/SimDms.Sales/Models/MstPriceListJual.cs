using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("OmMstPricelistSell")]
    public class OmMstPricelistJual
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string GroupPriceCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal? SalesModelYear { get; set; }
        public decimal? TotalMinStaff { get; set; }
        public decimal? TotalMinSpv { get; set; }
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
        public string TaxCode { get; set; }
        public decimal? TaxPct { get; set; }
    }

    public class GroupPriceCodeLookup
    {
        public string RefferenceCode { get; set; }
        public string RefferenceDesc1 { get; set; }
    }

    public class PriceListJualBrowse
    {
        public string GroupPriceCode { get; set; }
        public string GroupPriceName { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public decimal? SalesModelYear { get; set; }
        public decimal? TotalMinStaff { get; set; }
        public decimal? DPP { get; set; }
        public decimal? PPn { get; set; }
        public decimal? PPnBM { get; set; }
        public decimal? Total { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Remark { get; set; }
        public bool? Status { get; set; }
        public string TaxCode { get; set; }
        public decimal? TaxPct { get; set; }
    }
}