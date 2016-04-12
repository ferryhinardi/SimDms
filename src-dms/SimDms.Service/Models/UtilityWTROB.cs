using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvUtlTroubleCode")]
    public class SvUtlTroubleCode
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

    [Table("SvUtlTroubleDescription")]
    public class SvUtlTroubleDescription
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
        public string TroubleCodeFlag { get; set; }
        public string ComplainDefectCode { get; set; }
        public string Description { get; set; }
    }
}