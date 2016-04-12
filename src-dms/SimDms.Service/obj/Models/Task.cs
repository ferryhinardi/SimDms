using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svMstTask")]
    public class Task
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string JobType { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OperationNo { get; set; }
        public string Description { get; set; }
        public decimal? OperationHour { get; set; }
        public decimal? ClaimHour { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? LaborPrice { get; set; }
        public string TechnicalModelCode { get; set; }
        public bool? IsSubCon { get; set; }
        public bool IsCampaign { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string BillType { get; set; }

    }
}