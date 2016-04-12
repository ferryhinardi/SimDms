using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class Qa2RowDataSub
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string IsAdditionalMerkCode { get; set; }
        public string IsAdditionalMerkDescI { get; set; }
        public string IsAdditionalMerkDescE { get; set; }
        public string IsAdditionalMerkOthers { get; set; }
        public string IsAdditionalType { get; set; }
        public decimal IsAdditionalYear { get; set; }
    }
}