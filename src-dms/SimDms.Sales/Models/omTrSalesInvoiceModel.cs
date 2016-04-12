using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTrSalesInvoiceModel")]
    public class omTrSalesInvoiceModel
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string InvoiceNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BPKNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SalesModelYear { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? DiscIncludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscPPnBM { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? PPnBMPaid { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPn { get; set; }
        public decimal? QuantityReturn { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public decimal? ShipAmt { get; set; }
        public decimal? DepositAmt { get; set; }
        public decimal? OthersAmt { get; set; }
    }
}