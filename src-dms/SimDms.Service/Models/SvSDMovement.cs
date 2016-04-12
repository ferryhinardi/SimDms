using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svSDMovement")]
    public class SvSDMovement
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public int PartSeq { get; set; }
        public string WarehouseCode { get; set; }
        public decimal QtyOrder { get; set; }
        public decimal Qty { get; set; }
        public decimal DiscPct { get; set; }
        public decimal CostPrice { get; set; }
        public decimal RetailPrice { get; set; }
        public string TypeOfGoods { get; set; }
        public string CompanyMD { get; set; }
        public string BranchMD { get; set; }
        public string WarehouseMD { get; set; }
        public decimal? RetailPriceInclTaxMD { get; set; }
        public decimal? RetailPriceMD { get; set; }
        public decimal? CostPriceMD { get; set; }
        public string QtyFlag { get; set; }
        public string ProductType { get; set; }
        public string ProfitCenterCode { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}