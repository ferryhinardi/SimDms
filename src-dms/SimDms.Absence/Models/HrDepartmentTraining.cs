using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrDepartmentTraining")]
    public class HrDepartmentTraining
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Department { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Position { get; set; }
        [Key]
        [Column(Order = 4)]
        public string Grade { get; set; }
        [Key]
        [Column(Order = 5)]
        public string TrainingCode { get; set; }
        public bool? IsRequired { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}