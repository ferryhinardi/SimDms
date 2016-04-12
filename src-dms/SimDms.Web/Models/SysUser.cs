using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Web.Models
{
    [Table("SysUser")]
    public class SysUser
    {
        //[Key]
        //public Guid UserId { get; set; }
        //public String Username { get; set; }
        //public String Email { get; set; }
        //public String Password { get; set; }
        //public String FirstName { get; set; }
        //public String LastName { get; set; }
        //public String Comment { get; set; }
        //public Boolean IsApproved { get; set; }
        //public int PasswordFailuresSinceLastSuccess { get; set; }
        //public DateTime? LastPasswordFailureDate { get; set; }
        //public DateTime? LastActivityDate { get; set; }
        //public DateTime? LastLockoutDate { get; set; }
        //public DateTime? LastLoginDate { get; set; }
        //public String ConfirmationToken { get; set; }
        //public DateTime? CreateDate { get; set; }
        //public Boolean IsLockedOut { get; set; }
        //public DateTime? LastPasswordChangedDate { get; set; }
        //public String PasswordVerificationToken { get; set; }
        //public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
        //public string UserGroup { get; set; }

        //public ICollection<SysRole> Roles { get; set; }

        [Key]
        public string UserId { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string TypeOfGoods { get; set; }
        public bool IsActive { get; set; }
        public bool RequiredChange { get; set; }

        //public ICollection<SysRole> Roles { get; set; }
    }

    [Table("SysUserView")]
    public class SysUserView
    {
        [Key]
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string TypeOfGoods { get; set; }
        public bool IsActive { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public bool RequiredChange { get; set; }
    }

    [Table("gnMstCoProfile")]
    public class CoProfile
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyGovName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string ZipCode { get; set; }
        public bool? IsPKP { get; set; }
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public string NPWPNo { get; set; }
        public DateTime NPWPDate { get; set; }
        public string CityCode { get; set; }
        public string AreaCode { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string OwnershipName { get; set; }
        public string TaxTransCode { get; set; }
        public string TaxCabCode { get; set; }
        public bool? IsFPJCentralized { get; set; }
        public string ProductType { get; set; }
        public bool? IsLinkToService { get; set; }
        public bool? IsLinkToSpare { get; set; }
        public bool? IsLinkToSales { get; set; }
        public bool? IsLinkToFinance { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("sysUserProfitCenter")]
    public class SysUserProfitCenter
    {
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
        public string ProfitCenter { get; set; }
    }

    [Table("gnMstLookUpDtl")]
    public class LookUpDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CodeID { get; set; }
        [Key]
        [Column(Order = 3)]
        public string LookUpValue { get; set; }
        public decimal SeqNo { get; set; }
        public string ParaValue { get; set; }
        public string LookUpValueName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class MyUserInfo
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyGovName { get; set; }
        public string BranchCode { get; set; }
        public string CompanyName { get; set; }
        public string TypeOfGoods { get; set; }
        public string ProductType { get; set; }
        public string ProfitCenter { get; set; }
        public string TypeOfGoodsName { get; set; }
        public string ProductTypeName { get; set; }
        public string ProfitCenterName { get; set; }
        public bool IsActive { get; set; }
        public bool RequiredChange { get; set; }
        public string ShowHideTypePart { get; set; }
    }
}