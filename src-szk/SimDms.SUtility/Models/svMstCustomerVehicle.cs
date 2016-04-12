using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.SUtility.Models
{
    [Table("svMstCustomerVehicle")]
    public class svMstCustomerVehicle
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
}