using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("QaMstProtection")]
    public class QaMstProtection
    {
        [Key]
        [Column(Order = 1)]
        public string Event { get; set; }
        public int MaxPrevMonthDefault { get; set; }
    }
}