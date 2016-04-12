using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("svMstActiveVIN")]
    public class TargetVIN
    {
        [Key]
        [Column(Order = 1)]
        public int GroupNo { get; set; }
        [Key]
        [Column(Order = 2)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string OutletCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string OutletName { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal Year { get; set; }
        public int RevisitVin { get; set; } 
        public int NewVin { get; set; }
        public int Target { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class TargetVINModel
    {
        public int GroupNo { get; set; }
        public string Area { get; set; }
        public string DealerCode { get; set; }
        public string OutletCode { get; set; } 
        public string OutletName { get; set; }
        public int RevisitVin { get; set; }
        public int NewVin { get; set; } 
        public int Target { get; set; }
    }
}