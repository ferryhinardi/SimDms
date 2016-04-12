using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class Cs3DaysCallModel
    {
        public string CompanyCode { get; set; }
        public string CustomerCode { get; set; }
        public string Chassis { get; set; }
        public string IsDeliveredA { get; set; }
        public string IsDeliveredB { get; set; }
        public string IsDeliveredC { get; set; }
        public string IsDeliveredD { get; set; }
        public string IsDeliveredE { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string Address { get; set; }
        public string CarType { get; set; }
        public string Color { get; set; }
        public string PoliceRegNo { get; set; }
        public string Engine { get; set; }
    }

    public class StnkExtensionModel
    {
        public string CompanyCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string HPNo { get; set; }
        public DateTime? BpkbDate { get; set; }
        public DateTime? StnkDate { get; set; }
        public DateTime? StnkExpiredDate { get; set; }
        public bool? isLeasing { get; set; }
        public bool? CustomerCategory { get; set; }
        public string CustCtgDesc { get; set; }
        public string LeasingCode { get; set; }
        public string LeasingDesc { get; set; }
        public string Tenor { get; set; }
        public string StnkExtend { get; set; }
        public string ReqStnkDesc { get; set; }
        public string SalesModelCode { get; set; }
        public string ColourCode { get; set; }
        public string PoliceRegNo { get; set; }
        public string Engine { get; set; }
        public string Chassis { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public string SalesmanName { get; set; }
    }

    public class BpkbReminderModel
    {
        public string CompanyCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public DateTime? BpkbDate { get; set; }
        public DateTime? StnkDate { get; set; }
        public string IsLeasing { get; set; }
        public string FinanceInstitution { get; set; }
        public string Tenor { get; set; }
        public string BpkbReminder { get; set; }
        public string BpkbRequirements { get; set; }
        public string CarType { get; set; }
        public string CarColor { get; set; }
        public string PoliceNo { get; set; }
        public string Engine { get; set; }
        public string Chassis { get; set; }
        public string SalesName { get; set; }
        public string CustomerComments { get; set; }
        public string AdditionalInquiries { get; set; }
    }

    public class FeedbackModel
    {
        public string CompanyCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string HPNo { get; set; }
        public DateTime? BpkbDate { get; set; }
        public DateTime? StnkDate { get; set; }
        public DateTime? DODate { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string PoliceRegNo { get; set; }
        public string Chassis { get; set; }
        public bool? IsManual { get; set; }
        public string FeedbackA { get; set; }
        public string FeedbackB { get; set; }
        public string FeedbackC { get; set; }
        public string FeedbackD { get; set; }
        public string Feedback { get; set; }
        public string Feedback01 { get; set; }
        public string Feedback02 { get; set; }
        public string MyProperty { get; set; }
        public string Feedback03 { get; set; }
        public string Feedback04 { get; set; }
    }

    public class CustomerBirthdayModel
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerAddress { get; set; }
        public int PeriodOfYear { get; set; }
        public string AdditionalInquiries { get; set; }
        //public bool? Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTelephone { get; set; }
        public DateTime? CustomerBirthDate { get; set; }
        public DateTime? CustomerGiftSentDate { get; set; }
        public DateTime? CustomerGiftReceivedDate { get; set; }
        //public string CustomerTypeOfGift { get; set; }
        public string CustomerComment { get; set; }
        public string SpouseName { get; set; }
        public string SpouseTelephone { get; set; }
        public string SpouseRelation { get; set; }
        public DateTime? SpouseBirthDate { get; set; }
        public DateTime? SpouseGiftSentDate { get; set; }
        public DateTime? SpouseGiftReceivedDate { get; set; }
        //public string SpouseTypeOfGift { get; set; }
        public string SpouseComment { get; set; }
        public string ChildName1 { get; set; }
        public string ChildTelephone1 { get; set; }
        public string ChildRelation1 { get; set; }
        public DateTime? ChildBirthDate1 { get; set; }
        public DateTime? ChildGiftSentDate1 { get; set; }
        public DateTime? ChildGiftReceivedDate1 { get; set; }
        //public string ChildTypeOfGift1 { get; set; }
        public string ChildComment1 { get; set; }
        public string ChildName2 { get; set; }
        public string ChildTelephone2 { get; set; }
        public string ChildRelation2 { get; set; }
        public DateTime? ChildBirthDate2 { get; set; }
        public DateTime? ChildGiftSentDate2 { get; set; }
        public DateTime? ChildGiftReceivedDate2 { get; set; }
        //public string ChildTypeOfGift2 { get; set; }
        public string ChildComment2 { get; set; }
        public string ChildName3 { get; set; }
        public string ChildTelephone3 { get; set; }
        public string ChildRelation3 { get; set; }
        public DateTime? ChildBirthDate3 { get; set; }
        public DateTime? ChildGiftSentDate3 { get; set; }
        public DateTime? ChildGiftReceivedDate3 { get; set; }
        //public string ChildTypeOfGift3 { get; set; }
        public string ChildComment3 { get; set; }
        public int? NumberOfChildren { get; set; }
        public int? NumberOfSpouse { get; set; }
        public string OutStanding { get; set; }
    }

    public class CustomerBirthdayMonitoring
    {
        public int Month { set; get; }
        public string CompanyName { set; get; }
        public int TotalCustomer { set; get; }
        public int Reminder { set; get; }
        public int Gift { set; get; }
        public int SMS { set; get; }
        public int Telephone { set; get; }
        public int Letter { set; get; }
        public int Souvenir { set; get; }
    }

    public class CsSummary
    {
        public string RemCode { get; set; }
        public DateTime? RemDate { get; set; }
        public int? RemValue { get; set; }
        public string ControlLink { get; set; }
    }

    public class inqLogReport 
    {
        public string part_no { get; set; }
        public string date_access { get; set; } 
        public string part_name { get; set; }
        public string model_name { get; set; }
        public string search_by_dealer { get; set; }
        public string area { get; set; } 
    }
}