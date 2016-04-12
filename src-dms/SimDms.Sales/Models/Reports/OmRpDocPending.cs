using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models.Reports
{
    public class OmRpDocPending
    {
        public string Nomor { get; set; }
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string RefNo { get; set; }
        public DateTime RefDate { get; set; }
        public string TransType { get; set; }
        public string StatusClosed { get; set; }

    }
}