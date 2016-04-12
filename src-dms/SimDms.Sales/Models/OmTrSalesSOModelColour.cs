using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("OmTrSalesSOModelColour")]
    public class OmTrSalesSOModelColour
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SONo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal SalesModelYear { get; set; }
        [Key]
        [Column(Order = 6)]
        public string ColourCode { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        [NotMapped]
        public string ColourDescription { get; set; } 
    }

    public class SalesSOModelColourView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SONo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string ColourDescription { get; set; } 
    }
}
