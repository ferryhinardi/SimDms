using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.CStatisfication.Models
{
    [Table("CsCustRelation")]
    public class CsCustRelation
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RelationType { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public string RelationInfo { get; set; }
        public DateTime? BirthDate { get; set; }
        public string TypeOfGift { get; set; }
        public DateTime? SentGiftDate { get; set; }
        public DateTime? ReceivedGiftDate { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}