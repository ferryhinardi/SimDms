using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("PmStatusHistory")]
    public class PmStatusHistory
    {
        [Key]
        [Column(Order = 1)]
        public int InquiryNumber { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public int SequenceNo { get; set; }
        public string LastProgress { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    [Table("PmStatusHistoryExhibition")]
    public class PmStatusHistoryExhibition
    {
        [Key]
        [Column(Order = 1)]
        public int InquiryNumber { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public int SequenceNo { get; set; }
        public string LastProgress { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}