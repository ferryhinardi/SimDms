using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("omTrSalesDO")]
    public class omTrSalesDO
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DONo { get; set; }
        public DateTime? DODate { get; set; }
        public string SONo { get; set; }
        public string CustomerCode { get; set; }
        public string ShipTo { get; set; }
        public string WareHouseCode { get; set; }
        public string Expedition { get; set; }
        public string Remark { get; set; }
        public char? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }

    }
}