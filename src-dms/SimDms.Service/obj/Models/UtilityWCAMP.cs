using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvUtlCampaign")]
    public class SvUtlCampaign
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

    [Table("SvUtlCampaignRange")]
    public class SvUtlCampaignRange
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
        public string ComplainCode { get; set; }
        public string DefectCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisStartNo { get; set; }
        public decimal? ChassisEndNo { get; set; }
        public DateTime? CloseDate { get; set; }
        public string TaskCode { get; set; }
        public string Description { get; set; }
    }
}