using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omMstModelColour")]
    public class MstModelColour
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ColourCode { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }


    [Table("omMstModelColourView")]
    public class omMstModelColourView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ColourCode { get; set; }
        public string RefferenceDesc1 { get; set; }
        public string Remark { get; set; }
        public bool? Status { get; set; }
    }

    public class ColourCodeLookup
    {
        public string RefferenceCode { get; set; }
        public string RefferenceDesc1 { get; set; }

    }

    public class ColourLookup
    {
        public string ColourCodeNew { get; set; }
        public string ColourNew { get; set; }
        public string Remark { get; set; } 
    }

    public class omMstModelColourBrowse
    {
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
        public string ColourCode { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string SalesModelCodeTo { get; set; }
        public string ColourCodeTo { get; set; }
    }
}