using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.CStatisfication.Models
{
    [Table("CsReviews")]
    public class CsReview
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 4)]
        public DateTime DateFrom { get; set; }
        [Key]
        [Column(Order = 5)]
        public DateTime DateTo { get; set; }
        [Key]
        [Column(Order = 6)]
        public string Plan { get; set; }
        public string Do { get; set; }
        public string Check { get; set; }
        public string Action { get; set; }
        public string PIC { get; set; }
        public string CommentbyGM { get; set; }
        public string CommentbySIS { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public int? isDeleted { get; set; }
    }

    public class CsReviewModel
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string EmployeeID { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Plan { get; set; }
        public string Do { get; set; }
        public string Check { get; set; }
        public string Action { get; set; }
        public string PIC { get; set; }
        public string CommentbyGM { get; set; }
        public string CommentbySIS { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public int? isDeleted { get; set; }
        public string OutletAbbreviation { get; set; }
    }
}