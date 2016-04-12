using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.ConsoleApps.Models
{
    [Table("HrEmployeeAchievementLog")]
    public class HrEmployeeAchievementLog
    {
        [Key]
        [Column(Order = 1)]
        public string UnicID { get; set; }
        public string CompanyCode { get; set; }
        public string EmployeeID { get; set; }
        public DateTime? AssignDate { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Grade { get; set; }
        public bool IsJoinDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
