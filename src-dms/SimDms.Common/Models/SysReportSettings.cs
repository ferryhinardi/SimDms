using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimDms.Common.Models 
{
    [Table("SysReport")]
    public class SysReport  
    {
        [Key]
        [Column(Order = 1)]
        public string ReportID { get; set; }
        public string ReportPath { get; set; }
        public string ReportSource { get; set; }
        public string ReportProc { get; set; }
        public string ReportDeviceID { get; set; }
        public string ReportInfo { get; set; }
        public string DefaultParam { get; set; }
        public string Refference { get; set; }
    }


    [Table("SysReportSettings")]
    public class SysReportSettings
    {
        [Key]
        [Column(Order = 1)]
        public string ReportID { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Keyword { get; set; }
        public bool? IsVisible { get; set; }
    }
}