using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SysParameter")]
    public class SysParameter
    {
        [Key]
        [Column(Order = 1)]
        public string ParamId { get; set; }
        public string ParamValue { get; set; }
        public string ParamDescription { get; set; }
    }
}