using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.General.Models
{
    [Table("GnMstSupplierProfitCenter")]
    public class GnMstSupplierProfitCenter
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SupplierCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ProfitCenterCode { get; set; }
        public string ContactPerson { get; set; }
        public string SupplierClass { get; set; }
        public string SupplierGrade { get; set; }
        public decimal? DiscPct { get; set; }
        public string TOPCode { get; set; }
        public string TaxCode { get; set; }
        public bool? isBlackList { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class SupplierProfitCenterView
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string ProfitCenterCode { get; set; }
        public string ProfitCenterName { get; set; }
        public string ContactPerson { get; set; }
        public string SupplierClass { get; set; }
        public string SupplierClassName { get; set; }
        public string SupplierGrade { get; set; }
        public string SupplierGradeName { get; set; }
        public decimal? DiscPct { get; set; }
        public string TOPCode { get; set; }
        public string TaxCode { get; set; }
        public string TaxCodeName { get; set; }
        public bool? isBlackList { get; set; }
        public string Status { get; set; }
    }
}