using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class HrInqMutation
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public int? Muta01 { get; set; }
        public int? Muta02 { get; set; }
        public int? Muta03 { get; set; }
        public int? Muta04 { get; set; }
        public int? Muta05 { get; set; }
        public int? Muta06 { get; set; }
    }
}