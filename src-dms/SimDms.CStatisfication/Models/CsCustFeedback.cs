using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.CStatisfication.Models
{
    [Table("CsCustFeedback")]
    public class CsCustFeedback
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
        public bool? IsManual { get; set; }
        public string FeedbackA { get; set; }
        public string FeedbackB { get; set; }
        public string FeedbackC { get; set; }
        public string FeedbackD { get; set; }
        public string CreatedBy { get; set; }
        public bool Status { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    [Table("CsLkuFeedbackView")]
    public class CsLkuFeedbackView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        [Key]
        [Column(Order = 4)]
        public string Chassis { get; set; }
        public string Engine { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ColourCode { get; set; }
        public DateTime? BpkbDate { get; set; }
        public DateTime? StnkDate { get; set; }
        public DateTime? BPKDate { get; set; }
        public bool? IsLeasing { get; set; }
        public string LeasingCo { get; set; }
        public string Category { get; set; }
        public string LeasingName { get; set; }
        public decimal? Installment { get; set; }
        public string Tenor { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string Salesman { get; set; }
        public string SalesmanName { get; set; }
        public bool? IsManual { get; set; }
        public string Feedback { get; set; }
        public string FeedbackA { get; set; }
        public string FeedbackB { get; set; }
        public string FeedbackC { get; set; }
        public string FeedbackD { get; set; }
        public int? IsNew { get; set; }
        public string PoliceRegNo { get; set; }
        public string OutStanding { get; set; }
    }
}