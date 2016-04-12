using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("PmItsByLeadTime")]
    public class PmItsByLeadTime
    {
        [Key]
        [Column(Order = 1)]
        public string Area { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public long InquiryNumber { get; set; }
        public DateTime? InquiryDate { get; set; }
        public string DealerAbbreviation { get; set; }
        public string OutletAbbreviation { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string Transmisi { get; set; }
        public string Period { get; set; }
        public DateTime? PDate { get; set; }
        public DateTime? HPDate { get; set; }
        public DateTime? SPKDate { get; set; }
        public int? LeadTimeHp { get; set; }
        public int? LeadTimeSpk { get; set; }
        public string LastProgress { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }

    }
}