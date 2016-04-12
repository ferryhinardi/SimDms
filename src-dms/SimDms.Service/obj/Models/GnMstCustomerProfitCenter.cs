using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("gnMstCustomerProfitCenter")]
    public class svGnMstCustomerProfitCenter
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
        public decimal? CreditLimit { get; set; }
        public string PaymentCode { get; set; }
        public string CustomerClass { get; set; }
        public string TaxCode { get; set; }
        public string TaxTransCode { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? LaborDiscPct { get; set; }
        public decimal? PartDiscPct { get; set; }
        public decimal? MaterialDiscPct { get; set; }
        public string TOPCode { get; set; }
        public string CustomerGrade { get; set; }
        public string ContactPerson { get; set; }
        public string CollectorCode { get; set; }
        public string GroupPriceCode { get; set; }
        public bool? isOverDueAllowed { get; set; }
        public string SalesCode { get; set; }
        public string SalesType { get; set; }
        public string Salesman { get; set; }
        public bool? isBlackList { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }
}