using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimDms.General.Models
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
}