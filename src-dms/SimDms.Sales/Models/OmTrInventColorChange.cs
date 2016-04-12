using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OmTrInventColorChange")]
    public class OmTrInventColorChange
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
    }

    [Table("OmTrInventColorChangeDetail")]
    public class OmTrInventColorChangeDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DocNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCodeFrom { get; set; }
        public string ColourCodeTo { get; set; }
        public string WarehouseCode { get; set; }
        public string RemarkDtl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class ColorChangeView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public string LastUpdateBy { get; set; }
        //public DateTime? LastUpdateDate { get; set; }
        //public bool? isLocked { get; set; }
        //public string LockedBy { get; set; }
        //public DateTime? LockedDate { get; set; }
    }

    public class ColorChangeDetailView 
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string DocNo { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelYear { get; set; }
        public string ColourCodeFrom { get; set; }
        public string ColourNameFrom { get; set; }
        public string ColourCodeTo { get; set; }
        public string ColourNameTo { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string RemarkDtl { get; set; }
    }
}