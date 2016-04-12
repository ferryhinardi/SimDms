using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omMstModelPerlengkapan")]
    public class MstModelPerlengkapan
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SalesModelCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PerlengkapanCode { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool? isLocked { get; set; }
        public string LockingBy { get; set; }
        public DateTime? LockingDate { get; set; }
    }

    public class PerlengkapanCodeLookup
    {
        public string PerlengkapanCode { get; set; }
        public string PerlengkapanName { get; set; }
        public string PerlengkapanCodeTo { get; set; }
    }

    [Table("omMstModelPerlengkapanView")]
    public class MstPerlengkapanView
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SalesModelCode { get; set; }
        public string PerlengkapanCode { get; set; }
        public string PerlengkapanName { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }
        public bool Status { get; set; }
    }

    public class omMstModelPerlengkapanBrowse
    {
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
    }
}