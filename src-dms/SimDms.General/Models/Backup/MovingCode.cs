using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SimDms.Common;


namespace SimDms.Sparepart.Models
{
    [Table("spMstMovingCode")]
    public class spMstMovingCode : BaseTable
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string MovingCode { get; set; }
        public string MovingCodeName { get; set; }
        public decimal? Param1 { get; set; }
        public string Sign1 { get; set; }
        public string Variable { get; set; }
        public string Sign2 { get; set; }
        public decimal? Param2 { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }

    }

    [Table("spMstMovingCodeView")]
    public class spMstMovingCodeView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string MovingCode { get; set; }
        public string MovingCodeName { get; set; }
        public string Formula { get; set; }
        public decimal? Param1 { get; set; }
        public string Sign1 { get; set; }
        public string Variable { get; set; }
        public decimal? Param2 { get; set; }
        public string Sign2 { get; set; }
    }
}