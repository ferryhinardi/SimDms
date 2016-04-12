using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimDms.Web.Models
{
    [Table("SysRole")]
    public class SysRole
    {
        //[Key]
        //public Guid RoleId { get; set; }
        //public string RoleName { get; set; }
        //public string Description { get; set; }
        //public ICollection<SysUser> Users { get; set; }

        [Key]
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }
        public string Themes { get; set; }
        public int MaxScreen { get; set; }
        public bool IsAdmin { get; set; }

        //public ICollection<SysUser> Users { get; set; }
    }
}