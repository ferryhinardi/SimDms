using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("omHstInquirySales")]
    public class omHstInquirySales
    {
        public decimal Year { get; set; }
        public decimal Month { get; set; }
        [Key]
        [Column(Order = 0)]
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string Area { get; set; }
        public string BranchHeadID { get; set; }
        public string BranchHeadName { get; set; }
        public string SalesHeadID { get; set; }
        public string SalesHeadName { get; set; }
        public string SalesCoordinatorID { get; set; }
        public string SalesCoordinatorName { get; set; }
        public string SalesmanID { get; set; }
        public string SalesmanName { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? ResignDate { get; set; }
        public DateTime? GradeDate { get; set; }
        public string Grade { get; set; }
        public string ModelCatagory { get; set; }
        public string SalesType { get; set; }

        [Key]
        [Column(Order = 1)]
        public string SoNo { get; set; }
        public DateTime? SODate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string FakturPolisiDesc { get; set; }
        public string MarketModel { get; set; }
        public string GroupMarketModel { get; set; }
        public string ColumnMarketModel { get; set; }

        [Key]
        [Column(Order = 2)]
        public string ChassisCode { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
        public decimal? COGS { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public decimal? DiscExcludePPn { get; set; }
        public decimal? DiscIncludePPn { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscPPnBM { get; set; }
        public decimal? AfterDiscTotal { get; set; }
        public decimal? PPnBMPaid { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPn { get; set; }
        public decimal? ShipAmt { get; set; }
        public decimal? DepositAmt { get; set; }
        public decimal? OthersAmt { get; set; }
        public bool? Status { get; set; }
        public bool? DCSStatus { get; set; }
        public DateTime? DCSDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string CategoryCode { get; set; }
        public DateTime? SuzukiDODate { get; set; }
        public DateTime? SuzukiFPolDate { get; set; }
    }
}