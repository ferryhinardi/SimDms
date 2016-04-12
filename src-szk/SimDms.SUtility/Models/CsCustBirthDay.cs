using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
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
        public int PeriodYear { get; set; }
        public string TypeOfGift { get; set; }
        public DateTime? SentGiftDate { get; set; }
        public DateTime? ReceivedGiftDate { get; set; }
        public string Comment { get; set; }
        public string AdditionalInquiries { get; set; }
        public bool? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}