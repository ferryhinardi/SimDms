using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrEmployeeMutation")]
    public class HrEmployeeMutation
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 3)]
        public DateTime MutationDate { get; set; }
        public string BranchCode { get; set; }
        public bool? IsJoinDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class MutationModel
    {
        public string EmployeeID { get; set; }
        public string MutationDate { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string MutationInfo { get; set; }
    }

    public class ValidationMessage
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
}