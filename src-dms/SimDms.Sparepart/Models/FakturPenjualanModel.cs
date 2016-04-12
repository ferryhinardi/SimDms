using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    public class FakturPenjualanModel
    {


    }

    public class CustomerTransDtl
    {
        public string PartNo { get; set; }
        public string PartNoOriginal { get; set; }
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string ReferenceNo { get; set; }
        public decimal QtyBill { get; set; }
    }

    public class PembFakturPenjualan
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public string FPJSignature { get; set; }
        public string PickingSlipNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime PickingSlipDate { get; set; }
        public string TOPCode { get; set; }
        public int TopDay { get; set; }
        public string Pelanggan { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal TotDPPAmt { get; set; }
        public decimal TotPPNAmt { get; set; }
        public decimal TotSalesAmt { get; set; }
        public decimal TotSalesQty { get; set; }
        public decimal TotDiscAmt { get; set; }
        public string TransType { get; set; }
        public decimal TotFinalSalesAmt { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string CustomerCodeTagih { get; set; }
        public string CustomerNameTagih { get; set; }
        public string Address1Tagih { get; set; }
        public string Address2Tagih { get; set; }
        public string Address3Tagih { get; set; }
        public string Address4Tagih { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerCodeShip { get; set; }
    }
}
