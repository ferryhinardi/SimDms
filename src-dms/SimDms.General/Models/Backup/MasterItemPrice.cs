using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Sparepart.Models
{
    [Table("spMstItemPrice")]
    public class spMstItemPrice
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PartNo { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? OldRetailPrice { get; set; }
        public decimal? OldPurchasePrice { get; set; }
        public decimal? OldCostPrice { get; set; }
        public DateTime? LastPurchaseUpdate { get; set; }
        public DateTime? LastRetailPriceUpdate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }


    public class SpItemPriceView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string SupplierCode { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public string IsGenuinePart { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CategoryName { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public DateTime? LastPurchaseUpdate { get; set; }
        public DateTime? LastRetailPriceUpdate { get; set; }
        public decimal? OldCostPrice { get; set; }
        public decimal? OldPurchasePrice { get; set; }
        public decimal? OldRetailPrice { get; set; }
    }

    [Table("spHstItemPrice")]
    public class spHstItemPrice
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public DateTime UpdateDate { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? OldRetailPrice { get; set; }
        public decimal? OldPurchasePrice { get; set; }
        public decimal? OldCostPirce { get; set; }
        public decimal? OldDiscount { get; set; }
        public DateTime? LastPurchaseUpdate { get; set; }
        public DateTime? LastRetailPriceUpdate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string StatusMaintain { get; set; }
    }

    public class GetLatestRecord
    {
        public String CompanyCode { get; set; }
        public String BranchCode { get; set; }
        public String PartNo { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Decimal? RetailPrice { get; set; }
        public Decimal? RetailPriceInclTax { get; set; }
        public Decimal? PurchasePrice { get; set; }
        public Decimal? OldRetailPrice { get; set; }
        public Decimal? OldPurchasePrice { get; set; }
        public Decimal? Discount { get; set; }
        public Decimal? OldDiscount { get; set; }
        public Decimal? CostPrice { get; set; }
        public Decimal? OldCostPirce { get; set; }
        public String CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

 

}