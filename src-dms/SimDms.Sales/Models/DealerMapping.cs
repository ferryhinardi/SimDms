using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    [Table("gnMstDealerMapping")]
    public class DealerMapping
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

    public class Outlet
    {
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
    }

    public class GetInquiryBtn
    {
        public string GroupNo { get; set; }
        public string Area { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
    }

    public class GetInquirySalesLookUpBtn
    {
        public string GroupNo { get; set; }
		public string Area { get; set; }
		public string CompanyCode { get; set; }
		public string CompanyName { get; set; }
		public string BranchCode { get; set; }
		public string BranchName { get; set; }
		public string BranchHeadID { get; set; }
		public string BranchHeadName { get; set; }
		public string SalesHeadID { get; set; }
		public string SalesHeadName { get; set; }
		public string SalesCoordinatorID { get; set; }
		public string SalesCoordinatorName { get; set; }
		public string SalesmanID { get; set; }
		public string SalesmanName { get; set; }
		public string ModelCatagory { get; set; }
		public string SalesModelDesc { get; set; }
		public string MarketModel { get; set; }
		public string ColourName { get; set; }
		public string Year  { get; set; }
		public string IMonth { get; set; }
		public int SoldTotal { get; set; }
    }
}