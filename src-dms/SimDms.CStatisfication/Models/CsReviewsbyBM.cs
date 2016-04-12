using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.CStatisfication.Models
{
    [Table("CsReviewsbyBM")]
    public class CsReviewsbyBM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 1)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 2)]
        public DateTime ReviewDate { get; set; }
        public string Plan { get; set; }
        public string Do { get; set; }
        public string Check { get; set; }
        public string Action { get; set; }
        public string PIC { get; set; }
        public string CommentbyGM { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public int? isDeleted { get; set; }
    }
}