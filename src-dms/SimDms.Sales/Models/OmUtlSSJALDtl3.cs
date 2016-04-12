using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omUtlSSJALDtl3")]
    public class OmUtlSSJALDtl3
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
        public Decimal SalesModelYear { get; set; }
        [Key]
        [Column(Order = 7)]
        public string ColourCode { get; set; }
        [Key]
        [Column(Order = 8)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 9)]
        public Decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public Decimal EngineNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}