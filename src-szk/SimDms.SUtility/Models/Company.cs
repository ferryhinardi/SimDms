using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.SUtility.Models
{
    [Table("gnMstOrganizationHdr")]
    public class Company
    {
        [Key]
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
    }
}