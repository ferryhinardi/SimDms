using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Common.Models
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

    [Table("GnMstDealerOutletMapping")]
    public class GnMstDealerOutletMapping 
    {
        [Key]
        [Column(Order = 1)]
        public string DealerCode { get; set; }
        public string OutletCode { get; set; }
        public string OutletArea { get; set; }
        public string OutletAbbreviation { get; set; }
        public string OutletName { get; set; } 
        public bool? isActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class GetInquiryMapping
    {
        public string GroupNo { get; set; }
        public string Area { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
    }
}
