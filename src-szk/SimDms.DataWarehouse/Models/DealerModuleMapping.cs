using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.DataWarehouse.Models
{
    [Table("DealerModuleMapping")]
    public class DealerModuleMapping
    {
        [Key]
        [Column(Order = 1)]
        public string Module { get; set; }
        [Key]
        [Column(Order = 2)]
        public string DealerCode { get; set; }
        public bool IsActive { get; set; }
        public string ProductType { get; set; }
    }
}