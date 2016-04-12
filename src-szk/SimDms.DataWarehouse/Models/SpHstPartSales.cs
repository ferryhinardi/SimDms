using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("SpHstPartSales")]
    public class SpHstPartSales
    {
        [Key]
        public Guid RecordID { get; set; }
        public DateTime RecordDate { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerClass { get; set; }
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