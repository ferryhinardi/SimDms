using System;

namespace SimDms.DataWarehouse.Models
{
    public class UtilDealerInfo
    {
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string ProductType { get; set; }
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
        public DateTime? SalesDate { get; set; }
        public DateTime? ServiceDate { get; set; }
        public DateTime? SparepartDate { get; set; }
        public DateTime? ArDate { get; set; }
        public DateTime? ApDate { get; set; }
        public DateTime? GlDate { get; set; }
    }

    public class CsDashSummary
    {
        public string RemCode { get; set; }
        public DateTime? RemDate { get; set; }
        public int? RemValue { get; set; }
        public string ControlLink { get; set; }
    }

}