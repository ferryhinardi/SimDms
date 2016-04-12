using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omTrInventWH")]
    public class omTrInventWH
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal ChassisNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public DateTime? ReceivingDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Remark { get; set; }
        public string ProcessStatus { get; set; }
        public string Status { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string LockingNotes { get; set; }
    }

    [Table("omTrInventWHHistory")]
    public class omTrInventWHHistory
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal ChassisNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public int SeqNo { get; set; }
        public string BranchCode { get; set; }
        public string Driver { get; set; }
        public string DeliveryTo { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    [Table("omTrInventMovement")]
    public class omTrInventMovement
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string WarehouseCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public int Year { get; set; }
        [Key]
        [Column(Order = 4)]
        public int Month { get; set; }
        [Key]
        [Column(Order = 5)]
        public string SaleModelCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public int SalesModelYear { get; set; }
        [Key]
        [Column(Order = 7)]
        public string ColourCode { get; set; }
        public int? BOM { get; set; }
        public int? IN { get; set; }
        public int? OUT { get; set; }
        public int? EOM { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}