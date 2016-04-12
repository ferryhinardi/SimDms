using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("SpUtlStockTrfDtl")]
    public class SpUtlStockTrfDtl
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode{ get; set; }
        [Key]
        [Column(Order = 3)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string LampiranNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OrderNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string PartNo { get; set; }
        public string SalesNo { get; set; }
        public string PartNoShip { get; set; }
        public decimal? QtyShipped { get; set; }
        public decimal? SalesUnit { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}
