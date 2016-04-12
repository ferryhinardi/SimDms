using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Common.Models
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

    public class SysRoleModuleModels
    {
        public string RoleID { get; set; }
        public string[] ModuleID { get; set; }
    }   
}