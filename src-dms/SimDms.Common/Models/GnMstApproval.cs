using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Common.Models 
{
    [Table("GnMstApproval")]
    public class GnMstApproval
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DocumentType { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal ApprovalNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal SeqNo { get; set; }
        public string UserID { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    public class ApprovalView  
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string DocumentType { get; set; }
        public string ApprovalNo { get; set; }
        public string SeqNo { get; set; }
        public string UserID { get; set; }
        public string IsActive { get; set; }
    }
}