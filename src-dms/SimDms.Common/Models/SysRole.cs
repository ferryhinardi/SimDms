using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimDms.Common.Models
{
    [Table("SysRole")]
    public class SysRole
    {
        [Key]
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Themes { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsChangeBranchCode { get; set; }
    }
}