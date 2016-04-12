using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{
    class omSalesInvoice
    {
    }

    public class omSlsInvBrowse
    {
        public string BranchCode { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string SONo { get; set; }
        public string SKPKNo { get; set; }
        public string RefferenceNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string BillTo { get; set; }
        public string BillName { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string StatusDsc { get; set; }
        public string SalesType { get; set; }
        public string SalesTypeDsc { get; set; }
        public string Remark { get; set; }
    }

    public class omSlsInvLkpSO
    {
        public string SONo { get; set; }
        public decimal? QtyBPK { get; set; }
        public decimal? QtyInvoice { get; set; }        
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string BillTo { get; set; }
        public string BillName { get; set; }
        public string Address { get; set; }
        public string SalesType { get; set; }
        public string SalesTypeDsc { get; set; }
        public decimal? TOPDays { get; set; }
        public string SKPKNo { get; set; }
        public string RefferenceNo { get; set; }
    }

    public class omSlsInvLkpBillTo
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
    }

    public class omSlsInvLkpBPK
    {
        public string BPKNo { get; set; }
        public DateTime? BPKDate { get; set; }
        public string DONo { get; set; }
        public string SONo { get; set; }
    }


    public class omSlsInvSlctMdlVldt
    {        
        public string SalesModelCode { get; set; }
        public string SalesModelDesc { get; set; }
    }
    public class omSlsInvSlctMdlYrVldt
    {       
        public decimal SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }
    }


    public class omSlsInvSlctFrTblInvAccSeq
    {
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string SupplySlipNo { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? DPP { get; set; }
        public decimal? PPn { get; set; }
        public decimal? Total { get; set; }
    }







}