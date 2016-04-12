using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omUtlFpolReq")]
    public class omUtlFpolReq
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BatchNo { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    [Table("OmUtlFPolReqDetail")]
    public class OmUtlFPolReqDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BatchNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string SalesModelDescription { get; set; }
        public string ModelLine { get; set; }
        public string ColourCode { get; set; }
        public string ColourDescription { get; set; }
        public string ServiceBookNo { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string FpolisiModelDescription { get; set; }
        public string SISDeliveryOrderNo { get; set; }
        public DateTime? SISDeliveryOrderDate { get; set; }
        public string SISDeliveryOrderAtasNama { get; set; }
        public string SISSuratJalanNo { get; set; }
        public DateTime? SISSuratJalanDate { get; set; }
        public string SISSuratJalanAtasNama { get; set; }
        public string OldDealerCode { get; set; }
        public string DealerClass { get; set; }
        public string DealerName { get; set; }
        public string SKPKNo { get; set; }
        public string SuratPermohonanNo { get; set; }
        public string SalesmanName { get; set; }
        public string SKPKName { get; set; }
        public string SKPKName2 { get; set; }
        public string SKPKAddr1 { get; set; }
        public string SKPKAddr2 { get; set; }
        public string SKPKAddr3 { get; set; }
        public string SKPKCityCode { get; set; }
        public string SKPKPhoneNo1 { get; set; }
        public string SKPKPhoneNo2 { get; set; }
        public string SKPKHPNo { get; set; }
        public DateTime? SKPKBirthday { get; set; }
        public string FPolName { get; set; }
        public string FPolName2 { get; set; }
        public string FPolAddr1 { get; set; }
        public string FPolAddr2 { get; set; }
        public string FPolAddr3 { get; set; }
        public string FPolPostCode { get; set; }
        public string FPolPostName { get; set; }
        public string FPolCityCode { get; set; }
        public string FPolKecamatanCode { get; set; }
        public string FPolPhoneNo1 { get; set; }
        public string FPolPhoneNo2 { get; set; }
        public string FPolHPNo { get; set; }
        public DateTime? FPolBirthday { get; set; }
        public string IdentificationNo { get; set; }
        public string IsProject { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonDescription { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string IsCityTransport { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Status { get; set; }
    }

    [Table("OmUtlFPolReqDetailAdditional")]
    public class OmUtlFPolReqDetailAdditional
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BatchNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        public string Gender { get; set; }
        public string TempatPembelian { get; set; }
        public string TempatPembelianOthers { get; set; }
        public string KendaraanYgPernahDimiliki { get; set; }
        public string KendaraanYgPernahDimilikiModel { get; set; }
        public string SumberPembelian { get; set; }
        public string SumberPembelianOthers { get; set; }
        public string AsalPembelian { get; set; }
        public string AsalPembelianOthers { get; set; }
        public string InfoSuzukiDari { get; set; }
        public string InfoSuzukiDariOthers { get; set; }
        public string FaktorPentingKendaraan { get; set; }
        public string PendidikanTerakhir { get; set; }
        public string PendidikanTerakhirOthers { get; set; }
        public string PenghasilanKeluarga { get; set; }
        public string Pekerjaan { get; set; }
        public string PekerjaanOthers { get; set; }
        public string PenggunaanKendaraan { get; set; }
        public string PenggunaanKendaraanOthers { get; set; }
        public string CaraPembelian { get; set; }
        public string Leasing { get; set; }
        public string LeasingOthers { get; set; }
        public string JangkaWaktuKredit { get; set; }
        public string JangkaWaktuKreditOthers { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class omUtlFpolReqView
    {
        public string DealerCode { get; set; }
        public string BatchNo { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}