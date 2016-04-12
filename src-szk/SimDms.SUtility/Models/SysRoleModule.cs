using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("SysRoleModule")]
    public class SysRoleModule
    {
        [Key]
        [Column(Order=1)]
        public string RoleID { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ModuleID { get; set; }
    }

    [Table("SysRoleModuleView")]
    public class SysRoleModuleView
    {
        [Key]
        [Column(Order = 1)]
        public string RoleID { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ModuleID { get; set; }
        public string ModuleCaption { get; set; }
        public string ModuleUrl { get; set; }
        public int ModuleIndex { get; set; }
        public string RoleName { get; set; }
    }
}