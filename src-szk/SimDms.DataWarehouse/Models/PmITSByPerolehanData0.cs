using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("PmITSByPerolehanData0")]
    public class PmITSByPerolehanData0 : IPmITSByPerolehanData
    {
        //[Key]
        //[Column(Order = 1)]
        public string Area { get; set; }
        public int? AreaCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCode { get; set; }
        public string DealerAbbreviation { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
        public string OutletAbbreviation { get; set; }
        [Key]
        [Column(Order = 4)]
        public string Period { get; set; }
        public DateTime? InquiryDate { get; set; }
        [Key]
        [Column(Order = 5)]
        public string TipeKendaraan { get; set; }
        [Key]
        [Column(Order = 6)]
        public string Variant { get; set; }
        [Key]
        [Column(Order = 7)]
        public string Transmisi { get; set; }
        public string PerolehanData { get; set; }
        public string LastProgress { get; set; }
        public int? Total { get; set; }
    }

    interface IPmITSByPerolehanData
    {
        string Area { get; set; }
        int? AreaCode { get; set; }
        string CompanyCode { get; set; }
        string DealerAbbreviation { get; set; }
        string BranchCode { get; set; }
        string OutletAbbreviation { get; set; }
        string Period { get; set; }
        string TipeKendaraan { get; set; }
        string Variant { get; set; }
        string Transmisi { get; set; }
        string PerolehanData { get; set; }
        string LastProgress { get; set; }
        int? Total { get; set; }
    }
}