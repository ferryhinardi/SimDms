using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OmMstCompanyAccount")]
    public class OmMstCompanyAccount
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCodeTo { get; set; }
        public string CompanyCodeToDesc { get; set; }
        public string BranchCodeTo { get; set; }
        public string BranchCodeToDesc { get; set; }
        public string WarehouseCodeTo { get; set; }
        public string WarehouseCodeToDesc { get; set; }
        public string InterCompanyAccNoTo { get; set; }
        public string UrlAddress { get; set; }
        public bool isActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    public class CompanyAccountView
    {
        public string CompanyCode { get; set; }
        public string CompanyCodeTo { get; set; }
        public string CompanyCodeToDesc { get; set; }
        public string BranchCodeTo { get; set; }
        public string BranchCodeToDesc { get; set; }
        public string WarehouseCodeTo { get; set; }
        public string WarehouseCodeToDesc { get; set; }
        public string InterCompanyAccNoTo { get; set; }
        public string UrlAddress { get; set; }
        public bool? isActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Status { get; set; } 
    }
    
}