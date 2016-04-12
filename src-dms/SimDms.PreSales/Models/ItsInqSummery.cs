using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SimDms.PreSales.Models
{
    public class ItsInqSummery
    {
		public string Position { get; set; }
        public string EmployeeName { get; set; }
        public string TipeKendaraan { get; set; }
        public string SumberData { get; set; }
        public Int32 NEW { get; set; }
        public Int32 REORDER { get; set; }
        public Int32 PROSPECT { get; set; } 
        public Int32 HOTPROSPECT { get; set; }
        public Int32 SPK { get; set; }
        public Int32 DO { get; set; }
        public Int32 DELIVERY { get; set; }
        public Int32 LOST { get; set; }
    }

    public class ItsGetBranch
    {
        public string BranchCode { get; set; }
    }

    //Testing
    public class ItsHremployeeTest
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
    }

    public class ItsInqFollowUp
    {
        public string BranchCode { get; set; }  
        public Int32 InquiryNumber { get; set; }
        public string Pelanggan { get; set; }
        public string AlamatProspek { get; set; }
        public string TelpRumah { get; set; }
        public string NamaPerusahaan { get; set; }
        public String InquiryDate { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string Transmisi { get; set; }
        public string Warna { get; set; }
        public string PerolehanData { get; set; }
        public string Employee { get; set; }
        public string Supervisor { get; set; }
        public String NextFollowUpDate { get; set; }
        public string LastProgress { get; set; }
        public string ActivityDetail { get; set; }
    }

    public class inqItsLoadData
    {
        public string DealerAbbreviation { get; set; }
        public string OutletAbbreviation { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public decimal NewINQ { get; set; }
        public decimal HPNewINQ { get; set; }
        public decimal PrcntHPNewINQ { get; set; }
        public decimal SpkfrNI { get; set; }
        public decimal PrcntNewINQ { get; set; }
        public decimal OutsINQ { get; set; }
        public decimal HPOutsINQ { get; set; }
        public decimal PrcntHPOutsINQ { get; set; }
        public decimal OutSPKfrNI { get; set; }
        public decimal PrcntOutsINQ { get; set; }
        public decimal TotalINQ { get; set; }
        public decimal TotalHP { get; set; }
        public decimal PrcntTotalHP { get; set; }
        public decimal TotalSPK { get; set; }
        public decimal PrcntTotalSPK { get; set; }
        public decimal Lost { get; set; }
        public decimal Cancel { get; set; }
        public decimal FakturPolisi { get; set; }
        public decimal SOH { get; set; }
    }

    public class inqItsMktLoadData
    {
        public string DealerAbbreviation { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public decimal OutsINQ { get; set; }
        public decimal NewINQ { get; set; }
        public decimal OutSPK { get; set; }
        public decimal NewSPK { get; set; }
        public decimal CancelSPK { get; set; }
        public decimal FakturPolisi { get; set; }
        public decimal Balance { get; set; }
        public int ATTestDrive { get; set; }
        public int MTTestDrive { get; set; }
    }

    public class ItsInqPeriode  
    {
        public Int32 InquiryNumber { get; set; }
        public string Pelanggan { get; set; }
        public string AlamatProspek { get; set; }
        public string TelpRumah { get; set; }
        public string NamaPerusahaan { get; set; }
        public DateTime? InquiryDate { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string Transmisi { get; set; }
        public string Warna { get; set; }
        public string PerolehanData { get; set; }
        public string Employee { get; set; }
        public string Supervisor { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public string LastProgress { get; set; }
        public string ActivityDetail { get; set; }
    }

    public class inqItsStatusLoadData
    {
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string LastProgress { get; set; }
        public int SaldoAwal { get; set; }
        public int WeekOuts1 { get; set; }
        public int WeekOuts2 { get; set; }
        public int WeekOuts3 { get; set; }
        public int WeekOuts4 { get; set; }
        public int WeekOuts5 { get; set; }
        public int WeekOuts6 { get; set; }
        public int TotalWeekOuts { get; set; }
        public int Week1 { get; set; }
        public int Week2 { get; set; }
        public int Week3 { get; set; }
        public int Week4 { get; set; }
        public int Week5 { get; set; }
        public int Week6 { get; set; }
        public int TotalWeek { get; set; }
        public int Total { get; set; }
    }
}