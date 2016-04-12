using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    public class SendFile
    {
    }

    public class OmSelectFakturPolisi
    {
        public string ReqNo { get; set; }
        public DateTime reqDate { get; set; }
        public string StatusFaktur { get; set; }
        public string StandardCode { get; set; }
        public string StandardCodeDesc { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string DealerCategory { get; set; }
        public string SKPKNo { get; set; }
        public string SalesmanName { get; set; }
        public string SKPKName { get; set; }
        public string SKPKAddress1 { get; set; }
        public string SKPKAddress2 { get; set; }
        public string SKPKAddress3 { get; set; }
        public string SKPKCity { get; set; }
        public string SKPKTelp1 { get; set; }
        public string SKPKTelp2 { get; set; }
        public string SKPKHP { get; set; }
        public DateTime SKPKBirthday { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonNonFaktur { get; set; }
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
        public DateTime fakturPolisiBirthday { get; set; }
        public string IsCityTransport { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime fakturPolisiDate { get; set; }
        public string IDNo { get; set; }
        public string isProject { get; set; }
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
    }

    public class OmGetStockDataTable
    {
        public string RecordID { get; set; }
        public DateTime transactionDate { get; set; }
        public string transactionType { get; set; }
        public string ReasonCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string DoNo { get; set; }
        public string Supplier_CustomerCode { get; set; }
        public string Supplier_CustomerName { get; set; }
    }

    public class OmSelectRevFakturPolisi
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string RevisionNo { get; set; }
        public DateTime RevisionDate { get; set; }
        public int? RevisionSeq { get; set; }
        public string RevisionCode { get; set; }
        public string ChassisCode { get; set; }
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
        public DateTime FakturPolisiBirthday { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime FakturPolisiDate { get; set; }
        public string FakturPolisiArea { get; set; }
        public string IDNo { get; set; }
        public bool IsCityTransport { get; set; }
        public bool? IsProject { get; set; }
        public string SubDealerCode { get; set; }
        public string Status { get; set; }
        public int SendCounter { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}