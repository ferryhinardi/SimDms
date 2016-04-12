using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models
{
    [Table("HrLookupMapping")]
    public class HrLookupMapping
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CodeID { get; set; }
        public string CodeDescription { get; set; }
    }

    [Table("HrLookupView")]
    public class HrLookupView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CodeID { get; set; }
        [Key]
        [Column(Order = 3)]
        public string LookUpValue { get; set; }
        public string CodeDescription { get; set; }
        public string LookUpValueName { get; set; }
        public string ParaValue { get; set; }
        public decimal SeqNo { get; set; }
    }
}