using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    
        [Table("gnMstCompanyMapping")]
        public class CompanyMapping
        {
            [Key]
            [Column(Order = 1)]
            public string CompanyCode { get; set; }
            [Key]
            [Column(Order = 2)]
            public string BranchCode { get; set; }
            public bool MappingType { get; set; }
            public string CompanyMD { get; set; }
            public string BranchMD { get; set; }
            public string UnitBranchMD { get; set; }
            public string BranchNameMD { get; set; } 
            public string WarehouseMD { get; set; }
            public string DbMD { get; set; }
        }
    
}