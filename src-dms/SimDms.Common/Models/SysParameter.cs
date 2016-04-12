using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Common.Models
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


    [Table("SysParam")]
    public class SysParam
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        public string DBName { get; set; }
        public string Extensions { get; set; }
        public string Prefix { get; set; }
        public string FolderPath { get; set; }
        public string DCSURL { get; set; }
        public string TAXURL { get; set; }
    }
}
