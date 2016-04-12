using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omTrSalesInvoiceAccs")]
    public class omTrSalesInvoiceAccs
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string InvoiceNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string LmpNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? DPP { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
        public decimal? COGS { get; set; }
        public decimal? QuantityReturn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string TypeOfGoods { get; set; }

    }
}