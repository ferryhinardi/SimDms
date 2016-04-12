using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("SpMstItems")]
    public class SpMstItem
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
}