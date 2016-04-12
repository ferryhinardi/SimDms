using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("QaMstException")]
    public class QaMstException
    {
        [Key]
        [Column(Order = 1)]
        public string Event { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ServerYear { get; set; }
        [Key]
        [Column(Order = 3)]
        public int ServerMonth { get; set; }
        public int MaxPrevMonthEx { get; set; }
    }
}