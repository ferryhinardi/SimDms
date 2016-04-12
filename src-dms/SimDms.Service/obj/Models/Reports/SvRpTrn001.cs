using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models.Reports
{
    public class SvRpTrn001
    {
        public string Nomor {get;set;}
        public string SPKNo {get;set;}        
        public string Pemilik {get;set;}
        public string Alamat {get;set;}
        public string Telp {get;set;}
        public bool IsPKP {get;set;}
        public string CustomerName {get;set;}
        public string NPWPNo {get;set;}
        public string PhoneNo {get;set;}
        public string HPNo {get;set;}
        public string NoChasis {get;set;}
        public string NoMesin {get;set;}
        public DateTime? TglPenyerahan {get;set;}
        public string Warna {get;set;}
        public string NoPolisi {get;set;}
        public string FakturKepada {get;set;}
        public string Dealer {get;set;}
        public DateTime? TglJamMasuk {get;set;}
        public DateTime? TglJamKeluar {get;set;}
        public decimal? KM {get;set;}
        public string Frontman {get;set;}
        public string Keluhan {get;set;}
        public string JenisPekerjaan {get;set;}
        public bool PenggantianSukuCadang {get;set;}
        public string CompanyName {get;set;}
        public string NoOperasi {get;set;}
        public string Pekerjaan {get;set;}
        public string MechanicId {get;set;}
        public string DeskrispiPekerjaan {get;set;}
        public decimal? PrintSeq {get;set;}
        public decimal? ProductionYear { get; set; }
    }
}