using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.DataWarehouse.Models
{
    [Table("CsLkuTDayCallView")]
    public class CsLkuTDayCallView
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
        public DateTime? DODate { get; set; }
        public DateTime? BPKDate { get; set; }
        public string PoliceRegNo { get; set; }
        public string OutStanding { get; set; }
    }

    public class OutstandingTDayCall
    {
        public string CustomerCode{ get; set; }
        public string CustomerName { get; set; }
        public string Chassis { get; set; }
        public string PoliceRegNo { get; set; }
        public DateTime? DODate { get; set; }
    }
}