using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("GnMstCoProfileSales")]
    public class GnMstCoProfileSales
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public decimal? FiscalYear { get; set; }
        public decimal? FiscalMonth { get; set; }
        public decimal? FiscalPeriod { get; set; }
        public DateTime? PeriodBeg { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string ContactPersonName { get; set; }
        public string FaxNo { get; set; }
        public string PhoneNo { get; set; }
        public string HandPhoneNo { get; set; }
        public string EmailAddr { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public DateTime? TransDate { get; set; }

    }
}