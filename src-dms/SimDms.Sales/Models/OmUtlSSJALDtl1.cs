using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omUtlSSJALDtl1")]
    public class OmUtlSSJALDtl1
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
        public string SJNo { get; set; }
        public DateTime? SJDate { get; set; }
        public string SKPNo { get; set; }
        public string DONo { get; set; }
        public DateTime? DODate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string IsBlokir { get; set; }
        public string Status { get; set; }
        public string FlagRevisi { get; set; }
    }
}