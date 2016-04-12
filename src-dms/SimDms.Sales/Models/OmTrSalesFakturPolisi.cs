using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("OmTrSalesFakturPolisi")]
    public class OmTrSalesFakturPolisi
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string FakturPolisiNo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public bool IsBlanko { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public DateTime? FakturPolisiProcess { get; set; }
        public string SJImniNo { get; set; }
        public string DOImniNo { get; set; }
        public string ReqNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Status { get; set; }
        public bool? IsManual { get; set; }
    }

    public class FakturPolisiBrowse
    {
        public string FakturPolisiNo { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public bool IsBlanko { get; set; }
        public string ReqNo { get; set; }
        public string Status { get; set; }
        public string Stat { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ColourDescription { get; set; }
        public string CustomerName { get; set; }
        public string ProducType { get; set; }
    }

    public class ChassisBrowse
    {
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
    }

    public class DetailVehicle
    {
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string RefferenceDONo { get; set; }
        public string RefferenceSJNo { get; set; }
        public string ReqNo { get; set; }
    }
}