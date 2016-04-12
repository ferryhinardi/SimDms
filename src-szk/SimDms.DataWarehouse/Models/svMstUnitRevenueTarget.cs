using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.DataWarehouse.Models
{
    public class svMstUnitRevenueTarget
    {
        [Key]
        [Column(Order = 1)]
        public int GroupNo { get; set; }
        [Key]
        [Column(Order = 2)]
        public decimal PeriodYear { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TargetFlag { get; set; }
        public decimal? Target01 { get; set; }
        public decimal? Target02 { get; set; }
        public decimal? Target03 { get; set; }
        public decimal? Target04 { get; set; }
        public decimal? Target05 { get; set; }
        public decimal? Target06 { get; set; }
        public decimal? Target07 { get; set; }
        public decimal? Target08 { get; set; }
        public decimal? Target09 { get; set; }
        public decimal? Target10 { get; set; }
        public decimal? Target11 { get; set; }
        public decimal? Target12 { get; set; }
        public bool? isStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        /* Addtional */
        public string AreaDealer { get; set; }
        public decimal? TotalTarget { get; set; }
    }

    public class UnitRevenue
    {
        public string Type { get; set; }
        public string TargetFlag { get; set; }
        public decimal Jan { get; set; }
        public decimal Feb { get; set; }
        public decimal Mar { get; set; }
        public decimal Apr { get; set; }
        public decimal May { get; set; }
        public decimal Jun { get; set; }
        public decimal Jul { get; set; }
        public decimal Aug { get; set; }
        public decimal Sep { get; set; }
        public decimal Oct { get; set; }
        public decimal Nov { get; set; }
        public decimal Des { get; set; }
    }
}