using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvMstWarranty")]
    public class SvMstWarranty
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string OperationNo { get; set; }
        public string Description { get; set; }
        public decimal Odometer { get; set; }
        public decimal? TimePeriod { get; set; }
        public string TimeDim { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("SvGaransiView")]
    public class SvMstGaransiView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string OperationNo { get; set; }
        public string Description { get; set; }
        public decimal Odometer { get; set; }
        public decimal? TimePeriod { get; set; }
        public string TimeDim { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string TimeDimDesc { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("SvBMView")]
    public class SvBMView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        public string TechnicalModelCode { get; set; }
        public string ModelDescription { get; set; }
        public string Status { get; set; }
    }

    [Table("SvJTView")]
    public class SvJTView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string OperationNo { get; set; }
        public string Description { get; set; }
        public string TechnicalModelCode { get; set; }
        public string IsSubCon { get; set; }
        public string IsCampaign { get; set; }
        public string Status { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? ClaimHour { get; set; }
    }
    
}