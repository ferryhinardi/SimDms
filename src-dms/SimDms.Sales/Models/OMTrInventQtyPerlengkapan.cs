using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("OMTrInventQtyPerlengkapan")]
    public class OMTrInventQtyPerlengkapan
    {
    [Key]
    [Column(Order = 1)]
    public string CompanyCode { get; set; }
    [Key]
    [Column(Order = 2)]
    public string BranchCode { get; set; }
    [Key]
    [Column(Order = 3)]
    public decimal Year { get; set; }
    [Key]
    [Column(Order = 4)]
    public decimal Month { get; set; }
    [Key]
    [Column(Order = 5)]
    public string PerlengkapanCode { get; set; }
    public decimal? QuantityBeginning { get; set; }
    public decimal? QuantityIn { get; set; }
    public decimal? QuantityOut { get; set; }
    public decimal? QuantityEnding { get; set; }
    public string Remark { get; set; }
    public string Status { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string LastUpdateBy { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public bool? IsLocked { get; set; }
    public string LockedBy { get; set; }
    public DateTime? LockedDate { get; set; }
    }

    public class InquiryInventQtyPerlengkapanView
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string PerlengkapanCode { get; set; }
        public string PerlengkapanName { get; set; }
        public decimal? QuantityBeginning { get; set; }
        public decimal? QuantityIn { get; set; }
        public decimal? QuantityOut { get; set; }
        public decimal? QuantityEnding { get; set; }
        public string Status { get; set; }
    }

    public class InquiryPerlengkapanLookup
    {
        public string PerlengkapanCode { get; set; }
        public string PerlengkapanName { get; set; }
    }
}