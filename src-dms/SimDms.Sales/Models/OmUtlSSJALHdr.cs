using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("omUtlSSJALHdr")]
    public class OmUtlSSJALHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string RcvDealerCode { get; set; }
        [Key]
        [Column(Order = 5)]
        public string BatchNo { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    public class OmSelectReffSJTrueViewLookup
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BatchNo { get; set; }
        public string SJNo { get; set; }
        public DateTime	SJDate { get; set; }
        public string SKPNo { get; set; }
        public string DONo { get; set; }
        public string DealerCode { get; set; }
        public string ShipTo { get; set; }
        public DateTime	DODate { get; set; }
        public string PONo { get; set; }
        public string DealerName { get; set; }
        public string SupplierCode { get; set; }
        public string FlagRevisi { get; set; }
    }

    public class OmSelectReffSJBookingViewLookup
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BatchNo { get; set; }
        public string DONo { get; set; }
        public DateTime	DODate { get; set; }
        public string SJNo { get; set; }
        public DateTime	SJDate { get; set; }
        public string SKPNo { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string PONo { get; set; }
        public string ShipTo { get; set; }
        public string SupplierCode { get; set; }
    }
}