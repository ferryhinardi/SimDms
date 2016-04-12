using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    class EntryReturnService
    {
    }

    public class InvoiceCancelLookup
    {
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ReturnNo { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class PartInvoiceReturn
    {
        public long? No { get; set; }
        public string ReturnNo { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? QtyBill { get; set; }
        public decimal? QtyReturn { get; set; }
    }
}
