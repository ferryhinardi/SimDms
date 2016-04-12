using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models
{
    [Table("GnMstCoProfile")]
    public class GnMstCoProfile
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyGovName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string ZipCode { get; set; }
        public bool? isPKP { get; set; }
        public string SKPNo { get; set; }
        public DateTime? SKPDate { get; set; }
        public string NPWPNo { get; set; }
        public DateTime? NPWPDate { get; set; }
        public string CityCode { get; set; }
        public string AreaCode { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string OwnershipName { get; set; }
        public string TaxTransCode { get; set; }
        public string TaxCabCode { get; set; }
        public bool? isFPJCentralized { get; set; }
        public string ProductType { get; set; }
        public bool? isLinkToService { get; set; }
        public bool? isLinkToSpare { get; set; }
        public bool? isLinkToSales { get; set; }
        public bool? isLinkToFinance { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    
    }
}