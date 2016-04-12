using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
     [Table("omMstSalesTarget")]
    public class omMstSalesTarget
    {
        [Key]
        [Column(Order = 1)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string OutletCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal Year { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal Month { get; set; }
        [Key]
        [Column(Order = 5)]
        public string SalesmanID { get; set; }
        [Key]
        [Column(Order = 6)]
        public string MarketModel { get; set; }
        public decimal? TargetUnit { get; set; }
        public bool? isActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

     public class DealerCodeLookup
     {
         public string CompanyCode { get; set; }
         public string CompanyName { get; set; }
     }

     public class OutLetLookup
     {
         public string BranchCode { get; set; }
         public string BranchName { get; set; }
     }

     public class MarketModelLookup
     {
         public string LookUpValue { get; set; }
         public string LookUpValueName { get; set; }
     }

     public class SalesTargetBrowse
     {
         public string DealerCode { get; set; }
         public string OutletCode { get; set; }
         public decimal Year { get; set; }
         public decimal Month { get; set; }
         public string SalesmanID { get; set; }
         public string SalesmanName { get; set; }
         public string EmployeeName { get; set; }
         public string MarketModel { get; set; }
         public decimal? TargetUnit { get; set; }
         public bool? isActive { get; set; }
     }
    
}
