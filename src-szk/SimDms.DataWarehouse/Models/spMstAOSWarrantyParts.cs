using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.DataWarehouse.Models
{
    [Table("spMstAOSWarrantyParts")]
    public class spMstAOSWarrantyParts
    {
        [Key]
        [Column(Order = 1)]
        public string PartNo { get; set; }
        public bool? isWarrantyParts { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class WarrantyPartsSave
    {
        public string No { get; set; }
        public string PartNo { get; set; }
        public string isWarrantyParts { get; set; }
        public string PartName { get; set; }
        public string Status { get; set; }
    }

    public class WarrantyPartsModel
    {
        public int No { get; set; }
        public string PartNo { get; set; }
        public bool isWarrantyParts { get; set; }
        public string PartName { get; set; }
        public string Status { get; set; }
    }
}