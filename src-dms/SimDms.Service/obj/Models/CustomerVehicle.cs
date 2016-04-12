using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvMstCustomerVehicle")]
    public class CustomerVehicle
    {
        [Key]
        [Column(Order = 1)]     
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string TechnicalModelCode { get; set; }
        public string ServiceBookNo { get; set; }
        public string ColourCode { get; set; }
        public string DealerCode { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string CustomerCode { get; set; }
        public string ClubCode { get; set; }
        public string ClubNo { get; set; }
        public DateTime? ClubDateStart { get; set; }
        public DateTime? ClubDateFinish { get; set; }
        public DateTime? ClubSince { get; set; }
        public bool IsClubStatus { get; set; }
        public string ContractNo { get; set; }
        public bool IsContractStatus { get; set; }
        public DateTime? RemainderDate { get; set; }
        public string RemainderDescription { get; set; }
        public DateTime? FirstServiceDate { get; set; }
        public string LastJobType { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public decimal? LastServiceOdometer { get; set; }
        public bool IsActive { get; set; }
        public decimal? ProductionYear { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
    }

    [Table("SvCustomerVehicleView")]
    public class CustomerVehicleView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string ColorCode { get; set; }
        public string VinNo { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string ServiceBookNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddr { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string FaxNo { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }

        public string ClubCode { get; set; }
        public string ClubNo { get; set; }
        public string ClubEndPeriod { get; set; }
        public bool? ClubStatus { get; set; }
        public string ClubStatusDesc { get; set; }
        public string ContractNo { get; set; }
        public string ContractStatusDesc { get; set; }
        public string ContractEndPeriod { get; set; }
    }

    [Table("svMstCustomerVehicleHist")]
    public class svMstCustomerVehicleHist
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal ChassisNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public int SeqNo { get; set; }
        public string ChangeCode { get; set; }
        public string PreviousData { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class InqVehicleHistory
    {
        public bool? Cetak { get; set; }
        public string BranchCode { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string Chassis { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string Engine { get; set; }
        public string ServiceBookNo { get; set; }
        public string ColourCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Customer { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public decimal? LastServiceOdometer { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Dealer { get; set; }
        public string Remarks { get; set; }
        public string CustomerCode1 { get; set; }
    }

    public class GetVehicleInfo
    {
        public int? TaskPartSeq { get; set; }
        public string BranchCode { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string JobType { get; set; }
        public decimal? Odometer { get; set; }
        public string MechanicId { get; set; }
        public string ForemanId { get; set; }
        public string OperationNo { get; set; }
        public decimal? OperationQty { get; set; }
        public decimal? OperationAmt { get; set; }
        public decimal? TotalSrvAmount { get; set; }
        public decimal? SharingTask { get; set; }
        public string Description { get; set; }
        public string NameSA { get; set; }
        public string NameForeman { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class BrowseVehicleHistory
    {
        public string PoliceRegNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerDesc { get; set; }
        public string CustomerAddr { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string DealerDesc { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string ClubCode { get; set; }
        public string ColourCode { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string ClubNo { get; set; }
        public DateTime? ClubDateStart { get; set; }
        public DateTime? ClubDateFinish { get; set; }
        public DateTime? ClubSince { get; set; }
        public string IsClubStatusDesc { get; set; }
        public bool? IsClubStatus { get; set; }
        public string IsContractStatusDesc { get; set; }
        public bool? IsActive { get; set; }
        public string IsActiveDesc { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public string LastJobType { get; set; }
        public decimal? ChassisNo1 { get; set; }
        public string ContractNo { get; set; }
        public string ContactName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
    }

    public class MstCustomerVehicleView
    {
        public string CompanyCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string TechnicalModelCode { get; set; }
        public string ServiceBookNo { get; set; }
        public string ColourCode { get; set; }
        public string DealerCode { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string CustomerCode { get; set; }
        public string ClubCode { get; set; }
        public string ClubNo { get; set; }
        public DateTime? ClubDateStart { get; set; }
        public DateTime? ClubDateFinish { get; set; }
        public DateTime? ClubSince { get; set; }
        public bool IsClubStatus { get; set; }
        public string ContractNo { get; set; }
        public bool IsContractStatus { get; set; }
        public DateTime? RemainderDate { get; set; }
        public string RemainderDescription { get; set; }
        public DateTime? FirstServiceDate { get; set; }
        public string LastJobType { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public decimal? LastServiceOdometer { get; set; }
        public bool IsActive { get; set; }
        public decimal? ProductionYear { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
        public string ColourName { get; set; }
        public string DealerName { get; set; }
        public string CustomerName { get; set; }

        public string Address { get; set; }
        public string Customer { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string Color { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string FakturPolisiNo { get; set; }
        public string SeqNo { get; set; }
    }

    public class GetBookNoNewAndOld
    {
        public string BranchCode { get; set; }
        public string JobOrderNo { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string JobType { get; set; }
        public string Invoiceno { get; set; }
        public string ServiceBookNoOld { get; set; }
        public string ServiceBookNoNew { get; set; }
        public string EngineCodeOld { get; set; }
        public string EngineCodeNew { get; set; }
        public decimal? EngineNoOld { get; set; }
        public decimal? EngineNoNew { get; set; }
        public string PoliceRegNoOld { get; set; }
        public string PoliceRegNoNew { get; set; }
    }

    public class CustomerVehicleHistGrid
    {
        public string CompanyCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public int SeqNo { get; set; }
        public string ChangeCode { get; set; }
        public string PreviousData { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class InqVehicleHistoryWSDS
    {
         public string IsSelect { get; set; }
         public string PoliceRegNo { get; set; }
         public string BasicModel { get; set; }
         public string TransmissionType { get; set; }
         public string  Chassis { get; set; } 
         public string ChassisCode { get; set; }
         public string ChassisNo { get; set; }
         public string Engine { get; set; }
         public string ServiceBookNo { get; set; }
         public string ColourCode { get; set; }
         public string  Customer { get; set; }
         public string FakturPolisiDate { get; set; }	
         public string LastServiceDate { get; set; }
         public string LastServiceOdometer { get; set; }
         public string Dealer { get; set; }
         public string CustomerName { get; set; }
    }
}