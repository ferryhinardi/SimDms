using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("gnMstDealerOutletMapping")]
    public class DealerOutletMapping
    {
        [Key]
        [Column(Order = 1)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
        public string OutletArea { get; set; }
        public string OutletAbbreviation { get; set; }
        public bool? isActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

    }   
}