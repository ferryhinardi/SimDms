using System;

namespace SimDms.DataWarehouse.Models
{
    public class DealerList
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyShortName { get; set; }
        public string ProductType { get; set; }
        public string GroupNo { get; set; }
    }

    public class SrvOutletList
    {
        public string OutletArea { get; set; }
        public string OutletAbbreviation { get; set; }
    }

    public class SrvArea
    {
        public int GroupNo { get; set; } 
        public string Area { get; set; } 
    }

    public class getAreaDealerOutlet  
    {
        public string Area { get; set; }
        public string Dealer { get; set; }
        public string Showroom { get; set; }
    }

    public class MappingSrvGn
    {
        public string GroupNoGn { get; set; }
        public string GroupAreaGn { get; set; }
        public string CompanyCodeGn { get; set; }
        public string CompanyNameGn { get; set; }
        public string BranchCodeGn { get; set; }
        public string BranchNameGn { get; set; }
        public string GroupNoSrv { get; set; }
        public string GroupAreaSrv { get; set; }
        public string CompanyCodeSrv { get; set; }
        public string CompanyNameSrv { get; set; }
        public string BranchCodeSrv { get; set; }
        public string BranchNameSrv { get; set; }
    }
}