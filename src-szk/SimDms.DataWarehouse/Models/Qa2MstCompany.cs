using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("Qa2MstCompany")]
    public class Qa2MstCompany
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }

        public bool IsNonActive { get; set; }
    }

    
}