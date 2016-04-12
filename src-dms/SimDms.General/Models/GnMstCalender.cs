using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.General.Models
{
    [Table("GnMstCalendar")]
    public class GnMstCalender
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProfitCenterCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public DateTime CalendarDate { get; set; }
        public string CalendarDescription { get; set; }
        public string CalendarType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class CalenderView 
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ProfitCenterCode { get; set; }
        public string CalendarDate { get; set; }
        public string CalendarDescription { get; set; }
        public string CalendarType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

}