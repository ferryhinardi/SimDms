using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("gnMstOrganizationHdr")]
    public class OrganizationHdr
    {
        [Key]
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAccNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    [Table("gnMstOrganizationDtl")]
    public class OrganizationDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string BranchAccNo { get; set; }
        public decimal? SeqNo { get; set; }
        public string BranchName { get; set; }
        public bool IsBranch { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}