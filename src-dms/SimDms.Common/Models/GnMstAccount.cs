using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Common.Models
{
    [Table("gnMstAccount")]
    public class GnMstAccount
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string AccountNo { get; set; }
        public string Description { get; set; }
        public string AccountType { get; set; }
        public string Branch { get; set; }
        public string CostCtrCode { get; set; }
        public string ProductType { get; set; }
        public string NaturalAccount { get; set; }
        public string InterCompany { get; set; }
        public string Futureuse { get; set; }
        public string Consol { get; set; }
        public DateTime ? FromDate { get; set; }
        public DateTime ? EndDate { get; set; }
        public string Balance { get; set; }
        public DateTime ? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
    }
}