using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models.Reports
{
    public class OmRpSalesTrn004
    {
        public string No { get; set; }
        public string CompanyName { get; set; }
        public string Alamat1 { get; set; }
        public string Alamat2 { get; set; }
        public string CompanyPhone { get; set; }
        public string City { get; set; }
        public string NPWPNo { get; set; }
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public string InvoiceNo2 { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string SONo { get; set; }
        public DateTime? BPKDate { get; set; }
        public string CustomerName { get; set; }
        public string AlamatCust1 { get; set; }
        public string AlamatCust2 { get; set; }
        public string AlamatCust3 { get; set; }
        public string CustNPWPNo { get; set; }
        public string FPS { get; set; }
        public decimal? PPN { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? isStandard { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string InvoiceNo { get; set; }
        public string BPKNo { get; set; }
        public string Model { get; set; }
        public string ModelDesc { get; set; }
        public string Rangka { get; set; }
        public string Mesin { get; set; }
        public decimal? PPnBMPaid { get; set; }
        public decimal? TotalPPnBMPaid { get; set; }
        public decimal? DPPBefore { get; set; }
        public decimal? Jumlah { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscPPnBM { get; set; }
        public string Flag { get; set; }
        public string Filter { get; set; }
        public string ID { get; set; }
        public string SignName1 { get; set; }
        public string SKPKNo { get; set; }
        public string RefferenceNo { get; set; }
        public string TitleSign1 { get; set; }
        public string SignName2 { get; set; }
        public string TitleSign2 { get; set; }
        public string Keterangan { get; set; }
        public bool? HidePart { get; set; }
        public string DONo { get; set; }
        public decimal? potongan { get; set; }
        public decimal? TotalPPnBm { get; set; }
    }
}