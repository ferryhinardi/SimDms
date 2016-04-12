using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("OmMstIndentMapping")]
    public class OmMstIndentMapping
    {
        [Key]
        [Column(Order = 1)]
        public string IndentTypeCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string IndentVariant { get; set; }
        public string GroupCode { get; set; }
        public string TypeCode { get; set; }
    }
}