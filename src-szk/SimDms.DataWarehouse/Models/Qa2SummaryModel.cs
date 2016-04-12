using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class Qa2SummaryModel
    {
        public string Style { get; set; }
        public string Num { get; set; }
        public string Question { get; set; }
        public string Value { get; set; }
        public string Total { get; set; }
        public string GrandTotal { get; set; }
    }
}