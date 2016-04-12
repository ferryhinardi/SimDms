using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("PmDashboardData")]
    public class PmDashboardData
    {
        [Key]
        [Column(Order = 1)]
        public string DashboardName { get; set; }
        [Key]
        [Column(Order = 2)]
        public string GroupType { get; set; }
        [Key]
        [Column(Order = 3)]
        public int GroupSeq { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public decimal DataCount { get; set; }
    }
}