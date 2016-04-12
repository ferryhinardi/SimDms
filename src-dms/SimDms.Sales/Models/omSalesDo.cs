using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    class omSalesDo
    {
        
    }

    
    public class omSlsDOBrowse
    {
        public string DONo { get; set; }
        public DateTime? DODate { get; set; }
        public string SKPKNo { get; set; }
        public string RefferenceNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShipTo { get; set; }
        public string ShipToDsc { get; set; }
        public string Address { get; set; }
        public string WareHouseCode { get; set; }
        public string WrhDsc { get; set; }
        public string Expedition { get; set; }
        public string ExpeditionDsc { get; set; }
        public string SONo { get; set; }
        public string Status { get; set; }
        public string StatusDsc { get; set; }
        public string TypeSales { get; set; }
        public string Remark { get; set; }
    }

    public class omSlsDoDtl
    {
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public string EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string RefferenceDesc1 { get; set; }
        public string Remark { get; set; }
        public int DOSeq { get; set; }
    }

    public class omSlsDOLkpSO
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SONo { get; set; }
        public DateTime SODate { get; set; }
        public string SKPKNo { get; set; }
        public string RefferenceNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string Address { get; set; }
        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public string TypeSales { get; set; }
    }


    public class omSlsDOLkpExpdtion
    {
        public string SupplierCode { get; set; }
        public string Suppliername { get; set; }
        public string Alamat { get; set; }
        public decimal? Diskon { get; set; }
        public string Status { get; set; }
        public string Profit { get; set; }
    }

    public class OmInquiryChassisDO
    {
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string ColourName { get; set; }
    }


    public class omSlsDOUpdateSOVin
    {
        
        public string CompanyCode { get; set; }
        
        public string BranchCode { get; set; }
        
        public string DONo { get; set; }
        
        public int DOSeq { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string Remark { get; set; }
        public string StatusBPK { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string SONo { get; set; }
        public string ServiceBookNo { get; set; }
        public string KeyNo {get;set;}
    }



}


