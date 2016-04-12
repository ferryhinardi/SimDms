using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svTrnService")]
    public class TrnService
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public long ServiceNo { get; set; }
        public string ServiceType { get; set; }
        public string ServiceStatus { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string EstimationNo { get; set; }
        public DateTime? EstimationDate { get; set; }
        public string BookingNo { get; set; }
        public DateTime? BookingDate { get; set; }
        public string InvoiceNo { get; set; }
        public string ForemanID { get; set; }
        public string MechanicID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string PoliceRegNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string VIN { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColorCode { get; set; }
        public decimal? Odometer { get; set; }
        public string JobType { get; set; }
        public string ServiceRequestDesc { get; set; }
        public bool? ConfirmChangingPart { get; set; }
        public DateTime? EstimateFinishDate { get; set; }
        public DateTime? EstimateFinishDateSys { get; set; }
        public decimal? LaborDiscPct { get; set; }
        public decimal? PartDiscPct { get; set; }
        public decimal? MaterialDiscAmt { get; set; }
        public bool InsurancePayFlag { get; set; }
        public decimal? InsuranceOwnRisk { get; set; }
        public string InsuranceNo { get; set; }
        public string InsuranceJobOrderNo { get; set; }
        public decimal? PPNPct { get; set; }
        public decimal? PPHPct { get; set; }
        public decimal? LaborGrossAmt { get; set; }
        public decimal? PartsGrossAmt { get; set; }
        public decimal? MaterialGrossAmt { get; set; }
        public decimal? LaborDiscAmt { get; set; }
        public decimal? PartsDiscAmt { get; set; }
        public decimal? MaterialDiscPct { get; set; }
        public decimal? LaborDppAmt { get; set; }
        public decimal? PartsDppAmt { get; set; }
        public decimal? MaterialDppAmt { get; set; }
        public decimal? TotalDPPAmount { get; set; }
        public decimal? TotalPphAmount { get; set; }
        public decimal? TotalPpnAmount { get; set; }
        public decimal? TotalSrvAmount { get; set; }
        public decimal PrintSeq { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? IsSparepartClaim { get; set; }
    }

    [Table("svTrnSrvTask")]
    public class TrnSrvTask
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public long ServiceNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OperationNo { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? OperationCost { get; set; }
        public bool? IsSubCon { get; set; }
        public decimal? SubConPrice { get; set; }
        public string PONo { get; set; }
        public decimal? ClaimHour { get; set; }
        public string TypeOfGoods { get; set; }
        public string BillType { get; set; }
        public decimal SharingTask { get; set; }
        public string TaskStatus { get; set; }
        public DateTime? StartService { get; set; }
        public DateTime? FinishService { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public decimal? DiscPct { get; set; }
    }

    [Table("svTrnSrvItem")]
    public class TrnSrvItem
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public long ServiceNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal PartSeq { get; set; }
        public decimal? DemandQty { get; set; }
        public decimal? SupplyQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public char? TypeOfGoods { get; set; }
        public string BillType { get; set; }
        public string SupplySlipNo { get; set; }
        public DateTime? SupplySlipDate { get; set; }
        public string SSReturnNo { get; set; }
        public DateTime? SSReturnDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public decimal? DiscPct { get; set; }
        public string MechanicID { get; set; }
    }


   

    public class TrxSrvTaskDtl
    {
        public Int64 SeqNo { get; set; }
        public Int32 TaskPartSeq { get; set; }
        public string BillType { get; set; }
        public string BillTypeDesc { get; set; }
        public string TypeOfGoods { get; set; }
        public string TypeOfGoodsDesc { get; set; }
        public string ItemType { get; set; }
        public string TaskPartNo { get; set; }
        public decimal OprHourDemandQty { get; set; }
        public decimal SupplyQty { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal? OprRetailPrice { get; set; }
        public decimal OprRetailPriceTotal { get; set; }
        public string SupplySlipNo { get; set; }
        public string TaskPartDesc { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }
        public decimal? QtyAvail { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? PriceNet { get; set; }
        public Boolean? IsSubCon { get; set; }
        public string TaskStatus { get; set; }
        public Int64 ServiceNo { get; set; }
    }

    public class LookUpTrnServiceEstimation
    {
        public string InvoiceNo { get; set; }
        public Int64? ServiceNo { get; set; }
        public string ServiceType { get; set; }
        public string ForemanID { get; set; }
        public string EstimationNo { get; set; }
        public DateTime? EstimationDate { get; set; }
        public string BookingNo { get; set; }
        public DateTime? BookingDate { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string PoliceRegNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string KodeRangka { get; set; }
        public string KodeMesin { get; set; }
        public string ColorCode { get; set; }
        public string Customer { get; set; }
        public string CustomerBill { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public decimal? Odometer { get; set; }
        public string JobType { get; set; }
        public string ServiceStatus { get; set; }
        public bool? InsurancePayFlag { get; set; }
        public decimal? InsuranceOwnRisk { get; set; }
        public string InsuranceNo { get; set; }
        public string InsuranceJobOrderNo { get; set; }
        public decimal? LaborDiscPct { get; set; }
        public decimal? PartDiscPct { get; set; }
        public decimal? MaterialDiscPct { get; set; }
        public decimal? PPNPct { get; set; }
        public string ServiceRequestDesc { get; set; }
        public bool? ConfirmChangingPart { get; set; }
        public string MechanicID { get; set; }
        public DateTime? EstimateFinishDate { get; set; }
        public decimal? LaborDPPAmt { get; set; }
        public decimal? PartsDPPAmt { get; set; }
        public decimal? MaterialDPPAmt { get; set; }
        public decimal? TotalDPPAmount { get; set; }
        public decimal? TotalPpnAmount { get; set; }
        public decimal? TotalPphAmount { get; set; }
        public decimal? TotalSrvAmount { get; set; }
        public string EmployeeName { get; set; }
        public string AddressBill { get; set; }
        public string NPWPNo { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }

        public string ForemanName { get; set; }
        public string ColorName { get; set; }
    }

    public class DiscSPK
    {
        public Int32? SeqNo { get; set; }
        public String DiscountType { get; set; }
        public Decimal? LaborDiscPct { get; set; }
        public Decimal? PartDiscPct { get; set; }
        public Decimal? MaterialDiscPct { get; set; }
    }
}