using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTrSalesReqDetail")]
    public class omTrSalesReqDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReqNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        public string SONo { get; set; }
        public string BPKNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string DealerCategory { get; set; }
        public string SKPKNo { get; set; }
        public string SalesmanName { get; set; }
        public string SalesmanCode { get; set; }
        public string SKPKName { get; set; }
        public string SKPKAddress1 { get; set; }
        public string SKPKAddress2 { get; set; }
        public string SKPKAddress3 { get; set; }
        public string SKPKCity { get; set; }
        public string SKPKTelp1 { get; set; }
        public string SKPKTelp2 { get; set; }
        public string SKPKHP { get; set; }
        public DateTime? SKPKBirthday { get; set; }
        public string FakturPolisiName { get; set; }
        public string FakturPolisiAddress1 { get; set; }
        public string FakturPolisiAddress2 { get; set; }
        public string FakturPolisiAddress3 { get; set; }
        public string PostalCode { get; set; }
        public string PostalCodeDesc { get; set; }
        public string FakturPolisiCity { get; set; }
        public string FakturPolisiTelp1 { get; set; }
        public string FakturPolisiTelp2 { get; set; }
        public string FakturPolisiHP { get; set; }
        public DateTime? FakturPolisiBirthday { get; set; }
        public bool IsCityTransport { get; set; }
        public string FakturPolisiNo { get; set; }
        public string DOImniNo { get; set; }
        public DateTime? DOImniDate { get; set; }
        public string SJImniNo { get; set; }
        public DateTime? SJImniDate { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonNonFaktur { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string FakturPolisiArea { get; set; }
        public string IDNo { get; set; }
        public bool? IsProject { get; set; }

        [NotMapped]
        public string FakturPolisiName2 { get; set; }
    }

    [Table("omTrSalesReqDetailHist")]
    public class omTrSalesReqDetailHist
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReqNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        public string SONo { get; set; }
        public string BPKNo { get; set; }
        public string CreatedBy { get; set; }
        [Key]
        [Column(Order = 6)]
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string DealerCategory { get; set; }
        public string SKPKNo { get; set; }
        public string SalesmanName { get; set; }
        public string SalesmanCode { get; set; }
        public string SKPKName { get; set; }
        public string SKPKAddress1 { get; set; }
        public string SKPKAddress2 { get; set; }
        public string SKPKAddress3 { get; set; }
        public string SKPKCity { get; set; }
        public string SKPKTelp1 { get; set; }
        public string SKPKTelp2 { get; set; }
        public string SKPKHP { get; set; }
        public DateTime? SKPKBirthday { get; set; }
        public string FakturPolisiName { get; set; }
        public string FakturPolisiAddress1 { get; set; }
        public string FakturPolisiAddress2 { get; set; }
        public string FakturPolisiAddress3 { get; set; }
        public string PostalCode { get; set; }
        public string PostalCodeDesc { get; set; }
        public string FakturPolisiCity { get; set; }
        public string FakturPolisiTelp1 { get; set; }
        public string FakturPolisiTelp2 { get; set; }
        public string FakturPolisiHP { get; set; }
        public DateTime? FakturPolisiBirthday { get; set; }
        public bool IsCityTransport { get; set; }
        public string FakturPolisiNo { get; set; }
        public string DOImniNo { get; set; }
        public DateTime? DOImniDate { get; set; }
        public string SJImniNo { get; set; }
        public DateTime? SJImniDate { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonNonFaktur { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string FakturPolisiArea { get; set; }
        public string IDNo { get; set; }
        public bool? IsProject { get; set; }
    }

    [Table("omTrSalesReqDetailAdditional")]
    public class omTrSalesReqDetailAdditional
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReqNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string ChassisNo { get; set; }
        public string JenisKelamin { get; set; }
        public string TempatPembelian { get; set; }
        public string TempatPembelianOther { get; set; }
        public string KendaraanYgPernahDimiliki { get; set; }
        public string SumberPembelian { get; set; }
        public string SumberPembelianOther { get; set; }
        public string AsalPembelian { get; set; }
        public string AsalPembelianOther { get; set; }
        public string InfoSuzukiDari { get; set; }
        public string InfoSuzukiDariOther { get; set; }
        public string FaktorPentingMemilihMotor { get; set; }
        public string PendidikanTerakhir { get; set; }
        public string PendidikanTerakhirOther { get; set; }
        public string PenghasilanPerBulan { get; set; }
        public string Pekerjaan { get; set; }
        public string PekerjaanOther { get; set; }
        public string PenggunaanMotor { get; set; }
        public string PenggunaanMotorOther { get; set; }
        public string CaraPembelian { get; set; }
        public string Leasing { get; set; }
        public string LeasingOther { get; set; }
        public string JangkaWaktuKredit { get; set; }
        public string JangkaWaktuKreditOther { get; set; }
        public string ModelYgPernahDimiliki { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    [Table("omTrSalesFPolRevisionDetail")]
    public class omTrSalesFPolRevisionDetail
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RevisiNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        public string FakturPolisiName { get; set; }
        public string FakturPolisiAddress1 { get; set; }
        public string FakturPolisiAddress2 { get; set; }
        public string FakturPolisiAddress3 { get; set; }
        public string PostalCode { get; set; }
        public string PostalCodeDesc { get; set; }
        public string FakturPolisiCity { get; set; }
        public string FakturPolisiTelp1 { get; set; }
        public string FakturPolisiTelp2 { get; set; }
        public string FakturPolisiHP { get; set; }
        public DateTime? FakturPolisiBirthday { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string FakturPolisiArea { get; set; }
        public string IDNo { get; set; }
        public bool IsCityTransport { get; set; }
        public bool? IsProject { get; set; }
        public string RevisionCode { get; set; }
        public string SubDealerCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("omTrSalesFPolRevisionHistory")]
    public class omTrSalesFPolRevisionHistory
    {
        [Key]
        [Column(Order = 1)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public decimal ChassisNo { get; set; }
        [Key]
        [Column(Order = 3)]
        public int RevisionSeq { get; set; }
        public string RevisionNo { get; set; }
        public string RevisionCode { get; set; }
        public string FakturPolisiName { get; set; }
        public string FakturPolisiAddress1 { get; set; }
        public string FakturPolisiAddress2 { get; set; }
        public string FakturPolisiAddress3 { get; set; }
        public string PostalCode { get; set; }
        public string PostalCodeDesc { get; set; }
        public string FakturPolisiCity { get; set; }
        public string FakturPolisiTelp1 { get; set; }
        public string FakturPolisiTelp2 { get; set; }
        public string FakturPolisiHP { get; set; }
        public DateTime? FakturPolisiBirthday { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string FakturPolisiArea { get; set; }
        public string IDNo { get; set; }
        public bool IsCityTransport { get; set; }
        public bool? IsProject { get; set; }
        public string SubDealerCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    public class SalesReqDetailView
    {
        public string ReqNo { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string SONo { get; set; }
        public string BPKNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string DealerCategory { get; set; }
        public string DealerCategoryDesc { get; set; }
        public string SKPKNo { get; set; }
        public string SalesmanName { get; set; }
        public string SalesmanCode { get; set; }
        public string SKPKName { get; set; }
        public string SKPKAddress1 { get; set; }
        public string SKPKAddress2 { get; set; }
        public string SKPKAddress3 { get; set; }
        public string SKPKCity { get; set; }
        public string SKPKCityName { get; set; }
        public string SKPKTelp1 { get; set; }
        public string SKPKTelp2 { get; set; }
        public string SKPKHP { get; set; }
        public DateTime? SKPKBirthday { get; set; }
        public string FakturPolisiName { get; set; }
        public string FakturPolisiAddress1 { get; set; }
        public string FakturPolisiAddress2 { get; set; }
        public string FakturPolisiAddress3 { get; set; }
        public string PostalCode { get; set; }
        public string PostalCodeDesc { get; set; }
        public string FakturPolisiCity { get; set; }
        public string FakturPolisiCityName { get; set; }
        public string FakturPolisiTelp1 { get; set; }
        public string FakturPolisiTelp2 { get; set; }
        public string FakturPolisiHP { get; set; }
        public DateTime? FakturPolisiBirthday { get; set; }
        public bool IsCityTransport { get; set; }
        public string FakturPolisiNo { get; set; }
        public string DOImniNo { get; set; }
        public DateTime? DOImniDate { get; set; }
        public string SJImniNo { get; set; }
        public DateTime? SJImniDate { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonNonFaktur { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string FakturPolisiArea { get; set; }
        public string IDNo { get; set; }
        public bool? IsProject { get; set; }

        [NotMapped]
        public string StatusBlanko { get; set; }
        public string FakturPolisiName2 { get; set; }
    }

    public class ChassisNoSP
    {
        public string BranchCode { get; set; }
        public string CustomerCode { get; set; }
		public string ChassisCode { get; set; }
        public string BPKNo { get; set; }
        public string SONo { get; set; }
        public string DONo { get; set; }
        public string chassisNo { get; set; }
        public string salesModelCode { get; set; }
		public decimal? salesModelYear { get; set; }
        public string RefferenceDONo { get; set; }
		public DateTime? efferenceDODate { get; set; }
        public string RefferenceSJNo { get; set; }
        public DateTime? RefferenceSJDate { get; set; }
		public string EndUserName { get; set; }
        public string EndUserAddress1 { get; set; }
        public string EndUserAddress2 { get; set; }
        public string EndUserAddress3 { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
		public string CityCode { get; set; }
        public string CityName { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public DateTime? birthDate { get; set; }
        public string Salesman { get; set; }
        public string SalesmanName { get; set; }
        public string SalesType { get; set; }
    }

    public class NoFakturPolis
    {
        public string FakturPolisiNo { get; set; }
        public string statusBlanko { get; set; }
    }

    public class FPDetailCustomer
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BPKNo { get; set; }
        public string SONo { get; set; }
        public string SKPKNo { get; set; }
        public string EndUserName { get; set; }
        public string EndUserAddress1 { get; set; }
        public string EndUserAddress2 { get; set; }
        public string EndUserAddress3 { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public DateTime? birthDate { get; set; }
        public string Salesman { get; set; }
        public string SalesmanName { get; set; }
        public string SalesType { get; set; }
    }

    public class fpDetail
    {
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string ReqNo { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime FakturPolisiDate { get; set; }
        public string PoliceRegistrationNo { get; set; }
        public DateTime PoliceRegistrationDate { get; set; }
        public string SONo { get; set; }
    }

    public class SalesFPolRevisionDetailView  
    {
        public string RevisionNo { get; set; } 
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string FakturPolisiName { get; set; }
        public string FakturPolisiAddress1 { get; set; }
        public string FakturPolisiAddress2 { get; set; }
        public string FakturPolisiAddress3 { get; set; }
        public string PostalCode { get; set; }
        public string PostalCodeDesc { get; set; }
        public string FakturPolisiCity { get; set; }
        public string FakturPolisiCityName { get; set; }
        public string FakturPolisiTelp1 { get; set; }
        public string FakturPolisiTelp2 { get; set; }
        public string FakturPolisiHP { get; set; }
        public DateTime? FakturPolisiBirthday { get; set; }
        public bool IsCityTransport { get; set; }
        public string FakturPolisiNo { get; set; }
        public string RevisionCode { get; set; }
        public string RevisionName { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string FakturPolisiArea { get; set; }
        public string IDNo { get; set; }
        public bool? IsProject { get; set; }
        public DateTime? RevisionDate { get; set; }
        public string ReqNo { get; set; }
        public DateTime? ReqDate { get; set; }
        public string ReffNo { get; set; }
        public DateTime? ReffDate { get; set; }
        public string StatusFaktur { get; set; }
        public string Faktur { get; set; }
        public string SubDealerCode { get; set; }
        public string CustomerName { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Stat { get; set; }
        public bool? isCBU { get; set; }
    }

}