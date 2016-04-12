using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svTrnPoSubConTask")]
    public class SubConTask
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
        public string PONo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OperationNo { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? POPrice { get; set; }
        public decimal? SubConPrice { get; set; }
        public bool? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }

    public class SubConTaskView
    {
        [Key]
        [Column(Order = 1)]
        public Int64 No { get; set; }
        public string PONo { get; set; }
        public string OperationNo { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal POPrice { get; set; }
        public string Description { get; set; }
    }
}