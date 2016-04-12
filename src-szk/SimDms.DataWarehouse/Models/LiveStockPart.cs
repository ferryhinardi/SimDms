using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class LiveStockPart
    {
        public long No { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
    }

    public class ModelLiveStockPart
    {
        public string PartNo { get; set; }
        public string Model { set; get; }
    }

    public class LiveStockSales
    {
        public string Type { get; set; }
        public string Variant { get; set; }
        public string Transmission { get; set; }
        public string Colour { get; set; }
        public int Qty { get; set; }
        public string Status { get; set; }
        public bool IsVisible { get; set; }
    }

    public class LiveStockDealerSales
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string BranchCode { get; set; }
        public string Type { get; set; }
        public string Variant { get; set; }
        public string Transmission { get; set; }
        public string Colour { get; set; }
        public decimal Qty { get; set; }
        public bool IsVisible { get; set; }
    }
}