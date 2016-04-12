using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("svMstPackage")]
    public class svMstPackage
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public int? PackageSrvSeq { get; set; }
        public string PackageDesc { get; set; }
        public string BasicModel { get; set; }
        public string JobType { get; set; }
        public string BillTo { get; set; }
        public int? IntervalYear { get; set; }
        public int? IntervalKM { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }



    }

    [Table("svMstPackageTask")]
    public class svMstPackageTask
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PackageCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string OperationNo { get; set; }
        public decimal? DiscPct { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

    }

    [Table("svMstPackagePart")]
    public class svMstPackagePart
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PackageCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PartNo { get; set; }
        public decimal? DiscPct { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

    }

    [Table("SvPaymentPackage")]
    public class SvPaymentPackage
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProfitCenterCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string BillTo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ProfitCenter { get; set; }


    }

    public class OpNumberPackage
    {
        [Key]
        [Column(Order = 1)]
        public string OperationNo { get; set; }
        [Key]
        [Column(Order = 2)]
        public string OperationName { get; set; }
        public decimal OperationHour { get; set; }
        public decimal LaborPrice { get; set; }
        public string PartName { get; set; }
        public string IsActive { get; set; }
    }

    public class PackageLookUp
    {
        public string PackageCode { get; set; }
        public string Package { get; set; }
        public string PackageName { get; set; }
        public string BasicModel { get; set; }
        public string BasicMod { get; set; }
        public string CustomerBill { get; set; }
        public string BillTo { get; set; }
        public string CustomerName { get; set; }
        public Int32? IntervalYear { get; set; }
        public Int32? IntervalKM { get; set; }   
        public string BasicModelDesc { get; set; }
        public string PackageDesc { get; set; }
    }

    public class DetailPackage
    {
        public string OperationNo { get; set; }
        public string DiscTask { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? DiscPart { get; set; } 
    }

    public class svMstPackageBrowse
    {
        public string CompanyCode { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public int? PackageSrvSeq { get; set; }
        public string PackageDesc { get; set; }
        public string BasicModel { get; set; }
        public string ModelDescription { get; set; }
        public string JobType { get; set; }
        public string JobDescription { get; set; }
        public string BillTo { get; set; }
        public string CustomerName { get; set; }
        public int? IntervalYear { get; set; }
        public int? IntervalKM { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }
}