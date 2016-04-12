using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    public class SalesOrderForm
    {
        public string SONumber { get; set; }
        public DateTime? SODate { get; set; }
        public string ReffNumber { get; set; }
        public DateTime? ReffDate { get; set; }
        public bool? IsDirectSales { get; set; }
        public string ITSNumber { get; set; }
        public string VehicleType { get; set; }
        public string SKPKNumber { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanName { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPName { get; set; }
        public decimal? TOPDays { get; set; }
        public string TOPPaidWith { get; set; }
        public string ChargedToCode { get; set; }
        public string ChargedToName { get; set; }
        public string ShipToCode { get; set; }
        public string ShipToName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string GroupPriceCode { get; set; }
        public string GroupPriceName { get; set; }
        public bool? IsLeasing { get; set; }
        public string LeasingCode { get; set; }
        public string LeasingName { get; set; }
        public decimal? Tenor { get; set; }
        public DateTime? PaidPaymentDate { get; set; }
        public string Insurance { get; set; }
        public decimal? Advance { get; set; }
        public string AdvancePercentage { get; set; }
        public string PayeeCode { get; set; }
        public string PayeeName { get; set; }
        public string ReceiverCode { get; set; }
        public string ReceiverName { get; set; }
        public DateTime? ReceivingDate { get; set; }
        public string MediatorName { get; set; }
        public decimal? MediatorComission { get; set; }
        public string PONumber { get; set; }
        public string ContractNumber { get; set; }
        public DateTime? ContractDate { get; set; }
        public string ContractNote { get; set; }
        public string SOStatus { get; set; }
        public string SaleType { get; set; }
        public string Customer { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime? FinalPaymentDate { get; set; }
    }

    public class ITS
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string InquiryNo { get; set; }
        public DateTime? InquiryDate { get; set; }
        public string EmployeeName { get; set; }
        public string NamaProspek { get; set; }
        public string TipeKendaraan { get; set; }
        public string EmployeeID { get; set; }
        public string LastProgress { get; set; }
        public string ProspectNo { get; set; }
        public string ReturnNo { get; set; }
    }

    public class SOCustomer
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string TOPCode { get; set; }
        public string TOPCD { get; set; }
        public string GroupPriceCode { get; set; }
        public string GroupPriceDesc { get; set; }
        public string SalesCode { get; set; }
    }

    public class SalesModel
    {
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
    }

    public class Leasing
    {
        public string LeasingCode { get; set; }
        public string LeasingName { get; set; }
    }

    public class SalesModelYearModel
    {
        public string CompanyCode { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string ChassisCode { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class ColourModel
    {
        public string ColourCode { get; set; }
        public string ColourDesc { get; set; }
        public string Remark { get; set; }
    }

    //public class ChassisModel
    //{
    //    public string ChassisNo { get; set; }
    //    public string EngineNo { get; set; }
    //    public string ServiceBookNo { get; set; }
    //    public string KeyNo { get; set; }
    //}

    public class SupplierBNNModel
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
    }

    public class AccOtherModel
    {
        public string RefferenceCode { get; set; }
        public string RefferenceDesc { get; set; }
    }

    public class SelectPartsModel
    {
        public string PartNo { get; set; }
        public int? Available { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public string PartName { get; set; }
        public string Status { get; set; }
        public string JenisPart { get; set; }
        public string NilaiPart { get; set; }
    }

    public class SalesModelForm
    {
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public decimal? ShipAmt { get; set; }
        public decimal? OthersAmt { get; set; }
        public decimal? DepositAmt { get; set; }
        public decimal? Discount { get; set; }
        public string Remark { get; set; }
        public decimal? BeforeDiscTotal { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? DPP { get; set; }
        public decimal? PPn { get; set; }
        public decimal? PPnBM { get; set; }
        public string GroupPriceCode { get; set; }
        public string SONumber { get; set; }
        public bool IsDirectSales { get; set; }
        public string ITSNumber { get; set; }
        public decimal? BeforeDiscPPn { get; set; }
        public decimal? BeforeDiscPPnBM { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public string ChassisCode { get; set; }
    }

    public class ColourModelForm
    {
        public string ColourCode { get; set; }
        public string ColourDesc { get; set; }
        public int? Quantity { get; set; }
        public string RemarkColour { get; set; }
        public string SONumber { get; set; }
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
    }

    public class ChassisListModel
    {
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
    }

    public class CityListModel
    {
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public decimal? BBN { get; set; }
        public decimal KIR { get; set; }
    }

    public class SOOtherFormModel
    {
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string STNKName { get; set; }
        public string STNKAddress1 { get; set; }
        public string STNKAddress2 { get; set; }
        public string STNKAddress3 { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public decimal? BBN { get; set; }
        public decimal? KIR { get; set; }
        public string RemarkOther { get; set; }
        public string SONumber { get; set; }
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public string ColourCode { get; set; }
    }

    public class SalesModelList
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SONo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
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
        public decimal? QuantitySO { get; set; }
        public decimal? QuantityDO { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public decimal? ShipAmt { get; set; }
        public decimal? DepositAmt { get; set; }
        public decimal? OthersAmt { get; set; }
    }

    public class ColourModelList
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SONo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
        public string ColourDesc { get; set; }
        public string SalesModelDesc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class OmTrSalesSOModelOtherList
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SONo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
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
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class SalesSOModelOthersForm
    {
        public string CustomerCode { get; set; }
        public string SONumber { get; set; }
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public string RefferenceCode { get; set; }
        public string RefferenceDesc { get; set; }
        public decimal? AccOthersTotalBeforeDisc { get; set; }
        public decimal? AccOthersTotalAfterDisc { get; set; }
        public decimal? AccOthersDPPAfterDisc { get; set; }
        public decimal? AccOthersPPNAfterDisc { get; set; }
        public string AccOthersRemark { get; set; }
    }

    public class SparePartList
    {
        public string PartNo { get; set; }
        public decimal? Available { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public string PartName { get; set; }
        public string Status { get; set; }
        public string JenisPart { get; set; }
        public decimal? NilaiPart { get; set; }
    }

    public class SparePartForm
    {
        public string SONumber { get; set; }
        public string CustomerCode { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelYear { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? SparePartTotalBeforeDisc { get; set; }
        public decimal? SparePartTotalAfterDisc { get; set; }
        public decimal? SparePartDPPAfterDisc { get; set; }
        public decimal? SparePartPPnAfterDisc { get; set; }
        public decimal? SparePartQtyPart { get; set; }
        public decimal? SparePartQtyUnit { get; set; }
    }

    public class OmTrSalesSOAccsSeqList
    {
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? DemandQty { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? Total { get; set; }
        public string JenisPart { get; set; }
        public decimal? Qty { get; set; }
        public decimal? QtyUnit { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? PartSeq { get; set; }
        public string SONumber { get; set; }
    }

    public class WHDOList
    {
        public int? QtyDO { get; set; }
        public string WarehouseCode { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
    }

    public class ChassisCheckModel
    {
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
    }

    public class BpkSeqModel
    {
        public string BPKNo { get; set; }
        public int? BPKSeq { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
    }

    public class DOSeqModel
    {
        public string DONo { get; set; }
        public int? DOSeq { get; set; }
    }

    public class modelPONumber
    {
        public string PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
    }
}