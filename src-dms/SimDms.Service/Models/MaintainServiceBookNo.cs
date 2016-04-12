using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class MaintainServiceBookNo
    {
        public string BranchCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string InvoiceNo { get; set; }
        public string ServiceBookNoOld { get; set; }
        public string ServiceBookNoNew { get; set; }
        public string EngineCodeOld { get; set; }
        public string EngineCodeNew { get; set; }
        public decimal EngineNoOld { get; set; }
        public decimal EngineNoNew { get; set; }
    }

    public class MaintainServiceBookNoSave
    {
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string NewServiceBookNo { get; set; }
        public string Flag { get; set; }
    }
}