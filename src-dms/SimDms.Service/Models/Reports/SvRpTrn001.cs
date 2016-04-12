using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models.Reports
{
    public class SvRpTrn001
    {
        public string Nomor { get; set; }
        public string SPKNo { get; set; }
        public string Pemilik { get; set; }
        public string Alamat { get; set; }
        public string Telp { get; set; }
        public bool IsPKP { get; set; }
        public string CustomerName { get; set; }
        public string NPWPNo { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string NoChasis { get; set; }
        public string NoMesin { get; set; }
        public DateTime? TglPenyerahan { get; set; }
        public string Warna { get; set; }
        public string NoPolisi { get; set; }
        public string FakturKepada { get; set; }
        public string Dealer { get; set; }
        public DateTime? TglJamMasuk { get; set; }
        public DateTime? TglJamKeluar { get; set; }
        public decimal? KM { get; set; }
        public string Frontman { get; set; }
        public string Keluhan { get; set; }
        public string JenisPekerjaan { get; set; }
        public bool PenggantianSukuCadang { get; set; }
        public string CompanyName { get; set; }
        public string NoOperasi { get; set; }
        public string Pekerjaan { get; set; }
        public string MechanicId { get; set; }
        public string DeskrispiPekerjaan { get; set; }
        public decimal? PrintSeq { get; set; }
        public decimal? ProductionYear { get; set; }
    }

    public class SvRpTrn00101
    {
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string EstimationNo { get; set; }
        public DateTime? EstimationDate { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string Flag { get; set; }
        public string Kode { get; set; }
        public string Keterangan { get; set; }
        public decimal? Jumlah { get; set; }
        public decimal? HargaSatuan { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalGrossAmt { get; set; }
        public decimal? Potongan { get; set; }
        public decimal? LaborDPPAmt { get; set; }
        public decimal? PartsDPPAmt { get; set; }
        public decimal? MaterialDPPAmt { get; set; }
        public decimal? TotalDPPAmount { get; set; }
        public decimal? TotalPpnAmount { get; set; }
        public decimal? TotalSrvAmount { get; set; }
        public Int64 ServiceNo { get; set; }
        public string City { get; set; }
        public DateTime? Tanggal { get; set; }
        public decimal? PPNPct { get; set; }
    }
}