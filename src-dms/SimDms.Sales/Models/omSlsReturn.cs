using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    public class omSlsReturn
    {
    }

    public class omSlsRtrnBrowse
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ReturnNo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string CustomerCode { get; set; }
        public string FakturPajakNo { get; set; }
        public DateTime? FakturPajakDate { get; set; }
        public string WareHouseCode { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string StatusDsc { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string WarehouseName { get; set; }
        public string SalesType { get; set; }
        public string SalesTypeDsc { get; set; }
    }

    
    public class omSlsInvcLkp
    {
        public string CompanyCode {get;set;}
        public string BranchCode {get;set;}
        public string InvoiceNo {get;set;}
        public DateTime? InvoiceDate {get;set;}
        public string SONo {get;set;}
        public string CustomerCode {get;set;}
        public string BillTo {get;set;}
        public string FakturPajakNo {get;set;}
        public DateTime? FakturPajakDate {get;set;}
        public DateTime? DueDate {get;set;}
        public bool isStandard {get;set;}
        public string Remark {get;set;}
        public string Status {get;set;}
        public string CustomerName {get;set;}
        public string Address { get; set; }
        public string SalesType {get;set;}
        public string SalesTypeDsc { get; set; }
    }

    public class omSlsModelYrLkp
    {
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string ChassisCode { get; set; }
        public string BPKNo { get; set; }
        public decimal? BeforeDiscDPP { get; set; }
        public decimal? AfterDiscDPP { get; set; }
        public decimal? AfterDiscPPn { get; set; }
        public decimal? AfterDiscPPnBM { get; set; }
        public decimal? OthersDPP { get; set; }
        public decimal? OthersPPn { get; set; }
        public decimal? DiscExcludePPn { get; set; }
    }

    public class omSlsReturGridModel
    {
        public string SalesModelCode { get; set; }
        public decimal SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string BPKNo { get; set; }
        public string Remark { get; set; }
        public decimal BeforeDiscDPP { get; set; }
        public decimal AfterDiscDPP { get; set; }
        public decimal DiscExcludePPn { get; set; }
        public decimal AfterDiscPPn { get; set; }
        public decimal AfterDiscPPnBM { get; set; }
        public decimal OthersDPP { get; set; }
        public decimal OthersPPn { get; set; }
    }

}
