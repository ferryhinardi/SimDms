using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.CStatisfication.Models
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

        //public ICollection<SysRole> Roles { get; set; }
    }
}