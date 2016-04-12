using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("svCustomerSatisfactionScore")]
    public class SvCustomerSatisfactionScore
    {
        [Key]
        [Column(Order = 1)]
        public string ServiceCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public decimal PeriodYear { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal PeriodMonth { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public decimal? ServiceInitiation { get; set; }
        public decimal? ServiceAdvisor { get; set; }
        public decimal? ServiceFaciltiy { get; set; }
        public decimal? VehiclePickup { get; set; }
        public decimal? ServiceQuality { get; set; }
        public decimal? Score { get; set; }
        public bool? isStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class CSIScoreGridModel
    {
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public decimal? ServiceInitiation { get; set; }
        public decimal? ServiceAdvisor { get; set; }
        public decimal? ServiceFaciltiy { get; set; }
        public decimal? VehiclePickup { get; set; }
        public decimal? ServiceQuality { get; set; }
        public decimal? Score { get; set; }
    }

    public class CSIScoreChartModel
    {
        public decimal PeriodMonth { get; set; }
        public decimal? ServiceInitiation { get; set; }
        public decimal? ServiceAdvisor { get; set; }
        public decimal? ServiceFaciltiy { get; set; }
        public decimal? VehiclePickup { get; set; }
        public decimal? ServiceQuality { get; set; }
    }

    public class UnitIntakeFiveModel
    {
        public string text { get; set; }
        public decimal? value { get; set; }
    }
}