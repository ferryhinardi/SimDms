using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;



namespace SimDms.Service.Models
{

    //[Table("gnMstCoProfileService")]
    //public class CoProfileService
    //{
    //    [Key]
    //    [Column(Order = 1)]
    //    public string CompanyCode { get; set; }
    //    [Key]
    //    [Column(Order = 2)]
    //    public string BranchCode { get; set; }
    //    public string ContactPersonName { get; set; }
    //    public string PhoneNo { get; set; }
    //    public string FaxNo { get; set; }
    //    public string HandPhoneNo { get; set; }
    //    public string EmailAddr { get; set; }
    //    public string MOUNo { get; set; }
    //    public DateTime? MOUDate { get; set; }
    //    public string BuildingOwnership { get; set; }
    //    public string LandOwnership { get; set; }
    //    public decimal FiscalYear { get; set; }
    //    public decimal FiscalMonth { get; set; }
    //    public decimal FiscalPeriod { get; set; }
    //    public DateTime PeriodBeg { get; set; }
    //    public DateTime PeriodEnd { get; set; }
    //    public char EstimateTimeFlag { get; set; }
    //    public string CreatedBy { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public string LastupdateBy { get; set; }
    //    public DateTime? LastupdateDate { get; set; }
    //    public bool? isLocked { get; set; }
    //    public string LockingBy { get; set; }
    //    public DateTime? LockingDate { get; set; }
    //    public DateTime? TransDate { get; set; }

    [Table("gnMstCoProfileService")]    
    public class svCoProfileService
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string ContactPersonName { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string HandPhoneNo { get; set; }
        public string EmailAddr { get; set; }
        public string MOUNo { get; set; }
        public DateTime? MOUDate { get; set; }
        public string BuildingOwnership { get; set; }
        public string LandOwnership { get; set; }
        public decimal FiscalYear { get; set; }
        public decimal FiscalMonth { get; set; }
        public decimal FiscalPeriod { get; set; }
        public DateTime PeriodBeg { get; set; }
        public DateTime PeriodEnd { get; set; }
        public char EstimateTimeFlag { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public DateTime? TransDate { get; set; }

    }
}