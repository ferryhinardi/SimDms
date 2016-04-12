using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Common.Models
{
    [Table("SysRoleMenu")]
    public class SysRoleMenu
    {
        [Key]
        [Column(Order = 1)]
        public string RoleId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string MenuId { get; set; }
    }

    public class SysRoleMenuModel
    {
        public string RoleID { get; set; }
        public string[] MenuID { get; set; }
    }
}