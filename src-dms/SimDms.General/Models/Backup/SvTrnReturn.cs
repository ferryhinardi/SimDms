using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    [Table("svTrnReturn")]
    public class SvTrnReturn
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ReturnNo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public string RefferenceNo { get; set; }
        public DateTime? RefferenceDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
    }
}
