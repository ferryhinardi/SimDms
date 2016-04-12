using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Common.Models
{
    [Table("SysModule")]
    public class SysModule
    {
        [Key]
        public string ModuleID { get; set; }
        public string ModuleCaption { get; set; }
        public int ModuleIndex { get; set; }
        public string ModuleUrl { get; set; }
        public bool InternalLink { get; set; }
        public bool IsPublish { get; set; }
        public string Icon { get; set; }
    }

    [Table("SysModuleView")]
    public class SysModuleView
    {
        [Key]
        public string ModuleID { get; set; }
        public string ModuleCaption { get; set; }
        public int ModuleIndex { get; set; }
        public string ModuleUrl { get; set; }
        public bool InternalLink { get; set; }
        public bool IsPublish { get; set; }
        public string InternalLinkDescription { get; set; }
        public string IsPublishDescription { get; set; }
    }
}