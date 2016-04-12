using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    public class omSalesPerlengkapanOut
    {
    }


    public class omSalesPerlengkapanOutDtl
    {

        public string PerlengkapanNo { get; set; }
        public string PerlengkapanName { get; set; }
        public string SalesModelCode { get; set; }
        public string PerlengkapanCode { get; set; }
        public decimal? QuantityStd { get; set; }
        public decimal? Quantity { get; set; }
        public string Remark { get; set; }

    }

    public class omSalesPerlengkapanOutBrwDocBPK
    {
        public string BPKNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Customer { get; set; }
        public string Address { get; set; }
    }

    public class omSalesPerlengkapanOutBrwDocTransfer
    {
        public string TransferOutNo { get; set; }
        public DateTime? TransferOutDate { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string BranchCodeFrom { get; set; }
        public string BranchNameFrom { get; set; }
        public string BranchCodeTo { get; set; }
        public string BranchNameTo { get; set; }
        public string WareHouseCodeFrom { get; set; }
        public string WareHouseNameFrom { get; set; }
        public string WareHouseCodeTo { get; set; }
        public string WareHouseNameTo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string StatusDsc { get; set; }

    }

    public class omSlsPrlgkpnOutLkpMdlDtl
    {
        public string PerlengkapanCode { get; set; }
        public string PerlengkapanName { get; set; }
        public string Remark { get; set; }
        public decimal? Quantity { get; set; }
    }


}