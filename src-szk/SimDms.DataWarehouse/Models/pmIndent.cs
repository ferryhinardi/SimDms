using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("pmIndent")]
    public class pmIndent
    {
        [Key]
        [Column(Order = 1)]
        public int IndentNumber { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CompanyCode { get; set; }
        public int? InquiryNumber { get; set; }
        public string EmployeeID { get; set; }
        public string SpvEmployeeID { get; set; }
        public DateTime? IndentDate { get; set; }
        public string OutletID { get; set; }
        public string StatusProspek { get; set; }
        public string PerolehanData { get; set; }
        public string NamaProspek { get; set; }
        public string AlamatProspek { get; set; }
        public string TelpRumah { get; set; }
        public string CityID { get; set; }
        public string NamaPerusahaan { get; set; }
        public string AlamatPerusahaan { get; set; }
        public string Jabatan { get; set; }
        public string Handphone { get; set; }
        public string Faximile { get; set; }
        public string Email { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string Transmisi { get; set; }
        public string ColourCode { get; set; }
        public string CaraPembayaran { get; set; }
        public string TestDrive { get; set; }
        public int? QuantityInquiry { get; set; }
        public string LastProgress { get; set; }
        public DateTime? LastUpdateStatus { get; set; }
        public DateTime? SPKDate { get; set; }
        public DateTime? LostCaseDate { get; set; }
        public string LostCaseCategory { get; set; }
        public string LostCaseReasonID { get; set; }
        public string LostCaseOtherReason { get; set; }
        public string LostCaseVoiceOfCustomer { get; set; }
        public DateTime? CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Leasing { get; set; }
        public string DownPayment { get; set; }
        public string Tenor { get; set; }
        public string MerkLain { get; set; }
        public decimal? DeliveryYear { get; set; }
        public decimal? DeliveryMonth { get; set; }

        [NotMapped]
        public string StatusVehicle { get; set; }
        [NotMapped]
        public string BrandCode { get; set; }
        [NotMapped]
        public string ModelName { get; set; }
        [NotMapped]
        public string NikSales { get; set; }
        [NotMapped]
        public string NikSC { get; set; }
        [NotMapped]
        public string SPKNo { get; set; }
        [NotMapped]
        public string Hadiah { get; set; }
        [NotMapped]
        public string ShiftCode { get; set; }
    }

    [Table("pmIndentAdditional")]
    public class pmIndentAdditional
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public int IndentNumber { get; set; }
        public string StatusVehicle { get; set; }
        public string OthersBrand { get; set; }
        public string OthersType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string SPKNo { get; set; }
        public DateTime? SPKDate { get; set; }
        public string GiftRefferenceCode { get; set; }
        public string GiftRefferenceValue { get; set; }
        public string GiftRefferenceDesc { get; set; }
        public string ShiftCode { get; set; }
    }

    [Table("pmStatusHistoryIndent")]
    public class pmStatusHistoryIndent
    {
        [Key]
        [Column(Order = 1)]
        public int IndentNumber { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public int SequenceNo { get; set; }
        public string LastProgress { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    [Table("pmActivitiesIndent")]
    public class pmActivitiesIndent
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public int IndentNumber { get; set; }
        [Key]
        [Column(Order = 4)]
        public int ActivityID { get; set; }
        public DateTime ActivityDate { get; set; }
        public string ActivityType { get; set; }
        public string ActivityDetail { get; set; }
        public DateTime NextFollowUpDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    [Table("msMstModel")]
    public class msMstModel
    {
        [Key]
        [Column(Order = 1)]
        public string ModelType { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Variant { get; set; }
        public string ModelName { get; set; }
        public short? CylinderCapacity { get; set; }
        public short? Length { get; set; }
        public string WheelDrive { get; set; }
        public string TransmissionType { get; set; }
        public bool isSuzukiClass { get; set; }
        public string BrandCode { get; set; }
        public string CategoryCode { get; set; }
        public string DimensionCode { get; set; }
        public string SegmentCode { get; set; }
        public string FunctionCode { get; set; }
        public string GroupModel { get; set; }
        public string Utility1 { get; set; }
        public string Utility2 { get; set; }
        public string Utility3 { get; set; }
        public string Utility4 { get; set; }
        public string Utility5 { get; set; }
        public string Utility6 { get; set; }
        public string Utility7 { get; set; }
        public string Utility8 { get; set; }
        public string Utility9 { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("pmQuota")]
    public class pmQuota
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public decimal PeriodYear { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal PeriodMonth { get; set; }
        [Key]
        [Column(Order = 4)]
        public string TipeKendaraan { get; set; }
        [Key]
        [Column(Order = 5)]
        public string Variant { get; set; }
        [Key]
        [Column(Order = 6)]
        public string Transmisi { get; set; }
        [Key]
        [Column(Order = 7)]
        public string ColourCode { get; set; }
        public int QuotaQty { get; set; }
        public int IndentQty { get; set; }
        public string QuotaBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class InqIndent
    {
        public int IndentNumber { get; set; }
        public string BranchCode { get; set; }
        public string CompanyCode { get; set; }
        public int? InquiryNumber { get; set; }
        public string EmployeeID { get; set; }
        public string SpvEmployeeID { get; set; }
        public DateTime? IndentDate { get; set; }
        public string OutletID { get; set; }
        public string StatusProspek { get; set; }
        public string PerolehanData { get; set; }
        public string NamaProspek { get; set; }
        public string AlamatProspek { get; set; }
        public string TelpRumah { get; set; }
        public string CityID { get; set; }
        public string NamaPerusahaan { get; set; }
        public string AlamatPerusahaan { get; set; }
        public string Jabatan { get; set; }
        public string Handphone { get; set; }
        public string Faximile { get; set; }
        public string Email { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string Transmisi { get; set; }
        public string ColourCode { get; set; }
        public string CaraPembayaran { get; set; }
        public string TestDrive { get; set; }
        public int? QuantityInquiry { get; set; }
        public string LastProgress { get; set; }
        public DateTime? LastUpdateStatus { get; set; }
        public DateTime? SPKDate { get; set; }
        public DateTime? LostCaseDate { get; set; }
        public string LostCaseCategory { get; set; }
        public string LostCaseReasonID { get; set; }
        public string LostCaseOtherReason { get; set; }
        public string LostCaseVoiceOfCustomer { get; set; }
        public DateTime? CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Leasing { get; set; }
        public string DownPayment { get; set; }
        public string Tenor { get; set; }
        public string MerkLain { get; set; }
        public decimal? DeliveryYear { get; set; }
        public decimal? DeliveryMonth { get; set; }
        public string StatusVehicle { get; set; }
        public string BrandCode { get; set; }
        public string ModelName { get; set; }
        public string NikSales { get; set; }
        public string NikSC { get; set; }
        public string SPKNo { get; set; }
        public string Hadiah { get; set; }
        public string ShiftCode { get; set; }
        public string OutletAbbreviation { get; set; }
        public string EmployeeName { get; set; }
        public string Area { get; set; }
        public string DealerName { get; set; }
        public int QuotaQty { get; set; }
        public int IndentQty { get; set; }
        public string PeriodYear { get; set; }
        public string PeriodMonth { get; set; }
        public int GroupNo { get; set; } 
    }
}