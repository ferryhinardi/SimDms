using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.General.Models
{
    [Table("gnMstCollector")]
    public class GnMstCollector
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CollectorCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ProfitCenterCode { get; set; }
        public string CollectorName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    public class GnMstCollectorView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string CollectorCode { get; set; }
        public string ProfitCenterCode { get; set; }
        public string CollectorName { get; set; }
        public string ProfitCenterNameDisc { get; set; }
    }

}