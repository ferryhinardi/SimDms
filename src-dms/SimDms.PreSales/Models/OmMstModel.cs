using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.PreSales.Models
{
    [Table("OmMstModel")]
    public class OmMstModel
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public string FakturPolisiDesc { get; set; }
        public string EngineCode { get; set; }
        public string PpnBmCodeBuy { get; set; }
        public decimal? PpnBmPctBuy { get; set; }
        public string PpnBmCodeSell { get; set; }
        public decimal? PpnBmPctSell { get; set; }
        public string PpnBmCodePrincipal { get; set; }
        public decimal? PpnBmPctPrincipal { get; set; }
        public string Remark { get; set; }
        public string BasicModel { get; set; }
        public string TechnicalModelCode { get; set; }
        public string ProductType { get; set; }
        public string TransmissionType { get; set; }
        public bool IsChassis { get; set; }
        public bool IsCbu { get; set; }
        public string SMCModelCode { get; set; }
        public string GroupCode { get; set; }
        public string TypeCode { get; set; }
        public decimal? CylinderCapacity { get; set; }
        public string fuel { get; set; }
        public string ModelPrincipal { get; set; }
        public string Specification { get; set; }
        public string ModelLine { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? IsLocked { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
        public string MarketModelCode { get; set; }
        public string GroupMarketModel { get; set; }
        public string ColumnMarketModel { get; set; }

    }
}