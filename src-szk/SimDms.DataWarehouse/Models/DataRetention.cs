using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class DataRetentionReport
    {
        public string Inisial { set; get; }
        public string Type { set; get; }
        public string NoPolisi { set; get; }
        public string TM { set; get; }
        public int Tahun { set; get; }
        public string KodeMesin { set; get; }
        public string NoMesin { set; get; }
        public string KodeRangka { set; get; }
        public string NoRangka { set; get; }
        public string NamaPelanggan { set; get; }
        public string AlamatPelanggan { set; get; }
        public string TelponRumah { set; get; }
        public string TelponKantor { set; get; }
        public DateTime? TanggalKunjungan { set; get; }
        public string HP { set; get; }
        public string RM { set; get; }
        public string PMSekarang { set; get; }
        public string PMBerikut { set; get; }
        public DateTime? EstimasiBerikut { set; get; }
        public DateTime? TglReminder { set; get; }
        public string BerhasilDiHubungi { set; get; }
        public string Booking { set; get; }
        public DateTime? TglBooking { set; get; }
        public string KonsumenDatang { set; get; }
        public DateTime? TglFollowUp { set; get; }
        public string Kepuasan { set; get; }
        public string Alasan { set; get; }
        public string NamaKontak { set; get; }
        public string AlamatKontak { set; get; }
        public string TeleponKontak { set; get; }
        public string AdditionalPhone1 { set; get; }
        public string AdditionalPhone2 { set; get; }
        public string NamaServiceAdvisor { set; get; }
        public string NamaMekanik { set; get; }
        public string PermintaanPerawatan { set; get; }
        public string Rekomendasi { set; get; }
    }
}