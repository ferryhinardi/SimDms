using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("SpHstDemandCust")]
    public class SpHstDemandCust
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order=2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal Year { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal Month { get; set; }
        [Key]
        [Column(Order = 5)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public string PartNo { get; set; }
        public decimal? DemandFreq { get; set; }
        public decimal? DemandQty { get; set; }
        public decimal? SalesFreq { get; set; }
        public decimal? SalesQty { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string partCategory { get; set; }
        public string ABCClass { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
