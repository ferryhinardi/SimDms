using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("SpTrnIMovement")]
    public class SpTrnIMovement
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order=2)]
        public string BranchCode{ get; set; }
        [Key]
        [Column(Order=3)]
        public string DocNo { get; set; }
        [Key]
        [Column(Order=4)]
        public DateTime DocDate { get; set; }
        [Key]
        [Column(Order=5)]
        public DateTime CreatedDate { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public string PartNo { get; set; }
        public string SignCode { get; set; }
        public string SubSignCode{ get; set; }
        public decimal Qty { get; set; }
        public decimal? Price { get; set; }
        public decimal? CostPrice { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
    }
}

