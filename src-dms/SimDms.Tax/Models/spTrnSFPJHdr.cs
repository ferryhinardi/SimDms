using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Tax.Models
{
    [Table("svTrnFakturPajak")]
    public class spTrnSFPJHdr
    {
        [Key]
        [Column(Order = 1)]
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
        public DateTime? FPJSignature { get; set; }
        public string FPJCentralNo { get; set; }
        public DateTime? FPJCentralDate { get; set; }
        public string DeliveryNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string PickingSlipNo { get; set; }
        public DateTime? PickingSlipDate { get; set; }
        public string TransType { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerCodeShip { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public DateTime? DueDate { get; set; }
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

    public class spTrnSFPJHdrView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public string TPTrans { get; set; }
        public string FPJGovNo { get; set; }
        public DateTime? FPJSignature { get; set; }
        public string FPJCentralNo { get; set; }
        public DateTime? FPJCentralDate { get; set; }
        public string DeliveryNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string PickingSlipNo { get; set; }
        public DateTime? PickingSlipDate { get; set; }
        public string TransType { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerNameBill { get; set; }
        public string CustomerCodeShip { get; set; }
        public string TOPCode { get; set; }
        public decimal? TOPDays { get; set; }
        public DateTime? DueDate { get; set; }
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
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string NPWPNo { get; set; }
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public DateTime? NPWPDate { get; set; }
    }
}