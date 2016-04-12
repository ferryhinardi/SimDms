using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omFakturPajakHdr")]
    public class omFakturPajakHdr
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
        public DateTime? InvoiceDate { get; set; }
        public string FakturPajakNo { get; set; }
        public DateTime? FakturPajakDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string CustomerCode { get; set; }
        public decimal? DPPAmt { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? PPnAmt { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? TotQuantity { get; set; }
        public decimal? PPnBMPaid { get; set; }
        public string TaxType { get; set; }
        public decimal? PrintSeq { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FakturPajakCentralNo { get; set; }
        public DateTime? FakturPajakCentralDate { get; set; }
    }
}