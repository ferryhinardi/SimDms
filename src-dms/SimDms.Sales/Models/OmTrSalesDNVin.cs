using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("OmTrSalesDNVin")]
    public class OmTrSalesDNVin
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; } //key

        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; } //key
        
        [Key]
        [Column(Order = 3)]
        public string DNNo { get; set; } //key

        [Key]
        [Column(Order = 4)]
        public decimal? DNSeq { get; set; } //key
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public decimal? BBN { get; set; }
        public decimal? KIR { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}