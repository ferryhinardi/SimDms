using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimDms.Common
{
    [Table("GnMstSegmentAcc")]
    public class GnMstSegmentAcc
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string TipeSegAcc { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SegAccNo { get; set; }
        public string Description { get; set; }
        public string Parent { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class SegmentAcc 
    {
        public string SegAccNo { get; set; }
        public string Description { get; set; }
        public string SegAccNo1 { get; set; } 
        public string Description1 { get; set; } 
    }
}
