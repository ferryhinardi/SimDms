using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("CsCustBpkb")]
    public class CsCustBpkb
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
        public DateTime? BpkbReadyDate { get; set; }
        public DateTime? BpkbPickUp { get; set; }
        public bool? ReqInfoLeasing { get; set; }
        public bool? ReqInfoCust { get; set; }
        public bool? ReqKtp { get; set; }
        public bool? ReqStnk { get; set; }
        public bool? ReqSuratKuasa { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public int? Status { get; set; }
        public DateTime? FinishDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string LeasingCode { get; set; }
        public int? Tenor { get; set; }
        public string CustomerCategory { get; set; }
    }
}