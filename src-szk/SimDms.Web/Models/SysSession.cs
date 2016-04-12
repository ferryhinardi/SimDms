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

    [Table("ReportSession")]
    public partial class ReportSession
    {
        [Key]
        [StringLength(50)]
        public string SessionId { get; set; }

        [StringLength(150)]
        public string ReportId { get; set; }

        [StringLength(250)]
        public string ConnectionId { get; set; }

        [Column(TypeName = "text")]
        public string SQL { get; set; }

        [Column(TypeName = "text")]
        public string Parameters { get; set; }

        [StringLength(32)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
    }

}