using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Web.Models
{
    [Table("SysModule")]
    public class SysModule
    {
        [Key]
        public string ModuleId { get; set; }
        public string ModuleCaption { get; set; }
        public int ModuleIndex { get; set; }
        public string ModuleUrl { get; set; }
        public bool InternalLink { get; set; }
        public bool IsPublish { get; set; }
        public string Icon { get; set; }

    }
}