using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("vwPartSales")]
    public class PartSalesView
    {
        [Key]
        [Column(Order = 1)]
        public string Area { get; set; }
        [Key]
        [Column(Order = 2)]
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string DealerAbbreviation { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAbbreviation { get; set; }
        [Key]
        [Column(Order = 4)]
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerClass { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string TypeOfGoods { get; set; }
        public string TypeOfGoodsDesc { get; set; }
        public decimal? QtyBill { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? NetSalesAmt { get; set; }
    }
}