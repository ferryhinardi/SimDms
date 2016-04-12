using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTrSalesSoModelOthers")]
    public class OmTrSalesSoModelOther
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal SalesModelYear { get; set; }
        [Key]
        [Column(Order = 6)]
        public string OtherCode { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public decimal? BeforeDiscPPn { get; set; }
        public decimal? BeforeDiscTotal { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? DiscIncludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? DPP { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class SalesSoModelOther
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SONo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public string OtherCode { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public decimal? BeforeDiscPPn { get; set; }
        public decimal? BeforeDiscTotal { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? DiscIncludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? DPP { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
        public string Remark { get; set; }
        public string OtherDesc { get; set; } 
        
    }
}