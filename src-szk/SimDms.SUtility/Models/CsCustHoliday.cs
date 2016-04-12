using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("CsCustHoliday")]
    public class CsCustHoliday
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public int PeriodYear { get; set; }
        [Key]
        [Column(Order = 4)]
        public int GiftSeq { get; set; }
        public string ReligionCode { get; set; }
        public string HolidayCode { get; set; }
        public bool? IsGiftCard { get; set; }
        public bool? IsGiftLetter { get; set; }
        public bool? IsGiftSms { get; set; }
        public bool? IsGiftSouvenir { get; set; }
        public DateTime? SouvenirSent { get; set; }
        public DateTime? SouvenirReceived { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}