using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SimDms.Common;


namespace SimDms.Sparepart.Models
{
    [Table("spTrnPSUGGORHdr")]
    public class spTrnPSUGGORHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SuggorNo { get; set; }
        public DateTime? SuggorDate { get; set; }
        public string TypeOfGoods { get; set; }
        public string POSNo { get; set; }
        public DateTime? POSDate { get; set; }
        public string SupplierCode { get; set; }
        public string ProductType { get; set; }
        public string MovingCode { get; set; }
        public string OrderType { get; set; }
        public string Status { get; set; }
        public decimal? PrintSeq { get; set; }
        public bool? IsVoid { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }
    
    [Table("spTrnPSUGGORDtl")]
    public class spTrnPSUGGORDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SuggorNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        public decimal? SeqNo { get; set; }
        public decimal? OnHand { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? InTransit { get; set; }
        public decimal? AllocationSP { get; set; }
        public decimal? AllocationSR { get; set; }
        public decimal? AllocationSL { get; set; }
        public decimal? BackOrderSP { get; set; }
        public decimal? BackOrderSR { get; set; }
        public decimal? BackOrderSL { get; set; }
        public decimal? ReservedSP { get; set; }
        public decimal? ReservedSR { get; set; }
        public decimal? ReservedSL { get; set; }
        public decimal? DemandAvg { get; set; }
        public decimal? OrderPoint { get; set; }
        public decimal? SafetyStock { get; set; }
        public decimal? AvailableQty { get; set; }
        public decimal? SuggorQty { get; set; }
        public decimal? SuggorCorrecQty { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public bool isExistInItems { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("spTrnPSUGGORSubDtl")]
    public class spTrnPSUGGORSubDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SuggorNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNoSuggor { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        public decimal? SeqNo { get; set; }
        public decimal? I { get; set; }
        public decimal? II { get; set; }
        public decimal? III { get; set; }
        public decimal? IV { get; set; }
        public decimal? V { get; set; }
        public decimal? VI { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

    }

    public class SuggorLookup
    {
        [Key]
        [Column(Order = 1)]
        public string SuggorNo { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string SuggorDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }

    }



    public class supplierLookUp
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string SupplierCode { get; set; }
        public string StandardCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierGovName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string FaxNo { get; set; }
        public string ProvinceCode { get; set; }
        public string AreaCode { get; set; }
        public string CityCode { get; set; }
        public string ZipNo { get; set; }
        public bool? isPKP { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string ProfitCenterCode { get; set; }

        public string Alamat { get; set; }
        public Decimal? Diskon { get; set; }
        public string Profit { get; set; }
    }


    public class OrderSparepartview
    {
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
        public decimal? DiscPct { get; set; }
    }

    public class Posview
    {
        public string PosNo { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
    }

    public class SpTrnPPOSHdrView
    {
        public string PosNo { get; set; }
        public DateTime? PosDate { get; set; }
        public bool IsDeleted { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        
    }

    public class orderPartview
    {
        public string POSNo { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? Intransit { get; set; }
        public decimal? Received { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string SeqNo { get; set; }
        public string SupplierCode { get; set; }
        public string PartNoOriginal { get; set; }
        public string TypeOfGoods { get; set; }
    }

    public class orderPartviewByPart
    {
        public string PartNo { get; set; }
        public string PartName { get; set; }
    }

    [Table("spTrnPOrderBalance")]
    public class spTrnPOrderBalance
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string POSNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SupplierCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SeqNo { get; set; }
        public string PartNoOriginal { get; set; }
        public DateTime? POSDate { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? InTransit { get; set; }
        public decimal? Received { get; set; }
        public decimal? Located { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public string TypeOfGoods { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }


    public class spTrnPOrderBalance2
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string POSNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SupplierCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SeqNo { get; set; }
        public string PartNoOriginal { get; set; }
        public DateTime? POSDate { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? InTransit { get; set; }
        public decimal? Received { get; set; }
        public decimal? Located { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? newQty { get; set; }
        public string newPartNo { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public string TypeOfGoods { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class ProcessSuggor
    {
        public String PartNo { get; set; }
        public Decimal? DemandAverage { get; set; }
        public Decimal? LeadTime { get; set; }
        public Decimal? OrderCycle { get; set; }
        public Decimal? SafetyStock { get; set; }
        public Int32? No { get; set; }
        public Decimal? SeqNo { get; set; }
        public Decimal? AvailableQty { get; set; }
        public Decimal? SuggorQty { get; set; }
        public Decimal? SuggorCorrecQty { get; set; }
        public String ProductType { get; set; }
        public String PartCategory { get; set; }
        public Decimal? PurchasePrice { get; set; }
        public Decimal? CostPrice { get; set; }
        public Decimal? OrderPoint { get; set; }
        public Decimal? OnHand { get; set; }
        public Decimal? OnOrder { get; set; }
        public Decimal? InTransit { get; set; }
        public Decimal? AllocationSP { get; set; }
        public Decimal? AllocationSR { get; set; }
        public Decimal? AllocationSL { get; set; }
        public Decimal? BackOrderSP { get; set; }
        public Decimal? BackOrderSR { get; set; }
        public Decimal? BackOrderSL { get; set; }
        public Decimal? ReservedSP { get; set; }
        public Decimal? ReservedSR { get; set; }
        public Decimal? ReservedSL { get; set; }
    }

    public class PreSaveSuggor
    {
        public string PartNo { get; set; }
        public decimal? I { get; set; }
        public decimal? II { get; set; }
        public decimal? III { get; set; }
        public decimal? IV { get; set; }
        public decimal? V { get; set; }
        public decimal? VI { get; set; }
    }

    [Table("spHstPOrderBalance")]
    public class SpHstPOrderBalance
    {
        [Key]
        [Column(Order= 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public DateTime LastUpdateDate { get; set; }
        public string OldPartNo { get; set; }
        public decimal OldOrderQty { get; set; }
        public string NewPartNo { get; set; }
        public decimal NewOrderQty { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Discount { get; set; }
        public decimal CostPrice { get; set; }
        public string LastUpdateBy { get; set; }
    }
}