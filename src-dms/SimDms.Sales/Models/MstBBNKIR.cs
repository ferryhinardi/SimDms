using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omMstBBNKIR")]
    public class MstBBNKIR
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
        public string CityCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal SalesModelYear { get; set; }
        public decimal? BBN { get; set; }
        public decimal? KIR { get; set; }
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

    //public class SupplierView
    //{
    //    public string CompanyCode { get; set; }
    //    public string SupplierCode { get; set; }
    //    public string BranchCode { get; set; }
    //    public string ProfitCenterCode { get; set; }
    //    public string SupplierName { get; set; }
    //    public string Alamat { get; set; }
    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string Address3 { get; set; }
    //    public string Address4 { get; set; }
    //    public string Phone { get; set; }
    //    public decimal? DiscPct { get; set; }
    //    public string Status { get; set; }
    //    public string ProfitCenterName { get; set; }
    //    public string TOPDays { get; set; }
    //    public string CityCode { get; set; }
    //    public string CityName { get; set; }

    //}

    public class CityCodeLookup
    {
        public string LookUpValue { get; set; }
        public string LookUpValueName { get; set; }
    }

    public class SalesModelYearLookup
    {
        public Decimal? SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string EngineNo { get; set; }
    } 

    public class BBNKIRBrowse
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public decimal SalesModelYear { get; set; }
        public string SalesModelYearDesc { get; set; }
        public decimal? BBN { get; set; }
        public decimal? KIR { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class BBNKIRLookup
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public string CityCodeTo { get; set; }
        public string CityNameTo { get; set; }
    }
    
}