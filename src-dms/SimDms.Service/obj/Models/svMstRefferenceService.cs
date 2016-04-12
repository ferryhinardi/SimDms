using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Service.Models
{
    [Table("svMstRefferenceService")]
    public class svMstRefferenceService
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
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    [Table("SvRefferenceServiceView")]
    public class RefferenceServiceView
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
    }

    [Table("SvReffServiceView")]
    public class svMstRefferenceServiceView
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
        public string LookupValueName { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public string IsActiveDesc { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    public class ModelCodeOpen
    {
        public string ProductType { get; set; }
        public string RefferenceType { get; set; }
        public string BasicModel { get; set; }
        public string ModelDescription { get; set; }
        public string TechnicalModelCode { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
    }

    public class ColourCodeOpen
    {
        public string RefferenceCode { get; set; }
        public string RefferenceDesc1 { get; set; }
        public string RefferenceDesc2 { get; set; }
        public string ColourCode { get; set; }
    }

    public class CustomerCodeOpen
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string DealerCode { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
    }

    public class UploadFileType
    {
        public string RefferenceType { get; set; }
        public string RefferenceCode { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
    }
}