using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("pmGroupTypeSeq")]
    public class GroupType
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string GroupCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TypeCode { get; set; }
        public decimal GroupCodeSeq { get; set; }
        public decimal TypeCodeSeq { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UsageFlag { get; set; }
    }
}