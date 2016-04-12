using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("PmExecSummaryViewDash")]
    public class PmExecSummaryView
    {
        [Key]
        [Column(Order = 1)]
        public string FieldName { get; set; }
        public string FieldDesc { get; set; }
        [Key]
        [Column(Order = 2)]
        public string FieldType { get; set; }
        public int? InqValue1 { get; set; }
        public int? InqValue2 { get; set; }
        public decimal? InqValue3 { get; set; }
        public int? InqValAll { get; set; }
        public int? SpkValue1 { get; set; }
        public int? SpkValue2 { get; set; }
        public decimal? SpkValue3 { get; set; }
        public int? Sequence { get; set; }
    }
}