using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimDms.Web.Models
{
    [Table("SysRoleUser")]
    public class SysRoleUser
    {
        [Key]
        [Column(Order = 1)]
        public string RoleId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string UserId { get; set; }

        [ForeignKey("RoleId")]
        public virtual SysRole Role { get; set; }
        [ForeignKey("UserId")]
        public virtual SysUser User { get; set; }
    }
}