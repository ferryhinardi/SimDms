using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.DataWarehouse.Models
{
    [Table("CsLkuBpkbView")]
    public class CsLkuBpkbView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        [Key]
        [Column(Order = 4)]
        public string Chassis { get; set; }
        public string Engine { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public DateTime? BpkbDate { get; set; }
        public DateTime? StnkDate { get; set; }
        public DateTime? BPKDate { get; set; }
        public bool? IsLeasing { get; set; }
        public string Category { get; set; }
        public string LeasingCo { get; set; }
        public string LeasingName { get; set; }
        public decimal? Installment { get; set; }
        public string PoliceRegNo { get; set; }
        public string OutStanding { get; set; }
    }

    public class OutstandingBpkbReminder
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Chassis { get; set; }
        public string PoliceRegNo { get; set; }
        public DateTime? BPKDate { get; set; }
    }
}