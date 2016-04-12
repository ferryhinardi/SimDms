using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.DataWarehouse.Models
{
    [Table("vw_listdealer")]
    public class ListDealerActive
    {
        [Key]
        public string value { get; set; }
        public string text { get; set; }
        public string ProductType { get; set; }
    }

    [Table("vw_lastTransDate")]
    public class LastTransDateInfo
    {
        [Key]
        [Column(Order = 1)]
        public string DealerCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        public string DealerName { get; set; }
        public string ShortName { get; set; }
        public string DealerAbbr { get; set; }
        public string BranchAbbr { get; set; }
        public string ProductType { get; set; }
        public string Version { get; set; }
        public DateTime? LastSalesDate { get; set; }
        public DateTime? LastSpareDate { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public DateTime? LastAPDate { get; set; }
        public DateTime? LastARDate { get; set; }
        public DateTime? LastGLDate { get; set; }
        public DateTime? LastupdateDate { get; set; }
        public DateTime? GoLiveDate { get; set; }
    }

    [Table("CompaniesGroupMappingView")]
    public class CompaniesGroupMappingView
    {
        [Key]
        [Column(Order = 1)]
        public string GroupNo { get; set; }
        [Key]
        [Column(Order = 2)]
        public string AreaDealer { get; set; }
        public string DealerCode { get; set; }
        public string CompanyName { get; set; }
    }

    [Table("CompaniesGroupMappingView2")]
    public class CompaniesGroupMappingView2
    {
        [Key]
        [Column(Order = 1)]
        public string GroupNo { get; set; }
        [Key]
        [Column(Order = 2)]
        public string AreaDealer { get; set; }
        public string DealerCode { get; set; }
        public string CompanyName { get; set; }
        public string ProductType { get; set; }
    }
}