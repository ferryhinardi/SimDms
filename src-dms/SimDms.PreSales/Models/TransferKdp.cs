using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.Models
{
    public class TransferKdp
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public string TeamLeader { get; set; }
        public int? inquiryCount { get; set; }
    }

    public class KdpDtl
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string KDPQty { get; set; }
    }
}