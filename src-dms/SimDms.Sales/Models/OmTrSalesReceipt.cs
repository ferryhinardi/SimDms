using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("OmTrSalesReceipt")]
    public class OmTrSalesReceipt
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReceiptNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string EngineCode { get; set; }
        [Key]
        [Column(Order = 7)]
        public decimal EngineNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public string FakturPolisiNo { get; set; }
        public string ColourCode { get; set; }
        public string ColourDescription { get; set; }
        public decimal? PrintSeq { get; set; }
        public string ReceiptStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public class OmTrSalesReceiptView
    {
        public string ReceiptNo { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal EngineNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public string FakturPolisiNo { get; set; }
        public string ColourCode { get; set; }
        public string ColourDescription { get; set; }
        public decimal? PrintSeq { get; set; }
        public string ReceiptStatus { get; set; }
    }
}