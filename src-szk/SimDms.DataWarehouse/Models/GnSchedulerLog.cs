using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("GnSchedulerLog")]
    public class GnSchedulerLog
    {
        [Key]
        [Column(Order = 1)]
        public Guid ScheduleID { get; set; }
        public string DealerCode { get; set; }
        public string ScheduleName { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateFinish { get; set; }
        public string RunningTimes { get; set; }
        public bool? IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string Info { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}