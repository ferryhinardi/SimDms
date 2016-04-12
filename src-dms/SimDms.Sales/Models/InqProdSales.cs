using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    public class InqProdSales
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
	public string SalesType { get; set; }
	public string InvoiceNo { get; set; }
	public DateTime? InvoiceDate { get; set; }
	public string SONo { get; set; }
	public string SalesModelCode { get; set; }
	public string SalesModelYear { get; set; }
	public string SalesModelDesc { get; set; }
	public string FakturPolisiNo { get; set; }
	public DateTime? FakturPolisiDate { get; set; }
	public string FakturPolisiDesc { get; set; }
	public string MarketModel { get; set; }
	public string ColourCode { get; set; }
	public string ColourName { get; set; }
	public string GroupMarketModel { get; set; }
	public string ColumnMarketModel { get; set; }
	public DateTime? JoinDate { get; set; }
	public DateTime? ResignDate { get; set; }
	public DateTime? GradeDate { get; set; }
	public string Grade	 { get; set; }
	public string ChassisCode { get; set; }
    public string ChassisNo { get; set; }
	public string EngineCode { get; set; }
    public string EngineNo { get; set; }
    public int COGS { get; set; }
    public int BeforeDiscDPP { get; set; }
    public int DiscExcludePPn { get; set; }
    public int DiscIncludePPn { get; set; }
    public int AfterDiscDPP { get; set; }
    public int AfterDiscPPn { get; set; }
    public int AfterDiscPPnBM { get; set; }
    public int AfterDiscTotal { get; set; }
    public int PPnBMPaid { get; set; }
    public int OthersDPP { get; set; }
    public int OthersPPn { get; set; }
    public int ShipAmt { get; set; }
    public int DepositAmt { get; set; }
    public int OthersAmt { get; set; }
    }

    public class InqSales
    {
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
        public string SalesType { get; set; }
        public string SalesModelCode { get; set; }
        public Decimal SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string FakturPolisiDesc { get; set; }
        public string GroupMarketModel { get; set; }
        public string ColumnMarketModel { get; set; }
        public string Grade { get; set; }
        public string ModelCatagory { get; set; }
        public string MarketModel { get; set; }
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
        public Decimal Year { get; set; }
        public Decimal Month { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string SONo { get; set; }
        public string InvoiceNo { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime FakturPolisiDate { get; set; }
        public Decimal COGS { get; set; }
        public Decimal DPP { get; set; }
        public Decimal DPPAccs { get; set; }
        public Decimal COGSAccs { get; set; }
        public Decimal Margin { get; set; }
    }
}