using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    
    [Table("svMstEvent")]
    public class svMstEvent
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EventNo { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        public string BasicModel { get; set; }
        public string JobType { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public bool IsDiscount { get; set; }
        public decimal? LaborDiscPct { get; set; }
        public decimal? PartsDiscPct { get; set; }
        public decimal? MaterialDiscPct { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }


    }

    [Table("SvEventView")]
    public class SvEventView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EventNo { get; set; }
        public DateTime EventDate { get; set; }
        public string BasicModel { get; set; }
        public string JobType { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public decimal? LaborDiscPct { get; set; }
        public decimal? PartsDiscPct { get; set; }
        public decimal? MaterialDiscPct { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }

    }

    [Table("SvEventBM")]
    public class SvEventBM
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BasicModel { get; set; }
        public string TechnicalModelCode { get; set; }

    }

}