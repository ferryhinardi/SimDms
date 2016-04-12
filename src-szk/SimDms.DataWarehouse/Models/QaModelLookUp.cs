using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class QaModelLookUp
    {
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string SalesModelCode { get; set; }
        public string ErtigaOrWagonR { get; set; }
        public DateTime? SODate { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
}