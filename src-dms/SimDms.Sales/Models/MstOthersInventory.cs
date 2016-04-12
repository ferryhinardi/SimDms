using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace SimDms.Sales.Models
{
    [Table("omMstOthersNonInventory")]
    public class MstOthersInventory
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string OthersNonInventory { get; set; }
        public string OthersNonInventoryDesc { get; set; }
        public string OthersNonInventoryAccNo { get; set; }
        public string Remark { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class OthersInventoryBrowse
    { 
        public string OthersNonInventory { get; set; }
        public string OthersNonInventoryDesc { get; set; }
        public string OthersNonInventoryAccNo { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public bool? IsActive { get; set; }
    }
}