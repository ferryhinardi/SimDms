using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("CsStnkExt")]
    public class CsStnkExt
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
        public bool? IsStnkExtend { get; set; }
        public DateTime? StnkExpiredDate { get; set; }
        public bool? ReqKtp { get; set; }
        public bool? ReqStnk { get; set; }
        public bool? ReqBpkb { get; set; }
        public bool? ReqSuratKuasa { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public int? Status { get; set; }
        public DateTime? FinishDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? Tenor { get; set; }
        public string LeasingCode { get; set; }
        public string CustomerCategory { get; set; }
    }
}