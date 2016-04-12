using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    public class FPJLookup : CustomerDetailsTagih
    {
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public DateTime FPJSignature { get; set; }
        public string PickingSlipNo { get; set; }
        public DateTime PickingSlipDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string TOPCode { get; set; }
        public decimal TOPDays { get; set; }
        public decimal TotDPPAmt { get; set; }
        public decimal TotPPNAmt { get; set; }
        public decimal TotFinalSalesAmt { get; set; }
        public string TransType { get; set; }
        public string CustomerCodeShip { get; set; }
        public string CustomerCodeBill { get; set; }
        public decimal TotSalesAmt { get; set; }
        public decimal TotSalesQty { get; set; }
        public string PartNo { get; set; }
        public string PartNoOriginal { get; set; }
        public string DocNo { get; set; }
        public decimal QtyBill { get; set; }
        public decimal SalesAmt { get; set; }
        public decimal DiscPct { get; set; }
        public string PartName { get; set; }
        public string WarehouseCode { get; set; }
        public string Status { get; set; }
        public string Customer { get; set; }
    }

    public class LookupNonPenjualan : CustomerDetailsTagih
    {
        public string PickingSlipNo { get; set; }
        public DateTime PickingSlipDate { get; set; }
        public string BPSFNo { get; set; }
        public DateTime BPSFDate { get; set; }
        public string ProductType { get; set; }
        public string TransType { get; set; }
        public string CustomerCodeShip { get; set; }
    }

    public class DetailsPesananNP
    {
        public string PartNo { get; set; }
        public string PartNoOriginal { get; set; }
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string ReferenceNo { get; set; }
        public decimal QtyBill { get; set; }
    }

    public class SelectLMPDtl
    {
        public Int64 NoUrut { get; set; }
        public string PartNo { get; set; }
        public string PartNoOriginal { get; set; }
        public string DocNo { get; set; } 
        public string DocDate { get; set; } 
        public string ReferenceNo { get; set; }
        public decimal? QtyBill { get; set; }
    }

    public class CustomerDetailsTagih
    {
        public string CustomerName { get; set; }
        public string CustomerGovName { get; set; }
        public string CustomerCode { get; set; }
        public string Address { get; set; }
        public string ProfitCenter { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string CityCode { get; set; }
        public string City { get; set; }
        public string HpNo { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string CustomerCodeTagih { get; set; }
        public string CustomerNameTagih { get; set; }
        public string Address1Tagih { get; set; }
        public string Address2Tagih { get; set; }
        public string Address3Tagih { get; set; }
        public string Address4Tagih { get; set; }
        public string PickingSlipNo { get; set; }
        public string TOPCode { get; set; }
        public string OrderType { get; set; }
    }

    public class ReturPenjualanView
    {
        public string FPJNo { get; set; }
        public DateTime FPJDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime ReferenceDate { get; set; }
        public string ReturnNo { get; set; }
        public string CustomerCode { get; set; }
    }

    public class ReturnDetailsView
    {
        public Int64 NoUrut { get; set; }
        public string PartNo { get; set; }
        public string PartNoOriginal { get; set; }
        public string DocNo { get; set; }
        public decimal QtyReturn { get; set; }
        public decimal QtyBill { get; set; }
    }

    public class ReturSumAmt
    {
        public decimal TotReturQty { get; set; }
        public decimal TotDiscAmt { get; set; }
        public decimal TotReturAmt { get; set; }
        public decimal TotDPPAmt { get; set; }
        public decimal TotFinalReturAmt { get; set; }
        public decimal TotCostAmt { get; set; }
    }

    public class RturItem
    {
        public string PartNo { get; set; }
        public decimal QtyReturn { get; set; }
        public decimal CostPrice { get; set; }
    }

    public class ReturPart
    {
        public string  PartNo { get; set; }
        public string  PartNoOriginal { get; set; }
        public decimal QtyBill { get; set; }
        public string  DocNo { get; set; }
    }
}
