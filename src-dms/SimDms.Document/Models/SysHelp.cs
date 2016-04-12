using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Document.Models
{
    [Table("SysHelp")]
    public class SysHelp
    {
        [Key]
        [Column(Order = 1)]
        public int MenuID { get; set; }
        public int MenuLevel { get; set; }
        public int MenuSeq { get; set; }
        public int? MenuHeader { get; set; }
        public string MenuTitle { get; set; }
        public string Content { get; set; }
        public bool? IsContainModule { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}