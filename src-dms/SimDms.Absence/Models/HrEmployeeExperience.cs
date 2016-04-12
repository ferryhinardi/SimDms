using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimDms.Absence.Models
{
    [Table("HrEmployeeExperience")]
    public class HrEmployeeExperience
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 3)]
        public DateTime? JoinDate { get; set; }

        public int ExpSeq { get; set; }
        public string NameOfCompany { get; set; }
        public DateTime? ResignDate { get; set; }
        public string ReasonOfResign { get; set; }
        public string LeaderName { get; set; }
        public string LeaderPhone { get; set; }
        public string LeaderHP { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public class HrEmployeeExperienceModel
    {
        public string EmployeeID { get; set; }
        public string WorkingExperienceNameOfCompany { get; set; }
        public DateTime WorkingExperienceJoinDate { get; set; }
        public DateTime? WorkingExperienceResignDate { get; set; }
        public string WorkingExperienceReasonOfResign { get; set; }
        public string WorkingExperienceLeaderName { get; set; }
        public string WorkingExperienceLeaderPhone { get; set; }
        public string WorkingExperienceLeaderHP { get; set; }
    }
}