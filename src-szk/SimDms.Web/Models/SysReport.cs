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

    [Table("vw_lastTransDate")]
    public class LastTransDateInfo
    {
        [Key]
        [Column(Order = 1)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string DealerName { get; set; }
        public string ShortName { get; set; }
        public string DealerAbbr { get; set; }
        public string BranchAbbr { get; set; }
        public string ProductType { get; set; }
        public string Version { get; set; }

        public DateTime? GoLiveDate { get; set; }
        public DateTime? LastSalesDate { get; set; }
        public DateTime? LastSpareDate { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public DateTime? LastAPDate { get; set; }
        public DateTime? LastARDate { get; set; }
        public DateTime? LastGLDate { get; set; }
        public DateTime? LastupdateDate { get; set; }

    }

}