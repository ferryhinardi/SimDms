using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svTrnInvItem")]
    public class SvTrnInvItem
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string InvoiceNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        public string MovingCode { get; set; }
        public string ABCClass { get; set; }
        public decimal SupplyQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public string TypeOfGoods { get; set; }
        public decimal? DiscPct { get; set; }
        public string MechanicID { get; set; }
    }
}