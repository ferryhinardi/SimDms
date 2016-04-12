using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Web.Models
{
    [Table("SysReportDms")]
    public class SysReport
    {
        [Key]
        public string ReportID { get; set; }
        public string ReportPath { get; set; }
        public string ReportProc { get; set; }
    }
}