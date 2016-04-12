using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvUtlWarranty")]
    public class SvUtlWarranty
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public DateTime ProcessDate { get; set; }
        public string DealerCode { get; set; }
        public string ReceivedDealerCode { get; set; }
        public string DealerName { get; set; }
        public decimal? TotalNoOfItem { get; set; }
        public string ProductType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    [Table("SvUtlWarrantyTime")]
    public class SvUtlWarrantyTime
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public DateTime ProcessDate { get; set; }
        [Key]
        [Column(Order = 3)]
        public long SeqNo { get; set; }
        public string BasicModel { get; set; }
        public string TaskCode { get; set; }
        public decimal? Odometer { get; set; }
        public decimal? TimePeriod { get; set; }
        public string TimeDim { get; set; }
        public DateTime? EffectiveSalesDate { get; set; }
        public string Description { get; set; }
    }
}