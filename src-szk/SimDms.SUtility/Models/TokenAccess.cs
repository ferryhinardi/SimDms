using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("SysTokenAccess")]
    public class TokenAccess
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        public string TokenID { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string ComputerName { get; set; }
    }
}