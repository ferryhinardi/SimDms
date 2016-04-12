using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models
{
    [Table("GnMstDealerMapping")]
    public class GnMstDealerMapping
    {
        [Key]
        [Column(Order = 1)]
        public long SeqNo { get; set; }
        public int? GroupNo { get; set; }
        public string Area { get; set; }
        public string DealerName { get; set; }
        public string DealerAbbreviation { get; set; }
        public string DealerCode { get; set; }
        public bool? isActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}