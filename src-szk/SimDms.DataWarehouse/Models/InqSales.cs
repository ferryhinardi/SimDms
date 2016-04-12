using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClosedXML;

namespace SimDms.DataWarehouse.Models
{

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
        public Decimal? SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string FakturPolisiDesc { get; set; }
        public string GroupMarketModel { get; set; }
        public string ColumnMarketModel { get; set; }
        public string Grade  { get; set; }
        public string ModelCatagory { get; set; }
        public string MarketModel { get; set; }
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
        public Decimal? Year { get; set; }
        public Decimal? Month { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string SONo { get; set; }
        public string InvoiceNo { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public Decimal? COGS { get; set; }
        public Decimal? DPP { get; set; }
        public Decimal? DPPAccs { get; set; }
        public Decimal? COGSAccs { get; set; }
        public Decimal? Margin { get; set; }
    }

    public class omInqSalesLkpEmployee
    {
        public string Area { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchHeadID { get; set; }
        public string BranchHeadName { get; set; }
        public string SalesHeadID { get; set; }
        public string SalesHeadName { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
    }


}