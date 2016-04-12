using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    
    [Table("SvKendaraanPel")]
    public class SvKendaraanPel
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PoliceRegNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public string ChassisNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerDesc { get; set; }
        public string CustomerAddr { get; set; }
        public string PhoneNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string DealerDesc { get; set; }
       // public string ChassisCode { get; set; }
       // public string ChassisNo { get; set; }
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
        public bool IsClubStatus { get; set; }
        public string IsContractStatusDesc { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveDesc { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public string LastJobType { get; set; }
        public decimal? ProductionYear { get; set; }
        public string ContractNo { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string ProfitCenterCode { get; set; }
        public string ColourDesc { get; set; }
        public string ColourName { get; set; }
        public string TechnicalModelCode { get; set; }

    }

    [Table("SvChassicView")]
    public class SvChassicView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public DateTime? PoliceRegistrationDate { get; set; }
        public string PoliceRegistrationNo { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string FakturPolisiNo { get; set; }
        public string Status { get; set; }
        public string BasicModel { get; set; }
      //  public string TechnicalModelCode { get; set; }
        public string TransmissionType { get; set; }
        public string ColourCode { get; set; }
        public string CustomerCode { get; set; }

    }

    [Table("SvCBasmodView")]
    public class SvCBasmodView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RefferenceCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BasicModel { get; set; }
        public string TechnicalModelCode { get; set; }
        public string ModelDescription { get; set; }
        public string Status { get; set; }


    }

    [Table("SvColorView")]
    public class SvColorView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string RefferenceCode { get; set; }
        public string ColourCode { get; set; }
        public string RefferenceDesc1 { get; set; }
        public string RefferenceDesc2 { get; set; }

    }

    [Table("SvCustomerDetailView")]
    public class SvCustomerDetailView
    {
        [Key]
        [Column(Order = 1)]
        public string CustomerCode { get; set; }
        public string CompanyCode { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string HPNo { get; set; }
        public string FaxNo { get; set; }
        public string ProfitCenterCode { get; set; }

    }

    [Table("SvGetTableChassis")]
    public class SvGetTableChassis
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ChassisNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public int SeqNo { get; set; }
        public string PreviousData { get; set; }
        public string ChangeCode { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

    }

    [Table("SvMstDealerView")]
    public class SvMstDealerView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }

    }

    public class CustomerVehicleBrowse
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string PoliceRegNo { get; set; }
        public string CustomerCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerDesc { get; set; }
        public string CustomerAddr { get; set; }
        public string PhoneNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string DealerDesc { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string ClubCode { get; set; }
        public string ColourCode { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string ClubNo { get; set; }
        public DateTime? ClubDateStart { get; set; }
        public DateTime? ClubDateFinish { get; set; }
        public DateTime? ClubSince { get; set; }
        public string IsClubStatusDesc { get; set; }
        public bool IsClubStatus { get; set; }
        public string IsContractStatusDesc { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveDesc { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public string LastJobType { get; set; }
        public decimal? ProductionYear { get; set; }
        public string ContractNo { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string ProfitCenterCode { get; set; }
        public string ColourDesc { get; set; }
        public string ColourName { get; set; }
        public string TechnicalModelCode { get; set; }

    }

}