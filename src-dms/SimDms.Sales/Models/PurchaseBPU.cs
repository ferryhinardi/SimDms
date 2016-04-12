using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omTrPurchaseBPU")]
    public class omTrPurchaseBPU
    {
        public omTrPurchaseBPU()
        {
            this.isLocked = false;
        }

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BPUNo { get; set; }
        public DateTime? BPUDate { get; set; }
        public string SupplierCode { get; set; }
        public string ShipTo { get; set; }
        public string RefferenceDONo { get; set; }
        public DateTime? RefferenceDODate { get; set; }
        public string RefferenceSJNo { get; set; }
        public DateTime? RefferenceSJDate { get; set; }
        public string WarehouseCode { get; set; }
        public string Expedition { get; set; }
        public string BPUType { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public DateTime? BPUSJDate { get; set; }
    }

    [Table("omTrPurchaseBPUDetail")]
    public class omTrPurchaseBPUDetail
    {
        public omTrPurchaseBPUDetail()
        {
            this.SalesModelYear = 0;
            this.ChassisNo = 0;
            this.EngineNo = 0;
            this.isReturn = false;
        }

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BPUNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public int BPUSeq { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
        public string Remark { get; set; }
        public string StatusSJRel { get; set; }
        public string SJRelNo { get; set; }
        public DateTime? SJRelDate { get; set; }
        public string SJRelReff { get; set; }
        public DateTime? SJRelReffDate { get; set; }
        public string StatusDORel { get; set; }
        public string DORelNo { get; set; }
        public DateTime? DORelDate { get; set; }
        public string StatusHPP { get; set; }
        public bool? isReturn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class BPUView
    {
        public string PONo { get; set; }
        public string BPUNo { get; set; }
        public DateTime? BPUDate { get; set; }
        public string SupplierCode { get; set; }
        public string ShipTo { get; set; }
        public string RefferenceDONo { get; set; }
        public DateTime? RefferenceDODate { get; set; }
        public string RefferenceSJNo { get; set; }
        public DateTime? RefferenceSJDate { get; set; }
        public string WarehouseCode { get; set; }
        public string Expedition { get; set; }
        public string BPUType { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class BPUDetailModel
    {
        public string PONo { get; set; }
        public string BPUNo { get; set; }
        public string SalesModelCode { get; set; }
    }

    public class InquiryTrPurchaseBPUView
    {
        public string BPUNo { get; set; }
        public string BPUDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string ShipTo { get; set; }
        public string RefferenceDONo { get; set; }
        public string RefferenceDODate { get; set; }
        public string RefferenceSJNo { get; set; }
        public string RefferenceSJDate { get; set; }
        public string WarehouseCode { get; set; }
        public string WareHouseName { get; set; }
        public string Expedition { get; set; }
        public string ExpeditionName { get; set; }
        public string BPUType { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class InquiryTrPurchaseBPUDetailView
    {
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
        public string Remark { get; set; }
    }

    [Table("OmTrPurchaseBPULookupView")]
    public class OmTrPurchaseBPULookupView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BPUNo { get; set; }
        public DateTime? BPUDate { get; set; }
        public string SupplierCode { get; set; }
        public string ShipTo { get; set; }
        public string RefferenceDONo { get; set; }
        public DateTime? RefferenceDODate { get; set; }
        public string RefferenceSJNo { get; set; }
        public DateTime? RefferenceSJDate { get; set; }
        public string WarehouseCode { get; set; }
        public string Expedition { get; set; }
        public string BPUType { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public DateTime? BPUSJDate { get; set; }
        public string SupplierName { get; set; }
        public string Tipe { get; set; }
        public string StatusBPU { get; set; }
        public string ExpeditionName { get; set; }
        public string WarehouseName { get; set; }

        //Not Mapped
        [NotMapped]
        public string DealerCode { get; set; }
        [NotMapped]
        public string BatchNo { get; set; }
    }

    [Table("OmTrPurchaseBPUDetailView")]
    public class OmTrPurchaseBPUDetailView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BPUNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public int BPUSeq { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
        public string Remark { get; set; }
        public bool isReturn { get; set; }
    }


}