using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
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

    [Table("SysRoleMenuView")]
    public class SysRoleMenuView
    {
        [Key]
        [Column(Order = 1)]
        public string RoleID { get; set; }
        [Key]
        [Column(Order = 2)]
        public string MenuID { get; set; }
        public string RoleName { get; set; }
        public string MenuCaption { get; set; }
        public string MenuUrl { get; set; }
        public int MenuLevel { get; set; }
        public int MenuIndex { get; set; }
    }
}