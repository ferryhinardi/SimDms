using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Common.Models
{
    [Table("SysRoleUser")]
    public class SysRoleUser
    {
        [Key]
        public string UserId { get; set; }
        public string RoleId { get; set; }

        [ForeignKey("RoleId")]
        public SysRole SysRole { get; set; }
    }
}