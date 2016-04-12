using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class LiveStockPartDetail
    {
        public string Area { get; set; }
        public string Dealer { get; set; }
        public string Outlet { get; set; }
        public string QtyAvail { get; set; }
    }

    public class FormMonitoring
    {
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string OutletCode { get; set; }
        public string OutletName { get; set; } 
        public string UserName { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}