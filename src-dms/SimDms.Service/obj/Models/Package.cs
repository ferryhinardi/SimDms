using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svTrnPackage")]
    public class Package
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string GenerateNo { get; set; }
        public string BatchNo { get; set; }
        public DateTime? GenerateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }

    public class PackageSP
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string GenerateNo { get; set; }
        public DateTime? GenerateDate { get; set; }
        public string BatchNo { get; set; }
        public string BranchFrom { get; set; }
        public string BranchTo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int? NoOfInvoices { get; set; }
        public bool? IsSelected { get; set; }
    }

    public class BatchPackageSP
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BatchNo { get; set; }
        public DateTime? BatchDate { get; set; }
        public string ReceiptNo { get; set; }
        public string FpjNo { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime FpjDate { get; set; }

    }

    public class MaintainClaim
    {
        [Column(Order = 1)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BatchNo { get; set; }
        public DateTime? BatchDate { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        

    }

}