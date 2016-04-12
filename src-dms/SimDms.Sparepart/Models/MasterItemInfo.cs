using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Sparepart.Models
{
    [Table("spMstItemInfo")]
    public class MasterItemInfo
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PartNo { get; set; }
        public string SupplierCode { get; set; }
        public string PartName { get; set; }
        public bool? IsGenuinePart { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? SalesUnit { get; set; }
        public decimal? OrderUnit { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string UOMCode { get; set; }
        public string Status { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("SpMasterItemView")]
    public class SpMasterItemView
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
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CategoryName { get; set; }
        public string MovingCode { get; set; }
        public decimal? AvailableItems { get; set; }
        public string PartName { get; set; }
        public bool? IsGenuinePart { get; set; }
        public decimal? OrderUnit { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public DateTime? BornDate { get; set; }
        public string Status { get; set; }
        public decimal? SalesUnit { get; set; }
        public decimal PurcDiscPct { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public decimal? DiscPct { get; set; }
        public DateTime? LastDemandDate { get; set; }
        public DateTime? LastSalesDate { get; set; }
        public string ABCClass { get; set; }
        public string UOMCode { get; set; }
        public decimal? DemandAverage { get; set; }
        public string Utility1 { get; set; }
        public string Utility2 { get; set; }
        public string Utility3 { get; set; }
        public string Utility4 { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? InTransit { get; set; }
        public decimal? BorrowQty { get; set; }
        public decimal? BorrowedQty { get; set; }
        public decimal? BackOrderSR { get; set; }
        public decimal? ReservedSR { get; set; }
        public decimal? BackOrderSP { get; set; }
        public decimal? ReservedSP { get; set; }
        public decimal? BackOrderSL { get; set; }
        public decimal? ReservedSL { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public decimal? OnHand { get; set; }
        public decimal? AllocationSP { get; set; }
        public decimal? AllocationSR { get; set; }
        public decimal? AllocationSL { get; set; }
        public decimal? OrderPointQty { get; set; }
        public decimal? OrderCycle { get; set; }
        public decimal? SafetyStock { get; set; }
        public decimal? LeadTime { get; set; }
 
    }
    public class LoadPart
    {
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string SupplierCode { get; set; }
        public DateTime? LastPurchaseUpdate { get; set; }
        public DateTime? LastRetailPriceUpdate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public string IsGenuinePart { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CategoryName { get; set; }
        public decimal? OldPurchasePrice { get; set; }
        public decimal? OldCostPrice { get; set; }
        public decimal? OldRetailPrice { get; set; }
 

    }

    public class SpMasteritemStockAlokasiView
    {
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string LocationCode { get; set; }
        public decimal? OnHand { get; set; }
        public decimal? AllocationSP { get; set; }
        public decimal? AllocationSR { get; set; }
        public decimal? AllocationSL { get; set; }
    }


    public class SpMasterPartView
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
        public string PartName { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public bool? IsGenuinePart { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CategoryName { get; set; }
        public decimal? FromQty { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public DateTime? LastPurchaseUpdate { get; set; }
        public DateTime? LastRetailPriceUpdate { get; set; }
        public decimal? OldCostPrice { get; set; }
        public decimal? OldPurchasePrice { get; set; }
        public decimal? OldRetailPrice { get; set; }
    }

    public class InquiryPenerimaanBarang
    {
        public Int64? RowNumber { get; set; }
        public String BinningNo { get; set; }
        public DateTime? BinningDate { get; set; }
        public String StatusBinning { get; set; }
        public String ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public String WRSNo { get; set; }
        public DateTime? WRSDate { get; set; }
        public String StatusWRS { get; set; }
        public String HPPNo { get; set; }
        public DateTime? HPPDate { get; set; }
        public String StatusHPP { get; set; }
        public String SupplierName { get; set; }
        public DateTime? DueDate { get; set; }
        public String TaxNo { get; set; }
        public DateTime? TaxDate { get; set; }
        public String TaxPeriod { get; set; }
        public String PayableAccNo { get; set; }
        public Decimal? TotPurchAmt { get; set; }
        public Decimal? TotNetPurchAmt { get; set; }
        public Decimal? TotTaxAmt { get; set; }
        public Decimal? DiffNetPurchAmt { get; set; }
        public Decimal? DiffTaxAmt { get; set; }
        public Decimal? TotHPPAmt { get; set; }
        public Boolean? IsTransferGL { get; set; }
    }

    public class InqDefAnalisys
    {
        [Key]
        public int id { get; set; }
        public int AccountID { get; set; }
        public String Account { get; set; }
        public String BranchCode { get; set; }
        public String BranchName { get; set; }
        public Decimal? JAN { get; set; }
        public Decimal? FEB { get; set; }
        public Decimal? MAR { get; set; }
        public Decimal? APR { get; set; }
        public Decimal? MAY { get; set; }
        public Decimal? JUN { get; set; }
        public Decimal? JUL { get; set; }
        public Decimal? AUG { get; set; }
        public Decimal? SEP { get; set; }
        public Decimal? OCT { get; set; }
        public Decimal? NOV { get; set; }
        public Decimal? DEC { get; set; }
    }

    public class InqDefMetadata
    {
        public String display { get; set; }
        public String value { get; set; }
        public String iconCls { get; set; }
        public String type { get; set; }
    }


    public class InqSAparam
    {
        public String Periode { get; set; }
        public String PrintType { get; set; }
        public String ItemTypeS { get; set; }
        public String Area { get; set; }
        public String Dealer { get; set; }
        public String Outlet { get; set; }
    }

    public class SpMasterPartSelect4Lookup
    {
        public string PartNo { get; set; }
        public string ProductType { get; set; }
        public string CategoryName { get; set; }
        public string PartCategory { get; set; }
        public string PartName { get; set; }
        public string IsGenuinePart { get; set; }
        public string IsActive { get; set; }
        public decimal? OrderUnit { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string TypeOfGoods { get; set; }
    }
}