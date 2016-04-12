using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("omMstRefference")]
    public class OmMstRefference
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string RefferenceType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RefferenceCode { get; set; }
        public string RefferenceDesc1 { get; set; }
        public string RefferenceDesc2 { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public bool? IsLocked { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}