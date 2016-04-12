using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sparepart.Models
{
    [Table("spTrnSFPJHdr")]
    public class SpTrnSFPJHdr
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public string TPTrans { get; set; }
        public string FPJGovNo { get; set; }
        public DateTime FPJSignature { get; set; }
        public string FPJCentralNo { get; set; }
        public DateTime FPJCentralDate { get; set; }
        public string DeliveryNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string PickingSlipNo { get; set; }
        public DateTime PickingSlipDate { get; set; }
        public string TransType { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerCodeShip { get; set; }
        public string TOPCode { get; set; }
        public Decimal TOPDays { get; set; }
        public DateTime DueDate { get; set; }
        public Decimal TotSalesQty { get; set; }
        public Decimal TotSalesAmt { get; set; }
        public Decimal TotDiscAmt { get; set; }
        public Decimal TotDPPAmt { get; set; }
        public Decimal TotPPNAmt { get; set; }
        public Decimal TotFinalSalesAmt { get; set; }
        public bool IsPKP { get; set; }
        public string Status { get; set; }
        public Decimal PrintSeq { get; set; }
        public string TypeOfGoods { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime LockingDate { get; set; }
    }

    [Table("spTrnSFPJHdrLog")]
    public class SpTrnSFPJHdrLog
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string FPJNo { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomerCodeBill { get; set; }
        public decimal? TotDiscAmt { get; set; }
        public decimal? TotDPPAmt { get; set; }
        public decimal? TotPPNAmt { get; set; }
        public decimal? TotFinalSalesAmt { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    
    public class spTrnSFPJHdrLKP
    {
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public string PickingSlipNo { get; set; }
        public DateTime PickingSlipDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
    }



}
