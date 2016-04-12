using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models
{
    [Table("HrShift")]
    public class Shift
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public string OnDutyTime { get; set; }
        public string OffDutyTime { get; set; }
        public string OnRestTime { get; set; }
        public string OffRestTime { get; set; }
        public int WorkingHour { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}