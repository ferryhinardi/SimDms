using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.DataWarehouse.Models
{
    [Table("DealerGroupMappingView")]
    public class DealerGroupMappingView
    {
        [Key]
        [Column(Order=1)]
        public string GroupNo { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
        public string Area { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
    }
}