using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{

    [Table("svMstContract")]
    public class svMstContract
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ContractNo { get; set; }
        public DateTime? ContractDate { get; set; }
        public string Description { get; set; }
        public string CustomerCode { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public DateTime? BeginPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }
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

    [Table("SvKontrakServiceView")]
    public class SvKontrakServiceView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ContractNo { get; set; }
        public DateTime ContractDate { get; set; }
        public string Description { get; set; }
        public string CustomerCode { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public DateTime BeginPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
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
        public string CustomerName { get; set; }
        public string Status { get; set; }

    }

    /*[Table("SvCustomerDetailView")]
    public class SvCustomerDetailView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string HPNo { get; set; }
        public string FaxNo { get; set; }

    }*/

    [Table("SvVehicleDetailView")]
    public class SvVehicleDetailView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ContractNoStr { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PoliceRegNo { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string ClubCode { get; set; }
        public string ClubNo { get; set; }
        public string CustomerCode { get; set; }

    }
}