using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Web.Models
{
    [Table("SysSession")]
    public class SysSession
    {
        [Key]
        public string SessionId { get; set; }
        public string SessionUser { get; set; }
        public DateTime CreateDate { get; set; }
        public bool? IsLogout { get; set; }
        public DateTime? LogoutTime { get; set; }
    }
}