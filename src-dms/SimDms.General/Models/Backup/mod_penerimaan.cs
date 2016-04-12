using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SimDms.Common;


namespace SimDms.Sparepart.Models
{
    [Table ("spTrnPBinnHdr")]
    public class spTrnPBinnHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BinningNo { get; set; }
        public DateTime? BinningDate { get; set; }
        public string ReceivingType { get; set; }
        public string DNSupplierNo { get; set; }
        public DateTime? DNSupplierDate { get; set; }
        public string TransType { get; set; }
        public string SupplierCode { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public decimal? TotItem { get; set; }
        public decimal? TotBinningAmt { get; set; }
        public string Status { get; set; }
        public decimal? PrintSeq { get; set; }
        public string TypeOfGoods { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    public class BinnFromTransferStock
    {
        public int NoUrut { get; set; }
        public string DocNo { get; set; }
        public string PartNo { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? ReceivedQty { get; set; }
        public string BoxNo { get; set; }
        public string NmPart { get; set; }
    }

}