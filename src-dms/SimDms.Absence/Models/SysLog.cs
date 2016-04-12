using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models
{
    [Table("SysLog")]
    public class SysLog
    {
        [Key]
        public string LogId { get; set; }
        public DateTime LogDate { get; set; }
        public string LogMessage { get; set; }
    }
}