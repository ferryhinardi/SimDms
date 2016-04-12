using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    [Table("SvTrnSrvVOR")]
    public class VOR
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public long ServiceNo { get; set; }
        public string JobOrderNo { get; set; }
        public string JobDelayCode { get; set; }
        public string JobReasonDesc { get; set; }
        public DateTime? ClosedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsSparepart { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    [Table("SvTrnSrvVORDtl")]
    public class VORDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public long ServiceNo { get; set; }
        [Key]
        [Column(Order = 4)]
        public string POSNo { get; set; }
        [Key]
        [Column(Order = 5)]
        public string PartNo { get; set; }
        public decimal? PartQty { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}