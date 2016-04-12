using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Web.Models
{
    [Table("SysMenuDms")]
    public class SysMenu
    {
        [Key]
        public string MenuId { get; set; }
        public string MenuCaption { get; set; }
        public string MenuHeader { get; set; }
        public int MenuIndex { get; set; }
        public int MenuLevel { get; set; }
        public string MenuUrl { get; set; }
        public string MenuIcon { get; set; }

        [NotMapped]
        public virtual List<SysMenu> Detail { get; set; }
    }

    public class SysNavigation
    {
        public SysNavigation()
        {
            Detail = new List<SysNavigation>();
        }

        public string MenuId { get; set; }
        public string MenuCaption { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string ModuleName { get; set; }
        public string ParentName { get; set; }

        public virtual List<SysNavigation> Detail { get; set; }
    }


}