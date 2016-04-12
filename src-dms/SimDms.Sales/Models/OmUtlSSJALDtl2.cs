using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omUtlSSJALDtl2")]
    public class OmUtlSSJALDtl2
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
        [Key]
        [Column(Order = 5)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SalesModelYear { get; set; }
        public decimal? Quantity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}