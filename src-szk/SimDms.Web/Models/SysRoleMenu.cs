using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Web.Models
{
    [Table("SysRoleMenu")]
    public class SysRoleMenu
    {
        [Key]
        [Column(Order=1)]
        public string RoleID { get; set; }
        [Key]
        [Column(Order = 2)]
        public string MenuID { get; set; }
    }
}