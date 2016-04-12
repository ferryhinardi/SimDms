using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("SpTrnSPickingDtl")]
    public class SpTrnSPickingDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PickingSlipNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string WarehouseCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string PartNoOriginal { get; set; }
        [Key]
        [Column(Order = 7)]
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string LocationCode { get; set; }
        public decimal? QtyOrder { get; set; }
        public decimal? QtySupply { get; set; }
        public decimal? QtyPicked { get; set; }
        public decimal? QtyBill { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? SalesAmt { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? NetSalesAmt { get; set; }
        public decimal? TotSalesAmt { get; set; }
        public string MovingCode { get; set; }
        public string ABCClass { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string ExPickingSlipNo { get; set; }
        public DateTime? ExPickingSlipDate { get; set; }
        public bool? isClosed { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}
