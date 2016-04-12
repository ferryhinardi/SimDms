using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svMstWorkingTime")]
    public class SvMstWaktuKerja
    {
    [Key]
    [Column(Order = 1)]
    public string CompanyCode { get; set; }
    [Key]
    [Column(Order = 2)]
    public string BranchCode { get; set; }
    [Key]
    [Column(Order = 3)]
    public string DayCode { get; set; }
    public string Description { get; set; }
    public DateTime BeginWorkTime { get; set; }
    public DateTime EndWorkTime { get; set; }
    public DateTime BeginLunchTime { get; set; }
    public DateTime EndLunchTime { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string LastupdateBy { get; set; }
    public DateTime? LastupdateDate { get; set; }
    public bool IsLocked { get; set; }
    public string LockingBy { get; set; }
    public DateTime? LockingDate { get; set; }
    }

    [Table("SvWaktuKerja")]
    public class SvMstWaktuKerjaView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DayCode { get; set; }
        public string Description { get; set; }
        public DateTime BeginWorkTime { get; set; }
        public DateTime EndWorkTime { get; set; }
        public DateTime BeginLunchTime { get; set; }
        public DateTime EndLunchTime { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }

    }
}