using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.SUtility.Models
{
    [Table("svHstSzkMSI")]
    public class SvHstSzkMsi
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal PeriodYear { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal PeriodMonth { get; set; }
        [Key]
        [Column(Order = 5)]
        public int? SeqNo { get; set; }
        public string MsiGroup { get; set; }
        public string MsiDesc { get; set; }
        public string Unit { get; set; }
        public decimal? MsiData { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}