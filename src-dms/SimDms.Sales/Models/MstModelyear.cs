using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace SimDms.Sales.Models
{
     [Table("omMstModelYear")]
    public class MstModelYear
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string ChassisCode { get; set; }
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

     public class MstModelYearView
     {
         public string SalesModelCode { get; set; }
         public decimal SalesModelYear { get; set; }
         public string SalesModelDesc { get; set; }
         public string ChassisCode { get; set; }
         public string Remark { get; set; }
         public bool? Status { get; set; }
     }

     public class MstModelYearBrowse
     {
         public string SalesModelCode { get; set; }
         public decimal SalesModelYear { get; set; }
         public string SalesModelDesc { get; set; }
         public string ChassisCode { get; set; }
         public string Remark { get; set; }
         public string Status { get; set; }
     }

     public class MstModelYearDesc
     {     
         public decimal SalesModelYear { get; set; }
         public string SalesModelDesc { get; set; }         
     }
}