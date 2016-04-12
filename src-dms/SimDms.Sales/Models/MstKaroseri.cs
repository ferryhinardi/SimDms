using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omMstKaroseri")]
    public class MstKaroseri
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
        public string SalesModelCode { get; set; }
        public string SalesModelCodeNew { get; set; }
        public decimal? DPPMaterial { get; set; }
        public decimal? DPPFee { get; set; }
        public decimal? DPPOthers { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("KaroseriBrowseView")]
    public class KaroseriBrowseView
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
        public string SalesModelCode { get; set; }
        public string SalesModelCodeNew { get; set; }
        public decimal? DPPMaterial { get; set; }
        public decimal? DPPFee { get; set; }
        public decimal? DPPOthers { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string SupplierName { get; set; }
        public string SalesModelDesc { get; set; }
        public string SalesModelDescNew { get; set; }
    }

    public class KaroseriBrowse
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public string SalesModelCodeNew { get; set; }
        public string SalesModelDescNew { get; set; }
        public decimal? DPPMaterial { get; set; }
        public decimal? DPPFee { get; set; }
        public decimal? DPPOthers { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class KaroseriLookup
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public string SupplierCodeTo { get; set; }
        public string SalesModelCodeTo { get; set; }
        public string SalesModelCodeBaru { get; set; }
        public string SalesModelCodeBaruTo { get; set; }
    }
}