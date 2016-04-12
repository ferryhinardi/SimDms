using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvTrnDailyRetention")]
    public class SvTrnDailyRetention
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal PeriodYear { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal PeriodMonth { get; set; }
        [Key]
        [Column(Order = 5)]
        public long RetentionNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string CustomerCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string CustomerCategory { get; set; }
        public string VisitInitial { get; set; }
        public DateTime? VisitDate { get; set; }
        public decimal? PMNow { get; set; }
        public decimal? PMNext { get; set; }
        public DateTime? EstimationNextVisit { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool? IsConfirmed { get; set; }
        public bool? IsBooked { get; set; }
        public bool? IsVisited { get; set; }
        public bool? IsSatisfied { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string Reason { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public string LastRemark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string StatisfyReasonGroup { get; set; }
        public string StatisfyReasonCode { get; set; }
        public string CannotCallCode { get; set; }
        public bool? IsReminder { get; set; }
        public bool? IsFollowUp { get; set; }
        public bool? IsClosed { get; set; }
    }

    public class DailyServiceRetentionPopulate
    {
        public Int64? RetentionNo { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public decimal? PeriodYear { get; set; }
        public decimal? PeriodMonth { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string BasicModel { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string PoliceRegNo { get; set; }
        public string JobType { get; set; }
        public string Remark { get; set; }
        public DateTime? ReminderDate { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string IsConfirmed { get; set; }
        public string IsBooked { get; set; }
        public string IsVisited { get; set; }
        public string IsSatisfied { get; set; }
        public string Reason { get; set; }
        public string VisitInitial { get; set; }
        public string VisitInitialDesc { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
        public string PhoneNo { get; set; }
        public string AdditionalPhone1 { get; set; }
        public string AdditionalPhone2 { get; set; }
        public string StatisfyReasonGroup { get; set; }
        public string StatisfyReasonCode { get; set; }
        public string IsReminder { get; set; }
        public string IsFollowUp { get; set; }
        public string TransmissionType { get; set; }
        public decimal? Odometer { get; set; }
        public string MobilePhone { get; set; }
    }

    public class GetVehicleData
    {
        public String PoliceRegNo { get; set; }
        public String BasicModel { get; set; }
        public String TechnicalModel { get; set; }
        public String ChassisCode { get; set; }
        public Decimal? ChassisNo { get; set; }
        public String EngineCode { get; set; }
        public Decimal? EngineNo { get; set; }
        public String Colour { get; set; }
        public String Transmission { get; set; }
        public Decimal? ProductionYear { get; set; }
        public Decimal? Odometer { get; set; }
        public Decimal? PMNow { get; set; }
        public Decimal? PMNext { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public String LastRemark { get; set; }
        public Boolean? IsConfirmed { get; set; }
        public Boolean? IsBooked { get; set; }
        public Boolean? IsVisited { get; set; }
        public Boolean? IsSatisfied { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public String VisitInitial { get; set; }
        public String Reason { get; set; }
        public String CustCategory { get; set; }
        public String StatisfyReasonGroup { get; set; }
        public String StatisfyReasonCode { get; set; }
        public Boolean? IsReminder { get; set; }
        public Boolean? IsFollowUp { get; set; }
        public String CannotCallCode { get; set; }
        public Boolean? isClosed { get; set; }
    }

    public class GetCustomerData
    {
        public String CustomerCode { get; set; }
        public String CustomerName { get; set; }
        public String Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public String CityCode { get; set; }
        public String City { get; set; }
        public String PhoneNo { get; set; }
        public String OfficePhoneNo { get; set; }
        public String HPNo { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Address3 { get; set; }
        public String Address4 { get; set; }            
    }

    [Table("SvHstDailyRetention")]
    public class SvHstDailyRetention
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal PeriodYear { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal PeriodMonth { get; set; }
        [Key]
        [Column(Order = 5)]
        public long RetentionNo { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SeqNo { get; set; }
        public string VisitInitial { get; set; }
        public DateTime? VisitDate { get; set; }
        public decimal? PMNow { get; set; }
        public decimal? PMNext { get; set; }
        public DateTime? EstimationNextVisit { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool? IsConfirmed { get; set; }
        public bool? IsBooked { get; set; }
        public bool? IsVisited { get; set; }
        public bool? IsSatisfied { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string Reason { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public string LastRemark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}