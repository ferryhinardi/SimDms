using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("gnMstLookUpDtl")]
    public class LookUpDetail
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
        public decimal SeqNo { get; set; }
        public string ParaValue { get; set; }
        public string LookUpValueName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}