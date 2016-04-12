using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Sparepart.Models
{
    [Table("spMstItemMod")]
    public class spMstItemMod
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PartNo { get; set; }
        [Key]
        [Column(Order = 3)]
        public string NewPartNo { get; set; }
        public decimal? UnitConversion { get; set; }
        public string InterChangeCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public string EndMark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }



    public class SpMstItemModifInfo
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        public string PartNo { get; set; }
        public string ProductType { get; set; }
        public string ProductTypeName { get; set; }
        public string CategoryName { get; set; }
        public string PartCategory { get; set; }
        public string PartName { get; set; }
        public string IsGenuinePart { get; set; }
        public decimal? OrderUnit { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
     }

    public class SpMstItemconditionView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        public string PartNo { get; set; }
        public string NewPartNo { get; set; }
        public string InterchangeCode { get; set; }
        public string ProductType { get; set; }
        public string PartCategory { get; set; }
        public decimal? UnitConversion { get; set; }
        public string EndMark { get; set; }
    }

    public class Execs
    {
        public string Nama { get; set; }
    }

    public class SpMstItemModNew
    {
        public string PartNo { get; set; }
        public string InterChangeCode { get; set; }
    }
}