using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OmTrSalesInvoice")]
    public class OmTrSalesInvoice
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
        public string SONo { get; set; }
        public string CustomerCode { get; set; }
        public string BillTo { get; set; }
        public string FakturPajakNo { get; set; }
        public DateTime? FakturPajakDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? isStandard { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }

    }

    public class InquiryTrSalesInvoiceView
    {
        public string InvoiceNo { get; set; }
        public string CustomerCode { get; set; }
        public string Address { get; set; }
        public string InvoiceDate { get; set; }
        public string SalesType { get; set; }
        public string SONo { get; set; }
        public string CustomerName { get; set; }
        public string BillTo { get; set; }
        public string FakturPajakNo { get; set; }
        public string DNNo { get; set; }
        public string FakturPajakDate { get; set; }
        public string DueDate { get; set; }
        public string Remark { get; set; }
        public string SKPKNo { get; set; }
        public string SODate { get; set; }
        public string DONo { get; set; }
        public string DODate { get; set; }
        public string BPKNo { get; set; }
        public string BPKDate { get; set; }
        public string RefferenceNo { get; set; }
        public string Status { get; set; }
    }

    public class InvoiceView
    {
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string SONo { get; set; }
        public DateTime? SODate { get; set; }
        public string Status { get; set; }
        public string GroupPriceCode { get; set; }
    }

    public class omSlsInvGetTotal4Tax
    {
        public decimal DPPAmt { get; set; }
        public decimal TotQuantity { get; set; }
        public decimal PPnAmt { get; set; }
        public decimal TotalAmt { get; set; }
    }

    public class omSlsInvGetTotal4TaxModel
    {
        public decimal DPPAmt { get; set; }
        public decimal DiscAmt { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal PPnAmt { get; set; }
        public decimal TotQuantity { get; set; }
        public decimal PPnBMPaid { get; set; }
    }

}