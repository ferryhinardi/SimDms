using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svMstTarget")]
    public class svMstTarget
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal PeriodYear { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal PeriodMonth { get; set; }
        public decimal? ProductivityMechanic { get; set; }
        public decimal? ProductivityStall { get; set; }
        public decimal? TotalUnitService { get; set; }
        public decimal? TotalWorkingDays { get; set; }
        public decimal? TotalMechanic { get; set; }
        public decimal? TotalStall { get; set; }
        public decimal? TotalLift { get; set; }
        public decimal? HourlyLaborRate { get; set; }
        public decimal? OverheadCost { get; set; }
        public decimal? ServiceAmount { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public int? DasDailyTarget { get; set; }
        public int? DasMonthTarget { get; set; }
        public decimal? SMRTarget { get; set; }


    }

    [Table("SvTargetView")]
    public class SvTargetView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string Period { get; set; }
        public decimal PeriodYear { get; set; }
        public decimal PeriodMonth { get; set; }
        public decimal? ProductivityMechanic { get; set; }
        public decimal? TotalWorkingDays { get; set; }
        public decimal? TotalMechanic { get; set; }
        public decimal? TotalStall { get; set; }
        public decimal? TotalLift { get; set; }
        public decimal? HourlyLaborRate { get; set; }
        public decimal? ProductivityStall { get; set; }
        public decimal? TotalUnitService { get; set; }
        public decimal? OverheadCost { get; set; }
        public decimal? ServiceAmount { get; set; }
        public decimal? SMRTarget { get; set; }
        public int? DasDailyTarget { get; set; }
        public int? DasMonthTarget { get; set; }

    }
}