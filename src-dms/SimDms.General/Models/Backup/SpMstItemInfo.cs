using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    [Table("SpMstItemInfo")]
    public class SpMstItemInfo
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PartNo { get; set; }
        public string SupplierCode { get; set; }
        public string PartName { get; set; }
        public bool? IsGenuinePart { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? SalesUnit { get; set; }
        public decimal? OrderUnit { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string UOMCode { get; set; }
        public string Status { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }
}
