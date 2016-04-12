using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svMstLaborRate")]
    public class SvMstTarifJasa
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string LaborCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 5)]
        public DateTime EffectiveDate { get; set; }
        public string Description { get; set; }
        public decimal? LaborPrice { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }

    [Table("SvTarifJasaView")]
    public class SvTarifJasaView
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
        public string LaborCode { get; set; }
        public string Description { get; set; }
        public decimal? LaborPrice { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
       

    }

    [Table("SvLaborCode")]
    public class SvLaborCode 
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        public string BasicModel { get; set; }
    }
}