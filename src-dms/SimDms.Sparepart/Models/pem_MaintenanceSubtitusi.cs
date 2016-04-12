using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SimDms.Common;


namespace SimDms.Sparepart.Models
{

    public class spTrnPOrderBalanceLookup
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string POSNo { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }

 

    }
    public class spTrnPOrderBalancePartLookup
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string POSNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SupplierCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string SeqNo { get; set; }
        public string PartNoOriginal { get; set; }
        public string PartName { get; set; }
        public DateTime? POSDate { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? InTransit { get; set; }
        public decimal? Received { get; set; }
        public decimal? Located { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public string TypeOfGoods { get; set; }



    }

}