using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("SysUser")]
    public class SysUser
    {
        [Key]
        public Guid UserId { get; set; }
        public String Username { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Comment { get; set; }
        public string DealerCode { get; set; }
        public string OutletCode { get; set; }
        public Boolean IsApproved { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public DateTime? LastPasswordFailureDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public String ConfirmationToken { get; set; }
        public DateTime? CreateDate { get; set; }
        public Boolean IsLockedOut { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public String PasswordVerificationToken { get; set; }
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
        public string UserGroup { get; set; }

        public ICollection<SysRole> Roles { get; set; }
    }

    [Table("SysUserView")]
    public class SysUserView
    {
        [Key]
        public Guid UserId { get; set; }
        public String Username { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Comment { get; set; }
        public string DealerCode { get; set; }
        public string OutletCode { get; set; }
        public Boolean IsApproved { get; set; }
        public string IsApprovedDescription { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public DateTime? LastPasswordFailureDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public String ConfirmationToken { get; set; }
        public DateTime? CreateDate { get; set; }
        public Boolean IsLockedOut { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public String PasswordVerificationToken { get; set; }
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
        public string UserGroup { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}