using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("CsTDayCall")]
    public class CsTDayCall
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Chassis { get; set; }
        public bool? IsDeliveredA { get; set; }
        public bool? IsDeliveredB { get; set; }
        public bool? IsDeliveredC { get; set; }
        public bool? IsDeliveredD { get; set; }
        public bool? IsDeliveredE { get; set; }
        public bool? IsDeliveredF { get; set; }
        public bool? IsDeliveredG { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public int? Status { get; set; }
        public DateTime? FinishDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}