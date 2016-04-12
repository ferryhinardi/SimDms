using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("spTrnPPOSHdr")]
    public class spTrnPPOSHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string POSNo { get; set; }
        public DateTime? POSDate { get; set; }
        public string SupplierCode { get; set; }
        public string OrderType { get; set; }
        public bool? isBO { get; set; }
        public bool? isSubstution { get; set; }
        public bool? isSuggorProcess { get; set; }
        public string Remark { get; set; }
        public string ProductType { get; set; }
        public decimal? PrintSeq { get; set; }
        public string ExPickingSlipNo { get; set; }
        public DateTime? ExPickingSlipDate { get; set; }
        public string Status { get; set; }
        public string Transportation { get; set; }
        public string TypeOfGoods { get; set; }
        public bool? isGenPORDD { get; set; }
        public bool? isDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public bool? isDropSign { get; set; }
        public string DropSignReffNo { get; set; }

    }

    [Table("spTrnPPOSDtl")]
    public class spTrnPPOSDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string POSNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        public decimal SeqNo { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? SuggorQty { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? PurchasePriceNett { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? TotalAmount { get; set; }
        public string ABCClass { get; set; }
        public string MovingCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Note { get; set; }
    }
}