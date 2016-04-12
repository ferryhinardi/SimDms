using System;

namespace SimDms.Sales.Models
{
    public class uspfn_RptSO_A01_Rpt
    {
        public string SONumber { get; set; }
        public DateTime? SODate { get; set; }
        public string SKPKNumber { get; set; }
        public string ReffNumber { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGovName { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string LeasingCo { get; set; }
        public string LeasingCode { get; set; }
        public string LeasingName { get; set; }
        public string TOPCode { get; set; }
        public string TOPDesc { get; set; }
        public string City { get; set; }
    }

    public class uspfn_RptSO_A01_Dtl
    {
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public decimal? QuantitySO { get; set; }
    }

    public class usprpt_OmRpSalesTrn001B
    {
        public string SONumber { get; set; }
        public string SKPKNo { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? SODate { get; set; }
        public string Model { get; set; }
        public string SalesModelCode { get; set; }
        public string Remark { get; set; }
        public string ColourCode { get; set; }
        public decimal? Satuan { get; set; }
        public decimal? BeforeDiscTotal { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Total { get; set; }
        public decimal? BBN { get; set; }
        public decimal? Accs { get; set; }
        public decimal? Accesories { get; set; }
        public decimal? Diskon { get; set; }
        public decimal? Potongan { get; set; }
        public decimal? Lain { get; set; }
        public decimal? LainLain { get; set; }
        public decimal? Jumlah { get; set; }
        public decimal? SubTotal { get; set; }
        public string Sales { get; set; }
        public string TipeSales { get; set; }
        public string Pelanggan { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? JatuhTempo { get; set; }
        public string TOPCode { get; set; }
        public string shipto { get; set; }
        public string ShipName { get; set; }
        public decimal? PrePaymentAmt { get; set; }
        public string Leasing { get; set; }
        public string Ket { get; set; }
        public string SignName { get; set; }
        public string TitleSign { get; set; }
        public int? ChassisNo { get; set; }
        public int? EngineNo { get; set; }
        public string Prefix { get; set; }
        public bool? HidePart { get; set; }
    }
}