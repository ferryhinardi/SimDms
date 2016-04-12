using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    [Table("spTrnSORDHdr")]
    public class SpTrnSORDHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string UsageDocNo { get; set; }
        public DateTime? UsageDocDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerCodeShip { get; set; }
        public bool? isBO { get; set; }
        public bool? isSubstitution { get; set; }
        public bool? isIncludePPN { get; set; }
        public string TransType { get; set; }
        public string SalesType { get; set; }
        public bool? IsPORDD { get; set; }
        public string OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public string PaymentCode { get; set; }
        public string PaymentRefNo { get; set; }
        public decimal? TotSalesQty { get; set; }
        public decimal? TotSalesAmt { get; set; }
        public decimal? TotDiscAmt { get; set; }
        public decimal? TotDPPAmt { get; set; }
        public decimal? TotPPNAmt { get; set; }
        public decimal? TotFinalSalesAmt { get; set; }
        public bool? isPKP { get; set; }
        public string ExPickingSlipNo { get; set; }
        public DateTime? ExPickingSlipDate { get; set; }
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
        public bool? isDropSign { get; set; }
    }

    public class SpTrnSORDHdrLkp
    {
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string UsageDocNo { get; set; }
        public DateTime? UsageDocDate { get; set; }  
    }
}
