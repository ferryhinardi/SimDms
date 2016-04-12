using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("gnMstSignature")]
    public class Signature
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProfitCenterCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string DocumentType { get; set; }
        [Key]
        [Column(Order = 5)]
        public int SeqNo { get; set; }
        public string SignName { get; set; }
        public string TitleSign { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    public class SignatureView
    {
        [Key]
        public int SeqNo { get; set; }
        public string SignName { get; set; }
        public string TitleSign { get; set; }
    }

    public class SignatureViewLookup
    {
        public string SignName { get; set; }
        public string TitleSign { get; set; }
    }
}