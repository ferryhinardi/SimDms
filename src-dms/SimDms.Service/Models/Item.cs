using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("spMstItems")]
    public class Item
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
        public string MovingCode { get; set; }
        public decimal? DemandAverage { get; set; }
        public DateTime? BornDate { get; set; }
        public string ABCClass { get; set; }
        public DateTime? LastDemandDate { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public DateTime? LastSalesDate { get; set; }
        public decimal? BOMInvAmt { get; set; }
        public decimal? BOMInvQty { get; set; }
        public decimal? BOMInvCostPrice { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? InTransit { get; set; }
        public decimal? OnHand { get; set; }
        public decimal? AllocationSP { get; set; }
        public decimal? AllocationSR { get; set; }
        public decimal? AllocationSL { get; set; }
        public decimal? BackOrderSP { get; set; }
        public decimal? BackOrderSR { get; set; }
        public decimal? BackOrderSL { get; set; }
        public decimal? ReservedSP { get; set; }
        public decimal? ReservedSR { get; set; }
        public decimal? ReservedSL { get; set; }
        public decimal? BorrowQty { get; set; }
        public decimal? BorrowedQty { get; set; }
        public decimal? SalesUnit { get; set; }
        public decimal? OrderUnit { get; set; }
        public decimal? OrderPointQty { get; set; }
        public decimal? SafetyStockQty { get; set; }
        public decimal? LeadTime { get; set; }
        public decimal? OrderCycle { get; set; }
        public decimal? SafetyStock { get; set; }
        public string Utility1 { get; set; }
        public string Utility2 { get; set; }
        public string Utility3 { get; set; }
        public string Utility4 { get; set; }
        public string TypeOfGoods { get; set; }
        public string Status { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public decimal PurcDiscPct { get; set; }

    }

    [Table("spMstItemLoc")]
    public class ItemLoc
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
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public string LocationSub1 { get; set; }
        public string LocationSub2 { get; set; }
        public string LocationSub3 { get; set; }
        public string LocationSub4 { get; set; }
        public string LocationSub5 { get; set; }
        public string LocationSub6 { get; set; }
        public decimal? BOMInvAmount { get; set; }
        public decimal? BOMInvQty { get; set; }
        public decimal? BOMInvCostPrice { get; set; }
        public decimal? OnHand { get; set; }
        public decimal? AllocationSP { get; set; }
        public decimal? AllocationSR { get; set; }
        public decimal? AllocationSL { get; set; }
        public decimal? BackOrderSP { get; set; }
        public decimal? BackOrderSR { get; set; }
        public decimal? BackOrderSL { get; set; }
        public decimal? ReservedSP { get; set; }
        public decimal? ReservedSR { get; set; }
        public decimal? ReservedSL { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("spMstItemInfo")]
    public class ItemInfo
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PartNo { get; set; }
        public string SupplierCode { get; set; }
        public string PartName { get; set; }
        public bool? IsGenuinePart { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? SalesUnit { get; set; }
        public decimal? OrderUnit { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string UOMCode { get; set; }
        public string Status { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("spMstItemPrice")]
    public class ItemPrice
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

    [Table("spMstItemModel")]
    public class ItemModel
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ModelCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }
}