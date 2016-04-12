using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTrSalesBPKDetail")]
    public class OmTrSalesBPKDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BPKNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public int BPKSeq { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
        public string ReqOutNo { get; set; }
        public string Remark { get; set; }
        public string StatusPDI { get; set; }
        public string StatusInvoice { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

    }

    public class InquiryTrSalesBPKDetailView
    {
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
        public string Remark { get; set; }
        public string StatusPDI { get; set; }
    }
}