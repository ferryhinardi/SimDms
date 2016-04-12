using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Web.Models
{
    [Table("SysLogger")]
    public class SysLogger
    {
        [Key]
        public string ID { get; set; }
        public string Contents { get; set; }
    }
}