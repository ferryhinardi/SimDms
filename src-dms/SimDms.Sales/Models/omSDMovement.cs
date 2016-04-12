using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omSDMovement")]
    public class omSDMovement
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DocNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public DateTime? DocDate { get; set; }
        [Key]
        [Column(Order = 5)]
        public Int32 Seq { get; set; }

        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string WarehouseCode { get; set; }
        public string CustomerCode { get; set; }
        public string QtyFlag { get; set; }

        public string CompanyMD { get; set; }
        public string BranchMD { get; set; }
        public string WarehouseMD { get; set; }

        public string Status { get; set; }
        public string ProcessStatus { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

    }
}