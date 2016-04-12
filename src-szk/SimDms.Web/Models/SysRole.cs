using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimDms.Web.Models
{
    [Table("SysRole")]
    public class SysRole
    {
        [Key]
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public ICollection<SysUser> Users { get; set; }
    }
}