using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class QaMonitoringCust
    {
        public string Area { get; set; }
        public string CompanyName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string SalesModel { get; set; }
        public string CustomerName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string ContactNo { get; set; }
        public string Payment { get; set; }
        public string FakturPajakDate { get; set; }
        public string FakturPolisiDate { get; set; }
        public string TransactionDate { get; set; }
        public string StatusKonsumenDescI { get; set; }
    }
}