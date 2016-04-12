using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("svCustomerSatisfactionScoreLog")]
    public class svCustomerSatisfactionScoreLog
    {
        [Key]
        [Column(Order = 1)]
        public string ServiceCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public decimal PeriodYear { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal PeriodMonth { get; set; }
        [Key]
        [Column(Order = 4)]
        public int SeqNo { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public decimal? ServiceInitiation { get; set; }
        public decimal? ServiceAdvisor { get; set; }
        public decimal? ServiceFaciltiy { get; set; }
        public decimal? VehiclePickup { get; set; }
        public decimal? ServiceQuality { get; set; }
        public decimal? Score { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}