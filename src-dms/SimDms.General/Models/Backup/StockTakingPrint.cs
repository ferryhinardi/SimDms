using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    public class IsValidSTAnalyze
    {
        public string PartNo { get; set; }
        public string STNo { get; set; }
        public decimal? SEqNo { get; set; }
        public string Status { get; set; }
        public string LocationCode { get; set; }

    }

    public class SumStockTaking
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string STHdrNo { get; set; }
        public string STNo { get; set; }
        public decimal SeqNo { get; set; }
        public string PartNo { get; set; }
        public string LocationCode { get; set; }
        public bool? isMainLocation { get; set; }
        public string MovingCode { get; set; }
        public decimal? OnHandQty { get; set; }
        public decimal? STQty { get; set; }
        public decimal? STDmgQty { get; set; }
        public DateTime? EntryDate { get; set; }
        public decimal? PrintSeq { get; set; }
        public string TypeOfGoods { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }

    [Table("SpTrnIAdjustHdr")]
    public class SpTrnIAdjustHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string AdjustmentNo { get; set; }
        public DateTime? AdjustmentDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string TypeOfGoods { get; set; }
        public decimal? PrintSeq { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }


    [Table("SpTrnIAdjustDtl")]
    public class SpTrnIAdjustDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string AdjustmentNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string WarehouseCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        public string LocationCode { get; set; }
        public string AdjustmentCode { get; set; }
        public decimal? QtyAdjustment { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public string ReasonCode { get; set; }
        public string MovingCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class GetAdjustmentTransferSA
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string AdjustmentNo { get; set; }
        public string WHTrfNo { get; set; }
    }
}
