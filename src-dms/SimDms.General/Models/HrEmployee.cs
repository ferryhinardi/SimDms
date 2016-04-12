using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.General.Models
{
    //[Table("HrEmployeeView")]
    //public class HrEmployeeView
    //{
    //    [Key]
    //    [Column(Order = 1)]
    //    public string CompanyCode { get; set; }
    //    [Key]
    //    [Column(Order = 2)]
    //    public string EmployeeID { get; set; }
    //    public string SalesID { get; set; }
    //    public string ServiceID { get; set; }
    //    public string EmployeeName { get; set; }
    //    public string Email { get; set; }
    //    public string FaxNo { get; set; }
    //    public string Handphone1 { get; set; }
    //    public string Handphone2 { get; set; }
    //    public string Handphone3 { get; set; }
    //    public string Handphone4 { get; set; }
    //    public string Telephone1 { get; set; }
    //    public string Telephone2 { get; set; }
    //    public string OfficeLocation { get; set; }
    //    public bool? IsLinkedUser { get; set; }
    //    public string FullName { get; set; }
    //    public string RelatedUser { get; set; }
    //    public DateTime? JoinDate { get; set; }
    //    public string Department { get; set; }
    //    public string Position { get; set; }
    //    public string Grade { get; set; }
    //    public string Rank { get; set; }
    //    public string TeamLeader { get; set; }
    //    public string PersonnelStatus { get; set; }
    //    public DateTime? ResignDate { get; set; }
    //    public string ResignDescription { get; set; }
    //    public string ResignCategory { get; set; }
    //    public string IdentityNo { get; set; }
    //    public string NPWPNo { get; set; }
    //    public DateTime? NPWPDate { get; set; }
    //    public DateTime? BirthDate { get; set; }
    //    public string BirthPlace { get; set; }
    //    public string Province { get; set; }
    //    public string District { get; set; }
    //    public string SubDistrict { get; set; }
    //    public string Village { get; set; }
    //    public string ZipCode { get; set; }
    //    public string Gender { get; set; }
    //    public string Religion { get; set; }
    //    public string DrivingLicense1 { get; set; }
    //    public string DrivingLicense2 { get; set; }
    //    public string MaritalStatus { get; set; }
    //    public string MaritalStatusCode { get; set; }
    //    public decimal? Height { get; set; }
    //    public decimal? Weight { get; set; }
    //    public decimal? UniformSize { get; set; }
    //    public string UniformSizeAlt { get; set; }
    //    public decimal? ShoesSize { get; set; }
    //    public string FormalEducation { get; set; }
    //    public string BloodCode { get; set; }
    //    public string OtherInformation { get; set; }
    //    public string Address { get; set; }
    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string Address3 { get; set; }
    //    public string Address4 { get; set; }
    //    public string Status { get; set; }
    //    public string AdditionalJob1 { get; set; }
    //    public string AdditionalJob2 { get; set; }
    //    public string DepartmentName { get; set; }
    //    public string PositionName { get; set; }
    //    public string GradeName { get; set; }
    //    public string AdditionalJob1Name { get; set; }
    //    public string AdditionalJob2Name { get; set; }
    //    public string RankName { get; set; }
    //    public string SelfPhoto { get; set; }
    //    public string IdentityCardPhoto { get; set; }
    //}

    [Table("gnMSTEmployee")]
    public class gnMSTEmployee
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PhoneNo { get; set; }
        public string HpNo { get; set; }
        public string FaxNo { get; set; }
        public string ProvinceCode { get; set; }
        public string AreaCode { get; set; }
        public string CityCode { get; set; }
        public string ZipNo { get; set; }
        public string TitleCode { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? ResignDate { get; set; }
        public string GenderCode { get; set; }
        public string BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        public string MaritalStatusCode { get; set; }
        public string ReligionCode { get; set; }
        public string BloodCode { get; set; }
        public string IdentityNo { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? UniformSize { get; set; }
        public decimal? ShoesSize { get; set; }
        public string FormalEducation { get; set; }
        public string PersonnelStatus { get; set; }
        public bool? IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public string Nik { get; set; }
        ///public string EmpPhotoID { get; set; }
        public string EmpIdentityCardID { get; set; }
        public string EmpImageID { get; set; }
        public string EmpIdentityCardImageID { get; set; }
    }

    [Table("gnMSTEmployeeTraining")]
    public class GnMstEmployeeTraining
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string EmployeeID { get; set; }
        [Key]
        [Column(Order = 4)]
        public bool IsSuzukiTraining { get; set; }
        [Key]
        [Column(Order = 5)]
        public string TrainingCode { get; set; }
        public DateTime? BeginTrainingDate { get; set; }
        public DateTime? EndTrainingDate { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        
        [NotMapped]
        public string Notes { get; set; }
    }

    public class EmployeeTrainingView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string EmployeeID { get; set; }
        public bool IsSuzukiTraining { get; set; }
        public string TrainingCode { get; set; }
        public string BeginTrainingDate { get; set; }
        public string EndTrainingDate { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public string Notes { get; set; }
    }

}