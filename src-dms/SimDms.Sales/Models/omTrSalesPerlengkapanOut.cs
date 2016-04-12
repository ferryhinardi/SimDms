using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTrSalesPerlengkapanOut")]
    public class omTrSalesPerlengkapanOut
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PerlengkapanNo { get; set; }
        public DateTime? PerlengkapanDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string PerlengkapanType { get; set; }
        public string SourceDoc { get; set; }
        public string CustomerCode { get; set; }
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

    public class inquiryTrPerlengkapanOutView
    {
        public string PerlengkapanNo { get; set; }
        public string PerlengkapanDate { get; set; }
        public string ReferenceNo { get; set; }
        public string ReferenceDate { get; set; }
        public string PerlengkapanType { get; set; }
        public string SourceDoc { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class PerlengkapanOutView
    {
        public string PerlengkapanNo { get; set; }
        public DateTime? PerlengkapanDate { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
    }
}