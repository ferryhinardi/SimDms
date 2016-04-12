using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimDms.Absence.Models
{
    [Table("SysGroupDms")]
    public class SysRole
    {
        [Key]
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string Themes { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdmin { get; set; }
    }
}