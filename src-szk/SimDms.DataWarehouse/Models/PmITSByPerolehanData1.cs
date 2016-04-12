using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("PmITSByPerolehanData1")]
    public class PmITSByPerolehanData1 : IPmITSByPerolehanData
    {
        //[Key]
        //[Column (Order=1)]
        public string Area { get; set; }
        public int? AreaCode { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        public string DealerAbbreviation { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string OutletAbbreviation { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Period { get; set; }
        public DateTime? UpdateDate { get; set; }
        [Key]
        [Column(Order = 4)]
        public string TipeKendaraan { get; set; }
        [Key]
        [Column(Order = 5)]
        public string Variant { get; set; }
        [Key]
        [Column(Order = 6)]
        public string Transmisi { get; set; }
        [Key]
        [Column(Order = 7)]
        public string PerolehanData { get; set; }
        [Key]
        [Column(Order = 8)]
        public string LastProgress { get; set; }
        public int? Total { get; set; }
    }
}