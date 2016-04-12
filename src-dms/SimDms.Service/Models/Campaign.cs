using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvMstCampaign")]
    public class SvMstCampaign
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
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }       
    }

    [Table("svMstCampaignTrans")]
    public class svMstCampaignTrans
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ComplainCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string DefectCode { get; set; }
        [Key]
        [Column(Order = 6)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 7)]
        public decimal ChassisStartNo { get; set; }
        [Key]
        [Column(Order = 8)]
        public decimal ChassisEndNo { get; set; }
        [Key]
        [Column(Order = 9)]
        public string OperationNo { get; set; }
        [Key]
        [Column(Order = 10)]
        public string Description { get; set; }
        [Key]
        [Column(Order = 11)]
        public int Seq { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? ClaimDate { get; set; }
        public decimal? ChassisNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
    }

    [Table("SvCampaignView")]
    public class SvCampaignView
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
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
    }

    [Table("SvComplainView")]
    public class SvComplainView
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
        public string ComplainCode { get; set; }
        public string Keterangan { get; set; }
        public string DescriptionEng { get; set; }
        public string Status { get; set; }
    }

    [Table("SvDefectView")]
    public class SvDefectView
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
        public string DefectCode { get; set; }
        public string Keterangan { get; set; }
        public string DescriptionEng { get; set; }
        public string Status { get; set; }
    }

    [Table("SvOperationView")]
    public class SvOperationView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 3)]
        public string OperationNo { get; set; }
        public string Keterangan { get; set; }
        public string Status { get; set; }

    }
}