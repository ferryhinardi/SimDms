using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("OmUtlSFPOLHdr")]
    public class OmUtlSFPOLHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string RcvDealerCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string BatchNo { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("OmUtlSFPOLDtl1")]
    public class OmUtlSFPOLDtl1
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BatchNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string FakturPolisiNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SalesModelYear { get; set; }
        [Key]
        [Column(Order = 7)]
        public string ColourCode { get; set; }
        [Key]
        [Column(Order = 8)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 9)]
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public bool? IsBlanko { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public DateTime? FakturPolisiProcess { get; set; }
        public string DONo { get; set; }
        public string SJNo { get; set; }
        public string ReqNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}