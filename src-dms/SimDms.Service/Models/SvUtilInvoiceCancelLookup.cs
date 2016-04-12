using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class SvUtilInvoiceCancelLookup
    {
        public string ProductType { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceStatus { get; set; }
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string JobType { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string Remarks { get; set; }
        public string Customer { get; set; }
        public string CustomerBill { get; set; }
        public string ServiceBookNo { get; set; }
        public decimal? Odometer { get; set; }
    }
}