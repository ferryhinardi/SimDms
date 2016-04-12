using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class DetailRevenueRegSPK
    {
        public string CompanyCode { get; set; }
        public string DealerName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime JobOrderDate { get; set; }
        public string BasicModel { get; set; }
        public string PoliceRegNo { get; set; }
        public decimal Odometer { get; set; }
        public string JobType { get; set; }
        public string JobTypeDesc { get; set; }
        public string TaskPartNo { get; set; }
        public string TaskPartName { get; set; }
        public decimal DemandQty { get; set; }
        public decimal SupplyQty { get; set; }
        public decimal ReturnQty { get; set; }
        public string SupplySlipNo { get; set; }
        public string SSReturnNo { get; set; }
        public string SaName { get; set; }
        public string FmName { get; set; }
        public string ServiceStatusDesc { get; set; }
    }
}