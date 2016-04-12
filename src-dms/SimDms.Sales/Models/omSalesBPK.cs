using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    public class omSalesBPK
    {
    }
    public class omSlsBPKBrowse
    {
        public string BPKNo { get; set; }
        public DateTime? BPKDate { get; set; }
        public string DONo { get; set; }
        public string SONo { get; set; }
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
        public string Status { get; set; }
        public string StatusDsc { get; set; }
        public string SalesType { get; set; }
        public string TypeSales { get; set; }
        public string Remark { get; set; }
    }

    public class omSlsBPKDtl
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
        public string StatusPDI { get; set; }
        public int BPKSeq { get; set; }
    }

    public class omSlsBPkLkpDO
    {
        public string DONo { get; set; }
        public DateTime? DODate { get; set; }
        public string SONo { get; set; }
        public string SKPKNo { get; set; }
        public string RefferenceNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ShipTo { get; set; }
        public string ShipName { get; set; }
        public string WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public string Expedition { get; set; }
        public string ExpeditionName { get; set; }
        public string SalesType { get; set; }
        public string SalesTypeDsc { get; set; }
    }

    public class omSLsBPKLkpChasisNo
    {
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColourCode { get; set; }
        public string RefferenceDesc1 { get; set; }       
    }

    public class omSlsBPKMstlkpDtl
    {        
        public string CodeID { get; set; }
        public string LookUpValue { get; set; }        
        public string ParaValue { get; set; }
        public string LookUpValueName { get; set; }        
    }

    public class BPKLookUp
    {
        public string BPKNo { get; set; }
        public DateTime BPKDate { get; set; }
    }
}