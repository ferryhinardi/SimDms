using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omTrPurchaseKaroseri")]
    public class omTrPurchaseKaroseri
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string KaroseriSPKNo { get; set; }
        public DateTime? KaroseriSPKDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string SupplierCode { get; set; }
        public string SalesModelCodeOld { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string SalesModelCodeNew { get; set; }
        public string ChassisCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? DPPMaterial { get; set; }
        public decimal? DPPFee { get; set; }
        public decimal? DPPOthers { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
        public decimal? DurationDays { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string WarehouseCode { get; set; }
    }

    [Table("omTrPurchaseKaroseriDetail")]
    public class omTrPurchaseKaroseriDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string KaroseriSPKNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColourCodeOld { get; set; }
        public string ColourCodeNew { get; set; }
        public string Remark { get; set; }
        public bool? isKaroseriTerima { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class KaroseriDetailView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string KaroseriSPKNo { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string ColourCodeOld { get; set; }
        public string ColourOld { get; set; }
        public string ColourCodeNew { get; set; }
        public string ColourNew { get; set; }
        public string Remark { get; set; }
        public bool? isKaroseriTerima { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class KaroseriView
    {
        public string KaroseriSPKNo { get; set; }
        public DateTime? KaroseriSPKDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string SupplierCode { get; set; }
        public string SalesModelCodeOld { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string SalesModelCodeNew { get; set; }
        public string ChassisCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? DPPMaterial { get; set; }
        public decimal? DPPFee { get; set; }
        public decimal? DPPOthers { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
        public decimal? DurationDays { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class inquiryTrPurchaseKaroseriView
    {
        public string KaroseriSPKNo { get; set; }
        public DateTime? KaroseriSPKDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SalesModelCodeOld { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string SalesModelCodeNew { get; set; }
        public string ChassisCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? DPPMaterial { get; set; }
        public decimal? DPPFee { get; set; }
        public decimal? DPPOthers { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
        public decimal? DurationDays { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string SalesModelDesc { get; set; }
        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
    }

    public class KaroseriDetailModel
    {
        public string KaroseriSPKNo { get; set; }
        public DateTime? KaroseriSPKDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SalesModelCodeOld { get; set; }
        public string SalesModelDesc{ get; set; } 
        public decimal? SalesModelYear { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; } 
        public string RefferenceNo { get; set; }
    }

    public class KaroseriDetail
    {
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
    }
}