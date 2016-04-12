using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omTrPurchaseKaroseriTerima")]
    public class omTrPurchaseKaroseriTerima
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string KaroseriTerimaNo { get; set; }
        public DateTime? KaroseriTerimaDate { get; set; }
        public string KaroseriSPKNo { get; set; }
        public string SupplierCode { get; set; }
        public string RefferenceInvoiceNo { get; set; }
        public DateTime? RefferenceInvoiceDate { get; set; }
        public string RefferenceFakturPajakNo { get; set; }
        public DateTime? RefferenceFakturPajakDate { get; set; }
        public DateTime? DueDate { get; set; }
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
        public decimal? PPh { get; set; }
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

    [Table("omTrPurchaseKaroseriTerimaDetail")]
    public class omTrPurchaseKaroseriTerimaDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string KaroseriTerimaNo { get; set; }
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
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class KaroseriTerimaView
    {
        public string KaroseriTerimaNo { get; set; }
        public DateTime? KaroseriTerimaDate { get; set; }
        public string KaroseriSPKNo { get; set; }
        public string SupplierCode { get; set; }
        public string RefferenceInvoiceNo { get; set; }
        public DateTime? RefferenceInvoiceDate { get; set; }
        public string RefferenceFakturPajakNo { get; set; }
        public DateTime? RefferenceFakturPajakDate { get; set; }
        public DateTime? DueDate { get; set; }
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
        public decimal? PPh { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Stat { get; set; }
    }

    public class KaroseriTerimaDetailView
    {
        public string KaroseriTerimaNo { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColourCodeOld { get; set; }
        public string ColourCodeNew { get; set; }
        public string ColourNameOld { get; set; }
        public string ColourNameNew { get; set; }
        public string Remark { get; set; }
    }

    public class inquiryTrPurchaseKaroseriTerimaView
    {
        public string KaroseriTerimaNo { get; set; }
        public string KaroseriTerimaDate { get; set; }
        public string KaroseriSPKNo { get; set; }
        public string SupplierCode { get; set; }
        public string RefferenceInvoiceNo { get; set; }
        public string RefferenceInvoiceDate { get; set; }
        public string RefferenceFakturPajakNo { get; set; }
        public string RefferenceFakturPajakDate { get; set; }
        public string DueDate { get; set; }
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
        public decimal? PPh { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class KaroseriTerimaDetailModel
    {
        public string KaroseriSPKNo { get; set; }
        public DateTime? KaroseriSPKDate { get; set; }
        public string KaroseriTerimaNo { get; set; }
        public DateTime? KaroseriTerimaDate { get; set; }
        public string SalesModelCodeOld { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string Remark { get; set; }
    }
}