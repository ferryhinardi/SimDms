using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Service.Models
{
    [Table("SysUser")]
    public class SysUser
    {
        [Key]
        public string UserId { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string TypeOfGoods { get; set; }
        public bool IsActive { get; set; }
    }
}