using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.Models
{
    public class GetGenerateFollowUp
    {
        public String OutletName { get; set; }
        public Int32? InquiryNumber { get; set; }
        public String Pelanggan { get; set; }
        public DateTime? InquiryDate { get; set; }
        public String TipeKendaraan { get; set; }
        public String Variant { get; set; }
        public String Transmisi { get; set; }
        public String Warna { get; set; }
        public String PerolehanData { get; set; }
        public String Employee { get; set; }
        public String TeamLeader { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public String LastProgress { get; set; }
        public String AlamatProspek { get; set; }
        public String TelpRumah { get; set; }
        public String NamaPerusahaan { get; set; }
        public String AlamatPerusahaan { get; set; }
        public String Handphone { get; set; }
        public DateTime? LastUpdateStatus { get; set; }
        public DateTime? SPKDate { get; set; }
        public Int32? DaySPKDate { get; set; }
        public Int32? MonthSPKDate { get; set; }
        public Int32? YearSPKDate { get; set; }
        public DateTime? LostCaseDate { get; set; }
        public Int32? DayInquiryDate { get; set; }
        public Int32? MonthInquiryDate { get; set; }
        public Int32? YearInquiryDate { get; set; }
        public Int32? DayNextFollowUpDate { get; set; }
        public Int32? MonthNextFollowUpDate { get; set; }
        public Int32? YearNextFollowUpDate { get; set; }
        public Int32? QuantityInquiry { get; set; }
        public String LostCaseCategory { get; set; }
        public String LostCaseVoiceOfCustomer { get; set; }
        public String TestDrive { get; set; }
        public String CaraBayar { get; set; }
        public String Leasing { get; set; }
        public String DownPayment { get; set; }
        public String Tenor { get; set; }
        public String MerkLain { get; set; }
        public String SpvEmployeeId { get; set; }
    }

    public class OutStandingProspekBySalesman
    {
        public String PositionId { get; set; }
        public String CompanyCode { get; set; }
        public String EmployeeID { get; set; }
        public String EmployeeName { get; set; }
        public String Position { get; set; }
        public String PositionName { get; set; }
        public String TeamLeader { get; set; }
        public String ModelKendaraan { get; set; }
        public String PerolehanData { get; set; }
        public String TipeKendaraan { get; set; }
        public String Source { get; set; }
        public Int32? PROSPECT { get; set; }
        public Int32? HOTPROSPECT { get; set; }
        public Int32? SPK { get; set; }
    }
}