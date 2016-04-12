using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvTrnClaimJudgement")]
    public class SvTrnClaimJudgement
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string GenerateNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal GenerateSeq { get; set; }
        [Key]
        [Column(Order = 6)]
        public DateTime ReceivedDate { get; set; }
        public string SuzukiRefferenceNo { get; set; }
        public DateTime? SuzukiRefferenceDate { get; set; }
        public char DivisionCode { get; set; }
        public string JudgementCode { get; set; }
        public decimal? PaymentOprHour { get; set; }
        public decimal? PaymentSubletHour { get; set; }
        public decimal? PaymentOprAmt { get; set; }
        public decimal? PaymentSubletAmt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }
 }