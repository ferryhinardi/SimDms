﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.General.Models
{
    //[Table("SysUser")]
    //public class SysUser
    //{
    //    [Key]
    //    public string UserId { get; set; }
    //    public string Password { get; set; }
    //    public string FullName { get; set; }
    //    public string Email { get; set; }
    //    public string CompanyCode { get; set; }
    //    public string BranchCode { get; set; }
    //    public string TypeOfGoods { get; set; }
    //    public bool IsActive { get; set; }
    //}

    [Table("SysUserView")]
    public class SysUserView
    {
        [Key]
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string TypeOfGoods { get; set; }
        public bool IsActive { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string ProfitCenter { get; set; }
    }

    [Table("SysParameter")]
    public class SysParameter
    {
        [Key]
        [Column(Order = 1)]
        public string ParamId { get; set; }
        public string ParamValue { get; set; }
        public string ParamDescription { get; set; }
    }


}