﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Common.Models
{
    [Table("HrEmployeeAchievement")]
    public class HrEmployeeAchievement
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
        public string Department { get; set; }
        public string Position { get; set; }
        public string Grade { get; set; }
        public bool? IsJoinDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
