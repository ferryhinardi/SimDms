using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    /// <summary>
    /// Table SpTrnPSuggorAOS
    /// </summary>
    [Table("SpTrnPSuggorAOS")]
    public class SpTrnPSuggorAOS
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 51)]
        public DateTime ProcessDate { get; set; }
        [Key]
        [Column(Order = 3)]
        public string NewPartNo { get; set; }
        public string MovingCode { get; set; }
        public string ABCClass { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string TypeOfGoods { get; set; }
        public string SupplierCode { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DmnQty12 { get; set; }
        public decimal? DmnQty11 { get; set; }
        public decimal? DmnQty10 { get; set; }
        public decimal? DmnQty09 { get; set; }
        public decimal? DmnQty08 { get; set; }
        public decimal? DmnQty07 { get; set; }
        public decimal? DmnQty06 { get; set; }
        public decimal? DmnQty05 { get; set; }
        public decimal? DmnQty04 { get; set; }
        public decimal? DmnQty03 { get; set; }
        public decimal? DmnQty02 { get; set; }
        public decimal? DmnQty01 { get; set; }
        public decimal? OnHand { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? InTransit { get; set; }
        public decimal? AllocationSP { get; set; }
        public decimal? AllocationSR { get; set; }
        public decimal? AllocationSL { get; set; }
        public decimal? BackOrderSP { get; set; }
        public decimal? BackOrderSR { get; set; }
        public decimal? BackOrderSL { get; set; }
        public decimal? ReservedSP { get; set; }
        public decimal? ReservedSR { get; set; }
        public decimal? ReservedSL { get; set; }
        public decimal? AvailableQty { get; set; }
        public decimal? OrderUnit { get; set; }
        public decimal? DmnQty { get; set; }
        public decimal? DmnAvg { get; set; }
        public decimal? StdDev { get; set; }
        public decimal? DevFac { get; set; }
        public decimal? MaxQty { get; set; }
        public decimal? MinQty { get; set; }
        public decimal? LeadTime { get; set; }
        public decimal? OrderCycle { get; set; }
        public decimal? SafetyStock { get; set; }
        public decimal? OrderPoint { get; set; }
        public decimal? SafetyStokPoint { get; set; }
        public decimal? SuggorQty { get; set; }
        public string Status { get; set; }
    }
}
