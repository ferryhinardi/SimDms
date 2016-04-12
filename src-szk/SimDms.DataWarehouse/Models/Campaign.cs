using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("svMstRefferenceService")]
    public class ReffService
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RefferenceType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string RefferenceCode { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("svMstTask")]
    public class svMstTask{
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BasicModel { get; set; }
        [Key]
        [Column(Order = 4)]
        public string JobType { get; set; }
        [Key]
        [Column(Order = 5)]
        public string OperationNo { get; set; }
        public string Description { get; set; }
        public decimal OperationHour { get; set; }
        public decimal ClaimHour { get; set; }
        public decimal LaborCost { get; set; }
        public decimal LaborPrice { get; set; }
        public string TechnicalModelCode { get; set; }
        public bool IsSubCon { get; set; }
        public bool IsCampaign { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string BillType { get; set; }
    }
    
    [Table ("svMstCampaign")]
    public class svMstCampaign
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ComplainCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string DefectCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal ChassisStartNo { get; set; }
        [Key]
        [Column(Order = 7)]
        public decimal ChassisEndNo { get; set; }
        public string OperationNo { get; set; }
        public DateTime CloseDate { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    public class mdlCampaign
    {
        public string ProductType { get; set; }
        public string ComplainCode { get; set; }
        public string ComplainName { get; set; }
        public string DefectCode { get; set; }
        public string DefectName { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisStartNo { get; set; }
        public decimal ChassisEndNo { get; set; }
        public string OperationNo { get; set; }
        public string OperationName { get; set; }
        public DateTime ClosedDate { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class listCompanyCode
    {
        public string CompanyCode { get; set; }
    }

}