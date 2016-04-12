using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Web.Models
{
    [Table("sysGroup")]
    public class SimRole
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }
        public string Themes { get; set; }
        public int MaxScreen { get; set; }
        public bool IsAdmin { get; set; }
    }
}