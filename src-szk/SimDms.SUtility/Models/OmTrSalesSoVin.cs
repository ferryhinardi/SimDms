using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("OmTrSalesSoVin")]
    public class OmTrSalesSoVin
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
        public string ColourCode { get; set; }
        [Key]
        [Column(Order = 7)]
        public int SOSeq { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
        public string EndUserName { get; set; }
        public string EndUserAddress1 { get; set; }
        public string EndUserAddress2 { get; set; }
        public string EndUserAddress3 { get; set; }
        public string SupplierBBN { get; set; }
        public string CityCode { get; set; }
        public decimal? BBN { get; set; }
        public decimal? KIR { get; set; }
        public string Remark { get; set; }
        public char? StatusReq { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

    }
}