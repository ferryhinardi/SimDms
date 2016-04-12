using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.Models
{
    [Table("omMstModel")]
    public class MstModel
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
        public bool? IsChassis { get; set; }
        public bool? IsCbu { get; set; }
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
        public string LockedDate { get; set; }
        public string MarketModelCode { get; set; }
        public string GroupMarketModel { get; set; }
        public string ColumnMarketModel { get; set; }
    }

    [Table("MsMstModel")]
    public class ItsMstModel
    {
        [Key]
        [Column(Order = 1)]
        public string ModelType { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Variant { get; set; }
        public string ModelName { get; set; }
        public int? CylinderCapacity { get; set; }
        public int? Length { get; set; }
        public string WheelDrive { get; set; }
        public string TransmissionType { get; set; }
        public bool isSuzukiClass { get; set; }
        public string BrandCode { get; set; }
        public string CategoryCode { get; set; }
        public string DimensionCode { get; set; }
        public string SegmentCode { get; set; }
        public string FunctionCode { get; set; }
        public string GroupModel { get; set; }
        public string Utility1 { get; set; }
        public string Utility2 { get; set; }
        public string Utility3 { get; set; }
        public string Utility4 { get; set; }
        public string Utility5 { get; set; }
        public string Utility6 { get; set; }
        public string Utility7 { get; set; }
        public string Utility8 { get; set; }
        public string Utility9 { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}