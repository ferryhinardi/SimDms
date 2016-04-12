using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvKSGView")]
    public class svMstPdiFscView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public bool IsCampaign { get; set; }
        [Key]
        [Column(Order = 5)]
        public string TransmissionType { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal PdiFscSeq { get; set; }
        [Key]
        [Column(Order = 7)]
        public DateTime EffectiveDate { get; set; }
        public string IsCampaignType { get; set; }
        public string TransType { get; set; }
        public decimal RegularLaborAmount { get; set; }
        public decimal RegularMaterialAmount { get; set; }
        public decimal? RegularTotalAmount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public decimal? LaborRate { get; set; }
    }

    [Table("SvBasicKsgView")]
    public class SvBasicKsgView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        public string TechnicalModelCode { get; set; }
        public string ModelDescription { get; set; }
        public string Status { get; set; }
    }
}