using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("gnMstSupplierProfitCenter")]
    public class SupplierProfitCenter
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
        public char? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class SupplierProfitCenterView
    {
        [Key]
        [Column(Order = 1)]
        public int SeqNo { get; set; }
        public string SupplierCode { get; set; }
        public string ProfitCenterCode { get; set; }
        public string TOPCode { get; set; }
        public int? TOPDays { get; set; }
        public string TOPDaysString { get; set; }
        public string TaxCode { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? TaxPct { get; set; }
    }
}