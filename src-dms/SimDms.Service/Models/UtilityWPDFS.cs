using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvUtlPdiFsc")]
    public class SvUtlPdiFsc
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

    [Table("SvUtlPdiFscAmount")]
    public class SvUtlPdiFscAmount
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
        public string TransmissionType { get; set; }
        public string PdiFsc { get; set; }
        public decimal? RegularLaborAmount { get; set; }
        public decimal? RegularMaterialAmount { get; set; }
        public decimal? RegularTotalAmount { get; set; }
        public decimal? CampaignLaborAmount { get; set; }
        public decimal? CampaignMaterialAmount { get; set; }
        public decimal? CampaignTotalAmount { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Description { get; set; }
    }
}