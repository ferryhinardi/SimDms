using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("spTrnSLmpHdr")]
    public class SpTrnSLmpHdr
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order=2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order=3)]
        public string LmpNo { get; set; }
        public DateTime? LmpDate { get; set; }
        public string BPSFNo { get; set; }
        public string PickingSlipNo { get; set; }
        public DateTime? PickingSlipDate { get; set; }
        public string TransType { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerCodeShip { get; set; }
        public decimal? TotSalesQty { get; set; }
        public decimal? TotSalesAmt { get; set; }
        public decimal? TotDiscAmt { get; set; }
        public decimal? TotDPPAmt { get; set; }
        public decimal? TotPPNAmt { get; set; }
        public decimal? TotFinalSalesAmt { get; set; }
        public bool? isPKP { get; set; }
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

    public class GetLMPHdr : CustomerDetailsTagih
    {
        public string LmpNo { get; set; }
        public DateTime? LmpDate { get; set; }
        public string BPSFNo { get; set; }
        public DateTime? BPSFDate { get; set; }
        public string PickingSlipNo { get; set; }
        public DateTime? PickingSlipDate { get; set; }
        public string TransType { get; set; }
        public string DocNo { get; set; }
        public string UsageDocNo { get; set; }
        public string CustomerCodeShip { get; set; }
    }

    public class LMPHdrLKP
    {
        public string LmpNo { get; set; }
        public DateTime? LmpDate { get; set; }
        public string BPSFNo { get; set; }
        public DateTime? BPSFDate { get; set; }
        public string PickingSlipNo { get; set; }
        public DateTime? PickingSlipDate { get; set; }
        public string SSNo { get; set; }
        public DateTime? SSDate { get; set; }        
    }

    
}
