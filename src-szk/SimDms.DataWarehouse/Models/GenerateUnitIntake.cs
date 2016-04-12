using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("SvUnitIntake")]
    public class UnitIntakeView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public long ServiceNo { get; set; }
        public string JobOrderNo { get; set; }
        public string VinNo { get; set; }
        public DateTime? JobOrderClosed { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string CompanyName { get; set; }
        public string Area { get; set; }
        public int? AreaCode { get; set; }
        public decimal? Odometer { get; set; }
        public string SalesModelDesc { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? ProductionYear { get; set; }
        public DateTime? DoDate { get; set; }
        public string PoliceRegNo { get; set; }
        public decimal? EngineNo { get; set; }
        public decimal? ChassisNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNo { get; set; }
        public string OfficePhoneNo { get; set; }
        public string HPNo { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string JobType { get; set; }
        public string JobTypeDesc { get; set; }
        public string GroupJobType { get; set; }
        public string GroupJobTypeDesc { get; set; }
        public string SaName { get; set; }
        public string SaNik { get; set; }
        public string GroupType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string BasicModel { get; set; }
        //public string BranchName { get; set; }
    }

    [Table("SvUnitIntakeView")]
    public class SvUnitIntakeView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string OutletCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public long ServiceNo { get; set; }
        public string VinNo { get; set; }
        public DateTime? JobOrderClosed { get; set; }
        public string OutletName { get; set; }
        public string GroupJobTypeDesc { get; set; }
        public decimal? Odometer { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? ProductionYear { get; set; }
        public string PoliceRegNo { get; set; }
        public decimal? EngineNo { get; set; }
        public decimal? ChassisNo { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNo { get; set; }
        public string OfficePhoneNo { get; set; }
        public string HPNo { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string JobTypeDesc { get; set; }
        public string Area { get; set; }
        public string SaNik { get; set; }
        public string SaName { get; set; }
        public string CompanyName { get; set; }
        public string BasicModel { get; set; }
        public DateTime? DoDate { get; set; }
        public string ContactName { get; set; }
        public string JobType { get; set; }
        public string GroupNo { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        //public string GNGroupNo { get; set; }
        //public string GNCompanyCode { get; set; }
        //public string GNBranchCode { get; set; }
    }
}