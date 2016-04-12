using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class InputApproval
    {
        public Boolean? IsSelected { get; set; }
        public Int64? No { get; set; }
        public String BranchCode { get; set; }
        public Int64? ServiceNo { get; set; }
        public String JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public String ServiceBookNo { get; set; }
        public Decimal? ChassisNo { get; set; }
        public String BasicModel { get; set; }
        public String JobType { get; set; }
        public Decimal? TotalApprove { get; set; }
    }

    public class DataInputApproval
    {
        public string BranchCode { get; set; }
        public Int64? ServiceNo { get; set; }
    }
}