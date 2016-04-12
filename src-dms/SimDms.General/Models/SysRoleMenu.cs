using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.General.Models
{
    //[Table("SysRoleMenu")]
    //public class SysRoleMenu
    //{
    //    [Key]
    //    [Column(Order = 1)]
    //    public string RoleId { get; set; }
    //    [Key]
    //    [Column(Order = 2)]
    //    public string MenuId { get; set; }
    //}

    public class SysRoleMenuModel
    {
        public string RoleID { get; set; }
        public string[] MenuID { get; set; }
    }

    public class SysRoleMenuPermission
    {
        public string MenuCaption { get; set; }
        public string MenuHeader { get; set; }
        public int MenuLevel { get; set; }
        public int MenuIndex { get; set; }
        public string RoleId { get; set; }
        public string MenuId { get; set; }
        public bool Navigation { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowPrint { get; set; }
    }

}