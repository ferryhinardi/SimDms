using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("GnMstCustomerProfitCenter")]
    public class CustomerProfitCenter
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
        [Key]
        [Column(Order = 4)]
        public string ProfitCenterCode { get; set; }
        public Decimal CreditLimit { get; set; }
        public string PaymentCode { get; set; }
        public string CustomerClass { get; set; }
        public string TaxCode { get; set; }
        public string TaxTransCode { get; set; }
        public Decimal DiscPct { get; set; }
        public Decimal LaborDiscPct { get; set; }
        public Decimal PartDiscPct { get; set; }
        public Decimal MaterialDiscPct { get; set; }
        public string TOPCode { get; set; }
        public string CustomerGrade { get; set; }
        public string ContactPerson { get; set; }
        public string CollectorCode { get; set; }
        public string GroupPriceCode { get; set; }
        public bool IsOverDueAllowed { get; set; }
        public bool IsBlackList { get; set; }
        public string SalesCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool? IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime LockingDate { get; set; }
        public string SalesType { get; set; }
        public string Salesman { get; set; }
    }

}