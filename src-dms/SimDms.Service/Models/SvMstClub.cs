using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{

    [Table("svMstClub")]
    public class svMstClub
    {

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ClubCode { get; set; }
        public string Description { get; set; }
        public decimal? LaborDiscPct { get; set; }
        public decimal? PartDiscPct { get; set; }
        public decimal? MaterialDiscPct { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }

    }


    [Table("SvClubView")]
    public class SvClubView
    {

        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ClubCodeStr { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public decimal? LaborDiscPct { get; set; }
        public decimal? PartDiscPct { get; set; }
        public decimal? MaterialDiscPct { get; set; }
        public string IsActiveStr { get; set; }
        public bool IsActive { get; set; }

    }
    [Table("SvNoPolisi")]
    public class SvNoPolisi
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
        public string ChassisNoStr { get; set; }
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
        public bool IsActiveP { get; set; }
        public string IsActiveDesc { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public string LastJobType { get; set; }
        public decimal ChassisNo { get; set; }
        public string ContractNo { get; set; }
        public string ContactName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }


    }

    [Table("SvClubTable")]
    public class SvClubTable
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
        public bool IsClubStatus { get; set; }
        public string IsContractStatusDesc { get; set; }
        public bool IsActiveP { get; set; }
        public string IsActiveDesc { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public string LastJobType { get; set; }
        public string ContractNo { get; set; }
        public string ContactName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }


    }
   
}