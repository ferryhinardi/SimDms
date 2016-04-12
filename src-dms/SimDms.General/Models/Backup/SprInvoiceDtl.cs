using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    public class SprInvoiceDtl
    {
        public string PartNo  { get; set; }
        public string PartNoOriginal { get; set; }
        public string DocNo { get; set; }
        public decimal QtyBill { get; set; }
    }
}
