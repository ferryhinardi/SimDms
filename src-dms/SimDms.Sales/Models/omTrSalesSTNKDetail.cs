using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTrSalesSTNKDetail")]
    public class omTrSalesSTNKDetail
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
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal ChassisNo { get; set; }
        public string CustomerCode { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string FakturPolisiNo { get; set; }
        public string SalesModelCode { get; set; }
        public string ColourCode { get; set; }
        public string BPKBNo { get; set; }
        public string PoliceRegistrationNo { get; set; }
        public DateTime? PoliceRegistrationDate { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class SalesSTNKDetailView
    {
        public string DocNo { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string FakturPolisiNo { get; set; }
        public string SalesModelCode { get; set; }
        public string ColourCode { get; set; }
        public string BPKBNo { get; set; }
        public string PoliceRegistrationNo { get; set; }
        public DateTime? PoliceRegistrationDate { get; set; }
        public string Remark { get; set; }
    }
}