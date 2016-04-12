using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrEmployeeAchievement")]
    public class HrEmployeeAchievement
    {
        [Key]
        [Column(Order=0)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 1)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 2)]
        public DateTime? AssignDate { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Grade { get; set; }
        public bool IsJoinDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }

    [Table("HrEmployeeAchievementView")]
    public class HrEmployeeAchievementView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Grade { get; set; }
        [Key]
        [Column(Order = 3)]
        public DateTime AssignDate { get; set; }
        public bool? IsJoinDate { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string GradeName { get; set; }
        public string IsInitialPosition { get; set; }
        public string AssignDateStatus { get; set; }
    }
}