using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("ApUtlInvoiceOthersHdr")]
    public class ApUtlInvoiceOthersHdr
    {
        [Key]
        [Column(Order = 1)]
        public long IDNo { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ProductType { get; set; }
        public string TransactionType { get; set; }
        public string FakturNo { get; set; }
        public DateTime? FakturDate { get; set; }
        public string TaxNo { get; set; }
        public DateTime? TaxDate { get; set; }
        public decimal? TotalDPP { get; set; }
        public decimal? TotalPPN { get; set; }
        public decimal? TotalInvoice { get; set; }
        public string TermOfPayment { get; set; }
        public string Description { get; set; }
        public string TotalNoInquiry { get; set; }
        public string SupplierCode { get; set; }
        public bool? IsExists { get; set; }
        public decimal? BalanceDPP { get; set; }
        public decimal? BalancePPN { get; set; }
        public decimal? BalanceInvoice { get; set; }
    }
}