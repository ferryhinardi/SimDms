using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SimDms.Reports
{
    public class ReportContext : DbContext
    {
        public IDbSet<SysReport> SysReports { get; set; }
    }

    [Table("SysReportDms")]
    public class SysReport
    {
        [Key]
        public string ReportID { get; set; }
        public string ReportPath { get; set; }
        public string ReportProc { get; set; }
    }
}