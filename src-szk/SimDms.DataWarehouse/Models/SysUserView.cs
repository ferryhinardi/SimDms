using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("SysUserView")]
    public class SysUserView
    {
        [Key]
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string DealerCode { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string IsApprovedDescription { get; set; }
        public string OutletCode { get; set; }

    }


}