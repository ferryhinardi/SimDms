using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Common.Models
{
    [Table("gnMstEmployee")]
    public class Employee
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
        public bool IsLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastupdateBy { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public string Nik { get; set; }
        //public string EmpPhotoID { get; set; }
        public string EmpIdentityCardID { get; set; }
        public string EmpImageID { get; set; }
        public string EmpIdentityCardImageID { get; set; }
    }

    public class Select4LookupSalesman
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string TitleCode { get; set; }
        public string TitleName { get; set; }
    }
}
