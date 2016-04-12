using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Absence.Models
{
    [Table("HrEmployeeAdditionalBranch")]
    public class HrEmployeeAdditionalBranch
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 3)]
        public DateTime AssignDate { get; set; }
        public string BranchCode { get; set; }
        public int? SeqNo { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}