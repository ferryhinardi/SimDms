using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omFakturPolda")]
    public class omFakturPolda
    {
        [Key]
        [Column(Order = 1)]
        public string NoFaktur { get; set; }
        public DateTime? TglFaktur { get; set; }
        public string Merk { get; set; }
        public string Tipe { get; set; }
        public decimal? ThnPembuatan { get; set; }
        public decimal? ThnPerakitan { get; set; }
        public string Silinder { get; set; }
        public string Warna { get; set; }
        public string NoRangka { get; set; }
        public string NoMesin { get; set; }
        public string BahanBakar { get; set; }
        public string Pemilik { get; set; }
        public string Pemilik2 { get; set; }
        public string Alamat { get; set; }
        public string Alamat2 { get; set; }
        public string Alamat3 { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string JenisKendaraan { get; set; }
        public string NoFormA { get; set; }
        public DateTime? TglFormA { get; set; }
        public string NoKTP { get; set; }
        public string NoTelp { get; set; }
        public string NoHP { get; set; }
        public string NoPIB { get; set; }
        public string NoSUT { get; set; }
        public string NoTPT { get; set; }
        public string NoSRUT { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}