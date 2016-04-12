using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Web.Models
{
    [Table("SysControlDms")]
    public class SysControlDms
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string MenuID { get; set; }
        [Key]
        [Column(Order = 3)]
        public string FieldID { get; set; }
        [Key]
        [Column(Order = 4)]
        public string RoleID { get; set; }
        public byte Visibility { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
    }
}