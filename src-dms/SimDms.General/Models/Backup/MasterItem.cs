using SimDms.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{

    [Table("spMasterPartLookup")]
    public class spMasterPartLookup
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

    public class MasterItemBrowse
    {
        [Key]
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
        public bool IsSGP { get; set; }
    }

    public class MasterModelBrowse
    {
        public string ModelCode { get; set; }
        public string ModelName { get; set; }
    }
    
    [Table("spMstItemModel")]
    public class SpMstItemModel : BaseTable
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ModelCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("GnMstSupplierProfitCenter")]
    public class MstSupplierProfitCenter
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SupplierCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ProfitCenterCode { get; set; }
        public string ContactPerson { get; set; }
        public string SupplierClass { get; set; }
        public string SupplierGrade { get; set; }
        public decimal? DiscPct { get; set; }
        public string TOPCode { get; set; }
        public string TaxCode { get; set; }
        public bool? isBlackList { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("GnMstCustomerProfitCenter")]
    public class MstCustomerProfitCenter
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ProfitCenterCode { get; set; }
        public Decimal? CreditLimit { get; set; }
        public string PaymentCode { get; set; }
        public string CustomerClass { get; set; }
        public string TaxCode { get; set; }
        public string TaxTransCode { get; set; }
        public Decimal? DiscPct { get; set; }
        public Decimal? LaborDiscPct { get; set; }
        public Decimal? PartDiscPct { get; set; }
        public Decimal? MaterialDiscPct { get; set; }
        public string TOPCode { get; set; }
        public string CustomerGrade { get; set; }
        public string ContactPerson { get; set; }
        public string CollectorCode { get; set; }
        public string GroupPriceCode { get; set; }
        public bool IsOverDueAllowed { get; set; }
        public bool IsBlackList { get; set; }
        public string SalesCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string SalesType { get; set; }
        public string Salesman { get; set; }
    }

    //[Table("GnMstCustomer")]
    //public class GnMstCustomer
    //{
    //    [Key]
    //    [Column(Order = 1)]
    //    public string CompanyCode { get; set; }
    //    [Key]
    //    [Column(Order = 2)]
    //    public string CustomerCode { get; set; }
    //    public string StandardCode { get; set; }
    //    public string CustomerName { get; set; }
    //    public string CustomerAbbrName { get; set; }
    //    public string CustomerGovName { get; set; }
    //    public string CustomerType { get; set; }
    //    public string CategoryCode { get; set; }
    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string Address3 { get; set; }
    //    public string Address4 { get; set; }
    //    public string PhoneNo { get; set; }
    //    public string HPNo { get; set; }
    //    public string FaxNo { get; set; }
    //    public bool? isPKP { get; set; }
    //    public string NPWPNo { get; set; }
    //    public DateTime? NPWPDate { get; set; }
    //    public string SKPNo { get; set; }
    //    public DateTime? SKPDate { get; set; }
    //    public string ProvinceCode { get; set; }
    //    public string AreaCode { get; set; }
    //    public string CityCode { get; set; }
    //    public string ZipNo { get; set; }
    //    public string Status { get; set; }
    //    public string CreatedBy { get; set; }
    //    public DateTime? CreatedDate { get; set; }
    //    public string LastUpdateBy { get; set; }
    //    public DateTime? LastUpdateDate { get; set; }
    //    public bool? isLocked { get; set; }
    //    public string LockingBy { get; set; }
    //    public DateTime? LockingDate { get; set; }
    //    public string Email { get; set; }
    //    public DateTime? BirthDate { get; set; }
    //    public string Spare01 { get; set; }
    //    public string Spare02 { get; set; }
    //    public string Spare03 { get; set; }
    //    public string Spare04 { get; set; }
    //    public string Spare05 { get; set; }
    //    public string Gender { get; set; }
    //    public string OfficePhoneNo { get; set; }
    //    public string KelurahanDesa { get; set; }
    //    public string KecamatanDistrik { get; set; }
    //    public string KotaKabupaten { get; set; }
    //    public string IbuKota { get; set; }
    //    public string CustomerStatus { get; set; }
    //} 

    [Table("GnMstFPJSignDate")]
    public class GnMstFPJSignDate
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProfitCenterCode { get; set; }
        public string FPJOption { get; set; }
        public string FPJOptionDescription { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    public class PartInquiry
    {
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public decimal? QtyAvail { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public string IsGenuinePart { get; set; }
        public string SupplierCode { get; set; }
        public decimal? RetailPrice { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CategoryName { get; set; }
        public string IsActive { get; set; }
        public decimal? OrderUnit { get; set; }
        public string SupplierName { get; set; }
        public string TypeOfGoods { get; set; }
    }

    public class PartInquiry_Subsitusi
    {
        public Int32 No { get; set; }
        public String PartNo { get; set; }
        public String PartName { get; set; }
        public String InterchangeCode { get; set; }
        public Int32 UnitConversion { get; set; }
        public Decimal OnHand { get; set; }
        public Decimal AllocationSP { get; set; }
        public Decimal OnOrder { get; set; }
        public Decimal InTransit { get; set; }
        public Decimal Received { get; set; }
        public String isRegister { get; set; }
    }

    public class PartInquiry_Location
    {
        public String BranchCode { get; set; }
        public String WarehouseCode { get; set; }
        public String LookUpValueName { get; set; }
        public String LocationCode { get; set; }
        public Decimal OnHand { get; set; }
        public Decimal AllocationSP { get; set; }
        public Decimal AllocationSL { get; set; }
        public Decimal AllocationSR { get; set; }
        public Decimal OnOrder { get; set; }
        public Decimal InTransit { get; set; }
        public Decimal BorrowQty { get; set; }
        public Decimal BorrowedQty { get; set; }
        public Decimal BackOrderSP { get; set; }
        public Decimal BackOrderSL { get; set; }
        public Decimal BackOrderSR { get; set; }
        public Decimal ReservedSP { get; set; }
        public Decimal ReservedSL { get; set; }
        public Decimal ReservedSR { get; set; }
    }

    public class PartInquiry_DemandAndSales
    {
        public String BranchCode { get; set; }
        public Decimal Year { get; set; }
        public String Month { get; set; }
        public Decimal DemandFreq { get; set; }
        public Decimal DemandQty { get; set; }
        public Decimal SalesFreq { get; set; }
        public Decimal SalesQty { get; set; }
    }

    public class PartInquiry_OnOrder
    {
        public String POSNo { get; set; }
        public DateTime POSDate { get; set; }
        public Decimal OnOrder { get; set; }
        public Decimal InTransit { get; set; }
        public Decimal Received { get; set; }
        public String WRSNo { get; set; }
        public String SupplierName { get; set; }
    }

    public class SparePartLocationLookup
    {
        public String PartNo { get; set; }
        public String ABCClass { get; set; }
        public Decimal? AvailQty { get; set; }
        public Decimal? OnOrder { get; set; }
        public Decimal? ReservedSP { get; set; }
        public Decimal? ReservedSR { get; set; }
        public Decimal? ReservedSL { get; set; }
        public String MovingCode { get; set; }
        public String SupplierCode { get; set; }
        public String PartName { get; set; }
        public Decimal? RetailPrice { get; set; }
        public Decimal? RetailPriceInclTax { get; set; }
        public Decimal? PurchasePrice { get; set; }
    }

    [Table("spUtlItemSetup")]
    public class spUtlItemSetup
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
        public string MovingCode { get; set; }
        public string ABCClass { get; set; }
        public string LocationCode { get; set; }
        public string TypeOfGoods { get; set; }
        public DateTime? BornDate { get; set; }
        public decimal? Qty { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public string SupplierCode { get; set; }
        public string PartName { get; set; }
    }
}
