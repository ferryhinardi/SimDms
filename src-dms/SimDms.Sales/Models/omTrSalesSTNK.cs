using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTrSalesSTNK")]
    public class omTrSalesSTNK
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string BPKBOutType { get; set; }
        public string BPKBOutBy { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    public class STNKBrowse
    {
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string BPKBOutType { get; set; }
        public string BPKBOutTypeName { get; set; }
        public string BPKBOutBy { get; set; }
        public string BPKBOutByName { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Stat { get; set; }
    }

    public class STNKOut
    {
        public string BPKBOutBy { get; set; }
        public string BPKBOutByName { get; set; }
    }
}