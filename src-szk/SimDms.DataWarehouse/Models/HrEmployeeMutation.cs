using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
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
        public DateTime? MutationDate { get; set; }
        public string BranchCode { get; set; }
        public bool IsJoinDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}