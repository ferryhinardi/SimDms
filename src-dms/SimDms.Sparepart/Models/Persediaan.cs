using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    public class spTrnIAdjusthdrView
    {
        [Key]
        [Column(Order = 1)]
        public string AdjustmentNo { get; set; }
        public DateTime? AdjustmentDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
    }

    public class spTrnIAdjustDtlView
    {
        public long? No { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public string AdjustmentCode { get; set; }
        public string AdjustmentDesc { get; set; }
        public string WarehouseName { get; set; }
        public decimal? QtyAdjustment { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonDesc { get; set; }

    }

    public class SpMstItemPartView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PartNo { get; set; }
        public string ABCClass { get; set; }
        public decimal? AvailQty { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? ReservedSP { get; set; }
        public decimal? ReservedSR { get; set; }
        public decimal? ReservedSL { get; set; }
        public string MovingCode { get; set; }
        public string SupplierCode { get; set; }
        public string PartName { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public decimal? PurchasePrice { get; set; }

        public string ProductType { get; set; }
        public string TypeOfGoods { get; set; }
    }

    public class SpWarehouseCodeView
    {
        [Key]
        [Column(Order = 1)]
        public string warehousecode { get; set; }
        public string lookupvaluename { get; set; }
        
    }

    [Table("spTrnIWHTrfHdr")]
    public class spTrnIWHTrfHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string WHTrfNo { get; set; }
        public DateTime? WHTrfDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string TypeOfGoods { get; set; }
        public decimal? PrintSeq { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("spTrnIWHTrfDtl")]
    public class spTrnIWHTrfDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string WHTrfNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        public decimal? Qty { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? CostPrice { get; set; }
        [Key]
        [Column(Order = 9)]
        public string FromWarehouseCode { get; set; }
        public string FromLocationCode { get; set; }
        [Key]
        [Column(Order = 11)]
        public string ToWarehouseCode { get; set; }
        public string ToLocationCode { get; set; }
        public string ReasonCode { get; set; }
        public string MovingCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class WHDataDetail
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string PartNo { get; set; }
        public string MovingCode { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public decimal? CostPrice { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
    }

    public class WHLookupDetail
    {
        public long? No { get; set; }
        public string PartNo { get; set; }
        public string FromWarehouseCode { get; set; }
        public string ToWarehouseCode { get; set; }
        public decimal? Qty { get; set; }
        public string LookUpValueName { get; set; }
        public string PartName { get; set; }
    }
    
    public class spTrnIWHTrfDtlView
    {
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string FromWarehouseCode { get; set; }
        public string FromWarehouseName { get; set; }
        public string ToWarehouseCode { get; set; }
        public string ToWarehouseName { get; set; }
        public string ReasonCode { get; set; }
        public decimal? Qty { get; set; }
    }

    public class spTrnIWHTrfhdrView
    {
        [Key]
        [Column(Order = 1)]
        public string WHTrfNo { get; set; }
        public DateTime? WHTrfDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
    }

    public class spTrnGetRecordView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string WarehouseCode { get; set; }
        public decimal? OnHand { get; set; }
        public decimal? AllocationSP { get; set; }
        public decimal? AllocationSR { get; set; }
        public decimal? AllocationSL { get; set; }
        public decimal? BackOrderSP { get; set; }
        public decimal? BackOrderSR { get; set; }
        public decimal? BackOrderSL { get; set; }
        public decimal? ReservedSP { get; set; }
        public decimal? ReservedSR { get; set; }
        public decimal? ReservedSL { get; set; }

    }

    [Table("spTrnIReservedHdr")]
    public class spTrnIReservedHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReservedNo { get; set; }
        public DateTime? ReservedDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string PartNo { get; set; }
        public string TypeOfGoods { get; set; }
        public string OprCode { get; set; }
        public decimal? PrintSeq { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("spTrnIReservedDtl")]
    public class spTrnIReservedDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReservedNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string WarehouseCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        public string LocationCode { get; set; }
        public decimal? AvailableQty { get; set; }
        public decimal? ReservedQty { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class spTrnIReservedHdrView
    {
        public string ReservedNo { get; set; }
        public DateTime? ReservedDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string PartNo { get; set; }
        public string TypeOfGoods { get; set; }
        public string OprCode { get; set; }
        public string Status { get; set; }
    }

    public class spTrnIReservedDtlView
    {
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? AvailableQty { get; set; }
        public decimal? ReservedQty { get; set; }

        public string ReservedNo { get; set; }
        public string OprCode { get; set; }
    }

    public class GlAdjustContainer
    {
        public string AdjustmentCode { get; set; }
        public decimal? AmountPrice { get; set; }
    }

    public class spTrnIAdjustDtlContainer
    {
        public string PartNo { get; set; }
        public string WarehouseCode { get; set; }
        public string AdjustmentCode { get; set; }
        public decimal? QtyAdjustment { get; set; }
        public decimal? Qty { get; set; }
    }


    //public class spTrnPPOSHdrView
    //{

    //    public string POSNo { get; set; }
    //    public DateTime? PosDate { get; set; }
    //    public string Status { get; set; }
    //    public string SupplierCode { get; set; }
    //    public string SupplierName { get; set; }

    //}
}
