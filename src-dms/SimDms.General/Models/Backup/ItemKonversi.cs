using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Sparepart.Models
{
    [Table("SpMstItemConversion")]
    public class SpMstItemConversion
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PartNo { get; set; }
        public decimal? FromQty { get; set; }
        public decimal? ToQty { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsLocked { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

     }

    public class SpMstItemConversionview
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? FromQty { get; set; }
        public decimal? ToQty { get; set; }
        public bool? IsActive { get; set; }
    }

     
}