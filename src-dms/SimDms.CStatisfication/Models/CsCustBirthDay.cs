using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.CStatisfication.Models
{
    [Table("CsCustBirthDay")]
    public class CsCustBirthDay
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public int? PeriodYear { get; set; }
        public string TypeOfGift { get; set; }
        public DateTime? SentGiftDate { get; set; }
        public DateTime? ReceivedGiftDate { get; set; }
        public string Comment { get; set; }
        public string AdditionalInquiries { get; set; }
        public bool Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string SpouseName { get; set; }
        public string ChildName1 { get; set; }
        public string ChildName2 { get; set; }
        public string ChildName3 { get; set; }
        public DateTime? SpouseBirthday { get; set; }
        public DateTime? ChildBirthday1 { get; set; }
        public DateTime? ChildBirthday2 { get; set; }
        public DateTime? ChildBirthday3 { get; set; }
        public string SpouseComment { get; set; }
        public string ChildComment1 { get; set; }
        public string ChildComment2 { get; set; }
        public string ChildComment3 { get; set; }
        public DateTime? SpouseGiftSentDate { get; set; }
        public DateTime? ChildGiftSentDate1 { get; set; }
        public DateTime? ChildGiftSentDate2 { get; set; }
        public DateTime? ChildGiftSentDate3 { get; set; }
        public string SpouseTypeOfGift { get; set; }
        public string ChildTypeOfGift1 { get; set; }
        public string ChildTypeOfGift2 { get; set; }
        public string ChildTypeOfGift3 { get; set; }
        public string SpouseTelephone { get; set; }
        public DateTime? ChildGiftReceivedDate1 { get; set; }
        public DateTime? ChildGiftReceivedDate2 { get; set; }
        public DateTime? ChildGiftReceivedDate3 { get; set; }
        public DateTime? SpouseGiftReceivedDate { get; set; }
        public string SpouseRelation { get; set; }
        public string ChildRelation1 { get; set; }
        public string ChildRelation2 { get; set; }
        public string ChildRelation3 { get; set; }

        public string ChildTelephone1 { get; set; }
        public string ChildTelephone2 { get; set; }
        public string ChildTelephone3 { get; set; }
        public string Reason { get; set; }
    }

    [Table("CsLkuBirthdayView")]
    public class CsLkuBirthdayView
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CustomerCode { get; set; }
        public string CustomerAddress { get; set; }
        public int? PeriodOfYear { get; set; }
        public string AdditionalInquiries { get; set; }
        public bool? Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTelephone { get; set; }
        public DateTime? CustomerBirthDate { get; set; }
        public DateTime? CustomerGiftSentDate { get; set; }
        public DateTime? CustomerGiftReceivedDate { get; set; }
        public string CustomerTypeOfGift { get; set; }
        public string CustomerComment { get; set; }
        public string SpouseName { get; set; }
        public string SpouseTelephone { get; set; }
        public string SpouseRelation { get; set; }
        public DateTime? SpouseBirthDate { get; set; }
        public DateTime? SpouseGiftSentDate { get; set; }
        public DateTime? SpouseGiftReceivedDate { get; set; }
        public string SpouseTypeOfGift { get; set; }
        public string SpouseComment { get; set; }
        public string ChildName1 { get; set; }
        public string ChildTelephone1 { get; set; }
        public string ChildRelation1 { get; set; }
        public DateTime? ChildBirthDate1 { get; set; }
        public DateTime? ChildGiftSentDate1 { get; set; }
        public DateTime? ChildGiftReceivedDate1 { get; set; }
        public string ChildTypeOfGift1 { get; set; }
        public string ChildComment1 { get; set; }
        public string ChildName2 { get; set; }
        public string ChildTelephone2 { get; set; }
        public string ChildRelation2 { get; set; }
        public DateTime? ChildBirthDate2 { get; set; }
        public DateTime? ChildGiftSentDate2 { get; set; }
        public DateTime? ChildGiftReceivedDate2 { get; set; }
        public string ChildTypeOfGift2 { get; set; }
        public string ChildComment2 { get; set; }
        public string ChildName3 { get; set; }
        public string ChildTelephone3 { get; set; }
        public string ChildRelation3 { get; set; }
        public DateTime? ChildBirthDate3 { get; set; }
        public DateTime? ChildGiftSentDate3 { get; set; }
        public DateTime? ChildGiftReceivedDate3 { get; set; }
        public string ChildTypeOfGift3 { get; set; }
        public string ChildComment3 { get; set; }
        public int? NumberOfChildren { get; set; }
        public int? NumberOfSpouse { get; set; }
        public string Outstanding { get; set; }
        public string Reason { get; set; }
        public string CustomerBirthDay
        {
            get
            {
                return ("0" + CustomerBirthDate.Value.Day.ToString())
                    .Substring(CustomerBirthDate.Value.Day.ToString().Length - 1, 2);
            }
        }
    }

    public class CustomerBirthday
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public int? PeriodOfYear { get; set; }
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerTelephone { get; set; }
        public DateTime? CustomerBirthDate { get; set; }
        public string CustomerTypeOfGift { get; set; }
        public string SpouseTypeOfGift { get; set; }
        public string ChildTypeOfGift1 { get; set; }
        public string ChildTypeOfGift2 { get; set; }
        public string ChildTypeOfGift3 { get; set; }
        public DateTime? CustomerGiftSentDate { get; set; }
        public DateTime? SpouseGiftSentDate { get; set; }
        public DateTime? ChildGiftSentDate1 { get; set; }
        public DateTime? ChildGiftSentDate2 { get; set; }
        public DateTime? ChildGiftSentDate3 { get; set; }
        public DateTime? CustomerGiftReceivedDate { get; set; }
        public DateTime? SpouseGiftReceivedDate { get; set; }
        public DateTime? ChildGiftReceivedDate1 { get; set; }
        public DateTime? ChildGiftReceivedDate2 { get; set; }
        public DateTime? ChildGiftReceivedDate3 { get; set; }
        public string CustomerComment { get; set; }
        public string SpouseComment { get; set; }
        public string ChildComment1 { get; set; }
        public string ChildComment2 { get; set; }
        public string ChildComment3 { get; set; }
        public string AdditionalInquiries { get; set; }
        public bool Status { get; set; }
        public string SpouseName { get; set; }
        public string SpouseTelephone { get; set; }
        public string SpouseRelation { get; set; }
        public DateTime? SpouseBirthDate { get; set; }
        public string ChildName1 { get; set; }
        public string ChildTelephone1 { get; set; }
        public DateTime? ChildBirthDate1 { get; set; }
        public string ChildName2 { get; set; }
        public string ChildTelephone2 { get; set; }
        public DateTime? ChildBirthDate2 { get; set; }
        public string ChildName3 { get; set; }
        public string ChildTelephone3 { get; set; }
        public DateTime? ChildBirthDate3 { get; set; }
        public int NumberOfSpouse { get; set; }
        public int NumberOfChildren { get; set; }
        public string ChildRelation1 { get; set; }
        public string ChildRelation2 { get; set; }
        public string ChildRelation3 { get; set; }
        public string Reason { get; set; }
    }
}