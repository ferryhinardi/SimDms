using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("DealerGroupMapping")]
    public class DealerGroupMappings
    {
        [Key]
        [Column(Order = 1)]
        public string GroupNo { get; set; }
        [Key]
        [Column(Order = 2)]
        public string DealerAbbr { get; set; }

        public int SeqNo { get; set; }
        public string DealerCode { get; set; }
        public string ProductType { get; set; }
    }
}