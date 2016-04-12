using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimDms.CStatisfication.Models
{
    [Table("svMstCustomerVehicle")]
    public class CustomerVehicle
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public Nullable<decimal> EngineNo { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string TechnicalModelCode { get; set; }
        public string ServiceBookNo { get; set; }
        public string ColourCode { get; set; }
        public string DealerCode { get; set; }
        public Nullable<System.DateTime> FakturPolisiDate { get; set; }
        public string CustomerCode { get; set; }
        public string ClubCode { get; set; }
        public string ClubNo { get; set; }
        public Nullable<System.DateTime> ClubDateStart { get; set; }
        public Nullable<System.DateTime> ClubDateFinish { get; set; }
        public Nullable<System.DateTime> ClubSince { get; set; }
        public bool IsClubStatus { get; set; }
        public string ContractNo { get; set; }
        public bool IsContractStatus { get; set; }
        public Nullable<System.DateTime> RemainderDate { get; set; }
        public string RemainderDescription { get; set; }
        public Nullable<System.DateTime> FirstServiceDate { get; set; }
        public string LastJobType { get; set; }
        public Nullable<System.DateTime> LastServiceDate { get; set; }
        public Nullable<decimal> LastServiceOdometer { get; set; }
        public bool IsActive { get; set; }
        public Nullable<decimal> ProductionYear { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public Nullable<System.DateTime> LastupdateDate { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
    }
}