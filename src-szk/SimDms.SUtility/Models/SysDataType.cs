using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.SUtility.Models
{
    [Table("SysDataType")]
    public class SysDataType
    {
        [Key]
        public string DataType { get; set; }
        public string DataTypeDesc { get; set; }
    }
}