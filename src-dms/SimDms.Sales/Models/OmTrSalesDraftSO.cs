using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omTrSalesDraftSO")]
    public class OmTrSalesDraftSO
    {
        public OmTrSalesDraftSO()
        {
            this.TOPDays = 0;
            this.PrePaymentAmt = 0;
            this.CommissionAmt = 0;
            this.Installment = 0;
            this.PrePaymentDate = DateTime.Now;
        }

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DraftSONo { get; set; }
        public DateTime? DraftSODate { get; set; }
        public string SalesType { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string CustomerCode { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public string BillTo { get; set; }
        public string ShipTo { get; set; }
        public string ProspectNo { get; set; }
        public string SKPKNo { get; set; }
        public string Salesman { get; set; }
        public string WareHouseCode { get; set; }
        public bool isLeasing { get; set; }
        public string LeasingCo { get; set; }
        public string GroupPriceCode { get; set; }
        public string Insurance { get; set; }
        public string PaymentType { get; set; }
        public decimal? PrePaymentAmt { get; set; }
        public DateTime? PrePaymentDate { get; set; }
        public string PrePaymentBy { get; set; }
        public string CommissionBy { get; set; }
        public decimal? CommissionAmt { get; set; }
        public string PONo { get; set; }
        public string ContractNo { get; set; }
        public DateTime? RequestDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string ApproveBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string RejectBy { get; set; }
        public DateTime? RejectDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string SalesCode { get; set; }
        public decimal? Installment { get; set; }
        public DateTime? FinalPaymentDate { get; set; }
        public string StatusDraftSO { get; set; }
    }

    [Table("omTrSalesDraftSOAccs")]
    public class OmTrSalesDraftSOAccs 
    {
        public OmTrSalesDraftSOAccs()
        {
            this.DemandQty = 0;
            this.SupplyQty = 0;
            this.ReturnQty = 0;
            this.CostPrice = 0;
            this.RetailPrice = 0;
            this.CancelQty = 0;
            this.InvoiceQty = 0;
        }
        
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DraftSONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        public decimal? DemandQty { get; set; }
        public decimal? SupplyQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public string TypeOfGoods { get; set; }
        public string BillType { get; set; }
        public string SupplySlipNo { get; set; }
        public DateTime SupplySlipDate { get; set; }
        public string SSReturnNo { get; set; }
        public DateTime SSReturnDate { get; set; }
        public bool isSubstitution { get; set; }
        public decimal? CancelQty { get; set; }
        public decimal? InvoiceQty { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    [Table("omTrSalesDraftSOModel")]
    public class OmTrSalesDraftSOModel
    {
        public OmTrSalesDraftSOModel()
        {
            this.SalesModelYear = 0;
            this.BeforeDiscDPP = 0;
            this.BeforeDiscPPn = 0;
            this.BeforeDiscPPnBM = 0;
            this.BeforeDiscTotal = 0;
            this.DiscExcludePPn = 0;
            this.DiscIncludePPn = 0;
            this.AfterDiscDPP = 0;
            this.AfterDiscPPn = 0;
            this.AfterDiscPPnBM = 0;
            this.AfterDiscTotal = 0;
            this.OthersDPP = 0;
            this.OthersPPn = 0;
            this.QuantityDraftSO = 0;
            this.QuantitySO = 0;
            this.ShipAmt = 0;
            this.DepositAmt = 0;
            this.OthersAmt = 0;
        }

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DraftSONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal? SalesModelYear { get; set; }
        public string ChassisCode { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public decimal? BeforeDiscPPn { get; set; }
        public decimal? BeforeDiscPPnBM { get; set; }
        public decimal? BeforeDiscTotal { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? DiscIncludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscPPnBM { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPn { get; set; }
        public decimal? QuantityDraftSO { get; set; }
        public decimal? QuantitySO { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public decimal? ShipAmt { get; set; }
        public decimal? DepositAmt { get; set; }
        public decimal? OthersAmt { get; set; }
    }

    [Table("omTrSalesDraftSOModelColour")]
    public class OmTrSalesDraftSOModelColour
    {
        public OmTrSalesDraftSOModelColour()
        {
            this.Quantity = 0;
        }

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DraftSONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal? SalesModelYear { get; set; }
        [Key]
        [Column(Order = 6)]
        public string ColourCode { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    [Table("omTrSalesDraftSOModelOthers")]
    public class OmTrSalesDraftSOModelOthers {

        public OmTrSalesDraftSOModelOthers()
        {
            this.BeforeDiscDPP = 0;
            this.BeforeDiscPPn = 0;
            this.BeforeDiscTotal = 0;
            this.DiscExcludePPn = 0;
            this.DiscIncludePPn = 0;
            this.AfterDiscDPP = 0;
            this.AfterDiscPPn = 0;
            this.AfterDiscTotal = 0;
            this.DPP = 0;
            this.PPn = 0;
            this.Total = 0;
        }

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DraftSONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal? SalesModelYear { get; set; }
        [Key]
        [Column(Order = 6)]
        public string OtherCode { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public decimal? BeforeDiscPPn { get; set; }
        public decimal? BeforeDiscTotal { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? DiscIncludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? DPP { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    [Table("omTrSalesDraftSOVin")]
    public class OmTrSalesDraftSOVin{

        public OmTrSalesDraftSOVin()
        {
            this.ChassisNo = 0;
            this.EngineNo = 0;
            this.BBN = 0;
            this.KIR = 0;
        }

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DraftSONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal? SalesModelYear { get; set; }
        [Key]
        [Column(Order = 6)]
        public string ColourCode { get; set; }
        [Key]
        [Column(Order = 7)]
        public int SOSeq { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
        public string EndUserName { get; set; }
        public string EndUserAddress1 { get; set; }
        public string EndUserAddress2 { get; set; }
        public string EndUserAddress3 { get; set; }
        public string SupplierBBN { get; set; }
        public string CityCode { get; set; }
        public decimal? BBN { get; set; }
        public decimal? KIR { get; set; }
        public string Remark { get; set; }
        public string StatusReq { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    [Table("omTrSalesDraftSOLookupView")]
    public class OmTrSalesDraftSOLookupView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DraftSONo { get; set; }
        public string RefferenceDate { get; set; }
        public string Address { get; set; }
        public string RefferenceNo { get; set; }
        public string SKPKNo { get; set; }
        public DateTime DraftSODate { get; set; }
        public string Customer { get; set; }
        public string Salesman { get; set; }
        public string GroupPriceCode { get; set; }
        public string Stat { get; set; }
        public string TypeSales { get; set; }
    }

    public class Select4LookupCustomer
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string TopCode { get; set; }
        public string TOPCD { get; set; }
        public string GroupPriceCode { get; set; }
        public string GroupPriceDesc { get; set; }
        public string SalesCode { get; set; }
    }
    
    [Table("ITSBrowse")]
    public class ITSNoDraftSO{
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string InquiryNo { get; set; }
        public DateTime InquiryDate { get; set; }
        public string EmployeeName { get; set; }
        public string NamaProspek { get; set; }
        public string TipeKendaraan { get; set; }
        public string EmployeeID { get; set; }
    }

    [Table("ITSBrowseSo")]
    public class ITSNoDraftSO_2
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string InquiryNo { get; set; }
        public DateTime InquiryDate { get; set; }
        public string EmployeeName { get; set; }
        public string NamaProspek { get; set; }
        public string TipeKendaraan { get; set; }
        public string EmployeeID { get; set; }
        public string Createdby { get; set; } 
    }

    [Table("ITSBrowseSo4")]
    public class ITSNoDraftSO_4
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string InquiryNo { get; set; }
        public DateTime InquiryDate { get; set; }
        public string EmployeeName { get; set; }
        public string NamaProspek { get; set; }
        public string TipeKendaraan { get; set; }
        public string EmployeeID { get; set; }
        public string Createdby { get; set; }
    }

    public class ColourSO 
    {
        public string ColourCode { get; set; } 
        public string ColourDesc { get; set; }
        public Decimal Quantity { get; set; }
        public string Remark { get; set; }
    }

    public class ChassisSO
    {
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; } 
    }

    public class ChassisSO2
    {
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
    }

    public class CitySO  
    {
        public string CityCode { get; set; }
        public string CityDesc { get; set; }
        public decimal? BBN { get; set; } 
        public decimal? KIR { get; set; }
    }

    public class AksLL 
    {
        public string RefferenceCode { get; set; }
        public string RefferenceDesc1 { get; set; } 
    }

    public class ItemNewSO  
    {
        public string PartNo { get; set; }
        public decimal? Available { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public string PartName { get; set; }
        public string Status { get; set; }
        public string JenisPart { get; set; }
        public decimal? NilaiPart { get; set; }
        public decimal? CostPrice { get; set; } 
    }

    public class SKPKFromDrafSO  
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string DraftSONo { get; set; }
        public string Salesman { get; set; }
        public string SalesmanName { get; set; } 
        public string LeasingCo { get; set; }
        public DateTime? DraftSODate { get; set; }
        public DateTime? FinalPaymentDate { get; set; }
        public DateTime? RequestDate { get; set; } 
        public string ProspectNo { get; set; }
        public decimal? Installment { get; set; }
        public decimal? CommissionAmt { get; set; } 
        public string Remark { get; set; }
        public string GroupPriceCode { get; set; }
        public string GroupPriceName { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; } 
    }

    public class OmTrSalesDraftSOModelOthersSelect4Table
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string DraftSONo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string OtherCode { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public decimal? BeforeDiscPPn { get; set; }
        public decimal? BeforeDiscTotal { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? DiscIncludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? DPP { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
        public string Remark { get; set; }
        public string AccsName { get; set; }
    }

    [Table("LookUpSo")]
    public class TrSalesSOView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SONo { get; set; }
        public DateTime? SODate { get; set; }
        public string SalesType { get; set; }
        public string TypeSales { get; set; }
        public string Address { get; set; }  
        public string RefferenceNo { get; set; }
        public string RefferenceDate { get; set; }
        public string RefferenceDates { get; set; } 
        public string Customer { get; set; } 
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public string BillTo { get; set; }
        public string BillName { get; set; }
        public string ShipTo { get; set; }
        public string ShipName { get; set; }
        public string ProspectNo { get; set; }
        public string SKPKNo { get; set; }
        public string Sales { get; set; } 
        public string Salesman { get; set; }
        public string SalesmanName { get; set; }
        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public bool? isLeasing { get; set; }
        public bool? isC1 { get; set; }
        public bool? isC2 { get; set; }
        public bool? isC3 { get; set; }
        public bool? isC4 { get; set; } 
        public string LeasingCo { get; set; }
        public string LeasingCoName { get; set; }
        public string GroupPrice { get; set; } 
        public string GroupPriceCode { get; set; }
        public string GroupPriceName { get; set; }
        public string Insurance { get; set; }
        public string PaymentType { get; set; }
        public decimal? PrePaymentAmt { get; set; }
        public string PrePaymentDate { get; set; }
        public string PrePaymentBy { get; set; }
        public string PrePaymentName { get; set; }
        public string CommissionBy { get; set; }
        public decimal? CommissionAmt { get; set; }
        public string PONo { get; set; }
        public string ContractNo { get; set; }
        public string RequestDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Stat { get; set; }
        public decimal? Installment { get; set; }
        public DateTime? FinalPaymentDate { get; set; }
        public string SalesCoordinator { get; set; }
        public string SalesHead { get; set; }
        public string BranchManager { get; set; }
        public string VehicleType { get; set; } 
    }

    public class SOMerk
    {
        public string ModelType { get; set; }
        public string BrandCode { get; set; }
        public string Variant { get; set; }
        public string ModelName { get; set; }
        public int seq { get; set; }
    }

    public class OWSalesModel
    {
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; } 
        public string StatusVehicle { get; set; }
        public string BrandCode { get; set; } 
        public string ModelName { get; set; }
    }
}