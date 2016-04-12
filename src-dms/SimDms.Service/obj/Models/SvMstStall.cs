using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Models
{
    [Table("svMstStall")]
    public class svMstStall
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string StallCode { get; set; }
        public string Description { get; set; }
        public bool HaveLift { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BranchCode { get; set; }
    }

    [Table("SvStallView")]
    public class svMstStallView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string StallCode { get; set; }
        public string Description { get; set; }
        public bool HaveLift { get; set; }
        public bool IsActive { get; set; }
        public string HaveLiftString { get; set; }
        public string IsActiveString { get; set; }
        
    }
}
