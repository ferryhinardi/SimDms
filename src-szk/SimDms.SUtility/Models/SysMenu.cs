using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.SUtility.Models
{
    [Table("SysMenu")]
    public class SysMenu
    {
        [Key]
        public string MenuId { get; set; }
        public string MenuCaption { get; set; }
        public string MenuHeader { get; set; }
        public int MenuIndex { get; set; }
        public int MenuLevel { get; set; }
        public string MenuUrl { get; set; }

        [NotMapped]
        public virtual List<SysMenu> Detail { get; set; }
    }

    [Table("SysMenuView")]
    public class SysMenuView
    {
        [Key]
        public string MenuId { get; set; }
        public string MenuCaption { get; set; }
        public string MenuHeader { get; set; }
        public string MenuHeaderDescription { get; set; }
        public int? MenuIndex { get; set; }
        public int? MenuLevel { get; set; }
        public string MenuUrl { get; set; }

        [NotMapped]
        public virtual List<SysMenu> Detail { get; set; }
    }
}