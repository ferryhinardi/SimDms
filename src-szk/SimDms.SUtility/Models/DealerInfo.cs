using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.SUtility.Models
{
    [Table("DealerInfo")]
    public class DealerInfo
    {
        [Key]
        [Column(Order = 1)]
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string ScheduleTime { get; set; }
        public DateTime? GoLiveDate { get; set; }
        public int? SeqNo { get; set; }
        public string ProductType { get; set; }
        public string ShortName { get; set; }
        public string WebUrl { get; set; }
        public string WebStatus { get; set; }
        public bool? IsSfm { get; set; }
    }
}