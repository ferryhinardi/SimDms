using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("HrShift")]
    public class HrShift
    {
        public string CompanyCode { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public string OnDutyTime { get; set; }
        public string OffDutyTime { get; set; }
        public string OnRestTime { get; set; }
        public string OffRestTime { get; set; }
        public int? WorkingHour { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}