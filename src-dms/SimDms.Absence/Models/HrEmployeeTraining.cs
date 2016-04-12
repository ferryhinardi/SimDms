using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrEmployeeTraining")]
    public class HrEmployeeTraining
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        public string TrainingCode { get; set; }
        public int TrainingSeq { get; set; }
        [Key]
        [Column(Order = 3)]
        public DateTime? TrainingDate { get; set; }
        public int? TrainingDuration { get; set; }
        public int? PreTest { get; set; }
        public string PreTestAlt { get; set; }
        public int? PostTest { get; set; }
        public string PostTestAlt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }

    [Table("HrEmployeeTrainingView")]
    public class HrEmployeeTrainingView
    {
        [Key]
        [Column(Order=0)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 1)]
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        [Key]
        [Column(Order = 2)]
        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        [Key]
        [Column(Order = 3)]
        public DateTime TrainingDate { get; set; }
        public int? PostTest { get; set; }
        public string PostTestAlt { get; set; }
        public int? PreTest { get; set; }
        public string PreTestAlt { get; set; }
    }
}