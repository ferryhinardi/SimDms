using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svMstAvailableMechanic")]
    public class SvMstMecAvb
    {
            [Key]
            [Column(Order = 1)]
            public string CompanyCode { get; set; }
            [Key]
            [Column(Order = 2)]
            public string BranchCode { get; set; }
            [Key]
            [Column(Order = 3)]
            public string EmployeeID { get; set; }
            public bool IsAvailable { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public string LastupdateBy { get; set; }
            public DateTime? LastupdateDate { get; set; }
            public bool IsLocked { get; set; }
            public string LockingBy { get; set; }
            public DateTime? LockingDate { get; set; }
    }

    [Table("SvMechanicAvbView")]
    public class SvMstMecAvbView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string EmployeeID { get; set; }
        public bool IsAvailable { get; set; }
        public string EmployeeName { get; set; }
        public string AttendStatus { get; set; }
    }

    [Table("SvGetMekanik")]
    public class SvGetMekanik
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string EmployeeID { get; set; }
        public string IsAvailableStatus { get; set; }
        public string EmployeeName { get; set; }
        public string PersonnelStatus { get; set; }
    }
}